<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   
    <script type="module" src='<%= ResolveUrl("~/Scripts/Pages/WebMethod1.js")%>'></script>
    <input id="text1" type="text" name="name" value="" ><br /> 
    <textarea id="txt1" name="txt1"></textarea><br /> 
    <input type="radio" name="rad1" value="foo" id="foo"> 
    <input type="radio" name="rad1" value="bar" id="bar">
    <input type="radio" name="rad1" value="qux" id="qux">
    <br />
    <input type="checkbox" id="chk1" name="chk1" value="444"><br />
    <select id="select1" name="select1"  validation-message="Invalid select1">
        <option value="--" default>--</option>
        <option value="1"> 1 </option>
        <option value="2"> 2 </option> 
    </select><br />
          <input type="button" value="submit form" id="submitform" /> 
    <table id="grid">

    </table>
    <%--  
   
    --%>
    <input type="button" value="getit" id="getit" /> 
 
</asp:Content>
 
