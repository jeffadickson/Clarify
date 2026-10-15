import test from 'node:test';import assert from 'node:assert/strict';import{readFileSync}from'node:fs';import{instantiateTemplate}from'./templates.mjs';
const schema=JSON.parse(readFileSync(new URL('../examples/order-entry/dictionary.json',import.meta.url)));
const tpl=JSON.parse(readFileSync(new URL('../templates/master-detail/template.json',import.meta.url)));
test('instantiate master-detail layouts',()=>{const x=instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'});assert.equal(x.files.length,2);assert.match(x.files[0].source,/form OrderEditor for Order/);assert.match(x.files[1].source,/Quantity at/);});
test('reject nonexistent binding',()=>assert.throws(()=>instantiateTemplate(tpl,schema,{Parent:'Order',Child:'Missing'})));
test('reject unrelated tables',()=>assert.throws(()=>instantiateTemplate(tpl,schema,{Parent:'Customer',Child:'Product'})));
test('deterministic generation',()=>assert.deepEqual(instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'}),instantiateTemplate(tpl,schema,{Parent:'Order',Child:'OrderLine'})));
