// Decompiled with JetBrains decompiler
// Type: login
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using log4net;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class login : Page, IRequiresSessionState
{
  protected HtmlGenericControl error;
  protected HtmlGenericControl alertInfo;
  protected HtmlInputText username;
  protected HtmlInputPassword password;
  protected Button btn_Login;
  protected HtmlForm Form1;
  private string return_url = "";
  private bool auto_logon;
  public string site_full_path;
  private string current_url = HttpContext.Current.Request.Url.AbsoluteUri;
  public static ILog log = LogManager.GetLogger("fbs_log");

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (Convert.ToBoolean(ConfigurationManager.AppSettings["enable_https"]))
    {
      if (!this.current_url.Contains("https"))
      {
        this.site_full_path = this.current_url.Replace("http", "https");
        this.Response.Redirect(this.site_full_path);
      }
    }
    try
    {
      this.return_url = this.Request.QueryString["ReturnUrl"];
    }
    catch
    {
      this.return_url = "default.aspx";
    }
    this.ViewState.Add("ret_url", (object) this.return_url);
    this.auto_logon = Convert.ToBoolean(ConfigurationSettings.AppSettings["auto_logon"]);
    if (!this.auto_logon)
      return;
    string name = this.User.Identity.Name;
    this.username.Value = name;
    this.login_user(name.Substring(name.IndexOf('\\') + 1), "");
  }

  private void login_user(string username, string password)
  {
    bool flag = false;
    users_api uapi = new users_api();
    login.log.Info((object) ("getting data for user:" + username));
    user user = uapi.get_user(username);
    login.log.Info((object) ("Id for user " + username + " is:" + (object) user.user_id));
    if (user.user_id > 0L)
    {
      if (user.status != 1L)
        this.Response.Redirect("error.aspx?message=Unauthorized Access.");
      if (user.login_type == 1L || user.login_type == 0L)
      {
        login.log.Info((object) "user is AD");
        try
        {
          flag = this.get_user_data(uapi, user);
        }
        catch (Exception ex)
        {
          login.log.Error((object) "3. error:", ex);
        }
      }
      else
      {
        login.log.Info((object) "user is local");
        string md5 = uapi.get_md5(password);
        if (user.password == md5)
        {
          flag = this.get_user_data(uapi, user);
          this.check_password_pin_change();
        }
        else
        {
          this.show_error();
          return;
        }
      }
      if (!flag)
        return;
      try
      {
        account account = (account) this.Session["account"];
        uapi.update_last_login(user.user_id, DateTime.UtcNow.AddHours(account.timezone), user.account_id);
      }
      catch
      {
      }
      try
      {
        this.return_url = (string) this.ViewState["ret_url"];
        if (this.return_url == null)
          this.return_url = this.get_default_url(user);
        this.Response.Redirect(this.return_url, false);
      }
      catch
      {
        this.Response.Redirect(this.return_url, false);
      }
    }
    else
      this.show_error();
  }

  private string get_default_url(user obj)
  {
    user_property userProperty = new user_property();
    if (!obj.properties.ContainsKey("default_landing"))
      return "default.aspx";
    switch (obj.properties["default_landing"].property_value)
    {
      case "Calendar":
        return "bookings.aspx";
      case "Quick Booking":
        return "booking_quick.aspx";
      case "Advanced Booking":
        return "advanced_booking.aspx";
      case "Resource Booking":
        return "additional_resources/resource_request.aspx";
      default:
        return "default.aspx";
    }
  }

  private void check_password_pin_change()
  {
    user user = (user) this.Session["user"];
    account account = (account) this.Session["account"];
    short num = 0;
    if (account.properties.ContainsKey("reset_password_days"))
      num = Convert.ToInt16(account.properties["reset_password_days"]);
    if (num > (short) 0)
    {
      DateTime dateTime = new DateTime();
      if ((DateTime.UtcNow - (!user.properties.ContainsKey("last_password_change") ? user.created_on : Convert.ToDateTime(user.properties["last_password_change"].property_value))).Days >= (int) num)
      {
        this.return_url = (string) this.ViewState["ret_url"];
        if (this.return_url == null)
          this.return_url = this.get_default_url(user);
        this.Response.Redirect("reset_password.aspx?ret_url=" + this.return_url);
      }
    }
    if (account.properties.ContainsKey("reset_pin_days"))
      num = Convert.ToInt16(account.properties["reset_pin_days"]);
    if (num <= (short) 0)
      return;
    DateTime dateTime1 = new DateTime();
    if ((DateTime.UtcNow - (!user.properties.ContainsKey("last_pin_change") ? user.created_on : Convert.ToDateTime(user.properties["last_pin_change"].property_value))).Days < (int) num)
      return;
    this.return_url = (string) this.ViewState["ret_url"];
    if (this.return_url == null)
      this.return_url = "default.aspx";
    this.Response.Redirect("reset_pin.aspx?ret_url=" + this.return_url);
  }

  private void show_error()
  {
    this.error.InnerHtml = "Invalid username or password.";
    this.alertInfo.Visible = true;
  }

  private bool get_user_data(users_api uapi, user obj)
  {
    util util = new util();
    asset_api assetApi = new asset_api();
    login.log.Info((object) "getting permissions...");
    groups_permission permissions = uapi.get_permissions(obj);
    login.log.Info((object) "getting account...");
    account account = uapi.get_account(obj.account_id);
    user_group group = util.get_group(obj);
    timezone_api timezoneApi = new timezone_api(obj.timezone);
    login.log.Info((object) "getting blacklist...");
    bool flag = uapi.is_blacklisted(timezoneApi.current_timestamp(), obj.account_id, obj.user_id);
    this.Session.Add("blacklisted", (object) flag);
    if (flag)
      this.Response.Redirect("error.aspx?message=access denied.");
    List<long> bookableAssets = assetApi.get_bookable_assets(obj.user_id, obj.account_id, permissions.isAdminType);
    List<long> visibleAssets = assetApi.get_visible_assets(obj.user_id, obj.account_id, permissions.isAdminType);
    List<long> approvableAssets = assetApi.get_approvable_assets(obj.user_id, obj.account_id, permissions.isAdminType);
    this.Session.Add("rooms", (object) bookableAssets);
    this.Session.Add("visible_rooms", (object) visibleAssets);
    this.Session.Add("approvable_rooms", (object) approvableAssets);
    this.Session.Add("gp_info", (object) permissions);
    this.Session.Add("user", (object) obj);
    this.Session.Add("account", (object) account);
    this.Session.Add("user_group", (object) group);
    login.log.Info((object) "getting favourites...");
    this.Session.Add("favourites", (object) new asset_api().get_favourite_assets(obj.account_id, obj.user_id));
    return true;
  }

  protected void btn_Login_Click1(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(this.username.Value))
      this.show_error();
    else if (string.IsNullOrEmpty(this.password.Value))
      this.show_error();
    else
      this.login_user(this.username.Value.Trim(), this.password.Value.Trim());
  }
}
