// Decompiled with JetBrains decompiler
// Type: additional_resources_resource_bookings_calendar
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

public class additional_resources_resource_bookings_calendar : fbs_base_page, IRequiresSessionState
{
  public string accountid = "";
  public string html_table;
  public string current_date;
  protected DropDownList ddlType;
  protected DropDownList ddl_resources;
  protected DropDownList ddl_user;
  protected TextBox txt_startDate;
  protected TextBox txt_endDate;
  protected TextBox txt_keyword;
  protected Button btn_submit;
  protected HiddenField hdn_search;
  protected HiddenField hdn_orderby;
  protected HiddenField hdn_orderdir;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
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
    this.current_date = this.current_timestamp.ToString("yyyy-MM-dd");
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
    this.current_date = from.ToString("yyyy-MM-dd");
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
    int num1 = 1;
    foreach (long num2 in longList)
    {
      DataRow[] dataRowArray = bookingsByDateRange.Tables[0].Select("resource_Booking_id='" + (object) num2 + "'");
      if (dataRowArray.Length > 0)
      {
        DataRow dataRow = dataRowArray[0];
        string str = !(Convert.ToDateTime(dataRow["to_date"]) < this.current_timestamp) ? "#aaaaaa" : "#333333";
        stringBuilder.Append("{url: 'javascript:eventClick(" + dataRow["resource_booking_id"].ToString() + ");', title: '<b>" + dataRow["venue"].ToString() + "</b><br/>" + dataRow["purpose"] + "', start: '" + Convert.ToDateTime(dataRow["from_date"]).ToString("yyyy-MM-ddTHH:MM") + "',end: '" + Convert.ToDateTime(dataRow["to_date"]).ToString("yyyy-MM-ddTHH:MM") + "', backgroundColor:'" + str + "'}");
        if (num1 < longList.Count)
          stringBuilder.Append(",");
        ++num1;
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
    this.populate_requests(Convert.ToDateTime(this.txt_startDate.Text), Convert.ToDateTime(this.txt_endDate.Text), Convert.ToInt64(this.ddlType.SelectedItem.Value), Convert.ToInt64(this.ddl_resources.SelectedItem.Value));
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

  protected void btn_submit_Click(object sender, EventArgs e) => this.populate_requests(Convert.ToDateTime(this.txt_startDate.Text), Convert.ToDateTime(this.txt_endDate.Text), Convert.ToInt64(this.ddlType.SelectedItem.Value), Convert.ToInt64(this.ddl_resources.SelectedItem.Value));

  protected void txt_startDate_TextChanged(object sender, EventArgs e) => this.txt_endDate.Text = Convert.ToDateTime(this.txt_startDate.Text).AddDays(1.0).ToString();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
