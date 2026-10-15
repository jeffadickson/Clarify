// Compile ClarifyDSL Customer form into strongly typed shared Blazor component.
// node tools/generate-shared-blazor.mjs
import {readFileSync,writeFileSync} from 'node:fs';
import{resolve,dirname}from'node:path';
import{fileURLToPath}from'node:url';
import{parse}from'../designer/clarifydsl.mjs';
import{renderControl}from'./control-renderer.mjs';
const root=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const schema=JSON.parse(readFileSync(resolve(root,'examples/order-entry/dictionary.json'),'utf8'));
const source=readFileSync(resolve(root,'examples/forms/Customer.clarify'),'utf8');
const parsed=parse(source,schema);
if(!parsed.ok)throw Error(parsed.errors.join('\n'));
if(parsed.entity!=='Customer')throw Error('Expected Customer layout');
const entity=schema.entities.find(x=>x.name==='Customer');
const xmlEscape=s=>s.replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('"','&quot;');
const cssEscape=s=>s.replace(/[^A-Za-z0-9_-]/g,'');
const lines=parsed.controls.map(f=>{
 const field=entity.fields.find(x=>x.name===f.name);
 if(!field)throw Error('Unknown field '+f.name);
 if(field.type!=='string')throw Error('Unsupported Customer editor '+field.type);
 const label=xmlEscape(f.name),prop=field.name,id='customer-'+cssEscape(f.name.toLowerCase());
 return '  <div style="grid-column: '+f.x+' / span '+f.w+'; grid-row: '+f.y+' / span '+f.h+'">\n'
 +'    <label for="'+id+'">'+label+'</label>\n'
 +'    '+renderControl(field,f.control,prop,id)+'\n'
 +'    <ValidationMessage For="() => Model.'+prop+'" />\n'
 +'  </div>';
});
const out='@* Generated from examples/forms/Customer.clarify — do not edit manually. *@\n<div class="clarify-fields">\n'
+lines.join('\n')+'\n</div>\n'
+'<style>.clarify-fields{display:grid;grid-template-columns:repeat('+parsed.columns+',minmax(0,1fr));gap:1rem}.clarify-fields>div{display:flex;flex-direction:column;gap:.4rem}.clarify-fields input{min-width:0}@media(max-width:700px){.clarify-fields{display:flex;flex-direction:column}}</style>\n'
+'@code {[Parameter,EditorRequired] public CustomerModel Model { get; set; } = default!; }\n';
const target=resolve(root,'src/Clarify.Forms.Shared/CustomerFields.razor');
writeFileSync(target,out,'utf8');
console.log('Generated '+target);
