// Decompiled with JetBrains decompiler
// Type: administration_report_unassigned_report
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

public class administration_report_unassigned_report : fbs_base_page, IRequiresSessionState
{
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected HiddenField hdn_original_end;
  protected HiddenField hdn_original_start;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Button btnExportExcel;
  public string html_table = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["report_unassigned"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (this.IsPostBack)
        return;
      this.html_table = this.pageload_Data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected string table_header()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='unassigned_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th class='hidden-480' width='12%'>Name</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Email</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Username</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>No. of. Bookings</th>");
    stringBuilder.Append("<th class='hidden-480' width='8%'>Action</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    stringBuilder.Append("<tbody>");
    return stringBuilder.ToString();
  }

  protected string pageload_Data()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append(this.table_header());
      DataSet inactiveUsers = this.reportings.get_inactive_users(this.current_user.account_id);
      string ids = "0";
      foreach (DataRow row in (InternalDataCollectionBase) inactiveUsers.Tables[0].Rows)
        ids = ids + "," + row["user_id"];
      DataSet userBookingCount = this.reportings.get_future_inactive_user_booking_count(this.current_user.account_id, ids, DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(this.current_account.timezone));
      if (userBookingCount.Tables[0].Rows.Count == 0)
      {
        stringBuilder.Append("<tr class='odd gradeX'><td colspan='5'>There are no records.</td></tr>");
      }
      else
      {
        foreach (DataRow row in (InternalDataCollectionBase) userBookingCount.Tables[0].Rows)
        {
          DataRow[] dataRowArray = inactiveUsers.Tables[0].Select("user_id='" + row["created_by"].ToString() + "'");
          if (dataRowArray.Length > 0)
          {
            DataRow dataRow = dataRowArray[0];
            stringBuilder.Append("<tr class='odd gradeX'>");
            stringBuilder.Append("<td>" + dataRow["full_name"] + "</td>");
            stringBuilder.Append("<td>" + dataRow["username"] + "</td>");
            stringBuilder.Append("<td>" + dataRow["email"] + "</td>");
            stringBuilder.Append("<td>" + row["count"] + "</td>");
            stringBuilder.Append("<td>");
            stringBuilder.Append("<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
            stringBuilder.Append("<ul class='ddm p-r'>");
            stringBuilder.AppendFormat("<li><a href='reassign_unassign.aspx?id={0}'><i class='icon-pencil'></i> View Details</a></li>", (object) dataRow["user_id"].ToString());
            stringBuilder.Append("</ul>");
            stringBuilder.Append("</div></div></td>");
            stringBuilder.Append("</tr>");
          }
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return stringBuilder.ToString();
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='font-size:x-large;'>Unassigned Report List</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</table>");
      stringBuilder.Append(this.pageload_Data());
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tr><td>Generated on </td>");
      stringBuilder.Append("<td align='left'>" + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format) + "</td></tr>");
      stringBuilder.Append("<tr><td>Generated by </td>");
      stringBuilder.Append("<td align='left'>" + this.current_user.full_name + "</td></tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=UnassignedReport_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
