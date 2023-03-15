// Decompiled with JetBrains decompiler
// Type: controls_asset_audit_logs
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Profile;
using System.Web.UI.WebControls;

public class controls_asset_audit_logs : fbs_base_user_control
{
  protected Button btnExportExcel;
  protected Label lblDateRage;
  protected DropDownList ddl_module_name;
  protected DropDownList ddl_action;
  protected DropDownList ddl_status;
  protected HiddenField hdn_log_start;
  protected HiddenField hdn_log_end;
  protected HiddenField hdn_modulename;
  protected HiddenField hdn_action;
  protected HiddenField hdn_status;
  protected HiddenField hdn_daterange;
  protected HiddenField hdn_totalrecords;
  protected HiddenField hdn_searchcondition;
  protected HiddenField hdn_id;
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
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      if (this.IsPostBack)
        return;
      this.populate_module_name();
      this.hdn_log_end.Value = this.current_timestamp.ToString(api_constants.sql_datetime_format_short);
      this.hdn_log_start.Value = this.current_timestamp.AddDays(-29.0).ToString(api_constants.sql_datetime_format_short);
      this.pageload();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_module_name()
  {
    try
    {
      string str1 = HttpContext.GetGlobalResourceObject("fbs", "asset_audit_log").ToString();
      char[] chArray = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray))
        this.ddl_module_name.Items.Add(new ListItem()
        {
          Text = str2
        });
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
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='auditlog_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Module</th>");
      stringBuilder.Append("<th class='hidden-480'>Action</th>");
      stringBuilder.Append("<th class='hidden-480'>Status</th>");
      stringBuilder.Append("<th class='hidden-480'>Created On</th>");
      stringBuilder.Append("<th class='hidden-480'>Created By</th>");
      stringBuilder.Append("<th class='hidden-480'>Details</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr class='odd'><td valign='top' colspan='6' class='dataTables_empty'>No data available in table</td></tr>");
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
      if (this.hdn_modulename.Value == "")
        this.hdn_modulename.Value = "%";
      if (this.hdn_action.Value == "")
        this.hdn_action.Value = "%";
      if (this.hdn_status.Value == "")
        this.hdn_status.Value = "%";
      DataSet dataSet = new DataSet();
      string str1 = Convert.ToDateTime(this.hdn_log_start.Value).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.hdn_log_end.Value).ToString(api_constants.sql_datetime_format_short);
      string modulename = this.hdn_modulename.Value;
      string status = this.hdn_status.Value;
      string action = this.hdn_action.Value;
      string from = str1 + " 00:00:00:00";
      string to = str2 + " 23:59:59:59";
      DataSet logs = this.reportings.get_logs("1", this.hdn_totalrecords.Value, "module_name", "Asc", !(this.hdn_searchcondition.Value == "") ? this.hdn_searchcondition.Value : "%", this.current_user.account_id, modulename, action, status, from, to);
      if (logs != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) logs.Tables[0].Rows)
        {
          row["module_name"] = (object) row["module_name"].ToString();
          row["action"] = (object) row["action"].ToString();
          row["status"] = (object) row["status"].ToString();
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("module_name", "Module Name");
      dictionary.Add("action", "Action");
      dictionary.Add("status", "Status");
      dictionary.Add("created_on", "Created On");
      dictionary.Add("createduser", "Created By");
      excel excel = new excel();
      excel.file_name = "+ current_user.full_name + " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = logs;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Audit Log";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_Audit LogList_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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
