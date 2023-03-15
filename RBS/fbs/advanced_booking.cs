// Decompiled with JetBrains decompiler
// Type: advanced_booking
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class advanced_booking : fbs_base_page, IRequiresSessionState
{
  private DataTable booking_table;
  public string html_table;
  public DataSet public_holidays;
  public string html_error;
  public bool show_clear;
  private Guid repeat_reference_id = Guid.Empty;
  protected DropDownList ddl_category;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected TextBox txt_capacity;
  protected CheckBox chk_fav;
  protected Button btn_check_availability;
  protected Literal litError;
  protected HtmlGenericControl alt_err;
  protected DropDownList ddl_common;
  protected CheckBox chk_all;
  protected HiddenField hdn_selected;
  protected Button btn_submit;
  protected TextBox txt_daily_start;
  protected TextBox txt_daily_end;
  protected TextBox txt_daily_number_of_events;
  protected DropDownList ddl_daily_from_time;
  protected DropDownList ddl_daily_to_time;
  protected CheckBox chk_daily_all_day;
  protected TextBox txt_daily_occurence;
  protected DropDownList ddl_daily_weekend_option;
  protected Button btn_daily;
  protected TextBox txt_weekly_start;
  protected TextBox txt_weekly_end;
  protected TextBox txt_weekly_number_of_events;
  protected DropDownList ddl_weekly_from_time;
  protected DropDownList ddl_weekly_to_time;
  protected CheckBox chk_weekly_all_day;
  protected TextBox txt_weekly_occurence;
  protected Label lblchkError;
  protected CheckBox chkSun;
  protected CheckBox chkMon;
  protected CheckBox chkTue;
  protected CheckBox chkWed;
  protected CheckBox chkThu;
  protected CheckBox chkFri;
  protected CheckBox chkSat;
  protected DropDownList ddl_weekly_weekend_option;
  protected Button btn_weekly;
  protected TextBox txt_monthly_start;
  protected TextBox txt_monthly_end;
  protected TextBox txt_monthly_number_of_events;
  protected DropDownList ddl_monthly_from_time;
  protected DropDownList ddl_monthly_to_time;
  protected CheckBox chk_monthly_all_day;
  protected HtmlInputRadioButton rndMonthlyEvery;
  protected TextBox txtMonthlyDay;
  protected TextBox txtMonthlyMonth;
  protected DropDownList ddl_monthly_weekend_option;
  protected HtmlInputRadioButton rndMonthlySpecific;
  protected DropDownList cboMonthlyCount;
  protected DropDownList cboMonthlyWeekday;
  protected TextBox txt_monthly_frequency;
  protected HtmlTable monthlytable;
  protected Button btn_monthly;
  protected TextBox txt_single_date;
  protected DropDownList ddl_single_start_time;
  protected TextBox txt_single_to_date;
  protected DropDownList ddl_single_end_time;
  protected CheckBox chk_single_all_day;
  protected Button btn_specific;
  protected TextBox txt_change_from_date;
  protected DropDownList ddl_change_from_time;
  protected TextBox txt_change_to_date;
  protected DropDownList ddl_change_to_time;
  protected CheckBox chk_change_all_day;
  protected HiddenField hdn_change;
  protected Button btn_change;
  protected DropDownList ddl_o_from;
  protected DropDownList ddl_o_to;
  protected DropDownList ddl_n_from;
  protected DropDownList ddl_n_to;
  protected Button btn_bulk_change_time;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      setting setting1 = this.settings.get_setting("booking_hours", this.current_user.account_id);
      setting setting2 = this.settings.get_setting("booking_slot", this.current_user.account_id);
      string[] strArray = setting1.value.Split('|');
      DateTime dateTime1 = Convert.ToDateTime(strArray[0]);
      DateTime dateTime2 = Convert.ToDateTime(strArray[1]);
      int int32 = Convert.ToInt32(setting2.value);
      this._Populate_Time(this.ddl_single_start_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_single_end_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_daily_from_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_daily_to_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_weekly_from_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_weekly_to_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_monthly_from_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_monthly_to_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_change_from_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_change_to_time, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_o_from, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_o_to, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_n_from, dateTime1, dateTime2, int32);
      this._Populate_Time(this.ddl_n_to, dateTime1, dateTime2, int32);
      this.initialize_dates();
      this.initialize_table();
      DataSet data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (data == null)
      {
        data = this.settings.view_settings(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) data);
      }
      this.utilities.populate_dropdown(data, this.ddl_building, "building");
      this.utilities.populate_dropdown(data, this.ddl_category, "category");
      this.utilities.populate_dropdown(data, this.ddl_level, "level");
      long booking_id;
      try
      {
        booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        booking_id = 0L;
      }
      try
      {
        this.repeat_reference_id = new Guid(this.Request.QueryString["repeat"]);
      }
      catch
      {
        this.repeat_reference_id = Guid.Empty;
      }
      this.populate_existing_booking(booking_id, this.repeat_reference_id);
    }
    else
    {
      string event_id = this.Request.Form["__EVENTARGUMENT"];
      if (event_id != "")
        this.remove_item(event_id);
    }
    this.alt_err.Visible = false;
  }

  public void _Populate_Time(DropDownList cbo, DateTime from, DateTime to, int slot)
  {
    try
    {
      cbo.Items.Clear();
      int num = 1440 / slot;
      string str = this.current_timestamp.ToShortDateString() + " 00:00 AM";
      DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
      for (int index = 0; index < num; ++index)
      {
        if (!this.gp.isAdminType)
        {
          if (dateTime >= from && dateTime <= to)
            cbo.Items.Add(new ListItem(dateTime.ToShortTimeString(), dateTime.ToString("hh:mm tt")));
        }
        else
          cbo.Items.Add(new ListItem(dateTime.ToShortTimeString(), dateTime.ToString("hh:mm tt")));
        dateTime = dateTime.AddMinutes((double) slot);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_existing_booking(long booking_id, Guid repeat_reference_id)
  {
    if (booking_id <= 0L)
      return;
    DataSet dataSet1 = new DataSet();
    DataSet dataSet2;
    if (repeat_reference_id == Guid.Empty)
    {
      dataSet2 = this.bookings.get_bookings(this.current_user.account_id, booking_id);
      try
      {
        DataRow row = dataSet2.Tables[0].Rows[0];
        if (!string.IsNullOrEmpty(row[nameof (repeat_reference_id)].ToString()))
        {
          repeat_reference_id = new Guid(row[nameof (repeat_reference_id)].ToString());
          dataSet2 = this.bookings.get_repeat_bookings(repeat_reference_id, this.current_user.account_id);
        }
      }
      catch
      {
      }
    }
    else
      dataSet2 = this.bookings.get_repeat_bookings(repeat_reference_id, this.current_user.account_id);
    this.booking_table = (DataTable) this.ViewState["table"];
    foreach (DataRow row1 in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
    {
      switch (Convert.ToInt16(row1["status"]))
      {
        case 1:
        case 4:
          if (Convert.ToDateTime(row1["book_to"]) > DateTime.UtcNow.AddHours(this.current_account.timezone))
          {
            DataRow row2 = this.booking_table.NewRow();
            row2[nameof (booking_id)] = row1[nameof (booking_id)];
            row2["db_booking_id"] = row1[nameof (booking_id)];
            row2["book_from"] = (object) Convert.ToDateTime(row1["book_from"]);
            row2["book_to"] = (object) Convert.ToDateTime(row1["book_to"]);
            row2["asset_id"] = (object) Convert.ToInt64(row1["asset_id"]);
            row2[nameof (repeat_reference_id)] = (object) new Guid(row1[nameof (repeat_reference_id)].ToString());
            row2["status"] = (object) Convert.ToInt32(row1["status"]);
            row2["is_weekend"] = Convert.ToDateTime(row1["book_from"]).DayOfWeek == DayOfWeek.Sunday || Convert.ToDateTime(row1["book_from"]).DayOfWeek == DayOfWeek.Saturday ? (object) true : (object) false;
            this.public_holidays = this.holidays.get_holidays(Convert.ToDateTime(row1["book_from"]), Convert.ToDateTime(row1["book_to"]), this.account_id);
            if (this.public_holidays.Tables[0].Rows.Count > 0)
            {
              row2["is_holiday"] = (object) true;
              row2["holiday_name"] = (object) this.public_holidays.Tables[0].Rows[0]["holiday_name"].ToString();
            }
            else
            {
              row2["is_holiday"] = (object) false;
              row2["holiday_name"] = (object) "";
            }
            this.booking_table.Rows.Add(row2);
            this.booking_table.AcceptChanges();
            continue;
          }
          continue;
        default:
          continue;
      }
    }
    this.ViewState.Add("table", (object) this.booking_table);
    this.get_assets();
    this.populate_dates_table();
  }

  private void initialize_dates()
  {
    DateTime input = DateTime.UtcNow.AddHours(this.current_account.timezone);
    this.txt_single_date.Text = input.ToString("dd-MMM-yyyy");
    this.txt_single_to_date.Text = input.ToString("dd-MMM-yyyy");
    this.txt_daily_start.Text = input.ToString("dd-MMM-yyyy");
    this.txt_weekly_start.Text = input.ToString("dd-MMM-yyyy");
    this.txt_monthly_start.Text = input.ToString("dd-MMM-yyyy");
    this.txt_daily_end.Text = input.ToString("dd-MMM-yyyy");
    this.txt_weekly_end.Text = input.ToString("dd-MMM-yyyy");
    this.txt_monthly_end.Text = input.ToString("dd-MMM-yyyy");
    this.ddl_daily_from_time.SelectedValue = this.utilities.TimeRoundUp(input).ToString("hh:mm tt");
    this.ddl_daily_to_time.SelectedValue = this.utilities.TimeRoundUp(input.AddMinutes(60.0)).ToString("hh:mm tt");
    this.ddl_weekly_from_time.SelectedValue = this.utilities.TimeRoundUp(input).ToString("hh:mm tt");
    this.ddl_weekly_to_time.SelectedValue = this.utilities.TimeRoundUp(input.AddMinutes(60.0)).ToString("hh:mm tt");
    this.ddl_monthly_from_time.SelectedValue = this.utilities.TimeRoundUp(input).ToString("hh:mm tt");
    this.ddl_monthly_to_time.SelectedValue = this.utilities.TimeRoundUp(input.AddMinutes(60.0)).ToString("hh:mm tt");
    this.ddl_single_start_time.SelectedValue = this.utilities.TimeRoundUp(input).ToString("hh:mm tt");
    this.ddl_single_end_time.SelectedValue = this.utilities.TimeRoundUp(input.AddMinutes(60.0)).ToString("hh:mm tt");
  }

  private void remove_item(string event_id)
  {
    this.save_asset_ids();
    this.booking_table = (DataTable) this.ViewState["table"];
    switch (event_id)
    {
      case "0":
        DataTable dataTable = this.booking_table.Clone();
        foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
        {
          if (row["db_booking_id"].ToString() != "0")
          {
            row["status"] = (object) 0;
            dataTable.Rows.Add(row);
          }
          dataTable.AcceptChanges();
        }
        this.booking_table = dataTable;
        break;
      case "selected":
        string str1 = this.hdn_selected.Value;
        if (str1 != "")
        {
          string str2 = str1;
          char[] chArray = new char[1]{ '|' };
          foreach (string str3 in str2.Split(chArray))
          {
            if (str3 != "")
            {
              foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
              {
                if (row["booking_id"].ToString() == str3)
                {
                  if (row["db_booking_id"].ToString() != "0")
                    row["status"] = (object) 0;
                  else
                    row.Delete();
                  this.booking_table.AcceptChanges();
                  break;
                }
              }
            }
          }
          break;
        }
        break;
      default:
        IEnumerator enumerator = this.booking_table.Rows.GetEnumerator();
        try
        {
          while (enumerator.MoveNext())
          {
            DataRow current = (DataRow) enumerator.Current;
            if (current["booking_id"].ToString() == event_id)
            {
              if (current["db_booking_id"].ToString() != "0")
                current["status"] = (object) 0;
              else
                current.Delete();
              this.booking_table.AcceptChanges();
              break;
            }
          }
          break;
        }
        finally
        {
          if (enumerator is IDisposable disposable)
            disposable.Dispose();
        }
    }
    this.ViewState["table"] = (object) this.booking_table;
    this.populate_dates_table();
  }

  private void initialize_table()
  {
    this.booking_table = new DataTable();
    this.booking_table.Columns.Add(new DataColumn("booking_id", Type.GetType("System.String")));
    this.booking_table.Columns.Add(new DataColumn("book_from", Type.GetType("System.DateTime")));
    this.booking_table.Columns.Add(new DataColumn("book_to", Type.GetType("System.DateTime")));
    this.booking_table.Columns.Add(new DataColumn("asset_id", Type.GetType("System.Int64")));
    this.booking_table.Columns.Add(new DataColumn("asset_options", Type.GetType("System.String")));
    this.booking_table.Columns.Add(new DataColumn("is_holiday", Type.GetType("System.Boolean")));
    this.booking_table.Columns.Add(new DataColumn("holiday_name", Type.GetType("System.String")));
    this.booking_table.Columns.Add(new DataColumn("is_weekend", Type.GetType("System.Boolean")));
    this.booking_table.Columns.Add(new DataColumn("repeat_reference_id", Type.GetType("System.String")));
    this.booking_table.Columns.Add(new DataColumn("status", Type.GetType("System.Int32")));
    this.booking_table.Columns.Add(new DataColumn("db_booking_id", Type.GetType("System.Int64")));
    this.booking_table.AcceptChanges();
    this.ViewState.Add("table", (object) this.booking_table);
  }

  private void populate_table(Dictionary<string, string> selectedDates, DataSet public_holidays)
  {
    this.booking_table = (DataTable) this.ViewState["table"];
    foreach (string key in selectedDates.Keys)
    {
      DateTime dateTime1 = Convert.ToDateTime(key);
      DateTime dateTime2 = Convert.ToDateTime(selectedDates[key]);
      DataRow row = this.booking_table.NewRow();
      row["booking_id"] = (object) Guid.NewGuid().ToString().Replace('-', '_');
      row["book_from"] = (object) dateTime1;
      row["book_to"] = (object) dateTime2;
      row["asset_id"] = (object) 0;
      row["repeat_reference_id"] = (object) this.repeat_reference_id;
      row["db_booking_id"] = (object) "0";
      row["status"] = (object) 1;
      DataRow[] dataRowArray = public_holidays.Tables[0].Select("'" + (object) dateTime1 + "' >= from_date and '" + (object) dateTime2 + "' <= to_date");
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
      row["is_weekend"] = dateTime1.DayOfWeek == DayOfWeek.Sunday || dateTime1.DayOfWeek == DayOfWeek.Saturday ? (object) true : (object) false;
      this.booking_table.Rows.Add(row);
      this.booking_table.AcceptChanges();
    }
    this.ViewState["table"] = (object) this.booking_table;
  }

  private void populate_dates_table()
  {
    DataSet room_data = this.assets.view_assets(this.current_user.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    int num1 = 1;
    DataView defaultView = this.booking_table.DefaultView;
    defaultView.Sort = "book_from";
    this.booking_table = defaultView.ToTable();
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      if (row["status"].ToString() != "0")
      {
        DateTime dateTime1 = Convert.ToDateTime(row["book_from"]);
        DateTime dateTime2 = DateTime.UtcNow;
        DateTime dateTime3 = dateTime2.AddHours(this.current_account.timezone);
        if (dateTime1 > dateTime3)
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td>" + (object) num1 + "</td>");
          string str1 = "";
          if (row["status"].ToString() == "1")
            str1 = !(row["db_booking_id"].ToString() == "0") ? "<Span class='label label-Booked'>Booked</span>" : "<Span class='label label-New'>New</span>";
          if (row["status"].ToString() == "4")
            str1 = "<Span class='label label-Pending'>Pending</span>";
          if (row["db_booking_id"].ToString() == "0" || row["db_booking_id"].ToString() == "")
            stringBuilder.Append("<td><input type='checkbox' name='chk_date' id='chk_date' value='" + row["booking_id"].ToString() + "'></td>");
          else
            stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td>" + str1 + "</td>");
          stringBuilder.Append("<td id='from_" + row["booking_id"].ToString() + "'>" + Convert.ToDateTime(row["book_from"]).ToString("ddd, dd-MMM-yy hh:mm tt") + "</td>");
          stringBuilder.Append("<td id='to_" + row["booking_id"].ToString() + "'>" + Convert.ToDateTime(row["book_to"]).ToString("ddd, dd-MMM-yy hh:mm tt") + "</td>");
          stringBuilder.Append("<td>" + row["holiday_name"].ToString() + "</td>");
          stringBuilder.Append("<td>");
          string[] strArray = row["asset_options"].ToString().Split(',');
          stringBuilder.Append("<select id='ddl_" + row["booking_id"].ToString() + "' name='ddl_" + row["booking_id"].ToString() + "' class='ddlroom'>");
          foreach (string str2 in strArray)
          {
            if (str2 != "")
            {
              try
              {
                DataRow[] dataRowArray1 = room_data.Tables[0].Select("asset_id='" + str2 + "'");
                if (dataRowArray1.Length > 0)
                {
                  string str3 = "";
                  if (row["asset_id"].ToString() == str2)
                    str3 = "selected='true'";
                  string str4 = dataRowArray1[0]["name"].ToString();
                  if (!this.gp.isAdminType)
                  {
                    bool flag = true;
                    DataSet dataSet1 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
                    {
                      "booking_lead_time"
                    });
                    DataSet dataSet2 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
                    {
                      "book_weekend"
                    });
                    this.assets.view_asset_properties(this.current_user.account_id, new string[1]
                    {
                      "book_holiday"
                    });
                    DateTime dateTime4 = Convert.ToDateTime(row["book_from"]);
                    Convert.ToDateTime(row["book_to"]);
                    DataRow[] dataRowArray2 = dataSet1.Tables[0].Select("asset_id=" + str2);
                    DataRow[] dataRowArray3 = dataSet2.Tables[0].Select("asset_id=" + str2);
                    DataRow[] dataRowArray4 = dataSet2.Tables[0].Select("asset_id=" + str2);
                    try
                    {
                      if (dataRowArray2.Length > 0)
                      {
                        string str5 = dataRowArray2[0].ItemArray[2].ToString();
                        if (str5 != "")
                        {
                          if (str5.Split(' ')[0] != "0")
                          {
                            DateTime dateTime5 = new DateTime();
                            DateTime dateTime6;
                            if (str5.Split(' ')[1] == "Hour(s)")
                            {
                              dateTime6 = DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(Convert.ToDouble(str5.Split(' ')[0]));
                            }
                            else
                            {
                              int num2 = 0;
                              int num3 = 0;
                              while (true)
                              {
                                if ((long) num3 < Convert.ToInt64(str5.Split(' ')[0]) + 1L)
                                {
                                  ++num2;
                                  if (DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 1)).DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 1)).DayOfWeek == DayOfWeek.Sunday)
                                  {
                                    if (dataRowArray3.Length > 0)
                                    {
                                      if (dataRowArray3[0].ItemArray[2].ToString() == "False")
                                        ++num2;
                                    }
                                    else
                                      ++num2;
                                  }
                                  if (dataRowArray4.Length > 0)
                                  {
                                    if (dataRowArray4[0].ItemArray[2].ToString() == "False")
                                    {
                                      DataSet ds = this.holidays.view_holidays(DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 1)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 2)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", this.account_id);
                                      if (this.utilities.isValidDataset(ds) && ds.Tables[0].Rows.Count > 0)
                                        ++num2;
                                    }
                                  }
                                  else
                                    ++num2;
                                  ++num3;
                                }
                                else
                                  break;
                              }
                              dateTime2 = DateTime.UtcNow;
                              dateTime2 = dateTime2.AddHours(this.current_account.timezone);
                              dateTime2 = dateTime2.Date;
                              dateTime6 = dateTime2.AddDays((double) num2);
                            }
                            if (dateTime4 < dateTime6)
                              flag = false;
                          }
                        }
                      }
                    }
                    catch (Exception ex)
                    {
                      fbs_base_page.log.Error((object) ex.ToString());
                    }
                    if (flag)
                    {
                      if (dataRowArray1[0]["asset_owner_group_id"] != DBNull.Value)
                      {
                        if (!this.approvable_rooms.Contains(Convert.ToInt64(dataRowArray1[0]["asset_id"])))
                          str4 += " (Approval Required)";
                      }
                      stringBuilder.Append("<option value='" + str2 + "' " + str3 + " >" + str4 + "</option>");
                    }
                  }
                  else
                    stringBuilder.Append("<option value='" + str2 + "' " + str3 + " >" + str4 + "</option>");
                }
              }
              catch (Exception ex)
              {
                fbs_base_page.log.Error((object) ex.ToString());
              }
            }
          }
          stringBuilder.Append("</select>");
          stringBuilder.Append("</td>");
          if (strArray.Length > 0)
            stringBuilder.Append("<td><a class='btn orange icn-only' href='javascript:view(\"ddl_" + row["booking_id"].ToString().Trim() + "\");'><i class='icon-search icon-white'></i></a></td>");
          else
            stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td><a class='btn blue icn-only' href='javascript:show_change(\"" + row["booking_id"].ToString().Trim() + "\");'><i class='icon-pencil icon-white'></i></a></td>");
          if (row["db_booking_id"].ToString() == "0" || row["db_booking_id"].ToString() == "")
            stringBuilder.Append("<td><a class='btn red icn-only' href='javascript:remove_date(\"" + row["booking_id"].ToString().Trim() + "\");'><i class='icon-remove icon-white'></i></a></td>");
          else
            stringBuilder.Append("<td></td>");
          stringBuilder.Append("</tr>");
          ++num1;
        }
      }
    }
    stringBuilder.Append("</table>");
    this.html_table = stringBuilder.ToString();
    this.get_common_rooms(room_data);
    if (this.booking_table.Rows.Count > 0)
    {
      this.show_clear = true;
      this.btn_submit.Visible = true;
    }
    else
    {
      this.show_clear = false;
      this.btn_submit.Visible = false;
    }
  }

  private List<int> getDaysColl()
  {
    List<int> daysColl = new List<int>();
    try
    {
      if (this.chkSun.Checked)
        daysColl.Add(0);
      if (this.chkMon.Checked)
        daysColl.Add(1);
      if (this.chkTue.Checked)
        daysColl.Add(2);
      if (this.chkWed.Checked)
        daysColl.Add(3);
      if (this.chkThu.Checked)
        daysColl.Add(4);
      if (this.chkFri.Checked)
        daysColl.Add(5);
      if (this.chkSat.Checked)
        daysColl.Add(6);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return daysColl;
  }

  private void get_assets()
  {
    this.booking_table = (DataTable) this.ViewState["table"];
    if (this.bookable_rooms == null || this.bookable_rooms.Count == 0)
      this.bookable_rooms = this.assets.get_bookable_assets(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType);
    List<long> rooms = new List<long>();
    bool flag1 = true;
    bool flag2 = true;
    int num1 = 0;
    DateTime dateTime1 = new DateTime(2000, 1, 1, 0, 0, 0);
    DateTime dateTime2 = new DateTime(2000, 1, 1, 23, 59, 59);
    DateTime dateTime3 = new DateTime(2000, 1, 1, 0, 0, 0);
    DateTime dateTime4 = new DateTime(2000, 1, 1, 23, 59, 59);
    long int64_1 = Convert.ToInt64(this.ddl_building.SelectedItem.Value);
    long int64_2 = Convert.ToInt64(this.ddl_level.SelectedItem.Value);
    long int64_3 = Convert.ToInt64(this.ddl_category.SelectedItem.Value);
    int capacity;
    try
    {
      capacity = Convert.ToInt32(this.txt_capacity.Text);
    }
    catch (Exception ex)
    {
      capacity = 0;
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
        this.html_error = "<div class='alr a-err'><strong> Error!</strong>You to do have any favourites assigned to you.</div>";
        return;
      }
    }
    else
      rooms = this.bookable_rooms;
    DataSet assets = this.assets.get_assets(rooms, this.current_user.account_id, int64_1, int64_2, int64_3, capacity, "");
    rooms.Clear();
    if (assets.Tables[0].Rows.Count == 0)
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        row["asset_options"] = (object) "";
        this.booking_table.AcceptChanges();
      }
    }
    else
    {
      foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
        rooms.Add(Convert.ToInt64(row["asset_id"]));
      DataSet dataSet1 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (dataSet1 == null)
      {
        dataSet1 = this.settings.view_settings(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) dataSet1);
      }
      DataRow[] dataRowArray1 = dataSet1.Tables[0].Select("parameter='advance_booking_window'");
      if (dataRowArray1.Length > 0)
        num1 = Convert.ToInt32(dataRowArray1[0]["value"]);
      DataRow[] dataRowArray2 = dataSet1.Tables[0].Select("parameter='book_holiday'");
      if (dataRowArray2.Length > 0)
        flag1 = Convert.ToBoolean(dataRowArray2[0]["value"]);
      DataRow[] dataRowArray3 = dataSet1.Tables[0].Select("parameter='book_weekend'");
      if (dataRowArray3.Length > 0)
        flag2 = Convert.ToBoolean(dataRowArray3[0]["value"]);
      DataRow[] dataRowArray4 = dataSet1.Tables[0].Select("parameter='operating_hours'");
      if (dataRowArray4.Length > 0)
      {
        string[] strArray = dataRowArray4[0]["value"].ToString().Split('|');
        dateTime1 = Convert.ToDateTime(strArray[0]);
        dateTime2 = Convert.ToDateTime(strArray[1]);
      }
      DataRow[] dataRowArray5 = dataSet1.Tables[0].Select("parameter='booking_hours'");
      if (dataRowArray5.Length > 0)
      {
        string[] strArray = dataRowArray5[0]["value"].ToString().Split('|');
        dateTime3 = Convert.ToDateTime(strArray[0]);
        dateTime4 = Convert.ToDateTime(strArray[1]);
      }
      DataSet dataSet2 = this.assets.view_asset_properties(this.current_user.account_id, new string[4]
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
      Dictionary<long, DateTime> dic_bk_start = new Dictionary<long, DateTime>();
      Dictionary<long, DateTime> dic_bk_end = new Dictionary<long, DateTime>();
      foreach (long bookableRoom in this.bookable_rooms)
      {
        bool flag3 = false;
        if (this.gp.isAdminType)
        {
          flag3 = true;
        }
        else
        {
          DataRow[] dataRowArray6 = assets.Tables[0].Select("asset_id='" + (object) bookableRoom + "'");
          if (dataRowArray6.Length > 0 && dataRowArray6[0]["asset_owner_group_id"].ToString() != "" && !flag3)
            flag3 = this.approvable_rooms.Contains(bookableRoom);
        }
        if (flag3)
        {
          DataRow[] dataRowArray7 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='advance_booking_window'");
          if (dataRowArray7.Length > 0)
            dictionary.Add(bookableRoom, Convert.ToInt32(dataRowArray7[0]["property_value"]));
          else
            dictionary.Add(bookableRoom, num1);
          dic_advance_booking_date.Add(bookableRoom, this.current_timestamp.AddMonths(dictionary[bookableRoom]));
          dic_book_holiday.Add(bookableRoom, true);
          dic_book_weekend.Add(bookableRoom, true);
          dic_op_start.Add(bookableRoom, new DateTime(2000, 1, 1, 0, 0, 0));
          dic_op_end.Add(bookableRoom, new DateTime(2000, 1, 1, 23, 59, 29));
          dic_bk_start.Add(bookableRoom, new DateTime(2000, 1, 1, 0, 0, 0));
          dic_bk_end.Add(bookableRoom, new DateTime(2000, 1, 1, 23, 59, 59));
        }
        else
        {
          DataRow[] dataRowArray8 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='advance_booking_window'");
          if (dataRowArray8.Length > 0)
            dictionary.Add(bookableRoom, Convert.ToInt32(dataRowArray8[0]["property_value"]));
          else
            dictionary.Add(bookableRoom, num1);
          dic_advance_booking_date.Add(bookableRoom, this.current_timestamp.AddMonths(dictionary[bookableRoom]));
          DataRow[] dataRowArray9 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='book_holiday'");
          if (dataRowArray9.Length > 0)
            dic_book_holiday.Add(bookableRoom, Convert.ToBoolean(dataRowArray9[0]["property_value"]));
          else
            dic_book_holiday.Add(bookableRoom, flag1);
          DataRow[] dataRowArray10 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='book_weekend'");
          if (dataRowArray10.Length > 0)
            dic_book_weekend.Add(bookableRoom, Convert.ToBoolean(dataRowArray10[0]["property_value"]));
          else
            dic_book_weekend.Add(bookableRoom, flag2);
          DataRow[] dataRowArray11 = dataSet2.Tables[0].Select("property_name='operating_hours'");
          if (dataRowArray11.Length > 0)
          {
            string[] strArray = dataRowArray11[0]["property_value"].ToString().Split('|');
            dic_op_start.Add(bookableRoom, Convert.ToDateTime(strArray[0]));
            dic_op_end.Add(bookableRoom, Convert.ToDateTime(strArray[1]));
          }
          else
          {
            dic_op_start.Add(bookableRoom, dateTime1);
            dic_op_end.Add(bookableRoom, dateTime2);
          }
          dic_bk_start.Add(bookableRoom, dateTime3);
          dic_bk_end.Add(bookableRoom, dateTime4);
        }
      }
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        List<long> bookable_assets = new List<long>();
        List<long> longList1 = new List<long>();
        DateTime dateTime5 = Convert.ToDateTime(row["book_from"]);
        DateTime dateTime6 = Convert.ToDateTime(row["book_to"]);
        bool boolean1 = Convert.ToBoolean(row["is_weekend"]);
        bool boolean2 = Convert.ToBoolean(row["is_holiday"]);
        string str1 = "";
        if (row["db_booking_id"].ToString() != "0")
          str1 = row["asset_id"].ToString();
        foreach (long asset_id in rooms)
        {
          bool flag4;
          if (this.gp.isAdminType)
          {
            flag4 = true;
          }
          else
          {
            DataRow[] dataRowArray12 = assets.Tables[0].Select("asset_id='" + (object) asset_id + "'");
            if (dataRowArray12.Length > 0 && dataRowArray12[0]["asset_owner_group_id"].ToString() != "")
            {
              flag4 = this.approvable_rooms.Contains(asset_id);
              if (flag4)
                goto label_77;
            }
            flag4 = this.can_book_asset(asset_id, dateTime5, dateTime6, dic_advance_booking_date, dic_book_holiday, dic_book_weekend, boolean1, boolean2, dic_op_start, dic_op_end, dic_bk_start, dic_bk_end);
          }
label_77:
          if (flag4)
            bookable_assets.Add(asset_id);
        }
        List<long> longList2 = this.hdn_change.Value != "" || this.hdn_change.Value != "0" ? this.bookings.check_availability(dateTime5.AddSeconds(1.0), dateTime6.AddSeconds(-1.0), this.current_user.account_id, bookable_assets, this.hdn_change.Value) : this.bookings.check_availability(dateTime5.AddSeconds(1.0), dateTime6.AddSeconds(-1.0), this.current_user.account_id, bookable_assets);
        try
        {
          List<long> longList3 = new List<long>();
          string str2 = "";
          foreach (long num2 in bookable_assets)
          {
            if (!longList2.Contains(num2))
            {
              str2 = str2 + "," + num2.ToString();
              longList3.Add(num2);
            }
          }
          string str3 = str2 + ",-1";
          if (str1 != "" && this.can_book_asset(Convert.ToInt64(str1), dateTime5, dateTime6, dic_advance_booking_date, dic_book_holiday, dic_book_weekend, boolean1, boolean2, dic_op_start, dic_op_end, dic_bk_start, dic_bk_end))
          {
            if (str3 != "")
            {
              if (!longList3.Contains(Convert.ToInt64(str1)))
                str3 = str3 + "," + str1;
            }
            else
              str3 = str3 + "," + str1;
          }
          if (str3.Length > 0)
            str3 = str3.Substring(1).Replace(",-1", "");
          row["asset_options"] = (object) str3;
          this.booking_table.AcceptChanges();
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("check Availablility Inisde(assProDs,htmltable) : Error --> " + ex.ToString()));
        }
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
    Dictionary<long, DateTime> dic_op_end,
    Dictionary<long, DateTime> dic_bk_start,
    Dictionary<long, DateTime> dic_bk_end)
  {
    bool flag = true;
    if (sDT.Hour < dic_bk_start[asset_id].Hour && sDT.Minute < dic_bk_start[asset_id].Minute)
      flag = false;
    else if (sDT.Hour >= dic_bk_end[asset_id].Hour && sDT.Minute >= dic_bk_end[asset_id].Minute)
      flag = false;
    else if (eDT.Hour > dic_bk_end[asset_id].Hour && eDT.Minute > dic_bk_end[asset_id].Minute)
      flag = false;
    else if (sDT > dic_advance_booking_date[asset_id] && eDT > dic_advance_booking_date[asset_id])
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

  private void check_availability(
    Dictionary<string, string> selectedDates,
    DataSet holidays,
    int weekend_option)
  {
    selectedDates = this.do_holiday_filter(selectedDates, holidays, weekend_option);
    this.populate_table(selectedDates, holidays);
    this.get_assets();
    this.populate_dates_table();
  }

  private void get_common_rooms(DataSet room_data)
  {
    this.ddl_common.Items.Clear();
    this.ddl_common.Items.Add(new ListItem("", "0"));
    Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
    Dictionary<long, string> source = new Dictionary<long, string>();
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      string str1 = row["asset_options"].ToString();
      if (str1 != "")
      {
        string str2 = str1;
        char[] chArray = new char[1]{ ',' };
        foreach (string key in str2.Split(chArray))
        {
          if (dictionary1.ContainsKey(key))
            ++dictionary1[key];
          else
            dictionary1.Add(key, 1);
        }
      }
    }
    if (!this.chk_all.Checked)
    {
      foreach (string key in dictionary1.Keys)
      {
        if (dictionary1[key] == this.booking_table.Rows.Count)
        {
          DataRow[] dataRowArray = room_data.Tables[0].Select("asset_id='" + key + "'");
          if (dataRowArray.Length > 0)
          {
            string str = dataRowArray[0]["name"].ToString();
            if (dataRowArray[0]["asset_owner_group_id"].ToString() != "0" && dataRowArray[0]["asset_owner_group_id"].ToString() != "")
            {
              if (!this.approvable_rooms.Contains(Convert.ToInt64(key)))
                str += " (Approval Required)";
            }
            source.Add(Convert.ToInt64(dataRowArray[0]["asset_id"]), str);
          }
        }
      }
    }
    else
    {
      foreach (DataRow row in (InternalDataCollectionBase) room_data.Tables[0].Rows)
      {
        string str = row["name"].ToString();
        if (row["asset_owner_group_id"].ToString() != "0" && row["asset_owner_group_id"].ToString() != "")
        {
          if (!this.approvable_rooms.Contains(Convert.ToInt64(row["asset_id"])))
            str += " (Approval Required)";
        }
        if (dictionary1.ContainsKey(row["asset_id"].ToString()) && dictionary1[row["asset_id"].ToString()] == this.booking_table.Rows.Count)
          str += " - Available on all dates.";
        source.Add(Convert.ToInt64(row["asset_id"]), str);
      }
    }
    Dictionary<long, string> dictionary2 = source.OrderBy<KeyValuePair<long, string>, string>((System.Func<KeyValuePair<long, string>, string>) (pair => pair.Value)).ToDictionary<KeyValuePair<long, string>, long, string>((System.Func<KeyValuePair<long, string>, long>) (pair => pair.Key), (System.Func<KeyValuePair<long, string>, string>) (pair => pair.Value));
    foreach (long key in dictionary2.Keys)
      this.ddl_common.Items.Add(new ListItem(dictionary2[key], key.ToString()));
  }

  private Dictionary<string, string> do_holiday_filter(
    Dictionary<string, string> selecteDates,
    DataSet holiday,
    int weekend_option)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string key in selecteDates.Keys)
    {
      try
      {
        bool flag = true;
        DateTime from = Convert.ToDateTime(key);
        DateTime to = Convert.ToDateTime(selecteDates[key]);
        while (flag)
        {
          DataSet repeatHoliday = this.holidays.get_repeat_holiday(from, to, this.account_id);
          if (this.public_holidays.Tables[0].Select("'" + (object) from + "' >= from_date and '" + (object) to + "' <= to_date").Length > 0)
          {
            switch (weekend_option)
            {
              case 1:
                flag = false;
                continue;
              case 2:
                from = from.AddDays(1.0);
                to = to.AddDays(1.0);
                continue;
              case 3:
                from = from.AddDays(-1.0);
                to = to.AddDays(-1.0);
                continue;
              default:
                continue;
            }
          }
          else if (this.utilities.isValidDataset(repeatHoliday))
          {
            if (repeatHoliday.Tables[0].Rows.Count > 0)
            {
              switch (weekend_option)
              {
                case 1:
                  flag = false;
                  continue;
                case 2:
                  from = from.AddDays(1.0);
                  to = to.AddDays(1.0);
                  continue;
                case 3:
                  from = from.AddDays(-1.0);
                  to = to.AddDays(-1.0);
                  continue;
                default:
                  continue;
              }
            }
          }
          else if (from.DayOfWeek == DayOfWeek.Sunday || from.DayOfWeek == DayOfWeek.Saturday)
          {
            switch (weekend_option)
            {
              case 1:
                flag = false;
                continue;
              case 2:
                if (from.DayOfWeek == DayOfWeek.Saturday)
                {
                  from = from.AddDays(2.0);
                  to = to.AddDays(2.0);
                  if (!dictionary.ContainsKey(from.ToString(api_constants.sql_datetime_format)))
                    dictionary.Add(from.ToString(api_constants.sql_datetime_format), to.ToString(api_constants.sql_datetime_format));
                }
                if (from.DayOfWeek == DayOfWeek.Sunday)
                {
                  from = from.AddDays(1.0);
                  to = to.AddDays(1.0);
                  if (!dictionary.ContainsKey(from.ToString(api_constants.sql_datetime_format)))
                  {
                    dictionary.Add(from.ToString(api_constants.sql_datetime_format), to.ToString(api_constants.sql_datetime_format));
                    continue;
                  }
                  continue;
                }
                continue;
              case 3:
                if (from.DayOfWeek == DayOfWeek.Saturday)
                {
                  from = from.AddDays(-1.0);
                  to = to.AddDays(-1.0);
                  if (!dictionary.ContainsKey(from.ToString(api_constants.sql_datetime_format)))
                    dictionary.Add(from.ToString(api_constants.sql_datetime_format), to.ToString(api_constants.sql_datetime_format));
                }
                if (from.DayOfWeek == DayOfWeek.Sunday)
                {
                  from = from.AddDays(-2.0);
                  to = to.AddDays(-2.0);
                  if (!dictionary.ContainsKey(from.ToString(api_constants.sql_datetime_format)))
                  {
                    dictionary.Add(from.ToString(api_constants.sql_datetime_format), to.ToString(api_constants.sql_datetime_format));
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
        if (from > this.current_timestamp)
        {
          if (!dictionary.ContainsKey(from.ToString(api_constants.sql_datetime_format)))
            dictionary.Add(from.ToString(api_constants.sql_datetime_format), to.ToString(api_constants.sql_datetime_format));
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
      }
    }
    return dictionary;
  }

  protected void btn_specific_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_asset_ids();
      Dictionary<string, string> selectedDates = new Dictionary<string, string>();
      string str1 = Convert.ToDateTime(this.txt_single_date.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_single_start_time.SelectedItem.Text;
      string str2 = Convert.ToDateTime(this.txt_single_to_date.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_single_end_time.SelectedItem.Text;
      DateTime dateTime = Convert.ToDateTime(str1);
      DateTime to = Convert.ToDateTime(str2);
      if (dateTime == to)
        to = dateTime.AddDays(1.0);
      if (dateTime <= this.current_timestamp)
      {
        this.alt_err.Visible = true;
        this.litError.Text = "You cannot select an earlier date and time. Please check your inputs and try again.";
      }
      else
      {
        this.alt_err.Visible = false;
        if (to < dateTime)
        {
          this.alt_err.Visible = true;
          this.litError.Text = "End date/time cannot be earlier than Start date/time. Please correct the dates and try again.";
        }
        else
        {
          this.alt_err.Visible = false;
          this.public_holidays = this.holidays.get_holidays(dateTime, to, this.account_id);
          selectedDates.Add(dateTime.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format));
          this.check_availability(selectedDates, this.public_holidays, 1);
        }
      }
    }
    catch (Exception ex)
    {
      this.alt_err.Visible = true;
      this.litError.Text = "Invalid input. You may have forgotten to input some information that is required to get the dates.";
      fbs_base_page.log.Error((object) ("btn_specific: " + ex.ToString()));
    }
  }

  protected void btn_daily_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_asset_ids();
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string str1 = Convert.ToDateTime(this.txt_daily_start.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_daily_from_time.SelectedItem.Text;
      string str2 = Convert.ToDateTime(this.txt_daily_end.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_daily_to_time.SelectedItem.Text;
      DateTime dateTime = Convert.ToDateTime(str1);
      DateTime to = Convert.ToDateTime(str2);
      if (dateTime == to)
        to = dateTime.AddDays(1.0);
      if (dateTime <= this.current_timestamp)
      {
        this.alt_err.Visible = true;
        this.litError.Text = "You cannot select an earlier date and time. Please check your inputs and try again.";
      }
      else
      {
        this.alt_err.Visible = false;
        int int32 = Convert.ToInt32(this.txt_daily_occurence.Text);
        this.public_holidays = this.holidays.get_holidays(dateTime, to, this.account_id);
        int no_of_events;
        try
        {
          no_of_events = Convert.ToInt32(this.txt_daily_number_of_events.Text);
        }
        catch
        {
          no_of_events = 0;
        }
        this.check_availability(this.bookingsbl.get_daily_dates(dateTime.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format), no_of_events, int32), this.public_holidays, Convert.ToInt32(this.ddl_daily_weekend_option.SelectedItem.Value));
      }
    }
    catch (Exception ex)
    {
      this.alt_err.Visible = true;
      this.litError.Text = "Invalid input. You may have forgotten to input some information that is required to get the dates.";
      fbs_base_page.log.Error((object) ("btn_daily: " + ex.ToString()));
    }
  }

  protected void btn_weekly_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_asset_ids();
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string str1 = Convert.ToDateTime(this.txt_weekly_start.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_weekly_from_time.SelectedItem.Text;
      string str2 = Convert.ToDateTime(this.txt_weekly_end.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_weekly_to_time.SelectedItem.Text;
      DateTime dateTime = Convert.ToDateTime(str1);
      DateTime to = Convert.ToDateTime(str2);
      if (dateTime == to)
        to = dateTime.AddDays(1.0);
      if (dateTime <= this.current_timestamp)
      {
        this.alt_err.Visible = true;
        this.litError.Text = "You cannot select an earlier date and time. Please check your inputs and try again.";
      }
      else
      {
        this.alt_err.Visible = false;
        int int32_1 = Convert.ToInt32(this.ddl_weekly_weekend_option.SelectedItem.Value);
        this.public_holidays = this.holidays.get_holidays(dateTime, to, this.account_id);
        List<int> intList = new List<int>();
        List<int> daysColl = this.getDaysColl();
        int no_of_events;
        try
        {
          no_of_events = Convert.ToInt32(this.txt_weekly_number_of_events.Text);
        }
        catch
        {
          no_of_events = 0;
        }
        int int32_2 = Convert.ToInt32(this.txt_weekly_occurence.Text);
        this.check_availability(this.bookingsbl.get_weekly_dates(dateTime.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format), no_of_events, int32_2, daysColl), this.public_holidays, int32_1);
      }
    }
    catch (Exception ex)
    {
      this.alt_err.Visible = true;
      this.litError.Text = "Invalid input. You may have forgotten to input some information that is required to get the dates.";
      fbs_base_page.log.Error((object) ("btn_weekly: " + ex.ToString()));
    }
  }

  protected void btn_monthly_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_asset_ids();
      Dictionary<string, string> selectedDates = new Dictionary<string, string>();
      string str1 = Convert.ToDateTime(this.txt_monthly_start.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_monthly_from_time.SelectedItem.Text;
      string str2 = Convert.ToDateTime(this.txt_monthly_end.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_monthly_to_time.SelectedItem.Text;
      DateTime dateTime = Convert.ToDateTime(str1);
      DateTime to = Convert.ToDateTime(str2);
      if (dateTime == to)
        to = dateTime.AddDays(1.0);
      if (dateTime <= this.current_timestamp)
      {
        this.alt_err.Visible = true;
        this.litError.Text = "You cannot select an earlier date and time. Please check your inputs and try again.";
      }
      else
      {
        this.alt_err.Visible = false;
        int int32_1 = Convert.ToInt32(this.ddl_monthly_weekend_option.SelectedItem.Value);
        this.public_holidays = this.holidays.get_holidays(dateTime, to, this.account_id);
        int no_of_events;
        try
        {
          no_of_events = Convert.ToInt32(this.txt_monthly_number_of_events.Text);
        }
        catch
        {
          no_of_events = 0;
        }
        if (this.rndMonthlyEvery.Checked)
        {
          int int32_2 = Convert.ToInt32(this.txtMonthlyMonth.Text);
          int int32_3 = Convert.ToInt32(this.txtMonthlyDay.Text);
          selectedDates = this.bookingsbl.get_monthly_every_dates(dateTime.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format), no_of_events, int32_2, int32_3);
        }
        if (this.rndMonthlySpecific.Checked)
        {
          int int32_4 = Convert.ToInt32(this.txt_monthly_frequency.Text);
          int int32_5 = Convert.ToInt32(this.cboMonthlyCount.SelectedItem.Value);
          string text = this.cboMonthlyWeekday.SelectedItem.Text;
          selectedDates = this.bookingsbl.get_monthly_specific_dates(dateTime.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format), no_of_events, int32_4, int32_5, text);
        }
        if (!this.rndMonthlyEvery.Checked && !this.rndMonthlySpecific.Checked)
        {
          this.alt_err.Visible = true;
          this.litError.Text = "Invalid input. You may have forgotten to input some information that is required to get the dates.";
        }
        this.check_availability(selectedDates, this.public_holidays, int32_1);
      }
    }
    catch (Exception ex)
    {
      this.alt_err.Visible = true;
      this.litError.Text = "Invalid input. You may have forgotten to input some information that is required to get the dates.";
      fbs_base_page.log.Error((object) ("btn_monthly: " + ex.ToString()));
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    this.booking_table = (DataTable) this.ViewState["table"];
    this.populate_dates_table();
    try
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        try
        {
          row["asset_id"] = (object) this.Request.Form["ddl_" + row["booking_id"]];
          this.booking_table.AcceptChanges();
          dictionary2.Add(Convert.ToDateTime(row["book_from"]).ToString(api_constants.sql_datetime_format), Convert.ToDateTime(row["book_to"]).ToString(api_constants.sql_datetime_format));
          dictionary1.Add(Convert.ToDateTime(row["book_from"]).ToString(api_constants.sql_datetime_format), row["asset_id"].ToString());
        }
        catch
        {
        }
        if (row["asset_id"].ToString() == "0" || row["asset_id"].ToString() == "" || row["asset_id"] == DBNull.Value)
        {
          this.alt_err.Visible = true;
          this.litError.Text = "Some dates have no rooms available. Please remove those dates and try again.";
          return;
        }
      }
      if (!this.check_local_conflict() || !this.check_server_conflict())
        return;
      string str = Guid.NewGuid().ToString();
      this.Session["data_" + str] = (object) this.booking_table;
      if (dictionary1.Count <= 0)
        return;
      System.Web.UI.ScriptManager.RegisterStartupScript((Page) this, typeof (Page), "MyFun1", "$(function(){advance_booking('" + str + "');});", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Custom Booking Error->", ex);
    }
  }

  private bool check_server_conflict()
  {
    int num = 1;
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      DateTime dateTime1 = Convert.ToDateTime(row["book_from"]);
      DateTime dateTime2 = Convert.ToDateTime(row["book_to"]);
      DataSet dataSet = this.bookings.check_booking_for_asset(this.current_user.account_id, dateTime1.AddSeconds(1.0), dateTime2.AddSeconds(-1.0), Convert.ToInt64(row["asset_id"]));
      if (dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[0].Select("booking_id <> '" + row["db_booking_id"] + "'").Length > 0)
      {
        this.litError.Text = "There is a booking for the selected room in row " + (object) num + ". This could be because someone else booked the room recently at the specified date/time while you were preparing your booking. Kindly change the date and/or time of the affected record and try again. Alternatively, choose a different room.";
        this.alt_err.Visible = true;
        return false;
      }
      ++num;
    }
    return true;
  }

  private bool check_local_conflict()
  {
    int num1 = 1;
    foreach (DataRow row1 in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      DateTime dateTime1 = Convert.ToDateTime(row1["book_from"]);
      DateTime dateTime2 = Convert.ToDateTime(row1["book_to"]);
      int num2 = 1;
      foreach (DataRow row2 in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        if (row1["booking_id"].ToString() != row2["booking_id"].ToString())
        {
          DateTime dateTime3 = Convert.ToDateTime(row2["book_from"]);
          DateTime dateTime4 = Convert.ToDateTime(row2["book_to"]);
          if ((dateTime3 >= dateTime1 && dateTime3 <= dateTime2 || dateTime4 >= dateTime1 && dateTime4 <= dateTime2) && row1["asset_id"].ToString() == row2["asset_id"].ToString())
          {
            this.litError.Text = "There is a room conflict between rows " + (object) num1 + " and " + (object) num2 + ". If you need to create two records with the same date and time, then kindly choose different rooms. If not, change the dates/times on one of the records.";
            this.alt_err.Visible = true;
            return false;
          }
        }
        ++num2;
      }
      ++num1;
    }
    return true;
  }

  protected void btn_change_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_asset_ids();
      Dictionary<string, string> selecteDates = new Dictionary<string, string>();
      string str1 = Convert.ToDateTime(this.txt_change_from_date.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_change_from_time.SelectedItem.Text;
      string str2 = Convert.ToDateTime(this.txt_change_to_date.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_change_to_time.SelectedItem.Text;
      DateTime dateTime = Convert.ToDateTime(str1);
      DateTime to = Convert.ToDateTime(str2);
      if (dateTime == to)
        to = dateTime.AddDays(1.0);
      if (dateTime <= this.current_timestamp)
      {
        this.alt_err.Visible = true;
        this.litError.Text = "You cannot select an earlier date and time. Please check your inputs and try again.";
      }
      else
      {
        this.alt_err.Visible = false;
        if (dateTime > to)
        {
          this.alt_err.Visible = true;
          this.litError.Text = "You cannot select an earlier date and time. Please check your inputs and try again.";
        }
        else
        {
          this.booking_table = (DataTable) this.ViewState["table"];
          foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
          {
            if (row["booking_id"].ToString() == this.hdn_change.Value)
            {
              row["book_from"] = (object) dateTime;
              row["book_to"] = (object) to;
              this.public_holidays = this.holidays.get_holidays(dateTime, to, this.account_id);
              selecteDates.Add(dateTime.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format));
              this.do_holiday_filter(selecteDates, this.public_holidays, 1);
              DataRow[] dataRowArray = this.public_holidays.Tables[0].Select("'" + (object) dateTime + "' >= from_date and '" + (object) to + "' <= to_date");
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
              row["is_weekend"] = dateTime.DayOfWeek == DayOfWeek.Sunday || to.DayOfWeek == DayOfWeek.Saturday ? (object) true : (object) false;
              this.booking_table.AcceptChanges();
              break;
            }
          }
          this.ViewState["table"] = (object) this.booking_table;
          this.get_assets();
          this.populate_dates_table();
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("btn_specific: " + ex.ToString()));
      this.html_error = "<div class='alr a-err'><strong> Error!</strong>The filtering criteria provided is incorrect.</div>";
    }
  }

  private void save_asset_ids()
  {
    this.booking_table = (DataTable) this.ViewState["table"];
    foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      try
      {
        row["asset_id"] = (object) this.Request.Form["ddl_" + row["booking_id"]];
        this.booking_table.AcceptChanges();
      }
      catch
      {
      }
    }
    this.ViewState["table"] = (object) this.booking_table;
  }

  protected void btn_check_availability_Click(object sender, EventArgs e)
  {
    this.booking_table = (DataTable) this.ViewState["table"];
    this.get_assets();
    this.populate_dates_table();
  }

  protected void chk_all_CheckedChanged(object sender, EventArgs e)
  {
    this.booking_table = (DataTable) this.ViewState["table"];
    this.get_assets();
    this.populate_dates_table();
    this.get_common_rooms(this.assets.view_assets(this.current_user.account_id));
  }

  protected void btn_bulk_change_time_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_asset_ids();
      Dictionary<string, string> selecteDates = new Dictionary<string, string>();
      DateTime dateTime1 = Convert.ToDateTime("01-Jan-2000 " + this.ddl_o_from.SelectedItem.Value);
      DateTime dateTime2 = Convert.ToDateTime("01-Jan-2000 " + this.ddl_o_to.SelectedItem.Value);
      DateTime dateTime3 = Convert.ToDateTime("01-Jan-2000 " + this.ddl_n_from.SelectedItem.Value);
      DateTime dateTime4 = Convert.ToDateTime("01-Jan-2000 " + this.ddl_n_to.SelectedItem.Value);
      this.booking_table = (DataTable) this.ViewState["table"];
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        DateTime from = Convert.ToDateTime(row["book_from"]);
        DateTime to = Convert.ToDateTime(row["book_to"]);
        if (from >= this.current_timestamp && from.ToString("hh:mm tt") == dateTime1.ToString("hh:mm tt") && to.ToString("hh:mm tt") == dateTime2.ToString("hh:mm tt"))
        {
          from = new DateTime(from.Year, from.Month, from.Day, dateTime3.Hour, dateTime3.Minute, 0);
          to = new DateTime(to.Year, to.Month, to.Day, dateTime4.Hour, dateTime4.Minute, 0);
          row["book_from"] = (object) from;
          row["book_to"] = (object) to;
          this.public_holidays = this.holidays.get_holidays(from, to, this.account_id);
          selecteDates.Add(from.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format));
          selecteDates = this.do_holiday_filter(selecteDates, this.public_holidays, 1);
          DataRow[] dataRowArray = this.public_holidays.Tables[0].Select("'" + (object) from + "' >= from_date and '" + (object) to + "' <= to_date");
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
          row["is_weekend"] = from.DayOfWeek == DayOfWeek.Sunday || to.DayOfWeek == DayOfWeek.Saturday ? (object) true : (object) false;
          this.booking_table.AcceptChanges();
        }
      }
      this.ViewState["table"] = (object) this.booking_table;
      this.get_assets();
      this.populate_dates_table();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("btn_specific: " + ex.ToString()));
      this.html_error = "<div class='alr a-err'><strong> Error!</strong>The filtering criteria provided is incorrect.</div>";
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
