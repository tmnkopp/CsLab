Imports System.Data.SqlClient
Imports System.IO
Imports CyberScope.CS.Lab.CSServiceGrid
Imports SpreadsheetLight
Imports Telerik.Web.UI
Imports CyberBalance.CS.Core.Document
Imports CyberBalance.CS.Web.UI
Imports CyberBalance.VB.Core
Public Class _PageGrid
    Inherits Page
    Dim oDB As New DataBaseUtils2
    Dim validator As Validator = New Validator()
    Dim exporter As SpreadsheetExporter = New SpreadsheetExporter()
    Protected Sub DataImporter_RowValidating(ByVal sender As Object, ByVal e As SpreadsheetImporter.RowValidatingEventArgs) Handles EinsteinDataImporter.OnRowValidating

    End Sub
    Protected Sub DataImporter_InsertCompleted(ByVal sender As Object, ByVal e As SpreadsheetImporter.InsertEventArgs) Handles EinsteinDataImporter.OnInsertComplete
        MainGrid.Rebind()
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        MainGrid.TableName = "EinsteinPublicIP"
        MainGrid.CommandText = "EinsteinPublicIP_CRUD"
        MainGrid.PK_OrgSubmission = "26037"
        MainGrid.UserId = "0"
        MainGrid.DataBind()
        EinsteinDataImporter.PK_OrgSubmission = "1" '  "26037" 
        EinsteinDataImporter.UserId = "0"

    End Sub
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

    End Sub
    Protected Sub MainGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles MainGrid.ItemDataBound

        If (MainGrid.MasterTableView.IsItemInserted) Then
            Dim cmdItem = DirectCast(MainGrid.MasterTableView.GetItems(GridItemType.CommandItem)(0), GridCommandItem)
            DirectCast(cmdItem.FindControl("AddNewRecordButton"), RadButton).Enabled = False
            DirectCast(cmdItem.FindControl("DeleteAll"), RadButton).Enabled = False
        End If

        If TypeOf e.Item Is GridEditableItem And e.Item.IsInEditMode And Not MainGrid.MasterTableView.IsItemInserted Then
            Dim _DataRowView As DataRowView = DirectCast(e.Item.DataItem, DataRowView)
            Dim NonAdvertised As String = _DataRowView("NonAdvertised").ToString()
            If Not String.IsNullOrEmpty(NonAdvertised) Then
                DirectCast(e.Item.FindControl("NonAdvertised"), RadDropDownList).FindItemByValue(NonAdvertised).Selected = True
            End If
        End If
    End Sub
    Public Sub MainGrid_ValidateData(ByVal sender As Object, ByVal e As ValidatingEventArgs) Handles MainGrid.OnRowValidating
        bl_Errors.Items.Clear()

        e.IsValid = bl_Errors.Items.Count < 1
    End Sub
    Protected Sub MainGrid_RecordUpdating(ByVal sender As Object, ByVal e As RecordUpdatingEventArgs) Handles MainGrid.OnRecordUpdating
        If e.GridCommandEventArgs.CommandName = "PerformInsert" Or e.GridCommandEventArgs.CommandName = "Update" Then
            e.cmd.Parameters.RemoveAt("@NonAdvertised")
            e.cmd.Parameters.AddWithValue("@NonAdvertised", DirectCast(e("NonAdvertised"), RadDropDownList).SelectedValue)
        End If
    End Sub
    Protected Sub MainGrid_RecordUpdated(ByVal sender As Object, ByVal e As RecordUpdatingEventArgs) Handles MainGrid.OnRecordUpdated
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

    Protected Sub rbtnDownload_Click(sender As Object, e As EventArgs)
        exporter.DataFields = EinsteinDataImporter.DataFields
        exporter.TableName = "EinsteinPublicIP"
        exporter.SprocName = $"{exporter.TableName}_CRUD"
        exporter.PK_OrgSubmission = "26037"
        exporter.UserId = "0"
        exporter.Export()
        Response.End()
    End Sub
End Class