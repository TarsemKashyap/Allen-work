// Decompiled with JetBrains decompiler
// Type: administration_view_booking
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_view_booking : fbs_base_page, IRequiresSessionState
{
  private long booking_id;
  private DateTime startdatetime;
  private DateTime enddatetime;
  protected Literal litInfoMsg;
  protected HtmlGenericControl alertInfo;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected PlaceHolder control_booking_view;
  protected Button btn_cancel;
  protected HiddenField hdnID;
  protected HiddenField hdnRecID;
  protected HtmlForm form_sample_2;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.u_group.group_type == 0)
      this.redirect_unauthorized();
    try
    {
      this.booking_id = Convert.ToInt64(this.Request.QueryString["booking_id"]);
    }
    catch
    {
      this.booking_id = 0L;
    }
    this.hdnID.Value = this.booking_id.ToString();
    try
    {
      if (this.booking_id <= 0L)
        return;
      asset_booking booking = this.bookings.get_booking(this.booking_id, this.current_user.account_id);
      user user = new user();
      this.users.get_user(booking.booked_for, booking.account_id);
      this.startdatetime = booking.book_from;
      this.enddatetime = booking.book_to;
      UserControl userControl = new UserControl();
      UserControl child = (UserControl) this.LoadControl("~/controls/booking_view.ascx");
      child.Attributes.Add("booking_id", this.booking_id.ToString());
      this.control_booking_view.Controls.Add((Control) child);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
