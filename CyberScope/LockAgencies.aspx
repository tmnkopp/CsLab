<%@ Page Language="VB" AutoEventWireup="false" Inherits="CyberScope.LockAgencies"
    Title="Agency Lock" MasterPageFile="~/Admin.master" Codebehind="LockAgencies.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="currentpage">AGENCY FORM LOCK</div>
    <table class="table table-borderless table-sm mt-2">

        <tr>
            <td style="width: 36%; text-align: left;">
                <telerik:RadDropDownList RenderMode="Lightweight" Skin="Bootstrap" ID="rdlReportingCycles" runat="server" AutoPostBack="True" Width="80%"/>               
            </td>
            <td style="width: 28%; text-align: center;">
                <table class="table table-borderless table-sm">
                    <tr>
                        <td style="width: 100%; text-align: left;">
                            <asp:TextBox ID="tb_WildCard" runat="server" Width="60%" CssClass="mt-2"></asp:TextBox>
                            <asp:Button ID="btn_Filter" runat="server" Text="Filter" CssClass="btn btn-primary btn-sm" />
                        &nbsp;
                            <asp:Button ID="btn_Clear" CausesValidation="false" CssClass="btn btn-secondary btn-sm" runat="server" OnClientClick="clearXSSVals()" Text="Clear" />
                            <br />
                            Enter Wildcard Search to limit list: (*, ?)
                            <asp:PlaceHolder ID="phCbRegXSS" runat="server"></asp:PlaceHolder>
<%--                        </td>
                        <td style="width: 30%; text-align: center;" colspan="2">--%>

                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 18%; text-align: center;">
                <asp:Button ID="btn_LockAll" runat="server" Font-Bold="True" Font-Italic="False" CssClass="btn btn-danger btn-sm"
                    Text="LOCK ALL" OnClientClick="return confirm('Are you sure you want to Lock ALL Agencies? This affects all non-submitted Agencies, regardless of any specified wildcard search filter.');" />
            </td>
            
            <td style="width: 18%; text-align: center;">
                <asp:Button ID="btn_UnlockAll" runat="server" Font-Bold="True" Font-Italic="False" CssClass="btn btn-success btn-sm"
                     Text="UNLOCK ALL" OnClientClick="return confirm('Are you sure you want to Unlock ALL Agencies? This affects all non-submitted Agencies, regardless of any specified wildcard search filter.');" />
            </td>
        </tr>
        
        <tr>
            <td colspan="4">
                <telerik:RadGrid runat="server" ID="rgAgencyLock" Skin="Bootstrap" SkinID="Bootstrap"  BorderStyle="Solid" BorderWidth="1px" AutoGenerateColumns="False" GridLines="Horizontal" DataKeyNames="PK_Component">
                    <MasterTableView CssClass="table table-striped table-sm">
                        <Columns>
                            <telerik:GridBoundColumn DataField="PK_Component" HeaderText="PK_Component"
                                                     ReadOnly="True" SortExpression="PK_Component" Visible="False" />
                            <telerik:GridBoundColumn DataField="AgType" HeaderText="Type" ReadOnly="True" SortExpression="AgType">
                                <HeaderStyle Width="15%" /> 
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Component" HeaderText="Agency" SortExpression="Component">
                                <HeaderStyle Width="40%" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Acronym" HeaderText="Acronym" SortExpression="Acronym">
                                <HeaderStyle Width="15%" /> 
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Status_code") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:PlaceHolder runat="server" ID="iLockUnlock"/>
                                </ItemTemplate>
                                <ItemStyle Width="15%"/>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn>
                                <ItemTemplate>
                                <asp:Button ID="btn_ChangeStat" runat="server" CommandArgument='<%# Eval("PK_Component") %>'
                                    OnClick="btn_ChangeStat_Click" Text="Lock" CssClass="btn btn-primary btn-sm" />
                                </ItemTemplate>
                                <ItemStyle Width="15%"/>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="enableZeroTouch">
                                <ItemTemplate>
                                <asp:Button ID="btn_enableZeroTouch" runat="server" CommandArgument='<%# Eval("PK_Component") %>'
                                    OnClick="btn_enableZeroTouch_Click" Text="Zero Touch" CssClass="btn btn-primary btn-sm" />
                                </ItemTemplate>
                                <ItemStyle Width="15%"/>
                            </telerik:GridTemplateColumn> 
                        </Columns>
                    </MasterTableView>
                    <HeaderStyle BackColor="#606d88" ForeColor="white" Font-Bold="True"/>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
</asp:Content>
