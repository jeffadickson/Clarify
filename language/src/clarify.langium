grammar ClarifyDsl

// A conservative first grammar: existing form layouts are the compatibility
// target, not an invented replacement syntax. Comments remain in the CST.
entry Document: forms+=Form*;

Form:
    'form' name=ID 'for' entity=ID '{'
      elements+=Element*
    '}';

Element:
    Group | Grid | Placement | Title | Actions | Row | Stack;

Title: 'title' value=STRING;

Group:
    'group' name=STRING '{' elements+=Element* '}';

Grid:
    'grid' 'columns' columns=INT ('gap' gap=INT)?
    '{' elements+=Element* '}';

Row: 'row' '{' elements+=Element* '}';
Stack: 'stack' '{' elements+=Element* '}';
Actions: 'actions' '{' actions+=ID* '}';

Placement:
    name=ID 'at' '(' x=INT ',' y=INT ')'
    'span' '(' width=INT ',' height=INT ')';

hidden terminal WS: /\s+/;
hidden terminal SL_COMMENT: /\/\/[^\n\r]*/;
terminal ID: /[_a-zA-Z][_a-zA-Z0-9]*/;
terminal INT returns number: /[0-9]+/;
terminal STRING: /"(?:\\.|[^"\\])*"/;
