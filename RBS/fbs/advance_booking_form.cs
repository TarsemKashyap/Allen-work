// Decompiled with JetBrains decompiler
// Type: advance_booking_form
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
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public class advance_booking_form : fbs_base_page, IRequiresSessionState
{
  protected Literal litError;
  protected HtmlGenericControl alt_error_alrdybook;
  protected HtmlGenericControl li_invite_list;
  protected HiddenField hdnBookingId;
  protected HiddenField hdn_unique_id;
  protected HtmlInputText txt_purpose;
  protected HtmlGenericControl errorpurpose;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected TextBox txtBookedFor;
  protected Panel myPanel;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hfUserId;
  protected HtmlGenericControl errorbookedfor;
  protected TextBox txt_email;
  protected HtmlImage img_loading;
  protected RegularExpressionValidator RE_email;
  protected HtmlGenericControl erroremail;
  protected HtmlInputText txt_telephone;
  protected HtmlGenericControl errortelephone;
  protected HtmlTextArea txt_remarks;
  protected HtmlGenericControl remarks_error;
  protected HtmlGenericControl div_meeting_type;
  protected DropDownList ddlMeetingType;
  protected HtmlGenericControl errorMeetingType;
  protected HtmlGenericControl div_ddl_meeting_type;
  protected HtmlGenericControl lblassets;
  protected HiddenField hdn_invitee;
  protected TextBox txtUser;
  protected Panel pnl_invitee;
  protected AutoCompleteExtender AutoCompleteExtender1;
  protected Button btnAssignToGroup;
  protected GridView gdInviteList;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlForm form1;
  private long asset_id;
  private long bookingId;
  private DataSet setting_data;
  public string POP_height = "";
  private string type = "";
  private DataTable booking_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["advanced_booking"]))
      this.Server.Transfer("unauthorized.aspx");
    try
    {
      string str = "";
      try
      {
        str = this.Request.QueryString["uid"];
      }
      catch
      {
        this.Server.Transfer("unauthorized.aspx");
      }
      if (this.Session["data_" + str] != null)
        this.booking_table = (DataTable) this.Session["data_" + str];
      else
        fbs_base_page.log.Info((object) "Custom booking Form  SelectedDates session expired in function Page_Load ");
      try
      {
        this.setting_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
        if (this.setting_data == null)
        {
          this.setting_data = this.settings.view_settings(this.current_user.account_id);
          this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.setting_data);
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("Error2:" + ex.ToString()));
      }
      if (this.IsPostBack)
        return;
      if (this.booking_table.Rows.Count == 0)
      {
        Modal.Close((Page) this);
      }
      else
      {
        if (!this.check_local_conflict())
          return;
        this.populate_meeting_type();
        this.booking_table.Columns.Add(new DataColumn("asset_booking_id", Type.GetType("System.Int64")));
        this.booking_table.Columns.Add(new DataColumn("booking_status", Type.GetType("System.Int64")));
        this.booking_table.Columns.Add(new DataColumn("invitees", Type.GetType("System.String")));
        this.booking_table.AcceptChanges();
        this.hdn_unique_id.Value = str;
        this.ViewState.Add("data", (object) this.booking_table);
        asset_booking assetBooking = new asset_booking();
        DataTable table = new DataTable();
        DataRow[] dataRowArray = this.booking_table.Select("db_booking_id <> 0");
        if (dataRowArray.Length > 0)
        {
          asset_booking booking = this.bookings.get_booking(Convert.ToInt64(dataRowArray[0]["db_booking_id"]), this.current_user.account_id);
          this.ViewState.Add("global_appointment_id", (object) this.bookings.get_appointment_id(booking.booking_id, booking.account_id));
          this.ViewState.Add("record_id", (object) booking.record_id);
          this.ViewState.Add("repeat_reference_id", (object) booking.repeat_reference_id);
          this.txt_purpose.Value = booking.purpose;
          this.txt_remarks.Value = booking.remarks;
          this.txt_email.Text = booking.email;
          this.txt_telephone.Value = booking.contact;
          this.hdnBookingId.Value = booking.booking_id.ToString();
          foreach (ListItem listItem in this.ddlMeetingType.Items)
          {
            if (listItem.Value == booking.meeting_type.ToString())
              listItem.Selected = true;
          }
          if (booking.booked_for == this.current_user.user_id)
          {
            this.txtBookedFor.Text = this.current_user.full_name;
            this.hfUserId.Value = this.current_user.user_id.ToString();
          }
          else
          {
            this.txtBookedFor.Text = this.users.get_user(booking.booked_for, booking.account_id).full_name;
            this.hfUserId.Value = booking.booked_for.ToString();
          }
          DataSet invites = this.bookings.get_invites(booking.booking_id, this.current_user.account_id);
          if (this.utilities.isValidDataset(invites))
            this.SetInitialRow(invites.Tables[0]);
          else
            this.SetInitialRow(table);
        }
        else
        {
          Guid guid = Guid.NewGuid();
          this.ViewState.Add("record_id", (object) guid);
          this.ViewState.Add("repeat_reference_id", (object) guid);
          this.ViewState.Add("global_appointment_id", (object) Guid.NewGuid().ToString());
          this.txt_email.Text = this.current_user.email;
          this.txt_telephone.Value = this.current_user.properties["staff_offphone"].property_value + "/" + this.current_user.properties["staff_pager_mobile"].property_value;
          this.hdnBookingId.Value = "0";
          this.txtBookedFor.Text = this.current_user.full_name;
          this.hfUserId.Value = this.current_user.user_id.ToString();
          this.SetInitialRow(table);
        }
        this.li_invite_list.Visible = true;
        this.populate_asset_Custombooking(this.booking_table);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Custom Booking Form Error ->", ex);
    }
  }

  private void populate_meeting_type()
  {
    if (!this.utilities.isValidDataset(this.setting_data))
      return;
    foreach (DataRow dataRow in this.setting_data.Tables[0].Select("parameter='meeting_type' "))
      this.ddlMeetingType.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
  }

  private bool check_local_conflict()
  {
    int num1 = 1;
    foreach (DataRow row1 in (InternalDataCollectionBase) this.booking_table.Rows)
    {
      DateTime dateTime1 = Convert.ToDateTime(row1["book_from"]);
      DateTime dateTime2 = Convert.ToDateTime(row1["book_to"]);
      int num2 = 1;
      foreach (DataRow row2 in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        if (row1["booking_id"].ToString() != row2["booking_id"].ToString())
        {
          DateTime dateTime3 = Convert.ToDateTime(row2["book_from"]);
          DateTime dateTime4 = Convert.ToDateTime(row2["book_to"]);
          if ((dateTime3 > dateTime1 && dateTime3 < dateTime2 || dateTime4 > dateTime1 && dateTime4 < dateTime2) && row1["asset_id"].ToString() == row2["asset_id"].ToString())
          {
            this.litError.Text = "There is a room conflict between rows " + (object) num1 + " and " + (object) num2 + ". Please correct error and try again.";
            this.litError.Visible = true;
            this.btn_submit.Visible = false;
            return false;
          }
        }
        ++num2;
      }
      ++num1;
    }
    return true;
  }

  private void show_hide_terms_housekeeping_checkboxes(DataSet data)
  {
    try
    {
      this.setting_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (this.setting_data != null)
        return;
      this.setting_data = this.settings.view_settings(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.setting_data);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error2:" + ex.ToString()));
    }
  }

  public bool check_bookings() => true;

  private bool check_invite_list_visible()
  {
    bool flag = false;
    DataSet dataSet = new DataSet();
    DataSet settings = this.settings.get_settings(this.current_user.account_id);
    if (settings != null && settings.Tables[0].Rows.Count > 0)
      flag = this.utilities.isValidDataset(settings) && settings.Tables[0].Select("parameter='show_invite_list' and value='True'").Length > 0;
    return flag;
  }

  protected void chk_setup_CheckedChanged(object sender, EventArgs e)
  {
  }

  private void populate_asset_Custombooking(DataTable data)
  {
    try
    {
      List<long> rooms = new List<long>();
      foreach (DataRow row in (InternalDataCollectionBase) data.Rows)
        rooms.Add(Convert.ToInt64(row["asset_id"]));
      DataSet sDS = this.settings.view_settings(this.current_user.account_id);
      DataSet assets = this.assets.get_assets(rooms, this.current_user.account_id, 0L, 0L, 0L, 0, "");
      DataSet assProDs = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
      {
        "asset_property"
      });
      DataSet dataSet1 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
      {
        "booking_lead_time"
      });
      DataSet dataSet2 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
      {
        "book_weekend"
      });
      this.assets.view_asset_properties(this.current_user.account_id, new string[1]
      {
        "book_holiday"
      });
      if (!this.gp.isAdminType)
      {
        for (int index = 0; index < data.Rows.Count; ++index)
        {
          DateTime dateTime1 = Convert.ToDateTime(data.Rows[index]["book_from"]);
          Convert.ToDateTime(data.Rows[index]["book_to"]);
          DataRow[] dataRowArray1 = dataSet1.Tables[0].Select("asset_id=" + data.Rows[index]["asset_id"].ToString());
          DataRow[] dataRowArray2 = dataSet2.Tables[0].Select("asset_id=" + data.Rows[index]["asset_id"].ToString());
          DataRow[] dataRowArray3 = dataSet2.Tables[0].Select("asset_id=" + data.Rows[index]["asset_id"].ToString());
          if (dataRowArray1.Length > 0)
          {
            string newValue = dataRowArray1[0].ItemArray[2].ToString();
            if (newValue != "")
            {
              if (newValue.Split(' ')[0] != "0")
              {
                DateTime dateTime2 = new DateTime();
                DateTime dateTime3;
                if (newValue.Split(' ')[1] == "Hour(s)")
                {
                  dateTime3 = DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(Convert.ToDouble(newValue.Split(' ')[0]));
                }
                else
                {
                  int num1 = 0;
                  int num2 = 0;
                  while (true)
                  {
                    if ((long) num2 < Convert.ToInt64(newValue.Split(' ')[0]) + 1L)
                    {
                      ++num1;
                      if (DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 1)).DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 1)).DayOfWeek == DayOfWeek.Sunday)
                      {
                        if (dataRowArray2.Length > 0)
                        {
                          if (dataRowArray2[0].ItemArray[2].ToString() == "False")
                            ++num1;
                        }
                        else
                          ++num1;
                      }
                      if (dataRowArray3.Length > 0)
                      {
                        if (dataRowArray3[0].ItemArray[2].ToString() == "False")
                        {
                          DataSet ds = this.holidays.view_holidays(DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 1)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num2 + 2)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", this.account_id);
                          if (this.utilities.isValidDataset(ds) && ds.Tables[0].Rows.Count > 0)
                            ++num1;
                        }
                      }
                      else
                        ++num1;
                      ++num2;
                    }
                    else
                      break;
                  }
                  dateTime3 = DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) num1);
                }
                if (dateTime1 < dateTime3)
                {
                  this.alt_error_alrdybook.Attributes.Add("style", "display: block;");
                  this.alt_error_alrdybook.Visible = true;
                  this.litError.Text = Resources.fbs.booking_lead_time_error.Replace("[date]", dateTime3.ToString(api_constants.display_datetime_format_short)).Replace("[day]", newValue);
                  this.btn_submit.Visible = false;
                  return;
                }
              }
            }
          }
        }
      }
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table' width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>S.No.</th>");
      stringBuilder.Append("<th class='hidden-480'>Code/ Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Level</th>");
      stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
      stringBuilder.Append("<th class='hidden-480'>Category</th>");
      stringBuilder.Append("<th class='hidden-480'>From</th>");
      stringBuilder.Append("<th class='hidden-480'>To</th>");
      stringBuilder.Append("<th class='hidden-480'>Status</th>");
      stringBuilder.Append("<th class='hidden-480'>Comments</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      int counter = 1;
      foreach (DataRow row in (InternalDataCollectionBase) this.booking_table.Rows)
      {
        if (Convert.ToInt32(row["status"]) == 1)
        {
          string datesCustomBookings = this.bookingsbl.getAssetHtml_with_bookingDates_CustomBookings(counter, assets, sDS, assProDs, Convert.ToInt64(row["asset_id"]), new Dictionary<string, string>()
          {
            {
              Convert.ToDateTime(row["book_from"]).ToString(api_constants.sql_datetime_format),
              Convert.ToDateTime(row["book_to"]).ToString(api_constants.sql_datetime_format)
            }
          }, Convert.ToDateTime(row["book_from"]).ToString(api_constants.sql_datetime_format), "");
          stringBuilder.Append(datesCustomBookings.ToString());
          ++counter;
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.lblassets.InnerHtml = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Custom Booking Form Error->", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this);

  private void close_modal()
  {
    try
    {
      Modal.Close((Page) this);
      this.Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "close()", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Custom Booking Form Error->", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e) => this.Custom_Booking_transcations();

  public void Custom_Booking_transcations()
  {
    try
    {
      string str1 = "";
      List<asset_booking> bookings = new List<asset_booking>();
      DataTable table = (DataTable) this.ViewState["data"];
      bool flag1 = false;
      string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
      asset_booking assetBooking1 = new asset_booking()
      {
        invites = new Dictionary<long, asset_booking_invite>(),
        booking_type = Convert.ToInt32((object) api_constants.booking_type.Custom)
      };
      DataSet dataSet = this.assets.view_assets(this.current_user.account_id);
      Guid guid1 = (Guid) this.ViewState["repeat_reference_id"];
      Guid guid2 = (Guid) this.ViewState["record_id"];
      Guid guid3 = guid2;
      Guid event_id = Guid.NewGuid();
      bool flag2 = table.Rows.Count != 0 && this.booking_table.Select("status=1").Length > 1;
      foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
      {
        if (row["asset_id"].ToString() != "0" && row["asset_id"].ToString() != "" && row["asset_id"] != null)
        {
          asset_booking assetBooking2 = new asset_booking();
          asset_booking changed = new asset_booking();
          if (this.check_bookings())
            flag1 = true;
          else if (!flag1)
            flag1 = false;
          changed.account_id = this.current_user.account_id;
          changed.booking_id = Convert.ToInt64(row["db_booking_id"]);
          if (changed.booking_id > 0L)
          {
            assetBooking2 = this.bookings.get_booking(changed.booking_id, changed.account_id);
            Guid eventId = assetBooking2.event_id;
            event_id = assetBooking2.event_id;
            changed = this.bookings.get_booking(changed.booking_id, changed.account_id);
            changed.global_appointment_id = this.bookings.get_appointment_id(changed.booking_id, changed.account_id);
          }
          changed.is_repeat = flag2;
          changed.repeat_reference_id = guid3;
          changed.record_id = guid2;
          changed.transfer_original_booking_id = 0L;
          changed.transfer_reason = "";
          changed.transfer_request = false;
          changed.created_on = this.current_timestamp;
          changed.asset_id = Convert.ToInt64(row["asset_id"]);
          changed.book_from = Convert.ToDateTime(row["book_from"]);
          changed.book_to = Convert.ToDateTime(row["book_to"]);
          changed.book_duration = (double) this.utilities.getDuration(changed.book_from, changed.book_to);
          changed.contact = this.txt_telephone.Value;
          changed.created_by = this.current_user.user_id;
          changed.email = this.txt_email.Text;
          changed.housekeeping_required = false;
          changed.modified_by = this.current_user.user_id;
          changed.purpose = this.txt_purpose.Value;
          changed.remarks = this.txt_remarks.Value;
          changed.setup_required = false;
          changed.booked_for = Convert.ToInt64(this.Request.Form[this.hfUserId.UniqueID]);
          changed.setup_type = 0L;
          changed.meeting_type = Convert.ToInt64(this.ddlMeetingType.SelectedItem.Value);
          DataRow[] dataRowArray = dataSet.Tables[0].Select("asset_id=" + changed.asset_id.ToString());
          long num;
          try
          {
            num = Convert.ToInt64(dataRowArray[0]["asset_owner_group_id"]);
          }
          catch
          {
            num = 0L;
          }
          if (num == 0L)
            changed.status = Convert.ToInt16(row["status"]);
          else if (this.u_group.group_type == 1)
            changed.status = Convert.ToInt16(row["status"]);
          else
            changed.status = !this.approvable_rooms.Contains(changed.asset_id) ? (short) 4 : (short) 1;
          if (changed.booking_id == 0L)
          {
            changed.global_appointment_id = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            changed.sequence = 0;
          }
          else
            ++changed.sequence;
          if (this.bookings.compare_booking(assetBooking2, changed))
          {
            asset_booking assetBooking3 = this.bookings.update_booking(changed);
            if (assetBooking3.booking_id > 0L)
            {
              try
              {
                this.bookings.set_book_status(assetBooking3, assetBooking2);
              }
              catch
              {
              }
              this.bookings.update_event_id(assetBooking3.booking_id, event_id, assetBooking3.account_id);
              str1 = str1 + (object) assetBooking3.booking_id + ",";
              row["asset_booking_id"] = (object) assetBooking3.booking_id;
              row["db_booking_id"] = (object) assetBooking3.booking_id;
              row["booking_status"] = (object) assetBooking3.status;
              table.AcceptChanges();
              string str2 = "";
              if (this.gdInviteList.Rows.Count > 0)
              {
                Dictionary<long, asset_booking_invite> dictionary = new Dictionary<long, asset_booking_invite>();
                foreach (asset_booking_invite assetBookingInvite in this.bookings.get_invite_list(assetBooking3.booking_id, this.current_user.account_id).Values)
                  this.bookings.delete_invite(assetBookingInvite);
                assetBooking3.invites = new Dictionary<long, asset_booking_invite>();
                for (int index = 0; index < this.gdInviteList.Rows.Count; ++index)
                {
                  TextBox control1 = (TextBox) this.gdInviteList.Rows[index].Cells[2].FindControl("txtName");
                  TextBox control2 = (TextBox) this.gdInviteList.Rows[index].Cells[3].FindControl("txtEmail_invite");
                  if (Regex.IsMatch(control2.Text, pattern) && !string.IsNullOrEmpty(control2.Text))
                  {
                    asset_booking_invite assetBookingInvite1 = new asset_booking_invite();
                    assetBookingInvite1.account_id = this.current_user.account_id;
                    assetBookingInvite1.booking_id = assetBooking3.booking_id;
                    assetBookingInvite1.booking_invite_id = 0L;
                    assetBookingInvite1.created_by = this.current_user.user_id;
                    assetBookingInvite1.email = control2.Text;
                    assetBookingInvite1.modified_by = this.current_user.user_id;
                    assetBookingInvite1.name = control1.Text;
                    assetBookingInvite1.record_id = Guid.NewGuid();
                    assetBookingInvite1.repeat_reference_id = assetBooking3.repeat_reference_id;
                    asset_booking_invite assetBookingInvite2 = this.bookings.update_invite(assetBookingInvite1);
                    if (assetBookingInvite2.booking_invite_id > 0L)
                    {
                      str2 = str2 + control2.Text + ";";
                      assetBooking3.invites.Add(assetBookingInvite2.booking_invite_id, assetBookingInvite2);
                    }
                  }
                }
                row["invitees"] = (object) str2;
                this.booking_table.AcceptChanges();
              }
              else
                assetBooking3.invites = new Dictionary<long, asset_booking_invite>();
              bookings.Add(assetBooking3);
              if (assetBooking3.status != (short) 1)
              {
                workflow workflow = new workflow();
                workflow.account_id = this.current_user.account_id;
                workflow.action_owner_id = num <= 0L ? this.current_user.user_id : num;
                workflow.action_remarks = "Asset booking";
                workflow.action_type = (short) 1;
                workflow.created_by = Convert.ToInt64(this.Request.Form[this.hfUserId.UniqueID]);
                workflow.created_on = this.current_timestamp;
                workflow.due_on = new DateTime(1900, 1, 1);
                workflow.record_id = Guid.NewGuid();
                workflow.reference_id = assetBooking3.booking_id;
                workflow.workflow_id = 0L;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<properties><asset_id>" + (object) assetBooking3.asset_id + " </asset_id></properties>");
                workflow.properties = xmlDocument;
                this.workflows.update_workflow(workflow);
              }
            }
          }
        }
      }
      try
      {
        this.bookingsbl.update_outlook(bookings);
      }
      catch (Exception ex)
      {
      }
      if (Convert.ToBoolean(this.current_account.properties["send_email"]))
        this.bookingsbl.send_booking_emails(bookings);
      this.update_resources(table);
      this.close_modal();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Custom Booking Form Error->", ex);
    }
  }

  private void update_resources(DataTable table)
  {
    foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
    {
      if (Convert.ToInt64(row["db_booking_id"]) > 0L && this.resapi.get_resource_bookings(Convert.ToInt64(row["db_booking_id"]), this.current_user.account_id, this.str_resource_module).Tables[0].Rows.Count > 0)
        this.resapi.update_resource_booking_date(Convert.ToInt64(row["db_booking_id"]), Convert.ToDateTime(row["book_from"]).ToString(api_constants.sql_datetime_format), Convert.ToDateTime(row["book_to"]).ToString(api_constants.sql_datetime_format), this.str_resource_module, this.current_user.user_id);
    }
  }

  private void SetInitialRow(DataTable table)
  {
    try
    {
      DataTable dataTable = new DataTable();
      dataTable.Columns.Add("RowNumber");
      dataTable.Columns.Add("InviteID");
      dataTable.Columns.Add("Name");
      dataTable.Columns.Add("Email");
      dataTable.Columns.Add("Remove_image_visibility");
      int num = 1;
      foreach (DataRow row1 in (InternalDataCollectionBase) table.Rows)
      {
        DataRow row2 = dataTable.NewRow();
        row2["RowNumber"] = (object) num;
        row2["InviteID"] = (object) row1["booking_invite_id"].ToString();
        row2["Name"] = (object) row1["name"].ToString();
        row2["Email"] = (object) row1["email"].ToString();
        row2["Remove_image_visibility"] = (object) "True";
        dataTable.Rows.Add(row2);
        ++num;
      }
      this.ViewState["InviteList"] = (object) dataTable;
      this.gdInviteList.DataSource = (object) dataTable;
      this.gdInviteList.DataBind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void AddNewRowToGrid()
  {
    string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
    bool flag = true;
    int index1 = 0;
    if (this.ViewState["InviteList"] != null)
    {
      DataTable dataTable1 = (DataTable) this.ViewState["InviteList"];
      DataTable dataTable2 = dataTable1.Clone();
      if (dataTable1.Rows.Count > 0)
      {
        for (int index2 = 0; index2 < dataTable1.Rows.Count; ++index2)
        {
          TextBox control1 = (TextBox) this.gdInviteList.Rows[index1].Cells[1].FindControl("txtName");
          TextBox control2 = (TextBox) this.gdInviteList.Rows[index1].Cells[2].FindControl("txtEmail_invite");
          Label control3 = (Label) this.gdInviteList.Rows[index1].Cells[0].FindControl("lblInviteID");
          DataRow row = dataTable2.NewRow();
          row["RowNumber"] = (object) (index2 + 1);
          row["InviteID"] = (object) control3.Text;
          row["Name"] = (object) control1.Text;
          row["Email"] = (object) control2.Text;
          row["Remove_image_visibility"] = (object) "True";
          ++index1;
          if (Regex.IsMatch(control2.Text, pattern))
          {
            if (flag)
              flag = true;
          }
          else
            flag = false;
          if (flag)
          {
            dataTable2.Rows.Add(row);
            dataTable2.AcceptChanges();
          }
        }
        if (flag)
        {
          DataRow row = dataTable2.NewRow();
          row["RowNumber"] = (object) (index1 + 1);
          row["InviteID"] = (object) 0;
          row["Name"] = (object) "";
          row["Email"] = (object) "";
          row["Remove_image_visibility"] = (object) "False";
          dataTable2.Rows.Add(row);
          dataTable2.AcceptChanges();
          for (int index3 = 0; index3 < dataTable2.Rows.Count; ++index3)
          {
            dataTable2.Rows[index3]["Remove_image_visibility"] = (object) "True";
            dataTable2.AcceptChanges();
          }
          dataTable2.Rows[dataTable1.Rows.Count - 1]["Remove_image_visibility"] = (object) "False";
          dataTable2.AcceptChanges();
          this.ViewState["InviteList"] = (object) dataTable2;
          this.gdInviteList.DataSource = (object) dataTable2;
          this.gdInviteList.DataBind();
        }
      }
    }
    this.SetPreviousData();
    string script = "jQuery(document).ready(function () {activateTab('tab_invitelist');});";
    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", script, true);
  }

  private void SetPreviousData()
  {
    int index1 = 0;
    if (this.ViewState["InviteList"] == null)
      return;
    DataTable dataTable = (DataTable) this.ViewState["InviteList"];
    if (dataTable.Rows.Count <= 0)
      return;
    for (int index2 = 0; index2 < dataTable.Rows.Count - 1; ++index2)
    {
      TextBox control1 = (TextBox) this.gdInviteList.Rows[index1].Cells[2].FindControl("txtName");
      TextBox control2 = (TextBox) this.gdInviteList.Rows[index1].Cells[2].FindControl("txtEmail_invite");
      Label control3 = (Label) this.gdInviteList.Rows[index1].Cells[0].FindControl("lblInviteID");
      control1.Text = dataTable.Rows[index2]["Name"].ToString();
      control2.Text = dataTable.Rows[index2]["Email"].ToString();
      control3.Text = dataTable.Rows[index2]["InviteID"].ToString();
      ++index1;
    }
  }

  protected void ButtonAdd_Click(object sender, ImageClickEventArgs e) => this.AddNewRowToGrid();

  protected void gdInviteList_RowCreated(object sender, GridViewRowEventArgs e)
  {
    if (e.Row.RowType != DataControlRowType.DataRow)
      return;
    DataTable dataTable = (DataTable) this.ViewState["InviteList"];
    ImageButton control = (ImageButton) e.Row.FindControl("LinkButton1");
    if (control == null)
      return;
    if (dataTable.Rows.Count > 0)
    {
      if (e.Row.RowIndex != dataTable.Rows.Count - 1)
        return;
      control.Visible = false;
    }
    else
      control.Visible = false;
  }

  protected void gdInviteList_RowDataBound(object sender, GridViewRowEventArgs e)
  {
    if (e.Row.RowType != DataControlRowType.DataRow)
      return;
    DataTable dataTable = (DataTable) this.ViewState["InviteList"];
    ImageButton control = (ImageButton) e.Row.FindControl("LinkButton1");
    if (control != null)
    {
      if (dataTable.Rows.Count > 0)
      {
        if (e.Row.RowIndex == dataTable.Rows.Count - 1)
          control.Visible = false;
      }
      else
        control.Visible = false;
    }
    control.Visible = true;
  }

  protected void LinkButton1_Click(object sender, ImageClickEventArgs e)
  {
    try
    {
      int rowIndex = ((GridViewRow) ((Control) sender).NamingContainer).RowIndex;
      if (this.ViewState["InviteList"] != null)
      {
        DataTable dataTable = (DataTable) this.ViewState["InviteList"];
        dataTable.Rows.Remove(dataTable.Rows[rowIndex]);
        dataTable.AcceptChanges();
        this.ViewState["InviteList"] = (object) dataTable;
        this.gdInviteList.DataSource = (object) dataTable;
        this.gdInviteList.DataBind();
      }
      this.SetPreviousData();
      string script = "jQuery(document).ready(function () {activateTab('tab_invitelist');});";
      this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", script, true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btnAssignToGroup_Click(object sender, EventArgs e)
  {
    try
    {
      bool flag = false;
      string str1 = "";
      string str2 = "";
      long user_id;
      if (this.txtUser.Text != "")
      {
        try
        {
          user_id = Convert.ToInt64(this.hdn_invitee.Value);
        }
        catch
        {
          user_id = 0L;
        }
      }
      else
        user_id = this.current_user.user_id;
      if (user_id == 0L)
      {
        if (Regex.IsMatch(this.txtUser.Text, "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*"))
        {
          str2 = this.txtUser.Text;
          flag = true;
        }
      }
      else
      {
        user user = this.users.get_user(user_id, this.current_user.account_id);
        if (user.user_id > 0L)
        {
          str1 = user.full_name;
          str2 = user.email;
          flag = true;
        }
      }
      if (flag)
      {
        DataTable dataTable = (DataTable) this.ViewState["InviteList"];
        DataRow row = dataTable.NewRow();
        row["RowNumber"] = (object) (dataTable.Rows.Count + 1);
        row["InviteID"] = (object) (dataTable.Rows.Count + 1);
        row["Name"] = (object) str1;
        row["Email"] = (object) str2;
        row["Remove_image_visibility"] = (object) "True";
        dataTable.Rows.Add(row);
        dataTable.AcceptChanges();
        this.ViewState["InviteList"] = (object) dataTable;
        this.gdInviteList.DataSource = (object) dataTable;
        this.gdInviteList.DataBind();
        this.txtUser.Text = "";
        this.hdn_invitee.Value = "0";
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    string script = "jQuery(document).ready(function () {activateTab('tab_invitelist');});";
    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", script, true);
  }
}
