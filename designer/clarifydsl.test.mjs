import test from 'node:test';import assert from 'node:assert/strict';import{schemaForm,parse,patchField}from'./clarifydsl.mjs';
const s={entities:[{name:'Customer',fields:[{name:'Name'},{name:'Email'},{name:'Phone'}]}]};
test('schema-driven output parses',()=>{const a=parse(schemaForm(s,'Customer'),s);assert.equal(a.ok,true);assert.equal(a.controls.length,3)});
test('nested groups have path identities',()=>{const a='form F for Customer {\n group "Contact" {\n grid columns 12 {\n Name at (1,1) span (6,1)\n }\n }\n}\n';const p=parse(a,s);assert.equal(p.ok,true);assert.match(p.controls[0].path,/group/);});
test('visual change preserves comments elsewhere',()=>{let a=schemaForm(s,'Customer').replace('Email at','Email at').replace('Name at','Name at');a+='// retained comment';let b=patchField(a,s,'Phone',{y:4});assert.match(b,/retained comment/);assert.match(b,/Phone at \(1,4\)/)});
test('collision rejected',()=>{assert.throws(()=>patchField(schemaForm(s,'Customer'),s,'Email',{x:1}),/overlap/)});
