import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
const root=new URL('../',import.meta.url);
const read=p=>readFileSync(new URL(p,root),'utf8');
test('C# XML documentation compilation and DocFX are configured',()=>{
 assert.match(read('Directory.Build.props'),/<GenerateDocumentationFile>true<\/GenerateDocumentationFile>/);
 const tools=JSON.parse(read('.config/dotnet-tools.json'));
 assert.ok(tools.tools.docfx,'DocFX tool must be pinned');
 assert.ok(JSON.parse(read('docs/docfx.json')).metadata.length);
});
test('core generated files include explanatory XML comments',()=>{
 const ef=read('examples/relational-generation/generated/SalesDbContext.g.cs');
 assert.match(ef,/\/\/\/ <summary>Dictionary field Id/);
 assert.match(ef,/\/\/\/ <summary>Query and update Customer records/);
 const browse=read('examples/generated/AsyncStores.g.cs');
 assert.match(browse,/\/\/\/ <param name="ct">/);
 assert.match(browse,/\/\/\/ <returns>/);
 const razor=read('src/Clarify.Forms.Shared/CustomerFields.razor');
 assert.match(razor,/\/\/\/ <summary>Customer record edited/);
});
