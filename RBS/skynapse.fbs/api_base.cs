// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.api_base
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using System;
using System.Configuration;
using System.Web;

namespace skynapse.fbs
{
  public class api_base
  {
    public string str_resource_module = "resource_module";
    public string str_catering_module = "catering_module";
    public string site_full_path;
    private string current_url = "";
    public ILog log = LogManager.GetLogger("fbs_log");
    public DataAccess db;
    public string connection_string = ConfigurationManager.ConnectionStrings["sql"].ConnectionString;
    public bool ActiveDirectory;
    protected cache_api capi;
    protected util utilities;
    private bool enable_debug = true;

    public api_base()
    {
      this.current_url = HttpContext.Current.Request.Url.AbsoluteUri;
      this.capi = new cache_api(Convert.ToDouble(ConfigurationManager.AppSettings["cache_expiration"]));
      this.ActiveDirectory = Convert.ToBoolean(ConfigurationManager.AppSettings[nameof (ActiveDirectory)]);
      this.db = new DataAccess(this.connection_string, this.enable_debug);
      this.utilities = new util();
      if (string.IsNullOrEmpty(this.site_full_path))
        this.site_full_path = ConfigurationManager.AppSettings[nameof (site_full_path)];
      if (!Convert.ToBoolean(ConfigurationManager.AppSettings["enable_https"]) || this.site_full_path.Contains("https"))
        return;
      this.site_full_path = this.site_full_path.Replace("http", "https");
    }

    public bool is_valid(object obj) => obj != null && obj != DBNull.Value;
  }
}
