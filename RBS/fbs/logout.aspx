<%@ page language="C#" autoeventwireup="true" inherits="logout, fbs" %>

<!DOCTYPE html>
<!--[if IE 8]> <html lang="en" class="ie8"> <![endif]-->
<!--[if IE 9]> <html lang="en" class="ie9"> <![endif]-->
<!--[if !IE]><!-->
<html lang="en">
<!--<![endif]-->
<!-- BEGIN HEAD -->
<head>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
</head>
<body class="p-h-f p-f-w" style="background-color:#fff !important;">
    <div class="p-con rfl">
        <form id="Form1" runat="server" style="height: 340px;">
            <br />
            <br />
            <br />
            <center>
        <div>
            <div id="alert_logout" runat="server" visible="false" style='position:relative;margin-left:130px;'>
                <asp:Label id="error" runat="server"></asp:Label>
            </div>
            <div id="divWelcome" runat="server">               
                <div>
                    
                <h3><img src="assets/img/nparks.png" style="height:150px;width:auto;" /><br /><br />You have been successfully logged out of the</h3>
                </div>
                <div>
                <h3>Room Booking System</h3>
                    <h3>To Login again, click on the "Login" button.<br /><br />
                        <a  class="btn green" href="login.aspx"><i class="icon-signin"></i> &nbsp;  Login</a>
                    </h3>
                </div>
            </div>
        </div>       
		    </center>
        </form>
    </div>
    </div>
</body>
</html>
