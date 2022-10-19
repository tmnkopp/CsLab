import { RequestAsync } from '../../../core/request.js';
import { Cookie } from '../../../core/utils.js'; 
 
$(document).ready(() => { 
    let request = {}
    request = {
        resource: `Default.aspx/RequestParamData`,
        parms: {  MODE: 'AllParams' }
    }
    RequestAsync(request).then(result => { 
        LoadParms(result);
        $("#ddlReportingCycles_MFARestTrans").val();
        LoadAvgMFAChart(); 
        LoadMFAHeatMapData();
        LoadMFARestTrans();

        $("#ddlReportingCycles_AvgMFA").change(() => LoadAvgMFAChart());
        $("#ddlYesNo_AvgMFA").change(() => LoadAvgMFAChart());

        $("#ddlReportingCycles_MFAHeatMap").change(() => LoadMFAHeatMapData());
        $("#ddlYesNo_MFAHeatMap").change(() => LoadMFAHeatMapData());

        $("#ddlReportingCycles_MFARestTrans").change(() => LoadMFARestTrans());
        $("#ddlAgencies_MFARestTrans").change(() => LoadMFARestTrans());

        $("#dialog").hide();
        $("#dialog").click(() => $("#dialog").slideUp("fast"));
    }); 
});

function LoadParms(data) {
    data[0].forEach(row => $(".ddlYesNo").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`)); 
    data[1].forEach(row => $(".ddlReportingCycles").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`));
    data[2].forEach(row => $(".ddlAgencies").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`));
}
function LoadMFARestTrans() {
    let defRepcycle = $("#ddlReportingCycles_MFARestTrans option:first").val();
    let defMajAgency = $("#ddlAgencies_MFARestTrans option:first").val();
    let request = {
        resource: `Default.aspx/RequestMFARestTrans`,
        parms: {
            rptcycle: $("#ddlReportingCycles_MFARestTrans").val() ?? defRepcycle
            , MajAgency: $("#ddlAgencies_MFARestTrans").val() ?? defMajAgency
        }
    }
    RequestAsync(request).then(result => {
        let rest = result[0][0];
        let trans = result[1][0];
        let mfa = result[2][0];
        let drillData = result[3];
        /*   test data
            rest = { "Q1Avg": 22, "Q2Avg": 50, "Q3Avg": 20, "Q4Avg": 10, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
            trans = { "Q1Avg": 2, "Q2Avg": 5, "Q3Avg": 9, "Q4Avg": 23, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
            mfa = { "Q1Avg": 32, "Q2Avg": 10, "Q3Avg": 12, "Q4Avg": 30, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
        */
        CreateRTMCharts({ chartTitle: 'Average Data at Rest Encryption', drillData: drillData, data: rest, drillData: drillData, type: 'rest', chartSelector: '#chartMFARestTrans .rest' });
        CreateRTMCharts({ chartTitle: 'Average Data In Trans Encryption', drillData: drillData, data: trans, drillData: drillData, type: 'trans', chartSelector: '#chartMFARestTrans .trans' });
        CreateRTMCharts({ chartTitle: 'Average MFA Implementation', drillData: drillData, data: mfa, drillData: drillData, type: 'mfa', chartSelector: '#chartMFARestTrans .mfa' });
    });
}

function LoadAvgMFAChart() {
    let request = {
        resource: `Default.aspx/RequestAverageMFA`,
        parms: {
            rptcycle: $("#ddlReportingCycles_AvgMFA").val() ?? '119'
            , showcfoactagencies: $("#ddlYesNo_AvgMFA").val() ?? 'N'
        }
    }
    RequestAsync(request).then(result => {
        let rest = result[0][0];
        let trans = result[1][0];
        let mfa = result[2][0];
        let drillData = result[3];
        /* test data  
            rest = { "Q1Avg": 22, "Q2Avg": 50, "Q3Avg": 90, "Q4Avg": 20, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
            trans = { "Q1Avg": 2, "Q2Avg": 5, "Q3Avg": 9, "Q4Avg": 2, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
            mfa = { "Q1Avg": 32, "Q2Avg": 150, "Q3Avg": 40, "Q4Avg": 120, "Q1SeriesName": "FY22 Q1", "Q2SeriesName": "FY22 Q2", "Q3SeriesName": "FY22 Q3", "Q4SeriesName": "FY22 Q4" }
        */
        CreateRTMCharts({ chartTitle: 'Average Data at Rest Encryption', drillData: drillData, data: rest, type: 'rest', chartSelector: '#chartAvgMFA .rest' });
        CreateRTMCharts({ chartTitle: 'Average Data at Trans Encryption', drillData: drillData,data: trans, type: 'trans', chartSelector: '#chartAvgMFA .trans' });
        CreateRTMCharts({ chartTitle: 'Average Data at MFA Encryption', drillData: drillData, data: mfa, type: 'mfa', chartSelector: '#chartAvgMFA .mfa' });
    });
}
function CreateRTMCharts({ chartTitle = '', chartSelector = '', drillData = {}, data = {}, type = '' }) {
    $(chartSelector).kendoChart({
        chartArea: {  width: '360' },
        title: { text: chartTitle },
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
            }], seriesClick: (e) => LoadRTMDrilldown(e.series.name, type, drillData)
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
function LoadRTMDrilldown(series, type, drillData) {
    let data = drillData.filter(i => i.QtrDesc?.toUpperCase() == series.toUpperCase() && i.EncryptType?.toUpperCase() == type.toUpperCase())
    $("#dialog-caption").append('div').html(`Data Detail ${series} ${type.toUpperCase()}`);
    $("#dialog-data").kendoGrid({
        scrollable: false,
        columns: [{
            field: "identifier_text"
            , title: "Metric"
        }, {
            field: "Answer"
            , title: "Calculation"
        }],
        dataSource: { data: data }
    }); 
    $("#dialog").slideDown("slow");
}
 
function LoadMFAHeatMapData() {
    let request = {
        resource: `Default.aspx/RequestMFAHeatMap`,
        parms: {
            rptcycle: $("#ddlReportingCycles_MFAHeatMap").val() ?? '119'
            , showcfoactagencies: $("#ddlYesNo_MFAHeatMap").val() ?? 'N'
        }
    }
    RequestAsync(request).then(result => { 
        return;
        LoadMFAHeatMapChart(result);
    });
}

function LoadMFAHeatMapChart(data) {
    let dataset1 = data[0];
    let dataset2 = data[1];  
    $("#chartMFAHeatMap").kendoGrid({
        columns: [{
            field: "PK_OrgSubmission"
            , title: "PK_OrgSubmission"
            }, {
            field: "PK_Component"
            , title: "PK_Component"
            }, {
            field: "Acronym"
            , title: "Acronym"
            }, {
            field: "MFA"
            , title: "MFA"
            }, {
            field: "Transit"
            , title: "Transit"
            }, {
            field: "Rest"
            , title: "Rest"
            } , {
            field: "Cycle"
            , title: "Cycle"
            }],
        dataSource: {
            data: dataset1
        }
    });
}

 