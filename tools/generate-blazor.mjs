// ClarifyDSL -> Razor component generator. Run: node tools/generate-blazor.mjs
import{readFileSync,mkdirSync,writeFileSync}from'node:fs';
import{fileURLToPath}from'node:url';import{dirname,resolve}from'node:path';
import{schemaForm,parse}from'../designer/clarifydsl.mjs';
const root=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const schema=JSON.parse(readFileSync(resolve(root,'examples/order-entry/dictionary.json'),'utf8'));
const esc=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('"','&quot;');
const target=resolve(root,'blazor/Clarify.Forms/Components/Pages');
mkdirSync(target,{recursive:true});
for(const entity of schema.entities){
const file=resolve(root,'examples/forms/'+entity.name+'.clarify');
let src;try{src=readFileSync(file,'utf8')}catch{src=schemaForm(schema,entity.name)}
const p=parse(src,schema);if(!p.ok)throw Error(entity.name+': '+p.errors.join('; '));
const fields=new Map(entity.fields.map(f=>[f.name,f]));
const grid=p.controls.map(c=>{const f=fields.get(c.name);const typ=f.type==='int'?'number':f.type==='date'?'date':f.type==='money'?'number':'text';const name=esc(c.name);
return '      <label class="field" style="grid-column: '+c.x+' / span '+c.w+';grid-row: '+c.y+' / span '+c.h+'"><span>'+name+'</span><input type="'+typ+'" name="'+name+'" '+(f.required?'required ':'')+(typ==='number'?'step="'+(f.type==='int'?'1':'0.01')+'" ':'')+' /></label>';}).join('\n');
const razor='@page "/forms/'+entity.name.toLowerCase()+'"\n@using Microsoft.AspNetCore.Components.Forms\n<h1>'+esc(entity.name)+' editor</h1>\n<form method="post" action="/api/'+entity.name.toLowerCase()+'">\n  <div class="clarify-grid">\n'+grid+'\n  </div>\n  <button type="submit">Save</button>\n</form>\n<style>.clarify-grid{display:grid;grid-template-columns:repeat('+p.columns+',minmax(0,1fr));gap:.65rem}.field{display:flex;flex-direction:column;gap:.2rem}.field input{min-width:0;padding:.6rem;border:1px solid #abc;border-radius:.35rem}@@media(max-width:650px){.clarify-grid{display:flex;flex-direction:column}}</style>\n';
writeFileSync(resolve(target,entity.name+'Editor.razor'),razor);console.log('Generated '+entity.name+'Editor.razor');
}
