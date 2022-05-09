import { RequestAsync, GetDataAsync } from '../http/request.js';
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

GetDataAsync(DataRequestDict).then(r => {
    console.log(r);
});

 