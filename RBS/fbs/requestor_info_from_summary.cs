// Decompiled with JetBrains decompiler
// Type: requestor_info_from_summary
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

public class requestor_info_from_summary : fbs_base_page, IRequiresSessionState
{
  public string html_table = "";
  public string html_header = "";
  protected Button btnExportExcel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.Talbe_Header();
      if (this.IsPostBack)
        return;
      this.pageload_Data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Talbe_Header()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th class='hidden-480'>Requestor Name</th>");
    stringBuilder.Append("<th class='hidden-480'>Department</th>");
    stringBuilder.Append("<th class='hidden-480'>Contact No</th>");
    stringBuilder.Append("<th class='hidden-480'>Building</th>");
    stringBuilder.Append("<th class='hidden-480'>Level</th>");
    stringBuilder.Append("<th class='hidden-480'>Room Name</th>");
    stringBuilder.Append("<th class='hidden-480'>From</th>");
    stringBuilder.Append("<th class='hidden-480'>To</th>");
    stringBuilder.Append("<th class='hidden-480'>Purpose</th>");
    stringBuilder.Append("<th class='hidden-480'>Booked By</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    stringBuilder.Append("<tbody>");
    this.html_header = stringBuilder.ToString();
  }

  protected void pageload_Data()
  {
    try
    {
      string fromdate = this.Request.QueryString["fromdate"] + " 00:00:00.000";
      string todate = this.Request.QueryString["todate"] + " 23:59:59.999";
      string asset_id = this.Request.QueryString["asset_id"];
      string status = this.Request.QueryString["status"];
      int num = this.bookings.get_status(Convert.ToInt64(status)) == "No Show" ? 1 : 0;
      StringBuilder stringBuilder = new StringBuilder();
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet dataSet = this.reportings.requestor_info_by_status_and_daterange(fromdate, todate, status, asset_id, this.current_user.account_id, groupids);
      if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + row["RequestorName"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["Department"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["contact"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["Building"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["RoomLevel"].ToString() + "</td>");
          stringBuilder.Append("<td> " + row["name"].ToString() + " </td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td> " + row["purpose"].ToString() + " </td>");
          stringBuilder.Append("<td> " + row["BookedBy"].ToString() + " </td>");
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

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      string str1 = this.Request.QueryString["fromdate"];
      string str2 = this.Request.QueryString["todate"];
      string asset_id = this.Request.QueryString["asset_id"];
      string status = this.Request.QueryString["status"];
      string fromdate = str1 + " 00:00:00.000";
      string todate = str2 + " 23:59:59.999";
      StringBuilder stringBuilder1 = new StringBuilder();
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet dataSet = this.reportings.requestor_info_by_status_and_daterange(fromdate, todate, status, asset_id, this.current_user.account_id, groupids);
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_RequestorInfoFromSummary_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder2.Append("<tbody>");
      stringBuilder2.Append("<tr>");
      stringBuilder2.Append("<th style='font-size:x-large;'>Requestor Info From Summary Report</th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th></th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("</tr>");
      stringBuilder2.Append("<tr>");
      stringBuilder2.Append("<th></th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("</tr>");
      stringBuilder2.Append("<tr>");
      stringBuilder2.Append("<th>Date Range</th>");
      stringBuilder2.AppendFormat("<th>{0} to {1}</th>", (object) Convert.ToDateTime(fromdate).ToString(api_constants.display_datetime_format_short), (object) Convert.ToDateTime(todate).ToString(api_constants.display_datetime_format_short));
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("</tr>");
      stringBuilder2.Append("<tr>");
      stringBuilder2.Append("<th></th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("<th> </th>");
      stringBuilder2.Append("</tr>");
      stringBuilder2.Append("</tbody>");
      stringBuilder2.Append("</table>");
      stringBuilder2.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder2.Append("<tbody>");
      stringBuilder2.Append("<tr>");
      stringBuilder2.Append("<th>Requestor Name</th>");
      stringBuilder2.Append("<th>Department</th>");
      stringBuilder2.Append("<th>Contact</th>");
      stringBuilder2.Append("<th>Building</th>");
      stringBuilder2.Append("<th>Level</th>");
      stringBuilder2.Append("<th>Room</th>");
      stringBuilder2.Append("<th>From</th>");
      stringBuilder2.Append("<th>To</th>");
      stringBuilder2.Append("<th>Purpose</th>");
      stringBuilder2.Append("<th>Booked By</th>");
      stringBuilder2.Append("</tr>");
      if (dataSet != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
        {
          stringBuilder2.Append("<tr class='odd gradeX'>");
          stringBuilder2.Append("<td>" + row["RequestorName"] + "</td>");
          stringBuilder2.Append("<td>" + row["Department"] + "</td>");
          stringBuilder2.Append("<td>" + row["contact"] + "</td>");
          stringBuilder2.Append("<td>" + row["Building"] + "</td>");
          stringBuilder2.Append("<td>" + row["RoomLevel"] + "</td>");
          stringBuilder2.Append("<td> " + row["name"] + " </td>");
          stringBuilder2.Append("<td>" + Convert.ToDateTime(row["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder2.Append("<td>" + Convert.ToDateTime(row["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder2.Append("<td> " + row["purpose"] + " </td>");
          stringBuilder2.Append("<td> " + row["BookedBy"] + " </td>");
          stringBuilder2.Append("</tr>");
        }
        stringBuilder2.Append("<tr>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("</tr>");
        stringBuilder2.Append("<tr>");
        stringBuilder2.Append("<th>Generated By </th>");
        stringBuilder2.Append("<td>" + this.current_user.full_name + " </td>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("</tr>");
        stringBuilder2.Append("</tr>");
        stringBuilder2.Append("<tr>");
        stringBuilder2.Append("<th>Generated on </th>");
        stringBuilder2.Append("<td align='left'>" + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("<th> </th>");
        stringBuilder2.Append("</tr>");
      }
      stringBuilder2.Append("</tbody>");
      stringBuilder2.Append("</table>");
      this.Response.Write(stringBuilder2.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Unnamed1_Click(object sender, EventArgs e)
  {
    try
    {
      if (this.bookings.get_status(Convert.ToInt64(this.Request.QueryString["status"])) == "No Show")
        this.Response.Redirect("~/administration/report_for_noshow.aspx");
      else
        this.Response.Redirect("~/administration/report_for_cancellation.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
