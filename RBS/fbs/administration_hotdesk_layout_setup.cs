// Decompiled with JetBrains decompiler
// Type: administration_hotdesk_layout_setup
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

public class administration_hotdesk_layout_setup : fbs_base_page, IRequiresSessionState
{
  protected TextBox txt_code;
  protected TextBox txt_x;
  protected TextBox txt_y;
  protected DropDownList ddl_status;
  protected HiddenField hdn_seat_id;
  protected Button Button1;
  protected Button Button2;
  protected HiddenField hdn_layout_id;
  protected Button btn_cancel;
  private new DataAccess db;
  public string html_body;
  private long layout_id;
  public string html_img_url;
  private hotdesk_api hapi = new hotdesk_api();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      this.Server.Transfer("~//unauthorized.aspx");
    this.layout_id = 0L;
    try
    {
      this.layout_id = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
      this.layout_id = 0L;
    }
    this.hdn_seat_id.Value = "0";
    if (this.layout_id <= 0L)
      return;
    this.hdn_layout_id.Value = this.layout_id.ToString();
    this.db = new DataAccess(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
    DataSet layout = this.hapi.get_layout(this.current_user.account_id, this.layout_id);
    if (layout.Tables[0].Rows.Count <= 0)
      return;
    this.html_img_url = "../../hotdesk/" + layout.Tables[0].Rows[0]["image_name"].ToString();
    this.load_seats(this.layout_id);
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    int int32_1 = Convert.ToInt32(Convert.ToDouble(this.txt_x.Text));
    int int32_2 = Convert.ToInt32(Convert.ToDouble(this.txt_y.Text));
    int num1 = int32_1 - 12;
    int num2 = int32_2 - 12;
    this.db.open_connection();
    if (this.hdn_seat_id.Value == "0")
      this.db.execute_scalar("insert into sbt_hotdesk_seats (layout_id,name,position_top,position_left,account_id,created_on,created_by,modified_on,modified_by,status) values('" + this.hdn_layout_id.Value + "','" + this.txt_code.Text + "','" + (object) num2 + "','" + (object) num1 + "','" + (object) this.current_user.account_id + "',getutcdate(),'" + (object) this.current_user.user_id + "',getutcdate(),'" + (object) this.current_user.user_id + "','" + this.ddl_status.SelectedItem.Value + "')");
    else
      this.db.execute_scalar("update sbt_hotdesk_seats set name='" + this.txt_code.Text + "',position_top='" + this.txt_x.Text + "',position_left='" + this.txt_y.Text + "',modified_on=getutcdate(),modified_by='" + (object) this.current_user.user_id + "',status='" + this.ddl_status.SelectedItem.Value + "' where account_id='" + (object) this.account_id + "' and seat_id='" + this.hdn_seat_id.Value + "'");
    this.db.close_connection();
    this.Response.Redirect("layout_setup.aspx?id=" + (object) this.layout_id);
  }

  private void load_seats(long layout_id)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.db.get_dataset("select * from sbt_hotdesk_seats where layout_id='" + (object) layout_id + "' and account_id='" + (object) this.current_user.account_id + "' and status>=0"))
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
      {
        if (row["status"].ToString() == "1")
          stringBuilder.Append("<div style='position: absolute; top: " + row["position_top"].ToString() + "px; left: " + row["position_left"].ToString() + "px; '><abbr rel='tooltip' data-toggle='bottom' title='" + row["name"].ToString() + "'><a href='javascript:edit_seat(" + row["seat_id"].ToString() + "," + row["position_top"].ToString() + "," + row["position_left"].ToString() + "," + row["status"].ToString() + ",\"" + row["name"].ToString() + "\");'><img src='../../hotdesk/a.png' class='status-icon' class='tooltip' title='" + row["name"].ToString() + "' /></a></abbr></div>");
        else
          stringBuilder.Append("<div style='position: absolute; top: " + row["position_top"].ToString() + "px; left: " + row["position_left"].ToString() + "px; '><abbr rel='tooltip' data-toggle='bottom' title='" + row["name"].ToString() + "'><a href='javascript:edit_seat(" + row["seat_id"].ToString() + "," + row["position_top"].ToString() + "," + row["position_left"].ToString() + "," + row["status"].ToString() + ",\"" + row["name"].ToString() + "\");'><img src='../../hotdesk/n.png' class='status-icon' class='tooltip' title='" + row["name"].ToString() + "' /></a></abbr></div>");
      }
    }
    this.html_body = stringBuilder.ToString();
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("layouts.aspx");

  protected void Button2_Click(object sender, EventArgs e)
  {
    hotdesk_seat hotdeskSeat = new hotdesk_seat();
    hotdeskSeat.seat_id = Convert.ToInt64(this.hdn_seat_id.Value);
    hotdeskSeat.account_id = this.current_user.account_id;
    hotdeskSeat.modified_by = this.current_user.user_id;
    this.hapi.delete_seat(hotdeskSeat);
    this.Response.Redirect("layout_setup.aspx?id=" + (object) this.layout_id);
  }
}
