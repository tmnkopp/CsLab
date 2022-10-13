<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  
    
     <script type="module" src='<%= ResolveUrl("~/Scripts/Pages/WebMethod1.js")%>'></script>
    <script>
   
    </script>
    <%--  
   
    --%>
    <input type="button" value="getit" id="getit" /> 
 
</asp:Content>
 
