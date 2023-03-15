// Decompiled with JetBrains decompiler
// Type: administration_user_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_user_delete : fbs_base_page, IRequiresSessionState
{
  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (string.IsNullOrWhiteSpace(this.Request.QueryString["did"]))
        return;
      user user = this.users.get_user(Convert.ToInt64(this.Request.QueryString["did"]), this.current_user.account_id);
      user.modified_by = this.current_user.user_id;
      this.users.delete_user(user);
      this.Response.Redirect("users_list.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
