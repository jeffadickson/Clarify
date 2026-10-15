import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {parse} from '../generated/clarify-parser.mjs';
const example=readFileSync(new URL('../../examples/forms/Customer.clarify',import.meta.url),'utf8');
test('parses existing Customer form layout',()=>{
 const tree=parse(example);
 assert.equal(tree.forms[0].entity,'Customer');
 assert.ok(tree.forms[0].items.some(x=>x.kind==='Group'));
});
test('returns source locations for visual editing',()=>{
 const tree=parse('form Demo for Customer { grid columns 12 { Name at (1,1) span (6,1) } }');
 const item=tree.forms[0].items[0].items[0];
 assert.equal(item.kind,'Placement');
 assert.equal(item.location.start.line,1);
});
test('invalid field positioning rejects rather than silently dropping source',()=>{
 assert.throws(()=>parse('form Demo for Customer { Name at (1,) span (6,1) }'));
});
