import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {generateEfModels} from '../designer/ef-models.mjs';
import {analyzeDependencies} from '../designer/dependencies.mjs';
const root=new URL('../',import.meta.url);
const read=p=>readFileSync(new URL(p,root),'utf8');
const dictionary=JSON.parse(read('examples/equipment-checkout/dictionary.json'));
test('checkout app generated code is reproducible',()=>{
 assert.deepEqual(analyzeDependencies(dictionary).errors,[]);
 const output=generateEfModels(dictionary);
 assert.equal(output.files.length,1);
 assert.equal(output.files[0].source,read('examples/equipment-checkout/generated/CheckoutDbContext.g.cs'));
 const source=output.files[0].source;
 assert.match(source,/HasOne<Person>\(\)\.WithMany\(\)\.HasForeignKey<Loan>\(x => x.PersonId\)/);
 assert.match(source,/HasOne<Equipment>\(\)\.WithMany\(\)\.HasForeignKey<Loan>\(x => x.EquipmentId\)/);
});
test('checkout sample has independently authored host and UI',()=>{
 const program=read('examples/equipment-checkout/Program.cs');
 const html=read('examples/equipment-checkout/wwwroot/index.html');
 assert.match(program,/MapPost\("\/api\/loans"/);
 assert.match(program,/MapPost\("\/api\/loans\/\{id:long\}\/return"/);
 assert.match(program,/SaveChangesAsync/);
 assert.match(html,/id="checkout"/);
 assert.match(html,/id="history"/);
});
