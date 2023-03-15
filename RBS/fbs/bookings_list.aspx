<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="bookings_list, fbs" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" Runat="Server">
     <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datetimepicker/css/datetimepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <script type="text/javascript" src="assets/scripts/modal.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <script type="text/javascript">
        function closemodal() {
            set();
            try {
                call_booking_list();
            }
            catch (err) { }
        }

        function vb(booking_id) {
            show_modal("booking_view.aspx?id=" + booking_id);
        }
        function eb(booking_id) {
            show_modal("booking_form.aspx?id=" + booking_id);
        }
        function cb(booking_id) {
            show_modal("booking_cancel.aspx?id=" + booking_id);
        }
        function rb(booking_id) {
            show_modal("booking_reassign.aspx?id=" + booking_id);
        }

        function endb(booking_id) {
            var to_date = $("div#action_" + booking_id).parent().parent().find("td").eq(4).html()
            $.get("ajax_booking.aspx?action=actualize&booking_id=" + booking_id + "&book_to=" + to_date, function (data) {
                if (data.d != null) {
                    $("div#action_" + booking_id).parent().parent().find("td").eq(4).html(data.d);
                    $.gritter.add({
                        position: 'bottom-left',
                        title: "<%=Resources.fbs.lightbox_booking_actualize%>",
                        text: "<%=Resources.fbs.actualize_sucessfully%>" + data.d,
                        sticky: false,
                        time: 3000
                    });
                }
                else {
                    $.gritter.add({
                        position: 'bottom-left',
                        title: "<%=Resources.fbs.lightbox_booking_actualize%>",
                        text: "<%=Resources.fbs.actualize_Unsucessfully%>",
                        sticky: false,
                        time: 3000
                    });
                }
            });

        }
        function ns(booking_id) {
            $.get("ajax_booking.aspx?action=update_noshow&booking_id=" + booking_id, function (data) {
                if (data == booking_id) {

                    try {
                        call_booking_list();
                    }
                    catch (err) { }
                    $.gritter.add({
                        position: 'bottom-left',
                        title: "<%=Resources.fbs.lightbox_booking_updatenoshow%>",
                        text: "<%=Resources.fbs.updated_sucessfully%>",
                        sticky: false,
                        time: 3000
                    });
                }
                else {
                    $.gritter.add({
                        position: 'bottom-left',
                        title: "<%=Resources.fbs.lightbox_booking_updatenoshow%>",
                        text: "<%=Resources.fbs.updated_noshow_fauiler%>",
                        sticky: false,
                        time: 3000
                    });

                }
            });

        }
        function call_resend_email(booking_id) {
            $.get("booking_form.aspx?Email_resend=Y&booking_id=" + booking_id, function (data) {
                $.gritter.add({
                    position: 'bottom-left',
                    title: 'Resend Email',
                    text: 'Email sucessfully sent.',
                    sticky: false,
                    time: 3000
                });
            });
        }
    </script>
    <div class="c-f">
        <div class="rfl">
            <div class="span2">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">Filter</div>
                    </div>
                    <div class="pbd">
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">Room Type/Equipment</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlCategory" runat="server" Width="163px" CssClass="s12 s2-c" data-placeholder="Choose a Building" TabIndex="1">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="span2 ">
                                <div class="cg">
                                    <label class="cl">From</label>
                                    <div class="controls">
                                        <div class="i-a date datepicker">
                                            <asp:TextBox runat="server" size="16" ID="txtFromDate"  class="mw" style="width:120px;"></asp:TextBox>
                                            <span class="ao"><i class="icon-calendar"></i></span>
                                        </div>
                                        <div style="width: 150px;">
                                            <span class="error" style="display: none;" id="fromdateerror"><%=Resources.fbs.startdate_validate%></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="span2 ">
                                <div class="cg">
                                    <label class="cl">To</label>
                                    <div class="controls">
                                        <div class="i-a date datepicker">
                                            <asp:TextBox runat="server" size="16" ID="txtToDate"  class="mw" style="width:120px;"></asp:TextBox>
                                            <span class="ao"><i class="icon-calendar"></i></span>
                                        </div>
                                        <div style="width: 150px;">
                                            <span class="error" style="display: none;" id="todateerror"><%=Resources.fbs.End_Date_validate%></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">Building</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlBuilding" runat="server" CssClass="s12 s2-c" data-placeholder="Choose a Building" TabIndex="1" Width="163px">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">Levels</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlLevel" runat="server" CssClass="s12 s2-c" data-placeholder="Choose a Building" Width="163px" TabIndex="1">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">Type</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlType" runat="server" Width="163px" CssClass="s12 s2-c" data-placeholder="Choose a Building" TabIndex="1">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
