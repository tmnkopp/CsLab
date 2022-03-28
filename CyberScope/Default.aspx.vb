Imports System.Data.SqlClient
Imports System.IO
Imports CyberScope.CS.Lab.CBGrid
Imports SpreadsheetLight
Imports Telerik.Web.UI
Imports CyberBalance.CS.Core.Document
Imports CyberBalance.CS.Web.UI
Imports CyberBalance.VB.Core
Public Class _Default
    Inherits Page
    Dim oDb As DataBaseUtils2 = New DataBaseUtils2()
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit(e)
    End Sub
    Protected Overrides Sub OnPreInit(ByVal e As System.EventArgs)
        MyBase.OnPreInit(e)
        Dim sConn As String = oDB.SnagConnStr()
        Using oConn As New System.Data.SqlClient.SqlConnection(sConn)
            Dim oUser As New CAuser
            If oUser.AuthenticateUser("Bill-D-Robertson", sConn) Then
                Session.Add(CAuser.SESSIONKEY, oUser)
            End If
        End Using
    End Sub
End Class