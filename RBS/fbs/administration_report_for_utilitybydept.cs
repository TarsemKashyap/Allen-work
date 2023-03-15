// Decompiled with JetBrains decompiler
// Type: administration_report_for_utilitybydept
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_report_for_utilitybydept : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Button btnExportExcel;
  protected DropDownList ddlYear;
  protected DropDownList ddlMonth;
  protected DropDownList ddlDivision;
  protected DropDownList ddlDepartment;
  protected DropDownList ddlSection;
  protected Button btn_filter;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["report_util_department"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (this.IsPostBack)
        return;
      this.bind_all_dropdown();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Utility Reporting By Dept Error: ->", ex);
    }
  }

  private void bind_all_dropdown()
  {
    try
    {
      foreach (DataRow dataRow in this.reportings.get_divison_master(this.current_user.account_id).Tables[0].Select("type=1"))
        this.ddlDivision.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlDivision.Items.Insert(0, new ListItem("All", ""));
      for (int index = this.current_timestamp.Year - 5; index < this.current_timestamp.Year + 5; ++index)
        this.ddlYear.Items.Add(index.ToString());
      this.ddlYear.Items.Insert(0, "Select Year");
      this.ddlYear.Items.FindByText(this.current_timestamp.Year.ToString()).Selected = true;
      this.ddlMonth.Items.FindByValue("0" + this.current_timestamp.Month.ToString()).Selected = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Division On Change - Utility Reporting By Dept Error: ->", ex);
    }
  }

  protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      DataRow[] dataRowArray = this.reportings.get_divison_master(this.current_user.account_id).Tables[0].Select("type='2' AND parent_id='" + this.ddlDivision.SelectedValue + "'");
      this.ddlDepartment.Items.Clear();
      foreach (DataRow dataRow in dataRowArray)
        this.ddlDepartment.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlDepartment.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Utility Reporting By Dept Error: ->", ex);
    }
  }

  protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      DataRow[] dataRowArray = this.reportings.get_divison_master(this.current_user.account_id).Tables[0].Select("type='3' AND parent_id='" + this.ddlDepartment.SelectedValue + "'");
      this.ddlSection.Items.Clear();
      foreach (DataRow dataRow in dataRowArray)
        this.ddlSection.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlSection.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Utility Reporting By Dept Error: ->", ex);
    }
  }

  protected void btn_filter_Click(object sender, EventArgs e)
  {
    try
    {
      string str1 = this.ddlYear.SelectedValue + "-" + this.ddlMonth.SelectedValue + "-01";
      string str2 = this.LastDayOfMonthFromDateTime(Convert.ToDateTime(str1)).ToString(api_constants.sql_datetime_format_short);
      string fromdate = str1 + " 00:00:00.000";
      string todate = str2 + " 23:59:59.999";
      string where = "";
      if (this.ddlDivision.SelectedValue != "")
        where = where + " AND R.Division LIKE '" + this.ddlDivision.SelectedItem.Text + "'";
      if (this.ddlDepartment.SelectedValue != "")
        where = where + " AND R.Department LIKE '" + this.ddlDepartment.SelectedItem.Text + "'";
      if (this.ddlSection.SelectedValue != "")
        where = where + " AND R.Section LIKE '" + this.ddlSection.SelectedItem.Text + "'";
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet dataSet = this.reportings.utility_report_by_dept(fromdate, todate, where, this.current_user.account_id, groupids);
      this.alertError.Attributes.Add("style", "display: none;");
      this.litErrorMsg.Text = "";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table_report'>");
      stringBuilder.Append("<thead style='font-Weight:bold;'>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480' >Division</th>");
      stringBuilder.Append("<th class='hidden-480' >Department</th>");
      stringBuilder.Append("<th class='hidden-480' >Section</th>");
      stringBuilder.Append("<th class='hidden-480' >Room</th>");
      stringBuilder.Append("<th class='hidden-480' >Category</th>");
      stringBuilder.Append("<th class='hidden-480' >Room Type</th>");
      stringBuilder.Append("<th class='hidden-480' >Total No.Of Bookings</th>");
      stringBuilder.Append("<th class='hidden-480' >Total Hours Booked</th>");
      stringBuilder.Append("<th class='hidden-480' style='width:100px'>Monthly Utilization %</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      int num1 = 0;
      double num2 = 0.0;
      foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        stringBuilder.Append("<tr >");
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["Division"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["Department"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["Section"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["name"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["Category"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["RoomType"].ToString());
        stringBuilder.AppendFormat("<td >{0}</td>", (object) row["TotalNoOfBooked"].ToString());
        double num3 = Convert.ToDouble(row["TotalHoursBooked"].ToString()) / 60.0;
        stringBuilder.AppendFormat("<td >{0}</td>", (object) string.Format("{0:0.##}", (object) num3));
        num1 += Convert.ToInt32(row["TotalNoOfBooked"]);
        num2 += num3;
        int num4 = administration_report_for_utilitybydept.BusinessDaysUntil(Convert.ToDateTime(fromdate), Convert.ToDateTime(todate));
        Convert.ToDouble(num3 / (double) num4);
        double num5 = Convert.ToDouble(Convert.ToDouble(num3 / (double) (num4 * 15)) * 100.0);
        Convert.ToDouble(num5 / (double) num4);
        stringBuilder.AppendFormat("<td >{0}</td>", (object) string.Format("{0:0}", (object) num5));
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='6' style='font-Weight:bold;'  >Total</td>");
      stringBuilder.AppendFormat("<td style='font-Weight:bold;' >{0}</td>", (object) num1);
      stringBuilder.AppendFormat("<td style='font-Weight:bold;' >{0}</td>", (object) string.Format("{0:0.##}", (object) num2));
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Utility Reporting By Dept Error: ->", ex);
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      string str1 = this.ddlYear.SelectedValue + "-" + this.ddlMonth.SelectedValue + "-01";
      string str2 = this.LastDayOfMonthFromDateTime(Convert.ToDateTime(str1)).ToString(api_constants.sql_datetime_format_short);
      string fromdate = str1 + " 00:00:00.000";
      string todate = str2 + " 23:59:59.999";
      string where = "";
      if (this.ddlDivision.SelectedValue != "")
        where = where + " AND R.Division LIKE '" + this.ddlDivision.SelectedItem.Text + "'";
      if (this.ddlDepartment.SelectedValue != "")
        where = where + " AND R.Department LIKE '" + this.ddlDepartment.SelectedItem.Text + "'";
      if (this.ddlSection.SelectedValue != "")
        where = where + " AND R.Section LIKE '" + this.ddlSection.SelectedItem.Text + "'";
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet ds = this.reportings.utility_report_by_dept(fromdate, todate, where, this.current_user.account_id, groupids);
      if (ds != null)
      {
        if (!this.utilities.isValidDataset(ds))
        {
          this.alertError.Attributes.Add("style", "display: block;");
          this.litErrorMsg.Text = Resources.fbs.export_excel_no_data_msg;
          return;
        }
        this.alertError.Attributes.Add("style", "display: none;");
        this.litErrorMsg.Text = "";
      }
      StringBuilder stringBuilder = new StringBuilder();
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Utility_report_by_dept_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td style='font-size:24pt;' colspan='3'>Utilization Report (Department)</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Month</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.ddlMonth.SelectedItem.Text);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Year</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.ddlYear.SelectedValue);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Division</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.ddlDivision.SelectedItem.Text);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Department</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.ddlDepartment.SelectedItem.Text);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Section</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.ddlSection.SelectedItem.Text);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table_report' border='1'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<th class='hidden-480'>Division</th>");
      stringBuilder.Append("<th class='hidden-480'>Department</th>");
      stringBuilder.Append("<th class='hidden-480'>Section</th>");
      stringBuilder.Append("<th class='hidden-480'>Room</th>");
      stringBuilder.Append("<th class='hidden-480'>Category</th>");
      stringBuilder.Append("<th class='hidden-480'>Room Type</th>");
      stringBuilder.Append("<th class='hidden-480'>Total No.Of Bookings</th>");
      stringBuilder.Append("<th class='hidden-480'>Total Hours Booked</th>");
      stringBuilder.Append("<th class='hidden-480'>Monthly Utilization %</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      int num1 = 0;
      double num2 = 0.0;
      foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
      {
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td></td>");
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Department"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Section"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Category"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["RoomType"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["TotalNoOfBooked"].ToString());
        double num3 = Convert.ToDouble(row["TotalHoursBooked"].ToString()) / 60.0;
        stringBuilder.AppendFormat("<td>{0}</td>", (object) string.Format("{0:0.##}", (object) num3));
        num1 += Convert.ToInt32(row["TotalNoOfBooked"]);
        num2 += num3;
        int num4 = administration_report_for_utilitybydept.BusinessDaysUntil(Convert.ToDateTime(fromdate), Convert.ToDateTime(todate));
        Convert.ToDouble(num3 / (double) num4);
        double num5 = Convert.ToDouble(Convert.ToDouble(num3 / (double) (num4 * 15)) * 100.0);
        Convert.ToDouble(num5 / (double) num4);
        stringBuilder.AppendFormat("<td>{0}</td>", (object) string.Format("{0:0}", (object) num5));
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td colspan='6'>Total</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) num1);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) string.Format("{0:0.##}", (object) num2));
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Generated On</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format));
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Generated By</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.current_user.full_name);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
      this.Response.Write(this.html_table.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Export - Utility Reporting By Dept Error: ->", ex);
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

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
