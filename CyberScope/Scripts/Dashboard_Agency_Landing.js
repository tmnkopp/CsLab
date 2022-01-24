
$(document).ready(function () {
    d3.select('#filter').on('change', Render);
    $(function () {
        var request = {};
        request['SPROC'] = "CISA_CVE_CRUD";
        request['MODE'] = "SELECT";
        var json = JSON.stringify({ request: request });
        $.ajax({
            url: "Landing.aspx/RequestSubmissionHist",
            type: "POST", 
            data: json,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccess,
            failure: function (response) {
                console.log(response.d);
            },
            error: function (response) {
                console.log(response.d);
            }
        });
    });
    function OnSuccess(response) {
        var data = response.d;
        d3.select('#payload').attr('value', data);
        Render();
    } 
    function Render() {
        var data = d3.select('#payload').property('value');
        let obj = JSON.parse(data);
        let filter = d3.select('#filter').property('value'); 
 
        let group = obj.reduce(function (result, item) {
            if (!result[item.Vendor]) {
                result[item.Vendor]=1;
            } else {
                result[item.Vendor]++;
            }
            return result;
        }, {});
         
        if (isNumeric(filter)) { 
            Object.entries(group).forEach(([key, value]) => {
                if (value < parseInt(filter)) {
                    delete group[key];
                }
            });
        }

        var trace1 = {
            x: Object.keys(group),
            y: Object.values(group),
            type: 'bar', marker: { color: 'rgb(100,143,255)', opacity: 1, }
        };
        var data = [trace1];
        Plotly.newPlot('plot', data);

        var trace1 = {
            x: ['foo', 'bar', 'qux'],
            y: [20, 14, 23],
            name: 'Foo',
            type: 'bar', marker: { color: 'rgb(195,215,255)', opacity: 1, }
        }; 
        var trace2 = {
            x: ['foo', 'bar', 'qux'],
            y: [12, 18, 29],
            name: 'Bar',
            type: 'bar', marker: { color: 'rgb(100,143,255)', opacity: 1, }
        };

        var data = [trace1, trace2];
        var layout = { barmode: 'stack' };
        Plotly.newPlot('plot2', data, layout);
        Plotly.newPlot('plot3', data, layout);

        var trace1 = {
            type: 'bar', marker: {
                color: ['rgb(100,143,255)', 'rgb(195, 215, 255)', 'rgb(182, 182, 182)'], opacity: 1, },
            x: [20, 14, 23],
            y: ['foo', 'bar', 'qux'],
            orientation: 'h'
        };

        var data = [trace1];
        Plotly.newPlot('plot4', data);
        Plotly.newPlot('plot5', data);
        Plotly.newPlot('plot6', data);
        Plotly.newPlot('plot7', data);

    } 
    function isNumeric(value) {
        return /^-?\d+$/.test(value);
    }
});
 