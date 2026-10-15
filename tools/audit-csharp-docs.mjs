// Dependency-free inventory of public C# API documentation.
// XML compiler warnings (CS1591) remain the authoritative Roslyn-level check.
// This lightweight inventory tracks legacy debt; generated .g.cs code is gated.
import {readdirSync,readFileSync,writeFileSync,mkdirSync} from 'node:fs';
import {resolve,join,relative,dirname} from 'node:path';
const root=resolve(dirname(new URL(import.meta.url).pathname),'..');
const targets=['src','tools','examples','tests'];
const files=[];
function visit(path){
 for(const entry of readdirSync(path,{withFileTypes:true})){
  if(['bin','obj','node_modules','.git','_site'].includes(entry.name))continue;
  const name=join(path,entry.name);
  if(entry.isDirectory())visit(name);
  else if(entry.name.endsWith('.cs'))files.push(name);
 }
}
for(const base of targets){try{visit(join(root,base))}catch{}}
let total=0,documented=0;
const missing=[],perFile=[];
for(const file of files){
 const lines=readFileSync(file,'utf8').split(/\r?\n/);
 let count=0,docs=0;
 for(let i=0;i<lines.length;i++){
  const line=lines[i].trim();
  // Heuristic, not a C# grammar. Ignore generated methods in lambdas and
  // declarations following inline comments/attributes where feasible.
  if(!/^(public|protected)(?:\s+(?:static|sealed|abstract|override|virtual|async|partial|readonly|new))*\s+/.test(line))continue;
  if(/^public (?:class|interface|enum|struct|record|delegate|event|[A-Z_a-z][\w<>?\[\], ]*\s+[A-Z_a-z]\w*\s*\()/ .test(line)||/^(public|protected)\s+/.test(line)){
   count++;total++;
   let j=i-1;while(j>=0 && (lines[j].trim()==='' || lines[j].trim().startsWith('[')))j--;
   if(j>=0 && lines[j].trim().startsWith('///')){docs++;documented++}
   else missing.push({path:relative(root,file).replaceAll('\\','/'),line:i+1,text:line.slice(0,140)});
  }
 }
 perFile.push({path:relative(root,file).replaceAll('\\','/'),members:count,documented:docs});
}
const generatedMissing=missing.filter(x=>x.path.endsWith('.g.cs')||x.path.includes('/generated/'));
const summary={generatedAt:new Date().toISOString(),files:files.length,publicMembers:total,documentedMembers:documented,undocumentedMembers:missing.length,generatedUndocumented:generatedMissing.length,
 note:'Heuristic inventory only. Roslyn CS1591 is the authoritative C# public API documentation diagnostic.'};
const output={summary,files:perFile,missing};
console.log(JSON.stringify(summary,null,2));
if(process.argv.includes('--report')){
 const location=resolve(root,'artifacts/documentation-coverage.json');
 mkdirSync(dirname(location),{recursive:true});writeFileSync(location,JSON.stringify(output,null,2)+'\n');
 console.log('Documentation debt report: '+location);
}
if(process.argv.includes('--strict-generated')&&generatedMissing.length){
 console.error('Generated public members missing XML documentation:',generatedMissing.slice(0,20));
 process.exitCode=1;
}
