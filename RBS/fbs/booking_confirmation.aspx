<%@ page language="C#" autoeventwireup="true" inherits="booking_confirmation, fbs" %>

<!DOCTYPE html >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />    
    <script type="text/javascript">
        function close() {
            parent.$.fancybox.close();
        }
    </script>
</head>
<body>
    <%-- <form id="form1" runat="server">--%>
    <div class="p-c">
        
        <div class="c-f">
            
            <div class="rfl">
                <div class="s12">
                    <h3 class="pt-header">
                        Booking Details - <asp:Label runat="server" ForeColor="#A4212C" ID="lbl_assetname_heading"></asp:Label>
                    </h3>
                </div>
            </div>
            
            
            <div runat="server" class="pbd" id="alertInfo" style="display: block">
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
            <div class="pbd">
                             
            <div class="rfl">
                <div class="s12">
                    <div class="portlet box blue">
                        <div class="ptt">
                            <%--<div class="caption">
                                <i class="icon-reorder"></i>Meeting Meeting</div>--%>
                        </div>
                        <div class="pbd form">
                            <form action="#" id="form_sample_2" class="form-horizontal" runat="server">
                                <asp:HiddenField ID="hdnBookingID" runat="server" />
                            <asp:PlaceHolder runat="server" ID="control_booking_view"></asp:PlaceHolder>
                            <div class="fa">
                                 <a  target="_parent"  runat="server" id="btn_additional_resource" class="btn green">Additional Resources</a>
                                <%-- <a  target="_parent"  runat="server" id="btn_catering" class="btn green">Catering</a>--%>
                                <asp:Button ID="btn_close" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_close_Click" />
                            </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
        
    </div>
</body>
    <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
</html>
