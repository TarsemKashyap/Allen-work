// Decompiled with JetBrains decompiler
// Type: controls_view_mytask
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
using System.Web.UI.WebControls;
using System.Xml;

public class controls_view_mytask : fbs_base_user_control
{
  protected Label lblTaskName;
  protected Label lblRoom;
  protected Label lblCreatedOn;
  protected Label lblBookedFor;
  protected Label lblBookedFrom;
  protected Label lblBookedTo;
  protected HiddenField hidWorkflowID;
  protected TextBox txt_remarks;
  protected Panel pnl_reject_reason;
  protected Button btn_approved;
  protected Button btn_reject;
  protected Button btn_confirm_reject;
  protected Button btn_withdraw;
  protected Button close;
  private long workflow_id;
  public string html_message;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.workflow_id = Convert.ToInt64(this.Attributes["workflow_id"]);
      if (this.IsPostBack)
        return;
      this.hidWorkflowID.Value = this.workflow_id.ToString();
      if (this.gp.isAdminType)
      {
        this.btn_approved.Visible = true;
        this.btn_reject.Visible = true;
        this.btn_withdraw.Visible = false;
      }
      else if (this.gp.isSuperUserType)
      {
        this.btn_approved.Visible = true;
        this.btn_reject.Visible = true;
        this.btn_withdraw.Visible = false;
      }
      else
      {
        this.btn_approved.Visible = false;
        this.btn_reject.Visible = false;
        this.btn_withdraw.Visible = true;
      }
      this.populate_data(this.workflow_id);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_data(long workflow_id)
  {
    workflow_api workflowApi = new workflow_api();
    try
    {
      workflow workflow = workflowApi.get_workflow(workflow_id, this.current_user.account_id);
      XmlDocument xmlDocument = new XmlDocument();
      DataRow row1 = this.assets.get_assets(Convert.ToInt64(workflow.properties.SelectSingleNode("properties/asset_id").InnerText), this.current_user.account_id).Tables[0].Rows[0];
      this.lblRoom.Text = !string.IsNullOrEmpty(row1["code"].ToString()) ? row1["code"].ToString() + " / " + row1["name"].ToString() : row1["name"].ToString();
      DataRow row2 = this.bookings.get_booking_by_id(workflow.reference_id, this.current_user.account_id).Tables[0].Rows[0];
      this.lblTaskName.Text = !(workflow.action_type.ToString() == "1") || !(this.current_user.user_id.ToString() == workflow.created_by.ToString()) ? row2["purpose"].ToString() : row2["purpose"].ToString();
      this.lblCreatedOn.Text = workflow.created_on.ToString(api_constants.display_datetime_format);
      this.lblBookedFrom.Text = Convert.ToDateTime(row2["book_from"].ToString()).ToString(api_constants.display_datetime_format);
      this.lblBookedTo.Text = Convert.ToDateTime(row2["book_to"].ToString()).ToString(api_constants.display_datetime_format);
      this.lblBookedFor.Text = this.users.get_users(workflow.created_by, this.current_user.account_id).Tables[0].Rows[0]["full_name"].ToString();
      if (workflow.action_status <= (short) 0)
        return;
      this.btn_approved.Visible = false;
      this.btn_reject.Visible = false;
      this.btn_confirm_reject.Visible = false;
      this.html_message = "<div style='color:red;font-weight:bold;'>This booking has already been " + (workflow.action_status != (short) 2 ? "Approved" : "Rejected") + " by '" + this.users.get_user(workflow.action_taken_by, workflow.account_id).full_name + "' on '" + workflow.action_taken_on.ToString(api_constants.display_datetime_format) + "'.</div>";
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void close_Click(object sender, EventArgs e) => Modal.Close(this.Page, (object) "OK");

  protected void btn_approved_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect("mytask.aspx?workflow_id=" + (object) this.workflow_id + "&ap=1&modal=Y");
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_reject_Click(object sender, EventArgs e)
  {
    if (this.pnl_reject_reason.Visible)
      return;
    this.pnl_reject_reason.Visible = true;
    this.btn_confirm_reject.Visible = true;
    this.btn_reject.Visible = false;
    this.btn_approved.Visible = false;
  }

  protected void btn_withdraw_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect("mytask.aspx?workflow_id=" + (object) this.workflow_id + "&wd=1&modal=Y");
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_confirm_reject_Click(object sender, EventArgs e)
  {
    try
    {
      workflow_api workflowApi = new workflow_api();
      this.Session["workflow_rejected"] = (object) "0";
      workflow workflow = workflowApi.get_workflow(Convert.ToInt64(this.hidWorkflowID.Value), this.current_user.account_id);
      workflow.action_status = workflowApi.get_action_status("Rejected");
      workflow.action_taken_by = this.current_user.user_id;
      workflow.action_remarks = this.txt_remarks.Text;
      workflow.modified_by = this.current_user.user_id;
      workflow work = workflowApi.update_workflow(workflow);
      if (work.workflow_id > 0L)
      {
        if (work.action_type == (short) 3 || work.action_type == (short) 2)
        {
          this.update_booking_info(work.reference_id, this.bookings.get_status("Rejected"), work.action_remarks);
          this.add_message_log(work.reference_id, "email_transfer_request_rejected", work.action_remarks);
        }
        else
        {
          this.update_booking_info(work.reference_id, this.bookings.get_status("Rejected"), work.action_remarks);
          asset_booking booking = this.bookings.get_booking(work.reference_id, work.account_id);
          booking.global_appointment_id = this.bookings.get_appointment_id(booking.booking_id, booking.account_id);
          if (Convert.ToBoolean(this.current_account.properties["send_email"]))
            this.bookingsbl.send_booking_request_workflow_email(booking, false, work);
        }
        this.Session["workflow_rejected"] = (object) "1";
      }
      this.Session.Remove("inbx");
      Modal.Close(this.Page, (object) "OK");
      this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "Mytask();", true);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void update_booking_info(long booking_id, int booking_status, string rejected_remarks)
  {
    try
    {
      asset_booking assetBooking = new asset_booking();
      asset_booking booking = this.bookings.get_booking(booking_id, this.current_user.account_id);
      booking.status = Convert.ToInt16(booking_status);
      booking.modified_by = this.current_user.user_id;
      assetBooking = this.bookings.update_booking_status(booking);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void add_message_log(
    long booking_id,
    string email_template_name,
    string rejected_remarks)
  {
    try
    {
      DataSet bookingById = this.bookings.get_booking_by_id(booking_id, this.current_user.account_id);
      long user_id = 0;
      long booking_id1 = 0;
      if (this.utilities.isValidDataset(bookingById))
      {
        booking_id1 = Convert.ToInt64(bookingById.Tables[0].Rows[0]["transfer_original_booking_id"].ToString());
        if (this.utilities.isValidDataset(this.bookings.get_booking_by_id(booking_id1, this.current_user.account_id)))
          user_id = Convert.ToInt64(bookingById.Tables[0].Rows[0]["booked_for"].ToString());
      }
      email email = new email();
      asset_booking assetBooking1 = new asset_booking();
      asset_booking booking1 = this.bookings.get_booking(booking_id, this.current_user.account_id);
      asset_booking assetBooking2 = new asset_booking();
      asset_booking booking2 = this.bookings.get_booking(booking_id1, this.current_user.account_id);
      StringBuilder stringBuilder = new StringBuilder();
      user user1 = this.users.get_user(booking1.booked_for, this.current_user.account_id);
      user user2 = this.users.get_user(user_id, this.current_user.account_id);
      DataSet settings = this.settings.get_settings(this.current_user.account_id);
      DataRow[] dataRowArray = settings.Tables[0].Select("parameter='from_email_address'");
      template template = this.tapi.get_template(email_template_name, this.current_user.account_id);
      email.message_id = 0L;
      email.account_id = this.current_user.account_id;
      email.created_by = this.current_user.user_id;
      email.created_on = this.current_timestamp;
      email.modified_by = this.current_user.user_id;
      email.modified_on = this.current_timestamp;
      email.record_id = booking1.record_id;
      email.email_message_id = Guid.NewGuid();
      email.subject = template.title;
      switch (email_template_name)
      {
        case "email_booking_request_rejected":
          email.body = this.GetOriginalEmailContent(template.content_data, user1, booking1, settings, rejected_remarks, false, false);
          break;
        case "":
          email.body = this.GetOriginalEmailContent(template.content_data, user1, booking1, settings, rejected_remarks, false, false);
          break;
        default:
          email.body = this.GetOriginalEmailContentForTransfer(template.content_data, bookingById, booking2, user1, user2, settings, false);
          break;
      }
      email.from_msg = dataRowArray[0]["value"].ToString();
      email.to_msg = booking1.email;
      email.cc_msg = "";
      email.bcc_msg = "";
      email.is_html = true;
      email.sent = false;
      email.bounced = false;
      email.message = "";
      email.sent_on = this.current_timestamp;
      email.message_type = 1;
      email.failed_attempts = 0;
      email.last_attempted_on = this.current_timestamp;
      this.eapi.update_email(email);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private string GetOriginalEmailContent(
    string email_content,
    user objBookedFor,
    asset_booking objBookingData,
    DataSet setting_data,
    string rejected_remarks,
    bool showCancelLink,
    bool isSetupType)
  {
    try
    {
      string fullName = objBookedFor.full_name;
      string newValue1 = this.tzapi.convert_to_user_timestamp(objBookingData.created_on).ToString(api_constants.display_datetime_format);
      string contact = objBookingData.contact;
      string email = objBookingData.email;
      string purpose = objBookingData.purpose;
      string status = this.bookings.get_status((long) objBookingData.status);
      string newValue2 = Convert.ToDateTime(objBookingData.book_from).ToString(api_constants.display_datetime_format) + " to " + Convert.ToDateTime(objBookingData.book_to).ToString(api_constants.display_datetime_format);
      Dictionary<string, user_property> properties = objBookedFor.properties;
      string newValue3 = properties.ContainsKey("staff_division") ? properties["staff_division"].property_value : "";
      string newValue4 = properties.ContainsKey("staff_department") ? properties["staff_department"].property_value : "";
      string newValue5 = properties.ContainsKey("staff_section") ? properties["staff_section"].property_value : "";
      asset asset = this.assets.get_asset(objBookingData.asset_id, objBookingData.account_id);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr style='background-color:#DDD;'>");
      stringBuilder.Append("<th>Building</th>");
      stringBuilder.Append("<th>Level</th>");
      stringBuilder.Append("<th>Room</th>");
      stringBuilder.Append("<th>Category</th>");
      stringBuilder.Append("<th>Capacity</th>");
      stringBuilder.Append("<th>Remarks</th>");
      if (showCancelLink)
        stringBuilder.Append("<th>Cancel</th>");
      if (isSetupType)
        stringBuilder.Append("<th>Setup Type</th>");
      stringBuilder.Append("<th>View</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr class='odd gradeX'>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.building.value);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.level.value);
      if (string.IsNullOrEmpty(asset.code))
        stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.name);
      else
        stringBuilder.AppendFormat("<td>{0} / {1}</td>", (object) asset.code, (object) asset.name);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.category.value);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.capacity);
      string str1 = "";
      foreach (long key in asset.asset_properties.Keys)
      {
        asset_property assetProperty = asset.asset_properties[key];
        if (assetProperty.property_name == "asset_property")
          str1 = str1 + this.utilities.get_setting_value(setting_data.Tables[0], assetProperty.property_value) + " - " + assetProperty.remarks + "<br/>";
      }
      stringBuilder.AppendFormat("<td>{0}</td>", (object) str1);
      string str2 = this.site_full_path + "/booking_cancel.aspx?id=" + (object) objBookingData.booking_id;
      string str3 = this.site_full_path + "/booking_view.aspx?id=" + (object) objBookingData.booking_id;
      if (showCancelLink)
        stringBuilder.AppendFormat("<td><a href='{0}'/>Cancel</td>", (object) str2);
      if (isSetupType)
        stringBuilder.AppendFormat("<td>{0}</td>", (object) this.utilities.get_setting_value(setting_data.Tables[0], objBookingData.setup_type.ToString()));
      stringBuilder.AppendFormat("<td><a href='{0}'/>View</td>", (object) str3);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      string newValue6 = stringBuilder.ToString();
      email_content = email_content.Replace("[FULL NAME]", fullName);
      email_content = email_content.Replace("[PURPOSE]", purpose);
      email_content = email_content.Replace("[BOOKED RANGE]", newValue2);
      email_content = email_content.Replace("[BOOKED STATUS]", status);
      email_content = email_content.Replace("[REQUESTED BY]", fullName);
      email_content = email_content.Replace("[REQUESTED ON]", newValue1);
      email_content = email_content.Replace("[REQUESTOR DIVISION]", newValue3);
      email_content = email_content.Replace("[REQUESTOR DEPARTMENT]", newValue4);
      email_content = email_content.Replace("[REQUESTOR SECTION]", newValue5);
      email_content = email_content.Replace("[CONTACT NO]", contact);
      email_content = email_content.Replace("[EMAILS]", email);
      email_content = email_content.Replace("[FACILITY DETAILS]", newValue6);
      email_content = email_content.Replace("[REJECTION REASON]", rejected_remarks);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
    return email_content;
  }

  private string GetOriginalEmailContentForTransfer(
    string email_content,
    DataSet transfer_booking_data,
    asset_booking objOriginalBookingData,
    user objTransferBookedFor,
    user objOriginalBookedFor,
    DataSet setting_data,
    bool showCancelLink)
  {
    try
    {
      string newValue1 = "";
      Guid account_id = (Guid) transfer_booking_data.Tables[0].Rows[0]["account_id"];
      user user = this.users.get_user(Convert.ToInt64(transfer_booking_data.Tables[0].Rows[0]["booked_for"].ToString()), account_id);
      if (user.full_name != "")
        newValue1 = user.full_name;
      string newValue2 = this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(transfer_booking_data.Tables[0].Rows[0]["created_on"].ToString())).ToString(api_constants.display_datetime_format);
      string newValue3 = transfer_booking_data.Tables[0].Rows[0]["contact"].ToString();
      string newValue4 = transfer_booking_data.Tables[0].Rows[0]["email"].ToString();
      string newValue5 = transfer_booking_data.Tables[0].Rows[0]["purpose"].ToString();
      string status1 = this.bookings.get_status(Convert.ToInt64(transfer_booking_data.Tables[0].Rows[0]["status"].ToString()));
      string newValue6 = Convert.ToDateTime(transfer_booking_data.Tables[0].Rows[0]["book_from"].ToString()).ToString(api_constants.display_datetime_format) + " to " + Convert.ToDateTime(transfer_booking_data.Tables[0].Rows[0]["book_to"].ToString()).ToString(api_constants.display_datetime_format);
      string fullName = objOriginalBookedFor.full_name;
      string newValue7 = this.tzapi.convert_to_user_timestamp(objOriginalBookingData.created_on).ToString(api_constants.display_datetime_format);
      string contact = objOriginalBookingData.contact;
      string email = objOriginalBookingData.email;
      string purpose = objOriginalBookingData.purpose;
      string status2 = this.bookings.get_status((long) objOriginalBookingData.status);
      string newValue8 = Convert.ToDateTime(objOriginalBookingData.book_from).ToString(api_constants.display_datetime_format) + " to " + Convert.ToDateTime(objOriginalBookingData.book_to).ToString(api_constants.display_datetime_format);
      Dictionary<string, user_property> properties1 = objOriginalBookedFor.properties;
      string newValue9 = properties1.ContainsKey("staff_division") ? properties1["staff_division"].property_value : "";
      string newValue10 = properties1.ContainsKey("staff_department") ? properties1["staff_department"].property_value : "";
      string newValue11 = properties1.ContainsKey("staff_section") ? properties1["staff_section"].property_value : "";
      Dictionary<string, user_property> properties2 = objTransferBookedFor.properties;
      string newValue12 = properties2.ContainsKey("staff_division") ? properties2["staff_division"].property_value : "";
      string newValue13 = properties2.ContainsKey("staff_department") ? properties2["staff_department"].property_value : "";
      string newValue14 = properties2.ContainsKey("staff_section") ? properties2["staff_section"].property_value : "";
      asset asset = this.assets.get_asset(objOriginalBookingData.asset_id, objOriginalBookingData.account_id);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr style='background-color:#DDD;'>");
      stringBuilder.Append("<th>Building</th>");
      stringBuilder.Append("<th>Level</th>");
      stringBuilder.Append("<th>Room</th>");
      stringBuilder.Append("<th>Category</th>");
      stringBuilder.Append("<th>Capacity</th>");
      stringBuilder.Append("<th>Remarks</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr class='odd gradeX'>");
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.building.value);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.level.value);
      if (string.IsNullOrEmpty(asset.code))
        stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.name);
      else
        stringBuilder.AppendFormat("<td>{0} / {1}</td>", (object) asset.code, (object) asset.name);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.category.value);
      stringBuilder.AppendFormat("<td>{0}</td>", (object) asset.capacity);
      string str = "";
      foreach (long key in asset.asset_properties.Keys)
      {
        asset_property assetProperty = asset.asset_properties[key];
        if (assetProperty.property_name == "asset_property" && !string.IsNullOrWhiteSpace(assetProperty.remarks))
          str = str + this.utilities.get_setting_value(setting_data.Tables[0], assetProperty.property_value) + " - " + assetProperty.remarks + "<br/>";
      }
      stringBuilder.AppendFormat("<td>{0}</td>", (object) str);
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      string newValue15 = stringBuilder.ToString();
      email_content = email_content.Replace("[FULL NAME]", newValue1);
      email_content = email_content.Replace("[REJECTION REASON]", this.txt_remarks.Text);
      email_content = email_content.Replace("[NEW PURPOSE]", newValue5);
      email_content = email_content.Replace("[NEW BOOKED RANGE]", newValue6);
      email_content = email_content.Replace("[NEW BOOKED STATUS]", status1);
      email_content = email_content.Replace("[NEW REQUESTED BY]", newValue1);
      email_content = email_content.Replace("[NEW REQUESTED ON]", newValue2);
      email_content = email_content.Replace("[NEW REQUESTOR DIVISION]", newValue12);
      email_content = email_content.Replace("[NEW REQUESTOR DEPARTMENT]", newValue13);
      email_content = email_content.Replace("[NEW REQUESTOR SECTION]", newValue14);
      email_content = email_content.Replace("[NEW CONTACT NO]", newValue3);
      email_content = email_content.Replace("[NEW EMAILS]", newValue4);
      email_content = email_content.Replace("[OLD PURPOSE]", purpose);
      email_content = email_content.Replace("[OLD BOOKED RANGE]", newValue8);
      email_content = email_content.Replace("[OLD BOOKED STATUS]", status2);
      email_content = email_content.Replace("[OLD REQUESTED BY]", fullName);
      email_content = email_content.Replace("[OLD REQUESTED ON]", newValue7);
      email_content = email_content.Replace("[OLD REQUESTOR DIVISION]", newValue9);
      email_content = email_content.Replace("[OLD REQUESTOR DEPARTMENT]", newValue10);
      email_content = email_content.Replace("[OLD REQUESTOR SECTION]", newValue11);
      email_content = email_content.Replace("[OLD CONTACT NO]", contact);
      email_content = email_content.Replace("[OLD EMAILS]", email);
      email_content = email_content.Replace("[FACILITY DETAILS]", newValue15);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
    return email_content;
  }
}
