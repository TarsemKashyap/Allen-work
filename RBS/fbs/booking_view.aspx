<%@ page language="C#" autoeventwireup="true" inherits="booking_view, fbs" validaterequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
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
    <%-- <form id="form1" runat="server">--%>
    <div class="p-c">
        <div class="c-f">
            <div class="rfl">
                <div class="s12">
                    <h3 class="pt-header"></h3>
                </div>
            </div>
            <div runat="server" class="pbd" id="alertInfo" style="display: none;">
                <div class="alr a-inf">
                    <asp:Literal runat="server" ID="litInfoMsg" Text=""></asp:Literal>
                    <button class="close" data-dismiss="alert">
                    </button>
                </div>
            </div>
            <div class="pbd">
            </div>
            <div runat="server" class="pbd" id="alertError" style="display: none;">
                <div class="alr a-err">
                    <asp:Literal runat="server" ID="litErrorMsg" Text=""></asp:Literal>
                    <button class="close" data-dismiss="alert">
                    </button>
                </div>
            </div>
            <div class="rfl">
                <div class="s12">
                    <div class="portlet box blue">
                        <div class="ptt">
                            <div class="caption">
                                Booking Details -
                        <asp:Label runat="server" ForeColor="#A4212C" ID="lbl_assetname_heading"></asp:Label>
                            </div>
                        </div>
                        <div class="pbd form">
                            <form action="#" id="form_sample_2" class="form-horizontal" runat="server">
                                <asp:PlaceHolder runat="server" ID="control_booking_view"></asp:PlaceHolder>
                                <asp:HiddenField ID="hdnID" runat="server" />
                                <asp:HiddenField ID="hdnRecID" runat="server" />
                                <asp:Panel runat="server" ID="pnl_cancel" Visible="false">
                                    <table width="100%" cellpadding="10" cellspacing="10">
                                        <tr>
                                            <td colspan="2" style="width: 80%">
                                                <h4>Enter Reason for Cancellation (Optional)</h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="cg" runat="server" id="div_cancel_series">
                                                    <div class="controls s12">
                                                        <asp:CheckBox runat="server" ID="ckb_cancel" Text="Cancel all future meetings in this series" TextAlign="Right" />
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox runat="server" ID="txt_cancel_reason" TextMode="MultiLine" Rows="3" Columns="100"></asp:TextBox></td>
                                            <td>
                                                <asp:Button ID="btn_confirm_cancel" CssClass="btn red" runat="server" Text="Confirm Cancellation" OnClick="btn_confirm_cancel_Click" /><br />
                                                <br />
                                                <asp:Button ID="btn_cancel_confirm" CssClass="btn grey" runat="server" Text="Go Back" OnClick="btn_cancel_confirm_Click" /></td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <div class="fa">
                                    <center>
                                        <asp:Button ID="btn_start_meeting" CssClass="btn green" runat="server" Text="Start Meeting"
                                        OnClick="btn_start_meeting_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_transfer" CssClass="btn orange" runat="server" Text="Request for Transfer"
                                        OnClick="btn_transfer_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_cancel_booking" CssClass="btn red" runat="server"
                                        Text="Cancel Booking" OnClick="btn_cancel_booking_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_actualize" CssClass="btn green" runat="server" Text="End Meeting"
                                        OnClick="btn_actualize_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_noshow" CssClass="btn red" runat="server" Text="No Show" OnClick="btn_noshow_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_edit_booking" CssClass="btn green" runat="server" Text="Edit"
                                        OnClick="btn_edit_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_addroom" CssClass="btn green" runat="server" Text="Add Room"
                                        OnClick="btn_addroom_Click" OnClientClick="show_loading();" />
                                    <asp:Button ID="btn_re_assign" CssClass="btn green" runat="server" Text="Re-Assign"
                                        OnClick="btn_re_assign_Click" Visible="false" OnClientClick="show_loading();" />
                                    <a target="_parent" runat="server" id="btn_additional_resource" class="btn green" visible="false">Resources</a>
                                        <%--<a runat="server" id="A1" class="btn green" href="resources_form.aspx">Manage Resources</a>--%>
                                    <a target="_parent" runat="server" id="btn_clone" class="btn green">Clone</a>
                                    <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" OnClientClick="show_loading();" />
                                        <asp:HiddenField runat="server" ID="hdn_repeat_reference_id" />
                                        </center>
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
<script type="text/javascript" src="assets/scripts/app.js"></script>


<script type="text/javascript">
    $(window).load(function () {
        // Animate loader off screen
        $(".se-pre-con").fadeOut("slow");;
    });
    function show_loading() {
        document.getElementById("loader").style.display = "block";
    }
    function close(result) {
        DayPilot.Modal.close(result);
    }

    jQuery(document).ready(function () {
        App.init();       
    });
   
</script>

<script type="text/javascript">
    $(document).ready(function () {
        //$('#btn_cancel_booking').click(function () {
        //    if (confirm("Are you sure want to Cancel the Booking?")) {
        //        return true;

        //    }
        //    else {
        //        return false;
        //    }
        //});
    });

</script>

</html>
