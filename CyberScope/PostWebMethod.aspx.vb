Imports System.ComponentModel.DataAnnotations
Imports System.Web.Services
Imports CyberScope.CS.Lab
Imports Newtonsoft.Json
Public Class PostWebMethod
    Inherits System.Web.UI.Page
    <WebMethod()>
    Public Shared Function PostData(viewModel As MyViewModel)
        'do stuff with viewModel 
        CSModelState.Validate(viewModel)
        If Not CSModelState.IsValid() Then
            Dim errorList = CSModelState.Errors
            Return JsonConvert.SerializeObject(New With {Key .Errors = errorList})
        End If
        viewModel.Foo = "i did stuff with a view model"
        Return JsonConvert.SerializeObject(viewModel)
    End Function
End Class

Public Class MyViewModel
    <Required(ErrorMessage:="Foo is required")>
    Public Property Foo() As String
    <Required(ErrorMessage:="Bar is required")>
    <Range(1, 100, ErrorMessage:="Bar range valid between 1 and 100")>
    Public Property Bar() As Integer
    Public Property Baz() As String
End Class
























