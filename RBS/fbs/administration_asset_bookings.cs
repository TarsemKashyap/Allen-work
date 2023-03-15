// Decompiled with JetBrains decompiler
// Type: administration_asset_bookings
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_asset_bookings : fbs_base_page, IRequiresSessionState
{
  protected PlaceHolder control_asset_bookings;
  protected controls_asset_bookings ucAssetBookings;
  protected Button btn_cancel;
  public string isAdmin = "";
  public string room_name = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.gp.isAdminType)
        this.isAdmin = "yes";
      if (this.Session["current_tab"] != null)
      {
        List<string> stringList = (List<string>) this.Session["current_tab"];
      }
      if (this.Request.QueryString["asset_id"] != null)
      {
        this.room_name = this.assets.get_asset(Convert.ToInt64(string.IsNullOrEmpty(this.Request.QueryString["asset_id"]) ? "0" : this.Request.QueryString["asset_id"]), this.current_user.account_id).name;
        this.ucAssetBookings.Visible = true;
      }
      this.pageload_data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    try
    {
      string str = this.Request.QueryString["asset_id"];
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
