import { RequestAsync } from '../core/request.js'; 
import { DataBinder } from '../core/forms.js';

 
const viewModel = {
    txt1: 'textarea 1',
    rad1: 'bar',
    name: ' bind',
    select1: '1',
    chk1: '444'
}
let db = new DataBinder({ data: viewModel });
$(document).ready(() => { 
    db.Bind();
}); 
$('#submitform').on({
    click: (e) => console.log(db.GetForm()) 
}) 
$('#getit').on({
    click: (e) => {
        const request = {
            resource: `~DBUtils.aspx/GetDataTable`,
            parms: { PK_FORM: '2022-Q4-CIO' }
        }
        RequestAsync(request).then(data => {
            //console.log(data); 
            return RequestAsync(request);
        }).then(data => {
            //console.log(data); 
        });
    } 
})


  


 
 