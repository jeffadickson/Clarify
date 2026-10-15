import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {analyzeDependencies,impactOfChange} from './dependencies.mjs';
const sample=JSON.parse(readFileSync(new URL('../examples/order-entry/dictionary.json',import.meta.url),'utf8'));
test('order-entry dependency graph has three relationships and no errors',()=>{
 const graph=analyzeDependencies(sample);
 assert.equal(graph.errors.length,0,graph.errors.join('; '));
 assert.equal(graph.edges.filter(x=>x.kind==='relationship').length,3);
 assert.ok(graph.edges.some(x=>x.from==='Customer.Id'&&x.to==='Order.CustomerId'));
});
test('cross-database relationship must be explicitly external',()=>{
 const model=structuredClone(sample);
 model.entities.find(e=>e.name==='Product').database='inventory';
 assert.match(analyzeDependencies(model).errors.join('; '),/must be kind external/);
 model.relationships.find(r=>r.to==='Product').kind='external';
 const result=analyzeDependencies(model);
 assert.equal(result.errors.length,0,result.errors.join('; '));
 assert.match(result.warnings.join('; '),/no native foreign key/);
});
test('cross-database cascades are forbidden',()=>{
 const model=structuredClone(sample);
 model.entities.find(e=>e.name==='OrderLine').database='detail';
 const rel=model.relationships.find(r=>r.to==='Order');
 rel.kind='external';
 assert.match(analyzeDependencies(model).errors.join('; '),/Cross-database cascade/);
});
test('set-null rejects required foreign keys',()=>{
 const model=structuredClone(sample);
 model.relationships[0].onDelete='set-null';
 assert.match(analyzeDependencies(model).errors.join('; '),/set-null requires nullable/);
});
test('explicit principal and foreign key types must agree',()=>{
 const model=structuredClone(sample);
 model.entities.find(e=>e.name==='Customer').fields.unshift({name:'Id',type:'long',primaryKey:true});
 assert.match(analyzeDependencies(model).errors.join('; '),/type mismatch/);
});
test('cascade cycles are rejected',()=>{
 const model={entities:[{name:'A',fields:[{name:'Id',type:'long'},{name:'BId',type:'long'}]},{name:'B',fields:[{name:'Id',type:'long'},{name:'AId',type:'long'}]}],relationships:[{from:'A',to:'B',foreignKey:'BId',onDelete:'cascade'},{from:'B',to:'A',foreignKey:'AId',onDelete:'cascade'}]};
 assert.match(analyzeDependencies(model).errors.join('; '),/Cascade cycle/);
});
test('removing a parent key reports dependent foreign key impact',()=>{
 const before=structuredClone(sample),after=structuredClone(sample);
 after.entities.find(e=>e.name==='Customer').fields.unshift({name:'Id',type:'int',primaryKey:true});
 before.entities.find(e=>e.name==='Customer').fields.unshift({name:'Id',type:'int',primaryKey:true});
 after.entities.find(e=>e.name==='Customer').fields=after.entities.find(e=>e.name==='Customer').fields.filter(f=>f.name!=='Id');
 const impact=impactOfChange(before,after);
 const removed=impact.changes.find(x=>x.target==='Customer.Id');
 assert.equal(removed.risk,'destructive');
 assert.ok(removed.affected.includes('Order.CustomerId'));
 assert.equal(impact.requiresReview,true);
});
test('moving entity to another database requires review',()=>{
 const after=structuredClone(sample);
 after.entities.find(e=>e.name==='Product').database='inventory';
 const result=impactOfChange(sample,after);
 assert.ok(result.changes.some(x=>x.kind==='move-database'));
 assert.ok(result.errors.length);
});

test('multi-database sample validates without pretending a native FK exists',()=>{
 const model=JSON.parse(readFileSync(new URL('../examples/multi-database/dictionary.json',import.meta.url),'utf8'));
 const graph=analyzeDependencies(model);
 assert.deepEqual(graph.errors,[]);
 assert.deepEqual(graph.databases,['inventory','sales']);
 assert.ok(graph.edges.some(e=>e.kind==='external-reference'));
 assert.ok(graph.warnings.some(w=>w.includes('no native foreign key')));
 const invalid=structuredClone(model);
 invalid.entities[0].database='unknown';
 assert.match(analyzeDependencies(invalid).errors.join('; '),/Unregistered database/);
});

test('new fields and relationships appear in migration preview',()=>{
 const before=structuredClone(sample),after=structuredClone(sample);
 after.entities.find(e=>e.name==='Customer').fields.push({name:'Region',type:'string'});
 after.relationships.push({from:'Order',to:'Product',foreignKey:'ProductId',onDelete:'restrict'});
 const impact=impactOfChange(before,after);
 assert.ok(impact.changes.some(c=>c.kind==='add-field'&&c.target==='Customer.Region'));
 assert.ok(impact.changes.some(c=>c.kind==='add-relationship'));
 assert.equal(impact.requiresReview,true);
});
