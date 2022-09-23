Imports System.Web.Services
Imports CyberBalance.CS.Core
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports CyberScope.CS
Imports CyberScope.CS.Lab
Imports Newtonsoft.Json

Public Class DBUtils
    Inherits System.Web.UI.Page
    <WebMethod()>
    Public Shared Function GetDataTable(request As SprocRequest)
        request.SprocName = "spResponse"

        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        Dim oDb = New DataBaseUtils2()
        oDb.Parms = request.SprocParms
        Dim dt = oDb.GetDataTable(request.SprocName)

        Return JsonConvert.SerializeObject(dt)

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





