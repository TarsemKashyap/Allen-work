// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.fbs_base_page
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
  public class fbs_base_page : System.Web.UI.Page
  {
    private string current_url = HttpContext.Current.Request.Url.AbsoluteUri;
    public string site_full_path;
    public string str_resource_module = "resource_module";
    public string str_catering_module = "catering_module";
    public string unauthorized_url =  System.Configuration.ConfigurationManager.AppSettings[nameof (unauthorized_url)];
    public double AllowedMinutes = Convert.ToDouble(Resources.fbs.Apply_rules_for_minutes);
    public static ILog log = LogManager.GetLogger("fbs_log");
    public DataAccess db;
    public Guid account_id;
    public string connection_string;
    public bool enable_debug;
    public user current_user;
    public account current_account;
    public static bool ActiveDirectory;
    public holidays_api holidays;
    public booking_api bookings;
    public asset_api assets;
    public settings_api settings;
    public util utilities;
    public users_api users;
    public audit_logs_api logs;
    public blacklist_api blapi;
    public templates_api tapi;
    public workflow_api workflows;
    public email_api eapi;
    public groups_permission gp;
    public reporting_api reportings;
    public booking_bl bookingsbl;
    public help_api help;
    public resource_api resapi;
    public cache_api capi;
    public bool is_blacklisted;
    public reports_api rptapi;
    public user_group u_group;
    public timezone_api tzapi;
    public content_api contents;
    public excel_api exapi;
    public DateTime current_timestamp;
    public List<long> bookable_rooms;
    public List<long> visible_rooms;
    public List<long> approvable_rooms;
    private long mem_start;
    private long mem_end;

    public fbs_base_page() => this.Load += new EventHandler(this.Page_Load);

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
      this.workflows = (workflow_api) this.Application["workflow_api"];
      if (this.workflows == null)
      {
        this.workflows = new workflow_api();
        this.Application.Add("workflow_api", (object) this.workflows);
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
      this.help = (help_api) this.Application["help_api"];
      if (this.help == null)
      {
        this.help = new help_api();
        this.Application.Add("help_api", (object) this.help);
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
      this.capi = (cache_api) this.Application["capi"];
      if (this.capi == null)
      {
        this.capi = new cache_api(Convert.ToDouble(ConfigurationManager.AppSettings["cache_expiration"]));
        this.Application.Add("capi", (object) this.capi);
      }
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
      this.contents = (content_api) this.Application["content_api"];
      if (this.contents == null)
      {
        this.contents = new content_api();
        this.Application.Add("content_api", (object) this.contents);
      }
      this.rptapi = (reports_api) this.Application["reports_api"];
      if (this.rptapi == null)
      {
        this.rptapi = new reports_api();
        this.Application.Add("reports_api", (object) this.rptapi);
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
      if (!this.Response.IsClientConnected)
        this.Response.End();
      if (string.IsNullOrEmpty(this.site_full_path))
        this.site_full_path = ConfigurationManager.AppSettings["site_full_path"];
      if (Convert.ToBoolean(ConfigurationManager.AppSettings["enable_https"]) && !this.Request.Url.Scheme.Contains("https"))
      {
        this.site_full_path = this.site_full_path.Replace("http", "https");
        this.Response.Redirect(this.current_url.Replace("http", "https"));
      }
      this.Response.BufferOutput = true;
      fbs_base_page.ActiveDirectory = Convert.ToBoolean(ConfigurationManager.AppSettings["ActiveDirectory"]);
      this.connection_string = ConfigurationManager.ConnectionStrings["sql"].ConnectionString;
      this.enable_debug = Convert.ToBoolean(ConfigurationManager.AppSettings["enable_debug"]);
      try
      {
        this.db = (DataAccess) this.Application["db"];
      }
      catch
      {
        this.db = (DataAccess) null;
      }
      if (this.db == null)
      {
        this.db = new DataAccess(this.connection_string, this.enable_debug);
        this.Application.Add("db", (object) this.db);
      }
      try
      {
        this.current_user = (user) this.Session["user"];
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("null user data in session: " + ex.ToString()));
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      }
      if (this.current_user == null)
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      try
      {
        this.current_account = (account) this.Session["account"];
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
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
        this.visible_rooms = (List<long>) this.Session["visible_rooms"];
      }
      catch (Exception ex)
      {
        this.bookable_rooms = new List<long>();
      }
      try
      {
        this.approvable_rooms = (List<long>) this.Session["approvable_rooms"];
      }
      catch (Exception ex)
      {
        this.bookable_rooms = new List<long>();
      }
      try
      {
        this.u_group = (user_group) this.Session["user_group"];
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      }
      this.initialize_classes();
      this.tzapi.offset = this.current_account.timezone;
      this.current_timestamp = this.tzapi.current_user_timestamp();
      this.utilities.set_user(this.current_user);
      try
      {
        this.gp = (groups_permission) this.Session["gp_info"];
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      }
      if (this.current_user == null)
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      if (this.current_user.user_id == 0L)
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      this.account_id = this.current_user.account_id;
      this.bookingsbl.set_account(this.current_account, this.current_user, this.gp);
    }

    public bool is_valid(object obj) => obj != null && obj != DBNull.Value;

    public void redirect_unauthorized() => this.Response.Redirect(this.unauthorized_url);
  }
}
