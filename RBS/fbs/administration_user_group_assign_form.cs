// Decompiled with JetBrains decompiler
// Type: administration_user_group_assign_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_user_group_assign_form : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public string assign_gorupid = "";
  public string group_name = "";
  public string first_username = "";
  public string sucess_mag = "";
  protected HiddenField hdnGroupID;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected TextBox txtUser;
  protected Panel myPanel;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hfUserId;
  protected Button btnAssignToGroup;
  protected Button btnBulkAssignMembers;
  protected Button btnRemoveSelectedUsers;
  protected Button btn_exportExcel;
  protected HiddenField hdn_firstuser_id;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["user_groups"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.groups_view)
      this.redirect_unauthorized();
    this.btnAssignToGroup.Attributes.Add("style", "margin-top:-10px;");
    try
    {
      if (this.IsPostBack)
        return;
      long num = 0;
      if (string.IsNullOrWhiteSpace(this.Request.QueryString["id"]))
      {
        this.redirect_unauthorized();
      }
      else
      {
        num = Convert.ToInt64(this.Request.QueryString["id"]);
        DataSet dataSet = this.reportings.getgroup_name(num, this.current_user.account_id);
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
          this.group_name = dataSet.Tables[0].Rows[0][0].ToString();
        this.assign_gorupid = this.Request.QueryString["id"];
      }
      this.hdnGroupID.Value = num.ToString();
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["user_id"]))
        this.delete_user_from_group(Convert.ToInt64(this.Request.QueryString["user_id"]), num);
      this.populate_assigned_users_list();
      if (this.Session["user_group_update"] != null)
      {
        this.sucess_mag = this.Session["user_group_update"] != (object) "D" ? Resources.fbs.User_group_sucessfully_added : Resources.fbs.User_group_sucessfully_Deleted;
        this.Session.Remove("user_group_update");
      }
      else
        this.sucess_mag = "";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_assigned_users_list()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table_2'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'><input type='checkbox'  id='cbSelectAll' runat='server' onclick='SelectAll(this.id)' class='group-checkable' data-set='#list_table_2 .checkboxes'  /></th>");
      stringBuilder.Append("<th class='hidden-480'>Full Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Email</th>");
      stringBuilder.Append("<th class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private void delete_user_from_group(long user_id, long group_id)
  {
    try
    {
      user user = this.users.get_user(user_id, this.current_user.account_id);
      user_group_mapping userGroupMapping = new user_group_mapping();
      userGroupMapping.user_id = user_id;
      userGroupMapping.group_id = group_id;
      userGroupMapping.account_id = this.current_user.account_id;
      userGroupMapping.modified_by = this.current_user.user_id;
      userGroupMapping.modified_on = this.current_timestamp;
      userGroupMapping.record_id = user.record_id;
      this.users.delete_user_group_mapping(userGroupMapping);
      this.Session["user_group_update"] = (object) "D";
      this.Response.Redirect("user_group_assign_form.aspx?id=" + group_id.ToString(), true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btnAssignToGroup_Click(object sender, EventArgs e)
  {
    try
    {
      long user_id = !(this.txtUser.Text != "") ? Convert.ToInt64(this.hdn_firstuser_id.Value) : Convert.ToInt64(this.hfUserId.Value);
      long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
      user user = this.users.get_user(user_id, this.current_user.account_id);
      user_group_mapping userGroupMapping = new user_group_mapping();
      userGroupMapping.user_id = user_id;
      userGroupMapping.group_id = int64;
      userGroupMapping.account_id = this.current_user.account_id;
      userGroupMapping.created_by = this.current_user.user_id;
      userGroupMapping.created_on = this.current_timestamp;
      userGroupMapping.modified_by = this.current_user.user_id;
      userGroupMapping.modified_on = this.current_timestamp;
      userGroupMapping.record_id = user.record_id;
      this.users.insert_user_group_mapping(userGroupMapping);
      this.Session["user_group_update"] = (object) "U";
      this.Response.Redirect("user_group_assign_form.aspx?id=" + int64.ToString(), true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btnRemoveSelectedUsers_Click(object sender, EventArgs e)
  {
    try
    {
      long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
      string[] strArray = this.Request.Form["userids"].Split(',');
      if (strArray.Length > 0)
      {
        foreach (string str in strArray)
        {
          user user = this.users.get_user(Convert.ToInt64(str), this.current_user.account_id);
          user_group_mapping userGroupMapping = new user_group_mapping();
          userGroupMapping.user_id = Convert.ToInt64(str);
          userGroupMapping.group_id = int64;
          userGroupMapping.account_id = this.current_user.account_id;
          userGroupMapping.modified_by = this.current_user.user_id;
          userGroupMapping.modified_on = this.current_timestamp;
          userGroupMapping.record_id = user.record_id;
          this.users.delete_user_group_mapping(userGroupMapping);
        }
      }
      this.Response.Redirect("user_group_assign_form.aspx?id=" + int64.ToString(), true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_exportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      DataSet usersByGroup = this.users.get_users_by_group(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id);
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_AssignFormList_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='font-size:16pt;'>Assign Form List</th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Full Name</th>");
      stringBuilder.Append("<th>Email</th>");
      stringBuilder.Append("</tr>");
      foreach (DataRow row in (InternalDataCollectionBase) usersByGroup.Tables[0].Rows)
      {
        stringBuilder.Append("<tr class='odd gradeX'>");
        stringBuilder.Append("<td>" + row["full_name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["email"].ToString() + "</td>");
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Generated By </th>");
      stringBuilder.Append("<td>" + this.current_user.full_name + " </td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Generated on </th>");
      stringBuilder.Append("<td align='left'>" + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format) + "</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btnBulkAssignMembers_Click(object sender, EventArgs e)
  {
    long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
    DataSet usersNotInMappings = this.users.get_users_not_in_mappings(this.current_user.account_id, int64);
    if (this.utilities.isValidDataset(usersNotInMappings))
    {
      user_group_mapping userGroupMapping = new user_group_mapping();
      userGroupMapping.group_id = int64;
      userGroupMapping.account_id = this.current_user.account_id;
      userGroupMapping.created_by = this.current_user.user_id;
      userGroupMapping.created_on = this.current_timestamp;
      userGroupMapping.modified_by = this.current_user.user_id;
      userGroupMapping.modified_on = this.current_timestamp;
      for (int index = 0; index < usersNotInMappings.Tables[0].Rows.Count; ++index)
      {
        userGroupMapping.user_id = Convert.ToInt64(usersNotInMappings.Tables[0].Rows[index]["user_id"]);
        userGroupMapping.record_id = Guid.NewGuid();
        userGroupMapping = this.users.insert_user_group_mapping(userGroupMapping);
      }
      this.Session["user_group_update"] = (object) "U";
      this.Response.Redirect("user_group_assign_form.aspx?id=" + int64.ToString(), true);
    }
    else
    {
      this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "alert('There is no user to assign');", true);
      this.Response.Redirect("user_group_assign_form.aspx?id=" + int64.ToString(), true);
    }
  }

  protected void btnBulkRemove_Click(object sender, EventArgs e)
  {
    long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
    if (int64 > 0L)
      this.users.bulk_remove_from_group(int64, this.current_user.account_id);
    this.Response.Redirect("user_group_assign_form.aspx?id=" + int64.ToString(), true);
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
