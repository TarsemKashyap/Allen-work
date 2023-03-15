<%@ page title="" language="C#" autoeventwireup="true" inherits="view_booking, fbs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <%-- <meta http-equiv="X-UA-Compatible" content="IE=edge" />--%>
  <%--  <meta http-equiv="X-UA-Compatible" content="IE=IE11" />--%>
    
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    
    <script src="../assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
</head>
<body>
    <div class="p-c">        
        <div class="c-f">            
            <div class="rfl">
                <div class="s12">
                    <h3 class="pt-header">
                        Booking Details - <asp:Label runat="server" ForeColor="#A4212C" ID="lbl_assetname_heading"></asp:Label>
                    </h3>
                </div>
            </div>
            <div runat="server" class="pbd" id="alertInfo" style="display: none;">
                <div class="alr a-inf">
                    <asp:Literal runat="server" ID="litInfoMsg" Text="<strong>Info!</strong>"></asp:Literal>
                    <button class="close" data-dismiss="alert">
                    </button>
                </div>
            </div>
            <div runat="server" class="pbd" id="alertError" style="display: none;">
                <div class="alr a-err">
                    <asp:Literal runat="server" ID="litErrorMsg" Text="<strong>Error!</strong>"></asp:Literal>
                    <button class="close" data-dismiss="alert">
                    </button>
                </div>
            </div>
            <div class="rfl">
                <div class="s12">
                    <div class="portlet box blue">
                        <div class="ptt">
                           <%-- <div class="caption">
                                <i class="icon-reorder"></i>Meeting Meeting</div>--%>
                        </div>
                        <div class="pbd form">
                            <form action="#" id="form_sample_2" class="form-horizontal" runat="server">
                            <asp:PlaceHolder runat="server" ID="control_booking_view"></asp:PlaceHolder>
                            <asp:HiddenField ID="hdnBookingID" runat="server" />
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
    <script src="assets/plugins/bootstrap-hover-dropdown/twitter-bootstrap-hover-dropdown.min.js" type="text/javascript"></script>
    <script src="assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js" type="text/javascript"></script>
   <%-- <script src="assets/plugins/jquery.blockui.min.js" type="text/javascript"></script>
    <script src="assets/plugins/jquery.cookie.min.js" type="text/javascript"></script>--%>
    <script src="assets/plugins/uniform/jquery.uniform.min.js" type="text/javascript"></script>
  
    <script src="assets/scripts/app.js" type="text/javascript"></script>
   <%-- <script type="text/javascript" src="assets/plugins/chosen-bootstrap/chosen/chosen.jquery.min.js"></script>--%>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            App.init();
            activateTab('tab_default');
        });

        /////////////////////////
        function activateTab(tab) {
            $('.nav-tabs a[href="#' + tab + '"]').tab('show');
        }
    </script>
</body>
</html>
