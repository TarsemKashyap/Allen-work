<%@ page language="C#" autoeventwireup="true" inherits="booking_cancel, fbs" clienttarget="uplevel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
        });

        function SelectAll(id) {
            var frm = document.forms[0];
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "checkbox") {
                    frm.elements[i].checked = document.getElementById(id).checked;
                }
            }
        }

        function cancel_selected() {
            if (confirm("Are you sure you want to cancel the selected bookings?")) {
                var checkboxes = document.getElementsByName("chk_date");
                var checkboxesChecked = "";
                // loop over them all
                for (var i = 0; i < checkboxes.length; i++) {
                    // And stick the checked ones onto an array...
                    if (checkboxes[i].checked) {
                        checkboxesChecked += checkboxes[i].value + "|";
                    }
                }
                if (checkboxesChecked == "")
                    alert("No items selected. Please select at least one item for cancellation.");
                else {
                    $("#<%=hdn_selected.ClientID%>").val(checkboxesChecked);
                    __doPostBack(this, "selected");
                }
            }
        }
    </script>
</head>
<body>
    <div class="p-c">
        <div class="c-f">
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
                            <div class="caption">
Cancel Booking</span>
</div>
                        </div>
                        <div class="pbd form">
                            <form class="form-horizontal" runat="server">
                                <asp:HiddenField runat="server" ID="hdn_selected" />
                                <div class="form-horizontal form-view">
                                    <div class="rfl">
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
                                                <input class="mw span8" readonly type="text" runat="server" id="lbl_email" />
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
                                                <textarea class="mw span8" readonly type="text" rows="5" runat="server" id="lbl_remarks" />
                                            </div>
                                        </div>
                                        <div class="cg" runat="server" id="div_housekeeping">
                                            <label class="cl" style="width: 175px;">
                                                Housekeeping Required
           <abbr title="<%=Resources.fbs.tooltip_housekeeping%>" rel="tooltip" data-toggle='bottom'>
               <img src="../assets/img/Info.png" alt='' />
           </abbr>
                                            </label>
                                            <div class="controls">
                                                <input class="mw span8" readonly type="text" runat="server" id="lbl_housekeeping" />
                                            </div>
                                        </div>
                                        <div class="cg" runat="server" id="div_manpower">
                                            <label class="cl">
                                                Manpower Required
         <abbr title="<%=Resources.fbs.tooltip_setup%>" rel="tooltip" data-toggle='bottom'>
             <img src="../assets/img/Info.png" alt='' />
         </abbr>
                                            </label>
                                            <div class="controls">
                                                <input class="mw span8" readonly type="text" runat="server" id="lbl_Setup" />
                                            </div>
                                        </div>
                                        <div class="cg" runat="server" id="div_setup">
                                            <label class="cl">
                                                Setup Type</label>
                                            <div class="controls">
                                                <input class="mw span8" readonly type="text" runat="server" id="lbl_setupetype" />
                                            </div>
                                        </div>
                                        <div class="cg" runat="server" id="div_meetingtype">
                                            <label class="cl">
                                                Meeting Type</label>
                                            <div class="controls">
                                                <input class="mw span8" readonly type="text" runat="server" id="lbl_meetingtype" />
                                            </div>
                                        </div>
                                        <div class="cg">
                                            <label class="cl">
                                                Requested By</label>
                                            <div class="controls">
                                                <input class="mw span8" readonly type="text" runat="server" id="lbl_requestedBy" />
                                            </div>
                                        </div>
                                        <div class="cg" id="Div_Rej_reason" runat="server" visible="false">
                                            <label class="cl">
                                                <%=reasons%></label>
                                            <div class="controls">
                                                <input class="mw span8" readonly type="text" runat="server" id="txt_rejectedreason" />
                                            </div>
                                        </div>
                                        <div class="caption">
                                            <h4>Booking(s)</h4>
                                        </div>
                                        <div class="cg">
                                            <%=html_asset%>
                                        </div>
                                        <div class="caption">
                                            <h4 id="h4_invite" runat="server">Invite list</h4>
                                        </div>
                                        <div class="cg" id="contrlgrp_invite" runat="server">
                                            <%=html_invitelist %>
                                        </div>
                                        <%if (show_resources)
                                            { %>
                                        <div class="caption">
                                            <h4 id="h4_resource" runat="server">Additional Resources</h4>
                                        </div>
                                        <%} %>
                                        <div class="cg" id="contrlgrp_resource" runat="server">
                                            <%=html_resourcelist %>
                                        </div>
                                    </div>

                                </div>
                                <asp:HiddenField ID="hdn_asset_field" runat="server" />
                                <asp:HiddenField ID="hdnBookingID" runat="server" />
                                <div class="cg" runat="server" id="div_cancelreasons">
                                    <label class="cl">
                                        Cancel Reason</label>
                                    <div class="controls">
                                        <textarea class="span8 mw " runat="server" name="txt_reason" id="txt_reason" data-placeholder="Cancel Reason is required"></textarea>
                                        <span id="reason_error" runat="server"></span>
                                    </div>
                                </div>
                                <div class="fa">
                                    <a href="javascript:cancel_selected();" class="btn red" id="link_cancel_selected" runat="server">Cancel Selected Booking</a>
                                    <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" />
                                    <!-- <asp:LinkButton id="thisisabugfix" runat="server"> </asp:LinkButton> -->
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>
    <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="assets/scripts/app.js" type="text/javascript"></script>
</body>
</html>
