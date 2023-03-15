<%@ page language="C#" autoeventwireup="true" inherits="myprofile, fbs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
	<meta content="" name="description" />
	<meta content="" name="author" />
	
	<link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css"/>
	<link href="assets/plugins/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" type="text/css"/>
	<link href="assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css"/>
	<link href="assets/css/style-metro.css" rel="stylesheet" type="text/css"/>
	<link href="assets/css/style.css" rel="stylesheet" type="text/css"/>
	<link href="assets/css/style-responsive.css" rel="stylesheet" type="text/css"/>
	<link href="assets/css/default.css" rel="stylesheet" type="text/css" id="style_color"/>
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="form-horizontal form-view" style="height:380px;z-index:9999;">
        <h3>
            
        </h3>
        <h3 class="form-section">
            My Profile</h3>
        <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Email</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblEmail" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>            
        </div>
        <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Title</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblTitle" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
            <div class="span6 ">
                <div class="cg">
                    <label class="cl" for="lastName">
                        Full Name</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblFullName" runat="server"></asp:Label></span>
                    </div>
                </div>
                
            </div>
        </div>
           <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Given Name</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblGivenName" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
            
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Native Name</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblNativeName" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>
          <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Staff ID</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblStaffID" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
            
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Institution</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblInstitution" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>
        <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Division</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblDivision" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
            
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Department</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblDepartment" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>
             <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Section</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblSection" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
            
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Designation</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblDesignation" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>
        <div class="rfl">
            <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        Office Phone</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblOfficePhone" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
             <div class="span6 ">
                <div class="cg">
                    <label class="cl">
                        PIN</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lblPIN" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>
        <div class="rfl">
            <div class="s12 ">
                <div class="cg">
                    <label class="cl">
                        Groups</label>
                    <div class="controls">
                        <span class="text bold"><asp:Label ID="lbl_groups" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>
     
    </div>
    </form>
</body>
</html>
