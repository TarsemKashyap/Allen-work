// Decompiled with JetBrains decompiler
// Type: logout
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

public class logout : Page, IRequiresSessionState
{
  protected Label error;
  protected HtmlGenericControl alert_logout;
  protected HtmlGenericControl divWelcome;
  protected HtmlForm Form1;
  private string site_full_path = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    this.site_full_path = ConfigurationManager.AppSettings["site_full_path"];
    if (this.IsPostBack)
      return;
    try
    {
      this.Session.Clear();
      this.Session.Remove("user");
      this.Session.RemoveAll();
      this.Session.Abandon();
      if (this.Request.QueryString["Diff"] != null)
      {
        this.Response.Redirect(this.site_full_path + "login.aspx");
      }
      else
      {
        if (this.Request.QueryString["ad"] != null || this.Request.QueryString["UN"] == null)
          return;
        if (this.Request.QueryString["UN"] == "N")
          this.Response.Redirect(this.site_full_path + "error.aspx?message=" + this.Server.UrlEncode("An error occured. Please contact the administrator."), false);
        else if (this.Request.QueryString["UN"] == "LDAP")
        {
          this.error.Text = "<span  style='font-size:medium;'>" + Resources.fbs.Username_notfound + "</span>";
          this.alert_logout.Visible = true;
          this.divWelcome.Visible = false;
        }
        else if (this.Request.QueryString["UN"] == "out")
        {
          this.error.Text = "<span style='font-size:medium;'>" + Resources.fbs.logged_out_msg + "</span>";
          this.alert_logout.Visible = true;
          this.divWelcome.Visible = false;
        }
        else
          this.divWelcome.Visible = false;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
      this.Response.Redirect(this.site_full_path + "login.aspx");
    }
  }
}
