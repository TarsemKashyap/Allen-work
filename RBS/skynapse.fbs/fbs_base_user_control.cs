// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.fbs_base_user_control
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;

namespace skynapse.fbs
{
  public class fbs_base_user_control : UserControl
  {
    private string current_url = HttpContext.Current.Request.Url.AbsoluteUri;
    public string site_full_path;
    public DataAccess db;
    public string str_resource_module = "resource_module";
    public string str_catering_module = "catering_module";
    public double AllowedMinutes = Convert.ToDouble(Resources.fbs.Apply_rules_for_minutes);
    public string logo = "";
    public ILog log = LogManager.GetLogger("fbs_log");
    public Guid account_id;
    public groups_permission gp;
    public string connection_string;
    public bool enable_debug;
    public user current_user;
    public account current_account;
    public holidays_api holidays;
    public booking_api bookings;
    public blacklist_api blapi;
    public asset_api assets;
    public settings_api settings;
    public util utilities;
    public users_api users;
    public audit_logs_api logs;
    public templates_api tapi;
    public email_api eapi;
    public reporting_api reportings;
    public booking_bl bookingsbl;
    public workflow_api work_flow;
    public resource_api resapi;
    public cache_api capi;
    public timezone_api tzapi;
    public DateTime current_timestamp;
    public excel_api exapi;
    public user_group u_group;
    public List<long> bookable_rooms;

    public fbs_base_user_control()
    {
      this.Load += new EventHandler(this.Page_Load);
      if (string.IsNullOrEmpty(this.site_full_path))
        this.site_full_path = ConfigurationManager.AppSettings[nameof (site_full_path)];
      if (!Convert.ToBoolean(ConfigurationManager.AppSettings["enable_https"]) || this.site_full_path.Contains("https"))
        return;
      this.site_full_path = this.site_full_path.Replace("http", "https");
      this.Response.Redirect(this.current_url.Replace("http", "https"));
    }

    private void initialize_classes()
    {
      GC.GetTotalMemory(false);
      this.holidays = (holidays_api) this.Application["holidays_api"];
      if (this.holidays == null)
      {
        this.holidays = new holidays_api();
        this.Application.Add("holidays_api", (object) this.holidays);
      }
      this.bookings = (booking_api) this.Application["booking_api"];
      if (this.bookings == null)
      {
        this.bookings = new booking_api();
        this.Application.Add("booking_api", (object) this.bookings);
      }
      this.assets = (asset_api) this.Application["asset_api"];
      if (this.assets == null)
      {
        this.assets = new asset_api();
        this.Application.Add("asset_api", (object) this.assets);
      }
      this.settings = (settings_api) this.Application["settings_api"];
      if (this.settings == null)
      {
        this.settings = new settings_api();
        this.Application.Add("settings_api", (object) this.settings);
      }
      this.users = (users_api) this.Application["users_api"];
      if (this.users == null)
      {
        this.users = new users_api();
        this.Application.Add("users_api", (object) this.users);
      }
      this.logs = (audit_logs_api) this.Application["audit_log_api"];
      if (this.logs == null)
      {
        this.logs = new audit_logs_api();
        this.Application.Add("audit_log_api", (object) this.logs);
      }
      this.blapi = (blacklist_api) this.Application["blacklist_api"];
      if (this.blapi == null)
      {
        this.blapi = new blacklist_api();
        this.Application.Add("blacklist_api", (object) this.blapi);
      }
      this.tapi = (templates_api) this.Application["template_api"];
      if (this.tapi == null)
      {
        this.tapi = new templates_api();
        this.Application.Add("template_api", (object) this.tapi);
      }
      this.work_flow = (workflow_api) this.Application["workflow_api"];
      if (this.work_flow == null)
      {
        this.work_flow = new workflow_api();
        this.Application.Add("workflow_api", (object) this.work_flow);
      }
      this.reportings = (reporting_api) this.Application["reporting_api"];
      if (this.reportings == null)
      {
        this.reportings = new reporting_api();
        this.Application.Add("reporting_api", (object) this.reportings);
      }
      this.eapi = (email_api) this.Application["email_api"];
      if (this.eapi == null)
      {
        this.eapi = new email_api("smpt");
        this.Application.Add("email_api", (object) this.eapi);
      }
      this.resapi = (resource_api) this.Application["resource_api"];
      if (this.resapi == null)
      {
        this.resapi = new resource_api();
        this.Application.Add("resource_api", (object) this.resapi);
      }
      this.utilities = (util) this.Application["util"];
      if (this.utilities == null)
      {
        this.utilities = new util();
        this.Application.Add("util", (object) this.utilities);
      }
      this.gp = new groups_permission();
      this.capi = new cache_api(Convert.ToDouble(ConfigurationManager.AppSettings["cache_expiration"]));
      this.bookingsbl = (booking_bl) this.Application["booking_bl"];
      if (this.bookingsbl == null)
      {
        this.bookingsbl = new booking_bl();
        this.Application.Add("booking_bl", (object) this.bookingsbl);
      }
      this.tzapi = (timezone_api) this.Application["timezone_api"];
      if (this.tzapi == null)
      {
        this.tzapi = new timezone_api();
        this.Application.Add("timezone_api", (object) this.tzapi);
      }
      this.exapi = (excel_api) this.Application["excel_api"];
      if (this.exapi == null)
      {
        this.exapi = new excel_api();
        this.Application.Add("excel_api", (object) this.exapi);
      }
      GC.GetTotalMemory(false);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      this.connection_string = ConfigurationManager.ConnectionStrings["sql"].ConnectionString;
      this.enable_debug = Convert.ToBoolean(ConfigurationManager.AppSettings["enable_debug"]);
      this.db = new DataAccess(this.connection_string, this.enable_debug);
      try
      {
        this.current_user = (user) this.Session["user"];
      }
      catch
      {
        this.current_user.user_id = 0L;
      }
      try
      {
        this.u_group = (user_group) this.Session["user_group"];
      }
      catch
      {
      }
      try
      {
        this.bookable_rooms = (List<long>) this.Session["rooms"];
      }
      catch (Exception ex)
      {
        this.bookable_rooms = new List<long>();
      }
      try
      {
        this.current_account = (account) this.Session["account"];
      }
      catch
      {
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
      }
      this.logo = !(this.current_account.logo != "") ? this.site_full_path + "assets/img/ecobook_logo.png" : this.site_full_path + "assets/img/" + this.current_account.logo;
      this.initialize_classes();
      this.tzapi.offset = this.current_user.timezone;
      this.current_timestamp = this.tzapi.current_user_timestamp();
      try
      {
        this.gp = (groups_permission) this.Session["gp_info"];
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
      }
      if (this.current_user == null)
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      if (this.current_user.user_id == 0L)
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      this.account_id = this.current_user.account_id;
      this.bookingsbl.set_account(this.current_account, this.current_user, this.gp);
    }

    public enum booking_type
    {
      Quick = 1,
      Wizard = 2,
      Repeat = 3,
      Custom = 4,
      Clone = 5,
    }
  }
}
