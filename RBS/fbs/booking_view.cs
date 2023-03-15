// Decompiled with JetBrains decompiler
// Type: booking_view
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class booking_view : fbs_base_page, IRequiresSessionState
{
  protected HtmlHead Head1;
  protected Literal litInfoMsg;
  protected HtmlGenericControl alertInfo;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Label lbl_assetname_heading;
  protected PlaceHolder control_booking_view;
  protected HiddenField hdnID;
  protected HiddenField hdnRecID;
  protected CheckBox ckb_cancel;
  protected HtmlGenericControl div_cancel_series;
  protected TextBox txt_cancel_reason;
  protected Button btn_confirm_cancel;
  protected Button btn_cancel_confirm;
  protected Panel pnl_cancel;
  protected Button btn_start_meeting;
  protected Button btn_transfer;
  protected Button btn_cancel_booking;
  protected Button btn_actualize;
  protected Button btn_noshow;
  protected Button btn_edit_booking;
  protected Button btn_addroom;
  protected Button btn_re_assign;
  protected HtmlAnchor btn_additional_resource;
  protected HtmlAnchor btn_clone;
  protected Button btn_cancel;
  protected HiddenField hdn_repeat_reference_id;
  protected HtmlForm form_sample_2;
  public long booking_id;
  private string booking_id_string = "";
  private DataSet settings_data;
  private asset_booking objBooking;
  private bool resource_booking_enabled = true;
  private bool is_special_user;
  private Dictionary<string, string> selectedDates = new Dictionary<string, string>();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    asset_booking assetBooking = (asset_booking) this.Session["AssetBooking"];
    this.resource_booking_enabled = Convert.ToBoolean(this.current_account.properties["resource_booking"]);
    try
    {
      try
      {
        this.booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        this.booking_id = 0L;
      }
      if (this.booking_id > 0L)
      {
        this.hdnID.Value = this.booking_id.ToString();
        this.objBooking = this.bookings.get_booking(this.booking_id, this.current_user.account_id);
        this.hdn_repeat_reference_id.Value = this.objBooking.repeat_reference_id.ToString();
        if (this.objBooking.is_repeat)
        {
          foreach (DataRow row in (InternalDataCollectionBase) this.bookings.get_repeat_bookings(this.objBooking.repeat_reference_id, this.objBooking.account_id).Tables[0].Rows)
            this.booking_id_string = this.booking_id_string + "," + row["booking_id"].ToString();
        }
        else
          this.booking_id_string = this.booking_id.ToString();
        asset asset = this.assets.get_asset(this.objBooking.asset_id, this.current_user.account_id);
        user user = new user();
        user userObj = this.objBooking.booked_for == this.current_user.user_id ? this.current_user : this.users.get_user(this.objBooking.booked_for, this.objBooking.account_id);
        this.is_special_user = this.u_group.group_type == 1 || this.objBooking.booked_for == this.current_user.user_id || this.objBooking.created_by == this.current_user.user_id || asset.owner_group.group_id == this.u_group.group_id;
        this.hdnRecID.Value = this.objBooking.record_id.ToString();
        this.lbl_assetname_heading.Text = !string.IsNullOrEmpty(asset.code.ToString()) ? asset.code.ToString() + "/" + asset.name.ToString() : asset.name.ToString();
        UserControl userControl = new UserControl();
        UserControl child = (UserControl) this.LoadControl("controls/booking_view.ascx");
        child.Attributes.Add("booking_id", this.booking_id.ToString());
        child.Attributes.Add("booking_ids", this.booking_id_string);
        if (this.booking_id_string != this.booking_id.ToString())
          child.Attributes.Add("custombooking", "true");
        this.control_booking_view.Controls.Add((Control) child);
        this.hide_all_buttons();
        if (this.u_group.group_type == 0)
          return;
        if (this.Request.QueryString["Details"] != null && this.Request.QueryString["Details"] == "N")
          this.hide_all_buttons();
        if (this.is_special_user && this.objBooking.status != (short) 0)
        {
          this.btn_clone.HRef = "booking_wizard.aspx?cloneID=" + this.objBooking.booking_id.ToString();
          this.btn_clone.Visible = true;
        }
        switch (this.objBooking.status)
        {
          case 0:
            return;
          case 1:
            this.process_book(this.objBooking, asset, this.current_user, this.u_group, userObj);
            break;
          case 2:
            return;
          case 4:
            return;
          case 5:
            return;
          case 6:
            return;
          case 7:
            return;
        }
      }
      if (!Convert.ToBoolean(this.current_account.properties["clone"]))
        this.btn_clone.Visible = false;
      if (!Convert.ToBoolean(this.current_account.properties["add_room"]))
        this.btn_addroom.Visible = false;
      if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
        this.btn_additional_resource.Visible = false;
      if (Convert.ToBoolean(this.current_account.properties["reassign"]))
        return;
      this.btn_re_assign.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("booking_view page_load error: " + ex.ToString()));
      this.Response.Write("ASPX-" + ex.ToString());
    }
  }

  private void process_resource_booking(asset obj_asset, asset_booking book)
  {
    string str = !book.is_repeat ? "&isrepeat=false" : "&isrepeat=true";
    if (this.is_special_user)
      this.btn_additional_resource.Visible = this.check_asset_addresource(obj_asset);
    if (!this.btn_additional_resource.Visible)
      return;
    this.btn_additional_resource.HRef = "~/additional_resources/request_resources.aspx?booking_id=" + this.objBooking.booking_id.ToString() + str;
  }

  private void process_book(
    asset_booking book,
    asset room,
    user current_user,
    user_group grp,
    user userObj)
  {
    double num1 = -1.0;
    double num2 = -1.0;
    bool flag1 = false;
    foreach (long key in room.asset_properties.Keys)
    {
      asset_property assetProperty = room.asset_properties[key];
      if (assetProperty.property_name == "cancel_window_before")
        num1 = Convert.ToDouble(assetProperty.property_value);
      if (assetProperty.property_name == "cancel_window_after")
        num2 = Convert.ToDouble(assetProperty.property_value);
    }
    this.settings_data = this.settings.view_settings(current_user.account_id);
    if (num1 == -1.0)
    {
      DataRow[] dataRowArray = this.settings_data.Tables[0].Select("parameter='cancel_before'");
      num1 = dataRowArray.Length <= 0 ? 0.0 : Convert.ToDouble(dataRowArray[0]["value"]);
    }
    if (num2 == -1.0)
    {
      DataRow[] dataRowArray = this.settings_data.Tables[0].Select("parameter='cancel_after'");
      num2 = dataRowArray.Length <= 0 ? 0.0 : Convert.ToDouble(dataRowArray[0]["value"]);
    }
    bool flag2 = false;
    bool flag3 = false;
    if (this.current_timestamp >= book.book_from)
      flag2 = true;
    if (this.current_timestamp >= book.book_to)
      flag3 = true;
    if (this.is_special_user)
      this.btn_clone.Visible = true;
    if (flag3)
      return;
    if (flag2)
    {
      if ((this.current_timestamp - book.book_from).TotalMinutes <= num2)
        flag1 = true;
      else if (grp.group_type == 1 || room.owner_group.group_id == grp.group_id)
        flag1 = true;
      if (flag1)
        this.btn_cancel_booking.Visible = true;
      if ((book.book_to - this.current_timestamp).TotalMinutes > 15.0 && this.is_special_user)
        this.btn_actualize.Visible = true;
      if ((book.book_to - this.current_timestamp).TotalMinutes > 15.0)
        this.btn_noshow.Visible = true;
      this.process_start_meeting(book, this.settings_data);
    }
    else if (this.is_special_user)
    {
      if ((book.book_from - this.current_timestamp).TotalMinutes <= num1)
      {
        if (grp.group_type == 1 || room.owner_group.group_id == grp.group_id)
          this.btn_cancel_booking.Visible = true;
      }
      else if (this.is_special_user)
        this.btn_cancel_booking.Visible = true;
      this.btn_edit_booking.Visible = true;
      this.btn_clone.Visible = true;
      this.btn_addroom.Visible = true;
      if (book.book_from > this.current_timestamp)
        this.btn_re_assign.Visible = true;
      else
        this.btn_re_assign.Visible = false;
    }
    if (!this.resource_booking_enabled || flag3)
      return;
    this.process_resource_booking(room, book);
  }

  private void process_start_meeting(asset_booking book, DataSet settings_data)
  {
    bool flag = false;
    double num = 0.0;
    DataRow[] dataRowArray1 = settings_data.Tables[0].Select("parameter='allow_start_meeting'");
    if (dataRowArray1.Length > 0)
      flag = Convert.ToBoolean(dataRowArray1[0]["value"]);
    if (!flag)
      return;
    DataRow[] dataRowArray2 = settings_data.Tables[0].Select("parameter='auto_release_cutoff'");
    if (dataRowArray2.Length > 0)
      num = Convert.ToDouble(dataRowArray2[0]["value"]);
    if (this.bookings.get_usage(book.booking_id, book.account_id).usage_id != 0L || !(this.current_timestamp > book.book_from) || (this.current_timestamp - book.book_from).TotalMinutes > num)
      return;
    this.btn_start_meeting.Visible = true;
  }

  private void hide_all_buttons()
  {
    this.btn_actualize.Visible = false;
    this.btn_cancel_booking.Visible = false;
    this.btn_edit_booking.Visible = false;
    this.btn_noshow.Visible = false;
    this.btn_transfer.Visible = false;
    this.btn_re_assign.Visible = false;
    this.btn_clone.Visible = false;
    this.btn_addroom.Visible = false;
    this.btn_start_meeting.Visible = false;
    if (Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      return;
    this.btn_additional_resource.Visible = false;
  }

  private bool check_asset_addresource(asset objAsset)
  {
    if (objAsset.asset_id != 0L)
    {
      foreach (KeyValuePair<long, asset_property> assetProperty1 in objAsset.asset_properties)
      {
        asset_property assetProperty2 = assetProperty1.Value;
        if (assetProperty2.property_name == "request_additional_resources" && assetProperty2.property_value == "1")
          return true;
      }
    }
    return false;
  }

  protected void btn_transfer_Click(object sender, EventArgs e)
  {
    try
    {
      if (this.bookings.Check_isTransferBooking(this.booking_id, this.current_user.account_id))
      {
        this.litInfoMsg.Text = "";
        this.litErrorMsg.Text = Resources.fbs.validateTransfer_booking;
        this.alertError.Visible = true;
        this.alertInfo.Visible = false;
        this.alertError.Attributes.Add("style", "display:block");
        this.alertInfo.Attributes.Add("style", "display:none");
      }
      else
      {
        this.litInfoMsg.Text = "";
        this.litErrorMsg.Text = "";
        this.alertError.Visible = false;
        this.alertInfo.Visible = false;
        this.alertError.Attributes.Add("style", "display:none");
        this.alertInfo.Attributes.Add("style", "display:none");
        this.Response.Redirect("booking_transfer.aspx?id=" + (object) this.booking_id);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_booking_Click(object sender, EventArgs e) => this.Response.Redirect("booking_cancel.aspx?id=" + (object) this.booking_id);

  protected void btn_edit_Click(object sender, EventArgs e)
  {
    string str = "advanced_booking.aspx?id=" + (object) this.booking_id + "&repeat=" + this.hdn_repeat_reference_id.Value;
    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "msgbox", "window.top.location.href='" + str + "'", true);
  }

  protected void btn_re_assign_Click(object sender, EventArgs e) => this.Response.Redirect("booking_reassign.aspx?id=" + (object) this.booking_id);

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    this.Session["SelectedDates"] = (object) "";
    this.Session.Remove("SelectedDates");
    Modal.Close((Page) this, (object) "OK");
  }

  protected void btn_actualize_Click(object sender, EventArgs e)
  {
    try
    {
      string bookingIdString = this.booking_id_string;
      if ((this.objBooking.book_to - this.current_timestamp).TotalMinutes <= this.AllowedMinutes)
        return;
      string endDT = this.utilities.TimeRoundUp(this.current_timestamp).ToString(api_constants.sql_datetime_format);
      if (string.IsNullOrEmpty(bookingIdString))
      {
        if (this.bookings.actualize_booking(this.booking_id, this.current_user.account_id, endDT))
        {
          this.litInfoMsg.Text = Resources.fbs.booking_actuallized_successfully;
          this.litErrorMsg.Text = "";
          this.alertError.Visible = false;
          this.alertInfo.Visible = true;
          this.alertError.Attributes.Add("style", "display:none");
          this.alertInfo.Attributes.Add("style", "display:block");
        }
        else
        {
          this.litInfoMsg.Text = "";
          this.litErrorMsg.Text = endDT;
          this.alertError.Visible = true;
          this.alertInfo.Visible = false;
          this.alertError.Attributes.Add("style", "display:block");
          this.alertInfo.Attributes.Add("style", "display:none");
        }
      }
      else
      {
        int index = 0;
        while (true)
        {
          if (index < bookingIdString.Split(',').Length)
          {
            if (bookingIdString.Split(',')[index].ToString() != "")
            {
              if (this.bookings.actualize_booking(Convert.ToInt64(bookingIdString.Split(',')[index]), this.current_user.account_id, endDT))
              {
                this.litInfoMsg.Text = Resources.fbs.booking_actuallized_successfully;
                this.litErrorMsg.Text = "";
                this.alertError.Visible = false;
                this.alertInfo.Visible = true;
                this.alertError.Attributes.Add("style", "display:none");
                this.alertInfo.Attributes.Add("style", "display:block");
              }
              else
              {
                this.litInfoMsg.Text = "";
                this.litErrorMsg.Text = endDT;
                this.alertError.Visible = true;
                this.alertInfo.Visible = false;
                this.alertError.Attributes.Add("style", "display:block");
                this.alertInfo.Attributes.Add("style", "display:none");
              }
            }
            ++index;
          }
          else
            break;
        }
      }
      this.btn_actualize.Visible = false;
      this.btn_cancel_booking.Visible = false;
      this.btn_edit_booking.Visible = false;
      this.btn_clone.Visible = false;
      this.btn_noshow.Visible = false;
      this.btn_transfer.Visible = false;
      this.btn_re_assign.Visible = false;
      Modal.Close((Page) this, (object) "OK");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_noshow_Click(object sender, EventArgs e)
  {
    asset_booking assetBooking1 = new asset_booking();
    try
    {
      assetBooking1.account_id = this.current_user.account_id;
      assetBooking1.booking_id = Convert.ToInt64(this.hdnID.Value);
      assetBooking1.modified_by = this.current_user.user_id;
      assetBooking1.status = Convert.ToInt16(this.bookings.get_status("No Show"));
      assetBooking1.record_id = new Guid(this.hdnRecID.Value);
      asset_booking assetBooking2 = this.bookings.update_booking_status(assetBooking1);
      if (assetBooking2.booking_id <= 0L)
        return;
      asset_booking booking = this.bookings.get_booking(assetBooking2.booking_id, this.current_user.account_id);
      List<asset_booking> bookings = new List<asset_booking>();
      bookings.Add(booking);
      if (Convert.ToBoolean(this.current_account.properties["send_email"]))
        this.bookingsbl.send_booking_emails(bookings);
      this.alertInfo.Attributes.Add("style", "display: block;");
      this.litInfoMsg.Text = Resources.fbs.no_show_msg;
      this.btn_noshow.Visible = false;
      this.btn_actualize.Visible = false;
      this.btn_addroom.Visible = false;
      this.btn_edit_booking.Visible = false;
      this.btn_re_assign.Visible = false;
      this.btn_transfer.Visible = false;
      this.btn_cancel_booking.Visible = false;
      this.btn_additional_resource.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("error -->  " + ex.ToString()));
    }
  }

  protected void btn_addroom_Click(object sender, EventArgs e) => this.Response.Redirect("add_rooms.aspx?id=" + (object) this.booking_id);

  protected void btn_start_meeting_Click(object sender, EventArgs e)
  {
    book_usage bookUsage = new book_usage();
    bookUsage.usage_id = 0L;
    bookUsage.account_id = this.current_user.account_id;
    bookUsage.occupied = true;
    bookUsage.occupied_by = this.current_user.user_id;
    bookUsage.occupied_on = this.current_timestamp;
    bookUsage.booking_id = Convert.ToInt64(this.hdnID.Value);
    if (this.bookings.update_usage(bookUsage).usage_id <= 0L)
      return;
    this.btn_start_meeting.Visible = false;
  }

  protected void btn_confirm_cancel_Click(object sender, EventArgs e)
  {
    bool flag = false;
    asset_booking booking1 = this.bookings.get_booking(Convert.ToInt64(this.hdnID.Value), this.current_user.account_id);
    booking1.global_appointment_id = this.bookings.get_appointment_id(booking1.booking_id, booking1.account_id);
    List<asset_booking> bookings = new List<asset_booking>();
    if (!this.ckb_cancel.Checked)
    {
      try
      {
        booking1.cancel_reason = this.txt_cancel_reason.Text;
        booking1.status = (short) 0;
        booking1.modified_by = this.current_user.user_id;
        booking1.cancel_by = this.current_user.user_id;
        booking1.cancel_on = this.tzapi.current_user_timestamp();
        ++booking1.sequence;
        asset_booking assetBooking = this.bookings.cancel_booking(booking1);
        this.bookings.set_cancel_status(assetBooking);
        flag = true;
        bookings.Add(assetBooking);
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("error -->  " + ex.ToString()));
        flag = false;
      }
    }
    else
    {
      DataSet repeatBookings = this.bookings.get_repeat_bookings(booking1.repeat_reference_id, this.current_account.account_id);
      try
      {
        foreach (DataRow row in (InternalDataCollectionBase) repeatBookings.Tables[0].Rows)
        {
          asset_booking booking2 = this.bookings.get_booking(Convert.ToInt64(row["booking_id"]), this.current_user.account_id);
          booking2.global_appointment_id = this.bookings.get_appointment_id(booking2.booking_id, booking2.account_id);
          if (booking2.book_from >= booking1.book_from)
          {
            booking2.cancel_reason = this.txt_cancel_reason.Text;
            booking2.status = (short) 0;
            booking2.modified_by = this.current_user.user_id;
            booking2.cancel_by = this.current_user.user_id;
            booking2.cancel_on = this.tzapi.current_user_timestamp();
            ++booking1.sequence;
            asset_booking assetBooking = this.bookings.cancel_booking(booking2);
            this.bookings.set_cancel_status(assetBooking);
            flag = true;
            bookings.Add(assetBooking);
          }
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("error -->  " + ex.ToString()));
        flag = false;
      }
    }
    if (flag)
    {
      try
      {
        foreach (asset_booking assetBooking in bookings)
        {
          DataSet resourceBookings = this.resapi.get_resource_bookings(assetBooking.booking_id, this.current_user.account_id, this.str_resource_module);
          if (this.utilities.isValidDataset(resourceBookings))
          {
            foreach (DataRow row in (InternalDataCollectionBase) resourceBookings.Tables[0].Rows)
            {
              long resource_booking_id;
              try
              {
                resource_booking_id = Convert.ToInt64(row["resource_booking_id"]);
              }
              catch
              {
                resource_booking_id = 0L;
              }
              if (resource_booking_id > 0L)
                this.resapi.delete_resource_bookings(resource_booking_id, this.current_user.account_id, this.current_user.user_id);
            }
          }
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("error -->  " + ex.ToString()));
        this.alertError.Attributes.Add("style", "display: block;");
        this.litErrorMsg.Text = Resources.fbs.cancel_booking_error_msg;
        this.btn_cancel_booking.Visible = false;
        return;
      }
      try
      {
        this.bookingsbl.update_outlook(bookings);
      }
      catch (Exception ex)
      {
      }
      try
      {
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          this.bookingsbl.send_booking_emails(bookings);
        this.Response.Redirect("booking_view.aspx?id=" + this.hdnID.Value);
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("error -->  " + ex.ToString()));
      }
    }
    else
    {
      this.alertError.Attributes.Add("style", "display: block;");
      this.litErrorMsg.Text = Resources.fbs.cancel_booking_error_msg;
      this.btn_cancel_booking.Visible = false;
    }
  }

  protected void btn_cancel_confirm_Click(object sender, EventArgs e)
  {
  }
}
