// Decompiled with JetBrains decompiler
// Type: administration_black_list_Delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_black_list_Delete : fbs_base_page, IRequiresSessionState
{
  private long blacklist_id;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["blacklist_user"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (!this.BlockList_Delete())
        return;
      this.Response.Redirect("~/administration/users_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private bool BlockList_Delete()
  {
    blacklist blacklist = new blacklist();
    bool flag = false;
    try
    {
      if (this.Request.QueryString["blacklist_id"] != null)
      {
        this.blacklist_id = Convert.ToInt64(this.Request.QueryString["blacklist_id"].ToString());
        blacklist = this.blapi.get_blacklist(this.blacklist_id, this.current_user.account_id);
        blacklist.modified_by = this.current_user.user_id;
      }
      this.blapi.delete_blacklist(blacklist);
      this.capi.remove_cache("bl");
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["user_id"]))
      {
        this.Response.Redirect("users_list.aspx", true);
      }
      else
      {
        this.Session["Blacklist_delete"] = (object) "S";
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
