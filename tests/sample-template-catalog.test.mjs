import test from 'node:test';import assert from 'node:assert/strict';import{readFileSync}from'node:fs';import{instantiateTemplate}from'../designer/templates.mjs';import{parse}from'../designer/clarifydsl.mjs';
const root=new URL('../',import.meta.url);
const read=p=>JSON.parse(readFileSync(new URL(p,root),'utf8'));
const dictionary=read('examples/order-entry/dictionary.json');
const index=read('src/Clarify.Demo.Web/wwwroot/studio/sample-template-index.json');
test('all 20 templates have executable, parsable forms and served assets',()=>{
 assert.equal(index.length,20);assert.equal(new Set(index.map(x=>x.slug)).size,20);
 for(const item of index){
  const source=read('templates/samples/'+item.slug+'.json');
  const served=read('src/Clarify.Demo.Web/wwwroot/studio/samples/'+item.slug+'.json');
  assert.deepEqual(served,source,item.slug+' served manifest drift');
  const binds={Entity:item.entity,...(item.detail?{Detail:item.detail}:{})};
  const generated=instantiateTemplate(source,dictionary,binds);
  assert.ok(generated.files.length>0,item.slug);
  for(const file of generated.files){const parsed=parse(file.source,dictionary);assert.equal(parsed.ok,true,item.slug+': '+parsed.errors.join('; '))}
 }
});
