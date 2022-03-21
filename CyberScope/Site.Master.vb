Imports CyberBalance.CS.Web.UI
Imports CyberBalance.VB.Core

Public Class SiteMaster
    Inherits MasterPage
    Private _RenderForm As Boolean = True
    Public Property RenderForm() As Boolean
        Get
            Return _RenderForm
        End Get
        Set(ByVal value As Boolean)
            _RenderForm = value
        End Set
    End Property
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        Me.Controls.Remove(Me.FindControl("pageScriptManager"))
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
End Class