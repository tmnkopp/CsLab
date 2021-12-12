<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CBGTTGrid.aspx.vb" Inherits="CyberScope._CBGTTGrid" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <asp:BulletedList ID="bl_Errors" runat="server" Visible="false" Width="100%" 
            ForeColor="Red" BulletStyle="NotSet" CssClass="leftalign">
        </asp:BulletedList>
 

     <lab:MultiAnswerGrid ID="MainGrid" 
         StoredProcedure="GTT_CRUD" 
         PK_QuestionGroup="3210" 
         ColumnCaptionPKs="27213,27214" 
         Fields="SystemCount,TestsPerformedCount" runat="server"  > 
     </lab:MultiAnswerGrid> 




</asp:Content>
