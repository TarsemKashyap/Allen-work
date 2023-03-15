// Decompiled with JetBrains decompiler
// Type: administration_hotdesk_layouts
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
using System.Web.UI.WebControls;

public class administration_hotdesk_layouts : fbs_base_page, IRequiresSessionState
{
  protected HtmlGenericControl div_add_holiday;
  protected HiddenField searchcon;
  protected HiddenField hidorderby;
  protected HiddenField hidorderdir;
  private hotdesk_api hapi = new hotdesk_api();
  public string html_table = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (this.IsPostBack)
      return;
    DataSet settings = this.settings.get_settings(this.current_user.account_id);
    DataSet layouts = this.hapi.get_layouts(this.current_user.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) layouts.Tables[0].Rows)
    {
      int seatCount = this.hapi.get_seat_count(this.current_user.account_id, Convert.ToInt64(row["layout_id"]));
      string str1 = settings.Tables[0].Select("setting_id='" + row["building_id"] + "'")[0]["value"].ToString();
      string str2 = settings.Tables[0].Select("setting_id='" + row["level_id"] + "'")[0]["value"].ToString();
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + (object) seatCount + "</td>");
      stringBuilder.Append("<td>" + str1 + "</td>");
      stringBuilder.Append("<td>" + str2 + "</td>");
      stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
      stringBuilder.Append("<ul class='ddm p-r'>");
      stringBuilder.AppendFormat("<li><a href='layout_form.aspx?id={0}'><i class='icon-pencil'></i> Edit Layout</a></li>", (object) row["layout_id"].ToString());
      stringBuilder.AppendFormat("<li><a href='layout_setup.aspx?id={0}'><i class='icon-pushpin'></i> Setup Seats</a></li>", (object) row["layout_id"].ToString());
      stringBuilder.AppendFormat("<li><a href='heatmap.aspx?id={0}'><i class='icon-bar-chart'></i> Heat Map</a></li>", (object) row["layout_id"].ToString());
      stringBuilder.Append("</ul>");
      stringBuilder.Append("</div></div></td>");
      stringBuilder.Append("</tr>");
    }
    this.html_table = stringBuilder.ToString();
  }
}
