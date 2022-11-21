<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script type="module" src='<%= ResolveUrl("~/Scripts/Pages/default.js")%>'></script>
    <div id="foo">

    </div>
    <cb-canvas></cb-canvas>
    <table id="grid">

    </table>
    <div style="height:2vh;"></div>
    <input id="text1" type="text" name="name" value="" ><br /> 
    <textarea id="txt1" name="txt1"></textarea><br /> 
    <input type="radio" name="rad1" value="radfoo" id="radfoo" data-bind="foobar"> 
    <input type="radio" name="rad1" value="radbar" id="radbar">
    <input type="radio" name="rad1" value="radqux" id="radqux">
    <br />
    <input type="checkbox" id="chk1" name="chk1" value="444"><br />
    <select id="select1" name="select1"  validation-message="Invalid select1">
        <option value="--" default>--</option>
        <option value="SOC1"> SOC1 </option>
        <option value="SOC2"> SOC2 </option> 
        <option value="SOC3"> SOC3 </option> 
    </select><br />
          <input type="button" value="submit form" id="submitform" /> 
 
    <%--  
   
    --%>
    <input type="button" value="getit" id="getit" /> 
 
</asp:Content>
 
