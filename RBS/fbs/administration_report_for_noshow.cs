// Decompiled with JetBrains decompiler
// Type: administration_report_for_noshow
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

public class administration_report_for_noshow : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Button btnExportExcel;
  protected Label lblDateRage;
  protected Button btn_filter;
  protected HyperLink hyperlink_detail_view;
  protected HiddenField hdn_report_start;
  protected HiddenField hdn_report_end;
  protected HiddenField hdn_daterange;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["report_noshow"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (this.IsPostBack)
        return;
      string str1 = this.hdn_report_start.Value;
      string fromdate;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        fromdate = str1 + " 00:00:00.000";
      }
      else
      {
        fromdate = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
        this.hdn_report_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      }
      string str2 = this.hdn_report_end.Value;
      string todate;
      if (!string.IsNullOrWhiteSpace(str2))
      {
        todate = str2 + " 23:59:59.999";
      }
      else
      {
        todate = this.current_timestamp.ToString(api_constants.sql_datetime_format_short) + " 23:59:59.999";
        this.hdn_report_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      }
      this.hdn_daterange.Value = Convert.ToDateTime(this.hdn_report_start.Value).ToString("MMMM d, yyyy") + " - " + Convert.ToDateTime(this.hdn_report_end.Value).ToString("MMMM d, yyyy");
      this.populate_ui(fromdate, todate);
      this.btn_filter.Attributes.Add("style", "margin-top: -12px");
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
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet dataSet = this.reportings.noshow_report(fromdate, todate, this.current_user.account_id, groupids);
      this.alertError.Attributes.Add("style", "display: none;");
      this.litErrorMsg.Text = "";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<div style='padding:20px 0; font-weight:bold; font-size:16pt;'>By Division / Department</div>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Department</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Room</th>");
      stringBuilder.Append("<th class='hidden-480'>No.of Bookings</th>");
      stringBuilder.Append("<th class='hidden-480'>No.of No Shows</th>");
      stringBuilder.Append("<th class='hidden-480'>% of No Shows</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      string str1 = "";
      string str2 = "";
      for (int index = 0; index < dataSet.Tables[0].Rows.Count; ++index)
      {
        DataRow row = dataSet.Tables[0].Rows[index];
        int int32_1 = Convert.ToInt32(row["NoOfBooking"].ToString());
        int int32_2 = Convert.ToInt32(row["SumOfNoShow"].ToString());
        string str3 = int32_2 > int32_1 ? ">100" : Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(int32_2) / Convert.ToDouble(int32_1)) * 100.0).ToString("#.##");
        if (index == 0)
        {
          stringBuilder.Append("<tr class='division'>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("<tr>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Department"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["NoOfBooking"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) str3);
          stringBuilder.Append("</tr>");
          str1 = row["Division"].ToString();
          this.hyperlink_detail_view.NavigateUrl = "requestor_info_from_summary.aspx?fromdate=" + Convert.ToDateTime(fromdate).ToString(api_constants.sql_datetime_format_short) + "&todate=" + Convert.ToDateTime(todate).ToString(api_constants.sql_datetime_format_short) + "&status=" + this.bookings.get_status("No Show").ToString();
        }
        else if (row["Division"].ToString().Contains("Grand Total"))
        {
          stringBuilder.Append("<tr class='grandtotal'>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.Append("</tr>");
        }
        else if (row["Division"].ToString().Contains(row["Department"].ToString() + " Total"))
        {
          stringBuilder.Append("<tr class='departmenttotal'>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.Append("</tr>");
        }
        else if (row["Division"] == DBNull.Value && row["Department"] == DBNull.Value)
        {
          stringBuilder.Append("<tr class='divisiontotal'>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) (str1 + " Total"));
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.Append("</tr>");
          str2 = "yes";
        }
        else
        {
          if (str2 == "yes")
          {
            str2 = "no";
            stringBuilder.Append("<tr class='division'>");
            stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("</tr>");
          }
          str1 = row["Division"].ToString();
          stringBuilder.Append("<tr>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Department"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["NoOfBooking"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) str3);
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<div style='padding:20px 0; font-weight:bold; font-size:16pt;'>By Room</div>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='width:30%;'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Room</th>");
      stringBuilder.Append("<th class='hidden-480'>No.of No Shows</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      for (int index = 0; index < dataSet.Tables[1].Rows.Count; ++index)
      {
        DataRow row = dataSet.Tables[1].Rows[index];
        stringBuilder.Append("<tr>");
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["NoOfNoShow"].ToString());
        stringBuilder.Append("</tr>");
      }
      if (dataSet.Tables[0].Rows.Count > 0)
      {
        stringBuilder.Append("<tr class='departmenttotal'>");
        stringBuilder.Append("<td></td>");
        stringBuilder.AppendFormat("<td>{0}</td>", (object) dataSet.Tables[0].Rows[dataSet.Tables[0].Rows.Count - 1]["SumOfNoShow"].ToString());
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("</tbody>");
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
      string str1 = this.hdn_report_start.Value;
      string fromdate;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        fromdate = str1 + " 00:00:00.000";
      }
      else
      {
        fromdate = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
        this.hdn_report_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      }
      string str2 = this.hdn_report_end.Value;
      string todate;
      if (!string.IsNullOrWhiteSpace(str2))
      {
        todate = str2 + " 23:59:59.999";
      }
      else
      {
        todate = this.current_timestamp.ToString(api_constants.sql_datetime_format_short) + " 23:59:59.999";
        this.hdn_report_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      }
      string groupids = this.gp.isAdminType ? "" : "asset_owner_group_id IN (" + string.Join(",", this.gp.group_ids.ToArray()) + ") AND ";
      DataSet ds = this.reportings.noshow_report(fromdate, todate, this.current_user.account_id, groupids);
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
      this.Response.AddHeader("content-disposition", "attachment;filename=Noshow_report_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td style='font-size:24pt;' colspan='3'>No Show Report (Summary)</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td style='border:none;'></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.AppendFormat("<td>From: {0}</td>", (object) Convert.ToDateTime(this.hdn_report_start.Value).ToString(api_constants.display_datetime_format_short));
      stringBuilder.AppendFormat("<td>To: {0}</td>", (object) Convert.ToDateTime(this.hdn_report_end.Value).ToString(api_constants.display_datetime_format_short));
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
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
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td style='font-weight:bold;'>By Division / Department</td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<th class='hidden-480'>Department</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Room</th>");
      stringBuilder.Append("<th class='hidden-480'>No.of Bookings</th>");
      stringBuilder.Append("<th class='hidden-480'>No.of No Shows</th>");
      stringBuilder.Append("<th class='hidden-480'>% of No Shows</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      string str3 = "";
      string str4 = "";
      for (int index = 0; index < ds.Tables[0].Rows.Count; ++index)
      {
        DataRow row = ds.Tables[0].Rows[index];
        int int32_1 = Convert.ToInt32(row["NoOfBooking"].ToString());
        int int32_2 = Convert.ToInt32(row["SumOfNoShow"].ToString());
        string str5 = int32_2 > int32_1 ? ">100" : Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(int32_2) / Convert.ToDouble(int32_1)) * 100.0).ToString("#.##");
        if (index == 0)
        {
          stringBuilder.Append("<tr class='division' style='background-color:#9fd1fb;'>");
          stringBuilder.Append("<td></td>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("<td></td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td></td>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Department"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["NoOfBooking"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) str5);
          stringBuilder.Append("</tr>");
          str3 = row["Division"].ToString();
        }
        else if (row["Division"].ToString().Contains("Grand Total"))
        {
          stringBuilder.Append("<tr class='grandtotal' style='background-color:#045ba5; color:#FFF; font-weight:bold;'>");
          stringBuilder.Append("<td></td>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.Append("</tr>");
        }
        else if (row["Division"].ToString().Contains(row["Department"].ToString() + " Total"))
        {
          stringBuilder.Append("<tr class='departmenttotal' style='background-color:#b8bcbf;color:#000; font-weight:bold;'>");
          stringBuilder.Append("<td></td>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.Append("</tr>");
        }
        else if (row["Division"] == DBNull.Value && row["Department"] == DBNull.Value)
        {
          stringBuilder.Append("<tr class='divisiontotal' style='background-color:#575858; color:#FFF; font-weight:bold;'>");
          stringBuilder.Append("<td></td>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) (str3 + " Total"));
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) "");
          stringBuilder.Append("</tr>");
          str4 = "yes";
        }
        else
        {
          if (str4 == "yes")
          {
            str4 = "no";
            stringBuilder.Append("<tr class='division' style='background-color:#9fd1fb; color:#0667b8; font-weight:bold;'>");
            stringBuilder.Append("<td></td>");
            stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Division"].ToString());
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("<td></td>");
            stringBuilder.Append("</tr>");
          }
          str3 = row["Division"].ToString();
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td></td>");
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Department"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["Building"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["NoOfBooking"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) row["SumOfNoShow"].ToString());
          stringBuilder.AppendFormat("<td>{0}</td>", (object) str5);
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td style='font-weight:bold;'>By Room</td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='width:30%; font-size:11pt;' border='1'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<th class='hidden-480'>Room</th>");
      stringBuilder.Append("<th class='hidden-480'>No.of No Shows</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      for (int index = 0; index < ds.Tables[1].Rows.Count; ++index)
      {
        DataRow row = ds.Tables[1].Rows[index];
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td></td>");
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["name"].ToString());
        stringBuilder.AppendFormat("<td>{0}</td>", (object) row["NoOfNoShow"].ToString());
        stringBuilder.Append("</tr>");
      }
      if (ds.Tables[0].Rows.Count > 0)
      {
        stringBuilder.Append("<tr class='departmenttotal'>");
        stringBuilder.Append("<td></td>");
        stringBuilder.Append("<td></td>");
        stringBuilder.AppendFormat("<td>{0}</td>", (object) ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["SumOfNoShow"].ToString());
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table class='table table_summary_report table-bordered' id='list_table_report' style='font-size:11pt;' border='1'>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Generated On</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format_short));
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td>Generated By</td>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) this.current_user.full_name);
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
      stringBuilder.Append("<td></td>");
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

  protected void btn_filter_Click(object sender, EventArgs e)
  {
    try
    {
      string str1 = this.hdn_report_start.Value;
      string fromdate;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        fromdate = str1 + " 00:00:00.000";
      }
      else
      {
        fromdate = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
        this.hdn_report_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      }
      string str2 = this.hdn_report_end.Value;
      string todate;
      if (!string.IsNullOrWhiteSpace(str2))
      {
        todate = str2 + " 23:59:59.999";
      }
      else
      {
        todate = this.current_timestamp.ToString(api_constants.sql_datetime_format_short) + " 23:59:59.999";
        this.hdn_report_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      }
      this.hdn_daterange.Value = Convert.ToDateTime(this.hdn_report_start.Value).ToString("MMMM d, yyyy") + " - " + Convert.ToDateTime(this.hdn_report_end.Value).ToString("MMMM d, yyyy");
      this.populate_ui(fromdate, todate);
      this.btn_filter.Attributes.Add("style", "margin-top: -12px");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Export No Show Reporting Error: ->", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
