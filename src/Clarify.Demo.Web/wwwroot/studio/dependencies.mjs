// Dependency analysis for Clarify's semantic dictionary.
// Pure and deterministic: no database connections, SQL execution, or migrations.
// An implicit Id primary key is assumed only for legacy dictionary examples.
const key=(entity,field)=>entity+"."+field;
const relKey=r=>[r.from,r.foreignKey,r.to,r.principalKey||"Id"].join("|");
const store=(model,entity)=>entity.database||model.defaultDatabase||"default";
const allEntities=model=>new Map((model.entities||[]).map(e=>[e.name,e]));
export function analyzeDependencies(model){
 const entities=allEntities(model),nodes=[],edges=[],errors=[],warnings=[];
 const databases=new Set();
 const declared=new Map((model.databases||[]).map(d=>[d.id,d]));
 if(model.databases?.length){
  for(const d of model.databases){
   if(!/^[A-Za-z_][A-Za-z0-9_-]*$/.test(d.id||""))errors.push("Invalid database identifier");
   if(!["sqlite","postgresql","sqlserver"].includes(d.provider))errors.push("Unsupported database provider: "+d.id);
  }
  if(declared.size!==model.databases.length)errors.push("Duplicate database identifier");
 }
 for(const e of model.entities||[]){
  databases.add(store(model,e));
  if(declared.size && !declared.has(store(model,e)))errors.push("Unregistered database for "+e.name+": "+store(model,e));
  nodes.push({id:e.name,kind:"entity",database:store(model,e)});
  for(const f of e.fields||[]){
   nodes.push({id:key(e.name,f.name),kind:"field",type:f.type,database:store(model,e)});
   edges.push({from:e.name,to:key(e.name,f.name),kind:"contains"});
  }
 }
 for(const r of model.relationships||[]){
  const child=entities.get(r.from),parent=entities.get(r.to);
  if(!child||!parent){errors.push("Unknown relationship endpoint: "+r.from+" -> "+r.to);continue}
  const childField=(child.fields||[]).find(f=>f.name===r.foreignKey);
  const pkName=r.principalKey||"Id";
  const parentField=(parent.fields||[]).find(f=>f.name===pkName);
  if(!childField){errors.push("Missing foreign key "+key(r.from,r.foreignKey));continue}
  if(!parentField && pkName!=="Id"){errors.push("Missing principal key "+key(r.to,pkName));continue}
  if(parentField && childField.type!==parentField.type)
   errors.push("Foreign key type mismatch: "+key(r.from,r.foreignKey)+" -> "+key(r.to,pkName));
  if(r.onDelete==="set-null" && childField.required)
   errors.push("set-null requires nullable foreign key: "+key(r.from,r.foreignKey));
  const cross=store(model,child)!==store(model,parent);
  if(cross && r.kind!=="external")
   errors.push("Cross-database relationship must be kind external: "+relKey(r));
  if(!cross && r.kind==="external")
   warnings.push("External relationship within same database: "+relKey(r));
  if(cross && r.onDelete!=="restrict")
   errors.push("Cross-database cascade/set-null cannot be database-enforced: "+relKey(r));
  if(cross)warnings.push("Application-managed integrity, no native foreign key or atomic cross-database transaction: "+relKey(r));
  edges.push({from:key(r.to,pkName),to:key(r.from,r.foreignKey),kind:cross?"external-reference":"foreign-key",onDelete:r.onDelete});
  edges.push({from:r.to,to:r.from,kind:cross?"external-entity":"relationship",onDelete:r.onDelete});
 }
 // Cascading deletion cycles are rejected. Restrict edges do not cause deletion.
 const cascade=new Map();
 for(const r of model.relationships||[]){
  if(r.onDelete!=="cascade"||!entities.has(r.from)||!entities.has(r.to))continue;
  if(!cascade.has(r.to))cascade.set(r.to,[]);
  cascade.get(r.to).push(r.from);
 }
 const visiting=new Set(),visited=new Set();
 function visit(n){
  if(visiting.has(n)){errors.push("Cascade cycle involving "+n);return}
  if(visited.has(n))return;
  visiting.add(n);
  for(const m of cascade.get(n)||[])visit(m);
  visiting.delete(n);visited.add(n);
 }
 for(const n of cascade.keys())visit(n);
 return {nodes,edges,databases:[...databases].sort(),errors:[...new Set(errors)],warnings:[...new Set(warnings)]};
}
export function impactOfChange(before,after){
 const previous=analyzeDependencies(before),next=analyzeDependencies(after);
 const oldEntities=allEntities(before),newEntities=allEntities(after),changes=[];
 for(const [name,current] of newEntities){
  if(!oldEntities.has(name))changes.push({kind:"add-entity",target:name,risk:"manual-review"});
  else{
   const prior=new Set((oldEntities.get(name).fields||[]).map(f=>f.name));
   for(const field of current.fields||[]){
    if(!prior.has(field.name))changes.push({kind:"add-field",target:key(name,field.name),risk:"manual-review"});
   }
  }
 }
 for(const [name,old] of oldEntities){
  const current=newEntities.get(name);
  if(!current){changes.push({kind:"remove-entity",target:name,risk:"destructive"});continue}
  if(store(before,old)!==store(after,current))
   changes.push({kind:"move-database",target:name,risk:"manual-review"});
  const fields=new Map((current.fields||[]).map(f=>[f.name,f]));
  for(const f of old.fields||[]){
   const now=fields.get(f.name);
   if(!now)changes.push({kind:"remove-field",target:key(name,f.name),risk:"destructive"});
   else if(now.type!==f.type)changes.push({kind:"change-type",target:key(name,f.name),risk:"manual-review"});
   else if(Boolean(now.required)!==Boolean(f.required))changes.push({kind:"change-nullability",target:key(name,f.name),risk:"manual-review"});
  }
 }
 const nextRelations=new Set((after.relationships||[]).map(relKey));
 for(const r of before.relationships||[]){
  if(!nextRelations.has(relKey(r)))
   changes.push({kind:"remove-relationship",target:relKey(r),risk:"manual-review"});
 }
 const oldRelations=new Map((before.relationships||[]).map(r=>[relKey(r),r]));
 for(const r of after.relationships||[]){
  if(!oldRelations.has(relKey(r)))changes.push({kind:"add-relationship",target:relKey(r),risk:"manual-review"});
 }
 for(const r of after.relationships||[]){
  const old=oldRelations.get(relKey(r));
  if(old && (old.onDelete!==r.onDelete||old.kind!==r.kind))
   changes.push({kind:"change-relationship-policy",target:relKey(r),risk:"manual-review"});
 }
 const affected=changes.map(change=>{
  const affectedNodes=new Set([change.target]);
  // An entity or field change propagates to related entities and foreign keys.
  const queue=[change.target];
  while(queue.length){
   const id=queue.shift();
   for(const e of previous.edges){
    if(e.from!==id || !["foreign-key","external-reference","relationship","external-entity"].includes(e.kind))continue;
    if(!affectedNodes.has(e.to)){affectedNodes.add(e.to);queue.push(e.to)}
   }
  }
  return {...change,affected:[...affectedNodes].sort()};
 });
 return {changes:affected,errors:next.errors,warnings:next.warnings,
  requiresReview:affected.some(c=>c.risk!=="safe")||next.errors.length>0};
}
