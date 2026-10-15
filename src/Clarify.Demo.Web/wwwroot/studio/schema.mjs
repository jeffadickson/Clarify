// Clarify's early graphical data-dictionary model. Pure functions keep editing testable.
// Existing entity/field properties are preserved when editing; unknown metadata is not discarded.
const identifier=/^[A-Za-z_][A-Za-z0-9_]*$/;
export function validate(model){
 const errors=[];if(!model||!Array.isArray(model.entities))return ["Missing entities"];
 const entities=new Map();
 for(const e of model.entities){
  if(!identifier.test(e.name))errors.push("Invalid entity: "+e.name);
  if(entities.has(e.name))errors.push("Duplicate entity: "+e.name);
  entities.set(e.name,e);
  const names=new Set();
  if(!Array.isArray(e.fields)) {errors.push("Missing fields: "+e.name);continue}
  for(const f of e.fields){
   if(!identifier.test(f.name))errors.push("Invalid field: "+e.name+"."+f.name);
   if(names.has(f.name))errors.push("Duplicate field: "+e.name+"."+f.name);
   names.add(f.name);
   if(!["string","int","long","date","bool","decimal","money","length","mass","temperature","pressure"].includes(f.type))
    errors.push("Unsupported type: "+e.name+"."+f.name);
  }
 }
 for(const r of model.relationships||[]){
  const from=entities.get(r.from),to=entities.get(r.to);
  if(!from||!to){errors.push("Missing relationship entity");continue}
  if(!from.fields.some(f=>f.name===r.foreignKey))errors.push("Missing foreign key: "+r.from+"."+r.foreignKey);
  if(!["one-to-many","one-to-one"].includes(r.cardinality))errors.push("Invalid cardinality");
  if(!["restrict","cascade","set-null"].includes(r.onDelete))errors.push("Invalid deletion policy");
 }
 return errors;
}
export function addEntity(model,name){
 if(!identifier.test(name))throw Error("Entity name must be a valid identifier");
 const next=structuredClone(model);next.entities.push({name,fields:[{name:"Id",type:"long",required:true,primaryKey:true}]});
 const errors=validate(next);if(errors.length)throw Error(errors.join("; "));return next;
}
export function addField(model,entity,field){
 const next=structuredClone(model),e=next.entities.find(x=>x.name===entity);
 if(!e)throw Error("Unknown entity");
 e.fields.push(field);const errors=validate(next);if(errors.length)throw Error(errors.join("; "));return next;
}
export function addRelationship(model,from,to,foreignKey,onDelete="restrict"){
 const next=structuredClone(model),left=next.entities.find(x=>x.name===from),right=next.entities.find(x=>x.name===to);
 if(!left||!right)throw Error("Unknown entity");
 if(!left.fields.some(f=>f.name===foreignKey))left.fields.push({name:foreignKey,type:"long",required:true});
 next.relationships??=[];
 if(next.relationships.some(x=>x.from===from&&x.foreignKey===foreignKey))throw Error("Relationship already exists");
 next.relationships.push({from,to,foreignKey,cardinality:"one-to-many",onDelete});
 const errors=validate(next);if(errors.length)throw Error(errors.join("; "));return next;
}
