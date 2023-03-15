// Decompiled with JetBrains decompiler
// Type: booking_wizard
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class booking_wizard : fbs_base_page, IRequiresSessionState
{
  public string htmltable;
  public DataSet setting_data;
  private string startDateTime;
  private string endDateTime;
  public string holiday = "";
  protected HiddenField hdnBookingWindow;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Literal litIsBook;
  protected HtmlGenericControl alertIsBook;
  protected HtmlGenericControl divHolidays;
  protected DropDownList ddl_category;
  protected CheckBox chkAllDay;
  protected TextBox txt_startDate;
  protected DropDownList ddl_StartTime;
  protected TextBox txt_EndDate;
  protected DropDownList ddl_EndTime;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected TextBox txt_capacity;
  protected CheckBox chk_fav;
  protected Button btn_check_availability;
  protected HtmlGenericControl footer_msg;
  protected Literal litSubmit;
  protected Button btn_submit;
  protected HtmlGenericControl divAction;
  protected HiddenField HiddenStart;
  protected HiddenField HiddenEnd;
  protected HiddenField hdf_redirect;
  protected HiddenField hdnCloneID;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.Session["SelectedDates"] != null)
      this.Session.Remove("SelectedDates");
    this.setting_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
    if (this.setting_data == null)
    {
      this.setting_data = this.settings.view_settings(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.setting_data);
    }
    try
    {
      if (this.u_group.group_type == 0)
        this.redirect_unauthorized();
      try
      {
        this.hdnCloneID.Value = this.Request.QueryString["cloneID"];
      }
      catch
      {
        this.hdnCloneID.Value = "";
      }
      if (this.hdnCloneID.Value != "")
        this.Session.Add("CloneID", (object) this.hdnCloneID.Value);
      if (!this.IsPostBack)
      {
        this.txt_startDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        this.txt_EndDate.Text = this.txt_startDate.Text;
        this.utilities.populate_dropdown(this.setting_data, this.ddl_building, "building");
        this.utilities.populate_dropdown(this.setting_data, this.ddl_category, "category");
        this.utilities.populate_dropdown(this.setting_data, this.ddl_level, "level");
        this.utilities.Populate_Time(this.ddl_StartTime, this.current_timestamp);
        this.utilities.Populate_Time(this.ddl_EndTime, this.current_timestamp);
        if (Convert.ToInt32(this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh")) <= 9)
          this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("h:mm tt");
        else
          this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh:mm tt");
        if (Convert.ToInt32(this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh")) <= 9)
          this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("h:mm tt");
        else
          this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh:mm tt");
        this.htmltable = "<h5>Please select a date range and filter criteria and click on 'Check Availability' to load the available rooms.</h5>";
      }
      else
      {
        this.startDateTime = DateTime.Today.ToShortDateString() + "  " + this.ddl_StartTime.SelectedItem.Text;
        this.endDateTime = DateTime.Today.ToShortDateString() + "  " + this.ddl_EndTime.SelectedItem.Text;
        this.HiddenStart.Value = this.startDateTime;
        this.HiddenEnd.Value = this.endDateTime;
      }
      this.check_public_holiday();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_check_Click(object sender, EventArgs e)
  {
    try
    {
      long ticks = this.current_timestamp.Ticks;
      this.Check_Availability();
      TimeSpan timeSpan = new TimeSpan(this.current_timestamp.Ticks - ticks);
      fbs_base_page.log.Info((object) ("booking wizard - check availability time taken: " + timeSpan.TotalMilliseconds.ToString()));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  private void Check_Availability()
  {
    try
    {
      if (string.IsNullOrEmpty(this.txt_startDate.Text))
      {
        this.txt_startDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        this.startDateTime = Convert.ToDateTime(this.txt_startDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_StartTime.SelectedItem.Text;
      }
      else
        this.startDateTime = Convert.ToDateTime(this.txt_startDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_StartTime.SelectedItem.Text;
      if (string.IsNullOrEmpty(this.txt_EndDate.Text))
      {
        this.txt_EndDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        this.endDateTime = Convert.ToDateTime(this.txt_EndDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_EndTime.SelectedItem.Text;
      }
      else
        this.endDateTime = Convert.ToDateTime(this.txt_EndDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_EndTime.SelectedItem.Text;
      DateTime from = Convert.ToDateTime(this.startDateTime).AddMinutes(2.0);
      DateTime to = Convert.ToDateTime(this.endDateTime).AddMinutes(-2.0);
      new Dictionary<string, string>()
      {
        {
          from.ToString(api_constants.datetime_format),
          to.ToString(api_constants.datetime_format)
        }
      };
      if (this.is_blacklisted)
      {
        this.alertError.Attributes.Add("style", "display: block;");
        this.litErrorMsg.Text = Resources.fbs.blacklist_user_booking_msg;
      }
      else
      {
        this.alertError.Attributes.Add("style", "display: none;");
        this.HiddenStart.Value = this.startDateTime;
        this.HiddenEnd.Value = this.endDateTime;
        string building = "";
        if (this.ddl_building.SelectedItem.Text != "All")
          building = this.ddl_building.SelectedItem.Value;
        string category = "";
        if (this.ddl_category.SelectedItem.Text != "All")
          category = this.ddl_category.SelectedItem.Value;
        string level = "";
        if (this.ddl_level.SelectedItem.Text != "All")
          level = this.ddl_level.SelectedItem.Value;
        string str1 = this.validateInput();
        if (str1 != "")
        {
          this.litErrorMsg.Text = str1;
          this.alertError.Attributes.Add("style", "display:block;");
          this.bind_Empty_data();
        }
        else
        {
          Dictionary<long, asset> available_assets = new Dictionary<long, asset>();
          this.alertError.Attributes.Add("style", "display:none;");
          bool isAdmin = false;
          if (this.u_group.group_type != 0 && this.u_group.group_type != 3)
          {
            if (this.u_group.group_type == 1)
              isAdmin = true;
            long groupId = this.u_group.group_id;
          }
          if (building == "")
            building = "0";
          if (category == "")
            category = "0";
          if (level == "")
            level = "0";
          string capacity = !(this.txt_capacity.Text == "") ? this.txt_capacity.Text : "0";
          DataSet dataSet1 = new DataSet();
          DataSet dataSet2 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_ass_prop");
          if (dataSet2 == null)
          {
            dataSet2 = this.assets.view_asset_properties(this.current_user.account_id);
            this.capi.set_cache(this.current_user.account_id.ToString() + "_ass_prop", (object) dataSet2);
          }
          try
          {
            string groupIds = this.utilities.get_group_ids(this.current_user);
            string group_ids = string.IsNullOrEmpty(groupIds) ? "0" : groupIds;
            available_assets = this.bookings.check_available_assets(from.ToString(api_constants.datetime_format), to.ToString(api_constants.datetime_format), this.current_user.account_id, building, category, level, capacity, group_ids, isAdmin, dataSet2, this.setting_data, this.current_timestamp);
            if (available_assets.Count <= 0)
              this.btn_submit.Visible = false;
            else
              this.btn_submit.Visible = true;
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) ("check Availablility Inisde(available_assets) : Error --> " + ex.ToString()));
          }
          try
          {
            Dictionary<long, asset> av_assets1 = this.bookingsbl.filter_assets(this.assets.filter_favourites(available_assets, (DataSet) this.Session["favourites"], this.chk_fav.Checked), this.setting_data, dataSet2, from, to);
            Dictionary<long, asset> av_assets2 = new Dictionary<long, asset>();
            if (!this.gp.isAdminType)
            {
              foreach (long key in av_assets1.Keys)
              {
                try
                {
                  DataRow[] dataRowArray1 = dataSet2.Tables[0].Select("asset_id=" + key.ToString() + " and property_name='booking_lead_time'");
                  DataRow[] dataRowArray2 = dataSet2.Tables[0].Select("asset_id=" + key.ToString() + " and property_name='book_weekend'");
                  DataRow[] dataRowArray3 = dataSet2.Tables[0].Select("asset_id=" + key.ToString() + " and property_name='book_holiday'");
                  if (dataRowArray1.Length > 0)
                  {
                    string str2 = dataRowArray1[0].ItemArray[2].ToString();
                    if (str2 != "")
                    {
                      if (str2.Split(' ')[0] != "0")
                      {
                        DateTime dateTime1 = new DateTime();
                        DateTime dateTime2;
                        if (str2.Split(' ')[1] == "Hour(s)")
                        {
                          dateTime2 = DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(Convert.ToDouble(str2.Split(' ')[0]));
                        }
                        else
                        {
                          int num1 = 0;
                          int num2 = 0;
                          while (true)
                          {
                            if ((long) num2 < Convert.ToInt64(str2.Split(' ')[0]) + 1L)
                            {
                              ++num1;
                              if (DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 1)).DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 1)).DayOfWeek == DayOfWeek.Sunday)
                              {
                                if (dataRowArray2.Length > 0)
                                {
                                  if (dataRowArray2[0].ItemArray[2].ToString() == "False")
                                    ++num1;
                                }
                                else
                                  ++num1;
                              }
                              if (dataRowArray3.Length > 0)
                              {
                                if (dataRowArray3[0].ItemArray[2].ToString() == "False")
                                {
                                  DataSet ds = this.holidays.view_holidays(DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 1)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 2)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", this.account_id);
                                  if (this.utilities.isValidDataset(ds) && ds.Tables[0].Rows.Count > 0)
                                    ++num1;
                                }
                              }
                              else
                                ++num1;
                              ++num2;
                            }
                            else
                              break;
                          }
                          dateTime2 = DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) num1);
                        }
                        if (from > dateTime2)
                          av_assets2.Add(key, av_assets1[key]);
                      }
                    }
                  }
                  else
                    av_assets2.Add(key, av_assets1[key]);
                }
                catch (Exception ex)
                {
                  fbs_base_page.log.Error((object) ex.ToString());
                }
              }
            }
            this.htmltable = this.gp.isAdminType ? this.bookingsbl.getAssetHtml(av_assets1, this.setting_data, dataSet2) : this.bookingsbl.getAssetHtml(av_assets2, this.setting_data, dataSet2);
            this.footer_msg.Visible = true;
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) ("check Availablility Inisde(assProDs,htmltable) : Error --> " + ex.ToString()));
          }
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("check Availablility Inisde : Error --> " + ex.ToString()));
    }
  }

  protected void ddlStartTime_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      string[] strArray = this.ddl_StartTime.SelectedItem.Text.Split(':');
      int int16 = (int) Convert.ToInt16(strArray[0]);
      if (int16 == 11)
      {
        if (strArray[1].IndexOf("AM") > 0)
        {
          strArray[1] = strArray[1].Replace("AM", "PM");
          ++int16;
        }
      }
      else
        ++int16;
      if (int16 > 12)
        int16 -= 12;
      this.ddl_EndTime.SelectedItem.Text = (Convert.ToInt32(int16) >= 10 ? (object) int16.ToString() : (object) ("0" + (object) int16)).ToString() + ":" + strArray[1];
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void txt_startDate_TextChanged(object sender, EventArgs e)
  {
    try
    {
      if (this.chkAllDay.Checked)
      {
        if (this.txt_startDate.Text != this.current_timestamp.ToString(api_constants.display_datetime_format_short))
        {
          this.ddl_StartTime.ClearSelection();
          this.ddl_StartTime.Items.FindByValue("12:00 AM").Selected = true;
        }
        this.ddl_StartTime.Enabled = false;
        this.txt_EndDate.Text = Convert.ToDateTime(this.txt_startDate.Text).AddDays(1.0).ToString(api_constants.display_datetime_format_short);
        this.ddl_EndTime.ClearSelection();
        this.ddl_EndTime.Items.FindByValue("12:00 AM").Selected = true;
        this.ddl_EndTime.Enabled = false;
      }
      else
      {
        this.txt_EndDate.Text = this.txt_startDate.Text;
        this.check_public_holiday();
      }
      this.btn_submit.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void txt_endDate_TextChanged(object sender, EventArgs e)
  {
    try
    {
      this.check_public_holiday();
      this.btn_submit.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  private void check_public_holiday()
  {
    try
    {
      this.holiday = this.bookingsbl.getPublicHolidayList(Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.datetime_format), Convert.ToDateTime(this.txt_EndDate.Text).AddDays(1.0).AddSeconds(-1.0).ToString(api_constants.datetime_format), this.current_user.account_id);
      if (!string.IsNullOrEmpty(this.holiday))
        this.divHolidays.Attributes.Add("style", "display: block;");
      else
        this.divHolidays.Attributes.Add("style", "display: none;");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  private string validateInput()
  {
    try
    {
      if (Convert.ToDateTime(this.startDateTime) < this.current_timestamp || Convert.ToDateTime(this.endDateTime) < this.current_timestamp)
        return Resources.fbs.booking_wizard_select_futuerdate;
      if (Convert.ToDateTime(this.endDateTime) < Convert.ToDateTime(this.startDateTime))
        return Resources.fbs.booking_wizard_start_end_datecheck;
      if (Convert.ToDateTime(this.endDateTime).Date == Convert.ToDateTime(this.startDateTime).Date && this.ddl_StartTime.SelectedItem.Text == this.ddl_EndTime.SelectedItem.Text)
        return Resources.fbs.booking_wizard_enddate_time_check;
      DateTime dateTime = this.current_timestamp.AddMonths(Convert.ToInt32(this.hdnBookingWindow.Value.ToString())).AddDays(-1.0);
      if (Convert.ToDateTime(Convert.ToDateTime(this.txt_startDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short)) > dateTime)
        return Resources.fbs.booking_wizard_cannotbook + " " + this.txt_startDate.Text + ".";
      if (Convert.ToDateTime(Convert.ToDateTime(this.txt_EndDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short)) > dateTime)
        return Resources.fbs.booking_wizard_cannotbook + " " + this.txt_EndDate.Text + ".";
    }
    catch
    {
    }
    return "";
  }

  public void bind_Empty_data()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='available_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:8px;'></th>");
      stringBuilder.Append("<th class='hidden-480'>Code/ Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Level</th>");
      stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
      stringBuilder.Append("<th class='hidden-480'>Category</th>");
      stringBuilder.Append("<th class='hidden-480'>Type</th>");
      stringBuilder.Append("<th class='hidden-480'>Comments</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.htmltable = stringBuilder.ToString();
      this.btn_submit.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void chkAllDay_CheckedChanged(object sender, EventArgs e)
  {
    try
    {
      if (this.chkAllDay.Checked)
      {
        if (this.txt_startDate.Text != this.current_timestamp.ToString(api_constants.display_datetime_format_short))
        {
          this.ddl_StartTime.ClearSelection();
          this.ddl_StartTime.Items.FindByValue("12:00 AM").Selected = true;
        }
        this.ddl_StartTime.Enabled = false;
        this.txt_EndDate.Text = Convert.ToDateTime(this.txt_startDate.Text).AddDays(1.0).ToString(api_constants.display_datetime_format_short);
        this.ddl_EndTime.ClearSelection();
        this.ddl_EndTime.Items.FindByValue("12:00 AM").Selected = true;
        this.ddl_EndTime.Enabled = false;
      }
      else
      {
        this.txt_startDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        this.txt_EndDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        if (Convert.ToInt32(this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh")) <= 9)
          this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh:mm tt");
        else
          this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh:mm tt");
        if (Convert.ToInt32(this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh")) <= 9)
          this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh:mm tt");
        else
          this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh:mm tt");
        this.txt_EndDate.Enabled = true;
        this.ddl_StartTime.Enabled = true;
        this.ddl_EndTime.Enabled = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
