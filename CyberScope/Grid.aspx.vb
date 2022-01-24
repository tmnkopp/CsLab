Imports System.Data.SqlClient
Imports System.IO
Imports CyberScope.CS.Lab.CBCommandItemGrid
Imports SpreadsheetLight
Imports Telerik.Web.UI
Imports CyberBalance.CS.Core.Document
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports CyberBalance.CS.Web.UI
Imports CyberScope.CS.Lab

Public Class _PageGrid
    Inherits Page
    Dim oDB As New DataBaseUtils2
    Dim validator As Validator = New Validator()

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        MainGrid.PK_OrgSubmission = "26037"
    End Sub


    Public Sub MainGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles MainGrid.ItemDataBound
        Dim ctrl As RadTextBox
        Dim s = $"{GetRandom(1, 9)}"
        If e.Item.ItemType = GridItemType.EditItem Then
            ctrl = e.Item.FindControl("StartingIP")
            If String.IsNullOrEmpty(ctrl.Text) Then
                ctrl.Text = $"{s}{s}{s}.{s}{s}{s}.{s}{s}{s}.{s}{s}{s}"
                ctrl = e.Item.FindControl("EndingIP")
                ctrl.Text = $"{GenerateGUID()}"
            End If
        End If

    End Sub
    Protected Sub MainGrid_ItemCommand(source As Object, e As GridCommandEventArgs) Handles MainGrid.ItemCommand
        Dim cmd = e.CommandName
        If cmd = "PerformInsert" Or cmd = "Update" Then
            Dim src = e.CommandSource
        End If
    End Sub
    Public Sub MainGrid_RecordUpdating(ByVal sender As Object, ByVal e As RecordUpdatingEventArgs) Handles MainGrid.OnRecordUpdating
        Dim cmd = e.GridCommandEventArgs.CommandName
        If cmd = "PerformInsert" Or cmd = "Update" Then
            Dim parms = e.cmd.Parameters
            FismaFormUtils.UpStat(MainGrid.PK_OrgSubmission, "IP")
        End If
    End Sub
    Public Sub MainGrid_ValidateData(ByVal sender As Object, ByVal e As ValidatingEventArgs) Handles MainGrid.OnRowValidating
        bl_Errors.Items.Clear()
        e.IsValid = True
    End Sub
    Protected Sub MainGrid_RecordUpdated(ByVal sender As Object, ByVal e As RecordUpdatingEventArgs) Handles MainGrid.OnRecordUpdated
        If Not IsDBNull(e.cmd.Parameters("@OUT").Value) Then
            If e.cmd.Parameters("@OUT").Value = -11 Then
                bl_Errors.Items.Add("Duplicate Record")
            End If
        End If
        bl_Errors.Visible = bl_Errors.Items.Count > 0
    End Sub

    Protected Overrides Sub OnPreInit(ByVal e As System.EventArgs)
        Dim sConn As String = oDB.SnagConnStr()
        Using oConn As New System.Data.SqlClient.SqlConnection(sConn)
            Dim oUser As New CAuser
            If oUser.AuthenticateUser("Bill-D-Robertson", sConn) Then
                Session.Add(CAuser.SESSIONKEY, oUser)
            End If
        End Using
        MyBase.OnPreInit(e)
    End Sub
    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        Dim Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function
    Private Function GenerateGUID() As String
        Return System.Guid.NewGuid.ToString()
    End Function
End Class