import test from 'node:test';import assert from 'node:assert/strict';import{readFileSync}from'node:fs';import{instantiateTemplate}from'./templates.mjs';
const schema=JSON.parse(readFileSync(new URL('../examples/order-entry/dictionary.json',import.meta.url)));
const tpl=JSON.parse(readFileSync(new URL('../templates/master-detail/template.json',import.meta.url)));
test('instantiate master-detail layouts',()=>{const x=instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'});assert.equal(x.files.length,2);assert.match(x.files[0].source,/form OrderEditor for Order/);assert.match(x.files[1].source,/Quantity at/);});
test('reject nonexistent binding',()=>assert.throws(()=>instantiateTemplate(tpl,schema,{Parent:'Order',Child:'Missing'})));
test('reject unrelated tables',()=>assert.throws(()=>instantiateTemplate(tpl,schema,{Parent:'Customer',Child:'Product'})));
test('deterministic generation',()=>assert.deepEqual(instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'}),instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'})));

test('generated form records template identity and remains editable without losing provenance',async()=>{
 const {parse,patchField}=await import('./clarifydsl.mjs');
 const x=instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'});
 const source=x.files[0].source;
 const metadata=JSON.parse(source.split('\n')[0].replace('// @clarify-template ',''));
 assert.equal(metadata.id,'clarify.master-detail');
 assert.equal(metadata.version,tpl.version);
 assert.equal(metadata.bindings.Child,'OrderLine');
 const parsed=parse(source,schema);
 assert.equal(parsed.ok,true,parsed.errors.join('; '));
 const edited=patchField(source,schema,'CustomerId',{x:2,y:1,w:5,h:1});
 assert.equal(edited.split('\n')[0],source.split('\n')[0]);
 assert.match(edited,/CustomerId at \(2,1\) span \(5,1\)/);
});
