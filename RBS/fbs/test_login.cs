// Decompiled with JetBrains decompiler
// Type: test_login
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class test_login : Page, IRequiresSessionState
{
  protected TextBox txt_email;
  protected Button btn_submit;
  protected Literal lit_text;
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(ConfigurationManager.AppSettings["auto_logon"]))
      return;
    try
    {
      string name = this.User.Identity.Name;
      if (name != "")
        fbs_base_page.log.Info((object) ("User Identity: " + name));
      this.get_single_signon(name.Substring(name.IndexOf('\\') + 1));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error->", ex);
    }
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

  private void get_single_signon(string username)
  {
    try
    {
      util util = new util();
      users_api usersApi = new users_api();
      asset_api assetApi = new asset_api();
      user user = usersApi.get_user(username);
      if (user.user_id > 0L)
      {
        if (user.status > 0L)
        {
          groups_permission permissions = usersApi.get_permissions(user);
          this.Session.Add("gp_info", (object) permissions);
          this.Session.Add("user", (object) user);
          account account = usersApi.get_account(user.account_id);
          this.Session.Add("account", (object) account);
          this.Session.Add("user_group", (object) util.get_group(user));
          timezone_api timezoneApi = new timezone_api(user.timezone);
          bool flag = usersApi.is_blacklisted(timezoneApi.current_timestamp(), user.account_id, user.user_id);
          this.Session.Add("blacklisted", (object) flag);
          this.Session.Add("favourites", (object) new asset_api().get_favourite_assets(user.account_id, user.user_id));
          List<long> bookableAssets = assetApi.get_bookable_assets(user.user_id, user.account_id, permissions.isAdminType);
          List<long> visibleAssets = assetApi.get_visible_assets(user.user_id, user.account_id, permissions.isAdminType);
          List<long> approvableAssets = assetApi.get_approvable_assets(user.user_id, user.account_id, permissions.isAdminType);
          this.Session.Add("rooms", (object) bookableAssets);
          this.Session.Add("visible_rooms", (object) visibleAssets);
          this.Session.Add("approvable_rooms", (object) approvableAssets);
          usersApi.update_last_login(user.user_id, DateTime.UtcNow.AddHours(account.timezone), user.account_id);
          if (flag)
            this.Response.Redirect("error.aspx?message=access denied.");
          this.Response.Redirect(this.get_default_url(user));
        }
        else
          this.Response.Redirect("error.aspx?message=unathorised access.");
      }
      else
        this.lit_text.Text = "User email does not exist.";
    }
    catch (Exception ex)
    {
      this.lit_text.Text = ex.ToString();
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e) => this.get_single_signon(this.txt_email.Text);
}
