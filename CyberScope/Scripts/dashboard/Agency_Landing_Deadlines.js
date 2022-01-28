 
$(document).ready(async () => { 
   
    const up_dead_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {
            MODE: "UP_DEAD"
        }
    });
    const data = {
          upd: up_dead_data
    } 
    RenderUpcomingDeadlines(data); 
    Distinct('RC_Description', data.upd).forEach(i => $("#sel_datacall").append(`<option value="">${i}</option>`));
    $("#sel_datacall").change(() => RenderSubHist(data));
}); 
 
const RenderUpcomingDeadlines = (data) => {  
    data = data.upd;  
    $("#up-dead-grid").kendoGrid({
        columns: [{
            field: "Url",
            title: "Url",
            template : '<a href=\"#=Url#\" target="_blank">LINK</a>'  
        },{
            field: "RC_Description"
            , title: "Description" 
        },{
            field: "RC_ScheduledClosed"
            , title: "ScheduledClosed" 
        },{
            field: "RCC_Status_Code"
            , title: "Status Code" 
        }],
        dataSource: {
            data: data
        }
    });    
}   