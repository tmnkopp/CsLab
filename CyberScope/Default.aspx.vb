
Imports CyberBalance.CS.Core
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports CyberScope.CS.Lab
Imports CyberScope.CS.Web.UI

Public Class _Default
    Inherits Page
    Dim oDb As DataBaseUtils2 = New DataBaseUtils2()
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit(e)
    End Sub
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        Dim requests = New Dictionary(Of String, DataRequest)

        Dim dr = New DataRequest("BOD_CDM_CRUD")
        dr.PARMS.Add("MODE", "EXPORT")
        requests.Add("BOD_CDM_CRUD_EXPORT", dr)

        dr = New DataRequest("BOD_CDM_CRUD")
        dr.PARMS.Add("MODE", "ddlYN")
        requests.Add("BOD_CDM_CRUD_ddlYN", dr)

        Dim DictOfDataTables As Dictionary(Of String, DataTable) = New DataResponseService() _
            .SetUser(_CAUser) _
            .ApplyRequest(requests) _
            .ApplyUrlEncryption(_UrlParams) _
            .GetDataTables()

        Dim combiner = New SheetCombiner()
        combiner.DataTables = DictOfDataTables
        combiner.Export()
    End Sub
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