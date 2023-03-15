// Decompiled with JetBrains decompiler
// Type: controls_admin_master_list
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class controls_admin_master_list : fbs_base_user_control
{
  protected HtmlAnchor link_add_master;
  protected Button btnExportExcel;
  protected HiddenField hdn_mastertype;
  protected HiddenField hdn_building;
  protected HiddenField hdn_level;
  protected HiddenField hdn_category;
  protected HiddenField hdn_types;
  protected HiddenField hdn_setup;
  protected HiddenField hdn_assetprop;
  protected HiddenField hdn_meetingtype;
  public string html_table;
  public DataSet _objSettings;
  public DataSet _objUsers;
  public string JS_filter = "";
  public bool _isEditable;
  public bool _isDeletable;
  public string current_tab_li = "";
  public string current_tab = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  public bool isEditable
  {
    get => this._isEditable;
    set => this._isEditable = value;
  }

  public bool isDeletable
  {
    get => this._isDeletable;
    set => this._isDeletable = value;
  }

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.Session["Master_current_tab"] != null)
      {
        if (this.Session["count"] == null)
          this.Session["count"] = (object) 0;
        else
          this.Session["count"] = (object) (Convert.ToInt32(this.Session["count"].ToString()) + 1);
        List<string> stringList = (List<string>) this.Session["Master_current_tab"];
        this.current_tab_li = stringList[0];
        this.current_tab = stringList[1];
        if (this.Session["count"] != null && this.Session["count"].ToString() == "5")
        {
          this.Session.Remove("current_tab");
          this.Session.Remove("Master_current_tab");
          this.Session.Remove("count");
        }
      }
      else
      {
        this.current_tab_li = "Buildings";
        this.current_tab = "tab_buildings";
      }
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      string attribute = this.Attributes["filter"];
      this.hdn_mastertype.Value = attribute;
      this.link_add_master.InnerText = "Add " + this.Attributes["title"];
      this.link_add_master.HRef = this.site_full_path + "administration/master_edit.aspx?id=0&type=" + attribute;
      this.populate_ui(attribute);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_ui(string filter)
  {
    try
    {
      this.JS_filter = filter;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='" + filter + "_table' style='width:100%;'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:51%;' class='hidden-480'>Name</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Modified On</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Modified By</th>");
      stringBuilder.Append("<th style='width:2%;' class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
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
      string searchkey = "";
      string attribute = this.Attributes["filter"];
      string str = "";
      switch (attribute)
      {
        case "building":
          searchkey = this.hdn_building.Value;
          break;
        case "level":
          searchkey = this.hdn_level.Value;
          break;
        case "category":
          searchkey = this.hdn_category.Value;
          break;
        case "asset_type":
          searchkey = this.hdn_types.Value;
          break;
        case "setup_type":
          searchkey = this.hdn_setup.Value;
          break;
        case "asset_property":
          searchkey = this.hdn_assetprop.Value;
          break;
        case "meeting_type":
          searchkey = this.hdn_meetingtype.Value;
          break;
      }
      switch (attribute)
      {
        case "building":
          str = "Building Master List";
          break;
        case "level":
          str = "Level Master List";
          break;
        case "category":
          str = "Category Master List";
          break;
        case "asset_type":
          str = "Asset Type Master List";
          break;
        case "setup_type":
          str = "Setup Type Master List";
          break;
        case "asset_property":
          str = "Asset Property Master List";
          break;
        case "meeting_type":
          str = "Meeting Types Master List";
          break;
      }
      DataSet masterSettings = this.reportings.get_Master_settings(this.current_user.account_id, searchkey, attribute, "RR.value", "Asc");
      masterSettings.Tables[0].Columns.Add("status_string");
      foreach (DataRow row in (InternalDataCollectionBase) masterSettings.Tables[0].Rows)
      {
        row["value"] = (object) row["value"].ToString();
        row["modified_on"] = (object) this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["modified_on"])).ToString(api_constants.display_datetime_format);
        row["fullname"] = (object) row["fullname"].ToString();
        row["Status"] = (object) row["Status"].ToString();
        masterSettings.Tables[0].AcceptChanges();
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("value", "Name");
      dictionary.Add("modified_on", "Modified On");
      dictionary.Add("Status", "Status");
      dictionary.Add("fullname", "Modified By");
      excel excel = new excel();
      excel.file_name = "admin_master_list-" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = masterSettings;
      excel.column_names = dictionary;
      excel.table_identifier = "admin_master_list";
      excel.header = str;
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + str + "_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
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
