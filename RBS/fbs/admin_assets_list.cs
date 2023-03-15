// Decompiled with JetBrains decompiler
// Type: admin_assets_list
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

public class admin_assets_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public DataSet data;
  public string account_id;
  public string sucess_msg;
  private StringBuilder html = new StringBuilder();
  public string delete_gritter = "";
  public string reason = "";
  private DataSet data_forsearch_filter;
  private DataSet asset_data;
  private DataSet group_data;
  private DataSet setting_data;
  private DataSet asset_properties_data;
  protected DropDownList ddl_building;
  protected DropDownList ddl_category;
  protected DropDownList ddl_type;
  protected DropDownList ddl_owner;
  protected Button btn_filter;
  protected HtmlAnchor sample_editable_1_new;
  protected Button btnExportExcel;
  protected HiddenField hdn_assetsearch;
  protected HiddenField hidorderby;
  protected HiddenField hidorderdir;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (!this.gp.facility_view)
        this.redirect_unauthorized();
      this.asset_data = this.assets.get_assets(this.current_user.account_id);
      this.group_data = this.users.get_groups(this.current_user.account_id);
      this.setting_data = this.settings.get_settings(this.current_user.account_id);
      this.asset_properties_data = this.assets.get_asset_properties(this.current_user.account_id);
      if (!this.IsPostBack)
      {
        this.populate_ui();
        this.populate_rooms(this.ddl_building.SelectedItem.Value, this.ddl_category.SelectedItem.Value, this.ddl_type.SelectedItem.Value, this.ddl_owner.SelectedItem.Value);
      }
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      this.account_id = this.current_user.account_id.ToString();
      if (!this.gp.facility_add)
        this.sample_editable_1_new.Visible = false;
      if (this.Request.QueryString["del"] == null)
        return;
      this.delete_gritter = this.Request.QueryString["del"].ToString();
      if (!(this.delete_gritter == "N"))
        return;
      this.reason = this.Request.QueryString["reas"].ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_rooms(string building, string category, string type, string owner)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='assetlist_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th style='width:3%;' class='hidden-480'>Access</th>");
    stringBuilder.Append("<th style='width:12%;' class='hidden-480'>Code / Name</th>");
    stringBuilder.Append("<th style='width:8%;' class='hidden-480'>Building</th>");
    stringBuilder.Append("<th style='width:6%;' class='hidden-480'>Level</th>");
    stringBuilder.Append("<th style='width:6%;' class='hidden-480'>Capacity</th>");
    stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Category</th>");
    stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Type</th>");
    stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Operating Hrs.</th>");
    stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Allow Weekends</th>");
    stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Allow Holidays</th>");
    stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Owner Group</th>");
    stringBuilder.Append("<th style='width:6%;' class='hidden-480'>Status</th>");
    stringBuilder.Append("<th style='width:3%;' class='hidden-480'>Action</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    string str = "select b.*, (select count(group_id) from sbt_assets_permissions where asset_id = b.asset_id and is_book = 1) as is_book_count," + " (select count(group_id) from sbt_assets_permissions where asset_id = b.asset_id and is_view = 1 and is_book = 0) as is_view_only_count from sbt_assets as b where account_id='" + (object) this.current_user.account_id + "' ";
    if (building != "0")
      str = str + " and building_id='" + building + "' ";
    if (category != "0")
      str = str + " and category_id='" + category + "' ";
    if (type != "0")
      str = str + " and asset_type='" + type + "' ";
    if (owner == "-1")
      str += " and asset_owner_group_id is null ";
    else if (owner != "0")
      str = str + " and asset_owner_group_id='" + owner + "' ";
    if (this.db.get_dataset(str + "order by name"))
    {
      string[] strArray1 = this.settings.get_setting("operating_hours", this.current_user.account_id).value.ToString().Split('|');
      bool boolean1 = Convert.ToBoolean(this.settings.get_setting("book_weekend", this.current_account.account_id).value);
      bool boolean2 = Convert.ToBoolean(this.settings.get_setting("book_holiday", this.current_account.account_id).value);
      stringBuilder.Append("<tbody>");
      foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
      {
        long num = 0;
        if (row["asset_owner_group_id"].ToString() != "")
          num = Convert.ToInt64(row["asset_owner_group_id"]);
        bool flag = false;
        if (this.gp.isAdminType)
          flag = true;
        if (!flag)
        {
          foreach (user_group userGroup in this.current_user.groups.Values)
          {
            if (userGroup.group_id == num)
              flag = true;
          }
        }
        if (flag)
        {
          stringBuilder.Append("<tr>");
          if (row["asset_owner_group_id"].ToString() == "" || row["asset_owner_group_id"].ToString() == "0")
            stringBuilder.Append("<td><a href='#' class='btn green icn-only'><i class='icon-unlock icon-white'></i></a></i></td>");
          else
            stringBuilder.Append("<td><a href='#' class='btn red icn-only'><i class='icon-lock icon-white'></i></a></i></td>");
          stringBuilder.Append("<td>" + row["code"].ToString() + "/" + row["name"].ToString() + "</td>");
          DataRow[] dataRowArray1 = this.setting_data.Tables[0].Select("setting_id='" + row["building_id"].ToString() + "'");
          stringBuilder.Append("<td>" + dataRowArray1[0]["value"].ToString() + "</td>");
          DataRow[] dataRowArray2 = this.setting_data.Tables[0].Select("setting_id='" + row["level_id"].ToString() + "'");
          stringBuilder.Append("<td>" + dataRowArray2[0]["value"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["capacity"].ToString() + "</td>");
          DataRow[] dataRowArray3 = this.setting_data.Tables[0].Select("setting_id='" + row["category_id"].ToString() + "'");
          stringBuilder.Append("<td>" + dataRowArray3[0]["value"].ToString() + "</td>");
          DataRow[] dataRowArray4 = this.setting_data.Tables[0].Select("setting_id='" + row["asset_type"].ToString() + "'");
          stringBuilder.Append("<td>" + dataRowArray4[0]["value"].ToString() + "</td>");
          DataRow[] dataRowArray5 = this.asset_properties_data.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and property_name='operating_hours'");
          if (dataRowArray5.Length > 0)
          {
            string[] strArray2 = dataRowArray5[0]["property_value"].ToString().Split('|');
            stringBuilder.Append("<td>" + Convert.ToDateTime(strArray2[0]).ToString("HH:mm") + " to " + Convert.ToDateTime(strArray2[1]).ToString("HH:mm") + "</td>");
          }
          else
            stringBuilder.Append("<td>" + Convert.ToDateTime(strArray1[0]).ToString("HH:mm") + " to " + Convert.ToDateTime(strArray1[1]).ToString("HH:mm") + "</td>");
          DataRow[] dataRowArray6 = this.asset_properties_data.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and property_name='book_weekend'");
          if (dataRowArray6.Length > 0)
          {
            if (Convert.ToBoolean(dataRowArray6[0]["property_value"]))
              stringBuilder.Append("<td><span class='label label-Available'><i class='icon-ok icon-white'></i></span></td>");
            else
              stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
          }
          else if (boolean1)
            stringBuilder.Append("<td><span class='label label-Available'><i class='icon-ok icon-white'></i></span></td>");
          else
            stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
          DataRow[] dataRowArray7 = this.asset_properties_data.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and property_name='book_holiday'");
          if (dataRowArray7.Length > 0)
          {
            if (Convert.ToBoolean(dataRowArray7[0]["property_value"]))
              stringBuilder.Append("<td><span class='label label-Available'><i class='icon-ok icon-white'></i></span></td>");
            else
              stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
          }
          else if (boolean2)
            stringBuilder.Append("<td><span class='label label-Available'><i class='icon-ok icon-white'></i></span></td>");
          else
            stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
          if (row["asset_owner_group_id"].ToString() == "" || row["asset_owner_group_id"].ToString() == "0")
          {
            stringBuilder.Append("<td>&nbsp;</td>");
          }
          else
          {
            DataRow[] dataRowArray8 = this.group_data.Tables[0].Select("group_id='" + row["asset_owner_group_id"].ToString() + "'");
            if (dataRowArray8.Length > 0)
              stringBuilder.Append("<td>" + dataRowArray8[0]["group_name"].ToString() + "</td>");
            else
              stringBuilder.Append("<td></td>");
          }
          if (row["status"].ToString() == "1")
            stringBuilder.Append("<td><span class='label label-Available'>Active</span></td>");
          else
            stringBuilder.Append("<td><span class='label label-NotAvailable'>Inactive</span></td>");
          stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
          stringBuilder.Append("<ul class='ddm p-r'>");
          stringBuilder.Append("<li><a href='javascript:callfancybox(" + row["asset_id"].ToString() + ")'><i class='icon-table'> View</i></a></li>");
          if (this.gp.facility_edit)
          {
            stringBuilder.Append("<li><a href='javascript:callLoading(" + row["asset_id"].ToString() + ");'><i class='icon-pencil'></i> Edit</a></li>");
            stringBuilder.Append("<li><a href='asset_bookings.aspx?asset_id=" + row["asset_id"].ToString() + "'><i class='icon-table'></i> Bookings</a></li>");
            if (Convert.ToBoolean(this.current_account.properties["audit_log"]))
              stringBuilder.Append("<li><a href='asset_audit_logs.aspx?asset_id=" + row["asset_id"].ToString() + "'><i class='icon-table'></i> Audit Logs</a></li>");
            if (Convert.ToBoolean(this.current_account.properties["email_log"]))
              stringBuilder.Append("<li><a href='asset_email_logs.aspx?asset_id=" + row["asset_id"].ToString() + "'><i class='icon-table'></i> Email Logs</a></li>");
          }
          if (this.gp.facility_delete)
            stringBuilder.AppendFormat("<li><a onclick='javascript:delete_facilitylist({0})'><i class='icon-trash'></i> Delete</a></li>", (object) row["asset_id"].ToString());
          stringBuilder.Append("</ul>");
          stringBuilder.Append("</div></div></td>");
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
    }
    this.html_table = stringBuilder.ToString();
  }

  private void populate_ui()
  {
    DataRow[] dataRowArray1 = this.setting_data.Tables[0].Select("parameter='building'");
    this.ddl_building.Items.Add(new ListItem("All Buildings", "0"));
    foreach (DataRow dataRow in dataRowArray1)
      this.ddl_building.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    DataRow[] dataRowArray2 = this.setting_data.Tables[0].Select("parameter='category'");
    this.ddl_category.Items.Add(new ListItem("All Categories", "0"));
    foreach (DataRow dataRow in dataRowArray2)
      this.ddl_category.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    DataRow[] dataRowArray3 = this.setting_data.Tables[0].Select("parameter='asset_type'");
    this.ddl_type.Items.Add(new ListItem("All Types", "0"));
    foreach (DataRow dataRow in dataRowArray3)
      this.ddl_type.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
    DataRow[] dataRowArray4 = this.asset_data.Tables[0].Select("asset_owner_group_id is not null");
    this.ddl_owner.Items.Add(new ListItem("All Owner Groups", "0"));
    this.ddl_owner.Items.Add(new ListItem("No Owners", "-1"));
    List<long> longList = new List<long>();
    foreach (DataRow dataRow in dataRowArray4)
    {
      if (!longList.Contains(Convert.ToInt64(dataRow["asset_owner_group_id"])))
      {
        user_group group = this.users.get_group(Convert.ToInt64(dataRow["asset_owner_group_id"]), this.current_user.account_id);
        this.ddl_owner.Items.Add(new ListItem(group.group_name, group.group_id.ToString()));
        longList.Add(Convert.ToInt64(dataRow["asset_owner_group_id"]));
      }
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    try
    {
      this.html = new StringBuilder();
      this.html.Append("<table class='table table-striped table-bordered table-hover' id='assetlist_table'>");
      this.html.Append("<thead>");
      this.html.Append("<tr>");
      this.html.Append("<th style='width:12%;' class='hidden-480'>Code / Name</th>");
      this.html.Append("<th style='width:8%;' class='hidden-480'>Building</th>");
      this.html.Append("<th style='width:6%;' class='hidden-480'>Level</th>");
      this.html.Append("<th style='width:6%;' class='hidden-480'>Capacity</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>Category</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>Type</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>Operating Hrs.</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>Allow Weekends</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>Allow Holidays</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>Owner Group</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>No. of Groups That Can View Only</th>");
      this.html.Append("<th style='width:7%;' class='hidden-480'>No. of Groups That Can Book</th>");
      this.html.Append("<th style='width:6%;' class='hidden-480'>Status</th>");
      this.html.Append("<th style='width:3%;' class='hidden-480'>Owner Users</th>");
      this.html.Append("</tr>");
      this.html.Append("</thead>");
      string[] strArray1 = this.settings.get_setting("operating_hours", this.current_user.account_id).value.ToString().Split('|');
      foreach (DataRow row1 in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
      {
        string str = "";
        if (row1["asset_owner_group_id"].ToString() != "")
        {
          if (dictionary.ContainsKey(row1["asset_owner_group_id"].ToString()))
          {
            str = dictionary[row1["asset_owner_group_id"].ToString()];
          }
          else
          {
            foreach (DataRow row2 in (InternalDataCollectionBase) this.users.get_users_by_group(Convert.ToInt64(row1["asset_owner_group_id"]), this.current_user.account_id).Tables[0].Rows)
              str = str + row2["full_name"].ToString() + ", ";
            dictionary.Add(row1["asset_owner_group_id"].ToString(), str);
          }
        }
        long num = 0;
        if (row1["asset_owner_group_id"].ToString() != "")
          num = Convert.ToInt64(row1["asset_owner_group_id"]);
        bool flag = false;
        if (this.gp.isAdminType)
          flag = true;
        if (!flag)
        {
          foreach (user_group userGroup in this.current_user.groups.Values)
          {
            if (userGroup.group_id == num)
              flag = true;
          }
        }
        if (flag)
        {
          this.html.Append("<tr>");
          this.html.Append("<td>" + row1["code"].ToString() + "/" + row1["name"].ToString() + "</td>");
          this.html.Append("<td>" + this.setting_data.Tables[0].Select("setting_id='" + row1["building_id"].ToString() + "'")[0]["value"].ToString() + "</td>");
          this.html.Append("<td>" + this.setting_data.Tables[0].Select("setting_id='" + row1["level_id"].ToString() + "'")[0]["value"].ToString() + "</td>");
          this.html.Append("<td>" + row1["capacity"].ToString() + "</td>");
          this.html.Append("<td>" + this.setting_data.Tables[0].Select("setting_id='" + row1["category_id"].ToString() + "'")[0]["value"].ToString() + "</td>");
          this.html.Append("<td>" + this.setting_data.Tables[0].Select("setting_id='" + row1["asset_type"].ToString() + "'")[0]["value"].ToString() + "</td>");
          DataRow[] dataRowArray1 = this.asset_properties_data.Tables[0].Select("asset_id='" + row1["asset_id"].ToString() + "' and property_name='operating_hours'");
          if (dataRowArray1.Length > 0)
          {
            string[] strArray2 = dataRowArray1[0]["property_value"].ToString().Split('|');
            this.html.Append("<td>" + Convert.ToDateTime(strArray2[0]).ToString("HH:mm") + " to " + Convert.ToDateTime(strArray2[1]).ToString("HH:mm") + "</td>");
          }
          else
            this.html.Append("<td>" + Convert.ToDateTime(strArray1[0]).ToString("HH:mm") + " to " + Convert.ToDateTime(strArray1[1]).ToString("HH:mm") + "</td>");
          bool boolean = Convert.ToBoolean(this.settings.get_setting("book_weekend", this.current_account.account_id).value);
          Convert.ToBoolean(this.settings.get_setting("book_holiday", this.current_account.account_id).value);
          DataRow[] dataRowArray2 = this.asset_properties_data.Tables[0].Select("asset_id='" + row1["asset_id"].ToString() + "' and property_name='book_weekend'");
          if (dataRowArray2.Length > 0)
          {
            if (Convert.ToBoolean(dataRowArray2[0]["property_value"]))
              this.html.Append("<td>Allowed</td>");
            else
              this.html.Append("<td>Not Allowed</td>");
          }
          else if (boolean)
            this.html.Append("<td>Allowed</td>");
          else
            this.html.Append("<td>Not Allowed</td>");
          DataRow[] dataRowArray3 = this.asset_properties_data.Tables[0].Select("asset_id='" + row1["asset_id"].ToString() + "' and property_name='book_holiday'");
          if (dataRowArray3.Length > 0)
          {
            if (Convert.ToBoolean(dataRowArray3[0]["property_value"]))
              this.html.Append("<td>Allowed</td>");
            else
              this.html.Append("<td>Not Allowed</td>");
          }
          else if (boolean)
            this.html.Append("<td>Allowed</td>");
          else
            this.html.Append("<td>Not Allowed</td>");
          if (row1["asset_owner_group_id"].ToString() == "" || row1["asset_owner_group_id"].ToString() == "0")
          {
            this.html.Append("<td>&nbsp;</td>");
          }
          else
          {
            DataRow[] dataRowArray4 = this.group_data.Tables[0].Select("group_id='" + row1["asset_owner_group_id"].ToString() + "'");
            if (dataRowArray4.Length > 0)
              this.html.Append("<td>" + dataRowArray4[0]["group_name"].ToString() + "</td>");
            else
              this.html.Append("<td></td>");
          }
          this.html.Append("<td>" + row1["is_view_only_count"].ToString() + " </td>");
          this.html.Append("<td>" + row1["is_book_count"].ToString() + "</td>");
          if (row1["status"].ToString() == "1")
            this.html.Append("<td>Active</td>");
          else
            this.html.Append("<td>Inactive</td>");
          this.html.Append("<td>");
          this.html.Append(str);
          this.html.Append("</td>");
          this.html.Append("</tr>");
        }
      }
      this.html.Append("<tr><td colspan='12'>Generated By: " + this.current_user.full_name + "</td></tr>");
      this.html.Append("<tr><td colspan='12'>Generated On: " + this.current_timestamp.ToString(api_constants.display_datetime_format) + "</td></tr>");
      this.html.Append("</tbody>");
      this.html.Append("</table>");
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Assets List_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.html.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_filter_Click(object sender, EventArgs e) => this.populate_rooms(this.ddl_building.SelectedItem.Value, this.ddl_category.SelectedItem.Value, this.ddl_type.SelectedItem.Value, this.ddl_owner.SelectedItem.Value);

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
