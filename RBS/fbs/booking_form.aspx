<%@ page language="C#" autoeventwireup="true" validaterequest="false" inherits="booking_form, fbs" enableeventvalidation="false" %>

<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <% if (is_edit)
        { %>
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <%} %>
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ClientItemSelected(sender, e) {
            $get("<%=hfUserId.ClientID %>").value = e.get_value();
            var id = e.get_value();
            var acc = '<%=current_user.account_id.ToString() %>';
            $('#<%=img_loading.ClientID%>').show();
            $.ajax({
                url: "administration/ajax_page.aspx/useremail",
                type: "POST",
                data: '{ userid:"' + id + '",accountid:"' + acc + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    $('#<%=txt_email.ClientID%>').val(data.d[0]);
                    $('#<%=txt_telephone.ClientID%>').val(data.d[1]);
                    $('#<%=img_loading.ClientID%>').hide();
                },

                error: function (XMLHttpRequest, textStatus, errorThrown) {

                    $('#<%=img_loading.ClientID%>').hide();
                }
            });
            }
            $(document).ready(function () {
                $('#<%=txtBookedFor.ClientID%>').keypress(function () {
                    $('#errorbookedfor').html("<%=Resources.fbs.required_bookedfor%>").hide();
                    fade_wheel();
                    return true;
                });

                $('#<%=txt_purpose.ClientID%>').keypress(function () {
                    $('#errorpurpose').html("<%=Resources.fbs.required_purpose%>").hide();
                    fade_wheel();
                    return true;
                });

                $('#<%=txt_email.ClientID%>').keypress(function () {
                    $('#erroremail').html("<%=Resources.fbs.required_email%>").hide();
                    fade_wheel();
                    return true;
                });
                $('#<%=txt_telephone.ClientID%>').keypress(function (e) {
                    if (e.which != 8 && e.which != 0 && (e.which < 47 || e.which > 57)) {
                        fade_wheel();
                        e.preventDefault();
                        return false;
                    }
                    else {
                        $('#errortelephone').html("<%=Resources.fbs.required_telephonenbr%>").hide();
                        fade_wheel();
                        return true;
                    }
                });

                $('#<%=btn_submit.ClientID %>').click(function () {
                    var isvalidate = true;

                    if ($('#<%=txt_purpose.ClientID%>').val() == '') {
                        $('#<%=errorpurpose.ClientID %>').css('color', 'red');
                        $('#<%=errorpurpose.ClientID %>').html("<%=Resources.fbs.required_purpose%>").show();
                        isvalidate = false;
                    } else {
                        $('#<%=errorpurpose.ClientID %>').hide();
                    }

                    if ($('#<%=txtBookedFor.ClientID%>').val() == '') {
                        $('#<%=errorbookedfor.ClientID %>').css('color', 'red');
                        $('#<%=errorbookedfor.ClientID %>').html("<%=Resources.fbs.required_bookedfor%>").show();
                        isvalidate = false;
                    } else {
                        $('#<%=errorbookedfor.ClientID %>').hide();
                    }

                    if ($('#<%=txt_email.ClientID%>').val() == '') {
                        $('#<%=erroremail.ClientID %>').css('color', 'red');
                        $('#<%=erroremail.ClientID %>').html("<%=Resources.fbs.required_email%>").show();
                        isvalidate = false;
                    }
                    else if (!/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test($('#<%=txt_email.ClientID%>').val()))
                    {
                        $('#<%=erroremail.ClientID %>').css('color', 'red');
                        $('#<%=erroremail.ClientID %>').html('Incorrect email address entered').show();
                        isvalidate = false;
                    }
                    else {
                        $('#<%=erroremail.ClientID %>').hide();
                    }

                    if ($('#<%=txt_telephone.ClientID%>').val() == '') {
                        $('#<%=errortelephone.ClientID %>').css('color', 'red');
                        $('#<%=errortelephone.ClientID %>').html("<%=Resources.fbs.required_telephonenbr%>").show();
                        isvalidate = false;
                    } else {
                        $('#<%=errortelephone.ClientID %>').hide();
                    }

                    <% if (has_tandc)
        { %>
                    if (!document.getElementById('<%=chk_terms.ClientID %>').checked) {
                        $('#<%=errorTerms.ClientID %>').css('color', 'red');
                        $('#<%=errorTerms.ClientID %>').html("<%=Resources.fbs.required_termsandcondition%> ").show();
                        isvalidate = false;
                    } else {
                        $('#<%=errorTerms.ClientID %>').hide();
                    }
                    <% } %>
                    if (isvalidate == false) {
                        fade_wheel();
                        return false;
                    }
                    else {
                        return true;
                    }
                });
            });
    </script>
    <style>
        .no-js #loader {
            display: none;
        }

        .js #loader {
            display: block;
            position: absolute;
            left: 100px;
            top: 0;
        }

        .se-pre-con {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            opacity:0.5;
            z-index: 9999;
            background: url(assets/img/l_c.gif) center no-repeat #fff;
        }
    </style>
