import { RequestAsync } from '../core/request.js'; 


$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/Scripts/data.csv",
        dataType: "text",
        success: function (data) { processData(data); },
        error: (e) =>  console.log(e)
    });
});

function processData(allText) { 
    var allTextLines = allText.split(/\r\n|\n/);
    var headers = allTextLines[0].split(',');
    var lines = []; 
    for (var i = 1; i < allTextLines.length; i++) {
        var data = allTextLines[i].split(',');
        if (data.length == headers.length) { 
            var tarr = [];
            for (var j = 0; j < headers.length; j++) {
                let d = {};
                d[headers[j]] = data[j];
                tarr.push(d);
            }
            lines.push(tarr);
        }
    }
    console.log(lines);
}

 
const request = {
    resource: `~DBUtils.aspx/GetDataTable`,
    parms: { PK_FORM: '2022-Q4-CIO' }
} 
 
RequestAsync(request).then(data => {
    //console.log(data); 
    return RequestAsync(request);
}).then(data => {
    console.log(data); 
});

 
 