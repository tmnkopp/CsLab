import { RequestAsync } from './core/request.js';

$(document).ready(function () {
    csvReader('/Scripts/data.csv').then(r => {
        let tbl = '';
        r.forEach((r, i) => {
            let inp = `<input id='txt${i}' type='text' data-unit='${r.h3}' >`;
            tbl += `<tr id='row${i}'><td>${inp}</td><td>${r.h2}</td><td>${r.h3}</td></tr>`;
        });
        tbl += `<tr><td><input id='sum' type='text'></td></tr>`;
        $('#grid').html(tbl);
        $("input[id^='txt']").on({
            change: (e) => updatecal(e)
        });
    });
});
const updatecal = (evt) => {
    let tot = 0;
    $("input[id^='txt']").each((i, o) => {
        let unit = $(o).attr('data-unit');
        let val = $(o).val();
        unit = (unit != '') ? parseInt(unit) : 0;
        val = (val != '') ? parseInt(val) : 0;
        tot += (unit * val);
    });
    $('#sum').val(tot);
    $(evt.target).parent().parent().insertBefore("#grid tr:first-child");
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