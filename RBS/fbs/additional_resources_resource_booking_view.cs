// Decompiled with JetBrains decompiler
// Type: additional_resources_resource_booking_view
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

public class additional_resources_resource_booking_view : fbs_base_page, IRequiresSessionState
{
  protected Literal litError;
  protected HtmlGenericControl alt_err;
  protected PlaceHolder ph_control;
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    try
    {
      try
      {
        if (this.Request.QueryString["type"] == "delete")
        {
          if (this.delete_resources())
          {
            this.litError.Text = "The resource booking has been removed. Please close this pop-up and refresh the bookings calendar.";
            this.alt_err.Visible = true;
          }
        }
      }
      catch
      {
      }
      UserControl userControl = new UserControl();
      this.ph_control.Controls.Add(this.Page.LoadControl("~/controls/view_resource_booking.ascx"));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private bool delete_resources()
  {
    this.resapi.delete_resource_bookings(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id, this.current_user.user_id);
    return true;
  }
}
