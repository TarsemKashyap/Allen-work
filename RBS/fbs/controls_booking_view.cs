// Decompiled with JetBrains decompiler
// Type: controls_booking_view
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
using visitor_management;

public class controls_booking_view : fbs_base_user_control
{
  protected HtmlGenericControl tab_layout_li;
  protected HtmlGenericControl tab_facility_li;
  protected HtmlGenericControl Li1;
  protected HtmlGenericControl tab_report_problem_li;
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
  protected HtmlAnchor btn_visitors;
  protected Image imgLayout;
  protected Image imagfacility;
  protected TextBox txt_report_subject;
  protected HtmlTextArea txtarea;
  protected Button btn_report_problem;
  protected Literal lbl_report_sucess;
  protected HtmlGenericControl div_report_msg;
  protected HiddenField hdn_asset_field;
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
  public string html_created;
  public string html_modified;
  public string status = "";
  public bool show_resources;
  private Dictionary<string, string> selectedDates = new Dictionary<string, string>();
  private asset objAsset;
  private user obj = new user();
  public string html_visitor = "";
  public string report_mail_from = "";
  public string report_tab = "";
  public bool show_report_problem;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.obj = (user) this.Session["user"];
    if (this.obj == null)
      this.Response.Redirect("../error.aspx?message=not_authorized");
    this.show_report_problem = Convert.ToBoolean(this.current_account.properties["fault_reporting"]);
    this.report_mail_from = this.current_user.full_name;
    this.tab_report_problem_li.Visible = this.show_report_problem;
    try
    {
      Dictionary<string, user_property> dictionary = new Dictionary<string, user_property>();
      try
      {
        if (this.Attributes["custombooking"] == null)
          this.booking_id = Convert.ToInt64(this.Attributes["booking_id"]);
        else
          this.booking_id = Convert.ToInt64(this.Attributes["booking_id"].ToString().Split(',')[0]);
      }
      catch
      {
        this.booking_id = 0L;
      }
      if (this.booking_id > 0L)
      {
        this.selectedDates = (Dictionary<string, string>) this.Session["SelectedDates"];
        if (this.selectedDates == null)
          this.selectedDates = new Dictionary<string, string>();
        asset_booking booking = this.bookings.get_booking(this.booking_id, this.current_user.account_id);
        this.asset_id = booking.asset_id;
        this.setting_data = this.settings.view_settings(this.current_user.account_id);
        this.asset_pro_ds = this.assets.get_asset_properties(booking.asset_id, this.current_user.account_id);
        this.objAsset = this.assets.get_asset(booking.asset_id, this.current_user.account_id);
        this.hdn_asset_field.Value = this.objAsset.asset_id.ToString();
        workflow workflowReferenceId = this.work_flow.get_workflow_reference_id(this.booking_id, this.current_user.account_id);
        this.startdatetime = booking.book_from;
        this.enddatetime = booking.book_to;
        if (this.selectedDates.Count == 0)
          this.selectedDates.Add(this.startdatetime.ToString(), this.enddatetime.ToString());
        string str1 = booking.created_by != this.current_user.user_id ? this.users.get_user(booking.created_by, this.current_user.account_id).full_name : this.current_user.full_name;
        this.html_created = booking.created_on.AddHours(this.current_account.timezone).ToString(api_constants.display_datetime_format) + " by " + str1;
        if (booking.created_on != booking.modified_on)
        {
          string str2 = booking.modified_by != this.current_user.user_id ? this.users.get_user(booking.modified_by, this.current_user.account_id).full_name : this.current_user.full_name;
          this.html_modified = booking.modified_on.AddHours(this.current_account.timezone).ToString(api_constants.display_datetime_format) + " by " + str2;
        }
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
        if (booking.status == (short) 0)
        {
          this.status = "<Span class='label label-cancelled'>Cancelled</span>";
          this.txt_rejectedreason.Value = booking.cancel_reason;
          this.reasons = "Cancelled Reason";
          this.Div_Rej_reason.Visible = true;
        }
        else if (booking.status == (short) 1)
          this.status = "<Span class='label label-Booked'>Booked</span>";
        else if (booking.status == (short) 2)
        {
          this.status = "<Span class='label label-Blocked'>Blocked</span>";
          this.txt_rejectedreason.Value = booking.purpose;
          this.reasons = "Blocked Reason";
          this.Div_Rej_reason.Visible = true;
        }
        else if (booking.status == (short) 3)
          this.status = "<Span class='label label-NoShow'>No Show</span>";
        else if (booking.status == (short) 4)
          this.status = "<Span class='label label-Pending'>Pending</span>";
        else if (booking.status == (short) 6)
        {
          this.status = "<Span class='label label-rejected'>Rejected</Span>";
          if (workflowReferenceId != null && workflowReferenceId.action_remarks != null)
          {
            this.txt_rejectedreason.Value = workflowReferenceId.action_remarks;
            this.reasons = "Rejected Reason";
            this.Div_Rej_reason.Visible = true;
          }
        }
        else if (booking.status == (short) 5)
          this.status = "<Span class='label label-withdrawan'>WithDraw</Span>";
        else if (booking.status == (short) 7)
          this.status = "<Span class='label label-rejected'>Auto Rejected</Span>";
        user user1 = new user();
        user user2 = booking.booked_for == this.current_user.user_id ? this.current_user : this.users.get_user(Convert.ToInt64(booking.booked_for), this.current_user.account_id);
        user user3 = new user();
        user user4 = booking.booked_for == booking.created_by ? user2 : (booking.created_by == this.current_user.user_id ? this.current_user : this.users.get_user(Convert.ToInt64(booking.created_by), this.current_user.account_id));
        this.lbl_requestedBy.Value = user4.full_name + " (" + user4.email + ")";
        Dictionary<string, user_property> properties = user2.properties;
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
        this.populate_asset();
        this.populate_invitelist(this.booking_id);
        this.show_resources = Convert.ToBoolean(this.current_account.properties["resource_booking"]);
        if (this.show_resources)
          this.populate_resourcelist(this.booking_id);
      }
      else
      {
        this.lbl_requestedBy.Value = this.current_user.full_name + " (" + this.current_user.email + ")";
        Dictionary<string, user_property> properties = this.current_user.properties;
      }
      Dictionary<long, asset_property> assetProperties = this.objAsset.asset_properties;
      string str3 = "0";
      string str4 = "0";
      foreach (long key in assetProperties.Keys)
      {
        if (assetProperties[key].property_name == "facility_image")
          str3 = assetProperties[key].property_value;
        if (assetProperties[key].property_name == "layout_image")
          str4 = assetProperties[key].property_value;
      }
      if (str4 != "0")
        this.imgLayout.ImageUrl = this.site_full_path + "handlers/show_image.ashx?doc_id=" + str4;
      if (str3 != "0")
        this.imagfacility.ImageUrl = this.site_full_path + "handlers/show_image.ashx?doc_id=" + str3;
      this.tab_layout_li.Visible = true;
      this.tab_facility_li.Visible = true;
      if (this.Attributes["show"] == null || !(Convert.ToString(this.Attributes["show"]).ToUpper() == "NO"))
        return;
      this.tab_layout_li.Visible = false;
      this.tab_facility_li.Visible = false;
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void load_visitors()
  {
    visitor_management_api vapi = new visitor_management_api();
    long registerId = this.bookings.get_register_id(this.booking_id, this.current_user.account_id);
    DataSet dataSet1 = new DataSet();
    if (registerId > 0L)
    {
      DataSet dataSet2 = vapi.view_visitors(registerId, this.current_user.account_id);
      this.populate_visitors_table(vapi, dataSet2.Tables[0]);
    }
    this.btn_visitors.HRef = "/visitors/visitors.aspx?bid=" + (object) this.booking_id;
  }

  private void populate_visitors_table(visitor_management_api vapi, DataTable table)
  {
    StringBuilder stringBuilder = new StringBuilder();
    int num = 1;
    foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
    {
      visitor visitor = vapi.get_visitor(Convert.ToInt64(row["visitor_id"]), this.current_user.account_id);
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + num.ToString() + "</td>");
      stringBuilder.Append("<td>" + visitor.full_name + "</td>");
      stringBuilder.Append("<td>" + visitor.mobile + "</td>");
      stringBuilder.Append("<td>" + visitor.company_name + "</td>");
      stringBuilder.Append("<td>" + visitor.vehicle_number + "</td>");
      stringBuilder.Append("</tr>");
      ++num;
    }
    this.html_visitor = stringBuilder.ToString();
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
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_asset()
  {
    try
    {
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
      DataSet assets = this.assets.get_assets(this.current_user.account_id);
      string[] strArray = (this.Attributes["custombooking"] != null ? this.Attributes["booking_ids"].ToString() : this.Attributes["booking_id"].ToString()).Split(',');
      int counter = 1;
      for (int index = 0; index <= strArray.Length - 1; ++index)
      {
        if (!string.IsNullOrEmpty(strArray[index].ToString()))
        {
          asset_booking booking = this.bookings.get_booking(Convert.ToInt64(strArray[index]), this.current_user.account_id);
          if (booking.booking_id > 0L && booking.status != (short) 7 && booking.status != (short) 2 && booking.status != (short) 3 && booking.status != (short) 6 && booking.status != (short) 5)
          {
            this.selectedDates = new Dictionary<string, string>();
            this.selectedDates.Add(booking.book_from.ToString(api_constants.display_datetime_format), booking.book_to.ToString(api_constants.display_datetime_format));
            string datesCustomBookings = this.bookingsbl.getAssetHtml_with_bookingDates_CustomBookings(counter, assets, this.setting_data, this.asset_pro_ds, booking.asset_id, this.selectedDates, booking.book_from.ToString(api_constants.display_datetime_format), this.get_booking_status_string(booking.status));
            stringBuilder.Append(datesCustomBookings);
            ++counter;
          }
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_asset = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
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

  public string replace_template(
    string body,
    long asset_id,
    asset_report_problem obj_problem_report)
  {
    try
    {
      asset asset = this.assets.get_asset(asset_id, this.current_user.account_id);
      user user = this.users.get_user(obj_problem_report.Reported_by, obj_problem_report.account_id);
      body = body.Replace("[buildingcodename]", asset.building.value + "/" + asset.code + "-" + asset.name + " - " + obj_problem_report.Subject);
      body = body.Replace("[Building]", asset.building.value);
      body = body.Replace("[Level]", asset.level.value);
      body = body.Replace("[roomcodename]", asset.code + "/" + asset.name);
      body = body.Replace("[FullName]", user.full_name);
      body = body.Replace("[email]", user.email);
      body = body.Replace("[remarks]", obj_problem_report.Remarks);
    }
    catch (Exception ex)
    {
      this.log.Error((object) ex.ToString());
    }
    return body;
  }

  protected void btn_report_problem_Click(object sender, EventArgs e)
  {
    try
    {
      long int64 = Convert.ToInt64(this.Request.QueryString["r"]);
      string str = "";
      DataSet dataSet = this.assets.getowner_group_emailby_asset_id(int64, this.current_user.account_id);
      asset_report_problem assetReportProblem = new asset_report_problem();
      assetReportProblem.problem_id = 0L;
      assetReportProblem.modified_by = 0L;
      assetReportProblem.asset_id = Convert.ToInt64(this.hdn_asset_field.Value);
      assetReportProblem.account_id = this.current_user.account_id;
      assetReportProblem.Reported_by = this.current_user.user_id;
      assetReportProblem.Subject = this.txt_report_subject.Text;
      assetReportProblem.Remarks = this.txtarea.InnerText;
      assetReportProblem.account_id = this.current_user.account_id;
      assetReportProblem.created_by = this.current_user.user_id;
      asset_report_problem obj_problem_report = this.assets.update_report_problem(assetReportProblem);
      template template1 = new template();
      if (obj_problem_report.problem_id > 0L)
      {
        template template2 = this.tapi.get_template("email_report_problem", this.current_user.account_id);
        string body = this.replace_template(template2.content_data, obj_problem_report.asset_id, obj_problem_report);
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
            str = str + row["email"].ToString() + ";";
        }
        Guid recID = Guid.NewGuid();
        setting setting = this.settings.get_setting("report_problem", this.current_user.account_id);
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          this.bookingsbl.sendEmail("", body, template2.name, setting.value, this.current_user.email, recID);
        this.div_report_msg.Visible = true;
        this.lbl_report_sucess.Text = Resources.fbs.report_problem_sucess_msg;
        this.report_tab = "S";
        this.txt_report_subject.Text = "";
        this.txtarea.InnerText = "";
        this.btn_report_problem.Visible = false;
      }
      else
        this.div_report_msg.Visible = false;
    }
    catch (Exception ex)
    {
      this.log.Error((object) ex.ToString());
    }
  }
}
