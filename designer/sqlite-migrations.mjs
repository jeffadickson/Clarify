// Conservative SQLite schema-change planner.
// Only nullable scalar column additions are executable. All other changes are
// blocked for manual EF Core migration authoring and review. No SQL is executed.
import {analyzeDependencies,impactOfChange} from './dependencies.mjs';

const identifier=/^[A-Za-z_][A-Za-z0-9_]*$/;
const sqliteType={string:'TEXT',int:'INTEGER',long:'INTEGER',bool:'INTEGER',date:'TEXT'};
const storage=(model,entity)=>entity.database||model.defaultDatabase||'default';
const quote=name=>{
 if(!identifier.test(name))throw Error('Invalid SQL identifier: '+name);
 return '"'+name+'"';
};
const fingerprint=input=>{
 // Non-cryptographic deterministic change identifier, NOT an authorization token.
 const data=JSON.stringify(input);
 let h=2166136261;
 for(let i=0;i<data.length;i++){h=Math.imul(h^data.charCodeAt(i),16777619)>>>0}
 return h.toString(16).padStart(8,'0');
};
function same(a,b){return JSON.stringify(a)===JSON.stringify(b)}
export function planSqliteAdditiveMigration(before,after,database='sales'){
 const a=analyzeDependencies(before),b=analyzeDependencies(after);
 if(a.errors.length||b.errors.length)throw Error('Dictionary validation failed: '+[...a.errors,...b.errors].join('; '));
 const declared=(after.databases||[]).find(x=>x.id===database);
 if(!declared||declared.provider!=='sqlite')throw Error('This planner supports registered SQLite databases only');
 const changes=impactOfChange(before,after).changes;
 const blocked=[],operations=[];
 const beforeMap=new Map(before.entities.map(e=>[e.name,e]));
 const afterMap=new Map(after.entities.map(e=>[e.name,e]));
 if(!same(before.relationships||[],after.relationships||[]))
  blocked.push('Relationship changes require reviewed EF Core migrations');
 for(const e of before.entities){
  const next=afterMap.get(e.name);
  if(!next){blocked.push('Removing entity '+e.name+' is destructive');continue}
  if(storage(before,e)!==storage(after,next))blocked.push('Moving entity '+e.name+' requires manual migration');
  const previous=new Map(e.fields.map(f=>[f.name,f]));
  for(const field of e.fields){
   const now=next.fields.find(f=>f.name===field.name);
   if(!now)blocked.push('Removing or renaming field '+e.name+'.'+field.name+' requires manual migration');
   else if(!same(field,now))blocked.push('Changing field '+e.name+'.'+field.name+' requires manual migration');
  }
  for(const f of next.fields){
   if(previous.has(f.name))continue;
   if(storage(after,next)!==database){blocked.push('Field added to another database: '+next.name+'.'+f.name);continue}
   if(f.required||f.primaryKey||f.default!==undefined||!sqliteType[f.type]||f.maxLength!==undefined){
    blocked.push('Unsupported new field '+next.name+'.'+f.name+': only nullable simple scalar columns');continue
   }
   const sql='ALTER TABLE '+quote(next.name)+' ADD COLUMN '+quote(f.name)+' '+sqliteType[f.type]+' NULL;';
   operations.push({kind:'add-nullable-column',table:next.name,column:f.name,type:sqliteType[f.type],sql});
  }
 }
 for(const e of after.entities)if(!beforeMap.has(e.name))blocked.push('Creating entity '+e.name+' requires an EF Core migration');
 // Refuse migrations on unrelated DBs and all unrecognized changes.
 if(!same(before.databases||[],after.databases||[])||before.defaultDatabase!==after.defaultDatabase)
  blocked.push('Database registry changed');
 if(changes.length && operations.length!==changes.filter(c=>c.kind==='add-field').length)
  blocked.push('Unrecognized dictionary change');
 const expectedTables=before.entities.filter(e=>storage(before,e)===database).map(e=>({
  name:e.name,columns:e.fields.map(f=>({
   name:f.name,type:sqliteType[f.type]||null,
   // The runtime verifies existing names and types, but nullability is
   // provider-specific (SQLite PK notnull flags vary) and not compared.
  }))
 }));
 if(expectedTables.some(t=>t.columns.some(c=>!c.type)))blocked.push('Unsupported existing field type in SQLite schema preflight');
 if(!operations.length)blocked.push('No supported changes to apply');
 const precondition={database,expectedTables};
 const planId='clarify-sqlite-'+fingerprint({precondition,operations});
 return {format:'clarify-sqlite-migration/v1',planId,database,provider:'sqlite',
  executable:blocked.length===0,blocked:[...new Set(blocked)],
  operations,precondition,
  review:{requiresApproval:true,backupRequired:true,destructive:false,
   notice:'Generated plan is not applied automatically. Confirm a verified backup and use explicit approval.'}};
}
