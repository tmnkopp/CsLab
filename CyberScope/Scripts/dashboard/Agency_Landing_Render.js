$(document).ready(async () => {
    const request = {
        SPROC: "DashAgency", 
        PARMS: {
            MODE: "SELECT"
        }
    } 
    const sub_his_data = await RequestDataTable(request); 
    PrepareSubHist(sub_his_data);
    RenderSubHist(sub_his_data);
});

const PrepareUpcomingDeadlines = (data) => {
    /* Upcoming Deadlines Module */
}
const RenderUpcomingDeadlines = (data) => {
    /* Upcoming Deadlines Module */
} 
const PrepareRolesPermissions = (data) => {
    /* Permissions Module*/
}
const RenderRolesPermissions = (data) => {
    /* Permissions Module*/
}

/* Sub Hist Module */
const PrepareSubHist = (data) => {
    Distinct('Year', data).sort().reverse().forEach(i => $("#sel_year").append(`<option value=${i}>${i}</option>`));
   $("#sel_year").val('2021').change(() => RenderSubHist(data));
}
/* Sub Hist Module */
const RenderSubHist = (data) => {
  
    const sel_year_val = $("#sel_year").val(); 
    if (!/^20\d{2}$/.test(sel_year_val)) return; 
    const quart_data = data.filter(i => i.Year == sel_year_val).reduce((result, item) => {
        if (!result[item.ScheduledActivationQuarter]) {
            result[item.ScheduledActivationQuarter] = { OT: 0, OD: 0 };
        } 
        result[item.ScheduledActivationQuarter].OT += item.ONTIME;
        result[item.ScheduledActivationQuarter].OD += item.OVERDUE;
        return result;
    }, {});
     
    $(`div[id^='plotq']`).html('<h5 style="margin:25%;">NO DATA</h5>');
    Object.entries(quart_data).forEach(([k, v]) => {
        $(`div[id^='plotq${k}']`).html('');
        let trace1 = {
            type: 'bar',  marker: { color: ['rgb(100,143,255)', 'rgb(195, 215, 255)', 'rgb(182, 182, 182)'] },
            y: [v.OT, v.OD, v.OT + v.OD],
            x: ['ONTIME', 'OVERDUE', 'TOTAL'] 
        };
        let layout = {
            title: `${sel_year_val} Q${k}`, height: 320
        };
        Plotly.newPlot(`plotq${k}`, [trace1], layout, { displayModeBar: false });
    }); 
}
 