// Decompiled with JetBrains decompiler
// Type: administration_rooms
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

public class administration_rooms : fbs_base_page, IRequiresSessionState
{
  public string html_tree;
  protected Panel pnl_building;
  protected Panel pnl_level;
  protected Panel pnl_room;

  protected new void Page_Load(object sender, EventArgs e)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.IsPostBack)
      return;
    DataSet settings = this.settings.get_settings(this.current_user.account_id, "building");
    this.settings.get_settings(this.current_user.account_id, "level");
    DataSet assets = this.assets.get_assets(this.current_user.account_id);
    stringBuilder.Append("<ul class='tree' id='tree_1'>");
    stringBuilder.Append("<li>");
    stringBuilder.Append("<a href='#' data-role='branch' class='tree-toggle' data-toggle='branch' data-value='0'>Buildings");
    stringBuilder.Append("</a>");
    stringBuilder.Append("<ul class='branch in'>");
    foreach (DataRow row in (InternalDataCollectionBase) settings.Tables[0].Rows)
    {
      stringBuilder.Append("<li>");
      stringBuilder.Append("<a href='#' id='b_" + row["setting_id"].ToString() + "' data-role='branch' class='tree-toggle closed' data-toggle='branch' data-value='" + row["setting_id"].ToString() + "'><i class='icon-sitemap'></i>  " + row["value"].ToString() + "</a>");
      stringBuilder.Append(this.populate_rooms(assets, row["setting_id"].ToString()));
      stringBuilder.Append("</li>");
    }
    stringBuilder.Append("</ul");
    stringBuilder.Append("</li>");
    stringBuilder.Append("</ul>");
    this.html_tree = stringBuilder.ToString();
  }

  private string populate_rooms(DataSet rooms, string building_id)
  {
    StringBuilder stringBuilder = new StringBuilder();
    long int64 = Convert.ToInt64(building_id);
    DataRow[] dataRowArray = rooms.Tables[0].Select("building_id='" + (object) int64 + "'");
    stringBuilder.Append("<ul class='branch'>");
    foreach (DataRow dataRow in dataRowArray)
      stringBuilder.Append("<li><a href='#' data-role='leaf'><i class='icon-suitcase'></i>  " + dataRow["name"].ToString() + "</a></li>");
    stringBuilder.Append("</ul>");
    return stringBuilder.ToString();
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
