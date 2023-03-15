<%@ page language="C#" autoeventwireup="true" inherits="book, fbs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%--<meta http-equiv="X-UA-Compatible" content="IE=edge" />--%>
    <%-- <meta http-equiv="X-UA-Compatible" content="IE=IE11" />--%>
    
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
        <style>
        .no-js #loader {
            display: none;
        }

        .js #loader {
            display: block;
            position: absolute;
            left: 100px;
            top: 0;
        }

        .se-pre-con {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            opacity:0.5;
            z-index: 9999;
            background: url(assets/img/l_c.gif) center no-repeat #fff;
        }
    </style>
</head>
<body class="p-h-f">
    <div class="se-pre-con" id="loader"></div>
    <form id="form1" runat="server">
    <div class="rfl" style="margin-top: 20px;">
            <div class="p-c">
                <div class="c-f">
                    <div class="pbd" id="alt_error_alrdybook" style="display: none;" runat="server">
                        <div class="alr a-err">
                            <asp:Literal runat="server" ID="Literal1" Text="<strong>Error!</strong>  You can't make booking in this time slot."></asp:Literal>

                            <button class="close" data-dismiss="alert">
                            </button>
                        </div>
                    </div>
                    <div class="pbd">
                        <div class="rfl">
                            <div class="s12">
                                <div class="portlet box blue">
                                    <div class="ptt">
                                        <div class="caption">
                                            Booking Details -
                                            <asp:Label runat="server" ForeColor="#A4212C" ID="lbl_assetname_heading"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="pbd form">
                                        <div class="tabbable  tabbable-custom boxless nav-justified">
                                            <ul class="nav nav-tabs nav-justified">
                                                <li class="active"><a href="#tab_default" data-toggle="tab">Details</a></li>
                                                <li><a href="#tab_resource" data-toggle="tab">Resources</a></li>
                                                <li runat="server" id="li_invite_list"><a href="#tab_invitelist" data-toggle="tab">Invite List</a></li>
                                                <li runat="server" id="li_termsandconditions"><a href="#tab_terms" data-toggle="tab">Terms & Conditions</a></li>
                                            </ul>
                                            <div class="tab-content">
                                                <div class="tab-pane active" id="tab_default">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    Purpose<span class="required">*</span></label>
                                                            </td>
                                                            <td colspan="3">
                                                                <div class="controls">
                                                                    <input type="text" name="txt_purpose" maxlength="255" id="txt_purpose" runat="server" class="mw span10"
                                                                        data-required="1" />
                                                                    <span id="errorpurpose" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    From Date<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <input type="text" name="txt_from" maxlength="25" id="txt_from_date" runat="server" class="mw span10 datepicker"
                                                                        data-required="1" />
                                                                    <span id="Span1" runat="server"></span>
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <label class="cl">
                                                                    To Date<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <input type="text" name="txt_to" maxlength="25" id="txt_to_date" runat="server" class="mw span10 datepicker"
                                                                        data-required="1" />
                                                                    <span id="Span2" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    From Time<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <asp:DropDownList ID="ddl_from" runat="server" CssClass="mw span10"></asp:DropDownList>
                                                                    <span id="Span3" runat="server"></span>
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <label class="cl">
                                                                    To Time<span class="required">*</span></label>
                                                            </td>
                                                            <td>
                                                                <div class="controls">
                                                                    <asp:DropDownList ID="ddl_to" runat="server" CssClass="mw span10"></asp:DropDownList>
                                                                    <span id="Span4" runat="server"></span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <label class="cl">
                                                                    </label>
                                                            </td>
                                                            <td colspan="3">
                                                                <div class="controls">
                                                                    <asp:Button ID="btn_find_room" CssClass="btn blue" runat="server" Text="Show Rooms" OnClick="btn_find_room_Click" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="4">
                                                                <h4>Available Rooms</h4>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="4">
                                                                <table class='table table-striped table-bordered table-hover' id='available_list_table'>
                                                                    <thead>
                                                                    <tr>
                                                                        <td width="1%"></td>
                                                                        <td>Rooms</td>
                                                                        <td>Capacity</td>
                                                                    </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                    <%=html_rooms %>
                                                                    </tbody>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>                                               
                                            </div>
                                        </div>
                                    </div>
                                    <div class="fa">
                                        <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Save Booking"
                                           ValidationGroup="email_group" />
                                        <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" />
                                    </div>
                                </div>
                                <%-- </div>--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js" type="text/javascript"></script>
        <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/scripts/app.js"></script>

    <script type="text/javascript">
            $(window).load(function () {
                fade_wheel();
            });
            function fade_wheel()
            {
                $(".se-pre-con").fadeOut("slow");
            }

            jQuery(document).ready(function () {
                var maxDT = new Date();
                maxDT.setMonth(maxDT.getYear() + 100);
                var minDT = new Date(1900, 1, 1);
                $('.datepicker').datepicker({
                    format: 'dd-M-yyyy',
                    startDate: minDT,
                    endDate: maxDT
                });

                $('#btn_cancel').click(function () {

                    try {
                        parent.$.fancybox.close();
                    }
                    catch (Ex) {
                    }
                });

                $('.datepicker').datepicker()
             .on('changeDate', function (e) {
                 $(this).datepicker('hide');
             });
            });
</script>
</body>
</html>
