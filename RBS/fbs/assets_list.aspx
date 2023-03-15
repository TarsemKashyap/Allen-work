<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="assets_list, fbs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" Runat="Server">
<script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>    


 <script type="text/javascript">
     function callfancybox(e) {
         $.ajax({
             url: "../administration/ajax_page.aspx/Checkvaluelogin",
             type: "POST",
             contentType: "application/json; charset=utf-8",
             dataType: "json",
             success: function (data) {

                 if (data.d == "y") {

                     var modal = dialog();
                     modal.showUrl("asset_info.aspx?r=" + e);
                 }
                 else {
                     window.location.href = "../logout.aspx?UN=out";
                 }
             }
         });
       
     }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="c-f">
				
				<div class="rfl">
					<div class="s12"> 
						
						<h3 class="pt">
							Rooms
						</h3>
					</div>
				</div>
				
				
				<div class="rfl">
					<div class="s12">
						
						<div class="portlet box blue">
							<div class="ptt">
								<div class="caption">Rooms</div>
							</div>
							<div class="pbd">
								<div class="table-toolbar">
									

									<div class="bgp p-r">
										<button class="btn ddt" data-toggle="dropdown">Tools <i class="icon-angle-down"></i>
										</button>
										<ul class="ddm p-r">
											<li><asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" OnClick="btnExportExcel_Click" OnClientClick="return setvalue();" CssClass="excelExportButton" /></li>
										</ul>
									</div>
								</div>
                                <table class='table table-striped table-bordered table-hover' id='assetlist_table'>
                                    <thead>
                                        <tr>
                                            <th style='width:12%;' class='hidden-480'>Code / Name</th>
                                            <th style='width:8%;' class='hidden-480'>Building</th>
                                            <th style='width:6%;' class='hidden-480'>Level</th>
                                            <th style='width:6%;' class='hidden-480'>Capacity</th>
                                            <th style='width:7%;' class='hidden-480'>Category</th>
                                            <th style='width:7%;' class='hidden-480'>Type</th>
                                            <th style='width:7%;' class='hidden-480'>Restricted</th>
                                            <th style='width:25%;' class='hidden-480'>Comment</th>
                                            <th style='width:6%;' class='hidden-480'>Status</th>
                                            <th style='width:3%;' class='hidden-480'>View</th>
                                            </tr>
                                        </thead>
                                    <tbody>
                                        <%=html_table %>
                                    </tbody>
                                    </table>
                                <%--<%=html_table %>--%>
							</div>
						</div>
						
					</div>
				</div>
				
			</div>
            <asp:HiddenField  ID="hdn_assetsearch" runat="server"/>
            <asp:HiddenField ID="hidorderby" runat="server" />
            <asp:HiddenField ID="hidorderdir" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" Runat="Server">
<script type="text/javascript" src="../assets/plugins/select2/select2.min.js"></script>
	<script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
	<script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>	
	<script type="text/javascript">
	    function setvalue() {
	        $('#<%=hdn_assetsearch.ClientID%>').val($('#assetlist_table_filter input').val());
	        get_sorted_column_name();
	        if ($("td").hasClass("dataTables_empty")) {
	            alert('<%=Resources.fbs.export_excel_no_data_msg %>');
	            return false;
	        }
        }

        function get_sorted_column_name() {
            $("table#assetlist_table > thead > tr > th").each(function () {
                if ($(this).hasClass("sorting_asc")) {
                    $('#<%=hidorderby.ClientID%>').val($(this).html());
	                $('#<%=hidorderdir.ClientID%>').val("ASC");
	            }
	            else if ($(this).hasClass("sorting_desc")) {
	                $('#<%=hidorderby.ClientID%>').val($(this).html());
	                $('#<%=hidorderdir.ClientID%>').val("DESC");
	            }
	        });
    }
    jQuery(document).ready(function () {
        $('#assetlist_table').dataTable({
            "sPaginationType": "bootstrap",
            "bFilter": true,
            "bRetrive": true,
            "bDestroy": true,
            "aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
            "iDisplayLength": 25,
            "aaSorting": [[0, "asc"]],
            "oLanguage": {
                //"sLengthMenu": "_MENU_ per page", "sProcessing": "<img style='width:200px;'  src='../assets/img/loading.gif'>",
                "oPaginate": {
                    "sPrevious": "Prev",
                    "sNext": "Next"
                }
            },
            "aoColumnDefs": [{
                'bSortable': false,
                'aTargets': [9]

            }],

            //"bProcessing": true,
            //"bServerSide": true,
            "bPaginate": true,
            //"sAjaxSource": "serverside_asset_list.aspx"
        });
        $('#emaillog_list_table_wrapper div.paging_bootstrap UL').removeClass("pagination");
        $('#emaillog_list_table_wrapper div.paging_bootstrap').addClass("pagination");
        $('#emaillog_list_table_wrapper div.row').removeClass("row").addClass("rfl");
        $('#emaillog_list_table_wrapper div.col-xs-6').removeClass("col-xs-6").addClass("span6");
    });
	</script>
</asp:Content>

