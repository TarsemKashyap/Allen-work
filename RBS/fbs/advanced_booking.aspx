<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="advanced_booking, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <link href="assets/plugins/bootstrap-modal/css/bootstrap-modal.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style>
        .form_datetime {
            z-index: 10000;
        }
    </style>
    <div class="c-f">
        <div class="rfl">
            <div class="s12">
                <%=html_error %>
            </div>
        </div>
        <div class="rfl">
            <div class="span3">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Filter (Before or After choosing dates)
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Room Type/Equipment</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_category" runat="server" CssClass="s2-c">
                                        </asp:DropDownList>
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
                                        <asp:DropDownList ID="ddl_building" runat="server" CssClass="s2-c">
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
                                        <asp:DropDownList ID="ddl_level" runat="server" CssClass="s2-c">
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
                            <asp:Button ID="btn_check_availability" CssClass="btn blue" runat="server" Text="Refresh" OnClick="btn_check_availability_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="span9">
                <div class="pbd" id="alt_err" runat="server">
                    <div class="alr a-err">
                        <asp:Literal runat="server" ID="litError" Text=""></asp:Literal>
                    </div>
                </div>
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Choose One or More Dates
                        </div>
                        <div class="actions">
                            <%if (show_clear)
                                { %>
                            <div class="bgp">
                                <a class="btn blue" href="javascript:change_time();">
                                    <i class="icon-time"></i>&nbsp;Bulk Change Timings &nbsp;
                                </a>
                            </div>
                            <%} %>
                            <div class="bgp">
                                <a class="btn green" href="#" data-toggle="dropdown">
                                    <i class="icon-calendar"></i>&nbsp;Choose Dates &nbsp;
										<i class="icon-angle-down"></i>
                                </a>
                                <ul class="ddm p-r">
                                    <li><a data-toggle="modal" href="javascript:show_single();"><i class="icon-calendar"></i>&nbsp;Single</a></li>
                                    <li><a data-toggle="modal" href="javascript:show_daily();"><i class="icon-repeat"></i>&nbsp;Daily</a></li>
                                    <li><a data-toggle="modal" href="javascript:show_weekly();"><i class="icon-repeat"></i>&nbsp;Weekly</a></li>
                                    <li><a data-toggle="modal" href="javascript:show_monthly();"><i class="icon-repeat"></i>&nbsp;Monthly</a></li>
                                </ul>
                            </div>
                            <%if (show_clear)
                                { %>
                            <div class="bgp">
                                <a class="btn red" href="#" data-toggle="dropdown">
                                    <i class="icon-trash"></i>&nbsp;Clear &nbsp;
										<i class="icon-angle-down"></i>
                                </a>
                                <ul class="ddm p-r">
                                    <li><a data-toggle="modal" href="javascript:clear_selected();"><i class="icon-calendar"></i>&nbsp;Selected</a></li>
                                    <li><a data-toggle="modal" href="javascript:clear_all();"><i class="icon-repeat"></i>&nbsp;All</a></li>
                                </ul>
                            </div>
                            <%} %>
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <table cellpadding="10" cellspacing="10">
                                <tr>
                                    <td>Set common room for all dates</td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddl_common" onchange="javascript:select_room();">
                                        </asp:DropDownList></td>
                                    <td>
                                        <asp:CheckBox runat="server" ID="chk_all" OnCheckedChanged="chk_all_CheckedChanged" AutoPostBack="true" /></td>
                                    <td>Show all rooms</td>
                                </tr>
                            </table>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <table class='table table-striped table-bordered table-hover' id='available_list_table'>
                                    <thead>
                                        <tr>
                                            <th style='width: 8px;'>#</th>
                                            <th style='width: 8px;'>
                                                <input type="checkbox" name="items" id="chk_select_dates" onclick="SelectAll(this.id)" /></th>
                                            <th class='hidden-280'>Book Status</th>
                                            <th class='hidden-280'>From</th>
                                            <th class='hidden-280'>To</th>
                                            <th class='hidden-280'>Remarks</th>
                                            <th class='hidden-480'>Room</th>
                                            <th style='width: 10%;'>View Room</th>
                                            <th style='width: 10%;'>Change Date</th>
                                            <th style='width: 8px;'>Remove</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <%=html_table %>
                                    </tbody>
                                </table>
                                <asp:HiddenField runat="server" ID="hdn_selected" />
                            </div>
                            <div class="s12 ">
                                <asp:Button runat="server" ID="btn_submit" CssClass="btn green" Text="Submit" OnClick="btn_submit_Click" UseSubmitBehavior="false" Visible="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="daily" class="modal hide fade" tabindex="-1" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Daily Recurring</h3>
        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 350px" data-always-visible="1" data-rail-visible1="1">
                <div class="rfl">
                    <div class="span12">
                        <div id="daily_error"></div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">From<span class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_daily_start" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">To<span class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_daily_end" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">No. of events</label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="txt_daily_number_of_events" class="mw m-ctrl-medium" onkeypress="return check_numeric(event);"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">Start Time<span class="required">*</span></label>
                            <div class="controls">
                                <asp:DropDownList ID="ddl_daily_from_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">End Time<span class="required">*</span></label>
                            <div class="controls">
                                <asp:DropDownList ID="ddl_daily_to_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">All day</label>
                            <div class="controls">
                                <asp:CheckBox runat="server" ID="chk_daily_all_day" onclick="javascript:all_day(this);" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span12 ">
                        <div class="cg">
                            <label class="cl">Occurs Every<span class="required">*</span></label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="txt_daily_occurence" class="mw m-ctrl-medium " onkeypress="return check_numeric(event);"></asp:TextBox>
                                days
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span6 ">
                        <div class="cg">
                            <label class="cl">If a day falls on a weekend/public holiday?</label>
                            <div class="controls">
                                <asp:DropDownList runat="server" ID="ddl_daily_weekend_option">
                                    <asp:ListItem Text="Do Nothing" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Next Working Day" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Previous Working Day" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span6 "></div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
            <asp:Button runat="server" ID="btn_daily" CssClass="btn blue" Text="Add" OnClick="btn_daily_Click" UseSubmitBehavior="false" />
        </div>
    </div>

    <div id="weekly" class="modal hide fade" tabindex="-1" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Weekly Recurring</h3>
        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 300px" data-always-visible="1" data-rail-visible1="1">
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">From<span class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_weekly_start" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">To<span class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_weekly_end" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">No. of events</label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="txt_weekly_number_of_events" class="mw m-ctrl-medium " onkeypress="return check_numeric(event);"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">Start Time<span class="required">*</span></label>
                            <div class="controls">
                                <asp:DropDownList ID="ddl_weekly_from_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">End Time<span class="required">*</span></label>
                            <div class="controls">
                                <asp:DropDownList ID="ddl_weekly_to_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">All day</label>
                            <div class="controls">
                                <asp:CheckBox runat="server" ID="chk_weekly_all_day" onclick="javascript:all_day(this);" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span12 ">
                        <div class="cg">
                            <label class="cl">Occurs Every<span class="required">*</span></label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="txt_weekly_occurence" class="mw m-ctrl-medium " onkeypress="return check_numeric(event);"></asp:TextBox>
                                weeks
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span12 ">
                        <div class="cg">
                            <label class="cl">Occurs On<span class="required">*</span><asp:Label ID="lblchkError" ForeColor="Red" runat="server" ClientIDMode="Static"></asp:Label></label>
                            <div class="controls">
                                <table width="100%" cellpadding="5" cellspacing="5">
                                    <tr>
                                        <td>
                                            <%-- <input type="checkbox" value="" id="chkSun" name="chkSun" runat="server" />--%>
                                            <asp:CheckBox ID="chkSun" runat="server" ClientIDMode="Static" />
                                            Sunday
                                        </td>
                                        <td>
                                            <%--<input type="checkbox" value="" id="chkMon" runat="server" />--%>
                                            <asp:CheckBox ID="chkMon" runat="server" ClientIDMode="Static" />
                                            Monday
                                        </td>
                                        <td>
                                            <%--<input type="checkbox" value="" id="chkTue" runat="server" />--%>
                                            <asp:CheckBox ID="chkTue" runat="server" ClientIDMode="Static" />
                                            Tuesday
                                        </td>
                                        <td>
                                            <%--<input type="checkbox" value="" id="chkWed" runat="server" />--%>
                                            <asp:CheckBox ID="chkWed" runat="server" ClientIDMode="Static" />
                                            Wednesday
                                        </td>
                                        <td>
                                            <%--<input type="checkbox" value="" id="chkThu" runat="server" />--%>
                                            <asp:CheckBox ID="chkThu" runat="server" ClientIDMode="Static" />
                                            Thursday
                                        </td>
                                        <td>
                                            <%--<input type="checkbox" value="" id="chkFri" runat="server" />--%>
                                            <asp:CheckBox ID="chkFri" runat="server" ClientIDMode="Static" />
                                            Friday
                                        </td>
                                        <td>
                                            <%--<input type="checkbox" value="" id="chkSat" runat="server" />--%>
                                            <asp:CheckBox ID="chkSat" runat="server" ClientIDMode="Static" />
                                            Saturday
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span6 ">
                        <div class="cg">
                            <label class="cl">If a day falls on a public holiday?</label>
                            <div class="controls">
                                <asp:DropDownList runat="server" ID="ddl_weekly_weekend_option">
                                    <asp:ListItem Text="Do Nothing" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Next Working Day" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Previous Working Day" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span6 "></div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
            <asp:Button runat="server" ID="btn_weekly" CssClass="btn blue" Text="Add" OnClick="btn_weekly_Click" OnClientClick="return btn_week();" />
        </div>
    </div>

    <div id="monthly" class="modal hide fade" tabindex="-1" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Monthly Recurring</h3>
        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 400px" data-always-visible="1" data-rail-visible1="1">
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">From<span class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_monthly_start" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">To<span class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_monthly_end" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">No. of events</label>
                            <div class="controls">
                                <asp:TextBox runat="server" ID="txt_monthly_number_of_events" class="mw m-ctrl-medium " onkeypress="return check_numeric(event);"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">Start Time<span class="required">*</span></label>
                            <div class="controls">
                                <asp:DropDownList ID="ddl_monthly_from_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">End Time<span class="required">*</span></label>
                            <div class="controls">
                                <asp:DropDownList ID="ddl_monthly_to_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">All day</label>
                            <div class="controls">
                                <asp:CheckBox runat="server" ID="chk_monthly_all_day" onclick="javascript:all_day(this);" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span12 ">
                        <div class="cg">
                            <label class="cl"><b>Pattern</b></label>
                            <div class="controls">
                                <table id="monthlytable" runat="server" cellpadding="5" cellspacing="5">
                                    <tr>
                                        <td>
                                            <input type="radio" id="rndMonthlyEvery" name="rndMonth" runat="server" />
                                        </td>
                                        <td>Day:<span style="color: red">*</span>
                                            <asp:TextBox ID="txtMonthlyDay" MaxLength="3" runat="server" Style="width: 50px" onkeypress="return check_numeric(event);" />
                                            &nbsp;  of every &nbsp;
                                                           
                                                                <asp:TextBox ID="txtMonthlyMonth" MaxLength="3" runat="server" Style="width: 50px" onkeypress="return check_numeric(event);" />
                                            &nbsp;month(s)
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <div class="rfl">
                                                <div class="span6 ">
                                                    <div class="cg">
                                                        <label class="cl">If a day falls on a weekend/public holiday?</label>
                                                        <div class="controls">
                                                            <asp:DropDownList runat="server" ID="ddl_monthly_weekend_option">
                                                                <asp:ListItem Text="Do Nothing" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="Next Working Day" Value="2"></asp:ListItem>
                                                                <asp:ListItem Text="Previous Working Day" Value="3"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="span6 ">
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <table cellpadding="5" cellspacing="5">
                                                <tr>
                                                    <td>
                                                        <input type="radio" id="rndMonthlySpecific" name="rndMonth" runat="server" />
                                                    </td>
                                                    <td>The: &nbsp;
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
                                                        <asp:TextBox ID="txt_monthly_frequency" MaxLength="3" runat="server" Style="width: 50px;" onkeypress="return check_numeric(event);" />&nbsp;month(s).
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
        <div class="modal-footer">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
            <asp:Button runat="server" ID="btn_monthly" CssClass="btn blue" Text="Add" OnClick=" btn_monthly_Click" UseSubmitBehavior="false" />
        </div>
    </div>

    <div id="single" class="modal hide fade" tabindex="-1" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Specific Date</h3>
        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 200px" data-always-visible="1" data-rail-visible1="1">
                <div class="rfl" id="div-date">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">From<span id="span_from_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_single_date" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>

                                    <asp:DropDownList ID="ddl_single_start_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span2 ">
                        <div class="cg">
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">To<span id="span_to_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_single_to_date" class="mw m-ctrl-medium date-picker" Style="width: 85px;" onchange="javascript:date_changed(this);"></asp:TextBox>

                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                    <asp:DropDownList ID="ddl_single_end_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span2 ">
                        <div class="cg">
                            <label class="cl">All day</label>
                            <div class="controls">
                                <asp:CheckBox runat="server" ID="chk_single_all_day" onclick="javascript:all_day(this);" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" data-dismiss="modal" class="btn">Close</button>
                <asp:Button runat="server" ID="btn_specific" CssClass="btn blue" Text="Add" OnClick="btn_specific_Click" UseSubmitBehavior="false" />
            </div>
        </div>
    </div>

    <div id="change" class="modal hide fade" tabindex="-1" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Change Date</h3>
        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 100px" data-always-visible="1" data-rail-visible1="1">
                <div class="rfl" id="div-date">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">From<span id="span_from_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_change_from_date" class="mw m-ctrl-medium date-picker" Style="width: 85px;"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                    <asp:DropDownList ID="ddl_change_from_time" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span2 ">
                        <div class="cg">
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">To<span id="span_to_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_change_to_date" class="mw m-ctrl-medium date-picker" Style="width: 85px;"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"></span>
                                    <asp:DropDownList ID="ddl_change_to_time" runat="server" Style="width: 100px;">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span2 ">
                        <div class="cg">
                            <label class="cl">All day</label>
                            <div class="controls">
                                <asp:CheckBox runat="server" ID="chk_change_all_day" onclick="javascript:all_day(this);" /><asp:HiddenField runat="server" ID="hdn_change" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
            <asp:Button runat="server" ID="btn_change" CssClass="btn blue" Text="Change" OnClick="btn_change_Click" UseSubmitBehavior="false" />
        </div>
    </div>

    <div id="change_time" class="modal hide fade" tabindex="-1" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>Change Timings</h3>
        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 150px" data-always-visible="1" data-rail-visible1="1">
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">Original From Time<span id="span_from_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a">
                                    <asp:DropDownList ID="ddl_o_from" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span2 ">
                        <div class="cg">
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">Original To Time<span id="span_from_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a">
                                    <asp:DropDownList ID="ddl_o_to" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="rfl">
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">New From Time<span id="span_from_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a">
                                    <asp:DropDownList ID="ddl_n_from" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span2 ">
                        <div class="cg">
                        </div>
                    </div>
                    <div class="span4 ">
                        <div class="cg">
                            <label class="cl">New To Time<span id="span_from_date" class="required">*</span></label>
                            <div class="controls">
                                <div class="i-a">
                                    <asp:DropDownList ID="ddl_n_to" runat="server" Style="width: 100px;" onchange="javascript:set_to_time(this);">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
            <asp:Button runat="server" ID="btn_bulk_change_time" CssClass="btn blue" Text="Change" OnClick="btn_bulk_change_time_Click" UseSubmitBehavior="false" />
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script src="assets/scripts/table-managed.js"></script>
    <script src="assets/plugins/bootstrap-modal/js/bootstrap-modal.js" type="text/javascript"></script>
    <script src="assets/plugins/bootstrap-modal/js/bootstrap-modalmanager.js" type="text/javascript"></script>
    <script src="assets/scripts/moment.js"></script>
    <script type="text/javascript" src="assets/scripts/modal.js"></script>
    <script type="text/javascript">
        function select_room() {
            var valx = $("#<%=ddl_common.ClientID%>").val();
            $('.ddlroom').each(function (i, obj) {
                $("#" + obj.id).val(valx);
            });

            validate_ddl();
        }

        function validate_ddl() {
            $('.ddlroom').each(function (i, obj) {
                if ($("#" + obj.id).val() == null)
                    $("#" + obj.id).css('background-color', '#ff0000 !important');
                else
                    $("#" + obj.id).css('background-color', '#ffffff !important');
            });
        }

        function SelectAll(id) {
            var frm = document.forms[0];
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "checkbox") {
                    frm.elements[i].checked = document.getElementById(id).checked;
                }
            }
        }

        function view(ddl_id) {
            var modal = dialog();
            modal.showUrl("asset_info.aspx?&r=" + $("#" + ddl_id).val());
        }

        function date_changed(obj) {
            if (obj.id == '<%=txt_single_date.ClientID%>') {
                $("#<%=txt_single_to_date.ClientID %>").val($("#<%=txt_single_date.ClientID %>").val());
            }
            if (obj.id == '<%=txt_daily_start.ClientID%>') {
                $("#<%=txt_daily_end.ClientID %>").val($("#<%=txt_daily_start.ClientID %>").val());
            }
            if (obj.id == '<%=txt_weekly_start.ClientID%>')
                $("#<%=txt_weekly_end.ClientID %>").val($("#<%=txt_weekly_start.ClientID %>").val());

            if (obj.id == '<%=txt_monthly_start.ClientID%>')
                $("#<%=txt_monthly_end.ClientID %>").val($("#<%=txt_monthly_start.ClientID %>").val());
        }

        function all_day(obj) {

            if (obj.id == '<%=chk_daily_all_day.ClientID%>') {
                if (obj.checked) {
                    $('#<%=ddl_daily_to_time.ClientID %> option:last').prop('selected', true);
                    $('#<%=ddl_daily_to_time.ClientID %>').attr("disabled", true);
                    var current = new Date();
                    if (current <= $("#<%=txt_daily_start.ClientID %>").val())
                        $('#<%=ddl_daily_from_time.ClientID %> option:first').prop('selected', true);
                }
                else {
                    $('#<%=ddl_daily_to_time.ClientID %>').attr("disabled", false);
                }
            }

            if (obj.id == '<%=chk_weekly_all_day.ClientID%>') {
                if (obj.checked) {
                    $('#<%=ddl_weekly_to_time.ClientID %> option:last').prop('selected', true);
                    $('#<%=ddl_weekly_to_time.ClientID %>').attr("disabled", true);
                    var current = new Date();
                    if (current <= $("#<%=txt_weekly_start.ClientID %>").val())
                        $('#<%=ddl_weekly_from_time.ClientID %> option:first').prop('selected', true);
                }
                else
                    $('#<%=ddl_weekly_to_time.ClientID %>').attr("disabled", false);
            }

            if (obj.id == '<%=chk_monthly_all_day.ClientID%>') {
                if (obj.checked) {
                    $('#<%=ddl_monthly_to_time.ClientID %> option:last').prop('selected', true);
                    $('#<%=ddl_monthly_to_time.ClientID %>').attr("disabled", true);
                    var current = new Date();
                    if (current <= $("#<%=txt_monthly_start.ClientID %>").val())
                        $('#<%=ddl_monthly_from_time.ClientID %> option:first').prop('selected', true);
                }
                else
                    $('#<%=ddl_monthly_to_time.ClientID %>').attr("disabled", false);
            }

            if (obj.id == '<%=chk_single_all_day.ClientID%>') {
                if (obj.checked) {
                    $('#<%=ddl_single_end_time.ClientID %> option:last').prop('selected', true);
                    $('#<%=ddl_single_end_time.ClientID %>').attr("disabled", true);
                    var current = new Date();
                    if (current <= $("#<%=txt_single_date.ClientID %>").val())
                        $('#<%=ddl_single_start_time.ClientID %> option:first').prop('selected', true);
                }
                else
                    $('#<%=ddl_single_end_time.ClientID %>').attr("disabled", false);
            }

            if (obj.id == '<%=chk_change_all_day.ClientID%>') {
                if (obj.checked) {
                    $('#<%=ddl_change_to_time.ClientID %> option:last').prop('selected', true);
                    $('#<%=ddl_change_to_time.ClientID %>').attr("disabled", true);
                    var current = new Date();
                    if (current <= $("#<%=txt_change_from_date.ClientID %>").val())
                        $('#<%=ddl_change_from_time.ClientID %> option:first').prop('selected', true);
                }
                else
                    $('#<%=ddl_change_to_time.ClientID %>').attr("disabled", false);
            }
        }
        function set_to_time(obj) {
            if (obj.id == "<%=ddl_daily_from_time.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_daily_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_daily_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }

            if (obj.id == "<%=ddl_weekly_from_time.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_weekly_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_weekly_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }

            if (obj.id == "<%=ddl_monthly_from_time.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_monthly_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_monthly_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }

            if (obj.id == "<%=ddl_single_start_time.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_single_end_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_single_end_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }

            if (obj.id == "<%=ddl_change_from_time.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_change_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_change_to_time.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }

            if (obj.id == "<%=ddl_single_end_time.ClientID%>") {
                if ($("#<%=txt_single_to_date.ClientID %>").val() == $("#<%=txt_single_date.ClientID %>").val()) {
                    if ($("#<%=ddl_single_start_time.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                        var diff = obj.selectedIndex - 0;
                        if (diff >= 4)
                            $("#<%=ddl_single_start_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                        else
                            $("#<%=ddl_single_start_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                        return;
                    }
                }
            }

            if (obj.id == "<%=ddl_daily_to_time.ClientID%>") {
                if ($("#<%=ddl_daily_from_time.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                        var diff = obj.selectedIndex - 0;
                        if (diff >= 4) {
                            $("#<%=ddl_daily_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                    }
                    else {
                        $("#<%=ddl_daily_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                    }
                    return;
                }
            }

            if (obj.id == "<%=ddl_weekly_to_time.ClientID%>") {
                if ($("#<%=ddl_weekly_from_time.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                    var diff = obj.selectedIndex - 0;
                    if (diff >= 4)
                        $("#<%=ddl_weekly_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                    else
                        $("#<%=ddl_weekly_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                    return;
                }
            }

            if (obj.id == "<%=ddl_monthly_to_time.ClientID%>") {
                if ($("#<%=ddl_monthly_from_time.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                    var diff = obj.selectedIndex - 0;
                    if (diff >= 4)
                        $("#<%=ddl_monthly_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                    else
                        $("#<%=ddl_monthly_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                    return;
                }
            }

            if (obj.id == "<%=ddl_change_to_time.ClientID%>") {
                if ($("#<%=ddl_change_from_time.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                    var diff = obj.selectedIndex - 0;
                    if (diff >= 4)
                        $("#<%=ddl_change_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                    else
                        $("#<%=ddl_change_from_time.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                    return;
                }
            }

            if (obj.id == "<%=ddl_o_to.ClientID%>") {
                if ($("#<%=ddl_o_from.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                    var diff = obj.selectedIndex - 0;
                    if (diff >= 4)
                        $("#<%=ddl_o_from.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                    else
                        $("#<%=ddl_o_from.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                    return;
                }
            }

            if (obj.id == "<%=ddl_n_to.ClientID%>") {
                if ($("#<%=ddl_n_from.ClientID%>")[0].selectedIndex >= obj.selectedIndex) {
                    var diff = obj.selectedIndex - 0;
                    if (diff >= 4)
                        $("#<%=ddl_n_from.ClientID%>")[0].selectedIndex = obj.selectedIndex - 4;
                    else
                        $("#<%=ddl_n_from.ClientID%>")[0].selectedIndex = obj.selectedIndex - diff;
                    return;
                }
            }

            if (obj.id == "<%=ddl_o_from.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_o_to.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_o_to.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }

            if (obj.id == "<%=ddl_n_from.ClientID%>") {
                var diff = 95 - obj.selectedIndex;
                if (diff >= 4)
                    $("#<%=ddl_n_to.ClientID%>")[0].selectedIndex = obj.selectedIndex + 4;
                else
                    $("#<%=ddl_n_to.ClientID%>")[0].selectedIndex = obj.selectedIndex + diff;
                return;
            }
        }

        function check_numeric(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
        function refresh_availability() {
            __doPostBack(this, "refresh");
        }

        function remove_date(guid) {
            if (confirm("Are you sure you want to remove this date?")) {
                __doPostBack(this, guid);
            }
        }

        function clear_all() {
            if (confirm("Are you sure you want to remove all the date?")) {
                __doPostBack(this, "0");
            }
        }

        function clear_selected() {
            if (confirm("Are you sure you want to remove all the selected date?")) {
                var checkboxes = document.getElementsByName("chk_date");
                var checkboxesChecked = "";
                // loop over them all
                for (var i = 0; i < checkboxes.length; i++) {
                    // And stick the checked ones onto an array...
                    if (checkboxes[i].checked) {
                        checkboxesChecked += checkboxes[i].value + "|";
                    }
                }
                $("#<%=hdn_selected.ClientID%>").val(checkboxesChecked);
                __doPostBack(this, "selected");
            }
        }

        function getCheckedBoxes(chkboxName) {
            var checkboxes = document.getElementsByName(chkboxName);
            var checkboxesChecked = [];
            // loop over them all
            for (var i = 0; i < checkboxes.length; i++) {
                // And stick the checked ones onto an array...
                if (checkboxes[i].checked) {
                    checkboxesChecked.push(checkboxes[i]);
                }
            }
            // Return the array if it is non-empty, or null
            return checkboxesChecked.length > 0 ? checkboxesChecked : null;
        }

        function show_change(id) {
            val = $("#from_" + id).html();

            val = $("#from_" + id).html();
            $("#<%=txt_change_from_date.ClientID%>").val(moment(val, "dddd, DD-MMM-YY hh:mm A").format("DD-MMM-YYYY"));

            val = moment(val, "dddd, DD-MMM-YY hh:mm A").format("hh:mm A");
            $("#<%=ddl_change_from_time.ClientID %>").val(val);

            val = $("#to_" + id).html();
            $("#<%=txt_change_to_date.ClientID%>").val(moment(val, "dddd, DD-MMM-YY hh:mm A").format("DD-MMM-YYYY"));

            val = moment(val, "dddd, DD-MMM-YY hh:mm A").format("hh:mm A");
            $("#<%=ddl_change_to_time.ClientID %>").val(val);

            $("#<%=hdn_change.ClientID%>").val(id);

            $('#change').modal('show');
        }

        function show_single() {
            $('#single').modal('show');
        }

        function change_time() {
            $('#change_time').modal('show');
        }

        function show_daily() {
            $('#daily').modal('show');
        }

        function show_weekly() {
            $('#weekly').modal('show');
        }

        function show_monthly() {
            $('#monthly').modal('show');
        }

        jQuery(document).ready(function () {
            TableManaged.init();
            set_datepicker();

            $("#available_list_table").dataTable({
                "sPaginationType": "bootstrap",
                "oLanguage": {
                    "sLengthMenu": "_MENU_ per page", "sProcessing": "<img style='width:200px;'  src='../assets/img/loading.gif'>",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aoColumnDefs": [{
                    'bSortable': true,
                    'aTargets': [0]
                }],
                "bPaginate": false
            });

            $.fn.modal.defaults.manager = $.fn.modalmanager.defaults.manager = 'body form:first';
        });

        function advance_booking(val) {
            var modal = dialog();
            modal.showUrl("advance_booking_form.aspx?uid=" + val);
        }

        function btn_week() {
            if ($('#chkSun').is(":checked") || $('#chkMon').is(":checked") || $('#chkTue').is(":checked") || $('#chkWed').is(":checked") || $('#chkThu').is(":checked") || $('#chkFri').is(":checked") || $('#chkSat').is(":checked")) {
                $('#lblchkError').text('');
                return true;
            }
            else {
                $('#lblchkError').text('Please check any days');
                return false;
            }
        }
    </script>
</asp:Content>

