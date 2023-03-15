// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_requests
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_additional_resources_requests : fbs_base_page, IRequiresSessionState
{
  protected DropDownList ddlType;
  protected DropDownList ddl_resources;
  protected DropDownList ddl_user;
  protected TextBox txt_startDate;
  protected TextBox txt_endDate;
  protected TextBox txt_keyword;
  protected Button btn_submit;
  protected Button btnExportExcel;
  protected HiddenField hdn_search;
  protected HiddenField hdn_orderby;
  protected HiddenField hdn_orderdir;
  public string accountid = "";
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.accountid = this.current_user.account_id.ToString();
    try
    {
      if (this.Request.QueryString["type"] == "delete")
        this.resapi.delete_resource_booking_items(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id, this.current_user.user_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Delete Resource Booking Item Error ->", ex);
    }
    if (this.IsPostBack)
      return;
    this.ViewState.Add("allowed_items", (object) this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, this.str_resource_module));
    this.txt_startDate.Text = DateTime.Today.ToString(api_constants.display_datetime_format_short);
    this.txt_endDate.Text = DateTime.Today.AddDays(30.0).ToString(api_constants.display_datetime_format_short);
    this.populate_dropdown();
    this.set_resource_dropdown(0L);
    this.populate_requests(Convert.ToDateTime(this.txt_startDate.Text), Convert.ToDateTime(this.txt_endDate.Text).AddDays(1.0).AddSeconds(-1.0), Convert.ToInt64(this.ddlType.SelectedItem.Value), Convert.ToInt64(this.ddl_resources.SelectedItem.Value));
  }

  private void populate_requests(
    DateTime from,
    DateTime to,
    long resource_type_id,
    long resource_id)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    DataSet dataSet = (DataSet) this.ViewState["allowed_items"];
    if (dataSet.Tables[0].Rows.Count <= 0)
      return;
    StringBuilder stringBuilder = new StringBuilder();
    DataSet bookingsByDateRange = this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, from, to, resource_type_id, resource_id, this.str_resource_module);
    List<long> longList = new List<long>();
    bool flag1 = false;
    foreach (DataRow row in (InternalDataCollectionBase) bookingsByDateRange.Tables[0].Rows)
    {
      flag1 = false;
      bool flag2 = this.ddl_user.SelectedItem.Value == "0" || row["booked_for_id"].ToString() == this.ddl_user.SelectedItem.Value || row["requested_by"].ToString() == this.ddl_user.SelectedItem.Value;
      if (this.txt_keyword.Text != "")
        flag2 = row["purpose"].ToString().ToUpper().Contains(this.txt_keyword.Text.ToUpper()) || row["remarks"].ToString().ToUpper().Contains(this.txt_keyword.Text.ToUpper()) || row["other_remarks"].ToString().ToUpper().Contains(this.txt_keyword.Text.ToUpper());
      if (flag2)
      {
        long int64 = Convert.ToInt64(row["resource_booking_id"]);
        if (!longList.Contains(int64))
          longList.Add(int64);
      }
    }
    foreach (long num in longList)
    {
      DataRow[] rows = bookingsByDateRange.Tables[0].Select("resource_Booking_id='" + (object) num + "'");
      if (rows.Length > 0)
      {
        DataRow dataRow = rows[0];
        if (dataSet.Tables[0].Select("item_id='" + dataRow[nameof (resource_id)] + "'").Length > 0)
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(dataRow["from_date"]).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(dataRow["to_date"]).ToString(api_constants.display_datetime_format) + "</td>");
          stringBuilder.Append("<td>" + dataRow["venue"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["booked_for"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["requested_by_name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["purpose"].ToString() + "</td>");
          stringBuilder.Append("<td>" + this.get_items(rows) + "</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append("<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'>");
          stringBuilder.Append("<li><a href='javascript:eventClick(" + dataRow["resource_booking_id"].ToString() + ")'><i class='icon-table'></i> View</a></li>");
          if (Convert.ToDateTime(dataRow["from_date"]) > this.current_timestamp)
          {
            stringBuilder.Append("<li><a href='request_resources.aspx?resource_booking_id=" + dataRow["resource_booking_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
            stringBuilder.Append("<li><a href='javascript:delete_resource_booking(" + dataRow["resource_booking_item_id"].ToString() + ")'><i class='icon-trash'></i> Remove</a></li>");
          }
          stringBuilder.Append("</ul></div></div>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
        }
      }
    }
    this.html_table = stringBuilder.ToString();
  }

  private string get_items(DataRow[] rows)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow row in rows)
      stringBuilder.Append(row["name"].ToString() + " (" + row["requested_qty"].ToString() + "),");
    return stringBuilder.ToString().TrimEnd(',');
  }

  private void populate_dropdown()
  {
    DataSet dataSet1 = new DataSet();
    DataSet dataSet2 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_type");
    if (dataSet2 == null)
    {
      dataSet2 = this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, "resource_type", this.str_resource_module);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_type", (object) dataSet2);
    }
    this.ddlType.Items.Clear();
    this.ddlType.Items.Add(new ListItem("All Resource Types", "0"));
    foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
      this.ddlType.Items.Add(new ListItem(row["value"].ToString(), row["setting_id"].ToString()));
    DataSet usersNamesList = this.users.get_users_names_list(this.current_user.account_id);
    this.ddl_user.Items.Add(new ListItem(api_constants.all_users_text, "0"));
    foreach (DataRow row in (InternalDataCollectionBase) usersNamesList.Tables[0].Rows)
      this.ddl_user.Items.Add(new ListItem(row["full_name"].ToString(), row["user_id"].ToString()));
  }

  protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.set_resource_dropdown(Convert.ToInt64(this.ddlType.SelectedItem.Value));
    this.populate_requests(Convert.ToDateTime(this.txt_startDate.Text), Convert.ToDateTime(this.txt_endDate.Text).AddDays(1.0).AddSeconds(-1.0), Convert.ToInt64(this.ddlType.SelectedItem.Value), Convert.ToInt64(this.ddl_resources.SelectedItem.Value));
  }

  private void set_resource_dropdown(long resource_type_id)
  {
    this.ddl_resources.Items.Clear();
    this.ddl_resources.Items.Add(new ListItem("All Resources", "0"));
    if (resource_type_id == 0L)
      return;
    DataSet dataSet = (DataSet) this.ViewState["allowed_items"];
    foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resource_items_by_item_type_id(resource_type_id, this.current_user.account_id, this.str_resource_module).Tables[0].Rows)
    {
      if (dataSet.Tables[0].Select("item_id='" + row["item_id"] + "'").Length > 0)
        this.ddl_resources.Items.Add(new ListItem(row["name"].ToString(), row["item_id"].ToString()));
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e) => this.populate_requests(Convert.ToDateTime(this.txt_startDate.Text), Convert.ToDateTime(this.txt_endDate.Text).AddDays(1.0).AddSeconds(-1.0), Convert.ToInt64(this.ddlType.SelectedItem.Value), Convert.ToInt64(this.ddl_resources.SelectedItem.Value));

  protected void txt_startDate_TextChanged(object sender, EventArgs e) => this.txt_endDate.Text = Convert.ToDateTime(this.txt_startDate.Text).AddDays(30.0).ToString(api_constants.display_datetime_format_short);

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      if (((DataSet) this.ViewState["allowed_items"]).Tables[0].Rows.Count <= 0)
        return;
      DataSet bookingsByDateRange = this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, Convert.ToDateTime(this.txt_startDate.Text), Convert.ToDateTime(this.txt_endDate.Text), Convert.ToInt64(this.ddlType.SelectedItem.Value), Convert.ToInt64(this.ddl_resources.SelectedItem.Value), this.str_resource_module);
      if (bookingsByDateRange == null)
        return;
      List<long> longList = new List<long>();
      bool flag1 = false;
      foreach (DataRow row in (InternalDataCollectionBase) bookingsByDateRange.Tables[0].Rows)
      {
        flag1 = false;
        bool flag2 = this.ddl_user.SelectedItem.Value == "0" || row["booked_for_id"].ToString() == this.ddl_user.SelectedItem.Value || row["requested_by"].ToString() == this.ddl_user.SelectedItem.Value;
        if (this.txt_keyword.Text != "")
          flag2 = row["purpose"].ToString().ToUpper().Contains(this.txt_keyword.Text.ToUpper()) || row["remarks"].ToString().ToUpper().Contains(this.txt_keyword.Text.ToUpper()) || row["other_remarks"].ToString().ToUpper().Contains(this.txt_keyword.Text.ToUpper());
        if (flag2)
        {
          long int64 = Convert.ToInt64(row["resource_booking_id"]);
          if (!longList.Contains(int64))
            longList.Add(int64);
        }
      }
      foreach (DataRow row in (InternalDataCollectionBase) bookingsByDateRange.Tables[0].Rows)
      {
        row["from_date"] = (object) row["from_date"].ToString();
        row["to_date"] = (object) row["to_date"].ToString();
        row["venue"] = (object) row["venue"].ToString();
        row["booked_for"] = (object) row["booked_for"].ToString();
        row["requested_by_name"] = (object) row["requested_by_name"].ToString();
        row["purpose"] = (object) row["purpose"].ToString();
        row["name"] = (object) row["name"].ToString();
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("name", "Request Details");
      dictionary.Add("from_date", "From");
      dictionary.Add("to_date", "To");
      dictionary.Add("purpose", "Purpose");
      dictionary.Add("venue", "Location");
      dictionary.Add("booked_for", "Booked For");
      dictionary.Add("requested_by_name", "Requested By");
      excel excel = new excel();
      excel.file_name = "+ current_user.full_name + " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = bookingsByDateRange;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      excel.header = "Resource Bookings";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Resource Bookings_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.exapi.get_excel(excel));
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
