// Decompiled with JetBrains decompiler
// Type: administration_user_group_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_user_group_delete : fbs_base_page, IRequiresSessionState
{
  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["user_groups"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (string.IsNullOrWhiteSpace(this.Request.QueryString["did"]))
      return;
    try
    {
      user_group userGroup = new user_group();
      userGroup = this.users.delete_group(this.users.get_group(Convert.ToInt64(this.Request.QueryString["did"]), this.current_user.account_id));
      this.Response.Redirect("user_groups_list.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
