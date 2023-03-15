// Decompiled with JetBrains decompiler
// Type: bookings_list
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

public class bookings_list : fbs_base_page, IRequiresSessionState
{
  public string htmltable;
  public DataSet setting_data;
  public string Allowcheckbox = "";
  public static Guid accId;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected DropDownList ddlCategory;
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected DropDownList ddlBuilding;
  protected DropDownList ddlLevel;
  protected DropDownList ddlType;
  protected Button btn_submit;
  protected TextBox txtreassign;
  protected AutoCompleteExtender AutoCompleteExtender1;
  protected HiddenField hf_reassign_user_id;
  protected Button btnReAssign;
  protected HtmlTable tbl_reassign;
  protected HtmlGenericControl div_tbl_reassign;
  protected Button btnexport;
  protected HiddenField totlarecords;
  protected HiddenField hdnSelectedRowCount;
  protected HiddenField hdnBookingIDs;
  protected HiddenField hdnsearchvalue;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      bookings_list.accId = this.current_user.account_id;
      this.btnexport.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      if (this.IsPostBack)
        return;
      this.txtFromDate.Text = this.current_timestamp.AddDays(-3.0).ToString(api_constants.display_datetime_format_short);
      this.txtToDate.Text = this.current_timestamp.AddDays(3.0).ToString(api_constants.display_datetime_format_short);
      if (this.Request.QueryString["id"] != null)
      {
        this.tbl_reassign.Visible = false;
        this.Allowcheckbox = "F";
      }
      else
      {
        this.Allowcheckbox = "T";
        this.tbl_reassign.Visible = true;
      }
      this.setting_data = this.settings.get_settings(this.current_user.account_id);
      this.populate_building(this.setting_data);
      this.populate_level(this.setting_data);
      this.populate_category(this.setting_data);
      this.populate_type(this.setting_data);
      this.htmltable = this.populate_data("html", Convert.ToDateTime(this.txtFromDate.Text), Convert.ToDateTime(this.txtToDate.Text));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private void populate_building(DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='building'"))
        this.ddlBuilding.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlBuilding.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_level(DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='level'"))
        this.ddlLevel.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlLevel.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_category(DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='category'"))
        this.ddlCategory.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlCategory.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_type(DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='asset_type'"))
        this.ddlType.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlType.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private string populate_data(string type, DateTime from, DateTime to)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='booking_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:3%;' class='hidden-480'><input type='checkbox'  id='cbSelectAll' runat='server'  onclick='SelectAll(this.id)'/></th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Room</th>");
      stringBuilder.Append("<th style='width:7%;' class='hidden-480'>Building</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Purpose</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>From</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>To</th>");
      stringBuilder.Append("<th style='width:10%;' class='hidden-480'>Requested By</th>");
      stringBuilder.Append("<th style='width:10%;' class='hidden-480'>Booked For</th>");
      stringBuilder.Append("<th style='width:10%;' class='hidden-480'>Status</th>");
      if (type != "excel")
        stringBuilder.Append("<th style='width:2%;' class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append(this.get_bookings(type, from, to));
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
    long bk_for = 0;
    if (!this.gp.isAdminType)
      bk_for = this.current_user.user_id;
    DataSet bookings2 = this.bookings.get_bookings2(this.current_user.account_id, from, to, bk_for);
    string filterExpression = " 1=1 ";
    if (this.ddlCategory.SelectedItem.Value != "0" && this.ddlCategory.SelectedItem.Value != "")
      filterExpression = filterExpression + " and category_id='" + this.ddlCategory.SelectedItem.Value + "'";
    if (this.ddlBuilding.SelectedItem.Value != "")
      filterExpression = filterExpression + " and building_id='" + this.ddlBuilding.SelectedItem.Value + "'";
    if (this.ddlLevel.SelectedItem.Value != "0" && this.ddlLevel.SelectedItem.Value != "")
      filterExpression = filterExpression + " and level_id='" + this.ddlLevel.SelectedItem.Value + "'";
    if (this.ddlType.SelectedItem.Value != "0" && this.ddlType.SelectedItem.Value != "")
      filterExpression = filterExpression + " and asset_type='" + this.ddlType.SelectedItem.Value + "'";
    DataRow[] dataRowArray = assets.Tables[0].Select(filterExpression);
    Dictionary<string, DataRow> dictionary6 = new Dictionary<string, DataRow>();
    foreach (DataRow dataRow in dataRowArray)
      dictionary6.Add(dataRow["asset_id"].ToString(), dataRow);
    foreach (DataRow row in (InternalDataCollectionBase) bookings2.Tables[0].Rows)
    {
      if (dictionary6.ContainsKey(row["asset_id"].ToString()) && row["status"].ToString() == "1")
      {
        bool flag = true;
        if (!this.gp.isAdminType)
          flag = Convert.ToInt64(row["created_by"]) == this.current_user.user_id || Convert.ToInt64(row["booked_for"]) == this.current_user.user_id || row["owner_gp_id"].ToString() == this.u_group.group_id.ToString();
        if (flag)
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
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table>");
      stringBuilder.Append("<tr><td colspan='9'><h1>Bookings</h1></td></tr>");
      stringBuilder.Append("<tr><td colspan='9'><h2>Filter Criteria:</h2></td></tr>");
      stringBuilder.Append("<tr><td>Room Type/Equipment: </td><td colspan='8' style='text-align:left;'>" + this.ddlCategory.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>From: </td><td colspan='8' style='text-align:left;'>" + this.txtFromDate.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>To: </td><td colspan='8' style='text-align:left;'>" + this.txtToDate.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Building: </td><td colspan='8' style='text-align:left;'>" + this.ddlBuilding.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Level: </td><td colspan='8' style='text-align:left;'>" + this.ddlLevel.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Type: </td><td colspan='8' style='text-align:left;'>" + this.ddlType.SelectedItem.Text + "</br></td></tr>");
      stringBuilder.Append("<tr><td>Status: </td><td colspan='8' style='text-align:left;'>Booked</br></td></tr>");
      stringBuilder.Append("</table>");
      stringBuilder.Append(this.populate_data("excel", Convert.ToDateTime(this.txtFromDate.Text), Convert.ToDateTime(this.txtToDate.Text)));
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

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    DateTime dateTime1 = Convert.ToDateTime(this.Request.Form["ctl00$ContentPlaceHolder1$txtFromDate"]);
    DateTime dateTime2 = Convert.ToDateTime(this.Request.Form["ctl00$ContentPlaceHolder1$txtToDate"]);
    this.txtFromDate.Text = dateTime1.ToString(api_constants.display_datetime_format_short);
    this.txtToDate.Text = dateTime2.ToString(api_constants.display_datetime_format_short);
    this.htmltable = this.populate_data("html", dateTime1, dateTime2);
  }

  protected void btnReAssign_Click(object sender, EventArgs e)
  {
    try
    {
      List<asset_booking> bookings = new List<asset_booking>();
      string str1 = this.hdnBookingIDs.Value;
      string str2 = this.hf_reassign_user_id.Value;
      if (str1.Length > 0)
      {
        string[] strArray = str1.TrimEnd(',').Split(',');
        user user = this.users.get_user(Convert.ToInt64(str2), this.current_user.account_id);
        for (int index = 0; index <= strArray.Length - 1; ++index)
        {
          asset_booking booking = this.bookings.get_booking(Convert.ToInt64(strArray[index]), this.current_user.account_id);
          if (booking.book_from >= this.current_timestamp && booking.status == (short) 1)
          {
            if (booking.booked_for == booking.created_by)
            {
              booking.booked_for = user.user_id;
              booking.created_by = user.user_id;
              booking.modified_by = this.current_user.user_id;
              booking.email = user.email;
            }
            else
            {
              booking.created_by = user.user_id;
              booking.modified_by = this.current_user.user_id;
            }
            booking.purpose += " (Reassigned)";
            this.bookings.update_booking_reassign(booking, strArray[index]);
            bookings.Add(booking);
          }
        }
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          this.bookingsbl.email_reassign(bookings);
      }
      List<asset_booking> assetBookingList = new List<asset_booking>();
      foreach (asset_booking assetBooking in bookings)
      {
        assetBooking.status = (short) 0;
        assetBooking.invites = new Dictionary<long, asset_booking_invite>();
        assetBookingList.Add(assetBooking);
      }
      this.Response.Redirect("bookings_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
