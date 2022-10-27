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
    Public Shared Function CbGet(request As SprocRequest)
        request = request.StripScript()
        If String.IsNullOrWhiteSpace(request.Handler) Then
            Throw New System.Exception("Request must supply a Handler.")
        End If
        request.MapSprocMeta()
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)
        request.PARMS.Add("@UserId", _CAUser.UserPK.ToString())
        Dim data = New SprocRequestService(request).MapResponse(Of String)(Function(ds) JsonConvert.SerializeObject(ds.Tables))
        Return data.StripScript()
    End Function

    <WebMethod()>
    Public Shared Function CbPost(request As SprocRequest)
        request = request.StripScript()
        If String.IsNullOrWhiteSpace(request.Handler) Then
            Throw New System.Exception("Request must supply a Handler.")
        End If
        request.MapSprocMeta()
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)
        request.PARMS.Add("@UserId", _CAUser.UserPK.ToString())
        Dim data = New SprocRequestService(request).MapResponse(Of String)(Function(ds) JsonConvert.SerializeObject(ds.Tables))
        Return data.StripScript()
    End Function
    '' Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    ''     If Session(CAuser.SESSIONKEY) Is Nothing Then
    ''         Throw New Exception("LoggedOff")
    ''     End If
    '' End Sub

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





