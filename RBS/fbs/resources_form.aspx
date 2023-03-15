<%@ page language="C#" autoeventwireup="true" inherits="resources_form, fbs" validaterequest="false" enableeventvalidation="false" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
</head>
<body class="p-h-f">
    <form id="form1" runat="server">
        <div class="rfl" style="margin-top: 20px;">
            <div class="p-c">
                <div class="c-f">
                    <div class="pbd" id="alt_error_alrdybook" style="display: none;" runat="server">
                        <div class="alr a-err">
                            <asp:Literal runat="server" ID="Literal1" Text="<strong>Error!</strong>  You can't make booking in this time slot."></asp:Literal>

                            <button class="close" data-dismiss="alert">
                            </button>
                        </div>
                    </div>
                    <div class="pbd">
                        <div class="rfl">
                            <div class="s12">
                                <div class="portlet box blue">
                                    <div class="ptt">
                                        <div class="caption">
                                            Booking Details -
                                            <asp:Label runat="server" ForeColor="#A4212C" ID="lbl_assetname_heading"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="pbd form">
                                        <div class="rfl">
                                            <div class="cg">
                                                <table width="100%" cellpadding="10" cellspacing="10">
                                                    <tr>
                                                        <td>
                                                            <asp:ScriptManager ID="ScriptManager1" runat="server">
                                                                <Services>
                                                                            <asp:ServiceReference Path="~/resources.asmx" />
                                                                        </Services>
                                                                    </asp:ScriptManager>
                                                            <asp:HiddenField runat="server" ID="hdn_invitee" />
                                                            <asp:TextBox ID="txtResource" runat="server" onkeyup="SetContextKeyResources()" Style="font-size: 12pt !important; width: 100%"></asp:TextBox>
                                                            <asp:Panel runat="server" ID="pnl_resources" Style="width: 100% !important; z-index: 1; font-size: 14pt;">
                                                            </asp:Panel>
                                                            <ajaxtoolkit:autocompleteextender
                                                                runat="server"
                                                                id="AutoCompleteExtender1"
                                                                servicepath="~/resources.asmx"
                                                                servicemethod="get_resources"
                                                                minimumprefixlength="4"
                                                                completioninterval="1000"
                                                                enablecaching="true"
                                                                targetcontrolid="txtResource"
                                                                completionsetcount="1"
                                                                completionlistelementid="pnl_resources"
                                                                usecontextkey="true"
                                                                firstrowselected="true"
                                                                completionlistcssclass="AutoExtender" completionlistitemcssclass="AutoExtenderList"
                                                                completionlisthighlighteditemcssclass="AutoExtenderHighlight"
                                                                onclientitemselected="ResourceItemSelected">
                                                                        </ajaxtoolkit:autocompleteextender>
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btn_add" runat="server" CssClass="btn green " Text="Add Resource" OnClientClick="return validate();" OnClick="btn_add_Click"></asp:Button></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="rfl">
                                            <div class="s12 ">
                                                <div class="cg">
                                                    <div class="controls mw s12">

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="fa">

                                    </div>
                                </div>
                                <%-- </div>--%>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="loader" style="display: none;"></div>
            </div>
        </div>
        
        <script src="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js" type="text/javascript"></script>
        <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
        <script type="text/javascript" src="assets/scripts/app.js"></script>
        <script>
            function validate() {
                if ($("<%=txtResource.ClientID %>").val() == "No Record Found.." || $("<%=txtResource.ClientID %>").val() == "") {
                    return false;
                }
            }

            function SetContextKeyResources() {
                $find('<%=AutoCompleteExtender1.ClientID%>').set_contextKey("<%=account_id %>");
            }

            function ResourceItemSelected(sender, e) {
            }
        </script>
    </form>
</body>
</html>
