Imports System.Data.SqlClient
Imports System.IO
Imports CyberScope.CS.Lab.MultiAnswerGrid
Imports SpreadsheetLight
Imports Telerik.Web.UI
Imports CyberBalance.CS.Core.Document
Imports CyberBalance.CS.Web.UI
Imports CyberBalance.VB.Core
Public Class _CBGTTGrid
    Inherits Page
    Dim oDB As New DataBaseUtils2

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        MainGrid.PK_OrgSubmission = "28736"
        MainGrid.MasterTableView.DataKeyNames = New String() {"PK_OrgSubmission", "PK_Question", "PK_fsma_GTT"}
        MainGrid.IsPostBack = Page.IsPostBack
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' MainGrid.DataBind()
    End Sub
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
    End Sub
    Protected Sub MainGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles MainGrid.ItemDataBound
    End Sub
    Public Sub MainGrid_ValidateData(ByVal sender As Object, ByVal e As ValidatingEventArgs) Handles MainGrid.OnRowValidating
        bl_Errors.Items.Clear()
        e.IsValid = bl_Errors.Items.Count < 1
    End Sub

    Protected Sub MainGrid_RecordUpdating(ByVal sender As Object, ByVal e As RecordUpdatingEventArgs) Handles MainGrid.OnRecordUpdating
        If Not IsDBNull(e.cmd.Parameters("@OUT").Value) Then
            If e.cmd.Parameters("@OUT").Value = -11 Then
                bl_Errors.Items.Add("Duplicate Record")
            End If
        End If
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