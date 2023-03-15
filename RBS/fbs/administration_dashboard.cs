// Decompiled with JetBrains decompiler
// Type: administration_dashboard
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

public class administration_dashboard : fbs_base_page, IRequiresSessionState
{
  private dashboard_api dapi;
  public string html_start_range;
  private Dictionary<int, string> bgs;
  public string html_bookings_by_room;
  public string html_bookings_by_user;
  public string html_cancels_by_room;
  public string html_cancels_by_user;
  public string html_booking_bar_chart;
  public string html_booking_lead_time_bar_chart;
  public string html_cancel_lead_time_bar_chart;
  public string html_booking_hour_bar_chart;
  public string html_cancel_hour_bar_chart;
  public int total_bookings;
  public int total_cancels;
  public double total_duration;
  public string html_avg_duration = "";
  public string html_min_duration = "";
  public string html_max_duration = "";
  protected Label lblDateRage;
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected Button btn_filter;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.dapi = new dashboard_api();
    this.bgs = new Dictionary<int, string>();
    this.bgs.Add(1, "rgb(244,67,54)");
    this.bgs.Add(2, "rgb(233,30,99)");
    this.bgs.Add(3, "rgb(156,39,76)");
    this.bgs.Add(4, "rgb(33,150,243)");
    this.bgs.Add(5, "rgb(3,169,244)");
    this.bgs.Add(6, "rgb(0,188,212)");
    this.bgs.Add(7, "rgb(0,150,136)");
    this.bgs.Add(8, "rgb(76,175,80)");
    this.bgs.Add(9, "rgb(139,195,74)");
    this.bgs.Add(10, "rgb(205,220,57)");
    this.bgs.Add(11, "rgb(244,67,54)");
    if (!this.IsPostBack)
    {
      DateTime dateTime = DateTime.UtcNow.AddHours(this.current_account.timezone);
      dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
      this.hdn_log_start.Value = dateTime.AddDays(-30.0).ToString("yyyy-MM-dd");
      this.hdn_log_end.Value = dateTime.AddDays(1.0).ToString("yyyy-MM-dd");
      this.populate_dashboard(dateTime.AddDays(-30.0), dateTime.AddDays(1.0));
    }
    this.html_start_range = "$('#form-date-range-log span').html('" + Convert.ToDateTime(this.hdn_log_start.Value).ToString("dd-MMM-yyyy") + " to " + Convert.ToDateTime(this.hdn_log_end.Value).ToString("dd-MMM-yyyy") + "');";
  }

  private void populate_dashboard(DateTime from, DateTime to)
  {
    this.populate_stats(from, to);
    this.html_booking_bar_chart = this.populate_booking_bar_chart(from, to);
    this.html_booking_lead_time_bar_chart = this.populate_booking_lead_times(from, to);
    this.html_cancel_lead_time_bar_chart = this.populate_cancel_lead_times(from, to);
    this.html_bookings_by_room = this.populate_bookings_by_room(from, to);
    this.html_bookings_by_user = this.populate_bookings_by_user(from, to);
    this.html_cancels_by_room = this.populate_cancels_by_room(from, to);
    this.html_cancels_by_user = this.populate_cancels_by_user(from, to);
    this.html_booking_hour_bar_chart = this.populate_booking_hour(from, to);
    this.html_cancel_hour_bar_chart = this.populate_cancel_hour(from, to);
  }

  private void populate_stats(DateTime from, DateTime to)
  {
    DataSet dataSet = new DataSet();
    if (this.gp.isAdminType)
      dataSet = this.dapi.get_stats(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataSet = this.dapi.get_stats(from, to, this.current_user.account_id, this.current_user.user_id);
    if (dataSet.Tables[0].Rows.Count <= 0)
      return;
    DataRow row = dataSet.Tables[0].Rows[0];
    int num = 0;
    if (row["average"] != DBNull.Value)
      num = Convert.ToInt32(row["average"]);
    if (row["min"] != DBNull.Value)
      Convert.ToInt32(row["min"]);
    if (row["max"] != DBNull.Value)
      Convert.ToInt32(row["max"]);
    if (num < 60)
      this.html_avg_duration = num.ToString() + " mins.";
    if (num < 60)
      return;
    this.html_avg_duration = (num / 60).ToString() + " hrs.";
  }

  private string populate_booking_hour(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string newValue1 = "Top 10 Bookings";
    string str3 = "<canvas id='canvas-" + (object) guid + "' height='400' width=''></canvas><script>var barChartData4 = {labels: [#labels#],datasets: [{ label: 'Booking Lead Times',backgroundColor: 'rgb(54, 162, 235)',borderColor: 'rgb(54, 162, 235)', borderWidth: 1, data: [#data#] }]};function populate_booking_hour(){var ctx4 = document.getElementById('canvas-" + (object) guid + "').getContext('2d'); var b4 = new Chart(ctx4, { type: 'bar', data: barChartData4, options: { responsive: true,maintainAspectRatio: false, legend: { position: 'top', }, title: { display: false } } });};</script>";
    DataSet dataSet1 = new DataSet();
    DataSet dataSet2 = !this.gp.isAdminType ? this.dapi.get_booking_hour(from, to, this.current_user.account_id, this.current_user.user_id) : this.dapi.get_booking_hour(from, to, this.current_user.account_id);
    DateTime dateTime = new DateTime(2000, 1, 1);
    for (int index = 0; index < 24; ++index)
    {
      str1 = str1 + "'" + dateTime.ToString("hh tt") + " - " + dateTime.AddHours(1.0).ToString("hh tt") + "',";
      str2 = str2 + "'" + dataSet2.Tables[index].Rows[0][0].ToString() + "',";
      dateTime = dateTime.AddHours(1.0);
    }
    string newValue2 = str1.Substring(0, str1.Length - 1);
    string newValue3 = str2.Substring(0, str2.Length - 1);
    return str3.Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#data#", newValue3);
  }

  private string populate_cancel_hour(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string newValue1 = "Top 10 Bookings";
    string str3 = "<canvas id='canvas-" + (object) guid + "' height='400' width=''></canvas><script>var barChartData5 = {labels: [#labels#],datasets: [{ label: 'Booking Lead Times',backgroundColor: 'rgb(255, 99, 132)',borderColor: 'rgb(255, 99, 132)', borderWidth: 1, data: [#data#] }]};function populate_cancel_hour(){var ctx5 = document.getElementById('canvas-" + (object) guid + "').getContext('2d'); var b5 = new Chart(ctx5, { type: 'bar', data: barChartData5, options: { responsive: true,maintainAspectRatio: false, legend: { position: 'top', }, title: { display: false } } });};</script>";
    DataSet dataSet = new DataSet();
    if (this.gp.isAdminType)
      dataSet = this.dapi.get_cancel_hour(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataSet = this.dapi.get_cancel_hour(from, to, this.current_user.account_id, this.current_user.user_id);
    DateTime dateTime = new DateTime(2000, 1, 1);
    for (int index = 0; index < 24; ++index)
    {
      str1 = str1 + "'" + dateTime.ToString("hh tt") + " - " + dateTime.AddHours(1.0).ToString("hh tt") + "',";
      str2 = str2 + "'" + dataSet.Tables[index].Rows[0][0].ToString() + "',";
      dateTime = dateTime.AddHours(1.0);
    }
    string newValue2 = str1.Substring(0, str1.Length - 1);
    string newValue3 = str2.Substring(0, str2.Length - 1);
    return str3.Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#data#", newValue3);
  }

  private string populate_booking_lead_times(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string newValue1 = "Top 10 Bookings";
    string str3 = "<canvas id='canvas-" + (object) guid + "' height='400' width=''></canvas><script>var barChartData2 = {labels: [#labels#],datasets: [{ label: 'Booking Lead Times',backgroundColor: 'rgb(75, 192, 192)',borderColor: 'rgb(75, 192, 192)', borderWidth: 1, data: [#data#] }]};function populate_booking_lead_times(){var ctx2 = document.getElementById('canvas-" + (object) guid + "').getContext('2d'); var b2 = new Chart(ctx2, { type: 'bar', data: barChartData2, options: { responsive: true,maintainAspectRatio: false, legend: { position: 'top', }, title: { display: false } } });};</script>";
    DataTable dataTable1 = new DataTable();
    DataTable dataTable2 = !this.gp.isAdminType ? this.dapi.get_booking_lead_times(from, to, this.current_user.account_id, this.current_user.user_id) : this.dapi.get_booking_lead_times(from, to, this.current_user.account_id);
    int key = 0;
    foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
    {
      str1 = str1 + "'" + this.dapi.range_text[key] + "',";
      str2 = str2 + "'" + row["count"].ToString() + "',";
      ++key;
    }
    string newValue2 = str1.Substring(0, str1.Length - 1);
    string newValue3 = str2.Substring(0, str2.Length - 1);
    return str3.Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#data#", newValue3);
  }

  private string populate_cancel_lead_times(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string newValue1 = "Top 10 Bookings";
    string str3 = "<canvas id='canvas-" + (object) guid + "' height='400' width=''></canvas><script>var barChartData3 = {labels: [#labels#],datasets: [{ label: 'Booking Lead Times',backgroundColor: 'rgb(255, 99, 132)',borderColor: 'rgb(255, 99, 132)', borderWidth: 1, data: [#data#] }]};function populate_cancel_lead_times(){var ctx3 = document.getElementById('canvas-" + (object) guid + "').getContext('2d'); var b3 = new Chart(ctx3, { type: 'bar', data: barChartData3, options: { responsive: true,maintainAspectRatio: false, legend: { position: 'top', }, title: { display: false } } });};</script>";
    DataTable dataTable = new DataTable();
    if (this.gp.isAdminType)
      dataTable = this.dapi.get_cancel_lead_times(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataTable = this.dapi.get_cancel_lead_times(from, to, this.current_user.account_id, this.current_user.user_id);
    int key = 0;
    foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
    {
      str1 = str1 + "'" + this.dapi.cancel_range_text[key] + "',";
      str2 = str2 + "'" + row["count"].ToString() + "',";
      ++key;
    }
    string newValue2 = str1.Substring(0, str1.Length - 1);
    string newValue3 = str2.Substring(0, str2.Length - 1);
    return str3.Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#data#", newValue3);
  }

  private string populate_booking_bar_chart(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string newValue1 = "Top 10 Bookings";
    string str3 = "<canvas id='canvas-" + (object) guid + "' height='400' width=''></canvas><script>var barChartData1 = {labels: [#labels#],datasets: [{ label: 'Daily Booking Count',backgroundColor: 'rgb(54, 162, 235)',borderColor: 'rgb(54, 162, 235)', borderWidth: 1, data: [#data#] }]};function populate_booking_bar_chart(){var ctx1 = document.getElementById('canvas-" + (object) guid + "').getContext('2d'); var b1 = new Chart(ctx1, { type: 'bar', data: barChartData1, options: { responsive: true,maintainAspectRatio: false, legend: { position: 'top', }, title: { display: false } } });};</script>";
    DataSet dataSet = new DataSet();
    if (this.gp.isAdminType)
      dataSet = this.dapi.get_bookings_by_date(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataSet = this.dapi.get_bookings_by_date(from, to, this.current_user.account_id, this.current_user.user_id);
    string str4;
    if (dataSet.Tables[0].Rows.Count > 0)
    {
      for (DateTime dateTime = from; dateTime <= to; dateTime = dateTime.AddDays(1.0))
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("dt='" + dateTime.ToString("yyyy-MM-dd") + "'");
        if (dataRowArray.Length > 0)
        {
          str1 = str1 + "'" + dateTime.ToString("ddd, dd-MMM-yy") + "',";
          str2 = str2 + "'" + dataRowArray[0]["count"].ToString() + "',";
        }
        else
        {
          str1 = str1 + "'" + dateTime.ToString("ddd, dd-MMM-yy") + "',";
          str2 += "'0',";
        }
      }
      string newValue2 = str1.Substring(0, str1.Length - 1);
      string newValue3 = str2.Substring(0, str2.Length - 1);
      str4 = str3.Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#data#", newValue3);
    }
    else
      str4 = "No Data Available.";
    return str4;
  }

  private string populate_bookings_by_room(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string str3 = "";
    string newValue1 = "Top 10 Bookings";
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<canvas id='chartjs-" + (object) guid + "' class='chartjs' width='' height='100'></canvas>");
    stringBuilder.Append("<script>new Chart(document.getElementById('chartjs-" + (object) guid + "'),{'type':'doughnut','data':{'labels':[#labels#],'datasets':[{'label':'#title#','data':[#values#],'backgroundColor':[#bg#]}]}});</script>");
    DataSet dataSet = new DataSet();
    if (this.gp.isAdminType)
      dataSet = this.dapi.get_bookings_by_asset(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataSet = this.dapi.get_bookings_by_asset(from, to, this.current_user.account_id, this.current_user.user_id);
    string str4;
    if (dataSet.Tables[0].Rows.Count > 0)
    {
      int key = 1;
      foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        if (key <= 10)
        {
          str1 = str1 + "'" + row["name"].ToString() + "',";
          str2 = str2 + "'" + row["value"].ToString() + "',";
          str3 = str3 + "'" + this.bgs[key] + "',";
        }
        ++key;
        this.total_bookings += Convert.ToInt32(row["value"]);
        this.total_duration += Convert.ToDouble(row["duration"]);
      }
      this.total_duration /= 60.0;
      string newValue2 = str1.Substring(0, str1.Length - 1);
      string newValue3 = str2.Substring(0, str2.Length - 1);
      string newValue4 = str3.Substring(0, str3.Length - 1);
      str4 = stringBuilder.ToString().Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#values#", newValue3).Replace("#bg#", newValue4);
    }
    else
      str4 = "No Data Available.";
    return str4;
  }

  private string populate_bookings_by_user(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string str3 = "";
    string newValue1 = "Top 10 Bookings";
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<canvas id='chartjs-" + (object) guid + "' class='chartjs' width='' height='100'></canvas>");
    stringBuilder.Append("<script>new Chart(document.getElementById('chartjs-" + (object) guid + "'),{'type':'doughnut','data':{'labels':[#labels#],'datasets':[{'label':'#title#','data':[#values#],'backgroundColor':[#bg#]}]}});</script>");
    DataSet dataSet = new DataSet();
    if (this.gp.isAdminType)
      dataSet = this.dapi.get_bookings_by_user(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataSet = this.dapi.get_bookings_by_user(from, to, this.current_user.account_id, this.current_user.user_id);
    string str4;
    if (dataSet.Tables[0].Rows.Count > 0)
    {
      int key = 1;
      foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        if (key <= 10)
        {
          str1 = str1 + "'" + row["name"].ToString() + "',";
          str2 = str2 + "'" + row["value"].ToString() + "',";
          str3 = str3 + "'" + this.bgs[key] + "',";
        }
        ++key;
      }
      string newValue2 = str1.Substring(0, str1.Length - 1);
      string newValue3 = str2.Substring(0, str2.Length - 1);
      string newValue4 = str3.Substring(0, str3.Length - 1);
      str4 = stringBuilder.ToString().Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#values#", newValue3).Replace("#bg#", newValue4);
    }
    else
      str4 = "No Data Available.";
    return str4;
  }

  private string populate_cancels_by_room(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string str3 = "";
    string newValue1 = "Top 10 Bookings";
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<canvas id='chartjs-" + (object) guid + "' class='chartjs' width='' height='100'></canvas>");
    stringBuilder.Append("<script>new Chart(document.getElementById('chartjs-" + (object) guid + "'),{'type':'doughnut','data':{'labels':[#labels#],'datasets':[{'label':'#title#','data':[#values#],'backgroundColor':[#bg#]}]}});</script>");
    DataSet dataSet = new DataSet();
    if (this.gp.isAdminType)
      dataSet = this.dapi.get_cancels_by_asset(from, to, this.current_user.account_id);
    else if (this.gp.isSuperUserType)
      dataSet = this.dapi.get_cancels_by_asset(from, to, this.current_user.account_id, this.current_user.user_id);
    string str4;
    if (dataSet.Tables[0].Rows.Count > 0)
    {
      int key = 1;
      foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        if (key <= 10)
        {
          str1 = str1 + "'" + row["name"].ToString() + "',";
          str2 = str2 + "'" + row["value"].ToString() + "',";
          str3 = str3 + "'" + this.bgs[key] + "',";
        }
        ++key;
        this.total_cancels += Convert.ToInt32(row["value"]);
      }
      string newValue2 = str1.Substring(0, str1.Length - 1);
      string newValue3 = str2.Substring(0, str2.Length - 1);
      string newValue4 = str3.Substring(0, str3.Length - 1);
      str4 = stringBuilder.ToString().Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#values#", newValue3).Replace("#bg#", newValue4);
    }
    else
      str4 = "No Data Available.";
    return str4;
  }

  private string populate_cancels_by_user(DateTime from, DateTime to)
  {
    Guid guid = Guid.NewGuid();
    string str1 = "";
    string str2 = "";
    string str3 = "";
    string newValue1 = "Top 10 Bookings";
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<canvas id='chartjs-" + (object) guid + "' class='chartjs' width='' height='100'></canvas>");
    stringBuilder.Append("<script>new Chart(document.getElementById('chartjs-" + (object) guid + "'),{'type':'doughnut','data':{'labels':[#labels#],'datasets':[{'label':'#title#','data':[#values#],'backgroundColor':[#bg#]}]}});</script>");
    DataSet dataSet1 = new DataSet();
    DataSet dataSet2 = !this.gp.isAdminType ? this.dapi.get_cancels_by_user(from, to, this.current_user.account_id, this.current_user.user_id) : this.dapi.get_cancels_by_user(from, to, this.current_user.account_id);
    string str4;
    if (dataSet2.Tables[0].Rows.Count > 0)
    {
      int key = 1;
      foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
      {
        str1 = str1 + "'" + row["name"].ToString() + "',";
        str2 = str2 + "'" + row["value"].ToString() + "',";
        str3 = str3 + "'" + this.bgs[key] + "',";
        ++key;
        if (key == 10)
          break;
      }
      string newValue2 = str1.Substring(0, str1.Length - 1);
      string newValue3 = str2.Substring(0, str2.Length - 1);
      string newValue4 = str3.Substring(0, str3.Length - 1);
      str4 = stringBuilder.ToString().Replace("#labels#", newValue2).Replace("#title#", newValue1).Replace("#values#", newValue3).Replace("#bg#", newValue4);
    }
    else
      str4 = "No Data Available.";
    return str4;
  }

  protected void btn_filter_Click(object sender, EventArgs e) => this.populate_dashboard(Convert.ToDateTime(this.hdn_log_start.Value), Convert.ToDateTime(this.hdn_log_end.Value).AddDays(1.0));

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
