// Decompiled with JetBrains decompiler
// Type: controls_admin_menu
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.UI.HtmlControls;

public class controls_admin_menu : fbs_base_user_control
{
  protected HtmlGenericControl li2;
  protected HtmlGenericControl liBookings;
  protected HtmlGenericControl liFacilities;
  protected HtmlGenericControl licontent_list;
  protected HtmlGenericControl li9;
  protected HtmlGenericControl li5;
  protected HtmlGenericControl li1;
  protected HtmlGenericControl li10;
  protected HtmlGenericControl li11;
  protected HtmlGenericControl li3;
  protected HtmlGenericControl li_additional_resources;
  protected HtmlGenericControl li7;
  protected HtmlGenericControl li8;
  protected HtmlGenericControl li_hotdesk;
  protected HtmlGenericControl li_o_1;
  protected HtmlGenericControl li_o_2;
  protected HtmlGenericControl li_o;
  protected HtmlGenericControl li4;
  protected HtmlGenericControl li_panel;
  protected HtmlGenericControl li_modules;
  protected HtmlGenericControl liBlacklists;
  protected HtmlGenericControl liEmailLogs;
  protected HtmlGenericControl liGlobalSettings;
  protected HtmlGenericControl liGroups;
  protected HtmlGenericControl liHolidays;
  protected HtmlGenericControl liLogs;
  protected HtmlGenericControl liMaster;
  protected HtmlGenericControl liTemplates;
  protected HtmlGenericControl liUsers;
  protected HtmlGenericControl liSettings;
  protected HtmlGenericControl liCR;
  protected HtmlGenericControl liDR;
  protected HtmlGenericControl liHKP;
  protected HtmlGenericControl liNSR;
  protected HtmlGenericControl liUAR;
  protected HtmlGenericControl liUSR;
  protected HtmlGenericControl liURByDept;
  protected HtmlGenericControl liURByRoom;
  protected HtmlGenericControl liReports;
  public string full_name;
  public string gp_ids = "";
  public string sessiontimout_Seconds = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.full_name = this.current_user.full_name;
      this.gp_ids = new util().get_group_ids(this.current_user);
      if (this.IsPostBack)
        return;
      this.set_account_rights_based_link();
      this.set_permissions_based_link();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void set_permissions_based_link()
  {
    if (!this.gp.facility_view)
      this.liFacilities.Visible = false;
    if (!this.gp.holidays_view && !this.gp.users_view && !this.gp.groups_view && !this.gp.master_view && !this.gp.logs_view && !this.gp.settings_view && !this.gp.templates_view)
      this.liSettings.Visible = false;
    if (!this.gp.isAdminType && !this.gp.isSuperUserType)
      this.liSettings.Visible = false;
    if (!this.gp.holidays_view)
      this.liHolidays.Visible = false;
    if (!this.gp.users_view)
      this.liUsers.Visible = false;
    if (!this.gp.groups_view)
      this.liGroups.Visible = false;
    if (!this.gp.master_view)
      this.liMaster.Visible = false;
    if (!this.gp.logs_view)
      this.liLogs.Visible = false;
    if (!this.gp.settings_view)
      this.liGlobalSettings.Visible = false;
    if (!this.gp.templates_view)
      this.liTemplates.Visible = false;
    if (!this.gp.utilization_report_by_department_view && !this.gp.utilization_report_by_room_view && !this.gp.cancellation_report_view && !this.gp.noshow_report_view && !this.gp.unassigned_report_view && !this.gp.upcoming_setup_report_view && !this.gp.housekeeping_report_view && !this.gp.daily_report_view)
      this.liReports.Visible = false;
    if (!this.gp.utilization_report_by_department_view)
      this.liURByDept.Visible = false;
    if (!this.gp.utilization_report_by_room_view)
      this.liURByRoom.Visible = false;
    if (!this.gp.cancellation_report_view)
      this.liCR.Visible = false;
    if (!this.gp.noshow_report_view)
      this.liNSR.Visible = false;
    if (!this.gp.unassigned_report_view)
      this.liUAR.Visible = false;
    if (!this.gp.upcoming_setup_report_view)
      this.liUSR.Visible = false;
    if (!this.gp.housekeeping_report_view)
      this.liHKP.Visible = false;
    if (!this.gp.daily_report_view)
      this.liDR.Visible = false;
    if (!this.gp.isAdminType)
    {
      this.li9.Visible = false;
      this.li5.Visible = false;
      this.li3.Visible = false;
    }
    if (!this.gp.users_blacklist)
      this.liBlacklists.Visible = false;
    if (this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, "resource_module").Tables[0].Rows.Count <= 0)
      return;
    this.li5.Visible = true;
  }

  private void set_account_rights_based_link()
  {
    if (!Convert.ToBoolean(this.current_account.properties["announcements"]))
      this.licontent_list.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.li_additional_resources.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["blacklist_user"]))
      this.liBlacklists.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["email_log"]))
      this.liEmailLogs.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["email_log"]))
      this.liEmailLogs.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["global_settings"]))
      this.liGlobalSettings.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["user_groups"]))
      this.liGroups.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["holidays"]))
      this.liHolidays.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["audit_log"]))
      this.liLogs.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["master_list"]))
      this.liMaster.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["templates"]))
      this.liTemplates.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.liUsers.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_cancellation"]))
      this.liCR.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_daily_view"]))
      this.liDR.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_housekeeping"]))
      this.liHKP.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_noshow"]))
      this.liNSR.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_unassigned"]))
      this.liUAR.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_setup"]))
      this.liUSR.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_util_department"]))
      this.liURByDept.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["report_util_room"]))
      this.liURByRoom.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["outlook_plugin_access"]))
      this.li_o.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["room_display"]))
      this.li_panel.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["resource_items_group_permission"]))
      this.li1.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["resource_terms_and_conditions"]))
      this.li3.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["fault_reporting"]))
      this.li4.Visible = false;
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      return;
    this.li_hotdesk.Visible = true;
  }
}
