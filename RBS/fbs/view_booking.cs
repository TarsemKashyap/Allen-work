// Decompiled with JetBrains decompiler
// Type: view_booking
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

public class view_booking : fbs_base_page, IRequiresSessionState
{
  protected HtmlHead Head1;
  protected Label lbl_assetname_heading;
  protected Literal litInfoMsg;
  protected HtmlGenericControl alertInfo;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected PlaceHolder control_booking_view;
  protected HiddenField hdnBookingID;
  protected HtmlForm form_sample_2;
  private UserControl control = new UserControl();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    long booking_id;
    try
    {
      booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
      booking_id = 0L;
    }
    if (booking_id == 0L)
      this.Response.Redirect("error.aspx");
    string action = this.Request.QueryString["action"];
    Dictionary<long, asset_booking_invite> inviteList = this.bookings.get_invite_list(booking_id, this.current_user.account_id);
    if (inviteList.Count > 0)
    {
      foreach (long key in inviteList.Keys)
      {
        if (inviteList[key].email.Trim().ToUpper() == this.current_user.email.ToUpper())
          this.bookings.update_invite_status(booking_id, inviteList[key].booking_invite_id, action);
      }
    }
    this.initialize(booking_id);
  }

  private void initialize(long booking_id)
  {
    try
    {
      user objUsr = (user) this.Session["user"];
      if (objUsr == null)
      {
        Modal.Close((Page) this);
        this.redirect_unauthorized();
      }
      this.u_group = this.utilities.get_group(objUsr);
      if (this.u_group.group_type == 0)
        this.redirect_unauthorized();
      if (booking_id <= 0L)
        return;
      this.hdnBookingID.Value = booking_id.ToString();
      asset asset = this.assets.get_asset(this.bookings.get_booking(booking_id, this.current_user.account_id).asset_id, this.current_user.account_id);
      this.lbl_assetname_heading.Text = asset.code.ToString() + "/" + asset.name.ToString();
      this.control = (UserControl) this.LoadControl("controls/booking_view.ascx");
      this.control.Attributes.Add(nameof (booking_id), booking_id.ToString());
      this.control.Attributes.Add("show", "NO");
      this.control_booking_view.Controls.Add((Control) this.control);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
