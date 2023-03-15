// Decompiled with JetBrains decompiler
// Type: booking_reassign
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

public class booking_reassign : fbs_base_page, IRequiresSessionState
{
  protected Literal litInfoMsg;
  protected HtmlGenericControl alertInfo;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected HiddenField hdnBookingID;
  protected HiddenField hdn_repeat_reference;
  protected HtmlInputText lbl_purpose;
  protected HtmlInputText lbl_bookedfor;
  protected TextBox txt_email;
  protected HtmlImage img_loading;
  protected HtmlInputText lbl_telephone;
  protected HtmlInputText lbl_remarks;
  protected HtmlInputText lbl_housekeeping;
  protected HtmlInputText lbl_Setup;
  protected HtmlInputText lbl_setupetype;
  protected HtmlInputText lbl_requestedBy;
  protected DropDownList ddl_reassign;
  protected HtmlGenericControl h4_invite;
  protected HtmlGenericControl contrlgrp_invite;
  protected Button btnReAssign;
  protected Button btn_close;
  protected HtmlForm form_sample_2;
  private long booking_id;
  private long asset_id;
  private DateTime startdatetime;
  private DateTime enddatetime;
  private DataSet setting_data;
  private DataSet asset_pro_ds;
  public string html_asset;
  public string html_invitelist;
  private Dictionary<string, string> selectedDates = new Dictionary<string, string>();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.u_group.group_type == 0)
      this.redirect_unauthorized();
    if (this.IsPostBack)
      return;
    Dictionary<string, user_property> dictionary = new Dictionary<string, user_property>();
    Dictionary<string, user_property> properties = this.current_user.properties;
    this.setting_data = this.settings.view_settings(this.current_user.account_id);
    try
    {
      this.booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
      this.booking_id = 0L;
    }
    this.hdnBookingID.Value = this.booking_id.ToString();
    try
    {
      if (this.booking_id > 0L)
      {
        this.selectedDates = (Dictionary<string, string>) this.Session["SelectedDates"];
        if (this.selectedDates == null)
          this.selectedDates = new Dictionary<string, string>();
        asset_booking booking = this.bookings.get_booking(this.booking_id, this.current_user.account_id);
        this.hdn_repeat_reference.Value = booking.repeat_reference_id.ToString();
        asset asset = this.assets.get_asset(booking.asset_id, this.current_user.account_id);
        this.asset_pro_ds = this.assets.get_asset_properties(booking.asset_id, this.current_user.account_id);
        this.asset_id = booking.asset_id;
        if (this.selectedDates.Count == 0)
          this.selectedDates.Add(booking.book_from.ToString(), booking.book_to.ToString());
        this.txt_email.Text = booking.email;
        this.lbl_purpose.Value = booking.purpose;
        this.lbl_remarks.Value = booking.remarks;
        this.lbl_telephone.Value = booking.contact;
        user user1 = new user();
        user user2 = booking.booked_for == this.current_user.user_id ? this.current_user : this.users.get_user(Convert.ToInt64(booking.booked_for), this.current_user.account_id);
        user user3 = new user();
        user user4 = booking.created_by == this.current_user.user_id ? this.current_user : this.users.get_user(Convert.ToInt64(booking.created_by), this.current_user.account_id);
        this.lbl_bookedfor.Value = user2.full_name;
        this.lbl_requestedBy.Value = user4.full_name + "(" + user4.email + ")";
        if (booking.housekeeping_required)
          this.lbl_housekeeping.Value = "Yes";
        else
          this.lbl_housekeeping.Value = "No";
        if (booking.setup_required)
        {
          this.lbl_Setup.Value = "Yes";
          if (this.utilities.isValidDataset(this.setting_data))
            this.lbl_setupetype.Value = this.setting_data.Tables[0].Select("setting_id=" + booking.setup_type.ToString())[0]["value"].ToString();
        }
        else
          this.lbl_Setup.Value = "No";
        this.populate_asset(asset);
        this.populate_invitelist(booking.booking_id);
        this.populate_reassign_to();
      }
      if (!string.IsNullOrEmpty(this.Request.QueryString["btn"]))
      {
        if (this.Request.QueryString["btn"] == "f")
        {
          this.btnReAssign.Visible = false;
          this.alertInfo.Attributes.Add("style", "display: block;");
          this.litInfoMsg.Text = Resources.fbs.booking_reassign_confirmation_msg;
          this.btnReAssign.Visible = false;
        }
        else
          this.btnReAssign.Visible = true;
      }
      else
        this.btnReAssign.Visible = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_reassign_to()
  {
    try
    {
      DataSet dataSet = new DataSet();
      DataSet otherAllUserType = this.users.get_users_other_all_user_type(this.current_user.account_id);
      if (!this.utilities.isValidDataset(otherAllUserType))
        return;
      this.ddl_reassign.Items.Add(new ListItem("Select User", "0"));
      foreach (DataRow row in (InternalDataCollectionBase) otherAllUserType.Tables[0].Rows)
        this.ddl_reassign.Items.Add(new ListItem(row["full_name"].ToString(), row["user_id"].ToString()));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_invitelist(long bid)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
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

  private void populate_asset(asset objAsset)
  {
    try
    {
      this.html_asset = this.bookingsbl.getAssetHtml_with_bookingDates(this.assets.get_assets(objAsset.asset_id, this.current_user.account_id), this.setting_data, this.asset_pro_ds, this.asset_id, this.selectedDates);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_close_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  protected void btnReAssign_Click(object sender, EventArgs e)
  {
    try
    {
      List<asset_booking> bookings = new List<asset_booking>();
      string str = this.hdnBookingID.Value;
      Guid repeat_reference_id = new Guid(this.hdn_repeat_reference.Value);
      user user = this.users.get_user(Convert.ToInt64(this.ddl_reassign.SelectedItem.Value), this.current_user.account_id);
      if (repeat_reference_id != Guid.Empty)
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.bookings.get_repeat_bookings(repeat_reference_id, this.current_user.account_id).Tables[0].Rows)
        {
          asset_booking booking = this.bookings.get_booking(Convert.ToInt64(row["booking_id"]), this.current_user.account_id);
          booking.global_appointment_id = this.bookings.get_appointment_id(booking.booking_id, booking.account_id);
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
          booking.created_by = Convert.ToInt64(this.ddl_reassign.SelectedItem.Value);
          booking.modified_by = this.current_user.user_id;
          booking.purpose += " (Reassigned)";
          this.bookings.update_booking_reassign(booking, booking.booking_id.ToString());
          bookings.Add(booking);
        }
      }
      else
      {
        asset_booking booking = this.bookings.get_booking(Convert.ToInt64(str), this.current_user.account_id);
        booking.global_appointment_id = this.bookings.get_appointment_id(booking.booking_id, booking.account_id);
        if (booking.created_by == booking.booked_for)
        {
          booking.booked_for = Convert.ToInt64(this.ddl_reassign.SelectedItem.Value);
          booking.created_by = booking.booked_for;
          booking.email = user.email;
        }
        else
          booking.created_by = Convert.ToInt64(this.ddl_reassign.SelectedItem.Value);
        booking.created_by = Convert.ToInt64(this.ddl_reassign.SelectedItem.Value);
        booking.modified_by = this.current_user.user_id;
        booking.purpose += " (Reassigned)";
        this.bookings.update_booking_reassign(booking, booking.booking_id.ToString());
        bookings.Add(booking);
      }
      if (Convert.ToBoolean(this.current_account.properties["send_email"]))
        this.bookingsbl.email_reassign(bookings);
      this.Response.Redirect("booking_view.aspx?id=" + (object) bookings[0].booking_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
