import { RequestAsync } from '../../../core/request.js';
import { stripScript, PDFExporter } from '../../../core/utils.js';
import { LoadAvgMFAChart, LoadMFARestTrans } from './RTMChart.js';
import { LoadMFAHeatMapData } from './MFAHeatMap.js';

$(document).ready(() => { 
    let request = {
        resource: `Default.aspx/RequestParamData`,
        parms: {  MODE: 'AllParams' }
    }
    RequestAsync(request).then(result => { 
        Init(result); 
        LoadAvgMFAChart(); 
        LoadMFAHeatMapData();
        LoadMFARestTrans();
        FilterChange();
        document.title = result[3][0].ShortName;
    }).catch(r => console.error(r));
}); 
function Init(data) {
    data[0].forEach(row => $(".ddlYesNo").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`)); 
    data[1].forEach(row => $(".ddlReportingCycles").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`));
    data[2].forEach(row => $(".ddlAgencies").append(`<option value=${row["DataValue"]}>${row["DataText"]}</option>`));
    $("#ddlReportingCycles_AvgMFA").change(() => LoadAvgMFAChart());
    $("#ddlYesNo_AvgMFA").change(() => LoadAvgMFAChart()); 
    $("#ddlReportingCycles_MFAHeatMap").change(() => LoadMFAHeatMapData());
    $("#ddlYesNo_MFAHeatMap").change(() => LoadMFAHeatMapData()); 
    $("#ddlReportingCycles_MFARestTrans").change(() => LoadMFARestTrans());
    $("#ddlAgencies_MFARestTrans").change(() => LoadMFARestTrans());
    $("select[id^='ddl']").change(() => FilterChange());  
    $("#modal").click(() => $("#modal").slideUp()).hide(); 
    $("#btnExport").click(() => ExportPDF())
    $('#FullDescContainer').html(data[3][0].FullDescription)
}
function FilterChange() {
    $("#modal").slideUp();
    $("#AvgMFAHeader .postfix").html(`: ${$("#ddlReportingCycles_AvgMFA option:selected").text()} ${$("#ddlYesNo_AvgMFA option:selected").text()}`);
    $("#MFAHeatMapHeader .postfix").html(`: ${$("#ddlReportingCycles_MFAHeatMap option:selected").text()} ${$("#ddlYesNo_MFAHeatMap option:selected").text()}`);
    $("#MFARestTransHeader .postfix").html(`: ${$("#ddlReportingCycles_MFARestTrans option:selected").text()} ${$("#ddlAgencies_MFARestTrans option:selected").text()}`);
}
function ExportPDF() {
    FilterChange();
    var d = new Date();
    var strDate = (d.getMonth() + 1) + "/" + d.getDate() +"/" + d.getFullYear();
    var obj = $(".FISMACyberScorecard-wrapper");
    //obj.find("span:contains('Unclassified')").remove();
    var official = "<p class=\"delete\" style=\"color:#ff0000; text-align:center; font-weight: bold\">Unclassified // For Official Use Only</p>";
    obj.prepend("<div class=\"row delete\" style=\"background-color:#112e51;color:white;padding:5px;margin-bottom:2px;\"><div class=\"col-md-3\" > <img class=\"navbar-brand\" src=\"../../../Images/dhs-lg_cyberscope.png\"  style=\"border-width:0px;height:100px; \" /></div><div class=\"col-md-6\" style=\"display:inline-block; color:white; text-align:center;\"><h2 style=\"line-height:100px; font-weight:bold; text-align:center; margin:auto;\">FISMA Cybersecurity Scorecard</h2></div><div class=\"col-md-3\"><span style=\"display:table-cell; height:100px; vertical-align:bottom;\">DATE CREATED: " + stripScript(strDate) + "</span></div></div>");
    obj.prepend(official);
    
    $('.navbar , #btnExport, .param-wrapper').hide();
    PDFExporter.ExportAsync({
        container: 'body', filename: document.title, wait: 1200
    }).then((result) => { 
        $('.delete').remove();
        $('.navbar, #btnExport, .param-wrapper').show();
    });
}
 