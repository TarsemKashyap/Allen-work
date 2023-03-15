// Decompiled with JetBrains decompiler
// Type: terms_and_conditions
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class terms_and_conditions : fbs_base_page, IRequiresSessionState
{
  protected HiddenField hdnSettingID;
  protected HtmlTextArea txtarea;
  protected Button btn_submit;
  protected Button btn_cancel;
  private bool is_admin;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    foreach (user_group userGroup in this.current_user.groups.Values)
    {
      if (userGroup.group_type == 1)
        this.is_admin = true;
      else if (!this.is_admin)
        this.is_admin = false;
    }
    if (!this.is_admin)
      this.redirect_unauthorized();
    if (this.IsPostBack)
      return;
    try
    {
      DataSet settingsByParameter = this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, nameof (terms_and_conditions), this.str_resource_module);
      if (!this.utilities.isValidDataset(settingsByParameter))
        return;
      this.hdnSettingID.Value = settingsByParameter.Tables[0].Rows[0]["setting_id"].ToString();
      this.txtarea.Value = settingsByParameter.Tables[0].Rows[0]["value"].ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking - Terms and conditions - Error->", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect("resource_items.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      resource_settings resourceSettings = new resource_settings();
      try
      {
        resourceSettings.setting_id = Convert.ToInt64(this.hdnSettingID.Value);
      }
      catch
      {
        resourceSettings.setting_id = 0L;
      }
      resourceSettings.parent_id = 0L;
      resourceSettings.status = 1L;
      resourceSettings.account_id = this.current_user.account_id;
      resourceSettings.created_by = this.current_user.user_id;
      resourceSettings.description = "";
      resourceSettings.modified_by = this.current_user.user_id;
      resourceSettings.module_name = this.str_resource_module;
      resourceSettings.parameter = nameof (terms_and_conditions);
      resourceSettings.value = this.txtarea.InnerText;
      resourceSettings.record_id = Guid.NewGuid();
      this.resapi.update_resource_settings(resourceSettings);
      this.Response.Redirect("terms_and_conditions.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }
}
