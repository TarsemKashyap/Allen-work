<%@ page title="" language="C#" masterpagefile="~/fbs.master" autoeventwireup="true" inherits="subscribe_feed, fbs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_stylesheet" runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnView" runat="server" />
    <div class="c-f">
        <div class="rfl">            
            <div class="s12">
                
                <div class="portlet box blue">
                    <div class="ptt">
                        <div class="caption">
                            RSS Feeds</div>
                    </div>
                    <div class="pbd">  
                        <p>Best Viewed in Internet Explorer 8.0 and above.</p>                    
                        <%=html_table %>
                    </div>
                </div>
                
            </div>
        </div>
        
    </div>
    <asp:HiddenField runat="server" ID="totlarecords" />
     <asp:HiddenField ID="hdnSelectedRowCount" runat="server" />
     <asp:HiddenField  runat="server" ID="hdnBookingIDs" />
    </a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_footer_scripts" runat="Server">

</asp:Content>
