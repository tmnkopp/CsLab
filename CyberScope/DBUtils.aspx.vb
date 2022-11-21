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
        request = request.StripScript()
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        request.PARMS.Add("@UserId", _CAUser.UserPK.ToString())

        Dim oDb = New DataBaseUtils2()
        oDb.Parms = request.SprocParms
        Dim dt = oDb.GetDataTable(request.SprocName)

        Return JsonConvert.SerializeObject(dt).StripScript()

    End Function

    <WebMethod()>
    Public Shared Function RequestPicklist(request As SprocRequest)
        request = request.StripScript()
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        request.PARMS.Add("@UserId", _CAUser.UserPK.ToString())
        request.SprocName = "spSprocMeta"
        Dim oDb = New DataBaseUtils2()
        oDb.Parms = request.SprocParms
        Dim dt = oDb.GetDataTable(request.SprocName).EncryptUrls(_UrlParams)

        Return JsonConvert.SerializeObject(dt).StripScript()
    End Function

    Dim oDb As DataBaseUtils2 = New DataBaseUtils2()
    Protected Overrides Sub OnPreInit(ByVal e As System.EventArgs)
        MyBase.OnPreInit(e)
        Dim sConn As String = oDb.SnagConnStr()
        Using oConn As New System.Data.SqlClient.SqlConnection(sConn)
            Dim oUser As New CAuser
            If oUser.AuthenticateUser("Bill-D-Robertson", sConn) Then
                Session.Add(CAuser.SESSIONKEY, oUser)
            End If
        End Using
    End Sub
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





