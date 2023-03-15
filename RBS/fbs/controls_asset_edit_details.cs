// Decompiled with JetBrains decompiler
// Type: controls_asset_edit_details
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public class controls_asset_edit_details : fbs_base_user_control
{
  protected ScriptManager ScriptManager1;
  protected HtmlGenericControl errorname;
  protected TextBox txt_asset_name;
  protected HiddenField hdn_id;
  protected HiddenField hdn_fld_property_id;
  protected TextBox txt_asset_code;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected DropDownList ddl_category;
  protected DropDownList ddl_type;
  protected DropDownList ddl_group;
  protected TextBox txt_capacity;
  protected DropDownList ddl_status;
  protected CheckBox chk_senteamil;
  protected RegularExpressionValidator RegularExpressionValidator2;
  protected TextBox txt_description;
  protected CheckBox chkAddRes;
  protected Button btn_submit;
  protected Button btn_cancel;
  public asset _objAsset;
  public DataSet _objSetting;
  public string Staus = "";
  public string asset_id = "0";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  public asset obj
  {
    get => this._objAsset;
    set => this._objAsset = value;
  }

  public DataSet SettingsData
  {
    get => this._objSetting;
    set => this._objSetting = value;
  }

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      this.Session.Remove("asset_property_id");
      this.Session.Remove("email_asset_id");
      this.Session.Remove("additional_resource_asset_id");
      this.Pageload_date();
      if (this.Session["Status"] == null)
        return;
      if (this.Session["Status"] == (object) "S")
      {
        this.Staus = "S";
        this.Session.Remove("Status");
      }
      object obj = this.Session["nameexsit"];
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Pageload_date()
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (Convert.ToInt64(this.Attributes["asset_id"]) != 0L)
      {
        DataSet assetProperties = this.assets.get_asset_properties(Convert.ToInt64(this.Attributes["asset_id"]), this.current_user.account_id);
        this.asset_id = this.Attributes["asset_id"].ToString();
        if (assetProperties.Tables[0].Rows.Count > 0)
        {
          DataRow[] dataRowArray1 = assetProperties.Tables[0].Select("property_name='is_restricted_message'");
          this.hdn_fld_property_id.Value = dataRowArray1.Length <= 0 ? "0" : dataRowArray1[0][0].ToString();
          DataRow[] dataRowArray2 = assetProperties.Tables[0].Select("property_name='asset_status'");
          if (dataRowArray2.Length > 0)
            this.Session["asset_property_id"] = (object) dataRowArray2[0][0].ToString();
          else
            this.Session["asset_property_id"] = (object) "0";
          DataRow[] dataRowArray3 = assetProperties.Tables[0].Select("property_name='is_email_send'");
          DataRow[] dataRowArray4 = assetProperties.Tables[0].Select("property_name='request_additional_resources'");
          if (dataRowArray3.Length > 0)
          {
            this.Session["email_asset_id"] = (object) dataRowArray3[0]["asset_property_id"].ToString();
            this.chk_senteamil.Checked = dataRowArray3[0]["property_value"].ToString() == "1";
          }
          else
          {
            this.Session["email_asset_id"] = (object) "0";
            this.chk_senteamil.Checked = false;
          }
          if (dataRowArray4.Length > 0)
          {
            this.Session["additional_resource_asset_id"] = (object) dataRowArray4[0]["asset_property_id"].ToString();
            this.chkAddRes.Checked = dataRowArray4[0]["property_value"].ToString() == "1";
          }
          else
          {
            this.Session["additional_resource_asset_id"] = (object) "0";
            this.chkAddRes.Checked = false;
          }
        }
        else
        {
          this.Session["email_asset_id"] = (object) "0";
          this.Session["asset_property_id"] = (object) "0";
          this.Session["additional_resource_asset_id"] = (object) "0";
          this.hdn_fld_property_id.Value = "0";
        }
        this.txt_asset_code.Text = this.obj.code;
        this.txt_asset_name.Text = this.obj.name;
        this.txt_capacity.Text = Convert.ToString(this.obj.capacity);
        this.txt_description.Text = this.obj.description;
      }
      DataSet settingsData = this.SettingsData;
      DataSet groups = this.users.get_groups(this.current_user.account_id);
      this.hdn_id.Value = Convert.ToInt64(this.Attributes["asset_id"]).ToString();
      this.populate_building(this.obj, settingsData);
      this.populate_level(this.obj, settingsData);
      this.populate_category(this.obj, settingsData);
      this.populate_type(this.obj, settingsData);
      this.populate_groups(this.obj, groups);
      this.populate_status(this.obj, settingsData);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_building(asset obj, DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='building'"))
        this.ddl_building.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddl_building.SelectedValue = obj.building_id.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_level(asset obj, DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='level'"))
        this.ddl_level.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddl_level.SelectedValue = obj.level_id.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_category(asset obj, DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='category'"))
        this.ddl_category.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddl_category.SelectedValue = obj.category_id.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_type(asset obj, DataSet data)
  {
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='asset_type'"))
        this.ddl_type.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddl_type.SelectedValue = obj.asset_type.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_groups(asset obj, DataSet data)
  {
    try
    {
      DataSet groupsPermissions = this.assets.get_groups_permissions(this.current_user.account_id, string.IsNullOrEmpty(this.Attributes["asset_id"]) ? 0L : Convert.ToInt64(this.Attributes["asset_id"]), 0L, 0L, "");
      this.ddl_group.Items.Add(new ListItem("", "0"));
      foreach (DataRow row in (InternalDataCollectionBase) data.Tables[0].Rows)
      {
        if (row["group_type"].ToString() == "2")
        {
          bool flag = false;
          if (groupsPermissions != null && groupsPermissions.Tables[0].Rows.Count > 0)
          {
            DataRow[] dataRowArray = groupsPermissions.Tables[0].Select("group_id = '" + row["group_id"] + "'");
            if (dataRowArray.Length > 0 && (string.IsNullOrEmpty(dataRowArray[0]["is_book"].ToString()) || !Convert.ToBoolean(dataRowArray[0]["is_book"])))
              flag = true;
          }
          this.ddl_group.Items.Add(!flag ? new ListItem(row["group_name"].ToString(), row["group_id"].ToString()) : new ListItem(row["group_name"].ToString() + " (View Only)", row["group_id"].ToString()));
        }
      }
      this.ddl_group.SelectedValue = obj.asset_owner_group_id.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_status(asset obj, DataSet data)
  {
    try
    {
      this.ddl_status.Items.Add(new ListItem("Available", "1"));
      this.ddl_status.Items.Add(new ListItem("Not Available", "0"));
      this.ddl_status.SelectedValue = obj.status.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
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

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      if (this.assets.checknameavilablity(Convert.ToInt32(this.ddl_building.SelectedItem.Value), Convert.ToInt32(this.ddl_level.SelectedItem.Value), this.txt_asset_name.Text.Trim(), Convert.ToInt64(this.Attributes["asset_id"]), this.txt_asset_code.Text.Trim(), this.current_user.account_id))
      {
        if (!this.validate_fields())
          return;
        asset asset1 = new asset();
        asset_property assetProperty1 = new asset_property();
        DataSet settingsData = this.SettingsData;
        asset1.asset_id = Convert.ToInt64(this.Attributes["asset_id"]);
        if (asset1.asset_id > 0L)
          asset1 = this.obj;
        else
          asset1.record_id = Guid.NewGuid();
        DataRow[] dataRowArray = settingsData.Tables[0].Select("parameter ='asset_status' and value='" + this.ddl_status.SelectedItem.Text + "'");
        if (dataRowArray.Length > 0)
        {
          assetProperty1.property_name = "asset_status";
          assetProperty1.property_value = dataRowArray[0]["setting_id"].ToString();
          assetProperty1.status = Convert.ToInt16(dataRowArray[0]["status"].ToString());
        }
        asset1.account_id = this.current_user.account_id;
        asset1.asset_owner_group_id = Convert.ToInt64(this.ddl_group.SelectedItem.Value);
        asset1.asset_type = Convert.ToInt64(this.ddl_type.SelectedItem.Value);
        asset1.available_for_booking = true;
        asset1.is_restricted = false;
        asset1.level_id = Convert.ToInt32(this.ddl_level.SelectedItem.Value);
        asset1.created_by = this.current_user.user_id;
        asset1.created_on = this.current_timestamp;
        asset1.properties = new XmlDocument();
        asset1.properties.LoadXml("<properties></properties>");
        asset1.modified_by = this.current_user.user_id;
        asset1.modified_on = this.current_timestamp;
        asset1.name = this.txt_asset_name.Text;
        asset1.building_id = Convert.ToInt32(this.ddl_building.SelectedItem.Value);
        asset1.capacity = Convert.ToInt16(this.txt_capacity.Text);
        asset1.category_id = Convert.ToInt64(this.ddl_category.SelectedItem.Value);
        asset1.code = this.txt_asset_code.Text;
        asset1.description = this.txt_description.Text;
        asset1.status = Convert.ToInt16(this.ddl_status.SelectedItem.Value);
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.created_on = this.current_timestamp;
        assetProperty1.record_id = asset1.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.remarks = "";
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.modified_on = this.current_timestamp;
        asset1.category_id = Convert.ToInt64(this.ddl_category.SelectedItem.Value);
        assetProperty1.available = this.ddl_status.SelectedItem.Text.ToLower() == "available";
        asset asset2 = this.assets.update_asset(asset1);
        if (asset2.asset_id != 0L)
        {
          if (this.Session["asset_property_id"] == null)
            this.Session["asset_property_id"] = (object) "0";
          assetProperty1.asset_id = asset2.asset_id;
          assetProperty1.asset_property_id = Convert.ToInt64(this.Session["asset_property_id"].ToString());
          asset_property assetProperty2 = this.assets.update_asset_property(assetProperty1);
          assetProperty2.asset_property_id = this.Session["email_asset_id"] == null ? 0L : (!(this.Session["email_asset_id"].ToString() != "") ? 0L : Convert.ToInt64(this.Session["email_asset_id"].ToString()));
          asset_property assetProperty3;
          if (this.chk_senteamil.Checked)
          {
            assetProperty2.property_name = "is_email_send";
            assetProperty2.property_value = "1";
            assetProperty3 = this.assets.update_asset_property(assetProperty2);
          }
          else
          {
            assetProperty2.property_name = "is_email_send";
            assetProperty2.property_value = "0";
            assetProperty3 = this.assets.update_asset_property(assetProperty2);
          }
          assetProperty3.asset_property_id = this.Session["additional_resource_asset_id"] == null ? 0L : (!(this.Session["additional_resource_asset_id"].ToString() != "") ? 0L : Convert.ToInt64(this.Session["additional_resource_asset_id"].ToString()));
          asset_property assetProperty4;
          if (this.chkAddRes.Checked)
          {
            assetProperty3.property_name = "request_additional_resources";
            assetProperty3.property_value = "1";
            assetProperty4 = this.assets.update_asset_property(assetProperty3);
          }
          else
          {
            assetProperty3.property_name = "request_additional_resources";
            assetProperty3.property_value = "0";
            assetProperty4 = this.assets.update_asset_property(assetProperty3);
          }
          if (assetProperty4.asset_id == 0L)
          {
            this.Session.Remove("asset_property_id");
            this.Session.Remove("email_asset_id");
            this.Session.Remove("additional_resource_asset_id");
          }
        }
        else if (this.Session["asset_property_id"] != null)
          this.Session.Remove("asset_property_id");
        this.errorname.InnerHtml = "";
        if (asset2.asset_id != 0L)
        {
          DataSet groupsPermissions = this.assets.get_groups_permissions(this.current_user.account_id, asset2.asset_id, 0L, 0L, "");
          if (this.utilities.isValidDataset(groupsPermissions))
          {
            for (int index = 0; index < groupsPermissions.Tables[0].Rows.Count; ++index)
              this.call_update_asset_permissions(asset2.asset_id, Convert.ToInt64(groupsPermissions.Tables[0].Rows[index]["group_id"]), asset2.record_id);
          }
        }
        this.Session.Add("current_tab", (object) new List<string>()
        {
          "Edit Room Details",
          "tab_edit_details"
        });
        if (Convert.ToInt64(this.Attributes["asset_id"]) != 0L)
        {
          this.Session["Status"] = (object) "S";
          this.Response.Redirect("~/administration/asset_form.aspx?asset_id=" + (object) asset2.asset_id + "#tab_edit_details");
        }
        else
        {
          this.Session["Status"] = (object) "S";
          this.Response.Redirect("~/administration/asset_form.aspx?asset_id=" + (object) asset2.asset_id + "#tab_edit_properties");
        }
      }
      else
        this.errorname.InnerHtml = Resources.fbs.asset_edit_checkname;
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private bool validate_fields() => true;

  private void call_update_asset_permissions(long asset_id, long group_id, Guid group_record_id)
  {
    try
    {
      DataSet groupsPermissions = this.assets.get_groups_permissions(this.current_user.account_id, asset_id, 0L, 0L, "");
      if (groupsPermissions == null || groupsPermissions.Tables[0].Rows.Count <= 0)
        return;
      DataRow[] dataRowArray = groupsPermissions.Tables[0].Select("group_id = '" + (object) group_id + "'");
      if (dataRowArray.Length <= 0)
        return;
      long num = string.IsNullOrEmpty(dataRowArray[0]["asset_permission_id"].ToString()) ? 0L : Convert.ToInt64(dataRowArray[0]["asset_permission_id"]);
      asset_permission assetPermission1 = new asset_permission();
      assetPermission1.asset_permission_id = 0L;
      assetPermission1.account_id = this.current_user.account_id;
      assetPermission1.created_by = this.current_user.user_id;
      assetPermission1.modified_by = this.current_user.user_id;
      assetPermission1.asset_id = asset_id;
      assetPermission1.group_id = group_id;
      assetPermission1.user_id = this.current_user.user_id;
      assetPermission1.is_view = true;
      assetPermission1.is_book = true;
      assetPermission1.remarks = "";
      assetPermission1.record_id = group_record_id;
      asset_permission assetPermission2;
      if (num > 0L)
      {
        assetPermission1.asset_permission_id = num;
        assetPermission2 = this.assets.update_assets_permissions(assetPermission1);
      }
      else
        assetPermission2 = this.assets.update_assets_permissions(assetPermission1);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }
}
