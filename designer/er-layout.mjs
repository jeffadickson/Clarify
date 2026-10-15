// Pure ER layout: places parent entities to the left of their children.
// Existing manual positions are honored; no source dictionary is modified.
export function autoLayout(model, width=1180, saved={}) {
 const entities=model.entities||[], names=new Set(entities.map(e=>e.name));
 const incoming=new Map(entities.map(e=>[e.name,0]));
 const children=new Map(entities.map(e=>[e.name,[]]));
 for(const r of model.relationships||[]){
  if(!names.has(r.from)||!names.has(r.to)||r.from===r.to)continue;
  children.get(r.to).push(r.from);
  incoming.set(r.from,incoming.get(r.from)+1);
 }
 const layer=new Map(entities.map(e=>[e.name,0]));
 const queue=entities.filter(e=>incoming.get(e.name)===0).map(e=>e.name);
 const processed=new Set();
 while(queue.length){
  const name=queue.shift();
  if(processed.has(name))continue;
  processed.add(name);
  for(const child of children.get(name)){
   layer.set(child,Math.max(layer.get(child),layer.get(name)+1));
   incoming.set(child,incoming.get(child)-1);
   if(incoming.get(child)===0)queue.push(child);
  }
 }
 // Cyclic non-cascading references still need a position.
 for(const e of entities)if(!processed.has(e.name))layer.set(e.name,Math.max(1,layer.get(e.name)));
 const groups=new Map();
 for(const e of entities){const n=layer.get(e.name);if(!groups.has(n))groups.set(n,[]);groups.get(n).push(e)}
 const positions={};
 for(const [column,group] of groups){
  let y=24;
  for(const e of group.sort((a,b)=>a.name.localeCompare(b.name))){
   const provided=saved[e.name];
   positions[e.name]=provided&&Number.isFinite(provided.x)&&Number.isFinite(provided.y)
    ?{x:Math.max(0,provided.x),y:Math.max(0,provided.y)}
    :{x:24+column*285,y};
   y+=Math.max(170,78+e.fields.length*24)+26;
  }
 }
 const contentWidth=Math.max(width, ...Object.values(positions).map(p=>p.x+265),width);
 const contentHeight=Math.max(500,...entities.map(e=>positions[e.name].y+78+e.fields.length*24));
 return {positions,contentWidth,contentHeight};
}
// Orthogonal connector points anchored to facing table edges, with a lane
// per relationship to reduce identical route overlap.
export function routeRelationship(from,to,width=240,lane=0){
 const forward=from.x<=to.x;
 const sx=forward?from.x+width:from.x, tx=forward?to.x:to.x+width;
 const mid=(sx+tx)/2+lane*10;
 return 'M'+sx+','+from.y+' H'+mid+' V'+to.y+' H'+tx;
}
