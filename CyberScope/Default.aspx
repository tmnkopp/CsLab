<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="CyberScope._Default" %>
<%@ Register TagPrefix="CB" Namespace="CyberBalance.CS.Web.UI" Assembly= "CyberBalance.CS.Web.UI" %>
<%@ Register TagPrefix="lab" Namespace="CyberScope.CS.Lab" Assembly= "CyberScope.CS.Lab" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function getit() {
         
            $.getJSON("api/values",
            function (data) {
                console.log(data);
                // $.each(data, function (key, val) { 
                //     console.log(key);
                // });
            });
        }
        function postit() {
            var xhr = new XMLHttpRequest();
            xhr.open("POST", 'api/values', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.send(JSON.stringify({
                value: 'asdf'
            }));
        }
        $(document).ready(function () {
            getit();
            postit(); 
        });

  
    </script>
  
    <lab:CBGrid runat="server" ID="CBGrid"> 
    </lab:CBGrid>
</asp:Content>
 
