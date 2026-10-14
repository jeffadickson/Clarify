import {readFileSync,writeFileSync,mkdirSync} from "node:fs";
import {dirname,resolve} from "node:path";
import {fileURLToPath} from "node:url";
import {parseWorkflow} from "./workflowdsl.mjs";
export function compileWorkflow(source){
 const d=parseWorkflow(source);
 if(!d.ok)throw Error(d.diagnostics.map(x=>"Line "+x.line+": "+x.message).join("\n"));
 return {
  Name:d.name, Entity:d.entity,Version:d.version,
  Trigger:d.trigger.entity+"."+d.trigger.event,
  Operations:d.steps.map(s=>({Kind:s.kind,Name:s.kind==="step"?s.name:s.event,Role:s.role||null})),
  MaxRetries:d.retry
 };
}
if(process.argv[1]&&resolve(process.argv[1])===fileURLToPath(import.meta.url)){
 const root=resolve(dirname(fileURLToPath(import.meta.url)),"..");
 const source=readFileSync(resolve(root,"examples/workflows/OrderApproval.clarify"),"utf8");
 const plan=compileWorkflow(source);
 const dest=resolve(root,"examples/workflows/OrderApproval.plan.json");
 writeFileSync(dest,JSON.stringify(plan,null,2)+"\n");
 console.log("Compiled "+dest);
}
