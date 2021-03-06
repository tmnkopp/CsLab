$(document).ready(async () => {  
    const sub_his_quart_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {  MODE: "SUB_HIS_QUARTER"  }
    });
    const sub_his_mo_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {   MODE: "SUB_HIS_MONTH" }
    }); 
    const data = {
        quart: sub_his_quart_data
        , mo: sub_his_mo_data 
    }
    PrepareSubHist(data);
    RenderSubHist(data); 
}); 
 
const PrepareSubHist = (data) => { 
    Distinct('Year', data.quart).sort().reverse().forEach(i => $("#sel_year").append(`<option value=${i}>FY ${i}</option>`));
    $("#sel_year").change(() => RenderSubHist(data));
    $("#sel_interval").change(() => { 
        $('#plot_mo').hide();
        $('#plot_quart').hide();
        $('#plot_' + $("#sel_interval").val()).show();
    }); 
} 
 
const RenderSubHist = (data) => { 
    const sel_year_val = $("#sel_year").val();  
    const quart_data = data.quart.filter(i => i.Year == sel_year_val);
    const mo_data = data.mo.filter(i => i.Year == sel_year_val);

    const plot_mo_data = []; 
    range(0, 12).forEach(i => {
        let row = mo_data.find(d => d.ScheduledActivationMonth-1 == i);
        plot_mo_data[i] = (row) ? row : { Year: sel_year_val, ScheduledActivationMonth: i, ONTIME: 0, OVERDUE: 0, TOTAL: 0 };
    }); 
 
    const mo_ot = plot_mo_data.map((i) => i.ONTIME);
    const mo_od = plot_mo_data.map((i) => i.OVERDUE);
    const mo_tot = plot_mo_data.map((i) => i.TOTAL);
     
    let trace1 = {  y: mo_ot, x: mo_x, type: 'bar', mode: 'lines', marker: { color: rgbs  }, name: 'ONTIME'  };
    let trace2 = {  y: mo_od, x: mo_x, type: 'bar', mode: 'lines', marker: { color: rgbt  }, name: 'OVERDUE' };
    let trace3 = {  y: mo_tot, x: mo_x, type: 'bar', mode: 'lines', marker: { color: rgbp }, name: 'TOTAL'  };
    let layout = { xaxis: { tickangle: -45, tickmode: "linear", tick0: '1', dtick: 1 }, barmode: 'group' }; 
    Plotly.newPlot(`plot_mo`, [trace1, trace2, trace3], layout,{ displayModeBar: false });
     
    $(`div[id^='plotq']`).html('<h5 style="margin:25%;">NO DATA</h5>');
    Object.entries(quart_data).forEach(([k, v]) => {
        $(`div[id^='plotq${v.ScheduledActivationQuarter}']`).html('');
        let trace1 = {
            type: 'bar', marker: { color: [rgbs, rgbt, rgbp] },
            y: [v.ONTIME, v.OVERDUE, v.TOTAL],  x: ['ONTIME', 'OVERDUE', 'TOTAL'] 
        };
        let layout = {  title: `${sel_year_val} Q${v.ScheduledActivationQuarter}`, height: 320 };
        Plotly.newPlot(`plotq${v.ScheduledActivationQuarter}`, [trace1], layout, { displayModeBar: false });
    });  
} 