// Decompiled with JetBrains decompiler
// Type: fbs
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Configuration;
using System.Data;
using System.Web.Configuration;
using System.Web.Profile;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class fbs : fbs_base_master
{
  protected ContentPlaceHolder cph_stylesheet;
  protected HtmlGenericControl li1;
  protected HtmlGenericControl li_bk_qk;
  protected HtmlGenericControl li_bk_ad;
  protected HtmlGenericControl li_bk_res;
  protected HtmlGenericControl li_bk_hd;
  protected HtmlGenericControl li_new;
  protected HtmlGenericControl li_an;
  protected HtmlGenericControl li33;
  protected HtmlGenericControl li_view_res_bk;
  protected HtmlGenericControl li_reassign_bookings;
  protected HtmlGenericControl li3;
  protected HtmlGenericControl mytask_count;
  protected HtmlGenericControl li_tsk;
  protected HtmlGenericControl li_subscribe;
  protected HtmlGenericControl li_admin;
  protected HtmlGenericControl liprofile;
  protected HtmlGenericControl lipref;
  protected HiddenField hdnPageName;
  protected ContentPlaceHolder ContentPlaceHolder1;
  protected HtmlForm form1;
  protected ContentPlaceHolder cph_footer_scripts;
  public string full_name;
  private DataSet setting_data;
  public string gp_ids = "";
  public string user_id = "";
  public string ac = "";
  public string sessiontimout_Seconds = "";
  public new string unauthorized_url = ConfigurationManager.AppSettings[nameof (unauthorized_url)];
  public long login_type;
  public string html_unread;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.sessiontimout_Seconds = ((SessionStateSection) WebConfigurationManager.GetSection("system.web/sessionState")).Timeout.TotalSeconds.ToString();
    }
    catch
    {
      this.sessiontimout_Seconds = "1200";
    }
    if (this.current_user == null)
      this.redirect_unauthorized();
    this.full_name = this.current_user.full_name;
    if (this.current_user.login_type == 1L)
      this.liprofile.Visible = false;
    else
      this.liprofile.Visible = true;
    this.ac = this.current_user.account_id.ToString();
    this.user_id = this.current_user.user_id.ToString();
    if (!this.IsPostBack)
    {
      long unreadContentCount = new content_api().get_unread_content_count(this.current_user.user_id, this.current_user.account_id);
      if (unreadContentCount > 0L)
        this.html_unread = "<span class='badge badge-info'>" + (object) unreadContentCount + "</span>";
      this.set_account_rights_based_link();
    }
    this.set_permissions_based_link();
  }

  private void set_permissions_based_link()
  {
    this.gp_ids = new util().get_group_ids(this.current_user);
    if (this.u_group.group_type == 0)
    {
      this.li_tsk.Visible = false;
      this.li_new.Visible = false;
      this.li_admin.Visible = false;
    }
    if (this.u_group.group_type == 3 || this.u_group.group_type == 0)
      this.li_admin.Visible = false;
    else if (this.u_group.group_type == 2 || this.u_group.group_type == 1)
      this.li_admin.Visible = true;
    else
      this.li_admin.Visible = false;
    string[] segments = this.Request.Url.Segments;
    try
    {
      this.hdnPageName.Value = segments[segments.GetUpperBound(0)].Split('.')[0];
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void set_account_rights_based_link()
  {
    if (!Convert.ToBoolean(this.current_account.properties["quick_booking"]))
      this.li_bk_qk.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["advanced_booking"]))
      this.li_bk_ad.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
    {
      this.li_view_res_bk.Visible = false;
      this.li_bk_res.Visible = false;
    }
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      this.li_bk_hd.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["announcements"]))
      this.li_an.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["subscribe"]))
      this.li_subscribe.Visible = false;
    if (Convert.ToBoolean(this.current_account.properties["reassign"]))
      return;
    this.li_reassign_bookings.Visible = false;
  }

  public new void redirect_unauthorized() => this.Response.Redirect(this.unauthorized_url);
}
