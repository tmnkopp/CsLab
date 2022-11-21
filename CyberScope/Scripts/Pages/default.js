 
import { DataBinder } from '../core/forms.js';
$(document).ready(async () => {
    let data = { 'foobar': '1111' };
    const binder = new DataBinder({data:data});
    binder.Bind();
  
 
    $('#submitform').on({
        click: (e) => console.log(binder.GetFormVals())
    })
    
});