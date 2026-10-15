// Declarative control definitions, shared by Studio and generators.
// A field's data type is distinct from its visual editor. No executable expressions.
export const controlKinds=Object.freeze(["text","textarea","number","date","checkbox","dropdown","radio","buttons"]);
const identifiers=/^[A-Za-z_][A-Za-z0-9_]*$/;
export function inferControl(field){
 if(field.type==="bool")return "checkbox";
 if(field.type==="date")return "date";
 if(["int","long","decimal","money","length","mass","pressure","temperature"].includes(field.type))return "number";
 return "text";
}
export function validateControl(field,config){
 const errors=[],kind=config.kind||inferControl(field);
 if(!controlKinds.includes(kind))errors.push("Unsupported control: "+kind);
 const choices=config.choices||[];
 if(["dropdown","radio","buttons"].includes(kind)){
  if(!Array.isArray(choices)||choices.length<2||choices.some(x=>typeof x!=="string"||!x.trim()))
   errors.push(kind+" requires at least two non-empty choices");
  if(new Set(choices).size!==choices.length)errors.push("Duplicate choices");
  if(!["string","int","long"].includes(field.type))errors.push("Choice control requires string or integer field");
 }
 if(kind==="checkbox"&&field.type!=="bool")errors.push("Checkbox requires bool");
 if(kind==="date"&&field.type!=="date")errors.push("Date control requires date");
 if(kind==="number"&&!["int","long","decimal","money","length","mass","pressure","temperature"].includes(field.type))errors.push("Number control requires numeric field");
 if(["text","textarea"].includes(kind)&&field.type!=="string")errors.push("Text control requires string");
 const rules=config.validation||{};
 if(rules.min!==undefined&&(!Number.isFinite(rules.min)||!["int","long","decimal","money","length","mass","pressure","temperature"].includes(field.type)))errors.push("Invalid minimum");
 if(rules.max!==undefined&&(!Number.isFinite(rules.max)||!["int","long","decimal","money","length","mass","pressure","temperature"].includes(field.type)))errors.push("Invalid maximum");
 if(rules.min!==undefined&&rules.max!==undefined&&rules.min>rules.max)errors.push("Minimum exceeds maximum");
 if(rules.maxLength!==undefined&&(!Number.isInteger(rules.maxLength)||rules.maxLength<1||field.type!=="string"))errors.push("Invalid maximum length");
 return errors;
}
export function parseControlMetadata(source,fieldName){
 const line=source.split("\n").find(x=>x.trim().startsWith("// @clarify-control ")&&(()=>{
  try{return JSON.parse(x.trim().slice("// @clarify-control ".length)).field===fieldName}catch{return false}
 })());
 return line?JSON.parse(line.trim().slice("// @clarify-control ".length)):null;
}
// Replaces only the field's control metadata; field position and user C# are untouched.
export function patchControl(source,field,config){
 if(!identifiers.test(field.name))throw Error("Invalid field identifier");
 const errors=validateControl(field,config);
 if(errors.length)throw Error(errors.join("; "));
 const lines=source.split("\n");
 const marker="// @clarify-control ";
 const matches=lines.map((line,i)=>({line,i})).filter(x=>{
  if(!x.line.trim().startsWith(marker))return false;
  try{return JSON.parse(x.line.trim().slice(marker.length)).field===field.name}catch{return false}
 });
 if(matches.length>1)throw Error("Duplicate control metadata for "+field.name);
 const declaration=lines.findIndex(line=>new RegExp("^\\s*"+field.name+"\\s+at\\s*\\(").test(line));
 if(declaration<0)throw Error("Field declaration not found: "+field.name);
 const indentation=lines[declaration].match(/^\s*/)[0];
 const encoded=indentation+marker+JSON.stringify({field:field.name,kind:config.kind||inferControl(field),...(config.choices?{choices:config.choices}:{}),...(config.validation?{validation:config.validation}:{})});
 if(matches.length)lines[matches[0].i]=encoded;
 else lines.splice(declaration,0,encoded);
 return lines.join("\n");
}
