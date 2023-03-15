// Decompiled with JetBrains decompiler
// Type: controls_asset_email
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

public class controls_asset_email : fbs_base_user_control
{
  protected Button btnExportExcel;
  protected Label lblDateRage_email;
  protected DropDownList ddl_status_email;
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected HiddenField hdn_totalrecords;
  protected HiddenField hdn_status;
  protected HiddenField hdn_seachcondition;
  protected HiddenField hdn_id_email;
  protected HiddenField hdn_asset_id;
  public string html_table;
  public DataSet data;
  public asset _objAsset;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  public asset objAsset
  {
    get => this._objAsset;
    set => this._objAsset = value;
  }

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.IsPostBack)
        return;
      this.hdn_asset_id.Value = this.Request.QueryString["asset_id"];
      this.hdn_log_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      this.hdn_log_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      this.pageload();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload()
  {
    try
    {
      this.populate_ui();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_ui()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='emaillog_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Email to</th>");
      stringBuilder.Append("<th class='hidden-480'>Subject</th>");
      stringBuilder.Append("<th class='hidden-480'>Created By</th>");
      stringBuilder.Append("<th class='hidden-480'>Sent</th>");
      stringBuilder.Append("<th class='hidden-480'>Sent on</th>");
      stringBuilder.Append("<th class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr class='odd'><td valign='top' colspan='7' class='dataTables_empty'>No data available in table</td></tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
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
      DataSet dataSet1 = new DataSet();
      string str1 = Convert.ToDateTime(this.hdn_log_start.Value).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.hdn_log_end.Value).ToString(api_constants.sql_datetime_format_short);
      string status = !(this.ddl_status_email.SelectedItem.Value != "") ? "%" : this.ddl_status_email.SelectedItem.Value;
      string searchkey = !(this.hdn_seachcondition.Value != "") ? "%" : this.hdn_seachcondition.Value;
      string from = str1 + " 00:00:00:00";
      string to = str2 + " 23:59:59:59";
      DataSet dataSet2 = new DataSet();
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      if (this.hdn_totalrecords.Value != "")
      {
        string str3 = this.hdn_totalrecords.Value;
      }
      if (status == "" || status == "%")
        status = "0','1','2";
      DataSet emails = this.reportings.get_emails("1", this.hdn_totalrecords.Value, "from_msg", "Asc", searchkey, this.current_user.account_id, status, from, to, Convert.ToInt64(this.Request.QueryString["asset_id"]));
      if (emails != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) emails.Tables[0].Rows)
        {
          row["from_msg"] = (object) row["from_msg"].ToString();
          row["to_msg"] = (object) row["to_msg"].ToString();
          row["subject"] = (object) row["subject"].ToString();
          row["username"] = (object) row["username"].ToString();
          row["sent"] = !Convert.ToBoolean(row["sent"]) ? (object) "False" : (object) "True";
          row["sent_on"] = !(row["sent_on"].ToString() == "Not Yet Send") ? (object) row["sent_on"].ToString() : (object) row["sent_on"].ToString();
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("from_msg", "Email From");
      dictionary.Add("to_msg", "Email To");
      dictionary.Add("sent", "Sent");
      dictionary.Add("subject", "Subject");
      dictionary.Add("sent_on", "Sent On");
      dictionary.Add("username", "Created By");
      excel excel = new excel();
      excel.file_name = "+ current_user.full_name + " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = emails;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Email Log";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_Email Log List_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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
