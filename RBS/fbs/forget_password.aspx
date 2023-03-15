<%@ page language="C#" autoeventwireup="true" inherits="forget_password, fbs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forget Password</title>
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style-metro.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/style-responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/default.css" rel="stylesheet" type="text/css" id="style_color" />
    <link href="assets/plugins/uniform/css/uniform.default.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link href="assets/css/login.css" rel="stylesheet" type="text/css" />


    <style type="text/css">
        .blue {
        }
    </style>




</head>
<body class="login" style='background-image: url(assets/img/bg.jpg);'>
    <div class="logo">
    </div>
    <div class="content">
    <form id="fogetpassword" class="form-vertical login-form" runat="server">
         <div  id='div_outside'style="border:0px solid black;width:500px;height:300px;margin-left:-50px;">
            <div style="padding-left:50px; padding-right:30px;">
                 <h2 class="pt">&nbsp;</h2>
               
                 <h2 class="pt">Forget Password</h2>
                  <div class="cg">
             
                 <label class="cl">Enter UserName</label>
             <div class="controls">
					<div class="in-ic left">
						<i class="icon-user"></i>
                               
                                    <asp:TextBox ID="txt_email" runat="server" MaxLength="500" CssClass="mw s12"></asp:TextBox>
                               
                                 </div>
                 </div>
                           
                       
                          
                                <label class="cl">&nbsp;</label>
                               
                                    <asp:Button ID="btnSubmit" class="btn green pull-center m-icon-swapright m-icon-white" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
                                    &nbsp;<span id="isAvailable"></span><asp:Label ID="lblError" runat="server" Visible="false" CssClass="error"></asp:Label>
                       
                    <div id="divmsg" runat="server" visible="false">
                              <%--  <div class="cg" >
                               <div style="margin-left: 280px"  >--%>
                            <label class="cl">&nbsp;</label>
                            <asp:Label ID="msglbl" runat="server" Style="color: blue" align="center"></asp:Label>
                            <br />
                            <br />

                            <asp:Button align="center" class="btn green pull-center m-icon-swapright m-icon-white" ID="btOK" runat="server" Text="OK" OnClick="btOK_Click" Width="111px" />
                            <%--</div>
                                
                               </div>--%>
                        </div>

                    </div>


                </div>
            </div>







        </form>
    </div>
</body>
</html>
