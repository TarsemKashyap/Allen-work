// Decompiled with JetBrains decompiler
// Type: administration_holiday_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_holiday_delete : fbs_base_page, IRequiresSessionState
{
  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["holidays"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      string str = this.Request.QueryString["id"];
      if (!(str != "") || this.holidays.delete_holiday(this.holidays.get_holiday(Convert.ToInt64(str), this.current_user.account_id)).holiday_id <= 0L)
        return;
      this.Session["holiday"] = (object) "D";
      this.Response.Redirect("holiday_list.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
