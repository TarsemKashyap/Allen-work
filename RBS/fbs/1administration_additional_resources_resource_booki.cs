// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_resource_booking
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
using System.Web;
using System.Web.Profile;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_additional_resources_resource_booking : 
  fbs_base_page,
  IRequiresSessionState
{
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected TextBox txt_purpose;
  protected HtmlGenericControl errorpurpose;
  protected TextBox txtBookedFor;
  protected Panel myPanel;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hfUserId;
  protected HtmlGenericControl errorbookedfor;
  protected TextBox txt_email;
  protected HtmlImage img_loading;
  protected RegularExpressionValidator RE_email;
  protected HtmlGenericControl erroremail;
  protected HtmlInputText lbl_requestedBy;
  protected TextBox txt_location;
  protected HtmlGenericControl errorLocation;
  protected TextBox txtRemarks;
  protected TextBox txt_startDate;
  protected DropDownList ddl_StartTime;
  protected DropDownList ddl_EndTime;
  protected Button btn_add_date;
  protected Button btn_remove_date;
  protected HtmlGenericControl span_duplicate;
  protected HtmlTable tblDateSelectAdd;
  protected Repeater gridview_add_dates;
  protected Label lblError;
  protected DropDownList ddl_res_type;
  protected DropDownList ddl_res_name;
  protected TextBox txtAvailableQty;
  protected HtmlTableRow tr_avail_qty;
  protected TextBox txtQty;
  protected HtmlTableRow tr_qty;
  protected TextBox txt_requestor_remarks;
  protected HtmlTableRow tr_remarks;
  protected Image img_res;
  protected HtmlTableRow tr_img;
  protected Literal litResource;
  protected HtmlTableRow tr_template;
  protected Button btn_addResource;
  protected FileUpload upload_attachment;
  protected Label lblErrorUpload;
  protected HtmlGenericControl divUpload;
  protected LinkButton lnkAttachment;
  protected LinkButton lnkDelete;
  protected Table tblAttachment;
  protected HtmlGenericControl divAttachments;
  protected HtmlAnchor anchor_download;
  protected HtmlTableRow download_lnk;
  protected CheckBox chk_terms;
  protected HtmlGenericControl errorTerms;
  protected Button btn_cancel;
  protected Button btn_submit;
  protected HiddenField hdnMeetingBookingId;
  protected HiddenField hdnResourceBookingId;
  protected HiddenField hdnBookingType;
  protected HiddenField hdn_selecteddatecount;
  protected HiddenField hdn_selected_Date_value;
  protected HiddenField hdnDelResId;
  public string htmltable;
  public string accountid = "";
  private long assetBkId;
  private long resBkId;
  private DataSet user_item_data;
  private List<long> allowed_items;
  private bool is_admin;
  public string resourcesHtml;
  private DataSet resSetDs;
  private bool isRepeat;
  private DataSet resItems;
  private string imagepathfbs;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.imagepathfbs = this.site_full_path + "assets/img/tts_Logo.png";
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    this.accountid = this.current_user.account_id.ToString();
    foreach (user_group userGroup in this.current_user.groups.Values)
    {
      if (userGroup.group_type == 1)
        this.is_admin = true;
      else if (!this.is_admin)
        this.is_admin = false;
    }
    if (!this.is_admin)
      this.redirect_unauthorized();
    this.resSetDs = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
    if (this.resSetDs == null)
    {
      this.resSetDs = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) this.resSetDs);
    }
    this.resItems = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_items");
    if (this.resItems == null)
    {
      this.resItems = this.resapi.get_resource_items(this.current_user.account_id, this.str_resource_module);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_items", (object) this.resItems);
    }
    this.get_allowed_items();
    if (this.IsPostBack)
      return;
    try
    {
      this.resourcesHtml = "";
      this.Session.Remove("added_resources");
      this.lbl_requestedBy.Value = this.current_user.full_name;
      try
      {
        this.assetBkId = Convert.ToInt64(this.Request.QueryString["booking_id"]);
      }
      catch
      {
        this.assetBkId = 0L;
      }
      try
      {
        this.resBkId = Convert.ToInt64(this.Request.QueryString["resource_booking_id"]);
      }
      catch
      {
        this.resBkId = 0L;
      }
      try
      {
        if (!string.IsNullOrEmpty(this.Request.QueryString["isrepeat"]))
          this.isRepeat = Convert.ToBoolean(this.Request.QueryString["isrepeat"]);
      }
      catch
      {
        this.isRepeat = false;
      }
      this.initialize_start_end_time();
      this.populate_res_type();
      if (this.assetBkId > 0L)
        this.populate_asset_booking_data();
      else if (this.resBkId > 0L)
        this.populate_resource_booking_data();
      else if (this.assetBkId == 0L)
        this.hdnBookingType.Value = this.resSetDs.Tables[0].Select("value='Additional Resources without asset booking'")[0]["setting_id"].ToString();
      this.getUploadedAttachments();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  private void get_allowed_items()
  {
    this.allowed_items = new List<long>();
    this.user_item_data = this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, this.str_resource_module);
    foreach (DataRow row in (InternalDataCollectionBase) this.user_item_data.Tables[0].Rows)
      this.allowed_items.Add(Convert.ToInt64(row["item_id"]));
  }

  private void initialize_start_end_time()
  {
    this.utilities.Populate_Time(this.ddl_StartTime, this.current_timestamp);
    this.utilities.Populate_Time(this.ddl_EndTime, this.current_timestamp);
    this.txt_startDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
    if (Convert.ToInt32(this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh")) <= 9)
      this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("h:mm tt");
    else
      this.ddl_StartTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp).ToString("hh:mm tt");
    if (Convert.ToInt32(this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh")) <= 9)
      this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("h:mm tt");
    else
      this.ddl_EndTime.SelectedValue = this.utilities.TimeRoundUp(this.current_timestamp.AddMinutes(15.0)).ToString("hh:mm tt");
  }

  private void populate_asset_booking_data()
  {
    this.tblDateSelectAdd.Visible = false;
    this.hdnBookingType.Value = this.resSetDs.Tables[0].Select("value='Additional Resources with asset booking'")[0]["setting_id"].ToString();
    this.hdnMeetingBookingId.Value = this.assetBkId.ToString();
    DataSet byAssetBookingId = this.resapi.get_resource_bookings_items_by_asset_booking_id(this.assetBkId, this.current_user.account_id, "", "", this.str_resource_module);
    if (this.utilities.isValidDataset(byAssetBookingId))
    {
      this.hdnResourceBookingId.Value = byAssetBookingId.Tables[0].Rows[0]["resource_booking_id"].ToString();
      this.bind_booking_data(byAssetBookingId);
    }
    else
    {
      this.hdnResourceBookingId.Value = "0";
      try
      {
        DataSet dataSet = this.bookings.get_booking_assets(this.assetBkId, this.current_user.account_id);
        DataRow row1 = dataSet.Tables[0].Rows[0];
        this.lbl_requestedBy.Value = row1["requested_by_name"].ToString();
        this.txt_purpose.Text = row1["purpose"].ToString();
        this.txt_email.Text = row1["email"].ToString();
        this.txt_location.Text = row1["name"].ToString();
        this.txtBookedFor.Text = row1["booked_for_name"].ToString();
        this.hfUserId.Value = row1["booked_for"].ToString();
        this.txtRemarks.Text = row1["remarks"].ToString();
        this.txt_purpose.Enabled = false;
        this.txt_email.Enabled = false;
        this.txt_location.Enabled = false;
        this.txtBookedFor.Enabled = false;
        this.txtRemarks.Enabled = false;
        if (this.isRepeat)
          dataSet = this.bookings.get_repeat_booking_assets(this.current_user.account_id, new Guid(row1["repeat_reference_id"].ToString()));
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("from_date");
        dataTable.Columns.Add("to_date");
        dataTable.AcceptChanges();
        foreach (DataRow row2 in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
        {
          DataRow row3 = dataTable.NewRow();
          row3["from_date"] = row2["book_from"];
          row3["to_date"] = row2["book_to"];
          dataTable.Rows.Add(row3);
          dataTable.AcceptChanges();
        }
        this.popuate_date(new DataSet()
        {
          Tables = {
            dataTable
          }
        });
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Resource Booking :  get_booking_assets :  Error->", ex);
      }
    }
  }

  private void populate_resource_booking_data()
  {
    DataSet resourceBookingId = this.resapi.get_resource_bookings_by_resource_booking_id(this.current_user.account_id, this.resBkId, this.str_resource_module);
    this.hdnResourceBookingId.Value = this.resBkId.ToString();
    this.tblDateSelectAdd.Visible = true;
    if (!this.utilities.isValidDataset(resourceBookingId))
      return;
    if (resourceBookingId.Tables[0].Rows[0]["asset_booking_id"].ToString() == "0")
    {
      this.hdnBookingType.Value = this.resSetDs.Tables[0].Select("value='Additional Resources without asset booking'")[0]["setting_id"].ToString();
      this.hdnMeetingBookingId.Value = "0";
    }
    else
    {
      this.hdnBookingType.Value = this.resSetDs.Tables[0].Select("value='Additional Resources with asset booking'")[0]["setting_id"].ToString();
      this.hdnMeetingBookingId.Value = resourceBookingId.Tables[0].Rows[0]["asset_booking_id"].ToString();
      this.tblDateSelectAdd.Visible = false;
      this.txt_purpose.Enabled = false;
      this.txt_email.Enabled = false;
      this.txt_location.Enabled = false;
      this.txtBookedFor.Enabled = false;
      this.txtRemarks.Enabled = false;
    }
    this.bind_booking_data(resourceBookingId);
  }

  private void populate_booking_details(DataSet ds)
  {
    DataRow row = ds.Tables[0].Rows[0];
    this.lbl_requestedBy.Value = row["requested_by_name"].ToString();
    this.txt_purpose.Text = row["purpose"].ToString();
    this.txt_email.Text = row["email"].ToString();
    this.txt_location.Text = row["venue"].ToString();
    this.txtBookedFor.Text = row["booked_for"].ToString();
    this.hfUserId.Value = row["booked_for_id"].ToString();
    this.txtRemarks.Text = row["remarks"].ToString();
  }

  private void popuate_date(DataSet ds)
  {
    DataTable dataTable = new DataTable();
    dataTable.Columns.Add("Sno");
    dataTable.Columns.Add("From");
    dataTable.Columns.Add("To");
    dataTable.AcceptChanges();
    if (this.ViewState["Select_Dates_Details"] != null)
    {
      foreach (DataRow row in (InternalDataCollectionBase) ((DataTable) this.ViewState["Select_Dates_Details"]).Rows)
        dataTable.Rows.Add(row["Sno"], row["From"], row["To"]);
    }
    foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
    {
      DateTime dateTime1 = Convert.ToDateTime(row["from_date"]);
      DateTime dateTime2 = Convert.ToDateTime(row["to_date"]);
      if (dataTable.Select("From >= #" + dateTime1.ToString(api_constants.display_datetime_format) + "# and To <= #" + dateTime2.ToString(api_constants.display_datetime_format) + "# or To >= #" + dateTime1.ToString(api_constants.display_datetime_format) + "# and From <= #" + dateTime2.ToString(api_constants.display_datetime_format) + "#").Length <= 0 && dateTime1.ToString(api_constants.display_datetime_format) != dateTime2.ToString(api_constants.display_datetime_format))
        dataTable.Rows.Add((object) 1, (object) dateTime1.ToString(api_constants.display_datetime_format), (object) dateTime2.ToString(api_constants.display_datetime_format));
    }
    this.gridview_add_dates.DataSource = (object) dataTable;
    this.gridview_add_dates.DataBind();
    this.ViewState["Select_Dates_Details"] = (object) dataTable;
    this.gridview_add_dates.Visible = true;
    if (this.gridview_add_dates.Items.Count != 1)
      return;
    this.gridview_add_dates.Items[0].FindControl("btn_remove").Visible = false;
  }

  private void bind_booking_data(DataSet ds)
  {
    this.populate_booking_details(ds);
    this.popuate_date(ds);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> selectedDates = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      int num = 0;
      this.txt_startDate.Text.Split(',');
      this.hdn_selected_Date_value.Value = "";
      if (this.Session["SelectedDates"] != null)
        this.Session.Remove("SelectedDates");
      foreach (RepeaterItem repeaterItem in this.gridview_add_dates.Items)
      {
        ++num;
        Label control1 = (Label) repeaterItem.FindControl("lbl_from");
        Label control2 = (Label) repeaterItem.FindControl("lbl_to");
        dictionary1.Add(control1.Text, control2.Text);
        dictionary2.Add(Convert.ToString(num), control1.Text);
        selectedDates.Clear();
        selectedDates.Add(Convert.ToDateTime(Convert.ToString(control1.Text)).ToString(api_constants.display_datetime_format), Convert.ToDateTime(control2.Text).ToString(api_constants.display_datetime_format));
        stringBuilder.Append(this.populate_resources(ds, selectedDates, Convert.ToDateTime(control1.Text).ToString("dd-MMM-yyy"), repeaterItem.ItemIndex, (repeaterItem.ItemIndex + 1).ToString() + "--" + Convert.ToDateTime(control1.Text).ToString(api_constants.display_datetime_format_short), control1.Text + " To " + control2.Text));
      }
      this.hdn_selecteddatecount.Value = num.ToString();
      this.resourcesHtml = stringBuilder.ToString();
      this.Session.Add("SelectedDates", (object) dictionary1);
      this.Session.Add("Resource_Booking", (object) dictionary2);
      this.Session.Add("page", (object) "resourcebookings");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error->", ex);
    }
  }

  public string populate_resources(
    DataSet DsBk,
    Dictionary<string, string> selectedDates,
    string date,
    int i,
    string name,
    string headername)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataTable dtRes = new DataTable();
      dtRes.Columns.Add("resource_id");
      dtRes.Columns.Add("resource_booking_id");
      dtRes.Columns.Add("resource_booking_item_id");
      dtRes.Columns.Add("value");
      dtRes.Columns.Add(nameof (name));
      dtRes.Columns.Add("available_qty");
      dtRes.Columns.Add("req_quantity");
      dtRes.Columns.Add("acc_quantity");
      dtRes.Columns.Add("requestor_remarks");
      dtRes.Columns.Add("asset_booking_id");
      dtRes.Columns.Add("from_date");
      dtRes.Columns.Add("to_date");
      dtRes.Columns.Add("row_id");
      dtRes.AcceptChanges();
      foreach (string key in selectedDates.Keys)
      {
        DataTable table = DsBk.Tables[0];
        string filterExpression = "from_date='" + key + "' and to_date='" + selectedDates[key] + "'";
        foreach (DataRow dataRow in table.Select(filterExpression))
        {
          int num1;
          try
          {
            num1 = Convert.ToInt32(this.resSetDs.Tables[0].Select("parameter='Advance Notice Period' and parent_id=" + dataRow["resource_id"].ToString())[0]["value"]);
          }
          catch (Exception ex)
          {
            num1 = 0;
          }
          bool flag1 = true;
          if (num1 > 0 && (Convert.ToDateTime(key) - this.current_timestamp).TotalDays < (double) num1)
            flag1 = false;
          if (flag1)
          {
            DataSet resourceBookingItems = this.resapi.get_resource_booking_items(Convert.ToInt64(dataRow["resource_id"]), this.current_user.account_id, key, selectedDates[key], this.str_resource_module);
            if (this.utilities.isValidDataset(resourceBookingItems))
            {
              foreach (DataRow row1 in (InternalDataCollectionBase) resourceBookingItems.Tables[0].Rows)
              {
                double num2;
                try
                {
                  num2 = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]) - Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["accepted_qty"]);
                }
                catch (Exception ex)
                {
                  num2 = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]);
                }
                bool flag2 = true;
                foreach (DataRow row2 in (InternalDataCollectionBase) dtRes.Rows)
                {
                  if (row2["resource_id"].ToString() == dataRow["resource_id"].ToString() && row2["from_date"].ToString() == key && row2["from_date"].ToString() == selectedDates[key])
                    flag2 = false;
                }
                if (flag2)
                {
                  DataRow row3 = dtRes.NewRow();
                  row3["resource_id"] = (object) dataRow["resource_id"].ToString();
                  row3["resource_booking_id"] = (object) dataRow["resource_booking_id"].ToString();
                  row3["resource_booking_item_id"] = (object) dataRow["resource_booking_item_id"].ToString();
                  row3["value"] = (object) dataRow["value"].ToString();
                  row3[nameof (name)] = (object) dataRow[nameof (name)].ToString();
                  row3["available_qty"] = (object) num2;
                  row3["req_quantity"] = (object) dataRow["requested_qty"].ToString();
                  row3["acc_quantity"] = (object) dataRow["accepted_qty"].ToString();
                  row3["requestor_remarks"] = (object) dataRow["requestor_remarks"].ToString();
                  row3["asset_booking_id"] = (object) dataRow["asset_booking_id"].ToString();
                  row3["from_date"] = (object) key;
                  row3["to_date"] = (object) selectedDates[key];
                  row3["row_id"] = (object) Guid.NewGuid();
                  dtRes.Rows.Add(row3);
                }
              }
            }
          }
        }
      }
      if (dtRes.Rows.Count > 0)
      {
        this.Session.Add("added_resources", (object) dtRes);
        this.htmltable = this.getResourceHtml(selectedDates, dtRes, i, name);
        stringBuilder.Append("<div class='accordion-group'>");
        stringBuilder.Append("<div class='accordion-heading'>");
        stringBuilder.Append("<a class='accordion-toggle collapsed' data-toggle='collapse' data-parent='#accordion2' href='#" + name + "'>");
        stringBuilder.Append("<i class='icon-plus'></i>");
        stringBuilder.Append("  " + headername);
        stringBuilder.Append("</a>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div id='" + name + "' class='accordion-body collapse in'>");
        stringBuilder.Append("<div class='accordion-inner'>");
        stringBuilder.Append(this.htmltable.ToString());
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
    return stringBuilder.ToString();
  }

  private void populate_res_type()
  {
    this.ddl_res_type.Items.Clear();
    this.ddl_res_type.Items.Add(new ListItem()
    {
      Text = "select",
      Value = "select"
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
      if (this.ddl_res_type.SelectedItem.Text != "Template")
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resource_items_by_item_type_id(Convert.ToInt64(this.ddl_res_type.SelectedItem.Value), this.current_user.account_id, this.str_resource_module).Tables[0].Rows)
          this.ddl_res_name.Items.Add(new ListItem()
          {
            Text = row["name"].ToString(),
            Value = row["item_id"].ToString()
          });
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

  protected void btn_add_date_Click(object sender, EventArgs e)
  {
    try
    {
      bool flag = true;
      string text1 = this.ddl_StartTime.SelectedItem.Text;
      string text2 = this.ddl_EndTime.SelectedItem.Text;
      string text3 = this.ddl_StartTime.SelectedItem.Text;
      string text4 = this.ddl_EndTime.SelectedItem.Text;
      string str1 = text1.Substring(0, text1.Length - 2);
      string str2 = text2.Substring(0, text2.Length - 2);
      text3.Substring(text3.Length - 2, 2);
      text4.Substring(text4.Length - 2, 2);
      str1.Split(':');
      str2.Split(':');
      string str3 = Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_StartTime.SelectedItem.Text;
      string str4 = Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_EndTime.SelectedItem.Text;
      DateTime dateTime1 = Convert.ToDateTime(str3);
      DateTime dateTime2 = Convert.ToDateTime(str4);
      if ((dateTime2 - dateTime1).TotalMinutes <= 0.0)
        flag = false;
      if (flag)
      {
        if (this.ViewState["Select_Dates_Details"] == null)
        {
          DataTable dataTable = new DataTable();
          dataTable.Columns.Add("Sno");
          dataTable.Columns.Add("From");
          dataTable.Columns.Add("To");
          dataTable.Rows.Add((object) 1, (object) dateTime1.ToString(api_constants.display_datetime_format), (object) dateTime2.ToString(api_constants.display_datetime_format));
          this.gridview_add_dates.DataSource = (object) dataTable;
          this.gridview_add_dates.DataBind();
          this.ViewState["Select_Dates_Details"] = (object) dataTable;
          this.gridview_add_dates.Visible = true;
        }
        else
        {
          DataTable dataTable1 = new DataTable();
          dataTable1.Columns.Add("Sno");
          dataTable1.Columns.Add("From");
          dataTable1.Columns.Add("To");
          DataTable dataTable2 = (DataTable) this.ViewState["Select_Dates_Details"];
          foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
            dataTable1.Rows.Add(row["Sno"], row["From"], row["To"]);
          string str5 = Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_StartTime.SelectedItem.Text;
          string str6 = Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_EndTime.SelectedItem.Text;
          DateTime dateTime3 = Convert.ToDateTime(str5);
          DateTime dateTime4 = Convert.ToDateTime(str6);
          if (dataTable1.Select("From >= #" + dateTime3.ToString(api_constants.display_datetime_format) + "# and To <= #" + dateTime4.ToString(api_constants.display_datetime_format) + "# or To >= #" + dateTime3.ToString(api_constants.display_datetime_format) + "# and From <= #" + dateTime4.ToString(api_constants.display_datetime_format) + "#").Length <= 0)
          {
            if (dateTime3.ToString(api_constants.display_datetime_format) != dateTime4.ToString(api_constants.display_datetime_format))
            {
              dataTable1.Rows.Add((object) (dataTable2.Rows.Count + 1), (object) dateTime3.ToString(api_constants.display_datetime_format), (object) dateTime4.ToString(api_constants.display_datetime_format));
              this.span_duplicate.Visible = false;
            }
            else
            {
              this.span_duplicate.InnerText = "Start Time and End Time cannot same";
              this.span_duplicate.Visible = true;
            }
          }
          else
          {
            this.span_duplicate.InnerText = Resources.fbs.custombooking_date_alrdy_exsits;
            this.span_duplicate.Visible = true;
          }
          this.gridview_add_dates.DataSource = (object) dataTable1;
          this.gridview_add_dates.DataBind();
          this.ViewState["Select_Dates_Details"] = (object) dataTable1;
          this.gridview_add_dates.Visible = true;
        }
        if (this.gridview_add_dates.Items.Count == 1)
          this.gridview_add_dates.Items[0].FindControl("btn_remove").Visible = false;
        this.Process_Datas();
      }
      else
      {
        this.span_duplicate.InnerText = " To time should greater then  From time";
        this.span_duplicate.Visible = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  protected void btn_remove_click(object sender, EventArgs e)
  {
    try
    {
      if (this.ViewState["Select_Dates_Details"] == null)
        return;
      int itemIndex = ((RepeaterItem) ((Control) sender).NamingContainer).ItemIndex;
      DataTable dataTable = (DataTable) this.ViewState["Select_Dates_Details"];
      if (dataTable.Rows.Count >= 2)
        dataTable.Rows.Remove(dataTable.Rows[itemIndex]);
      if (dataTable.Rows.Count == 0)
      {
        this.gridview_add_dates.DataSource = (object) new DataTable()
        {
          Columns = {
            "From",
            "To"
          }
        };
        this.gridview_add_dates.DataBind();
        this.gridview_add_dates.Visible = false;
      }
      else
      {
        for (int index = 0; index < dataTable.Rows.Count; ++index)
          dataTable.Rows[index]["Sno"] = (object) (index + 1);
        this.gridview_add_dates.DataSource = (object) dataTable;
        this.gridview_add_dates.DataBind();
        this.ViewState["Select_Dates_Details"] = (object) dataTable;
      }
      if (this.gridview_add_dates.Items.Count == 1)
        this.gridview_add_dates.Items[0].FindControl("btn_remove").Visible = false;
      this.Process_Datas();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  protected void btn_remove_date_Click(object sender, EventArgs e)
  {
    try
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = (DataTable) this.ViewState["Select_Dates_Details"];
      for (int index = this.gridview_add_dates.Items.Count - 1; index >= 0; --index)
      {
        CheckBox control = (CheckBox) this.gridview_add_dates.Items[index].FindControl("singleselect");
        if (control.Checked)
        {
          int itemIndex = ((RepeaterItem) control.NamingContainer).ItemIndex;
          if (dataTable2.Rows.Count >= 2)
            dataTable2.Rows.RemoveAt(itemIndex);
        }
      }
      DataTable dataTable3 = dataTable2;
      this.gridview_add_dates.DataSource = (object) dataTable3;
      this.gridview_add_dates.DataBind();
      this.ViewState["Select_Dates_Details"] = (object) dataTable3;
      this.Session["SelectedDates"] = (object) dataTable3;
      if (dataTable3.Rows.Count <= 0)
      {
        this.span_duplicate.Visible = false;
        this.gridview_add_dates.Visible = false;
      }
      if (this.gridview_add_dates.Items.Count == 1)
        this.gridview_add_dates.Items[0].FindControl("btn_remove").Visible = false;
      this.Process_Datas();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  protected void ddl_StartTime_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      string[] strArray = this.ddl_StartTime.SelectedItem.Text.Split(':');
      int int16 = (int) Convert.ToInt16(strArray[0]);
      if (int16 == 11)
      {
        if (strArray[1].IndexOf("AM") > 0)
        {
          strArray[1] = strArray[1].Replace("AM", "PM");
          ++int16;
        }
      }
      else
        ++int16;
      if (int16 > 12)
        int16 -= 12;
      this.ddl_EndTime.SelectedValue = int16.ToString() + ":" + strArray[1];
      this.span_duplicate.InnerText = "";
      this.span_duplicate.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  protected void ddl_res_type_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.populate_resource_names();
    this.txtAvailableQty.Text = "";
    this.txtQty.Text = "";
    this.txt_requestor_remarks.Text = "";
  }

  public void Process_Datas()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> selectedDates = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      int num = 0;
      this.txt_startDate.Text.Split(',');
      this.hdn_selected_Date_value.Value = "";
      if (this.Session["SelectedDates"] != null)
        this.Session.Remove("SelectedDates");
      foreach (RepeaterItem repeaterItem in this.gridview_add_dates.Items)
      {
        ++num;
        Label control1 = (Label) repeaterItem.FindControl("lbl_from");
        Label control2 = (Label) repeaterItem.FindControl("lbl_to");
        dictionary1.Add(control1.Text, control2.Text);
        dictionary2.Add(Convert.ToString(num), control1.Text);
        selectedDates.Clear();
        selectedDates.Add(Convert.ToDateTime(Convert.ToString(control1.Text)).ToString(api_constants.display_datetime_format), Convert.ToDateTime(control2.Text).ToString(api_constants.display_datetime_format));
        if (this.ddl_res_type.SelectedItem.Text != "Template")
        {
          stringBuilder.Append(this.Bind_resources(selectedDates, Convert.ToDateTime(control1.Text).ToString(api_constants.display_datetime_format_short), repeaterItem.ItemIndex, (repeaterItem.ItemIndex + 1).ToString() + "--" + Convert.ToDateTime(control1.Text).ToString(api_constants.display_datetime_format_short), control1.Text + " To " + control2.Text));
        }
        else
        {
          try
          {
            DataSet itemByTemplateId = this.resapi.get_resource_template_item_by_templateId(Convert.ToInt64(this.ddl_res_name.SelectedItem.Value), this.current_user.account_id);
            stringBuilder.Append(this.Bind_resources_template(selectedDates, Convert.ToDateTime(control1.Text).ToString(api_constants.display_datetime_format_short), repeaterItem.ItemIndex, (repeaterItem.ItemIndex + 1).ToString() + "--" + Convert.ToDateTime(control1.Text).ToString(api_constants.display_datetime_format_short), control1.Text + " To " + control2.Text, itemByTemplateId));
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) "Add Resource Template items : Error->", ex);
          }
        }
      }
      this.hdn_selecteddatecount.Value = num.ToString();
      this.resourcesHtml = stringBuilder.ToString();
      this.Session.Add("SelectedDates", (object) dictionary1);
      this.Session.Add("Resource_Booking", (object) dictionary2);
      this.Session.Add("page", (object) "resourcebookings");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error->", ex);
    }
  }

  public string Bind_resources_template(
    Dictionary<string, string> selectedDates,
    string date,
    int i,
    string name,
    string headername,
    DataSet dtRes)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataTable dtRes1 = new DataTable();
      try
      {
        DataTable dataTable = (DataTable) this.Session["added_resources"];
        if (dataTable.Rows.Count > 0)
        {
          dtRes1 = dataTable;
        }
        else
        {
          dtRes1.Columns.Add("resource_id");
          dtRes1.Columns.Add("resource_booking_id");
          dtRes1.Columns.Add("resource_booking_item_id");
          dtRes1.Columns.Add("value");
          dtRes1.Columns.Add(nameof (name));
          dtRes1.Columns.Add("available_qty");
          dtRes1.Columns.Add("req_quantity");
          dtRes1.Columns.Add("acc_quantity");
          dtRes1.Columns.Add("requestor_remarks");
          dtRes1.Columns.Add("asset_booking_id");
          dtRes1.Columns.Add("from_date");
          dtRes1.Columns.Add("to_date");
          dtRes1.Columns.Add("row_id");
          dtRes1.AcceptChanges();
        }
      }
      catch
      {
        dtRes1.Columns.Add("resource_id");
        dtRes1.Columns.Add("resource_booking_id");
        dtRes1.Columns.Add("resource_booking_item_id");
        dtRes1.Columns.Add("value");
        dtRes1.Columns.Add(nameof (name));
        dtRes1.Columns.Add("available_qty");
        dtRes1.Columns.Add("req_quantity");
        dtRes1.Columns.Add("acc_quantity");
        dtRes1.Columns.Add("requestor_remarks");
        dtRes1.Columns.Add("asset_booking_id");
        dtRes1.Columns.Add("from_date");
        dtRes1.Columns.Add("to_date");
        dtRes1.Columns.Add("row_id");
        dtRes1.AcceptChanges();
      }
      foreach (string key in selectedDates.Keys)
      {
        foreach (DataRow row1 in (InternalDataCollectionBase) dtRes.Tables[0].Rows)
        {
          int num1 = 0;
          long res_item_id = 0;
          try
          {
            res_item_id = Convert.ToInt64(row1["item_id"].ToString());
            try
            {
              num1 = Convert.ToInt32(this.resSetDs.Tables[0].Select("parameter='Advance Notice Period' and parent_id=" + (object) res_item_id)[0]["value"]);
            }
            catch (Exception ex)
            {
              num1 = 0;
            }
          }
          catch
          {
          }
          bool flag1 = true;
          if (num1 > 0 && (Convert.ToDateTime(key) - this.current_timestamp).TotalDays < (double) num1)
            flag1 = false;
          if (flag1)
          {
            DataSet resourceBookingItems = this.resapi.get_resource_booking_items(res_item_id, this.current_user.account_id, key, selectedDates[key], this.str_resource_module);
            if (this.utilities.isValidDataset(resourceBookingItems))
            {
              foreach (DataRow row2 in (InternalDataCollectionBase) resourceBookingItems.Tables[0].Rows)
              {
                double num2;
                try
                {
                  num2 = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]) - Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["accepted_qty"]);
                }
                catch (Exception ex)
                {
                  num2 = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]);
                }
                bool flag2 = true;
                foreach (DataRow row3 in (InternalDataCollectionBase) dtRes1.Rows)
                {
                  if (row3["resource_id"].ToString() == row1["item_id"].ToString() && row3["from_date"].ToString() == key && row3["to_date"].ToString() == selectedDates[key])
                    flag2 = false;
                }
                if (flag2)
                {
                  DataRow row4 = dtRes1.NewRow();
                  row4["resource_id"] = (object) row1["item_id"].ToString();
                  row4["resource_booking_id"] = (object) 0;
                  row4["resource_booking_item_id"] = (object) 0;
                  row4["value"] = (object) row2["value"].ToString();
                  row4[nameof (name)] = (object) row2[nameof (name)].ToString();
                  row4["req_quantity"] = (object) row1["quantity"].ToString();
                  row4["acc_quantity"] = (object) row1["quantity"].ToString();
                  row4["available_qty"] = (object) num2;
                  row4["requestor_remarks"] = (object) "";
                  row4["asset_booking_id"] = (object) this.hdnMeetingBookingId.Value;
                  row4["from_date"] = (object) key;
                  row4["to_date"] = (object) selectedDates[key];
                  row4["row_id"] = (object) Guid.NewGuid();
                  dtRes1.Rows.Add(row4);
                }
              }
            }
          }
        }
      }
      if (dtRes1.Rows.Count > 0)
      {
        this.Session.Add("added_resources", (object) dtRes1);
        this.htmltable = this.getResourceHtml(selectedDates, dtRes1, i, name);
        stringBuilder.Append("<div class='accordion-group'>");
        stringBuilder.Append("<div class='accordion-heading'>");
        stringBuilder.Append("<a class='accordion-toggle collapsed' data-toggle='collapse' data-parent='#accordion2' href='#" + name + "'>");
        stringBuilder.Append("<i class='icon-plus'></i>");
        stringBuilder.Append("  " + headername);
        stringBuilder.Append("</a>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div id='" + name + "' class='accordion-body collapse in'>");
        stringBuilder.Append("<div class='accordion-inner'>");
        stringBuilder.Append(this.htmltable.ToString());
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
    return stringBuilder.ToString();
  }

  public string Bind_resources(
    Dictionary<string, string> selectedDates,
    string date,
    int i,
    string name,
    string headername)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataTable dtRes = new DataTable();
      try
      {
        DataTable dataTable = (DataTable) this.Session["added_resources"];
        if (dataTable.Rows.Count > 0)
        {
          dtRes = dataTable;
        }
        else
        {
          dtRes.Columns.Add("resource_id");
          dtRes.Columns.Add("resource_booking_id");
          dtRes.Columns.Add("resource_booking_item_id");
          dtRes.Columns.Add("value");
          dtRes.Columns.Add(nameof (name));
          dtRes.Columns.Add("available_qty");
          dtRes.Columns.Add("req_quantity");
          dtRes.Columns.Add("acc_quantity");
          dtRes.Columns.Add("requestor_remarks");
          dtRes.Columns.Add("asset_booking_id");
          dtRes.Columns.Add("from_date");
          dtRes.Columns.Add("to_date");
          dtRes.Columns.Add("row_id");
          dtRes.AcceptChanges();
        }
      }
      catch
      {
        dtRes.Columns.Add("resource_id");
        dtRes.Columns.Add("resource_booking_id");
        dtRes.Columns.Add("resource_booking_item_id");
        dtRes.Columns.Add("value");
        dtRes.Columns.Add(nameof (name));
        dtRes.Columns.Add("available_qty");
        dtRes.Columns.Add("req_quantity");
        dtRes.Columns.Add("acc_quantity");
        dtRes.Columns.Add("requestor_remarks");
        dtRes.Columns.Add("asset_booking_id");
        dtRes.Columns.Add("from_date");
        dtRes.Columns.Add("to_date");
        dtRes.Columns.Add("row_id");
        dtRes.AcceptChanges();
      }
      int num1 = 0;
      long res_item_id = 0;
      try
      {
        res_item_id = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
        try
        {
          num1 = Convert.ToInt32(this.resSetDs.Tables[0].Select("parameter='Advance Notice Period' and parent_id=" + (object) res_item_id)[0]["value"]);
        }
        catch (Exception ex)
        {
          num1 = 0;
        }
      }
      catch
      {
      }
      foreach (string key in selectedDates.Keys)
      {
        bool flag1 = true;
        if (num1 > 0 && (Convert.ToDateTime(key) - this.current_timestamp).TotalDays < (double) num1)
          flag1 = false;
        if (flag1)
        {
          DataSet resourceBookingItems = this.resapi.get_resource_booking_items(res_item_id, this.current_user.account_id, key, selectedDates[key], this.str_resource_module);
          if (this.utilities.isValidDataset(resourceBookingItems))
          {
            foreach (DataRow row1 in (InternalDataCollectionBase) resourceBookingItems.Tables[0].Rows)
            {
              double num2;
              try
              {
                num2 = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]) - Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["accepted_qty"]);
              }
              catch (Exception ex)
              {
                num2 = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]);
              }
              bool flag2 = true;
              foreach (DataRow row2 in (InternalDataCollectionBase) dtRes.Rows)
              {
                if (row2["resource_id"].ToString() == this.ddl_res_name.SelectedItem.Value && row2["from_date"].ToString() == key && row2["to_date"].ToString() == selectedDates[key])
                  flag2 = false;
              }
              if (flag2)
              {
                DataRow row3 = dtRes.NewRow();
                row3["resource_id"] = (object) this.ddl_res_name.SelectedItem.Value;
                row3["resource_booking_id"] = (object) 0;
                row3["resource_booking_item_id"] = (object) 0;
                row3["value"] = (object) row1["value"].ToString();
                row3[nameof (name)] = (object) row1[nameof (name)].ToString();
                row3["req_quantity"] = (object) this.txtQty.Text;
                row3["acc_quantity"] = (object) this.txtQty.Text;
                row3["available_qty"] = (object) num2;
                row3["requestor_remarks"] = (object) this.txt_requestor_remarks.Text;
                row3["asset_booking_id"] = (object) this.hdnMeetingBookingId.Value;
                row3["from_date"] = (object) key;
                row3["to_date"] = (object) selectedDates[key];
                row3["row_id"] = (object) Guid.NewGuid();
                dtRes.Rows.Add(row3);
              }
            }
          }
        }
      }
      if (dtRes.Rows.Count > 0)
      {
        this.Session.Add("added_resources", (object) dtRes);
        this.htmltable = this.getResourceHtml(selectedDates, dtRes, i, name);
        stringBuilder.Append("<div class='accordion-group'>");
        stringBuilder.Append("<div class='accordion-heading'>");
        stringBuilder.Append("<a class='accordion-toggle collapsed' data-toggle='collapse' data-parent='#accordion2' href='#" + name + "'>");
        stringBuilder.Append("<i class='icon-plus'></i>");
        stringBuilder.Append("  " + headername);
        stringBuilder.Append("</a>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div id='" + name + "' class='accordion-body collapse in'>");
        stringBuilder.Append("<div class='accordion-inner'>");
        stringBuilder.Append(this.htmltable.ToString());
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
    return stringBuilder.ToString();
  }

  public string getResourceHtml(
    Dictionary<string, string> sDates,
    DataTable dtRes,
    int i,
    string name)
  {
    int num = 0;
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      foreach (string key in sDates.Keys)
      {
        ++i;
        string siteFullPath = this.site_full_path;
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' class='resTable' data-name='res_table' id='" + (object) i + "'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>Resource Type</th>");
        stringBuilder.Append("<th class='hidden-480'>Resource</th>");
        stringBuilder.Append("<th class='hidden-480'>Available Qty.</th>");
        stringBuilder.Append("<th class='hidden-480'>Requested Qty</th>");
        stringBuilder.Append("<th class='hidden-480'>Accepted Qty</th>");
        stringBuilder.Append("<th class='hidden-480'>Remarks</th>");
        stringBuilder.Append("<th class='hidden-480'>Remove</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        DataRow[] dataRowArray = dtRes.Select("from_date='" + key + "' and to_date='" + sDates[key] + "'");
        foreach (DataRow dataRow in dataRowArray)
        {
          ++num;
          string str1 = "req_qty_" + (object) i + "_" + (object) num;
          string str2 = "error_" + (object) i + "_" + (object) num;
          string str3 = "remarks_" + (object) i + "_" + (object) num;
          stringBuilder.Append("<tr class='odd gradeX' id='" + dataRow["row_id"].ToString() + "'>");
          stringBuilder.Append("<td>" + dataRow["value"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow[nameof (name)].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["available_qty"].ToString() + "</td>");
          stringBuilder.Append("<td><input type='text' readonly  class='resQty'  value='" + dataRow["req_quantity"].ToString() + "' id='" + str1 + "'  name='" + str1 + "' required='required'  onkeypress=' return validate(event)'  onchange='checkQty(this," + dataRow["available_qty"].ToString() + ")' /><span style='color:red;' id='" + str2 + "' ></span></td>");
          string str4 = "";
          if (!this.is_admin && this.allowed_items.Contains(Convert.ToInt64(dataRow["resource_id"])))
            str4 = "readonly";
          string str5 = "acc_qty_" + (object) i + "_" + (object) num;
          stringBuilder.Append("<td><input type='text' " + str4 + " class='resQty'  value='" + dataRow["acc_quantity"].ToString() + "' id='" + str5 + "'  name='" + str5 + "' required='required'  onkeypress=' return validate(event)'  onchange='checkQty(this," + dataRow["available_qty"].ToString() + ")' /><span style='color:red;' id='" + str2 + "' ></span></td>");
          stringBuilder.Append("<td><input type='text' readonly  value='" + dataRow["requestor_remarks"].ToString() + "' id='" + str3 + "' name='" + str3 + "' /></td>");
          if (dataRowArray.Length > 1)
          {
            if (this.is_admin)
              stringBuilder.Append("<td><input type='image' ID='btnRemove_resource' runat='server' src='" + this.site_full_path + "assets/img/btn_remove.png'    onclick='javascript:delete_resource(this,\"" + dataRow["row_id"].ToString() + "\");'  /></td>");
            else if (this.allowed_items.Contains(Convert.ToInt64(dataRow["item_id"])))
              stringBuilder.Append("<td><input type='image' ID='btnRemove_resource' runat='server' src='" + this.site_full_path + "assets/img/btn_remove.png'    onclick='javascript:delete_resource(this,\"" + dataRow["row_id"].ToString() + "\");'  /></td>");
          }
          else
            stringBuilder.Append("<td></td>");
          stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
    return stringBuilder.ToString();
  }

  protected void btn_addResource_Click(object sender, EventArgs e)
  {
    try
    {
      long ticks = this.current_timestamp.Ticks;
      this.Process_Datas();
      this.gridview_add_dates.Visible = true;
      TimeSpan timeSpan = new TimeSpan(this.current_timestamp.Ticks - ticks);
      fbs_base_page.log.Info((object) ("Resource - check availability time taken: " + timeSpan.TotalMilliseconds.ToString()));
      this.btn_submit.Visible = true;
      this.btn_addResource.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  protected void ddl_res_name_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      if (this.ddl_res_type.SelectedItem.Text != "Template")
      {
        this.tr_template.Visible = false;
        long int64 = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
        string fdate = Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_StartTime.SelectedItem.Text;
        string tdate = Convert.ToDateTime(this.txt_startDate.Text).ToString(api_constants.sql_datetime_format_short) + " " + this.ddl_EndTime.SelectedItem.Text;
        DataSet resourceBookingItems = this.resapi.get_resource_booking_items(int64, this.current_user.account_id, fdate, tdate, this.str_resource_module);
        byte[] inArray = (byte[]) resourceBookingItems.Tables[0].Rows[0]["binary_data"];
        this.img_res.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(inArray, 0, inArray.Length);
        try
        {
          this.txtAvailableQty.Text = (Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]) - Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["accepted_qty"])).ToString();
        }
        catch (Exception ex)
        {
          this.txtAvailableQty.Text = Convert.ToDouble(resourceBookingItems.Tables[0].Rows[0]["total_qty"]).ToString();
        }
        this.resSetDs = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
        if (this.resSetDs == null)
        {
          this.resSetDs = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
          this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) this.resSetDs);
        }
        int num;
        try
        {
          num = Convert.ToInt32(this.resSetDs.Tables[0].Select("parameter='Advance Notice Period' and parent_id=" + (object) int64)[0]["value"]);
        }
        catch (Exception ex)
        {
          num = 0;
        }
        if (num > 0)
        {
          if ((Convert.ToDateTime(fdate).Date - this.current_timestamp.Date).TotalDays < (double) num)
          {
            this.txtQty.Enabled = false;
            this.lblError.Visible = true;
            this.lblError.Text = Resources.fbs.error_resource_advance_notice_period_reach.Replace("#day#", num.ToString());
            this.btn_addResource.Visible = false;
          }
          else
          {
            this.lblError.Visible = false;
            this.txtQty.Enabled = true;
            this.btn_addResource.Visible = true;
          }
        }
        else
        {
          this.lblError.Visible = false;
          this.txtQty.Enabled = true;
          this.btn_addResource.Visible = true;
        }
      }
      else
      {
        this.tr_avail_qty.Visible = false;
        this.tr_qty.Visible = false;
        this.tr_remarks.Visible = false;
        this.tr_img.Visible = false;
        this.btn_addResource.Visible = true;
        this.tr_template.Visible = true;
        DataSet dataSet = new DataSet();
        DataSet itemByTemplateId = this.resapi.get_resource_template_item_by_templateId(Convert.ToInt64(this.ddl_res_name.SelectedItem.Value), this.current_user.account_id);
        if (this.utilities.isValidDataset(itemByTemplateId))
        {
          this.litResource.Text = "";
          foreach (DataRow row in (InternalDataCollectionBase) itemByTemplateId.Tables[0].Rows)
          {
            Literal litResource = this.litResource;
            litResource.Text = litResource.Text + row["resource_name"].ToString() + " <br />";
          }
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    this.txtQty.Text = "";
    this.txt_requestor_remarks.Text = "";
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      DataTable dt = new DataTable();
      Dictionary<string, string> selectedDates = new Dictionary<string, string>();
      if (this.Session["SelectedDates"] != null)
        selectedDates = (Dictionary<string, string>) this.Session["SelectedDates"];
      if (this.Session["added_resources"] != null)
        dt = (DataTable) this.Session["added_resources"];
      if (selectedDates.Count == 0)
      {
        this.litErrorMsg.Text = Resources.fbs.error_resource_booking_no_dates;
        this.alertError.Attributes.Add("style", "display: block;");
        this.btn_submit.Visible = false;
      }
      else
      {
        if (this.checkQuantity(selectedDates, dt))
          return;
        int num1 = 0;
        foreach (string key in selectedDates.Keys)
        {
          ++num1;
          DataRow[] dataRowArray = dt.Select("from_date='" + key + "' and to_date='" + selectedDates[key] + "'");
          if (dataRowArray.Length > 0)
          {
            resource_booking resourceBooking1 = new resource_booking();
            try
            {
              resourceBooking1.asset_booking_id = Convert.ToInt64(dataRowArray[0]["asset_booking_id"]);
            }
            catch
            {
              resourceBooking1.asset_booking_id = 0L;
            }
            try
            {
              resourceBooking1.booked_for_id = Convert.ToInt64(this.Request.Form[this.hfUserId.UniqueID]);
            }
            catch
            {
              resourceBooking1.booked_for_id = this.current_user.user_id;
            }
            resourceBooking1.book_from = Convert.ToDateTime(dataRowArray[0]["from_date"]);
            resourceBooking1.book_to = Convert.ToDateTime(dataRowArray[0]["to_date"]);
            try
            {
              resourceBooking1.resource_booking_id = Convert.ToInt64(dataRowArray[0]["resource_booking_id"]);
            }
            catch
            {
              resourceBooking1.resource_booking_id = 0L;
            }
            resourceBooking1.booking_type = Convert.ToInt32(this.hdnBookingType.Value);
            resourceBooking1.account_id = this.current_user.account_id;
            resourceBooking1.created_by = this.current_user.user_id;
            resourceBooking1.created_on = this.current_timestamp;
            resourceBooking1.email = this.txt_email.Text;
            resourceBooking1.item_id = 0L;
            resourceBooking1.layout_id = 0L;
            resourceBooking1.modified_by = this.current_user.user_id;
            resourceBooking1.modified_on = this.current_timestamp;
            resourceBooking1.module_name = this.str_resource_module;
            resourceBooking1.purpose = this.txt_purpose.Text;
            resourceBooking1.record_id = Guid.NewGuid();
            resourceBooking1.remarks = this.txtRemarks.Text;
            resourceBooking1.requested_by_id = this.current_user.user_id;
            resourceBooking1.status = 1;
            resourceBooking1.venue = this.txt_location.Text;
            resource_booking resourceBooking2 = this.resapi.update_resource_booking(resourceBooking1);
            if (resourceBooking2.resource_booking_id > 0L)
            {
              this.resBkId = resourceBooking2.resource_booking_id;
              int num2 = 0;
              this.resapi.delete_resource_booking_items_by_resource_booking_id(this.resBkId, this.current_user.account_id, this.current_user.user_id);
              foreach (DataRow dataRow in dataRowArray)
              {
                ++num2;
                resource_booking_item objRes = new resource_booking_item();
                objRes.account_id = this.current_user.account_id;
                objRes.created_by = this.current_user.user_id;
                objRes.created_on = this.current_timestamp;
                objRes.modified_by = this.current_user.user_id;
                objRes.modified_on = this.current_timestamp;
                objRes.module_name = this.str_resource_module;
                objRes.other_remarks = "";
                objRes.record_id = Guid.NewGuid();
                objRes.req_price = 0.0;
                objRes.resource_id = Convert.ToInt64(dataRow["resource_id"]);
                string name1 = "req_qty_" + (object) num1 + "_" + (object) num2;
                double num3 = 0.0;
                try
                {
                  num3 = Convert.ToDouble(this.Request.Form.Get(name1));
                }
                catch
                {
                }
                objRes.req_qty = num3;
                string name2 = "acc_qty_" + (object) num1 + "_" + (object) num2;
                double num4 = 0.0;
                try
                {
                  num4 = Convert.ToDouble(this.Request.Form.Get(name2));
                }
                catch
                {
                }
                string name3 = "remarks_" + (object) num1 + "_" + (object) num2;
                string str1 = "";
                try
                {
                  str1 = this.Request.Form.Get(name3).ToString();
                }
                catch
                {
                }
                objRes.accepted_qty = num4;
                objRes.requestor_remakrs = str1;
                objRes.resource_booking_id = this.resBkId;
                objRes.resource_booking_item_id = 0L;
                objRes.status = 1;
                objRes.accepted_price = 0.0;
                resource_booking_item resourceBookingItem = this.resapi.update_resource_booking_item(objRes);
                try
                {
                  DataSet usersByGroup = this.users.get_users_by_group(this.resapi.get_resource_item_obj(resourceBookingItem.resource_id, this.current_user.account_id, this.str_resource_module).owner_group_id, this.current_user.account_id);
                  string str2 = "";
                  if (this.utilities.isValidDataset(usersByGroup))
                  {
                    foreach (DataRow row in (InternalDataCollectionBase) usersByGroup.Tables[0].Rows)
                      str2 = str2 + row["email"].ToString() + ";";
                  }
                  if (str2 != "")
                  {
                    string str3 = str2.Trim().Substring(0, str2.Length - 1);
                    template template1 = new template();
                    template template2 = this.tapi.get_template("email_resource_owner_request", this.current_user.account_id);
                    string body = this.replaceTemplate_resource_owner(template2.content_data, objRes, resourceBooking1);
                    string title = template2.title;
                    string cc = "";
                    string bcc = "";
                    string to = str3;
                    email email = new email();
                    if (Convert.ToBoolean(this.current_account.properties["send_email"]))
                      this.utilities.sendEmail(bcc, body, title, cc, to, resourceBooking1.record_id);
                  }
                }
                catch (Exception ex)
                {
                  fbs_base_page.log.Error((object) "Send email to resource owner - Error -> ", ex);
                }
              }
              this.saveAttachments(resourceBooking2.resource_booking_id);
              try
              {
                template template = this.tapi.get_template("email_resource_request_requestor", this.current_user.account_id);
                string body = this.replaceTemplate_resource_requestor(template.content_data, resourceBooking1, this.resBkId);
                string title = template.title;
                string cc = "";
                string bcc = "";
                string email1 = resourceBooking1.email;
                email email2 = new email();
                if (Convert.ToBoolean(this.current_account.properties["send_email"]))
                  this.utilities.sendEmail(bcc, body, title, cc, email1, resourceBooking1.record_id);
              }
              catch (Exception ex)
              {
                fbs_base_page.log.Error((object) "Send email to resource requestor - Error -> ", ex);
              }
            }
            this.alertError.Attributes.Add("style", "display: none;");
          }
          else
          {
            this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.error_resource_booking;
            this.alertError.Attributes.Add("style", "display: block;");
            return;
          }
        }
        this.Session.Remove("SelectedDates");
        this.Session.Remove("added_resources");
        this.Response.Redirect("requests.aspx");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
      this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.error_resource_booking_save;
      this.alertError.Attributes.Add("style", "display: block;");
    }
  }

  private bool checkQuantity(Dictionary<string, string> selectedDates, DataTable dt)
  {
    bool flag = false;
    int num1 = 0;
    try
    {
      foreach (string key in selectedDates.Keys)
      {
        ++num1;
        DataRow[] dataRowArray = dt.Select("from_date='" + key + "' and to_date='" + selectedDates[key] + "'");
        if (dataRowArray.Length > 0)
        {
          int num2 = 0;
          foreach (DataRow dataRow in dataRowArray)
          {
            ++num2;
            string name1 = "req_qty_" + (object) num1 + "_" + (object) num2;
            double num3 = 0.0;
            try
            {
              num3 = Convert.ToDouble(this.Request.Form.Get(name1));
            }
            catch
            {
            }
            double num4 = 0.0;
            try
            {
              num4 = Convert.ToDouble(dataRow["available_qty"]);
            }
            catch
            {
            }
            if (num3 > num4)
            {
              try
              {
                this.litErrorMsg.Text = "<strong>Error!</strong> Quantity is more than the available quantity.";
                this.alertError.Attributes.Add("style", "display: block;");
              }
              catch
              {
              }
              flag = true;
              return flag;
            }
            if (!flag)
              flag = false;
            string name2 = "acc_qty_" + (object) num1 + "_" + (object) num2;
            double num5 = 0.0;
            try
            {
              num5 = Convert.ToDouble(this.Request.Form.Get(name2));
            }
            catch
            {
            }
            if (num5 > num4)
            {
              try
              {
                this.litErrorMsg.Text = "<strong>Error!</strong> Quantity is more than the available quantity.";
                this.alertError.Attributes.Add("style", "display: block;");
              }
              catch
              {
              }
              flag = true;
              return flag;
            }
            if (!flag)
              flag = false;
          }
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
      this.litErrorMsg.Text = "<strong>Error!</strong> Invalid quantity.";
      this.alertError.Attributes.Add("style", "display: block;");
      flag = true;
    }
    return flag;
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("../default.aspx");

  private string replaceTemplate_resource_owner(
    string content,
    resource_booking_item objRes,
    resource_booking obj)
  {
    string str = content;
    try
    {
      str = str.Replace("cid:headerImageId", this.imagepathfbs);
      str = str.Replace("[FULL NAME]", this.txtBookedFor.Text);
      str = str.Replace("[PURPOSE]", obj.purpose);
      str = str.Replace("[BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
      str = str.Replace("[BOOKED STATUS]", this.bookings.get_status((long) obj.status));
      str = str.Replace("[REQUESTED BY]", this.lbl_requestedBy.Value);
      str = str.Replace("[REQUESTED ON]", this.tzapi.convert_to_user_timestamp(obj.created_on).ToString(api_constants.display_datetime_format));
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
      this.resItems = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_items");
      if (this.resItems == null)
      {
        this.resItems = this.resapi.get_resource_items(this.current_user.account_id, this.str_resource_module);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_items", (object) this.resItems);
      }
      DataRow[] dataRowArray = this.resItems.Tables[0].Select("item_id=" + (object) obj.resource_id);
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

  private string get_resourcelist(long bid)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      DataSet byAssetBookingId = this.resapi.get_resource_bookings_items_by_asset_booking_id(bid, this.current_user.account_id, "", "", this.str_resource_module);
      if (this.utilities.isValidDataset(byAssetBookingId))
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
        foreach (DataRow row in (InternalDataCollectionBase) byAssetBookingId.Tables[0].Rows)
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

  private string replaceTemplate_resource_requestor(
    string content,
    resource_booking obj,
    long resource_booking_id)
  {
    string str = content;
    try
    {
      str = str.Replace("cid:headerImageId", this.imagepathfbs);
      str = str.Replace("[FULL NAME]", this.txtBookedFor.Text);
      str = str.Replace("[PURPOSE]", obj.purpose);
      str = str.Replace("[BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
      str = str.Replace("[BOOKED STATUS]", this.bookings.get_status((long) obj.status));
      str = str.Replace("[REQUESTED BY]", this.lbl_requestedBy.Value);
      str = str.Replace("[REQUESTED ON]", this.tzapi.convert_to_user_timestamp(obj.created_on).ToString(api_constants.display_datetime_format));
      str = str.Replace("[EMAILS]", obj.email);
      str = str.Replace("[RESOURCE DETAILS]", this.get_resourcelist(resource_booking_id));
      str = str.Replace("[SITE_FULL_PATH]", this.site_full_path);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return str;
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
        if ((int) Convert.ToInt16(setting.value) < contentLength)
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
        string extension = new FileInfo(this.upload_attachment.PostedFile.FileName).Extension;
        if (setting.value.Contains(extension))
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

  public void bind_attachments(DataSet dsDoc)
  {
    bool flag = false;
    foreach (DataRow row in (InternalDataCollectionBase) dsDoc.Tables[0].Rows)
    {
      flag = true;
      this.lnkAttachment.CommandArgument = row["resource_document_id"].ToString();
      this.lnkAttachment.Text = row["document_name"].ToString();
      this.lnkDelete.CommandArgument = row["resource_document_id"].ToString();
      this.lnkDelete.Text = "Delete";
      this.lnkDelete.OnClientClick = "javascript:return delete_confirmation('" + Resources.fbs.delete_confirmation_msg + "');";
    }
    if (flag)
      return;
    this.lnkAttachment.Text = "";
    this.lnkDelete.Text = "";
  }

  protected void lnkAttachment_Click(object sender, EventArgs e)
  {
    try
    {
      if (!(this.hdnResourceBookingId.Value != ""))
        return;
      long res_booking_id = 0;
      try
      {
        res_booking_id = Convert.ToInt64(this.hdnResourceBookingId.Value);
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
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.lnkAttachment.Text);
      this.Response.Buffer = true;
      this.Response.Clear();
      this.Response.BinaryWrite(buffer);
      this.Response.End();
      this.Response.Flush();
      this.lnkAttachment.Text = resourceBookingId.Tables[0].Rows[0]["document_name"].ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void lnkDeleteAttachment_Click(object sender, EventArgs e)
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

  private void getUploadedAttachments()
  {
    if (this.hdnResourceBookingId.Value != "")
    {
      long res_booking_id = 0;
      try
      {
        res_booking_id = Convert.ToInt64(this.hdnResourceBookingId.Value);
      }
      catch
      {
      }
      if (res_booking_id > 0L)
      {
        DataSet resourceBookingId = this.resapi.get_resource_document_by_resource_booking_id(res_booking_id, this.current_user.account_id);
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
    else
    {
      this.divUpload.Visible = true;
      this.divAttachments.Visible = false;
    }
  }

  [WebMethod(EnableSession = true)]
  public static string DeleteResource(string del_id)
  {
    try
    {
      if (HttpContext.Current.Session["added_resources"] == null)
        return "";
      DataTable dataTable = (DataTable) HttpContext.Current.Session["added_resources"];
      foreach (DataRow row in dataTable.Select("row_id='" + del_id + "' "))
        dataTable.Rows.Remove(row);
      dataTable.AcceptChanges();
      HttpContext.Current.Session["added_resources"] = (object) dataTable;
      return "true";
    }
    catch
    {
      return "";
    }
  }
}
