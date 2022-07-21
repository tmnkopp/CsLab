export const RequestAsync = async (request, uri) => { 
    return await new Promise((resolve, reject) => { 
        $.ajax({
            url: uri,
            type: "POST",
            data: JSON.stringify(request),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (response) => {
                const _json = JSON.parse(response.d); 
                resolve(_json);
            },
            failure: (response) => console.log(response.d),
            error: (response) => console.log(response.d)
        });
    });
}
export const GetDataAsync = async (request) => {
    request = { requests: request };
    let uri = `DBUtils.aspx/GetDataTables`;
    return RequestAsync(request, uri);
}
export const ExportDataAsync = async (request) => {
    request = { requests: request };
    let uri = `DBUtils.aspx/Export`;
    return RequestAsync(request, uri);
}
export const PostRequestAsync = async (request, uri) => {
    return await new Promise((resolve, reject) => {
        $.ajax({
            url: uri,
            type: "POST",
            data: JSON.stringify(request),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (response) => {
                const _json = JSON.parse(response.d);
                resolve(_json);
            },
            failure: (response) => console.log(response.d),
            error: (response) => console.log(response.d)
        });
    });
}

