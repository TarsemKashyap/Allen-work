// Decompiled with JetBrains decompiler
// Type: administration_reassign_unassign
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_reassign_unassign : fbs_base_page, IRequiresSessionState
{
  public string htmltable;
  public string html_user;
  public DataSet setting_data;
  public string Allowcheckbox = "";
  public static Guid accId;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected TextBox txtreassign;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hf_reassign_user_id;
  protected Button btnReAssign;
  protected HtmlTable tbl_reassign;
  protected HtmlGenericControl div_tbl_reassign;
  protected Button btnexport;
  protected HiddenField hdn_user_id;
  protected HiddenField hdnSelectedRowCount;
  protected HiddenField hdnBookingIDs;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.btnexport.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      if (this.IsPostBack)
        return;
      long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
      this.hdn_user_id.Value = int64.ToString();
      user user = this.users.get_user(int64, this.current_user.account_id);
      this.html_user = "Reassign booking of - " + user.full_name + " (" + user.email + ")";
      this.setting_data = this.settings.get_settings(this.current_user.account_id);
      this.htmltable = this.populate_data(int64);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private string populate_data(long user_id)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='booking_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:3%;' class='hidden-480'><input type='checkbox'  id='cbSelectAll' runat='server'  onclick='SelectAll(this.id)'/></th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Room</th>");
      stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Purpose</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>From</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>To</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Remarks</th>");
      stringBuilder.Append("<th style='width:10%;' class='hidden-480'>Contact</th>");
      stringBuilder.Append("<th style='width:10%;' class='hidden-480'>Email</th>");
      stringBuilder.Append("<th style='width:10%;' class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      DataSet futureBookingsByUser = this.reportings.get_future_bookings_by_user(this.current_user.account_id, user_id, DateTime.UtcNow.AddHours(this.current_account.timezone));
      if (futureBookingsByUser != null && futureBookingsByUser.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) futureBookingsByUser.Tables[0].Rows)
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td><input type='checkbox' id='chkSelect'  runat='server'  name='chk_book' value='" + row["booking_id"].ToString() + "'  /></td>");
          stringBuilder.Append("<td>" + row["asset_name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["purpose"].ToString() + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_from"]).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_to"]).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + row["remarks"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["contact"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["email"].ToString() + "</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append("<div class='actions' id='action_" + row["booking_id"].ToString() + "'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
          stringBuilder.Append("<ul class='ddm p-r'>");
          stringBuilder.Append("<li><a href='javascript:vb(" + row["booking_id"].ToString() + ")'><i class='icon-table'></i> View</a></li>");
          stringBuilder.Append("</ul>");
          stringBuilder.Append("</div></div>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
        }
      }
      else
        stringBuilder.Append("<tr><td colspan=''>No data found for this user.</td></tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.htmltable = stringBuilder.ToString();
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
    DataSet settings = this.settings.get_settings(this.current_user.account_id);
    DataSet usersNamesList = this.users.get_users_names_list(this.current_user.account_id);
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
    dictionary5.Add("0", "System");
    foreach (DataRow row in (InternalDataCollectionBase) usersNamesList.Tables[0].Rows)
      dictionary5.Add(row["user_id"].ToString(), row["full_name"].ToString());
    foreach (DataRow row in (InternalDataCollectionBase) settings.Tables[0].Rows)
    {
      if (row["parameter"].ToString() == "building")
        dictionary1.Add(row["setting_id"].ToString(), row["value"].ToString());
      if (row["parameter"].ToString() == "level")
        dictionary2.Add(row["setting_id"].ToString(), row["value"].ToString());
      if (row["parameter"].ToString() == "category")
        dictionary3.Add(row["setting_id"].ToString(), row["value"].ToString());
      if (row["parameter"].ToString() == "asset_type")
        dictionary4.Add(row["setting_id"].ToString(), row["value"].ToString());
    }
    DataSet assets = this.assets.get_assets(this.current_user.account_id);
    long userId = this.current_user.user_id;
    DataSet bookings2 = this.bookings.get_bookings2(this.current_user.account_id, from, to, userId);
    string filterExpression = " 1=1 ";
    DataRow[] dataRowArray = assets.Tables[0].Select(filterExpression);
    Dictionary<string, DataRow> dictionary6 = new Dictionary<string, DataRow>();
    foreach (DataRow dataRow in dataRowArray)
      dictionary6.Add(dataRow["asset_id"].ToString(), dataRow);
    foreach (DataRow row in (InternalDataCollectionBase) bookings2.Tables[0].Rows)
    {
      if (dictionary6.ContainsKey(row["asset_id"].ToString()) && row["status"].ToString() == "1" && (Convert.ToInt64(row["created_by"]) == this.current_user.user_id || Convert.ToInt64(row["booked_for"]) == this.current_user.user_id))
      {
        DataRow dataRow = dictionary6[row["asset_id"].ToString()];
        stringBuilder.Append("<tr>");
        if (Convert.ToDateTime(row["book_from"]) > this.current_timestamp.AddHours(this.current_account.timezone))
          stringBuilder.Append("<td><input type='checkbox' id='chkSelect'  runat='server'  name='chk_book' value='" + row["booking_id"].ToString() + "'  /></td>");
        else
          stringBuilder.Append("<td></td>");
        stringBuilder.Append("<td>" + dataRow["name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + dictionary1[dataRow["building_id"].ToString()] + "</td>");
        stringBuilder.Append("<td>" + row["purpose"].ToString() + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_from"]).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(row["book_to"]).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>" + dictionary5[row["created_by"].ToString()] + "</td>");
        stringBuilder.Append("<td>" + dictionary5[row["booked_for"].ToString()] + "</td>");
        stringBuilder.Append("<td>" + this.get_status(row["status"].ToString()) + "</td>");
        stringBuilder.Append("<td>");
        if (row["status"].ToString() == "1" && type == "html")
        {
          stringBuilder.Append("<div class='actions' id='action_" + row["booking_id"].ToString() + "'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
          stringBuilder.Append("<ul class='ddm p-r'>");
          stringBuilder.Append("<li><a href='javascript:vb(" + row["booking_id"].ToString() + ")'><i class='icon-table'></i> View</a></li>");
          stringBuilder.Append("</ul>");
          stringBuilder.Append("</div></div>");
        }
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
      }
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
        return "<Span class='label label-NoShow'>No Show</Span>";
      case "4":
        return "<Span class='label label-Pending'>Pending</Span>";
      case "5":
        return "<Span class='label label-withdrawan'>Withdrwal</Span>";
      case "6":
        return "<Span class='label label-rejected'>Rejected</Span>";
      case "7":
        return "<Span class='label label-rejected'>Auto Rejected</Span>";
      default:
        return status;
    }
  }

  protected void btnexport_Click(object sender, EventArgs e)
  {
    try
    {
      user user = this.users.get_user(Convert.ToInt64(this.hdn_user_id.Value), this.current_user.account_id);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table>");
      stringBuilder.Append("<tr><td colspan='9'><h1>Reassign booking of - " + user.full_name + " (" + user.email + ")</h1></td></tr>");
      stringBuilder.Append("</table>");
      stringBuilder.Append(this.populate_data(user.user_id));
      stringBuilder.Append("<table>");
      stringBuilder.Append("<tr><td>Generated By: </td><td colspan='8' style='text-align:left;'>" + this.current_user.full_name + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Generated On: </td><td colspan='8' style='text-align:left;'>" + this.current_timestamp.ToString(api_constants.display_datetime_format) + "</br></td></tr>");
      stringBuilder.Append("</table>");
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=BookingList_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = "application/vnd.xls";
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btnReAssign_Click(object sender, EventArgs e)
  {
    try
    {
      List<asset_booking> bookings = new List<asset_booking>();
      string str = this.hdnBookingIDs.Value;
      long int64 = Convert.ToInt64(this.hf_reassign_user_id.Value);
      user user = this.users.get_user(int64, this.current_user.account_id);
      if (str.Length > 0)
      {
        string[] strArray = str.TrimEnd(',').Split(',');
        for (int index = 0; index <= strArray.Length - 1; ++index)
        {
          asset_booking booking = this.bookings.get_booking(Convert.ToInt64(strArray[index]), this.current_user.account_id);
          if (booking.book_from >= this.current_timestamp && booking.status == (short) 1)
          {
            if (booking.booked_for == booking.created_by)
              booking.booked_for = int64;
            booking.created_by = int64;
            booking.email = user.email;
            booking.modified_by = this.current_user.user_id;
            this.bookings.update_booking_reassign(booking, strArray[index]);
            bookings.Add(booking);
          }
        }
        this.bookingsbl.email_reassign(bookings);
      }
      List<asset_booking> assetBookingList = new List<asset_booking>();
      foreach (asset_booking assetBooking in bookings)
      {
        assetBooking.status = (short) 0;
        assetBooking.invites = new Dictionary<long, asset_booking_invite>();
        assetBookingList.Add(assetBooking);
      }
      this.Response.Redirect("reassign_unassigned.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
