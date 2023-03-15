// Decompiled with JetBrains decompiler
// Type: administration_asset_audit_logs
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_asset_audit_logs : fbs_base_page, IRequiresSessionState
{
  public string room_name = "";
  protected PlaceHolder control_asset_logs;
  protected controls_asset_audit_logs ucAssetAuditLogs;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["audit_log"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (this.Request.QueryString["asset_id"] == null)
        return;
      this.room_name = this.assets.get_asset(Convert.ToInt64(string.IsNullOrEmpty(this.Request.QueryString["asset_id"]) ? "0" : this.Request.QueryString["asset_id"]), this.current_user.account_id).name;
      this.ucAssetAuditLogs.Visible = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
