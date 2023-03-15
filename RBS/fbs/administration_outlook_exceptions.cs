// Decompiled with JetBrains decompiler
// Type: administration_outlook_exceptions
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

public class administration_outlook_exceptions : fbs_base_page, IRequiresSessionState
{
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["outlook_plugin_access"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    outlook_api outlookApi = new outlook_api();
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) outlookApi.get_users_without_outlook(this.current_user.account_id).Tables[0].Rows)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["full_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["email"].ToString() + "</td>");
      stringBuilder.Append("</tr>");
    }
    this.html_table = stringBuilder.ToString();
  }
}
