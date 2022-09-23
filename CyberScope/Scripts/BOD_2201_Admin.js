function cmdZeroTouch_click(sender, args) {
    var mess = 'Are you sure you would like to Submit Automated Reporting Agencies?';
    if (confirm(mess)) {
        return true;
    }
    else {
        args.set_cancel(true);
    }
}
 
$(document).ready(function () { 
    var response = $('input[id$="_hidResponse"]').val(); 
    var status = $('div[id$="wrapper-status"]');
    var tabindex = 0;
    var statusText = '';
    if (response != '') {
        response = stripScript(response);
        var json = JSON.parse(response);
        if (json.event == 'btn_CVEimport_Click') { 
            statusText += '<br><strong>CVE Updated</strong> ';
            statusText += '<br>Prior Count: ' + json.preCount;
            statusText += '<br>Updated Catalog Version: ' + json.cisaKevFeed.catalogVersion;
            statusText += '<br>Updated CVE Count: ' + json.cisaKevFeed.count;
            statusText += '<br>Release Date: ' + json.cisaKevFeed.dateReleased;
        }
        if (json.event == 'cmdZeroTouch_Click') {
            tabindex = json.tab;
            statusText += '<br>Submissions Update: ' + json.message;
        }
        status.html(statusText); 
    }
    $("#tabstrip").kendoTabStrip().data("kendoTabStrip").select(tabindex);

    var ztdata = $('input[id$="ZeroTouchData"]').val();
    if (ztdata != '') {
        ztdata = stripScript(ztdata);
        ztdata = JSON.parse(ztdata);
        $("#wrapper-zt-data").kendoGrid({
            filterable: true,
            columns: [{
                field: "Component",
                title: "Agency", 
            }, {
                field: "OrgSub_Status"
                , title: " Subission Status"
            }, {
                field: "enableZeroTouch"
                , title: "Automated Reporting Enabled"
            }, {
                field: "ScheduledClosed"
                , title: "ScheduledClosed"
            }],
            dataSource: {
                data: ztdata
            }, 
        }); 
    } 
}); 