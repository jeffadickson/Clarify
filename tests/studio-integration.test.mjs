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

test('graphical dictionary uses same validated model as design library',()=>{
 assert.equal(read('src/Clarify.Demo.Web/wwwroot/studio/schema.mjs'),read('designer/schema.mjs'));
 const schema=JSON.parse(read('examples/order-entry/dictionary.json'));
 assert.equal(schema.relationships.length,3);
 const page=read('src/Clarify.Demo.Web/Components/Pages/Dictionary.razor');
 assert.match(page,/schema-designer.html/);
});

test('Studio dependency analyzer matches canonical implementation',()=>{
 assert.equal(read('src/Clarify.Demo.Web/wwwroot/studio/dependencies.mjs'),read('designer/dependencies.mjs'));
 const html=read('src/Clarify.Demo.Web/wwwroot/studio/schema-designer.html');
 assert.match(html,/dependency-summary/);
 assert.match(html,/export-impact/);
 assert.match(html,/options:\["native","external"\]/);
});

test('Studio EF generator matches canonical pure generator',()=>{
 assert.equal(read('src/Clarify.Demo.Web/wwwroot/studio/ef-models.mjs'),read('designer/ef-models.mjs'));
 const html=read('src/Clarify.Demo.Web/wwwroot/studio/schema-designer.html');
 assert.match(html,/preview-ef/);
 assert.match(html,/generateEfModels\(model\)/);
});

test('Studio migration preview uses canonical safe planner and fixture',()=>{
 assert.equal(read('src/Clarify.Demo.Web/wwwroot/studio/sqlite-migrations.mjs'),read('designer/sqlite-migrations.mjs'));
 assert.equal(read('src/Clarify.Demo.Web/wwwroot/studio/relational-dictionary.json'),read('examples/relational-generation/dictionary.json'));
 const html=read('src/Clarify.Demo.Web/wwwroot/studio/schema-designer.html');
 assert.match(html,/preview-migration/);
 assert.match(html,/planSqliteAdditiveMigration\(baseline,model/);
 assert.match(html,/relational-demo/);
});
