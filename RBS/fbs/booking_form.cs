// Decompiled with JetBrains decompiler
// Type: booking_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public class booking_form : fbs_base_page, IRequiresSessionState
{
  protected HiddenField hdnCloneBookingID;
  protected Literal Literal1;
  protected HtmlGenericControl alt_error_alrdybook;
  protected Label lbl_assetname_heading;
  protected HtmlGenericControl li_invite_list;
  protected HtmlGenericControl li_FandB_list;
  protected HtmlGenericControl li_termsandconditions;
  protected Literal litError;
  protected HtmlGenericControl alertError;
  protected HiddenField hdnBookingId;
  protected HiddenField hdn_restricted_message;
  protected HiddenField hdnStart;
  protected HiddenField hdnEnd;
  protected HiddenField hdnAsset;
  protected HiddenField hdnBookingType;
  protected HiddenField hdnSeq;
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
  protected HtmlGenericControl erroremail;
  protected HtmlInputText txt_telephone;
  protected HtmlGenericControl errortelephone;
  protected HtmlTextArea txt_remarks;
  protected HtmlGenericControl remarks_error;
  protected CheckBox chk_housekeeping;
  protected Panel pnl_housekeeping;
  protected HtmlGenericControl div_setup_required;
  protected CheckBox chk_setup;
  protected HtmlGenericControl div_chk_setup;
  protected HtmlGenericControl div_setup_type;
  protected DropDownList ddl_setupType;
  protected HtmlGenericControl errorsetuptype;
  protected HtmlGenericControl div_ddl_setype_type;
  protected HtmlGenericControl div_meeting_type;
  protected DropDownList ddlMeetingType;
  protected HtmlGenericControl errorMeetingType;
  protected HtmlGenericControl div_ddl_meeting_type;
  protected HtmlGenericControl lblassets;
  protected Repeater rpt_list_table;
  protected HtmlGenericControl contrlgrp_resource;
  protected HtmlTableRow row_additional_resources;
  protected HtmlAnchor anchor_download;
  protected HtmlTableRow download_lnk;
  protected CheckBox chk_terms;
  protected HtmlGenericControl lbterms;
  protected HtmlGenericControl errorTerms;
  protected HtmlGenericControl divterms;
  protected HtmlTableRow row_tremsandcondition;
  protected HiddenField hdn_invitee;
  protected TextBox txtUser;
  protected Panel pnl_invitee;
  protected AutoCompleteExtender AutoCompleteExtender1;
  protected Button btnAssignToGroup;
  protected GridView gdInviteList;
  protected HtmlGenericControl lblTerms;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlForm form1;
  public bool is_edit;
  private long asset_id;
  private long bookingId;
  private long cloneBookingId;
  private DateTime startdatetime;
  private DateTime enddatetime;
  private DataSet assetData = new DataSet();
  private DataSet setting_data;
  private DataSet asset_pro_ds;
  public string POP_height = "";
  private string type = "";
  private string invite_mail_address = "";
  private List<asset_booking_invite> list_of_invites = new List<asset_booking_invite>();
  private long asset_owner_Group_id;
  private Dictionary<string, string> selectedDates = new Dictionary<string, string>();
  public string html_resourcelist;
  private double total_amount;
  private int total_quantity;
  public bool showFandB;
  private static DataSet resSetDs;
  public bool has_tandc;
  private asset obj_asset;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.Session.Remove("added_resources");
    if (this.u_group.group_type == 0)
      this.redirect_unauthorized();
    this.initialize_data();
    this.process_resend_email();
    this.process_clone();
    this.process_new();
    this.check_conflict();
  }

  private void check_conflict()
  {
    Convert.ToDateTime(this.Request.QueryString["start"]);
    Convert.ToDateTime(this.Request.QueryString["end"]);
  }

  private void initialize_data()
  {
    this.setting_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
    if (this.setting_data == null)
    {
      this.setting_data = this.settings.view_settings(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.setting_data);
    }
    try
    {
      if (Convert.ToBoolean(ConfigurationManager.AppSettings["Inivite_list"]))
        this.li_invite_list.Visible = true;
      else
        this.li_invite_list.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void process_new()
  {
    if (this.Request.QueryString["r"] != null)
    {
      this.asset_id = Convert.ToInt64(this.Request.QueryString["r"]);
      this.assetData = this.assets.view_assets(this.asset_id, this.current_user.account_id);
      this.asset_pro_ds = this.assets.view_asset_properties(this.asset_id, this.current_user.account_id);
      this.obj_asset = this.assets.get_asset(this.asset_id, this.current_user.account_id);
      this.lbl_assetname_heading.Text = this.obj_asset.code.ToString() + "/" + this.obj_asset.name.ToString();
      if (!this.IsPostBack)
        this.configure_form(this.obj_asset.asset_properties);
      if (!this.gp.isAdminType)
      {
        bool flag = false;
        foreach (string key in this.current_user.groups.Keys)
        {
          if (this.obj_asset.asset_owner_group_id == this.current_user.groups[key].group_id)
            flag = true;
        }
        if (!flag)
        {
          if (!this.bookingsbl.can_book(this.obj_asset.asset_id, this.obj_asset.asset_properties, this.setting_data, Convert.ToDateTime(this.Request.QueryString["start"]), Convert.ToDateTime(this.Request.QueryString["end"])))
          {
            this.alertError.Attributes.Add("style", "display: block;");
            this.litError.Text = Resources.fbs.operating_hours_booking_msg;
            this.btn_submit.Visible = false;
            return;
          }
        }
      }
    }
    try
    {
      this.type = this.Request.QueryString["type"];
      if (this.type.ToLower() == "repeat")
        this.hdnBookingType.Value = this.bookings.get_booking_type(Convert.ToInt32((object) api_constants.booking_type.Repeat));
      else if (this.type.ToLower() == "wizard")
        this.hdnBookingType.Value = this.bookings.get_booking_type(Convert.ToInt32((object) api_constants.booking_type.Wizard));
      else if (this.type.ToLower() == "quick")
        this.hdnBookingType.Value = this.bookings.get_booking_type(Convert.ToInt32((object) api_constants.booking_type.Quick));
    }
    catch
    {
      this.type = "";
    }
    if (!(this.type == ""))
    {
      if (this.type != null)
      {
        if (!(this.type.ToLower() == "wizard"))
        {
          if (!(this.type.ToLower() == "quick"))
          {
            this.selectedDates = (Dictionary<string, string>) this.Session["SelectedDates"];
            goto label_27;
          }
        }
      }
    }
    try
    {
      this.selectedDates.Add(Convert.ToDateTime(this.Request.QueryString["start"]).ToString(api_constants.display_datetime_format), Convert.ToDateTime(this.Request.QueryString["end"]).ToString(api_constants.display_datetime_format));
    }
    catch (Exception ex)
    {
      this.startdatetime = new DateTime(1900, 1, 1);
      this.enddatetime = new DateTime(1900, 1, 1);
    }
label_27:
    if (this.IsPostBack)
      return;
    this.populate_meeting_type();
    this.SetInitialRow(new DataTable());
    this.txtBookedFor.Text = this.current_user.full_name;
    this.hfUserId.Value = this.current_user.user_id.ToString();
    this.hdnAsset.Value = this.asset_id.ToString();
    if (this.type == "quick")
    {
      DateTime dateTime1 = Convert.ToDateTime(this.Request.QueryString["start"]).AddSeconds(1.0);
      DateTime dateTime2 = Convert.ToDateTime(this.Request.QueryString["end"]).AddSeconds(-1.0);
      DataSet dataSet = new DataSet();
      DataSet ds = this.bookings.check_bookings(this.asset_id, dateTime1.ToString(api_constants.sql_datetime_format), dateTime2.ToString(api_constants.sql_datetime_format), this.current_user.account_id);
      if (this.utilities.isValidDataset(ds))
      {
        this.alertError.Attributes.Add("style", "display: block;");
        this.litError.Text = Resources.fbs.cant_book_room_in_timeslot;
        this.btn_submit.Visible = false;
        return;
      }
      ds.Dispose();
    }
    if (!this.gp.isAdminType)
    {
      try
      {
        foreach (string key in this.selectedDates.Keys)
        {
          DateTime dateTime3 = Convert.ToDateTime(key);
          Convert.ToDateTime(this.selectedDates[key]);
          DataRow[] dataRowArray1 = this.asset_pro_ds.Tables[0].Select("asset_id=" + this.asset_id.ToString() + " and property_name='booking_lead_time'");
          DataRow[] dataRowArray2 = this.asset_pro_ds.Tables[0].Select("asset_id=" + this.asset_id.ToString() + " and property_name='book_weekend'");
          DataRow[] dataRowArray3 = this.asset_pro_ds.Tables[0].Select("asset_id=" + this.asset_id.ToString() + " and property_name='book_holiday'");
          if (dataRowArray1.Length > 0)
          {
            string newValue = dataRowArray1[0].ItemArray[2].ToString();
            if (newValue != "")
            {
              if (newValue.Split(' ')[0] != "0")
              {
                DateTime dateTime4 = new DateTime();
                DateTime dateTime5;
                if (newValue.Split(' ')[1] == "Hour(s)")
                {
                  dateTime5 = DateTime.UtcNow.AddHours(this.current_account.timezone).AddHours(Convert.ToDouble(newValue.Split(' ')[0]));
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
                  dateTime5 = DateTime.UtcNow.AddHours(this.current_account.timezone).Date.AddDays((double) num1);
                }
                if (dateTime3 < dateTime5)
                {
                  this.alertError.Attributes.Add("style", "display: block;");
                  this.litError.Text = Resources.fbs.booking_lead_time_error.Replace("[date]", dateTime5.ToString(api_constants.display_datetime_format_short)).Replace("[day]", newValue);
                  this.btn_submit.Visible = false;
                  return;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
      }
    }
    this.txt_email.Text = this.current_user.email;
    this.txt_telephone.Value = this.current_user.properties["staff_offphone"].property_value + " - " + this.current_user.properties["staff_pager_mobile"].property_value;
    this.hdnBookingId.Value = "0";
    if (this.check_asset_restricted(this.obj_asset))
    {
      if (this.u_group.group_type == 0 || this.u_group.group_type == 3 || this.u_group.group_type == 1)
      {
        this.alertError.Attributes.Add("style", "display: block;");
        this.litError.Text = this.hdn_restricted_message.Value;
        this.btn_submit.Visible = false;
        return;
      }
      if (this.obj_asset.asset_owner_group_id > 0L && this.obj_asset.asset_owner_group_id != this.u_group.group_id)
      {
        this.alertError.Attributes.Add("style", "display: block;");
        this.litError.Text = this.hdn_restricted_message.Value;
        this.btn_submit.Visible = false;
        return;
      }
    }
    this.lblassets.InnerHtml = this.bookingsbl.getAssetHtml_with_bookingDates(this.assetData, this.setting_data, this.asset_pro_ds, this.asset_id, this.selectedDates).ToString();
  }

  private void process_existing_booking()
  {
    try
    {
      this.bookingId = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error --> " + ex.ToString()));
      this.bookingId = 0L;
    }
    if (this.bookingId <= 0L)
      return;
    this.selectedDates = new Dictionary<string, string>();
    this.is_edit = true;
    asset_booking booking = this.bookings.get_booking(this.bookingId, this.current_user.account_id);
    this.hdnSeq.Value = booking.sequence.ToString();
    this.obj_asset = this.assets.get_asset(booking.asset_id, this.current_user.account_id);
    this.lbl_assetname_heading.Text = this.obj_asset.code.ToString() + "/" + this.obj_asset.name.ToString();
    this.asset_id = booking.asset_id;
    this.assetData = this.assets.view_assets(this.asset_id, this.current_user.account_id);
    this.asset_pro_ds = this.assets.view_asset_properties(this.asset_id, this.current_user.account_id);
    this.selectedDates.Add(booking.book_from.ToString(), booking.book_to.ToString());
    this.hdnBookingType.Value = this.bookings.get_booking_type(booking.booking_type);
    user user = new user();
    if (booking.booked_for != this.current_user.user_id)
    {
      this.users.get_user(Convert.ToInt64(booking.booked_for), this.current_user.account_id);
    }
    else
    {
      user currentUser = this.current_user;
    }
    this.configure_form(this.obj_asset.asset_properties);
    if (this.IsPostBack)
      return;
    this.populate_details(this.bookingId, "edit", booking, this.obj_asset);
    DataSet invites = this.bookings.get_invites(this.bookingId, this.current_user.account_id);
    if (this.utilities.isValidDataset(invites))
      this.SetInitialRow(invites.Tables[0]);
    else
      this.SetInitialRow(new DataTable());
    this.lblassets.Visible = false;
    this.rpt_list_table.Visible = true;
    this.rpt_list_table.DataSource = (object) this.bookingsbl.getAssetDTable_with_bookingDates(this.assetData, this.setting_data, this.asset_pro_ds, this.asset_id, this.selectedDates);
    this.rpt_list_table.DataBind();
    this.populate_resourcelist(this.bookingId);
  }

  private void process_clone()
  {
    try
    {
      if (this.Session["CloneID"] != (object) "")
      {
        this.cloneBookingId = Convert.ToInt64(this.Session["CloneID"]);
        this.hdnBookingType.Value = this.bookings.get_booking_type(Convert.ToInt32((object) api_constants.booking_type.Clone));
      }
    }
    catch (Exception ex)
    {
      this.cloneBookingId = 0L;
      fbs_base_page.log.Error((object) ("Error --> " + ex.ToString()));
    }
    if (this.cloneBookingId <= 0L)
      return;
    this.hdnCloneBookingID.Value = this.cloneBookingId.ToString();
    this.type = "clone";
    asset_booking booking = this.bookings.get_booking(this.cloneBookingId, this.current_user.account_id);
    asset asset = this.assets.get_asset(booking.asset_id, this.current_user.account_id);
    user user = new user();
    if (booking.booked_for != this.current_user.user_id)
    {
      this.users.get_user(Convert.ToInt64(booking.booked_for), this.current_user.account_id);
    }
    else
    {
      user currentUser = this.current_user;
    }
    this.hdnBookingType.Value = this.bookings.get_booking_type(Convert.ToInt32((object) api_constants.booking_type.Clone));
    try
    {
      this.asset_id = Convert.ToInt64(this.Request.QueryString["r"]);
    }
    catch
    {
    }
    this.assetData = this.assets.view_assets(this.asset_id, this.current_user.account_id);
    this.asset_pro_ds = this.assets.view_asset_properties(this.asset_id, this.current_user.account_id);
    try
    {
      string str = this.Request.QueryString["start"];
      this.startdatetime = Convert.ToDateTime(this.Request.QueryString["start"]);
      this.enddatetime = Convert.ToDateTime(this.Request.QueryString["end"]);
      this.selectedDates.Add(Convert.ToDateTime(this.startdatetime).ToString(api_constants.display_datetime_format), Convert.ToDateTime(this.enddatetime).ToString(api_constants.display_datetime_format));
    }
    catch (Exception ex)
    {
      this.startdatetime = new DateTime(1900, 1, 1);
      this.enddatetime = new DateTime(1900, 1, 1);
    }
    this.configure_form(asset.asset_properties);
    if (this.IsPostBack)
      return;
    this.populate_meeting_type();
    this.txtBookedFor.Text = this.current_user.full_name;
    this.hfUserId.Value = this.current_user.user_id.ToString();
    this.hdnAsset.Value = this.asset_id.ToString();
    this.populate_details(this.cloneBookingId, "clone", booking, asset);
    DataSet invites = this.bookings.get_invites(this.cloneBookingId, this.current_user.account_id);
    if (this.utilities.isValidDataset(invites))
      this.SetInitialRow(invites.Tables[0]);
    else
      this.SetInitialRow(new DataTable());
    this.populate_resourcelist(this.cloneBookingId);
    this.lblassets.InnerHtml = this.bookingsbl.getAssetHtml_with_bookingDates(this.assetData, this.setting_data, this.asset_pro_ds, this.asset_id, this.selectedDates).ToString();
  }

  private void process_resend_email()
  {
    if (this.Request.QueryString["Email_resend"] == null || string.IsNullOrEmpty(this.Request.QueryString["Email_resend"].ToString()) || !(this.Request.QueryString["Email_resend"].ToString() == "Y"))
      return;
    this.ResendEmail(this.Request.QueryString["booking_ID"].ToString());
    this.Response.End();
    this.Response.Clear();
    this.Response.Flush();
  }

  private void configure_form(Dictionary<long, asset_property> data)
  {
    try
    {
      asset_property assetProperty1 = new asset_property();
      foreach (asset_property assetProperty2 in data.Values)
      {
        if (assetProperty2.property_name == "allow_housekeeping")
        {
          assetProperty1 = assetProperty2;
          break;
        }
      }
      if (Convert.ToBoolean(assetProperty1.property_value))
        this.pnl_housekeeping.Visible = true;
      else
        this.pnl_housekeeping.Visible = false;
      foreach (asset_property assetProperty3 in data.Values)
      {
        if (assetProperty3.property_name == "show_terms_and_conditions")
        {
          assetProperty1 = assetProperty3;
          break;
        }
      }
      if (Convert.ToBoolean(assetProperty1.property_value))
      {
        this.get_terms_conditions();
        this.divterms.Visible = true;
        this.has_tandc = true;
      }
      else
      {
        this.divterms.Visible = false;
        this.has_tandc = false;
      }
      foreach (asset_property assetProperty4 in data.Values)
      {
        if (assetProperty4.property_name == "allow_setup")
        {
          assetProperty1 = assetProperty4;
          break;
        }
      }
      if (assetProperty1.asset_property_id > 0L)
      {
        bool boolean = Convert.ToBoolean(assetProperty1.property_value);
        this.div_setup_required.Visible = boolean;
        this.div_chk_setup.Visible = boolean;
      }
      else
      {
        this.div_setup_required.Visible = false;
        this.div_chk_setup.Visible = false;
        this.div_setup_type.Visible = false;
        this.div_ddl_setype_type.Visible = false;
      }
      if (this.IsPostBack)
        return;
      if (this.check_invite_list_visible())
      {
        this.li_invite_list.Visible = true;
        this.SetInitialRow(new DataTable());
      }
      else
        this.li_invite_list.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error2:" + ex.ToString()));
      this.has_tandc = false;
      this.pnl_housekeeping.Visible = false;
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
        this.row_additional_resources.Visible = false;
        this.contrlgrp_resource.Visible = false;
      }
      else
      {
        this.row_additional_resources.Visible = true;
        this.contrlgrp_resource.Visible = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  public bool check_bookings()
  {
    bool flag = true;
    try
    {
      foreach (string key in this.selectedDates.Keys)
      {
        this.startdatetime = Convert.ToDateTime(key);
        this.enddatetime = Convert.ToDateTime(this.selectedDates[key]);
        DataSet dataSet = this.bookings.check_bookings(Convert.ToInt64(this.hdnAsset.Value), this.startdatetime.AddMinutes(2.0).ToString(api_constants.sql_datetime_format), this.enddatetime.AddMinutes(-2.0).ToString(api_constants.sql_datetime_format), this.current_user.account_id);
        if (dataSet != null)
        {
          if (dataSet.Tables[0].Rows.Count > 0)
            flag = false;
          else if (flag)
            flag = true;
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return flag;
  }

  private void populate_meeting_type()
  {
    if (!this.utilities.isValidDataset(this.setting_data))
      return;
    foreach (DataRow dataRow in this.setting_data.Tables[0].Select("parameter='meeting_type' "))
      this.ddlMeetingType.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
  }

  private void populate_details(long bid, string type, asset_booking objBooking, asset obj_asset)
  {
    try
    {
      this.hdnBookingId.Value = bid.ToString();
      if (type != "clone")
      {
        this.asset_id = objBooking.asset_id;
        this.startdatetime = objBooking.book_from;
        this.enddatetime = objBooking.book_to;
        this.hdnAsset.Value = objBooking.asset_id.ToString();
      }
      this.txt_purpose.Value = objBooking.purpose;
      this.hfUserId.Value = objBooking.booked_for.ToString();
      user user = new user();
      this.txtBookedFor.Text = (objBooking.booked_for == this.current_user.user_id ? this.current_user : this.users.get_user(objBooking.booked_for, this.current_user.account_id)).full_name;
      this.txt_email.Text = objBooking.email;
      this.txt_telephone.Value = objBooking.contact;
      this.txt_remarks.Value = objBooking.remarks;
      this.chk_housekeeping.Checked = objBooking.housekeeping_required;
      this.ddlMeetingType.SelectedValue = objBooking.meeting_type.ToString();
      if (!objBooking.setup_required)
        return;
      this.div_setup_required.Visible = true;
      this.chk_setup.Checked = true;
      this.div_setup_type.Visible = true;
      this.div_ddl_setype_type.Visible = true;
      this.asset_pro_ds.Tables[0].Select("asset_id=" + this.asset_id.ToString() + "and Property_name='setup_type'");
      string str = "";
      foreach (asset_property assetProperty in obj_asset.asset_properties.Values)
      {
        if (assetProperty.property_name == "setup_type")
          str = str + assetProperty.property_value + ",";
      }
      if (str.Trim().Length > 0)
        str = " Setting_id IN (" + str.Substring(0, str.Length - 1) + ")";
      if (!this.utilities.isValidDataset(this.setting_data))
        return;
      string filterExpression = "parameter='setup_type' ";
      if (!string.IsNullOrEmpty(str))
        filterExpression = filterExpression + " and " + str;
      DataRow[] dataRowArray = this.setting_data.Tables[0].Select(filterExpression);
      this.ddl_setupType.Items.Clear();
      foreach (DataRow dataRow in dataRowArray)
        this.ddl_setupType.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddl_setupType.SelectedValue = objBooking.setup_type.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void get_terms_conditions()
  {
    try
    {
      template template = this.tapi.get_template("terms_and_conditions", this.current_user.account_id);
      if (template.template_id <= 0L)
        return;
      this.lblTerms.InnerHtml = template.content_data;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private bool check_invite_list_visible()
  {
    try
    {
      return this.utilities.isValidDataset(this.setting_data) && this.setting_data.Tables[0].Select("parameter='show_invite_list' and value='True'").Length > 0;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return false;
  }

  private bool check_asset_restricted(asset objAsset)
  {
    try
    {
      if (objAsset.asset_id != 0L)
      {
        string[] strArray = this.assets.get_restricted_remarks(objAsset.asset_properties, "is_restricted_message").Split('|');
        if (strArray.Length > 0)
        {
          try
          {
            if (strArray.Length >= 2)
            {
              this.hdn_restricted_message.Value = strArray[1].ToString();
              if (strArray[1].ToString() != "")
              {
                if (Convert.ToInt64(objAsset.owner_group.group_id) != Convert.ToInt64(this.u_group.group_id))
                {
                  this.row_tremsandcondition.Visible = false;
                  this.li_invite_list.Visible = false;
                  this.li_invite_list.Visible = false;
                  this.li_termsandconditions.Visible = false;
                }
              }
              else
              {
                this.row_tremsandcondition.Visible = true;
                this.li_invite_list.Visible = true;
                this.row_tremsandcondition.Visible = true;
                this.li_termsandconditions.Visible = true;
              }
            }
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) "Error -> ", ex);
          }
        }
        this.hdn_restricted_message.Value = this.hdn_restricted_message.Value;
        return objAsset.is_restricted;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return false;
  }

  protected void chk_setup_CheckedChanged(object sender, EventArgs e)
  {
    try
    {
      if (this.chk_setup.Checked)
      {
        this.asset_id = Convert.ToInt64(this.hdnAsset.Value);
        this.asset_pro_ds = this.assets.view_asset_properties(this.asset_id, this.current_user.account_id, new string[1]
        {
          "setup_type"
        });
        this.div_setup_type.Visible = true;
        this.div_ddl_setype_type.Visible = true;
        DataRow[] dataRowArray1 = this.asset_pro_ds.Tables[0].Select("asset_id=" + this.asset_id.ToString() + "and Property_name='setup_type'");
        string str = "";
        foreach (DataRow dataRow in dataRowArray1)
        {
          if (dataRow["property_value"].ToString() != "")
            str = str + dataRow["property_value"].ToString() + ",";
        }
        if (str.Trim().Length > 0)
          str = " Setting_id IN (" + str.Substring(0, str.Length - 1) + ")";
        if (!this.utilities.isValidDataset(this.setting_data))
          return;
        string filterExpression = "parameter='setup_type' ";
        if (!string.IsNullOrEmpty(str))
          filterExpression = filterExpression + " and " + str;
        DataRow[] dataRowArray2 = this.setting_data.Tables[0].Select(filterExpression);
        this.ddl_setupType.Items.Clear();
        foreach (DataRow dataRow in dataRowArray2)
          this.ddl_setupType.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      }
      else
      {
        this.div_setup_type.Visible = false;
        this.div_ddl_setype_type.Visible = false;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Session["CloneID"] = (object) "";
      this.Session.Remove("CloneID");
      this.hdnCloneBookingID.Value = "";
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["Rdct"]))
      {
        if (this.Request.QueryString["Rdct"].ToString() == "repeat")
          Modal.Close((Page) this);
        else
          Modal.Close((Page) this, (object) "OK");
      }
      else
        Modal.Close((Page) this, (object) "OK");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      List<asset_booking> bookings_list = new List<asset_booking>();
      Guid guid = Guid.NewGuid();
      long[] numArray = new long[this.selectedDates.Count + 1];
      int index1 = 0;
      bool flag1 = false;
      string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
      bool flag2 = true;
      bool flag3 = false;
      asset_booking assetBooking1 = new asset_booking();
      asset_booking assetBooking2 = new asset_booking();
      try
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (this.rpt_list_table.Visible)
        {
          foreach (RepeaterItem repeaterItem in this.rpt_list_table.Items)
          {
            TextBox control1 = (TextBox) repeaterItem.FindControl("txt_startDate");
            DropDownList control2 = (DropDownList) repeaterItem.FindControl("ddl_startTime");
            TextBox control3 = (TextBox) repeaterItem.FindControl("txt_endDate");
            DropDownList control4 = (DropDownList) repeaterItem.FindControl("ddl_endTime");
            string text = control1.Text;
            DateTime dateTime1 = Convert.ToDateTime(control1.Text + " " + control2.SelectedItem.Value);
            DateTime dateTime2 = Convert.ToDateTime(control3.Text + " " + control4.SelectedItem.Value);
            if (DateTime.Compare(dateTime1, dateTime2) == -1)
            {
              dictionary.Add(dateTime1.ToString(), dateTime2.ToString());
              if (!this.check_bookings(Convert.ToInt64(this.hdnBookingId.Value), dictionary))
              {
                dictionary.Clear();
                flag2 = false;
                break;
              }
            }
            else
            {
              flag2 = false;
              break;
            }
          }
        }
        if (dictionary.Count > 0)
        {
          flag3 = false;
          if (this.selectedDates.Count == dictionary.Count)
          {
            for (int index2 = 0; index2 < this.selectedDates.Count; ++index2)
            {
              KeyValuePair<string, string> keyValuePair1 = this.selectedDates.ElementAt<KeyValuePair<string, string>>(index2);
              string key1 = keyValuePair1.Key;
              string str1 = keyValuePair1.Value;
              KeyValuePair<string, string> keyValuePair2 = dictionary.ElementAt<KeyValuePair<string, string>>(index2);
              string key2 = keyValuePair2.Key;
              string str2 = keyValuePair2.Value;
              if (key1 != key2 || str1 != str2)
              {
                flag3 = true;
                break;
              }
            }
          }
          else
            flag3 = true;
          this.selectedDates.Clear();
          this.selectedDates = dictionary;
          this.lblassets.InnerHtml = this.bookingsbl.getAssetHtml_with_bookingDates(this.assetData, this.setting_data, this.asset_pro_ds, this.asset_id, this.selectedDates).ToString();
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Error -> ", ex);
      }
      if (flag2)
      {
        string str3 = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        foreach (string key in this.selectedDates.Keys)
        {
          asset_booking assetBooking3 = new asset_booking();
          assetBooking3.booking_id = Convert.ToInt64(this.hdnBookingId.Value);
          try
          {
            assetBooking3.booking_type = this.bookings.get_bookingtype(this.hdnBookingType.Value);
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) "Error -> ", ex);
          }
          try
          {
            if (this.hdnCloneBookingID.Value != "")
            {
              if (Convert.ToInt64(this.hdnCloneBookingID.Value) > 0L)
              {
                this.hdnBookingId.Value = "0";
                assetBooking3.booking_id = 0L;
                this.bookingId = 0L;
              }
            }
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) "Error -> ", ex);
          }
          string str4;
          asset_booking obj_previous;
          if (assetBooking3.booking_id > 0L)
          {
            str4 = "update";
            obj_previous = this.bookings.get_booking(assetBooking3.booking_id, this.current_user.account_id);
            obj_previous.global_appointment_id = this.bookings.get_appointment_id(obj_previous.booking_id, obj_previous.account_id);
            assetBooking3.account_id = obj_previous.account_id;
            assetBooking3.is_repeat = obj_previous.is_repeat;
            assetBooking3.record_id = obj_previous.record_id;
            assetBooking3.repeat_reference_id = obj_previous.repeat_reference_id;
            assetBooking3.transfer_original_booking_id = obj_previous.transfer_original_booking_id;
            assetBooking3.transfer_reason = obj_previous.transfer_reason;
            assetBooking3.transfer_request = obj_previous.transfer_request;
            assetBooking3.created_on = obj_previous.created_on;
            assetBooking3.sequence = obj_previous.sequence + 1;
            assetBooking3.record_id = obj_previous.record_id;
            assetBooking3.global_appointment_id = obj_previous.global_appointment_id;
            assetBooking3.event_id = obj_previous.event_id;
            flag1 = true;
          }
          else
          {
            obj_previous = new asset_booking();
            obj_previous.booking_id = 0L;
            if (this.check_bookings())
              flag1 = true;
            else if (!flag1)
              flag1 = false;
            str4 = "insert";
            assetBooking3.account_id = this.current_user.account_id;
            assetBooking3.event_id = Guid.NewGuid();
            assetBooking3.record_id = guid;
            assetBooking3.repeat_reference_id = guid;
            assetBooking3.is_repeat = this.type == "repeat";
            assetBooking3.transfer_original_booking_id = 0L;
            assetBooking3.transfer_reason = "";
            assetBooking3.transfer_request = false;
            if (assetBooking3.created_on.Year <= 2000)
              assetBooking3.created_on = this.current_timestamp;
          }
          assetBooking3.asset_id = Convert.ToInt64(this.hdnAsset.Value);
          assetBooking3.book_from = Convert.ToDateTime(key);
          assetBooking3.book_to = Convert.ToDateTime(this.selectedDates[key]);
          assetBooking3.book_duration = (double) this.utilities.getDuration(assetBooking3.book_from, assetBooking3.book_to);
          assetBooking3.booked_for = Convert.ToInt64(this.hfUserId.Value);
          assetBooking3.contact = this.txt_telephone.Value;
          assetBooking3.created_by = this.current_user.user_id;
          assetBooking3.email = this.txt_email.Text;
          assetBooking3.housekeeping_required = this.chk_housekeeping.Checked;
          assetBooking3.modified_by = this.current_user.user_id;
          assetBooking3.purpose = this.txt_purpose.Value;
          assetBooking3.remarks = this.txt_remarks.Value;
          try
          {
            assetBooking3.meeting_type = Convert.ToInt64(this.ddlMeetingType.SelectedItem.Value);
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) "Error -> ", ex);
          }
          assetBooking3.setup_required = this.chk_setup.Checked;
          assetBooking3.setup_type = !this.chk_setup.Checked ? 0L : Convert.ToInt64(this.ddl_setupType.SelectedItem.Value);
          try
          {
            if (!this.utilities.isValidDataset(this.assetData))
              this.assetData = this.assets.view_assets(assetBooking3.asset_id, this.current_user.account_id);
            this.asset_owner_Group_id = Convert.ToInt64(this.assetData.Tables[0].Select("asset_id=" + assetBooking3.asset_id.ToString())[0]["asset_owner_group_id"]);
          }
          catch
          {
            this.asset_owner_Group_id = 0L;
          }
          if (assetBooking3.booking_id == 0L)
          {
            assetBooking3.status = this.bookings.get_booking_status(this.obj_asset.asset_owner_group_id, this.current_user, this.gp);
          }
          else
          {
            assetBooking3.status = obj_previous.status;
            if (flag3)
              assetBooking3.status = this.bookings.get_booking_status(this.obj_asset.asset_owner_group_id, this.current_user, this.gp);
          }
          if (flag1)
          {
            this.alt_error_alrdybook.Attributes.Add("style", "display: none;");
            this.alt_error_alrdybook.Visible = false;
            asset_booking objbk = this.bookings.update_booking(assetBooking3);
            try
            {
              this.bookings.set_book_status(objbk, obj_previous);
            }
            catch
            {
            }
            this.bookingId = objbk.booking_id;
            if (objbk.booking_id > 0L)
            {
              this.bookings.update_event_id(objbk.booking_id, objbk.event_id, objbk.account_id);
              if (flag3 && this.utilities.isValidDataset(this.resapi.get_resource_bookings_items_by_asset_booking_id(objbk.booking_id, this.current_user.account_id, "", "", this.str_resource_module)))
                this.resapi.update_resource_booking_date(objbk.booking_id, objbk.book_from.ToString(api_constants.sql_datetime_format), objbk.book_to.ToString(api_constants.sql_datetime_format), this.str_resource_module, this.current_user.user_id);
              if (this.hdnCloneBookingID.Value != "" && Convert.ToInt64(this.hdnCloneBookingID.Value) > 0L)
                this.clone_booking_additional_resources(Convert.ToInt64(this.hdnCloneBookingID.Value), this.bookingId, objbk);
              numArray[index1] = this.bookingId;
              ++index1;
              Dictionary<long, asset_booking_invite> dictionary = new Dictionary<long, asset_booking_invite>();
              foreach (asset_booking_invite assetBookingInvite in this.bookings.get_invite_list(this.bookingId, this.current_user.account_id).Values)
                this.bookings.delete_invite(assetBookingInvite);
            }
            asset_booking_invite assetBookingInvite1 = new asset_booking_invite();
            this.list_of_invites = new List<asset_booking_invite>();
            if (this.gdInviteList.Rows.Count > 0)
            {
              objbk.invites = new Dictionary<long, asset_booking_invite>();
              for (int index3 = 0; index3 < this.gdInviteList.Rows.Count; ++index3)
              {
                TextBox control5 = (TextBox) this.gdInviteList.Rows[index3].Cells[2].FindControl("txtName");
                TextBox control6 = (TextBox) this.gdInviteList.Rows[index3].Cells[3].FindControl("txtEmail_invite");
                Label control7 = (Label) this.gdInviteList.Rows[index3].Cells[0].FindControl("lblInviteID");
                string text1 = control5.Text;
                string text2 = control6.Text;
                long num = 0;
                if (Regex.IsMatch(text2, pattern) && !string.IsNullOrEmpty(text2))
                {
                  asset_booking_invite assetBookingInvite2 = new asset_booking_invite();
                  assetBookingInvite2.account_id = this.current_user.account_id;
                  assetBookingInvite2.booking_id = this.bookingId;
                  assetBookingInvite2.booking_invite_id = num;
                  assetBookingInvite2.created_by = this.current_user.user_id;
                  assetBookingInvite2.email = text2;
                  assetBookingInvite2.modified_by = this.current_user.user_id;
                  assetBookingInvite2.name = text1;
                  assetBookingInvite2.record_id = Guid.NewGuid();
                  assetBookingInvite2.repeat_reference_id = assetBookingInvite2.record_id;
                  asset_booking_invite assetBookingInvite3 = this.bookings.update_invite(assetBookingInvite2);
                  if (assetBookingInvite3.booking_invite_id > 0L)
                  {
                    this.list_of_invites.Add(assetBookingInvite3);
                    objbk.invites.Add(assetBookingInvite3.booking_invite_id, assetBookingInvite3);
                    this.invite_mail_address = this.invite_mail_address + text2 + ";";
                  }
                }
              }
            }
            else
              objbk.invites = new Dictionary<long, asset_booking_invite>();
            user user = new user();
            if (this.current_user.user_id != objbk.booked_for)
            {
              this.users.get_user(objbk.booked_for, this.current_user.account_id);
            }
            else
            {
              user currentUser = this.current_user;
            }
            if (str4 == "insert" && objbk.status != (short) 1)
            {
              workflow workflow = new workflow();
              workflow.account_id = this.current_user.account_id;
              workflow.action_owner_id = this.asset_owner_Group_id <= 0L ? this.current_user.user_id : this.asset_owner_Group_id;
              if (objbk.transfer_request)
              {
                workflow.action_remarks = "Transfer asset booking";
                if (this.asset_owner_Group_id > 0L && this.asset_owner_Group_id != this.u_group.group_id)
                {
                  workflow.action_owner_id = this.asset_owner_Group_id;
                  workflow.action_type = (short) 2;
                }
                else
                {
                  DataSet userGroup = this.bookings.get_user_group(objbk.transfer_original_booking_id, this.current_user.account_id);
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
              workflow.created_by = Convert.ToInt64(this.Request.Form[this.hfUserId.UniqueID]);
              workflow.created_on = this.current_timestamp;
              workflow.due_on = new DateTime(1900, 1, 1);
              workflow.record_id = Guid.NewGuid();
              workflow.reference_id = objbk.booking_id;
              workflow.workflow_id = 0L;
              XmlDocument xmlDocument = new XmlDocument();
              xmlDocument.LoadXml("<properties><asset_id>" + (object) objbk.asset_id + " </asset_id></properties>");
              workflow.properties = xmlDocument;
              this.workflows.update_workflow(workflow);
            }
            objbk.global_appointment_id = str3;
            bookings_list.Add(objbk);
          }
          else
          {
            this.btn_submit.Visible = false;
            this.li_invite_list.Visible = false;
            this.li_termsandconditions.Visible = false;
            this.alt_error_alrdybook.Visible = true;
            this.alt_error_alrdybook.Attributes.Add("style", "display: block;");
            return;
          }
        }
        try
        {
          this.bookingsbl.update_outlook(bookings_list);
        }
        catch
        {
        }
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          Parallel.Invoke((Action) (() => this.bookingsbl.send_booking_emails(bookings_list)));
        if (Convert.ToInt32(this.hdnBookingId.Value) > 0)
        {
          this.Session["CloneID"] = (object) "";
          this.Session.Remove("CloneID");
          this.hdnCloneBookingID.Value = "";
          this.Response.Redirect("booking_view.aspx?id=" + this.hdnBookingId.Value);
        }
        else if (Convert.ToInt32(this.bookingId) > 0)
        {
          this.Session["CloneID"] = (object) "";
          this.Session.Remove("CloneID");
          this.hdnCloneBookingID.Value = "";
          this.Response.Redirect("booking_view.aspx?id=" + (object) this.bookingId);
        }
        else
        {
          this.litError.Text = "<strong>Error!</strong>" + Resources.fbs.booking_confirmation_failuer;
          this.alertError.Attributes.Add("style", "display: block;");
        }
      }
      else
      {
        this.btn_submit.Visible = false;
        this.li_invite_list.Visible = false;
        this.li_termsandconditions.Visible = false;
        this.alt_error_alrdybook.Visible = true;
        this.alt_error_alrdybook.Attributes.Add("style", "display: block;");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void clone_booking_additional_resources(
    long cl_bk_id,
    long current_bk_id,
    asset_booking objbk)
  {
    try
    {
      DataSet byAssetBookingId = this.resapi.get_resource_bookings_items_by_asset_booking_id(cl_bk_id, this.current_user.account_id, "", "", this.str_resource_module);
      if (!this.utilities.isValidDataset(byAssetBookingId))
        return;
      booking_form.resSetDs = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_settings");
      if (booking_form.resSetDs == null)
      {
        booking_form.resSetDs = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_settings", (object) booking_form.resSetDs);
      }
      this.assets.view_assets(objbk.asset_id, this.current_user.account_id);
      resource_booking resourceBooking1 = new resource_booking();
      resourceBooking1.account_id = this.current_user.account_id;
      resourceBooking1.asset_booking_id = current_bk_id;
      resourceBooking1.book_from = objbk.book_from;
      resourceBooking1.book_to = objbk.book_to;
      resourceBooking1.booked_for_id = objbk.booked_for;
      DataRow[] dataRowArray1 = booking_form.resSetDs.Tables[0].Select("value='Additional Resources with asset booking'");
      resourceBooking1.booking_type = Convert.ToInt32(dataRowArray1[0]["setting_id"]);
      resourceBooking1.created_by = this.current_user.user_id;
      resourceBooking1.created_on = this.current_timestamp;
      resourceBooking1.email = objbk.email;
      resourceBooking1.item_id = 0L;
      resourceBooking1.layout_id = 0L;
      resourceBooking1.modified_by = this.current_user.user_id;
      resourceBooking1.modified_on = this.current_timestamp;
      resourceBooking1.module_name = this.str_resource_module;
      resourceBooking1.purpose = objbk.purpose;
      resourceBooking1.record_id = Guid.NewGuid();
      resourceBooking1.remarks = objbk.remarks;
      resourceBooking1.requested_by_id = objbk.created_by;
      resourceBooking1.resource_booking_id = 0L;
      resourceBooking1.status = 1;
      DataRow[] dataRowArray2 = this.assetData.Tables[0].Select("asset_id=" + (object) objbk.asset_id);
      resourceBooking1.venue = dataRowArray2[0]["name"].ToString();
      resource_booking resourceBooking2 = this.resapi.update_resource_booking(resourceBooking1);
      if (resourceBooking2.resource_booking_id <= 0L)
        return;
      long resourceBookingId = resourceBooking2.resource_booking_id;
      foreach (DataRow row1 in (InternalDataCollectionBase) byAssetBookingId.Tables[0].Rows)
      {
        resource_booking_item objRes = new resource_booking_item();
        objRes.account_id = this.current_user.account_id;
        objRes.created_by = this.current_user.user_id;
        objRes.modified_by = this.current_user.user_id;
        objRes.module_name = this.str_resource_module;
        objRes.other_remarks = "";
        objRes.record_id = Guid.NewGuid();
        objRes.req_price = 0.0;
        objRes.resource_id = Convert.ToInt64(row1["resource_id"]);
        double num1 = 0.0;
        double num2 = 0.0;
        try
        {
          num1 = Convert.ToDouble(row1["requested_qty"]);
          num2 = Convert.ToDouble(row1["accepted_qty"]);
        }
        catch
        {
        }
        objRes.req_qty = num1;
        objRes.requestor_remakrs = row1["requestor_remarks"].ToString();
        objRes.resource_booking_id = resourceBookingId;
        objRes.resource_booking_item_id = 0L;
        objRes.status = 1;
        objRes.accepted_price = 0.0;
        objRes.accepted_qty = num2;
        resource_booking_item resourceBookingItem = this.resapi.update_resource_booking_item(objRes);
        try
        {
          DataSet usersByGroup = this.users.get_users_by_group(this.resapi.get_resource_item_obj(resourceBookingItem.resource_id, this.current_user.account_id, this.str_resource_module).owner_group_id, this.current_user.account_id);
          string str1 = "";
          if (this.utilities.isValidDataset(usersByGroup))
          {
            foreach (DataRow row2 in (InternalDataCollectionBase) usersByGroup.Tables[0].Rows)
              str1 = str1 + row2["email"].ToString() + ";";
          }
          if (str1 != "")
          {
            string str2 = str1.Trim().Substring(0, str1.Length - 1);
            template template1 = new template();
            template template2 = this.tapi.get_template("email_resource_owner_request", this.current_user.account_id);
            string body = this.replaceTemplate_resource_owner(template2.content_data, objRes, resourceBooking1);
            string title = template2.title;
            string cc = "";
            string bcc = "";
            string to = str2;
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
      try
      {
        template template = this.tapi.get_template("email_resource_request_requestor", this.current_user.account_id);
        string body = this.replaceTemplate_resource_requestor(template.content_data, resourceBooking1, resourceBookingId);
        string title = template.title;
        string cc = "";
        string bcc = "";
        string email1 = resourceBooking1.email;
        email email2 = new email();
        if (!Convert.ToBoolean(this.current_account.properties["send_email"]))
          return;
        this.utilities.sendEmail(bcc, body, title, cc, email1, resourceBooking1.record_id);
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Send email to resource requestor - Error -> ", ex);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  public bool ResendEmail(string boooking_id)
  {
    try
    {
      asset_booking booking = this.bookings.get_booking(Convert.ToInt64(boooking_id), this.current_user.account_id);
      this.selectedDates.Add(booking.book_from.ToString(), booking.book_to.ToString());
      this.bookingsbl.getAssetHtml_with_bookingDates(this.assetData, this.setting_data, this.asset_pro_ds, booking.asset_id, this.selectedDates);
      DataRow[] dataRowArray = this.assetData.Tables[0].Select("asset_id=" + booking.asset_id.ToString());
      try
      {
        this.asset_owner_Group_id = Convert.ToInt64(dataRowArray[0]["asset_owner_group_id"]);
      }
      catch
      {
        this.asset_owner_Group_id = 0L;
      }
      user user = this.users.get_user(booking.booked_for, this.current_user.account_id);
      if (Convert.ToBoolean(this.current_account.properties["send_email"]))
        this.bookingsbl.send_booking_emails(user, this.lblassets.InnerHtml, booking, this.asset_owner_Group_id, this.setting_data, this.invite_mail_address);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return true;
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
    try
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
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void SetPreviousData()
  {
    try
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
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void ButtonAdd_Click(object sender, ImageClickEventArgs e) => this.AddNewRowToGrid();

  protected void gdInviteList_RowCreated(object sender, GridViewRowEventArgs e)
  {
    try
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
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void gdInviteList_RowDataBound(object sender, GridViewRowEventArgs e)
  {
    try
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
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
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

  public DataTable bind_ddTimeList()
  {
    DataTable dataTable = new DataTable();
    try
    {
      dataTable.Columns.Add("ddText");
      dataTable.Columns.Add("ddValue");
      string str = this.current_timestamp.ToShortDateString() + " 00:00 AM";
      DateTime dateTime = new DateTime(this.current_timestamp.Year, this.current_timestamp.Month, this.current_timestamp.Day, 0, 0, 0);
      for (int index = 0; index <= 95; ++index)
      {
        DataRow row = dataTable.NewRow();
        row["ddText"] = (object) dateTime.ToShortTimeString();
        row["ddValue"] = (object) dateTime.ToShortTimeString();
        dateTime = dateTime.AddMinutes(this.AllowedMinutes);
        dataTable.Rows.Add(row);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
    return dataTable;
  }

  protected void rpt_list_table_ItemDataBound(object sender, RepeaterItemEventArgs e)
  {
    try
    {
      DropDownList control1 = (DropDownList) e.Item.FindControl("ddl_startTime");
      HiddenField control2 = (HiddenField) e.Item.FindControl("hdn_startTime");
      DropDownList control3 = (DropDownList) e.Item.FindControl("ddl_endTime");
      HiddenField control4 = (HiddenField) e.Item.FindControl("hdn_endTime");
      DataTable dataTable = this.bind_ddTimeList();
      if (control1 != null)
      {
        control1.DataSource = (object) dataTable;
        control1.DataTextField = "ddText";
        control1.DataValueField = "ddValue";
        control1.DataBind();
        control1.SelectedValue = control2.Value.TrimStart('0');
      }
      if (control3 == null)
        return;
      control3.DataSource = (object) dataTable;
      control3.DataTextField = "ddText";
      control3.DataValueField = "ddValue";
      control3.DataBind();
      control3.SelectedValue = control4.Value.TrimStart('0');
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  public bool check_bookings(long booking_id, Dictionary<string, string> new_selectedDates)
  {
    bool flag = true;
    try
    {
      foreach (string key in new_selectedDates.Keys)
      {
        this.startdatetime = Convert.ToDateTime(key);
        this.enddatetime = Convert.ToDateTime(new_selectedDates[key]);
        DataSet dataSet = this.bookings.check_bookings(Convert.ToInt64(this.hdnAsset.Value), this.startdatetime.AddMinutes(2.0).ToString(api_constants.sql_datetime_format), this.enddatetime.AddMinutes(-2.0).ToString(api_constants.sql_datetime_format), this.current_user.account_id);
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
        {
          if (dataSet.Tables[0].Select("booking_id <> " + (object) booking_id).Length > 0)
            flag = false;
          else if (flag)
            flag = true;
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
    return flag;
  }

  protected void ddlStartTime_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      DropDownList dropDownList = (DropDownList) sender;
      RepeaterItem namingContainer = (RepeaterItem) dropDownList.NamingContainer;
      string[] strArray = dropDownList.SelectedItem.Text.Split(':');
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
      string str = int16.ToString() + ":" + strArray[1];
      ((ListControl) namingContainer.FindControl("ddl_endTime")).SelectedValue = str;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void txt_startdate_changed(object sender, EventArgs e)
  {
    try
    {
      TextBox textBox = (TextBox) sender;
      ((TextBox) textBox.NamingContainer.FindControl("txt_endDate")).Text = textBox.Text;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
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
      str = str.Replace("[REQUESTED BY]", this.current_user.full_name);
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
      stringBuilder.Append("<td>" + (object) obj.resource_id + "</td>");
      stringBuilder.Append("<td>" + (object) obj.resource_booking_item_id + "</td>");
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
      str = str.Replace("[FULL NAME]", this.txtBookedFor.Text);
      str = str.Replace("[PURPOSE]", obj.purpose);
      str = str.Replace("[BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
      str = str.Replace("[BOOKED STATUS]", this.bookings.get_status((long) obj.status));
      str = str.Replace("[REQUESTED BY]", this.current_user.full_name);
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
