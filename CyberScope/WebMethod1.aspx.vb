Imports System.Runtime.CompilerServices
Imports System.Web.Services
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports CyberScope.CS.Lab.Models
Imports Newtonsoft.Json

Public Class WebMethod1
    Inherits System.Web.UI.Page

    <WebMethod()>
    Public Shared Function SprocRequest(request As Dictionary(Of String, SprocRequest))
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        Dim response = New DataResponseService() _
            .SetUser(_CAUser) _
            .ApplySprocRequest(request) _
            .PerformRequest() _
            .ApplyUrlEncryption(Function(f) _UrlParams.EncryptURL(f)) _
            .GetResponseAsJson()

        Return response

    End Function

    <WebMethod()>
    Public Shared Function PostData(emp As Employee)
        Dim dt = New DataTable
        dt.Columns.Add("col1")
        Return JsonConvert.SerializeObject(dt)
    End Function
End Class
Public Class Employee
    Public EmpId As String
    Public EmpName As String
    Public EmpDate As String
End Class




