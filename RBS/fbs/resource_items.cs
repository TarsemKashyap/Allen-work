﻿// Decompiled with JetBrains decompiler
// Type: resource_items
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

public class resource_items : fbs_base_page, IRequiresSessionState
{
  protected HtmlAnchor sample_editable_1_new;
  protected Button btnExportExcel;
  protected HiddenField hdn_search;
  protected HiddenField hdn_orderby;
  protected HiddenField hdn_orderdir;
  public string html_table;
  public string Save = "";
  public string delete = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    try
    {
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

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      DataSet dataSet = new DataSet();
      DataSet resourceItemList = this.resapi.get_export_resource_item_list(this.current_user.account_id, this.str_resource_module);
      resourceItemList.Tables[0].Columns.Add("status_string");
      if (resourceItemList != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) resourceItemList.Tables[0].Rows)
        {
          row["type_name"] = (object) row["type_name"].ToString();
          row["name"] = (object) row["name"].ToString();
          row["description"] = (object) row["description"].ToString();
          row["quantity"] = (object) row["quantity"].ToString();
          row["group_name"] = (object) row["group_name"].ToString();
          row["status_string"] = !(row["status"].ToString() == "1") ? (object) "NotAvailable" : (object) "Available";
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("type_name", "Resource Type");
      dictionary.Add("name", "Name");
      dictionary.Add("group_name", "Owner Group");
      dictionary.Add("quantity", "Quantity");
      dictionary.Add("description", "Description");
      dictionary.Add("status_string", "Status");
      excel excel = new excel();
      excel.file_name = "+ current_user.full_name + " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on : " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = resourceItemList;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Resource Items";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Resource Items_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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

  private void populate_ui()
  {
    try
    {
      DataSet userItemMap = this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, "resource_module");
      DataSet resourceItemList = this.resapi.get_resource_item_list(this.current_user.account_id, this.str_resource_module);
      this.ViewState.Add("data", (object) resourceItemList);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (DataRow row in (InternalDataCollectionBase) resourceItemList.Tables[0].Rows)
      {
        if (row["status"].ToString() != "-1")
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td>" + row["type_name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["description"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["quantity"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["unit_of_measure"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["group_name"] + "</td>");
          if (row["status"].ToString() == "1")
            stringBuilder.Append("<td><span class='label label-Available'>Available</span></td>");
          else
            stringBuilder.Append("<td><span class='label label-NotAvailable'>Not Available</span></td>");
          DataSet bookingsByDateRange = this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, this.current_timestamp.Date.AddDays(-1.0).AddSeconds(1.0), this.current_timestamp.Date.AddDays(1.0).AddSeconds(-1.0), 0L, Convert.ToInt64(row["item_id"]), this.str_resource_module);
          if (!this.gp.isAdminType)
          {
            if (userItemMap.Tables[0].Select("item_id='" + row["item_id"].ToString() + "'").Length > 0)
            {
              stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'>");
              stringBuilder.Append("<li><a href='resource_items_add_edit.aspx?id=" + row["item_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
              if (!this.utilities.isValidDataset(bookingsByDateRange))
                stringBuilder.Append("<li><a href='javascript:delete_resource_item(" + row["item_id"].ToString() + ")'><i class='icon-trash'></i> Remove</a></li>");
              stringBuilder.Append("</ul></div></div></td>");
            }
            else
              stringBuilder.Append("<td></td>");
          }
          else
          {
            stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'>");
            stringBuilder.Append("<li><a href='resource_items_add_edit.aspx?id=" + row["item_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
            this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, this.current_timestamp.Date.AddDays(-1.0).AddSeconds(1.0), this.current_timestamp.Date.AddDays(1.0).AddSeconds(-1.0), 0L, Convert.ToInt64(row["item_id"]), this.str_resource_module);
            if (!this.utilities.isValidDataset(bookingsByDateRange))
              stringBuilder.Append("<li><a href='javascript:delete_resource_item(" + row["item_id"].ToString() + ")'><i class='icon-trash'></i> Remove</a></li>");
            stringBuilder.Append("</ul></div></div></td>");
          }
          stringBuilder.Append("</tr>");
        }
        this.html_table = stringBuilder.ToString();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }
}
