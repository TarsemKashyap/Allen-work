<%@ page title="" language="C#" autoeventwireup="true" inherits="reject_mytask, fbs" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
	<link href="../assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css"/>
	<link href="../assets/plugins/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" type="text/css"/>
	<link href="../assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css"/>
	<link href="../assets/css/style-metro.css" rel="stylesheet" type="text/css"/>
	<link href="../assets/css/style.css" rel="stylesheet" type="text/css"/>
	<link href="../assets/css/style-responsive.css" rel="stylesheet" type="text/css"/>
	<link href="../assets/css/default.css" rel="stylesheet" type="text/css" id="style_color"/>
	<%--<link rel="stylesheet" type="text/css" href="../assets/plugins/select2/select2_metro.css" />--%>
    <link rel="stylesheet" href="../assets/plugins/data-tables/DT_bootstrap.css" />
	
   
    
    <script src="../assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function Mytask() {

            var tab = jQuery('#hidtab').val();
            parent.location.href = 'mytask.aspx?action=' + tab;

        }
        function close_fancybox() {

        }
        function validate_reject_form() {

            if (jQuery.trim(jQuery("#<%=txt_remarks.ClientID %>").val()) == "") {

                jQuery("#lblRemarksError").html("Please enter remarks.");
                jQuery("#<%=txt_remarks.ClientID %>").val("");
                jQuery("#<%=txt_remarks.ClientID %>").focus();
                return false;
            } else {
                jQuery("#lblRemarksError").html("");
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="p-c">
        <div class="c-f">
            <div class="portlet box blue">
                <div class="ptt">
                    <div class="caption">
                        <i class="icon-reorder"></i>Reject Request</div>
                </div>
                <div class="pbd form">
                    <span class="error">
                        </span>
                    
                    <div class="rfl">
                        <div class="span6 ">
                            <div class="cg">
                                <label class="cl">
                                    Remarks<span class="required">*</span> <span class="error" id="lblRemarksError"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txt_remarks" runat="server" TextMode="MultiLine" CssClass="mw s12" Width="719"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        
                    </div>
                    
                    <div class="fa">
                        <asp:HiddenField ID="hidWorkflowID" runat="server" />                        
                        <asp:HiddenField ID="hidtab" runat="server" />                        
                        <asp:Button ID="btn_reject" CssClass="btn red" runat="server" Text="Reject Request"
                            OnClientClick="return validate_reject_form();" OnClick="btn_reject_Click" />
                        <asp:Button ID="Button1" CssClass="btn grey" runat="server" Text="Close"
                            OnClientClick="javascript:parent.$.fancybox.close(); return false;" 
                            onclick="Button1_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
