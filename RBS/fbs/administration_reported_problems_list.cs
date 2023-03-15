// Decompiled with JetBrains decompiler
// Type: administration_reported_problems_list
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
using System.Web.UI.WebControls;

public class administration_reported_problems_list : fbs_base_page, IRequiresSessionState
{
  protected Button btnExportExcel;
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.Session["user"] == null)
      this.Response.Redirect("~/error.aspx?message=not_authorized");
    try
    {
      if (this.IsPostBack)
        return;
      this.populate_ui("", "");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Reporting Error: ->", ex);
    }
  }

  protected void populate_ui(string fromdate, string todate)
  {
    try
    {
      DataSet dataSet = new DataSet();
      if (this.gp.isAdminType)
        dataSet = this.reportings.get_report_problem_admin(this.current_user.account_id, "", "", "", "", "");
      else if (this.gp.isSuperUserType)
        dataSet = this.reportings.get_report_problem_owner(this.current_user.user_id, this.current_user.account_id, "", "", "", "", "");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='tbl_report_problem'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Facility</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Level</th>");
      stringBuilder.Append("<th class='hidden-480'>Subject</th>");
      stringBuilder.Append("<th class='hidden-480'>Reported By</th>");
      stringBuilder.Append("<th class='hidden-480'>Reported On</th>");
      stringBuilder.Append("<th class='hidden-480'>View</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<body>");
      if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["building_name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["level_name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["Subject"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["reported_by_name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["Reported_on"].ToString()).AddHours(this.current_user.timezone).ToString("dd-MMM-yyyy hh:mm tt") + "</td>");
          stringBuilder.AppendFormat("<td><a href='javascript:ShowRoomInfo({0},{1})'>View</a></td>", (object) row["asset_id"].ToString(), (object) row["Problem_id"].ToString());
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("</body>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "No Show Reporting Error: ->", ex);
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      DataSet dataSet = new DataSet();
      if (this.gp.isAdminType)
        dataSet = this.reportings.get_report_problem_admin(this.current_user.account_id, "", "", "", "", "");
      else if (this.gp.isSuperUserType)
        dataSet = this.reportings.get_report_problem_owner(this.current_user.user_id, this.current_user.account_id, "", "", "", "", "");
      StringBuilder stringBuilder = new StringBuilder();
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_problem_report_" + DateTime.UtcNow.AddHours(this.current_account.timezone).ToString("ddMMyyyyhhmmss") + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = "application/vnd.xls";
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td style='font-size:24pt;' colspan='8'>Report A Problem List</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td style='border:none;' colspan='8'></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Date Range</td>");
      stringBuilder.AppendFormat("<td>From: {0}</td>");
      stringBuilder.AppendFormat("<td colspan='6'>To: {0}</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='8'></td></tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Facility</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Level</th>");
      stringBuilder.Append("<th class='hidden-480'>Subject</th>");
      stringBuilder.Append("<th class='hidden-480'>Remarks</th>");
      stringBuilder.Append("<th class='hidden-480'>Reported By</th>");
      stringBuilder.Append("<th class='hidden-480'>Reported On</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["building_name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["level_name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["Subject"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["remarks"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["reported_by_name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(row["Reported_on"].ToString()).AddHours(this.current_user.timezone).ToString("dd-MMM-yyyy hh:mm tt") + "</td>");
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='8'></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Generated On</td>");
      stringBuilder.AppendFormat("<td colspan='7' align='left'>{0}</td>", (object) DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(this.current_user.timezone).ToString("dd-MMM-yyyy hh:mm tt"));
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Generated By</td>");
      stringBuilder.AppendFormat("<td colspan='7'>{0}</td>", (object) this.current_user.full_name);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
      this.Response.Write(this.html_table.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Export No Show Reporting Error: ->", ex);
    }
  }
}
