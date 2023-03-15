<%@ page language="C#" autoeventwireup="true" inherits="add_rooms, fbs" %>

<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function getRoomsId() {

            var frm = document.forms[0];
            var cnt = 0;
            var val = "";
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "radio") {
                    if (frm.elements[i].checked) {
                        var id = frm.elements[i].id;
                        if (id.indexOf(id) != -1) {
                            cnt = cnt + 1;
                            val = val + frm.elements[i].value + "#";
                        }
                    }
                }
            }
            if (cnt > 0) {
                document.getElementById('<%=hdnvalues.ClientID%>').value = val;

            }
            else {
                alert("Please select room(s)!!");
                return false;
            }
        }
    </script>

</head>



<body class="p-h-f">
    <div class="rfl" style="margin-top: 20px;">
        <div class="p-c">
            <div class="c-f">
                <div class="pbd">
                    <div class="rfl">
                        <div class="s12">
                            <div class="portlet box blue s10">
                                <div class="ptt">
                                    <div class="caption">
                                        Booking Details -
                                    <asp:Label runat="server" ForeColor="#A4212C" ID="lbl_assetname_heading"></asp:Label>
                                    </div>
                                </div>
                                <div class="pbd" id="alt_error_alrdybook" runat="server" visible="false">
                                    <div class="alr a-err">
                                        <asp:Literal runat="server" ID="litError" Text=""></asp:Literal>
                                        <button class="close" data-dismiss="alert">
                                        </button>
                                    </div>
                                </div>
                                <div class="pbd form">
                                    <form action="#" id="form_sample_2" class="form-horizontal" runat="server">
                                        <div class="form-horizontal form-view">
                                            <div class="rfl">
                                                <div class="cg">
                                                    <label class="cl">
                                                        Booked From</label>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="txt_from" />
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <label class="cl">
                                                        Booked To</label>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="txt_to" />
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <label class="cl">
                                                        Purpose</label>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_purpose" />
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <label class="cl">
                                                        Booked For</label>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_bookedfor" />
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <label class="cl">
                                                        Email</label>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_email" />
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <label class="cl">
                                                        Telephone</label>
                                                    <div class="controls">
                                                        <input class="mw span8" readonly="readonly" type="text" runat="server" id="lbl_telephone" />
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <label class="cl">
                                                        Remarks</label>
                                                    <div class="controls">
                                                        <textarea class="mw span8 " readonly="readonly" type="text" rows="10" runat="server" id="lbl_remarks"></textarea>
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <div class="caption">
                                                        <h4>Available Room List</h4>
                                                    </div>
                                                </div>
                                                <div class="cg">
                                                    <%=htmltable%>
                                                </div>
                                            </div>
                                            <asp:HiddenField ID="hdnID" runat="server" />
                                            <asp:HiddenField ID="hdnRecID" runat="server" />
                                            <asp:HiddenField ID="hdnEventID" runat="server" />
                                            <asp:HiddenField ID="hdnvalues" runat="server" />
                                            <div class="fa">
                                                <asp:Button runat="server" ID="btn_save" Text="Save" CssClass="btn green" OnClick="btn_save_Click" OnClientClick="return getRoomsId()" />
                                                <asp:Button runat="server" ID="btn_close" Text="Close" CssClass="btn grey" OnClick="btn_close_Click" />
                                                <asp:Button runat="server" ID="btn_cancel" Text="Cancel" CssClass="btn grey" OnClick="btn_cancel_Click" />
                                            </div>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
