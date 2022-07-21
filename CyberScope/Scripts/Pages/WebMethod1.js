import { RequestAsync, GetDataAsync, ExportDataAsync } from '../http/request.js';
var DataRequestDict = {
    "CISA_CVE_CRUD_SEL": {
        SprocName: "CISA_CVE_CRUD",
        PARMS: {
            "MODE": "SELECT"
        }
    }, "CISA_CVE_CRUD_EXP": {
        SprocName: "CISA_CVE_CRUD",
        PARMS: {
            "MODE": "EXPORT"
        }
    }
}

ExportDataAsync(DataRequestDict).then(r => {
    console.log(r);
});

 