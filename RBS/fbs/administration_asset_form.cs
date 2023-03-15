// Decompiled with JetBrains decompiler
// Type: administration_asset_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_asset_form : fbs_base_page, IRequiresSessionState
{
  public string room_name = "";
  public string htmlheading = "";
  public string htmlviewtabl = "";
  public string isAdmin = "";
  public string activetab = "";
  public string assetID_for_allowjs = "NO";
  public string current_tab_li = "";
  public string current_tab = "";
  public string Allow = "";
  public string allow_group = "";
  public string Allow_resend_email_notify = "";
  private DataSet settings_data;
  protected PlaceHolder control_asset_edit_details;
  protected controls_asset_edit_details ucAssetEditDetails;
  protected PlaceHolder control_asset_edit_properties;
  protected controls_asset_edit_properties ucAssetEditProperties;
  protected PlaceHolder control_asset_edit_business_rules;
  protected controls_asset_edit_business_rules ucAssetEditBusinessRules;
  protected PlaceHolder control_asset_edit_blocked_dates;
  protected controls_asset_edit_blocked_dates ucAssetEditBlockedDate;
  protected PlaceHolder control_asset_layout;
  protected controls_asset_layout_image ucALI;
  protected PlaceHolder control_asset_image;
  protected controls_asset_image ucAFI;
  protected PlaceHolder control_permissions;
  protected controls_asset_permissions ucAssetPermissions;
  protected HiddenField hdn_asset_id;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.gp = (groups_permission) this.Session["gp_info"];
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
      this.Response.Redirect(this.site_full_path + Resources.fbs.login_page);
    }
    if (this.Session["Active_tab"] != null)
    {
      this.activetab = this.Session["Active_tab"].ToString();
      this.Session.Remove("Active_tab");
    }
    if (this.Session["Mailsend"] != null)
    {
      if (this.Session["Mailsend"].ToString() == "resend")
      {
        this.Allow_resend_email_notify = "y";
        this.Session.Remove("Mailsend");
      }
    }
    try
    {
      if (this.gp.isAdminType)
        this.isAdmin = "yes";
      if (this.Request.QueryString["Grp"] != null && !string.IsNullOrWhiteSpace(this.Request.QueryString["Grp"]))
        this.allow_group = this.Request.QueryString["Grp"].ToString();
      if (this.Session["current_tab"] != null)
      {
        List<string> stringList = (List<string>) this.Session["current_tab"];
        this.current_tab_li = stringList[0];
        this.current_tab = stringList[1];
        this.Session.Remove("current_tab");
      }
      else if (this.Request.QueryString["asset_id"] != null)
      {
        if (this.Request.QueryString["asset_id"].ToString() == "0")
        {
          this.current_tab_li = "Add Room Details";
          this.current_tab = "tab_edit_details";
        }
        else if (this.gp.facility_edit)
        {
          this.current_tab_li = "Edit Room Details";
          this.current_tab = "tab_edit_details";
        }
        else
        {
          this.current_tab = "tab_edit_properties";
          this.current_tab_li = "Edit Room Equipment";
        }
      }
      this.pageload_data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    try
    {
      string str = this.Request.QueryString["asset_id"];
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      if (str != "0")
      {
        this.assetID_for_allowjs = "YES";
        if (this.Request.QueryString["Type"] == null)
        {
          this.Allow = "YES";
          if (this.gp.isAdminType)
            stringBuilder1.Append("<li class='active'><a href='#tab_edit_details' data-toggle='tab'>Edit Room Details</a></li>");
          else if (this.gp.facility_edit)
            stringBuilder1.Append("<li class='active'><a href='#tab_edit_details' data-toggle='tab'>Edit Room Details</a></li>");
          stringBuilder1.Append("<li><a href='#tab_edit_properties' data-toggle='tab'>Edit Room Equipment</a></li>");
          stringBuilder1.Append("<li><a href='#tab_edit_business_rules' data-toggle='tab'>Business Rules</a></li>");
          stringBuilder1.Append("<li><a href='#tab_blocked_dates' data-toggle='tab'>Blocked Dates</a></li>");
          stringBuilder1.Append("<li><a href='#tab_layout' data-toggle='tab'>Location</a></li>");
          stringBuilder1.Append("<li><a href='#tab_image' data-toggle='tab'>Room Image</a></li>");
          if (this.gp.facility_permissions || this.gp.isAdminType)
            stringBuilder1.Append("<li><a href='#tab_permissions' data-toggle='tab'>Permissions</a></li>");
          this.htmlheading = stringBuilder1.ToString();
        }
      }
      else
      {
        stringBuilder1.Append("<li class='active'><a href='#tab_edit_details' data-toggle='tab'>Add Room Details</a></li>");
        this.htmlheading = stringBuilder1.ToString();
      }
      this.settings_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (this.settings_data == null)
      {
        this.settings_data = this.settings.view_settings(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) this.settings_data);
      }
      if (str != "0")
      {
        asset asset = this.assets.get_asset(Convert.ToInt64(str), this.current_user.account_id);
        DataRow[] dataRowArray1 = this.settings_data.Tables[0].Select("setting_id='" + (object) asset.building_id + "'");
        asset.building = new setting();
        asset.building.setting_id = Convert.ToInt64(dataRowArray1[0]["setting_id"]);
        asset.building.value = dataRowArray1[0]["value"].ToString();
        DataRow[] dataRowArray2 = this.settings_data.Tables[0].Select("setting_id='" + (object) asset.level_id + "'");
        asset.level = new setting();
        asset.level.setting_id = Convert.ToInt64(dataRowArray2[0]["setting_id"]);
        asset.level.value = dataRowArray2[0]["value"].ToString();
        DataRow[] dataRowArray3 = this.settings_data.Tables[0].Select("setting_id='" + (object) asset.category_id + "'");
        asset.category = new setting();
        asset.category.setting_id = Convert.ToInt64(dataRowArray3[0]["setting_id"]);
        asset.category.value = dataRowArray3[0]["value"].ToString();
        DataRow[] dataRowArray4 = this.settings_data.Tables[0].Select("setting_id='" + (object) asset.asset_type + "'");
        asset.type = new setting();
        asset.type.setting_id = Convert.ToInt64(dataRowArray4[0]["setting_id"]);
        asset.type.value = dataRowArray4[0]["value"].ToString();
        asset.status_value = new setting();
        asset.status_value.setting_id = (long) asset.status;
        asset.status_value.value = asset.status != (short) 1 ? "Not Available" : "Available";
        if (this.Request.QueryString["Type"] != null)
        {
          this.room_name = asset.name;
          this.ucAFI.Visible = false;
          this.ucALI.Visible = false;
          this.ucAssetEditProperties.Visible = false;
          this.ucAssetEditBusinessRules.Visible = false;
          this.ucAssetEditBlockedDate.Visible = false;
          this.ucAssetPermissions.Visible = false;
        }
        else
        {
          this.room_name = asset.name;
          this.ucAFI.objAsset = asset;
          this.ucALI.objAsset = asset;
          this.ucAssetEditDetails.Attributes.Add("asset_id", str);
          this.ucAssetEditDetails.obj = asset;
          this.ucAssetEditDetails.SettingsData = this.settings_data;
          this.ucAssetEditProperties.Attributes.Add("asset_id", str);
          this.ucAssetEditBusinessRules.Attributes.Add("asset_id", str);
          this.ucAssetEditBusinessRules.obj = asset;
          this.ucAssetEditBusinessRules.SettingsData = this.settings_data;
          this.ucAssetPermissions.Attributes.Add("asset_id", str);
        }
      }
      else
      {
        this.room_name = "  ";
        this.ucAssetEditDetails.SettingsData = this.settings_data;
        this.ucAFI.Visible = false;
        this.ucALI.Visible = false;
        this.ucAssetEditProperties.Visible = false;
        this.ucAssetEditBusinessRules.Visible = false;
        this.ucAssetEditBlockedDate.Visible = false;
        this.ucAssetPermissions.Visible = false;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
