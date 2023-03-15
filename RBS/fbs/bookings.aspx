<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="x_bookings, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <script src="<%=site_full_path %>assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <link href="<%=site_full_path %>assets/plugins/bootstrap-modal/css/bootstrap-modal.css" rel="stylesheet" />
    <link rel="stylesheet" href="<%=site_full_path %>assets/plugins/fullcalendar/fullcalendar.min.css" />
    <link rel="stylesheet" type="text/css" href="<%=site_full_path %>assets/plugins/bootstrap-datepicker/css/datepicker.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:HiddenField ID="hdnBookingWindow" runat="server" />
    <div class="c-f">
        <div class="pbd" id="alertError" style="display: none;" runat="server">
            <div class="alr a-err">
                <asp:Literal runat="server" ID="litErrorMsg" Text=""></asp:Literal>
                <button class="close" data-dismiss="alert">
                </button>
            </div>
        </div>
        <div class="rfl">
            <div class="span2">                
                <div class="portlet box blue">
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="s12">
                                <a href="booking_quick.aspx" class="btn green span6">
										<i class="icon-tasks"></i>
										<div>Quick</div>
									</a>
                                <a href="advanced_booking.aspx" class="btn blue span6">
										<i class="icon-calendar"></i>
										<div>Advanced</div>
									</a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Filter
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="s12">
                                <div class="cg">
                                    <label class="cl">
                                        Rooms</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_room" runat="server" CssClass="s12 s2-c">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <% if (showResources)
                            { %>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">
                                        Resources</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_resources" runat="server" CssClass="s12 s2-c">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <%} %>
                        <div class="rfl">
                            <div class="s12">
                                <div class="cg">
                                    <label class="cl">
                                        Show Bookings</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddl_show" runat="server" CssClass="s12 s2-c">
                                            <asp:ListItem Text="All" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="My Bookings" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="My Favourites" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12">
                                <div class="cg">
                                    <label class="cl">
                                        Hide Past Bookings</label>
                                    <div class="controls">
                                        <asp:CheckBox runat="server" ID="chk_hide_past" Checked="true" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div style="padding-top: 10px; padding-bottom: 20px;">
                            <a href="javascript:load();" class="btn blue">Submit</a>
                            <a href="bookings.aspx" class="btn grey">Clear</a>
                        </div>
                    </div>
                </div>
                <%--<div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            <img src="bot/bot.jpg" /> Bot
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey" id="chats">
                        <div class="slimScrollDiv" style="position: relative; overflow: hidden; width: auto; height: 435px;"><div class="scroller" style="height: 435px; overflow: hidden; width: auto;" data-always-visible="1" data-rail-visible1="1">
                                <ul class="chats">
											<li class="in">
												<img src="bot/bot.jpg">
												<div class="message">
													<span class="arrow"></span>
													<span class="body">
													When do you want a room?
													</span>
												</div>
											</li>
											<li class="out">
												<img src="bot/u.png">
												<div class="message">
													<span class="arrow"></span>
													<span class="body">
													Tomorrow
													</span>
												</div>
											</li>
										</ul>
                            </div>
                            <div class="slimScrollBar ui-draggable" style="background: rgb(161, 178, 189); width: 7px; position: absolute; top: 0px; opacity: 0.4; display: block; border-radius: 7px; z-index: 99; right: 1px; height: 324.571px;"></div><div class="slimScrollRail" style="width: 7px; height: 100%; position: absolute; top: 0px; display: none; border-radius: 7px; background: rgb(51, 51, 51); opacity: 0.2; z-index: 90; right: 1px;"></div>
                        </div>
                        <div class="form-actions">
										<asp:TextBox runat="server" ID="txt_bot" CssClass="mw span10"></asp:TextBox><a ID="btn_bot" class="btn blue">></a> 
									</div>
                    </div>
                    
                </div>--%>
                <%--<div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Quick Find
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <div class="rfl">
                            <div class="cg">
                                <label class="cl">Date/Time</label>
                                <div class="controls">
                                    <div class="i-a date datepicker">
                                    <asp:TextBox runat="server" ID="txt_dt" class="mw m-ctrl-medium date-picker" Style="width: 100%;" onchange="javascript:date_changed(this);"></asp:TextBox>
                                    <span class="ao" style="margin-right: 11px;"><i class="icon-calendar"></i></span>
                                </div>
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">When</label>
                                <div class="controls">
                                    <asp:DropDownList runat="server" ID="ddl_when">
                                        <asp:ListItem Text="Anytime" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Morning (7am - 12pm)" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Afternoon (12pm - 3pm)" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="After Lunch (12pm - 6pm)" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Evening (3pm - 6pm)" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Night (6pm - 12am)" Value="4"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">Duration (Mins.)</label>
                                <div class="controls">
                                    <asp:TextBox runat="server" ID="txt_dur"></asp:TextBox>
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl">Capacity</label>
                                <div class="controls">
                                    <asp:TextBox runat="server" ID="txt_cap"></asp:TextBox>
                                </div>
                            </div>
                            <div class="cg">
                                <label class="cl"></label>
                                <div class="controls">
                                    <a href="javascript:search();" class="btn blue">Search</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>--%>
            </div>
            <div class="span10">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Bookings
                        </div>
                        <div class="actions">
                            <div class="controls">
                                <a id="btnPrint" class="btn yellow" href="#"><i class="icon-print"></i></a>
                            </div>
                        </div>
                    </div>
                    <div class="pbd" style="padding-bottom: 40px;" id="masterContent">
                        <div style="height: auto!important; overflow-y: auto;">
                            <style>
                                .fc-scroller {
                                    overflow-y: hidden !important;
                                }

                                .fc-day-grid-container.fc-scroller {
                                    height: auto !important;
                                    overflow-y: auto;
                                }
                            </style>
                            <div id='calendar' style="height: auto;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="<%=site_full_path %>assets/scripts/moment.js"></script>
    <script type="text/javascript" src="<%=site_full_path %>assets/plugins/fullcalendar/fullcalendar.min.js"></script>
    <script type="text/javascript" src="<%=site_full_path %>assets/scripts/modal.js"></script>
    <script type="text/javascript" src="<%=site_full_path %>assets/plugins/print_preview/printPreview.js"></script>
    <script type="text/javascript" src="<%=site_full_path %>assets/scripts/app.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script src="<%=site_full_path %>assets/plugins/bootstrap-modal/js/bootstrap-modal.js"></script>
    <script src="<%=site_full_path %>assets/plugins/bootstrap-modal/js/bootstrap-modalmanager.js"></script>

    <script type="text/javascript">
        function load() {
            var calendar = $('#calendar').fullCalendar('getCalendar');
            var view = calendar.view;
            var start = view.start._d;
            var end = view.end._d;
            var resLnk = '';
            if ('<%= showResources %>') {
                resLnk = '&resource=' + $('#<%=ddl_resources.ClientID%>').val();
            }
            try {
                $.ajax({
                    type: "GET",
                    url: 'handlers/get_bookings.ashx?room=' + $('#<%=ddl_room.ClientID%>').val() + resLnk + '&show=' + $('#<%=ddl_show.ClientID%>').val() + "&start=" + moment(start).format("YYYY-MM-DD") + "&end=" + moment(end).format("YYYY-MM-DD") + "&hp=" + $('#<%=chk_hide_past.ClientID%>')[0].checked + "&ts=" + Date.now(),
                    success: function (events) {
                        $('#calendar').fullCalendar('removeEvents');
                        $('#calendar').fullCalendar('addEventSource', events);
                    }
                });
            }
            catch (e) {
                alert("An error occured when retrieving data.");
            }
        }

        function _show_modal(url) {
            var modal = dialog();
            modal.closed = function () {
                load();
            }
            modal.showUrl(url);
        }

        function prepare_month_links() {
            $('.fc-day-number').each(function () {
                $(this).removeAttr('data-goto');
                $(this).attr("href", 'booking_quick.aspx?d=' + $(this).parent().attr('data-date'));
            });
        }
        function cancel(e) {
            _show_modal("booking_cancel.aspx?id=" + e.value());
        }
        function ShowRoomInfo(resource) {
            _show_modal("asset_info.aspx?&r=" + resource);
        }

        $(document).ready(function () {
            set_datepicker();

            var resLnk = '';
            if ('<%= showResources %>') {
                    resLnk = '&resource=' + $('#<%=ddl_resources.ClientID%>').val();
                }
                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,agendaWeek,agendaDay,listWeek'
                    },
                    eventRender: function (event, element, view) {
                        var $title = element.find('.fc-title');
                        $title.html($title.text());
                        var $title_list = element.find('.fc-list-item-title');
                        $title_list.html($title_list.text());
                    },
		    height:'auto',
                    defaultDate: '<%=current_date%>',
                    navLinks: true,
                    eventLimit: false,
                    views: {
                        month: { columnFormat: 'ddd' },
                        week: { columnFormat: 'ddd D/M' },
                        day: { columnFormat: 'ddd D MMM' }
                    },
                    //viewRender:load()
                    events: {
                        //url: 'handlers/get_bookings.ashx?room=' + $('#<%=ddl_room.ClientID%>').val() + resLnk + '&show=' + $('#<%=ddl_show.ClientID%>').val() + "&hp=" + $('#<%=chk_hide_past.ClientID%>')[0].checked,
                        url:'handlers/get_bookings.ashx',
                        type: 'GET',
                        data: {
                            room: function () { return $('#<%=ddl_room.ClientID%>').val(); },
                            resource: function () { return $('#<%=ddl_resources.ClientID%>').val(); },
                            show: function () { return $('#<%=ddl_show.ClientID%>').val(); },
                            hp: function () { return $('#<%=chk_hide_past.ClientID%>')[0].checked; }
                        },
                        success: function (events) {
                            $('#calendar').fullCalendar('removeEvents');
                            prepare_month_links();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                        },
                        color: 'white'   // a non-ajax option
                    }
                });

                prepare_month_links();
            });

            function ab(id) {
                _show_modal("booking_view.aspx?id=" + id);
            }

            function rb(id) {
                _show_modal("additional_resources/resource_booking_view.aspx?id=" + id);
        }

        function cb(id) {
                _show_modal("booking_cancel.aspx?id=" + id);
            }

            function eventClick(e) {
                _show_modal("resource_booking_view.aspx?id=" + e);
            }

            $(function () {
                $("#btnPrint").printPreview({
                    obj2print: '#masterContent',
                    width: '810'
                });
            });

            jQuery(document).ready(function () {
                set_datepicker();
                $.fn.modal.defaults.manager = $.fn.modalmanager.defaults.manager = 'body form:first';
            });

            function new_b(val)
            {
                _show_modal("book.aspx?dt=" + val)
        }

        <%=html_action%>
    </script>
</asp:Content>

