// Decompiled with JetBrains decompiler
// Type: controls_view_resource_booking
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class controls_view_resource_booking : fbs_base_user_control
{
  protected HtmlInputText lbl_purpose;
  protected HtmlInputText lbl_location;
  protected HtmlInputText lbl_bookedfor;
  protected HtmlInputText lbl_email;
  protected HtmlInputText lbl_telephone;
  protected HtmlGenericControl divTelephone;
  protected HtmlTextArea lbl_remarks;
  protected HtmlInputText lbl_housekeeping;
  protected HtmlGenericControl divHouserkeeping;
  protected HtmlInputText lbl_Setup;
  protected HtmlGenericControl divManpower;
  protected HtmlInputText lbl_setupetype;
  protected HtmlGenericControl div_setup;
  protected HtmlInputText lbl_requestedBy;
  protected HtmlGenericControl divFacility;
  protected HtmlGenericControl divInvitelist;
  protected HtmlGenericControl divAddRes;
  protected LinkButton lnkAttachment;
  protected Table tblAttachment;
  protected HtmlGenericControl divAttachments;
  protected HtmlAnchor btn_edit;
  protected HtmlAnchor link_remove;
  protected Button btn_cancel;
  protected HiddenField hdnResourceBookingId;
  public string html_asset;
  public string html_resourcelist;
  public string html_invitelist;
  public string reasons;
  public string html_cateringlist;
  private Dictionary<string, string> selectedDates = new Dictionary<string, string>();
  private long asset_id;
  private DateTime startdatetime;
  private DateTime enddatetime;
  private DataSet setting_data;
  private DataSet asset_pro_ds;
  private user obj = new user();
  private Dictionary<string, user_property> objProperty = new Dictionary<string, user_property>();
  private Guid reference_id = Guid.Empty;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.obj = (user) this.Session["user"];
    if (this.obj == null)
      this.Response.Redirect("../error.aspx?message=not_authorized");
    if (this.IsPostBack)
      return;
    this.setting_data = this.settings.view_settings(this.current_user.account_id);
    this.asset_pro_ds = this.assets.get_asset_properties(this.current_user.account_id);
    long num;
    try
    {
      num = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch (Exception ex)
    {
      num = 0L;
      this.log.Error((object) "Error -> ", ex);
    }
    if (num <= 0L)
      return;
    this.populate_details(num);
    this.hdnResourceBookingId.Value = num.ToString();
    this.getUploadedAttachments(num);
    this.link_remove.Attributes["onclick"] = "delete_resource_booking_item('" + num.ToString() + "')";
    this.btn_edit.HRef = "~/additional_resources/request_resources.aspx?resource_booking_id=" + num.ToString() + "&repeat=" + (object) this.reference_id;
  }

  private void getUploadedAttachments(long res_bk_id)
  {
    DataSet resourceBookingId = this.resapi.get_resource_document_by_resource_booking_id(res_bk_id, this.current_user.account_id);
    if (this.utilities.isValidDataset(resourceBookingId))
    {
      this.divAttachments.Visible = true;
      this.bind_attachments(resourceBookingId);
    }
    else
      this.divAttachments.Visible = false;
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
      this.log.Error((object) "Error -> ", ex);
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
    }
    if (flag)
      return;
    this.lnkAttachment.Text = "";
  }

  private void populate_details(long bkid)
  {
    DataSet dataSet1 = new DataSet();
    DataSet dataSet2 = this.resapi.get_resource_bookings_by_resource_booking_id(this.current_user.account_id, bkid, this.str_resource_module);
    if (dataSet2.Tables[0].Rows.Count > 0)
    {
      this.reference_id = new Guid(dataSet2.Tables[0].Rows[0]["repeat_reference"].ToString());
      if (this.reference_id != Guid.Empty)
        dataSet2 = this.resapi.get_resource_bookings_by_repeat_reference(this.current_user.account_id, this.reference_id, this.str_resource_module);
    }
    long booking_id = 0;
    if (this.gp.isAdminType)
    {
      this.link_remove.Visible = true;
      this.btn_edit.Visible = true;
    }
    else
    {
      this.link_remove.Visible = false;
      this.btn_edit.Visible = false;
    }
    if (dataSet2.Tables[0].Rows.Count == 0)
    {
      this.link_remove.Visible = false;
      this.btn_edit.Visible = false;
    }
    if (!this.utilities.isValidDataset(dataSet2))
      return;
    DataRow row = dataSet2.Tables[0].Rows[0];
    this.lbl_location.Value = row["venue"].ToString();
    user user1 = new user();
    user user2 = this.users.get_user(Convert.ToInt64(row["booked_for_id"]), this.current_user.account_id);
    this.lbl_email.Value = row["email"].ToString();
    this.lbl_purpose.Value = row["purpose"].ToString();
    this.lbl_remarks.Value = row["remarks"].ToString();
    user user3 = new user();
    user user4 = !(row["booked_for_id"].ToString() != row["requested_by"].ToString()) ? user2 : this.users.get_user(Convert.ToInt64(row["requested_by"]), this.current_user.account_id);
    this.lbl_requestedBy.Value = user4.full_name + " (" + user4.email + ")";
    if (user2.user_id == this.current_user.user_id || user4.user_id == this.current_user.user_id)
    {
      this.link_remove.Visible = true;
      this.btn_edit.Visible = true;
    }
    else
    {
      this.link_remove.Visible = false;
      this.btn_edit.Visible = false;
    }
    this.lbl_bookedfor.Value = user2.full_name;
    if (this.utilities.isValidDataset(dataSet2))
    {
      this.divAddRes.Visible = true;
      this.populate_resourcelist(dataSet2);
    }
    else
      this.divAddRes.Visible = false;
    bool flag = false;
    try
    {
      booking_id = Convert.ToInt64(row["asset_booking_id"]);
      if (booking_id > 0L)
        flag = true;
    }
    catch (Exception ex)
    {
      flag = false;
      this.log.Error((object) "Error -> ", ex);
    }
    if (flag)
    {
      this.selectedDates = (Dictionary<string, string>) this.Session["SelectedDates"];
      if (this.selectedDates == null)
        this.selectedDates = new Dictionary<string, string>();
      asset_booking booking = this.bookings.get_booking(booking_id, this.current_user.account_id);
      this.assets.get_asset(booking.asset_id, this.current_user.account_id);
      this.asset_id = booking.asset_id;
      this.startdatetime = booking.book_from;
      this.enddatetime = booking.book_to;
      if (this.selectedDates.Count == 0)
        this.selectedDates.Add(this.startdatetime.ToString(), this.enddatetime.ToString());
      this.lbl_telephone.Value = booking.contact;
      if (booking.housekeeping_required)
        this.lbl_housekeeping.Value = "Yes";
      else
        this.lbl_housekeeping.Value = "No";
      if (this.lbl_remarks.Value == "")
        this.lbl_remarks.Value = booking.remarks;
      if (booking.setup_required)
      {
        this.lbl_Setup.Value = "Yes";
        if (this.utilities.isValidDataset(this.setting_data))
          this.lbl_setupetype.Value = this.setting_data.Tables[0].Select("setting_id=" + booking.setup_type.ToString())[0]["value"].ToString();
      }
      else
      {
        this.lbl_Setup.Value = "No";
        this.div_setup.Visible = false;
      }
      this.populate_asset();
      this.populate_invitelist(booking.booking_id);
    }
    else
    {
      this.divTelephone.Visible = false;
      this.divHouserkeeping.Visible = false;
      this.divManpower.Visible = false;
      this.div_setup.Visible = false;
      this.divFacility.Visible = false;
      this.divInvitelist.Visible = false;
    }
  }

  private void populate_resourcelist(DataSet dsRes)
  {
    Dictionary<DateTime, DateTime> dictionary = new Dictionary<DateTime, DateTime>();
    foreach (DataRow row in (InternalDataCollectionBase) dsRes.Tables[0].Rows)
    {
      if (!dictionary.ContainsKey(Convert.ToDateTime(row["from_date"])))
        dictionary.Add(Convert.ToDateTime(row["from_date"]), Convert.ToDateTime(row["to_date"]));
    }
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
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
      foreach (DateTime key in dictionary.Keys)
      {
        int num = 1;
        stringBuilder.Append("<tr><td colspan='5'><b>" + Convert.ToDateTime(key).ToString(api_constants.display_datetime_format) + " - " + Convert.ToDateTime(dictionary[key]).ToString(api_constants.display_datetime_format) + "</b></td></tr>");
        foreach (DataRow dataRow in dsRes.Tables[0].Select("from_date='" + (object) key + "'"))
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + num.ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["value"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["accepted_qty"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRow["requestor_remarks"].ToString() + "</td>");
          stringBuilder.Append("</tr>");
          ++num;
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_resourcelist = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
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
        stringBuilder.Append("<th class='hidden-480'>Going</th>");
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
          if (row["is_attending"] == DBNull.Value)
            stringBuilder.Append("<td><img src='assets/img/question.png' width='32' height='32' /></td>");
          else if (Convert.ToBoolean(row["is_attending"]))
            stringBuilder.Append("<td><img src='assets/img/tick.jpg' width='32' height='32' /></td>");
          else
            stringBuilder.Append("<td><img src='assets/img/cross.jpg' width='32' height='32' /></td>");
          stringBuilder.Append("</tr>");
          ++num;
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      this.html_invitelist = stringBuilder.ToString();
      if (this.html_invitelist == "")
        this.divInvitelist.Visible = false;
      else
        this.divInvitelist.Visible = true;
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_asset()
  {
    try
    {
      this.html_asset = this.bookingsbl.getAssetHtml_with_bookingDates(this.assets.get_assets(this.current_user.account_id), this.setting_data, this.asset_pro_ds, this.asset_id, this.selectedDates);
      if (this.html_asset == "")
        this.divFacility.Visible = false;
      else
        this.divFacility.Visible = true;
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }
}
