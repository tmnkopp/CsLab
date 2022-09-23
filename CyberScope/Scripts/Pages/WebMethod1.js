import { RequestAsync } from '../http/request.js';
import { Environment } from '../core/utils.js';
 
const request = {
    resource: `${Environment.GetBaseUrl()}DBUtils.aspx/GetDataTable`,
    parms: { PK_FORM: '2022-Q4-CIO' }
} 
 
RequestAsync(request).then(data => {
    //console.log(data); 
    return RequestAsync(request);
}).then(data => {
    console.log(data); 
});

 
 