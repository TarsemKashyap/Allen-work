<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="update_profile, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <%--<link rel="stylesheet" type="text/css" href="../assets/plugins/select2/select2_metro.css" />--%>
    <link rel="stylesheet" href="../assets/plugins/data-tables/DT_bootstrap.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="p-c">
        
        <div class="c-f">
            <div class="rfl">
                <div class="s12">
                    <h3 class="pt">My Profile
                    </h3>
                </div>
            </div>
            
            <div class="portlet box blue">
                <div class="ptt">
                    <div class="caption"><i class="icon-reorder"></i>Information</div>
                </div>
                <div class="pbd form">
                    <div class="rfl">
                        <div class="span5">
                            <div class="cg">
                                <label class="cl">Email<span class="required">*</span> <span id="errorEmail" class="requiredField"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_email" runat="server" MaxLength="500" CssClass="mw s12"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="span5">
                            <div class="cg">
                                <label class="cl">Username</label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_username" runat="server" MaxLength="500" CssClass="mw s12" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="rfl">
                        <div class="span5">
                            <div class="cg">
                                <label class="cl">Display Name<span class="required">*</span> <span id="errorFullName" class="requiredField"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_full_name" runat="server" MaxLength="255" CssClass="mw s12"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="span5">
                            <div class="cg">
                                <label class="cl">Institution</label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_institution" runat="server" CssClass="mw s12" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        
                    </div>

                    <div class="rfl">
                        <div class="span3">
                            <div class="cg">
                                <label class="cl">Division<span class="required">*</span> <span id="errorDivision" class="requiredField"></span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlDivision" runat="server" Width="300" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="span3">
                            <div class="cg">
                                <label class="cl">Department<span class="required">*</span> <span id="errorDepartment" class="requiredField"></span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlDepartment" runat="server" Width="300" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="">Select Department</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="span3">
                            <div class="cg">
                                <label class="cl">Section</label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlSection" runat="server" Width="300">
                                        <asp:ListItem Value="">Select Section</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="span2">
                            <div class="cg">
                                <label class="cl">PIN</label>
                                <div class="controls">
                                     <asp:TextBox ID="txtPIN" runat="server" MaxLength="500" CssClass="mw s12" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="rfl">
                        <div class="span3">
                            <div class="cg">
                                <%--  <label class="cl">Office Phone<span class="required">*</span> <span id="errorOfficePhone" class="requiredField"></span></label>--%>
                                <label class="cl">Office Phone</label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_office_phone" runat="server" CssClass="mw s12"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="span3">
                            <div class="cg">
                                <%-- <label class="cl">Mobile Phone<span class="required">*</span> <span id="errorMobilePhone" class="requiredField"></span></label>--%>
                                <label class="cl">Mobile Phone</label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_mobile_phone" runat="server" CssClass="mw s12"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="span3">
                            <div class="cg">
                                <label class="cl">Staff ID</label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_staff_id" runat="server" CssClass="mw s12"></asp:TextBox>
                                </div>
                            </div>
                            
                        </div>
                    </div>
                    <div class="fa">
                        <asp:HiddenField ID="hidUserId" runat="server" />
                        <asp:HiddenField ID="hdnEditUserId" runat="server" />
                        <asp:HiddenField ID="hdnEmail" runat="server" />
                        <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Save" OnClientClick="return UserForm_Validation();" OnClick="btn_submit_Click" />
                        <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Cancel (Go Back)" OnClientClick="javascript:window.location.href='bookings.aspx';return false;" />
                    </div>
                </div>
            </div>

            <div class="portlet box blue">
                <div class="ptt">
                    <div class="caption"><i class="icon-reorder"></i>Update Password</div>
                </div>
                <div class="pbd form">
                    <div class="rfl">
                            <div class="span6 ">
                                <div class="cg">
                                    <label class="cl">Password<span class="required">*</span> <span id="errorPassword" class="requiredField"></span></label>
                                    <div class="controls">
                                        <asp:TextBox TextMode="Password" ID="txt_Password" runat="server" MaxLength="255" CssClass="mw s12"></asp:TextBox>
                                        <label class="cl" style="color: #A3A3A3">(Password must be a minimum of 8 characters)</label>
                                    </div>
                                    <label class="cl">Repeat Password<span class="required">*</span> <span id="errorRe_Password" class="requiredField"></span></label>
                                    <div class="controls">
                                        <asp:TextBox TextMode="Password" ID="txt_rePassword" runat="server" MaxLength="255" CssClass="mw s12"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                    </div>
                    <div class="fa">
                        <asp:Button ID="btn_password_update" CssClass="btn green" runat="server" Text="Change Password" OnClientClick="return Password_Validation();" OnClick="btn_password_update_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="../assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="../assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="../assets/scripts/app.js"></script>

    <script type="text/javascript" src="../assets/scripts/table-managed.js"></script>
    <script type="text/javascript" src="../assets/scripts/form-components.js"></script>

    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('.s2-c').select2({
                placeholder: "Select an option",
                allowClear: true
            });

            TableManaged.init();
            FormComponents.init();
        });

        function Password_Validation()
        {
                if (jQuery.trim(jQuery("#<%=txt_Password.ClientID%>").val()) == "") {
                    jQuery("#errorPassword").html("<%=Resources.fbs.required_password %>");
                    if (isfocus) {
                        jQuery("#<%=txt_Password.ClientID %>").focus(); isfocus = false;
                    }

                    isValid = false;
                }

                else {
                    jQuery("#errorPassword").html("");

                }

                if (jQuery.trim(jQuery("#<%=txt_Password.ClientID %>").val()).length <= 7) {
                    jQuery("#errorPassword").html("<%=Resources.fbs.password_validation %>");
                     if (isfocus) {
                         jQuery("#<%=txt_Password.ClientID %>").focus(); isfocus = false;

                }
                isValid = false;
            }

            else {
                jQuery("#errorPassword").html("");

            }

            if (jQuery.trim(jQuery("#<%=txt_rePassword.ClientID %>").val()) == "") {
                    jQuery("#errorRe_Password").html("<%=Resources.fbs.required_repeat_password %>");
                 if (isfocus) {
                     jQuery("#<%=txt_rePassword.ClientID %>").focus(); isfocus = false;
                }

                isValid = false;
            }
            else {

                jQuery("#errorRe_Password").html("");
            }

            if (jQuery.trim(jQuery("#<%=txt_rePassword.ClientID %>").val()) != jQuery.trim(jQuery("#<%=txt_Password.ClientID%>").val())) {

                    jQuery("#errorRe_Password").html("<%=Resources.fbs.password_match %>");
                 if (isfocus) {
                     jQuery("#<%=txt_rePassword.ClientID %>").focus(); isfocus = false;

                }
                isValid = false;
            }
            else {
                jQuery("#errorRe_Password").html("");
            }
        
        }

        function UserForm_Validation() {
            var isValid = true;
            var isfocus = true;

            if (jQuery.trim(jQuery("#<%=txt_email.ClientID %>").val()) == "") {
                jQuery("#errorEmail").html("<%=Resources.fbs.required_email%>");
                if (isfocus) {
                    jQuery("#<%=txt_email.ClientID %>").focus(); isfocus = false;
                }
                isValid = false;
            }
            else {
                if (validateEmail(jQuery("#<%=txt_email.ClientID %>").val()) == "") {
                    jQuery("#errorEmail").html("<%=Resources.fbs.required_email_valid%>");
                    if (isfocus) {
                        jQuery("#<%=txt_email.ClientID %>").focus(); isfocus = false;
                    }
                    isValid = false;
                }
                else {
                    jQuery("#errorEmail").html("");
                }
            }

            if (jQuery.trim(jQuery("#<%=txt_full_name.ClientID %>").val()) == "") {
                jQuery("#errorFullName").html("<%=Resources.fbs.required_fullname %>");
                if (isfocus) {
                    jQuery("#<%=txt_full_name.ClientID %>").focus(); isfocus = false;
                }

                isValid = false;
            }
            else {
                jQuery("#errorFullName").html("");
            }

            

        if (jQuery.trim(jQuery("#<%=ddlDivision.ClientID %>").val()) == "") {
                jQuery("#errorDivision").html("<%=Resources.fbs.required_division%>");
            if (isfocus) {
                jQuery("#<%=ddlDivision.ClientID %>").focus(); isfocus = false;
            }
            //
            isValid = false;
        }
        else {
            jQuery("#errorDivision").html("");
        }

        if (jQuery.trim(jQuery("#<%=ddlDepartment.ClientID %>").val()) == "") {
                jQuery("#errorDepartment").html("<%=Resources.fbs.required_department%>");
                if (isfocus) {
                    jQuery("#<%=ddlDepartment.ClientID %>").focus(); isfocus = false;
                }
                //
                isValid = false;
            }
            else {
                jQuery("#errorDepartment").html("");
            }

            if (isValid) {
                return true;
            }
            else {
                return false;
            }
        }

        function validateEmail(email) {
            var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (!regex.test(email)) {
                return false;
            } else {
                return true;
            }
        }
    </script>
</asp:Content>
