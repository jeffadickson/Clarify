import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {generateEfModels} from '../tools/generate-ef-models.mjs';

const read=p=>JSON.parse(readFileSync(new URL('../'+p,import.meta.url),'utf8'));
const fixture=read('examples/relational-generation/dictionary.json');
const external=read('examples/multi-database/dictionary.json');

test('generates real EF Core relationship mappings and delete behavior',()=>{
 const result=generateEfModels(fixture);
 assert.equal(result.files.length,1);
 const source=result.files[0].source;
 assert.match(source,/class SalesDbContext/);
 assert.match(source,/HasOne<Customer>\(\)\.WithMany\(\)\.HasForeignKey<Order>\(x => x.CustomerId\)\.OnDelete\(DeleteBehavior.Restrict\)/);
 assert.match(source,/HasOne<Order>\(\)\.WithMany\(\)\.HasForeignKey<OrderLine>\(x => x.OrderId\)\.OnDelete\(DeleteBehavior.Cascade\)/);
 assert.match(source,/HasOne<Product>\(\)\.WithMany\(\)\.HasForeignKey<OrderLine>\(x => x.ProductId\)/);
 assert.match(source,/HasIndex\(x => x.CustomerId\)/);
 assert.match(source,/HasMaxLength\(160\)/);
 assert.equal(generateEfModels(fixture).files[0].source,source);
});
test('external references never generate cross-database EF foreign keys',()=>{
 const result=generateEfModels(external);
 assert.equal(result.files.length,2);
 assert.ok(result.warnings.some(w=>w.includes('no native foreign key')));
 for(const file of result.files)assert.doesNotMatch(file.source,/\.HasOne</);
});
test('rejects unsafe or incomplete model instead of guessing a schema',()=>{
 const old=read('examples/order-entry/dictionary.json');
 assert.throws(()=>generateEfModels(old),/explicit primary key/);
 const wrong=structuredClone(fixture);
 wrong.entities.find(e=>e.name==='Order').fields.find(f=>f.name==='CustomerId').type='int';
 assert.throws(()=>generateEfModels(wrong),/type mismatch/i);
 const unsupported=structuredClone(fixture);
 unsupported.entities.find(e=>e.name==='Product').fields.push({name:'Weight',type:'mass'});
 assert.throws(()=>generateEfModels(unsupported),/Unsupported generated field type/);
});
test('one-to-one references generate a unique index',()=>{
 const model=structuredClone(fixture);
 model.relationships[0].cardinality='one-to-one';
 const source=generateEfModels(model).files[0].source;
 assert.match(source,/HasOne<Customer>\(\)\.WithOne\(\)/);
 assert.match(source,/HasIndex\(x => x.CustomerId\)\.IsUnique\(\)/);
});
