// Rebuild generated form source from the checked-in ClarifyDSL and dictionary.
// Run after saving in Studio, then rebuild the .NET application. No dynamic
// source execution or package installation occurs here.
import {spawnSync} from 'node:child_process';
import {fileURLToPath} from 'node:url';
import {dirname,resolve} from 'node:path';
const root=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const child=spawnSync(process.execPath,[resolve(root,'tools/generate-shared-blazor.mjs')],{
 cwd:root,encoding:'utf8',stdio:'inherit'
});
if(child.error)throw child.error;
if(child.status!==0)process.exit(child.status||1);
console.log('Regeneration complete. Rebuild the web/native host to see changes.');
