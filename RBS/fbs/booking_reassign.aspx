<%@ page language="C#" autoeventwireup="true" inherits="booking_reassign, fbs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%-- <meta http-equiv="X-UA-Compatible" content="IE=edge" />--%>
  <%--  <meta http-equiv="X-UA-Compatible" content="IE=IE11" />--%>
    
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
     <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
        <style>
        .no-js #loader {
            display: none;
        }

        .js #loader {
            display: block;
            position: absolute;
            left: 100px;
            top: 0;
        }

        .se-pre-con {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            opacity:0.5;
            z-index: 9999;
            background: url(assets/img/l_c.gif) center no-repeat #fff;
        }
    </style>  
</head>
<body>
   <div class="se-pre-con" id="loader"></div>
    <div class="p-c">
        
        <div class="c-f">
            
            <div class="rfl">
                <div class="s12">
                    <h3 class="pt-header">
                    </h3>
                </div>
            </div>
            
            
            <div runat="server" class="pbd" id="alertInfo" style="display: block">
                <div class="alr a-inf">
                    <asp:Literal runat="server" ID="litInfoMsg" Text=""></asp:Literal>
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
                            <div class="caption">
                                Booking Details
                            </div>
                        </div>
                        <div class="pbd form">
                            <form action="#" id="form_sample_2" class="form-horizontal" runat="server">
                             <asp:HiddenField ID="hdnBookingID" runat="server" />
                                <asp:HiddenField runat="server" ID="hdn_repeat_reference" />
                            <div class="cg">
                                <label class="cl">
                                    Purpose</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_purpose" />
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Booked For</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_bookedfor" />
                                </div>
                            </div>                            
                            <div class="cg">
                                <label class="cl">
                                    Email</label>
                                <div class="controls">

                                    <asp:TextBox runat="server" class="mw span8" ID="txt_email"></asp:TextBox>
                                    <img  alt="Loading.." runat="server" ID="img_loading" style="display:none;height:40px;"  src="~/assets/img/ajax-loading.gif" />
                                                           
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Telephone</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_telephone" />
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Remarks</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_remarks" />
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Housekeeping Required</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_housekeeping" />
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Setup Required</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_Setup" />
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Setup Type</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_setupetype" />
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">
                                    Current Owner</label>
                                <div class="controls">
                                    <input class="mw span8" readonly type="text" runat="server" id="lbl_requestedBy" />
                                </div>
                            </div>
                                <div class="cg">
                                <label class="cl">
                                    <b>Re-Assign To</b><span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddl_reassign" runat="server" CssClass="span8 s2-c" onchange="javascript:check_assign();"
                                        data-placeholder="Choose re-assign to" Width="450">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="caption">
                                <h4>
                                    Room Details</h4>
                            </div>
                            <div class="cg">
                                <%=html_asset%>
                            </div>
                            <div class="caption">
                                <h4 id="h4_invite" runat="server">
                                    Invite list</h4>
                            </div>
                            <div class="cg" id="contrlgrp_invite" runat="server" >
                                <%=html_invitelist %>
                            </div>
                            <div class="fa">
                                <asp:Button ID="btnReAssign" runat="server" CssClass="btn green" Text="Re-Assign" style="display:none;"
                                     OnClick="btnReAssign_Click" OnClientClick="javascript:return reassign_confirm();">
                                </asp:Button>
                                <asp:Button ID="btn_close" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_close_Click" />
                            </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
        
    </div>
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>  
    <script type="text/javascript" src="assets/scripts/form-components.js"></script>
    <script src="assets/scripts/app.js"></script>
    <script type="text/javascript">
        function check_assign()
        {
            if($("#<%=ddl_reassign.ClientID%>").val() !="0")
            {
                $("#<%=btnReAssign.ClientID%>").css("display", "block");
            }
            else
                $("#<%=btnReAssign.ClientID%>").css("display", "none");
        }

         function reassign_confirm() {             
             var value = false;
             if (confirm("Are you sure you want to reassign?")) {
                 show_loading();
                 value = true;
             }
             else {
                 value = false;
             }

             return value;
        }

        $(document).ready(function () {           
            $('.s2-c').select2({
                placeholder: "Select an option",
                allowClear: true
            });

            App.init();
            fade_wheel();

        });

        function show_loading() {
            document.getElementById("loader").style.display = "block";
        }

        function fade_wheel() {
            $(".se-pre-con").fadeOut("slow");
        }
    </script>
</body>
</html>
