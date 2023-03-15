// Decompiled with JetBrains decompiler
// Type: administration_panels_panels
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

public class administration_panels_panels : fbs_base_page, IRequiresSessionState
{
  protected Button btn_filters;
  public string html_panels;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    DataSet devices = new device_admin_api().get_devices(this.current_user.account_id);
    DataSet assets = this.assets.get_assets(this.current_user.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) devices.Tables[0].Rows)
    {
      string str = "";
      DataRow[] dataRowArray = assets.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "'");
      if (dataRowArray.Length > 0)
        str = dataRowArray[0]["name"].ToString();
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["device_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["device_code"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["mac_address"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["serial_no"].ToString() + "</td>");
      stringBuilder.Append("<td>" + str + "</td>");
      if (row["status"].ToString() == "1")
        stringBuilder.Append("<td><span class='label label-Available'><i class='icon-ok icon-white'></i></span></td>");
      else
        stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
      stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
      stringBuilder.Append("<ul class='ddm p-r'>");
      stringBuilder.Append("<li><a href='emulate.aspx?did=" + row["device_id"].ToString() + "&sc=" + row["app_config_id"].ToString() + "'><i class='icon-table'> View</i></a></li>");
      stringBuilder.Append("<li><a href='panel.aspx?did=" + row["device_id"].ToString() + "'><i class='icon-pencil'> Edit</i></a></li>");
      stringBuilder.Append("</ul></div></div></td>");
      stringBuilder.Append("</tr>");
    }
    this.html_panels = stringBuilder.ToString();
  }
}
