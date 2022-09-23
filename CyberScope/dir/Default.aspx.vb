Imports CyberBalance.CS.Core
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports CyberScope.CS.Lab
Imports CyberScope.CS.Web.UI
Public Class _Default1
    Inherits System.Web.UI.Page
    Dim oDb As DataBaseUtils2 = New DataBaseUtils2()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)
        Dim clean = _UrlParams.ParmStringClear
        link.NavigateUrl = _UrlParams.EncryptURL("Default.aspx?PK_ReportingCycle_Component=9999")
        link.Text = link.NavigateUrl
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