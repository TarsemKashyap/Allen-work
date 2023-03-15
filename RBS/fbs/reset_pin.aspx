<%@ page title="" language="C#" autoeventwireup="true" inherits="reset_pin, fbs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset PIN</title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/login.css" rel="stylesheet" type="text/css" />
    <script src="<%=site_full_path %>assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
</head>
<body class="login">
    <div class="content l-c" style="height:400px;">
        <form id="Form1" class="lg" runat="server" defaultbutton="btn_Login">
            <div id='d_ou'>
                <div id="d_o_i">
                    <h3>Reset PIN</h3>
                    <div class="cg">
                        <asp:Label ID="label1" runat="server"></asp:Label>
                        <label class="cl">Current PIN<span class="required">*</span><span id="errorCurrentPassword" class="requiredField"></span></label>
                        <div class="controls">
                            <div class="in-ic left">
                                    <asp:TextBox TextMode="Password" ID="txt_currentPassword" runat="server" class="mw ph-no-fix"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="cg">
                        <label class="cl">New PIN<span class="required">*</span> <span id="errorPassword" class="requiredField"></span></label>
                        <div class="controls">
                            <div class="in-ic left">
                                    <asp:TextBox TextMode="Password" ID="txt_Password" runat="server" class="mw ph-no-fix"></asp:TextBox>
                            </div>
                        </div>

                    </div>

                    <div class="cg">
                        <label class="cl">Repeat PIN<span class="required">*</span> <span id="error2Password" class="requiredField"></span></label>
                        <div class="controls">
                            <div class="in-ic left">
                                    <asp:TextBox TextMode="Password" ID="txt_rePassword" runat="server" class="mw ph-no-fix"></asp:TextBox>
                            </div>
                        </div>

                    </div>

                    <div class="controls">
                        <label class="cl">&nbsp;</label>
                        <asp:Button ID="btn_Login" CssClass="btn green pull-center m-icon-swapright m-icon-white" runat="server" Text="Change Password" OnClientClick="return UserForm_Validation();" OnClick="btnSubmit_Click" />
                        &nbsp;<asp:Label ID="lblError" runat="server" Visible="false" CssClass="error"></asp:Label>
                        <a href="bookings.aspx" class="btn blue">Skip</a>
                    </div>
                </div>
            </div>
        </form>
    </div>

    <script type="text/javascript">

        function UserForm_Validation() {

            var isValid = true;
            var isfocus = true;

            if (jQuery.trim(jQuery("#<%=txt_currentPassword.ClientID%>").val()) == "") {
                jQuery("#errorCurrentPassword").html("<%=Resources.fbs.required_password %>");
                if (isfocus) {
                    jQuery("#<%=txt_currentPassword.ClientID %>").focus(); isfocus = false;
                }

                isValid = false;
            }
            else {
                jQuery("#errorCurrentPassword").html("");
            }


            if (jQuery.trim(jQuery("#<%=txt_rePassword.ClientID%>").val()) == "") {
	            jQuery("#error2Password").html("<%=Resources.fbs.required_password %>");
                if (isfocus) {
                    jQuery("#<%=txt_rePassword.ClientID %>").focus(); isfocus = false;
                }

                isValid = false;
            }
            else {
                jQuery("#error2Password").html("");
            }

            if (jQuery.trim(jQuery("#<%=txt_Password.ClientID%>").val()) == "") {
	            jQuery("#errorPassword").html("<%=Resources.fbs.required_password %>");
                  if (isfocus) {
                      jQuery("#<%=txt_Password.ClientID %>").focus(); isfocus = false;
                }

                isValid = false;
            }
            else {
                if (jQuery.trim(jQuery("#<%=txt_Password.ClientID %>").val()).length <= 4) {
                      jQuery("#errorPassword").html("<%=Resources.fbs.password_validation %>");
                     if (isfocus) {
                         jQuery("#<%=txt_Password.ClientID %>").focus(); isfocus = false;

                }
                isValid = false;
            }

            else {
                jQuery("#errorPassword").html("");

            }
        }



        if (jQuery.trim(jQuery("#<%=txt_rePassword.ClientID %>").val()) != jQuery.trim(jQuery("#<%=txt_Password.ClientID%>").val())) {

	            jQuery("#error2Password").html("<%=Resources.fbs.password_match %>");
                if (isfocus) {
                    jQuery("#<%=txt_rePassword.ClientID %>").focus(); isfocus = false;

                }
                isValid = false;
            }
            else {
                jQuery("#error2Password").html("");
            }


            if (isValid) {
                return true;
            }
            else {
                return false;
            }
        }



    </script>
</body>
</html>
