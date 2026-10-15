import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {planSqliteAdditiveMigration} from '../designer/sqlite-migrations.mjs';
const read=path=>JSON.parse(readFileSync(new URL('../'+path,import.meta.url),'utf8'));
const before=read('examples/relational-generation/dictionary.json');
const after=read('examples/relational-generation/dictionary-with-notes.json');
const clone=x=>structuredClone(x);
test('nullable Product.Notes produces a deterministic, reviewable SQLite plan',()=>{
 const plan=planSqliteAdditiveMigration(before,after);
 assert.equal(plan.executable,true);
 assert.deepEqual(plan.blocked,[]);
 assert.equal(plan.operations.length,1);
 assert.equal(plan.operations[0].sql,'ALTER TABLE "Product" ADD COLUMN "Notes" TEXT NULL;');
 assert.equal(plan.planId,planSqliteAdditiveMigration(before,after).planId);
 assert.deepEqual(plan,read('examples/relational-generation/product-notes.migration.json'));
});
test('reject destructive column drops and rename-like changes',()=>{
 const changed=clone(before);
 changed.entities.find(e=>e.name==='Product').fields=changed.entities.find(e=>e.name==='Product').fields.filter(f=>f.name!=='Name');
 const plan=planSqliteAdditiveMigration(before,changed);
 assert.equal(plan.executable,false);
 assert.match(plan.blocked.join(' '),/Removing or renaming/);
});
test('reject adding non-nullable or rich quantity columns',()=>{
 for(const field of [{name:'Status',type:'string',required:true},{name:'Mass',type:'mass'}]){
  const changed=clone(before);
  changed.entities.find(e=>e.name==='Product').fields.push(field);
  const plan=planSqliteAdditiveMigration(before,changed);
  assert.equal(plan.executable,false);
  assert.match(plan.blocked.join(' '),/Unsupported new field/);
 }
});
test('reject changed relationship behavior and database registry',()=>{
 const changed=clone(after);
 changed.relationships[0].onDelete='cascade';
 assert.equal(planSqliteAdditiveMigration(before,changed).executable,false);
 const other=clone(after);
 other.databases[0].provider='postgresql';
 assert.throws(()=>planSqliteAdditiveMigration(before,other),/SQLite/);
});
test('reject SQL identifier injection and unchanged schema',()=>{
 const changed=clone(before);
 changed.entities.find(e=>e.name==='Product').fields.push({name:'Notes;DROP',type:'string'});
 assert.throws(()=>planSqliteAdditiveMigration(before,changed),/Invalid SQL identifier/);
 assert.equal(planSqliteAdditiveMigration(before,before).executable,false);
});
