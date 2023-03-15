// Decompiled with JetBrains decompiler
// Type: administration_holiday_list
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

public class administration_holiday_list : fbs_base_page, IRequiresSessionState
{
  protected HtmlGenericControl div_add_holiday;
  protected HtmlGenericControl div_upload_holiday;
  protected Button btnExportExcel;
  protected HiddenField searchcon;
  protected HiddenField hidorderby;
  protected HiddenField hidorderdir;
  public string html_table;
  public string holidaystatus = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["holidays"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.holidays_view)
      this.redirect_unauthorized();
    if (!this.IsPostBack)
      this.populate_ui();
    if (this.gp.holidays_add)
      this.div_add_holiday.Visible = true;
    if (this.gp.holidays_upload)
      this.div_upload_holiday.Visible = true;
    this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
    if (this.Session["holiday"] == null)
      return;
    if (this.Session["holiday"] == (object) "S")
    {
      this.holidaystatus = "S";
      this.Session.Remove("holiday");
    }
    if (this.Session["holiday"] != (object) "D")
      return;
    this.holidaystatus = "D";
    this.Session.Remove("holiday");
  }

  private void populate_ui()
  {
    StringBuilder stringBuilder = new StringBuilder();
    DataSet holidays = this.holidays.get_holidays(this.current_user.account_id);
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='holidylist_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th>Holiday</th>");
    stringBuilder.Append("<th class='hidden-480'>From Date</th>");
    stringBuilder.Append("<th class='hidden-480'>End Date</th>");
    stringBuilder.Append("<th class='hidden-480'>Year</th>");
    stringBuilder.Append("<th class='hidden-480'>Action</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    stringBuilder.Append("<tbody>");
    foreach (DataRow row in (InternalDataCollectionBase) holidays.Tables[0].Rows)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["holiday_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + Convert.ToDateTime(row["from_date"]).ToString(api_constants.display_datetime_format_short) + "</td>");
      stringBuilder.Append("<td>" + Convert.ToDateTime(row["to_date"]).ToString(api_constants.display_datetime_format_short) + "</td>");
      if (!Convert.ToBoolean(row["repeat"]))
        stringBuilder.Append("<td>" + (object) Convert.ToDateTime(row["from_date"]).Year + "</td>");
      else
        stringBuilder.Append("<td>Every Year</td>");
      stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
      if (this.gp.holidays_edit || this.gp.holidays_delete)
      {
        stringBuilder.Append("<ul class='ddm p-r'>");
        if (this.gp.holidays_edit)
          stringBuilder.AppendFormat("<li><a href='holiday_form.aspx?id={0}'><i class='icon-pencil'></i> Edit</a></li>", (object) row["holiday_id"].ToString());
        if (this.gp.holidays_delete)
          stringBuilder.AppendFormat("<li><a href='javascript:delete_holiday({0});'><i class='icon-trash'></i> Delete</a></li>", (object) row["holiday_id"].ToString());
      }
      stringBuilder.Append("</ul>");
      stringBuilder.Append("</div></div></td>");
      stringBuilder.Append("</tr>");
    }
    stringBuilder.Append("</tbody>");
    stringBuilder.Append("</table>");
    this.html_table = stringBuilder.ToString();
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      if (!(this.searchcon.Value == ""))
      {
        string str = this.searchcon.Value;
      }
      DataSet holidays = this.holidays.get_holidays(this.current_user.account_id);
      holidays.Tables[0].Columns.Add("repeat_string");
      holidays.Tables[0].AcceptChanges();
      if (!this.utilities.isValidDataset(holidays))
        return;
      try
      {
        foreach (DataRow row in (InternalDataCollectionBase) holidays.Tables[0].Rows)
        {
          row["repeat_string"] = Convert.ToBoolean(row["repeat"]) ? (object) "Every Year" : (object) Convert.ToDateTime(row["from_date"]).Year;
          holidays.Tables[0].AcceptChanges();
        }
      }
      catch
      {
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("holiday_name", "Holiday Name");
      dictionary.Add("from_date", "From Date");
      dictionary.Add("to_date", "To Date");
      dictionary.Add("repeat_string", "Year");
      excel excel = new excel();
      excel.file_name = "holidaylist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = holidays;
      excel.column_names = dictionary;
      excel.table_identifier = "holidaylist";
      excel.header = "Holiday List";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + excel.file_name);
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.exapi.get_excel(excel));
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
