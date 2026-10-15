import {parseControlMetadata,validateControl} from './controls.mjs';
export function schemaForm(s,name){const e=s.entities.find(x=>x.name===name);if(!e)throw Error("Unknown entity");return 'form '+name+'Editor for '+name+' {\n  grid columns 12 {\n'+e.fields.map((f,i)=>'    '+f.name+' at ('+(1+(i%2)*6)+','+(1+Math.floor(i/2))+') span (6,1)').join('\n')+'\n  }\n}\n'}
const loc=/\bat\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)\s*span\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)/;
export function parse(source,schema){
const lines=source.split('\n'),errors=[],controls=[],nodes=[],header=source.match(/^\s*form\s+(\w+)\s+for\s+(\w+)\s*\{/m);
if(!header)return {ok:false,errors:['Missing form declaration'],controls,lines,nodes};
const entity=schema.entities.find(e=>e.name===header[2]);if(!entity)errors.push('Unknown entity '+header[2]);
let stack=[{kind:'form',columns:12,path:'form'}],opened=false;
for(let i=0;i<lines.length;i++){let line=lines[i].replace(/\/\/.*$/,''),t=line.trim();if(!t)continue;
if(/^form\s+/.test(t)){opened=true;continue}
if(t==='}'){if(stack.length>1)stack.pop();else opened=false;continue}
const b=t.match(/^(grid|group|row|stack|tabs|tab|actions)\b(.*)\{\s*$/);
if(b){const p=stack.at(-1),n={kind:b[1],line:i,columns:+(b[2].match(/columns\s+(\d+)/)?.[1]||p.columns),path:p.path+'/'+b[1]+'@'+i};nodes.push(n);stack.push(n);continue}
if(/^(title|size|breakpoint)\b/.test(t)||/^actions\s*\{.*\}\s*$/.test(t)||stack.at(-1).kind==='actions')continue;
const n=t.match(/^(\w+)\b/),m=line.match(loc);if(!n||!m){errors.push('Line '+(i+1)+': invalid field declaration');continue}
if(!entity?.fields.some(f=>f.name===n[1]))errors.push('Line '+(i+1)+': unknown field '+n[1]);
const c={id:n[1],name:n[1],x:+m[1],y:+m[2],w:+m[3],h:+m[4],line:i,path:stack.at(-1).path};
if(c.x<1||c.y<1||c.w<1||c.h<1||c.x+c.w-1>stack.at(-1).columns)errors.push('Line '+(i+1)+': out of bounds');
if(controls.some(z=>z.name===c.name))errors.push('Line '+(i+1)+': duplicate field');
if(controls.some(z=>z.path===c.path&&c.x<z.x+z.w&&z.x<c.x+c.w&&c.y<z.y+z.h&&z.y<c.y+c.h))errors.push('Line '+(i+1)+': overlap');
const meta=parseControlMetadata(source,c.name);if(meta){const field=entity?.fields.find(f=>f.name===c.name);if(field){for(const issue of validateControl(field,meta))errors.push('Line '+(i+1)+': '+issue)}c.control=meta;}controls.push(c)}
if(opened||stack.length!==1)errors.push('Unclosed block');
return {ok:!errors.length,errors,controls,lines,nodes,form:header[1],entity:header[2],columns:nodes.find(n=>n.kind==='grid')?.columns||12}}
export function patchField(source,schema,id,change){const p=parse(source,schema);if(!p.ok)throw Error(p.errors.join('; '));const c=p.controls.find(x=>x.id===id);if(!c)throw Error('Unknown field');const v={...c,...change};if(![v.x,v.y,v.w,v.h].every(x=>Number.isInteger(x)&&x>0))throw Error('Invalid position');const lines=source.split('\n');lines[c.line]=lines[c.line].replace(loc,'at ('+v.x+','+v.y+') span ('+v.w+','+v.h+')');const out=lines.join('\n'),q=parse(out,schema);if(!q.ok)throw Error(q.errors.join('; '));return out}
