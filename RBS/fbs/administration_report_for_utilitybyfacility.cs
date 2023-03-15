// Decompiled with JetBrains decompiler
// Type: administration_report_for_utilitybyfacility
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

public class administration_report_for_utilitybyfacility : fbs_base_page, IRequiresSessionState
{
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Button btnExportExcel;
  protected DropDownList ddlYear;
  protected DropDownList ddlMonth;
  protected DropDownList ddlBuilding;
  protected DropDownList ddlLevel;
  protected DropDownList ddlCategory;
  protected Button btn_filter;
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["report_util_room"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (this.IsPostBack)
        return;
      this.bind_all_dropdown();
      this.ddlYear.Items.FindByText(this.current_timestamp.Year.ToString()).Selected = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Utility Reporting By Room Error: ->", ex);
    }
  }

  private void bind_all_dropdown()
  {
    try
    {
      DataSet settings = this.settings.get_settings(this.current_user.account_id);
      DataRow[] dataRowArray1 = settings.Tables[0].Select("parameter='building'");
      DataRow[] dataRowArray2 = settings.Tables[0].Select("parameter='level'");
      DataRow[] dataRowArray3 = settings.Tables[0].Select("parameter='category'");
      foreach (DataRow dataRow in dataRowArray1)
        this.ddlBuilding.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlBuilding.Items.Insert(0, new ListItem("All", ""));
      foreach (DataRow dataRow in dataRowArray2)
        this.ddlLevel.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlLevel.Items.Insert(0, new ListItem("All", ""));
      foreach (DataRow dataRow in dataRowArray3)
        this.ddlCategory.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlCategory.Items.Insert(0, new ListItem("All", ""));
      for (int index = this.current_timestamp.Year - 5; index < this.current_timestamp.Year + 5; ++index)
        this.ddlYear.Items.Add(index.ToString());
      this.ddlYear.Items.Insert(0, "Select Year");
      this.ddlYear.Items.FindByText(this.current_timestamp.Year.ToString()).Selected = true;
      this.ddlMonth.Items.FindByValue("0" + this.current_timestamp.Month.ToString()).Selected = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Buidling, Level, Category Binding - Utility Reporting By Room Error: ->", ex);
    }
  }

  protected void btn_filter_Click(object sender, EventArgs e) => this.html_table = this.populate_data();

  private string populate_data()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataSet settings = this.settings.get_settings(this.current_user.account_id);
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
      foreach (DataRow row in (InternalDataCollectionBase) settings.Tables[0].Rows)
      {
        if (row["parameter"].ToString() == "building")
          dictionary1.Add(row["setting_id"].ToString(), row["value"].ToString());
        if (row["parameter"].ToString() == "level")
          dictionary2.Add(row["setting_id"].ToString(), row["value"].ToString());
        if (row["parameter"].ToString() == "category")
          dictionary3.Add(row["setting_id"].ToString(), row["value"].ToString());
        if (row["parameter"].ToString() == "asset_type")
          dictionary4.Add(row["setting_id"].ToString(), row["value"].ToString());
      }
      DataSet assets = this.assets.get_assets(this.current_user.account_id);
      string fromdate;
      string todate;
      if (this.ddlMonth.SelectedItem.Value == "00")
      {
        fromdate = this.ddlYear.SelectedItem.Value + "-01-01 00:00:00";
        todate = this.ddlYear.SelectedItem.Value + "-12-31 23:59:59";
      }
      else
      {
        string str1 = this.ddlYear.SelectedValue + "-" + this.ddlMonth.SelectedValue + "-01";
        string str2 = this.LastDayOfMonthFromDateTime(Convert.ToDateTime(str1)).ToString("yyyy-MM-dd");
        fromdate = str1 + " 00:00:00.000";
        todate = str2 + " 23:59:59.999";
      }
      string where = "";
      if (this.ddlBuilding.SelectedValue != "")
        where = where + " AND building_id='" + this.ddlBuilding.SelectedValue + "'";
      if (this.ddlLevel.SelectedValue != "")
        where = where + " AND level_id='" + this.ddlLevel.SelectedValue + "'";
      if (this.ddlCategory.SelectedValue != "")
        where = where + " AND category_id='" + this.ddlCategory.SelectedValue + "'";
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet dataSet = this.reportings.utility_report_by_facility2(fromdate, todate, where, this.current_user.account_id, groupids);
      this.alertError.Attributes.Add("style", "display: none;");
      this.litErrorMsg.Text = "";
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table_report'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr >");
      stringBuilder.Append("<th class='hidden-480' >Building</th>");
      stringBuilder.Append("<th class='hidden-480' >Level</th>");
      stringBuilder.Append("<th class='hidden-480' >Room</th>");
      stringBuilder.Append("<th class='hidden-480' >Category</th>");
      stringBuilder.Append("<th class='hidden-480' >Room Type</th>");
      stringBuilder.Append("<th class='hidden-480' >Total No.Of Bookings</th>");
      stringBuilder.Append("<th class='hidden-480' >Total Hours Booked</th>");
      stringBuilder.Append("<th class='hidden-480' >Utilization %</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      int num1 = 0;
      double num2 = 0.0;
      foreach (DataRow dataRow in assets.Tables[0].Select("1=1 " + where))
      {
        DataRow[] dataRowArray1 = dataSet.Tables[0].Select("asset_id='" + dataRow["asset_id"].ToString() + "'");
        DataRow[] dataRowArray2 = dataSet.Tables[1].Select("asset_id='" + dataRow["asset_id"].ToString() + "'");
        double num3 = 0.0;
        double num4 = 0.0;
        if (dataRowArray1.Length > 0)
        {
          num3 = Convert.ToDouble(dataRowArray1[0]["TotalHoursBooked"]);
          num4 = Convert.ToDouble(dataRowArray1[0]["TotalNoOfBooked"]);
        }
        if (dataRowArray2.Length > 0)
        {
          num3 -= Convert.ToDouble(dataRowArray2[0]["TotalHoursBooked"]);
          num4 -= Convert.ToDouble(dataRowArray2[0]["TotalNoOfBooked"]);
        }
        stringBuilder.Append("<tr>");
        stringBuilder.AppendFormat("<td >{0}</td>", (object) dictionary1[dataRow["building_id"].ToString()]);
        stringBuilder.AppendFormat("<td >{0}</td>", (object) dictionary2[dataRow["level_id"].ToString()]);
        stringBuilder.AppendFormat("<td >{0}</td>", (object) dataRow["name"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) dictionary3[dataRow["category_id"].ToString()]);
        stringBuilder.AppendFormat("<td >{0}</td>", (object) dictionary4[dataRow["asset_type"].ToString()]);
        stringBuilder.AppendFormat("<td >{0}</td>", (object) num4.ToString());
        double num5 = num3 / 60.0;
        stringBuilder.AppendFormat("<td >{0}</td>", (object) string.Format("{0:0.##}", (object) num5));
        num1 += (int) num4;
        num2 += num5;
        int num6 = administration_report_for_utilitybyfacility.BusinessDaysUntil(Convert.ToDateTime(fromdate), Convert.ToDateTime(todate));
        Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(num5 / (double) num6)) * 100.0);
        double num7 = Convert.ToDouble(Convert.ToDouble(num5 / (double) (num6 * 15)) * 100.0);
        stringBuilder.AppendFormat("<td >{0}</td>", (object) string.Format("{0:0}", (object) num7));
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='5' style='font-Weight:bold;' >Total</td>");
      stringBuilder.AppendFormat("<td style='font-Weight:bold;' >{0}</td>", (object) num1);
      stringBuilder.AppendFormat("<td  style='font-Weight:bold;'>{0}</td>", (object) string.Format("{0:0.##}", (object) num2));
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Utility Reporting By Facility Error: ->", ex);
    }
    return stringBuilder.ToString();
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table>");
      stringBuilder.Append("<tr><td colspan='9'><h1>Bookings</h1></td></tr>");
      stringBuilder.Append("<tr><td colspan='9'><h2>Filter Criteria:</h2></td></tr>");
      stringBuilder.Append("<tr><td>Room Type/Equipment: </td><td colspan='8' style='text-align:left;'>" + this.ddlCategory.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Year: </td><td colspan='8' style='text-align:left;'>" + this.ddlYear.SelectedItem.Value + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Month: </td><td colspan='8' style='text-align:left;'>" + this.ddlMonth.SelectedItem.Value + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Building: </td><td colspan='8' style='text-align:left;'>" + this.ddlBuilding.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Level: </td><td colspan='8' style='text-align:left;'>" + this.ddlLevel.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("</table>");
      stringBuilder.Append(this.populate_data());
      stringBuilder.Append("<table>");
      stringBuilder.Append("<tr><td>Generated By: </td><td colspan='8' style='text-align:left;'>" + this.current_user.full_name + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Generated On: </td><td colspan='8' style='text-align:left;'>" + this.current_timestamp.ToString(api_constants.display_datetime_format) + "</br></td></tr>");
      stringBuilder.Append("</table>");
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Utility_report_by_facility_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = "application/vnd.xls";
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Export - Utility Reporting By Facility Error: ->", ex);
    }
  }

  public DateTime LastDayOfMonthFromDateTime(DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1.0);

  public static int BusinessDaysUntil(DateTime firstDay, DateTime lastDay)
  {
    firstDay = firstDay.Date;
    lastDay = lastDay.Date;
    if (firstDay > lastDay)
      throw new ArgumentException("Incorrect last day " + (object) lastDay);
    TimeSpan timeSpan = firstDay - lastDay;
    int num = 0;
    for (; DateTime.Compare(firstDay, lastDay) <= 0; firstDay = firstDay.AddDays(1.0))
    {
      if (firstDay.DayOfWeek != DayOfWeek.Saturday && firstDay.DayOfWeek != DayOfWeek.Sunday)
        ++num;
    }
    return num;
  }
}
