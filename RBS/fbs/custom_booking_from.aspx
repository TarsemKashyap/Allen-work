<%@ page language="C#" autoeventwireup="true" inherits="custom_booking_from, fbs" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
     <%--<meta http-equiv="X-UA-Compatible" content="IE=edge" />--%>
 <%--   <meta http-equiv="X-UA-Compatible" content="IE=IE11" />--%>
    
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet"  type="text/css" />
    <link href="assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet"   type="text/css" />
    <link href="assets/css/style-metro.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style-responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/default.css" rel="stylesheet" type="text/css" id="style_color" />
    <link href="assets/plugins/uniform/css/uniform.default.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/status.css" rel="stylesheet" type="text/css" />

    
    
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
   <%-- <link rel="stylesheet" type="text/css" href="assets/plugins/chosen-bootstrap/chosen/chosen.css" />--%>
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function ClientItemSelected(sender, e) {
            $get("<%=hfUserId.ClientID %>").value = e.get_value();
            var id = e.get_value();
            var acc = '<%=accountid %>';
            $('#<%=img_loading.ClientID%>').show();
            $.ajax({
                url: "/administration/ajax_page.aspx/useremail",
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
            $('#<%=txt_purpose.ClientID%>').keypress(function () {
                $('#errorpurpose').html("<%=Resources.fbs.required_purpose%>").hide();
                return true;
            });
            <%--$('#<%=ddlBookedFor.ClientID%>').keypress(function () {--%>
            $('#<%=txtBookedFor.ClientID%>').keypress(function () {
                $('#errorbookedfor').html("<%=Resources.fbs.required_bookedfor%>").hide();
                return true;
            });
            $('#<%=txt_email.ClientID%>').keypress(function () {
                $('#erroremail').html("<%=Resources.fbs.required_email%>").hide();
                return true;
            });
            $('#<%=txt_telephone.ClientID%>').keypress(function () {
                $('#errortelephone').html("<%=Resources.fbs.required_telephonenbr%>").hide();
                return true;
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

                <%--if ($('#<%=ddlBookedFor.ClientID%>').val() == '') {--%>
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
                } else {
                    $('#<%=erroremail.ClientID %>').hide();
                }

                if ($('#<%=txt_telephone.ClientID%>').val() == '') {
                    $('#<%=errortelephone.ClientID %>').css('color', 'red');
                    $('#<%=errortelephone.ClientID %>').html("<%=Resources.fbs.required_telephonenbr%>").show();
                    isvalidate = false;
                } else {
                    $('#<%=errortelephone.ClientID %>').hide();
                }

                if (!document.getElementById('<%=chk_terms.ClientID %>').checked) {
                    $('#<%=errorTerms.ClientID %>').css('color', 'red');
                    $('#<%=errorTerms.ClientID %>').html("<%=Resources.fbs.required_termsandcondition%> ").show();
                    isvalidate = false;
                } else {
                    $('#<%=errorTerms.ClientID %>').hide();
                }

                if (isvalidate == false) {
                    return false;
                }
                else {
                    return true;
                }
            });
        });
    </script>
    
</head>
<body class="p-h-f">
    <form id="form1" runat="server">
    
    <div class="p-con rfl">
        
        <div class="p-c">
            
            <div class="c-f">
                
                <div class="rfl">
                    <div class="s12">
                        <h3 class="pt-header">
                            Booking Details
                        </h3>
                    </div>
                </div>
                
                
                 <div class="pbd" id="alt_error_alrdybook" style="display: none;" runat="server">
                                <div class="alr a-err">
                                    <asp:Literal runat="server" ID="Literal1" Text="<strong>Error!</strong>  You can't make booking in this time slot."></asp:Literal>
                                    <button class="close" data-dismiss="alert">
                                    </button>
                                </div>
                            </div>
                <div class="rfl">
                    <div class="s12">
                        
                         <div class="portlet box blue">
                             <div class="ptt">
                            
                        </div>
                       <%-- <div class="portlet tabbable">--%>
                            
                          
                          <%--  <div class="ptt">
                                <div class="caption">
                                    <i class="icon-reorder"></i>Meeting Meeting
                                    </div>
                            </div>--%>
                            <div class="pbd form">
                                <div class="tabbable  tabbable-custom boxless nav-justified">
                                    <ul class="nav nav-tabs nav-justified">
                                        <li class="active"><a href="#tab_default" data-toggle="tab">Details</a></li>
                                        <li runat="server" id="li_invite_list" ><a  href="#tab_invitelist" data-toggle="tab">Invite
                                            List</a></li>
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
                                            <asp:HiddenField ID="hdnAssetOwner" runat="server" />
                                            <asp:HiddenField ID="hdnStart" runat="server" />
                                            <asp:HiddenField ID="hdnEnd" runat="server" />
                                            <asp:HiddenField ID="hdnAsset" runat="server" />
                                            <asp:HiddenField ID="hdnAccount" runat="server" />
                                            <asp:HiddenField ID="hdnApplication" runat="server" />
                                            <table width="100%">
                                                <tr>
                                                    <td style="width:180px;">
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
                                                    <td style="width:180px;">
                                                        <label class="cl">
                                                            Booked For<span class="required">*</span></label>
                                                    </td>
                                                    <td >
                                                        <div class="controls chzn-controls">
                                                           <%-- <asp:DropDownList ID="ddlBookedFor" Width="540" runat="server" CssClass="span10 chosen" data-placeholder="Choose booked for">
                                                            </asp:DropDownList>--%>
                                                           <%--  <asp:DropDownList ID="ddlBookedFor" Width="540" runat="server" CssClass="span10 " data-placeholder="Choose booked for">
                                                            </asp:DropDownList>--%>
                                                              <asp:ScriptManager ID="ScriptManager1" runat="server">
                                                         <Services>
                                                            <asp:ServiceReference Path="~/user_autocomplete.asmx" />
                                                         </Services>
                                                        </asp:ScriptManager>
                                                <asp:TextBox ID="txtBookedFor" runat="server" CssClass="mw span10" data-placeholder="Choose booked for" ></asp:TextBox>  
                                                  <asp:Panel runat="server" ID="myPanel" style=" width:250px !important;z-index:1 "> 
                                                                            </asp:Panel> 
                                                                             <ajaxToolkit:AutoCompleteExtender
                                                                             runat="server" 
                                                                             ID="autoComplete1"  
                                                                             ServicePath="~/user_autocomplete.asmx" 
                                                                             ServiceMethod="get_users_other_all_user_type_view"
                                                                             MinimumPrefixLength="4" 
                                                                             CompletionInterval="1000"
                                                                             EnableCaching="true" 
                                                                             TargetControlID="txtBookedFor"
                                                                             CompletionSetCount="1"       
                                                                             CompletionListElementID="myPanel" FirstRowSelected="true"
                                                                             CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList"  
                                                                             CompletionListHighlightedItemCssClass="AutoExtenderHighlight"   
                                                                             OnClientItemSelected="ClientItemSelected"    >                                                                           
                                                                        </ajaxToolkit:AutoCompleteExtender>
                                                  <asp:HiddenField ID="hfUserId" runat="server" />
                                                            <span id="errorbookedfor" runat="server"></span>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
                                                        <label class="cl">
                                                            Email<span class="required">*</span></label>
                                                    </td>
                                                    <td>
                                                        <div class="controls">
                                                           <%-- <input type="text" name="txt_email" id="txt_email" runat="server" data-placeholder="Email is required"
                                                                class="mw span10"  /><span id="erroremail" runat="server"></span>--%>
                                                            <asp:TextBox ID="txt_email" CssClass="mw span10" runat="server" maxlength="255" CausesValidation="false"></asp:TextBox>
                                                             <img  alt="Loading.." runat=server ID="img_loading" style="display:none;height:40px;"  src="~/assets/img/ajax-loading.gif" />
                                                                            <asp:RegularExpressionValidator ID="RE_email" runat="server"  CssClass="error"
                                                                                ErrorMessage="Incorrect email address entered" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                                                ControlToValidate="txt_email" />
                                                            <span id="erroremail" runat="server"></span>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
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
                                                    <td style="width:180px;">
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
                                                <tr>
                                                    <td>
                                                        <div id="divhousekeeping" runat="server">
                                                            <table>
                                                                <tr>
                                                            <td style="width:180px;">
                                                        <label class="cl">
                                                            Housekeeping Required</label>
                                                    </td>
                                                    <td>
                                                        <div class="controls">
                                                            <asp:CheckBox ID="chk_housekeeping" runat="server" />
                                                        </div>
                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                   
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
                                                        <div class="cg" id="div_setup_required" runat="server">
                                                            <label class="cl">
                                                                Setup Required</label>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="controls" id="div_chk_setup" runat="server">
                                                            <asp:CheckBox ID="chk_setup" runat="server" AutoPostBack="true" OnCheckedChanged="chk_setup_CheckedChanged" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
                                                        <div runat="server" id="div_setup_type">
                                                            <label class="cl">
                                                                Setup Type<span class="required">*</span></label></div>
                                                    </td>
                                                    <td>
                                                        <div class="controls" id="div_ddl_setype_type" runat="server">
                                                            <%--<asp:DropDownList ID="ddl_setupType" Width="540" runat="server" CssClass="span10 chosen" data-placeholder="Choose setup type">
                                                            </asp:DropDownList>--%>
                                                            <asp:DropDownList ID="ddl_setupType" Width="540" runat="server" CssClass="span10 " data-placeholder="Choose setup type">
                                                            </asp:DropDownList>
                                                            <span id="errorsetuptype" runat="server"></span>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
                                                        <label class="cl">
                                                            Requested By</label>
                                                    </td>
                                                    <td>
                                                        <div class="controls">
                                                            <input class="mw span10" readonly type="text" runat="server" id="lbl_requestedBy" /> 
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
                                                        <label class="cl">
                                                            Requestor Email</label>
                                                    </td>
                                                    <td>
                                                        <div class="controls">
                                                            <input class="mw span10" readonly type="text" runat="server" id="lbl_requestor_email" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:180px;">
                                                        <label class="cl">
                                                            Division / Department</label>
                                                    </td>
                                                    <td>
                                                        <div class="controls">
                                                            <input class="mw span10" readonly type="text" runat="server" id="lbl_division_department" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <div class="caption">
                                                            <h4>
                                                                Room Details</h4>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <label runat="server" id="lblassets" class="cl mw s12">
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div id="divterms" runat="server">
                                                            <table>
                                                      <tr runat="server" id="row_tremsandcondition">
                                                    <td style="text-align: right; vertical-align: top">
                                                        <asp:CheckBox ID="chk_terms" runat="server" />
                                                    </td>
                                                    <td>
                                                        <label class="cl span6" >
                                                            I have read and agree the Terms & Conditions<span class="required ">*</span></label>
                                                        <span id="errorTerms" runat="server"></span>
                                                    </td>
                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                               
                                                <tr>
                                                    <td colspan="2">
                                                        <div class="fa ">
                                                            <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Save Booking"
                                                                OnClick="btn_submit_Click" ValidationGroup="email_group" />
                                                            <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="tab-pane" id="tab_invitelist">
                                            <div class="rfl">
                                                <div class="s12 ">
                                                    <div class="cg">
                                                        <label class="cl mw s12">
                                                            When you invite users to the meeting, they will get an email once your booking is
                                                            confirmed. They will also get an email if there is a change in details of when the
                                                            booking is cancelled. However, they cannot cancel or make changes to the booking.</label>
                                                        <div class="controls mw s12">
                                                            <asp:GridView ID="gdInviteList" runat="server" ShowFooter="true" AutoGenerateColumns="false"
                                                                CssClass="grid-table" HeaderStyle-CssClass="grid-header" OnRowCreated="gdInviteList_RowCreated"
                                                                GridLines="None" Width="80%" OnRowDataBound="gdInviteList_RowDataBound">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="InviteID" Visible="false">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblInviteID"  maxlength="255" Text='<%# Eval("InviteID") %>' runat="server"></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField ControlStyle-Width="10px" DataField="RowNumber" HeaderText="SNo" />
                                                                    <asp:TemplateField HeaderText="Name">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtName" Width="200px" maxlength="255" Text='<%# Eval("Name") %>' runat="server"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Email">
                                                                        <ItemTemplate>
                                                                            <br />
                                                                            <asp:TextBox ID="txtEmail_invite" Width="400px" maxlength="255" Text='<%# Eval("Email") %>' runat="server" CausesValidation="True"></asp:TextBox>
                                                                            <asp:RegularExpressionValidator ID="REV_email" ValidationGroup="email_group" runat="server"
                                                                                ErrorMessage="Enter a valid Email Id" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                                                ControlToValidate="txtEmail_invite" ForeColor="red" Font-Bold="true" />
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Right" />
                                                                        <FooterTemplate>
                                                                            <asp:ImageButton ID="ButtonAdd" runat="server" ImageUrl="~/assets/img/btn_add.png"
                                                                                ToolTip="Add New Row" OnClick="ButtonAdd_Click" CausesValidation="true" ValidationGroup="email_group" />
                                                                        </FooterTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ControlStyle-CssClass="grid-button">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="LinkButton1" runat="server" ImageUrl="~/assets/img/btn_remove.png"
                                                                                ToolTip="Remove Row" OnClick="LinkButton1_Click"  ValidationGroup="email_group"/>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane " id="tab_terms">
                                            <div class="cg">
                                                <label runat="server" id="lblTerms" class="cl mw s12">
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            </div>
                       <%-- </div>--%>
                        
                    </div>
                </div>
                
            </div>
            
        </div>
        
    </div>
    
    
    

    <script src="assets/scripts/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <!-- IMPORTANT! Load jquery-ui-1.10.1.custom.min.js before bootstrap.min.js to fix bootstrap tooltip conflict with jquery ui tooltip -->
    <script src="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js" type="text/javascript"></script>
    <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="assets/plugins/bootstrap-hover-dropdown/twitter-bootstrap-hover-dropdown.min.js"
        type="text/javascript"></script>
 <%--   [if lt IE 9]>
	<script src="assets/plugins/excanvas.min.js"></script>
	<script src="assets/plugins/respond.min.js"></script>  
	<![endif]--%>
    <script src="assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js" type="text/javascript"></script>
    <%--<script src="assets/plugins/jquery.blockui.min.js" type="text/javascript"></script>
    <script src="assets/plugins/jquery.cookie.min.js" type="text/javascript"></script>--%>
    <script src="assets/plugins/uniform/jquery.uniform.min.js" type="text/javascript"></script>
    
    
   <%-- <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>--%>
    <%--<script type="text/javascript" src="assets/plugins/chosen-bootstrap/chosen/chosen.jquery.min.js"></script>--%>
    
    
    <%--<script src="assets/scripts/app.js"></script>--%>
    <script type="text/javascript">

        //Dummy function to avoid error Generate at runntime for below javascript function
        function WebForm_PostBackOptions(eventTarget, eventArgument, validation, validationGroup, actionUrl, trackFocus, clientSubmit) {

        }
        function WebForm_DoPostBackWithOptions() {

        }
        /////////////////////////
        function activateTab(tab) {
            $('.nav-tabs a[href="#' + tab + '"]').tab('show');
        }

        jQuery(document).ready(function () {
            // initiate layout and plugins
            // App.init();

            $('#btn_cancel').click(function () {

                try {
                    parent.$.fancybox.close();
                }
                catch (Ex) {
                }
            });


            //$('.nav-tabs a[href="#tab_default"]').tab('show');
        });
             
      
    </script>
    
    
    </form>
</body>
</html>
