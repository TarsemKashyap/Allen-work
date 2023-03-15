// Decompiled with JetBrains decompiler
// Type: resource_item_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;

public class resource_item_delete : fbs_base_page, IRequiresSessionState
{
  protected HtmlForm form1;
  private long res_id;
  private List<long> allowed_items;
  private bool is_admin;
  private DataSet user_item_data;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    foreach (user_group userGroup in this.current_user.groups.Values)
    {
      if (userGroup.group_type == 1)
        this.is_admin = true;
      else if (!this.is_admin)
        this.is_admin = false;
    }
    if (!this.is_admin)
    {
      this.get_allowed_items();
      if (!this.allowed_items.Contains(Convert.ToInt64(this.Request.QueryString["id"])))
        this.redirect_unauthorized();
    }
    try
    {
      this.resource_delete();
      this.Response.Redirect("resource_items.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void get_allowed_items()
  {
    this.allowed_items = new List<long>();
    this.user_item_data = this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, this.str_resource_module);
    foreach (DataRow row in (InternalDataCollectionBase) this.user_item_data.Tables[0].Rows)
      this.allowed_items.Add(Convert.ToInt64(row["item_id"]));
  }

  private bool resource_delete()
  {
    additional_resource additionalResource = new additional_resource();
    bool flag = false;
    try
    {
      if (this.Request.QueryString["id"] != null)
      {
        this.res_id = Convert.ToInt64(this.Request.QueryString["id"].ToString());
        additionalResource = this.resapi.get_resource_item_obj(this.res_id, this.current_user.account_id, this.str_resource_module);
        additionalResource.modified_by = this.current_user.user_id;
        additionalResource.module_name = "resource_module";
      }
      this.resapi.delete_resource_item(additionalResource);
      if (additionalResource.item_id > 0L)
      {
        this.Session["delete"] = (object) "S";
        flag = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
      flag = false;
    }
    return flag;
  }
}
