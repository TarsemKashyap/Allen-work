// Decompiled with JetBrains decompiler
// Type: report_housekeeping_report
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

public class report_housekeeping_report : fbs_base_page, IRequiresSessionState
{
  public string html_table = "";
  public string html_header = "";
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected HiddenField hdn_original_end;
  protected HiddenField hdn_original_start;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Button btnExportExcel;
  protected Label lblDateRage;
  protected Button btn_filter;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["report_housekeeping"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      this.Talbe_Header();
      if (this.IsPostBack)
        return;
      if (this.hdn_original_end.Value != "" && this.hdn_original_start.Value != "")
        this.pageload_Data(this.hdn_original_start.Value, this.hdn_original_end.Value);
      else
        this.pageload_Data(DateTime.Today.ToString(api_constants.sql_datetime_format_short), DateTime.Today.ToString(api_constants.sql_datetime_format_short));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Talbe_Header()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='housekeeping_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th class='hidden-480' width='12%'>Requestor Name</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Department</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Contact No</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Setup Type</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Building</th>");
    stringBuilder.Append("<th class='hidden-480' width='8%'>Level</th>");
    stringBuilder.Append("<th class='hidden-480' width='10%'>Room</th>");
    stringBuilder.Append("<th class='hidden-480' width='11%'>From</th>");
    stringBuilder.Append("<th class='hidden-480' width='11%'>To</th>");
    stringBuilder.Append("<th class='hidden-480' width='8%'>Status</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    stringBuilder.Append("<tbody>");
    this.html_header = stringBuilder.ToString();
  }

  protected void pageload_Data(string Fromdate, string Todate)
  {
    try
    {
      Fromdate += " 00:00:00 AM";
      Todate += " 23:59:59 PM";
      StringBuilder stringBuilder = new StringBuilder();
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet unassignedData = this.reportings.getUnassignedData("Housekepping", Fromdate, Todate, groupids, this.current_user.account_id);
      this.alertError.Attributes.Add("style", "display: none;");
      this.litErrorMsg.Text = "";
      if (unassignedData != null && unassignedData.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) unassignedData.Tables[0].Rows)
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + row["full_name"] + "</td>");
          stringBuilder.Append("<td>" + row["Department"] + "</td>");
          stringBuilder.Append("<td>" + row["contact"] + "</td>");
          stringBuilder.Append("<td>" + row["Setup_type"] + "</td>");
          stringBuilder.Append("<td>" + row["BuilidingName"] + "</td>");
          stringBuilder.Append("<td>" + row["LEVEL"] + "</td>");
          stringBuilder.Append("<td> " + row["RoomName"] + " </td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          if (row["BookedStatus"].ToString() == "Pending")
            stringBuilder.Append("<td><Span class='label label-Pending'> Pending</Span></td>");
          else
            stringBuilder.Append("<td><Span class='label label-Booked'> Booked </Span></td>");
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_filter_Click(object sender, EventArgs e)
  {
    try
    {
      if (this.hdn_original_end.Value != "" && this.hdn_original_start.Value != "")
        this.pageload_Data(this.hdn_original_start.Value, this.hdn_original_end.Value);
      else
        this.pageload_Data(DateTime.Today.ToString(api_constants.sql_datetime_format_short), DateTime.Today.ToString(api_constants.sql_datetime_format_short));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet ds = !(this.hdn_original_end.Value != "") || !(this.hdn_original_start.Value != "") ? this.reportings.getUnassignedData("Housekepping", DateTime.Today.ToString(api_constants.sql_datetime_format_short) + " 00:00:00 AM", DateTime.Today.ToString(api_constants.sql_datetime_format_short) + " 23:59:59 PM", groupids, this.current_user.account_id) : this.reportings.getUnassignedData("Housekepping", this.hdn_original_start.Value + " 00:00:00 AM", this.hdn_original_end.Value + " 23:59:59 PM", groupids, this.current_user.account_id);
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
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=HousekeepingReport_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='font-size:x-large;'>Housekeeping Report List</th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Date Range</th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr class='odd gradeX'>");
      if (this.hdn_original_start.Value != "" && this.hdn_original_end.Value != "")
        stringBuilder.Append("<td> " + this.hdn_original_start.Value + "&" + this.hdn_original_end.Value + "</td>");
      else
        stringBuilder.Append("<td> " + DateTime.Today.ToString(api_constants.sql_datetime_format_short) + "&" + DateTime.Today.ToString(api_constants.sql_datetime_format_short) + "</td>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Requestor Name</th>");
      stringBuilder.Append("<th>Department</th>");
      stringBuilder.Append("<th>Contact No</th>");
      stringBuilder.Append("<th>Setup Type</th>");
      stringBuilder.Append("<th>Building</th>");
      stringBuilder.Append("<th>Level</th>");
      stringBuilder.Append("<th>Room</th>");
      stringBuilder.Append("<th>From</th>");
      stringBuilder.Append("<th>To</th>");
      stringBuilder.Append("<th>Status</th>");
      stringBuilder.Append("</tr>");
      if (ds != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + row["full_name"] + "</td>");
          stringBuilder.Append("<td>" + row["Department"] + "</td>");
          stringBuilder.Append("<td>" + row["contact"] + "</td>");
          stringBuilder.Append("<td>" + row["Setup_type"] + "</td>");
          stringBuilder.Append("<td>" + row["BuilidingName"] + "</td>");
          stringBuilder.Append("<td>" + row["LEVEL"] + "</td>");
          stringBuilder.Append("<td> " + row["RoomName"] + " </td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td> " + row["Bookedstatus"] + " </td>");
          stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th>Generated By </th>");
        stringBuilder.Append("<td>" + this.current_user.full_name + " </td>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th>Generated on </th>");
        stringBuilder.Append("<td align='left'>" + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("<th> </th>");
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
