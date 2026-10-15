import test from 'node:test';
import assert from 'node:assert/strict';
import {inferControl,patchControl,parseControlMetadata,validateControl} from '../designer/controls.mjs';
import {parse,patchField} from '../designer/clarifydsl.mjs';
import {renderControl} from '../tools/control-renderer.mjs';
const schema={entities:[{name:'Demo',fields:[
 {name:'Name',type:'string'},{name:'Status',type:'string'},
 {name:'Active',type:'bool'},{name:'Count',type:'int'},{name:'Date',type:'date'}
]}]};
const base='form DemoEditor for Demo {\n grid columns 12 {\n Name at (1,1) span (6,1)\n Status at (7,1) span (6,1)\n Active at (1,2) span (6,1)\n Count at (7,2) span (6,1)\n Date at (1,3) span (6,1)\n }\n}\n';
test('type inference selects appropriate default controls',()=>{
 assert.equal(inferControl({type:'string'}),'text');
 assert.equal(inferControl({type:'bool'}),'checkbox');
 assert.equal(inferControl({type:'date'}),'date');
 assert.equal(inferControl({type:'money'}),'number');
});
test('control edits survive layout movement and remain valid',()=>{
 const field=schema.entities[0].fields[1];
 const withControl=patchControl(base,field,{kind:'dropdown',choices:['Active','Pending','Inactive']});
 const moved=patchField(withControl,schema,'Status',{x:7,y:4});
 assert.equal(parse(moved,schema).ok,true);
 assert.deepEqual(parseControlMetadata(moved,'Status').choices,['Active','Pending','Inactive']);
 assert.match(moved,/Status at \(7,4\)/);
});
test('reject missing choices, duplicate choices, and mismatched types',()=>{
 assert.ok(validateControl({name:'Status',type:'string'},{kind:'radio',choices:[]}).length);
 assert.ok(validateControl({name:'Status',type:'string'},{kind:'dropdown',choices:['A','A']}).length);
 assert.ok(validateControl({name:'Name',type:'string'},{kind:'checkbox'}).length);
 assert.throws(()=>patchControl(base,{name:'Name',type:'string'},{kind:'number'}));
});
test('Blazor renderer supports all standard field controls',()=>{
 const string={name:'Status',type:'string'};
 for(const kind of ['dropdown','radio','buttons']){
  const output=renderControl(string,{kind,choices:['Active','Pending']},'Status','demo-status');
  assert.match(output,/Model.Status/);
 }
 assert.match(renderControl({name:'Name',type:'string'},{kind:'text',validation:{maxLength:50}},'Name','name'),/maxlength="50"/);
 assert.match(renderControl({name:'Notes',type:'string'},{kind:'textarea'},'Notes','notes'),/InputTextArea/);
 assert.match(renderControl({name:'Active',type:'bool'},{kind:'checkbox'},'Active','active'),/InputCheckbox/);
 assert.match(renderControl({name:'Count',type:'int'},{kind:'number'},'Count','count'),/InputNumber/);
 assert.match(renderControl({name:'Date',type:'date'},{kind:'date'},'Date','date'),/InputDate/);
});
