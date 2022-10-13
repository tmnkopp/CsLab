import { RequestAsync } from './core/request.js'; 
import { Cookie } from './core/utils.js';
import { Validator } from './core/forms.js';

const validator = new Validator({ target: ' form div ' });
$('form').prepend('<div id="err"><div>');
$('#formsubmit').on({
    click: async (e) => {
        $('#err').html('');
        await validator.ValidateAsync().then(r => { 
            const isValid = validator.isValid; 
            $('#err').html(`isValid: ${isValid}<br> ${r.Errors.map(i => i.message).join('<br>')}`);
        }); 
    }
});


 
let request = {
    resource: `~DBUtils.aspx/GetPicklist`,
    parms: { UsageField: 'CSP' }
}
RequestAsync(request).then(result => {
    console.log(result);
    request = {
        resource: `~DBUtils.aspx/GetPicklist`,
        parms: { UsageField: 'YNNA' }
    }
    return RequestAsync(request);
}).then(anotherResult => {
    console.log(anotherResult);
}).catch(err => {
    console.log(err);
});
