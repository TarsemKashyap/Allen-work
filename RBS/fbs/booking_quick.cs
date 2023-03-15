// Decompiled with JetBrains decompiler
// Type: booking_quick
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using ASP.App_Code.Data;
using DayPilot.Web.Ui;
using DayPilot.Web.Ui.Enums;
using DayPilot.Web.Ui.Enums.Scheduler;
using DayPilot.Web.Ui.Events;
using DayPilot.Web.Ui.Events.Scheduler;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class booking_quick : fbs_base_page, IRequiresSessionState
{
  private DataTable table;
  public DataSet setting_data;
  public DataSet asset_properties_data;
  public DataSet asset_data;
  public string holiday;
  private DataRow[] bookingRows;
  private Dictionary<string, DateTime> op_start;
  private Dictionary<string, DateTime> op_end;
  private Dictionary<string, DateTime> lead_times = new Dictionary<string, DateTime>();
  protected HiddenField hdnBookingWindow;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Literal litIsBook;
  protected HtmlGenericControl alertIsBook;
  protected HtmlGenericControl divHolidays;
  protected TextBox txt_startDate;
  protected DropDownList count_select;
  protected TextBox txt_search;
  protected Button btn_search;
  protected Button btn_clear_search;
  protected DayPilotScheduler DayPilotScheduler1;
  protected DayPilotBubble DayPilotBubble1;
  protected Label lbl_count;
  protected HtmlGenericControl daypilot_pagination;
  protected HiddenField hdn_ph;
  protected HiddenField hdn_we;
  protected HiddenField save_count;
  protected HiddenField skip_count;
  protected HiddenField total_count;
  protected HiddenField hdn_capacity;
  protected HiddenField hdn_building;
  protected HiddenField hdn_level;
  protected HiddenField hdn_start;
  protected HiddenField hdn_room;
  protected HiddenField hdn_asset_ids;
  protected DropDownList ddl_category;
  protected TextBox txt_capacity;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected CheckBox chk_fav;
  protected Button btn_submit;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      DateTime location = DateTime.UtcNow.AddHours(this.current_account.timezone);
      if (this.Session["SelectedDates"] != null)
        this.Session.Remove("SelectedDates");
      try
      {
        if (this.u_group.group_type == 0)
          this.redirect_unauthorized();
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("Error at checking user group Booking.quick.aspx-->>>" + ex.ToString()));
      }
      this.setting_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (this.setting_data == null)
      {
        this.setting_data = this.settings.view_settings(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.setting_data);
      }
      DateTime dateTime = new DateTime(2000, 1, 1);
      try
      {
        dateTime = Convert.ToDateTime(this.Request.QueryString["d"]);
      }
      catch
      {
        dateTime = new DateTime(2000, 1, 1);
      }
      try
      {
        this.DayPilotScheduler1.StartDate = dateTime.Year <= 2000 ? new DateTime(location.Year, location.Month, location.Day) : dateTime;
        this.initData();
        this.DayPilotScheduler1.Separators.Add(location, Color.Red);
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("Error at daypilot cheduler Init Booking.quick.aspx-->>>" + ex.ToString()));
      }
      if (!this.IsPostBack)
      {
        this.op_start = new Dictionary<string, DateTime>();
        this.op_end = new Dictionary<string, DateTime>();
        if (!string.IsNullOrWhiteSpace(this.Request.QueryString["cap"]))
        {
          this.txt_capacity.Text = this.Request.QueryString["cap"].ToString();
          this.hdn_capacity.Value = this.Request.QueryString["cap"].ToString();
        }
        try
        {
          this.populate_count_dropdown();
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("populat dropdown booking.aspx-->>>" + ex.ToString()));
        }
        this.txt_startDate.Text = location.ToString(api_constants.display_datetime_format_short);
        this.utilities.populate_dropdown(this.setting_data, this.ddl_building, "building");
        this.utilities.populate_dropdown(this.setting_data, this.ddl_category, "category");
        this.utilities.populate_dropdown(this.setting_data, this.ddl_level, "level");
        if (!string.IsNullOrEmpty(this.Request.QueryString["BUI"]) && this.Request.QueryString["BUI"].ToString() != "ALL")
        {
          this.ddl_building.Items.FindByValue(this.Request.QueryString["BUI"].ToString()).Selected = true;
          this.hdn_building.Value = this.Request.QueryString["BUI"].ToString();
        }
        if (!string.IsNullOrEmpty(this.Request.QueryString["LEV"]) && this.Request.QueryString["LEV"].ToString() != "ALL")
        {
          this.ddl_level.Items.FindByValue(this.Request.QueryString["LEV"].ToString()).Selected = true;
          this.hdn_level.Value = this.Request.QueryString["LEV"].ToString();
        }
        if (!string.IsNullOrEmpty(this.Request.QueryString["DAT"]))
          this.txt_startDate.Text = this.Request.QueryString["DAT"].ToString();
        if (!string.IsNullOrEmpty(this.Request.QueryString["d"]))
          this.txt_startDate.Text = Convert.ToDateTime(this.Request.QueryString["d"]).ToString("dd-MMM-yyyy");
        if (!string.IsNullOrEmpty(this.Request.QueryString["ROOM"]) && this.Request.QueryString["ROOM"].ToString() != "ALL")
        {
          this.ddl_category.Items.FindByValue(this.Request.QueryString["ROOM"].ToString()).Selected = true;
          this.hdn_room.Value = this.Request.QueryString["ROOM"].ToString();
        }
        this.populate_timeline(Convert.ToDateTime(this.txt_startDate.Text));
        this.DayPilotScheduler1.SetScrollX(this.utilities.TimeRoundUp(this.current_timestamp));
        try
        {
          this.rebind();
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("Error at setDataSourceAndBind Booking.quick.aspx-->>>" + ex.ToString()));
        }
        this.DayPilotScheduler1.Update();
      }
      else
        this.populate_timeline(Convert.ToDateTime(this.txt_startDate.Text));
      this.ViewState.Remove("bds");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  private void populate_timeline(DateTime date)
  {
    if (this.gp.isAdminType)
      return;
    setting setting = this.settings.get_setting("booking_hours", this.current_user.account_id);
    int int32 = Convert.ToInt32(this.settings.get_setting("booking_slot", this.current_user.account_id).value);
    int num = 1440 / int32;
    string[] strArray = setting.value.Split('|');
    DateTime dateTime1 = Convert.ToDateTime(strArray[0]);
    DateTime dateTime2 = Convert.ToDateTime(strArray[1]);
    this.DayPilotScheduler1.Scale = TimeScale.Manual;
    this.DayPilotScheduler1.Timeline.Clear();
    DateTime dateTime3 = date;
    dateTime3 = new DateTime(dateTime3.Year, dateTime3.Month, dateTime3.Day);
    for (int index = 0; index < num; ++index)
    {
      DateTime start = dateTime3.AddMinutes((double) (index * int32));
      DateTime end = dateTime3.AddMinutes((double) ((index + 1) * int32));
      if (start.Hour >= dateTime1.Hour && start.Hour < dateTime2.Hour)
        this.DayPilotScheduler1.Timeline.Add(start, end);
    }
  }

  private void setDataSourceAndBind()
  {
    try
    {
      long building = 0;
      long category = 0;
      long level = 0;
      long capacity = 0;
      if (this.ddl_building.SelectedItem.Text != "All")
        building = Convert.ToInt64(this.ddl_building.SelectedItem.Value);
      if (this.ddl_category.SelectedItem.Text != "All")
        category = Convert.ToInt64(this.ddl_category.SelectedItem.Value);
      if (this.ddl_level.SelectedItem.Text != "All")
        level = Convert.ToInt64(this.ddl_level.SelectedItem.Value);
      if (!string.IsNullOrEmpty(this.txt_capacity.Text))
        capacity = Convert.ToInt64(this.txt_capacity.Text);
      if (!this.IsPostBack)
        this.loadResources(level, category, building, capacity);
      string filter = (string) this.DayPilotScheduler1.ClientState["filter"];
      DataTable data;
      if (this.txt_startDate.Text != "")
      {
        fbs_base_page.log.Info((object) "getData 1");
        data = this.getData(Convert.ToDateTime(this.txt_startDate.Text + " 00:00:00"), Convert.ToDateTime(this.txt_startDate.Text + " 23:59:59"), filter);
      }
      else
      {
        fbs_base_page.log.Info((object) "getData 2");
        data = this.getData(this.DayPilotScheduler1.StartDate, this.DayPilotScheduler1.EndDate, filter);
      }
      this.DayPilotScheduler1.DataSource = (object) data;
      this.DayPilotScheduler1.DataBind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error at setDataSourceAndBind(Inside) Booking.quick.aspx-->>>" + ex.ToString()));
    }
  }

  private void populate_count_dropdown()
  {
    try
    {
      this.count_select.Items.Add("25");
      this.count_select.Items.Add("50");
      this.count_select.Items.Add("100");
      this.count_select.Items.Add("ALL");
      if (this.Session["selected_index"] != null)
        this.count_select.SelectedIndex = this.count_select.Items.IndexOf(this.count_select.Items.FindByText(this.Session["selected_index"].ToString()));
      else
        this.count_select.SelectedIndex = 0;
      if (this.Request.QueryString["count"] != null)
      {
        if (this.Session["txt_search"] == null)
          return;
        this.txt_search.Text = this.Session["txt_search"].ToString();
        this.Session.Remove("txt_search");
      }
      else
        this.Session.Remove("txt_search");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void initData()
  {
    try
    {
      if (!string.IsNullOrEmpty(this.Request.QueryString["DAT"]))
        this.txt_startDate.Text = this.Request.QueryString["DAT"].ToString();
      if (!string.IsNullOrEmpty(this.Request.QueryString["d"]))
        this.txt_startDate.Text = Convert.ToDateTime(this.Request.QueryString["d"]).ToString("dd-MMM-yyyy");
      DateTime dateTime1 = this.DayPilotScheduler1.StartDate;
      DateTime dateTime2 = dateTime1.AddDays(1.0).AddSeconds(-1.0);
      if (this.txt_startDate.Text != "")
      {
        dateTime1 = Convert.ToDateTime(Convert.ToDateTime(this.txt_startDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000");
        dateTime2 = dateTime1.AddDays(1.0).AddSeconds(-1.0);
      }
      this.table = (DataTable) this.ViewState["bds"];
      if (this.table != null)
        return;
      DataSet dataSet = new DataSet();
      DataSet quickbookings = this.bookings.get_Quickbookings(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format), this.current_user.account_id);
      if (!this.utilities.isValidDataset(quickbookings))
        return;
      DataRow[] source = quickbookings.Tables[0].Select("status = 1 or status = 4 or status=2 ");
      this.table = source.Length <= 0 ? quickbookings.Tables[0].Clone() : ((IEnumerable<DataRow>) source).CopyToDataTable<DataRow>();
      this.Session[this.PageHash] = (object) source;
      this.ViewState.Add("bds", (object) this.table);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error at daypilot cheduler Init(Inside) Booking.quick.aspx-->>>" + ex.ToString()));
    }
  }

  protected string PageHash => Hash.ForPage((Page) this);

  private DataTable getData(DateTime start, DateTime end, string filter)
  {
    DataTable data = new DataTable();
    try
    {
      string filterExpression = !string.IsNullOrEmpty(filter) ? "name like '%" + filter + "%'" : "";
      if (!(filterExpression != ""))
        return this.table;
      DataRow[] source = this.table.Select(filterExpression);
      if (source.Length <= 0)
        return data;
      data = ((IEnumerable<DataRow>) source).CopyToDataTable<DataRow>();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("get data(Inside) : " + ex.ToString() + ". start: " + (object) start + ". end: " + (object) end + "."));
    }
    return data;
  }

  private int process_count()
  {
    int num1 = 0;
    int num2 = 0;
    if (this.save_count.Value == "")
    {
      this.save_count.Value = this.count_select.SelectedItem.Text;
      if (this.Request.QueryString["count"] != null)
      {
        if (this.Request.QueryString["count"].ToString() != "")
        {
          if (this.Request.QueryString["count"].ToString() == "nxt")
          {
            if (this.Session["skip"] != null)
            {
              this.skip_count.Value = this.Session["skip"].ToString();
              num1 = Convert.ToInt32(this.skip_count.Value) / Convert.ToInt32(this.count_select.SelectedItem.Text);
              if (num1 == 0)
                num1 = 0;
            }
            if (this.save_count.Value == "")
            {
              this.save_count.Value = this.count_select.SelectedItem.Text;
              this.skip_count.Value = "0";
            }
            else
            {
              Convert.ToInt32(this.save_count.Value);
              if (this.Session["skip"] != null)
              {
                if (this.Session["total_count"] != null)
                  num2 = Convert.ToInt32(this.Session["total_count"].ToString());
                if (Convert.ToInt32(this.skip_count.Value) <= num2)
                {
                  this.skip_count.Value = Convert.ToString(Convert.ToInt32(this.Session["skip"].ToString()) + Convert.ToInt32(this.save_count.Value));
                  num1 = Convert.ToInt32(this.skip_count.Value) / Convert.ToInt32(this.count_select.SelectedItem.Text);
                  if (num1 == 0)
                    num1 = 0;
                }
              }
            }
          }
          else if (this.Request.QueryString["count"].ToString() == "prev")
          {
            this.save_count.Value = this.count_select.SelectedItem.Text;
            if (this.Session["skip"] != null)
            {
              if (Convert.ToInt32(this.Session["skip"].ToString()) > 1)
              {
                this.skip_count.Value = Convert.ToString(Convert.ToInt32(this.Session["skip"].ToString()) - Convert.ToInt32(this.save_count.Value));
                num1 = Convert.ToInt32(this.skip_count.Value) / Convert.ToInt32(this.count_select.SelectedItem.Text);
                if (num1 == 0)
                  num1 = 0;
              }
              else
                this.skip_count.Value = "0";
            }
          }
          else if (Convert.ToInt32(this.Request.QueryString["count"].ToString()) > 1)
          {
            this.skip_count.Value = Convert.ToString(Convert.ToInt32(this.Request.QueryString["count"].ToString()) * Convert.ToInt32(this.save_count.Value) - Convert.ToInt32(this.save_count.Value));
            num1 = Convert.ToInt32(this.skip_count.Value) / Convert.ToInt32(this.count_select.SelectedItem.Text);
            if (num1 == 0)
              num1 = 0;
          }
          else
          {
            this.skip_count.Value = Convert.ToString(Convert.ToInt32(this.Request.QueryString["count"].ToString()) - 1);
            num1 = Convert.ToInt32(this.skip_count.Value) / Convert.ToInt32(this.count_select.SelectedItem.Text);
            if (num1 == 0)
              num1 = 0;
          }
        }
        else
          this.skip_count.Value = "0";
      }
      else
        this.skip_count.Value = "0";
    }
    return num1;
  }

  private void loadResources(long level, long category, long building, long capacity)
  {
    try
    {
      string str1 = "";
      DateTime dateTime = Convert.ToDateTime(this.txt_startDate.Text);
      bool boolean1 = Convert.ToBoolean(this.hdn_ph.Value);
      bool boolean2 = Convert.ToBoolean(this.hdn_we.Value);
      int num1 = this.process_count();
      DataTable dataTable1 = new DataTable();
      this.setting_data.Tables[0].Select("parameter='building'");
      string groupIds = this.utilities.get_group_ids(this.current_user);
      string group_ids = string.IsNullOrEmpty(groupIds) ? "0" : groupIds;
      DataSet dataSet1 = new DataSet();
      DataSet assetDs = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_ass");
      if (assetDs == null)
      {
        assetDs = this.assets.get_assets_booking_slot(this.current_timestamp, Convert.ToDateTime(this.txt_startDate.Text), building, category, level, capacity, this.current_user.account_id, this.current_user.user_id, this.gp.isAdminType, group_ids);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_ass", (object) assetDs);
      }
      this.asset_properties_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_ass_prop");
      if (this.asset_properties_data == null)
      {
        this.asset_properties_data = this.assets.view_asset_properties(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_ass_prop", (object) this.asset_properties_data);
        this.ViewState.Add("asset_properties", (object) this.asset_properties_data);
      }
      if (assetDs != null)
      {
        if (assetDs.Tables[0].Rows.Count <= 0)
        {
          if (assetDs.Tables[1].Rows.Count <= 0)
            goto label_23;
        }
        try
        {
          assetDs.Tables[0].DefaultView.Sort = "building_name,level_id,name asc";
          DataTable table = assetDs.Tables[0].DefaultView.ToTable();
          assetDs.Tables[0].Clear();
          foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
          {
            if (Convert.ToInt16(row["status"]) == (short) 1)
            {
              bool flag = false;
              if (this.gp.isAdminType)
              {
                flag = true;
              }
              else
              {
                if (row["asset_owner_group_id"].ToString() != "" && ("," + group_ids).Contains("," + row["asset_owner_group_id"].ToString()))
                  flag = true;
                if (!flag)
                  flag = !(this.current_timestamp.AddMonths(Convert.ToInt32(this.asset_properties_data.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and property_name='advance_booking_window'")[0]["property_value"])) < dateTime);
              }
              if (flag)
              {
                assetDs.Tables[0].ImportRow(row);
                assetDs.AcceptChanges();
              }
            }
          }
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ex.ToString());
        }
label_23:
        if (this.txt_search.Text != "")
        {
          DataRow[] dataRowArray = assetDs.Tables[0].Select("name like '%" + this.txt_search.Text + "%' or code like '%" + this.txt_search.Text + "%'");
          if (dataRowArray.Length > 0)
          {
            DataTable dataTable2 = assetDs.Tables[0].Clone();
            foreach (DataRow dataRow in dataRowArray)
            {
              dataTable2.Rows.Add(dataRow.ItemArray);
              dataTable2.AcceptChanges();
            }
            assetDs.Tables[0].Clear();
            foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
              assetDs.Tables[0].Rows.Add(row.ItemArray);
            dataTable2.Dispose();
            assetDs.AcceptChanges();
          }
        }
        int int32 = Convert.ToInt32(this.skip_count.Value);
        int count1 = !(this.save_count.Value == "ALL") ? Convert.ToInt32(this.save_count.Value) : 1000000000;
        try
        {
          if (this.chk_fav.Checked)
          {
            DataSet dataSet2 = (DataSet) this.Session["favourites"];
            if (dataSet2.Tables[0].Rows.Count > 0)
            {
              DataSet dataSet3 = assetDs.Clone();
              foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
              {
                if (row["module_name"].ToString() == "asset")
                {
                  DataRow[] dataRowArray = assetDs.Tables[0].Select("asset_id='" + row["resource_id"].ToString() + "'");
                  if (dataRowArray.Length > 0)
                    dataSet3.Tables[0].Rows.Add(dataRowArray[0].ItemArray);
                }
              }
              assetDs.Tables[1].Rows.Clear();
              assetDs.Tables[0].Rows.Clear();
              foreach (DataRow row in (InternalDataCollectionBase) dataSet3.Tables[0].Rows)
                assetDs.Tables[0].Rows.Add(row.ItemArray);
              assetDs.Tables[0].AcceptChanges();
            }
          }
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) "fav routine error (quick_booking):", ex);
        }
        if (assetDs.Tables[0].Rows.Count > 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          this.DayPilotScheduler1.Resources.Clear();
          int count2;
          if (this.save_count.Value == "ALL")
          {
            HiddenField saveCount = this.save_count;
            count2 = assetDs.Tables[0].Rows.Count;
            string str2 = count2.ToString();
            saveCount.Value = str2;
          }
          if (Convert.ToInt32(this.skip_count.Value) <= assetDs.Tables[0].Rows.Count)
          {
            dataTable1 = assetDs.Tables[0].AsEnumerable().Skip<DataRow>(int32).Take<DataRow>(count1).CopyToDataTable<DataRow>();
            dataTable1.Columns.Add("Flag");
          }
          HiddenField totalCount = this.total_count;
          count2 = assetDs.Tables[0].Rows.Count;
          string str3 = count2.ToString();
          totalCount.Value = str3;
          if (dataTable1 != null)
          {
            this.lbl_count.ForeColor = Color.Black;
            int num2 = !(this.count_select.SelectedItem.Text == "ALL") ? Convert.ToInt32(this.count_select.SelectedItem.Text) : Convert.ToInt32(this.total_count.Value);
            if (int32 + num2 > assetDs.Tables[0].Rows.Count)
            {
              int num3 = int32 + num2 - assetDs.Tables[0].Rows.Count;
              if (Convert.ToInt32(this.total_count.Value) <= int32 + num2 - num3)
                this.lbl_count.Text = "Showing  " + Convert.ToString(int32 + 1) + " to " + this.total_count.Value + " of " + this.total_count.Value + " Assets.";
              else
                this.lbl_count.Text = "Showing  " + Convert.ToString(int32 + 1) + " to " + Convert.ToString(int32 + num2 - num3) + " of " + this.total_count.Value + " Assets.";
            }
            else if (int32 + num2 > Convert.ToInt32(this.total_count.Value))
              this.lbl_count.Text = "Showing  " + Convert.ToString(int32 + 1) + " to " + this.total_count.Value + " of " + this.total_count.Value + " Assets.";
            else
              this.lbl_count.Text = "Showing  " + Convert.ToString(int32 + 1) + " to " + Convert.ToString(int32 + num2) + " of " + this.total_count.Value + " Assets.";
            int num4 = Convert.ToInt32(this.total_count.Value) / count1;
            if (Convert.ToInt32(this.total_count.Value) % count1 == 0)
              --num4;
            try
            {
              if (int32 <= assetDs.Tables[0].Rows.Count)
              {
                if (num1 > 0)
                {
                  stringBuilder.Append("<li><a href='javascript:page_pass(1);'><i class='icon-double-angle-left'></i>First</a></li>");
                  stringBuilder.AppendFormat("<li><a href='javascript:page_pass({0});'><i class='icon-double-angle-left'></i>Previous</a></li>", (object) -1);
                }
                else
                {
                  stringBuilder.Append("<li style='color:#999;'><i class='icon-double-angle-left'></i>First</li>");
                  stringBuilder.Append("<li style='color:#999;'><i class='icon-angle-left'></i>Previous</li>");
                }
                if (num4 <= 5)
                {
                  for (int index = 0; index <= num1 + 4; ++index)
                  {
                    int num5 = Convert.ToInt32(index) + 1;
                    if (num1 == index)
                      stringBuilder.Append("<li style='color:#999;'>" + (object) num5 + "</li>");
                    else if (index <= num4)
                      stringBuilder.AppendFormat("<li><a href='javascript:page_pass({0});'>" + (object) num5 + "</a></li>", (object) num5);
                  }
                }
                else
                {
                  for (int index = num1; index <= num1 + 4; ++index)
                  {
                    int num6 = Convert.ToInt32(index) + 1;
                    if (num1 == index)
                      stringBuilder.Append("<li style='color:#999;'>" + (object) num6 + "</li>");
                    else if (index <= num4)
                      stringBuilder.AppendFormat("<li><a href='javascript:page_pass({0});'>" + (object) num6 + "</a></li>", (object) num6);
                  }
                }
                if (Convert.ToInt32(this.skip_count.Value) + Convert.ToInt32(num2) < Convert.ToInt32(this.total_count.Value))
                  stringBuilder.AppendFormat("<li><a href='javascript:page_pass({0});'> Next</a></li>", (object) 999);
                else
                  stringBuilder.Append("<li style='color:#999;'>Next<i class='iar'></i></li>");
              }
            }
            catch (Exception ex)
            {
              fbs_base_page.log.Error((object) ("LoadResource(Inside)(Pagination Flow) : " + ex.ToString()));
            }
            long num7 = 0;
            this.asset_properties_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_ass_prop");
            if (this.asset_properties_data == null)
            {
              this.asset_properties_data = this.assets.view_asset_properties(this.current_user.account_id);
              this.capi.set_cache(this.current_user.account_id.ToString() + "_ass_prop", (object) this.asset_properties_data);
              this.ViewState.Add("asset_properties", (object) this.asset_properties_data);
            }
            this.daypilot_pagination.InnerHtml = stringBuilder.ToString();
            try
            {
              foreach (DataRow dataRow1 in building != 0L ? this.setting_data.Tables[0].Select("parameter='building' and setting_id='" + (object) building + "'") : this.setting_data.Tables[0].Select("parameter='building'"))
              {
                DataRow[] dataRowArray1 = dataTable1.Select("building_id='" + dataRow1["setting_id"].ToString() + "'");
                if (dataRowArray1.Length > 0)
                {
                  int length = dataRowArray1.Length;
                  Resource resource1 = new Resource();
                  resource1.Id = "b_" + dataRow1["setting_id"].ToString();
                  resource1.Name = "";
                  resource1.Columns.Add(new ResourceColumn("<b>" + dataRow1["value"].ToString() + " (" + (object) length + " rooms)</b>"));
                  resource1.Columns.Add(new ResourceColumn(""));
                  resource1.Columns.Add(new ResourceColumn(""));
                  resource1.Expanded = true;
                  this.DayPilotScheduler1.Resources.Add(resource1);
                  foreach (DataRow dataRow2 in dataRowArray1)
                  {
                    this.calculate_daypiot_calendar_lenght(dataTable1.Rows.Count);
                    DataSet dataSet4 = new DataSet();
                    asset asset = new asset();
                    long int64 = Convert.ToInt64(dataRow2["asset_id"]);
                    bool flag1 = this.gp.isAdminType || this.check_booking_capability(int64, this.asset_properties_data, this.setting_data, boolean1, boolean2);
                    if (this.assets.get_visible_assets(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType).Contains(int64) && flag1)
                    {
                      ++num7;
                      DataRow[] dataRowArray2 = this.setting_data.Tables[0].Select("setting_id='" + dataRow2["level_id"].ToString() + "' and parameter='level'");
                      if (dataRowArray2.Length > 0)
                        str1 = dataRowArray2[0]["value"].ToString();
                      if (dataRow2["status"].ToString() == "1")
                      {
                        bool flag2 = false;
                        bool flag3 = false;
                        DataRow[] dataRowArray3 = assetDs.Tables[1].Select("asset_id='" + (object) int64 + "'");
                        if (dataRowArray3.Length > 0)
                        {
                          flag2 = Convert.ToBoolean(dataRowArray3[0]["is_view"]);
                          flag3 = Convert.ToBoolean(dataRowArray3[0]["is_book"]);
                        }
                        if (flag2)
                        {
                          DataRow[] dataRowArray4 = this.asset_properties_data.Tables[0].Select("asset_id=" + dataRow2["asset_id"].ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
                          bool flag4 = true;
                          if (!flag3)
                            flag4 = false;
                          dataRow2["Flag"] = dataRowArray4.Length <= 0 ? (object) "F" : (object) "T";
                          bool flag5 = false;
                          string innerHTML;
                          if (dataRow2["asset_owner_group_id"].ToString() != "")
                          {
                            if (this.approvable_rooms.Contains(int64))
                              flag5 = true;
                            if (this.gp.isAdminType)
                              flag5 = true;
                            if (flag5)
                              innerHTML = "<span class='qbr'><img src='assets/img/bo.png'/> " + str1 + " - " + dataRow2["name"].ToString() + "</span>";
                            else if (!flag4)
                              innerHTML = "<span class='qbr'><img src='assets/img/bn.png'/> " + str1 + " - " + dataRow2["name"].ToString() + "</span>";
                            else
                              innerHTML = "<span class='qbr'><img src='assets/img/ba.png'/> " + str1 + " - " + dataRow2["name"].ToString() + "</span>";
                          }
                          else if (!flag4)
                            innerHTML = "<span class='qbr'><img src='assets/img/bn.png'/> " + str1 + " - " + dataRow2["name"].ToString() + "</span>";
                          else
                            innerHTML = "<span class='qbr'><img src='assets/img/bo.png'/> " + str1 + " - " + dataRow2["name"].ToString() + "</span>";
                          if (this.gp.isAdminType)
                            innerHTML = "<span class='qbr'><img src='assets/img/bo.png'/> " + str1 + " - " + dataRow2["name"].ToString() + "</span>";
                          if (true)
                          {
                            Resource resource2 = new Resource();
                            resource2.Id = dataRow2["asset_id"].ToString();
                            if (flag5)
                            {
                              if (string.IsNullOrEmpty(this.hdn_asset_ids.Value))
                                this.hdn_asset_ids.Value = 0.ToString() + "#";
                            }
                            else if (!flag3)
                              this.hdn_asset_ids.Value = !string.IsNullOrEmpty(this.hdn_asset_ids.Value) ? this.hdn_asset_ids.Value + "#" + resource2.Id : 0.ToString() + "#" + resource2.Id;
                            resource2.Name = "";
                            ResourceColumn resourceColumn1 = new ResourceColumn(innerHTML);
                            ResourceColumn resourceColumn2 = !(dataRow2[nameof (capacity)].ToString() == "-1") ? new ResourceColumn("<center>" + dataRow2[nameof (capacity)].ToString() + "</center>") : new ResourceColumn("<center> NA </center>");
                            ResourceColumn resourceColumn3 = new ResourceColumn();
                            resourceColumn3.InnerHTML = !(dataRow2["Flag"].ToString() == "T") ? "" : "<center><img id='img_prop' align='middle' src='assets/img/Facilityerro.png' alt='Faulty Room' /></center>";
                            resource2.Columns.Add(resourceColumn1);
                            resource2.Columns.Add(resourceColumn2);
                            resource2.Columns.Add(resourceColumn3);
                            resource1.Children.Add(resource2);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              fbs_base_page.log.Error((object) ("LoadResource(Inside)Setting Table rows : " + ex.ToString()));
            }
          }
          else
            this.no_record(assetDs);
        }
        else
          this.no_record(assetDs);
      }
      this.Session["skip"] = (object) this.skip_count.Value;
      this.Session["save_count"] = (object) this.save_count.Value;
      this.Session["selected_index"] = (object) this.count_select.SelectedItem.Text;
      this.Session["txt_search"] = (object) this.txt_search.Text;
      this.Session["total_count"] = (object) this.total_count.Value;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("LoadResource(Inside) : " + ex.ToString()));
    }
  }

  private void no_record(DataSet assetDs)
  {
    this.lbl_count.Text = Resources.fbs.daypilot_page_no_record;
    this.lbl_count.ForeColor = Color.Red;
    this.daypilot_pagination.InnerText = "";
    this.skip_count.Value = "0";
    this.save_count.Value = "0";
    this.total_count.Value = "0";
    assetDs = new DataSet();
    this.DayPilotScheduler1.Resources.Clear();
  }

  private bool check_booking_capability(
    long asset_id,
    DataSet asset_properties,
    DataSet settings,
    bool is_publicholiday,
    bool is_weekend)
  {
    bool flag1 = true;
    DataRow[] dataRowArray1 = asset_properties.Tables[0].Select("asset_id='" + (object) asset_id + "' and property_name='book_holiday'");
    bool flag2;
    if (dataRowArray1.Length > 0)
    {
      flag2 = Convert.ToBoolean(dataRowArray1[0]["property_value"]);
    }
    else
    {
      DataRow[] dataRowArray2 = settings.Tables[0].Select("parameter='book_holiday'");
      flag2 = dataRowArray2.Length <= 0 || Convert.ToBoolean(dataRowArray2[0]["value"]);
    }
    DataRow[] dataRowArray3 = asset_properties.Tables[0].Select("asset_id='" + (object) asset_id + "' and property_name='book_weekend'");
    bool flag3;
    if (dataRowArray3.Length > 0)
    {
      flag3 = Convert.ToBoolean(dataRowArray3[0]["property_value"]);
    }
    else
    {
      DataRow[] dataRowArray4 = settings.Tables[0].Select("parameter='book_weekend'");
      flag3 = dataRowArray4.Length <= 0 || Convert.ToBoolean(dataRowArray4[0]["value"]);
    }
    if (is_weekend)
      flag1 = flag3;
    if (is_publicholiday && flag1)
      flag1 = flag2;
    return flag1;
  }

  public void calculate_daypiot_calendar_lenght(int total_records)
  {
    try
    {
      if (total_records <= 10)
      {
        this.DayPilotScheduler1.Height = (Unit) (total_records * 50);
      }
      else
      {
        if (total_records <= 10)
          return;
        this.DayPilotScheduler1.Height = (Unit) 515;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Calculate dayPilot Length(Inside) : " + ex.ToString()));
    }
  }

  protected void DayPilotScheduler1_Command(object sender, DayPilot.Web.Ui.Events.CommandEventArgs e)
  {
    try
    {
      switch (e.Command)
      {
        case "refresh":
          this.setDataSourceAndBind();
          this.DayPilotScheduler1.Update();
          break;
        case "filter":
          this.setDataSourceAndBind();
          this.DayPilotScheduler1.Update();
          break;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Day Pilot Command(Inside) : " + ex.ToString()));
    }
  }

  protected void DayPilotScheduler1_BeforeCellRender(object sender, BeforeCellRenderEventArgs e)
  {
    if (e.ResourceId.Contains("b_"))
    {
      e.BackgroundColor = "#eeeeee";
    }
    else
    {
      this.op_start = (Dictionary<string, DateTime>) this.ViewState["op_start"];
      this.op_end = (Dictionary<string, DateTime>) this.ViewState["op_end"];
      if (this.op_start == null)
        this.op_start = new Dictionary<string, DateTime>();
      if (this.op_end == null)
        this.op_end = new Dictionary<string, DateTime>();
      this.asset_properties_data = (DataSet) this.ViewState["asset_properties"];
      if (this.asset_properties_data == null)
      {
        this.asset_properties_data = this.assets.view_asset_properties(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_ass_prop", (object) this.asset_properties_data);
        this.ViewState.Add("asset_properties", (object) this.asset_properties_data);
      }
      if (!this.op_start.ContainsKey(e.ResourceId))
      {
        DateTime dateTime1 = new DateTime();
        DateTime dateTime2 = new DateTime();
        DataRow[] dataRowArray1 = this.asset_properties_data.Tables[0].Select("asset_id=" + e.ResourceId + " and property_name='operating_hours'");
        if (dataRowArray1.Length > 0)
        {
          string[] strArray = dataRowArray1[0]["property_value"].ToString().Split('|');
          DateTime dateTime3 = Convert.ToDateTime(strArray[0]);
          DateTime dateTime4 = Convert.ToDateTime(strArray[1]);
          this.op_start.Add(e.ResourceId, dateTime3);
          this.op_end.Add(e.ResourceId, dateTime4);
        }
        else
        {
          DataRow[] dataRowArray2 = this.setting_data.Tables[0].Select("parameter='operating_hours'");
          if (dataRowArray2.Length > 0)
          {
            string[] strArray = dataRowArray2[0]["value"].ToString().Split('|');
            DateTime dateTime5 = Convert.ToDateTime(strArray[0]);
            DateTime dateTime6 = Convert.ToDateTime(strArray[1]);
            this.op_start.Add(e.ResourceId, dateTime5);
            this.op_end.Add(e.ResourceId, dateTime6);
          }
          else
          {
            this.op_start.Add(e.ResourceId, new DateTime(2000, 1, 1, 0, 0, 0));
            this.op_end.Add(e.ResourceId, new DateTime(2000, 1, 1, 23, 45, 0));
          }
        }
      }
      DateTime dateTime7 = new DateTime(2000, 1, 1, e.End.Hour, e.End.Minute, e.End.Second);
      DateTime dateTime8 = new DateTime(2000, 1, 1, e.Start.Hour, e.Start.Minute, e.Start.Second);
      if (!(dateTime7 <= this.op_start[e.ResourceId]))
      {
        if (!(dateTime8 >= this.op_end[e.ResourceId]))
          goto label_17;
      }
      e.BackgroundColor = "#f5f5f5";
label_17:
      try
      {
        if (!this.lead_times.ContainsKey(e.ResourceId))
        {
          DateTime dateTime9 = DateTime.UtcNow.AddHours(this.current_account.timezone);
          DataRow[] dataRowArray3 = this.asset_properties_data.Tables[0].Select("asset_id=" + e.ResourceId + " and property_name='booking_lead_time'");
          DataRow[] dataRowArray4 = this.asset_properties_data.Tables[0].Select("asset_id=" + e.ResourceId + " and property_name='book_weekend'");
          DataRow[] dataRowArray5 = this.asset_properties_data.Tables[0].Select("asset_id=" + e.ResourceId + " and property_name='book_holiday'");
          if (dataRowArray3.Length > 0)
          {
            if (dataRowArray3[0].ItemArray[2].ToString() != "")
            {
              string[] strArray = dataRowArray3[0].ItemArray[2].ToString().Split(' ');
              if (strArray[0] != "0")
              {
                if (strArray[1] == "Hour(s)")
                {
                  dateTime9 = dateTime9.AddHours(Convert.ToDouble(strArray[0]));
                }
                else
                {
                  int num = 0;
                  for (int index = 0; (long) index < Convert.ToInt64(strArray[0]) + 1L; ++index)
                  {
                    ++num;
                    if (dateTime9.Date.AddDays((double) (index + 1)).DayOfWeek == DayOfWeek.Saturday || dateTime9.Date.AddDays((double) (index + 1)).DayOfWeek == DayOfWeek.Sunday)
                    {
                      if (dataRowArray4.Length > 0)
                      {
                        if (dataRowArray3[0].ItemArray[2].ToString() == "False")
                          ++num;
                      }
                      else
                        ++num;
                    }
                    if (dataRowArray5.Length > 0)
                    {
                      if (dataRowArray5[0].ItemArray[2].ToString() == "False")
                      {
                        DataSet ds = this.holidays.view_holidays(dateTime9.Date.AddDays((double) (index + 1)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", dateTime9.Date.AddDays((double) (index + 2)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", this.account_id);
                        if (this.utilities.isValidDataset(ds) && ds.Tables[0].Rows.Count > 0)
                          ++num;
                      }
                    }
                    else
                      ++num;
                  }
                  dateTime9 = dateTime9.AddDays((double) num);
                }
                this.lead_times.Add(e.ResourceId, dateTime9);
              }
              else
                this.lead_times.Add(e.ResourceId, new DateTime(2000, 1, 1, 0, 0, 0));
            }
          }
          else
            this.lead_times.Add(e.ResourceId, new DateTime(2000, 1, 1, 0, 0, 0));
        }
        if (e.Start > DateTime.UtcNow.AddHours(this.current_account.timezone))
        {
          if (e.Start < this.lead_times[e.ResourceId])
            e.BackgroundColor = "#f5f5f5";
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
      }
      this.ViewState.Add("op_start", (object) this.op_start);
      this.ViewState.Add("op_end", (object) this.op_end);
    }
  }

  protected void DayPilotScheduler1_BeforeEventRender(object sender, BeforeEventRenderEventArgs e)
  {
    try
    {
      e.BubbleHtml = (string) e.DataItem["purpose"];
      e.ToolTip = (string) e.DataItem["created_by_Name"];
      e.InnerHTML = string.Format("{0} ({1:d} - {2:d})", (object) e.Text, (object) e.Start.ToString("HH:mm"), (object) e.End.ToString("HH:mm"));
      string str = e.Tag["status"];
      if (e.Start < this.current_timestamp)
      {
        if (e.End < this.current_timestamp)
        {
          e.DurationBarColor = "gray";
        }
        else
        {
          switch (str)
          {
            case "2":
              e.DurationBarColor = "gray";
              break;
            case "4":
              e.DurationBarColor = "blue";
              break;
            default:
              e.DurationBarColor = "orange";
              break;
          }
        }
      }
      else
      {
        switch (str)
        {
          case "2":
            e.DurationBarColor = "gray";
            break;
          case "4":
            e.DurationBarColor = "blue";
            break;
          default:
            e.DurationBarColor = "green";
            break;
        }
      }
      e.InnerHTML += string.Format("<br /><span style='color:gray'>{0}</span>", (object) e.ToolTip);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Daypilot Before Event Render(Inside) : " + ex.ToString()));
    }
  }

  protected void DayPilotScheduler1_Refresh(object sender, RefreshEventArgs e)
  {
    try
    {
      this.DayPilotScheduler1.StartDate = e.StartDate;
      this.setDataSourceAndBind();
      this.DayPilotScheduler1.Update(CallBackUpdateType.Full);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Daypilot refresh(Inside) : " + ex.ToString()));
    }
  }

  private void process_weekend_publicholiday(DateTime sd)
  {
    this.hdn_we.Value = sd.DayOfWeek == DayOfWeek.Saturday || sd.DayOfWeek == DayOfWeek.Sunday ? "true" : "false";
    this.holiday = this.bookingsbl.getPublicHolidayList(sd.ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", sd.AddDays(1.0).AddSeconds(-1.0).ToString(api_constants.datetime_format), this.current_user.account_id);
    if (!string.IsNullOrEmpty(this.holiday))
    {
      this.divHolidays.Attributes.Add("style", "display: block;");
      this.hdn_ph.Value = "true";
    }
    else
    {
      this.divHolidays.Attributes.Add("style", "display: none;");
      this.hdn_ph.Value = "false";
    }
  }

  private void rebind()
  {
    try
    {
      DateTime dateTime1 = Convert.ToDateTime(this.txt_startDate.Text.Trim());
      string str;
      if (!string.IsNullOrEmpty(this.txt_startDate.Text))
      {
        str = dateTime1.ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
      }
      else
      {
        this.txt_startDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        str = dateTime1.ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
      }
      DateTime dateTime2 = Convert.ToDateTime(str);
      this.process_weekend_publicholiday(dateTime2);
      this.DayPilotScheduler1.StartDate = dateTime2;
      this.initData();
      this.DayPilotScheduler1.StartDate = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day);
      this.DayPilotScheduler1.Separators.Add(dateTime2, Color.Red);
      long building = 0;
      if (this.ddl_building.SelectedItem.Text != "All")
        building = Convert.ToInt64(this.ddl_building.SelectedItem.Value);
      long category = 0;
      if (this.ddl_category.SelectedItem.Text != "All")
        category = Convert.ToInt64(this.ddl_category.SelectedItem.Value);
      long level = 0;
      if (this.ddl_level.SelectedItem.Text != "All")
        level = Convert.ToInt64(this.ddl_level.SelectedItem.Value);
      long capacity = 0;
      if (!string.IsNullOrEmpty(this.txt_capacity.Text))
        capacity = Convert.ToInt64(this.txt_capacity.Text);
      this.loadResources(level, category, building, capacity);
      string filter = (string) this.DayPilotScheduler1.ClientState["filter"];
      fbs_base_page.log.Info((object) "getData 3");
      this.DayPilotScheduler1.DataSource = (object) this.getData(dateTime2, dateTime2.AddDays(1.0).AddSeconds(-1.0), filter);
      this.DayPilotScheduler1.DataBind();
      this.DayPilotScheduler1.Update();
      new Dictionary<string, string>()
      {
        {
          dateTime2.ToString(api_constants.datetime_format),
          dateTime2.AddDays(1.0).AddSeconds(-1.0).ToString(api_constants.datetime_format)
        }
      };
      if (this.is_blacklisted)
      {
        this.alertError.Attributes.Add("style", "display: block;");
        this.litErrorMsg.Text = Resources.fbs.blacklist_user_booking_msg;
      }
      else
        this.alertError.Attributes.Add("style", "display: none;");
      this.DayPilotScheduler1.SetScrollX(this.utilities.TimeRoundUp(new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, this.current_timestamp.Hour, this.current_timestamp.Minute, this.current_timestamp.Second)));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Daypilot Rebind(Inside) : " + ex.ToString()));
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_count.Value = this.count_select.SelectedItem.Text;
      this.skip_count.Value = "0";
      this.rebind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Sybmit(Inside) : " + ex.ToString()));
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("booking_quick.aspx");

  protected void txt_startDate_TextChanged(object sender, EventArgs e)
  {
    try
    {
      this.rebind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Date text chage fun(Inside) : " + ex.ToString()));
    }
  }

  protected void count_select_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      this.Session["selected_index"] = (object) this.count_select.SelectedItem.Text;
      this.save_count.Value = this.count_select.SelectedItem.Text;
      this.skip_count.Value = "0";
      this.rebind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Select count(Inside) : " + ex.ToString()));
    }
  }

  protected void btn_search_Click(object sender, EventArgs e)
  {
    try
    {
      this.save_count.Value = this.count_select.SelectedItem.Text;
      this.skip_count.Value = "0";
      this.rebind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("search btn(Inside) : " + ex.ToString()));
    }
  }

  protected void btn_clear_search_Click(object sender, EventArgs e)
  {
    try
    {
      this.txt_search.Text = "";
      this.save_count.Value = this.count_select.SelectedItem.Text;
      this.skip_count.Value = "0";
      this.rebind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Clear Btn(Inside) : " + ex.ToString()));
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
