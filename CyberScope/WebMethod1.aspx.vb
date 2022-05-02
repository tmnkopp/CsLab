Imports System.Web.Services
Imports CyberBalance.CS.Core
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Public Class WebMethod1
    Inherits System.Web.UI.Page
    <WebMethod()>
    Public Shared Function SprocRequest(requests As Dictionary(Of String, DataRequest))
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        Dim response = New DataResponseService() _
            .SetUser(_CAUser) _
            .ApplyRequest(requests) _
            .ApplyUrlEncryption(_UrlParams) _
            .GetResponse()

        Return response

    End Function
End Class



















'
'
'    <WebMethod()>
''    Public Shared Function PostData(user As UserFormViewModel)
''        Dim dt = New DataTable
''        dt.Columns.Add("col1")
''        Return JsonConvert.SerializeObject(dt)
''    End Function
'
'' Public Class UserFormViewModel
''     Public EmpId As String
''     Public EmpName As String
''     Public EmpDate As String
'' End Class





