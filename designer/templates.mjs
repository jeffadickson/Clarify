// Declarative template instantiation. No eval, executable hooks or C# rewriting.
// Template packages provide immutable output recipes, never arbitrary JS code.
const ident=/^[A-Za-z_][A-Za-z0-9_]*$/;
export function instantiateTemplate(template,dictionary,bindings){
 if(template.format!=="clarify-template/v1")throw Error("Unsupported template format");
 const entities=new Map(dictionary.entities.map(e=>[e.name,e]));
 const bound={};
 for(const p of template.parameters||[]){
  const name=bindings[p.name];
  if(!ident.test(name||"")||!entities.has(name))throw Error("Invalid binding: "+p.name);
  bound[p.name]=entities.get(name);
 }
 const output=[];
 const instance={id:template.id,version:template.version,bindings:Object.fromEntries(Object.entries(bound).map(([k,v])=>[k,v.name]))};
 for(const recipe of template.forms||[]){
  const parent=bound[recipe.entity];
  if(!parent)throw Error("Unbound entity: "+recipe.entity);
  const columns=recipe.columns||12;
  if(!Number.isInteger(columns)||columns<1||columns>24)throw Error("Invalid grid columns");
  const lines=[`// @clarify-template ${JSON.stringify({...instance,formEntity:parent.name})}`,`form ${parent.name}Editor for ${parent.name} {`,`  title "${parent.name} Editor"`,`  grid columns ${columns} {`];
  const fields=parent.fields.filter(f=>!f.primaryKey);
  fields.forEach((field,i)=>{
   if(!ident.test(field.name))throw Error("Invalid field name");
   const width=Math.floor(columns/2);
   lines.push(`    ${field.name} at (${1+(i%2)*width},${1+Math.floor(i/2)}) span (${width},1)`);
  });
  lines.push("  }","}");
  output.push({path:`forms/${parent.name}Editor.clarify`,source:lines.join("\n")+"\n",entity:parent.name});
 }
 for(const requirement of template.relationships||[]){
  const child=bound[requirement.child],parent=bound[requirement.parent];
  if(!child||!parent)throw Error("Invalid relationship parameter");
  const matches=(dictionary.relationships||[]).filter(r=>r.from===child.name&&r.to===parent.name);
  if(matches.length!==1)throw Error(`Expected one relationship: ${child.name} -> ${parent.name}`);
 }
 return {templateId:template.id,version:template.version,bindings:{...bindings},files:output};
}
