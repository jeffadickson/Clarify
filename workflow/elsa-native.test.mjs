import test from "node:test";
import assert from "node:assert/strict";
import {parseWorkflow} from "./workflowdsl.mjs";
import {compileWorkflow} from "./compile.mjs";
import {compileElsaManifest} from "./elsa-manifest.mjs";
const source=`workflow NativeExample for Order {
 version 1
 start when Order.Submitted
 step ValidateOrder
 elsa "Elsa.Http.SendHttpRequest" as HttpStep config "activities/send.json"
 wait Approval from Manager
}`;
test("native Elsa syntax parses qualified type and config path",()=>{
 const p=parseWorkflow(source);
 assert.equal(p.ok,true,JSON.stringify(p.diagnostics));
 assert.equal(p.steps[1].kind,"native");
 assert.equal(p.steps[1].type,"Elsa.Http.SendHttpRequest");
});
test("native metadata survives plan and Elsa manifest",()=>{
 const p=compileWorkflow(source);
 const m=compileElsaManifest(source,["ValidateOrder"]);
 assert.equal(p.Operations[1].NativeType,"Elsa.Http.SendHttpRequest");
 assert.equal(m.steps[1].type,"native-elsa-activity");
 assert.equal(m.steps[1].config,"activities/send.json");
});
test("native unsafe relative path rejected",()=>{
 const bad=source.replace("activities/send.json","../secrets.json");
 assert.equal(parseWorkflow(bad).ok,false);
});
