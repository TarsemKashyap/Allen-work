// Decompiled with JetBrains decompiler
// Type: booking_confirmation
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class booking_confirmation : fbs_base_page, IRequiresSessionState
{
  protected Label lbl_assetname_heading;
  protected Literal litInfoMsg;
  protected HtmlGenericControl alertInfo;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected HiddenField hdnBookingID;
  protected PlaceHolder control_booking_view;
  protected HtmlAnchor btn_additional_resource;
  protected Button btn_close;
  protected HtmlForm form_sample_2;
  private long booking_id;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      if (this.u_group.group_type == 0)
        this.redirect_unauthorized();
      try
      {
        this.booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        this.booking_id = 0L;
      }
      this.hdnBookingID.Value = this.booking_id.ToString();
      if (this.booking_id <= 0L)
        return;
      asset_booking booking = this.bookings.get_booking(this.booking_id, this.current_user.account_id);
      if ((int) booking.status == this.bookings.get_status("Booked"))
        this.btn_additional_resource.HRef = "~/additional_resources/request_resources.aspx?booking_id=" + booking.booking_id.ToString() + "&isrepeat=" + (object) booking.is_repeat;
      asset asset = this.assets.get_asset(booking.asset_id, this.current_user.account_id);
      this.lbl_assetname_heading.Text = asset.code.ToString() + "/" + asset.name.ToString();
      UserControl userControl = new UserControl();
      UserControl child = (UserControl) this.LoadControl("controls/booking_view.ascx");
      if (!string.IsNullOrEmpty(this.Request.QueryString["confrmtype"]))
      {
        if (this.Request.QueryString["confrmtype"] == "custom")
        {
          child.Attributes.Add("booking_id", this.Session["Custom_bookingID"].ToString());
          child.Attributes.Add("custombooking", "yes");
          this.Session.Add("custombooking", (object) "yes");
        }
        else
        {
          child.Attributes.Add("booking_id", this.booking_id.ToString());
          this.Session.Add("custombooking", (object) "no");
        }
      }
      else
      {
        child.Attributes.Add("booking_id", this.booking_id.ToString());
        this.Session.Add("custombooking", (object) "no");
      }
      child.Attributes.Add("show", "YES");
      this.control_booking_view.Controls.Add((Control) child);
      this.litInfoMsg.Text = "<strong>Info! </strong>" + Resources.fbs.booking_confirmation_msg;
      if ((int) booking.status != this.bookings.get_status("Booked"))
        return;
      this.btn_additional_resource.Visible = this.check_asset_addresource(asset);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_close_Click(object sender, EventArgs e)
  {
    try
    {
      Modal.Close((Page) this, (object) "OK");
      this.Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "close()", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_additional_resource_Click(object sender, EventArgs e) => this.Response.Redirect("~/additional_resources/additionalresources.aspx?id=" + this.hdnBookingID.Value);

  private bool check_asset_addresource(asset objAsset)
  {
    if (objAsset.asset_id != 0L)
    {
      foreach (KeyValuePair<long, asset_property> assetProperty1 in objAsset.asset_properties)
      {
        asset_property assetProperty2 = assetProperty1.Value;
        if (assetProperty2.property_name == "request_additional_resources" && assetProperty2.property_value == "1")
          return true;
      }
    }
    return false;
  }
}
