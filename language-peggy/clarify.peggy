{
  const asNumber = x => Number(x);
}
Document
  = _ forms:Form* _ { return {kind:"Document",forms}; }

Form
  = "form" __ name:Identifier __ "for" __ entity:Identifier _ "{" _ items:Item* "}" _
    { return {kind:"Form",name,entity,items}; }

Item = Group / Grid / Placement / Title

Title
  = "title" __ value:StringLiteral _ { return {kind:"Title",value}; }

Group
  = "group" __ name:StringLiteral _ "{" _ items:Item* "}" _
    { return {kind:"Group",name,items}; }

Grid
  = "grid" __ "columns" __ columns:Integer gap:(_ "gap" __ Integer)? _ "{" _ items:Item* "}" _
    { return {kind:"Grid",columns,gap:gap?gap[3]:null,items}; }

Placement
  = name:Identifier __ "at" _ "(" _ x:Integer _ "," _ y:Integer _ ")" __
    "span" _ "(" _ width:Integer _ "," _ height:Integer _ ")" _
    { return {kind:"Placement",name,x,y,width,height,location:location()}; }

Identifier = $([a-zA-Z_][a-zA-Z_0-9]*)
Integer = digits:$([0-9]+) { return asNumber(digits); }
StringLiteral = '"' chars:(( '\\\\' . / '\\"' / !'"' . )*) '"' { return chars.join(""); }

_ = (WhiteSpace / Comment)*
__ = (WhiteSpace / Comment)+
WhiteSpace = [ \t\r\n]
Comment = "//" [^\r\n]* ("\r\n" / "\n" / "\r" / !.)
