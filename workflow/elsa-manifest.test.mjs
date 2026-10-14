import test from "node:test";
import assert from "node:assert/strict";
import {readFileSync} from "node:fs";
import {compileElsaManifest} from "./elsa-manifest.mjs";
const source=readFileSync(new URL("../examples/workflows/OrderApproval.clarify",import.meta.url),"utf8");
test("Elsa manifest retains version and stable IDs",()=>{
 const m=compileElsaManifest(source,["ValidateOrder","ReserveInventory","PublishOrder"]);
 assert.equal(m.version,1);
 assert.equal(m.steps.length,4);
 assert.equal(m.steps[1].type,"human-approval-bookmark");
 assert.equal(m.steps[1].role,"Manager");
 assert.equal(m.steps[2].id,"OrderApproval/v1/2");
});
test("unknown C# activities are rejected",()=>assert.throws(()=>compileElsaManifest(source,["ValidateOrder"]),AggregateError));
test("compilation deterministic",()=>{
 assert.deepEqual(compileElsaManifest(source,["ValidateOrder","ReserveInventory","PublishOrder"]),compileElsaManifest(source,["ValidateOrder","ReserveInventory","PublishOrder"]));
});
