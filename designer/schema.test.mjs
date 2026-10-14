import test from 'node:test';import assert from 'node:assert/strict';
import{validate,addEntity,addField,addRelationship}from'./schema.mjs';
const base={entities:[{name:"Customer",fields:[{name:"Name",type:"string",required:true}]}]};
test("create entity and its primary key",()=>{const m=addEntity(base,"Order");assert.equal(m.entities[1].fields[0].primaryKey,true)});
test("add field and relationship from child to parent",()=>{let m=addEntity(base,"Order");m=addRelationship(m,"Order","Customer","CustomerId");assert.equal(m.relationships[0].cardinality,"one-to-many");assert.equal(m.entities[1].fields.at(-1).name,"CustomerId")});
test("duplicate column rejected",()=>assert.throws(()=>addField(base,"Customer",{name:"Name",type:"string"}),/Duplicate/));
test("invalid foreign keys rejected",()=>assert.ok(validate({...base,relationships:[{from:"Order",to:"Customer",foreignKey:"X",cardinality:"one-to-many",onDelete:"restrict"}]}).length));
test("existing dictionary preserved",()=>{let m=addEntity(base,"Product");assert.deepEqual(base.entities[0].fields,[{name:"Name",type:"string",required:true}]);assert.equal(validate(m).length,0)});
