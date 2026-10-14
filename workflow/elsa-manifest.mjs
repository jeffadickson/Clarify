import {compileWorkflow} from "./compile.mjs";
/**
 * Validate the ClarifyDSL subset that the Elsa 3.8 adapter can represent.
 * The output is a deterministic intermediate manifest; Elsa's runtime-specific
 * definition objects must be created by a typed .NET adapter, not guessed JSON.
 */
export function compileElsaManifest(source, knownActivities) {
  const plan=compileWorkflow(source);
  const registered=new Set(knownActivities);
  const issues=[];
  for(let i=0;i<plan.Operations.length;i++){
    const op=plan.Operations[i];
    if(op.Kind==="step"&&!registered.has(op.Name))
      issues.push({index:i,message:"Unregistered C# activity: "+op.Name});
    if(op.Kind==="native" && (!op.NativeType || !op.Config)) issues.push({index:i,message:"Native activity needs type and config"});
    if(op.Kind==="wait"&&!op.Role)
      issues.push({index:i,message:"Human approval needs a role"});
  }
  if(issues.length)throw new AggregateError(issues.map(x=>new Error(x.message)),"Unsupported workflow plan");
  return {
    schema:"clarify.elsa.manifest/1",
    id:plan.Name,
    version:plan.Version,
    entity:plan.Entity,
    trigger:plan.Trigger,
    steps:plan.Operations.map((op,i)=>({
      id:plan.Name+"/v"+plan.Version+"/"+i,
      type:op.Kind==="wait"?"human-approval-bookmark":op.Kind==="native"?"native-elsa-activity":"registered-csharp-activity",
      activity:op.Name,
      role:op.Role??null,
      nativeType:op.NativeType??null,
      config:op.Config??null
    })),
    retryLimit:plan.MaxRetries
  };
}
