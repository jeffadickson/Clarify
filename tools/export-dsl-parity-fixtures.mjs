// Produce native parser compatibility fixtures from the CURRENT active parser.
// This is a golden comparator; it never rewrites user-owned application code.
import {readFileSync,writeFileSync,mkdirSync} from 'node:fs';
import {resolve,dirname} from 'node:path';
import {fileURLToPath} from 'node:url';
import {instantiateTemplate} from '../designer/templates.mjs';
import {parse} from '../designer/clarifydsl.mjs';
const root=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const read=p=>JSON.parse(readFileSync(resolve(root,p),'utf8'));
const dictionary=read('examples/order-entry/dictionary.json');
const catalog=read('src/Clarify.Demo.Web/wwwroot/studio/sample-template-index.json');
if(catalog.length!==40)throw Error('Expected 40 sample templates');
const fixtures=[];
function add(label,source){
 const parsed=parse(source,dictionary);
 if(!parsed.ok)throw Error(label+': '+parsed.errors.join('; '));
 fixtures.push({label,source,form:parsed.form,entity:parsed.entity,
  fields:parsed.controls.map(c=>({name:c.name,x:c.x,y:c.y,width:c.w,height:c.h,line:c.line+1}))});
}
add('Customer.clarify',readFileSync(resolve(root,'examples/forms/Customer.clarify'),'utf8'));
for(const sample of catalog){
 const manifest=read('templates/samples/'+sample.slug+'.json');
 const bindings={Entity:sample.entity,...(sample.detail?{Detail:sample.detail}:{})};
 const output=instantiateTemplate(manifest,dictionary,bindings);
 for(const generated of output.files)add(sample.slug+'/'+generated.entity,generated.source);
}
const dir=resolve(root,'tests/Clarify.Dsl.Tests/Fixtures');
mkdirSync(dir,{recursive:true});
const path=resolve(dir,'parity.json');
writeFileSync(path,JSON.stringify(fixtures,null,2)+'\n','utf8');
console.log('Wrote '+fixtures.length+' active-parser parity cases');
