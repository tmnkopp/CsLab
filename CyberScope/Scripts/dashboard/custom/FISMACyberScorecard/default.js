import { RequestAsync } from '../../../core/request.js';
import { Cookie } from '../../../core/utils.js'; 
import { LoadAvgMFAChart, LoadMFARestTrans } from './RTMChart.js';
import { LoadMFAHeatMapData } from './MFAHeatMap.js';

$(document).ready(() => { 
    let request = {
        resource: `Default.aspx/RequestParamData`,
        parms: {  MODE: 'AllParams' }
    }
    RequestAsync(request).then(result => { 
        LoadParms(result); 
        LoadAvgMFAChart(); 
        LoadMFAHeatMapData();
        LoadMFARestTrans();   
    }); 
}); 
function LoadParms(data) {
    data[0].forEach(row => $(".ddlYesNo").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`)); 
    data[1].forEach(row => $(".ddlReportingCycles").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`));
    data[2].forEach(row => $(".ddlAgencies").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`));
    $("#ddlReportingCycles_AvgMFA").change(() => LoadAvgMFAChart());
    $("#ddlYesNo_AvgMFA").change(() => LoadAvgMFAChart()); 
    $("#ddlReportingCycles_MFAHeatMap").change(() => LoadMFAHeatMapData());
    $("#ddlYesNo_MFAHeatMap").change(() => LoadMFAHeatMapData()); 
    $("#ddlReportingCycles_MFARestTrans").change(() => LoadMFARestTrans());
    $("#ddlAgencies_MFARestTrans").change(() => LoadMFARestTrans());
} 