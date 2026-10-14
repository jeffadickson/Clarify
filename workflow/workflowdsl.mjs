// Declarative workflow subset, strict and free of arbitrary code execution.
const identifier=/^[A-Za-z_][A-Za-z0-9_]*$/;
export function parseWorkflow(source) {
 const diagnostics=[], steps=[], waits=[];
 const lines=source.split(/\r?\n/), header=source.match(/^\s*workflow\s+(\w+)\s+for\s+(\w+)\s*\{/m);
 if(!header) return {ok:false,diagnostics:[{line:1,message:"Expected workflow Name for Entity {"}],steps,waits};
 let version=null, trigger=null, retry=0, notify=null, depth=0;
 for(let i=0;i<lines.length;i++){
  const line=lines[i].replace(/\/\/.*$/,"").trim();if(!line)continue;
  if(/^workflow\b/.test(line)){depth++;continue}
  if(line==="}"){depth--;continue}
  let m;
  if((m=line.match(/^version\s+(\d+)$/))){version=Number(m[1]);continue}
  if((m=line.match(/^start\s+when\s+(\w+)\.(\w+)$/))){trigger={entity:m[1],event:m[2]};continue}
  if((m=line.match(/^step\s+(\w+)$/))){steps.push({kind:"step",name:m[1],line:i+1});continue}
  if((m=line.match(/^wait\s+(\w+)\s+from\s+(\w+)$/))){const w={kind:"wait",event:m[1],role:m[2],line:i+1};steps.push(w);waits.push(w);continue}
  if((m=line.match(/^on\s+error\s*\{\s*retry\s+(\d+)\s+notify\s+(\w+)\s*\}$/))){retry=Number(m[1]);notify=m[2];continue}
  diagnostics.push({line:i+1,message:"Unsupported workflow statement: "+line});
 }
 if(depth!==0)diagnostics.push({line:lines.length,message:"Unbalanced workflow braces"});
 if(!Number.isSafeInteger(version)||version<1)diagnostics.push({line:1,message:"Missing positive workflow version"});
 if(!trigger)diagnostics.push({line:1,message:"Missing start when event"});
 if(steps.length===0)diagnostics.push({line:1,message:"No workflow activities"});
 if(steps.some(s=>s.kind==="step"&&!identifier.test(s.name)))diagnostics.push({line:1,message:"Invalid activity name"});
 if(retry>100)diagnostics.push({line:1,message:"Retry budget exceeds maximum 100"});
 return {ok:diagnostics.length===0,diagnostics,name:header[1],entity:header[2],version,trigger,steps,waits,retry,notify};
}
export function moveActivity(source,name,direction){
 const parsed=parseWorkflow(source);if(!parsed.ok)throw Error(parsed.diagnostics.map(x=>x.message).join("; "));
 const step=parsed.steps.find(s=>s.kind==="step"&&s.name===name);if(!step)throw Error("Unknown step");
 const index=parsed.steps.indexOf(step),other=parsed.steps[index+direction];if(!other) return source;
 const lines=source.split(/\r?\n/),a=step.line-1,b=other.line-1;[lines[a],lines[b]]=[lines[b],lines[a]];
 const output=lines.join("\n");if(!parseWorkflow(output).ok)throw Error("Invalid edit");return output;
}
