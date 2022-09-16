Imports System.Web.Configuration
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports Telerik.Web.UI
Imports System.Data.SqlClient
Imports System.Data

Partial Class LockAgencies
    Inherits AdminPageBase

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Dim sAccessPerms As String() = {"ombapsub", "FSMADM", "ISCMADM", "PMCADM", "CAPGOALADM", "HVAADMIN", "BODADMIN", "EINSTEINADMIN"}
        MyBase.SetCanViewPerms(sAccessPerms)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindDropdownlistToDataTable(rdlReportingCycles) 
            rgAgencyLock.DataBind()
        End If
        Page.MaintainScrollPositionOnPostBack = True
    End Sub

    Private Sub rdlReportingCycles_SelectedIndexChanged(sender As Object, e As DropDownListEventArgs) Handles rdlReportingCycles.SelectedIndexChanged
        'lbl_ReportCycle.Text = rdlReportingCycles.SelectedText
        rgAgencyLock.Rebind()
    End Sub

    Private Sub rgAgencyLock_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles rgAgencyLock.ItemDataBound
        If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then

            Dim drValItem = DirectCast(e.Item.DataItem, DataRowView)
            Dim sStat = CType(drValItem("Status_Code"), String)
            Dim enableZeroTouch = CType(drValItem("enableZeroTouch"), String)
            Dim btnChangeStat = DirectCast(e.Item.FindControl("btn_ChangeStat"), Button)
            Dim btn_enableZeroTouch = DirectCast(e.Item.FindControl("btn_enableZeroTouch"), Button)
            Dim iLock = DirectCast(e.Item.FindControl("iLockUnlock"), PlaceHolder)

            btn_enableZeroTouch.Text = "Enable Zero Touch"
            btn_enableZeroTouch.CssClass = "btn btn-primary btn-sm"
            If enableZeroTouch = "1" Then
                btn_enableZeroTouch.Text = "Disable Zero Touch"
                btn_enableZeroTouch.CssClass = "btn btn-warning btn-sm"
            End If

            If sStat = "LOK" Then
                iLock.Controls.Add(New LiteralControl("<i class=" & """fas fa-lock fa-2x""" & " style=" &"""color:red""" & "></i>"))
                btnChangeStat.Text = "Unlock"
                btnChangeStat.OnClientClick = "return confirm('Are you sure you want to Unlock this Agency?');"
            Else
                iLock.Controls.Add(New LiteralControl("<i class=" & """fas fa-lock-open fa-2x""" & " style=" &"""color:green""" & "></i>"))
                btnChangeStat.Text = "Lock"
                btnChangeStat.OnClientClick = "return confirm('Are you sure you want to Lock this Agency?');"
            End If

        End If
    End Sub

    Private Sub rgAgencyLock_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles rgAgencyLock.NeedDataSource
        rgAgencyLock.DataSource = Populate(rdlReportingCycles.SelectedValue)
    End Sub

    Function Populate(rptCycle As String) As DataTable
        Dim odb As New DataBaseUtils2
        Dim sSrch As String = tb_WildCard.Text
        If sSrch.Length = 0 Then
            sSrch = "%"
        Else
            If Not tb_WildCard.Text.Contains("*") And Not tb_WildCard.Text.Contains("?") Then
                tb_WildCard.Text = "*" & sSrch & "*"
                sSrch = tb_WildCard.Text
            End If
            sSrch = sSrch.Replace("?", "_")
            sSrch = sSrch.Replace("*", "%")
        End If
        oDb.Parms.Add("@FK_ReportingCycle",  rptCycle)

        odb.Parms.Add("@AgencySearchChar", sSrch)
        Dim cqDt As DataTable
        cqDt = odb.GetDataTable("LockAgencies")
        Return cqDt
    End Function

    Private Sub BindDropdownlistToDataTable(dropdownlist As RadDropDownList)
        Dim odb As New DataBaseUtils2
        Dim activeDataCallList As New DataTable()

        activeDataCallList = odb.GetDataTable("SELECT [PK_ReportingCycle] PKcycle, [Description] Cycle FROM fsma_ReportingCycles WHERE IsActive = 1")
        With dropdownlist
            .DataValueField = "PKcycle"
            .DataTextField = "Cycle"           
            .SelectedValue = _CAUser.PK_ReportingCycle
            .DataSource = activeDataCallList
            .DataBind()
        End With
    End Sub
    Protected Sub btn_enableZeroTouch_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim odb As New DataBaseUtils2
        odb.Parms.Add("@PK_Component", sender.CommandArgument)
        odb.Parms.Add("@enableZeroTouch", IIf(sender.text.ToString().Contains("Enable"), "1", "NULL")) ' 
        odb.dbUpdate("UPDATE [Component List] SET enableZeroTouch = @enableZeroTouch WHERE PK_Component = @PK_Component")
        rgAgencyLock.Rebind()
    End Sub
    Protected Sub btn_ChangeStat_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim odb As New DataBaseUtils2
        Dim sCode = ""
        Dim btnChangeStat As Button = sender

        If sender.text = "Lock" Then
            sCode = "LOK"
        End If

        odb.Parms.Add("@PK_Component", btnChangeStat.CommandArgument)
        
        odb.Parms.Add("@Status_Code", IIf(sender.text = "Unlock", "NULL", sCode))'CS-4695 pass in null when unlocking a datacall status, complementing the select sql in reporterhome (rg_Cycles_NeedDataSource) and omb home(rg_Cycles_NeedDataSource) datasource for History tabs
        odb.Parms.Add("@FK_ReportingCycle", rdlReportingCycles.SelectedValue)

        odb.dbUpdate("UPDATE fsma_ReportingCycle_Components SET DatacallStatusCode = @Status_Code WHERE FK_Component = @PK_Component AND FK_ReportingCycle = @FK_ReportingCycle")
        
        odb.Parms.Clear()
        odb.Parms.Add("@PK_PrimeKey", btnChangeStat.CommandArgument)
        odb.Parms.Add("@PK_UserID", Session(CAuser.SESSIONKEY).UserPK)
        odb.Parms.Add("@Status_Code", IIf(sender.text = "Unlock", sender.text, sCode)) 

        oDb.dbInsert("INSERT INTO fsma_WorkFlowHist (PK_PrimeKey, WFtype, PK_UserID, EventDateTime, Status_Code) VALUES (@PK_PrimeKey, 'AGENCY', @PK_UserID, GetDate(), @Status_Code )")
        
        rgAgencyLock.Rebind()
    End Sub

    Protected Sub btn_LockAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_LockAll.Click, btn_UnlockAll.Click
        Dim odb As New DataBaseUtils2
        Dim sCode = ""

        If sender.text = "LOCK ALL" Then
            sCode = "LOK"
        End If
        
        odb.Parms.Add("@Status_Code", IIf(sender.text = "UNLOCK ALL", "NULL", sCode)) 'CS-4695 pass in null when unlocking a datacall status, complementing the select sql in reporterhome (rg_Cycles_NeedDataSource) and omb home(rg_Cycles_NeedDataSource) datasource for History tabs 
        odb.Parms.Add("@FK_ReportingCycle", rdlReportingCycles.SelectedValue)

        odb.dbUpdate("UPDATE fsma_ReportingCycle_Components SET DatacallStatusCode = @Status_Code WHERE FK_ReportingCycle = @FK_ReportingCycle")

        odb.Parms.Clear()
        odb.Parms.Add("@PK_UserID", Session(CAuser.SESSIONKEY).UserPK)
        odb.Parms.Add("@Status_Code", IIf(sender.text = "UNLOCK ALL", sender.text, sCode))
        odb.Parms.Add("@FK_ReportingCycle", _CAUser.PK_ReportingCycle)
        oDb.dbInsert("INSERT INTO fsma_WorkFlowHist (PK_PrimeKey, WFtype, PK_UserID, EventDateTime, Status_Code) SELECT [Component List].PK_Component, 'AGENCY' AS WFtype, @PK_UserID AS PK_UserID, GETDATE() AS EventDateTime, @Status_Code AS Status_Code FROM     [Component List] INNER JOIN fsma_ReportingCycle_Components AS rcc ON [Component List].PK_Component = rcc.FK_Component WHERE  ([Component List].FK_PK_Component IS NULL) AND ([Component List].isActive = 1) AND (rcc.Status_Code <> 'APO') AND (rcc.Status_Code <> 'SUBO') AND (rcc.Status_Code <> 'IR') AND (rcc.FK_ReportingCycle = @FK_ReportingCycle)")

        rgAgencyLock.Rebind()
    End Sub

    Protected Sub btn_Filter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Filter.Click
        rgAgencyLock.Rebind()
    End Sub

    Protected Sub btn_Clear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Clear.Click
        tb_WildCard.Text = ""
        rgAgencyLock.Rebind()
    End Sub

    Protected Overrides Sub OnPreRender(e As System.EventArgs)
        MyBase.OnPreRender(e)
        Dim cbValRegexXSS As New CbValRegexXSS
        cbValRegexXSS.CtrlObjectToValidate = Me.tb_WildCard
        phCbRegXSS.Controls.Add(cbValRegexXSS)

        rgAgencyLock.MasterTableView.GetColumn("enableZeroTouch").Visible = rdlReportingCycles.SelectedValue = "108"
    End Sub
   
End Class