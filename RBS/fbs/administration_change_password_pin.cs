// Decompiled with JetBrains decompiler
// Type: administration_change_password_pin
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_change_password_pin : fbs_base_page, IRequiresSessionState
{
  private user obj;
  private users_api uapi = new users_api();
  protected HtmlGenericControl lblUserName;
  protected CheckBox check_password;
  protected TextBox txt_Password;
  protected TextBox txt_rePassword;
  protected HtmlGenericControl div_password;
  protected CheckBox chk_email;
  protected Button Button1;
  protected Button Button2;
  protected HtmlGenericControl div_pwd;
  protected CheckBox check_pin;
  protected Literal litError;
  protected HtmlGenericControl alt_err;
  protected TextBox txt_pin;
  protected HtmlGenericControl div_pin;
  protected Button btn_pin;
  protected Button btn_cancel;
  protected HtmlGenericControl div_show_pin;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      this.obj = this.users.get_user(Convert.ToInt64(this.Request.QueryString["user_id"]), this.current_user.account_id);
      this.lblUserName.InnerText = this.obj.username;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void btn_pin_Click(object sender, EventArgs e)
  {
    try
    {
      if (!this.users.is_duplicate_pin(this.txt_pin.Text, this.current_user.account_id))
      {
        string str = !this.check_pin.Checked ? this.txt_pin.Text : this.users.generate_check_pin(this.current_user.account_id);
        user_property property = this.users.get_user(Convert.ToInt64(this.Request.QueryString["user_id"]), this.current_account.account_id).properties["pin"];
        property.property_value = str;
        property.modified_by = this.current_user.user_id;
        if (this.users.update_user_property(property).user_property_id <= 0L)
          return;
        this.txt_pin.Text = "";
        this.alt_err.Visible = false;
        this.litError.Text = "";
        this.Response.Redirect("users_list.aspx", true);
      }
      else
      {
        this.litError.Text = "PIN is already assigned to someone. Please enter a unique PIN.";
        this.alt_err.Visible = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void btn_password_Click(object sender, EventArgs e)
  {
    try
    {
      if (!(this.Request.QueryString["user_id"].ToString() != ""))
        return;
      string str = !this.check_password.Checked ? this.txt_Password.Text.Trim() : this.utilities.generate_password();
      this.obj = this.users.get_user(Convert.ToInt64(this.Request.QueryString["user_id"]), this.current_user.account_id);
      this.obj.account_id = this.current_user.account_id;
      this.obj.created_by = this.current_user.user_id;
      this.obj.created_on = this.current_timestamp;
      this.obj.modified_by = this.current_user.user_id;
      this.obj.modified_on = this.current_timestamp;
      this.obj.password = this.users.get_md5(str);
      this.users.update_password(this.obj);
      if (this.chk_email.Checked)
      {
        template template1 = new template();
        template template2 = this.tapi.get_template("email_reset_password", this.current_user.account_id);
        account account = this.users.get_account(this.current_user.account_id);
        string newValue = this.site_full_path + "assets/img/" + this.current_account.logo;
        template2.content_data = template2.content_data.Replace("[password]", str);
        template2.content_data = template2.content_data.Replace("[full_name]", this.obj.full_name);
        template2.content_data = template2.content_data.Replace("[logo]", newValue);
        template2.content_data = template2.content_data.Replace("[company_name]", account.name);
        template2.content_data = template2.content_data.Replace("[creator]", this.current_user.full_name);
        template2.content_data = template2.content_data.Replace("[login_link]", this.site_full_path + "login.aspx");
        template2.content_data = template2.content_data.Replace("[username]", this.obj.username);
        email email = new email();
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          this.utilities.sendEmail("", template2.content_data, template2.title, "", this.obj.email, this.obj.record_id);
      }
      this.Response.Redirect("users_list.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
