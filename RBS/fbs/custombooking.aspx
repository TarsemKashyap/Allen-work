<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="custombooking, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <asp:HiddenField ID="hdnBookingWindow" runat="server" />
    <div class="c-f">
        <div class="rfl">
            <div class="s12">                
                <h3 class="pt">Custom Booking
                </h3>
            </div>
        </div>
        <div class="pbd" id="alertError" style="display: none;" runat="server">
            <div class="alr a-err">
                <asp:Literal runat="server" ID="litErrorMsg" Text="<strong>Error!</strong> Your selected time slot has already passed. You can't make the booking."></asp:Literal>
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
            <div class="s6">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Step 1: Choose Dates & Times
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
                        <table cellpadding="5" cellspacing="10">
                            <tr>
                                <td colspan="2">Date</td>
                                <td colspan="2">
                                    <div class="i-a date datepicker">  <%--OnTextChanged="txt_startDate_TextChanged" AutoPostBack="true"--%>
                                        <asp:TextBox runat="server" ID="txt_startDate"  Style="width: 90px;" class="mw m-ctrl-medium date-picker" OnTextChanged="txt_startDate_TextChanged" CausesValidation="false">
                                        </asp:TextBox>
                                        <span class="ao"><i class="icon-calendar"></i></span>
                                    </div>
                                </td>
                                <td>From Time:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddl_StartTime" runat="server" CausesValidation="false" AutoPostBack="true"
                                        Width="100px" OnSelectedIndexChanged="ddl_StartTime_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td>To Time:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddl_EndTime" runat="server" Width="100px"></asp:DropDownList>
                                </td>
                                <td style="position: absolute; margin-top: 5px;">
                                    <asp:Button Text="Add Date" CssClass="btn green" runat="server" ID="btn_add_date" OnClick="btn_add_date_Click" />
                                </td>
                                <td style="position: absolute; margin-left: 103px; margin-top: 5px;">
                                    <asp:Button runat="server" Text="Bulk Remove" CssClass="btn red"
                                        ID="btn_remove_date" OnClick="btn_remove_date_Click" OnClientClick="return Delete();" />
                                </td>
                                <td>
                                    <span runat="server" id="span_duplicate" visible="false" style="color: red; position: absolute; margin-left: 225px; margin-top: -16px;"></span>
                                </td>
                            </tr>
                        </table>

                        <asp:Repeater ID="gridview_add_dates" runat="server">
                            <HeaderTemplate>
                                <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered table-hover" id='table_dates'>
                                    <thead>
                                        <tr>

                                            <th style="width: 10px;">S.NO
                                            </th>
                                            <th a-inflCheckbox" style="width: 105px;">
                                                <asp:CheckBox runat="server" ID="select_all" class='group-checkable'
                                                    AutoPostBack="true" OnCheckedChanged="select_all_CheckedChanged" />Select All
                                            </th>

                                            <th style="width: 200px;">From
                                            </th>
                                            <th style="width: 200px;">To
                                            </th>
                                            <th style="width: 50px;">Remove
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td style="width: 10px;">
                                        <%# Eval("Sno")%>
                                    </td>
                                    <td class="singleCheckbox" style="width: 105px;">
                                        <asp:CheckBox ID="singleselect" runat="server" class='checkboxes' name="singleselect" />
                                    </td>
                                    <td style="width: 200px;">
                                        <asp:Label ID="lbl_from" Enabled="false" maxlength="255" Text='<%# Eval("From") %>' runat="server"></asp:Label>
                                    </td>
                                    <td style="width: 200px;">
                                        <asp:Label ID="lbl_to" Enabled="false" maxlength="255" Text='<%# Eval("To") %>' runat="server"></asp:Label>
                                    </td>
                                    <td style="width: 50px;">
                                        <asp:ImageButton ID="btn_remove" runat="server" ImageUrl="~/assets/img/btn_remove.png"
                                            OnClick="btn_remove_click" OnClientClick="return Delete_single();" />
                                    </td>

                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody> </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>            </div>
        </div>
        <div class="rfl">
            <div class="s6">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Step 2: Filter
                        </div>
                    </div>
                    <div class="pbd fbs-lt-bg-grey">
                        <table runat="server" id="tbl_filter" visible="false" cellpadding="5" cellspacing="10">
                            <tr>
                                <td>Room Type/Equipment
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddl_category" runat="server" CssClass="s12 s2-c"
                                        data-placeholder="Choose a Building" Width="140px">
                                    </asp:DropDownList>
                                </td>
                                <td>Building
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddl_building" runat="server" CssClass="s12 s2-c"
                                        data-placeholder="Choose a Building" Width="140px">
                                    </asp:DropDownList>
                                </td>

                                <td>Level
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddl_level" runat="server" CssClass="s12 s2-c"
                                        data-placeholder="Choose a Building" Width="140px">
                                    </asp:DropDownList>
                                </td>


                                <td>Capacity
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txt_capacity" MaxLength="5" Style="width: 150px"></asp:TextBox>
                                </td>
                                <td>Show My Favourites</td>
                                <td>
                                    <asp:CheckBox runat="server" ID="chk_fav"></asp:CheckBox></td>
                                <td>
                                    <asp:Button ID="btn_cancel" Visible="false" CssClass="btn grey" runat="server" Text="Clear" OnClick="btn_cancel_Click" />
                                </td>
                                <td>
                                    <div id="div_btn_checkavailability" runat="server" visible="false">
                                        <asp:Button ID="btn_checkavailability" CssClass="btn blue" runat="server" Visible="false" Text="Check Availability" OnClick="btn_check_availability" />
                                    </div>
                                </td>
                            </tr>
                        </table>


                    </div>
                </div>
            </div>
        </div>
        <div class="rfl" runat="server" id="div_custombooking">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Available Rooms By Selected Date/Time Range
                        </div>
                    </div>
                    <div class="pbd">

                        <%=facilites_html%>

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

                        <div class="fa" runat="server" id="div_action" visible="false">
                            <asp:Literal ID="litSubmit" runat="server"></asp:Literal>

                            <asp:Button ID="btn_next" CssClass="btn green" runat="server" Text="Next"
                                OnClick="btn_next_Click" OnClientClick="return Check_single_radio();" />
                        </div>
                    </div>



                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField runat="server" ID="hdn_selecteddatecount" />
    <asp:HiddenField runat="server" ID="hdn_selected_Date_value" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script type="text/javascript" src="assets/scripts/search.js"></script>
    <script type="text/javascript" src="assets/scripts/table-managed.js"></script>
    <script type="text/javascript" src="assets/scripts/modal.js"></script>
    <script type="text/javascript">
        function showBookingSummary() {
            var frm = document.forms[0];
            var cnt = 1;
            var issueIDs = "";
            var ass_id = 0;
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "radio") {
                    if (frm.elements[i].checked) {
                        var id = frm.elements[i].id;
                        if (id.indexOf(id) != -1) {
                            cnt = cnt + 1;
                            ass_id = frm.elements[i].value;
                        }
                    }
                }
            }

            if ((cnt - 1) > 0) {
                location.href = "booking_repeat_summary.aspx?asset=" + ass_id;
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
                document.getElementById('<%=alertError.ClientID%>').style.display = 'block';
            }
            else {
                document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
            }
        }

        function eventClick(e) {
            var modal = dialog();

        }
        function cancel(e) {
            var modal = dialog();
        }

        function ShowRoomInfo(resource) {
            var modal = dialog();
            modal.showUrl("asset_info.aspx?&r=" + resource);
        }
    </script>
     <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <%--  <script type="text/javascript" src="assets/plugins/fancybox/source/jquery.fancybox.pack.js"></script>--%>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script type="text/javascript" src="assets/scripts/search.js"></script>
    <script type="text/javascript">
        function Check_single_radio() {
            var is_book = "view";
            var class_name = "";
            var allow = false;
            $('input[type="radio"]').each(function () {
                if ($(this).is(":checked")) {
                    allow = true;
                    if (class_name != "radio view")
                        class_name = $(this).attr("class");
                }
            });

            if (class_name.indexOf("book") != -1) { is_book = "book"; }

            if (allow) {

                if (is_book == "view") {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'block';
                    return false;
                }
                else {
                    document.getElementById('<%=alertError.ClientID%>').style.display = 'none';
                    document.getElementById('<%=alertIsBook.ClientID%>').style.display = 'none';
                    return true;
                }
            }
            else {
                alert("<%=Resources.fbs.required_Next_booking_setp%>");
                return false;
            }
        }

        function Delete() {
            var allow = false;
            $('input[type="checkbox"]').each(function () {
                if ($(this).is(":checked")) {
                    allow = true;
                }
            });
            if (allow) {
                if (confirm("<%=Resources.fbs.delete_confirmation_msg%>")) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                alert("<%=Resources.fbs.date_required_to_delete %>");
                return false;
            }
        }

        function Delete_single() {
            if (confirm("<%=Resources.fbs.delete_confirmation_msg%>")) {
                return true;
            }
            else {
                return false;
            }
        }

        jQuery(document).ready(function () {
            jQuery('.group-checkable').change(function () {

                var checkboxes = document.getElementsByName('singleselect');

                //var set = jQuery(this).attr("data-set");
                var checked = true; //jQuery(this).is(":checked");
                //alert("asdasd");
                jQuery(checkboxes).each(function () {
                    if (checked) {

                        $(this).attr("checked", "checked");
                    } else {
                        $(this).attr("checked", "checked");
                    }
                });

                jQuery.uniform.update(checkboxes);
            });


            $('label.tree-toggler').click(function () {
                $(this).parent().children('ul.tree').toggle(300);
            });

            $('img').css("margin-top", "-5px");
            $('#<%=txt_startDate.ClientID %>').css({ width: "90px" });
            $('#<%=txt_capacity.ClientID %>').css({ width: "100px" });
            $('.scheduler_default_timeheader_float_inner').first().html(document.getElementById('<%=txt_startDate.ClientID %>').value);

            $('.scheduler_default_scrollable').scroll(function () {
                $('.scheduler_default_timeheader_float_inner').first().html(document.getElementById('<%=txt_startDate.ClientID %>').value);
            });
            App.init();
            Search.init();
            var hdnBookingWindow = document.getElementById('<%= hdnBookingWindow.ClientID %>').value;
            set_datepicker();
            assigntables();
        });

        function assetinfo(asset_id) {
            try {
                var modal = dialog();
                modal.showUrl("asset_info.aspx?&r=" + asset_id);
            }
            catch (ex) {
            }
        }

        function customBooking() {
            var modal = dialog();
            modal.showUrl("custom_booking_from.aspx");
        }


        function assigntables() {
            try {
                var $allCheckbox = $('.allCheckbox :checkbox');
                var $checkboxes = $('.singleCheckbox :checkbox');
                $allCheckbox.change(function () {
                    if ($allCheckbox.is(':checked')) {
                        $checkboxes.attr('checked', 'checked');
                    }
                    else {
                        $checkboxes.removeAttr('checked');
                    }
                });
                $checkboxes.change(function () {
                    if ($checkboxes.not(':checked').length) {
                        $allCheckbox.removeAttr('checked');
                    }
                    else {
                        $allCheckbox.attr('checked', 'checked');
                    }
                });
            }
            catch (e) {
                alert(e);
            }

            set_table_class("table");

            $('#table_dates').dataTable({
                "sPaginationType": "bootstrap",
                "aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
                "iDisplayLength": 25,
                "bRetrive": true,
                "bDestroy": true,
                "oLanguage": {
                    "sLengthMenu": "_MENU_ per page", "sProcessing": "<img style='width:200px;'  src='../assets/img/loading.gif'>",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0, 1, 4]
                }],
                "bPaginate": true
            });

        }
    </script>
</asp:Content>

