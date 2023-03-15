// Decompiled with JetBrains decompiler
// Type: administration_user_groups_list
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

public class administration_user_groups_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public DataSet data;
  public string success = "";
  protected HtmlGenericControl div_add_group;
  protected Button btnExportExcel;
  protected Button btn_group;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["user_groups"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.groups_view)
      this.redirect_unauthorized();
    if (!this.IsPostBack)
    {
      string str;
      try
      {
        str = this.Request.QueryString["action"];
      }
      catch
      {
        str = "";
      }
      if (str == "copy")
        this.copy_group();
      this.data = this.users.get_groups(this.current_user.account_id);
      this.populate_ui(this.data);
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
    }
    if (this.gp.groups_add)
      this.div_add_group.Visible = true;
    else
      this.div_add_group.Visible = false;
    if (this.Session["Usergroup"] == null || this.Session["Usergroup"] != (object) "S")
      return;
    this.success = "S";
    this.Session.Remove("Usergroup");
  }

  private void copy_group()
  {
    user_group group = this.users.get_group(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id);
    List<asset_permission> assetPermissions = this.assets.get_asset_permissions(group.group_id, group.account_id);
    group.group_name += " - (Copy)";
    group.group_id = 0L;
    user_group userGroup = this.users.update_group(group);
    if (userGroup.group_id > 0L)
    {
      foreach (asset_permission assetPermission in assetPermissions)
      {
        assetPermission.asset_permission_id = 0L;
        assetPermission.group_id = userGroup.group_id;
        this.assets.update_assets_permissions(assetPermission);
      }
    }
    this.Response.Redirect("user_groups_list.aspx");
  }

  protected void btn_group_Click(object sender, EventArgs e)
  {
    Dictionary<long, string> dictionary1 = new Dictionary<long, string>();
    Dictionary<long, string> dictionary2 = new Dictionary<long, string>();
    StringBuilder stringBuilder = new StringBuilder();
    DataSet dataSet = this.assets.view_assets(this.current_user.account_id);
    DataSet groups = this.users.get_groups(this.current_user.account_id);
    DataSet aclGroup = this.reportings.get_acl_group(this.current_user.account_id);
    foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      dictionary1.Add(Convert.ToInt64(row["asset_id"]), row["name"].ToString());
    foreach (DataRow row in (InternalDataCollectionBase) groups.Tables[0].Rows)
      dictionary2.Add(Convert.ToInt64(row["group_id"]), row["group_name"].ToString());
    stringBuilder.Append("<h1>Group Room ACL List</h1>");
    stringBuilder.Append("<table>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<td><b>User Groups</b></td><td></td>");
    foreach (string str in dictionary1.Values)
      stringBuilder.Append("<td colspan='2'><b>" + str + "</b></td>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<td></td><td></td>");
    foreach (string str in dictionary1.Values)
    {
      stringBuilder.Append("<td>Can View</td>");
      stringBuilder.Append("<td>Can Book</td>");
    }
    stringBuilder.Append("</tr>");
    foreach (long key1 in dictionary2.Keys)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + dictionary2[key1] + "</td><td></td>");
      foreach (long key2 in dictionary1.Keys)
      {
        string str1 = "N";
        string str2 = "N";
        DataRow[] dataRowArray = aclGroup.Tables[0].Select("asset_id='" + (object) key2 + "' and group_id='" + (object) key1 + "'");
        bool flag1;
        bool flag2;
        if (dataRowArray.Length > 0)
        {
          flag1 = Convert.ToBoolean(dataRowArray[0]["is_view"]);
          flag2 = Convert.ToBoolean(dataRowArray[0]["is_book"]);
        }
        else
        {
          flag1 = false;
          flag2 = false;
        }
        if (flag1)
          str2 = "Y";
        if (flag2)
          str1 = "Y";
        stringBuilder.Append("<td>" + str2 + "</td>");
        stringBuilder.Append("<td>" + str1 + "</td>");
      }
      stringBuilder.Append("</tr>");
    }
    stringBuilder.Append("</table>");
    stringBuilder.Append("<h1>Users in Group List</h1>");
    stringBuilder.Append("<table>");
    stringBuilder.Append("<tr><td><b>Group Name</b></td><td><b>Group Type</b></td><td><b>Full Name</b></td><td><b>Email</b></td><td><b>Status</b></td></tr>");
    foreach (DataRow row1 in (InternalDataCollectionBase) groups.Tables[0].Rows)
    {
      if (row1["group_name"].ToString() != "All Users")
      {
        string str3 = "";
        if (row1["group_type"].ToString() == "1")
          str3 = "Administrator";
        if (row1["group_type"].ToString() == "2")
          str3 = "Room Owner";
        if (row1["group_type"].ToString() == "3")
          str3 = "Requestor";
        foreach (DataRow row2 in (InternalDataCollectionBase) this.users.get_users_by_group(Convert.ToInt64(row1["group_id"]), this.current_user.account_id).Tables[0].Rows)
        {
          string str4 = !(row2["status"].ToString() == "1") ? "Inactive" : "Active";
          stringBuilder.Append("<tr><td>" + row1["group_name"].ToString() + "</td><td>" + str3 + "</td><td>" + row2["full_name"].ToString() + "</td><td>" + row2["email"].ToString() + "</td><td>" + str4 + "</td></tr>");
        }
      }
    }
    stringBuilder.Append("</table>");
    stringBuilder.Append("<p>Generated On:" + (object) this.current_timestamp + "</p>");
    stringBuilder.Append("<p>Generated By:" + this.current_user.full_name + "</p>");
    this.Response.Clear();
    this.Response.AddHeader("content-disposition", "attachment;filename=ACL_report" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
    this.Response.Charset = "";
    this.Response.ContentType = Resources.fbs.excel_mime_type;
    this.Response.Write(stringBuilder.ToString());
    this.Response.End();
  }

  private void populate_ui(DataSet data)
  {
    try
    {
      DataSet dataSet1 = new DataSet();
      DataSet groupMappings = (DataSet) this.ViewState["user_group_data"];
      if (groupMappings == null)
      {
        groupMappings = this.users.get_group_mappings(this.current_user.account_id);
        this.ViewState.Add("user_group_data", (object) groupMappings);
      }
      DataSet dataSet2 = this.assets.view_assets(this.current_user.account_id);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='grouplist_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:25%' class='hidden-480'>Group Name</th>");
      stringBuilder.Append("<th style='width:10%' class='hidden-480'>Group Type</th>");
      stringBuilder.Append("<th style='width:20%' class='hidden-480'>Description</th>");
      stringBuilder.Append("<th style='width:28%' class='hidden-480'>Assets</th>");
      stringBuilder.Append("<th style='width:8%' class='hidden-480'>User Count</th>");
      stringBuilder.Append("<th style='width:5%' class='hidden-480'>Status</th>");
      stringBuilder.Append("<th style='width:5%'  class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      foreach (DataRow row1 in (InternalDataCollectionBase) data.Tables[0].Rows)
      {
        DataRow[] dataRowArray = groupMappings.Tables[0].Select("group_id='" + row1["group_id"].ToString() + "'");
        DataSet assetsByOwner = this.assets.get_assets_by_owner(Convert.ToInt64(row1["group_id"].ToString()), this.current_user.account_id);
        stringBuilder.Append("<tr class='odd gradeX'>");
        stringBuilder.Append("<td>" + row1["group_name"].ToString() + "</td>");
        if (row1["group_type"].ToString() == "1")
          stringBuilder.Append("<td><span class='label label-success'>Administrator</span></td>");
        else if (row1["group_type"].ToString() == "2")
          stringBuilder.Append("<td><span class='label label-info'>Super User</span></td>");
        else if (row1["group_type"].ToString() == "3")
          stringBuilder.Append("<td><span class='label label-warning'>Requestor</span></td>");
        if (row1["group_type"].ToString() == "0")
          stringBuilder.Append("<td><span class='label label-inverse'>" + api_constants.all_users_text + "</span></td>");
        stringBuilder.Append("<td>" + row1["description"].ToString() + "</td>");
        if (assetsByOwner != null)
        {
          if (assetsByOwner.Tables[0].Rows.Count > 0)
          {
            stringBuilder.Append("<td>");
            string str = "";
            foreach (DataRow row2 in (InternalDataCollectionBase) assetsByOwner.Tables[0].Rows)
            {
              str = str + "<a  href='javascript:callfancybox(" + row2["asset_id"].ToString() + ")'>" + row2["name"].ToString() + "</a>";
              str += " , ";
            }
            stringBuilder.Append(str.Substring(0, str.Length - 2));
            stringBuilder.Append("</td>");
          }
          else
            stringBuilder.Append("<td></td>");
        }
        else
          stringBuilder.Append("<td></td>");
        stringBuilder.Append("<td>" + (object) dataRowArray.Length + "</td>");
        if (Convert.ToInt32(row1["status"]) == 1)
          stringBuilder.Append("<td><span class='label label-Active'>Active</span></td>");
        else
          stringBuilder.Append("<td><span class='label label-DeActive'>Inactive</span></td>");
        if (row1["group_name"].ToString().ToUpper() != api_constants.all_users_text.ToUpper())
        {
          stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
          stringBuilder.Append("<ul class='ddm p-r'>");
          if (this.gp.groups_edit)
          {
            stringBuilder.AppendFormat("<li><a href='user_group_form.aspx?id={0}'><i class='icon-pencil'></i> Edit</a></li>", (object) row1["group_id"].ToString());
            stringBuilder.AppendFormat("<li><a href='user_groups_list.aspx?action=copy&id={0}'><i class='icon-copy'></i> Create Copy</a></li>", (object) row1["group_id"].ToString());
            stringBuilder.AppendFormat("<li><a href='user_group_assign_form.aspx?id={0}'><i class='icon-pencil'></i> Assign Members</a></li>", (object) row1["group_id"].ToString());
          }
          if (this.gp.groups_delete && groupMappings.Tables[0].Select("group_id='" + row1["group_id"].ToString() + "'").Length == 0 && dataSet2.Tables[0].Select("asset_owner_group_id='" + row1["group_id"].ToString() + "'").Length == 0)
            stringBuilder.AppendFormat("<li><a href='javascript:delete_group({0})'><i class='icon-trash'></i> Delete</a></li>", (object) row1["group_id"].ToString());
          stringBuilder.Append("</ul>");
          stringBuilder.Append("</div></div></td>");
        }
        else
          stringBuilder.Append("<td></td>");
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
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
      DataSet groups = this.users.get_groups(this.current_user.account_id);
      groups.Tables[0].Columns.Add("status_string");
      groups.Tables[0].Columns.Add("group_type_string");
      foreach (DataRow row in (InternalDataCollectionBase) groups.Tables[0].Rows)
      {
        row["group_name"] = (object) row["group_name"].ToString();
        if (row["group_type"].ToString() == "1")
          row["group_type_string"] = (object) "Administrator";
        else if (row["group_type"].ToString() == "2")
          row["group_type_string"] = (object) "Super User";
        else if (row["group_type"].ToString() == "3")
          row["group_type_string"] = (object) "Requestor";
        if (row["group_type"].ToString() == "0")
          row["group_type_string"] = (object) "All  Users";
        row["status_string"] = Convert.ToInt32(row["status"]) != 1 ? (object) "Inactive" : (object) "Active";
        groups.Tables[0].AcceptChanges();
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("group_name", "Group Name");
      dictionary.Add("status_string", "Status");
      dictionary.Add("group_type_string", "Group Type");
      excel excel = new excel();
      excel.file_name = "Usergrouplist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = groups;
      excel.column_names = dictionary;
      excel.table_identifier = "Usergrouplist";
      excel.header = "User Group List";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=groupslist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.exapi.get_excel(excel));
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
