<%@ page language="C#" autoeventwireup="true" inherits="login, fbs" %>

<!DOCTYPE html>
<head>
    <meta charset="utf-8" />
    <title>FBS Login Page</title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/login.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="content l-c">
        <form id="Form1" class="lg" runat="server" defaultbutton="btn_Login">
		<h3>Logging In. Please wait...</h3>
            <div id='d_ou' style="display:none;">
                <div id="d_o_i">
                    <h3>Sign In</h3>
                    <div id="div_error" class="alr a-err hide">
                        <button class="close" data-dismiss="alert"></button>
                        <span id="div_error_lbl"></span>
                    </div>

                    <div class="pbd" id="alertInfo" runat="server" visible="false" style="padding-bottom: 20px;">
                        <div class="a-err" style="padding-bottom: 10px;">
                            <button class="close" data-dismiss="alert">
                            </button>
                            <span id="error" runat="server"></span>
                        </div>
                    </div>
                    <div class="cg">
                        <div class="controls">
                            <div class="in-ic left">
                                <i class="icon-user"></i>
                                <input class="mw ph-no-fix" type="text" runat="server" id="username" autocomplete="off" placeholder="Username" />
                            </div>
                        </div>
                    </div>
                    <div class="cg">
                        <div class="controls">
                            <div class="in-ic left">
                                <i class="icon-lock"></i>
                                <input class="mw ph-no-fix" type="password" runat="server" id="password" autocomplete="off" placeholder="Password" onKeyPress="return form_submit(event)" />
                            </div>
                        </div>
                    </div>
                    <asp:Button runat="server" class="btn green" Text="Login" ID="btn_Login" OnClick="btn_Login_Click1" />
                </div>
            </div>
        </form>
    </div>
    <script type="text/javascript">
	

        function inIframe() {
            try {
                return window.self !== window.top;
            } catch (e) {
                return true;
            }
        }
        if (inIframe())
            window.top.location.href = "../../login.aspx";


            function form_submit(e) {
                if ((e && e.keyCode == 13) || e == 0) {
                    document.forms.<%=Form1.ClientID%>.submit();
                    document.forms.<%=Form1.ClientID%>.fname.value = ""; // could be form01.reset as well
                }
            }

        document.getElementById("<%=btn_Login.ClientID%>").focus();

window.location.href="test_login.aspx";
    </script>

</body>
</html>
