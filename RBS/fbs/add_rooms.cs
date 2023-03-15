// Decompiled with JetBrains decompiler
// Type: add_rooms
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
using System.Xml;

public class add_rooms : fbs_base_page, IRequiresSessionState
{
  protected Label lbl_assetname_heading;
  protected Literal litError;
  protected HtmlGenericControl alt_error_alrdybook;
  protected HtmlInputText txt_from;
  protected HtmlInputText txt_to;
  protected HtmlInputText lbl_purpose;
  protected HtmlInputText lbl_bookedfor;
  protected HtmlInputText lbl_email;
  protected HtmlInputText lbl_telephone;
  protected HtmlTextArea lbl_remarks;
  protected HiddenField hdnID;
  protected HiddenField hdnRecID;
  protected HiddenField hdnEventID;
  protected HiddenField hdnvalues;
  protected Button btn_save;
  protected Button btn_close;
  protected Button btn_cancel;
  protected HtmlForm form_sample_2;
  private long booking_id;
  private bool is_book = true;
  public string htmltable;
  public DataSet setting_data;
  private Guid eventid = Guid.Empty;
  private booking_api objbooking_api = new booking_api();
  private Dictionary<long, asset> available_assets = new Dictionary<long, asset>();
  private asset_booking objasset_booking = new asset_booking();
  private DataSet assProDs = new DataSet();
  private user obj = new user();
  private asset_booking newObj = new asset_booking();
  private List<string> selectedRooms = new List<string>();
  private DataSet assetData;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.obj = (user) this.Session["user"];
    if (this.obj == null)
      this.Response.Redirect("../error.aspx?message=not_authorized");
    try
    {
      this.setting_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (this.setting_data == null)
      {
        this.setting_data = this.settings.view_settings(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.setting_data);
      }
      if (this.IsPostBack)
        return;
      try
      {
        this.booking_id = Convert.ToInt64(this.Request.QueryString["id"]);
        this.objasset_booking = this.objbooking_api.get_booking(this.booking_id, this.current_user.account_id);
        asset asset = this.assets.get_asset(this.objasset_booking.asset_id, this.current_user.account_id);
        this.lbl_assetname_heading.Text = asset.code.ToString() + "/" + asset.name.ToString();
        this.is_book = this.check_permissions(this.booking_id);
        if (this.gp.isAdminType)
          this.is_book = true;
        user user = this.users.get_user(this.objasset_booking.booked_for, this.current_user.account_id);
        this.txt_from.Value = this.objasset_booking.book_from.ToString(api_constants.display_datetime_format);
        this.txt_to.Value = this.objasset_booking.book_to.ToString(api_constants.display_datetime_format);
        this.lbl_email.Value = this.objasset_booking.email;
        this.lbl_purpose.Value = this.objasset_booking.purpose;
        this.lbl_remarks.Value = this.objasset_booking.remarks;
        this.lbl_telephone.Value = this.objasset_booking.contact;
        this.lbl_bookedfor.Value = user.full_name.ToString();
        this.Check_Availability();
      }
      catch
      {
        this.booking_id = 0L;
      }
      this.hdnID.Value = this.booking_id.ToString();
      this.hdnEventID.Value = this.objasset_booking.event_id.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  public void bind_Empty_data()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='available_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:8px;'></th>");
      stringBuilder.Append("<th class='hidden-480'>Code/ Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Building</th>");
      stringBuilder.Append("<th class='hidden-480'>Level</th>");
      stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.htmltable = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void Check_Availability()
  {
    string groupIds = this.utilities.get_group_ids(this.current_user);
    string group_ids = string.IsNullOrEmpty(groupIds) ? "0" : groupIds;
    this.assProDs = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_ass_prop");
    if (this.assProDs == null)
    {
      this.assProDs = this.assets.view_asset_properties(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_ass_prop", (object) this.assProDs);
    }
    this.available_assets = this.objbooking_api.check_available_assets(this.objasset_booking.book_from.AddSeconds(1.0).ToString(api_constants.datetime_format), this.objasset_booking.book_to.AddSeconds(-1.0).ToString(api_constants.datetime_format), this.current_user.account_id, "0", "0", "0", "0", group_ids, this.gp.isAdminType, this.assProDs, this.setting_data, this.current_timestamp);
    this.bind_Empty_data();
    this.htmltable = this.bookingsbl.getAssetHtml(this.available_assets, this.setting_data, this.assProDs);
  }

  private bool check_permissions(long booking_id)
  {
    bool flag = false;
    try
    {
      DataSet groupsPermissions = this.assets.get_groups_permissions(this.current_user.account_id, this.bookings.get_booking(booking_id, this.current_user.account_id).asset_id, 0L, 0L, "");
      string groupIds = this.utilities.get_group_ids(this.current_user);
      string[] strArray = (string.IsNullOrEmpty(groupIds) ? "0" : groupIds).Split(',');
      string str1 = "";
      foreach (string str2 in strArray)
        str1 = str1 + " group_id = " + str2.ToString() + " or ";
      string filterExpression = str1.Substring(0, str1.Length - 3);
      if (groupsPermissions != null)
      {
        if (groupsPermissions.Tables[0].Rows.Count > 0)
        {
          DataRow[] dataRowArray = groupsPermissions.Tables[0].Select(filterExpression);
          if (dataRowArray.Length > 0)
          {
            foreach (DataRow dataRow in dataRowArray)
            {
              if (!flag)
                flag = !string.IsNullOrEmpty(dataRow["is_book"].ToString()) && Convert.ToBoolean(dataRow["is_book"]);
            }
          }
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return flag;
  }

  private void add_workflow(asset_booking obj, long asset_owner_groupid)
  {
    if (obj.status == (short) 1)
      return;
    workflow workflow = new workflow();
    workflow.account_id = this.current_user.account_id;
    workflow.action_owner_id = asset_owner_groupid <= 0L ? this.current_user.user_id : asset_owner_groupid;
    if (obj.transfer_request)
    {
      workflow.action_remarks = "asset booking";
      if (asset_owner_groupid > 0L && asset_owner_groupid != this.u_group.group_id)
      {
        workflow.action_owner_id = asset_owner_groupid;
        workflow.action_type = (short) 2;
      }
      else
      {
        DataSet userGroup = this.bookings.get_user_group(obj.transfer_original_booking_id, this.current_user.account_id);
        if (userGroup != null && userGroup.Tables[0].Rows.Count > 0)
          workflow.action_owner_id = Convert.ToInt64(userGroup.Tables[0].Rows[0][0].ToString());
        workflow.action_type = (short) 3;
      }
    }
    else
    {
      workflow.action_remarks = "Asset booking";
      workflow.action_type = (short) 1;
    }
    workflow.created_by = obj.created_by;
    workflow.created_on = this.current_timestamp;
    workflow.due_on = new DateTime(1900, 1, 1);
    workflow.record_id = Guid.NewGuid();
    workflow.reference_id = this.newObj.booking_id;
    workflow.workflow_id = 0L;
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.LoadXml("<properties><asset_id>" + (object) obj.asset_id + " </asset_id></properties>");
    workflow.properties = xmlDocument;
    this.workflows.update_workflow(workflow);
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("booking_view.aspx?id=" + (object) this.objasset_booking.booking_id);

  protected void btn_close_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  protected void btn_save_Click(object sender, EventArgs e)
  {
    List<asset_booking> bookings = new List<asset_booking>();
    Guid.NewGuid();
    asset_booking assetBooking1 = new asset_booking();
    try
    {
      assetBooking1 = this.bookings.get_booking(Convert.ToInt64(this.hdnID.Value), this.current_user.account_id);
      if (assetBooking1.booking_id > 0L)
      {
        assetBooking1.global_appointment_id = this.bookings.get_appointment_id(assetBooking1.booking_id, assetBooking1.account_id);
        this.assetData = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_assets");
        if (this.assetData == null)
        {
          this.assetData = this.assets.view_assets(this.current_user.account_id);
          this.capi.set_cache(this.current_user.account_id.ToString() + "_assets", (object) this.assetData);
        }
        if (assetBooking1.event_id == Guid.Empty)
        {
          this.eventid = Guid.NewGuid();
          assetBooking1.event_id = this.eventid;
        }
        else
          this.eventid = assetBooking1.event_id;
        assetBooking1.is_repeat = true;
        assetBooking1 = this.bookings.update_booking(assetBooking1);
        string[] strArray = this.hdnvalues.Value.Split('#');
        if (strArray.Length > 0)
        {
          foreach (string str in strArray)
            this.selectedRooms.Add(str);
        }
        foreach (string selectedRoom in this.selectedRooms)
        {
          asset_booking assetBooking2 = new asset_booking();
          if (selectedRoom != "")
          {
            asset_booking assetBooking3 = assetBooking1;
            assetBooking3.asset_id = Convert.ToInt64(selectedRoom);
            assetBooking3.booking_id = 0L;
            DataRow[] dataRowArray1 = this.assetData.Tables[0].Select("asset_id=" + assetBooking3.asset_id.ToString());
            long num1;
            try
            {
              num1 = Convert.ToInt64(dataRowArray1[0]["asset_owner_group_id"]);
            }
            catch
            {
              num1 = 0L;
            }
            short bookingStatus = this.bookings.get_booking_status(num1, this.current_user, this.gp);
            assetBooking3.status = bookingStatus;
            if (!this.gp.isAdminType)
            {
              DataRow[] dataRowArray2 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
              {
                "booking_lead_time"
              }).Tables[0].Select("asset_id=" + assetBooking3.asset_id.ToString());
              DataRow[] dataRowArray3 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
              {
                "book_weekend"
              }).Tables[0].Select("asset_id=" + assetBooking3.asset_id.ToString());
              DataRow[] dataRowArray4 = this.assets.view_asset_properties(this.current_user.account_id, new string[1]
              {
                "book_holiday"
              }).Tables[0].Select("asset_id=" + assetBooking3.asset_id.ToString());
              if (dataRowArray2.Length > 0)
              {
                string newValue = dataRowArray2[0].ItemArray[2].ToString();
                if (newValue != "")
                {
                  if (newValue.Split(' ')[0] != "0")
                  {
                    DateTime dateTime1 = new DateTime();
                    DateTime dateTime2;
                    if (newValue.Split(' ')[1] == "Hour(s)")
                    {
                      dateTime2 = DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(Convert.ToDouble(newValue.Split(' ')[0]));
                    }
                    else
                    {
                      int num2 = 0;
                      int num3 = 0;
                      while (true)
                      {
                        if ((long) num3 < Convert.ToInt64(newValue.Split(' ')[0]) + 1L)
                        {
                          ++num2;
                          if (DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 1)).DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 1)).DayOfWeek == DayOfWeek.Sunday)
                          {
                            if (dataRowArray3.Length > 0)
                            {
                              if (dataRowArray3[0].ItemArray[2].ToString() == "False")
                                ++num2;
                            }
                            else
                              ++num2;
                          }
                          if (dataRowArray4.Length > 0)
                          {
                            if (dataRowArray4[0].ItemArray[2].ToString() == "False")
                            {
                              DataSet ds = this.holidays.view_holidays(DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 1)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) (num3 + 2)).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000", this.account_id);
                              if (this.utilities.isValidDataset(ds) && ds.Tables[0].Rows.Count > 0)
                                ++num2;
                            }
                          }
                          else
                            ++num2;
                          ++num3;
                        }
                        else
                          break;
                      }
                      dateTime2 = DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) num2);
                    }
                    if (assetBooking3.book_from < dateTime2)
                    {
                      this.alt_error_alrdybook.Visible = true;
                      this.litError.Text = Resources.fbs.booking_lead_time_error.Replace("[date]", dateTime2.ToString(api_constants.display_datetime_format_short)).Replace("[day]", newValue);
                      this.btn_save.Visible = false;
                      return;
                    }
                  }
                }
              }
            }
            asset_booking assetBooking4 = this.bookings.update_booking(assetBooking3);
            this.bookings.update_event_id(assetBooking4.booking_id, this.eventid, assetBooking4.account_id);
            this.add_workflow(assetBooking4, num1);
            foreach (asset_booking_invite assetBookingInvite in assetBooking1.invites.Values)
            {
              assetBookingInvite.booking_id = assetBooking4.booking_id;
              assetBookingInvite.created_by = this.current_user.user_id;
              assetBookingInvite.created_on = DateTime.UtcNow;
              assetBookingInvite.modified_by = this.current_user.user_id;
              assetBookingInvite.modified_on = DateTime.UtcNow;
              assetBookingInvite.repeat_reference_id = assetBooking4.repeat_reference_id;
              this.bookings.update_invite(assetBookingInvite);
            }
            asset_booking booking = this.bookings.get_booking(assetBooking4.booking_id, assetBooking4.account_id);
            bookings.Add(booking);
            if (Convert.ToBoolean(this.current_account.properties["send_email"]))
              this.bookingsbl.send_booking_emails(bookings);
          }
        }
      }
    }
    catch
    {
    }
    this.Response.Redirect("booking_view.aspx?id=" + (object) assetBooking1.booking_id);
  }
}
