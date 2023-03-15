// Decompiled with JetBrains decompiler
// Type: administration_panels_emulate
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_panels_emulate : fbs_base_page, IRequiresSessionState
{
  public string html_panels;
  public string device_name;
  public string url;

  protected new void Page_Load(object sender, EventArgs e)
  {
    long int64 = Convert.ToInt64(this.Request.QueryString["did"]);
    device_admin_api deviceAdminApi = new device_admin_api();
    DataSet device = deviceAdminApi.get_device(int64, this.current_user.account_id);
    this.url = deviceAdminApi.get_setting("emulator_url", this.current_user.account_id);
    if (device.Tables[0].Rows.Count <= 0)
      return;
    DataRow row = device.Tables[0].Rows[0];
    this.device_name = row["device_name"].ToString() + "/" + row["device_code"].ToString();
    this.url = this.url.Replace("[code]", int64.ToString());
    this.html_panels = "<iframe width='" + row["screen_width"].ToString() + "' height='" + row["screen_height"].ToString() + "' src='" + this.url + "'>";
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
