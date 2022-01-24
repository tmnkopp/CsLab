<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Grid.aspx.vb" Inherits="CyberScope._PageGrid" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:BulletedList ID="bl_Errors" runat="server" Visible="false" Width="100%" 
        ForeColor="Red" BulletStyle="NotSet" CssClass="leftalign">
    </asp:BulletedList>
 


    <lab:CBCommandItemGrid ID="MainGrid" StoredProcedureCommands="SELECT_ALL, SelectPKOrg"
        GridRecordCaption="Einstein Public IP" 
        StoredProcedure="EinsteinPublicIP_CRUD"  runat="server" >
          <MasterTableView EditMode="InPlace" DataKeyNames="PK_EinsteinPublicIP, PK_OrgSubmission" > 
         <Columns>
     
            <telerik:GridEditCommandColumn UniqueName="EditCol" >
                <HeaderStyle Width="1%"></HeaderStyle>
            </telerik:GridEditCommandColumn> 
            <telerik:GridButtonColumn CommandName="Delete" Text="Delete" UniqueName="DeleteCol" ConfirmText="Are you sure you want to delete?">
                <HeaderStyle Width="1%"></HeaderStyle>
            </telerik:GridButtonColumn>
            <telerik:GridBoundColumn DataField="StartingIP" HeaderText="StartingIP" UniqueName="StartingIP" />
            <telerik:GridBoundColumn DataField="EndingIP" HeaderText="EndingIP" UniqueName="EndingIP" />
            <telerik:GridDateTimeColumn  UniqueName="DateUploaded" HeaderText="DateUploaded" DataField="DateUploaded" />
     
            <telerik:GridTemplateColumn HeaderText="TIC_MTIPS" UniqueName="TIC_MTIPS">
            <EditItemTemplate>
                <CB:CBDropDownPickList ID="TIC_MTIPS" UsageField="EINTICMTIPS" SelectedPicklistPK='<%# Bind("TIC_MTIPS") %>' runat="server"></CB:CBDropDownPickList>
            </EditItemTemplate>
            <ItemTemplate>
                <CB:CBLabelPickList ID="lblTIC_MTIPS" UsageField="EINTICMTIPS" SelectedPicklistPK='<%# Bind("TIC_MTIPS") %>' runat="server"></CB:CBLabelPickList>
            </ItemTemplate>
            </telerik:GridTemplateColumn>
          
            <telerik:GridBoundColumn DataField="NamePurpose" HeaderText="NamePurpose" UniqueName="NamePurpose" />
 
            <telerik:GridTemplateColumn HeaderText="Internet Service Provider (ISP)" UniqueName="ISP" SortExpression="ISP">
            <EditItemTemplate>
                <CB:CBDropDownPickList ID="ISP" UsageField="EINUMISP" SelectedPicklistPK='<%# Bind("ISP") %>' runat="server"></CB:CBDropDownPickList>
            </EditItemTemplate>
            <ItemTemplate>
                <CB:CBLabelPickList ID="lblISP" UsageField="EINUMISP" SelectedPicklistPK='<%# Bind("ISP") %>' runat="server"></CB:CBLabelPickList>
            </ItemTemplate>
            </telerik:GridTemplateColumn>
           
            <telerik:GridBoundColumn DataField="OtherISP" HeaderText="OtherISP" UniqueName="OtherISP" />
 
            <telerik:GridTemplateColumn HeaderText="Visible to EINSTEIN?" UniqueName="VisibleToEinstein" SortExpression="VisibleToEinstein">
            <EditItemTemplate>
                <CB:CBDropDownPickList ID="VisibleToEinstein" UsageField="YESNOUNK" SelectedPicklistPK='<%# Bind("VisibleToEinstein") %>' runat="server"></CB:CBDropDownPickList>
            </EditItemTemplate>
            <ItemTemplate>
                <CB:CBLabelPickList ID="lblVisibleToEinstein" UsageField="YESNOUNK" SelectedPicklistPK='<%# Bind("VisibleToEinstein") %>' runat="server"></CB:CBLabelPickList>
            </ItemTemplate>
            </telerik:GridTemplateColumn>

            <telerik:GridBoundColumn DataField="lblExternalOrganization" HeaderText="lblExternalOrganization" UniqueName="lblExternalOrganization" />
              
             
        </Columns> 
    </MasterTableView> 
    </lab:CBCommandItemGrid> 
     <style>
         table[id*=mainTable] table tr td:first-child{
             padding:.5em 1em; 
             font-weight:bold;
         }
         table[id*=mainTable]{ 
             width:100vw;
         }
     </style>
</asp:Content>
