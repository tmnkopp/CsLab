import { RequestAsync, Environment } from './core/request.js'; 
 
const request = {
    resource: `${Environment.GetBaseUrl()}DBUtils.aspx/GetPicklist`,
    parms: { PK_PickListType: '327' }
} 
 
RequestAsync(request).then(result => {
    console.log(result);

    const anotherRequest = {
        resource: `${Environment.GetBaseUrl()}DBUtils.aspx/GetPicklist`,
        parms: { UsageField: 'YNNA' }
    }
    return RequestAsync(anotherRequest);
}).then(anotherResult => {
    console.log(anotherResult);
});
