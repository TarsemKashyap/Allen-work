// Decompiled with JetBrains decompiler
// Type: administration_report_DailyView_
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

public class administration_report_DailyView_ : fbs_base_page, IRequiresSessionState
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
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected Button btn_filter;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["report_daily_view"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      this.Talbe_Header();
      if (this.IsPostBack || !(this.txtFromDate.Text == ""))
        return;
      this.txtFromDate.Text = this.current_timestamp.AddDays(-29.0).ToString(api_constants.display_datetime_format_short);
      this.txtToDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
      this.pageload_Data(this.txtFromDate.Text, this.txtToDate.Text);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Talbe_Header()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='unassigned_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th  style='width:9%;'class='hidden-480'>Room Name</th>");
    stringBuilder.Append("<th  style='width:9%;' class='hidden-480'>Booked Date</th>");
    stringBuilder.Append("<th style='width:9%;' class='hidden-480'>Time From</th>");
    stringBuilder.Append("<th style='width:9%;' class='hidden-480'>Time To</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Booked By</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Requested By</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Department</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Contact</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Builiding Name</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Category</th>");
    stringBuilder.Append("<th style='width:9%;' class='hidden-480'>Setup Type</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Purpose</th>");
    stringBuilder.Append("<th style='width:9%;'  class='hidden-480'>Status</th>");
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
      DateTime dateTime1 = Convert.ToDateTime(Fromdate);
      Todate += " 23:59:59 PM";
      DateTime dateTime2 = Convert.ToDateTime(Todate);
      Fromdate = dateTime1.ToString(api_constants.sql_datetime_format);
      Todate = dateTime2.ToString(api_constants.sql_datetime_format);
      StringBuilder stringBuilder = new StringBuilder();
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet dailyviewReport = this.reportings.getDailyview_Report(Fromdate, Todate, groupids, this.current_user.account_id);
      this.alertError.Attributes.Add("style", "display: none;");
      this.litErrorMsg.Text = "";
      if (dailyviewReport != null && dailyviewReport.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dailyviewReport.Tables[0].Rows)
        {
          if (row["status"].ToString() == "1")
          {
            stringBuilder.Append("<tr class='odd gradeX'>");
            stringBuilder.Append("<td>" + row["FacilityName"] + "</td>");
            stringBuilder.Append("<td>" + Convert.ToDateTime(row["BookedDate"].ToString()).ToString(api_constants.display_datetime_format_short) + "</td>");
            stringBuilder.Append("<td>" + row["TimeFrom"] + "</td>");
            stringBuilder.Append("<td>" + row["TimeTo"] + "</td>");
            stringBuilder.Append("<td>" + row["BookedBy"] + "</td>");
            stringBuilder.Append("<td>" + row["RequestedBy"] + "</td>");
            if (row["Department"].ToString().Trim() == "")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td> " + row["Department"] + " </td>");
            if (row["contact"].ToString().Trim() == "")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td> " + row["contact"] + " </td>");
            stringBuilder.Append("<td>" + row["BuilidingName"] + "</td>");
            stringBuilder.Append("<td>" + row["category"] + "</td>");
            if (row["Setup_type"].ToString().Trim() == "")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td>" + row["Setup_type"] + "</td>");
            stringBuilder.Append("<td>" + row["purpose"] + "</td>");
            stringBuilder.Append("<td><Span class='label label-Booked'> Booked </Span></td>");
            stringBuilder.Append("</tr>");
          }
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

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      string text1 = this.txtFromDate.Text;
      string text2 = this.txtToDate.Text;
      string str1 = text1 + " 00:00:00 AM";
      string Fromdate = Convert.ToDateTime(str1).ToString(api_constants.sql_datetime_format);
      string str2 = text2 + " 23:59:59 PM";
      string Todate = Convert.ToDateTime(str2).ToString(api_constants.sql_datetime_format);
      DataSet dailyviewReport = this.reportings.getDailyview_Report(Fromdate, Todate, groupids, this.current_user.account_id);
      if (dailyviewReport != null)
      {
        if (!this.utilities.isValidDataset(dailyviewReport))
        {
          this.alertError.Attributes.Add("style", "display: block;");
          this.litErrorMsg.Text = Resources.fbs.export_excel_no_data_msg;
          return;
        }
        this.alertError.Attributes.Add("style", "display: none;");
        this.litErrorMsg.Text = "";
      }
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=DailyViewReport_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='font-size:x-large;'>Daily View Report</th>");
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
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr class='odd gradeX'>");
      stringBuilder.Append("<td> " + str1 + "&" + str2 + "</td>");
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
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Room Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Booked Date</th>");
      stringBuilder.Append("<th class='hidden-480'>Time From</th>");
      stringBuilder.Append("<th class='hidden-480'>Time To</th>");
      stringBuilder.Append("<th class='hidden-480'>Booked By</th>");
      stringBuilder.Append("<th class='hidden-480'>Requested By</th>");
      stringBuilder.Append("<th class='hidden-480'>Department</th>");
      stringBuilder.Append("<th class='hidden-480'>contact</th>");
      stringBuilder.Append("<th class='hidden-480'>Builiding Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Setup Type</th>");
      stringBuilder.Append("<th class='hidden-480'>Purpose</th>");
      stringBuilder.Append("<th class='hidden-480'>Status</th>");
      stringBuilder.Append("</tr>");
      if (dailyviewReport != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dailyviewReport.Tables[0].Rows)
        {
          if (row["status"].ToString() == "1")
          {
            stringBuilder.Append("<tr class='odd gradeX'>");
            stringBuilder.Append("<td>" + row["FacilityName"] + "</td>");
            stringBuilder.Append("<td>" + Convert.ToDateTime(row["BookedDate"].ToString()).ToString(api_constants.display_datetime_format_short) + "</td>");
            stringBuilder.Append("<td>" + row["TimeFrom"] + "</td>");
            stringBuilder.Append("<td>" + row["TimeTo"] + "</td>");
            stringBuilder.Append("<td>" + row["BookedBy"] + "</td>");
            stringBuilder.Append("<td>" + row["RequestedBy"] + "</td>");
            if (row["Department"].ToString().Trim() == "")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td> " + row["Department"] + " </td>");
            if (row["contact"].ToString().Trim() == "")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td> " + row["contact"] + " </td>");
            stringBuilder.Append("<td>" + row["BuilidingName"] + "</td>");
            if (row["Setup_type"].ToString().Trim() == "")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td>" + row["Setup_type"] + "</td>");
            stringBuilder.Append("<td>" + row["purpose"] + "</td>");
            stringBuilder.Append("<td> Booked </td>");
            stringBuilder.Append("</tr>");
          }
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

  protected void btn_filter_Click(object sender, EventArgs e)
  {
    try
    {
      this.pageload_Data(this.txtFromDate.Text, this.txtToDate.Text);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
