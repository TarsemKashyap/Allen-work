// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_request_resources
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_additional_resources_request_resources : 
  fbs_base_page,
  IRequiresSessionState
{
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected TextBox txt_purpose;
  protected HtmlGenericControl errorpurpose;
  protected TextBox txtBookedFor;
  protected Panel myPanel;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hfUserId;
  protected HtmlGenericControl errorbookedfor;
  protected TextBox txt_email;
  protected HtmlImage img_loading;
  protected HtmlGenericControl erroremail;
  protected HtmlInputText lbl_requestedBy;
  protected TextBox txt_location;
  protected HtmlGenericControl errorLocation;
  protected TextBox txtRemarks;
  protected Button btn_next_step;
  protected HtmlAnchor sample_editable_1_new;
  protected TextBox txt_startDate;
  protected DropDownList ddl_StartTime;
  protected TextBox txt_endDate;
  protected DropDownList ddl_EndTime;
  protected Button btn_add_date;
  protected HtmlGenericControl span_duplicate;
  protected HtmlTable tblDateSelectAdd;
  protected Repeater gridview_add_dates;
  protected Panel pnl_dates;
  protected Label lblError;
  protected DropDownList ddl_res_type;
  protected DropDownList ddl_res_name;
  protected Literal lit_available_qty;
  protected HiddenField hdn_available_qty;
  protected HtmlTableRow tr_avail_qty;
  protected TextBox txtQty;
  protected HtmlTableRow tr_qty;
  protected TextBox txt_requestor_remarks;
  protected HtmlTableRow tr_remarks;
  protected Image img_res;
  protected HtmlTableRow tr_img;
  protected Literal litResource;
  protected HtmlTableRow tr_template;
  protected Button btn_add_resource;
  protected Panel pnl_items;
  protected FileUpload upload_attachment;
  protected Label lblErrorUpload;
  protected HtmlGenericControl divUpload;
  protected Panel pnl_items_table;
  protected LinkButton link_attachment;
  protected LinkButton link_attachment_delete;
  protected Table tblAttachment;
  protected HtmlGenericControl divAttachments;
  protected HtmlAnchor anchor_download;
  protected HtmlTableRow download_lnk;
  protected CheckBox chk_terms;
  protected HtmlGenericControl errorTerms;
  protected Label lbl_error_message;
  protected Button btn_cancel;
  protected Button btn_submit;
  protected HiddenField hdn_asset_booking_id;
  protected HiddenField hdn_resource_booking_id;
  protected HiddenField hdn_booking_type;
  protected HiddenField hdn_number_of_dates;
  protected HiddenField hdn_number_of_items;
  private long asset_booking_id;
  private long resource_booking_id;
  private bool is_repeat;
  public string resourcesHtml;
  private DataSet resource_settings_data;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    this.resource_settings_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
    if (this.resource_settings_data == null)
    {
      this.resource_settings_data = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) this.resource_settings_data);
    }
    if (!this.IsPostBack)
    {
      this.ViewState.Add("allowed_items", (object) this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, this.str_resource_module));
      this.initialize_ui();
      this.initialize_date_table();
      this.initialize_items_table();
      this.process_asset_booking();
      this.process_resource_booking();
    }
    else
    {
      DataTable dates_table = (DataTable) this.ViewState["dates_table"];
      DataTable items_table = (DataTable) this.ViewState["items_table"];
      if (this.Request.Form[0] == "btn_remove_item")
      {
        int index = 0;
        foreach (DataRow row in (InternalDataCollectionBase) items_table.Rows)
        {
          if (row["no"].ToString() != this.Request.Form[1].ToString())
            ++index;
        }
        items_table.Rows.RemoveAt(index);
        this.ViewState["items_table"] = (object) items_table;
      }
      this.update_resource_table_data();
      if (items_table.Rows.Count <= 0)
        return;
      this.populate_resource_html(dates_table, items_table);
    }
  }

  public void bind_attachments(DataSet dsDoc)
  {
    bool flag = false;
    foreach (DataRow row in (InternalDataCollectionBase) dsDoc.Tables[0].Rows)
    {
      flag = true;
      this.link_attachment.CommandArgument = row["resource_document_id"].ToString();
      this.link_attachment.Text = row["document_name"].ToString();
      this.link_attachment_delete.CommandArgument = row["resource_document_id"].ToString();
      this.link_attachment_delete.Text = "Delete";
      this.link_attachment_delete.OnClientClick = "javascript:return delete_confirmation('" + Resources.fbs.delete_confirmation_msg + "');";
    }
    if (flag)
      return;
    this.link_attachment.Text = "";
    this.link_attachment_delete.Text = "";
  }

  private void getUploadedAttachments(long id)
  {
    if (id > 0L)
    {
      DataSet resourceBookingId = this.resapi.get_resource_document_by_resource_booking_id(id, this.current_user.account_id);
      if (this.utilities.isValidDataset(resourceBookingId))
      {
        this.divUpload.Visible = false;
        this.divAttachments.Visible = true;
        this.bind_attachments(resourceBookingId);
      }
      else
      {
        this.divUpload.Visible = true;
        this.divAttachments.Visible = false;
      }
    }
    else
    {
      this.divUpload.Visible = true;
      this.divAttachments.Visible = false;
    }
  }

  private void process_resource_booking()
  {
    DataTable dataTable = (DataTable) this.ViewState["dates_table"];
    DataTable items_table = (DataTable) this.ViewState["items_table"];
    DataSet resourceItems = this.resapi.get_resource_items(this.current_user.account_id, this.str_resource_module);
    long num1 = 0;
    if (this.asset_booking_id > 0L)
    {
      DataSet resourceBookings = this.resapi.get_resource_bookings(this.asset_booking_id, this.current_user.account_id, this.str_resource_module);
      if (resourceBookings.Tables[0].Rows.Count > 0)
        num1 = Convert.ToInt64(resourceBookings.Tables[0].Rows[0]["resource_Booking_id"]);
    }
    long resourceBookingId1 = this.resource_booking_id;
    this.hdn_resource_booking_id.Value = resourceBookingId1.ToString();
    if (resourceBookingId1 <= 0L)
      return;
    this.getUploadedAttachments(resourceBookingId1);
    DataSet resourceBookingId2 = this.resapi.get_resource_bookings_by_resource_booking_id(this.current_user.account_id, resourceBookingId1, this.str_resource_module);
    if (resourceBookingId2.Tables[0].Rows.Count > 0)
    {
      if (this.asset_booking_id == 0L)
      {
        DataRow row1 = resourceBookingId2.Tables[0].Rows[0];
        this.asset_booking_id = Convert.ToInt64(row1["asset_booking_id"]);
        if (this.asset_booking_id > 0L)
        {
          this.process_asset_booking();
          dataTable = (DataTable) this.ViewState["dates_table"];
        }
        else
        {
          DataRow row2 = dataTable.NewRow();
          row2["no"] = (object) (dataTable.Rows.Count + 1);
          row2["from_date"] = (object) Convert.ToDateTime(row1["from_date"]).ToString(api_constants.display_datetime_format);
          row2["to_date"] = (object) Convert.ToDateTime(row1["to_date"]).ToString(api_constants.display_datetime_format);
          row2["id"] = (object) "0";
          row2["status"] = row1["booking_status"];
          dataTable.Rows.Add(row2);
          dataTable.AcceptChanges();
          this.ViewState["dates_table"] = (object) dataTable;
          this.bind_dates_grid(dataTable);
          this.txt_purpose.Text = row1["purpose"].ToString();
          this.txtRemarks.Text = row1["remarks"].ToString();
          this.txtBookedFor.Text = row1["booked_for"].ToString();
          this.txt_email.Text = row1["email"].ToString();
          this.txt_location.Text = row1["venue"].ToString();
          if (row1["booked_for"].ToString() == row1["requested_by_name"].ToString())
            this.lbl_requestedBy.Value = row1["booked_for"].ToString();
          else
            this.lbl_requestedBy.Value = row1["requested_by_name"].ToString();
        }
      }
      foreach (DataRow row3 in (InternalDataCollectionBase) resourceBookingId2.Tables[0].Rows)
      {
        DataRow dataRow = resourceItems.Tables[0].Select("item_id='" + row3["resource_id"] + " '")[0];
        double bookedQuantity = this.resapi.get_booked_quantity(Convert.ToInt64(row3["resource_id"]), this.current_user.account_id, Convert.ToDateTime(row3["from_date"]), Convert.ToDateTime(row3["to_date"]), this.str_resource_module);
        double num2 = Convert.ToDouble(dataRow["quantity"]) - bookedQuantity + Convert.ToDouble(row3["accepted_qty"]);
        DataRow row4 = items_table.NewRow();
        row4["no"] = (object) (items_table.Rows.Count + 1);
        row4["item_id"] = row3["resource_id"];
        row4["item_name"] = row3["name"];
        row4["item_type"] = row3["value"];
        row4["available_qty"] = (object) num2;
        row4["requested_qty"] = row3["requested_qty"];
        row4["accepted_qty"] = row3["accepted_qty"];
        row4["old_qty"] = row3["accepted_qty"];
        row4["remarks"] = row3["requestor_remarks"];
        row4["date_no"] = (object) "1";
        row4["id"] = (object) resourceBookingId1;
        row4["status"] = row3["status"];
        row4["from_date"] = (object) Convert.ToDateTime(row3["from_date"]).ToString(api_constants.display_datetime_format);
        row4["to_date"] = (object) Convert.ToDateTime(row3["to_date"]).ToString(api_constants.display_datetime_format);
        row4["resource_booking_item_id"] = row3["resource_booking_item_id"];
        items_table.Rows.Add(row4);
        items_table.AcceptChanges();
      }
      this.ViewState["items_table"] = (object) items_table;
    }
    this.populate_resource_html(dataTable, items_table);
    this.pnl_dates.Visible = true;
    this.pnl_items.Visible = true;
    this.pnl_items_table.Visible = true;
  }

  private void process_asset_booking()
  {
    if (this.asset_booking_id <= 0L)
      return;
    this.hdn_asset_booking_id.Value = this.asset_booking_id.ToString();
    asset_booking booking = this.bookings.get_booking(this.asset_booking_id, this.current_user.account_id);
    if (booking.booking_id <= 0L)
      return;
    asset asset = this.assets.get_asset(booking.asset_id, booking.account_id);
    this.txt_purpose.Text = booking.purpose;
    this.txtRemarks.Text = booking.remarks;
    user user = this.users.get_user(booking.booked_for, booking.account_id);
    this.txtBookedFor.Text = user.full_name;
    this.txt_email.Text = user.email;
    this.txt_location.Text = asset.name;
    if (booking.booked_for == booking.created_by)
      this.lbl_requestedBy.Value = user.full_name;
    else
      this.lbl_requestedBy.Value = this.users.get_user(booking.created_by, booking.account_id).full_name;
    try
    {
      if (!string.IsNullOrEmpty(this.Request.QueryString["isrepeat"]))
        this.is_repeat = Convert.ToBoolean(this.Request.QueryString["isrepeat"]);
    }
    catch
    {
      this.is_repeat = false;
    }
    DataSet dataSet = new DataSet();
    if (this.is_repeat)
      dataSet = this.bookings.get_repeat_booking_assets(this.current_user.account_id, booking.repeat_reference_id);
    DataTable table = (DataTable) this.ViewState["dates_table"];
    if (!this.is_repeat)
    {
      DataRow row = table.NewRow();
      row["no"] = (object) (table.Rows.Count + 1);
      row["from_date"] = (object) booking.book_from.ToString(api_constants.display_datetime_format);
      row["to_date"] = (object) booking.book_to.ToString(api_constants.display_datetime_format);
      row["id"] = (object) this.asset_booking_id.ToString();
      row["status"] = (object) booking.status.ToString();
      table.Rows.Add(row);
      table.AcceptChanges();
    }
    else
    {
      foreach (DataRow row1 in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      {
        DataRow row2 = table.NewRow();
        row2["no"] = (object) (table.Rows.Count + 1);
        row2["from_date"] = (object) Convert.ToDateTime(row1["book_from"]).ToString(api_constants.display_datetime_format);
        row2["to_date"] = (object) Convert.ToDateTime(row1["book_from"]).ToString(api_constants.display_datetime_format);
        row2["id"] = row1["booking_id"];
        row2["status"] = (object) "1";
        table.Rows.Add(row2);
        table.AcceptChanges();
      }
    }
    this.ViewState["dates_table"] = (object) table;
    this.bind_dates_grid(table);
    foreach (Control control in this.gridview_add_dates.Items)
      control.FindControl("btn_remove").Visible = false;
    this.span_duplicate.InnerText = "";
    this.span_duplicate.Visible = false;
    this.txt_purpose.Enabled = false;
    this.txtRemarks.Enabled = false;
    this.txtBookedFor.Enabled = false;
    this.txt_email.Enabled = false;
    this.txt_location.Enabled = false;
    this.btn_next_step.Visible = false;
    this.tblDateSelectAdd.Visible = false;
    this.pnl_dates.Visible = true;
    this.pnl_items.Visible = true;
    this.pnl_items_table.Visible = true;
  }

  private void update_resource_table_data()
  {
    DataTable dataTable = (DataTable) this.ViewState["items_table"];
    foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
    {
      row["requested_qty"] = (object) this.Request.Form["id_" + row["no"]];
      row["accepted_qty"] = (object) this.Request.Form["id_" + row["no"]];
      row["remarks"] = (object) this.Request.Form["rem_" + row["no"]];
      dataTable.AcceptChanges();
      this.ViewState["items_table"] = (object) dataTable;
    }
  }

  private DataTable initialize_date_table()
  {
    DataTable dataTable = new DataTable();
    dataTable.Columns.Add(new DataColumn("no"));
    dataTable.Columns.Add(new DataColumn("from_date"));
    dataTable.Columns.Add(new DataColumn("to_date"));
    dataTable.Columns.Add(new DataColumn("id"));
    dataTable.Columns.Add(new DataColumn("type"));
    dataTable.Columns.Add(new DataColumn("status"));
    dataTable.AcceptChanges();
    this.ViewState.Add("dates_table", (object) dataTable);
    return dataTable;
  }

  private DataTable initialize_items_table()
  {
    DataTable dataTable = new DataTable();
    dataTable.Columns.Add(new DataColumn("no"));
    dataTable.Columns.Add(new DataColumn("item_id"));
    dataTable.Columns.Add(new DataColumn("item_type"));
    dataTable.Columns.Add(new DataColumn("item_name"));
    dataTable.Columns.Add(new DataColumn("available_qty"));
    dataTable.Columns.Add(new DataColumn("requested_qty"));
    dataTable.Columns.Add(new DataColumn("accepted_qty"));
    dataTable.Columns.Add(new DataColumn("old_qty"));
    dataTable.Columns.Add(new DataColumn("remarks"));
    dataTable.Columns.Add(new DataColumn("date_no"));
    dataTable.Columns.Add(new DataColumn("from_date"));
    dataTable.Columns.Add(new DataColumn("to_date"));
    dataTable.Columns.Add(new DataColumn("status"));
    dataTable.Columns.Add(new DataColumn("id"));
    dataTable.Columns.Add(new DataColumn("resource_booking_item_id"));
    dataTable.AcceptChanges();
    this.ViewState.Add("items_table", (object) dataTable);
    return dataTable;
  }

  private void initialize_ui()
  {
    this.lbl_requestedBy.Value = this.current_user.full_name;
    this.txtBookedFor.Text = this.current_user.full_name;
    this.txt_email.Text = this.current_user.email;
    this.hfUserId.Value = this.current_user.user_id.ToString();
    try
    {
      this.asset_booking_id = Convert.ToInt64(this.Request.QueryString["booking_id"]);
    }
    catch
    {
      this.asset_booking_id = 0L;
    }
    try
    {
      this.resource_booking_id = Convert.ToInt64(this.Request.QueryString["resource_booking_id"]);
    }
    catch
    {
      this.resource_booking_id = 0L;
    }
    if (this.asset_booking_id > 0L)
    {
      DataRow[] dataRowArray = this.resource_settings_data.Tables[0].Select("value='Additional Resources with asset booking'");
      if (dataRowArray.Length > 0)
        this.hdn_booking_type.Value = dataRowArray[0]["setting_id"].ToString();
    }
    else
    {
      DataRow[] dataRowArray = this.resource_settings_data.Tables[0].Select("value='Additional Resources without asset booking'");
      if (dataRowArray.Length > 0)
        this.hdn_booking_type.Value = dataRowArray[0]["setting_id"].ToString();
    }
    try
    {
      if (!string.IsNullOrEmpty(this.Request.QueryString["isrepeat"]))
        this.is_repeat = Convert.ToBoolean(this.Request.QueryString["isrepeat"]);
    }
    catch
    {
      this.is_repeat = false;
    }
    this.initialize_start_end_time();
    this.populate_res_type();
  }

  private void initialize_start_end_time()
  {
    this.utilities.Populate_Time(this.ddl_StartTime, this.current_timestamp);
    this.utilities.Populate_Time(this.ddl_EndTime, this.current_timestamp);
    this.txt_startDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
    this.txt_endDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
    this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("h:mm tt");
    this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddHours(1.0)).ToString("h:mm tt");
  }

  private void populate_res_type()
  {
    this.ddl_res_name.Items.Clear();
    this.ddl_res_type.Items.Clear();
    this.ddl_res_type.Items.Add(new ListItem()
    {
      Text = "select",
      Value = "select"
    });
    this.ddl_res_type.Items.Add(new ListItem()
    {
      Text = "Templates",
      Value = "0"
    });
    try
    {
      foreach (DataRow dataRow in this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, "resource_type", this.str_resource_module).Tables[0].Select("status > 0 "))
        this.ddl_res_type.Items.Add(new ListItem()
        {
          Text = dataRow["value"].ToString(),
          Value = dataRow["setting_id"].ToString()
        });
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_next_step_Click(object sender, EventArgs e) => this.pnl_dates.Visible = true;

  protected void btn_add_date_Click(object sender, EventArgs e)
  {
    bool flag = true;
    DateTime dateTime1 = Convert.ToDateTime(Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_StartTime.SelectedItem.Text);
    DateTime dateTime2 = Convert.ToDateTime(Convert.ToDateTime(this.txt_endDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_EndTime.SelectedItem.Text);
    if ((dateTime2 - dateTime1).TotalMinutes <= 0.0)
    {
      this.span_duplicate.InnerText = "The start and end dates/times are the same. The end date/time should be later than the start date/time.";
      this.span_duplicate.Visible = true;
    }
    else if (flag)
    {
      DataTable table = (DataTable) this.ViewState["dates_table"] ?? this.initialize_date_table();
      if (table.Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
        {
          if (row["status"].ToString() == "1")
          {
            if (Convert.ToDateTime(row["from_date"]) != dateTime1 || Convert.ToDateTime(row["to_date"]) != dateTime2)
            {
              string filterExpression = "from_date > #" + dateTime1.ToString(api_constants.display_datetime_format) + "# and to_date < #" + dateTime2.ToString(api_constants.display_datetime_format) + "# or to_date > #" + dateTime1.ToString(api_constants.display_datetime_format) + "# and from_date < #" + dateTime2.ToString(api_constants.display_datetime_format) + "#";
              if (table.Select(filterExpression).Length > 0)
              {
                this.span_duplicate.InnerText = "Select a date and time that does not overlap with the ones that you have already selected below.";
                this.span_duplicate.Visible = true;
                return;
              }
            }
            else
            {
              this.span_duplicate.InnerText = "The specified date and time range has already been added or is overlapping with another date and time range.";
              this.span_duplicate.Visible = true;
              return;
            }
          }
        }
      }
      DataRow row1 = table.NewRow();
      row1["no"] = (object) (table.Rows.Count + 1);
      row1["from_date"] = (object) dateTime1.ToString(api_constants.display_datetime_format);
      row1["to_date"] = (object) dateTime2.ToString(api_constants.display_datetime_format);
      row1["id"] = (object) "0";
      row1["status"] = (object) "1";
      table.Rows.Add(row1);
      table.AcceptChanges();
      this.ViewState["dates_table"] = (object) table;
      this.bind_dates_grid(table);
      this.span_duplicate.InnerText = "";
      this.span_duplicate.Visible = false;
      if (table.Rows.Count > 0)
      {
        this.pnl_items.Visible = true;
        this.pnl_items_table.Visible = true;
      }
      else
      {
        this.pnl_items.Visible = false;
        this.pnl_items_table.Visible = false;
      }
    }
    else
    {
      this.span_duplicate.InnerText = " To time should later than From time";
      this.span_duplicate.Visible = true;
    }
  }

  private void populate_resource_names()
  {
    this.img_res.ImageUrl = (string) null;
    this.ddl_res_name.Items.Clear();
    this.ddl_res_name.Items.Add(new ListItem()
    {
      Text = "select",
      Value = "select"
    });
    try
    {
      if (this.ddl_res_type.SelectedItem.Value != "0")
      {
        DataSet dataSet1 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
        if (dataSet1 == null)
        {
          dataSet1 = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
          this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) dataSet1);
        }
        DataSet itemsByItemTypeId = this.resapi.get_resource_items_by_item_type_id(Convert.ToInt64(this.ddl_res_type.SelectedItem.Value), this.current_user.account_id, this.str_resource_module);
        DataSet dataSet2 = (DataSet) this.ViewState["allowed_items"];
        foreach (DataRow row in (InternalDataCollectionBase) itemsByItemTypeId.Tables[0].Rows)
        {
          if (dataSet2.Tables[0].Select("item_id='" + row["item_id"].ToString() + "'").Length > 0)
          {
            int num1;
            try
            {
              num1 = Convert.ToInt32(dataSet1.Tables[0].Select("parameter='Advance Notice Period' and parent_id=" + row["item_id"].ToString())[0]["value"]);
            }
            catch (Exception ex)
            {
              num1 = 0;
            }
            int num2;
            try
            {
              num2 = Convert.ToInt32(dataSet1.Tables[0].Select("parameter='Advance Notice Period Hours' and parent_id=" + row["item_id"].ToString())[0]["value"]);
            }
            catch (Exception ex)
            {
              num2 = 0;
            }
            ListItem listItem = new ListItem();
            if (num1 > 0)
              listItem.Text = row["name"].ToString() + " (" + (object) num1 + " day(s) notice reqd.)";
            else if (num2 > 0)
              listItem.Text = row["name"].ToString() + " (" + (object) num2 + " hour(s) notice reqd.)";
            else
              listItem.Text = row["name"].ToString();
            listItem.Value = row["item_id"].ToString();
            this.ddl_res_name.Items.Add(listItem);
          }
        }
      }
      else
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resource_templates(this.current_user.account_id).Tables[0].Rows)
          this.ddl_res_name.Items.Add(new ListItem()
          {
            Text = row["template_name"].ToString(),
            Value = row["resource_template_id"].ToString()
          });
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void ddl_res_type_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.populate_resource_names();
    this.lit_available_qty.Text = "";
    this.img_res.ImageUrl = "../assets/img/noimage.gif";
    this.img_res.AlternateText = "Item Image";
    this.txt_requestor_remarks.Text = "";
    this.litResource.Text = "";
  }

  protected void ddl_res_name_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      this.lblError.Visible = false;
      this.txtQty.Text = "";
      this.txt_requestor_remarks.Text = "";
      this.litResource.Text = "";
      if (this.ddl_res_type.SelectedItem.Text != "Templates")
      {
        if (this.ddl_res_name.SelectedItem.Value == "select")
        {
          this.txtQty.Text = "";
          this.txt_requestor_remarks.Text = "";
          this.litResource.Text = "";
          this.img_res.ImageUrl = "";
          this.img_res.AlternateText = "Image";
        }
        else
        {
          int num1 = 0;
          int num2 = 0;
          this.tr_template.Visible = false;
          long int64 = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
          foreach (DataRow row in (InternalDataCollectionBase) ((DataTable) this.ViewState["items_table"]).Rows)
          {
            if (row["item_id"].ToString() == int64.ToString())
            {
              this.lit_available_qty.Text = "<p><h3 style='color:#ff0000;font-size:15px;'>Item has already been added.</h3></p>";
              this.tr_qty.Visible = false;
              this.txtQty.Visible = false;
              this.btn_add_resource.Enabled = false;
              return;
            }
          }
          DataSet resourceItemsById = this.resapi.get_resource_items_by_id(int64, this.current_user.account_id, this.str_resource_module);
          if (resourceItemsById.Tables[0].Rows.Count > 0)
          {
            DataRow row = resourceItemsById.Tables[0].Rows[0];
            this.litResource.Text = row["description"].ToString();
            try
            {
              num1 = Convert.ToInt32(row["advance_notice_period"]);
            }
            catch
            {
              num1 = 0;
            }
            try
            {
              num2 = Convert.ToInt32(row["advance_notice_period_hours"]);
            }
            catch
            {
              num2 = 0;
            }
          }
          byte[] image = this.resapi.get_image(int64, this.current_user.account_id);
          DataTable dataTable = (DataTable) this.ViewState["dates_table"];
          if (dataTable.Rows.Count >= 1)
          {
            string str = "<h3>Availability</h3><table width='100%' cellpadding='5' cellspacing='5' border='1'><tr><td><b>From</b></td><td><b>To</b></td><td><b>Qty Available</b></td><td><b>Notice Period</b></td><td><b>Can Request</b></td></tr>";
            foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            {
              if (row["status"].ToString() == "1")
              {
                DateTime dateTime1 = Convert.ToDateTime(row["from_date"]);
                DateTime dateTime2 = Convert.ToDateTime(row["to_date"]);
                double bookedQuantity = this.resapi.get_booked_quantity(int64, this.current_user.account_id, dateTime1, dateTime2, this.str_resource_module);
                double num3 = Convert.ToDouble(resourceItemsById.Tables[0].Rows[0]["quantity"]) - bookedQuantity;
                this.hdn_available_qty.Value = num3.ToString();
                str = str + "<tr><td>" + dateTime1.ToString("dd-MMM-yy hh:mm tt") + "</td>";
                str = str + "<td>" + dateTime2.ToString("dd-MMM-yy hh:mm tt") + "</td>";
                str = str + "<td>" + (object) num3 + "</td>";
                if (num1 > 0)
                  str = str + "<td>" + (object) num1 + " day(s)</td>";
                else if (num2 > 0)
                  str = str + "<td>" + (object) num2 + " hour(s)</td>";
                else
                  str += "<td> - </td>";
                int num4;
                try
                {
                  num4 = Convert.ToInt32(resourceItemsById.Tables[0].Rows[0]["owner_group_id"]);
                }
                catch
                {
                  num4 = 0;
                }
                bool flag = false;
                if (num4 > 0)
                {
                  foreach (string key in this.current_user.groups.Keys)
                  {
                    if (this.current_user.groups[key].group_id == (long) num4)
                    {
                      flag = true;
                      break;
                    }
                  }
                }
                if (flag)
                {
                  str += "<td style='color:green'>yes</td></tr>";
                }
                else
                {
                  TimeSpan timeSpan = dateTime1 - this.current_timestamp;
                  if (num2 > 0)
                    str = timeSpan.Hours > num2 ? str + "<td style='color:green'>yes</td></tr>" : str + "<td style='color:red'>No</td></tr>";
                  else if (num1 > 0)
                  {
                    int num5 = 0;
                    for (DateTime dateTime3 = this.current_timestamp.Date; dateTime3 <= dateTime1; dateTime3 = dateTime3.AddDays(1.0))
                    {
                      if (dateTime3.DayOfWeek == DayOfWeek.Saturday || dateTime3.DayOfWeek == DayOfWeek.Sunday)
                        ++num5;
                      if (this.bookingsbl.getPublicHolidayList(dateTime3.ToString(api_constants.datetime_format), dateTime3.AddDays(1.0).AddSeconds(-1.0).ToString(api_constants.datetime_format), this.current_user.account_id) != "")
                        ++num5;
                    }
                    str = timeSpan.Days - num5 > num1 ? str + "<td style='color:green'>yes</td></tr>" : str + "<td style='color:red'>No</td></tr>";
                  }
                  else
                    str += "<td style='color:green'>yes</td></tr>";
                }
              }
            }
            this.lit_available_qty.Text = str + "</table><br/><br/>";
          }
          this.img_res.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(image, 0, image.Length);
        }
      }
      else if (this.ddl_res_name.SelectedItem.Value == "select")
      {
        this.txtQty.Text = "";
        this.txt_requestor_remarks.Text = "";
        this.litResource.Text = "";
        this.img_res.ImageUrl = "";
        this.img_res.AlternateText = "Image";
      }
      else
      {
        this.tr_qty.Visible = false;
        this.tr_remarks.Visible = false;
        this.tr_img.Visible = false;
        int num6 = 0;
        int num7 = 0;
        this.tr_template.Visible = true;
        long int64 = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
        foreach (DataRow row in (InternalDataCollectionBase) ((DataTable) this.ViewState["items_table"]).Rows)
        {
          if (row["item_id"].ToString() == int64.ToString())
          {
            this.lit_available_qty.Text = "<p><h3 style='color:#ff0000;font-size:15px;'>Item has already been added.</h3></p>";
            this.tr_qty.Visible = false;
            this.txtQty.Visible = false;
            this.btn_add_resource.Enabled = false;
            return;
          }
        }
        DataSet dataSet1 = new DataSet();
        DataSet itemByTemplateId = this.resapi.get_resource_template_item_by_templateId(Convert.ToInt64(this.ddl_res_name.SelectedItem.Value), this.current_user.account_id);
        DataSet dataSet2 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
        if (dataSet2 == null)
        {
          dataSet2 = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
          this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) dataSet2);
        }
        DataTable dataTable = (DataTable) this.ViewState["dates_table"];
        if (dataTable.Rows.Count < 1)
          return;
        string str = "<h3>Availability</h3><table width='100%' cellpadding='5' cellspacing='5' border='1'><tr><td><b>From</b></td><td><b>To</b></td><td><b>Item</b></td><td><b>Qty Available</b></td><td><b>Notice Period</b></td><td><b>Can Request</b></td></tr>";
        foreach (DataRow row1 in (InternalDataCollectionBase) dataTable.Rows)
        {
          if (row1["status"].ToString() == "1")
          {
            foreach (DataRow row2 in (InternalDataCollectionBase) itemByTemplateId.Tables[0].Rows)
            {
              DataRow[] dataRowArray1 = dataSet2.Tables[0].Select("parameter='Advance Notice Period' and parent_id='" + row2["item_id"] + "'");
              if (dataRowArray1.Length > 0)
                num6 = Convert.ToInt32(dataRowArray1[0]["value"]);
              DataRow[] dataRowArray2 = dataSet2.Tables[0].Select("parameter='Advance Notice Period Hours' and parent_id='" + row2["item_id"] + "'");
              if (dataRowArray2.Length > 0)
                num7 = Convert.ToInt32(dataRowArray2[0]["value"]);
              DateTime dateTime4 = Convert.ToDateTime(row1["from_date"]);
              DateTime dateTime5 = Convert.ToDateTime(row1["to_date"]);
              double bookedQuantity = this.resapi.get_booked_quantity(Convert.ToInt64(row2["item_id"].ToString()), this.current_user.account_id, dateTime4, dateTime5, this.str_resource_module);
              double num8 = Convert.ToDouble(row2["quantity"]) - bookedQuantity;
              str = str + "<td>" + dateTime4.ToString("dd-MMM-yy hh:mm tt") + "</td>";
              str = str + "<td>" + dateTime5.ToString("dd-MMM-yy hh:mm tt") + "</td>";
              str = str + "<td>" + row2["resource_name"] + "</td>";
              str = str + "<td>" + (object) num8 + "</td>";
              if (num6 > 0)
                str = str + "<td>" + (object) num6 + " day(s)</td>";
              else if (num7 > 0)
                str = str + "<td>" + (object) num7 + " hour(s)</td>";
              else
                str += "<td> - </td>";
              DataSet resourceItemsById = this.resapi.get_resource_items_by_id(int64, this.current_user.account_id, this.str_resource_module);
              int num9;
              try
              {
                num9 = Convert.ToInt32(resourceItemsById.Tables[0].Rows[0]["owner_group_id"]);
              }
              catch
              {
                num9 = 0;
              }
              bool flag = false;
              if (num9 > 0)
              {
                foreach (string key in this.current_user.groups.Keys)
                {
                  if (this.current_user.groups[key].group_id == (long) num9)
                  {
                    flag = true;
                    break;
                  }
                }
              }
              if (flag)
              {
                str += "<td style='color:green'>yes</td></tr>";
              }
              else
              {
                TimeSpan timeSpan = dateTime4 - this.current_timestamp;
                if (num7 > 0)
                  str = timeSpan.Hours > num7 ? str + "<td style='color:green'>yes</td></tr>" : str + "<td style='color:red'>No</td></tr>";
                else if (num6 > 0)
                {
                  int num10 = 0;
                  for (DateTime dateTime6 = this.current_timestamp.Date; dateTime6 <= dateTime4; dateTime6 = dateTime6.AddDays(1.0))
                  {
                    if (dateTime6.DayOfWeek == DayOfWeek.Saturday || dateTime6.DayOfWeek == DayOfWeek.Sunday)
                      ++num10;
                    if (this.bookingsbl.getPublicHolidayList(dateTime6.ToString(api_constants.datetime_format), dateTime6.AddDays(1.0).AddSeconds(-1.0).ToString(api_constants.datetime_format), this.current_user.account_id) != "")
                      ++num10;
                  }
                  str = timeSpan.Days - num10 > num6 ? str + "<td style='color:green'>yes</td></tr>" : str + "<td style='color:red'>No</td></tr>";
                }
                else
                  str += "<td style='color:green'>yes</td></tr>";
              }
            }
          }
          str = str + "</table><br/><br/>" + "<p><b>Items where the notice period is too short will not be added to the list.</b></p>" + "<p><b>When adding templates, individual quantities must be set after adding the resources.</b></p>";
          this.lit_available_qty.Text = str;
          this.btn_add_resource.Enabled = true;
          this.btn_add_resource.CssClass += " blue";
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_add_resource_Click(object sender, EventArgs e)
  {
    this.txt_requestor_remarks.Text = "";
    DataTable items_table = (DataTable) this.ViewState["items_table"];
    DataTable dates_table = (DataTable) this.ViewState["dates_table"];
    int num1 = 0;
    int num2 = 0;
    if (this.ddl_res_type.SelectedItem.Text != "Templates")
    {
      long int64 = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
      DataSet resourceItemsById = this.resapi.get_resource_items_by_id(int64, this.current_user.account_id, this.str_resource_module);
      if (resourceItemsById.Tables[0].Rows.Count > 0)
      {
        DataRow row = resourceItemsById.Tables[0].Rows[0];
        try
        {
          num1 = Convert.ToInt32(row["advance_notice_period"]);
        }
        catch
        {
          num1 = 0;
        }
        try
        {
          num2 = Convert.ToInt32(row["advance_notice_period_hours"]);
        }
        catch
        {
          num2 = 0;
        }
      }
      foreach (DataRow row1 in (InternalDataCollectionBase) dates_table.Rows)
      {
        if (items_table.Select("item_id='" + (object) int64 + "' and date_no='" + row1["no"] + "'").Length == 0)
        {
          TimeSpan timeSpan = Convert.ToDateTime(row1["from_date"]) - this.current_timestamp;
          int num3;
          try
          {
            num3 = Convert.ToInt32(resourceItemsById.Tables[0].Rows[0]["owner_group_id"]);
          }
          catch
          {
            num3 = 0;
          }
          bool flag = false;
          if (num3 > 0)
          {
            foreach (string key in this.current_user.groups.Keys)
            {
              if (this.current_user.groups[key].group_id == (long) num3)
              {
                flag = true;
                break;
              }
            }
          }
          if (flag)
          {
            if (row1["status"].ToString() == "1")
            {
              DateTime dateTime1 = Convert.ToDateTime(row1["from_date"]);
              DateTime dateTime2 = Convert.ToDateTime(row1["to_date"]);
              double bookedQuantity = this.resapi.get_booked_quantity(int64, this.current_user.account_id, dateTime1, dateTime2, this.str_resource_module);
              double num4 = Convert.ToDouble(resourceItemsById.Tables[0].Rows[0]["quantity"]) - bookedQuantity;
              DataRow row2 = items_table.NewRow();
              row2["no"] = (object) (items_table.Rows.Count + 1);
              row2["item_id"] = (object) int64;
              row2["item_name"] = (object) this.ddl_res_name.SelectedItem.Text;
              row2["item_type"] = (object) this.ddl_res_type.SelectedItem.Text;
              row2["available_qty"] = (object) num4;
              row2["requested_qty"] = (object) this.txtQty.Text;
              row2["accepted_qty"] = (object) this.txtQty.Text;
              row2["old_qty"] = (object) "0";
              row2["remarks"] = (object) this.txt_requestor_remarks.Text;
              row2["date_no"] = row1["no"];
              row2["id"] = (object) "0";
              row2["status"] = (object) "1";
              row2["from_date"] = row1["from_date"];
              row2["to_date"] = row1["to_date"];
              row2["resource_booking_item_id"] = (object) "0";
              items_table.Rows.Add(row2);
              items_table.AcceptChanges();
            }
          }
          else if (num2 > 0)
          {
            if (timeSpan.Hours > num2 && row1["status"].ToString() == "1")
            {
              DateTime dateTime3 = Convert.ToDateTime(row1["from_date"]);
              DateTime dateTime4 = Convert.ToDateTime(row1["to_date"]);
              double bookedQuantity = this.resapi.get_booked_quantity(int64, this.current_user.account_id, dateTime3, dateTime4, this.str_resource_module);
              double num5 = Convert.ToDouble(resourceItemsById.Tables[0].Rows[0]["quantity"]) - bookedQuantity;
              DataRow row3 = items_table.NewRow();
              row3["no"] = (object) (items_table.Rows.Count + 1);
              row3["item_id"] = (object) int64;
              row3["item_name"] = (object) this.ddl_res_name.SelectedItem.Text;
              row3["item_type"] = (object) this.ddl_res_type.SelectedItem.Text;
              row3["available_qty"] = (object) num5;
              row3["requested_qty"] = (object) this.txtQty.Text;
              row3["accepted_qty"] = (object) this.txtQty.Text;
              row3["old_qty"] = (object) "0";
              row3["remarks"] = (object) this.txt_requestor_remarks.Text;
              row3["date_no"] = row1["no"];
              row3["id"] = (object) "0";
              row3["status"] = (object) "1";
              row3["from_date"] = row1["from_date"];
              row3["to_date"] = row1["to_date"];
              row3["resource_booking_item_id"] = (object) "0";
              items_table.Rows.Add(row3);
              items_table.AcceptChanges();
            }
          }
          else if (num1 > 0)
          {
            int num6 = 0;
            for (DateTime dateTime = this.current_timestamp.Date; dateTime <= Convert.ToDateTime(row1["from_date"]); dateTime = dateTime.AddDays(1.0))
            {
              if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
                ++num6;
              if (this.bookingsbl.getPublicHolidayList(dateTime.ToString(api_constants.datetime_format), dateTime.AddDays(1.0).AddSeconds(-1.0).ToString(api_constants.datetime_format), this.current_user.account_id) != "")
                ++num6;
            }
            if (timeSpan.Days - num6 > num1 && row1["status"].ToString() == "1")
            {
              DateTime dateTime5 = Convert.ToDateTime(row1["from_date"]);
              DateTime dateTime6 = Convert.ToDateTime(row1["to_date"]);
              double bookedQuantity = this.resapi.get_booked_quantity(int64, this.current_user.account_id, dateTime5, dateTime6, this.str_resource_module);
              double num7 = Convert.ToDouble(resourceItemsById.Tables[0].Rows[0]["quantity"]) - bookedQuantity;
              DataRow row4 = items_table.NewRow();
              row4["no"] = (object) (items_table.Rows.Count + 1);
              row4["item_id"] = (object) int64;
              row4["item_name"] = (object) this.ddl_res_name.SelectedItem.Text;
              row4["item_type"] = (object) this.ddl_res_type.SelectedItem.Text;
              row4["available_qty"] = (object) num7;
              row4["requested_qty"] = (object) this.txtQty.Text;
              row4["accepted_qty"] = (object) this.txtQty.Text;
              row4["old_qty"] = (object) "0";
              row4["remarks"] = (object) this.txt_requestor_remarks.Text;
              row4["date_no"] = row1["no"];
              row4["id"] = (object) "0";
              row4["status"] = (object) "1";
              row4["from_date"] = row1["from_date"];
              row4["to_date"] = row1["to_date"];
              row4["resource_booking_item_id"] = (object) "0";
              items_table.Rows.Add(row4);
              items_table.AcceptChanges();
            }
          }
          else if (row1["status"].ToString() == "1")
          {
            DateTime dateTime7 = Convert.ToDateTime(row1["from_date"]);
            DateTime dateTime8 = Convert.ToDateTime(row1["to_date"]);
            double bookedQuantity = this.resapi.get_booked_quantity(int64, this.current_user.account_id, dateTime7, dateTime8, this.str_resource_module);
            double num8 = Convert.ToDouble(resourceItemsById.Tables[0].Rows[0]["quantity"]) - bookedQuantity;
            DataRow row5 = items_table.NewRow();
            row5["no"] = (object) (items_table.Rows.Count + 1);
            row5["item_id"] = (object) int64;
            row5["item_name"] = (object) this.ddl_res_name.SelectedItem.Text;
            row5["item_type"] = (object) this.ddl_res_type.SelectedItem.Text;
            row5["available_qty"] = (object) num8;
            row5["requested_qty"] = (object) this.txtQty.Text;
            row5["accepted_qty"] = (object) this.txtQty.Text;
            row5["old_qty"] = (object) "0";
            row5["remarks"] = (object) this.txt_requestor_remarks.Text;
            row5["date_no"] = row1["no"];
            row5["id"] = (object) "0";
            row5["status"] = (object) "1";
            row5["from_date"] = row1["from_date"];
            row5["to_date"] = row1["to_date"];
            row5["resource_booking_item_id"] = (object) "0";
            items_table.Rows.Add(row5);
            items_table.AcceptChanges();
          }
        }
      }
    }
    else
    {
      DataSet resourceItems = this.resapi.get_resource_items(this.current_user.account_id, this.str_resource_module);
      DataSet dataSet = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
      if (dataSet == null)
      {
        dataSet = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) dataSet);
      }
      DataSet itemByTemplateId = this.resapi.get_resource_template_item_by_templateId(Convert.ToInt64(this.ddl_res_name.SelectedItem.Value), this.current_user.account_id);
      foreach (DataRow row6 in (InternalDataCollectionBase) dates_table.Rows)
      {
        foreach (DataRow row7 in (InternalDataCollectionBase) itemByTemplateId.Tables[0].Rows)
        {
          double num9 = 0.0;
          if (items_table.Select("item_id='" + row7["item_id"] + "' and date_no='" + row6["no"] + "'").Length == 0)
          {
            int num10 = 0;
            int num11 = 0;
            DataRow[] dataRowArray1 = dataSet.Tables[0].Select("parameter='Advance Notice Period' and parent_id='" + row7["item_id"] + "'");
            if (dataRowArray1.Length > 0)
              num10 = Convert.ToInt32(dataRowArray1[0]["value"]);
            DataRow[] dataRowArray2 = dataSet.Tables[0].Select("parameter='Advance Notice Period Hours' and parent_id='" + row7["item_id"] + "'");
            if (dataRowArray2.Length > 0)
              num11 = Convert.ToInt32(dataRowArray2[0]["value"]);
            TimeSpan timeSpan = Convert.ToDateTime(row6["from_date"]) - this.current_timestamp;
            if (num11 > 0)
            {
              if (timeSpan.Hours > num11)
              {
                DataRow[] dataRowArray3 = resourceItems.Tables[0].Select("item_id='" + row7["item_id"] + "'");
                if (dataRowArray3.Length > 0)
                  num9 = Convert.ToDouble(dataRowArray3[0]["quantity"]);
                double bookedQuantity = this.resapi.get_booked_quantity(Convert.ToInt64(row7["item_id"]), this.current_user.account_id, Convert.ToDateTime(row6["from_date"]), Convert.ToDateTime(row6["to_date"]), this.str_resource_module);
                double num12 = num9 - bookedQuantity;
                DataRow row8 = items_table.NewRow();
                row8["no"] = (object) (items_table.Rows.Count + 1);
                row8["item_id"] = row7["item_id"];
                row8["item_name"] = dataRowArray3[0]["name"];
                row8["item_type"] = dataRowArray3[0]["resource_type"];
                row8["available_qty"] = (object) num12;
                row8["requested_qty"] = row7["quantity"];
                row8["accepted_qty"] = row7["quantity"];
                row8["old_qty"] = (object) "0";
                row8["remarks"] = (object) "";
                row8["date_no"] = row6["no"];
                row8["id"] = (object) "0";
                row8["status"] = (object) "1";
                row8["from_date"] = row6["from_date"];
                row8["to_date"] = row6["to_date"];
                row8["resource_booking_item_id"] = (object) "0";
                items_table.Rows.Add(row8);
                items_table.AcceptChanges();
              }
            }
            else if (num10 > 0)
            {
              int num13 = 0;
              for (DateTime dateTime9 = this.current_timestamp.Date; dateTime9 <= Convert.ToDateTime(row7["from_date"]); dateTime9 = dateTime9.AddDays(1.0))
              {
                if (dateTime9.DayOfWeek == DayOfWeek.Saturday || dateTime9.DayOfWeek == DayOfWeek.Sunday)
                  ++num13;
                booking_bl bookingsbl = this.bookingsbl;
                string from = dateTime9.ToString(api_constants.datetime_format);
                DateTime dateTime10 = dateTime9.AddDays(1.0);
                dateTime10 = dateTime10.AddSeconds(-1.0);
                string to = dateTime10.ToString(api_constants.datetime_format);
                Guid accountId = this.current_user.account_id;
                string publicHolidayList = bookingsbl.getPublicHolidayList(from, to, accountId);
                int num14 = publicHolidayList == "" ? 1 : 0;
                if (publicHolidayList != "")
                  ++num13;
              }
              if (timeSpan.Days - num13 > num10)
              {
                DataRow[] dataRowArray4 = resourceItems.Tables[0].Select("item_id='" + row7["item_id"] + "'");
                if (dataRowArray4.Length > 0)
                  num9 = Convert.ToDouble(dataRowArray4[0]["quantity"]);
                double bookedQuantity = this.resapi.get_booked_quantity(Convert.ToInt64(row7["item_id"]), this.current_user.account_id, Convert.ToDateTime(row6["from_date"]), Convert.ToDateTime(row6["to_date"]), this.str_resource_module);
                double num15 = num9 - bookedQuantity;
                DataRow row9 = items_table.NewRow();
                row9["no"] = (object) (items_table.Rows.Count + 1);
                row9["item_id"] = row7["item_id"];
                row9["item_name"] = dataRowArray4[0]["name"];
                row9["item_type"] = dataRowArray4[0]["resource_type"];
                row9["available_qty"] = (object) num15;
                row9["requested_qty"] = row7["quantity"];
                row9["accepted_qty"] = row7["quantity"];
                row9["old_qty"] = (object) "0";
                row9["remarks"] = (object) "";
                row9["date_no"] = row6["no"];
                row9["id"] = (object) "0";
                row9["status"] = (object) "1";
                row9["from_date"] = row6["from_date"];
                row9["to_date"] = row6["to_date"];
                row9["resource_booking_item_id"] = (object) "0";
                items_table.Rows.Add(row9);
                items_table.AcceptChanges();
              }
            }
            else
            {
              DataRow[] dataRowArray5 = resourceItems.Tables[0].Select("item_id='" + row7["item_id"] + "'");
              if (dataRowArray5.Length > 0)
                num9 = Convert.ToDouble(dataRowArray5[0]["quantity"]);
              double bookedQuantity = this.resapi.get_booked_quantity(Convert.ToInt64(row7["item_id"]), this.current_user.account_id, Convert.ToDateTime(row6["from_date"]), Convert.ToDateTime(row6["to_date"]), this.str_resource_module);
              double num16 = num9 - bookedQuantity;
              DataRow row10 = items_table.NewRow();
              row10["no"] = (object) (items_table.Rows.Count + 1);
              row10["item_id"] = row7["item_id"];
              row10["item_name"] = dataRowArray5[0]["name"];
              row10["item_type"] = dataRowArray5[0]["resource_type"];
              row10["available_qty"] = (object) num16;
              row10["requested_qty"] = row7["quantity"];
              row10["accepted_qty"] = row7["quantity"];
              row10["old_qty"] = (object) "0";
              row10["remarks"] = (object) "";
              row10["date_no"] = row6["no"];
              row10["id"] = (object) "0";
              row10["status"] = (object) "1";
              row10["from_date"] = row6["from_date"];
              row10["to_date"] = row6["to_date"];
              row10["resource_booking_item_id"] = (object) "0";
              items_table.Rows.Add(row10);
              items_table.AcceptChanges();
            }
          }
        }
      }
    }
    this.ViewState["items_table"] = (object) items_table;
    this.populate_resource_html(dates_table, items_table);
    this.clean_up_resource_selection_form();
    this.chk_terms.Visible = true;
    this.btn_submit.Enabled = true;
    this.btn_submit.CssClass += "green";
  }

  private void clean_up_resource_selection_form()
  {
    this.txtQty.Text = "";
    this.txt_requestor_remarks.Text = "";
    this.lit_available_qty.Text = "";
    this.litResource.Text = "";
    this.img_res.ImageUrl = "";
    this.img_res.AlternateText = "Item Image";
    this.populate_res_type();
  }

  private void populate_resource_html(DataTable dates_table, DataTable items_table)
  {
    StringBuilder stringBuilder = new StringBuilder();
    int num1 = 0;
    if (items_table.Rows.Count > 0)
    {
      foreach (DataRow row in (InternalDataCollectionBase) dates_table.Rows)
      {
        stringBuilder.Append("<h3>" + row["from_date"] + " to " + row["to_date"] + "</h3></tr>");
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover'>");
        DataRow[] dataRowArray = items_table.Select("from_date='" + row["from_date"] + "'");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr class='resource_table_header'><td><b>No.</b></td><td><b>Type</b></td><td><b>Item</b></td><td><b>Available Qty.</b></td><td><b>Requested Qty.</b></td><td><b>Remarks</b></td><td><b>Remove</b></td></tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        int num2 = 1;
        foreach (DataRow dataRow in dataRowArray)
        {
          if (dataRow["status"].ToString() != "0")
          {
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<td>" + (object) num2 + "</td>");
            stringBuilder.Append("<td>" + dataRow["item_type"].ToString() + "</td>");
            stringBuilder.Append("<td>" + dataRow["item_name"].ToString() + "</td>");
            stringBuilder.Append("<td id='aq_" + dataRow["no"] + "'>" + dataRow["available_qty"].ToString() + "</td>");
            stringBuilder.Append("<td><input type='text' class='resQty'  value='" + dataRow["accepted_qty"].ToString() + "' id='id_" + dataRow["no"] + "'  name='id_" + dataRow["no"] + "' required='required'  onkeypress=' return validate(event)' onchange='checkQty(this," + dataRow["available_qty"].ToString() + ")' /><br/>");
            if (Convert.ToDouble(dataRow["accepted_qty"]) > Convert.ToDouble(dataRow["available_qty"]))
              stringBuilder.Append("<span style='color:red;' id='err_" + dataRow["no"] + "' >Quantity is more than the available quantity</span></td>");
            else
              stringBuilder.Append("<span style='color:red;' id='err_" + dataRow["no"] + "' ></span></td>");
            stringBuilder.Append("<td><input type='text'  value='" + dataRow["remarks"].ToString() + "' id='rem_" + dataRow["no"] + "' name='rem_" + dataRow["no"] + "' /></td>");
            stringBuilder.Append("<td><input type='image' ID='btnRemove_resource' runat='server' src='" + this.site_full_path + "assets/img/btn_remove.png'    onclick='javascript:delete_resource(this,\"" + dataRow["no"].ToString() + "\");'  /></td>");
            stringBuilder.Append("</tr>");
            ++num1;
            ++num2;
          }
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
    }
    this.resourcesHtml = stringBuilder.ToString();
    this.hdn_number_of_items.Value = num1.ToString();
  }

  protected void link_attachment_Click(object sender, EventArgs e)
  {
    try
    {
      if (!(this.hdn_resource_booking_id.Value != ""))
        return;
      long res_booking_id = 0;
      try
      {
        res_booking_id = Convert.ToInt64(this.hdn_resource_booking_id.Value);
      }
      catch
      {
      }
      if (res_booking_id <= 0L)
        return;
      DataSet resourceBookingId = this.resapi.get_resource_document_by_resource_booking_id(res_booking_id, this.current_user.account_id);
      if (!this.utilities.isValidDataset(resourceBookingId))
        return;
      DataRow row = resourceBookingId.Tables[0].Rows[0];
      byte[] buffer = (byte[]) row["binary_Data"];
      this.Response.Clear();
      this.Response.ContentType = row["document_type"].ToString();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.link_attachment.Text);
      this.Response.Buffer = true;
      this.Response.Clear();
      this.Response.BinaryWrite(buffer);
      this.Response.End();
      this.Response.Flush();
      this.link_attachment.Text = resourceBookingId.Tables[0].Rows[0]["document_name"].ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void link_attachment_delete_Click(object sender, EventArgs e)
  {
    try
    {
      string commandArgument = ((LinkButton) sender).CommandArgument;
      long res_doc_id = 0;
      try
      {
        res_doc_id = Convert.ToInt64(commandArgument);
      }
      catch
      {
      }
      if (res_doc_id <= 0L)
        return;
      DataSet resourceDocumentById = this.resapi.get_resource_document_by_id(res_doc_id, this.current_user.account_id);
      if (!this.utilities.isValidDataset(resourceDocumentById))
        return;
      DataRow row = resourceDocumentById.Tables[0].Rows[0];
      resource_document resourceDocument = new resource_document();
      resourceDocument.resource_document_id = Convert.ToInt64(commandArgument);
      resourceDocument.account_id = this.current_user.account_id;
      resourceDocument.modified_by = this.current_user.user_id;
      resourceDocument.record_id = new Guid(row["record_id"].ToString());
      if (this.resapi.delete_resource_document(resourceDocument).resource_document_id <= 0L)
        return;
      this.divUpload.Visible = true;
      this.divAttachments.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("../default.aspx");

  private bool validate_qty(DataTable items_table)
  {
    foreach (DataRow row in (InternalDataCollectionBase) items_table.Rows)
    {
      if (row["status"].ToString() == "1")
      {
        string name = "id_" + row["no"];
        double num;
        try
        {
          num = Convert.ToDouble(this.Request.Form.Get(name));
        }
        catch
        {
          num = 0.0;
        }
        if (Convert.ToDouble(num) > Convert.ToDouble(row["available_qty"]))
        {
          this.lbl_error_message.Text = "Entered quantity is more than the available quantity. Please correct it before proceeding to save.";
          this.lbl_error_message.Visible = true;
          return false;
        }
      }
    }
    return true;
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    DataTable dataTable = (DataTable) this.ViewState["dates_table"];
    DataTable items_table = (DataTable) this.ViewState["items_table"];
    if (dataTable.Rows.Count >= 0)
    {
      bool flag = false;
      foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
      {
        if (row["status"].ToString() == "1")
          flag = true;
      }
      if (!flag)
      {
        this.lbl_error_message.Text = "There are no dates selected. Please select dates and choose items before saving.";
        this.lbl_error_message.Visible = true;
        return;
      }
    }
    if (items_table.Rows.Count >= 0)
    {
      bool flag = false;
      foreach (DataRow row in (InternalDataCollectionBase) items_table.Rows)
      {
        if (row["status"].ToString() == "1")
          flag = true;
      }
      if (!flag)
      {
        this.lbl_error_message.Text = "There are no items selected. Please choose items before saving.";
        this.lbl_error_message.Visible = true;
        return;
      }
    }
    if (!this.validate_qty(items_table))
      return;
    foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
    {
      DataRow[] dataRowArray = items_table.Select("date_no='" + row["no"].ToString() + "'");
      resource_booking resourceBooking = new resource_booking();
      try
      {
        resourceBooking.asset_booking_id = Convert.ToInt64(row["id"]);
      }
      catch
      {
        resourceBooking.asset_booking_id = 0L;
      }
      try
      {
        resourceBooking.booked_for_id = Convert.ToInt64(this.Request.Form[this.hfUserId.UniqueID]);
      }
      catch
      {
        resourceBooking.booked_for_id = this.current_user.user_id;
      }
      resourceBooking.book_from = Convert.ToDateTime(row["from_date"]);
      resourceBooking.book_to = Convert.ToDateTime(row["to_date"]);
      try
      {
        resourceBooking.resource_booking_id = Convert.ToInt64(dataRowArray[0]["id"]);
      }
      catch
      {
        resourceBooking.resource_booking_id = 0L;
      }
      resourceBooking.booking_type = Convert.ToInt32(this.hdn_booking_type.Value);
      resourceBooking.account_id = this.current_user.account_id;
      resourceBooking.created_by = this.current_user.user_id;
      resourceBooking.created_on = this.current_timestamp;
      resourceBooking.email = this.txt_email.Text;
      resourceBooking.item_id = 0L;
      resourceBooking.layout_id = 0L;
      resourceBooking.modified_by = this.current_user.user_id;
      resourceBooking.modified_on = this.current_timestamp;
      resourceBooking.module_name = this.str_resource_module;
      resourceBooking.purpose = this.txt_purpose.Text;
      resourceBooking.record_id = Guid.NewGuid();
      resourceBooking.remarks = this.txtRemarks.Text;
      resourceBooking.requested_by_id = this.current_user.user_id;
      resourceBooking.status = Convert.ToInt32(row["status"]);
      resourceBooking.venue = this.txt_location.Text;
      resource_booking resource_book = this.resapi.update_resource_booking(resourceBooking);
      if (resource_book.resource_booking_id > 0L)
        this.saveAttachments(resource_book.resource_booking_id);
      List<resource_booking_item> resourceBookingItemList = new List<resource_booking_item>();
      if (resource_book.resource_booking_id > 0L)
      {
        foreach (DataRow dataRow in dataRowArray)
        {
          resource_booking_item resourceBookingItem1 = new resource_booking_item();
          resourceBookingItem1.account_id = this.current_user.account_id;
          resourceBookingItem1.created_by = this.current_user.user_id;
          resourceBookingItem1.created_on = this.current_timestamp;
          resourceBookingItem1.modified_by = this.current_user.user_id;
          resourceBookingItem1.modified_on = this.current_timestamp;
          resourceBookingItem1.module_name = this.str_resource_module;
          resourceBookingItem1.other_remarks = "";
          resourceBookingItem1.record_id = Guid.NewGuid();
          resourceBookingItem1.req_price = 0.0;
          resourceBookingItem1.resource_id = Convert.ToInt64(dataRow["item_id"]);
          string name1 = "rem_" + dataRow["no"];
          string str = "";
          try
          {
            str = this.Request.Form.Get(name1).ToString();
          }
          catch
          {
          }
          resourceBookingItem1.requestor_remakrs = str;
          resourceBookingItem1.resource_booking_id = resource_book.resource_booking_id;
          resourceBookingItem1.resource_booking_item_id = Convert.ToInt64(dataRow["resource_booking_item_id"]);
          resourceBookingItem1.status = Convert.ToInt32(dataRow["status"]);
          resourceBookingItem1.accepted_price = 0.0;
          string name2 = "id_" + dataRow["no"];
          double num;
          try
          {
            num = Convert.ToDouble(this.Request.Form.Get(name2));
          }
          catch
          {
            num = Convert.ToDouble(dataRow["accepted_qty"]);
          }
          if (resourceBookingItem1.resource_booking_item_id > 0L)
          {
            resourceBookingItem1.accepted_qty = num;
            resourceBookingItem1.req_qty = Convert.ToDouble(dataRow["requested_qty"]);
          }
          else
          {
            resourceBookingItem1.accepted_qty = num;
            resourceBookingItem1.req_qty = num;
          }
          resource_booking_item resourceBookingItem2 = this.resapi.update_resource_booking_item(resourceBookingItem1);
          if (resourceBookingItem2.resource_booking_item_id > 0L)
            resourceBookingItemList.Add(resourceBookingItem2);
        }
        asset_booking assetBooking = new asset_booking();
        asset asset1 = new asset();
        asset_booking book;
        asset asset2;
        if (resource_book.asset_booking_id > 0L)
        {
          book = this.bookings.get_booking(resource_book.asset_booking_id, resource_book.account_id);
          asset2 = this.assets.get_asset(book.asset_id, book.account_id);
        }
        else
        {
          book = (asset_booking) null;
          asset2 = (asset) null;
        }
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          this.bookingsbl.email_resource_owners(book, asset2, resource_book);
      }
    }
    this.Response.Redirect("../default.aspx");
  }

  private void email_resource_owner(resource_booking obj, resource_booking_item obj_item)
  {
    try
    {
      DataSet usersByGroup = this.users.get_users_by_group(this.resapi.get_resource_item_obj(obj_item.resource_id, this.current_user.account_id, this.str_resource_module).owner_group_id, this.current_user.account_id);
      string str1 = "";
      if (this.utilities.isValidDataset(usersByGroup))
      {
        foreach (DataRow row in (InternalDataCollectionBase) usersByGroup.Tables[0].Rows)
          str1 = str1 + row["email"].ToString() + ";";
      }
      if (!(str1 != ""))
        return;
      string str2 = str1.Trim().Substring(0, str1.Length - 1);
      template template1 = new template();
      template template2 = this.tapi.get_template("email_resource_owner_request", this.current_user.account_id);
      string body = this.replaceTemplate_resource_owner(template2.content_data, obj_item, obj);
      string title = template2.title;
      string cc = "";
      string bcc = "";
      string to = str2;
      email email = new email();
      if (!Convert.ToBoolean(this.current_account.properties["send_email"]))
        return;
      this.utilities.sendEmail(bcc, body, title, cc, to, obj.record_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Send email to resource owner - Error -> ", ex);
    }
  }

  private string replaceTemplate_resource_owner(
    string content,
    resource_booking_item objRes,
    resource_booking obj)
  {
    string str = content;
    try
    {
      str = str.Replace("[FULL NAME]", this.txtBookedFor.Text);
      str = str.Replace("[PURPOSE]", obj.purpose);
      str = str.Replace("[BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
      str = str.Replace("[BOOKED STATUS]", this.bookings.get_status((long) obj.status));
      str = str.Replace("[REQUESTED BY]", this.lbl_requestedBy.Value);
      str = str.Replace("[REQUESTED ON]", obj.created_on.ToString(api_constants.display_datetime_format));
      str = str.Replace("[EMAILS]", obj.email);
      str = str.Replace("[RESOURCE DETAILS]", this.get_resourceItem(objRes));
      str = str.Replace("[SITE_FULL_PATH]", this.site_full_path);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return str;
  }

  private string get_resourceItem(resource_booking_item obj)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataSet dataSet = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_items");
      if (dataSet == null)
      {
        dataSet = this.resapi.get_resource_items(this.current_user.account_id, this.str_resource_module);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_items", (object) dataSet);
      }
      DataRow[] dataRowArray = dataSet.Tables[0].Select("item_id=" + (object) obj.resource_id);
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Resource Type</th>");
      stringBuilder.Append("<th class='hidden-480'>Resource Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Quantity</th>");
      stringBuilder.Append("<th class='hidden-480'>Remarks</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr class='odd gradeX'>");
      stringBuilder.Append("<td>" + dataRowArray[0]["resource_type"].ToString() + "</td>");
      stringBuilder.Append("<td>" + dataRowArray[0]["name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + (object) obj.req_qty + "</td>");
      stringBuilder.Append("<td>" + obj.requestor_remakrs + "</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return stringBuilder.ToString();
  }

  private void email_requestor(resource_booking obj)
  {
    try
    {
      template template = this.tapi.get_template("email_resource_request_requestor", this.current_user.account_id);
      string body = this.replaceTemplate_resource_requestor(template.content_data, obj);
      string subject = template.title + " - " + obj.purpose;
      string cc = "";
      string bcc = "";
      string email1 = obj.email;
      email email2 = new email();
      if (!Convert.ToBoolean(this.current_account.properties["send_email"]))
        return;
      this.utilities.sendEmail(bcc, body, subject, cc, email1, obj.record_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Send email to resource requestor - Error -> ", ex);
    }
  }

  private string replaceTemplate_resource_requestor(string content, resource_booking obj)
  {
    string str = content;
    try
    {
      str = str.Replace("[FULL NAME]", this.txtBookedFor.Text);
      str = str.Replace("[PURPOSE]", obj.purpose);
      str = str.Replace("[BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
      str = str.Replace("[BOOKED STATUS]", this.bookings.get_status((long) obj.status));
      str = str.Replace("[REQUESTED BY]", this.lbl_requestedBy.Value);
      str = str.Replace("[REQUESTED ON]", obj.created_on.ToString(api_constants.display_datetime_format));
      str = str.Replace("[EMAILS]", obj.email);
      str = str.Replace("[RESOURCE DETAILS]", this.get_resourcelist(obj));
      str = str.Replace("[SITE_FULL_PATH]", this.site_full_path);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return str;
  }

  private string get_resourcelist(resource_booking obj)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataSet dataSet = new DataSet();
      DataSet ds = obj.asset_booking_id <= 0L ? this.resapi.get_resource_bookings_by_resource_booking_id(obj.account_id, obj.resource_booking_id, this.str_resource_module) : this.resapi.get_resource_bookings_items_by_asset_booking_id(obj.asset_booking_id, this.current_user.account_id, "", "", this.str_resource_module);
      if (this.utilities.isValidDataset(ds))
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' width='100%'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>S/No.</th>");
        stringBuilder.Append("<th class='hidden-480'>Resource Type</th>");
        stringBuilder.Append("<th class='hidden-480'>Resource Name</th>");
        stringBuilder.Append("<th class='hidden-480'>Quantity</th>");
        stringBuilder.Append("<th class='hidden-480'>Remarks</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        int num = 1;
        foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + num.ToString() + "</td>");
          stringBuilder.Append("<td>" + row["value"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["requested_qty"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["requestor_remarks"].ToString() + "</td>");
          stringBuilder.Append("</tr>");
          ++num;
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return stringBuilder.ToString();
  }

  private void saveAttachments(long res_bk_id)
  {
    try
    {
      if (res_bk_id <= 0L || !this.upload_attachment.HasFile)
        return;
      resource_document resourceDocument = new resource_document();
      resourceDocument.account_id = this.current_user.account_id;
      resourceDocument.attachment_type = "resource_booking";
      int contentLength = this.upload_attachment.PostedFile.ContentLength;
      try
      {
        setting setting = this.settings.get_setting("upload_document_size", this.current_user.account_id);
        if (Convert.ToInt32(setting.value) < contentLength)
        {
          this.lblErrorUpload.Text = Resources.fbs.error_upload_file_size + setting.value.ToString();
          return;
        }
        this.lblErrorUpload.Text = "";
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Error -> ", ex);
      }
      try
      {
        setting setting = this.settings.get_setting("upload_document_type", this.current_user.account_id);
        string str = new FileInfo(this.upload_attachment.PostedFile.FileName).Extension.Substring(1);
        if (setting.value.Contains(str))
        {
          this.lblErrorUpload.Text = "";
        }
        else
        {
          this.lblErrorUpload.Text = Resources.fbs.resource_document_valid + setting.value.ToString();
          return;
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Error -> ", ex);
      }
      byte[] numArray = new byte[contentLength];
      byte[] fileBytes = this.upload_attachment.FileBytes;
      resourceDocument.document_name = this.upload_attachment.FileName;
      resourceDocument.document_size = new int?(contentLength);
      resourceDocument.document_type = this.upload_attachment.PostedFile.ContentType;
      resourceDocument.binary_data = fileBytes;
      resourceDocument.created_by = this.current_user.user_id;
      resourceDocument.document_meta = "";
      resourceDocument.modified_by = this.current_user.user_id;
      resourceDocument.record_id = Guid.NewGuid();
      resourceDocument.resource_document_id = 0L;
      resourceDocument.resource_item_id = res_bk_id;
      this.resapi.update_resource_document(resourceDocument);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_remove_Click(object sender, ImageClickEventArgs e)
  {
    int itemIndex = ((RepeaterItem) ((Control) sender).NamingContainer).ItemIndex;
    DataTable dataTable1 = (DataTable) this.ViewState["dates_table"];
    DataTable items_table = (DataTable) this.ViewState["items_table"];
    DataRow dataRow = (DataRow) null;
    int num = 0;
    foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
    {
      if (!(row["status"].ToString() == "0"))
      {
        if (num == itemIndex)
        {
          row["status"] = (object) "0";
          dataRow = row;
          dataTable1.AcceptChanges();
        }
        ++num;
      }
    }
    DataTable dataTable2 = items_table.Clone();
    foreach (DataRow row in (InternalDataCollectionBase) items_table.Rows)
    {
      if (row["from_date"].ToString() != dataRow["from_date"].ToString())
      {
        dataTable2.Rows.Add(row.ItemArray);
        dataTable2.AcceptChanges();
      }
      else
      {
        row["status"] = (object) "0";
        dataTable2.Rows.Add(row.ItemArray);
        dataTable2.AcceptChanges();
      }
    }
    this.ViewState["dates_table"] = (object) dataTable1;
    this.ViewState["items_table"] = (object) items_table;
    this.bind_dates_grid(dataTable1);
    bool flag = false;
    foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
    {
      if (row["status"].ToString() == "1")
        flag = true;
    }
    if (flag)
    {
      this.pnl_items.Visible = true;
      this.pnl_items_table.Visible = true;
    }
    else
    {
      this.pnl_items.Visible = false;
      this.pnl_items_table.Visible = false;
    }
    this.populate_resource_html(dataTable1, items_table);
  }

  private void bind_dates_grid(DataTable table)
  {
    DataTable dataTable = table.Clone();
    foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
    {
      if (row["status"].ToString() == "1")
        dataTable.Rows.Add(row.ItemArray);
      dataTable.AcceptChanges();
    }
    this.hdn_number_of_dates.Value = dataTable.Rows.Count.ToString();
    this.gridview_add_dates.DataSource = (object) dataTable;
    this.gridview_add_dates.DataBind();
    this.gridview_add_dates.Visible = true;
  }
}
