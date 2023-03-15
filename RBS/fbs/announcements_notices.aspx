<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="announcements__notices, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" Runat="Server">
<link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-daterangepicker/daterangepicker.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="c-f">
        <div class="portlet box blue">
            <div class="ptt">
                <div class="caption">
                    Announcements & Notices
                </div>
            </div>
            <div class="pbd">
                <div class="table-toolbar">
                    <div class="controls">
                        <div id="form-date-range-log" class="btn">
                            <i class="icon-calendar"></i>&nbsp;<span><asp:Label ID="lblDateRage" runat="server" /></span>
                            <b class="caret"></b>
                        </div>
                        <asp:Button runat="server" ID="btn_filter" Text="Filter" CssClass="btn green" OnClientClick="filter();"
                            OnClick="btn_filter_Click" />
                    </div>
                </div>
                <div class="rfl">
                    <div class="s12 news-page">
                <%if (html_builder.ToString() != "") %>
                <%{ %>
                <%=html_builder.ToString()%>
                <%} %></div></div>
            </div>
            <asp:HiddenField ID="hdn_report_start" runat="server" />
            <asp:HiddenField ID="hdn_report_end" runat="server" />
            <asp:HiddenField ID="hdn_daterange" runat="server" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" Runat="Server">
    <script type="text/javascript" src="assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/daterangepicker.js"></script> 
    <script type="text/javascript" src="assets/scripts/app.js"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          $('#form-date-range-log').daterangepicker({
              ranges: {
                  'Today': ['today', 'today'],
                  'Yesterday': ['yesterday', 'yesterday'],
                  'Last 7 Days': [Date.today().add({
                      days: -6
                  }), 'today'],
                  'Last 29 Days': [Date.today().add({
                      days: -29
                  }), 'today'],
                  'This Month': [Date.today().moveToFirstDayOfMonth(), Date.today().moveToLastDayOfMonth()],
                  'Last Month': [Date.today().moveToFirstDayOfMonth().add({
                      months: -1
                  }), Date.today().moveToFirstDayOfMonth().add({
                      days: -1
                  })]
              },
              opens: (App.isRTL() ? 'left' : 'right'),
              format: 'MM/dd/yyyy',
              separator: ' to ',
              startDate: Date.today().add({
                  days: -29
              }),
              endDate: Date.today(),
              minDate: '01/01/2012',
              maxDate: Date.today().addMonths(+9999),
              locale: {
                  applyLabel: 'Submit',
                  fromLabel: 'From',
                  toLabel: 'To',
                  customRangeLabel: 'Custom Range',
                  daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                  monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                  firstDay: 1
              },
              showWeekNumbers: true,
              buttonClasses: ['btn-danger']
          },

            function (start, end) {
                $('#form-date-range-log span').html(start.toString('MMMM d, yyyy') + ' - ' + end.toString('MMMM d, yyyy'));
                if (document.getElementById("ContentPlaceHolder1_hdn_log_start") != "") {
                    var obj = document.getElementById("<%=hdn_report_start.ClientID %>");
                    obj.value = start.toString('yyyy-MM-dd');
                }

                if (document.getElementById("ContentPlaceHolder1_hdn_log_end") != "") {
                    var obj2 = document.getElementById("<%=hdn_report_end.ClientID %>");
                    obj2.value = end.toString('yyyy-MM-dd');
                }
            });

          $('#form-date-range-log span').html(Date.today().toString('MMMM d, yyyy') + ' - ' + Date.today().toString('MMMM d, yyyy'));
          if (document.getElementById("<%=hdn_report_start.ClientID %>")) {
              $('#form-date-range-log span').html(document.getElementById("<%=hdn_daterange.ClientID %>").value);
          }

      });
  </script>
</asp:Content>

