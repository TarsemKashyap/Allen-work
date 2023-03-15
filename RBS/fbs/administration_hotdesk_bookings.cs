// Decompiled with JetBrains decompiler
// Type: administration_hotdesk_bookings
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_hotdesk_bookings : fbs_base_page, IRequiresSessionState
{
  private hotdesk_api hapi = new hotdesk_api();
  public string html_table = "";
  public new Guid account_id;
  public DataSet setting_data;
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected DropDownList ddl_layout;
  protected DropDownList ddlStatus;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected TextBox txtRequestedBy;
  protected Panel myPanel;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hfUserId;
  protected Button btn_submit;
  protected Button btnexport;
  protected HiddenField totlarecords;
  protected HiddenField hdnSelectedRowCount;
  protected HiddenField hdnBookingIDs;
  protected HiddenField hdnsearchvalue;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.account_id = this.current_user.account_id;
    if (!this.IsPostBack)
    {
      this.txtFromDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
      this.txtToDate.Text = this.current_timestamp.AddDays(7.0).ToString(api_constants.display_datetime_format_short);
      this.setting_data = this.settings.get_settings(this.current_user.account_id);
      this.populate_layouts();
      this.html_table = this.populate_data("html", Convert.ToDateTime(this.txtFromDate.Text), Convert.ToDateTime(this.txtToDate.Text));
    }
    else
      this.perform_action(this.Request.Form[0], this.Request.Form[1]);
  }

  private void perform_action(string action, string id)
  {
    switch (action)
    {
      case "can":
        this.do_cancel(Convert.ToInt64(id));
        break;
      case "rel":
        this.do_release(Convert.ToInt64(id));
        break;
    }
  }

  private void do_cancel(long id)
  {
    hotdesk_booking hotdeskBooking = new hotdesk_booking();
    hotdeskBooking.hotdesk_booking_id = id;
    hotdeskBooking.modified_by = this.current_user.user_id;
    hotdeskBooking.account_id = this.current_user.account_id;
    this.hapi.cancel_booking(hotdeskBooking);
    this.Response.Redirect("bookings.aspx");
  }

  private void do_release(long id)
  {
    hotdesk_booking hotdeskBooking = new hotdesk_booking();
    hotdeskBooking.hotdesk_booking_id = id;
    hotdeskBooking.modified_by = this.current_user.user_id;
    hotdeskBooking.account_id = this.current_user.account_id;
    hotdeskBooking.to_date = this.current_timestamp;
    this.hapi.release_booking(hotdeskBooking);
    this.Response.Redirect("bookings.aspx");
  }

  private string populate_data(string type, DateTime from, DateTime to)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append(this.get_bookings(type, from, to));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
      stringBuilder.Append("No data found. Error: " + ex.ToString());
    }
    return stringBuilder.ToString();
  }

  private string get_bookings(string type, DateTime from, DateTime to)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in (InternalDataCollectionBase) this.hapi.get_bookings(this.current_user.account_id, from, to.AddDays(1.0).AddSeconds(-1.0), Convert.ToInt64(this.ddl_layout.SelectedItem.Value), 0L, Convert.ToInt64(this.ddlStatus.SelectedItem.Value), 0L).Tables[0].Rows)
    {
      DateTime dateTime1 = Convert.ToDateTime(row["from_date"]);
      DateTime dateTime2 = Convert.ToDateTime(row["to_date"]);
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + row["building_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["level_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["layout_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + Convert.ToDateTime(row["from_date"]).ToString(api_constants.display_datetime_format) + "</td>");
      stringBuilder.Append("<td>" + Convert.ToDateTime(row["to_date"]).ToString(api_constants.display_datetime_format) + "</td>");
      stringBuilder.Append("<td>" + row["booked_for_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["requested_by_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + this.get_status(row["status"].ToString()) + "</td>");
      stringBuilder.Append("<td>");
      stringBuilder.Append("<div class='actions' id='action_" + row["hotdesk_booking_id"].ToString() + "'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
      stringBuilder.Append("<ul class='ddm p-r'>");
      stringBuilder.Append("<li><a href='javascript:vb(" + row["hotdesk_booking_id"].ToString() + ")'><i class='icon-table'></i> View Booking</a></li>");
      if (row["status"].ToString() != "0")
      {
        if (dateTime1 > this.current_timestamp)
          stringBuilder.Append("<li><a href='return javascript:can(" + row["hotdesk_booking_id"].ToString() + ");'><i class='icon-table'></i> Cancel Booking</a></li>");
        else if (dateTime2 > this.current_timestamp)
          stringBuilder.Append("<li><a href='javascript:rel(" + row["hotdesk_booking_id"].ToString() + ");'><i class='icon-table'></i> Release Booking</a></li>");
      }
      stringBuilder.Append("</ul>");
      stringBuilder.Append("</div></div>");
      stringBuilder.Append("</td>");
      stringBuilder.Append("</tr>");
    }
    return stringBuilder.ToString();
  }

  private string get_status(string status)
  {
    switch (status)
    {
      case "0":
        return "<Span class='label label-cancelled'>Cancelled</Span>";
      case "1":
        return "<Span class='label label-Booked'>Booked</Span>";
      case "2":
        return "<Span class='label label-Blocked'>Blocked</Span>";
      case "3":
        return "<Span class='label label-NoShow'>Reserved</Span>";
      default:
        return status;
    }
  }

  protected void btnexport_Click(object sender, EventArgs e)
  {
    try
    {
      string str = this.populate_data("xls", Convert.ToDateTime(this.txtFromDate.Text), Convert.ToDateTime(this.txtToDate.Text)).Replace("<br/>", "").Replace("<hr/>", "");
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=BookingList_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = "application/vnd.xls";
      this.Response.Write(str.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    DateTime dateTime1 = Convert.ToDateTime(this.Request.Form["ctl00$ContentPlaceHolder1$txtFromDate"]);
    DateTime dateTime2 = Convert.ToDateTime(this.Request.Form["ctl00$ContentPlaceHolder1$txtToDate"]);
    this.txtFromDate.Text = dateTime1.ToString(api_constants.display_datetime_format_short);
    this.txtToDate.Text = dateTime2.ToString(api_constants.display_datetime_format_short);
    this.html_table = this.populate_data("html", dateTime1, dateTime2);
  }

  private void populate_layouts()
  {
    try
    {
      DataSet layouts = this.hapi.get_layouts(this.current_user.account_id);
      this.ddl_layout.Items.Insert(0, new ListItem("All", "0"));
      foreach (DataRow row in (InternalDataCollectionBase) layouts.Tables[0].Rows)
        this.ddl_layout.Items.Add(new ListItem(row["name"].ToString(), row["layout_id"].ToString()));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
