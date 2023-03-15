<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="booking_repeat, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnBookingWindow" runat="server" />
    <div class="c-f">
        <div class="rfl">
            <div class="s12">                
                <h3 class="pt">Repeat Booking
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
                <asp:Literal runat="server" ID="litIsBook" Text="<strong>Error!</strong> Selected assets is not permission to booking."></asp:Literal>
                <button class="close" data-dismiss="alert">
                </button>
            </div>
        </div>
        <div class="alr alert-block " id="divHolidays" style="display: none;" runat="server">
            <strong>Public Holidays!</strong>
            <%=holidays %>
        </div>
        <div class="rfl">
            <div class="span6">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Step 1: Select Pattern Details
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlPattern" runat="server" Style="width: 100px;" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlPattern_SelectedIndexChanged">
                                            <Items>
                                                <asp:ListItem Text="Daily" Value="Daily" />
                                                <asp:ListItem Text="Weekly" Value="Weekly" />
                                                <asp:ListItem Text="Monthly" Value="Monthly" />
                                            </Items>
                                        </asp:DropDownList>
                                        <table id="dailytable" runat="server" visible="true">
                                            <tr>
                                                <td>
                                                    <label>
                                                        Every:<span class="required" style="color: red">*</span> &nbsp;
                                                    </label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtDailyEvery" runat="server" MaxLength="3" Style="width: 50px;" onkeypress="return isNumber(event);" />
                                                    &nbsp;day(s)
                                                </td>
                                                <%--   <td valign="top">
                                                                <label class="cl">
                                                                    days.</label>
                                                            </td>--%>
                                            </tr>
                                        </table>
                                        <table id="weeklytable" runat="server" visible="false">
                                            <tr>
                                                <td>
                                                    <label>
                                                        Every:<span class="required" style="color: red">*</span>  &nbsp;   
                                                    </label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtWeeklyEvery" MaxLength="3" runat="server" Style="width: 50px;" onkeypress="return isNumber(event);" />
                                                    &nbsp; week(s)
                                                </td>
                                                <%--<td valign="top">
                                                                <label class="cl">
                                                                    weeks.</label>
                                                            </td>--%>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <label class="cl">
                                                        On:<span class="required">*</span></label>
                                                </td>
                                                <td colspan="2">
                                                    <div class="controls">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkSun" runat="server" />
                                                                    Sunday
                                                                </td>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkMon" runat="server" />
                                                                    Monday
                                                                </td>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkTue" runat="server" />
                                                                    Tuesday
                                                                </td>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkWed" runat="server" />
                                                                    Wednesday
                                                                </td>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkThu" runat="server" />
                                                                    Thursday
                                                                </td>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkFri" runat="server" />
                                                                    Friday
                                                                </td>
                                                                <td>
                                                                    <input type="checkbox" value="" id="chkSat" runat="server" />
                                                                    Saturday
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <table id="monthlytable" runat="server" visible="false">
                                            <tr>
                                                <td>
                                                    <input type="radio" id="rndMonthlyEvery" name="rndMonth" runat="server" />Day:<span style="color: red">*</span> &nbsp;
                                                                <asp:TextBox ID="txtMonthlyDay" MaxLength="3" runat="server" Style="width: 50px" onkeypress="return isNumber(event);" />
                                                    &nbsp;  of every &nbsp;
                                                           
                                                                <asp:TextBox ID="txtMonthlyMonth" MaxLength="3" runat="server" Style="width: 50px" onkeypress="return isNumber(event);" />
                                                    &nbsp;month(s)
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table cellpadding="2" cellspacing="2">
                                                        <tr>
                                                            <td style="vertical-align: central;">
                                                                <input type="radio" id="rndMonthlySpecific" name="rndMonth" runat="server" />The: &nbsp;
                                                                            <asp:DropDownList ID="cboMonthlyCount" runat="server" Style="width: 100px;">
                                                                                <asp:ListItem Text="First" Value="1" />
                                                                                <asp:ListItem Text="Second" Value="2" />
                                                                                <asp:ListItem Text="Third" Value="3" />
                                                                                <asp:ListItem Text="Fourth" Value="4" />
                                                                                <asp:ListItem Text="Last" Value="5" />
                                                                            </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cboMonthlyWeekday" Style="width: 100px;" runat="server">
                                                                    <asp:ListItem Text="Sunday" Value="Sunday" />
                                                                    <asp:ListItem Text="Monday" Value="Monday" />
                                                                    <asp:ListItem Text="Tuesday" Value="Tuesday" />
                                                                    <asp:ListItem Text="Wednesday" Value="Wednesday" />
                                                                    <asp:ListItem Text="Thursday" Value="Thursday" />
                                                                    <asp:ListItem Text="Friday" Value="Friday" />
                                                                    <asp:ListItem Text="Saturday" Value="Saturday" />
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>&nbsp;of every &nbsp;
                                                                           
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtMonthlyEveryCount" MaxLength="3" runat="server" Style="width: 50px;" onkeypress="return isNumber(event);" />&nbsp;month(s).
                                                            </td>
                                                        </tr>
                                                    </table>
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
            <div class="span6">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Step 2: Select Date Range
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                         <div class="rfl">
                            <div class="s12">
                                <div class="cg">
                                    <div class="controls">
                                        <asp:CheckBox runat="server" ID="chkAllDay" Text="All Day" TextAlign="Left" OnCheckedChanged="chkAllDay_CheckedChanged" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <table>
                                        <tr>
                                            <td>
                                                <table cellpadding="10" cellspacing="10">
                                                    <tr>

                                                        <td style="vertical-align: center">
                                                            <label class="cl">
                                                                Start Date</label>
                                                        </td>
                                                        <td style="vertical-align: top">
                                                            <div class="controls">
                                                                <div class="i-a date datepicker">
                                                                    <asp:TextBox runat="server" size="16" ID="txt_startDate" class="mw m-ctrl-medium date-picker"
                                                                        Width="106px" OnTextChanged="txt_startDate_TextChanged" AutoPostBack="true" CausesValidation="false"></asp:TextBox><span
                                                                            class="ao"><i class="icon-calendar"></i></span>
                                                                </div>


                                                            </div>
                                                        </td>
                                                        <td valign="top">
                                                            <label class="cl">From Time: </label>
                                                        </td>
                                                        <td valign="top">&nbsp;<asp:DropDownList ID="ddl_StartTime" runat="server" AutoPostBack="true" CausesValidation="false"
                                                            OnSelectedIndexChanged="ddlStartTime_SelectedIndexChanged" Width="100px">
                                                        </asp:DropDownList></td>
                                                        <td valign="top">
                                                            <label class="cl">To Time:</label></td>
                                                        <td valign="top">
                                                            <asp:DropDownList ID="ddl_EndTime" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="vertical-align: central;" class="auto-style1">
                                                            <label>
                                                                <input type="radio" id="rndEndDate" name="rnd" runat="server" />
                                                                &nbsp; End Date
                                                            </label>
                                                        </td>
                                                        <td valign="top">
                                                            <div class="controls">
                                                                <div class="i-a date datepicker">
                                                                    <asp:TextBox runat="server" size="16" ID="txt_EndDate" class="mw m-ctrl-medium date-picker"
                                                                        Width="106px" OnTextChanged="txt_endDate_TextChanged" AutoPostBack="true" CausesValidation="false"></asp:TextBox><span class="ao"><i class="icon-calendar"></i></span>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td style="vertical-align: center;">
                                                            <label>
                                                                <input type="radio" id="rndEndsCount" name="rnd" runat="server" />
                                                                Number of Events  &nbsp;
                                                            </label>
                                                        </td>
                                                        <td style="vertical-align: center;" align="left">
                                                            <div class="controls">
                                                                <asp:TextBox ID="txtNoOfEvents" runat="server" Style="width: 50px;" MaxLength="3" onkeypress="return isNumber(event);" />
                                                            </div>
                                                        </td>
                                                        
                                                    </tr>
                                                    

                                                </table>
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
        <div class="rfl">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Step 3: Set Room Filter Criteria
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <table>
                                        <tr>
                                            <td>
                                                <table cellpadding="10" cellspacing="10">
                                                    <tr>
                                                        <td valign="top">
                                                            <label class="cl">
                                                                Room Type/Equipment</label>
                                                        </td>
                                                        <td valign="top">
                                                            <div class="controls">
                                                                &nbsp;
                                                                <asp:DropDownList ID="ddl_category" runat="server" Width="150px"
                                                                    data-placeholder="Choose category">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </td>
                                                        <td valign="top">
                                                            <label class="cl">
                                                                Building &nbsp;&nbsp;&nbsp;&nbsp;</label>
                                                        </td>
                                                        <td valign="top">
                                                            <div class="controls">
                                                                <asp:DropDownList ID="ddl_building" runat="server" Width="100px"
                                                                    data-placeholder="Choose a Building">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </td>
                                                        <td valign="top">
                                                            <label class="cl">
                                                                Level&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                                                        </td>
                                                        <td valign="top">
                                                            <div class="controls">
                                                                <asp:DropDownList ID="ddl_level" runat="server" Width="100px"
                                                                    data-placeholder="Choose level">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </td>

                                                        <td valign="top">
                                                            <label class="cl">
                                                                Capacity</label>
                                                        </td>
                                                        <td valign="top">
                                                            <div class="controls">
                                                                <asp:TextBox runat="server" ID="txt_capacity" MaxLength="5" Width="140px"></asp:TextBox>
                                                            </div>
                                                        </td>
                                                        <td colspan="2">
                                                            <label class="cl">
                                                                Show My Favourites</label>
                                                        </td>
                                                        <td colspan="4">
                                                            <div class="controls">
                                                                <asp:CheckBox runat="server" ID="chk_fav"></asp:CheckBox>
                                                            </div>
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btn_check_availability" CssClass="btn blue" runat="server" Text="Check Availability"
                                                                OnClick="btn_check_Click" />
                                                        </td>
                                                    </tr>
                                                </table>
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
        <div class="rfl" id="div1" runat="server" visible="false">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Chosen Dates
                        </div>
                    </div>
                    <div class="pbd ">
                        <div class="rfl">
                            <div class="s12 ">
                                <%=htmlDates %>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="rfl" id="divFacility" runat="server">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Available Rooms
                        </div>
                    </div>
                    <div class="pbd">
                    <div>
                        <%=htmltable %>
                        <div id="footer_msg" runat="server" visible="false">
                            <table>
                                <tr>
                                    <td><img src="assets/img/bo.png" /></td>
                                    <td>Rooms that can be booked.</td>
                                    <td><img src="assets/img/ba.png" /></td>
                                    <td>Rooms that require approval.</td>
                                    <td><img src="assets/img/bn.png" /></td>
                                    <td>Rooms that cannot be booked.</td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="fa">
                        <asp:Literal ID="litSubmit" runat="server"></asp:Literal>
                        <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Next" OnClientClick="if ( ! showBookingSummary()) return false;" />
                    </div>
                        </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript">
        function assetinfo(asset_id) {
            try {
                            var modal = dialog();
                            modal.showUrl("asset_info.aspx?&r=" + asset_id);
            }
            catch (ex) {
            }
        }

        function showBookingSummary() {
            var frm = document.forms[0];
            var cnt = 0;
            var issueIDs = "";
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

            if ((cnt - 1) > 0) {

                if (is_book == "view") {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'block';
                }
                else {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'none';
                    location.href = "booking_repeat_summary.aspx?asset=" + ass_id;
                }
            }
            else {
                alert("<%=Resources.fbs.required_delete%>");
            }
        }
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
    </script>
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script src="assets/scripts/table-managed.js"></script>
    <script type="text/javascript">

        jQuery(document).ready(function () {
            TableManaged.init();
            var hdnBookingWindow = document.getElementById('<%= hdnBookingWindow.ClientID %>').value;
            set_datepicker();
            set_table("available_list_table");
        });
    </script>
</asp:Content>
