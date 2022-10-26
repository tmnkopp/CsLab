import { RequestAsync } from './core/request.js'; 
import { Cookie } from './core/utils.js';
import { Validator, DataBinder } from './core/forms.js';


$(document).ready(() => {

    let db = new DataBinder({
        data: { name: '25.25.25.25' }
    });
    db.Bind();

    let v = new Validator();
    $('#formsubmit').click(async ()  => {
        let errors = await v.ValidateAsync();
        let frm = db.GetFormVals();
        console.log(frm);
    });
     
})

let request = {
    resource: `~DBUtils.aspx/CbGet`,
    handler:'GetPicklist', 
    parms: { UsageField: 'YN' }
}
RequestAsync(request).then(result => {
    console.log(result);
    request.parms = { UsageField: 'YNNA' } 
    return RequestAsync(request);
}).then(anotherResult => console.log(anotherResult));
