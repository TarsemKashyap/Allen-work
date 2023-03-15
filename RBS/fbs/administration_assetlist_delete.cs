// Decompiled with JetBrains decompiler
// Type: administration_assetlist_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_assetlist_delete : fbs_base_page, IRequiresSessionState
{
  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      string asset_id = this.Request.QueryString["asset_id"];
      if (!(asset_id != ""))
        return;
      this.pageload_data(asset_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data(string asset_id)
  {
    try
    {
      this.Response.Clear();
      this.Response.ClearHeaders();
      this.Response.ClearContent();
      asset asset1 = new asset();
      asset asset2 = this.assets.get_asset(Convert.ToInt64(asset_id), this.current_user.account_id);
      this.assets.delete_asset(asset2);
      if (asset2.asset_id > 0L)
      {
        this.Response.Write("success");
        this.Response.Flush();
        this.Response.End();
      }
      else
      {
        this.Response.Write("fauiler");
        this.Response.Flush();
        this.Response.End();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
