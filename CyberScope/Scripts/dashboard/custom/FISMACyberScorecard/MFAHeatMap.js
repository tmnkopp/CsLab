import { RequestAsync } from '../../../core/request.js';
import { Cookie } from '../../../core/utils.js';
 
export function LoadMFAHeatMapData() {
    let request = {
        resource: `Default.aspx/RequestMFAHeatMap`,
        parms: {
            rptcycle: $("#ddlReportingCycles_AvgMFA").val()
            , showcfoactagencies: $("#ddlYesNo_AvgMFA").val()
        }
    }
    RequestAsync(request).then(result => {
        LoadMFAHeatMapChart(result);
    });
}

function LoadMFAHeatMapChart(data) {
    let dataset1 = data[0];
    let dataset2 = data[1];

    var restValOnload = dataset2[0].Rest;
    var transitValOnload = dataset2[0].Transit;
    var mfaValOnload = dataset2[0].MFA;
    LoadGauge(restValOnload, transitValOnload, mfaValOnload);

    $("#chartMFAHeatMap").kendoGrid({
        dataBound: function (e) {
            var restIndex = this.wrapper.find(".k-grid-header [data-field=" + "Rest" + "]").index();
            var TransitIndex = this.wrapper.find(".k-grid-header [data-field=" + "Transit" + "]").index();
            var MFAIndex = this.wrapper.find(".k-grid-header [data-field=" + "MFA" + "]").index();
            var AcronymIndex = this.wrapper.find(".k-grid-header [data-field=" + "Acronym" + "]").index();
            var rows = e.sender.tbody.children();

            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);

                var dataItem = e.sender.dataItem(row);
                var restVal = dataItem.get("Rest");
                var transitVal = dataItem.get("Transit");
                var mfaVal = dataItem.get("MFA");
                var AcVal = dataItem.get("Acronym");

                $(row).children('td:eq(' + AcronymIndex + ')').addClass('Agrows');
                if (restVal > 90) {
                    $(row).children('td:eq(' + restIndex + ')').addClass('greenrows');
                }
                if (restVal >= 70 & restVal <= 90) {
                    $(row).children('td:eq(' + restIndex + ')').addClass('yellowrows');
                }
                if (restVal < 70) {
                    $(row).children('td:eq(' + restIndex + ')').addClass('redrows');
                }

                if (mfaVal > 90) {
                    $(row).children('td:eq(' + MFAIndex + ')').addClass('greenrows');
                }
                if (mfaVal >= 70 & mfaVal <= 90) {
                    $(row).children('td:eq(' + MFAIndex + ')').addClass('yellowrows');
                }
                if (mfaVal < 70) {
                    $(row).children('td:eq(' + MFAIndex + ')').addClass('redrows');
                }

                if (transitVal > 90) {
                    $(row).children('td:eq(' + TransitIndex + ')').addClass('greenrows');
                }
                if (transitVal >= 70 & transitVal <= 90) {
                    $(row).children('td:eq(' + TransitIndex + ')').addClass('yellowrows');
                }
                if (transitVal < 70) {
                    $(row).children('td:eq(' + TransitIndex + ')').addClass('redrows');
                }
            }
        },
        change: function () {
                var gview = $("#chartMFAHeatMap").data("kendoGrid");
                var selected = $.map(this.select(), function (item) {
                    return $(item);
                });
                console.log(selected);
                //var selected = gview.select();
                //console.log(selected);
                LoadGauge(restValOnload, transitValOnload, mfaValOnload);
                if (selected.length > 0) {
                    if (selected[0].hasClass("k-selected")) {
                        var selectedItem = gview.dataItem(gview.select());
                        var selRowRest = selectedItem.Rest;
                        var selRowTransit = selectedItem.Transit;
                        var selRowMFA = selectedItem.MFA;
                        LoadGauge(selRowRest, selRowTransit, selRowMFA);
                    }
                }
        },
        columnMenu: {
            height: 400,
            width: 450
        },
        selectable: true,
        sortable:true,
        columns: [{
            field: "Acronym"
            , title: "Agency"
        },
        {
            field: "Rest"
            , title: "Rest"
        },
        {
            field: "Transit"
            , title: "Transit"
        },
        {
            field: "MFA"
            , title: "MFA"
        }],
        dataSource: {
            data: dataset1
        }
    });


}
function LoadGauge(restVal, transitVal, mfaVal) {
    $("#gaugeRest").kendoArcGauge({
        value: restVal,
        centerTemplate: '#: value #%',
        scale: {
            min: 0,
            max: 100,
            rangeSize: 20,
            labels: {
                visible: true,
                font: "15px Arial, Helvetica, sans- serif"
            }
        },
        colors: [{
            to: 69,
            color: '#C36E6E'
        }, {
            from: 70,
            to: 90,
            color: '#FFFD91'
        }, {
            from: 91,
            to: 100,
            color: '#C0DDA1'
        }]
    });

    $("#gaugeTransit").kendoArcGauge({
        value: transitVal,
        centerTemplate: '#: value #%',
        scale: {
            min: 0,
            max: 100,
            rangeSize: 20,
            labels: {
                visible: true,
                font: "15px Arial, Helvetica, sans- serif"
            }
        },
        colors: [{
            to: 69,
            color: '#C36E6E'
        }, {
            from: 70,
            to: 90,
            color: '#FFFD91'
        }, {
            from: 91,
            to: 100,
            color: '#C0DDA1'
        }]


    });

    $("#gaugeMFA").kendoArcGauge({
        value: mfaVal,
        centerTemplate: '#: value #%',
        scale: {
            min: 0,
            max: 100,
            rangeSize: 17,
            labels: {
                visible: true,
                font: "15px Arial, Helvetica, sans- serif"
            }
        },
        colors: [{
            to: 69,
            color: '#C36E6E'
        }, {
            from: 70,
            to: 90,
            color: '#FFFD91'
        }, {
            from: 91,
            to: 100,
            color: '#C0DDA1'
        }]


    });

}