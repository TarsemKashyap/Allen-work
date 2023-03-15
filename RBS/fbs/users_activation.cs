// Decompiled with JetBrains decompiler
// Type: users_activation
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class users_activation : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public DataSet data;
  public string Save = "";
  public string blkSave = "";
  public string blkdlt = "";
  protected HiddenField hdn_userlistsearch;
  protected HiddenField hdn_totalrecord;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!this.gp.users_view)
      this.redirect_unauthorized();
    if (this.IsPostBack)
      return;
    this.populate_ui();
    if (!(this.Request.QueryString["user_id"] != ""))
      return;
    try
    {
      this.activate_user(Convert.ToInt64(this.Request.QueryString["user_id"]));
    }
    catch
    {
    }
  }

  private void activate_user(long uid)
  {
    if (!this.users.activate_user(uid, this.current_user.account_id))
      return;
    user user = this.users.get_user(uid, this.current_user.account_id);
    try
    {
      template template = this.tapi.get_template("email_user_activation", this.current_user.account_id);
      string body = this.replaceTemplate_user(template.content_data, user);
      string title = template.title;
      string cc = "";
      string bcc = "";
      string email = user.email;
      if (!Convert.ToBoolean(this.current_account.properties["send_email"]))
        return;
      this.sendEmail(bcc, body, title, cc, email, user.record_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Send activation email to user email_address Error ->", ex);
    }
  }

  private string replaceTemplate_user(string content, user objU) => content;

  private email sendEmail(
    string bcc,
    string body,
    string subject,
    string cc,
    string to,
    Guid recID)
  {
    email email = new email();
    try
    {
      setting setting = this.settings.get_setting("from_email_address", this.current_user.account_id);
      email.account_id = this.current_user.account_id;
      email.created_on = this.current_timestamp;
      email.modified_on = this.current_timestamp;
      email.bcc_msg = bcc;
      email.body = string.IsNullOrEmpty(body) ? "" : body;
      email.bounced = false;
      email.cc_msg = cc;
      email.created_by = 0L;
      email.email_message_id = Guid.NewGuid();
      email.from_msg = setting.value;
      email.is_html = true;
      email.message = "";
      email.message_id = 0L;
      email.message_type = 0;
      email.modified_by = 0L;
      email.record_id = recID;
      email.sent = false;
      email.subject = subject;
      email.to_msg = to;
      email = new email_api("smpt").update_email(email);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Ërror--> " + ex.ToString()));
    }
    return email;
  }

  private void populate_ui()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='user_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480' style='WIDTH:75px'>Institution</th>");
      stringBuilder.Append("<th class='hidden-480'>Division</th>");
      stringBuilder.Append("<th class='hidden-480'>Section</th>");
      stringBuilder.Append("<th class='hidden-480'>Full Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Email Address</th>");
      stringBuilder.Append("<th class='hidden-480'>Designation</th>");
      stringBuilder.Append("<th class='hidden-480' style='WIDTH:125px'>Off Phone</th>");
      stringBuilder.Append("<th class='hidden-480'>Created By</th>");
      stringBuilder.Append("<th class='hidden-480' style='WIDTH:50px'>Status</th>");
      stringBuilder.Append("<th class='hidden-480' style='WIDTH:50px'>Locked</th>");
      stringBuilder.Append("<th class='hidden-480' style='WIDTH:50px'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      DataSet usersWithProperties = this.reportings.get_users_With_properties(this.current_user.account_id);
      stringBuilder.Append("<tbody>");
      if (this.utilities.isValidDataset(usersWithProperties))
      {
        foreach (DataRow row in (InternalDataCollectionBase) usersWithProperties.Tables[0].Rows)
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["staff_inst"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["staff_division"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["staff_section"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["full_name"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append("<a href='mailto:" + row["email"].ToString() + "'>" + row["email"].ToString() + "</a>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["staff_desg"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["staff_offphone"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append(row["full_name"].ToString());
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          if (Convert.ToBoolean(row["status"]))
            stringBuilder.Append(" <span class='label label-Active'>Active</span> ");
          else
            stringBuilder.Append("<span class='label label-DeActive'> DeActive</span>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          if (Convert.ToBoolean(row["locked"]))
            stringBuilder.Append(" Yes ");
          else
            stringBuilder.Append(" No ");
          stringBuilder.Append("</td>");
          stringBuilder.Append("<td>");
          stringBuilder.Append("<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'>");
          stringBuilder.Append("<li><a href='javascript:view(" + row["user_id"].ToString() + ")'><i class='icon-table'></i>View Details</a></li>");
          stringBuilder.Append("<li><a href='users_activation.aspx?user_id=" + row["user_id"].ToString() + "'><i class='icon-pencil'></i> Activate</a></li></ul></div></div>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
