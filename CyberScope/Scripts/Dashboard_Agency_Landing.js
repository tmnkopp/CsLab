$(document).ready(() => { 
    for (var i = 2022; i > 2012; i--) {
        $("#fy_filter").append(`<option value=${i}>${i}</option>`); 
    }
    $("#fy_filter").val('2021');
    $("#fy_filter").change(() => {
        Render();
    });
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
    const response_data = $("#payload").val();
    const response_obj = JSON.parse(response_data);
    const fy_filter_val = $("#fy_filter").val();
    const fy_data = response_obj.reduce((result, item) => {
        if (!result[item.Year]) {
            result[item.Year] = [];
        }
        result[item.Year].push(item);
        return result;
    }, {});
  
    let quart_data = { 1: { OT: 0, OD: 0 }, 2: { OT: 0, OD: 0 }, 3: { OT: 0, OD: 0 }, 4: { OT: 0, OD: 0 } };
    const ycoords = fy_data[fy_filter_val].reduce((result, item) => {
        result[item.ScheduledActivationQuarter].OT += item.ONTIME;
        result[item.ScheduledActivationQuarter].OD += item.OVERDUE;
        return result;
    }, quart_data);

    Object.keys(quart_data).forEach(quart => {
        let trace1 = {
            type: 'bar',
            marker: { color: ['rgb(100,143,255)', 'rgb(195, 215, 255)', 'rgb(182, 182, 182)'], opacity: 1, },
            y: [ycoords[quart].OT, ycoords[quart].OD, ycoords[quart].OT + ycoords[quart].OD],
            x: ['ONTIME', 'OVERDUE', 'TOTAL'],
            width: [.9, .9, .9]
        };
        let layout = {
            title: `${fy_filter_val} Q${quart}`, height: 320
        };
        Plotly.newPlot(`plotq${quart}`, [trace1], layout, { displayModeBar: false });
    });
}

const RequestDataTable = (request, successFn, webservice ='RequestDataTable') => {
    var json = JSON.stringify({ request: request });
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
 