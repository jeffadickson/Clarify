import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
const read=p=>readFileSync(new URL('../'+p,import.meta.url),'utf8');
test('embedded designer uses canonical ClarifyDSL parser source',()=>{
 assert.equal(read('src/Clarify.Demo.Web/wwwroot/studio/clarifydsl.mjs'),read('designer/clarifydsl.mjs'));
});
test('embedded dictionary matches source schema',()=>{
 assert.deepEqual(JSON.parse(read('src/Clarify.Demo.Web/wwwroot/studio/dictionary.json')),JSON.parse(read('examples/order-entry/dictionary.json')));
});
test('Studio form opens integrated editor and runtime',()=>{
 const page=read('src/Clarify.Demo.Web/Components/Pages/Studio.razor');
 assert.match(page,/src="\/studio\/designer.html"/);
 assert.match(page,/href="\/customers"/);
});
