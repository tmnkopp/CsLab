import { RequestAsync } from '../core/request.js';
 
 
 
$(document).ready(async () => { 
    const request = {
        resource: `~DBUtils.aspx/RequestPicklist`, 
        parms: { MODE: 'YN' }
    }
    RequestAsync(request).then(data => {
        console.log(data);  
    });
})
 