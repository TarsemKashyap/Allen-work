// Decompiled with JetBrains decompiler
// Type: administration_hotdesk_heatmap
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_hotdesk_heatmap : fbs_base_page, IRequiresSessionState
{
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected DropDownList ddlStatus;
  protected Button btn_submit;
  protected HiddenField hdn_layout_id;
  private new DataAccess db;
  public string html_body;
  public string json_data;
  private long layout_id;
  public string html_img_url;
  private hotdesk_api hapi = new hotdesk_api();
  private hotdesk_dashboard_api dapi = new hotdesk_dashboard_api();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (this.IsPostBack)
      return;
    DateTime dateTime = new DateTime(this.current_timestamp.Year, this.current_timestamp.Month, this.current_timestamp.Day);
    this.txtFromDate.Text = dateTime.AddMonths(-1).ToString(api_constants.display_datetime_format_short);
    this.txtToDate.Text = dateTime.AddDays(1.0).ToString(api_constants.display_datetime_format_short);
    this.layout_id = 0L;
    try
    {
      this.layout_id = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
      this.layout_id = 0L;
    }
    if (this.layout_id <= 0L)
      return;
    this.hdn_layout_id.Value = this.layout_id.ToString();
    this.db = new DataAccess(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
    DataSet layout = this.hapi.get_layout(this.current_user.account_id, this.layout_id);
    if (layout.Tables[0].Rows.Count <= 0)
      return;
    this.html_img_url = "../../hotdesk/" + layout.Tables[0].Rows[0]["image_name"].ToString();
    this.load_seats(this.layout_id);
    this.load_heatmap(this.layout_id, this.current_timestamp.AddMonths(-1), this.current_timestamp.AddDays(1.0));
  }

  private void load_heatmap(long layout_id, DateTime from, DateTime to)
  {
    StringBuilder stringBuilder = new StringBuilder();
    long num1 = 0;
    long num2 = 0;
    DataSet bookHeatmap = this.dapi.get_book_heatmap(layout_id, this.current_user.account_id, from, to);
    foreach (DataRow row in (InternalDataCollectionBase) bookHeatmap.Tables[0].Rows)
      num2 += Convert.ToInt64(row["sum"]);
    stringBuilder.Append("'data':[");
    foreach (DataRow row in (InternalDataCollectionBase) bookHeatmap.Tables[0].Rows)
    {
      double num3 = Convert.ToDouble(row["sum"]) / (double) num2 * 100.0;
      if (num3 > (double) num1)
        num1 = (long) (int) num3;
      stringBuilder.Append("{'x':" + (object) (Convert.ToInt32(row["x"]) + 14) + ",'y':" + (object) (Convert.ToInt32(row["y"]) + 14) + ",'value':" + (object) (int) num3 + "},");
    }
    stringBuilder.Append("{'x':0,'y':0,'value':0}");
    stringBuilder.Append("]}");
    this.json_data = "{'max':" + (object) num1 + "," + stringBuilder.ToString();
  }

  private void load_seats(long layout_id)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.db.get_dataset("select * from sbt_hotdesk_seats where layout_id='" + (object) layout_id + "' and account_id='" + (object) this.current_user.account_id + "' and status>=0"))
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
      {
        if (row["status"].ToString() == "1")
          stringBuilder.Append("<div style='position: absolute; top: " + row["position_top"].ToString() + "px; left: " + row["position_left"].ToString() + "px; '><img src='../../hotdesk/a.png' class='status-icon' class='tooltip' title='" + row["name"].ToString() + "' /></div>");
        else
          stringBuilder.Append("<div style='position: absolute; top: " + row["position_top"].ToString() + "px; left: " + row["position_left"].ToString() + "px; '><img src='../../hotdesk/n.png' class='status-icon' class='tooltip' title='" + row["name"].ToString() + "' /></div>");
      }
    }
    this.html_body = stringBuilder.ToString();
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    DateTime dateTime1 = Convert.ToDateTime(this.Request.Form[this.txtFromDate.ClientID]);
    DateTime dateTime2 = Convert.ToDateTime(this.Request.Form[this.txtToDate.ClientID]);
    this.txtFromDate.Text = dateTime1.ToString(api_constants.display_datetime_format_short);
    this.txtToDate.Text = dateTime2.ToString(api_constants.display_datetime_format_short);
    this.load_heatmap(Convert.ToInt64(this.hdn_layout_id.Value), dateTime1, dateTime2);
  }
}
