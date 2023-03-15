// Decompiled with JetBrains decompiler
// Type: hotdesk_display_base_page
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web.UI;

public class hotdesk_display_base_page : Page
{
  public string site_full_path = ConfigurationManager.AppSettings[nameof (site_full_path)];
  public hotdesk_api hapi;
  public users_api uapi;
  public Guid account_id;
  public Guid device_code;
  public long layout_id;
  public account current_account;
  public DataSet device;
  public DataSet device_setting;
  public DataSet layout;
  public util utilities = new util();
  public static ILog log = LogManager.GetLogger("fbs_log");
  public DateTime current_timestamp;
  public DataAccess db;
  public user current_user;
  protected Dictionary<string, string> parameters;

  public hotdesk_display_base_page() => this.Load += new EventHandler(this.Page_Load);

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.Response.IsClientConnected)
      this.Response.End();
    this.Response.BufferOutput = true;
    this.query_string();
    this.hapi = new hotdesk_api();
    this.uapi = new users_api();
    this.db = new DataAccess(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
    this.device_code = new Guid(this.Request.QueryString["dcode"]);
    this.device = (DataSet) this.Session["device"];
    if (this.device == null)
    {
      this.device = this.hapi.get_device(this.device_code);
      if (this.device.Tables[0].Rows.Count > 0)
        this.Session.Add("device", (object) this.device);
    }
    if (this.device.Tables[0].Rows.Count <= 0)
      return;
    this.account_id = new Guid(this.device.Tables[0].Rows[0]["account_id"].ToString());
    this.current_account = (account) this.Session["account"];
    if (this.current_account == null)
    {
      this.current_account = this.uapi.get_account(this.account_id);
      this.Session.Add("account", (object) this.current_account);
    }
    this.device_setting = (DataSet) this.Session["device_setting"];
    if (this.device_setting == null)
    {
      this.device_setting = this.hapi.get_device_settings(this.device_code);
      this.Session.Add("device_setting", (object) this.device_setting);
    }
    this.layout = (DataSet) this.Session["layout"];
    if (this.layout == null)
    {
      this.layout = this.hapi.get_layout(this.account_id, Convert.ToInt64(this.device.Tables[0].Rows[0]["asset_id"]));
      this.Session.Add("layout", (object) this.layout);
    }
    this.layout_id = Convert.ToInt64(this.layout.Tables[0].Rows[0]["layout_id"]);
    this.current_timestamp = DateTime.UtcNow.AddHours(Convert.ToDouble(this.current_account.timezone));
  }

  private void query_string()
  {
    this.parameters = new Dictionary<string, string>();
    foreach (string str in (NameObjectCollectionBase) this.Request.QueryString)
    {
      try
      {
        if (!this.parameters.ContainsKey(str))
          this.parameters.Add(str, this.Request.QueryString[str]);
      }
      catch (Exception ex)
      {
        hotdesk_display_base_page.log.Error((object) ("key '" + str + "' does not have a value. err:" + ex.ToString()));
      }
    }
  }
}
