<%@ Page Title="" Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="PostWebMethod.aspx.vb" Inherits="CyberScope.PostWebMethod" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <ul id="errors"></ul>
    <input type="button" value="postit" id="postit" /> 
    Foo: <input type="text" value="post me" id="txtFoo" /> 
    Bar: <input type="text" value="99" id="txtBar" /> 
    Baz: <input type="text" value="" id="txtBaz" /> 
 
    <script type="text/javascript">
    $(document).ready(function () {
         
        $('#postit').click(function () {
            postData();
        });

        function postData() {
            var myViewModel = {
                Foo: $('#txtFoo').val(),
                Bar: $('#txtBar').val() != '' ? $('#txtBar').val() : -1,
                Baz: $('#txtBaz').val()
            }
            var request = JSON.stringify({ viewModel: myViewModel });

            $.ajax({
                url: `PostWebMethod.aspx/PostData`,
                type: "POST",
                data: request,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: (response) => {
                    var json = JSON.parse(response.d); 
                    data_response(json);
                }
            });
        }
        function data_response(response) {
            $('#txtFoo').val(response.viewModel.Foo);
            $('#errors').html('');
            if (response.errors != null) {
                response.errors.forEach(function (e) {
                    $('#errors').append('li').text(e);
                });
            };
        };
    });
    </script>  
    <style>       
        input {
            display:block;
        }
    </style>
</asp:Content>
