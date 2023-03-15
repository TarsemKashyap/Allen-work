// Decompiled with JetBrains decompiler
// Type: asset_owner_list
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
using System.Web.UI.HtmlControls;

public class asset_owner_list : fbs_base_page, IRequiresSessionState
{
  public string html_users;
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e) => this.show_groups();

  private bool show_groups()
  {
    if (this.Request.QueryString.Count <= 0)
      return true;
    StringBuilder stringBuilder = new StringBuilder();
    DataSet usersByGroup = this.users.get_users_by_group(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id);
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover'>");
    stringBuilder.Append("<thead><tr><td><b>S.No.</b></td><td><b>Full Name</b></td><td><b>Email</b></td></tr></thead>");
    int num = 1;
    stringBuilder.Append("<tbody>");
    foreach (DataRow row in (InternalDataCollectionBase) usersByGroup.Tables[0].Rows)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + (object) num + "</td>");
      stringBuilder.Append("<td>" + row["full_name"].ToString() + "</td>");
      stringBuilder.Append("<td><a href='mailto:" + row["email"].ToString() + "'>" + row["email"].ToString() + "</a></td>");
      stringBuilder.Append("</tr>");
      ++num;
    }
    stringBuilder.Append("</tbody>");
    stringBuilder.Append("</table>");
    this.html_users = stringBuilder.ToString();
    return false;
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
