import test from 'node:test';
import assert from 'node:assert/strict';
import {validate,addField} from '../designer/schema.mjs';
const base={entities:[{name:'Instrument',fields:[{name:'Id',type:'long',primaryKey:true,required:true}]}]};
test('money and engineering quantity types accepted by dictionary',()=>{
 let model=base;
 for(const type of ['money','length','mass','pressure','temperature'])
  model=addField(model,'Instrument',{name:'Field'+type,type,required:true});
 assert.deepEqual(validate(model),[]);
});
test('unknown semantic types rejected',()=>{
 assert.throws(()=>addField(base,'Instrument',{name:'Bad',type:'randomUnit'}),/Unsupported type/);
});
