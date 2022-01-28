$(document).ready(() => {
    $('[data-toggle="tooltip"]').tooltip();
}); 

const RequestDataTable = async (request, successFn = (r)=>r) => {
    return await new Promise((resolve, reject) => {
        const json = JSON.stringify({ request: request }); 
        $.ajax({
            url: `Landing.aspx/RequestDataTable`,
            type: "POST",
            data: json,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (response) => {
                const _json = JSON.parse(response.d);
                successFn(_json);
                resolve(_json);
            },
            failure: (response) => console.log(response.d),
            error: (response) => console.log(response.d)
        });
    });
}
const RenderGrid = (data, gridid) => { 
    let container = document.getElementById(gridid);
    let tr = document.createElement("tr");
    container.appendChild(tr);
    Object.entries(data[0]).forEach(([k, v]) => {
        let td = document.createElement("td");
        td.innerHTML = k;
        tr.appendChild(td);
    });
    data.forEach(r => {
        let tr = document.createElement("tr");
        container.appendChild(tr);
        Object.entries(r).forEach(([k, v]) => {
            let td = document.createElement("td");
            td.innerHTML = v;
            tr.appendChild(td);
        });
    }); 
}
const Distinct = (key, json) => {
    let arr = json.reduce((result, item) => [...result, item[key]], []);
    return [...new Set(arr)];
} 
const range = (start, end) => {
    const length = end - start;
    return Array.from({ length }, (_, i) => start + i);
}
const RequestKey = ({ SPROC, PARMS }) => (SPROC + '_' + JSON.stringify(PARMS).replace(/[^\w]/g, "_")).toLowerCase();

const rgbp = 'rgb(100, 143, 255)', rgbs = 'rgb(195, 215, 255)', rgbt = 'rgb(182, 182, 182)'; 
const mo_x = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']; 

 