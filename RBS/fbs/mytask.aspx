<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="mytask, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <link href="../assets/plugins/gritter/css/jquery.gritter.css" rel="stylesheet" type="text/css" />
    <link href="../assets/css/style-metro.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <%--   <script src="../assets/plugins/fancybox/lib/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../assets/plugins/fancybox/source/jquery.fancybox.js" type="text/javascript"></script>
    <script src="../assets/plugins/fancybox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../assets/plugins/fancybox/source/jquery.fancybox.css" rel="stylesheet" />--%>    
    <script type="text/javascript">        
        function Mytask() {           
            parent.location.href = 'mytask.aspx?action=inbox';
        }

        function withdraw(id) {
            var x;
            if (confirm("Are you sure you want to Withdraw?") == true) {
                window.location = "mytask.aspx?workflow_id=" + id + "&wd=1";
            }
        }

        function approve(id) {
            window.location = "mytask.aspx?workflow_id=" + id + "&ap=1";
        }
        function dialog(modalheight) {
            var modal = new DayPilot.Modal();
            modal.height=modalheight;
            modal.closed = function () {
                if (this.result == "OK") {
                    if ("<%=actionname%>" == "history") {
                        window.location = "mytask.aspx?action=history";
                    }
                    else
                        window.location = "mytask.aspx?action=inbox";
                }
            };
            return modal;
        }

        function closemodal() {
            $('#mainmenu').removeClass().addClass('navbar hor-menu hidden-phone');
            window.location = "mytask.apsx?action=inbox"
        }

        function set() {
            $('#mainmenu').removeClass().addClass('navbar hor-menu hidden-phone');
        }
        function callfancybox(workflow_id) {
                        var modal = dialog(200);
                        modal.showUrl("view_mytask.aspx?workflow_id=" + workflow_id);
        }
        function callfancybox_noright(workflow_id) {
                        var modal = dialog(200);
                        modal.showUrl("view_mytask.aspx?workflow_id=" + workflow_id + "&app=no");
        }

        function callviewbookfancybox(booking_id) {
                        var modal = dialog(700);
                        modal.showUrl("booking_view.aspx?id=" + booking_id + "&Details=N");
        }

        function callfancybox_for_reject(workflow_id) {
            var actionname;
            try {

                var sPageURL = window.location.search.substring(1);
                var sURLVariables = sPageURL.split('&');
                for (var i = 0; i < sURLVariables.length; i++) {
                    var sParameterName = sURLVariables[i].split('=');
                    if (sParameterName[0] == 'action')
                        actionname = sParameterName[1];
                }
            }
            catch (data) {
                actionname = 'inbox';
            }


            if (confirm("<%=Resources.fbs.delete_rejecjtask%>")) {
                if (actionname == '' || actionname == undefined) {
                    actionname = 'inbox'
                }
                            var modal = dialog(250);
                            modal.showUrl("reject_mytask.aspx?workflow_id=" + workflow_id + "+&tab=" + actionname + "&modal=Y");
            }
        }
    </script>
    <div class="c-f">
        <div class="rfl">
            <div>
                <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>
            </div>
        </div>

        <div class="rfl">
            <div class="span2">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            My Tasks
                        </div>
                    </div>
                    <div class="pbd">
                        <div class="cg">
                            <div class="rfl">
                                <div class="span12">
                                    <div id="divInbox" onmouseover="this.style.cursor='pointer'"  runat="server" onclick="javascript:window.location='mytask.aspx?action=inbox';">
                                        <div class="nav">
                                            Inbox &nbsp; <asp:Label runat="server" ID="lblCountOfInbox"></asp:Label>
                                        </div>
                                    </div>
                                    <div id="divHistory" onmouseover="this.style.cursor='pointer'" runat="server" onclick="javascript:window.location='mytask.aspx?action=history';">
                                        <div class="nav">
                                           History
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="span10" style="width: 79%;">


                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption"><span id="task_span"><%=actionname.ToUpper() %></span></div>
                    </div>
                    <div class="pbd" id="forInbox">
                        <div class="bgp p-r">
                            <button class="btn ddt" data-toggle="dropdown">
                                Tools <i class="icon-angle-down"></i>
                            </button>
                            <ul class="ddm p-r">
                                <li>
                                    <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" OnClick="btnExportExcel_Click"  OnClientClick ="return setvalue();" CssClass="excelExportButton" /></li>
                            </ul>
                        </div>

                        <div id="div_date_filter" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        <label class="cl" id="lblfrom" runat="server">
                                            From</label>
                                    </td>
                                    <td>
                                           <div class="controls">
                                            <div class="i-a date datepicker">
                                                <asp:TextBox runat="server" size="16" ID="txtFromDate" AutoPostBack="true" OnTextChanged="txtFromDate_TextChanged" class="mw" Style="width: 120px;"></asp:TextBox>
                                                <span class="ao"><i class="icon-calendar"></i></span>
                                            </div>
                                        </div>
                                    </td>

                                    <td>
                                        <label class="cl" id="lblto" runat="server">
                                            To</label>
                                    </td>
                                    <td>
                                          <div class="controls">
                                            <div class="i-a date datepicker" >
                                                <asp:TextBox runat="server" size="16" ID="txtToDate" class="mw " Style="width: 120px;"></asp:TextBox>
                                                <span class="ao"><i class="icon-calendar"  ></i></span>
                                            </div>
                                        </div>
                                    </td>
                                    <td>
                                         <div style="margin-top: -15px;">
                                            <asp:Button runat="server" CssClass="btn blue" Text="Search" ID="btn_filter"
                                                OnClick="btn_filter_Click" OnClientClick="return set();" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="table-toolbar">
                            <div class="bgp">
                                <asp:Button ID="btn_bulk_approve" runat="server" CssClass="btn green" Text="Bulk Approve"
                                    OnClick="btn_bulk_approve_Click" OnClientClick="return isanychecked();" />
                            </div>
                        </div>
                        <%=html_table%>
                    </div>
                </div>
                <asp:HiddenField ID="hdn_id" runat="server" />

            </div>
        </div>
    </div>
    <asp:HiddenField runat="server" ID="hdnfromdate" />
    <asp:HiddenField runat="server" ID="hdntodate" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>
    <script type="text/javascript" src="assets/scripts/form-components.js"></script>
    <script type="text/javascript" src="assets/scripts/modal.js"></script>
    <script src="../assets/plugins/gritter/js/jquery.gritter.js" type="text/javascript"></script>
    <script type="text/javascript">

        function setvalue() {
            if ($("td").hasClass("dataTables_empty")) {
                alert('<%=Resources.fbs.export_excel_no_data_msg %>');
                return false;
            }
            return true;
        }

        function set() {
            $("#<%=hdnfromdate.ClientID%>").val($("#<%=txtFromDate.ClientID %>").val());
            $("#<%=hdntodate.ClientID%>").val($("#<%=txtToDate.ClientID %>").val());
        }

        jQuery(document).ready(function () {
            if ('<%=msg %>' != "") {
                $.gritter.add({                    
                    position: 'bottom-right',
                    title: '<%=Resources.fbs.MyTasks_heading %>',
                    text: '<%=msg%>',                     
                    sticky: false,
                    time: 8000
                });
            }

            $.ajax({
                url: "../administration/ajax_page.aspx/mycount",
                type: "POST",
                data: '{groupid:"<%=gp_ids %>",userid:"<%=user_id %>",ac:"<%=ac %>"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.d == "0") {
                        $('#mytask_count').hide();
                    }
                    else {
                        $('#mytask_count').html(data.d);
                        $('#mytask_count').show();
                    }
                }
            });

            $('#selectall').change(function () {

                var sellctall = document.getElementById("selectall");
                if (sellctall.checked == true) {
                    var chk = document.getElementsByName('InboxWorkflowIds');
                    for (var i = 0; i <= chk.length - 1; i++) {
                        chk[i].checked = true;
                    }
                }
                else {
                    var chk = document.getElementsByName('InboxWorkflowIds');
                    for (var i = 0; i <= chk.length - 1; i++) {
                        chk[i].checked = false;
                    }
                }
            });

            FormComponents.init();

            var minDT = new Date(1900, 1, 1);
            $('.datepicker').datepicker({
                format: 'dd-M-yyyy',
                startDate: minDT
                //,clearBtn: true
            });


            $('.datepicker').on('changeDate', function (ev) {

                $(this).datepicker('hide');
            });

            // begin second table
            $('#list_table_22').dataTable({
                "aLengthMenu": [
                         [25, 50, 100, -1],
                         [25, 50, 100, "All"] // change per page values here
                ],

                // set the initial value
                "iDisplayLength": 25,
                "sDom": "<'rfl'<'span6'l><'span6'f>r>t<'rfl'<'span6'i><'span6'p>>",
                "sPaginationType": "bootstrap",
                "oLanguage": {
                    "sLengthMenu": "_MENU_ per page",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aoColumnDefs": [{ 'bSortable': false, 'aTargets': [0] },
                     { 'bSortable': false, 'aTargets': [6] }
                ]

            });

            jQuery('#list_table_2 .group-checkable').change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            // begin second table
            $('#list_table_3').dataTable({
                "aLengthMenu": [
                         [25, 50, 100, -1],
                         [25, 50, 100, "All"] // change per page values here
                ],

                // set the initial value
                "iDisplayLength": 25,
                "sDom": "<'rfl'<'span6'l><'span6'f>r>t<'rfl'<'span6'i><'span6'p>>",
                "sPaginationType": "bootstrap",
                "oLanguage": {
                    "sLengthMenu": "_MENU_ per page", "sProcessing": "<img style='width:200px;'  src='../assets/img/loading.gif'>",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aoColumnDefs": [
                     { 'bSortable': false, 'aTargets': [5] }
                ]

            });

            jQuery('#list_table_3 .group-checkable').change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            jQuery("#forPending").css("display", "none");
        });



        function isanychecked() {
            var ischked = false;
            jQuery('.checkboxes').each(function () {
                var checked = jQuery(this).is(":checked");
                if (checked) {
                    ischked = true;
                }
            });

            if (!ischked) {
                alert('<%=Resources.fbs.required_selectoneuser%>');
            }
            return ischked;
        }
    </script>
    <asp:Literal runat="server" ID="lit_open"></asp:Literal>
</asp:Content>

