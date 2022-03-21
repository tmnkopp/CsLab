<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="WebMethod1.aspx.vb" Inherits="CyberScope.WebMethod1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

 
    <input  type="text" id="cEmpId" /><br />
    <input  type="text" id="cEmpName" /><br />
    <button type="button" >Submit</button>
 
<script type="text/javascript">
    $.ajax({
        url: "WebMethod1.aspx/PostData",
        type: "POST",
        data: JSON.stringify({ emp: employee }),
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            console.log(result.d)
        }
    });
</script>
 
</asp:Content>
