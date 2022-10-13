import { RequestAsync } from '../core/request.js'; 


$(document).ready(function () {
    csvReader('/Scripts/data.csv').then(r => {
        let tbl = '';
        r.forEach((r, i) => {
            let inp = `<input id='txt${i}' type='text' data-unit='30' >`;
            tbl += `<tr><td>${inp}</td><td>${r.h2}</td><td>${r.h3}</td></tr>`;
        });
        $('#grid').html(tbl);
        $("input[type='text']").on({
            change: (e) => { 
                updatecal(e);
            }
        }); 
    }); 
});
const updatecal = (obj) => {
    let tot = 0
    $("input[type='text']").each((i, o) => {
        let unit = $(o).attr('data-unit');
        let val = $(o).val(); 
        unit = (unit != '') ? parseInt(unit): 0;
        val = (val != '') ? parseInt(val) : 0;
        tot += (unit * val); 
    });
    console.log(tot);
}

const csvReader = async (src) => {
    const process = (allText) => {
        var allTextLines = allText.split(/\r\n|\n/);
        var headers = allTextLines[0].split(',');
        var lines = [];
        for (var i = 1; i < allTextLines.length; i++) {
            var data = allTextLines[i].split(',');
            if (data.length == headers.length) {
                let d = {};
                for (var j = 0; j < headers.length; j++) {
                    d[headers[j]] = data[j];
                }
                lines.push(d);
            }
        }
        return lines;
    }
    return await new Promise((resolve) => {
        $.ajax({
            type: "GET",
            url: "/Scripts/data.csv",
            dataType: "text",
            success: (response) => {
                let obj = process(response);
                resolve(obj);
            }
        });
    }); 
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

 
 