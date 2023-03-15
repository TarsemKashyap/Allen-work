// Decompiled with JetBrains decompiler
// Type: outlook
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using log4net;
using Resources;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public class outlook : Page, IRequiresSessionState
{
  public string html_error;
  public string html_parameters;
  public string html_table;
  public string html_invitelist;
  public string user;
  private util utilities = new util();
  private asset_api assets = new asset_api();
  private booking_api bookings = new booking_api();
  private outlook_api outlooks = new outlook_api();
  private booking_bl bookingsbl = new booking_bl();
  private settings_api settings = new settings_api();
  private users_api uapi = new users_api();
  private holidays_api holidays = new holidays_api();
  private timezone_api tzapi = new timezone_api();
  private cache_api capi = new cache_api(Convert.ToDouble(ConfigurationManager.AppSettings["cache_expiration"]));
  private bool enable_debug = Convert.ToBoolean(ConfigurationManager.AppSettings[nameof (enable_debug)]);
  private ILog log = LogManager.GetLogger("log_outlook");
  private bool has_changed;
  public account current_account;
  public skynapse.fbs.user current_user;
  public user_group u_group;
  public groups_permission gp;
  public DateTime current_timestamp;
  private Dictionary<string, string> parameters;
  private Dictionary<string, string> bookingDates;
  private List<long> bookable_rooms;
  private DataTable booking_table;
  private DataSet public_holidays;
  private DataSet setting_data;
  private DataSet fav;
  private string action = "";
  protected HiddenField hdnAction;
  protected Label lblError;
  protected HtmlGenericControl divError;
  protected DropDownList ddl_category;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected TextBox txt_capacity;
  protected CheckBox chk_fav;
  protected HiddenField HiddenStart;
  protected HiddenField HiddenEnd;
  protected HiddenField hdf_redirect;
  protected Button btn_refresh;
  protected Panel pnl_filter;
  protected TextBox txt_purpose;
  protected TextBox txtBookedFor;
  protected TextBox txt_email;
  protected TextBox txt_telephone;
  protected HtmlTextArea txt_remarks;
  protected HtmlGenericControl remarks_error;
  protected HtmlTableRow contrlgrp_invite;
  protected HtmlTableCell tr_invite;
  protected TextBox txt_cancel_reason;
  protected Button btn_confirm_cancel;
  protected Button btn_cancel_cancel;
  protected Panel pnl_cancel;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlGenericControl divMain;
  protected HtmlForm form1;

  protected void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.action = this.Request.Form["action"].ToLower();
    }
    catch
    {
      this.action = "";
    }
    if (this.action == "")
      return;
    this.hdnAction.Value = this.action;
    this.get_parameters();
    if (this.parameters.Count == 0)
      this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
    if (!this.IsPostBack)
    {
      if (!this.initialize_session())
        this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
      this.bookingDates = this.get_booking_dates();
      if (this.bookingDates.Count == 0)
        this.Response.Redirect("outlook_error.aspx?error=no_dates");
      this.get_settings_bind_dropdown(this.current_user.account_id);
      switch (this.action)
      {
        case "new":
          if (this.check_existing_global_appointment_id(this.parameters["gid"], this.current_user.account_id))
          {
            this.hdnAction.Value = "view";
            this.action = "view";
            this.process_view();
            break;
          }
          this.process_new();
          break;
        case "view":
          this.process_view();
          break;
      }
    }
    else
    {
      this.load_parameters();
      this.initialize_session();
    }
  }

  private bool initialize_session()
  {
    this.current_user = (skynapse.fbs.user) this.Session["user"];
    if (this.current_user == null)
    {
      this.current_user = this.uapi.get_user(this.parameters["user"]);
      if (this.current_user.user_id == 0L)
        this.current_user = this.uapi.get_user_by_email(this.parameters["email"]);
      if (this.current_user.user_id == 0L)
        this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
      this.Session.Add("user", (object) this.current_user);
    }
    if (this.current_user.user_id <= 0L)
      return false;
    this.user = this.current_user.full_name + " (" + this.current_user.email + ")";
    this.initialize_user_session_data();
    return true;
  }

  private void get_parameters()
  {
    try
    {
      int num = 1;
      this.parameters = new Dictionary<string, string>();
      foreach (string key in this.Request.Form.Keys)
      {
        this.parameters.Add(key, this.Request.Form[key]);
        this.ViewState.Add("p" + (object) num, (object) (key + "|" + this.Request.Form[key]));
        this.write_log(this.Session.SessionID + ":" + key + ":" + this.Request.Form[key]);
        ++num;
      }
      this.ViewState.Add("param_count", (object) num);
    }
    catch
    {
    }
  }

  private void load_parameters()
  {
    int num1 = (int) this.ViewState["param_count"];
    int num2 = 1;
    this.parameters = new Dictionary<string, string>();
    for (; num2 < num1; ++num2)
    {
      string[] strArray = ((string) this.ViewState["p" + (object) num2]).Split('|');
      this.parameters.Add(strArray[0], strArray[1]);
    }
  }

  private bool check_existing_global_appointment_id(string gid, Guid account_id) => this.outlooks.check_global_appointment_id_exists(gid, account_id, 0L);

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
      this.gp = (groups_permission) this.Session["gp"];
      if (this.gp == null)
      {
        this.gp = this.uapi.get_permissions(this.current_user);
        this.Session.Add("gp", (object) this.gp);
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
    this.bookable_rooms = this.assets.get_bookable_assets(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType);
    this.ViewState.Add("rooms", (object) this.bookable_rooms);
  }

  private void process_new()
  {
    string str = this.validateInput(this.parameters);
    if (!string.IsNullOrEmpty(str))
    {
      this.lblError.Text = str;
      this.divError.Visible = true;
      this.divMain.Visible = false;
    }
    else
    {
      DateTime dateTime = Convert.ToDateTime(this.parameters["start"]);
      Convert.ToDateTime(this.parameters["end"]);
      if (dateTime < this.current_timestamp)
      {
        this.lblError.Text = "Please choose a future date/time.";
        this.divError.Visible = true;
        this.divMain.Visible = false;
      }
      else
      {
        this.divError.Visible = false;
        this.check_availability(this.bookingDates, 0, 0);
        this.ViewState.Add("booking_table", (object) this.booking_table);
        this.populate_ui(this.booking_table);
        this.btn_cancel.Visible = false;
      }
    }
  }

  private void process_view()
  {
    try
    {
      if (Convert.ToDateTime(this.parameters["start"]) <= this.current_timestamp)
      {
        this.get_bookings_to_booking_table(this.bookingDates);
        if (this.booking_table.Rows.Count > 0)
        {
          this.Session.Add("user", (object) this.current_user);
          this.Session.Add("bookingsDS", (object) this.booking_table);
          this.Response.Redirect("outlook_view.aspx");
        }
        else
        {
          this.lblError.Text = outlookplugin.non_asset_booking;
          this.divMain.Visible = false;
          return;
        }
      }
    }
    catch
    {
      this.lblError.Text = outlookplugin.non_asset_booking;
      this.divMain.Visible = false;
      return;
    }
    try
    {
      if (this.outlooks.get_owner_by_appointment_id(this.parameters["gid"], this.current_user.account_id) != this.current_user.user_id)
      {
        this.get_bookings_to_booking_table(this.bookingDates);
        if (this.booking_table.Rows.Count > 0)
        {
          this.Session.Add("user", (object) this.current_user);
          this.Session.Add("bookingsDS", (object) this.booking_table);
          this.Response.Redirect("outlook_view.aspx");
        }
        else
        {
          this.lblError.Text = outlookplugin.non_asset_booking;
          this.divMain.Visible = false;
        }
      }
      else
      {
        this.check_availability(this.bookingDates, 0, 0);
        this.ViewState.Add("booking_table", (object) this.booking_table);
        this.populate_ui(this.booking_table);
      }
    }
    catch
    {
    }
  }

  private void populate_ui(DataTable dt)
  {
    if (dt.Rows.Count <= 0)
      return;
    DataRow row1 = dt.Rows[0];
    this.txt_email.Text = row1["email"].ToString();
    if (this.parameters["subject"] == row1["purpose"].ToString())
    {
      this.txt_purpose.Text = row1["purpose"].ToString();
    }
    else
    {
      this.txt_purpose.Text = this.parameters["subject"];
      this.has_changed = true;
    }
    this.txtBookedFor.Text = row1["booked_for_name"].ToString();
    this.txt_telephone.Text = row1["telephone"].ToString();
    this.txt_remarks.Value = row1["remarks"].ToString();
    if (string.IsNullOrEmpty(row1["invites_email"].ToString()))
    {
      this.contrlgrp_invite.Visible = false;
      this.tr_invite.Visible = false;
    }
    else
    {
      this.contrlgrp_invite.Visible = true;
      this.tr_invite.Visible = true;
      this.populate_invitelist(row1["invites_email"].ToString(), row1["invites_name"].ToString());
    }
    int num = 0;
    foreach (DataRow row2 in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      if (row2["booking_status"].ToString() == "0")
        ++num;
    }
    if (num != this.booking_table.Rows.Count)
      return;
    this.btn_submit.Visible = false;
    this.btn_cancel.Visible = false;
    this.pnl_filter.Visible = false;
  }

  private Dictionary<string, string> do_holiday_filter(
    Dictionary<string, string> selecteDates,
    DataSet holiday,
    int weekend_option)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string key in selecteDates.Keys)
    {
      bool flag = true;
      DateTime dateTime1 = Convert.ToDateTime(key);
      DateTime dateTime2 = Convert.ToDateTime(selecteDates[key]);
      while (flag)
      {
        if (this.public_holidays.Tables[0].Select("'" + (object) dateTime1 + "' >= from_date and '" + (object) dateTime2 + "' <= to_date").Length > 0)
        {
          switch (weekend_option)
          {
            case 1:
              flag = false;
              continue;
            case 2:
              dateTime1 = dateTime1.AddDays(1.0);
              dateTime2 = dateTime2.AddDays(1.0);
              continue;
            case 3:
              dateTime1 = dateTime1.AddDays(-1.0);
              dateTime2 = dateTime2.AddDays(-1.0);
              continue;
            default:
              continue;
          }
        }
        else if (dateTime1.DayOfWeek == DayOfWeek.Sunday || dateTime1.DayOfWeek == DayOfWeek.Saturday)
        {
          switch (weekend_option)
          {
            case 1:
              flag = false;
              continue;
            case 2:
              if (dateTime1.DayOfWeek == DayOfWeek.Saturday)
              {
                dateTime1 = dateTime1.AddDays(2.0);
                dateTime2 = dateTime2.AddDays(2.0);
                if (!dictionary.ContainsKey(dateTime1.ToString(this.outlooks.sql_datetime_format)))
                  dictionary.Add(dateTime1.ToString(this.outlooks.sql_datetime_format), dateTime2.ToString(this.outlooks.sql_datetime_format));
              }
              if (dateTime1.DayOfWeek == DayOfWeek.Sunday)
              {
                dateTime1 = dateTime1.AddDays(1.0);
                dateTime2 = dateTime2.AddDays(1.0);
                if (!dictionary.ContainsKey(dateTime1.ToString(this.outlooks.sql_datetime_format)))
                {
                  dictionary.Add(dateTime1.ToString(this.outlooks.sql_datetime_format), dateTime2.ToString(this.outlooks.sql_datetime_format));
                  continue;
                }
                continue;
              }
              continue;
            case 3:
              if (dateTime1.DayOfWeek == DayOfWeek.Saturday)
              {
                dateTime1 = dateTime1.AddDays(-1.0);
                dateTime2 = dateTime2.AddDays(-1.0);
                if (!dictionary.ContainsKey(dateTime1.ToString(this.outlooks.sql_datetime_format)))
                  dictionary.Add(dateTime1.ToString(this.outlooks.sql_datetime_format), dateTime2.ToString(this.outlooks.sql_datetime_format));
              }
              if (dateTime1.DayOfWeek == DayOfWeek.Sunday)
              {
                dateTime1 = dateTime1.AddDays(-2.0);
                dateTime2 = dateTime2.AddDays(-2.0);
                if (!dictionary.ContainsKey(dateTime1.ToString(this.outlooks.sql_datetime_format)))
                {
                  dictionary.Add(dateTime1.ToString(this.outlooks.sql_datetime_format), dateTime2.ToString(this.outlooks.sql_datetime_format));
                  continue;
                }
                continue;
              }
              continue;
            default:
              continue;
          }
        }
        else
          flag = false;
      }
      if (dateTime1 > this.current_timestamp && !dictionary.ContainsKey(dateTime1.ToString(this.outlooks.sql_datetime_format)))
        dictionary.Add(dateTime1.ToString(this.outlooks.sql_datetime_format), dateTime2.ToString(this.outlooks.sql_datetime_format));
    }
    return dictionary;
  }

  private void check_availability(
    Dictionary<string, string> selectedDates,
    int holiday_option,
    int weekend_option)
  {
    if (this.parameters == null)
      this.load_parameters();
    this.initialize_session();
    this.public_holidays = this.holidays.get_holidays(this.current_timestamp, this.current_timestamp.AddYears(10), this.current_user.account_id);
    selectedDates = this.do_holiday_filter(selectedDates, this.public_holidays, 1);
    this.booking_table = this.outlooks.initialize_table();
    if (this.hdnAction.Value == "new")
    {
      foreach (string key in selectedDates.Keys)
      {
        if (Convert.ToDateTime(key) > this.current_timestamp)
          this.parameter_booking_table(key, selectedDates[key]);
      }
    }
    else
      this.get_bookings_to_booking_table(selectedDates);
    if (!this.btn_submit.Visible)
      return;
    this.get_assets();
    this.populate_dates_table();
    foreach (string key in selectedDates.Keys)
    {
      if (Convert.ToDateTime(key) <= this.current_timestamp)
        this.btn_cancel.Visible = false;
    }
    this.ViewState.Add("selected_dates", (object) selectedDates);
  }

  private void parameter_booking_table(string sdt, string edt)
  {
    if (this.booking_table.Select("book_from='" + (object) Convert.ToDateTime(sdt) + "' and book_to='" + (object) Convert.ToDateTime(edt) + "'").Length > 0)
      return;
    DataRow row = this.booking_table.NewRow();
    row["booking_id"] = (object) 0;
    row["outlook_id"] = (object) 0;
    row["outlook_guid"] = (object) this.parameters["gid"];
    row["purpose"] = (object) this.parameters["subject"];
    row["booking_status"] = (object) 1;
    string parameter = this.parameters["emails"];
    string str = "";
    char ch = ';';
    string[] strArray = parameter.Split(ch);
    for (int index = 2; index < strArray.Length; ++index)
      str = str + strArray[index] + ";";
    row["booked_for"] = (object) this.current_user.user_id;
    row["booked_for_name"] = (object) this.current_user.full_name;
    try
    {
      row["telephone"] = (object) (this.current_user.properties["staff_offphone"].property_value + "/" + this.current_user.properties["staff_pager_mobile"].property_value);
    }
    catch
    {
      row["telephone"] = (object) "";
    }
    row["email"] = (object) this.current_user.email;
    row["remarks"] = (object) "";
    row["invites_email"] = !(str != "") ? (object) "" : (object) str.Substring(0, str.Length - 1);
    row["invites_name"] = (object) "";
    row["book_from"] = (object) Convert.ToDateTime(sdt);
    row["book_to"] = (object) Convert.ToDateTime(edt);
    row["asset_id"] = (object) 0;
    row["is_weekend"] = Convert.ToDateTime(sdt).DayOfWeek == DayOfWeek.Sunday || Convert.ToDateTime(sdt).DayOfWeek == DayOfWeek.Saturday ? (object) true : (object) false;
    DataRow[] dataRowArray = this.public_holidays.Tables[0].Select("'" + (object) Convert.ToDateTime(sdt) + "' >= from_date and '" + (object) Convert.ToDateTime(sdt) + "' <= to_date");
    if (dataRowArray.Length > 0)
    {
      row["is_holiday"] = (object) true;
      row["holiday_name"] = (object) dataRowArray[0]["holiday_name"].ToString();
    }
    else
    {
      row["is_holiday"] = (object) false;
      row["holiday_name"] = (object) "";
    }
    this.booking_table.Rows.Add(row);
    this.booking_table.AcceptChanges();
  }

  private void get_assets()
  {
    this.bookable_rooms = (List<long>) this.capi.get_cache(this.current_user.user_id.ToString() + "_bookable_rooms");
    if (this.bookable_rooms == null)
    {
      this.bookable_rooms = this.assets.get_bookable_assets(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType);
      this.capi.set_cache(this.current_user.user_id.ToString() + "_bookable_rooms", (object) this.bookable_rooms);
    }
    List<long> rooms = new List<long>();
    long building_id = 0;
    long level_id = 0;
    long category_id = 0;
    int capacity = 0;
    bool flag1 = true;
    bool flag2 = true;
    int num1 = 0;
    DateTime dateTime1 = new DateTime(2000, 1, 1, 0, 0, 0);
    DateTime dateTime2 = new DateTime(2000, 1, 1, 23, 59, 59);
    try
    {
      if (this.ddl_building.SelectedItem.Value != "")
        building_id = Convert.ToInt64(this.ddl_building.SelectedItem.Value);
    }
    catch
    {
    }
    try
    {
      if (this.ddl_level.SelectedItem.Value != "")
        level_id = Convert.ToInt64(this.ddl_level.SelectedItem.Value);
    }
    catch
    {
    }
    try
    {
      if (this.ddl_category.SelectedItem.Value != "")
        category_id = Convert.ToInt64(this.ddl_category.SelectedItem.Value);
    }
    catch
    {
    }
    try
    {
      if (this.txt_capacity.Text != "")
        capacity = Convert.ToInt32(this.txt_capacity.Text);
    }
    catch
    {
    }
    if (this.chk_fav.Checked)
    {
      DataRow[] dataRowArray = ((DataSet) this.Session["favourites"]).Tables[0].Select("module_name='asset'");
      if (dataRowArray.Length > 0)
      {
        foreach (DataRow dataRow in dataRowArray)
          rooms.Add(Convert.ToInt64(dataRow["resource_id"]));
      }
      else
      {
        this.html_error = "<div class='alr a-err'><strong> Error!</strong>" + outlookplugin.err_fav_1 + " </div>";
        return;
      }
    }
    else
      rooms = this.bookable_rooms;
    DataSet assets = this.assets.get_assets(rooms, this.current_user.account_id, building_id, level_id, category_id, capacity, "");
    if (this.utilities.isValidDataset(assets))
    {
      rooms.Clear();
      foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
        rooms.Add(Convert.ToInt64(row["asset_id"]));
    }
    else
      rooms.Clear();
    try
    {
      if (this.Session["system_settings"] == null)
      {
        this.setting_data = this.settings.view_settings(this.current_user.account_id);
        this.Session.Add("system_settings", (object) this.setting_data);
      }
      else
        this.setting_data = (DataSet) this.Session["system_settings"];
    }
    catch
    {
    }
    DataRow[] dataRowArray1 = this.setting_data.Tables[0].Select("parameter='advance_booking_window'");
    if (dataRowArray1.Length > 0)
      num1 = Convert.ToInt32(dataRowArray1[0]["value"]);
    DataRow[] dataRowArray2 = this.setting_data.Tables[0].Select("parameter='book_holiday'");
    if (dataRowArray2.Length > 0)
      flag1 = Convert.ToBoolean(dataRowArray2[0]["value"]);
    DataRow[] dataRowArray3 = this.setting_data.Tables[0].Select("parameter='book_weekend'");
    if (dataRowArray3.Length > 0)
      flag2 = Convert.ToBoolean(dataRowArray3[0]["value"]);
    DataRow[] dataRowArray4 = this.setting_data.Tables[0].Select("parameter='operating_hours'");
    if (dataRowArray4.Length > 0)
    {
      string[] strArray = dataRowArray4[0]["value"].ToString().Split('|');
      dateTime1 = Convert.ToDateTime(strArray[0]);
      dateTime2 = Convert.ToDateTime(strArray[1]);
    }
    DataSet dataSet = this.assets.view_asset_properties(this.current_user.account_id, new string[4]
    {
      "operating_hours",
      "book_holiday",
      "book_weekend",
      "advance_booking_window"
    });
    Dictionary<long, bool> dic_book_holiday = new Dictionary<long, bool>();
    Dictionary<long, bool> dic_book_weekend = new Dictionary<long, bool>();
    Dictionary<long, int> dictionary = new Dictionary<long, int>();
    Dictionary<long, DateTime> dic_advance_booking_date = new Dictionary<long, DateTime>();
    Dictionary<long, DateTime> dic_op_start = new Dictionary<long, DateTime>();
    Dictionary<long, DateTime> dic_op_end = new Dictionary<long, DateTime>();
    foreach (long bookableRoom in this.bookable_rooms)
    {
      DataRow[] dataRowArray5 = dataSet.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='advance_booking_window'");
      if (dataRowArray5.Length > 0)
        dictionary.Add(bookableRoom, Convert.ToInt32(dataRowArray5[0]["property_value"]));
      else
        dictionary.Add(bookableRoom, num1);
      dic_advance_booking_date.Add(bookableRoom, this.current_timestamp.AddMonths(dictionary[bookableRoom]));
      DataRow[] dataRowArray6 = dataSet.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='book_holiday'");
      if (dataRowArray6.Length > 0)
        dic_book_holiday.Add(bookableRoom, Convert.ToBoolean(dataRowArray6[0]["property_value"]));
      else
        dic_book_holiday.Add(bookableRoom, flag1);
      DataRow[] dataRowArray7 = dataSet.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='book_weekend'");
      if (dataRowArray7.Length > 0)
        dic_book_weekend.Add(bookableRoom, Convert.ToBoolean(dataRowArray7[0]["property_value"]));
      else
        dic_book_weekend.Add(bookableRoom, flag2);
      DataRow[] dataRowArray8 = dataSet.Tables[0].Select("property_name='operating_hours'");
      if (dataRowArray8.Length > 0)
      {
        string[] strArray = dataRowArray8[0]["property_value"].ToString().Split('|');
        dic_op_start.Add(bookableRoom, Convert.ToDateTime(strArray[0]));
        dic_op_end.Add(bookableRoom, Convert.ToDateTime(strArray[1]));
      }
      else
      {
        dic_op_start.Add(bookableRoom, dateTime1);
        dic_op_end.Add(bookableRoom, dateTime2);
      }
    }
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      List<long> bookable_assets = new List<long>();
      List<long> longList1 = new List<long>();
      DateTime dateTime3 = Convert.ToDateTime(row["book_from"]);
      DateTime dateTime4 = Convert.ToDateTime(row["book_to"]);
      bool boolean1 = Convert.ToBoolean(row["is_weekend"]);
      bool boolean2 = Convert.ToBoolean(row["is_holiday"]);
      foreach (long asset_id in rooms)
      {
        bool flag3 = true;
        if (this.gp.isAdminType)
        {
          flag3 = true;
        }
        else
        {
          DataRow[] dataRowArray9 = assets.Tables[0].Select("asset_id='" + (object) asset_id + "'");
          if (dataRowArray9.Length > 0 && dataRowArray9[0]["asset_owner_group_id"].ToString() != "")
          {
            foreach (user_group userGroup in this.current_user.groups.Values)
            {
              if (userGroup.group_id == Convert.ToInt64(dataRowArray9[0]["asset_owner_group_id"]))
              {
                flag3 = true;
                goto label_73;
              }
            }
          }
          flag3 = this.can_book_asset(asset_id, dateTime3, dateTime4, dic_advance_booking_date, dic_book_holiday, dic_book_weekend, boolean1, boolean2, dic_op_start, dic_op_end);
        }
label_73:
        if (flag3)
          bookable_assets.Add(asset_id);
      }
      List<long> longList2 = this.bookings.check_availability(dateTime3.AddSeconds(1.0), dateTime4.AddSeconds(-1.0), this.current_user.account_id, bookable_assets);
      try
      {
        string str1 = "";
        foreach (long num2 in rooms)
        {
          if (!longList2.Contains(num2))
            str1 = str1 + "," + num2.ToString();
        }
        string str2 = str1.Substring(1);
        row["asset_options"] = (object) str2;
        this.booking_table.AcceptChanges();
      }
      catch (Exception ex)
      {
      }
    }
  }

  private bool can_book_asset(
    long asset_id,
    DateTime sDT,
    DateTime eDT,
    Dictionary<long, DateTime> dic_advance_booking_date,
    Dictionary<long, bool> dic_book_holiday,
    Dictionary<long, bool> dic_book_weekend,
    bool is_weekend,
    bool is_holiday,
    Dictionary<long, DateTime> dic_op_start,
    Dictionary<long, DateTime> dic_op_end)
  {
    bool flag = true;
    if (sDT > dic_advance_booking_date[asset_id] && eDT > dic_advance_booking_date[asset_id])
      flag = false;
    else if (is_holiday && !dic_book_holiday[asset_id])
      flag = false;
    else if (is_weekend && !dic_book_weekend[asset_id])
      flag = false;
    else if (sDT.Hour < dic_op_start[asset_id].Hour)
      flag = false;
    else if (sDT.Hour == dic_op_start[asset_id].Hour && sDT.Minute < dic_op_start[asset_id].Minute)
      flag = false;
    else if (eDT.Hour > dic_op_end[asset_id].Hour)
      flag = false;
    else if (eDT.Hour == dic_op_end[asset_id].Hour && eDT.Minute > dic_op_end[asset_id].Minute)
      flag = false;
    return flag;
  }

  private void populate_dates_table()
  {
    DataSet dataSet = this.assets.view_assets(this.current_user.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    int num = 1;
    foreach (DataRow row1 in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      if (!row1["asset_options"].ToString().Contains(row1["asset_id"].ToString()))
      {
        DataSet bookingsAsset = this.outlooks.get_bookings_asset(Convert.ToDateTime(row1["book_from"]).ToString(this.outlooks.sql_datetime_format), Convert.ToDateTime(row1["book_to"]).ToString(this.outlooks.sql_datetime_format), Convert.ToInt64(row1["asset_id"]));
        if (this.utilities.isValidDataset(bookingsAsset))
        {
          foreach (DataRow row2 in (InternalDataCollectionBase) bookingsAsset.Tables[0].Rows)
          {
            if (row2["global_appointment_id"].ToString() == this.parameters["gid"])
              row1["asset_options"] = (object) (row1["asset_options"].ToString() + "," + row1["asset_id"]);
          }
        }
      }
      this.booking_table.AcceptChanges();
    }
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + (object) num + "</td>");
      stringBuilder.Append("<td id='from_" + (object) num + "'>" + Convert.ToDateTime(row["book_from"]).ToString("dddd, dd-MMM-yy HH:mm tt") + "</td>");
      stringBuilder.Append("<td id='to_" + (object) num + "'>" + Convert.ToDateTime(row["book_to"]).ToString("dddd, dd-MMM-yy HH:mm tt") + "</td>");
      if (row["booking_status"].ToString() == "0")
        stringBuilder.Append("<td><span class='label label-cancelled'>Cancelled</span></td>");
      else
        stringBuilder.Append("<td>" + row["holiday_name"].ToString() + "</td>");
      stringBuilder.Append("<td>");
      string[] strArray = row["asset_options"].ToString().Split(',');
      if (row["booking_status"].ToString() == "0")
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "'");
        if (dataRowArray.Length > 0)
          stringBuilder.Append(dataRowArray[0]["name"].ToString());
      }
      else
      {
        stringBuilder.Append("<select id='ddl_" + (object) num + "' name='ddl_" + (object) num + "'>");
        foreach (string str1 in strArray)
        {
          if (str1 != "")
          {
            DataRow[] dataRowArray = dataSet.Tables[0].Select("asset_id='" + str1 + "'");
            if (dataRowArray.Length > 0)
            {
              string str2 = "";
              if (row["asset_id"].ToString() == str1)
                str2 = "selected='true'";
              stringBuilder.Append("<option value='" + str1 + "' " + str2 + " >" + dataRowArray[0]["name"].ToString() + "</option>");
            }
          }
        }
      }
      stringBuilder.Append("</select>");
      stringBuilder.Append("</td>");
      stringBuilder.Append("</tr>");
      ++num;
    }
    stringBuilder.Append("</table>");
    this.html_table = stringBuilder.ToString();
  }

  private void get_bookings_to_booking_table(Dictionary<string, string> datesColl)
  {
    this.booking_table = this.outlooks.initialize_table();
    DataSet ds = this.outlooks.get_bookings(this.parameters["gid"]);
    int length = ds.Tables[0].Select("status='1'").Length;
    bool boolean = Convert.ToBoolean(ds.Tables[0].Rows[0]["is_repeat"]);
    if (boolean)
    {
      if (this.parameters["RecurrenceState"] == "olApptOccurrence" || this.parameters["RecurrenceState"] == "olApptException")
      {
        foreach (string key in datesColl.Keys)
        {
          DateTime dateTime1 = Convert.ToDateTime(key);
          DateTime dateTime2 = Convert.ToDateTime(datesColl[key]);
          ds = this.outlooks.get_bookings_date_only(dateTime1.ToString(this.outlooks.sql_short_date_format), dateTime2.ToString(this.outlooks.sql_short_date_format), this.parameters["gid"]);
          if (ds.Tables[0].Rows.Count == 0)
            this.Response.Redirect("outlook_error.aspx?error=no_dates");
          foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
          {
            row["book_from"] = (object) key;
            row["book_to"] = (object) datesColl[key];
            ds.Tables[0].AcceptChanges();
          }
        }
      }
    }
    else if (length == 1 && datesColl.Count > 1 && !boolean)
    {
      this.divMain.Visible = false;
      this.divError.Visible = true;
      this.lblError.Text = "Cannot convert a single booking to recurrent booking. Kindly create a new recurrent booking.";
      this.btn_submit.Visible = false;
    }
    if (length == datesColl.Count)
    {
      int index = 0;
      foreach (string key in datesColl.Keys)
      {
        DataRow row = ds.Tables[0].Rows[index];
        row["book_from"] = (object) Convert.ToDateTime(key);
        row["book_to"] = (object) Convert.ToDateTime(datesColl[key]);
        ds.Tables[0].AcceptChanges();
        ++index;
      }
    }
    this.dataset_to_booking_table(ds);
  }

  private void dataset_to_booking_table(DataSet ds)
  {
    foreach (DataRow row1 in (InternalDataCollectionBase) ds.Tables[0].Rows)
    {
      int int32 = Convert.ToInt32(row1["status"]);
      if (this.booking_table.Select("booking_id='" + row1["booking_id"] + "'").Length == 0)
      {
        DataRow row2 = this.booking_table.NewRow();
        row2["booking_id"] = row1["booking_id"];
        row2["outlook_id"] = row1["outlook_id"];
        row2["outlook_guid"] = row1["global_appointment_id"];
        row2["purpose"] = row1["purpose"];
        row2["booked_for"] = row1["booked_for"];
        row2["booked_for_name"] = row1["booked_for_name"];
        row2["telephone"] = row1["contact"];
        row2["email"] = row1["email"];
        row2["remarks"] = row1["remarks"];
        row2["booking_status"] = (object) int32;
        string str1 = row1["invites_email"].ToString();
        string str2 = "";
        try
        {
          string parameter = this.parameters["emails"];
          char ch = ';';
          string[] strArray1 = parameter.Split(ch);
          for (int index = 2; index < strArray1.Length; ++index)
            str2 = str2 + strArray1[index] + ";";
          string[] strArray2 = str1.Split(ch);
          for (int index = 2; index < strArray2.Length; ++index)
          {
            if (!parameter.Contains(strArray2[index]))
            {
              try
              {
                this.outlooks.delete_invites(Convert.ToInt64(row1["booking_id"]), strArray2[index], this.current_user.account_id);
              }
              catch
              {
              }
            }
          }
        }
        catch
        {
        }
        row2["invites_name"] = (object) "";
        row2["invites_email"] = !(str2 != "") ? (object) "" : (object) str2.Substring(0, str2.Length - 1);
        row2["book_from"] = row1["book_from"];
        row2["book_to"] = row1["book_to"];
        row2["asset_id"] = row1["asset_id"];
        row2["is_weekend"] = Convert.ToDateTime(row1["book_from"]).DayOfWeek == DayOfWeek.Sunday || Convert.ToDateTime(row1["book_from"]).DayOfWeek == DayOfWeek.Saturday ? (object) true : (object) false;
        DataRow[] dataRowArray = this.public_holidays.Tables[0].Select("'" + (object) Convert.ToDateTime(row1["book_from"]) + "' >= from_date and '" + (object) Convert.ToDateTime(row1["book_from"]) + "' <= to_date");
        if (dataRowArray.Length > 0)
        {
          row2["is_holiday"] = (object) true;
          row2["holiday_name"] = (object) dataRowArray[0]["holiday_name"].ToString();
        }
        else
        {
          row2["is_holiday"] = (object) false;
          row2["holiday_name"] = (object) "";
        }
        this.booking_table.Rows.Add(row2);
        this.booking_table.AcceptChanges();
      }
    }
  }

  private void populate_invitelist(string invites_email, string invites_name)
  {
    DataTable dataTable = new DataTable();
    dataTable.Columns.Add("invite_id");
    dataTable.Columns.Add("name");
    dataTable.Columns.Add("email");
    dataTable.AcceptChanges();
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      char ch = ';';
      invites_email = invites_email.Replace(ConfigurationManager.AppSettings["smtp_from_email"], "");
      string[] strArray1 = invites_email.Split(ch);
      string[] strArray2 = invites_name.Split(',');
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>S/No.</th>");
      stringBuilder.Append("<th class='hidden-480'>Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Email</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      int num = 1;
      foreach (string str1 in strArray1)
      {
        if (!string.IsNullOrEmpty(str1))
        {
          DataRow row = dataTable.NewRow();
          row["invite_id"] = (object) 0;
          string str2;
          try
          {
            str2 = strArray2[num - 1];
          }
          catch
          {
            str2 = "";
          }
          row["name"] = (object) invites_name;
          row["email"] = (object) str1;
          dataTable.Rows.Add(row);
          dataTable.AcceptChanges();
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + num.ToString() + "</td>");
          stringBuilder.Append("<td>" + str2 + "</td>");
          stringBuilder.Append("<td>" + str1 + "</td>");
          stringBuilder.Append("</tr>");
        }
        ++num;
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_invitelist = stringBuilder.ToString();
      this.ViewState.Add("invite_data", (object) dataTable);
    }
    catch (Exception ex)
    {
    }
  }

  private Dictionary<string, string> get_booking_dates()
  {
    Dictionary<string, string> bookingDates = new Dictionary<string, string>();
    try
    {
      if (((IEnumerable<string>) this.Request.Form.AllKeys).Contains<string>("recurring"))
      {
        if (this.parameters["recurring"].ToUpper() == "TRUE")
        {
          int no_of_events = Convert.ToInt32(this.parameters["Occurences"]);
          int int32_1 = Convert.ToInt32(this.parameters["Interval"]);
          DateTime dateTime1 = new DateTime();
          DateTime dateTime2 = new DateTime();
          if (this.action == "new")
          {
            dateTime1 = Convert.ToDateTime(this.parameters["PatternStartDate"]);
            dateTime2 = Convert.ToDateTime(this.parameters["PatternEndDate"]);
            DateTime dateTime3 = Convert.ToDateTime(this.parameters["start"]);
            DateTime dateTime4 = Convert.ToDateTime(this.parameters["end"]);
            dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime3.Hour, dateTime3.Minute, dateTime3.Second);
            dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime4.Hour, dateTime4.Minute, dateTime4.Second);
          }
          else if (this.parameters["RecurrenceState"] == "olApptMaster")
          {
            dateTime1 = Convert.ToDateTime(this.parameters["PatternStartDate"]);
            dateTime2 = Convert.ToDateTime(this.parameters["PatternEndDate"]);
            DateTime dateTime5 = Convert.ToDateTime(this.parameters["start"]);
            DateTime dateTime6 = Convert.ToDateTime(this.parameters["end"]);
            dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime5.Hour, dateTime5.Minute, dateTime5.Second);
            dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime6.Hour, dateTime6.Minute, dateTime6.Second);
          }
          else if (this.parameters["RecurrenceState"] == "olApptOccurrence")
          {
            dateTime1 = Convert.ToDateTime(this.parameters["start"]);
            dateTime2 = Convert.ToDateTime(this.parameters["end"]);
            no_of_events = 1;
          }
          else if (this.parameters["RecurrenceState"] == "olApptException")
          {
            dateTime1 = Convert.ToDateTime(this.parameters["start"]);
            dateTime2 = Convert.ToDateTime(this.parameters["end"]);
            no_of_events = 1;
          }
          DateTime from = dateTime1;
          DateTime to = dateTime2;
          this.public_holidays = this.holidays.get_holidays(from, to, this.current_user.account_id);
          if (this.parameters["RecurrenceType"].Contains("Daily"))
            bookingDates = this.bookingsbl.get_daily_dates(from.ToString(this.outlooks.sql_datetime_format), to.ToString(this.outlooks.sql_datetime_format), no_of_events, int32_1);
          else if (this.parameters["RecurrenceType"].Contains("Week"))
          {
            List<int> days = new List<int>();
            try
            {
              days = this.outlooks.getDaysColl(this.parameters["DayOfWeekMask"]);
            }
            catch (Exception ex)
            {
            }
            if (int32_1 == 0 && this.parameters["DayOfWeekMask"] == "62")
            {
              int frequency = 1;
              bookingDates = this.bookingsbl.get_weekly_dates(from.ToString(this.outlooks.sql_datetime_format), to.ToString(this.outlooks.sql_datetime_format), no_of_events, frequency, days);
            }
            else
              bookingDates = this.bookingsbl.get_weekly_dates(from.ToString(this.outlooks.sql_datetime_format), to.ToString(this.outlooks.sql_datetime_format), no_of_events, int32_1, days);
          }
          else if (this.parameters["RecurrenceType"].Contains("Month"))
          {
            int int32_2 = Convert.ToInt32(this.parameters["DayOfMonth"]);
            if (int32_2 > 0)
            {
              bookingDates = this.bookingsbl.get_monthly_every_dates(from.ToString(this.outlooks.sql_datetime_format), to.ToString(this.outlooks.sql_datetime_format), no_of_events, int32_1, int32_2);
            }
            else
            {
              int int32_3 = Convert.ToInt32(this.parameters["Instance"]);
              string day = this.parameters["DayOfWeekMask"].Substring(2);
              bookingDates = this.bookingsbl.get_monthly_specific_dates(from.ToString(this.outlooks.sql_datetime_format), to.ToString(this.outlooks.sql_datetime_format), no_of_events, int32_1, int32_3, day);
            }
          }
          else if (!this.parameters["RecurrenceType"].Contains("Year"))
            ;
        }
        else
        {
          DateTime dateTime7 = Convert.ToDateTime(this.Request.Form["start"]);
          DateTime dateTime8 = Convert.ToDateTime(this.Request.Form["end"]);
          this.public_holidays = this.holidays.get_holidays(dateTime7, dateTime8, this.current_user.account_id);
          bookingDates.Add(dateTime7.ToString(this.outlooks.sql_datetime_format), dateTime8.ToString(this.outlooks.sql_datetime_format));
        }
      }
    }
    catch
    {
      bookingDates = new Dictionary<string, string>();
    }
    this.ViewState.Add("selectedDates", (object) bookingDates);
    return bookingDates;
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    this.booking_table = (DataTable) this.ViewState["booking_table"];
    try
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      int num = 1;
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        try
        {
          row["asset_id"] = (object) this.Request.Form["ddl_" + (object) num];
          this.booking_table.AcceptChanges();
        }
        catch
        {
        }
        ++num;
      }
      this.ViewState["booking_table"] = (object) this.booking_table;
      if (this.booking_table.Rows.Count > 0)
        this.booking_table = this.Booking_transcations(this.booking_table);
    }
    catch
    {
    }
    string str = Guid.NewGuid().ToString().Replace("-", "");
    this.Session.Add(str + "_ds", (object) this.booking_table);
    this.Response.Redirect("outlook_confirm.aspx?location=" + this.ViewState["asset_names_"].ToString() + "&gid=" + str + "&email=" + this.current_user.email + "&user=" + this.current_user.username);
  }

  private void get_settings_bind_dropdown(Guid acc_id)
  {
    try
    {
      this.setting_data = (DataSet) this.capi.get_cache(acc_id.ToString() + "system_settings");
      if (this.setting_data == null)
      {
        this.setting_data = this.settings.view_settings(acc_id);
        this.capi.set_cache(acc_id.ToString() + "system_settings", (object) this.setting_data);
      }
    }
    catch
    {
    }
    if (!this.utilities.isValidDataset(this.setting_data))
      return;
    this.outlooks.populate_dropdown(this.setting_data, this.ddl_building, "building");
    this.outlooks.populate_dropdown(this.setting_data, this.ddl_category, "category");
    this.outlooks.populate_dropdown(this.setting_data, this.ddl_level, "level");
  }

  private string validateInput(Dictionary<string, string> inputParameters)
  {
    try
    {
      if (this.parameters["subject"] == "")
        return outlookplugin.validate_subject;
      try
      {
        if (this.parameters["NoEndDate"].ToUpper() == "TRUE")
          return outlookplugin.validate_no_enddate;
      }
      catch
      {
      }
      if (this.parameters["recurring"].ToUpper() == "TRUE")
      {
        DateTime dateTime1 = new DateTime();
        DateTime dateTime2 = new DateTime();
        if (this.parameters["RecurrenceState"] == "olApptMaster")
        {
          dateTime1 = Convert.ToDateTime(this.parameters["PatternStartDate"]);
          dateTime2 = Convert.ToDateTime(this.parameters["PatternEndDate"]);
          DateTime dateTime3 = Convert.ToDateTime(this.parameters["start"]);
          DateTime dateTime4 = Convert.ToDateTime(this.parameters["end"]);
          dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime3.Hour, dateTime3.Minute, dateTime3.Second);
          dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime4.Hour, dateTime4.Minute, dateTime4.Second);
        }
        else if (this.parameters["RecurrenceState"] == "olApptOccurrence")
        {
          dateTime1 = Convert.ToDateTime(this.parameters["start"]);
          dateTime2 = Convert.ToDateTime(this.parameters["end"]);
        }
        if (dateTime1 < this.current_timestamp || dateTime2 < this.current_timestamp)
          return outlookplugin.select_futuerdate;
        if (dateTime2 < dateTime1)
          return outlookplugin.start_end_datecheck;
      }
      else
      {
        DateTime dateTime5 = Convert.ToDateTime(this.parameters["start"]);
        DateTime dateTime6 = Convert.ToDateTime(this.parameters["end"]);
        if (dateTime5 < this.current_timestamp || dateTime6 < this.current_timestamp)
          return outlookplugin.select_futuerdate;
        if (dateTime6 < dateTime5)
          return outlookplugin.start_end_datecheck;
      }
    }
    catch
    {
    }
    return "";
  }

  private void parameters_list()
  {
    if (!this.enable_debug)
      return;
    string str = "<table>";
    foreach (string key in this.Request.Form.Keys)
    {
      str += "<tr>";
      str = str + "<td>" + key + "</td><td>" + this.Request.Form[key] + "</td>";
      str += "</tr>";
    }
    this.html_parameters = str + "</table>";
  }

  private string get_user(string email) => new users_api().get_users_name(email);

  public DataTable Booking_transcations(DataTable booking_data)
  {
    try
    {
      if (this.parameters == null)
        this.load_parameters();
      this.initialize_session();
      asset_booking assetBooking = new asset_booking();
      assetBooking.booking_type = Convert.ToInt32((object) api_constants.booking_type.Outlook);
      DataSet dataSet = this.assets.view_assets(this.current_user.account_id);
      Guid guid = Guid.NewGuid();
      string ids = "";
      bool flag1 = false;
      if (this.parameters["recurring"].ToUpper() == "TRUE")
        flag1 = true;
      foreach (DataRow row1 in (InternalDataCollectionBase) booking_data.Rows)
      {
        if (row1["booking_status"].ToString() != "0")
        {
          assetBooking.account_id = this.current_user.account_id;
          assetBooking.booking_id = Convert.ToInt64(row1["booking_id"]);
          assetBooking.is_repeat = flag1;
          if (flag1)
          {
            assetBooking.is_repeat = true;
            assetBooking.repeat_reference_id = guid;
          }
          assetBooking.record_id = guid;
          assetBooking.transfer_original_booking_id = 0L;
          assetBooking.transfer_reason = "";
          assetBooking.transfer_request = false;
          assetBooking.created_on = this.current_timestamp;
          ids = ids + row1["asset_id"].ToString() + ",";
          assetBooking.asset_id = Convert.ToInt64(row1["asset_id"]);
          assetBooking.book_from = Convert.ToDateTime(row1["book_from"]);
          assetBooking.book_to = Convert.ToDateTime(row1["book_to"]);
          assetBooking.book_duration = (double) this.utilities.getDuration(assetBooking.book_from, assetBooking.book_to);
          assetBooking.contact = this.txt_telephone.Text;
          assetBooking.created_by = this.current_user.user_id;
          assetBooking.booked_for = Convert.ToInt64(row1["booked_for"]);
          assetBooking.email = this.txt_email.Text;
          assetBooking.modified_by = this.current_user.user_id;
          assetBooking.purpose = this.txt_purpose.Text;
          assetBooking.remarks = this.txt_remarks.Value;
          assetBooking.repeat_reference_id = assetBooking.record_id;
          assetBooking.booking_type = 10;
          DataRow[] dataRowArray = dataSet.Tables[0].Select("asset_id=" + assetBooking.asset_id.ToString());
          long num1;
          try
          {
            num1 = Convert.ToInt64(dataRowArray[0]["asset_owner_group_id"]);
          }
          catch
          {
            num1 = 0L;
          }
          if (num1 == 0L)
            assetBooking.status = (short) 1;
          else if (this.u_group.group_type == 1)
            assetBooking.status = (short) 1;
          else if (this.u_group.group_type == 2)
          {
            long num2 = 0;
            foreach (user_group userGroup in this.uapi.get_user_group(assetBooking.created_by, this.current_user.account_id).Values)
            {
              if (userGroup.group_type != 0)
                num2 = userGroup.group_id;
            }
            assetBooking.status = num2 != num1 ? (short) 4 : (short) 1;
          }
          else
            assetBooking.status = (short) 4;
          assetBooking = this.bookings.update_booking(assetBooking);
          if (assetBooking.booking_id > 0L)
          {
            row1["booking_status"] = (object) assetBooking.status;
            row1["booking_id"] = (object) assetBooking.booking_id;
            row1["remarks"] = (object) assetBooking.remarks;
            booking_data.AcceptChanges();
            try
            {
              outlook_booking outlookBooking = this.outlooks.outlook_booking_update(new outlook_booking()
              {
                account_id = this.current_account.account_id,
                booking_id = assetBooking.booking_id,
                created_by = this.current_user.user_id,
                global_appointment_id = row1["outlook_guid"].ToString(),
                outlook_id = Convert.ToInt64(row1["outlook_id"])
              });
              row1["outlook_id"] = (object) outlookBooking.outlook_id;
              booking_data.AcceptChanges();
            }
            catch
            {
            }
            try
            {
              DataTable dataTable = (DataTable) this.ViewState["invite_data"];
              if (dataTable != null)
              {
                Dictionary<long, asset_booking_invite> inviteList = this.bookings.get_invite_list(assetBooking.booking_id, assetBooking.account_id);
                foreach (DataRow row2 in (InternalDataCollectionBase) dataTable.Rows)
                {
                  asset_booking_invite assetBookingInvite1 = new asset_booking_invite();
                  bool flag2 = false;
                  foreach (asset_booking_invite assetBookingInvite2 in inviteList.Values)
                  {
                    if (assetBookingInvite2.email == row2["email"].ToString())
                      flag2 = true;
                  }
                  if (!flag2)
                  {
                    assetBookingInvite1.account_id = this.current_user.account_id;
                    assetBookingInvite1.booking_id = assetBooking.booking_id;
                    assetBookingInvite1.booking_invite_id = Convert.ToInt64(row2["invite_id"]);
                    assetBookingInvite1.created_by = this.current_user.user_id;
                    assetBookingInvite1.email = row2["email"].ToString();
                    assetBookingInvite1.modified_by = this.current_user.user_id;
                    assetBookingInvite1.name = row2["name"].ToString();
                    assetBookingInvite1.record_id = Guid.NewGuid();
                    assetBookingInvite1.repeat_reference_id = assetBookingInvite1.record_id;
                    this.bookings.update_invite(assetBookingInvite1);
                  }
                }
              }
            }
            catch
            {
            }
            try
            {
              if (assetBooking.status != (short) 1)
              {
                workflow workflow = new workflow();
                workflow.account_id = this.current_user.account_id;
                workflow.action_owner_id = num1 <= 0L ? this.current_user.user_id : num1;
                workflow.action_remarks = "Asset booking";
                workflow.action_type = (short) 1;
                workflow.created_by = this.current_user.user_id;
                workflow.created_on = this.current_timestamp;
                workflow.due_on = new DateTime(1900, 1, 1);
                workflow.record_id = Guid.NewGuid();
                workflow.reference_id = assetBooking.booking_id;
                workflow.workflow_id = 0L;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<properties><asset_id>" + (object) assetBooking.asset_id + " </asset_id></properties>");
                workflow.properties = xmlDocument;
                new workflow_api().update_workflow(workflow);
              }
            }
            catch
            {
            }
          }
        }
        string str = "";
        if (ids != "")
        {
          ids = ids.TrimEnd(',');
          DataSet assets = this.outlooks.get_assets(this.current_user.account_id, ids);
          if (assets != null)
          {
            foreach (DataRow row3 in (InternalDataCollectionBase) assets.Tables[0].Rows)
              str = str + row3["name"].ToString() + ",";
          }
        }
        if (str != "")
          this.ViewState.Add("asset_names_", (object) str.Substring(0, str.Length - 1));
      }
    }
    catch (Exception ex)
    {
    }
    return booking_data;
  }

  protected void btn_refresh_Click(object sender, EventArgs e)
  {
    if (this.parameters == null)
      this.load_parameters();
    this.bookable_rooms = (List<long>) this.ViewState["rooms"];
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    this.check_availability((Dictionary<string, string>) this.ViewState["selected_dates"], 0, 0);
    this.ViewState.Add("booking_table", (object) this.booking_table);
    this.populate_ui(this.booking_table);
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    this.pnl_cancel.Visible = true;
    this.btn_cancel.Visible = false;
    this.btn_submit.Visible = false;
    this.pnl_filter.Visible = false;
    this.bookingDates = (Dictionary<string, string>) this.ViewState["selected_dates"];
    this.load_parameters();
    this.initialize_session();
    this.process_view();
  }

  private void write_log(string message)
  {
    if (!this.enable_debug)
      return;
    this.log.Info((object) message);
  }

  protected void btn_confirm_cancel_Click(object sender, EventArgs e)
  {
    this.initialize_session();
    this.booking_table = (DataTable) this.ViewState["booking_table"];
    if (this.booking_table.Rows.Count > 0)
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        asset_booking booking = this.bookings.get_booking(Convert.ToInt64(row["booking_id"]), this.current_account.account_id);
        booking.cancel_reason = "";
        booking.status = (short) 0;
        booking.modified_by = this.current_user.user_id;
        booking.cancel_by = this.current_user.user_id;
        booking.cancel_on = this.current_timestamp;
        booking.cancel_reason = this.txt_cancel_reason.Text;
        this.bookings.set_cancel_status(this.bookings.cancel_booking(booking));
      }
    }
    this.divMain.Visible = false;
    this.divError.Visible = true;
    this.lblError.Text = "The booking has been cancelled. Please cancel the appointment from your calendar.";
    if (this.parameters == null)
      this.load_parameters();
    this.process_view();
  }

  protected void btn_cancel_cancel_Click(object sender, EventArgs e)
  {
    this.pnl_cancel.Visible = false;
    this.btn_cancel.Visible = true;
    this.btn_submit.Visible = true;
    this.pnl_filter.Visible = true;
    this.bookingDates = (Dictionary<string, string>) this.ViewState["selected_dates"];
    this.load_parameters();
    this.initialize_session();
    this.process_view();
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
