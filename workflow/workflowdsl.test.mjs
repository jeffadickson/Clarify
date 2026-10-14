import test from "node:test";
import assert from "node:assert/strict";
import {readFileSync} from "node:fs";
import {parseWorkflow,moveActivity} from "./workflowdsl.mjs";
const sample=readFileSync(new URL("../examples/workflows/OrderApproval.clarify",import.meta.url),"utf8");
test("parses versioned approval workflow with durable wait",()=>{const r=parseWorkflow(sample);assert.equal(r.ok,true,JSON.stringify(r.diagnostics));assert.equal(r.waits.length,1);assert.equal(r.version,1)});
test("source patch reorders steps and preserves comments",()=>{const changed=moveActivity(sample,"ReserveInventory",1);assert.ok(changed.indexOf("step PublishOrder")<changed.indexOf("step ReserveInventory"));assert.ok(changed.includes("// Planned workflow syntax"))});
test("rejects invalid missing trigger",()=>assert.equal(parseWorkflow("workflow X for Order {\nversion 1\nstep Test\n}").ok,false));
test("rejects arbitrary C# expression",()=>assert.equal(parseWorkflow("workflow X for Order {\nversion 1\nstart when Order.Submitted\nstep Do()\n}").ok,false));