<%--                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">Status</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlStatus" Width="163px" runat="server" CssClass="s12 s2-c" data-placeholder="Choose a Building" TabIndex="1">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="rfl">
                            <div class="s12 ">
                                <div class="cg">
                                    <label class="cl">Requested By</label>
                                    <div class="controls">
                                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                                            <Services>
                                                <asp:ServiceReference Path="~/user_autocomplete.asmx" />
                                            </Services>
                                        </asp:ScriptManager>
                                        <asp:TextBox ID="txtRequestedBy" runat="server" Width="163" CssClass="span10" data-placeholder="Choose requeted by"  onkeyup = "SetContextKey()"></asp:TextBox>
                                        <asp:Panel runat="server" ID="myPanel" Style="width: 250px !important; z-index: 1">
                                        </asp:Panel>
                                        <ajaxToolkit:AutoCompleteExtender
                                            runat="server"
                                            ID="autoComplete1"
                                            ServicePath="~/user_autocomplete.asmx"
                                            ServiceMethod="get_users_other_all_user_type_view"
                                            MinimumPrefixLength="2"
                                            CompletionInterval="1000"
                                            EnableCaching="true"
                                            UseContextKey="true"
                                            TargetControlID="txtRequestedBy"
                                            CompletionSetCount="1"
                                            CompletionListElementID="myPanel" FirstRowSelected="true"
                                            CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList"
                                            CompletionListHighlightedItemCssClass="AutoExtenderHighlight"
                                            OnClientItemSelected="ClientItemSelected">
                                        </ajaxToolkit:AutoCompleteExtender>
                                        <asp:HiddenField ID="hfUserId" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>--%>
                        <div style="padding-top: 10px; padding-bottom: 20px;">
                            <asp:Button ID="btn_submit" CssClass="btn blue" runat="server" Text="Search"  OnClick="btn_submit_Click" />
                            <a href="bookings.aspx" class="btn grey">Clear</a>
                        </div>
                    </div>
                </div>
            </div>
            <div class="span10">
                
                <div class="portlet box blue">
                    <div class="ptt">
                      
                        <div id="div_tbl_reassign" runat="server">
                            <table width="100%" runat="server" id="tbl_reassign">
                                <tr>
                                    <td>
                                        <label class="control-label span12">
                                            You can re-assign the bookings by selecting the check box, select re-assign to and
                                        clicking on "re-assign" button.
                                            <b>Only future dated confirmed bookings will be reassigned.</b>
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td valign="top">
                                                    <label class="control-label">
                                                        Re-Assign To</label>
                                                </td>
                                                <td valign="top">
                                                    <asp:TextBox ID="txtreassign" runat="server" Width="200" data-placeholder="Choose reassign to" onkeyup="SetContextKey();"></asp:TextBox>
                                                     <ajaxToolkit:AutoCompleteExtender
                                                        runat="server"
                                                        ID="AutoCompleteExtender1"
                                                        ServicePath="~/user_autocomplete.asmx"
                                                        ServiceMethod="get_users_other_all_user_type_view"
                                                        MinimumPrefixLength="2"
                                                        UseContextKey="true" 
                                                        CompletionInterval="1000"
                                                        EnableCaching="true"
                                                        TargetControlID="txtreassign"
                                                        CompletionSetCount="1"
                                                        CompletionListElementID="myPanel" FirstRowSelected="true"
                                                        CompletionListCssClass="AutoExtender" CompletionListItemCssClass="AutoExtenderList"
                                                        CompletionListHighlightedItemCssClass="AutoExtenderHighlight"
                                                        OnClientItemSelected="ClientItemSelected_reassign">
                                                    </ajaxToolkit:AutoCompleteExtender>
                                                     <asp:HiddenField ID="hf_reassign_user_id" runat="server" />
                                                </td>
                                                <td valign="top">
                                                    <asp:Button ID="btnReAssign" runat="server" CssClass="btn green " Text="Re-Assign"
                                                        OnClientClick="if ( ! bulkReassign()) return false;" OnClick="btnReAssign_Click"
                                                        ></asp:Button>
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                    </td>
                                </tr>
                            </table>
                        </div>
                       
                        <div class="caption">Bookings</div>
                         
                        <div class="actions">
                                    <div class="controls">
                                        <div class="bgp p-r">
                                <button class="btn ddt" data-toggle="dropdown">
                                    Tools <i class="icon-angle-down"></i>
                                </button>
                                <ul class="ddm p-r">
                                    <li>
                                        <asp:Button ID="btnexport" Text="Export to Excel" runat="server"
                                            OnClick="btnexport_Click" CssClass="excelExportButton" OnClientClick="return setvalue();" />
                                       
                                    </li>
                                </ul>
                            </div>
                                    </div>
                            </div>
                    </div>
                    <div class="pbd">
                        <%=htmltable %>
                    </div>
                </div>
                
            </div>
        </div>
        
    </div>
    <div class='notifications top-left'></div>
    <asp:HiddenField runat="server" ID="totlarecords" />
    <asp:HiddenField ID="hdnSelectedRowCount" runat="server" />
    <asp:HiddenField runat="server" ID="hdnBookingIDs" />
    <asp:HiddenField runat="server" ID="hdnsearchvalue" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
     <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
     <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script type="text/javascript" src="assets/scripts/form-components.js"></script>
    <script type="text/javascript">
        <%--function ClientItemSelected(sender, e) {
            $get("<%=hfUserId.ClientID %>").value = e.get_value();

        }--%>
        function ClientItemSelected_reassign(sender, e) {
            $get("<%=hf_reassign_user_id.ClientID %>").value = e.get_value();

        }
        function SetContextKey() {

            $find('<%=AutoCompleteExtender1.ClientID%>').set_contextKey("<%=account_id %>");
        }
        jQuery(document).ready(function () {

            $('#<%=txtFromDate.ClientID%>').change(function () {
                var first = $('#<%=txtFromDate.ClientID%>').val();
                var second = $('#<%=txtToDate.ClientID%>').val();

                if (Date.parse(first) > Date.parse(second)) {
                    $('#fromdateerror').attr("style", "display:block;");
                    $('#fromdateerror').show();
                    $('input[Value=Search]').attr('disabled', 'disabled');
                }
                else {
                    $('#fromdateerror').hide();
                    $('input[Value=Search]').removeAttr('disabled');
                }
            });

            $('#<%=txtToDate.ClientID%>').change(function () {
                var first = $('#<%=txtFromDate.ClientID%>').val();
                var second = $('#<%=txtToDate.ClientID%>').val();

                if (Date.parse(first) > Date.parse(second)) {
                    $('#todateerror').attr("style", "display:block;");
                    $('#todateerror').show();
                    $('input[Value=Search]').attr('disabled', 'disabled');

                }
                else {
                    $('#todateerror').hide();
                    $('input[Value=Search]').removeAttr('disabled');
                }
            });
            $('#<%=btnexport.ClientID %>').click(function () {
                $('#<%=hdnsearchvalue.ClientID%>').val($('#booking_list_table_filter input').val());
                $('#<%=totlarecords.ClientID%>').val($('div.altv').first().html());

            });


            try {
                call_booking_list();
            }
            catch (err) { }
            //FormComponents.init();
            //set_datepicker();

            var minDT = new Date(1900, 1, 1);
            $('.datepicker').datepicker({
                format: 'dd-M-yyyy',
                startDate: minDT
            });

            $('.datepicker').on('changeDate', function (ev) {
                $(this).datepicker('hide');
            });

        });
        function setvalue() {
            if ($("td").hasClass("dataTables_empty")) {
                alert('<%=Resources.fbs.export_excel_no_data_msg %>');
                return false;
            }
        }


        function call_booking_list() {
            $('#booking_list_table').dataTable({
                "sPaginationType": "bootstrap",
                "bFilter": true,
                "bRetrive": true,
                "bDestroy": true,

                "aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
                "iDisplayLength": 25,
                "aaSorting": [[4, "asc"]],
                "oLanguage": {
                    "sLengthMenu": "_MENU_ per page", "sProcessing": "<img style='width:200px;'  src='assets/img/loading.gif'>",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0, 9]
                }],
                "bProcessing": true,
                "bPaginate": true
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

        function bulkReassign() {
            var reassignto = document.getElementById('<%=hf_reassign_user_id.ClientID%>').value;

            if (reassignto != "") {
                var ids = "";
                var frm = document.forms[0];
                var cnt = 0;
                var issueIDs = "";
                for (i = 0; i < frm.elements.length; i++) {
                    if (frm.elements[i].type == "checkbox") {
                        if (frm.elements[i].checked) {
                            var id = frm.elements[i].id;
                            if (id.indexOf("chkSelect") != -1) {
                                cnt = cnt + 1;
                                ids = ids + frm.elements[i].value + ",";
                            }
                        }
                    }
                }
                if (cnt > 0) {                      
                    if (confirm("Are you sure you want to reassign?")) {
                        document.getElementById('<%=hdnBookingIDs.ClientID%>').value = ids;
                        return true;
                    }
                    else {
                        return false;
                    }
                   <%-- document.getElementById('<%=hdnBookingIDs.ClientID%>').value = ids;
                    return true;--%>
                }
                else {
                    alert("No booking(s) selected.  should select the booking(s) and click re-assign.");
                }
            }
            else
                alert("Select re-assign to");
        }

    </script>
</asp:Content>

