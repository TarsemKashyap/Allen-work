// Decompiled with JetBrains decompiler
// Type: administration_email_view
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_email_view : fbs_base_page, IRequiresSessionState
{
  protected Button btn_cancel;
  protected HiddenField hdn_facility;
  protected HtmlForm form1;
  public string email_from;
  public string email_to;
  public string email_subject;
  public string email_body;
  public string email_cmsg;
  public string email_bcc_msg;
  public string email_message;
  public string email_sent;
  public string imagepathfbs;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.imagepathfbs = this.site_full_path + "assets/img/tts_logo.png";
    if (!Convert.ToBoolean(this.current_account.properties["email_log"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (this.Request.QueryString["Facility"] != null)
        this.hdn_facility.Value = this.Request.QueryString["Facility"];
      if (this.Request.QueryString["Type"] == "V")
        this.pageload_Data();
      else
        this.Email_log();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void pageload_Data()
  {
    try
    {
      email email = this.eapi.get_email(Convert.ToInt64(this.Request.QueryString["message_id"]), this.current_user.account_id);
      this.email_from = email.from_msg;
      this.email_to = email.to_msg;
      this.email_subject = email.subject;
      this.email_body = email.body.Replace("cid:headerImageId", this.imagepathfbs);
      this.email_cmsg = email.cc_msg;
      this.email_bcc_msg = email.bcc_msg;
      this.email_message = email.message;
      if (!email.sent)
        this.email_sent = "Not yet send";
      else
        this.email_sent = "Sucessfully sent";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Email_log()
  {
    email email1 = new email();
    asset asset = new asset();
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      email email2 = this.eapi.get_email(Convert.ToInt64(this.Request.QueryString["message_id"]), this.current_user.account_id);
      email2.message_id = 0L;
      email1.created_on = this.current_timestamp;
      email1.modified_on = this.current_timestamp;
      email1.is_html = true;
      email1.bounced = false;
      email1.sent = false;
      email1.account_id = this.current_user.account_id;
      email1.created_by = this.current_user.user_id;
      email1.modified_by = this.current_user.user_id;
      email1.record_id = asset.record_id;
      email1.email_message_id = asset.record_id;
      email1.message_id = 0L;
      email1.message = "";
      email2.created_on = this.current_timestamp;
      email2.sent = false;
      this.eapi.update_email(email2);
      if (this.Request.QueryString["Form"] != "Asset")
      {
        this.Session["Mailsend"] = (object) "S";
        this.Response.Redirect("~/administration/email_loglist.aspx");
      }
      else
      {
        this.Session["Mailsend"] = (object) "resend";
        if (!string.IsNullOrEmpty(this.hdn_facility.Value))
          this.Response.Redirect("~/administration/asset_email_logs.aspx?asset_id=" + this.Request.QueryString["asset_id"]);
        else
          this.Response.Redirect("~/administration/asset_form.aspx?asset_id=" + this.Request.QueryString["asset_id"] + "#tab_email_log");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this);
}
