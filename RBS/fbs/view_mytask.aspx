<%@ page language="C#" autoeventwireup="true" inherits="view_mytask, fbs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .btn {
            background-color: #e5e5e5;
            background-image: none;
            border: 0 none;
            color: #333333;
            cursor: pointer;
            filter: none;
            font-family: "Segoe UI",Helvetica,Arial,sans-serif;
            font-size: 14px;
            outline: medium none;
            padding: 8px;
        }
    </style>


    <script src="../assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>

</head>
<body>
    <div class="p-c">
        <div class="c-f">
            <div class="rfl">
                <div class="s12">
                    <div class="portlet box blue">
                        <div class="ptt">
                            <h3>Task Details</h3>
                        </div>
                        <div class="pbd form">
                            <form id="form1" runat="server">
                                <asp:PlaceHolder runat="server" ID="ph_control"></asp:PlaceHolder>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="assets/scripts/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js" type="text/javascript"></script>
    <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="assets/plugins/uniform/jquery.uniform.min.js" type="text/javascript"></script>
    <script src="assets/scripts/app.js" type="text/javascript"></script>
</body>
</html>
