// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.fbs_base_master
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;

namespace skynapse.fbs
{
  public class fbs_base_master : MasterPage
  {
    private string current_url = HttpContext.Current.Request.Url.AbsoluteUri;
    private Dictionary<string, user_group> dicUserGroups;
    public string site_full_path;
    public string unauthorized_url = ConfigurationManager.AppSettings[nameof (unauthorized_url)];
    public ILog log = LogManager.GetLogger("fbs_log");
    protected user current_user;
    protected account current_account;
    public string logo = "";
    public user_group u_group;
    public groups_permission gp;
    public List<long> bookable_rooms;
    public List<long> visible_rooms;
    public List<long> approvable_rooms;

    public fbs_base_master() => this.Load += new EventHandler(this.Page_Load);

    protected void Page_Load(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.site_full_path))
        this.site_full_path = ConfigurationManager.AppSettings["site_full_path"];
      if (Convert.ToBoolean(ConfigurationManager.AppSettings["enable_https"]))
      {
        if (!this.site_full_path.Contains("https"))
        {
          this.site_full_path = this.site_full_path.Replace("http", "https");
          this.Response.Redirect(this.current_url.Replace("http", "https"));
        }
      }
      try
      {
        this.current_user = (user) this.Session["user"];
      }
      catch
      {
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
      }
      try
      {
        this.current_account = (account) this.Session["account"];
      }
      catch
      {
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
      }
      try
      {
        this.bookable_rooms = (List<long>) this.Session["rooms"];
      }
      catch (Exception ex)
      {
        this.bookable_rooms = new List<long>();
      }
      try
      {
        this.visible_rooms = (List<long>) this.Session["visible_rooms"];
      }
      catch (Exception ex)
      {
        this.bookable_rooms = new List<long>();
      }
      try
      {
        this.approvable_rooms = (List<long>) this.Session["approvable_rooms"];
      }
      catch (Exception ex)
      {
        this.bookable_rooms = new List<long>();
      }
      try
      {
        this.u_group = (user_group) this.Session["user_group"];
      }
      catch
      {
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
      }
      if (this.current_account != null)
        this.logo = !(this.current_account.logo != "") ? this.site_full_path + "assets/img/ecobook_logo.png" : this.site_full_path + "assets/img/" + this.current_account.logo;
      try
      {
        this.gp = (groups_permission) this.Session["gp_info"];
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
      }
      if (this.current_user == null)
        this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
      if (this.current_user.user_id != 0L)
        return;
      this.Response.Redirect(this.site_full_path + Resources.fbs.login_page + "?ReturnUrl=" + this.Server.UrlEncode(this.current_url));
    }

    public void redirect_unauthorized() => this.Response.Redirect(this.unauthorized_url);
  }
}
