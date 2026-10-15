import test from 'node:test';import assert from 'node:assert/strict';import{readFileSync}from'node:fs';import{instantiateTemplate}from'../designer/templates.mjs';import{parse}from'../designer/clarifydsl.mjs';
const root=new URL('../',import.meta.url);
const read=p=>JSON.parse(readFileSync(new URL(p,root),'utf8'));
const dictionary=read('examples/order-entry/dictionary.json');
const index=read('src/Clarify.Demo.Web/wwwroot/studio/sample-template-index.json');
test('all 40 templates have parsable forms and matching served assets',()=>{
 assert.equal(index.length,40);assert.equal(new Set(index.map(x=>x.slug)).size,20);
 for(const item of index){
  const source=read('templates/samples/'+item.slug+'.json');
  const served=read('src/Clarify.Demo.Web/wwwroot/studio/samples/'+item.slug+'.json');
  assert.deepEqual(served,source,item.slug+' served manifest drift');
  assert.equal(source.version,'0.2.0',item.slug+' requires typed template version');
  assert.equal(source.valueTypes.money,'NodaMoney.Money');
  assert.equal(source.valueTypes.quantities,'UnitsNet');
  const binds={Entity:item.entity,...(item.detail?{Detail:item.detail}:{})};
  const generated=instantiateTemplate(source,dictionary,binds);
  assert.ok(generated.files.length>0,item.slug);
  for(const file of generated.files){const parsed=parse(file.source,dictionary);assert.equal(parsed.ok,true,item.slug+': '+parsed.errors.join('; '))}
 }
});

test('product and order line templates carry typed money and engineering metadata',()=>{
 const product=read('templates/samples/product-editor.json');
 const productResult=instantiateTemplate(product,dictionary,{Entity:'Product'});
 const source=productResult.files[0].source;
 for(const type of ['money','length','mass','pressure','temperature'])
  assert.ok(source.includes('"type":"'+type+'"'),type);
 assert.ok(source.includes('"currency":"USD"'));
 assert.ok(source.includes('"dotnet":"NodaMoney.Money"'));
 assert.ok(source.includes('"dotnet":"UnitsNet.Length"'));
 const line=read('templates/samples/orderline-pricing.json');
 const result=instantiateTemplate(line,dictionary,{Entity:'OrderLine'});
 assert.ok(result.files[0].source.includes('"dotnet":"NodaMoney.Money"'));
});

test('visual position editing retains money and quantity declarations',async()=>{
 const product=read('templates/samples/product-editor.json');
 const output=instantiateTemplate(product,dictionary,{Entity:'Product'});
 const before=output.files[0].source;
 const {patchField}=await import('../designer/clarifydsl.mjs');
 const after=patchField(before,dictionary,'Length',{x:2,y:2,w:5,h:1});
 assert.ok(after.includes('"dotnet":"UnitsNet.Length"'));
 assert.ok(after.includes('"currency":"USD"'));
 assert.match(after,/Length at \(2,2\) span \(5,1\)/);
});
