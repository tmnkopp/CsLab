<%@ Page Title="" Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="PostWebMethod.aspx.vb" Inherits="CyberScope.PostWebMethod" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 
    <input type="button" value="postit" id="postit" /> 

    <script type="text/javascript">
    $(document).ready(function () {

        $('#postit').click(function () {
            postData();
        });

        function postData() {
            var myViewModel = {
                Foo: "myFoo",
                Bar: 23,
                Baz: ""
            }
            var json = JSON.stringify({ viewModel: myViewModel });

            $.ajax({
                url: `PostWebMethod.aspx/PostData`,
                type: "POST",
                data: json,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: (response) => {
                    var _json = JSON.parse(response.d);
                    console.log(_json);
                }
            });
        } 
    });
    </script>  
</asp:Content>
