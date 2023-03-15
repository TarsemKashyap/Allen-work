<%@ page language="C#" autoeventwireup="true" inherits="asset_info, fbs" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="assets/fbs-fixed.css" rel="stylesheet" />
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css" />
</head>
<body class="p-h-f">
    <div class="p-c">
        <div class="c-f">
            <form id="form1" runat="server">
                <div class="rfl" style="margin-top: 20px;">
                    <div class="s12">
                        <div class="portlet box blue">
                            <div class="ptt">
                                <div class="caption">
                                    <%=asset_name %>
                                </div>
                                <div class="actions">
                                    <div class="controls">Favourite&nbsp;<asp:CheckBox runat="server" ID="chk_fav" CssClass="mw" OnCheckedChanged="chk_fav_CheckedChanged" AutoPostBack="true" /></div>
                                </div>
                            </div>
                            <div class="pbd form">
                                <div class="tabbable  tabbable-custom boxless">
                                    <ul class="nav nav-tabs" id="mutab">
                                        <li class="active"><a href="#tab_default" data-toggle="tab">Room Details</a></li>
                                        <li><a href="#tab_facility" data-toggle="tab">Room Owners</a></li>
                                        <li><a href="#tab_layout" data-toggle="tab">Location</a></li>
                                        <li runat="server" id="li_faulty"><a href="#tab_faulty" data-toggle="tab">Reported Problems</a></li>
                                    </ul>
                                    <div class="tab-content">
                                        <div class="tab-pane active" id="tab_default">
                                            <div class="form-horizontal">
                                                <asp:HiddenField runat="server" ID="hdn_asset_id" />
                                                <asp:HiddenField runat="server" ID="hdn_fav_id" />
                                                <div class="rfl">
                                                    <div class="span6">
                                                        <table width="100%" cellpadding="5" cellspacing="5">
                                                            <tr>
                                                                <td  width="40%">Building \ Level</td>
                                                                <td><span class="text bold">(B) <%=asset_building %> \ (L) <%=asset_level %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Category</td>
                                                                <td><span class="text bold">
                                                                    <%=asset_category %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Type</td>
                                                                <td><span class="text bold">
                                                                    <%=asset_type %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Seating Capacity</td>
                                                                <td><span class="text bold">
                                                                    <%=asset_capacity %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Operating Hours</td>
                                                                <td><span class="text bold">
                                                                    <%=op_hours %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Book on Weekends</td>
                                                                <td><span class="text bold">
                                                                    <%=book_weekend %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Book on Holidays</td>
                                                                <td><span class="text bold">
                                                                    <%=book_holiday %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Booking Lead Time</td>
                                                                <td><span class="text bold">
                                                                    <%=booking_lead_time %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Advance Booking Window</td>
                                                                <td><span class="text bold">
                                                                    <%=advance_booking_window %></span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Cutoff time for cancellation (before meeting starts)</td>
                                                                <td><span class="text bold">
                                                                    <%=cancel_before_window %> Mins.</span></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Cutoff time for cancellation (after meeting starts)</td>
                                                                <td><span class="text bold">
                                                                    <%=cancel_after_window %> Mins.</span></td>
                                                            </tr>
                                                              <tr>
                                                                <td style="vertical-align:top;">Description</td>
                                                                <td><span class="text bold">
                                                                    <%=asset_description %></span></td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <div class="span6">
                                                        <asp:Image ID="imagfacility" Style="width: 100%; height: auto;" runat="server" ImageUrl="~/assets/img/noimage.gif" AlternateText="Layout Image" />
                                                    </div>
                                                </div>
                                                <div class="rfl">
                                                    <div class="s12 ">
                                                        <h3>Properties</h3>
                                                        <div class="cg">
                                                            <%=asset_properties %>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="tab-pane" id="tab_facility">
                                            <h3 class="form-section">Owned By:  <b><%=asset_owner_group %></b></h3>
                                            <div class="rfl" runat="server" id="owner_group" visible="false">
                                                <div class="span6 ">
                                                    <div class="cg">
                                                        <label class="cl">
                                                            Group Members</label>
                                                        <div class="controls">
                                                            <span class="text">
                                                                <%=group_memebers%></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane" id="tab_layout">
                                            <asp:Image ID="imgLayout" runat="server" ImageUrl="~/assets/img/noimage.gif" AlternateText="Layout Image" />
                                        </div>

                                        <div class="tab-pane" id="tab_faulty">
                                        </div>

                                    </div>
                                </div>
                                <div class="fa">
                                    <asp:Button ID="btn_cancel" CssClass="btn grey" runat="server" Text="Close" OnClick="btn_cancel_Click" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
    <script src="assets/scripts/jquery-1.10.1.min.js" type="text/javascript"></script>
    <script src="assets/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <%--<script src="assets/scripts/app.js"></script>--%>
    <script type="text/javascript">
        $(document).ready(function () {
            //App.init();
            activateTab('tab_default');
        });

        function activateTab(tab) {
            $('.nav-tabs a[href="#' + tab + '"]').tab('show');
        };
    </script>
</body>
</html>
