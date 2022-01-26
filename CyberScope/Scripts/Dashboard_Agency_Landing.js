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
}
const RenderUpcomingDeadlines = (data) => {
} 
const PrepareRolesPermissions = (data) => {
}
const RenderRolesPermissions = (data) => {
}


const PrepareSubHist = (data) => {
    Distinct('Year', data).sort().reverse().forEach(i => $("#sel_year").append(`<option value=${i}>${i}</option>`));
   $("#sel_year").val('2021').change(() => RenderSubHist(data));
} 
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
 
const RequestDataTable = async (request, successFn = (r)=>r) => {
    return await new Promise((resolve, reject) => {
        const json = JSON.stringify({ request: request }); 
        $.ajax({
            url: `Landing.aspx/RequestDataTable`,
            type: "POST",
            data: json,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (response) => {
                const _json = JSON.parse(response.d);
                successFn(_json);
                resolve(_json);
            },
            failure: (response) => console.log(response.d),
            error: (response) => console.log(response.d)
        });
    });
}
const Distinct = (key, json) => {
    let arr = json.reduce((result, item) => [...result, item[key]], []);
    return [...new Set(arr)];
}
const RequestKey = ({ SPROC, PARMS }) => (SPROC + '_' + JSON.stringify(PARMS).replace(/[^\w]/g, "_")).toLowerCase();