import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {generateAsyncContracts} from '../tools/generate-async-contracts.mjs';
const dictionary=JSON.parse(readFileSync(new URL('../examples/order-entry/dictionary.json',import.meta.url)));
test('every entity gets an async cancellable browse contract',()=>{
 const result=generateAsyncContracts(dictionary);
 for(const entity of dictionary.entities)assert.ok(result.includes('I'+entity.name+'AsyncBrowse'));
 assert.equal((result.match(/CancellationToken ct = default/g)||[]).length,dictionary.entities.length);
 assert.equal(result,readFileSync(new URL('../examples/generated/AsyncStores.g.cs',import.meta.url),'utf8'));
 assert.doesNotMatch(result,/\.Result|\.Wait\(/);
});