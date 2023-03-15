// Decompiled with JetBrains decompiler
// Type: administration_blacklist_list
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

public class administration_blacklist_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public string blockliststatus = "";
  public string blockliststatusremoved = "";
  protected Button btnExportExcel;
  protected HiddenField hdn_search;
  protected HiddenField hdn_orderby;
  protected HiddenField hdn_orderdir;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["blacklist_user"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.users_blacklist)
      this.redirect_unauthorized();
    try
    {
      if (!this.IsPostBack)
        this.populate_ui();
      if (this.Session["Blacklist"] != null && this.Session["Blacklist"] == (object) "S")
      {
        this.blockliststatus = "S";
        this.Session.Remove("Blacklist");
      }
      if (this.Session["Blacklist_delete"] != null && this.Session["Blacklist_delete"] == (object) "S")
      {
        this.blockliststatusremoved = "S";
        this.Session.Remove("Blacklist_delete");
      }
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
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
      DataSet dataSet = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_bl");
      if (dataSet == null)
      {
        dataSet = this.blapi.get_blacklist(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_bl", (object) dataSet);
      }
      foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td>" + row["full_name"].ToString() + "</td>");
        stringBuilder.Append("<td><a href='mailto:" + row["email"].ToString() + "'>" + row["email"].ToString() + "</a></td>");
        if (row["blacklist_type"].ToString() == "1")
          stringBuilder.Append("<td>Indefinite</td>");
        else
          stringBuilder.Append("<td>Duration</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(row["from_date"]).ToString(api_constants.display_datetime_format) + "</td>");
        if (row["blacklist_type"].ToString() == "1")
          stringBuilder.Append("<td>-</td>");
        else
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["to_date"]).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>");
        stringBuilder.Append("<div class='actions' id=action_" + row["blacklist_id"].ToString() + "><div class='bgp'>");
        stringBuilder.Append("<a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
        stringBuilder.Append("<ul class='ddm p-r'>");
        if (row["blacklist_type"].ToString() == "1")
        {
          stringBuilder.Append("<li><a href='blacklist_form.aspx?blacklist_id=" + row["blacklist_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
          stringBuilder.Append("<li><a href='javascript:delete_blocklist(" + row["blacklist_id"].ToString() + ");' style='cursor:pointer;'><i class='icon-trash'></i> Remove</a></li>");
        }
        else if (Convert.ToDateTime(row["to_date"]) >= DateTime.Today)
        {
          stringBuilder.Append("<li><a href='blacklist_form.aspx?blacklist_id=" + row["blacklist_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
          stringBuilder.Append("<li><a href='javascript:delete_blocklist(" + row["blacklist_id"].ToString() + ");' style='cursor:pointer;'><i class='icon-trash'></i> Remove</a></li>");
        }
        else
          stringBuilder.Append("<li><a href='javascript:delete_blocklist(" + row["blacklist_id"].ToString() + ");' style='cursor:pointer;'><i class='icon-trash'></i> Remove</a></li>");
        stringBuilder.Append("</ul></div></div>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
      }
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
      DataSet blacklist = this.blapi.get_blacklist(this.current_user.account_id);
      blacklist.Tables[0].Columns.Add("blacklist_type_string");
      foreach (DataRow row in (InternalDataCollectionBase) blacklist.Tables[0].Rows)
      {
        row["blacklist_type_string"] = !(row["blacklist_type"].ToString() == "1") ? (object) "Duration" : (object) "Indefinite";
        row["from_date"] = (object) Convert.ToDateTime(row["from_date"]).ToString(api_constants.datetime_format);
        row["to_date"] = !(row["blacklist_type"].ToString() == "1") ? (object) Convert.ToDateTime(row["to_date"]).ToString(api_constants.datetime_format) : (object) " -";
        blacklist.Tables[0].AcceptChanges();
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("from_date", "From");
      dictionary.Add("to_date", "To");
      dictionary.Add("full_name", "Person");
      dictionary.Add("email", "Email");
      dictionary.Add("blacklist_type_string", "Type");
      excel excel = new excel();
      excel.file_name = " " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "    <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = blacklist;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Black List";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=blacklist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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
