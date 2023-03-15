// Decompiled with JetBrains decompiler
// Type: controls_asset_permissions
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class controls_asset_permissions : fbs_base_user_control
{
  protected HiddenField hdn_asset_id;
  protected HtmlGenericControl error;
  protected Repeater rpt_permissions;
  protected Button btn_add_permissions;
  protected Button btn_cancel;
  public string html_table;
  public string htmlbtn;
  public DataSet _objSetting;
  public string Staus = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.Session["Status_permissions"] != null && this.Session["Status_permissions"] == (object) "S")
      {
        this.Staus = "S";
        this.Session.Remove("Status_permissions");
      }
      if (this.IsPostBack)
        return;
      this.load_permissions_data();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void load_permissions_data()
  {
    try
    {
      long asset_id = 0;
      if (Convert.ToInt64(this.Attributes["asset_id"]) != 0L)
      {
        asset_id = Convert.ToInt64(this.Attributes["asset_id"]);
        this.hdn_asset_id.Value = asset_id.ToString();
      }
      DataSet groupsPermissions = this.assets.get_groups_permissions(this.current_user.account_id, asset_id, 0L, 0L, "");
      if (groupsPermissions == null)
        return;
      this.rpt_permissions.DataSource = (object) groupsPermissions.Tables[0];
      this.rpt_permissions.DataBind();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  public bool IsChecked(string is_view)
  {
    bool flag = true;
    try
    {
      if (!string.IsNullOrEmpty(is_view))
        flag = Convert.ToBoolean(is_view);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error ->", ex);
    }
    return flag;
  }

  public bool IsEnabled(
    string asset_id,
    string asset_owner_group_id,
    string asset_owner_group_type,
    string group_id,
    string group_type)
  {
    bool flag = true;
    try
    {
      long num1 = string.IsNullOrEmpty(this.hdn_asset_id.Value) ? 0L : Convert.ToInt64(this.hdn_asset_id.Value);
      long num2 = string.IsNullOrEmpty(asset_id) ? 0L : Convert.ToInt64(asset_id);
      if (!string.IsNullOrEmpty(asset_owner_group_id))
        Convert.ToInt64(asset_owner_group_id);
      if (!string.IsNullOrEmpty(asset_owner_group_type))
        Convert.ToInt64(asset_owner_group_type);
      if (!string.IsNullOrEmpty(group_id))
        Convert.ToInt64(group_id);
      if (!string.IsNullOrEmpty(group_type))
        Convert.ToInt64(group_type);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error ->", ex);
    }
    return flag;
  }

  protected void btn_add_permissions_Click(object sender, EventArgs e)
  {
    try
    {
      bool flag = false;
      long num = string.IsNullOrEmpty(this.hdn_asset_id.Value) ? 0L : Convert.ToInt64(this.hdn_asset_id.Value);
      foreach (RepeaterItem repeaterItem in this.rpt_permissions.Items)
      {
        CheckBox control1 = (CheckBox) repeaterItem.FindControl("chkRowView");
        CheckBox control2 = (CheckBox) repeaterItem.FindControl("chkRowBook");
        HiddenField control3 = (HiddenField) repeaterItem.FindControl("hdn_assetID");
        HiddenField control4 = (HiddenField) repeaterItem.FindControl("hdn_asset_permission_id");
        HiddenField control5 = (HiddenField) repeaterItem.FindControl("hdn_group_id");
        HiddenField control6 = (HiddenField) repeaterItem.FindControl("hdn_group_record_id");
        asset_permission assetPermission1 = new asset_permission();
        assetPermission1.asset_permission_id = 0L;
        assetPermission1.account_id = this.current_user.account_id;
        assetPermission1.created_by = this.current_user.user_id;
        assetPermission1.modified_by = this.current_user.user_id;
        assetPermission1.asset_id = num;
        assetPermission1.group_id = string.IsNullOrEmpty(control5.Value) ? 0L : Convert.ToInt64(control5.Value);
        assetPermission1.user_id = this.current_user.user_id;
        assetPermission1.is_view = control1.Checked;
        assetPermission1.is_book = control2.Checked;
        assetPermission1.remarks = "";
        assetPermission1.record_id = string.IsNullOrEmpty(control6.Value) ? Guid.NewGuid() : new Guid(control6.Value);
        asset_permission assetPermission2;
        if (!string.IsNullOrEmpty(control4.Value))
        {
          assetPermission1.asset_permission_id = string.IsNullOrEmpty(control4.Value) ? 0L : Convert.ToInt64(control4.Value);
          assetPermission2 = this.assets.update_assets_permissions(assetPermission1);
          flag = true;
        }
        else
        {
          assetPermission2 = this.assets.update_assets_permissions(assetPermission1);
          flag = true;
        }
      }
      if (!flag)
        return;
      this.Session["Status_permissions"] = (object) "S";
      this.Session.Add("current_tab", (object) new List<string>()
      {
        "Permissions",
        "tab_permissions"
      });
      this.Response.Redirect("~/administration/asset_form.aspx?asset_id=" + (object) num + "#tab_permissions");
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect(this.Request.UrlReferrer.ToString());
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }
}
