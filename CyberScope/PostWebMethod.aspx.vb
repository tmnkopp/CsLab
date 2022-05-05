Imports System.ComponentModel.DataAnnotations
Imports System.Web.Services
Imports CyberBalance.CS.Core
Imports Newtonsoft.Json
Public Class PostWebMethod
    Inherits System.Web.UI.Page
    <WebMethod()>
    Public Shared Function PostData(viewModel As MyViewModel)
        'do stuff with viewModel 
        CBModelState.Validate(viewModel)
        If Not CBModelState.IsValid() Then
            Dim errorList = CBModelState.Errors
            Dim response = New With {Key .viewModel = viewModel, Key .errors = errorList}
            Return JsonConvert.SerializeObject(response)
        End If
        viewModel.Foo = "i did stuff"
        Return JsonConvert.SerializeObject(New With {Key .viewModel = viewModel, Key .errors = Nothing})
    End Function
End Class

Public Class MyViewModel
    <Required(ErrorMessage:="Foo is required")>
    Public Property Foo() As String
    <Required(ErrorMessage:="Bar is required")>
    <Range(1, 100, ErrorMessage:="Bar range valid between 1 and 100")>
    Public Property Bar() As Integer = 0
    Public Property Baz() As String
End Class
























