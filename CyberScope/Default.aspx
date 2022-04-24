<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {
       
            var DataRequestDict = {
                "DataTable1": {
                    SprocName: "CISA_CVE_CRUD",
                    PARMS: {
                        "MODE": "SELECT"
                    } 
                }, "DataTable2": {
                    SprocName: "CISA_CVE_CRUD",
                    PARMS: {
                        "MODE": "SELECT"
                    }
                }
            }

            var json = JSON.stringify({ requests: DataRequestDict });
            $.ajax({
                url: `WebMethod1.aspx/SprocRequest`,
                type: "POST",
                data: json,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: (response) => {
                    var _json = JSON.parse(response.d);
                    console.log(_json);
                },
                failure: (response) => console.log(response.d),
                error: (response) => console.log(response.d)
            });


        });
  
        function postit() {
            var xhr = new XMLHttpRequest();
            xhr.open("POST", 'api/values', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.send(JSON.stringify({
                value: 'asdf'
            }));
        }
 

  
    </script>
    <%= If((HttpContext.Current.IsDebuggingEnabled), "src", "dist") %>
 
</asp:Content>
 
