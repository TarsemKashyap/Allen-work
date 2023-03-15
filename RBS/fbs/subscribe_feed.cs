// Decompiled with JetBrains decompiler
// Type: subscribe_feed
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

public class subscribe_feed : fbs_base_page, IRequiresSessionState
{
  protected HiddenField hdnView;
  protected HiddenField totlarecords;
  protected HiddenField hdnSelectedRowCount;
  protected HiddenField hdnBookingIDs;
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["subscribe"]))
      this.Server.Transfer("~//unauthorized.aspx");
    this.show_feeds();
  }

  private void show_feeds()
  {
    StringBuilder stringBuilder = new StringBuilder();
    DataRow[] dataRowArray = this.settings.view_settings(this.current_user.account_id).Tables[0].Select("parameter='meeting_type'");
    stringBuilder.Append("<table cellpadding='10' cellspacing='10'>");
    stringBuilder.Append("<tr><td><b>Type of Meeting</b></td><td><b>RSS</b></td><td><b>iCAL</b></td></tr>");
    foreach (DataRow dataRow in dataRowArray)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + dataRow["value"].ToString() + "</td>");
      stringBuilder.Append("<td><a href='subscribe.aspx?type=rss&id=" + dataRow["setting_id"].ToString() + "&title=" + dataRow["value"].ToString() + "' target='_blank'><img src='assets/img/rss.png'/></a></td>");
      stringBuilder.Append("<td><a href='subscribe.aspx?type=ics&id=" + dataRow["setting_id"].ToString() + "&title=" + dataRow["value"].ToString() + "' target='_blank'><img src='assets/img/ical.png'/></a></td>");
      stringBuilder.Append("</tr>");
    }
    stringBuilder.Append("</table>");
    this.html_table = stringBuilder.ToString();
  }
}
