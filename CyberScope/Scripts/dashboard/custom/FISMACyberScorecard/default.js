import { RequestAsync } from '../../../core/request.js';
import { PDFExporter } from '../../../core/utils.js';
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
        document.title = 'FISMA Cybersecurity Scorecard';
    }); 
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
}
function FilterChange() {
    $("#modal").slideUp();
    $("#AvgMFAHeader .postfix").html(`: ${$("#ddlReportingCycles_AvgMFA option:selected").text()} ${$("#ddlYesNo_AvgMFA option:selected").text()}`);
    $("#MFAHeatMapHeader .postfix").html(`: ${$("#ddlReportingCycles_MFAHeatMap option:selected").text()} ${$("#ddlYesNo_MFAHeatMap option:selected").text()}`);
    $("#MFARestTransHeader .postfix").html(`: ${$("#ddlReportingCycles_MFARestTrans option:selected").text()} ${$("#ddlAgencies_MFARestTrans option:selected").text()}`);
}
function ExportPDF() {
    FilterChange();
    $(".param-wrapper").hide(); 
    var d = new Date();
    var strDate = d.getFullYear() + "/" + (d.getMonth() + 1) + "/" + d.getDate();
    var obj = $(".FISMACyberScorecard-wrapper"); 
    var official = "<p class=\"delete\" style=\"color:#ff0000; text-align:center; font-weight: bold\">Unclassified // For Official Use Only</p>";
    obj.prepend("<div class=\"row delete\" style=\"background-color:#112e51;color:white;padding:5px;margin-bottom:2px;\"><div class=\"col-md-3\" > <img class=\"navbar-brand\" src=\"../../../Images/dhs-lg_cyberscope.png\"  style=\"border-width:0px;height:100px; \" /></div ><div class=\"col-md-6\" style=\"color:white;\"><h2 style=\"display:table-cell;height:100px;vertical-align: middle;\">FISMA Cybersecurity Scorecard</h2></div><div class=\"col-md-3\"><span style=\"display:table-cell;height:100px;vertical-align: bottom;\">Date created: " + strDate + "</span></div></div >");
    obj.prepend(official);
    obj.append(official);
    $('.navbar, #btnExport').hide();
    PDFExporter.ExportAsync({
        container: '.FISMACyberScorecard-wrapper', filename:document.title, wait:200
    }).then((result)=>{
        console.log(result);
        $('.delete').remove();
        $('.navbar, #btnExport').show();
    });
}
 