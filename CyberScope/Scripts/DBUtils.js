import { RequestAsync } from './core/request.js'; 
import { Cookie } from './core/utils.js';
import { Validator } from './core/forms.js';



 
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
