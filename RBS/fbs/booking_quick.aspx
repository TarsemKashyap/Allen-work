<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="booking_quick, fbs" %>

<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <link href="assets/css/fbs.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap-datepicker/css/datepicker.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link href="<%=site_full_path %>assets/plugins/bootstrap-modal/css/bootstrap-modal.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnBookingWindow" runat="server" />
    <div class="c-f">
        <div class="pbd" id="alertError" style="display: none;" runat="server">
            <div class="alr a-err">
                <asp:Literal runat="server" ID="litErrorMsg" Text="<strong>Error!</strong> Your selected time slot has already passed. You can't make the booking."></asp:Literal>
                <button class="close" data-dismiss="alert">
                </button>
            </div>
        </div>

        <div class="pbd" id="alertIsBook" style="display: none;" runat="server">
            <div class="alr a-err">
                <asp:Literal runat="server" ID="litIsBook" Text="<strong>Info!</strong> Don't have permission to book this room."></asp:Literal>
                <button class="close" data-dismiss="alert">
                </button>
            </div>
        </div>

        <div class="alr alert-block " id="divHolidays" style="display: none;" runat="server">
            <strong>Public Holidays!</strong>
            <%=holidays %>
        </div>
        <%--<div class="rfl">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Filter
                        </div>
                        <div class="tools">
                            <a href="javascript:;" class="expand"></a>
                        </div>
                    </div>
                    <div class="pbd hide fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="span2">
                                <div class="cg">
                                    <label class="cl">
                                        Date</label>
                                    <div class="controls">
                                        
                                    </div>
                                </div>
                            </div>
                            <div class="span2 ">
                                <div class="cg">
                                    <label class="cl">
                                        Room Type/Equipment</label>
                                    <div class="controls">
                                        
                                    </div>
                                </div>
                            </div>
                            <div class="span1 ">
                                <div class="cg">
                                    <label class="cl">
                                        Building</label>
                                    <div class="controls">
                                        
                                    </div>
                                </div>
                            </div>
                            <div class="span1 ">
                                <div class="cg">
                                    <label class="cl">
                                        Level</label>
                                    <div class="controls">
                                        
                                    </div>
                                </div>
                            </div>
                            <div class="span1">
                                <div class="cg">
                                    <label class="cl">
                                        Capacity</label>
                                    <div class="controls">
                                        
                                    </div>
                                </div>
                            </div>
                            <div class="span2">
                                <div class="cg">
                                    <label class="cl">
                                        Show My Favourites</label>
                                    <div class="controls">
                                        
                                    </div>
                                </div>
                            </div>
                            <div class="span2">
                                <div style="padding-top: 10px; padding-bottom: 20px;">
                                    
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>--%>
        <div class="rfl">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Quick Booking
                        </div>
                        <div class="actions">
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_startDate" AutoPostBack="false" class="mw m-ctrl-medium date-picker"></asp:TextBox>
                                    <span class="ao"><i class="icon-calendar"></i></span>
                                </div>
                                <a class="btn blue" data-toggle="modal" href="javascript:show_modal_filter();"><i class="icon-filter"></i></a>
                            </div>
                        </div>
                    </div>
                    <div class="pbd" style="padding-bottom: 40px;">
                        <div>

                            <table width="100%">
