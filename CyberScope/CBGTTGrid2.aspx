<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CBGTTGrid2.aspx.vb" Inherits="CyberScope._CBGTTGrid2" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <asp:BulletedList ID="bl_Errors" runat="server" Visible="false" Width="100%" 
            ForeColor="Red" BulletStyle="NotSet" CssClass="leftalign">
        </asp:BulletedList>
 
 
  <CB:CBDataGrid ID="MainGrid" StoredProcedureCommands="SELECT, PerformInsert, Update, DeleteAll"   
        StoredProcedure="TIC_CloudServices_CRUD" runat="server" >
         <%--    
          <SelectParams>
              <CB:GridSqlParam ParamName="PK_QuestionGroup"  ParamValue="3210" />
          </SelectParams>
         --%>  
         <MasterTableView EditMode="InPlace"
             DataKeyNames="PK_OrgSubmission, PK_CloudServices"  
            CommandItemDisplay="Top">
            <CommandItemTemplate> 
                <telerik:RadButton ID="AddNewRecordButton" runat="server" CommandName="InitInsert" Text="Add New Row" ToolTip="Add New Row" />
                &nbsp;&nbsp;&nbsp;
                <telerik:RadButton ID="DeleteButton" runat="server" CommandName="DeleteAll" Text="Clear All" ToolTip="Clear All" AutoPostBack="false" onclientclicked="OnClientClickedCloud" ValidationGroup="vgClearCloud"/>
                &nbsp;&nbsp;&nbsp; 
                <asp:CheckBox ID="chkboxNACloud" runat="server" Text="No Services to Report"  AutoPostBack="False" onclick="javascript:TestConfirmCloud(this)" />
            </CommandItemTemplate>
            <Columns>
                <telerik:GridEditCommandColumn UniqueName="EditCol"></telerik:GridEditCommandColumn>  
                <telerik:GridTemplateColumn HeaderText="ProviderName" UniqueName="ProviderName">
                <EditItemTemplate> 
                    <asp:TextBox ID="ProviderName" runat="server" ></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblProviderName" runat="server" Text='<%# Bind("ProviderName") %>'></asp:Label>
                </ItemTemplate>
                </telerik:GridTemplateColumn>  
             </Columns>
         </MasterTableView> 
     </CB:CBDataGrid> 
    
</asp:Content>
