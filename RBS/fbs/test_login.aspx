<%@ page language="C#" autoeventwireup="true" inherits="test_login, fbs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>Username</h1>
        <p><ul>
            <li>If you are using Active Directory account to test, please use your AD username.</li>
            <li>If you are using a Local Account to test, please use your email address.</li>
           </ul></p>
        <asp:TextBox runat="server" ID="txt_email" Visible="false"></asp:TextBox>
        <br />
        <asp:Button runat="server" id="btn_submit" OnClick="btn_submit_Click" Text="Login" Visible="false" />

        <asp:Literal runat="server" ID="lit_text" Visible="false"></asp:Literal>
    </div>
    </form>
</body>
</html>
