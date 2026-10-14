import test from "node:test";
import assert from "node:assert/strict";
import {readFileSync} from "node:fs";
import {generateObservabilityRegistration} from "../tools/generate-observability.mjs";
const base=new URL("../",import.meta.url);
const dictionary=JSON.parse(readFileSync(new URL("examples/order-entry/dictionary.json",base),"utf8"));
test("every generated application has automatic instrumentation",()=>{
 const generated=generateObservabilityRegistration(dictionary);
 assert.match(generated,/AddClarifyObservability\(configuration\)/);
 assert.match(generated,/ApplicationName = "OrderEntry"/);
 assert.equal(generated,readFileSync(new URL("src/Clarify.Demo.Web/GeneratedObservability.cs",base),"utf8"));
});
test("invalid dictionary or app identity fails generation",()=>{
 assert.throws(()=>generateObservabilityRegistration({entities:[]}));
 assert.throws(()=>generateObservabilityRegistration(dictionary,'bad"name'));
});
