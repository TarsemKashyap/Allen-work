<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="booking_repeat_summary, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <link href="assets/css/search.css" rel="stylesheet" type="text/css" />
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="c-f">
        
        <div class="rfl">
            <div class="s12">
                
                <h3 class="pt">
                    Repeat Booking Summary
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
        <div class="rfl">
            <div class="s12">
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            Booking Summary</div>
                    </div>
                    <div class="pbd">
                        <table width="100%">
                            <tr>
                                <td>
                                    <label class="cl s12">
                                        You can customize the pattern by removing any of the meetings by selecting the check
                                        box and clicking on "Delete" button.
                                    </label>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <asp:Button ID="btnDelete" runat="server" CssClass="btn red" Text="Remove"
                                        OnClientClick="if ( ! bulkDelete()) return false;" OnClick="btnDelete_Click">
                                    </asp:Button>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                        </table>
                        <%=htmltable %>
                    </div>
                    <div class="fa">
                        <asp:Literal ID="litSubmit" runat="server"></asp:Literal>
                        <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Next" OnClientClick="if ( ! showBookingForm()) return false;" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnValues" runat="server" />
    <asp:HiddenField ID="hdnAssetID" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
    <script type="text/javascript" src="assets/scripts/modal.js"></script>
    <script type="text/javascript">
        function dialog() {
            var modal = new DayPilot.Modal();
            modal.top = 60;
            modal.width = 900;
            modal.height = 700;
            modal.opacity = 70;
            modal.border = "3px solid #d0d0d0";
            modal.closed = function () {
                if (this.result == "OK") {
                //    window.scrollTo(0, 0);

                    // window.location = "/MyBookings_month.aspx";
                    window.location = "/bookings.aspx";
                }
            };

            modal.zIndex = 100;
            return modal;
        }
        function showBookingForm() {
            var frm = document.forms[0];
            var cnt = 0;
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "checkbox") {
                    var id = frm.elements[i].id;
                    if (id.indexOf(id) != -1) {
                        cnt = cnt + 1;
                    }
                }
            }
            if (cnt > 0) {
                var modal = dialog();
                modal.height = 800;                
                modal.showUrl("booking_form.aspx?type=repeat&r=" + document.getElementById('<%=hdnAssetID.ClientID%>').value+"&Rdct=repeat");
            }
            else {
                alert("No items in booking list.");
            }
           
        }
        function bulkDelete() {
            var frm = document.forms[0];
            var cnt = 0;
            var val = "";
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "checkbox") {
                    if (frm.elements[i].checked) {
                        var id = frm.elements[i].id;
                        if (id.indexOf("chkSelect") != -1) {
                            cnt = cnt + 1;
                            val = val + frm.elements[i].value + "#";
                        }
                    }
                }
            }
            if (cnt > 0) {
                document.getElementById('<%=hdnValues.ClientID%>').value = val;
                return confirm("Are you sure you want to delete the selected records?")
            }
            else {
                alert("<%=Resources.fbs.required_delete%>");
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
        jQuery(document).ready(function () {
            Search.init();
            TableManaged.init();
            $('.datepicker').datepicker({
                format: 'dd-M-yyyy'
            });
            $('#summary_list_table').dataTable({
                "sPaginationType": "bootstrap",
                "aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
                "iDisplayLength": 25,
                "oLanguage": {
                    "sLengthMenu": "_MENU_ per page", "sProcessing": "<img style='width:200px;'  src='../assets/img/loading.gif'>",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }],

                "bPaginate": true

            });
        });
    </script>
</asp:Content>
