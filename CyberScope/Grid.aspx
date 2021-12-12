<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Grid.aspx.vb" Inherits="CyberScope._PageGrid" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:BulletedList ID="bl_Errors" runat="server" Visible="false" Width="100%" 
        ForeColor="Red" BulletStyle="NotSet" CssClass="leftalign">
    </asp:BulletedList>

    <telerik:RadButton ID="rbtnDownload" runat="server" OnClick="rbtnDownload_Click" Text="Download" CommandName="Excel"  />
   
    <CB:EinsteinDataImporter 
        ID="EinsteinDataImporter" 
        TableName="EinsteinPublicIP"  
        SprocName="EinsteinPublicIP_CRUD"  
        OnOnRowValidating="DataImporter_RowValidating" UseUploader="True"  runat="server">  
        <DataFields>
            <CB:DataField  DBColumnName="StartingIP"  runat="server"/>  
            <CB:DataField  DBColumnName="EndingIP"   runat="server"/>  
            <CB:DataField  DBColumnName="Name" ImportColumnName="Name_Purpose"  runat="server"/>  
            <CB:DataField  DBColumnName="ISP"  PickListTypeID="294" runat="server"/>  
            <CB:DataField  DBColumnName="OtherISP" Require="false"  runat="server"/>  
            <CB:DataField  DBColumnName="ExternalOrg" ImportColumnName="External_Organization"  runat="server"/>          
            <CB:DataField  DBColumnName="TIC_MTIPS"  PickListTypeID="332"  runat="server"/>  
            <CB:DataField  DBColumnName="Visible"  PickListTypeID="162" runat="server"/>  
        </DataFields> 
    </CB:EinsteinDataImporter> 


    <lab:CSServiceGrid ID="MainGrid"  runat="server" >
          <MasterTableView EditMode="InPlace" DataKeyNames="PK_EinsteinPublicIP, PK_OrgSubmission" CommandItemDisplay="Top">
 

         <Columns>
            <telerik:GridEditCommandColumn UniqueName="EditCol" >
                <HeaderStyle Width="4%"></HeaderStyle>
            </telerik:GridEditCommandColumn> 
            <telerik:GridButtonColumn CommandName="Delete" Text="Delete" UniqueName="DeleteCol" ConfirmText="Are you sure you want to delete?">
                <HeaderStyle Width="4%"></HeaderStyle>
            </telerik:GridButtonColumn>
  
            
            <telerik:GridTemplateColumn HeaderText="Starting IP" UniqueName="StartingIP" SortExpression="StartingIP">
            <EditItemTemplate>
                <telerik:RadTextBox ID="StartingIP" CssClass="IPAddress"  runat="server" MaxLength="150"></telerik:RadTextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblStartingIP" runat="server" Text='<%# Bind("StartingIP") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>


            <telerik:GridTemplateColumn HeaderText="Ending IP" UniqueName="EndingIP" SortExpression="EndingIP">
            <EditItemTemplate>
                <telerik:RadTextBox ID="EndingIP" CssClass="IPAddress" runat="server" MaxLength="150"></telerik:RadTextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblEndingIP" runat="server" Text='<%# Bind("EndingIP") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>


            <telerik:GridTemplateColumn HeaderText="TIC/MTIPS" UniqueName="TIC_MTIPS" SortExpression="TIC_MTIPS">
            <EditItemTemplate>
                <telerik:RadDropDownList ID="TIC_MTIPS" runat="server" MaxLength="150"></telerik:RadDropDownList>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblTIC_MTIPS" runat="server" Text='<%# Bind("TIC_MTIPSText") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>


            <telerik:GridTemplateColumn HeaderText="Name Purpose" UniqueName="Name" SortExpression="Name">
            <EditItemTemplate>
                <telerik:RadTextBox ID="Name" runat="server" MaxLength="150"></telerik:RadTextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>


            <telerik:GridTemplateColumn HeaderText="Internet Service Provider (ISP)" UniqueName="ISP" SortExpression="ISP">
            <EditItemTemplate>
                <telerik:RadDropDownList ID="ISP" runat="server" ></telerik:RadDropDownList>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblISP" runat="server" Text='<%# Bind("ISPText") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>


            <telerik:GridTemplateColumn HeaderText="Other ISP" UniqueName="OtherISP" SortExpression="OtherISP">
            <EditItemTemplate>
                <telerik:RadTextBox ID="OtherISP" runat="server" MaxLength="150"></telerik:RadTextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblOtherISP" runat="server" Text='<%# Bind("OtherISP") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>

            <telerik:GridTemplateColumn HeaderText="Visible to EINSTEIN?" UniqueName="Visible" SortExpression="Visible">
            <EditItemTemplate>
                <telerik:RadDropDownList ID="Visible" runat="server" ></telerik:RadDropDownList>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblVisible" runat="server" Text='<%# Bind("VisibleText") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>

            <telerik:GridTemplateColumn HeaderText="External Organization" UniqueName="ExternalOrg" SortExpression="ExternalOrg">
            <EditItemTemplate>
                <telerik:RadTextBox ID="ExternalOrg" runat="server" MaxLength="150"></telerik:RadTextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblExternalOrg" runat="server" Text='<%# Bind("ExternalOrg") %>'></asp:Label>
            </ItemTemplate>
            </telerik:GridTemplateColumn>
             
        </Columns> 
    </MasterTableView> 
    </lab:CSServiceGrid> 




</asp:Content>
