<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CBGTTGrid2.aspx.vb" Inherits="CyberScope._CBGTTGrid2" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <asp:BulletedList ID="bl_Errors" runat="server" Visible="false" Width="100%" 
            ForeColor="Red" BulletStyle="NotSet" CssClass="leftalign">
        </asp:BulletedList>
 
 
  <CB:CBDataGrid ID="MainGrid" StoredProcedureCommands="Export, Update, Delete"   
        StoredProcedure="GTT_CRUD" runat="server" >
         <%--    --%> 
          <SelectParams>
              <CB:GridSqlParam ParamName="PK_QuestionGroup"  ParamValue="3210" />
          </SelectParams>
       
         <MasterTableView EditMode="InPlace"  
             DataKeyNames="PK_OrgSubmission, PK_Question, PK_fsma_GTT"  
             CommandItemDisplay="None">  
            <Columns>
                <telerik:GridEditCommandColumn UniqueName="EditCol"></telerik:GridEditCommandColumn> 
                <telerik:GridBoundColumn ReadOnly="true" HeaderText="Type of Test" DataField="CAPTION"></telerik:GridBoundColumn>
                  <telerik:GridTemplateColumn HeaderText="Total count of systems (from 1.1.1 and 1.1.2) that received this form of testing (Numeric)" UniqueName="SystemCount">
                <EditItemTemplate> 
                    <telerik:RadNumericTextBox ID="SystemCount" runat="server" AutoCompleteType="Disabled" > 
                        <NumberFormat DecimalDigits="0" />
                    </telerik:RadNumericTextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSystemCount" runat="server" Text='<%# Bind("SystemCount") %>'></asp:Label>
                </ItemTemplate>
                </telerik:GridTemplateColumn> 
                <telerik:GridTemplateColumn HeaderText="Total count of tests performed (Numeric)" UniqueName="TestsPerformedCount">
                <EditItemTemplate> 
                    <telerik:RadNumericTextBox ID="TestsPerformedCount" AutoCompleteType="Disabled" runat="server" > 
                        <NumberFormat DecimalDigits="0" />
                    </telerik:RadNumericTextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblTestsPerformedCount" runat="server" Text='<%# Bind("TestsPerformedCount") %>'></asp:Label>
                </ItemTemplate>
                </telerik:GridTemplateColumn>  
             </Columns>
         </MasterTableView> 
     </CB:CBDataGrid> 
    
</asp:Content>
