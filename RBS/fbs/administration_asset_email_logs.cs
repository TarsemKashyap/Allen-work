// Decompiled with JetBrains decompiler
// Type: administration_asset_email_logs
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

public class administration_asset_email_logs : fbs_base_page, IRequiresSessionState
{
  protected PlaceHolder control_asset_Email;
  protected controls_asset_email ucAssetEmailLog;
  protected Button btn_cancel;
  public string isAdmin = "";
  public string room_name = "";
  public string Allow_resend_email_notify = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["Mailsend"] != null && this.Session["Mailsend"].ToString() == "resend")
      {
        this.Allow_resend_email_notify = "y";
        this.Session.Remove("Mailsend");
      }
      if (this.gp.isAdminType)
        this.isAdmin = "yes";
      if (this.Session["current_tab"] != null)
      {
        List<string> stringList = (List<string>) this.Session["current_tab"];
      }
      if (this.Request.QueryString["asset_id"] != null)
      {
        string str = string.IsNullOrEmpty(this.Request.QueryString["asset_id"]) ? "0" : this.Request.QueryString["asset_id"];
        asset asset = this.assets.get_asset(Convert.ToInt64(str), this.current_user.account_id);
        this.room_name = asset.name;
        this.ucAssetEmailLog.Attributes.Add("asset_id", str);
        this.ucAssetEmailLog.objAsset = asset;
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
