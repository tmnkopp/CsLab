$(document).ready(() => { 
    RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {
            MODE: "SELECT"
        }
    }, (response) => {
        const years = JSON.parse(response.d).reduce((result, item) => [...result, item.Year], []);
        [...new Set(years)].sort().reverse().forEach(i => $("#fy_filter").append(`<option value=${i}>${i}</option>`))
    });

    let DataRequest = {
        SPROC: "DashAgency",
        PARMS: {
            MODE: "SELECT" 
        }
    } 
    RequestDataTable(DataRequest, (response) => RenderPage(response.d));

    $("#fy_filter").val('2021').change(() => RenderPage());
});
 
const RenderPage = (_json = $("#_data_cache").val()) => {
    $("#_data_cache").val(_json);
    const response_data = JSON.parse(_json);
    const fy_filter_val = $("#fy_filter").val();

    if (!/^20\d{2}$/.test(fy_filter_val)) return;

    const quart_data = response_data.filter(i => i.Year == fy_filter_val).reduce((result, item) => {
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
            title: `${fy_filter_val} Q${k}`, height: 320
        };
        Plotly.newPlot(`plotq${k}`, [trace1], layout, { displayModeBar: false });
    }); 
}

 
const RequestDataTable = (request, successFn, webservice = 'RequestDataTable') => {  
    const json = JSON.stringify({ request: request });
    $.ajax({
        url: `Landing.aspx/${webservice}`,
        type: "POST",
        data: json,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: (response) => { 
            successFn(response);
        },
        failure: (response) => {
            console.log(response.d);
        },
        error: (response) => {
            console.log(response.d);
        }
    });
}
const RequestKey = ({ SPROC, PARMS }) => (SPROC + '_' + JSON.stringify(PARMS).replace(/[^\w]/g, "_")).toLowerCase();
const Distinct = (key, json) => { 
    let result = JSON.parse(json).reduce((result, item) => [...result, item[key], []);
    return [...new Set(result)]; 
}