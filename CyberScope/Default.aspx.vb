Imports System.Data.SqlClient
Imports System.IO
Imports CyberScope.CS.Lab.CSServiceGrid
Imports SpreadsheetLight
Imports Telerik.Web.UI
Imports CyberBalance.CS.Core.Document
Imports CyberBalance.CS.Web.UI
Imports CyberBalance.VB.Core
Public Class _Default
    Inherits Page
    Dim oDb As DataBaseUtils2 = New DataBaseUtils2()
    Dim validator As Validator = New Validator()
    Dim exporter As SpreadsheetExporter = New SpreadsheetExporter()

    Protected Sub DataImporter_RowValidating(ByVal sender As Object, ByVal e As SpreadsheetImporter.RowValidatingEventArgs) Handles EinsteinDataImporter.OnRowValidating

    End Sub
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

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

    End Sub
End Class