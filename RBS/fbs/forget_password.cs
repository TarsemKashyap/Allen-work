// Decompiled with JetBrains decompiler
// Type: forget_password
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Configuration;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class forget_password : Page, IRequiresSessionState
{
  private string email = "";
  private string login_type = "";
  private string username = "";
  private Guid accountid;
  private Guid applicationid;
  private string password = "";
  private string fullname = "";
  private long user_id;
  private bool status;
  private util utilities = new util();
  private templates_api tapi = new templates_api();
  private users_api uapi = new users_api();
  private user obj = new user();
  protected HtmlLink style_color;
  protected TextBox txt_email;
  protected Button btnSubmit;
  protected Label lblError;
  protected Label msglbl;
  protected Button btOK;
  protected HtmlGenericControl divmsg;
  protected HtmlForm fogetpassword;

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    try
    {
      user user = this.uapi.get_user(this.txt_email.Text);
      if (user.user_id == 0L)
      {
        this.lblError.Text = "AD user cannot change password.Please contact Administrator";
        this.lblError.Visible = true;
      }
      else if (user.login_type == 1L)
      {
        this.lblError.Text = "AD user cannot change password.Please contact Administrator";
        this.lblError.Visible = true;
      }
      else
      {
        account account = this.uapi.get_account(user.account_id);
        this.password = this.utilities.generate_password();
        user.password = this.password;
        user.password = this.uapi.get_md5(user.password);
        this.Session.Add("user", (object) user);
        this.status = this.uapi.update_password(user);
        if (!this.status || !Convert.ToBoolean(account.properties["send_email"]))
          return;
        this.email_setup(user, account, this.password);
        this.divmsg.Visible = true;
        this.msglbl.Text = "Email has been sent to the account!!";
      }
    }
    catch (Exception ex)
    {
      this.lblError.Text = ex.Message.ToString();
      this.lblError.Visible = true;
    }
  }

  private void email_setup(user obj, account acc, string password)
  {
    string newValue = ConfigurationManager.AppSettings["site_full_path"] + "assets/img/" + acc.logo;
    template template1 = new template();
    template template2 = this.tapi.get_template("email_reset_password", acc.account_id);
    template2.content_data = template2.content_data.Replace("[password]", password);
    template2.content_data = template2.content_data.Replace("[full_name]", obj.full_name);
    template2.content_data = template2.content_data.Replace("[logo]", newValue);
    template2.content_data = template2.content_data.Replace("[company_name]", acc.name);
    template2.content_data = template2.content_data.Replace("[creator]", "System");
    template2.content_data = template2.content_data.Replace("[login_link]", "~/login.aspx");
    template2.content_data = template2.content_data.Replace("[username]", obj.username);
    this.utilities.sendEmail("", template2.content_data, template2.title, "", obj.email, obj.record_id);
  }

  protected void btOK_Click(object sender, EventArgs e)
  {
    this.Response.Redirect("~/login.aspx", false);
    this.divmsg.Visible = false;
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
