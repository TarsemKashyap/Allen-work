<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="booking_wizard, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnBookingWindow" runat="server" />
    <div class="c-f">

        <div class="rfl">
            <div class="s12">

                <h3 class="pt">Booking Wizard
                </h3>
            </div>
        </div>
        <div runat="server" class="pbd" id="alertError" style="display: none;">
            <div class="alr a-err">
                <asp:Literal runat="server" ID="litErrorMsg" Text="<strong>Error!</strong>"></asp:Literal>
                <button class="close" data-dismiss="alert">
                </button>
            </div>
        </div>

        <div class="pbd" id="alertIsBook" style="display: none;" runat="server">
            <div class="alr a-err">
                <asp:Literal runat="server" ID="litIsBook" Text="<strong>Info!</strong> Do not have permission to book this room."></asp:Literal>
                <button class="close" data-dismiss="alert">
                </button>
            </div>
        </div>

        <div class="alr alert-block " id="divHolidays" style="display: none;" runat="server">
            <strong>Public Holidays!</strong>
            <%=holidays %>
        </div>
        <div class="rfl">
            <div class="span3">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Filter
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Room Type/Equipment</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_category" runat="server" CssClass="s2-c"
                                            data-placeholder="Choose a Building">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12">
                                <div class="cg">
                                    <div class="controls">
                                        <asp:CheckBox runat="server" ID="chkAllDay" Text="All Day"  TextAlign="Left" OnCheckedChanged="chkAllDay_CheckedChanged" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12">
                                <div class="cg">
                                    <label class="cl">
                                        From</label>
                                    <div class="controls">
                                        <div class="i-a date datepicker">

                                            <asp:TextBox runat="server" ID="txt_startDate" class="mw m-ctrl-medium date-picker" Style="width: 85px;"
                                                OnTextChanged="txt_startDate_TextChanged" AutoPostBack="true" CausesValidation="false"></asp:TextBox>
                                            <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>

                                            <asp:DropDownList ID="ddl_StartTime" runat="server" AutoPostBack="true" CausesValidation="false"
                                                OnSelectedIndexChanged="ddlStartTime_SelectedIndexChanged" Style="width: 100px;">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        To</label>
                                    <div class="controls">
                                        <div class="i-a date form_date datepicker" data-date-format="dd-mm-yyyy" data-date-viewmode="years"
                                            data-date-minviewmode="months">

                                            <asp:TextBox runat="server" ID="txt_EndDate" class="mw m-ctrl-medium date-picker" Style="width: 85px;"
                                                OnTextChanged="txt_endDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                            <asp:DropDownList ID="ddl_EndTime" runat="server" Style="width: 100px;">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Building</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_building" runat="server" CssClass="s2-c"
                                            data-placeholder="Choose a Building">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Level</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_level" runat="server" CssClass="s2-c"
                                            data-placeholder="Choose a Building">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Capacity</label>
                                    <div class="controls">
                                        <asp:TextBox runat="server" ID="txt_capacity" MaxLength="5"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Show My Favourites</label>
                                    <div class="controls">
                                        <asp:CheckBox runat="server" ID="chk_fav"></asp:CheckBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div>
                            <asp:Button ID="btn_check_availability" CssClass="btn blue" runat="server" Text="Check Availability"
                                OnClick="btn_check_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="span9">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Booking Wizard
                        </div>
                    </div>

                    <div class="pbd">
                        <%=htmltable %>
                        <div id="footer_msg" runat="server" visible="false">
                            <table>
                                <tr>
                                    <td>
                                        <img src="assets/img/bo.png" /></td>
                                    <td>Rooms that can be booked.</td>
                                    <td>
                                        <img src="assets/img/ba.png" /></td>
                                    <td>Rooms that require approval.</td>
                                    <td>
                                        <img src="assets/img/bn.png" /></td>
                                    <td>Rooms that cannot be booked.</td>
                                </tr>
                            </table>
                        </div>
                    </div>

                </div>
                <div class="fa" runat="server" id="divAction">
                    <asp:Literal ID="litSubmit" runat="server"></asp:Literal>
                    <asp:Button ID="btn_submit" Visible="false" CssClass="btn green" runat="server" Text="Next" OnClientClick="if ( ! showBookingForm()) return false;" />
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="HiddenStart" runat="server" />
    <asp:HiddenField ID="HiddenEnd" runat="server" />
    <asp:HiddenField ID="hdf_redirect" runat="server" />
    <asp:HiddenField ID="hdnCloneID" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">

    <script type="text/javascript">
        function showBookingForm() {
            var frm = document.forms[0];
            var cnt = 0;
            var ass_id = 0;

            var is_book = "view";
            var class_name = "";
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "radio") {
                    if (frm.elements[i].checked) {
                        var id = frm.elements[i].id;
                        if (id.indexOf(id) != -1) {
                            cnt = cnt + 1;
                            ass_id = frm.elements[i].value;
                            class_name = frm.elements[i].className;
                        }
                    }
                }
            }

            if (class_name.indexOf("book") != -1) { is_book = "book"; }

            if (cnt > 0) {

                if (is_book == "view") {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'block';
                }
                else {
                    var start = document.getElementById('<%=HiddenStart.ClientID%>').value;
                    var end = document.getElementById('<%=HiddenEnd.ClientID%>').value;
                    timeRangeSelected(start, end, ass_id);
                }
            }
            else {
                alert("<%=Resources.fbs.required_delete%>");
            }
        }
        function timeRangeSelected(start, end, resource) {
            var dt = new Date();
            var ut = dt.getTimezoneOffset();
            var val1 = dt.getMinutes();
            var val2 = parseInt(ut);
            diff = dt - start.d;
            diff = diff - ut * 60000;
            if (diff > 0) {
                document.getElementById('alertError').style.display = 'block';
                document.getElementById('alertIsBook').style.display = 'none';
            }
            else {
                document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'none';
                var modal = dialog();
                modal.closed = function () {
                    if (this.result == "OK") {
                        if ($('#<%=hdf_redirect.ClientID%>').val() == "wizard") {
                        }
                        else {
                            //window.location = "/MyBookings_month.aspx";
                            window.location = "/bookings.aspx";
                        }
                    }
                };
                modal.height = 800;
                modal.showUrl("booking_form.aspx?type=wizard&start=" + start + "&end=" + end + "&r=" + resource + "&Rdct=repeat");


            }
        }

    </script>
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script type="text/javascript" src="assets/scripts/search.js"></script>
    <script src="assets/scripts/table-managed.js"></script>
    <script type="text/javascript">
        function assetinfo(asset_id) {
            try {
                $('#<%=hdf_redirect.ClientID%>').val("wizard");

                var modal = dialog();
                modal.closed = function () {
                    if (this.result == "OK") {
                        if ($('#<%=hdf_redirect.ClientID%>').val() == "wizard") {
                        }
                        else {
                            //window.location = "/MyBookings_month.aspx";
                            window.location = "/bookings.aspx";
                        }
                    }
                };
                modal.showUrl("asset_info.aspx?&r=" + asset_id);


            }
            catch (ex) {
            }
        }
        jQuery(document).ready(function () {
            Search.init();
            TableManaged.init();
            var hdnBookingWindow = document.getElementById('<%= hdnBookingWindow.ClientID %>').value;
            
            set_datepicker();
            set_table("available_list_table");
        });
    </script>

</asp:Content>
