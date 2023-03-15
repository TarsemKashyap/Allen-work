// Decompiled with JetBrains decompiler
// Type: administration_email_loglist
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
using System.Web.UI.WebControls;

public class administration_email_loglist : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public DataSet data;
  public string show_resend;
  protected Button btnExportExcel;
  protected Label lblDateRage;
  protected DropDownList ddl_status;
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected HiddenField hdn_exportexcel;
  protected HiddenField hdn_status;
  protected HiddenField hdn_daterange;
  protected HiddenField hdn_totalrecords;
  protected HiddenField hdn_Searchcondition;
  protected HiddenField hdn_id;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["email_log"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      if (this.IsPostBack)
        return;
      this.hdn_log_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      this.hdn_log_start.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      this.pageload_data();
      if (this.Session["Mailsend"] == null)
        return;
      this.show_resend = "S";
      this.Session.Remove("Mailsend");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    try
    {
      this.populate_ui();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_ui()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:18%' class='hidden-480' >Email to</th>");
      stringBuilder.Append("<th  style='width:27%' class='hidden-480'>Subject</th>");
      stringBuilder.Append("<th style='width:15%'  class='hidden-480'>Created By</th>");
      stringBuilder.Append("<th style='width:5%' class='hidden-480'>Sent</th>");
      stringBuilder.Append("<th style='width:15%' class='hidden-480'>Sent On</th>");
      stringBuilder.Append("<th style='width:5%' class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
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
      string str1 = Convert.ToDateTime(this.hdn_log_start.Value).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.hdn_log_end.Value).ToString(api_constants.sql_datetime_format_short);
      string status = !(this.ddl_status.SelectedItem.Value != "") ? "%" : this.ddl_status.SelectedItem.Value;
      string from = str1 + " 00:00:00:00";
      string to = str2 + " 23:59:59:59";
      DataSet dataSet = new DataSet();
      string searchkey = !(this.hdn_Searchcondition.Value != "") ? "%" : this.hdn_Searchcondition.Value;
      if (status == "" || status == "%")
        status = "0','1','2";
      DataSet emails = this.reportings.get_emails("1", this.hdn_totalrecords.Value, "from_msg", "Asc", searchkey, this.current_user.account_id, status, from, to);
      emails.Tables[0].Columns.Add("sent_string");
      emails.Tables[0].Columns.Add("sent_on_string");
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("to_msg", "Email To");
      dictionary.Add("subject", "Subject");
      dictionary.Add("sent_string", "Sent");
      dictionary.Add("sent_on_string", "Sent On");
      if (emails == null)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) emails.Tables[0].Rows)
      {
        row["sent_string"] = !Convert.ToBoolean(row["sent"]) ? (object) "False" : (object) "True";
        row["sent_on_string"] = !(row["sent_on"].ToString() == "1") ? (object) "NOT YET SEND" : (object) row["sent_on"].ToString();
        emails.Tables[0].AcceptChanges();
      }
      excel excel = new excel();
      excel.file_name = this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = emails;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Email Log List";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=EmailLogList_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
