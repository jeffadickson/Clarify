import test from 'node:test';
import assert from 'node:assert/strict';
import {autoLayout,routeRelationship} from '../designer/er-layout.mjs';
const model={entities:[
 {name:'Customer',fields:[{name:'Id'},{name:'Name'}]},
 {name:'Order',fields:[{name:'Id'},{name:'CustomerId'}]},
 {name:'OrderLine',fields:[{name:'Id'},{name:'OrderId'}]}
],relationships:[{from:'Order',to:'Customer',foreignKey:'CustomerId'},{from:'OrderLine',to:'Order',foreignKey:'OrderId'}]};
test('parents precede children and layout is deterministic',()=>{
 const a=autoLayout(model);
 assert.deepEqual(a,autoLayout(model));
 assert.ok(a.positions.Customer.x<a.positions.Order.x);
 assert.ok(a.positions.Order.x<a.positions.OrderLine.x);
});
test('saved table coordinates are preserved',()=>{
 const result=autoLayout(model,1180,{Order:{x:777,y:111}});
 assert.deepEqual(result.positions.Order,{x:777,y:111});
});
test('reference cycles still receive finite positions',()=>{
 const cyclic=structuredClone(model);
 cyclic.relationships.push({from:'Customer',to:'OrderLine',foreignKey:'Id'});
 const result=autoLayout(cyclic);
 for(const p of Object.values(result.positions)){assert.ok(Number.isFinite(p.x));assert.ok(Number.isFinite(p.y))}
});
test('orthogonal connector returns SVG path',()=>{
 assert.match(routeRelationship({x:0,y:30},{x:500,y:100},240,1),/^M240,30 H\d+ V100 H500$/);
});
