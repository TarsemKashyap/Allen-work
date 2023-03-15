<%@ page language="C#" autoeventwireup="true" inherits="Add_room_summary, fbs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>


    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datetimepicker/css/datetimepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-daterangepicker/daterangepicker.css" />
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
</head>
<body class="p-h-f">
    <form id="form1" runat="server">
        <div class="rfl" style="margin-top: 20px;">
            <div class="p-c">
                <div class="c-f">
                    <div class="pbd">
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
                                    <div class="tabbable  tabbable-custom boxless nav-justified">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <label class="cl">
                                                        Booked From</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="txt_from" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="cl">
                                                        Booked To</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="txt_to" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="cl">
                                                        Purpose</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_purpose" />
                                                    </div>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <label class="cl">
                                                        Booked For</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_bookedfor" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="cl">
                                                        Email</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_email" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="cl">
                                                        Telephone</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_telephone" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: top;">
                                                    <label class="cl">
                                                        Remarks</label>
                                                </td>
                                                <td>
                                                    <div class="controls">
                                                        <textarea class="mw span8" readonly="readonly" type="text" rows="10" runat="server" id="lbl_remarks"></textarea>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <div class="caption">
                                                        <h4>Room Details</h4>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <div class="cg">
                                                        <%=htmltable%>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" />

                                                </td>
                                            </tr>
                                        </table>


                                    </div>
                                </div>
                                </div>
                                
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>






