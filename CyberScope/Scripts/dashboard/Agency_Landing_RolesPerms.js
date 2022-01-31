 
$(document).ready(async () => { 
   

    const access_code_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {
            MODE: "ACCESS_CODES"
        }
    });

    const data = {
        upd: access_code_data
    }

    RenderAccessCodes(data);

    const agency_lead_poc_data = await RequestDataTable({
        SPROC: "DashAgency",
        PARMS: {
            MODE: "AGENCY_LEAD_POC"
        }
    });

    const pocdata = {
        upd: agency_lead_poc_data
    }

    RenderAgencyLeadPOCs(pocdata);

}); 
 
const RenderAccessCodes = (data) => {   
    data = data.upd;
    $(".access-wrapper .k-grid").html('<div id="access-code-grid"></div>');
    $("#access-code-grid").kendoGrid({
        columns: [{
            field: "AccessCode"
            , title: "Access Code"
        }, {
            field: "ApprovedDate"
            , title: "Approved Date"
            }
        ],
        dataSource: {
            data: data
        }
    });
}


const RenderAgencyLeadPOCs = (data) => {
    data = data.upd;
    $(".poc-wrapper .k-grid").html('<div id="agency-lead-poc-grid"></div>');
    $("#agency-lead-poc-grid").kendoGrid({
        columns: [
            {
            field: "Name"
            , title: "POC Name"
            },
            {
            field: "Email"
            , title: "Email"
            },
            {
                field: "WorkPhone"
                , title: "Phone"
            }
        ],
        dataSource: {
            data: data
        }
    });
}