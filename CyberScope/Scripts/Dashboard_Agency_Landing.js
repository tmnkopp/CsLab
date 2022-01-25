$(document).ready(() => {
    for (var i = 2022; i > 2010; i--) {
        $("#fy_filter").append(`<option value=${i}>${i}</option>`)
    }
    $("#fy_filter").val('2021').change(() => Render() );

    let request = {
        SPROC: "DashAgency",
        PARMS: {
            MODE: "SELECT" 
        }
    } 
    RequestDataTable( request, (response) => {
        $("#payload").val(response.d);
        Render();
    }); 
});
const Render = () => {
    const response_json = $("#payload").val();
    const response_data = JSON.parse(response_json);
    const fy_filter_val = $("#fy_filter").val();
  
    const ycoords = response_data.filter(i => i.Year == fy_filter_val).reduce((result, item) => {
        if (!result[item.ScheduledActivationQuarter]) {
            result[item.ScheduledActivationQuarter] = { OT: 0, OD: 0 };
        } 
        result[item.ScheduledActivationQuarter].OT += item.ONTIME;
        result[item.ScheduledActivationQuarter].OD += item.OVERDUE;
        return result;
    }, {});
    
    Object.keys(ycoords).forEach(quart => {
        let trace1 = {
            type: 'bar',  marker: { color: ['rgb(100,143,255)', 'rgb(195, 215, 255)', 'rgb(182, 182, 182)'] },
            y: [ycoords[quart].OT, ycoords[quart].OD, ycoords[quart].OT + ycoords[quart].OD],
            x: ['ONTIME', 'OVERDUE', 'TOTAL'] 
        };
        let layout = {
            title: `${fy_filter_val} Q${quart}`, height: 320
        };
        Plotly.newPlot(`plotq${quart}`, [trace1], layout, { displayModeBar: false });
    });
}

const RequestDataTable = (request, successFn, webservice ='RequestDataTable') => {
    const json = JSON.stringify({ request: request });
    $.ajax({
        url: `Landing.aspx/${webservice}`,
        type: "POST",
        data: json,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFn,
        failure: (response) => {
            console.log(response.d);
        },
        error: (response) => {
            console.log(response.d);
        }
    });
}
 