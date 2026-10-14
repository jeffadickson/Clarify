import test from "node:test";
import assert from "node:assert/strict";
import {mkdtempSync,mkdirSync,writeFileSync,symlinkSync,rmSync} from "node:fs";
import {tmpdir} from "node:os";
import {join} from "node:path";
import {validateElsaCatalog} from "./elsa-catalog.mjs";

const catalog={activities:[{type:"Elsa.Http.SendHttpRequest",enabled:true,properties:["Url","Method"]}]};
const manifest={schema:"clarify.elsa.manifest/1",steps:[{id:"step/1",type:"native-elsa-activity",nativeType:"Elsa.Http.SendHttpRequest",config:"activities/send.json"}]};
function fixture(fn){
 const d=mkdtempSync(join(tmpdir(),"clarify-elsa-"));
 try{mkdirSync(join(d,"activities"));writeFileSync(join(d,"activities/send.json"),JSON.stringify({Url:"https://example.invalid",Method:"GET"}));return fn(d)}
 finally{rmSync(d,{recursive:true,force:true})}
}
test("known native activity and valid config",()=>fixture(dir=>{
 const r=validateElsaCatalog(manifest,catalog,dir);
 assert.equal(r.ok,true,JSON.stringify(r.diagnostics));
 assert.equal(r.configurations.get("step/1").configuration.Method,"GET");
}));
test("reject unknown activities",()=>fixture(dir=>{
 const r=validateElsaCatalog(manifest,{activities:[]},dir);
 assert.equal(r.diagnostics[0].code,"ELSA1001");
}));
test("reject unsupported property",()=>fixture(dir=>{
 writeFileSync(join(dir,"activities/send.json"),'{"Secret":"x"}');
 assert.equal(validateElsaCatalog(manifest,catalog,dir).diagnostics[0].code,"ELSA1004");
}));
test("reject symlink escape",()=>fixture(dir=>{
 symlinkSync(join(dir,"../outside.json"),join(dir,"activities/escape.json"));
 const m={...manifest,steps:[{...manifest.steps[0],config:"activities/escape.json"}]};
 assert.equal(validateElsaCatalog(m,catalog,dir).ok,false);
}));
test("reject traversal",()=>fixture(dir=>{
 const m={...manifest,steps:[{...manifest.steps[0],config:"../secret.json"}]};
 assert.equal(validateElsaCatalog(m,catalog,dir).diagnostics[0].code,"ELSA1003");
}));