</head>
<body class="p-h-f">
    <div class="se-pre-con" id="loader"></div>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdnCloneBookingID" runat="server" />

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
                                        <div class="tabbable  tabbable-custom boxless nav-justified">
                                            <ul class="nav nav-tabs nav-justified">
                                                <li class="active"><a href="#tab_default" data-toggle="tab">Details</a></li>
                                                <li runat="server" id="li_invite_list" visible="false"><a href="#tab_invitelist" data-toggle="tab">Invite List</a></li>
                                                <li runat="server" id="li_FandB_list" visible="false"><a href="#tab_FandB" data-toggle="tab">Food and Beverages</a> </li>
                                                <li runat="server" id="li_termsandconditions"><a href="#tab_terms" data-toggle="tab">Terms & Conditions</a></li>
                                            </ul>
                                            <div class="tab-content">
                                                <div class="tab-pane active" id="tab_default">
                                                    <div class="alr a-err hide">
                                                        <button class="close" data-dismiss="alert">
                                                        </button>
                                                        You have some form errors. Please check below.
                                                    </div>
                                                    <div class="pbd" id="alertError" style="display: none;" runat="server">
                                                        <div class="alr a-err">
                                                            <asp:Literal runat="server" ID="litError" Text="<strong>Error!</strong> Your selected time slot has already passed. You can't make the booking."></asp:Literal>
                                                            <button class="close" data-dismiss="alert">
                                                            </button>
                                                        </div>
                                                    </div>
                                                    <asp:HiddenField runat="server" ID="hdnBookingId" />
                                                    <asp:HiddenField ID="hdn_restricted_message" runat="server" />
                                                    <asp:HiddenField ID="hdnStart" runat="server" />
                                                    <asp:HiddenField ID="hdnEnd" runat="server" />
                                                    <asp:HiddenField ID="hdnAsset" runat="server" />
                                                    <asp:HiddenField ID="hdnBookingType" runat="server" />
                                                    <asp:HiddenField ID="hdnSeq" runat="server" Value="0" />
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    Purpose<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <input type="text" name="txt_purpose" maxlength="255" id="txt_purpose" runat="server" class="mw span10"
                                                                        data-required="1" />
                                                                    <span id="errorpurpose" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    Booked For<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls chzn-controls">
                                                                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                                                                        <Services>
                                                                            <asp:ServiceReference Path="~/user_autocomplete.asmx" />
                                                                        </Services>
                                                                    </asp:ScriptManager>
                                                                    <asp:TextBox ID="txtBookedFor" runat="server" CssClass="span10" data-placeholder="Choose booked for" onkeyup="SetContextKey()"></asp:TextBox>
                                                                    <asp:Panel runat="server" ID="myPanel" Style="width: 250px !important; z-index: 1">
                                                                    </asp:Panel>
                                                                    <ajaxToolkit:AutoCompleteExtender
                                                                        runat="server"
                                                                        ID="autoComplete1"
                                                                        ServicePath="~/user_autocomplete.asmx"
                                                                        ServiceMethod="get_users_other_all_user_type_view"
                                                                        MinimumPrefixLength="2"
                                                                        CompletionInterval="1000"
                                                                        EnableCaching="true"
                                                                        UseContextKey="true"
                                                                        TargetControlID="txtBookedFor"
                                                                        CompletionSetCount="1"
                                                                        CompletionListElementID="myPanel" FirstRowSelected="true"
                                                                        CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList"
                                                                        CompletionListHighlightedItemCssClass="AutoExtenderHighlight"
                                                                        OnClientItemSelected="ClientItemSelected">
                                                                    </ajaxToolkit:AutoCompleteExtender>
                                                                    <asp:HiddenField ID="hfUserId" runat="server" />
                                                                    <span id="errorbookedfor" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    Email<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <%-- <input type="text" name="txt_email" id="txt_email" runat="server" data-placeholder="Email is required"
                                                                class="mw span10"  /><span id="erroremail" runat="server"></span>--%>
                                                                    <asp:TextBox ID="txt_email" CssClass="span10" runat="server" MaxLength="255" CausesValidation="false"></asp:TextBox>

                                                                    <img alt="Loading.." runat="server" id="img_loading" style="display: none; height: 40px;" src="~/assets/img/ajax-loading.gif" />

                                                                 <%--   <asp:RegularExpressionValidator ID="RE_email" runat="server" CssClass="error"
                                                                        ErrorMessage="Incorrect email address entered" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                                        ControlToValidate="txt_email" />--%>
                                                                    <span id="erroremail" runat="server"></span>


                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    Telephone<span class="required number">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <input type="text" name="txt_telephone" id="txt_telephone" maxlength="255" runat="server" data-placeholder="Telephone number is required"
                                                                        class="mw span10" /><span id="errortelephone" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    Remarks</label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <textarea class="span10 mw " maxlength="500" runat="server" name="txt_remarks" id="txt_remarks"
                                                                        data-error-container="#remarks_error"></textarea>
                                                                    <span id="remarks_error" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <asp:Panel runat="server" ID="pnl_housekeeping">
                                                            <tr>
                                                                <td>
                                                                    <label class="cl">
                                                                        Housekeeping Required
                                                                    </label>
                                                                </td>
                                                                <td>
                                                                    <div class="controls">
                                                                        <asp:CheckBox ID="chk_housekeeping" runat="server" />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </asp:Panel>
                                                        <tr>
                                                            <td>
                                                                <div class="cg" id="div_setup_required" runat="server">
                                                                    <label class="cl">
                                                                        Setup Required
                                                                    </label>
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <div class="controls" id="div_chk_setup" runat="server">
                                                                    <asp:CheckBox ID="chk_setup" runat="server" AutoPostBack="true" OnCheckedChanged="chk_setup_CheckedChanged" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div runat="server" id="div_setup_type" visible="false">
                                                                    <label class="cl">
                                                                        Setup Type</label>
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <div class="controls" id="div_ddl_setype_type" runat="server" visible="false">
                                                                    <asp:DropDownList ID="ddl_setupType" runat="server" CssClass="span10 " data-placeholder="Choose setup type">
                                                                    </asp:DropDownList>
                                                                    <span id="errorsetuptype" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div runat="server" id="div_meeting_type">
                                                                    <label class="cl">
                                                                        Meeting Type</label>
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <div class="controls" id="div_ddl_meeting_type" runat="server">
                                                                    <asp:DropDownList ID="ddlMeetingType" runat="server" CssClass="span10 " data-placeholder="Choose meeting type">
                                                                    </asp:DropDownList>
                                                                    <span id="errorMeetingType" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <div class="caption">
                                                                    <h4>Room Details</h4>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <label runat="server" id="lblassets" class="cl mw s12">
                                                                </label>
                                                                <div style="width: 810px; overflow: auto;">
                                                                    <asp:Repeater ID="rpt_list_table" runat="server" Visible="false" OnItemDataBound="rpt_list_table_ItemDataBound">
                                                                        <HeaderTemplate>
                                                                            <table cellpadding="0" cellspacing="0" border="0" class='table table-striped table-bordered table-hover' id='list_table'>
                                                                                <thead>
                                                                                    <tr>
                                                                                        <th class='hidden-480'>Code/Name</th>
                                                                                        <th class='hidden-480'>Building</th>
                                                                                        <th class='hidden-480'>Level</th>
                                                                                        <th class='hidden-480'>Capacity</th>
                                                                                        <th class='hidden-480'>Category</th>
                                                                                        <th class='hidden-480'>From</th>
                                                                                        <th class='hidden-480'>To</th>
                                                                                        <th class='hidden-480'>Status</th>
                                                                                        <th class='hidden-480'>Comments</th>
                                                                                    </tr>
                                                                                </thead>
                                                                                <tbody>
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <tr>
                                                                                <td><%# Eval("Code_Name")%> </td>
                                                                                <td><%# Eval("Building")%> </td>
                                                                                <td><%# Eval("Level")%> </td>
                                                                                <td><%# Eval("capacity")%> </td>
                                                                                <td><%# Eval("Category")%> </td>
                                                                                <td>
                                                                                    <div class="controls">
                                                                                        <div class="i-a date datepicker">
                                                                                            <asp:TextBox ID="txt_startDate" runat="server" Style="width: 85px;" class="mw m-ctrl-medium date-picker"
                                                                                                Text='<%# Eval("FromDate")%>' OnTextChanged="txt_startdate_changed" AutoPostBack="true" CausesValidation="false">
                                                                                            </asp:TextBox>
                                                                                            <span class="ao"><i class="icon-calendar"></i></span>
                                                                                            </br>
                                                                                        <asp:DropDownList ID="ddl_startTime" runat="server" Style="width: 125px;" OnSelectedIndexChanged="ddlStartTime_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false">
                                                                                        </asp:DropDownList>
                                                                                            <asp:HiddenField ID="hdn_startTime" runat="server" Value='<%# Eval("FromTime")%>' />
                                                                                        </div>
                                                                                    </div>
                                                                                </td>
                                                                                <td>
                                                                                    <div class="controls">
                                                                                        <div class="i-a date datepicker">
                                                                                            <asp:TextBox ID="txt_endDate" runat="server" Style="width: 85px;" class="mw m-ctrl-medium date-picker"
                                                                                                Text='<%# Eval("ToDate")%>' CssClass="clsEndDate">
                                                                                            </asp:TextBox>
                                                                                            <span class="ao"><i class="icon-calendar"></i></span>
                                                                                            </br>
                                                                                        <asp:DropDownList ID="ddl_endTime" runat="server" Style="width: 125px;">
                                                                                        </asp:DropDownList>
                                                                                            <asp:HiddenField ID="hdn_endTime" runat="server" Value='<%# Eval("ToTime")%>' />
                                                                                        </div>
                                                                                    </div>
                                                                                </td>
                                                                                <td><%# Eval("Status")%> </td>
                                                                                <td><%# Eval("Comments")%> </td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                        <FooterTemplate>
                                                                            </tbody> </table>
                                                                        </FooterTemplate>
                                                                    </asp:Repeater>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr runat="server" id="row_additional_resources" visible="false">
                                                            <td colspan="2">
                                                                <div class="caption">
                                                                    <h4>Additional Resources</h4>
                                                                </div>
                                                                <div class="cg" id="contrlgrp_resource" runat="server">
                                                                    <%=html_resourcelist %>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr runat="server" id="row_tremsandcondition" visible="false">
                                                            <td colspan="2">
                                                                <div runat="server" id="divterms">
                                                                    <table width="100%">
                                                                        <tr id="download_lnk" runat="server">
                                                                            <td colspan="2"><a runat="server" id="anchor_download"></a></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="text-align: right; vertical-align: top" class="auto-style1">
                                                                                <asp:CheckBox ID="chk_terms" runat="server" />
                                                                            </td>
                                                                            <td>

                                                                                <label class="cl span6" runat="server" id="lbterms">
                                                                                    I have read and agree to the Terms & Conditions.<span class="required ">*</span></label>
                                                                                <span id="errorTerms" runat="server"></span>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2"></td>
                                                        </tr>
                                                    </table>
                                                </div>

                                                <div class="tab-pane" id="tab_invitelist">
                                                    <div class="rfl">
                                                        <div class="s12">
                                                            <div class="s12">
                                                                <label class="cl mw s12">
                                                                    When you invite users to the meeting, they will get an email once your booking is
                                                            confirmed. They will also get an email if there is a change in details of when the
                                                            booking is cancelled. However, they cannot cancel or make changes to the booking.</label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="rfl">
                                                        <div class="cg">
                                                            <table width="100%" cellpadding="10" cellspacing="10">
                                                                <tr>
                                                                    <td>
                                                                        <asp:HiddenField runat="server" ID="hdn_invitee" />
                                                                        <asp:TextBox ID="txtUser" runat="server" onkeyup="SetContextKeyInvitees()" Style="font-size: 12pt !important; width: 100%"></asp:TextBox>
                                                                        <asp:Panel runat="server" ID="pnl_invitee" Style="width: 100% !important; z-index: 1; font-size: 14pt;">
                                                                        </asp:Panel>
                                                                        <ajaxToolkit:AutoCompleteExtender
                                                                            runat="server"
                                                                            ID="AutoCompleteExtender1"
                                                                            ServicePath="~/user_autocomplete.asmx"
                                                                            ServiceMethod="get_users_other_all_user_type"
                                                                            MinimumPrefixLength="4"
                                                                            CompletionInterval="1000"
                                                                            EnableCaching="true"
                                                                            TargetControlID="txtUser"
                                                                            CompletionSetCount="1"
                                                                            CompletionListElementID="pnl_invitee"
                                                                            UseContextKey="true"
                                                                            FirstRowSelected="true"
                                                                            CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList"
                                                                            CompletionListHighlightedItemCssClass="AutoExtenderHighlight"
                                                                            OnClientItemSelected="ClientInviteeSelected">
                                                                        </ajaxToolkit:AutoCompleteExtender>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="btnAssignToGroup" runat="server" CssClass="btn green " Text="Invite" OnClientClick="return validate();" OnClick="btnAssignToGroup_Click"></asp:Button></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>
                                                    <div class="rfl">
                                                        <div class="s12 ">
                                                            <div class="cg">
                                                                <div class="controls mw s12">
                                                                    <asp:GridView ID="gdInviteList" runat="server" ShowFooter="true" AutoGenerateColumns="false"
                                                                        CssClass="grid-table" HeaderStyle-CssClass="grid-header" OnRowCreated="gdInviteList_RowCreated"
                                                                        GridLines="None" Width="80%" OnRowDataBound="gdInviteList_RowDataBound">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="InviteID" Visible="false">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblInviteID" maxlength="255" Text='<%# Eval("InviteID") %>' runat="server"></asp:Label>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField ControlStyle-Width="10px" DataField="RowNumber" HeaderText="SNo" />
                                                                            <asp:TemplateField HeaderText="Name">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="txtName" Width="200px" MaxLength="255" Text='<%# Eval("Name") %>' runat="server"></asp:TextBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Email">
                                                                                <ItemTemplate>
                                                                                    <br />
                                                                                    <asp:TextBox ID="txtEmail_invite" Width="400px" MaxLength="255" Text='<%# Eval("Email") %>' runat="server" CausesValidation="True"></asp:TextBox>
                                                                                    <asp:RegularExpressionValidator ID="REV_email" ValidationGroup="email_group" runat="server"
                                                                                        ErrorMessage="Enter a valid Email Id" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                                                        ControlToValidate="txtEmail_invite" ForeColor="red" Font-Bold="true" />
                                                                                </ItemTemplate>
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                                <FooterTemplate>
                                                                                    <asp:ImageButton ID="ButtonAdd" runat="server" ImageUrl="~/assets/img/btn_add.png"
                                                                                        ToolTip="Add New Row" OnClick="ButtonAdd_Click" CausesValidation="true" ValidationGroup="email_group" Visible="false" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField ControlStyle-CssClass="grid-button">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton ID="LinkButton1" runat="server" ImageUrl="~/assets/img/btn_remove.png"
                                                                                        ToolTip="Remove Row" OnClick="LinkButton1_Click" ValidationGroup="email_group" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="tab-pane" id="tab_terms">
                                                    <div class="cg">
                                                        <label runat="server" id="lblTerms" class="cl mw s12">
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="fa">
                                        <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Save Booking"
                                            OnClick="btn_submit_Click" ValidationGroup="email_group" OnClientClick="show_loading();" />
                                        <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" />
                                    </div>
                                </div>
                                <%-- </div>--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script src="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js" type="text/javascript"></script>
        <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
        <% if (is_edit)
            { %>
        <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
        <%} %>
        <script type="text/javascript" src="assets/scripts/app.js"></script>

        <script type="text/javascript">
            $(window).load(function () {
                fade_wheel();
            });
            function fade_wheel()
            {
                $(".se-pre-con").fadeOut("slow");
            }

            function validate() {
                if ($("<%=txtUser.ClientID %>").val() == "No Record Found.." || $("<%=txtUser.ClientID %>").val() == "") {
                    $('#select_grouperror').show();
                    return false;
                }
            }

            function show_loading() {                
                document.getElementById("loader").style.display = "block";
            }
            function WebForm_PostBackOptions(eventTarget, eventArgument, validation, validationGroup, actionUrl, trackFocus, clientSubmit) { }
            function WebForm_DoPostBackWithOptions() { }

            function activateTab(tab) {
                $('.nav-tabs a[href="#' + tab + '"]').tab('show');
            }

            function SetContextKey() {
                $find('<%=autoComplete1.ClientID%>').set_contextKey("<%=account_id %>");
            }

            function SetContextKeyInvitees() {
                $find('<%=AutoCompleteExtender1.ClientID%>').set_contextKey("<%=account_id %>");
            }

            function ClientInviteeSelected(sender, e) {
                $get("<%=hdn_invitee.ClientID %>").value = e.get_value();
                }

                var Search = function () {
                    return {
                        init: function () {
                            if (jQuery().datepicker) {
                                $('.date-picker').datepicker();
                            }
                            App.initFancybox();
                        }
                    };
                }

            jQuery(document).ready(function () {
                var maxDT = new Date();
                maxDT.setMonth(maxDT.getYear() + 100);
                var minDT = new Date(1900, 1, 1);
                $('.datepicker').datepicker({
                    format: 'dd-M-yyyy',
                    startDate: minDT,
                    endDate: maxDT
                });

                $('#btn_cancel').click(function () {

                    try {
                        parent.$.fancybox.close();
                    }
                    catch (Ex) {
                    }
                });

                $('.datepicker').datepicker()
             .on('changeDate', function (e) {
                 $(this).datepicker('hide');
             });
            });


        </script>
        
        
    </form>
</body>
</html>
