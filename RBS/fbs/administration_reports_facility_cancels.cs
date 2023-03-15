// Decompiled with JetBrains decompiler
// Type: administration_reports_facility_cancels
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
using System.Web.UI.WebControls;

public class administration_reports_facility_cancels : fbs_base_page, IRequiresSessionState
{
  protected Label lblDateRage;
  protected Button btn_filter;
  protected HiddenField hdn_report_start;
  protected HiddenField hdn_report_end;
  protected HiddenField hdn_daterange;
  protected HiddenField hdn_current_tab;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected DropDownList ddl_category;
  protected DropDownList ddl_type;
  protected DropDownList ddl_rooms;
  protected RadioButtonList rdo_frequency;
  protected RadioButtonList rdo_chart;
  protected Button btn_filters;
  public string html_details_table;
  public string html_total_bookings;
  public string html_total_hours_booked;
  public string html_avg_duration;
  public string html_utilization;
  public string top_10_room_names;
  public string top_10_numbers;
  public string chart_data;
  public string chart_label;
  public string chart_color;
  public string html_title;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      this.load_ui();
      string str1 = this.hdn_report_start.Value;
      string str2;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        str2 = str1 + " 00:00:00.000";
      }
      else
      {
        str2 = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
        this.hdn_report_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      }
      string str3 = this.hdn_report_end.Value;
      string str4;
      if (!string.IsNullOrWhiteSpace(str3))
      {
        str4 = str3 + " 23:59:59.999";
      }
      else
      {
        str4 = this.current_timestamp.ToString(api_constants.sql_datetime_format_short) + " 23:59:59.999";
        this.hdn_report_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      }
      this.get_report(this.current_user.account_id, Convert.ToDateTime(this.hdn_report_start.Value), Convert.ToDateTime(this.hdn_report_end.Value));
    }
    this.hdn_daterange.Value = Convert.ToDateTime(this.hdn_report_start.Value).ToString("MMMM d, yyyy") + " - " + Convert.ToDateTime(this.hdn_report_end.Value).ToString("MMMM d, yyyy");
  }

  private void load_ui()
  {
    DataSet dataSet1 = this.settings.view_settings(this.current_user.account_id);
    this.ddl_building.Items.Add(new ListItem("All Buildings", "0"));
    foreach (DataRow dataRow in dataSet1.Tables[0].Select("parameter='building'"))
      this.ddl_building.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    this.ddl_level.Items.Add(new ListItem("All Levels", "0"));
    foreach (DataRow dataRow in dataSet1.Tables[0].Select("parameter='level'"))
      this.ddl_level.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    this.ddl_category.Items.Add(new ListItem("All Categories", "0"));
    foreach (DataRow dataRow in dataSet1.Tables[0].Select("parameter='category'"))
      this.ddl_category.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    this.ddl_type.Items.Add(new ListItem("All Types", "0"));
    foreach (DataRow dataRow in dataSet1.Tables[0].Select("parameter='asset_type'"))
      this.ddl_type.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    DataSet dataSet2 = this.assets.view_assets(this.current_user.account_id);
    this.ddl_rooms.Items.Add(new ListItem("All Rooms", "0"));
    foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
      this.ddl_rooms.Items.Add(new ListItem(row["name"].ToString(), row["asset_id"].ToString()));
  }

  private void get_report(Guid account_id, DateTime from_date, DateTime to_date)
  {
    DataSet dataSet = this.assets.view_assets(account_id);
    DataSet settings1 = this.settings.get_settings(account_id, "building");
    DataSet settings2 = this.settings.get_settings(account_id, "level");
    DataSet cancelsByFacility = this.rptapi.get_cancels_by_facility(this.current_user.account_id, from_date, to_date.AddDays(1.0).AddSeconds(-1.0), "");
    DataSet byFacilitySummary = this.rptapi.get_cancels_by_facility_summary(this.current_user.account_id, from_date, to_date.AddDays(1.0).AddSeconds(-1.0), "");
    int num1 = 0;
    int num2 = 0;
    int count1 = cancelsByFacility.Tables[0].Rows.Count;
    foreach (DataRow row in (InternalDataCollectionBase) cancelsByFacility.Tables[0].Rows)
    {
      int int32 = Convert.ToInt32(row["total_minutes"]);
      if (int32 > num1)
        num1 = int32;
      num2 += int32;
    }
    double totalDays = (to_date - from_date).TotalDays;
    int count2 = dataSet.Tables[0].Rows.Count;
    int count3 = dataSet.Tables[0].Rows.Count;
    if (count1 > 0)
    {
      this.html_total_bookings = count1.ToString("###,###");
      this.html_total_hours_booked = (num2 / 60).ToString("###,###.##") + " Hrs." + (object) (num2 % 60) + " Mins.";
      this.html_avg_duration = (num2 / count1 / 60).ToString("###,###.##") + " Hrs. " + (object) (num2 / count1 % 60) + " Mins.";
    }
    else
    {
      this.html_total_bookings = "0";
      this.html_total_hours_booked = "0 Hrs. 0 Mins.";
      this.html_avg_duration = "-";
    }
    DataTable table = new DataTable();
    table.Columns.Add("building");
    table.Columns.Add("level");
    table.Columns.Add("room");
    table.Columns.Add("total_bookings");
    table.Columns.Add("total_duration");
    table.Columns.Add("max_duration");
    table.Columns.Add("min_duration");
    table.Columns.Add("avg_duration");
    table.AcceptChanges();
    foreach (DataRow row1 in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
    {
      double num3 = 0.0;
      double num4 = 0.0;
      double num5 = 0.0;
      double num6 = 0.0;
      double num7 = 0.0;
      DataRow[] dataRowArray1 = byFacilitySummary.Tables[0].Select("asset_id='" + row1["asset_id"].ToString() + "'");
      if (dataRowArray1.Length > 0)
      {
        num3 = Convert.ToDouble(dataRowArray1[0]["count_bookings"]);
        num4 = Convert.ToDouble(dataRowArray1[0]["total_duration"]);
        num5 = Convert.ToDouble(dataRowArray1[0]["max_duration"]);
        num6 = Convert.ToDouble(dataRowArray1[0]["min_duration"]);
        num7 = Convert.ToDouble(dataRowArray1[0]["avg_duration"]);
      }
      DataRow[] dataRowArray2 = settings1.Tables[0].Select("setting_id='" + row1["building_id"].ToString() + "'");
      DataRow[] dataRowArray3 = settings2.Tables[0].Select("setting_id='" + row1["level_id"].ToString() + "'");
      DataRow row2 = table.NewRow();
      row2["building"] = (object) dataRowArray2[0]["value"].ToString();
      row2["level"] = (object) dataRowArray3[0]["value"].ToString();
      row2["room"] = (object) row1["name"].ToString();
      row2["total_bookings"] = (object) num3;
      row2["total_duration"] = (object) num4;
      row2["max_duration"] = (object) num5;
      row2["min_duration"] = (object) num6;
      row2["avg_duration"] = (object) num7;
      table.Rows.Add(row2);
      table.AcceptChanges();
    }
    this.ui_utilization_table(table);
    this.ui_top_10(table);
    this.ui_graph(cancelsByFacility.Tables[0], from_date, to_date, this.rdo_frequency.SelectedItem.Value);
  }

  private void ui_graph(DataTable table, DateTime from, DateTime to, string type)
  {
    Dictionary<DateTime, double> dictionary = new Dictionary<DateTime, double>();
    if (type == "days")
    {
      double totalDays = (to - from).TotalDays;
      for (DateTime key = from; key <= to; key = key.AddDays(1.0))
      {
        table.Select("book_date='" + (object) key + "'");
        double num;
        try
        {
          num = !(this.rdo_chart.SelectedItem.Value == "duration") ? Convert.ToDouble(table.Compute("sum(counter)", "book_date='" + (object) key + "'")) : Convert.ToDouble(table.Compute("sum(total_minutes)", "book_date='" + (object) key + "'"));
        }
        catch
        {
          num = 0.0;
        }
        dictionary.Add(key, num);
      }
    }
    if (type == "weeks")
    {
      for (DateTime key = this.rptapi.start_of_week(from, DayOfWeek.Sunday); key <= to; key = key.AddDays(7.0))
      {
        double num = 0.0;
        DataTable dataTable = table;
        string filterExpression = "book_date>='" + (object) key + "' and book_date<='" + (object) key.AddDays(7.0) + "'";
        foreach (DataRow dataRow in dataTable.Select(filterExpression))
        {
          if (this.rdo_chart.SelectedItem.Value == "duration")
            num += Convert.ToDouble(dataRow["total_minutes"]);
          else
            num += Convert.ToDouble(dataRow["counter"]);
        }
        dictionary.Add(key, num);
      }
    }
    if (type == "months")
    {
      for (DateTime key = from.AddDays((double) ((from.Day - 1) * -1)); key <= to; key = key.AddMonths(1))
      {
        double num = 0.0;
        DataTable dataTable = table;
        string filterExpression = "book_date>='" + (object) key + "' and book_date<='" + (object) key.AddMonths(1) + "'";
        foreach (DataRow dataRow in dataTable.Select(filterExpression))
        {
          if (this.rdo_chart.SelectedItem.Value == "duration")
            num += Convert.ToDouble(dataRow["total_minutes"]);
          else
            num += Convert.ToDouble(dataRow["counter"]);
        }
        dictionary.Add(key, num);
      }
    }
    string str1 = "";
    string str2 = "";
    string str3 = "";
    int count = dictionary.Count;
    int num1 = 0;
    foreach (DateTime key in dictionary.Keys)
    {
      str2 = str2 + "'" + key.ToString("dd-MMM-yyyy") + "'";
      str1 = str1 + "'" + dictionary[key].ToString() + "'";
      str3 += "'#FF6384'";
      if (num1 < count - 1)
      {
        str2 += ",";
        str1 += ",";
        str3 += ",";
      }
      ++num1;
    }
    this.chart_data = str1;
    this.chart_color = str3;
    this.chart_label = str2;
    if (this.rdo_chart.SelectedItem.Value == "duration")
      this.html_title = "Total Duration (Mins.)";
    else
      this.html_title = "Total Bookings";
  }

  private void ui_utilization_table(DataTable table)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["building"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["level"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["room"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["total_bookings"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["total_duration"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["max_duration"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["min_duration"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["avg_duration"].ToString() + "</td>");
      stringBuilder.Append("</tr>");
    }
    this.html_details_table = stringBuilder.ToString();
  }

  private void ui_top_10(DataTable table)
  {
    int num = table.Rows.Count <= 10 ? table.Rows.Count : 10;
    DataView defaultView = table.DefaultView;
    defaultView.Sort = "total_bookings desc";
    DataTable table1 = defaultView.ToTable();
    for (int index = 0; index < num; ++index)
    {
      DataRow row = table1.Rows[index];
      this.top_10_room_names = this.top_10_room_names + "'" + row["room"].ToString() + "'";
      this.top_10_numbers = this.top_10_numbers + "'" + row["total_duration"].ToString() + "'";
      if (index < num - 1)
      {
        this.top_10_room_names += ",";
        this.top_10_numbers += ",";
      }
    }
  }

  protected void btn_filter_Click(object sender, EventArgs e) => this.get_report(this.current_user.account_id, Convert.ToDateTime(this.hdn_report_start.Value), Convert.ToDateTime(this.hdn_report_end.Value));

  protected void btn_filters_Click(object sender, EventArgs e) => this.get_report(this.current_user.account_id, Convert.ToDateTime(this.hdn_report_start.Value), Convert.ToDateTime(this.hdn_report_end.Value));
}
