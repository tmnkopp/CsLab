const rgbp = 'rgb(100, 143, 255)'
const rgbs = 'rgb(195, 215, 255)'
const rgbt = 'rgb(182, 182, 182)'


$(document).ready(async () => {
 
    const sub_his_quart_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {
            MODE: "SUB_HIS_QUARTER"
        }
    });
    const sub_his_mo_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {
            MODE: "SUB_HIS_MONTH"
        }
    });
    const sub_his_data = {
        quart: sub_his_quart_data
        , mo: sub_his_mo_data
    }
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
    Distinct('Year', data.quart).sort().reverse().forEach(i => $("#sel_year").append(`<option value=${i}>FY ${i}</option>`));
    $("#sel_year").val('2021').change(() => RenderSubHist(data));
    $("#sel_interval").change(() => { 
        $('#plot_mo').show();
        $('#plot_quart').show();
        if ($("#sel_interval").val() == 'quart') $('#plot_mo').hide();
        if ($("#sel_interval").val() == 'mo') $('#plot_quart').hide();
    });
}
/* Sub Hist Module */
const RenderSubHist = (data) => { 
    const sel_year_val = $("#sel_year").val();  
    const quart_data = data.quart.filter(i => i.Year == sel_year_val);
    const mo_data = data.mo.filter(i => i.Year == sel_year_val);
     
    const mo_x = mo_data.map((i) => i.ScheduledActivationMonth);
    const mo_ot = mo_data.map((i) => i.ONTIME);
    const mo_od = mo_data.map((i) => i.OVERDUE);
    const mo_tot = mo_data.map((i) => i.TOTAL);
    let trace1 = {
        y: mo_ot, x: mo_x, type: 'bar', mode: 'lines', marker: { color: rgbs, width: 4  }, name: 'ONTIME',
    };
    let trace2 = {
        y: mo_od, x: mo_x, type: 'bar', mode: 'lines', marker: { color: rgbt, width: 4  }, name: 'OVERDUE',
    };
    let trace3 = {
        y: mo_tot, x: mo_x, type: 'bar', mode: 'lines', marker: { color: rgbp, width: 4 }, name: 'TOTAL',
    };
    var layout = { barmode: 'group' };
    Plotly.newPlot(`plot_mo`, [trace1, trace2, trace3], layout,{ displayModeBar: false });
   
    $(`div[id^='plotq']`).html('<h5 style="margin:25%;">NO DATA</h5>');
    Object.entries(quart_data).forEach(([k, v]) => {
        $(`div[id^='plotq${v.ScheduledActivationQuarter}']`).html('');
        let trace1 = {
            type: 'bar', marker: { color: [rgbs, rgbt, rgbp] },
            y: [v.ONTIME, v.OVERDUE, v.TOTAL],
            x: ['ONTIME', 'OVERDUE', 'TOTAL'] 
        };
        let layout = {
            title: `${sel_year_val} Q${v.ScheduledActivationQuarter}`, height: 320
        };
        Plotly.newPlot(`plotq${v.ScheduledActivationQuarter}`, [trace1], layout, { displayModeBar: false });
    });  
}
 