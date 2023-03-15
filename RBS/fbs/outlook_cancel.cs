// Decompiled with JetBrains decompiler
// Type: outlook_cancel
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public class outlook_cancel : Page, IRequiresSessionState
{
  protected HtmlForm form1;
  private users_api uapi = new users_api();
  private settings_api settings = new settings_api();
  private asset_api assets = new asset_api();
  private booking_bl bookingsbl = new booking_bl();
  private outlook_api outlooks = new outlook_api();
  private util utilities = new util();
  private DataTable booking_table;
  private booking_api bookings = new booking_api();
  private DateTime current_timestamp;
  private user current_user;
  private account current_account;
  private user_group u_group;
  private static groups_permission gp;
  public string html_error;
  public string html_parameters;
  public string html_table;
  public string html_invitelist;
  public string sql_datetime_format = ConfigurationManager.AppSettings["sql_date_time"];
  private holidays_api holidays = new holidays_api();
  private timezone_api tzapi = new timezone_api();
  private cache_api capi = new cache_api(Convert.ToDouble(ConfigurationManager.AppSettings["cache_expiration"]));
  private Dictionary<string, string> parameters;
  private Dictionary<string, string> bookingDates;
  private List<long> bookable_rooms;
  private DataSet public_holidays;
  private DataSet setting_data;
  private DataSet fav;
  private string action = "";
  private string current_outlook_email_address = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    this.get_parameters();
    if (this.parameters.Count == 0)
      this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
    this.parameters_list();
    this.current_user = (user) this.Session["user"];
    if (this.current_user == null)
    {
      this.current_user = this.uapi.get_user_by_email(this.parameters["organizer"]);
      this.Session.Add("user", (object) this.current_user);
    }
    if (this.current_user.user_id > 0L)
      this.initialize_user_session_data();
    else
      this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
    this.current_timestamp = DateTime.UtcNow.AddHours(this.current_account.timezone);
    this.u_group = (user_group) this.Session["user_group"];
    if (this.Session["bookingsDS"] == null)
      return;
    this.booking_table = (DataTable) this.Session["bookingsDS"];
    if (this.booking_table.Rows.Count <= 0)
      return;
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      asset_booking booking = this.bookings.get_booking(Convert.ToInt64(row["booking_id"]), this.current_account.account_id);
      booking.cancel_reason = "";
      booking.status = (short) 0;
      booking.modified_by = this.current_user.user_id;
      booking.cancel_by = this.current_user.user_id;
      booking.cancel_on = this.current_timestamp;
      this.bookings.set_cancel_status(this.bookings.cancel_booking(booking));
    }
  }

  private void parameters_list()
  {
    string str = "<table>";
    foreach (string key in this.Request.Form.Keys)
    {
      str += "<tr>";
      str = str + "<td>" + key + "</td><td>" + this.Request.Form[key] + "</td>";
      str += "</tr>";
    }
    this.html_parameters = str + "</table>";
  }

  private void get_parameters()
  {
    try
    {
      this.parameters = new Dictionary<string, string>();
      foreach (string key in this.Request.QueryString.Keys)
        this.parameters.Add(key, this.Request.QueryString[key]);
    }
    catch
    {
    }
  }

  private void initialize_user_session_data()
  {
    if (this.current_user.status != 1L)
      this.Response.Redirect("outlook_error.aspx?error=inactive_user");
    this.current_account = (account) this.capi.get_cache("account");
    if (this.current_account == null)
    {
      this.current_account = this.uapi.get_account(this.current_user.account_id);
      this.capi.set_cache("account", (object) this.current_account);
    }
    this.current_timestamp = DateTime.UtcNow.AddHours(this.current_account.timezone);
    this.tzapi = new timezone_api(this.current_account.timezone);
    if (this.uapi.is_blacklisted(this.tzapi.current_timestamp(), this.current_user.account_id, this.current_user.user_id))
      this.Response.Redirect("outlook_error.aspx?error=blacklist_user");
    try
    {
      outlook_cancel.gp = (groups_permission) this.Session["gp"];
      if (outlook_cancel.gp == null)
      {
        outlook_cancel.gp = this.uapi.get_permissions(this.current_user);
        this.Session.Add("gp", (object) outlook_cancel.gp);
      }
    }
    catch
    {
    }
    try
    {
      this.u_group = (user_group) this.Session["user_group"];
      if (this.u_group == null)
      {
        this.u_group = this.utilities.get_group(this.current_user);
        this.Session.Add("user_group", (object) this.u_group);
      }
    }
    catch
    {
    }
    this.fav = (DataSet) this.Session["fav"];
    if (this.fav == null)
    {
      this.fav = this.assets.get_favourite_assets(this.current_user.account_id, this.current_user.user_id);
      this.Session.Add("fav", (object) this.fav);
    }
    this.bookable_rooms = (List<long>) this.Session["rooms"];
    if (this.bookable_rooms != null)
      return;
    this.bookable_rooms = this.assets.get_bookable_assets(this.current_user.user_id, this.current_user.account_id, outlook_cancel.gp.isAdminType);
    this.ViewState.Add("rooms", (object) this.bookable_rooms);
  }
}