<tr><td><div style="padding-top: 20px;">
                            <table>
                                <tr>
				    <td><b>Booking Status Legend:</b></td>
                                    <td>
                                        <div style="width:15px; height:15px; display:block;background-color:green;"></div></td>
                                    <td>Upcoming Bookings.</td>
                                    <td>
                                        <div style="width:15px; height:15px; display:block;background-color:orange;"></div></td>
                                    <td>Ongoing Bookings.</td>
                                    <td>
                                        <div style="width:15px; height:15px; display:block;background-color:blue;"></div></td>
                                    <td>Bookings Pending Approval.</td>
                                    <td>
                                        <div style="width:15px; height:15px; display:block;background-color:gray;"></div></td>
                                    <td>Past Bookings or Blocked Rooms.</td>
                                </tr>
                            </table>
                        </div></td></tr>
                                <tr>
                                    <td align="left">
                                        <label>
                                            <asp:DropDownList runat="server" ID="count_select" CssClass="dropdown" Height="31px"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="count_select_SelectedIndexChanged" Width="70px">
                                            </asp:DropDownList>&nbsp;per page  
                                        </label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox runat="server" ID="txt_search" autocomplete="off"></asp:TextBox>
                                        &nbsp;  &nbsp;
                   <asp:Button runat="server" Style="position: relative; margin-top: -7px;" Text="search" CssClass="btn blue" ID="btn_search"
                       OnClick="btn_search_Click" />

                                        <asp:Button ID="btn_clear_search"
                                            Style="position: relative; margin-top: -7px;" CssClass="btn grey" runat="server"
                                            Text="Clear" OnClick="btn_clear_search_Click" />
                                    </td>
                                </tr>
                            </table>

                        </div>
                        <DayPilot:DayPilotScheduler ID="DayPilotScheduler1" runat="server" DataStartField="book_from"
                            DataEndField="book_to" DataTextField="purpose" DataValueField="booking_id" DataResourceField="asset_id"
                            DataTagFields="status" TimeRangeSelectedHandling="JavaScript" CssOnly="true"
                            TimeRangeSelectedJavaScript="timeRangeSelected(start, end, resource)" ClientObjectName="dps"
                            EventClickHandling="JavaScript" EventClickJavaScript="eventClick(e);" 
                            EnableViewState="false" ScrollLabelsVisible="false" ShowToolTip="false" BusinessBeginsHour="0" BusinessEndsHour="24"
                            TreeEnabled="true" EventFontSize="10pt" TreeIndent="15" ResourceHeaderClickHandling="JavaScript"
                            ResourceHeaderClickJavaScript="ShowRoomInfo(resource.value)" CellDuration="15"
                            CellGroupBy="Month" OnBeforeEventRender="DayPilotScheduler1_BeforeEventRender"
                            CellWidth="50" CellHeight="70" RowHeaderWidthAutoFit="true" OnBeforeCellRender="DayPilotScheduler1_BeforeCellRender"
                            BubbleID="DayPilotBubble1" ShowNonBusiness="true" HeaderDateFormat="dd-MMM-yyyy" 
                            RowMinHeight="40" HeightSpec="Fixed" Height="2800"
                            OnCommand="DayPilotScheduler1_Command">
                            <TimeHeaders>
                                <DayPilot:TimeHeader GroupBy="Day" Format="d" />
                                <DayPilot:TimeHeader GroupBy="Hour" />
                                <DayPilot:TimeHeader GroupBy="Cell" />
                            </TimeHeaders>
                            <HeaderColumns>
                                <DayPilot:RowHeaderColumn Width="1" Title="" />
                                <DayPilot:RowHeaderColumn Width="90" Title="Room" />
                                <DayPilot:RowHeaderColumn Width="40" Title="Cap." />
                                <DayPilot:RowHeaderColumn Width="50" Title="Status" />
                                <%-- <DayPilot:RowHeaderColumn Width="90" Title="Room" />
                                <DayPilot:RowHeaderColumn Width="60" Title="Capacity" />
                                <DayPilot:RowHeaderColumn Width="50" Title="Status" />--%>
                            </HeaderColumns>
                        </DayPilot:DayPilotScheduler>
                        <DayPilot:DayPilotBubble ID="DayPilotBubble1" runat="server" CssOnly="true" CssClassPrefix="bubble_default"
                            ClientObjectName="dbs" />

                        <div style="padding: 3px;" class="controls">
                            <asp:Label Style="float: left;" runat="server" ID="lbl_count"></asp:Label>

                            <ul runat="server" id="daypilot_pagination" class="Daypilot_Page nav  p-r ">
                            </ul>
                            <asp:HiddenField runat="server" ID="hdn_ph" />
                            <asp:HiddenField runat="server" ID="hdn_we" />
                            <asp:HiddenField runat="server" ID="save_count" />
                            <asp:HiddenField runat="server" ID="skip_count" />
                            <asp:HiddenField runat="server" ID="total_count" />
                            <asp:HiddenField runat="server" ID="hdn_capacity" />
                            <asp:HiddenField runat="server" ID="hdn_building" />
                            <asp:HiddenField runat="server" ID="hdn_level" />
                            <asp:HiddenField runat="server" ID="hdn_start" />
                            <asp:HiddenField runat="server" ID="hdn_room" />
                            <asp:HiddenField runat="server" ID="hdn_asset_ids" />
                        </div>

                        <div style="position: absolute; padding-top: 20px;">
                            <table>
                                <tr>
				    <td><b>Room Color Status:</b></td>
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
            </div>
        </div>
    </div>
    <div id="responsive" class="modal hide fade" tabindex="-1" data-width="760" aria-hidden="true" style="display: none; width: 760px; margin-left: -380px; margin-top: -221px;">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Filter</h3>
        </div>
        <div class="modal-body">
            <div class="slimScrollDiv" style="position: relative; overflow: hidden; width: auto; height: 150px;">
                <div class="scroller" style="height: 150px; overflow: hidden; width: auto;" data-always-visible="1" data-rail-visible1="1">
                    <div class="row-fluid">
                        <div class="span12">
                            <table width="100%" cellpadding="10" cellspacing="10">
                                <tr>
                                    <td>Category</td>
                                    <td>
                                        <asp:DropDownList ID="ddl_category" runat="server" CssClass="s12 s2-c"></asp:DropDownList></td>
                                    <td>Capacity</td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txt_capacity" MaxLength="5"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Building</td>
                                    <td>
                                        <asp:DropDownList ID="ddl_building" runat="server" CssClass="s12 s2-c"></asp:DropDownList></td>
                                    <td>Level</td>
                                    <td>
                                        <asp:DropDownList ID="ddl_level" runat="server" CssClass="s12 s2-c"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td>Show My Favourites</td>
                                    <td colspan="3">
                                        <asp:CheckBox runat="server" ID="chk_fav"></asp:CheckBox></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <a href="booking_quick.aspx" class="btn grey">Clear</a>
            <asp:Button ID="btn_submit" CssClass="btn blue" runat="server" Text="Search" OnClick="btn_submit_Click" />
            <button type="button" data-dismiss="modal" class="btn">Close</button>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="assets/scripts/modal.js"></script>
    <script type="text/javascript">
        function timeRangeSelected(start, end, resource) {
            if (resource.indexOf("b_") != -1) {
                return;
            }
            var dt = new Date();
            var ut = dt.getTimezoneOffset();
            var val1 = dt.getMinutes();
            var val2 = parseInt(ut);
            diff = dt - start.d;
            diff = diff - ut * 60000;
            var is_book = true;
            var asset_ids = document.getElementById('<%= hdn_asset_ids.ClientID %>').value;
            var view_bookingIDs = asset_ids.split("#");

            for (i = 0; i < view_bookingIDs.length; i++) {
                if (resource == view_bookingIDs[i]) {
                    is_book = false;
                }
            }

            if (diff > 0) {
                document.getElementById('<%=alertError.ClientID%>').style.display = 'block';
                document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'none';
            }
            else {
                if (is_book == false) {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'block';
                }
                else {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'none';

                    show_modal("booking_form.aspx?type=quick&start=" + start.toStringSortable() + "&end=" + end.toStringSortable() + "&r=" + resource + "&hash=<%= PageHash %>");
                }
            }
        }

        function show_modal(url) {
            var modal = dialog();
            modal.closed = function () {
                if (this.result == "OK") {
                    set();
                    dps.commandCallBack('refresh');
                    call_task();
                }
                dps.clearSelection();
            }
            modal.showUrl(url);
        }

        function eventClick(e) {
            show_modal("booking_view.aspx?id=" + e.value() + "&hash=<%= PageHash %>");
        }

        function cancel(e) {
            show_modal("booking_cancel.aspx?id=" + e.value() + "&hash=<%= PageHash %>");
        }

        function ShowRoomInfo(resource) {
            show_modal("asset_info.aspx?&r=" + resource);
        }

        function afterRender(data, isCallBack) {
        }

        function filter(property, value) {
            if (!dps.clientState.filter) {
                dps.clientState.filter = {};
            }
            if (dps.clientState.filter[property] != value) {
                dps.clientState.filter[property] = value;
                dps.commandCallBack('filter');
            }
        }
    </script>
    <script type="text/javascript" src="<%=site_full_path %>assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="<%=site_full_path %>assets/scripts/search.js"></script>
    <script src="<%=site_full_path %>assets/plugins/bootstrap-modal/js/bootstrap-modal.js"></script>
    <script src="<%=site_full_path %>assets/plugins/bootstrap-modal/js/bootstrap-modalmanager.js"></script>
    <script type="text/javascript">
        function show_modal_filter() {
            var obj = $('#responsive').modal();
            obj.parent().appendTo($("form:first"));
        }

        function page_pass(count) {

            if ($('#<%=hdn_level.ClientID %>').val() == "") {
                $('#<%=hdn_level.ClientID %>').val("ALL")
            }
            if ($('#<%=hdn_building.ClientID %>').val() == "") {
                $('#<%=hdn_building.ClientID %>').val("ALL");
            }

            if ($('#<%=hdn_room.ClientID %>').val() == "") {
                $('#<%=hdn_room.ClientID %>').val("ALL")
            }

            var query = "&BUI=" + $('#<%=hdn_building.ClientID %>').val() + "&LEV=" + $('#<%=hdn_level.ClientID %>').val() + "&DAT=" + $('#<%=hdn_start.ClientID %>').val() + "&ROOM=" + $('#<%=hdn_room.ClientID %>').val();

            var _val = "booking_quick.aspx?cap=" + $('#<%=hdn_capacity.ClientID%>').val() + query + "&count=";
            if (count == 999) {
                _val = _val + "nxt";
            }
            else if (count == -1) {
                _val = _val + "prev";
            }
            else if (count == 1) {
                _val = _val + "1";
            }
            else {
                _val = _val + count;
            }
            location.href = _val;
        }
        jQuery(document).ready(function () {

            $('#<%=txt_startDate.ClientID%>').change(function () {
        var count = "1";
        if ($('#<%=hdn_level.ClientID %>').val() == "") {
            $('#<%=hdn_level.ClientID %>').val("ALL")
        }
        if ($('#<%=hdn_building.ClientID %>').val() == "") {
            $('#<%=hdn_building.ClientID %>').val("ALL");
        }
        if ($('#<%=hdn_room.ClientID %>').val() == "") {
            $('#<%=hdn_room.ClientID %>').val("ALL")
        }
        var DAT = $('#<%=txt_startDate.ClientID%>').val();
        var query = "&BUI=" + $('#<%=hdn_building.ClientID %>').val() + "&LEV=" + $('#<%=hdn_level.ClientID %>').val() + "&DAT=" + DAT + "&ROOM=" + $('#<%=hdn_room.ClientID %>').val();
        location.href = "booking_quick.aspx?count=1&cap=" + $('#<%=hdn_capacity.ClientID%>').val() + query;
    });
    $('#<%=hdn_start.ClientID %>').val($('#<%=txt_startDate.ClientID %>').val());
    $('#<%=txt_capacity.ClientID%>').change(function () {
        $('#<%=hdn_capacity.ClientID%>').val($('#<%=txt_capacity.ClientID%>').val());
    });
    $('#<%=ddl_building.ClientID %>').change(function () {
        $('#<%=hdn_building.ClientID %>').val($('#<%=ddl_building.ClientID %>').val());
    });
    $('#<%=ddl_category.ClientID %>').change(function () {

        $('#<%=hdn_room.ClientID %>').val($('#<%=ddl_category.ClientID %>').val());
    });

    $('#<%=ddl_level.ClientID %>').change(function () {
        $('#<%=hdn_level.ClientID %>').val($('#<%=ddl_level.ClientID %>').val());
    });
    $('img').css("margin-top", "-5px");
    $('#ContentPlaceHolder1_txt_startDate').css({ width: "90px" });
    $('#ContentPlaceHolder1_txt_capacity').css({ width: "100px" });
    $('.scheduler_default_timeheader_float_inner').first().html(document.getElementById('<%=txt_startDate.ClientID %>').value);
    $('.scheduler_default_scrollable').scroll(function () {
        $('.scheduler_default_timeheader_float_inner').first().html(document.getElementById('<%=txt_startDate.ClientID %>').value);
            });
    //App.init();
    set_datepicker();
    window.scrollTo(0, 0);
});
    </script>
</asp:Content>
