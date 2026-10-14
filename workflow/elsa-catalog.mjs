// Validates a compiled ClarifyDSL manifest against an Elsa activity catalog.
// This is a build-time compatibility check, NOT the Elsa runtime adapter.
import {readFileSync,realpathSync} from "node:fs";
import {resolve,relative,isAbsolute,sep,dirname} from "node:path";

export function validateElsaCatalog(manifest,catalog,projectRoot) {
  if(manifest.schema!=="clarify.elsa.manifest/1") throw Error("Unsupported manifest schema");
  if(!Array.isArray(catalog.activities)) throw Error("Catalog must contain activities");
  const diagnostics=[], configurations=new Map();
  const entries=new Map();
  for(const a of catalog.activities) {
    if(!a.type || entries.has(a.type))throw Error("Missing or duplicate catalog activity type");
    entries.set(a.type,a);
  }
  const root=realpathSync(projectRoot);
  for(const step of manifest.steps) {
    if(step.type!=="native-elsa-activity")continue;
    const descriptor=entries.get(step.nativeType);
    if(!descriptor) {diagnostics.push({node:step.id,code:"ELSA1001",message:"Unknown native Elsa activity: "+step.nativeType});continue}
    if(!descriptor.enabled) {diagnostics.push({node:step.id,code:"ELSA1002",message:"Activity not enabled: "+step.nativeType});continue}
    const path=step.config;
    if(typeof path!=="string"||isAbsolute(path)||path.split(/[\\/]/).includes("..")) {
      diagnostics.push({node:step.id,code:"ELSA1003",message:"Unsafe config path"});continue;
    }
    try {
      const full=realpathSync(resolve(root,path));
      const rel=relative(root,full);
      if(rel===".."||rel.startsWith(".."+sep)||isAbsolute(rel))throw Error("Config path escapes project");
      const raw=readFileSync(full,"utf8");
      const props=JSON.parse(raw);
      if(!props||typeof props!=="object"||Array.isArray(props))throw Error("Configuration must be an object");
      const allowed=new Set(descriptor.properties||[]);
      for(const key of Object.keys(props))if(!allowed.has(key))
        diagnostics.push({node:step.id,code:"ELSA1004",message:"Unsupported property: "+key});
      configurations.set(step.id,{type:step.nativeType,configuration:props});
    }catch(error) {
      diagnostics.push({node:step.id,code:"ELSA1005",message:"Cannot load configuration: "+error.message});
    }
  }
  return {ok:diagnostics.length===0,diagnostics,configurations};
}
