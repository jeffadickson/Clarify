import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
const index=JSON.parse(readFileSync(new URL('../src/Clarify.Demo.Web/wwwroot/studio/sample-template-index.json',import.meta.url)));
test('gallery has 40 unique template identifiers',()=>{
 assert.equal(index.length,40);
 assert.equal(new Set(index.map(x=>x.slug)).size,40);
});
