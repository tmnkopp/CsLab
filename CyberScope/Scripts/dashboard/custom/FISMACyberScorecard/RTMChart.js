import { RequestAsync } from '../../../core/request.js';
export class RTMChart{
    constructor(
        {
            container = '#RTMChart',
            subplots = [],
            OnSeriesClick = () => { } 
        } = {}
    ){
        this.container = container;
        this.subplots = subplots;
        this.data = {}; 
        this.OnSeriesClick = OnSeriesClick;
    }
    render(data){
        this.data=data;   
        $( this.container ).html(`
            <div class="col-4 rest"></div>
            <div class="col-4 trans"></div>
            <div class="col-4 mfa"></div>
        `);  
        let dummydata = { "Q1Avg": 22, "Q2Avg": 50, "Q3Avg": 20, "Q4Avg": 10, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
        this.subplots.forEach((sp, i) => {
            let data = this.data[i][0]; 
            sp.data = data;
            this._plotChart(sp); 
        });  
    }
    _plotChart(subplot) {
        let OnSeriesClickArgs = {
            type: subplot.type,
            subplot:subplot
        };
        const data = subplot.data;
        $(`${this.container} .${subplot.type}`).kendoChart({ 
            chartArea: {  width: '360' },
            title: { text: subplot.title },
            legend: {  position: "bottom"  },
            seriesDefaults: {   type: "column"  },
            series: [{
                name: data.Q1SeriesName,
                data: [data.Q1Avg],
                color: "rgb(100, 143, 255)"
            }, {
                name: data.Q2SeriesName,
                data: [data.Q2Avg],
                color: "rgb(195, 215, 255)"
            }, {
                name: data.Q3SeriesName,
                data: [data.Q3Avg],
                color: "rgb(182, 182, 182)"
            }, {
                name: data.Q4SeriesName,
                data: [data.Q4Avg],
                color: "rgb(255,165,0)"
                }], seriesClick: (e) => {
                    OnSeriesClickArgs.e=e;
                    this.OnSeriesClick(this, OnSeriesClickArgs)     
                }
            ,
            valueAxis: {
                labels: {  format: "{0}%" },
                line: { visible: false  },
                axisCrossingValue: 0
            },
            categoryAxis: {
                line: {  visible: false  },
                labels: { padding: { top: 135 } }
            } 
        });
    } 
}

const RTMSeriesClick = (sender, args)=>{   
    let e = args.e;
    $("#modal").html('');
    $(`<div></div>`).kendoGrid({
        dataSource: {
            data: sender.data[3].filter(i => i.QtrDesc?.toUpperCase() == e.series.name.toUpperCase() && i.EncryptType?.toUpperCase() == args.type.toUpperCase())
        },
        scrollable: false,
        columns: [{
            field: "identifier_text"
            , title: "Metric"
        }, {
            field: "Answer"
            , title: "Calculation"
        }]
    }).appendTo($("#modal"));

    $("#modal").kendoWindow({
        actions: ["Maximize", "Close"],
        draggable: true,
        resizable: true,
        width: "70%",
        visible: false 
    });
    $("#modal").data("kendoWindow").center().open(); 
}
 
export const LoadMFARestTrans = () => {
    let chart = new RTMChart({ container: '#chartMFARestTrans' });
    chart.subplots.push({ title: 'Average Data at Rest Encryption', type: 'rest' });
    chart.subplots.push({ title: 'Average Data at Trans Encryption', type: 'trans' });
    chart.subplots.push({ title: 'Average Data at MFA Encryption', type: 'mfa' }); 
    chart.OnSeriesClick = RTMSeriesClick;
    RequestAsync({
        resource: `Default.aspx/RequestMFARestTrans`,
        parms: {
            rptcycle: $("#ddlReportingCycles_MFARestTrans").val()
            , MajAgency: $("#ddlAgencies_MFARestTrans").val()
        }
    }).then(data => chart.render(data));
}
export const LoadAvgMFAChart = () => {
    let chart = new RTMChart({ container: '#chartAvgMFA' });
    chart.subplots.push({ title: 'Average Data at Rest Encryption', type: 'rest' });
    chart.subplots.push({ title: 'Average Data at Trans Encryption', type: 'trans' });
    chart.subplots.push({ title: 'Average Data at MFA Encryption', type: 'mfa' });
    chart.OnSeriesClick = RTMSeriesClick;
    RequestAsync({
        resource: `Default.aspx/RequestAverageMFA`,
        parms: {
            rptcycle: $("#ddlReportingCycles_AvgMFA").val()
            , showcfoactagencies: $("#ddlYesNo_AvgMFA").val()
        }
    }).then(data => chart.render(data));
}
 