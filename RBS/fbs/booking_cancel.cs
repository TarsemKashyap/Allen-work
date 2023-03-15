// Decompiled with JetBrains decompiler
// Type: booking_cancel
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class booking_cancel : fbs_base_page, IRequiresSessionState
{
  protected HtmlHead Head1;
  protected Literal litInfoMsg;
  protected HtmlGenericControl alertInfo;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected HiddenField hdn_selected;
  protected HtmlInputText lbl_purpose;
  protected HtmlInputText lbl_bookedfor;
  protected HtmlInputText lbl_email;
  protected HtmlInputText lbl_telephone;
  protected HtmlTextArea lbl_remarks;
  protected HtmlInputText lbl_housekeeping;
  protected HtmlGenericControl div_housekeeping;
  protected HtmlInputText lbl_Setup;
  protected HtmlGenericControl div_manpower;
  protected HtmlInputText lbl_setupetype;
  protected HtmlGenericControl div_setup;
  protected HtmlInputText lbl_meetingtype;
  protected HtmlGenericControl div_meetingtype;
  protected HtmlInputText lbl_requestedBy;
  protected HtmlInputText txt_rejectedreason;
  protected HtmlGenericControl Div_Rej_reason;
  protected HtmlGenericControl h4_invite;
  protected HtmlGenericControl contrlgrp_invite;
  protected HtmlGenericControl h4_resource;
  protected HtmlGenericControl contrlgrp_resource;
  protected HiddenField hdn_asset_field;
  protected HiddenField hdnBookingID;
  protected HtmlTextArea txt_reason;
  protected HtmlGenericControl reason_error;
  protected HtmlGenericControl div_cancelreasons;
  protected HtmlAnchor link_cancel_selected;
  protected Button btn_cancel;
  protected LinkButton thisisabugfix;
  private long booking_id;
  private long asset_id;
  private DateTime startdatetime;
  private DateTime enddatetime;
  private DataSet setting_data;
  private DataSet asset_pro_ds;
  public string html_asset;
  public string html_resourcelist;
  public string html_invitelist;
  public string reasons;
  public string status = "";
  public bool show_resources;
  private asset objAsset;
  private user obj = new user();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      try
      {
        if (this.u_group.group_type == 0)
          this.redirect_unauthorized();
        try
        {
          this.booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
        }
        catch
        {
          this.booking_id = 0L;
        }
        if (this.booking_id <= 0L)
          return;
        string booking_ids = this.booking_id.ToString();
        this.hdnBookingID.Value = this.booking_id.ToString();
        asset_booking booking = this.bookings.get_booking(this.booking_id, this.current_user.account_id);
        if (booking.is_repeat)
        {
          foreach (DataRow row in (InternalDataCollectionBase) this.bookings.get_repeat_bookings(booking.repeat_reference_id, booking.account_id).Tables[0].Rows)
          {
            if (row["booking_id"].ToString() != this.booking_id.ToString())
              booking_ids = booking_ids + "," + row["booking_id"].ToString();
          }
        }
        this.show_bookings(this.booking_id, booking_ids);
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Error -> ", ex);
      }
    }
    else
    {
      string ids = this.hdn_selected.Value;
      if (ids != "")
      {
        this.remove_item(ids);
      }
      else
      {
        this.alertError.Attributes.Add("style", "display: block;");
        this.litErrorMsg.Text = "There were no selected items.";
        this.div_cancelreasons.Visible = false;
      }
    }
  }

  private void remove_item(string ids)
  {
    List<asset_booking> bookings = new List<asset_booking>();
    string str1 = ids;
    char[] chArray = new char[1]{ '|' };
    foreach (string str2 in str1.Split(chArray))
    {
      if (str2 != "")
      {
        asset_booking booking = this.bookings.get_booking(Convert.ToInt64(str2), this.current_user.account_id);
        booking.cancel_reason = this.txt_reason.InnerText;
        booking.status = (short) 0;
        booking.modified_by = this.current_user.user_id;
        booking.cancel_by = this.current_user.user_id;
        booking.cancel_on = this.tzapi.current_user_timestamp();
        asset_booking assetBooking = this.bookings.cancel_booking(booking);
        this.bookings.set_cancel_status(assetBooking);
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
        bookings.Add(assetBooking);
      }
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
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("error -->  " + ex.ToString()));
      this.alertError.Attributes.Add("style", "display: block;");
    }
    Modal.Close((Page) this, (object) "OK");
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  protected void show_bookings(long booking_id, string booking_ids)
  {
    try
    {
      if (booking_id > 0L)
      {
        asset_booking booking = this.bookings.get_booking(booking_id, this.current_user.account_id);
        this.setting_data = this.settings.view_settings(this.current_user.account_id);
        this.workflows.get_workflow_reference_id(booking_id, this.current_user.account_id);
        this.lbl_email.Value = booking.email;
        this.lbl_purpose.Value = booking.purpose;
        this.lbl_remarks.Value = booking.remarks;
        this.lbl_telephone.Value = booking.contact;
        try
        {
          if (this.utilities.isValidDataset(this.setting_data))
            this.lbl_meetingtype.Value = this.setting_data.Tables[0].Select("setting_id=" + booking.meeting_type.ToString())[0]["value"].ToString();
        }
        catch
        {
        }
        user user1 = new user();
        user user2 = booking.booked_for == this.current_user.user_id ? this.current_user : this.users.get_user(Convert.ToInt64(booking.booked_for), this.current_user.account_id);
        user user3 = new user();
        user user4 = booking.booked_for == booking.created_by ? user2 : (booking.created_by == this.current_user.user_id ? this.current_user : this.users.get_user(Convert.ToInt64(booking.created_by), this.current_user.account_id));
        this.lbl_requestedBy.Value = user4.full_name + " (" + user4.email + ")";
        this.lbl_bookedfor.Value = user2.full_name;
        if (booking.housekeeping_required)
        {
          this.lbl_housekeeping.Value = "Yes";
        }
        else
        {
          this.lbl_housekeeping.Value = "No";
          this.div_housekeeping.Visible = false;
        }
        if (booking.setup_required)
        {
          this.lbl_Setup.Value = "Yes";
          if (this.utilities.isValidDataset(this.setting_data))
            this.lbl_setupetype.Value = this.setting_data.Tables[0].Select("setting_id=" + booking.setup_type.ToString())[0]["value"].ToString();
        }
        else
        {
          this.lbl_Setup.Value = "No";
          this.div_manpower.Visible = false;
          this.div_setup.Visible = false;
        }
        this.populate_asset(booking_id, booking_ids);
        this.populate_invitelist(booking_id);
        this.show_resources = Convert.ToBoolean(this.current_account.properties["resource_booking"]);
        if (!this.show_resources)
          return;
        this.populate_resourcelist(booking_id);
      }
      else
        this.lbl_requestedBy.Value = this.current_user.full_name + " (" + this.current_user.email + ")";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_resourcelist(long bid)
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
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
          stringBuilder.Append("<td>" + row["accepted_qty"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["requestor_remarks"].ToString() + "</td>");
          stringBuilder.Append("</tr>");
          ++num;
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      this.html_resourcelist = stringBuilder.ToString();
      if (this.html_resourcelist == "")
      {
        this.h4_resource.Visible = false;
        this.contrlgrp_resource.Visible = false;
      }
      else
      {
        this.h4_resource.Visible = true;
        this.contrlgrp_resource.Visible = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_invitelist(long bid)
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      DataSet invites = this.bookings.get_invites(bid, this.current_user.account_id);
      if (this.utilities.isValidDataset(invites))
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' width='100%'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>S/No.</th>");
        stringBuilder.Append("<th class='hidden-480'>Name</th>");
        stringBuilder.Append("<th class='hidden-480'>Email</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        int num = 1;
        foreach (DataRow row in (InternalDataCollectionBase) invites.Tables[0].Rows)
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + num.ToString() + "</td>");
          stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["email"].ToString() + "</td>");
          stringBuilder.Append("</tr>");
          ++num;
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      this.html_invitelist = stringBuilder.ToString();
      if (this.html_invitelist == "")
      {
        this.h4_invite.Visible = false;
        this.contrlgrp_invite.Visible = false;
      }
      else
      {
        this.h4_invite.Visible = true;
        this.contrlgrp_invite.Visible = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_asset(long booking_id, string booking_ids_string)
  {
    try
    {
      int num = 0;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table' width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'><input type='checkbox' name='items' id='chk_select_dates' onclick='SelectAll(this.id)' /></th>");
      stringBuilder.Append("<th class='hidden-480'>Code/ Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Building/ Level</th>");
      stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
      stringBuilder.Append("<th class='hidden-480'>Category</th>");
      stringBuilder.Append("<th class='hidden-480'>From</th>");
      stringBuilder.Append("<th class='hidden-480'>To</th>");
      stringBuilder.Append("<th class='hidden-480'>Status</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      this.assets.get_assets(this.current_user.account_id);
      string str = "";
      if (booking_ids_string != "")
        str = booking_ids_string;
      string[] strArray = str.Split(',');
      for (int index = 0; index <= strArray.Length - 1; ++index)
      {
        if (!string.IsNullOrEmpty(strArray[index].ToString()))
        {
          asset_booking booking = this.bookings.get_booking(Convert.ToInt64(strArray[index]), this.current_user.account_id);
          if (booking.booking_id > 0L)
          {
            if (booking.status != (short) 7 && booking.status != (short) 2 && booking.status != (short) 0 && booking.status != (short) 3 && booking.status != (short) 6 && booking.status != (short) 5)
            {
              asset asset = this.assets.get_asset(booking.asset_id, booking.account_id);
              bool flag = false;
              if (this.gp.isAdminType)
                flag = true;
              if (!flag && this.approvable_rooms.Contains(asset.asset_id))
                flag = true;
              if (!flag && (booking.created_by == this.current_user.user_id || booking.booked_for == this.current_user.user_id))
              {
                DateTime dateTime = DateTime.UtcNow.AddHours(this.current_account.timezone);
                if (booking.book_from < dateTime)
                {
                  asset_property assetProperty1 = new asset_property();
                  foreach (asset_property assetProperty2 in asset.asset_properties.Values)
                  {
                    if (assetProperty2.property_name == "cancel_window_after")
                    {
                      assetProperty1 = assetProperty2;
                      break;
                    }
                  }
                  if ((dateTime - booking.book_from).TotalMinutes < Convert.ToDouble(assetProperty1.property_value))
                    flag = true;
                }
                else
                {
                  asset_property assetProperty3 = new asset_property();
                  foreach (asset_property assetProperty4 in asset.asset_properties.Values)
                  {
                    if (assetProperty4.property_name == "cancel_window_before")
                    {
                      assetProperty3 = assetProperty4;
                      break;
                    }
                  }
                  if ((booking.book_from - dateTime).TotalMinutes > Convert.ToDouble(assetProperty3.property_value))
                    flag = true;
                }
              }
              if (!flag)
              {
                DateTime dateTime = DateTime.UtcNow.AddHours(this.current_account.timezone);
                if (booking.book_from < dateTime)
                {
                  asset_property assetProperty5 = new asset_property();
                  foreach (asset_property assetProperty6 in asset.asset_properties.Values)
                  {
                    if (assetProperty6.property_name == "cancel_window_after")
                    {
                      assetProperty5 = assetProperty6;
                      break;
                    }
                  }
                  if ((dateTime - booking.book_from).TotalMinutes < Convert.ToDouble(assetProperty5.property_value))
                    flag = true;
                }
                else
                {
                  asset_property assetProperty7 = new asset_property();
                  foreach (asset_property assetProperty8 in asset.asset_properties.Values)
                  {
                    if (assetProperty8.property_name == "cancel_window_before")
                    {
                      assetProperty7 = assetProperty8;
                      break;
                    }
                  }
                  if ((booking.book_from - dateTime).TotalMinutes > Convert.ToDouble(assetProperty7.property_value))
                    flag = true;
                }
              }
              stringBuilder.Append("<tr>");
              if (!flag)
              {
                if (this.gp.isAdminType)
                  stringBuilder.Append("<td></td>");
              }
              else if (booking.booking_id.ToString() == this.hdnBookingID.Value)
                stringBuilder.Append("<td><input type='checkbox' checked name='chk_date' id='chk_date' value='" + booking.booking_id.ToString() + "'></td>");
              else
                stringBuilder.Append("<td><input type='checkbox' name='chk_date' id='chk_date' value='" + booking.booking_id.ToString() + "'></td>");
              stringBuilder.Append("<td>" + asset.code + " / " + asset.name + "</td>");
              stringBuilder.Append("<td>" + asset.building.value + " / " + asset.level.value + "</td>");
              stringBuilder.Append("<td>" + (object) asset.capacity + "</td>");
              stringBuilder.Append("<td>" + asset.category.value + "</td>");
              stringBuilder.Append("<td>" + booking.book_from.ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder.Append("<td>" + booking.book_to.ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder.Append("<td>" + this.get_booking_status_string(booking.status) + "</td>");
              stringBuilder.Append("</tr>");
              ++num;
            }
            if (booking.status == (short) 0)
            {
              asset asset = this.assets.get_asset(booking.asset_id, booking.account_id);
              stringBuilder.Append("<tr>");
              stringBuilder.Append("<td></td>");
              stringBuilder.Append("<td>" + asset.code + " / " + asset.name + "</td>");
              stringBuilder.Append("<td>" + asset.building.value + " / " + asset.level.value + "</td>");
              stringBuilder.Append("<td>" + (object) asset.capacity + "</td>");
              stringBuilder.Append("<td>" + asset.category.value + "</td>");
              stringBuilder.Append("<td>" + booking.book_from.ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder.Append("<td>" + booking.book_to.ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder.Append("<td>" + this.get_booking_status_string(booking.status) + "</td>");
              stringBuilder.Append("</tr>");
            }
            this.txt_reason.InnerText = booking.cancel_reason;
          }
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_asset = stringBuilder.ToString();
      if (num != 0)
        return;
      this.link_cancel_selected.Visible = false;
      this.txt_reason.Visible = false;
      this.div_cancelreasons.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private string get_booking_status_string(short bkStatus)
  {
    string bookingStatusString = "";
    switch (bkStatus)
    {
      case 0:
        bookingStatusString = "<Span class='label label-cancelled'>Cancelled</span>";
        break;
      case 1:
        bookingStatusString = "<Span class='label label-Booked'>Booked</span>";
        break;
      case 2:
        bookingStatusString = "<Span class='label label-Blocked'>Blocked</span>";
        break;
      case 3:
        bookingStatusString = "<Span class='label label-NoShow'>No Show</span>";
        break;
      case 4:
        bookingStatusString = "<Span class='label label-Pending'>Pending</span>";
        break;
      case 5:
        bookingStatusString = "<Span class='label label-withdrawan'>WithDraw</Span>";
        break;
      case 6:
        bookingStatusString = "<Span class='label label-rejected'>Rejected</Span>";
        break;
      case 7:
        bookingStatusString = "<Span class='label label-rejected'>Auto Rejected</Span>";
        break;
    }
    return bookingStatusString;
  }
}
