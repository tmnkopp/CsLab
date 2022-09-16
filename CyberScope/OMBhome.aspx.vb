Imports System.Data
Imports System.Data.SqlClient
Imports Dundas.Charting.WebControl
Imports System.Drawing
Imports System.IO
Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Partial Class OMBhome
    Inherits OmbPageBase
    Private bViewForms As Boolean = False
    Private canView As Boolean
    Dim oDb As DataBaseUtils2 = New DataBaseUtils2()
    Dim parms As Dictionary(Of String, String) = New Dictionary(Of String, String)()
    Public Class FormColumnTemplate
        Implements ITemplate

        Protected _PkFormType As String
        Protected _CapabilityVal As String
        Protected _IsEnabled As Nullable(Of Boolean)
        Protected _WorkFlow As CSWorkFlow = CSWorkFlow.DHS
        Protected _CaUser As CAuser

        Public Sub New(ByVal PkFormType As String, ByVal IsEnabled As Nullable(Of Boolean), caUser As CAuser, workFlow As CSWorkFlow)
            Me._PkFormType = PkFormType
            Me._IsEnabled = IsEnabled
            Me._WorkFlow = workFlow
            Me._CaUser = caUser
        End Sub

        Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
            Dim Spacer As LiteralControl = New LiteralControl("&nbsp;")
            Dim hlForm As HyperLink = New HyperLink()
            AddHandler hlForm.DataBinding, AddressOf hl_DataBinding
            container.Controls.Add(Spacer)
            container.Controls.Add(hlForm)
        End Sub

        Protected Sub hl_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
            Dim HyperLinkBound As HyperLink = DirectCast(sender, HyperLink)
            ' Dim container As GridViewRow = HyperLinkBound.NamingContainer
            Dim container As GridDataItem = DirectCast(HyperLinkBound.NamingContainer, GridDataItem)
            'Dim DataRowViewValue As DataRowView = DirectCast(DataBinder.GetDataItem(container), DataRowView)
            Dim DataRowViewValue As DataRowView = DirectCast(container.DataItem, DataRowView)


            If DataRowViewValue("OrgSubmission_" & _PkFormType) IsNot DBNull.Value Then
                Dim formStatus As String = DataRowViewValue("FormStatus_" & _PkFormType)
                Dim formStatusCode As String = DataRowViewValue("FormStatusCode_" & _PkFormType)
                'Dim FormInstanceCount As Integer = Convert.ToInt32(DataRowViewValue("FormCount_" & _PkFormType))
                Dim datacallStatus As String = String.Empty
                If Not (DataRowViewValue("DatacallStatusCode_" & _PkFormType)) = "" Then
                    datacallStatus = DataRowViewValue("DatacallStatusCode_" & _PkFormType)
                End If
                Dim UrlToNavigate As String = Nothing
                'If FormInstanceCount = 1 Then
                UrlToNavigate = "~/" & DataRowViewValue("InternalForm_" & _PkFormType) & "?PK_OrgSubmission=" & DataRowViewValue("OrgSubmission_" & _PkFormType)
                HyperLinkBound.ForeColor = OMBhome.RetColor(formStatus)
                'Dim oDB As New DataBaseUtils2
                Dim oParms As New Dictionary(Of String, String)
                If oParms.Count > 0 Then
                    oParms.Clear()
                End If

                oParms.Add("@PK_OrgSubmission", DataRowViewValue("OrgSubmission_" & _PkFormType))
                HyperLinkBound.Text = formStatus
                If _WorkFlow = CSWorkFlow.IC Then
                    If formStatusCode = "SUBO" Then
                        HyperLinkBound.Text = "Submitted to ODNI"
                    ElseIf formStatusCode = "APO" Then
                        HyperLinkBound.Text = "ODNI Approved"
                    End If
                End If
                'Else
                '    UrlToNavigate = "~/OMB_AgReview.aspx?PK_ReportingCycle_Component=" & DataRowViewValue("PK_ReportingCycle_Component")
                '    HyperLinkBound.Text = "Multiple Forms"
                'End If

                Dim oCrptURL As URLParms = New URLParms(_CaUser.SessionToken, False, _CaUser.SessionIV, HttpContext.Current.Request.QueryString, _CaUser.SessionHashToken)
                HyperLinkBound.NavigateUrl = oCrptURL.EncryptURL(UrlToNavigate)
                Dim sStat As String = String.Empty
                Dim bEnable As Boolean = False

                If _WorkFlow = CSWorkFlow.DHS Then
                    sStat = DataRowViewValue("Agency_Status_code").ToString()
                    bEnable = (sStat = "SUBO" Or sStat = "APO" Or sStat = "IR")
                ElseIf _WorkFlow = CSWorkFlow.IC Then ' we do this at the form level for IC
                    bEnable = formStatusCode = "APO" Or ((formStatusCode = "SUBO" Or formStatusCode = "IR") And _CaUser.HasPermission(FismaFormUtils.GetFormTypePermsString(_PkFormType, 1)))
                End If

                If Me._IsEnabled.HasValue Then
                    bEnable = _IsEnabled.Value
                End If

                If _CaUser.PK_ReportingCycle = 48 Or _CaUser.PK_ReportingCycle = 50 Then
                    bEnable = False
                End If

                If Not bEnable Then
                    HyperLinkBound.NavigateUrl = ""
                End If

                HyperLinkBound.Enabled = bEnable
            Else
                HyperLinkBound.Visible = False
            End If
        End Sub
    End Class



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        canView = _CAUser.hasPermission2(Nothing, "Admin-DataViewOnly")
        bViewForms = _CAUser.hasPermission2(Nothing, "Admin-FormViewList")
        If Me._WorkFlow = CSWorkFlow.IC Then
            SqlDS_StatusOMB.SelectCommand = SqlDS_StatusOMB.SelectCommand.ToString.Replace("OMB", "[" & _CAUser.CurrentDataCallOwner & "]")
        End If

        'Dim PK_ReportingCycle As Integer = _CAUser.PK_ReportingCycle
        'bViewForms = _CAUser.HasPermission("OMBVWRT,OMBAPSUB,FULLADMIN")
        'bViewForms = _CAUser.HasPermission("OMBVWRT,OMBTICVWRT,OMBISCMVWRT,OMBPMCVWRT,OMBCAPGOALVWRT,OMBAPSUB,OMBISCMPOAMAPSUB,OMBTICPOAMSSAAPSUB,OMBPMCAPSUB,OMBCAPGOALAPSUB,OMBBODPOAMAPSUB,OMBBOD1802POAMSUB,OMBHVAPOAMSUB,OMBEINSTEINSUB,OMBBOD2001SUB,FISMAADM,TICADM,ISCMADM,PMCADMIN,BODADMIN,CAPGOALADMIN,BOD2001ADMIN,FULLADMIN")

        If Not Page.IsPostBack Then

            parms.Add("@PK_ReportingCycle_Component", _CAUser.PK_ReportingCycle_Component)

            Dim adminAcronym As String = ConfigurationManager.AppSettings("AdminAcronym")
            If String.IsNullOrEmpty(adminAcronym) Then
                adminAcronym = "FNR"
            End If

            parms.Add("@AllowAcronym", adminAcronym)
            oDb.dbUpdate("admin_FormPreview_AddRecs", parms)

            LoadSurveyTabs()
            If _CAUser.PK_ReportingCycle <> radTS_Surveys.Tabs(0).Value Then
                radTS_Surveys.SelectedIndex = 0
                If Not radTS_Surveys.Tabs(radTS_Surveys.SelectedIndex) Is Nothing Then
                    _CAUser.PK_ReportingCycle = radTS_Surveys.Tabs(radTS_Surveys.SelectedIndex).Value
                    _CAUser.PK_ReportingCycle_Component = radTS_Surveys.Tabs(radTS_Surveys.SelectedIndex).Attributes("PK_ReportingCycle_Component")
                End If
            Else
                _CAUser.PK_ReportingCycle = -1
                Me.radTS_Surveys.SelectedIndex = 0
            End If

            'If _CAUser.Pk_reportingCycle = -1 Then
            '    Me.radTS_Surveys.SelectedIndex = 0

            'End If


        End If

        If radTS_Surveys.SelectedIndex <> -1 AndAlso _CAUser.PK_ReportingCycle <> radTS_Surveys.Tabs(radTS_Surveys.SelectedIndex).Value Then
            _CAUser.PK_ReportingCycle_Component = radTS_Surveys.Tabs(radTS_Surveys.SelectedIndex).Attributes("PK_ReportingCycle_Component").ToString()
            _CAUser.PK_ReportingCycle = radTS_Surveys.Tabs(radTS_Surveys.SelectedIndex).Value

            If _CAUser.PK_ReportingCycle <> 80 Then
                Me.gv_MainStatusDynamic.Rebind()
            End If
            If _CAUser.PK_ReportingCycle = 80 Then
                rgBOD1802Poam.Rebind()
            End If

            FV_DataCallStatus.DataBind()
            LoadFormList()
        End If

        Dim PK_ReportingCycle As Integer = _CAUser.PK_ReportingCycle
        rgBOD1802Poam.Visible = False
        If _CAUser.PK_ReportingCycle = 80 Then
            rgBOD1802Poam.Visible = True
            gv_MainStatusDynamic.Visible = False
        End If

        lnkManageAssessment.Visible = False
        lnkManageAgencyRemediation.Visible = False
        If PK_ReportingCycle = 54 Then
            Dim sPkRc As String = "0"
            lnkManageAgencyRemediation.Visible = True
            lnkManageAgencyRemediation.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/EnterAdminNcatsScan.aspx?PK_ReportingCycle_Component=" & sPkRc)
            lnkAuditBODStatus.Attributes("onclick") = "return OpenRadWindow('" & _UrlParams.EncryptURL(ResolveUrl("~/BOD/BODStatusDisplay.aspx")) & "','remplan');"
            lnkAuditBODStatus.Visible = True
        ElseIf PK_ReportingCycle = 80 Then
            lnkManageAssessment.Visible = True
            lnkManageAssessment.NavigateUrl = _UrlParams.EncryptURL("~/Maintenance/ManageHvaAssesments.aspx")
            lnkManageAgencyRemediation.Visible = False
            lnkAuditBODStatus.Visible = False
        Else
            lnkManageAgencyRemediation.Visible = False
            lnkAuditBODStatus.Visible = False
        End If

        If PK_ReportingCycle = 1 Then
            Me.gv_MainStatus.Visible = True
            Me.gv_MainStatusDynamic.Visible = False
            DrawCharts(Me.SqlDS_Chart)
        Else
            Me.gv_MainStatus.Visible = False


            If radTS_Surveys.SelectedTab Is Nothing Then
                radTS_Surveys.SelectedIndex = 0
                radTS_Surveys.Tabs(0).Selected = True
            End If

            If _CAUser.PK_ReportingCycle > 0 Then
                Dim targetTab = radTS_Surveys.FindTabByValue(_CAUser.PK_ReportingCycle)

                If targetTab Is Nothing Then
                    _CAUser.PK_ReportingCycle = -1
                Else
                    radTS_Surveys.SelectedIndex = targetTab.Index
                End If
            End If


            If radTS_Surveys.SelectedTab.PageViewID = "RadPV_Hist" Then
                Me.gv_MainStatusDynamic.Visible = False
            Else

                Me.gv_MainStatusDynamic.Visible = True
                DrawCharts(Me.SqlDS_Chart)
                If (CheckValue(RmaCycles(), Function(x) x.Item("RmaCycles") = _CAUser.PK_ReportingCycle)) Then
                    rmaStatusArea.Visible = True
                    BindRmaStatusData()
                Else
                    rmaStatusArea.Visible = False
                End If
                Me.gv_MainStatusDynamic.Rebind()
            End If
        End If

        Me.Page.MaintainScrollPositionOnPostBack = True
    End Sub
    Private Sub OMBhome_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ScriptManager.RegisterClientScriptInclude(Me, Me.GetType(), "OMBHome", Page.ResolveClientUrl("~/scripts/OMBHome.js"))
        Dim Radbtn_LokAgency As RadButton = FV_DataCallStatus.FindControl("Radbtn_LokAgency")
        Dim btn_Edit As Button = FV_DataCallStatus.FindControl("btn_Edit")
        Dim Radbtn_CritCapabilities As RadButton = FV_DataCallStatus.FindControl("Radbtn_CritCapabilities")
        Dim Radbtn_ReopenSubmission As RadButton = FV_DataCallStatus.FindControl("Radbtn_ReopenSubmission")

        If FV_DataCallStatus.PageCount > 0 Then
            btn_Edit.Visible = True
            Radbtn_LokAgency.Visible = True
            If Not canView Then
                btn_Edit.Visible = False
                Radbtn_LokAgency.Visible = False
                lnkManageAssessment.Visible = False
                lnkManageAgencyRemediation.Visible = False
                Radbtn_CritCapabilities.Visible = False
                Radbtn_ReopenSubmission.Visible = False
            End If
        End If

    End Sub
    Protected Sub BindRmaStatusData()
        Dim oDb As New DataBaseUtils2
        oDb.Parms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)
        Dim dtRmaStatusSource As DataTable
        dtRmaStatusSource = oDb.GetDataTable("GetRMAStats")
        rmaStatus.DataSource = dtRmaStatusSource
        rmaStatus.DataBind()
    End Sub

    Shared Function CheckValue(myTable As DataTable, checkFunc As Func(Of DataRow, Boolean)) As Boolean
        For Each row As DataRow In myTable.Rows
            If checkFunc(row) Then Return True
        Next
        Return False
    End Function

    Protected Function RmaCycles() As DataTable
        Dim oDb As New DataBaseUtils2
        Dim dt As DataTable
        dt = oDb.GetDataTable("SELECT PK_ReportingCycle AS rmaCycles FROM fsma_ReportingCycles WHERE pk_datacall = 9")
        Return dt
    End Function


    Protected Sub DrawCharts(ByVal sqlDataSource As SqlDataSource)

        If _CAUser.PK_ReportingCycle <= 0 Then
            Return
        End If

        Dim oDb As New DataBaseUtils2
        Dim oParms As New Dictionary(Of String, String)
        oParms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)

        Dim dtChartData As DataTable = oDb.GetDataTable("
        SELECT TOP 1 a.[Description], b.Form_Description, a.PK_DataCall 
        FROM fsma_ReportingCycles a 
        INNER JOIN fsma_FormMaster b 
            ON a.PK_ReportingCycle = b.FK_ReportingCycle 
        WHERE a.PK_ReportingCycle = @PK_ReportingCycle
        ", oParms)

        Dim chartForm As String = String.Empty
        Dim DatacallDesc As String = String.Empty
        Dim pkDCall As Integer

        For Each dr As DataRow In dtChartData.Rows
            chartForm = dr("Form_Description").ToString()
            DatacallDesc = dr("Description").ToString()
            pkDCall = dr("PK_DataCall")
        Next

        'Chart1.Visible = False
        ChartArea1.Visible = False
        ChartArea2.Visible = False

        Dim radDb As New DataBaseUtils2
        Dim radChartdt As New DataTable
        'Dim ChTooltip As String = "The following graph displays the number of agencies that have submitted the " & chartForm & " form within the " & DatacallDesc & " data call against the total number of agencies that have been provided the " & chartForm & " form."
        Dim ChTooltip As String = "The following graph displays the number of agencies that have been provided the " & DatacallDesc & " data call, grouped by submission status."


        If pkDCall = 10 Then 'BOD, RepCycle=54
            ChartArea2.Visible = True
            RadChDon.ChartTitle.Text = DatacallDesc
            RadChDon.ToolTip = ChTooltip

            radDb.Parms.Clear()
            radDb.Parms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)
            'radChartdt = radDb.GetDataTable("BOD1902GraphAdmin")
            radChartdt = radDb.GetDataTable("GraphAdmin1")
            RadChDon.DataSource = radChartdt
            RadChDon.DataBind()
        ElseIf pkDCall = 10 Then ' RC = 80 Remediation Plans
            ChartArea1.Visible = False
            divMaingrid.Style.Item("flex-basis") = "98%"
            divChartSpace.Style.Item("flex-basis") = "1%"
        Else
            If pkDCall = 19 Then
                ChTooltip = "The following graph displays the number of agencies that have submitted the " & chartForm & " form within the " & DatacallDesc & " data call against the total number of agencies that have been provided the " & chartForm & " form."
            End If

            ChartArea1.Visible = True
            RadChPie.ChartTitle.Text = DatacallDesc
            RadChPie.ToolTip = ChTooltip
            radDb.Parms.Clear()
            radDb.Parms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)
            radChartdt = radDb.GetDataTable("GraphAdmin1")
            RadChPie.DataSource = radChartdt
            RadChPie.DataBind()
        End If


    End Sub

    Protected Sub GetFormTypeCount(ByVal PK_ReportingCycle_Component As Integer, ByVal DataRowIn As DataRow, ByVal FormTypes As DataView, ByVal AgencyData As DataView)
        For Each Form As DataRowView In FormTypes
            Dim PK_FormType As Integer = Convert.ToInt32(Form("PK_FormType"))
            Dim FormCountCol As String = "FormCount_" & PK_FormType.ToString()
            Dim sumObject As Object
            'AgencyData.table.Compute("Sum(FK_FormType)", "FK_FormType = 1 AND PK_ReportingCycle_Component = ") count for duplicate formtypes
            sumObject = AgencyData.Table.Compute("Count(FK_FormType)", "FK_FormType = " & PK_FormType.ToString() & " AND PK_ReportingCycle_Component= " & PK_ReportingCycle_Component.ToString())
            'set the count for this formtype
            DataRowIn(FormCountCol) = sumObject.ToString()
        Next
    End Sub

    Protected Sub gv_MainStatus_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gv_MainStatus.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim sStat As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Status_Code"))
            Dim bEnable As Boolean = (sStat = "SUBO" Or sStat = "IR" Or sStat = "APO")

            If Not Session("GRUUDADS") Is Nothing Then
                bEnable = (Session("GRUUDADS") = "GRUUDADS")
            End If

            Dim sPK As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_CIO"))
            Dim hl_CIOlink As HyperLink = CType(e.Row.FindControl("hl_CIOlink"), HyperLink)
            If bEnable And bViewForms Then
                hl_CIOlink.NavigateUrl = _UrlParams.EncryptURL("~/FismaForms/CIOannual09_A.aspx?PK_OrgSubmission=" & sPK)
            End If
            hl_CIOlink.ForeColor = RetColor(hl_CIOlink.Text)

            sPK = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_IG"))
            Dim hl_IGlink As HyperLink = CType(e.Row.FindControl("hl_IGlink"), HyperLink)
            If bEnable And bViewForms Then
                hl_IGlink.NavigateUrl = _UrlParams.EncryptURL("~/FismaForms/IGannual09_A.aspx?PK_OrgSubmission=" & sPK)
            End If
            hl_IGlink.ForeColor = RetColor(hl_IGlink.Text)

            sPK = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_IG_NSS"))
            Dim hl_IGHSSlink As HyperLink = CType(e.Row.FindControl("hl_IGHSSlink"), HyperLink)
            If bEnable And bViewForms Then
                hl_IGHSSlink.NavigateUrl = _UrlParams.EncryptURL("~/FismaForms/IGannual09_A.aspx?PK_OrgSubmission=" & sPK)
            End If
            hl_IGHSSlink.ForeColor = RetColor(hl_IGHSSlink.Text)

            sPK = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_SAO"))
            Dim hl_SAOlink As HyperLink = CType(e.Row.FindControl("hl_SAOlink"), HyperLink)
            If bEnable And bViewForms Then
                hl_SAOlink.NavigateUrl = _UrlParams.EncryptURL("~/FismaForms/SAOPannual09_A.aspx?PK_OrgSubmission=" & sPK)
            End If
            hl_SAOlink.ForeColor = RetColor(hl_SAOlink.Text)

            sPK = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_MICRO"))
            Dim hl_MICROlink As HyperLink = CType(e.Row.FindControl("hl_MICROlink"), HyperLink)
            hl_MICROlink.NavigateUrl = ""
            If bEnable And bViewForms Then
                hl_MICROlink.NavigateUrl = _UrlParams.EncryptURL("~/FismaForms/MICROannual09_A.aspx?PK_OrgSubmission=" & sPK)
            End If
            hl_MICROlink.ForeColor = RetColor(hl_MICROlink.Text)

            Dim odb As New DataBaseUtils2
            Dim PK_RC As String

            PK_RC = odb.dbLookUp("SELECT PK_ReportingCycle_Component FROM fsma_ReportingCycle_Components WHERE FK_ReportingCycle = 1 AND FK_Component = " & Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_Component")))

            Dim sStatus As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Status_code"))
            Dim hl_Details As HyperLink = CType(e.Row.FindControl("hl_Details"), HyperLink)
            If sStatus = "SUBO" Or sStatus = "IR" Then
                If bEnable Then
                    hl_Details.NavigateUrl = _UrlParams.EncryptURL("~/OMB_AgReview.aspx?PK_ReportingCycle_Component=" & PK_RC)
                End If
                hl_Details.ImageUrl = "~/Images/smallsuccess1.gif"
                If Me._WorkFlow = CSWorkFlow.IC Then
                    hl_Details.ToolTip = "Submitted to ODNI. Click to Review/Approve/Reject"
                Else
                    hl_Details.ToolTip = "Submitted. Click to Review/Approve/Reject"
                End If
            ElseIf sStatus = "APO" Then
                If bEnable Then
                    hl_Details.NavigateUrl = _UrlParams.EncryptURL("~/OMB_AgReview.aspx?PK_ReportingCycle_Component=" & PK_RC)
                End If
                hl_Details.ImageUrl = "~/Images/1blustar.gif"

                If Me._WorkFlow = CSWorkFlow.IC Then
                    hl_Details.ToolTip = "ODNI Approved. Click to Review"
                Else
                    hl_Details.ToolTip = "Approved. Click to Review"
                End If
            ElseIf sStatus = "LOK" Then
                Dim img As New HtmlImage
                img.Src = "~/Images/redX.gif"
                img.Width = 18
                img.Height = 18
                img.Border = 0
                hl_Details.Controls.Add(img)
                hl_Details.BorderStyle = BorderStyle.None
                'If Session(CAuser.SESSIONKEY).haspermission("ombapsub,fulladmin") Then
                If Session(CAuser.SESSIONKEY).haspermission("ombapsub,OMBISCMPOAMAPSUB,OMBTICPOAMSSAAPSUB,OMBPMCAPSUB,OMBCAPGOALAPSUB,OMBBODPOAMAPSUB,OMBBOD1802POAMSUB,OMBHVAPOAMSUB,OMBEINSTEINSUB,OMBBOD2001SUB") Then
                    hl_Details.ToolTip = "Locked. Click to Access Agency Locking screen"
                    hl_Details.NavigateUrl = "~/LockAgencies.aspx"
                    'hl_Details.Target = "_comp"
                Else
                    hl_Details.NavigateUrl = ""
                End If
            Else
                hl_Details.Visible = False
            End If

            If Not bViewForms Then
                hl_Details.NavigateUrl = ""
                hl_Details.ToolTip = "Inadequate Permissions"
            End If
        End If
    End Sub

    Protected Sub gv_MainStatusDynamic_ItemDataBound1(sender As Object, e As GridItemEventArgs) Handles gv_MainStatusDynamic.ItemDataBound

        Dim odb As DataBaseUtils2 = New DataBaseUtils2()



        If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then

            odb.Parms.Add("@PK_ReportingCycle_Component", Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")))
            odb.Parms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)
            Dim sStatus As String = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Agency_Status_code"))
            'Dim sDatacallStatus As String = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "DatacallStatusCode"))
            Dim bShowReview As Boolean = False
            Dim bEnable As Boolean = (sStatus = "SUBO" Or sStatus = "IR" Or sStatus = "APO")
            Dim hlinkAdmin As HyperLink = CType(e.Item.FindControl("lnkAdmin"), HyperLink)
            Dim hl_Details As HyperLink = CType(e.Item.FindControl("hl_Details"), HyperLink)
            Dim lblAdminLinkHeader As Literal = DirectCast(e.Item.FindControl("AdminLinkHeader"), Literal)

            'Dim bLok As Boolean = (sDatacallStatus = "LOK")
            If Not Session("GRUUDADS") Is Nothing Then
                bEnable = (Session("GRUUDADS") = "GRUUDADS")
            End If
            Dim pkform As String = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_Form"))
            Dim PKFormPart As String = pkform.Split("-")(2) ' Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PKFormPart"))

            Dim AdminLinkColHeaderText As String = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AdminLinkColHeaderText"))
            Dim itemIndex As Integer = e.Item.ItemIndex
            If itemIndex <= 0 Then
                lblAdminLinkHeader.Text = $"<span style='display:none;' class='AdminLinkColHeaderText-{Convert.ToString(itemIndex)}'>{AdminLinkColHeaderText}</span>"
            End If

            If pkform = "2017-Q4-RISKEVA" Then 'Q4 2017 Risk
                'Original CyberSecurity Risk Assessment Worksheet
                Dim OriginalRiskworksheet As HyperLink = DirectCast(e.Item.FindControl("ThenlnkOriginalWorksheet"), HyperLink)
                OriginalRiskworksheet.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/UploadOMBAssessment.aspx?PK_ArtifactType=28&PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")).ToString())
                Dim existingOriginalRiskworksheetArtID As String = odb.dbLookUp("SELECT TOP 1 PK_Artifacts FROM Artifacts WHERE PK_ArtifactType = 28 AND FK_ReportingCycle_Component = @PK_ReportingCycle_Component")
                If Not existingOriginalRiskworksheetArtID = "" Then
                    OriginalRiskworksheet.Text = "View"
                    OriginalRiskworksheet.ForeColor = Color.Green
                    OriginalRiskworksheet.Font.Bold = True
                Else
                    OriginalRiskworksheet.Text = "Upload"
                End If

                'Final CyberSecurity Risk Assessment Worksheet

                Dim FinalRiskworksheet As HyperLink = DirectCast(e.Item.FindControl("lnkFinalWorksheet"), HyperLink)
                FinalRiskworksheet.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/UploadOMBAssessment.aspx?PK_ArtifactType=29&PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")).ToString())
                Dim existingFinalRiskworksheetArtID As String = odb.dbLookUp("SELECT TOP 1 PK_Artifacts FROM Artifacts WHERE PK_ArtifactType = 29 AND FK_ReportingCycle_Component = @PK_ReportingCycle_Component")
                If Not existingFinalRiskworksheetArtID = "" Then
                    FinalRiskworksheet.Text = "View"
                    FinalRiskworksheet.ForeColor = Color.Green
                    FinalRiskworksheet.Font.Bold = True
                Else
                    FinalRiskworksheet.Text = "Upload"
                End If
            End If

            'Original Assessment Link
            Dim originalAssessmentLink As HyperLink = DirectCast(e.Item.FindControl("lnkAssessment"), HyperLink)
            If pkform = "2018-Q4-RISKEVA" Then 'Q42018	Annual Agency Performance Summary 2018 '2018-Q4-RISKEVA
                originalAssessmentLink.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/UploadOMBAssessment.aspx?PK_ArtifactType=31&PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")).ToString())
                Dim existingArtId61 As String = odb.dbLookUp("SELECT TOP 1 PK_Artifacts FROM Artifacts WHERE PK_ArtifactType = 31 AND FK_ReportingCycle_Component = @PK_ReportingCycle_Component")
                If Not existingArtId61 = "" Then
                    originalAssessmentLink.Text = "View"
                    originalAssessmentLink.ForeColor = Color.Green
                    originalAssessmentLink.Font.Bold = True
                Else
                    originalAssessmentLink.Text = "Upload"
                End If
            ElseIf pkform.Contains("Q3-RISKEVA") Or pkform.Contains("Q4-RISKEVA") Then 'Q3 2017 Risk Or Q4 2017   
                'Dim OriginalAssessmentLink As HyperLink = DirectCast(e.Item.FindControl("lnkAssessment"), HyperLink)
                originalAssessmentLink.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/UploadOMBAssessment.aspx?PK_ArtifactType=19&PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")).ToString())
                Dim existingArtId As String = odb.dbLookUp("SELECT TOP 1 PK_Artifacts FROM Artifacts WHERE PK_ArtifactType = 19 AND FK_ReportingCycle_Component = @PK_ReportingCycle_Component")
                If Not existingArtId = "" Then
                    originalAssessmentLink.Text = "View"
                    originalAssessmentLink.ForeColor = Color.Green
                    originalAssessmentLink.Font.Bold = True
                Else
                    originalAssessmentLink.Text = "Upload"
                End If

            End If

            'Final Assessment Link
            Dim FinalAssessmentLink As HyperLink = DirectCast(e.Item.FindControl("lnkFinalAssessment"), HyperLink)

            If pkform = "2018-Q4-RISKEVA" Then 'Q4	2018	Annual Agency Performance Summary 2018
                FinalAssessmentLink.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/UploadOMBAssessment.aspx?PK_ArtifactType=33&PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")).ToString())
                Dim existingFinalArtId61 As String = odb.dbLookUp("SELECT TOP 1 PK_Artifacts FROM Artifacts WHERE PK_ArtifactType = 33 AND FK_ReportingCycle_Component = @PK_ReportingCycle_Component")
                If Not existingFinalArtId61 = "" Then
                    FinalAssessmentLink.Text = "View"
                    FinalAssessmentLink.ForeColor = Color.Green
                    FinalAssessmentLink.Font.Bold = True
                Else
                    FinalAssessmentLink.Text = "Upload"
                End If
            ElseIf pkform.Contains("Q3-RISKEVA") Or pkform.Contains("Q4-RISKEVA") Then 'Q3 2017 Risk Or Q4 2017 Risk
                FinalAssessmentLink.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/UploadOMBAssessment.aspx?PK_ArtifactType=27&PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")).ToString())
                Dim existingFinalArtId As String = odb.dbLookUp("SELECT TOP 1 PK_Artifacts FROM Artifacts WHERE PK_ArtifactType = 27 AND FK_ReportingCycle_Component = @PK_ReportingCycle_Component")
                If Not existingFinalArtId = "" Then
                    FinalAssessmentLink.Text = "View"
                    FinalAssessmentLink.ForeColor = Color.Green
                    FinalAssessmentLink.Font.Bold = True
                Else
                    FinalAssessmentLink.Text = "Upload"
                End If
            End If

            If PKFormPart = "HVA" Or PKFormPart = "RMA" Or PKFormPart = "BOD" Or PKFormPart = "HVAPOAM" Or PKFormPart = "AAPS" Or PKFormPart = "CIO" Or PKFormPart = "NC" Or PKFormPart = "CYBEREO" Then
                hlinkAdmin.Text = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AdminTextLink"))
                hlinkAdmin.NavigateUrl = _UrlParams.EncryptURL(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AdminURLLink")))
            End If
            If PKFormPart = "ED2101A" Then
                hlinkAdmin.Text = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AdminTextLink"))
                hlinkAdmin.NavigateUrl = _UrlParams.EncryptURL("~/Maintenance/ManageSolarWinds.aspx?PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")))
            End If
            If PKFormPart = "BOD" Then
                hl_Details.Enabled = False
            End If
            If PKFormPart = "CYBEREO" Then
                gv_MainStatusDynamic.MasterTableView.GetColumn("AgReviewLink").Visible = False
            End If

            If PKFormPart = "BODVDP" Or PKFormPart = "ED2101" Or PKFormPart = "BOD2201" Then
                lblAdminLinkHeader.Visible = False
            End If

            If PKFormPart = "CIO" Then
                Dim dbCapval As String = hlinkAdmin.Text
                If Not (String.IsNullOrEmpty(dbCapval)) Then
                    Dim sCapval As String() = dbCapval.Split("|")
                    gv_MainStatusDynamic.Columns(2).Visible = True
                    hlinkAdmin.Text = dbCapval(0).ToString()
                    If sCapval(0).ToString() = "Add" Then
                        hlinkAdmin.ToolTip = "Add Critical Capabilities"
                        hlinkAdmin.ForeColor = Color.Blue
                        hlinkAdmin.Font.Bold = True
                        hlinkAdmin.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/ManageCriticalCapabilities.aspx?PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")) & "?CapState=view" & "?frmType=add")

                    ElseIf sCapval(0).ToString() = "View" Then
                        hlinkAdmin.ToolTip = "View Critical Capabilities"
                        hlinkAdmin.ForeColor = Color.Green
                        hlinkAdmin.Font.Bold = True
                        hlinkAdmin.NavigateUrl = _UrlParams.EncryptURL("~/AgencyManagement/ManageCriticalCapabilities.aspx?PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")) & "?CapState=view" & "?frmType=view")

                    Else
                        If Not (String.IsNullOrEmpty(sCapval(1))) Then
                            If sCapval(1).ToString() = "Form Locked" Then
                                hlinkAdmin.Text = "N/A"
                                hlinkAdmin.ToolTip = "Agency Form has been Locked"
                            ElseIf sCapval(1).ToString() = "Form Approved" Then
                                hlinkAdmin.Text = "N/A"
                                hlinkAdmin.ToolTip = "Agency Form has been Approved"
                            Else
                                hlinkAdmin.Text = "N/A"
                                hlinkAdmin.ToolTip = "N/A"
                                hlinkAdmin.ForeColor = Color.Black
                            End If
                        End If
                    End If
                Else
                    hlinkAdmin.Text = "N/A"
                    hlinkAdmin.ToolTip = "N/A"
                    hlinkAdmin.ForeColor = Color.Black
                End If


                If hl_Details Is Nothing Then
                    hl_Details = New HyperLink
                    'hl_Details.Target = "_blank"
                    hl_Details.NavigateUrl = "~/OMB_AgReview.aspx?PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component"))
                    e.Item.Controls(1).Controls.AddAt(0, hl_Details)
                End If
            Else

            End If


            ' determine the workflow to use
            If Me._WorkFlow = CSWorkFlow.IC Then 'If this is Icon make sure that at least 1 of the forms level statuses are SUB or APO to show link
                Dim drv As DataRowView = DirectCast(e.Item.DataItem, DataRowView)

                For Each col As DataColumn In drv.DataView.Table.Columns
                    If col.ColumnName.StartsWith("FormStatusCode") Then
                        Dim pkFormType As Integer = Convert.ToInt32(col.ColumnName.Substring(col.ColumnName.IndexOf("_") + 1))
                        If drv(col.ColumnName).ToString() = "APO" Or ((drv(col.ColumnName).ToString() = "SUBO" Or drv(col.ColumnName).ToString() = "IR") And _CAUser.HasPermission(FismaFormUtils.GetFormTypePermsString(pkFormType, 1))) Then
                            bEnable = True
                            bShowReview = True
                        End If
                    End If
                Next

            End If

            hl_Details.Visible = bEnable

            If bEnable Then
                hl_Details.NavigateUrl = _UrlParams.EncryptURL("~/OMB_AgReview.aspx?PK_ReportingCycle_Component=" & Convert.ToString(DataBinder.Eval(e.Item.DataItem, "PK_ReportingCycle_Component")))

                Select Case sStatus
                    Case "SUBO", "IR"
                        hl_Details.ImageUrl = "~/Images/smallsuccess1.gif"
                        If Me._WorkFlow = CSWorkFlow.IC Then
                            hl_Details.ToolTip = "Submitted to ODNI. Click to Review/Approve/Reject"
                        Else
                            hl_Details.ToolTip = "Submitted. Click to Review/Approve/Reject"
                        End If

                    Case "APO"
                        hl_Details.ImageUrl = "~/Images/1blustar.gif"
                        If Me._WorkFlow = CSWorkFlow.IC Then
                            hl_Details.ToolTip = "ODNI Approved. Click to Review"
                        Else
                            hl_Details.ToolTip = _CAUser.CurrentDataCallOwner + " Approved. Click to Review"
                        End If
                    Case "LOK"
                        Dim img As New HtmlImage
                        img.Src = "~/Images/redX.gif"
                        img.Width = 18
                        img.Height = 18
                        img.Border = 0
                        hl_Details.Controls.Add(img)
                        hl_Details.BorderStyle = BorderStyle.None
                        'If Session(CAuser.SESSIONKEY).haspermission("OMBAPSUB,FULLADMIN") Then
                        If Session(CAuser.SESSIONKEY).haspermission("OMBAPSUB,OMBISCMPOAMAPSUB,OMBTICPOAMSSAAPSUB,OMBISCMPOAMAPSUB,OMBPMCAPSUB,OMBCAPGOALAPSUB,OMBBODPOAMAPSUB,OMBBOD1802POAMSUB,OMBHVAPOAMAPSUB,OMBEINSTEINSUB,OMBBOD2001SUB") Then
                            hl_Details.ToolTip = "Locked. Click to Access Agency Locking screen"
                            hl_Details.NavigateUrl = "~/LockAgencies.aspx"
                            'hl_Details.Target = "_comp"
                        Else
                            hl_Details.NavigateUrl = ""
                        End If
                    Case Else
                        If bShowReview Then
                            hl_Details.ImageUrl = "~/Images/smallsuccess1.gif"
                            hl_Details.ToolTip = "Click here to Review/Approve/Reject Forms"
                        End If
                End Select

                If Not bViewForms Then
                    hl_Details.NavigateUrl = ""
                    hl_Details.ToolTip = "Inadequate Permissions"
                End If
            End If

        End If

    End Sub

    Protected Sub ReopenAgency(sender As Object, e As System.EventArgs)
        Dim odb As New DataBaseUtils2
        odb.Parms.Clear()
        odb.TableName = "fsma_OrgSubmissions"
        Dim oEmail As New eMAIL

        Dim WasReopened As String = ""

        If _CAUser.PK_ReportingCycle = 96 Then
            odb.Parms.Add("@PK_CSAMUser", _CAUser.UserPK)

            WasReopened = odb.dbUpdateRet("ReopenVDPSubmission")

            oEmail.AutoMailCode = "Bod2001VDPReopenAgencies"

            If WasReopened <> -1 Then
                ' if clear, send the email to remind all agencies
                odb.Parms.Clear()

                Dim emailSubject As String = odb.dbLookUp("SELECT EMAIL_DESCRIPTION FROM EMAIL_MASTER WHERE EMAIL_CODE = 'Bod2001VDPReopenAgencies'")

                If oEmail.eMailActive Then

                    oEmail.eMailReason = ""
                    oEmail.eMailSubject = emailSubject
                    oEmail.PK_ReportingCycle = _CAUser.PK_ReportingCycle
                    oEmail.PK_Prime = _CAUser.MyOrg

                    oEmail.AutoGenEmail()
                End If
            End If

        End If

        If _CAUser.PK_ReportingCycle = 108 Then
            odb.Parms.Add("@PK_CSAMUser", _CAUser.UserPK)

            WasReopened = odb.dbUpdateRet("ReopenAllBOD2201Submissions")

            oEmail.AutoMailCode = "Bod2201ReopenAgencies"

            If WasReopened <> -1 Then
                ' if clear, send the email to remind all agencies
                odb.Parms.Clear()

                Dim emailSubject As String = odb.dbLookUp("SELECT EMAIL_DESCRIPTION FROM EMAIL_MASTER WHERE EMAIL_CODE = 'Bod2201ReopenAgencies'")

                If oEmail.eMailActive Then

                    oEmail.eMailReason = ""
                    oEmail.eMailSubject = emailSubject
                    oEmail.PK_ReportingCycle = _CAUser.PK_ReportingCycle
                    oEmail.PK_Prime = _CAUser.MyOrg

                    oEmail.AutoGenEmail()
                End If
            End If

        End If

    End Sub

    'Public Property FormTypeColumnFiltersSelected() As Dictionary(Of String, String)
    '    Get
    '        If ViewState("FormTypeColumnFiltersSelected") Is Nothing Then
    '            ViewState("FormTypeColumnFiltersSelected") = New Dictionary(Of String, String)
    '        End If
    '        Return ViewState("FormTypeColumnFiltersSelected")
    '    End Get

    '    Set(value As Dictionary(Of String, String))
    '        ViewState("FormTypeColumnFiltersSelected") = value
    '    End Set
    'End Property

    'Public Property AgencyTypeColumnFiltersSelected() As Dictionary(Of String, String)
    '    Get
    '        If ViewState("AgencyTypeColumnFiltersSelected") Is Nothing Then
    '            ViewState("AgencyTypeColumnFiltersSelected") = New Dictionary(Of String, String)
    '        End If
    '        Return ViewState("AgencyTypeColumnFiltersSelected")
    '    End Get

    '    Set(value As Dictionary(Of String, String))
    '        ViewState("AgencyTypeColumnFiltersSelected") = value
    '    End Set
    'End Property


    'Protected Sub gv_MainStatusDynamic_ItemCreated(sender As Object, e As GridItemEventArgs) Handles gv_MainStatusDynamic.ItemDataBound
    '    If TypeOf e.Item Is GridFilteringItem Then

    '        Dim filterItem As GridFilteringItem = CType(e.Item, GridFilteringItem)
    '        Dim filterBox As RadComboBox = TryCast(TryCast(e.Item, GridFilteringItem).FindControl("rdbAgencyTypes"), RadComboBox)
    '        Dim agencyTypeVal As String = filterBox.SelectedItem.Text
    '        For Each ctrlCol As Control In filterItem.Controls
    '            If ctrlCol.Controls.Count = 1 Then
    '                Dim hidFilterControl = TryCast(ctrlCol.Controls(0), WebControls.HiddenField)
    '                hidFilterControl.Value = hidFilterControl.Value & "-|-" & agencyTypeVal
    '                Dim controlDetails As String = hidFilterControl.Value
    '                If controlDetails.Contains("-|-") Then
    '                    Dim controlDetailsArray = controlDetails.Split(New String() {"-|-"}, StringSplitOptions.None)
    '                    Dim formType = controlDetailsArray(0)
    '                    Dim agencyType = controlDetailsArray(2)


    '                    AgencyTypeColumnFiltersSelected.Add("AgencyType", agencyType)



    '                    Dim colFilterVals = controlDetailsArray(1).Split("|").ToList()
    '                    Dim rcbColFilter As New RadComboBox()
    '                    rcbColFilter.AutoPostBack = True
    '                    rcbColFilter.ViewStateMode = ViewStateMode.Disabled

    '                    rcbColFilter.Items.Add(New RadComboBoxItem("All", ""))

    '                    Dim filterSelected As String = Nothing

    '                    If FormTypeColumnFiltersSelected IsNot Nothing Then
    '                        If FormTypeColumnFiltersSelected.Keys.Count() > 0 Then
    '                            If FormTypeColumnFiltersSelected.ContainsKey(formType) Then
    '                                filterSelected = FormTypeColumnFiltersSelected(formType)
    '                            End If

    '                        End If
    '                    End If



    '                    For Each value In colFilterVals
    '                        Dim rcbCur = New RadComboBoxItem(value, value)
    '                        If Not String.IsNullOrEmpty(filterSelected) Then
    '                            If filterSelected = value Then
    '                                rcbCur.Selected = True
    '                                Dim gridColum = gv_MainStatusDynamic.MasterTableView.GetColumn("FormStatusCode_" & formType)
    '                                gridColum.CurrentFilterFunction = GridKnownFunction.EqualTo
    '                                gridColum.CurrentFilterValue = value
    '                            End If
    '                        End If
    '                        rcbColFilter.Items.Add(rcbCur)
    '                    Next
    '                    rcbColFilter.Attributes("formType") = formType

    '                    AddHandler rcbColFilter.SelectedIndexChanged, AddressOf rcbColFilter_SelectedIndexChanged
    '                    ctrlCol.Controls.Add(rcbColFilter)
    '                End If
    '            End If
    '        Next

    '    End If

    'End Sub

    'Private Sub rcbColFilter_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
    '    Dim rcbColFilter = CType(sender, RadComboBox)
    '    Dim columnKey = rcbColFilter.Attributes("formType").ToString()

    '    If FormTypeColumnFiltersSelected.ContainsKey(columnKey) Then
    '        FormTypeColumnFiltersSelected(columnKey) = e.Value
    '    Else
    '        FormTypeColumnFiltersSelected.Add(columnKey, e.Value)
    '    End If

    '    If String.IsNullOrEmpty(e.Value) Then
    '        FormTypeColumnFiltersSelected.Remove(columnKey)
    '    End If
    'End Sub


    'Protected Sub SetFormStatusCodeFilters()
    '    Dim sourceTable As DataTable = gv_MainStatusDynamic.DataSource

    '    Dim rowFilter = ""
    '    Dim pos As Integer = 0

    '    If FormTypeColumnFiltersSelected.Count > 0 Then
    '        For Each kv In FormTypeColumnFiltersSelected
    '            Dim rowFilterCur = "FormStatusCode_" & kv.Key & " = '" & kv.Value & "'"
    '            If pos < FormTypeColumnFiltersSelected.Count - 1 Then
    '                rowFilterCur = rowFilterCur & " and "
    '            End If
    '            rowFilter = rowFilter & rowFilterCur
    '            pos += 1
    '        Next
    '    End If

    '    Dim posAg As Integer = 0
    '    If AgencyTypeColumnFiltersSelected.Count > 0 Then
    '        For Each av In AgencyTypeColumnFiltersSelected
    '            If FormTypeColumnFiltersSelected.Count > 0 Then
    '                rowFilter = rowFilter & " and " & av.Key & " = '" & av.Value & "'"
    '            Else
    '                rowFilter = av.Key & " = '" & av.Value & "'"
    '            End If
    '        Next
    '        posAg += 1
    '    End If
    '    sourceTable.DefaultView.RowFilter = rowFilter
    '    gv_MainStatusDynamic.Rebind()

    'End Sub


    'Public Sub sqlStatusOMBDynamic_Selecting(ByVal sender As Object, ByVal e As SqlDataSourceSelectingEventArgs) Handles sqlStatusOMBDynamic.Selecting
    '    e.Command.Parameters("@PK_ReportingCycle").Value = _CAUser.PK_ReportingCycle.ToString
    'End Sub

    Protected Shared Function RetColor(ByVal sCode As String) As Color
        Select Case sCode
            Case "Not Started"
                Return Color.Red
            Case "In Progress"
                Return Color.Tan
            Case "In Review"
                Return Color.BlueViolet
            Case Else
                If sCode.Contains("Submitted") Then
                    Return Color.Green
                ElseIf sCode.Contains("Approved") Then
                    Return Color.Blue
                End If
        End Select
    End Function

    Protected Sub SqlDS_Chart_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles SqlDS_Chart.Selecting
        e.Command.Parameters("@iCycle").Value = _CAUser.PK_ReportingCycle.ToString
    End Sub

    Protected Function GetDataCallPermString() As String
        Dim sbDataCallAccess As New StringBuilder
        'Oct 29 2014 - Vidya - CS-3813 OMB users having FISMA datacall permissions will be granted FISMA admin role, user having TIC datacall permissions will be granted TIC admin role and so forth. 
        'script in Utils/AdminPermissions_to_OMBUsersWithDCPerms.sql handles this grant.
        If (_CAUser.HasPermission("FISMAADM")) Then
            sbDataCallAccess.Append("'OMBFISMADC'")
            'if you have fismaddm you should be able to see all fismadatacalls
            _CAUser.HasPermSetSQLWhereIn("OMBFISMACIO", sbDataCallAccess)
            _CAUser.HasPermSetSQLWhereIn("OMBFISMAIG", sbDataCallAccess)
            _CAUser.HasPermSetSQLWhereIn("OMBFISMASAOP", sbDataCallAccess)
        End If

        ' if they have admin, they should technically have access to the data call
        If (_CAUser.HasPermission("TICADM")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBTICPOAMDC'")
        If (_CAUser.HasPermission("ISCMADM")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBISCMPOAMDC'")
        If (_CAUser.HasPermission("PMCADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBPMCDC'")
        If (_CAUser.HasPermission("CAPGOALADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBCAPGOALDC'")
        If (_CAUser.HasPermission("BODADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBBODPOAMDC'")
        If (_CAUser.HasPermission("BOD1802POAMADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBBOD1802POAMDC'")
        If (_CAUser.HasPermission("HVAADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBHVAPOAMDC'")
        If (_CAUser.HasPermission("EINSTEINADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBEINSTEINDC'")
        If (_CAUser.HasPermission("BOD2001ADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBBOD2001DC'")
        If (_CAUser.HasPermission("ED2101ADMIN")) Then sbDataCallAccess.Append(IIf(sbDataCallAccess.Length > 0, ",", "") & "'OMBED2101DC'")

        Return sbDataCallAccess.ToString
    End Function

    Private Sub LoadSurveyTabs()
        Dim oDB As New DataBaseUtils2
        Dim tab11 As RadTab
        radTS_Surveys.Tabs.Clear()
        'Dim sPerms As String = GetDataCallPermString()
        'Dim sPermsArray = sPerms.Split(",")
        'If sPerms.Length > 0 Then

        Using oConn As SqlConnection = New SqlConnection(oDB.ConnString)
            oConn.Open()
            Dim oRead As SqlDataReader
            Dim cmd = New SqlCommand()
            cmd.Connection = oConn
            Dim sbPermsParams = New StringBuilder()
            'Dim i As Integer = 1
            'For Each perm In sPermsArray
            '    sbPermsParams.Append("@Perm" & i.ToString() & ",")
            '    cmd.Parameters.AddWithValue("@Perm" & i.ToString(), perm.Replace("'", ""))
            '    i += 1
            'Next
            ''remove last comma
            'sbPermsParams.Remove(sbPermsParams.Length - 1, 1)

            'cmd.CommandText = "SELECT fsma_ReportingCycles.PK_ReportingCycle, fsma_ReportingCycles.Description, fsma_ReportingCycles.IsActive FROM fsma_DataCall INNER JOIN fsma_ReportingCycles ON fsma_DataCall.PK_DataCall = fsma_ReportingCycles.PK_DataCall INNER JOIN wf_Permissions ON fsma_DataCall.PK_Permissions = wf_Permissions.PK_Permissions WHERE (wf_Permissions.Access_Code IN (" & sbPermsParams.ToString() & ")) AND (fsma_ReportingCycles.Status IN ('V', 'A')) ORDER BY fsma_ReportingCycles.ScheduledActivation"

            cmd.CommandText = "TabStripAdmin"
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@PK_CSAMusers", _CAUser.UserPK)
            cmd.Parameters.AddWithValue("@PK_Component", _CAUser.MyOrg)
            oRead = cmd.ExecuteReader
            While oRead.Read
                tab11 = New RadTab(oRead.Item("Description").ToString, oRead.Item("PK_ReportingCycle").ToString)
                tab11.Attributes.Add("PK_ReportingCycle_Component", oRead.Item("PK_ReportingCycle_Component"))
                tab11.PageViewID = "RadPV_Survey"
                tab11.Font.Bold = oRead.Item("IsActive")
                tab11.ForeColor = IIf(oRead.Item("IsActive"), tab11.ForeColor, System.Drawing.Color.DarkSlateBlue)
                radTS_Surveys.Tabs.Add(tab11)
            End While
        End Using
        'End If


        If _CAUser.ClosedFormList.Count > 0 Then ' And sPerms.Length > 0 Then
            Dim oDBUtils As New DataBaseUtils2


            Dim sInList As New StringBuilder
            For Each sPK As String In _CAUser.ClosedFormList
                sInList.Append(sPK & ",")
            Next
            sInList.Remove(sInList.Length - 1, 1)
            oDBUtils.Parms.Add("@closedcycleslist", sInList.ToString)
            oDBUtils.Parms.Add("@PK_Component", _CAUser.MyOrg)
            'Dim dtClosedForms As DataTable = oDB.GetDataTable("SELECT fsma_ReportingCycles.PK_ReportingCycle, fsma_ReportingCycles.Description FROM fsma_DataCall INNER JOIN fsma_ReportingCycles ON fsma_DataCall.PK_DataCall = fsma_ReportingCycles.PK_DataCall INNER JOIN wf_Permissions ON fsma_DataCall.PK_Permissions = wf_Permissions.PK_Permissions WHERE (wf_Permissions.Access_Code IN (" & sbDataCallAccess.ToString & ")) AND (fsma_ReportingCycles.IsActive = 1) AND (fsma_ReportingCycles.PK_ReportingCycle IN (" & sInList.ToString & "))")
            'Dim dtClosedForms As DataTable = oDBUtils.GetDataTable("SELECT fsma_ReportingCycles.PK_ReportingCycle, fsma_ReportingCycles.Description FROM fsma_DataCall INNER JOIN fsma_ReportingCycles ON fsma_DataCall.PK_DataCall = fsma_ReportingCycles.PK_DataCall INNER JOIN wf_Permissions ON fsma_DataCall.PK_Permissions = wf_Permissions.PK_Permissions WHERE (fsma_ReportingCycles.PK_ReportingCycle IN (" & sInList.ToString & "))ORDER BY ScheduledActivation")
            Dim dtClosedForms As DataTable = oDBUtils.GetDataTable("SELECT fsma_ReportingCycles.PK_ReportingCycle, fsma_ReportingCycles.Description, rc.PK_ReportingCycle_Component FROM fsma_DataCall INNER JOIN fsma_ReportingCycles ON fsma_DataCall.PK_DataCall = fsma_ReportingCycles.PK_DataCall INNER JOIN wf_Permissions ON fsma_DataCall.PK_Permissions = wf_Permissions.PK_Permissions INNER JOIN fsma_ReportingCycle_Components rc ON rc.FK_ReportingCycle = fsma_ReportingCycles.PK_ReportingCycle WHERE (fsma_ReportingCycles.PK_ReportingCycle IN (@closedcycleslist)) AND rc.FK_Component = @PK_Component ORDER BY ScheduledActivation")
            For Each dr As DataRow In dtClosedForms.Rows
                If radTS_Surveys.FindTabByValue(dr("PK_ReportingCycle").ToString()) Is Nothing Then
                    tab11 = New RadTab(dr("Description").ToString, dr("PK_ReportingCycle").ToString)
                    tab11.Attributes.Add("PK_ReportingCycle_Component", dr("PK_ReportingCycle_Component"))
                    tab11.PageViewID = "RadPV_Survey"
                    tab11.ForeColor = System.Drawing.Color.Firebrick
                    radTS_Surveys.Tabs.Add(tab11)
                End If
            Next
        End If

        'If _CAUser.PK_ReportingCycle > 0 Then
        '    If Not radTS_Surveys.FindTabByValue(_CAUser.PK_ReportingCycle) Is Nothing Then
        '        radTS_Surveys.SelectedIndex = radTS_Surveys.FindTabByValue(_CAUser.PK_ReportingCycle).Index
        '    Else
        '        radTS_Surveys.SelectedIndex = 0
        '    End If
        'Else
        '    radTS_Surveys.SelectedIndex = 0
        '    _CAUser.PK_ReportingCycle = radTS_Surveys.Tabs(0).Value
        'End If

        tab11 = New RadTab("History", "-1")
        tab11.PageViewID = "RadPV_Hist"
        tab11.Attributes.Add("PK_ReportingCycle_Component", "-1")
        tab11.ForeColor = System.Drawing.Color.Firebrick
        radTS_Surveys.Tabs.Add(tab11)

    End Sub

    Protected Sub rg_Cycles_ItemCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles rg_Cycles.ItemCommand
        Select Case e.CommandName
            Case "BUTTPUSH"

                Dim iPK As Integer = DirectCast(e.Item, Telerik.Web.UI.GridDataItem).GetDataKeyValue("PK_ReportingCycle")
                If Not _CAUser.ClosedFormList.Contains(iPK) Then
                    _CAUser.ClosedFormList.Add(iPK)
                    _CAUser.PK_ReportingCycle = iPK
                Else
                    _CAUser.ClosedFormList.Remove(iPK)
                    If _CAUser.PK_ReportingCycle = iPK Then
                        _CAUser.PK_ReportingCycle = radTS_Surveys.Tabs(0).Value
                    End If
                End If

                Response.Redirect("~/OMBhome.aspx")

        End Select
    End Sub

    Protected Sub rg_Cycles_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles rg_Cycles.NeedDataSource
        If Not e.IsFromDetailTable Then
            Dim oDb As New DataBaseUtils2
            Dim oParms As New Dictionary(Of String, String)
            oParms.Add("@PK_Component", _CAUser.MyOrg)
            Dim sPerms As String = GetDataCallPermString()
            If sPerms.Length > 0 Then
                rg_Cycles.DataSource = oDb.GetDataTable("
                    SELECT r.PK_ReportingCycle, 
                    r.Year, 
                    r.Description, 
                    CASE [Status] WHEN 'C' THEN 'Closed' WHEN 'V' THEN 'Review Only' WHEN 'P' THEN 'Under Construction' WHEN 'A' THEN 'Active Cycle' END AS StatusDesc, 
                    [Status], 
                    r.ScheduledActivation, 
                    r.ActualActivation, 
                    r.ScheduledClosed, 
                    r.ActualClosed, 
                    r.Notes, 
                    @PK_Component AS PK_Component, 
                    CASE PK_ReportingCycle 
	                    WHEN 1 THEN (SELECT TOP (1) Status_Code FROM [Component List] WHERE PK_Component = @PK_Component) 
	                    ELSE (SELECT TOP (1) ISNULL(DatacallStatusCode,Status_Code) FROM fsma_ReportingCycle_Components WHERE FK_ReportingCycle = r.PK_ReportingCycle AND FK_Component = @PK_Component) 
                    END AS Status_Code, 
                    r.IsActive, 
                    rc.PK_ReportingCycle_Component 
                    FROM fsma_ReportingCycles r
                    INNER JOIN fsma_DataCall dc 
	                    ON r.PK_DataCall = dc.PK_DataCall 
                    INNER JOIN wf_Permissions p
	                    ON dc.PK_Permissions = p.PK_Permissions 
                    LEFT OUTER JOIN (SELECT DISTINCT FK_ReportingCycle, PK_ReportingCycle_Component FROM fsma_ReportingCycle_Components WHERE FK_Component = @PK_Component) AS rc 
                    ON rc.FK_ReportingCycle = r.PK_ReportingCycle 
                    WHERE p.Access_Code IN (" & sPerms & ") 
                    ORDER BY r.PK_ReportingCycle DESC
                    ", oParms)
            End If
        End If
    End Sub

    Protected Sub rg_Cycles_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rg_Cycles.ItemDataBound
        If e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Then
            Dim btn_View As System.Web.UI.WebControls.Button
            Dim GrdDataItem As GridDataItem = TryCast(e.Item, GridDataItem)
            If IsNumeric(GrdDataItem.GetDataKeyValue("PK_ReportingCycle")) Then
                If _CAUser.ClosedFormList.Contains(GrdDataItem.GetDataKeyValue("PK_ReportingCycle")) Then
                    btn_View = TryCast(GrdDataItem("BUTTPUSH").Controls(0), System.Web.UI.WebControls.Button)
                    btn_View.Text = "Remove Tab from display"
                    btn_View.OnClientClick = ""
                Else
                    btn_View = TryCast(GrdDataItem("BUTTPUSH").Controls(0), System.Web.UI.WebControls.Button)
                    Dim sStatus As String = Convert.ToString(DataBinder.Eval(GrdDataItem.DataItem, "Status"))
                    If sStatus = "V" Or sStatus = "A" Then
                        btn_View.Enabled = False
                        btn_View.Text = IIf(sStatus = "A", "Active", "Review Mode")
                        btn_View.BorderStyle = BorderStyle.None
                    End If
                End If
                Dim hl_wf_Hist As System.Web.UI.WebControls.HyperLink = TryCast(GrdDataItem("hl_wf_Hist").Controls(0), System.Web.UI.WebControls.HyperLink)
                hl_wf_Hist.NavigateUrl = _UrlParams.EncryptURL("OMB_ReportCycleHistory.aspx?PK_ReportingCycle=" & GrdDataItem.GetDataKeyValue("PK_ReportingCycle").ToString)
                hl_wf_Hist.ImageUrl = "Images\audit.gif"
                hl_wf_Hist.Target = "_new"
            End If
        End If
    End Sub

    Protected Sub SqlDS_DataCallStatus_Selecting(sender As Object, e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles SqlDS_DataCallStatus.Selecting
        e.Command.Parameters("@PK_ReportingCycle").Value = _CAUser.PK_ReportingCycle
    End Sub

    Public Property EditMode() As Boolean
        Get
            If ViewState("EditMode") Is Nothing Then
                ViewState("EditMode") = False
            End If
            Return ViewState("EditMode")
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditMode") = value
        End Set
    End Property

    Protected _AgencyList As List(Of String)
    Protected _AgencyTypeList As List(Of String)

    Protected Sub FV_DataCallStatus_DataBound(sender As Object, e As System.EventArgs) Handles FV_DataCallStatus.DataBound
        Dim Radbtn_LokAgency As RadButton = sender.findcontrol("Radbtn_LokAgency")
        Dim Radbtn_CritCap As RadButton = sender.findcontrol("Radbtn_CritCapabilities")
        Dim Radbtn_Reopen As RadButton = sender.findcontrol("Radbtn_ReopenSubmission")

        Dim oDBUtils As New DataBaseUtils2
        Dim rptCycleSql As String = "SELECT PK_ReportingCycle FROM fsma_ReportingCycles WHERE PK_ReportingCycle > 1 AND PK_DataCall = 1 and Status Not in ('C')"
        Dim oRptCycle As DataTable = oDBUtils.GetDataTable(rptCycleSql)
        Dim varRptCycle As String
        Dim list As New List(Of Integer)

        For Each row As DataRow In oRptCycle.Rows
            varRptCycle = row.Item("PK_ReportingCycle").ToString()
            list.Add(varRptCycle)
        Next

        Dim i As Integer

        If radTS_Surveys.SelectedTab IsNot Nothing Then
            'Reopen submission button show for VDP & BOD 22-01 
            If radTS_Surveys.SelectedTab.Text.Contains("BOD 20-01") Then
                If Not radTS_Surveys.FindTabByValue(96) Is Nothing Then
                    If (radTS_Surveys.SelectedIndex = radTS_Surveys.FindTabByValue(96).Index) Then
                        Radbtn_Reopen.Visible = True
                    Else
                        If Not Radbtn_Reopen Is Nothing Then
                            Radbtn_Reopen.Visible = False
                        End If
                    End If
                End If
            End If

            'BOD 22-01
            If radTS_Surveys.SelectedTab.Text.Contains("BOD 22-01") Then
                If Not radTS_Surveys.FindTabByValue(108) Is Nothing Then
                    If (radTS_Surveys.SelectedIndex = radTS_Surveys.FindTabByValue(108).Index) Then
                        Radbtn_Reopen.Visible = True
                    Else
                        If Not Radbtn_Reopen Is Nothing Then
                            Radbtn_Reopen.Visible = False
                        End If
                    End If
                End If
            End If
        End If
        For i = 0 To list.Count - 1
            If (list.Item(i).CompareTo(_CAUser.PK_ReportingCycle) = 0) Then
                Radbtn_CritCap.Visible = True
            End If
        Next i

        'If Not Radbtn_LokAgency Is Nothing Then
        '    Dim fv As FormView = sender
        '    Dim row As DataRowView = fv.DataItem
        '    Radbtn_LokAgency.Visible = _CAUser.HasPermission("ombapsub") And row("IsActive")
        'End If

    End Sub

    Protected Sub FV_DataCallStatus_ItemCommand(ByVal sender As Object, e As FormViewCommandEventArgs) Handles FV_DataCallStatus.ItemCommand
        Dim bSwitchMode As Boolean = True
        Dim oDB As New DataBaseUtils("fsma_ReportingCycles", "PK_ReportingCycle", Me.radTS_Surveys.SelectedTab.Value)
        Dim aControlsToEdit() As String = {"CBpl_Status", "CBDate_OpenPlan", "CBDate_OpenAct", "CBDate_ClosePlan", "CBDate_CloseAct", "CBNar_CallSplash", "CBCycleInterval", "CBDate_Interval"}
        Select Case e.CommandName
            Case "EditMe"
                bSwitchMode = True
            Case "SaveMe"
                bSwitchMode = oDB.SaveData(aControlsToEdit, FV_DataCallStatus, _CAUser.PK_ReportingCycle)
                'if _CAUser.PK_ReportingCycle is not removed from the collection object, when changing the status from closed to active, there are 2 tabs of the same form
                '_CAUser.ClosedFormList.Remove(_CAUser.PK_ReportingCycle)
                LoadSurveyTabs()
                rg_Cycles.Rebind()
                PostProc()
                Response.Redirect("~/OMBhome.aspx")
                If bSwitchMode Then
                    FV_DataCallStatus.DataBind()
                    PostProc()
                End If
            Case "ReopenSubmission"
                ReopenAgency(sender, e)

                bSwitchMode = False
        End Select
        If bSwitchMode Then
            EditMode = Not EditMode
            oDB.SetControlStates(FV_DataCallStatus, aControlsToEdit, EditMode, True)
        End If

        Dim btn_Save As Button = sender.findcontrol("btn_Save")
        If Not btn_Save Is Nothing Then
            btn_Save.Visible = EditMode
        End If

        Dim Radbtn_Reopen As RadButton = sender.findcontrol("Radbtn_ReopenSubmission")
        If Not Radbtn_Reopen Is Nothing Then
            Radbtn_Reopen.Visible = Not EditMode
        End If

        'Dim div_DataCallStatus = CType(Me.Page.FindControl("div_DataCallStatus"), System.Web.UI.HtmlControls.HtmlGenericControl)

        'disables all the other parts of the form if in editmode.

        If EditMode Then
            'Chart1.Visible = False
            If gv_Forms IsNot Nothing Then
                gv_Forms.Visible = False
            End If

            If gv_MainStatusDynamic IsNot Nothing Then
                gv_MainStatusDynamic.Visible = False
            End If

            'ddlMainStatusDynamicColumn.Visible = False
            'ddlMainStatusDynamicValue.Visible = False
            'lblFilterBy.Visible = False
            'lblFilterValue.Visible = False
            If Not radTS_Surveys.FindTabByText("History") Is Nothing Then
                radTS_Surveys.FindTabByText("History").Visible = False
            End If
            div_DataCallStatus.Style("width") = "100%"
        Else
            'Chart1.Visible = True
            gv_Forms.Visible = True
            gv_MainStatusDynamic.Visible = True
            'ddlMainStatusDynamicColumn.Visible = True
            'ddlMainStatusDynamicValue.Visible = True
            'lblFilterBy.Visible = True
            'lblFilterValue.Visible = True
            If Not radTS_Surveys.FindTabByText("History") Is Nothing Then
                radTS_Surveys.FindTabByText("History").Visible = True
            End If
            div_DataCallStatus.Style("width") = "75%"
        End If


    End Sub

    Protected Sub FV_DataCallStatus_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles FV_DataCallStatus.PreRender
        Dim CBCycleInterval As CustomControls_CBpicklist2 = FV_DataCallStatus.FindControl("CBCycleInterval")
        Dim CBDate_Interval As CBDate = FV_DataCallStatus.FindControl("CBDate_Interval")

        If CBCycleInterval Is Nothing Or CBDate_Interval Is Nothing Then
            Return
        End If

        If CBCycleInterval.Value = "BW" Or CBCycleInterval.Value = "W" Or CBCycleInterval.Value = "M" Then
            CBDate_Interval.Visible = True
        Else
            CBDate_Interval.Visible = False
        End If
        If CBCycleInterval.DataChanged = True Then
            If _CAUser.PK_ReportingCycle <> 80 Then
                Me.gv_MainStatusDynamic.Rebind()
            End If
            If _CAUser.PK_ReportingCycle = 80 Then
                rgBOD1802Poam.Rebind()
            End If
        End If

    End Sub

    Private Sub PostProc()
        Dim CBpl_Status As CbBaseUserControl = FV_DataCallStatus.FindControl("CBpl_Status")
        Dim oDB As New DataBaseUtils2
        Dim oParms As New Dictionary(Of String, String)
        oParms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)
        If CBpl_Status.Value = "A" Then
            oDB.dbUpdate("UPDATE fsma_ReportingCycles SET IsActive = 1 WHERE ISNULL(IsActive,0) <> 1 AND PK_ReportingCycle = @PK_ReportingCycle", oParms)
            oDB.dbUpdate("UPDATE fsma_ReportingCycles SET ActualActivation = GetDate() WHERE ActualActivation IS NULL AND PK_ReportingCycle = @PK_ReportingCycle", oParms)
        Else
            oDB.dbUpdate("UPDATE fsma_ReportingCycles SET IsActive = 0 WHERE ISNULL(IsActive,0) <> 0 AND PK_ReportingCycle = @PK_ReportingCycle", oParms)
        End If
        If CBpl_Status.Value = "C" Then
            oDB.dbUpdate("UPDATE fsma_ReportingCycles SET ActualClosed = GetDate() WHERE ActualClosed IS NULL AND PK_ReportingCycle = @PK_ReportingCycle", oParms)
        End If
    End Sub

    'Protected Sub FV_DataCallStatus_Load(sender As Object, e As System.EventArgs) Handles FV_DataCallStatus.Load
    '    'Dim btn_Edit As Button = sender.findcontrol("btn_Edit")
    '    'Dim Radbtn_LokAgency As RadButton = sender.findcontrol("Radbtn_LokAgency")

    '    'If Not btn_Edit Is Nothing Then
    '    '    btn_Edit.Visible = _CAUser.HasPermission("ombapsub,OMBISCMPOAMAPSUB,OMBTICPOAMSSAAPSUB,OMBPMCAPSUB,OMBCAPGOALAPSUB,OMBBODPOAMAPSUB,OMBBOD1802POAMSUB,OMBHVAPOAMAPSUB,OMBEINSTEINSUB,OMBBOD1802POAMSUB,OMB2001SUB")
    '    'End If


    'End Sub

    Protected Sub PrePopMe()
        Dim CBpl_Status As CustomControls_CBpicklist2 = FV_DataCallStatus.FindControl("CBpl_Status")
        If Not CBpl_Status Is Nothing Then
            If CBpl_Status.PickItemCollection.Count = 0 Then
                CBpl_Status.PickItemCollection.Add(New ListItem("Under Construction", "P"))
                CBpl_Status.PickItemCollection.Add(New ListItem("Review Only", "V"))
                CBpl_Status.PickItemCollection.Add(New ListItem("Active", "A"))
                CBpl_Status.PickItemCollection.Add(New ListItem("Closed", "C"))
            End If
        End If


    End Sub

    Protected Sub CycleIntervals()
        Dim CBCycleInterval As CustomControls_CBpicklist2 = FV_DataCallStatus.FindControl("CBCycleInterval")
        If Not CBCycleInterval Is Nothing Then
            CBCycleInterval.PickItemCollection.Add(New ListItem("Annual", "A"))
            CBCycleInterval.PickItemCollection.Add(New ListItem("Quarterly", "Q"))
            CBCycleInterval.PickItemCollection.Add(New ListItem("Monthly", "M"))
            CBCycleInterval.PickItemCollection.Add(New ListItem("Weekly", "BW"))
            CBCycleInterval.PickItemCollection.Add(New ListItem("Bi-Weekly", "W"))
        End If
    End Sub

    Protected Sub LoadFormList()
        'bViewForms = _CAUser.hasPermission2(Nothing, "Admin-FormViewList")
        If bViewForms Then ' _CAUser.HasPermission("AUTHEDIT,AUTHVIEW,FISMAADM,ISCMADM,TICADM,PMCADMIN,CAPGOALADMIN,BODADMIN,HVADMIN,BOD1802POAMADMIN,EINSTEINADMIN,BOD2001ADMIN") And _CAUser.PK_ReportingCycle > 0 Then
            gv_Forms.DataSourceID = Nothing
            Dim adminAcro As String = ConfigurationManager.AppSettings("AdminAcronym")
            If adminAcro Is Nothing Then adminAcro = "FNR"
            Dim oDb As New DataBaseUtils2
            oDb.Parms.Add("@FK_ReportingCycle", _CAUser.PK_ReportingCycle)
            oDb.Parms.Add("@Acronym", adminAcro)
            gv_Forms.DataSource = oDb.GetDataTable("SELECT fsma_FormMaster.PK_Form, fsma_FormMaster.Report_Year, fsma_FormMaster.Form_Description, fsma_FormMaster.InternalForm, fsma_FormMaster.TypeCode, fsma_FormMaster.IntervalCode, fsma_FormMaster.Period, fsma_FormMaster.FK_ReportingCycle, fsma_FormMaster.FK_FormType, fsma_FormMaster.CrystalReportForm, fsma_OrgSubmissions.PK_OrgSubmission FROM fsma_OrgSubmissions INNER JOIN fsma_FormMaster ON fsma_OrgSubmissions.PK_Form = fsma_FormMaster.PK_Form INNER JOIN fsma_ReportingCycle_Components ON fsma_OrgSubmissions.FK_ReportingCycle_Component = fsma_ReportingCycle_Components.PK_ReportingCycle_Component INNER JOIN [Component List] ON fsma_ReportingCycle_Components.FK_Component = [Component List].PK_Component WHERE (fsma_FormMaster.FK_ReportingCycle = @FK_ReportingCycle) AND ([Component List].Acronym = @Acronym)")
            gv_Forms.DataBind()
        Else
            gv_Forms.Visible = False
        End If
    End Sub
    'Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
    '    Page.StyleSheetTheme = Page.Theme
    '    'Page.Theme = Nothing
    'End Sub

    Protected Sub gv_Forms_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gv_Forms.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim sPK As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "PK_OrgSubmission"))
            Dim sForm As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "InternalForm"))
            Dim hl_Form As HyperLink = CType(e.Row.FindControl("hl_Form"), HyperLink)
            Dim urlToUse As String = "~/" & sForm & "?PK_Orgsubmission=" & sPK & "&AdminPreview=1"
            'If _UrlParams.GetParm("AdminPreview") = "1" Then
            '    urlToUse += "&AdminPreview=1"
            'End If
            hl_Form.NavigateUrl = _UrlParams.EncryptURL(urlToUse)
        End If
    End Sub

    'Private Function GetDirection(ByVal expression As String) As String
    '    Dim direction As String = ""

    '    If String.IsNullOrEmpty(ViewState(expression)) Then
    '        direction = "ASC"
    '        ViewState(expression) = "DESC"
    '    Else
    '        direction = ViewState(expression)
    '        If direction = "ASC" Then
    '            ViewState(expression) = "DESC"
    '        Else
    '            ViewState(expression) = "ASC"
    '        End If
    '    End If

    '    ViewState("LastSortExpression") = expression

    '    Return direction
    'End Function

    Protected Sub RadMultiPage1_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadMultiPage1.PreRender

        If Not (ViewState("ReportingCycle") = _CAUser.PK_ReportingCycle.ToString()) Then
            ViewState("ReportingCycle") = _CAUser.PK_ReportingCycle.ToString()
        End If

    End Sub


    'Private Function Sort(ByRef dv As DataView, ByVal col As String, ByVal direction As String) As DataView

    '    If direction = "ASC" Then
    '        dv.Sort = col & " DESC"
    '    Else
    '        dv.Sort = col & " ASC"
    '    End If

    '    Return dv
    'End Function

    Protected Sub gv_MainStatusDynamic_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles gv_MainStatusDynamic.NeedDataSource
        If _CAUser.PK_ReportingCycle <> 80 And _CAUser.PK_ReportingCycle > 0 Then
            gv_MainStatusDynamic.DataSource = CreateDynamicGridDataSource()
        End If
    End Sub

    Private Function GenConn() As String
        Dim config As System.Configuration.Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~")
        Return config.ConnectionStrings.ConnectionStrings("CAclientConnectionString").ToString
    End Function

    Private Function GetDatasetOMBHome() As DataSet

        Using oConn As SqlConnection = New SqlConnection(GenConn())
            Dim retOMBHomeSprocRecs As DataSet = New DataSet
            Dim adapter1 As New SqlDataAdapter
            Dim Query As String = "GetOMBHomeFormStats"
            Dim cmd As SqlCommand = New SqlCommand(Query, oConn)
            If Not Query.Contains(" ") Then 'if no space, assume stored procedure
                cmd.CommandType = CommandType.StoredProcedure
            End If
            cmd.Parameters.AddWithValue("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)
            adapter1.SelectCommand = cmd
            oConn.Open()
            Try
                adapter1.Fill(retOMBHomeSprocRecs)
            Finally
                oConn.Close()
            End Try
            Return retOMBHomeSprocRecs
        End Using
    End Function

    'Public Class ColumnDropDownTemplate
    '    Implements System.Web.UI.ITemplate

    '    Protected _PkFormType As String
    '    Protected _SourceTable As DataTable
    '    Protected _SortValues As List(Of String)
    '    Protected _rcbFilter As New RadComboBox

    '    Sub New(pkFormType As String, sortValues As List(Of String))
    '        _PkFormType = pkFormType
    '        _SortValues = sortValues
    '        '_ColumnName = columnName
    '        '_SourceTable = sourceTable
    '    End Sub

    '    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
    '        Dim filterHolder = New WebControls.HiddenField()
    '        filterHolder.Value = _PkFormType & "-|-" & String.Join("|", _SortValues.ToArray())
    '        container.Controls.Add(filterHolder)

    '        'For Each value In _SortValues
    '        '    _rcbFilter.Items.Add(New RadComboBoxItem(value, _PkFormType & "|" & value))
    '        'Next
    '        'AddHandler _rcbFilter.DataBinding, AddressOf _rcbFilter_DataBinding
    '        '_rcbFilter.ViewStateMode = ViewStateMode.Disabled ' must disable viewstate or throws error
    '        'container.Controls.Add(_rcbFilter)
    '    End Sub
    '    Private Sub _rcbFilter_DataBinding(sender As Object, e As EventArgs)
    '        Throw New NotImplementedException()
    '    End Sub
    'End Class

    Function CreateDynamicGridDataSource() As DataTable

        Dim SprocData As DataSet = GetDatasetOMBHome()
        'Dim oDbDS As New DataBaseUtils2

        'While gv_MainStatusDynamic.Columns.Count > 6
        'While gv_MainStatusDynamic.Columns.Count > 9
        'While gv_MainStatusDynamic.Columns.Count > 13
        While gv_MainStatusDynamic.Columns.Count > 10
            gv_MainStatusDynamic.Columns.RemoveAt(gv_MainStatusDynamic.Columns.Count - 1)
        End While




        Dim AgencyData As DataView = SprocData.Tables(0).DefaultView
        Dim FormTypes As DataView = SprocData.Tables(1).DefaultView

        Dim bEnable As Nullable(Of Boolean)
        If Not Session("GRUUDADS") Is Nothing Then
            bEnable = (Session("GRUUDADS") = "GRUUDADS")
        End If
        'build the columns for the forms
        For Each FormType As DataRowView In FormTypes
            Dim FormTemplateFieldCol As GridTemplateColumn
            FormTemplateFieldCol = New GridTemplateColumn()
            Dim PkformTypeCur = FormType("PK_FormType").ToString()
            FormTemplateFieldCol.AllowFiltering = False
            FormTemplateFieldCol.HeaderText = FormType("Type").ToString()
            If _CAUser.PK_ReportingCycle = 48 Or _CAUser.PK_ReportingCycle = 50 Or _CAUser.PK_ReportingCycle = 61 Or _CAUser.PK_ReportingCycle = 76 Then
                FormTemplateFieldCol.HeaderText = "Agency Performance Response"
            End If

            Dim colStatus As GridTemplateColumn = gv_MainStatusDynamic.MasterTableView.GetColumn("RMAOverrideStatus")
            If colStatus IsNot Nothing Then
                If _CAUser.PK_ReportingCycle = 76 Then
                    colStatus.HeaderText = "AAPS Status"
                Else
                    colStatus.HeaderText = "OMB Override Status"
                End If
            End If

            'Dim colName = "FormStatusCode_" & PkformTypeCur
            'FormTemplateFieldCol.UniqueName = colName
            FormTemplateFieldCol.ItemTemplate = New FormColumnTemplate(PkformTypeCur, bEnable, _CAUser, _WorkFlow)
            FormTemplateFieldCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            'FormTemplateFieldCol.DataField = colName
            ' FormTemplateFieldCol.SortExpression = colName

            'Dim filterDropDownData = (From d In AgencyData.Table.AsEnumerable() Where (d.Field(Of Integer)("FK_FormType")).ToString() = PkformTypeCur Select d.Field(Of String)("Form_Status_code")).Distinct().ToList()
            'filterDropDownData.Sort()
            'FormTemplateFieldCol.FilterTemplate = New ColumnDropDownTemplate(PkformTypeCur, filterDropDownData)

            'insert the dynamic column before the datafeed uploads
            Me.gv_MainStatusDynamic.Columns.Add(FormTemplateFieldCol)
        Next
        'now build the table to bind to basically flatten the query with the forms as new columns each
        Dim DataToBind As DataTable = New DataTable()
        Dim DataColPK As New DataColumn("PK_ReportingCycle_Component")
        Dim DataColPKComp As New DataColumn("PK_Component")
        Dim DataColAgency As New DataColumn("Agency")
        Dim DataColAgencyType As New DataColumn("AgencyType")
        Dim DataColAgencyStatus As New DataColumn("Agency_Status")
        Dim DataColAgencyStatusCode As New DataColumn("Agency_Status_code")
        Dim DataColDatacallStatusCode As New DataColumn("DatacallStatusCode")
        Dim DataColdfUpload As New DataColumn("dfUpload")
        Dim DataColCapabilityVal As New DataColumn("AdminURLLink")
        Dim DataColAdminTextLink As New DataColumn("AdminTextLink")
        Dim DataColAdminLinkColHeaderText As New DataColumn("AdminLinkColHeaderText")
        Dim DataColPkForm As New DataColumn("PK_Form")
        Dim DataColRMAOverrideStatus As New DataColumn("RMAOverrideStatus")
        Dim DataColBodIssuedDate As New DataColumn("BodIssuedDate")
        'add singular data columns (repeating rows)
        DataToBind.Columns.Add(DataColPK)
        DataToBind.Columns.Add(DataColPKComp)
        DataToBind.Columns.Add(DataColAgency)
        DataToBind.Columns.Add(DataColAgencyType)
        DataToBind.Columns.Add(DataColAgencyStatus)
        DataToBind.Columns.Add(DataColAgencyStatusCode)
        DataToBind.Columns.Add(DataColdfUpload)
        DataToBind.Columns.Add(DataColDatacallStatusCode)
        DataToBind.Columns.Add(DataColCapabilityVal)
        DataToBind.Columns.Add(DataColAdminTextLink)
        DataToBind.Columns.Add(DataColAdminLinkColHeaderText)
        DataToBind.Columns.Add(DataColPkForm)
        DataToBind.Columns.Add(DataColRMAOverrideStatus)
        DataToBind.Columns.Add(DataColBodIssuedDate)
        'add dynamic columns that are unique for each repeat
        For Each FormType As DataRowView In FormTypes
            Dim DataColFormStatus As New DataColumn("FormStatus_" & FormType("PK_FormType").ToString())
            Dim DataColFormStatusCode As New DataColumn("FormStatusCode_" & FormType("PK_FormType").ToString())
            Dim DataColOrg As New DataColumn("OrgSubmission_" & FormType("PK_FormType").ToString())
            Dim DataColInternalForm As New DataColumn("InternalForm_" & FormType("PK_FormType").ToString())
            Dim DataColFormCount As New DataColumn("FormCount_" & FormType("PK_FormType").ToString())
            Dim DataColDataCallStatus As New DataColumn("DatacallStatusCode_" & FormType("PK_FormType").ToString())
            DataToBind.Columns.Add(DataColFormStatus)
            DataToBind.Columns.Add(DataColFormStatusCode)
            DataToBind.Columns.Add(DataColOrg)
            DataToBind.Columns.Add(DataColInternalForm)
            DataToBind.Columns.Add(DataColFormCount)
            DataToBind.Columns.Add(DataColDataCallStatus)
        Next

        Dim PkLast As Integer = 0
        Dim PkCurrent As Integer = 0
        Dim DataRowToAdd As DataRow = DataToBind.NewRow()

        If AgencyData.Count > 0 Then

            ' do first manually
            PkCurrent = AgencyData(0)("PK_ReportingCycle_Component")
            PkLast = PkCurrent
            DataRowToAdd("PK_ReportingCycle_Component") = PkCurrent
            DataRowToAdd("PK_Component") = AgencyData(0)("PK_Component")
            DataRowToAdd("Agency") = AgencyData(0)("Agency")
            DataRowToAdd("AgencyType") = AgencyData(0)("ComponentGroup")
            DataRowToAdd("Agency_Status") = AgencyData(0)("Agency_Status")
            DataRowToAdd("Agency_Status_code") = AgencyData(0)("Agency_Status_code")
            DataRowToAdd("DatacallStatusCode") = AgencyData(0)("DatacallStatusCode")
            DataRowToAdd("AdminURLLink") = AgencyData(0)("AdminURLLink")
            DataRowToAdd("AdminTextLink") = AgencyData(0)("AdminTextLink")
            DataRowToAdd("AdminLinkColHeaderText") = AgencyData(0)("AdminLinkColHeaderText")
            DataRowToAdd("PK_Form") = AgencyData(0)("PK_Form")
            DataRowToAdd("RMAOverrideStatus") = AgencyData(0)("RMAOverrideStatus")
            DataRowToAdd("BodIssuedDate") = AgencyData(0)("BodIssuedDate")

            DataRowToAdd("FormStatus_" & AgencyData(0)("FK_FormType").ToString()) = AgencyData(0)("Form_Status")
            DataRowToAdd("FormStatusCode_" & AgencyData(0)("FK_FormType").ToString()) = AgencyData(0)("Form_Status_Code")
            DataRowToAdd("OrgSubmission_" & AgencyData(0)("FK_FormType").ToString()) = AgencyData(0)("PK_OrgSubmission")
            DataRowToAdd("InternalForm_" & AgencyData(0)("FK_FormType").ToString()) = AgencyData(0)("InternalForm")
            DataRowToAdd("DatacallStatusCode_" & AgencyData(0)("FK_FormType").ToString()) = AgencyData(0)("DatacallStatusCode")
            DataRowToAdd("dfUpload") = AgencyData(0)("dfUpload")
            DataRowToAdd("AdminURLLink") = AgencyData(0)("AdminURLLink")
            DataRowToAdd("AdminTextLink") = AgencyData(0)("AdminTextLink")
            DataRowToAdd("BodIssuedDate") = AgencyData(0)("BodIssuedDate")
            DataRowToAdd("AdminLinkColHeaderText") = AgencyData(0)("AdminLinkColHeaderText")
            DataRowToAdd("PK_Form") = AgencyData(0)("PK_Form")
            DataRowToAdd("RMAOverrideStatus") = AgencyData(0)("RMAOverrideStatus")

            GetFormTypeCount(PkCurrent, DataRowToAdd, FormTypes, AgencyData)
            ' start at the second
            For index As Integer = 1 To AgencyData.Count - 1
                Dim dv As DataRowView = AgencyData(index)

                PkCurrent = Convert.ToInt32(dv("PK_ReportingCycle_Component"))

                If PkLast <> PkCurrent Then
                    ' add the last one
                    DataToBind.Rows.Add(DataRowToAdd)

                    'create the next one
                    DataRowToAdd = DataToBind.NewRow()
                    DataRowToAdd("PK_ReportingCycle_Component") = dv("PK_ReportingCycle_Component")
                    DataRowToAdd("PK_Component") = dv("PK_Component")
                    DataRowToAdd("Agency") = dv("Agency")
                    DataRowToAdd("AgencyType") = dv("ComponentGroup")
                    DataRowToAdd("Agency_Status") = dv("Agency_Status")
                    DataRowToAdd("Agency_Status_code") = dv("Agency_Status_code")
                    DataRowToAdd("DatacallStatusCode") = dv("DatacallStatusCode")
                    DataRowToAdd("dfUpload") = dv("dfUpload")
                    DataRowToAdd("AdminURLLink") = dv("AdminURLLink")
                    DataRowToAdd("AdminTextLink") = dv("AdminTextLink")
                    DataRowToAdd("BodIssuedDate") = dv("BodIssuedDate")
                    DataRowToAdd("AdminLinkColHeaderText") = dv("AdminLinkColHeaderText")
                    DataRowToAdd("PK_Form") = dv("PK_Form")
                    DataRowToAdd("RMAOverrideStatus") = dv("RMAOverrideStatus")
                    GetFormTypeCount(PkCurrent, DataRowToAdd, FormTypes, AgencyData)
                End If
                'add the column to the associated formtype column
                DataRowToAdd("FormStatus_" & dv("FK_FormType").ToString()) = dv("Form_Status")
                DataRowToAdd("FormStatusCode_" & dv("FK_FormType").ToString()) = dv("Form_Status_Code")
                DataRowToAdd("OrgSubmission_" & dv("FK_FormType").ToString()) = dv("PK_OrgSubmission")
                DataRowToAdd("InternalForm_" & dv("FK_FormType").ToString()) = dv("InternalForm")
                DataRowToAdd("DatacallStatusCode_" & dv("FK_FormType").ToString()) = dv("DatacallStatusCode")
                PkLast = PkCurrent
            Next

            ' add the last one
            DataToBind.Rows.Add(DataRowToAdd)
        End If

        _AgencyList = (From d In AgencyData.Table.AsEnumerable() Select d.Field(Of String)("Agency")).Distinct().ToList()
        _AgencyList.Sort()

        Me._AgencyTypeList = (From d In AgencyData.Table.AsEnumerable() Select d.Field(Of String)("ComponentGroup")).Distinct().ToList()
        _AgencyTypeList.Sort()

        Return DataToBind
        'End Using
    End Function

    Protected Sub gv_MainStatusDynamic_PreRender(sender As Object, e As EventArgs) Handles gv_MainStatusDynamic.PreRender
        Dim oDbDS As New DataBaseUtils2
        oDbDS.Parms.Add("@PK_ReportingCycle", _CAUser.PK_ReportingCycle)

        gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").Visible = False
        gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").Visible = False
        gv_MainStatusDynamic.MasterTableView.GetColumn("OriginalWorksheet").Visible = False
        gv_MainStatusDynamic.MasterTableView.GetColumn("FinalWorksheet").Visible = False
        gv_MainStatusDynamic.MasterTableView.GetColumn("RMAOverrideStatus").Visible = False
        gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = False
        gv_MainStatusDynamic.MasterTableView.GetColumn("BodIssuedDate").Visible = False

        Dim fType As String
        Dim PkFormPart As String

        Dim tblform As DataTable = oDbDS.GetDataTable("
        SELECT DISTINCT PK_Form, dbo.fnGetKeyForm(PK_Form) PKFormPart  
        FROM fsma_OrgSubmissions a 
        INNER JOIN fsma_ReportingCycle_Components b 
	        ON a.FK_ReportingCycle_Component = b.PK_ReportingCycle_Component
        WHERE FK_ReportingCycle IN (@PK_ReportingCycle)
        ")

        If tblform.Rows.Count > 0 Then
            fType = tblform.Rows(0)("PK_Form")
            PkFormPart = tblform.Rows(0)("PKFormPart")

            If fType = "2017-Q3-RISKEVA" Then 'Risk Management Assessment Response 2017/_CAUser.PK_ReportingCycle = 48
                gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").Visible = True
            ElseIf fType = "2017-Q4-RISKEVA" Then 'Annual Cybersecurity Risk Management Assessment 2017 /_CAUser.PK_ReportingCycle = 50
                gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("OriginalWorksheet").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalWorksheet").Visible = True
            ElseIf fType = "2018-Q4-RISKEVA" Then 'Annual Agency Performance Summary 2018/_CAUser.PK_ReportingCycle = 61
                gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").ItemStyle.Width = Unit.Percentage(15)
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").ItemStyle.Width = Unit.Percentage(15)
                gv_MainStatusDynamic.MasterTableView.GetColumn("Agency").ItemStyle.Width = Unit.Percentage(30)
            ElseIf PkFormPart = "RMA" Then 'ElseIf _CAUser.PK_ReportingCycle = 64 Or _CAUser.PK_ReportingCycle = 66 Or _CAUser.PK_ReportingCycle = 70 Or _CAUser.PK_ReportingCycle = 73 Then 'Risk Management Assessment 2019 Q1 Q2
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("RMAOverrideStatus").Visible = True
            ElseIf PkFormPart = "BOD" Then 'BOD
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("BodIssuedDate").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("BodIssuedDate").ItemStyle.Width = Unit.Percentage(15)
            ElseIf PkFormPart = "HVA" Then 'BOD 18-02
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
            ElseIf PkFormPart = "HVAPOAM" Then 'BOD 18-02
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
            ElseIf PkFormPart = "AAPS" Then  'AAPS
                gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("OriginalWorksheet").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalWorksheet").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("RMAOverrideStatus").Visible = True
            ElseIf PkFormPart = "EINSTEIN" Then  'EINSTEIN
                gv_MainStatusDynamic.MasterTableView.GetColumn("Assessment").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalAssessment").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("OriginalWorksheet").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("FinalWorksheet").Visible = False
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
                gv_MainStatusDynamic.MasterTableView.GetColumn("RMAOverrideStatus").Visible = False
            ElseIf PkFormPart = "ED2101A" Then 'ED2101A
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
            ElseIf PkFormPart = "CIO" Then
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = False
            ElseIf PkFormPart = "CYBEREO" Then
                gv_MainStatusDynamic.MasterTableView.GetColumn("AdminLink").Visible = True
            End If
        End If


    End Sub


    Protected Sub gv_MainStatusDynamic_SortCommand(sender As Object, e As GridSortCommandEventArgs) Handles gv_MainStatusDynamic.SortCommand
        ' gv_MainStatusDynamic.Rebind()
    End Sub

    Protected Sub rdbAgencies_DataBinding(sender As Object, e As EventArgs)
        Dim rcbAgencies = CType(sender, RadComboBox)
        rcbAgencies.DataSource = _AgencyList
    End Sub
    Protected Sub rdbAgencyTypes_DataBinding(sender As Object, e As EventArgs)
        Dim rcbAgencyTypes = CType(sender, RadComboBox)
        rcbAgencyTypes.DataSource = _AgencyTypeList
    End Sub




    <System.Web.Services.WebMethod()>
    Public Shared Function SetDeadlineDateException(POAMid As String, UserId As String, currDeadlineDate As String) As String
        Dim PoamDeadlineDatesCalc As DataTable = New DataTable
        Dim deadlinedate As String = String.Empty
        Dim PkUser As Integer = UserId
        Dim finding As String = String.Empty
        Dim Component As String = String.Empty
        Dim SystemName As String = String.Empty
        Dim RiskCat As String = String.Empty
        Dim FinalStringBuilder As StringBuilder = New StringBuilder
        Dim FinalString As String = String.Empty
        Dim oDBU As New DataBaseUtils2
        oDBU.Parms.Add("@POAMid", POAMid)
        oDBU.Parms.Add("@UserId", PkUser)
        oDBU.Parms.Add("@CurrentPoamDeadlineDate", currDeadlineDate)
        Try

            oDBU.dbUpdate("Set1802PoamDeadlineDate")
            PoamDeadlineDatesCalc = oDBU.GetDataTable("SELECT POAM_CurrentDeadlineDate, Finding, Component,SystemName,RiskCat FROM vwHVAAssessments WHERE PK_POAM =  @POAMid")
            If PoamDeadlineDatesCalc.Rows.Count > 0 Then

                For Each r As DataRow In PoamDeadlineDatesCalc.Rows
                    If Not String.IsNullOrEmpty(r("POAM_CurrentDeadlineDate")) Then
                        deadlinedate = r("POAM_CurrentDeadlineDate")
                        finding = r("Finding")
                        Component = r("Component")
                        SystemName = r("SystemName")
                        RiskCat = r("RiskCat")
                        'FinalStringBuilder.AppendFormat("{0}{1}{2}", "Current Deadline: ", currDeadlineDate, "</br></br>")
                        'FinalStringBuilder.AppendFormat("{0}{1}{2}", "Agency: ", Component, "</br>")
                        'FinalStringBuilder.AppendFormat("{0}{1}{2}", "HVA: ", SystemName, "</br>")
                        'FinalStringBuilder.AppendFormat("{0}{1}{2}", "Finding: ", finding, "</br>")
                        'FinalStringBuilder.AppendFormat("{0}{1}{2}", "Severity: ", RiskCat, "</br></br></br>")
                        'FinalStringBuilder.AppendFormat("{0}{1}", "New Deadline Date: ", deadlinedate)
                        'FinalString = FinalStringBuilder.ToString()
                        FinalString = "An exception has been created for Agency - " + Component + ", System - " + SystemName + ", Finding - " + finding + ". Their deadline has been updated from " + currDeadlineDate + " to " + deadlinedate
                        Exit For
                    Else
                        deadlinedate = "no data"
                    End If

                Next
            End If

        Catch ex As Exception
            oDBU.Parms.Add("@ErrorDate", System.DateTime.Today)
            oDBU.Parms.Add("@ErrorMsg", "Unable to apply exception date for POAM ID - @POAMid. Exception Message: " + ex.Message)
            oDBU.dbUpdate("INSERT INTO ErrorLog (ErrDescription, ErrDate) VALUES (@ErrorMsg, @ErrorDate )")
        End Try
        Return FinalString
        'Return deadlinedate
    End Function



    Private Sub radTS_Surveys_TabClick(sender As Object, e As RadTabStripEventArgs) Handles radTS_Surveys.TabClick

        If _CAUser.PK_ReportingCycle = 80 Then
            rgBOD1802Poam.Rebind()
            rgBOD1802Poam.Visible = True
            gv_MainStatusDynamic.Visible = False
        Else
            gv_MainStatusDynamic.Visible = True
            rgBOD1802Poam.Visible = False
        End If

        If e.Tab.Value = "-1" Then
            gv_MainStatusDynamic.Visible = False
        End If
        canView = _CAUser.hasPermission2(Nothing, "Admin-DataViewOnly")
    End Sub

    Private Function BOD1802RemPlanGridDataSource()
        Dim oDB As New DataBaseUtils2
        Dim dvStatusResults As DataTable = New DataTable
        oDB.Parms.Clear()
        dvStatusResults = oDB.GetDataTable("OMB1802Poams")
        Return dvStatusResults
    End Function

    Private Sub rgBOD1802Poam_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles rgBOD1802Poam.NeedDataSource
        If _CAUser.PK_ReportingCycle = 80 Then
            rgBOD1802Poam.DataSource = BOD1802RemPlanGridDataSource()
            rgBOD1802Poam.Visible = True
            gv_MainStatusDynamic.Visible = False
        End If
    End Sub

    Public Sub RefreshBod1802RemPlanGrid()
        rgBOD1802Poam.Rebind()
    End Sub

    Private Sub rgBOD1802Poam_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles rgBOD1802Poam.ItemDataBound
        If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then
            'Dim data As GridDataItem = CType(e.Item, GridDataItem)
            'Dim btndeadline As ImageButton = CType(data.FindControl("btnSetDeadlineDateRule"), ImageButton)

            Dim drVal = DirectCast(e.Item.DataItem, DataRowView)
            If Not String.IsNullOrEmpty(drVal(10).ToString) Then
                If drVal(10) = 1 Then
                    e.Item.BackColor = Drawing.ColorTranslator.FromHtml("#F6f0de") '#C8edf5
                    'btndeadline.BackColor = Drawing.ColorTranslator.FromHtml("#F6f0de")
                Else
                    e.Item.BackColor = Drawing.Color.White
                    'btndeadline.BackColor = Drawing.Color.White
                End If
            End If
            Dim hlinkAdmin As HyperLink = CType(e.Item.FindControl("lnkAdmin"), HyperLink)
            hlinkAdmin.Text = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AdminTextLink"))
            hlinkAdmin.NavigateUrl = _UrlParams.EncryptURL(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "AdminURLLink")))

            If Not String.IsNullOrEmpty(drVal(11).ToString) Then
                Dim linklnkDeadlineRelatedSubmissionIndicator As HtmlButton = CType(e.Item.FindControl("lnkDeadlineRelatedSubmissionIndicator"), HtmlButton)
                linklnkDeadlineRelatedSubmissionIndicator.Attributes.Add("class", (Convert.ToString(DataBinder.Eval(e.Item.DataItem, "DeadlineRelatedSubmissionIndicator"))))
                'linklnkDeadlineRelatedSubmissionIndicator.Attributes.Add("title", drVal(13).ToString) 'drVal(#) is the datapoint index (start at 0) from sproc
            End If

            Dim btnSetDeadlineDateRule As HtmlButton = CType(e.Item.FindControl("btnSetDeadlineDateRule"), HtmlButton)
            btnSetDeadlineDateRule.Visible = (Convert.ToString(DataBinder.Eval(e.Item.DataItem, "ShowExceptionBtn")))
            'btnSetDeadlineDateRule.Attributes.Add("title", drVal(15))


            '-Audit log link
            Dim btnAudit As RadImageButton = e.Item.FindControl("btnAudit")
            Dim url As String = ResolveUrl("~/AuditPopUp.aspx?Table=BOD1802-RemPlan&Field=&PKey=" & drVal("PK_POAM").ToString()) 'table=fsma_POAMs 
            btnAudit.Attributes("onclick") = "return ShowPopupDialog('" & _UrlParams.EncryptURL(url) & "');"
        End If
    End Sub

    'Private Sub rgBOD1802Poam_ItemCreated(sender As Object, e As GridItemEventArgs) Handles rgBOD1802Poam.ItemCreated
    '    If TypeOf e.Item Is GridDataItem Then
    '        Dim data As GridDataItem = CType(e.Item, GridDataItem)
    '        Dim btndeadline As Button = CType(data.FindControl("RadCombobox1"), Button)
    '        btndeadline.OnClientClick = "function (button,args){Selected('" & data.ItemIndex & "','" + comboBox1.ClientID & "');}"
    '    End If

    'End Sub
End Class

