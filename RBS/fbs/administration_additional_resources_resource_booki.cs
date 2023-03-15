// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_resource_booking_view
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

public class administration_additional_resources_resource_booking_view : 
  fbs_base_page,
  IRequiresSessionState
{
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
      UserControl userControl = new UserControl();
      this.ph_control.Controls.Add(this.Page.LoadControl("~/controls/view_resource_booking.ascx"));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
