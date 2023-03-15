// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_resource_group_permissions
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_additional_resources_resource_group_permissions : 
  fbs_base_page,
  IRequiresSessionState
{
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HiddenField hdn_perm;
  protected HiddenField hdn_gid;
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
    this.hdn_gid.Value = int64.ToString();
    DataSet permissionsByGroup = this.resapi.get_permissions_by_group(this.current_user.account_id, int64);
    DataSet resourcesList = this.resapi.get_resources_list(this.current_user.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) resourcesList.Tables[0].Rows)
    {
      string str = "";
      DataRow[] dataRowArray = permissionsByGroup.Tables[0].Select("group_id='" + (object) int64 + "' and item_id='" + row["item_id"].ToString() + "'");
      if (dataRowArray.Length > 0 && Convert.ToBoolean(dataRowArray[0]["can_book"]))
        str = "checked";
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
      stringBuilder.Append("<td><input class='chkperm' name='cbc' type='checkbox' " + str + " id='item_" + row["item_id"].ToString() + "' /></td>");
      stringBuilder.Append("</tr>");
    }
    this.html_table = stringBuilder.ToString();
  }

  protected void btn_submit_click(object sender, EventArgs e)
  {
    long int64 = Convert.ToInt64(this.hdn_gid.Value);
    DataSet permissionsByGroup = this.resapi.get_permissions_by_group(this.current_user.account_id, int64);
    foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resources_list(this.current_user.account_id).Tables[0].Rows)
    {
      resource_permission resourcePermission = new resource_permission();
      resourcePermission.account_id = this.current_user.account_id;
      DataRow[] dataRowArray = permissionsByGroup.Tables[0].Select("group_id='" + (object) int64 + "' and item_id='" + row["item_id"].ToString() + "'");
      if (dataRowArray.Length > 0)
      {
        resourcePermission.created_by = Convert.ToInt64(dataRowArray[0]["created_by"]);
        resourcePermission.created_on = Convert.ToDateTime(dataRowArray[0]["created_on"]);
        resourcePermission.group_id = Convert.ToInt64(dataRowArray[0]["group_id"]);
        resourcePermission.item_id = Convert.ToInt64(dataRowArray[0]["item_id"]);
        resourcePermission.modified_by = Convert.ToInt64(dataRowArray[0]["modified_by"]);
        resourcePermission.modified_on = Convert.ToDateTime(dataRowArray[0]["modified_on"]);
        resourcePermission.resource_permission_id = Convert.ToInt64(dataRowArray[0]["resource_permission_id"]);
      }
      else
      {
        resourcePermission.created_by = this.current_user.user_id;
        resourcePermission.created_on = DateTime.UtcNow;
        resourcePermission.group_id = int64;
        resourcePermission.item_id = Convert.ToInt64(row["item_id"]);
        resourcePermission.modified_by = this.current_user.user_id;
        resourcePermission.modified_on = DateTime.UtcNow;
        resourcePermission.resource_permission_id = 0L;
      }
      resourcePermission.can_book = this.hdn_perm.Value.Contains("item_" + row["item_id"].ToString());
      this.resapi.update_resource_permission(resourcePermission);
    }
    this.Response.Redirect("resource_groups.aspx");
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("resource_groups.aspx");
}
