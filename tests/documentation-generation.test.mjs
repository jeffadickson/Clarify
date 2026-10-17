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
 const workflow=read('tests/Clarify.Elsa.Approval/GeneratedOrderApprovalWorkflow.cs');
 assert.match(workflow,/\/\/\/ <summary>Constructs the generated Elsa activity graph/);
 const razor=read('src/Clarify.Forms.Shared/CustomerFields.razor');
 assert.match(razor,/\/\/\/ <summary>Customer record edited/);
});

test('Razor source and both generators escape CSS media rules',()=>{
 for(const path of [
  'src/Clarify.Forms.Shared/CustomerPage.razor',
  'src/Clarify.Forms.Shared/OrderPage.razor',
  'src/Clarify.Forms.Shared/CustomerFields.razor',
  'src/Clarify.Demo.Web/Components/Pages/Studio.razor'
 ]) {
  assert.doesNotMatch(read(path),/(?<!@)@media\\b/,path);
 }
 for(const path of ['tools/generate-shared-blazor.mjs','tools/generate-blazor.mjs'])
  assert.match(read(path),/@@media/,path);
});
