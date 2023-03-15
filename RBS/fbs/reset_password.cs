﻿// Decompiled with JetBrains decompiler
// Type: reset_password
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

public class reset_password : fbs_base_page, IRequiresSessionState
{
  protected Label label1;
  protected TextBox txt_currentPassword;
  protected TextBox txt_Password;
  protected TextBox txt_rePassword;
  protected Button btn_Login;
  protected Label lblError;
  protected HtmlForm Form1;
  private string password = "";
  private bool status;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e) => this.ViewState.Add("ret_url", (object) this.Request.QueryString["ret_url"]);

  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    string password = this.txt_currentPassword.Text.ToString();
    try
    {
      if (this.current_user.password == this.users.get_md5(password))
      {
        this.current_user.password = this.txt_Password.Text.ToString();
        this.current_user.password = this.users.get_md5(this.current_user.password);
        this.status = this.users.update_password(this.current_user);
        if (!this.status)
          return;
        user_property userProperty1 = new user_property();
        user_property userProperty2;
        if (this.current_user.properties.ContainsKey("last_password_change"))
        {
          userProperty2 = this.current_user.properties["last_password_change"];
          userProperty2.modified_on = DateTime.UtcNow;
          userProperty2.property_value = DateTime.UtcNow.ToString(api_constants.sql_datetime_format);
        }
        else
        {
          userProperty2 = new user_property();
          userProperty2.account_id = this.current_user.account_id;
          userProperty2.created_by = this.current_user.user_id;
          userProperty2.created_on = DateTime.UtcNow;
          userProperty2.modified_by = this.current_user.user_id;
          userProperty2.modified_on = DateTime.UtcNow;
          userProperty2.property_name = "last_password_change";
          userProperty2.property_value = DateTime.UtcNow.ToString(api_constants.sql_datetime_format);
          userProperty2.record_id = Guid.NewGuid();
          userProperty2.user_id = this.current_user.user_id;
          userProperty2.user_property_id = 0L;
        }
        userProperty1 = this.users.update_user_property(userProperty2);
        this.Response.Redirect((string) this.ViewState["ret_url"]);
      }
      else
      {
        this.lblError.Text = "Password is wrong!!";
        this.lblError.Visible = true;
      }
    }
    catch (Exception ex)
    {
      this.lblError.Text = ex.Message.ToString();
      this.lblError.Visible = true;
    }
  }
}
