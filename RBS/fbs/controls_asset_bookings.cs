// Decompiled with JetBrains decompiler
// Type: controls_asset_bookings
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
using System.Web.UI.WebControls;

public class controls_asset_bookings : fbs_base_user_control
{
  protected Button btnExportExcel;
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected HiddenField hdn_asset_ID;
  protected HiddenField hdn_totalrecords;
  protected HiddenField hdn_search_book;
  protected HiddenField hdn_booking_window;
  protected HiddenField hdn_id;
  protected HiddenField hdn_refreshmethod;
  public string html_table_Facility = "";
  public string html_header_Facility = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      if (this.IsPostBack)
        return;
      this.hdn_asset_ID.Value = this.Request.QueryString["asset_id"];
      this.Talbe_Header();
      setting setting = this.settings.get_setting("advance_booking_window", this.current_user.account_id);
      if (!(this.hdn_log_end.Value == "") || !(this.hdn_log_start.Value == ""))
        return;
      this.hdn_log_end.Value = this.current_timestamp.AddMonths(Convert.ToInt32(setting.value)).ToString(api_constants.sql_datetime_format_short);
      this.hdn_log_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      this.hdn_booking_window.Value = this.current_timestamp.AddMonths(Convert.ToInt32(setting.value)).ToString(api_constants.sql_datetime_format_short);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Talbe_Header()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='Facility_Booking'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th width='20%'>From</th>");
      stringBuilder.Append("<th width='20%'>To</th>");
      stringBuilder.Append("<th width='20%'>Requested By</th>");
      stringBuilder.Append("<th width='20%'>Status</th>");
      stringBuilder.Append("<th width='20%'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr class='odd'><td valign='top' colspan='6' class='dataTables_empty'>No data available in table</td></tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_header_Facility = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      string str1 = Convert.ToDateTime(this.hdn_log_start.Value).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.hdn_log_end.Value).ToString(api_constants.sql_datetime_format_short);
      string from = str1 + " 00:00:00:00";
      string to = str2 + " 23:59:59:59";
      DataSet dataSet = new DataSet();
      DataSet bookings = this.bookings.get_bookings("1", this.hdn_totalrecords.Value, "Status", "Asc", !(this.hdn_search_book.Value != "") ? "%" : this.hdn_search_book.Value, Convert.ToInt64(this.hdn_asset_ID.Value), this.current_user.account_id, from, to);
      bookings.Tables[0].Columns.Add("status_string");
      if (bookings != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) bookings.Tables[0].Rows)
        {
          row["book_from"] = (object) row["book_from"].ToString();
          row["book_to"] = (object) row["book_to"].ToString();
          switch (row["status"].ToString())
          {
            case "0":
              row["status_string"] = (object) "Cancelled";
              break;
            case "1":
              row["status_string"] = (object) "Booked";
              break;
            case "2":
              row["status_string"] = (object) "Blocked";
              break;
            case "3":
              row["status_string"] = (object) "No Show";
              break;
            case "4":
              row["status_string"] = (object) "Pending";
              break;
          }
          row["RequestedBy"] = (object) row["RequestedBy"].ToString();
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("book_from", "From");
      dictionary.Add("book_to", "To");
      dictionary.Add("RequestedBy", "RequestedBy");
      dictionary.Add("status_string", "Status");
      excel excel = new excel();
      excel.file_name = "Bookings_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = bookings;
      excel.column_names = dictionary;
      excel.table_identifier = "Bookings";
      excel.header = "Bookings";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "Bookings_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.exapi.get_excel(excel));
      this.Response.End();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }
}
