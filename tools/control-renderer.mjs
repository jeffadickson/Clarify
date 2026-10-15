import {inferControl,validateControl} from '../designer/controls.mjs';
const escapeHtml=value=>String(value).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;').replaceAll('"','&quot;');
export function renderControl(field,config,property,id){
 const kind=config?.kind||inferControl(field);
 const errors=validateControl(field,{...config,kind});
 if(errors.length)throw Error(field.name+': '+errors.join('; '));
 const bind='@bind-Value="Model.'+property+'"';
 const choices=(config?.choices||[]).map(x=>escapeHtml(x));
 const validation=config?.validation||{};
 const attrs=(kind==='text'||kind==='textarea')&&validation.maxLength?' maxlength="'+validation.maxLength+'"':'';
 if(kind==='text')return '<InputText id="'+id+'" '+bind+attrs+' />';
 if(kind==='textarea')return '<InputTextArea id="'+id+'" '+bind+attrs+' />';
 if(kind==='number'){
  if(['money','length','mass','pressure','temperature'].includes(field.type))
   throw Error('Rich quantity editor requires generated typed binding: '+field.type);
  const min=validation.min!==undefined?' min="'+validation.min+'"':'';
  const max=validation.max!==undefined?' max="'+validation.max+'"':'';
  return '<InputNumber id="'+id+'" '+bind+min+max+' />';
 }
 if(kind==='date')return '<InputDate id="'+id+'" '+bind+' />';
 if(kind==='checkbox')return '<InputCheckbox id="'+id+'" '+bind+' />';
 if(kind==='dropdown')return '<InputSelect id="'+id+'" '+bind+'><option value="">Select…</option>'+choices.map(x=>'<option value="'+x+'">'+x+'</option>').join('')+'</InputSelect>';
 if(kind==='radio'||kind==='buttons'){
  const css=kind==='buttons'?'clarify-choice-buttons':'clarify-choice-radio';
  return '<InputRadioGroup '+bind+'><div class="'+css+'" role="group" aria-label="'+escapeHtml(field.name)+'">'+choices.map((x,i)=>'<label><InputRadio TValue="string" Value="@('+JSON.stringify(x)+')" />'+x+'</label>').join('')+'</div></InputRadioGroup>';
 }
 throw Error('Unsupported control '+kind);
}
