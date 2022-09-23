<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default1" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script type="module" src='<%= ResolveUrl("~/Scripts/Pages/WebMethod1.js")%>'></script> 
    <asp:HyperLink ID="link" runat="server"></asp:HyperLink>
</asp:Content>
