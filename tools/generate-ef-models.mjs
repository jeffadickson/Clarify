// CLI wrapper for the shared pure EF Core generator.
import {generateEfModels} from '../designer/ef-models.mjs';
import {readFileSync,writeFileSync,mkdirSync} from 'node:fs';
import {resolve,dirname,join} from 'node:path';
import {fileURLToPath} from 'node:url';
export {generateEfModels};
const self=fileURLToPath(import.meta.url);
if(process.argv[1] && resolve(process.argv[1])===self){
 const root=resolve(dirname(self),'..');
 const input=resolve(root,process.argv[2]||'examples/relational-generation/dictionary.json');
 const outputDir=resolve(root,process.argv[3]||'examples/relational-generation/generated');
 const result=generateEfModels(JSON.parse(readFileSync(input,'utf8')));
 mkdirSync(outputDir,{recursive:true});
 for(const file of result.files){
  const target=join(outputDir,file.path.split('/').at(-1));
  writeFileSync(target,file.source,'utf8');
  console.log('Generated '+target);
 }
 for(const warning of result.warnings)console.warn('Clarify warning: '+warning);
}
