// Decompiled with JetBrains decompiler
// Type: mytask
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

public class mytask : fbs_base_page, IRequiresSessionState
{
  protected Label lblMessage;
  protected Label lblCountOfInbox;
  protected HtmlGenericControl divInbox;
  protected HtmlGenericControl divHistory;
  protected Button btnExportExcel;
  protected HtmlGenericControl lblfrom;
  protected TextBox txtFromDate;
  protected HtmlGenericControl lblto;
  protected TextBox txtToDate;
  protected Button btn_filter;
  protected HtmlGenericControl div_date_filter;
  protected Button btn_bulk_approve;
  protected HiddenField hdn_id;
  protected HiddenField hdnfromdate;
  protected HiddenField hdntodate;
  protected Literal lit_open;
  public string html_table;
  public string html_table2;
  public static string actionname;
  public string gp_ids = "";
  public string user_id = "";
  public string ac = "";
  public string msg;
  public string notpostback = "";
  private List<long> workflow_ids;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.gp_ids = this.utilities.get_group_ids(this.current_user);
    this.user_id = this.current_user.user_id.ToString();
    this.ac = this.current_user.account_id.ToString();
    if (!this.gp.isAdminType && !this.gp.isSuperUserType)
      this.btn_bulk_approve.Visible = false;
    if (this.IsPostBack)
      return;
    this.notpostback = "y";
    this.txtFromDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
    this.txtToDate.Text = this.current_timestamp.AddDays(1.0).ToString(api_constants.display_datetime_format_short);
    this.hdnfromdate.Value = this.txtFromDate.Text;
    this.hdntodate.Value = this.txtToDate.Text;
    try
    {
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["ap"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["workflow_id"]))
        this.update_approved(Convert.ToInt64(this.Request.QueryString["workflow_id"]));
      else if (!string.IsNullOrWhiteSpace(this.Request.QueryString["wd"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["workflow_id"]))
        this.update_withdraw(Convert.ToInt64(this.Request.QueryString["workflow_id"]));
      else if (!string.IsNullOrWhiteSpace(this.Request.QueryString["action"]))
      {
        switch (this.Request.QueryString["action"])
        {
          case "inbox":
            this.div_date_filter.Visible = false;
            mytask.actionname = "inbox";
            break;
          case "history":
            this.div_date_filter.Visible = true;
            this.btn_bulk_approve.Visible = false;
            mytask.actionname = "history";
            break;
          case "approve":
            mytask.actionname = "approve";
            break;
          case "reject":
            mytask.actionname = "reject";
            break;
        }
        this.populate_ui(mytask.actionname);
      }
      else
      {
        mytask.actionname = "inbox";
        this.populate_ui("inbox");
      }
      this.show_task_message();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void show_task_message()
  {
    if (this.Session["workflow_approved"] != null && !string.IsNullOrWhiteSpace(this.Session["workflow_approved"].ToString()))
      this.msg = !(this.Session["workflow_approved"].ToString() == "1") ? Resources.fbs.task_approved_unsucessfully : Resources.fbs.task_approved_sucessfully;
    if (this.Session["workflow_rejected"] != null)
    {
      if (!string.IsNullOrWhiteSpace(this.Session["workflow_rejected"].ToString()))
        this.msg = !(this.Session["workflow_rejected"].ToString() == "1") ? Resources.fbs.task_rejected_unsuccessfully : Resources.fbs.task_rejected_successfull;
      this.Session.Remove("workflow_rejected");
    }
    if (this.Session["workflow_withdraw"] == null || string.IsNullOrWhiteSpace(this.Session["workflow_withdraw"].ToString()))
      return;
    if (this.Session["workflow_withdraw"].ToString() == "1")
      this.msg = Resources.fbs.task_withdraw_successfully;
    else
      this.msg = Resources.fbs.task_withdraw_unsuccessfully;
  }

  private void populate_ui(string action)
  {
    try
    {
      long reference_id;
      try
      {
        reference_id = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        reference_id = 0L;
      }
      if ((action == "approve" || action == "reject") && reference_id > 0L)
      {
        DataSet workflowData = this.workflows.get_workflow_data(reference_id, this.current_user.account_id);
        if (workflowData.Tables[0].Rows.Count > 0)
        {
          if (action == "approve")
            this.update_approved(Convert.ToInt64(workflowData.Tables[0].Rows[0]["workflow_id"].ToString()));
          this.lit_open.Text = "<script type='text/javascript'>callfancybox(" + workflowData.Tables[0].Rows[0]["workflow_id"].ToString() + ");</script>";
        }
      }
      string todate;
      string fromdate;
      if (this.notpostback == "y")
      {
        todate = "";
        fromdate = "";
      }
      else
      {
        fromdate = this.hdnfromdate.Value;
        todate = this.hdntodate.Value;
      }
      try
      {
        if (action == "inbox")
        {
          fromdate = "";
          todate = "";
        }
        else
        {
          this.txtFromDate.Text = !(fromdate == "") ? fromdate : this.current_timestamp.ToString(api_constants.display_datetime_format_short);
          this.txtToDate.Text = !(todate == "") ? todate : this.current_timestamp.ToString(api_constants.display_datetime_format_short);
          fromdate = this.txtFromDate.Text + " 00:00:00 AM";
          todate = this.txtToDate.Text + " 23:59:59 PM";
        }
      }
      catch
      {
      }
      string groupIds = this.utilities.get_group_ids(this.current_user);
      StringBuilder stringBuilder1 = new StringBuilder();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      DataSet dataSet = new DataSet();
      int num4;
      if (action == "inbox" || action == "approve" || action == "reject")
      {
        this.workflow_ids = new List<long>();
        StringBuilder stringBuilder2 = new StringBuilder();
        stringBuilder2.Append(this.headerrowforinboxandhistory("inbox"));
        stringBuilder2.Append("<tbody>");
        if (this.gp.isAdminType)
        {
          DataSet mytaskRecordsAdmin = this.workflows.get_mytask_records_admin(this.current_user.account_id, fromdate, todate);
          if (this.utilities.isValidDataset(mytaskRecordsAdmin))
          {
            DataRow[] bk_rows = mytaskRecordsAdmin.Tables[0].Select("status = 4");
            if (bk_rows.Length > 0)
            {
              num4 = num1 + bk_rows.Length;
              stringBuilder2.Append(this.getHtml("admin", "inbox", bk_rows, "approval"));
            }
          }
        }
        if (this.gp.isSuperUserType)
        {
          DataSet recordsSuperuser = this.workflows.get_mytask_records_superuser(this.current_user.account_id, fromdate, todate, groupIds);
          if (this.utilities.isValidDataset(recordsSuperuser))
          {
            DataRow[] bk_rows = recordsSuperuser.Tables[0].Select();
            if (bk_rows.Length > 0)
            {
              num2 += bk_rows.Length;
              stringBuilder2.Append(this.getHtml("superuser", "inbox", bk_rows, "approval"));
            }
          }
        }
        DataSet recordsRequestor = this.workflows.get_mytask_records_requestor(this.current_user.account_id, fromdate, todate, this.current_user.user_id);
        if (this.utilities.isValidDataset(recordsRequestor))
        {
          DataRow[] bk_rows = recordsRequestor.Tables[0].Select();
          if (bk_rows.Length > 0)
          {
            num3 += bk_rows.Length;
            stringBuilder2.Append(this.getHtml("requestor", "inbox", bk_rows, "pending"));
          }
        }
        num1 = num2 + num3;
        if (num1 > 0)
        {
          if (this.gp.isAdminType || this.gp.isSuperUserType)
            this.btn_bulk_approve.Visible = true;
          else
            this.btn_bulk_approve.Visible = false;
          this.lblCountOfInbox.Text = num1.ToString();
          this.lblCountOfInbox.Font.Bold = false;
          this.lblCountOfInbox.Attributes.Add("Class", "badge badge-important");
        }
        else
        {
          this.bind_empty(action);
          this.btn_bulk_approve.Visible = false;
          this.lblCountOfInbox.Text = "";
          this.lblCountOfInbox.Attributes.Remove("class");
        }
        this.Session["inbx"] = (object) num1.ToString();
        stringBuilder2.Append("</tbody>");
        stringBuilder2.Append("</table>");
        this.html_table = stringBuilder2.ToString();
        stringBuilder2.Clear();
      }
      if (!(action == "history"))
        return;
      StringBuilder stringBuilder3 = new StringBuilder();
      stringBuilder3.Append(this.headerrowforinboxandhistory("history"));
      stringBuilder3.Append("<tbody>");
      DataRow[] bk_rows1 = this.workflows.get_mytask_records_history(this.current_user.account_id, "", "", this.current_user.user_id, groupIds).Tables[0].Select();
      if (bk_rows1.Length > 0)
        stringBuilder3.Append(this.getHtml("requestor_superuser", "history", bk_rows1, ""));
      if (this.gp.isAdminType)
      {
        DataRow[] bk_rows2 = (dataSet = this.workflows.get_mytask_records_history_admin(this.current_user.account_id, "", "")).Tables[0].Select();
        if (bk_rows2.Length > 0)
          stringBuilder3.Append(this.getHtml("admin", "history", bk_rows2, ""));
      }
      stringBuilder3.Append("</tbody>");
      stringBuilder3.Append("</table>");
      this.html_table = stringBuilder3.ToString();
      stringBuilder3.Clear();
      this.btn_bulk_approve.Visible = false;
      if (this.gp.isAdminType)
      {
        DataSet mytaskRecordsAdmin = this.workflows.get_mytask_records_admin(this.current_user.account_id, "", "");
        if (this.utilities.isValidDataset(mytaskRecordsAdmin))
        {
          DataRow[] dataRowArray = mytaskRecordsAdmin.Tables[0].Select("status = 4");
          if (dataRowArray.Length > 0)
            num4 = num1 + dataRowArray.Length;
        }
      }
      if (this.gp.isSuperUserType)
      {
        DataSet recordsSuperuser = this.workflows.get_mytask_records_superuser(this.current_user.account_id, "", "", groupIds);
        if (this.utilities.isValidDataset(recordsSuperuser))
        {
          DataRow[] dataRowArray = recordsSuperuser.Tables[0].Select();
          if (dataRowArray.Length > 0)
            num2 += dataRowArray.Length;
        }
      }
      DataSet recordsRequestor1 = this.workflows.get_mytask_records_requestor(this.current_user.account_id, "", "", this.current_user.user_id);
      if (this.utilities.isValidDataset(recordsRequestor1))
      {
        DataRow[] dataRowArray = recordsRequestor1.Tables[0].Select();
        if (dataRowArray.Length > 0)
          num3 += dataRowArray.Length;
      }
      int num5 = num2 + num3;
      if (num5 > 0)
      {
        this.lblCountOfInbox.Text = num5.ToString();
        this.lblCountOfInbox.Font.Bold = false;
        this.lblCountOfInbox.Attributes.Add("Class", "badge badge-important");
      }
      else
      {
        this.btn_bulk_approve.Visible = false;
        this.lblCountOfInbox.Text = "";
        this.lblCountOfInbox.Attributes.Remove("class");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void bind_empty(string action)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    if (action == "history")
    {
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.Append(this.headerrowforinboxandhistory("history"));
      stringBuilder2.Append("<tbody>");
      stringBuilder2.Append("</tbody>");
      stringBuilder2.Append("</table>");
      this.html_table = stringBuilder2.ToString();
      stringBuilder2.Clear();
    }
    else
    {
      StringBuilder stringBuilder3 = new StringBuilder();
      stringBuilder3.Append(this.headerrowforinboxandhistory("inbox"));
      stringBuilder3.Append("<tbody>");
      stringBuilder3.Append("</tbody>");
      stringBuilder3.Append("</table>");
      this.html_table = stringBuilder3.ToString();
      stringBuilder3.Clear();
    }
  }

  private string getHtml(string userType, string taskType, DataRow[] bk_rows, string type)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (DataRow bkRow in bk_rows)
    {
      if (!this.workflow_ids.Contains(Convert.ToInt64(bkRow["workflow_id"])))
      {
        this.workflow_ids.Add(Convert.ToInt64(bkRow["workflow_id"]));
        double num;
        try
        {
          num = Convert.ToDouble(bkRow["approvalperiod"]);
        }
        catch
        {
          num = 0.0;
        }
        stringBuilder.Append("<tr class='odd gradeX'>");
        if (taskType == "inbox")
        {
          if (type == "approval")
            stringBuilder.Append("<td><input type='checkbox' class='checkboxes' name='InboxWorkflowIds' value='" + bkRow["workflow_id"].ToString() + "' /></td>");
          else
            stringBuilder.Append("<td>&nbsp;</td>");
        }
        string str1 = !(type == "approval") ? (!(this.workflows.get_action_type(Convert.ToInt16(bkRow["action_type"].ToString())) == "New_Booking") ? "" : "<span class='label label-info'>Pending Approval</span>") : (!(this.workflows.get_action_type(Convert.ToInt16(bkRow["action_type"].ToString())) == "New_Booking") ? "" : "<span class='label label-warning'>Approval Required</span>");
        if (taskType == "history")
          str1 = "";
        DateTime dateTime = Convert.ToDateTime(bkRow["created_on"]).AddHours(num);
        if (dateTime > Convert.ToDateTime(bkRow["book_from"]))
          dateTime = Convert.ToDateTime(bkRow["book_from"]);
        string str2 = (this.current_timestamp - dateTime).TotalMinutes >= 60.0 ? (!(dateTime < this.current_timestamp) ? "<span class='label label-info'>Pending</span>" : "<span class='label label-warning'>Overdue</span>") : "<span class='label label-important'>Urgent</span>";
        stringBuilder.Append("<td>" + str1 + "</td>");
        stringBuilder.AppendFormat("<td>" + bkRow["purpose"].ToString() + "</td>");
        stringBuilder.Append("<td>" + bkRow["name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(bkRow["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(bkRow["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>" + dateTime.ToString(api_constants.display_datetime_format) + "</td>");
        if (taskType == "inbox")
        {
          stringBuilder.Append("<td>" + str2 + "</td>");
        }
        else
        {
          if (bkRow["action_status"].ToString() == "2")
            stringBuilder.Append("<td><span class='label label-important'>Rejected</span></td>");
          if (bkRow["action_status"].ToString() == "1")
            stringBuilder.Append("<td><span class='label label-success'>Approved</span></td>");
          if (bkRow["action_status"].ToString() == "0")
            stringBuilder.Append("<td><span class='label label-danger'>No Action</span></td>");
        }
        stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
        stringBuilder.Append("<ul class='ddm p-r'>");
        stringBuilder.AppendFormat("<li><a href='javascript:callviewbookfancybox({0})'><i class='icon-pencil'></i> View Booking Details</a></li>", (object) bkRow["reference_id"].ToString());
        if (taskType == "inbox")
          stringBuilder.AppendFormat("<li><a href='javascript:callfancybox({0})'><i class='icon-pencil'></i> View Task Details</a></li>", (object) bkRow["workflow_id"].ToString());
        if ((userType == "admin" || userType == "superuser") && taskType == "inbox")
        {
          stringBuilder.AppendFormat("<li><a href='javascript:approve({0})'><i class='icon-pencil'></i> Approve</a></li>", (object) bkRow["workflow_id"].ToString());
          stringBuilder.AppendFormat("<li><a href='javascript:callfancybox_for_reject({0})'><i class='icon-pencil'></i> Reject</a></li>", (object) bkRow["workflow_id"].ToString());
        }
        if (userType == "requestor" && taskType == "inbox")
          stringBuilder.AppendFormat("<li><a href='javascript:withdraw({0})'><i class='icon-pencil'></i> Withdraw</a></li>", (object) bkRow["workflow_id"].ToString());
        stringBuilder.Append("</ul>");
        stringBuilder.Append("</div></div></td>");
        stringBuilder.Append("</tr>");
      }
    }
    return stringBuilder.ToString();
  }

  private void update_approved(long workflow_id)
  {
    try
    {
      this.Session["workflow_approved"] = (object) "0";
      workflow workflow1 = this.workflows.get_workflow(workflow_id, this.current_user.account_id);
      workflow1.action_status = this.workflows.get_action_status("Approved");
      workflow1.action_taken_by = this.current_user.user_id;
      workflow1.modified_by = this.current_user.user_id;
      workflow workflow2 = this.workflows.update_workflow(workflow1);
      if (workflow2.workflow_id > 0L)
      {
        if (workflow2.action_type == (short) 2)
        {
          DataSet bookingById1 = this.bookings.get_booking_by_id(workflow2.reference_id, this.current_user.account_id);
          if (this.utilities.isValidDataset(bookingById1))
          {
            DataSet bookingById2 = this.bookings.get_booking_by_id(Convert.ToInt64(bookingById1.Tables[0].Rows[0]["transfer_original_booking_id"].ToString()), this.current_user.account_id);
            if (this.utilities.isValidDataset(bookingById2))
            {
              this.bookings.get_user_group(Convert.ToInt64(bookingById2.Tables[0].Rows[0]["booked_for"].ToString()), this.current_user.account_id);
              workflow2.workflow_id = 0L;
              workflow2.action_type = this.workflows.get_action_type("Transfer_User");
              workflow2.action_status = this.workflows.get_action_status("Pending");
              workflow2.action_taken_by = 0L;
              workflow2.action_remarks = "";
              workflow2.action_owner_id = Convert.ToInt64(bookingById2.Tables[0].Rows[0]["booked_for"].ToString());
              workflow2.created_by = Convert.ToInt64(bookingById1.Tables[0].Rows[0]["booked_for"].ToString());
              if (this.workflows.update_workflow(workflow2).workflow_id <= 0L)
                ;
            }
          }
        }
        else
        {
          this.update_booking_info(workflow2.reference_id, this.bookings.get_status("Booked"));
          asset_booking booking = this.bookings.get_booking(workflow2.reference_id, workflow2.account_id);
          booking.global_appointment_id = this.bookings.get_appointment_id(booking.booking_id, booking.account_id);
          if (Convert.ToBoolean(this.current_account.properties["send_email"]))
            this.bookingsbl.send_booking_request_workflow_email(booking, true, (workflow) null);
        }
        this.Session["workflow_approved"] = (object) "1";
      }
      this.Session.Remove("inbx");
      try
      {
        if (!string.IsNullOrWhiteSpace(this.Request.QueryString["modal"]))
        {
          if (!(this.Request.QueryString["modal"] == "Y"))
            return;
          Modal.Close((Page) this, (object) "OK");
          this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "Mytask();", true);
        }
        else
          this.Response.Redirect("mytask.aspx?action=inbox", true);
      }
      catch (Exception ex)
      {
        this.Response.Redirect("mytask.aspx?action=inbox", true);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void update_withdraw(long workflow_id)
  {
    try
    {
      workflow workflow1 = this.workflows.get_workflow(workflow_id, this.current_user.account_id);
      workflow1.action_status = this.workflows.get_action_status("Withdraw");
      workflow1.action_taken_by = this.current_user.user_id;
      workflow1.modified_by = this.current_user.user_id;
      workflow workflow2 = this.workflows.update_workflow(workflow1);
      try
      {
        DataSet workflowWithdrawEmail = this.workflows.get_workflow_withdraw_email(workflow2.reference_id, this.current_user.account_id);
        asset_booking booking = this.bookings.get_booking(workflow2.reference_id, this.current_user.account_id);
        string str1 = "";
        if (this.utilities.isValidDataset(workflowWithdrawEmail))
        {
          foreach (DataRow row in (InternalDataCollectionBase) workflowWithdrawEmail.Tables[0].Rows)
            str1 = str1 + row["email"].ToString() + ";";
        }
        if (str1 != "")
        {
          string str2 = str1.Trim().Substring(0, str1.Length - 1);
          template template1 = new template();
          template template2 = this.tapi.get_template("email_withdraw", this.current_user.account_id);
          asset asset = this.assets.get_asset(booking.asset_id, booking.account_id);
          string body = this.replaceTemplate_withdraw(template2.content_data, booking, asset).Replace("[email_title]", template2.title);
          string title = template2.title;
          string cc = "";
          string bcc = "";
          string to = str2;
          email email = new email();
          if (Convert.ToBoolean(this.current_account.properties["send_email"]))
            this.utilities.sendEmail(bcc, body, title, cc, to, booking.record_id);
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Send email to resource owner - Error -> ", ex);
      }
      if (workflow2.workflow_id > 0L)
      {
        this.update_booking_info(workflow2.reference_id, this.bookings.get_status("Withdraw"));
        this.Session["workflow_withdraw"] = (object) "1";
      }
      else
        this.Session["workflow_withdraw"] = (object) "0";
      this.Session.Remove("inbox");
      try
      {
        if (!string.IsNullOrWhiteSpace(this.Request.QueryString["modal"]))
        {
          if (!(this.Request.QueryString["modal"] == "Y"))
            return;
          Modal.Close((Page) this, (object) "OK");
          this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "Mytask();", true);
        }
        else
          this.Response.Redirect("mytask.aspx?action=inbox", true);
      }
      catch (Exception ex)
      {
        this.Response.Redirect("mytask.aspx?action=inbox", true);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private string replaceTemplate_withdraw(string content, asset_booking obj, asset obj_assets)
  {
    string str = content;
    try
    {
      str = str.Replace("[requestor]", this.users.get_user_name(obj.booked_for, this.current_user.account_id));
      str = str.Replace("[purpose]", obj.purpose);
      str = str.Replace("[remarks]", obj.remarks);
      str = str.Replace("[modified_by]", this.users.get_user_name(obj.modified_by, this.current_user.account_id));
      str = str.Replace("[date_range]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
      str = str.Replace("[email]", obj.email);
      str = str.Replace("[company_name]", this.current_account.name);
      str = str.Replace("[room_name]", obj_assets.name);
      str = str.Replace("[building]", obj_assets.building.value);
      str = str.Replace("[level]", obj_assets.level.value);
      str = str.Replace("[logo]", this.site_full_path + "assets/img/" + this.current_account.logo);
      str = str.Replace("[view_more_link]", this.site_full_path + "booking_view.aspx?id=" + (object) obj.booking_id);
      str = str.Replace("[contact]", obj.contact);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return str;
  }

  private void update_booking_info(long booking_id, int booking_status)
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
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_bulk_approve_Click(object sender, EventArgs e)
  {
    try
    {
      string[] strArray = this.Request.Form["InboxWorkflowIds"].Split(',');
      if (strArray.Length > 0)
      {
        foreach (string str in strArray)
        {
          workflow workflow1 = this.workflows.get_workflow(Convert.ToInt64(str), this.current_user.account_id);
          workflow1.action_status = this.workflows.get_action_status("Approved");
          workflow1.action_taken_by = this.current_user.user_id;
          workflow1.action_remarks = "";
          workflow workflow2 = this.workflows.update_workflow(workflow1);
          if (workflow2.workflow_id > 0L)
          {
            if (workflow2.action_type == (short) 2)
            {
              DataSet bookingById1 = this.bookings.get_booking_by_id(workflow2.reference_id, this.current_user.account_id);
              long num = 0;
              if (this.utilities.isValidDataset(bookingById1))
              {
                DataSet bookingById2 = this.bookings.get_booking_by_id(Convert.ToInt64(bookingById1.Tables[0].Rows[0]["transfer_original_booking_id"].ToString()), this.current_user.account_id);
                if (this.utilities.isValidDataset(bookingById2))
                {
                  DataSet userGroup = this.bookings.get_user_group(Convert.ToInt64(bookingById2.Tables[0].Rows[0]["booked_for"].ToString()), this.current_user.account_id);
                  if (userGroup != null && userGroup.Tables[0].Rows.Count > 0)
                    num = Convert.ToInt64(userGroup.Tables[0].Rows[0][0].ToString());
                  workflow2.workflow_id = 0L;
                  workflow2.action_type = this.workflows.get_action_type("Transfer_User");
                  workflow2.action_status = this.workflows.get_action_status("Pending");
                  workflow2.action_taken_by = 0L;
                  workflow2.action_remarks = "";
                  workflow2.action_owner_id = num;
                  if (this.workflows.update_workflow(workflow2).workflow_id <= 0L)
                    ;
                }
              }
            }
            else
            {
              this.update_booking_info(workflow2.reference_id, this.bookings.get_status("Booked"));
              asset_booking booking = this.bookings.get_booking(workflow2.reference_id, workflow2.account_id);
              booking.global_appointment_id = this.bookings.get_appointment_id(booking.booking_id, booking.account_id);
              if (Convert.ToBoolean(this.current_account.properties["send_email"]))
                this.bookingsbl.send_booking_request_workflow_email(booking, true, (workflow) null);
            }
          }
        }
      }
      this.Session.Remove("inbx");
      this.Response.Redirect("mytask.aspx?action=inbox", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_filter_Click(object sender, EventArgs e) => this.populate_ui(this.Request.QueryString["action"]);

  protected void txtFromDate_TextChanged(object sender, EventArgs e)
  {
    this.hdnfromdate.Value = this.txtFromDate.Text;
    this.hdntodate.Value = this.txtFromDate.Text;
    this.txtToDate.Text = this.txtFromDate.Text;
    this.populate_ui(this.Request.QueryString["action"]);
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      mytask.actionname = this.Request.QueryString["action"];
    }
    catch
    {
      mytask.actionname = "inbox";
    }
    try
    {
      if (mytask.actionname == "inbox")
        this.Generate_excel(mytask.actionname, "", "");
      else
        this.Generate_excel(mytask.actionname, this.hdnfromdate.Value, this.hdntodate.Value);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void Generate_excel(string action, string from_date, string to_date)
  {
    try
    {
      this.utilities.get_group_ids(this.current_user);
      DataTable dataTable1 = new DataTable();
      DataSet dataSet = new DataSet();
      DataTable dataTable2;
      switch (action)
      {
        case "inbox":
          if (this.gp.isAdminType)
          {
            DataSet mytaskRecordsAdmin = this.workflows.get_mytask_records_admin(this.current_user.account_id, from_date, to_date);
            dataTable2 = new DataTable();
            dataTable1 = mytaskRecordsAdmin.Tables[0].Clone();
            foreach (DataRow row in mytaskRecordsAdmin.Tables[0].Select("status = 4"))
            {
              try
              {
                dataTable1.ImportRow(row);
                dataTable1.AcceptChanges();
              }
              catch
              {
              }
            }
            break;
          }
          if (this.gp.isSuperUserType)
          {
            DataSet recordsSuperuser = this.workflows.get_mytask_records_superuser(this.current_user.account_id, from_date, to_date, this.gp_ids);
            dataTable2 = new DataTable();
            dataTable1 = recordsSuperuser.Tables[0].Clone();
            foreach (DataRow row in recordsSuperuser.Tables[0].Select())
            {
              try
              {
                dataTable1.ImportRow(row);
                dataTable1.AcceptChanges();
              }
              catch
              {
              }
            }
            break;
          }
          DataSet recordsRequestor = this.workflows.get_mytask_records_requestor(this.current_user.account_id, from_date, to_date, this.current_user.user_id);
          dataTable2 = new DataTable();
          dataTable1 = recordsRequestor.Tables[0].Clone();
          foreach (DataRow row in recordsRequestor.Tables[0].Select())
          {
            try
            {
              dataTable1.ImportRow(row);
              dataTable1.AcceptChanges();
            }
            catch
            {
            }
          }
          break;
        case "history":
          if (this.gp.isAdminType)
          {
            DataSet recordsHistoryAdmin = this.workflows.get_mytask_records_history_admin(this.current_user.account_id, from_date, to_date);
            dataTable2 = new DataTable();
            dataTable1 = recordsHistoryAdmin.Tables[0].Clone();
            foreach (DataRow row in recordsHistoryAdmin.Tables[0].Select())
            {
              try
              {
                dataTable1.ImportRow(row);
                dataTable1.AcceptChanges();
              }
              catch
              {
              }
            }
            break;
          }
          DataSet mytaskRecordsHistory = this.workflows.get_mytask_records_history(this.current_user.account_id, from_date, to_date, this.current_user.user_id, this.gp_ids);
          dataTable1 = mytaskRecordsHistory.Tables[0].Clone();
          foreach (DataRow row in mytaskRecordsHistory.Tables[0].Select())
          {
            try
            {
              dataTable1.ImportRow(row);
              dataTable1.AcceptChanges();
            }
            catch
            {
            }
          }
          break;
      }
      try
      {
        if (dataTable1.Rows.Count <= 0)
          return;
        dataTable1.Columns.Add("action_type_string");
        this.excel_common(action, new DataSet()
        {
          Tables = {
            dataTable1
          }
        });
      }
      catch
      {
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  public string headerrowforinboxandhistory(string type)
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table_22'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      if (type == "inbox")
        stringBuilder.Append("<th style='width:4%;'><input type='checkbox' id='selectall' class='group-checkable' data-set='#list_table_22 .checkboxes' /></th>");
      stringBuilder.Append("<th style='width:14%;' class='hidden-480'>Action Type</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Purpose</th>");
      stringBuilder.Append("<th style='width:15%;' class='hidden-480'>Room</th>");
      stringBuilder.Append("<th style='width:16%;' class='hidden-480'>From</th>");
      stringBuilder.Append("<th style='width:16%;' class='hidden-480'>To</th>");
      stringBuilder.Append("<th  style='width:16%;' class='hidden-480'>Due On</th>");
      stringBuilder.Append("<th  style='width:16%;' class='hidden-480'>Status</th>");
      stringBuilder.Append("<th style='width:4%;' class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return stringBuilder.ToString();
  }

  public email get_email_object()
  {
    email emailObject = new email();
    DataSet dataSet = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
    if (dataSet == null)
    {
      dataSet = this.settings.view_settings(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) dataSet);
    }
    DataRow[] dataRowArray = dataSet.Tables[0].Select("parameter='from_email_address'");
    try
    {
      emailObject.message_id = 0L;
      emailObject.account_id = this.current_user.account_id;
      emailObject.created_by = this.current_user.user_id;
      emailObject.created_on = this.current_timestamp;
      emailObject.modified_by = this.current_user.user_id;
      emailObject.modified_on = this.current_timestamp;
      emailObject.email_message_id = Guid.NewGuid();
      emailObject.from_msg = dataRowArray[0]["value"].ToString();
      emailObject.cc_msg = "";
      emailObject.bcc_msg = "";
      emailObject.is_html = true;
      emailObject.sent = false;
      emailObject.bounced = false;
      emailObject.message = "";
      emailObject.sent_on = this.current_timestamp;
      emailObject.message_type = 1;
      emailObject.failed_attempts = 0;
      emailObject.last_attempted_on = this.current_timestamp;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    return emailObject;
  }

  public void excel_common(string action, DataSet dst)
  {
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) dst.Tables[0].Rows)
      {
        row["action_type_string"] = !(row["action_type"].ToString() == "1") ? (!(row["action_type"].ToString() == "2") ? (object) "Transfer_User" : (object) "Transfer_Group") : (object) "New_Booking";
        dst.AcceptChanges();
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("created_on", "Due On");
      dictionary.Add("purpose", "Purpose");
      dictionary.Add("book_from", "From");
      dictionary.Add("book_to", "To");
      dictionary.Add("name", "Room");
      dictionary.Add("action_type_string", "Action Type");
      excel excel = new excel();
      excel.file_name = " " + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "    <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = dst;
      excel.column_names = dictionary;
      excel.table_identifier = "+ current_user.full_name + ";
      if (action == "inbox")
        excel.header = "Inbox";
      if (action == "history")
        excel.header = "History";
      this.Response.Clear();
      if (action == "inbox")
        this.Response.AddHeader("content-disposition", "attachment;filename=Inbox_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      else if (action == "history")
        this.Response.AddHeader("content-disposition", "attachment;filename=History_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.exapi.get_excel(excel));
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }
}
