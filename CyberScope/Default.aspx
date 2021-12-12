<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>




<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
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


</asp:Content>
