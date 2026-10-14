import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {emitElsaWorkflow} from './generate-elsa.mjs';
const source=readFileSync(new URL('../examples/workflows/OrderApproval.clarify',import.meta.url),'utf8');
test('generates real Elsa WorkflowBase with bookmark activity',()=>{
 const text=emitElsaWorkflow(source);
 assert.match(text,/class GeneratedOrderApprovalWorkflow : WorkflowBase/);
 assert.match(text,/new WaitForManagerApproval/);
 assert.match(text,/new WriteLine/);
});
test('fails closed on non-compiled Elsa native nodes',()=>{
 const extra=source.replace('step PublishOrder','elsa "Elsa.Http.SendHttpRequest" as Notify config "activities/send.json"\n step PublishOrder');
 assert.throws(()=>emitElsaWorkflow(extra),/does not support/);
});
test('checked in Elsa source matches compiler output',()=>{
 const generated=readFileSync(new URL('../tests/Clarify.Elsa.Approval/GeneratedOrderApprovalWorkflow.cs',import.meta.url),'utf8');
 assert.equal(generated,emitElsaWorkflow(source));
});
