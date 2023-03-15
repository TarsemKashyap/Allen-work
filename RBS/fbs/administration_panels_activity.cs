// Decompiled with JetBrains decompiler
// Type: administration_panels_activity
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
using System.Web.UI;
using System.Web.UI.WebControls;

public class administration_panels_activity : Page, IRequiresSessionState
{
  protected Button btn_filters;
  public string html_panels;
  private Guid account_id;
  private double timezone;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    this.timezone = 8.0;
    this.account_id = new Guid("f483e05f-caff-4227-ba4c-c899b2782da4");
    asset_api assetApi = new asset_api();
    double num1 = 900000.0;
    int num2 = 0;
    device_admin_api deviceAdminApi = new device_admin_api();
    DataSet devices = deviceAdminApi.get_devices(this.account_id);
    DataSet deviceProperties = deviceAdminApi.get_device_properties("screen_refresh_rate", this.account_id);
    DataSet assets = assetApi.get_assets(this.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    if (deviceAdminApi.get_setting("default_cuttoff", this.account_id) != "")
      num1 = Convert.ToDouble(deviceAdminApi.get_setting("default_cuttoff", this.account_id));
    if (deviceAdminApi.get_setting("default_refresh_rate", this.account_id) != "")
      num2 = Convert.ToInt32(deviceAdminApi.get_setting("default_refresh_rate", this.account_id));
    foreach (DataRow row in (InternalDataCollectionBase) devices.Tables[0].Rows)
    {
      DateTime dateTime = new DateTime();
      try
      {
        dateTime = Convert.ToDateTime(row["last_ping_date"]).AddHours(this.timezone);
      }
      catch
      {
        dateTime = new DateTime(2000, 1, 1);
      }
      double totalMilliseconds = (DateTime.UtcNow.AddHours(this.timezone) - dateTime).TotalMilliseconds;
      DataRow[] dataRowArray1 = deviceProperties.Tables[0].Select("device_id='" + row["device_id"].ToString() + "'");
      if (dataRowArray1.Length > 0)
        Convert.ToInt32(dataRowArray1[0]["value"]);
      string str = "";
      DataRow[] dataRowArray2 = assets.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "'");
      if (dataRowArray2.Length > 0)
        str = dataRowArray2[0]["name"].ToString();
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + str + "</td>");
      if (dateTime.Year > 2000)
        stringBuilder.Append("<td>" + dateTime.ToString("dd-MMM-yy hh:mm tt") + "</td>");
      else
        stringBuilder.Append("<td></td>");
      if (totalMilliseconds > num1)
      {
        if (dateTime.Year == 2000)
          stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
        else
          stringBuilder.Append("<td><span class='label label-NotAvailable'><i class='icon-remove icon-white'></i></span></td>");
      }
      else
        stringBuilder.Append("<td><span class='label label-Available'><i class='icon-ok icon-white'></i></span></td>");
      stringBuilder.Append("</tr>");
    }
    this.html_panels = stringBuilder.ToString();
  }
}
