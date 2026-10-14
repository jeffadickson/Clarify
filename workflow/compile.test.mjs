import test from "node:test";
import assert from "node:assert/strict";
import{readFileSync}from"node:fs";
import{compileWorkflow}from"./compile.mjs";
const source=readFileSync(new URL("../examples/workflows/OrderApproval.clarify",import.meta.url),"utf8");
test("compile executable workflow plan",()=>{
 const p=compileWorkflow(source);
 assert.equal(p.Name,"OrderApproval");assert.equal(p.Version,1);
 assert.deepEqual(p.Operations.map(x=>x.Kind),["step","wait","step","step"]);
 assert.equal(p.Operations[1].Name,"Approval");
 assert.equal(p.Operations[1].Role,"Manager");
});
test("reject unknown statements",()=>assert.throws(()=>compileWorkflow("workflow X for Order {\n version 1\n start when Order.Submitted\n eval evil\n}")));
