<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 
    <input type="button" value="getit" id="getit" /> 
    <script type="text/javascript"> 

        $(document).ready(function () {
            $('#getit').click(function () {
                requestData() ;
            });
     
        });
         
        function requestData() {
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
                }
            });
        } 
  
    </script>
    <%= If((HttpContext.Current.IsDebuggingEnabled), "src", "dist") %>
 
</asp:Content>
 
