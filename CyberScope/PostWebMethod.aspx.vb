Imports System.ComponentModel.DataAnnotations
Imports System.Web.Services
Imports Newtonsoft.Json
Public Class PostWebMethod
    Inherits System.Web.UI.Page
    <WebMethod()>
    Public Shared Function PostData(viewModel As MyViewModel)
        'do stuff with viewModel 
        viewModel.Foo = "i did stuff with a view model"
        Return JsonConvert.SerializeObject(viewModel)
    End Function
End Class

Public Class MyViewModel
    <Required(ErrorMessage:="Foo is required")>
    Public Property Foo() As String
    <Required(ErrorMessage:="Bar is required")>
    Public Property Bar() As String
    <Required(ErrorMessage:="Baz is required")>
    Public Property Baz() As String
End Class
























