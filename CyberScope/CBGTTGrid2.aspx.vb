Imports System.Data.SqlClient
Imports System.IO
Imports SpreadsheetLight
Imports Telerik.Web.UI
Imports CyberBalance.CS.Core.Document
Imports CyberBalance.CS.Web.UI
Imports CyberBalance.VB.Core
Imports CyberBalance.CS.Web.UI.CBDataGrid

Public Class _CBGTTGrid2
    Inherits Page
    Dim oDB As New DataBaseUtils2

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        MainGrid.PK_OrgSubmission = "30501"
    End Sub
    Protected Sub MainGrid_Selecting(ByVal sender As Object, ByVal e As SelectingEventArgs) Handles MainGrid.OnSelecting
        'MainGrid.AddSqlParam(e.cmd, "@PK_QuestionGroup", "3210")
        'e.cmd.Parameters.AddWithValue("@PK_QuestionGroup", "3210")
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