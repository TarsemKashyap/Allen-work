// Decompiled with JetBrains decompiler
// Type: resource_types
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

public class resource_types : fbs_base_page, IRequiresSessionState
{
  protected HtmlAnchor sample_editable_1_new;
  protected Button btnExportExcel;
  public string html_table;
  public string Save = "";
  public string delete = "";
  private bool is_admin;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    try
    {
      foreach (user_group userGroup in this.current_user.groups.Values)
      {
        if (userGroup.group_type == 1)
          this.is_admin = true;
        else if (!this.is_admin)
          this.is_admin = false;
      }
      if (!this.is_admin)
        this.redirect_unauthorized();
      if (this.IsPostBack)
        return;
      this.populate_ui();
      if (this.Session["Save"] != null && this.Session["Save"] == (object) "S")
      {
        this.Save = "S";
        this.Session.Remove("Save");
      }
      if (this.Session["delete"] == null)
        return;
      if (this.Session["delete"] == (object) "S")
      {
        this.delete = "S";
        this.Session.Remove("delete");
      }
      else
      {
        if (this.Session["delete"] != (object) "F")
          return;
        this.delete = "F";
        this.Session.Remove("delete");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private void populate_ui()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataSet resourceTypes = this.resapi.get_resource_types(this.current_user.account_id, this.str_resource_module);
      this.ViewState.Add("data", (object) resourceTypes);
      if (resourceTypes != null && resourceTypes.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) resourceTypes.Tables[0].Rows)
        {
          if (Convert.ToInt16(row["status"]) > (short) -1)
          {
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<td>" + row["value"].ToString() + "</td>");
            stringBuilder.Append("<td>" + row["item_count"].ToString() + "</td>");
            if (row["modified_on"] != DBNull.Value)
              stringBuilder.Append("<td>" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["modified_on"])).ToString(api_constants.display_datetime_format) + "</td>");
            else
              stringBuilder.Append("<td>" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["created_on"])).ToString(api_constants.display_datetime_format) + "</td>");
            if (row["modified_name"] != DBNull.Value)
              stringBuilder.Append("<td>" + row["modified_name"].ToString() + "</td>");
            else
              stringBuilder.Append("<td>" + row["created_name"].ToString() + "</td>");
            if (Convert.ToInt16(row["status"]) == (short) 1)
              stringBuilder.Append("<td><span class='label label-Available'>Available</span></td>");
            else
              stringBuilder.Append("<td><span class='label label-NotAvailable'>Not Available</span></td>");
            stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'>");
            stringBuilder.Append("<li><a href='resource_type_add_edit.aspx?id=" + row["setting_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
            if (!this.utilities.isValidDataset(this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, this.current_timestamp.Date.AddDays(-1.0).AddSeconds(1.0), this.current_timestamp.Date.AddDays(1.0).AddSeconds(-1.0), Convert.ToInt64(row["setting_id"]), 0L, this.str_resource_module)))
              stringBuilder.Append("<li><a href='javascript:delete_resource_type(" + row["setting_id"].ToString() + ")'><i class='icon-trash'></i> Remove</a></li>");
            stringBuilder.Append("</ul></div></div></td>");
            stringBuilder.Append("</tr>");
          }
        }
      }
      else
        this.html_table = "<tr><td colspan=''>No data</td></tr>";
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    DataSet dataSet = (DataSet) this.ViewState["data"] ?? this.resapi.get_resource_types(this.current_user.account_id, this.str_resource_module);
    dataSet.Tables[0].Columns.Add("status_string");
    try
    {
      if (dataSet != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
        {
          if (Convert.ToInt16(row["status"]) > (short) -1)
          {
            row["value"] = (object) row["value"].ToString();
            row["item_count"] = (object) row["item_count"].ToString();
            row["modified_on"] = row["modified_on"] == DBNull.Value ? (object) this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["created_on"])).ToString(api_constants.display_datetime_format) : (object) this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["modified_on"])).ToString(api_constants.display_datetime_format);
            row["modified_name"] = row["modified_name"] == DBNull.Value ? (object) row["created_name"].ToString() : (object) row["modified_name"].ToString();
            row["status_string"] = Convert.ToInt16(row["status"]) != (short) 1 ? (object) "NotAvailable" : (object) "Available";
          }
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("value", "Resource Type");
      dictionary.Add("modified_on", "Modified On");
      dictionary.Add("modified_name", "Modified By");
      dictionary.Add("item_count", "Items");
      dictionary.Add("status_string", "Status");
      excel excel = new excel();
      excel.file_name = "+ current_user.full_name + " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = dataSet;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Resource Types";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Resource Types_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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
