// Decompiled with JetBrains decompiler
// Type: administration_hotdesk_detailed_report
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

public class administration_hotdesk_detailed_report : fbs_base_page, IRequiresSessionState
{
  protected TextBox txt_startDate;
  protected DropDownList ddl_StartTime;
  protected TextBox txt_endDate;
  protected DropDownList ddl_EndTime;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected Button btn_check_availability;
  protected Label lbl_error;
  protected HiddenField searchcon;
  protected HiddenField hidorderby;
  protected HiddenField hidorderdir;
  private hotdesk_api hapi = new hotdesk_api();
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (this.IsPostBack)
      return;
    this.txt_startDate.Text = DateTime.Today.ToString("dd-MMM-yyyy");
    this.txt_endDate.Text = DateTime.Today.AddDays(1.0).ToString("dd-MMM-yyyy");
    this.populate_buildings();
    this.populate_levels();
    this.utilities.Populate_Time(this.ddl_StartTime, this.current_timestamp);
    this.utilities.Populate_Time(this.ddl_EndTime, this.current_timestamp);
    this.populate_table(this.hapi.get_bookings(this.current_user.account_id, Convert.ToDateTime(this.txt_startDate.Text + " " + this.ddl_StartTime.SelectedItem.Value), Convert.ToDateTime(this.txt_endDate.Text + " " + this.ddl_EndTime.SelectedItem.Value), 0L, Convert.ToInt64(this.ddl_building.SelectedItem.Value), Convert.ToInt64(this.ddl_level.SelectedItem.Value)));
  }

  private void populate_buildings()
  {
    this.ddl_building.Items.Add(new ListItem("All", "0"));
    foreach (DataRow row in (InternalDataCollectionBase) this.settings.get_settings(this.current_user.account_id, "building").Tables[0].Rows)
      this.ddl_building.Items.Add(new ListItem(row["value"].ToString(), row["setting_id"].ToString()));
  }

  private void populate_levels()
  {
    this.ddl_level.Items.Add(new ListItem("All", "0"));
    foreach (DataRow row in (InternalDataCollectionBase) this.settings.get_settings(this.current_user.account_id, "level").Tables[0].Rows)
      this.ddl_level.Items.Add(new ListItem(row["value"].ToString(), row["setting_id"].ToString()));
  }

  protected void btn_check_availability_Click(object sender, EventArgs e) => this.populate_table(this.hapi.get_bookings(this.current_user.account_id, Convert.ToDateTime(this.txt_startDate.Text + " " + this.ddl_StartTime.SelectedItem.Value), Convert.ToDateTime(this.txt_endDate.Text + " " + this.ddl_EndTime.SelectedItem.Value), 0L, Convert.ToInt64(this.ddl_building.SelectedItem.Value), Convert.ToInt64(this.ddl_level.SelectedItem.Value)));

  private void populate_table(DataSet data)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) data.Tables[0].Rows)
    {
      double totalMinutes = (Convert.ToDateTime(row["to_date"]) - Convert.ToDateTime(row["from_date"])).TotalMinutes;
      int num1 = 0;
      if (totalMinutes >= 60.0)
        num1 = (int) totalMinutes / 60;
      double num2 = totalMinutes - (double) (num1 * 60);
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["building_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["level_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["layout_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + Convert.ToDateTime(row["from_date"]).ToString("dd-MMM-yyyy hh:mm tt") + "</td>");
      stringBuilder.Append("<td>" + Convert.ToDateTime(row["to_date"]).ToString("dd-MMM-yyyy hh:mm tt") + "</td>");
      stringBuilder.Append("<td>" + (object) num1 + " Hrs. " + (object) num2 + " Mins.</td>");
      stringBuilder.Append("<td>" + row["booked_for_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["requested_by_name"].ToString() + "</td>");
      stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
      stringBuilder.Append("<ul class='ddm p-r'>");
      stringBuilder.AppendFormat("<li><a href='javascript:view_modal(" + row["seat_id"].ToString() + "," + row["hotdesk_booking_id"].ToString() + ");'></i>View</a></li>", (object) row["hotdesk_booking_id"].ToString());
      stringBuilder.Append("</ul>");
      stringBuilder.Append("</div></div></td>");
      stringBuilder.Append("</tr>");
    }
    this.html_table = stringBuilder.ToString();
  }
}
