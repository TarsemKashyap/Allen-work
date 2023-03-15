<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="preferences, fbs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    <%--<link rel="stylesheet" type="text/css" href="../assets/plugins/select2/select2_metro.css" />--%>
    <link rel="stylesheet" href="../assets/plugins/data-tables/DT_bootstrap.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="p-c">        
        <div class="c-f">
            <div class="rfl">
                <div class="s12">
                    <h3 class="pt">My Preferences
                    </h3>
                </div>
            </div>            
            <div class="portlet box blue">
                <div class="ptt">
                    <div class="caption"><i class="icon-reorder"></i>Information</div>
                </div>
                <div class="pbd form">
                    <div class="rfl">
                        <div class="span4">
                            <div class="cg">
                                <label class="cl">Default Landing Page<span class="required">*</span> <span id="errorEmail" class="requiredField"></span></label>
                                <div class="controls">
                                    <asp:DropDownList runat="server" ID="ddl_landing">
                                        <asp:ListItem Text="Calendar" Value="Calendar"></asp:ListItem>
                                        <asp:ListItem Text="Quick Booking" Value="Quick Booking"></asp:ListItem>
                                        <asp:ListItem Text="Advanced Booking" Value="Advanced Booking"></asp:ListItem>
                                        <asp:ListItem Text="Resource Booking" Value="Resource Booking"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="span4">
                            <div class="cg">
                                <label class="cl">Calendar Filter</label>
                                <div class="controls">
                                    <asp:DropDownList runat="server" ID="ddl_calendar_filter">
                                        <asp:ListItem Text="All Bookings" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="My Bookings" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="My Favourites" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="span4">
                            <div class="cg">
                                <label class="cl">Show Past Events</label>
                                <div class="controls">
                                    <asp:CheckBox runat="server" ID="chk_past" />
                                </div>
                            </div>
                        </div>
                    </div>                    
                </div>
                <div class="fa">
                        <asp:Button ID="btn_submit" CssClass="btn green" runat="server" Text="Save" OnClick="btn_submit_Click" />
                    </div>
            </div>
            <div class="portlet box blue">
                <div class="ptt">
                    <div class="caption"><i class="icon-reorder"></i>My Favourites</div>
                </div>
                <div class="pbd form">
                    <div class="rfl">
                        <div class="span2">
                            <div class="cg">
                                <label class="cl"></label>
                                <div class="controls">
                                    <asp:DropDownList runat="server" ID="ddl_rooms">

                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="span10">
                            <div class="cg">
                                <label class="cl"></label>
                                <div class="controls">
                                    <asp:Button runat="server" ID="btn_add" Text="Add Room To Favourites" CssClass="btn green" OnClick="btn_add_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="rfl">
                        <%=html_favourites %>
                    </div>                  
                </div>
                <div class="fa">
                        <asp:Button ID="Button1" CssClass="btn green" runat="server" Text="Save" OnClick="btn_submit_Click" />
                    </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function remove_fav(val)
        {
            if(confirm("Are you sure you want to remove this from your favourite?"))
            {
                location.href = "preferences.aspx?id=" + val;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">
</asp:Content>