<%@ Page Language="C#" Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=16.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePointWebControls:CssRegistration Name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server" />

    <PublishingWebControls:EditModePanel runat="server">
		<!-- Styles for edit mode only-->
		<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %>"
			After="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>
    </PublishingWebControls:EditModePanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
          	<!-- Zone match UtilityService-->
            <WebPartPages:WebPartZone runat="server" AllowPersonalization="false" ID="SysAccRequestWP" FrameType="TitleBarOnly" Orientation="Vertical"></WebPartPages:WebPartZone>
       
    <style>
        .active {
            color: #c90207 !important
        }

        .heading {
            color: #08662A;
            border-bottom: 1.5px solid #779E34 !important;
            padding-bottom: 11px;
            color: #779E34;
            font-size: 20px;
            font-weight: 400;
        }

            .heading span {
                color: #08662A;
                padding-bottom: 5px;
                border-bottom: 7px solid #08662A;
            }

        img {
            float: left;
        }

        p {
            font-family: arial;
            font-size: 13px;
            text-align: justify;
        }
    </style>
</asp:Content>
