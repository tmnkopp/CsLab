
$(document).ready(function () {
    d3.select('#filter').on('change', Render);
    $(function () {
        var request = {};
        request['SPROC'] = "CISA_CVE_CRUD";
        request['MODE'] = "SELECT";
        var json = JSON.stringify({ request: request });
        $.ajax({
            url: "BOD_2201_Admin.aspx/GetDataDict",
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
        //obj.forEach(e => {  console.log(e) }); 
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
            type: 'bar',
            name: 'Secondary Product',
            marker: { color: 'rgb(149,130,89)', opacity: 0.7, }
        };
        var data = [trace1];
        Plotly.newPlot('plot', data);
    } 
    function isNumeric(value) {
        return /^-?\d+$/.test(value);
    }
});
 