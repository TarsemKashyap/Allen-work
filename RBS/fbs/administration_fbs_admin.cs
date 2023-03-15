// Decompiled with JetBrains decompiler
// Type: administration_fbs_admin
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Configuration;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_fbs_admin : fbs_base_master
{
  public string full_name;
  public string gp_ids = "";
  public string user_id = "";
  public string ac = "";
  public bool showFandB;
  public string sessiontimout_Seconds = "";
  protected ContentPlaceHolder cph_stylesheet;
  protected PlaceHolder ph_menu;
  protected HiddenField hdnPageName;
  protected ContentPlaceHolder ContentPlaceHolder1;
  protected HtmlForm form1;
  protected ContentPlaceHolder cph_footer_scripts;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.sessiontimout_Seconds = ((SessionStateSection) WebConfigurationManager.GetSection("system.web/sessionState")).Timeout.TotalSeconds.ToString();
    }
    catch (Exception ex)
    {
      this.sessiontimout_Seconds = "1200";
    }
    if (this.current_user.groups.Count == 0)
      this.redirect_unauthorized();
    else if (!this.gp.isAdminType && !this.gp.isSuperUserType)
      this.redirect_unauthorized();
    this.gp_ids = new util().get_group_ids(this.current_user);
    UserControl userControl = new UserControl();
    this.ph_menu.Controls.Add(this.Page.LoadControl("~/controls/admin_menu.ascx"));
    this.full_name = this.current_user.full_name;
    this.user_id = this.current_user.user_id.ToString();
    this.ac = this.current_user.account_id.ToString();
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
