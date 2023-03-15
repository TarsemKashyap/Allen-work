// Decompiled with JetBrains decompiler
// Type: controls_asset_view
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;

public class controls_asset_view : fbs_base_user_control
{
  public string asset_name;
  public string asset_building;
  public string asset_level;
  public string asset_category;
  public string asset_type;
  public string asset_capacity;
  public string asset_status;
  public string asset_description;
  public string asset_is_restricted;
  public string asset_owner_group;
  public string modified_on;
  public string modified_by;
  public asset _objAsset;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  public asset obj
  {
    get => this._objAsset;
    set => this._objAsset = value;
  }

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.IsPostBack)
        return;
      this.pageload_data();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void pageload_data()
  {
    try
    {
      if (Convert.ToInt64(this.Attributes["asset_id"]) == 0L)
        return;
      Convert.ToInt64(this.Attributes["asset_id"]);
      asset asset = this.obj;
      DataSet user = this.reportings.get_user(this.current_user.account_id, Convert.ToInt64(asset.modified_by));
      this.asset_name = !string.IsNullOrEmpty(asset.code) ? asset.code + " / " + asset.name : asset.name;
      this.asset_building = asset.building.value;
      this.asset_level = asset.level.value;
      this.asset_category = asset.category.value;
      this.asset_type = asset.type.value;
      this.asset_capacity = !(asset.capacity.ToString() != "-1") ? "NA" : asset.capacity.ToString();
      this.asset_status = asset.status_value.value;
      this.asset_description = asset.description;
      DataSet groups = this.users.get_groups(this.current_user.account_id);
      if (groups.Tables[0].Rows.Count > 0)
      {
        DataRow[] dataRowArray = groups.Tables[0].Select("group_id='" + (object) asset.asset_owner_group_id + "'");
        if (dataRowArray.Length > 0)
          this.asset_owner_group = Convert.ToString(dataRowArray[0]["group_name"].ToString());
      }
      this.asset_is_restricted = !asset.is_restricted ? "No" : "Yes";
      this.modified_on = this.tzapi.convert_to_user_timestamp(asset.modified_on).ToString(api_constants.display_datetime_format);
      if (user.Tables[0].Rows.Count <= 0)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) user.Tables[0].Rows)
        this.modified_by = row["full_name"].ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }
}
