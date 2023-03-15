// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_resource_type_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;

public class administration_additional_resources_resource_type_delete : 
  fbs_base_page,
  IRequiresSessionState
{
  protected HtmlForm form1;
  private long res_type_id;
  private bool is_admin;

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
      this.redirect_unauthorized();
    try
    {
      if (this.resource_type_Delete())
        this.Response.Redirect("resource_types.aspx", true);
      else
        this.Response.Redirect("resource_types.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private bool resource_type_Delete()
  {
    resource_type resourceType = new resource_type();
    bool flag = false;
    try
    {
      if (this.Request.QueryString["id"] != null)
      {
        this.res_type_id = Convert.ToInt64(this.Request.QueryString["id"].ToString());
        resourceType = this.resapi.get_resource_type_obj(this.res_type_id, this.current_user.account_id);
        resourceType.modified_by = this.current_user.user_id;
      }
      this.resapi.delete_resource_type(resourceType);
      if (resourceType.setting_id > 0L)
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
