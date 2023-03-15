// Decompiled with JetBrains decompiler
// Type: asset_info
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class asset_info : fbs_base_page, IRequiresSessionState
{
  protected HtmlHead Head1;
  protected CheckBox chk_fav;
  protected HtmlGenericControl li_faulty;
  protected HiddenField hdn_asset_id;
  protected HiddenField hdn_fav_id;
  protected Image imagfacility;
  protected HtmlGenericControl owner_group;
  protected Image imgLayout;
  protected Button btn_cancel;
  protected HtmlForm form1;
  public string asset_name;
  public string asset_building;
  public string asset_level;
  public string asset_category;
  public string asset_type;
  public string asset_capacity;
  public string cancel_before_window;
  public string cancel_after_window;
  public string booking_lead_time;
  public string asset_status;
  public string asset_description;
  public string asset_owner_group;
  public string modified_on;
  public string modified_by;
  public string asset_properties;
  public string group_memebers;
  public string op_hours;
  public string book_weekend;
  public string book_holiday;
  public string advance_booking_window;
  private long asset_id;
  private long problem_id;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    try
    {
      if (this.current_user == null)
      {
        Modal.Close((Page) this);
        this.redirect_unauthorized();
      }
      this.asset_id = Convert.ToInt64(this.Request.QueryString["r"]);
      if (this.asset_id <= 0L || this.IsPostBack)
        return;
      this.populate_data(this.asset_id);
      try
      {
        this.problem_id = Convert.ToInt64(this.Request.QueryString["prob_id"]);
      }
      catch
      {
        this.problem_id = 0L;
      }
      if (this.problem_id > 0L)
        this.li_faulty.Visible = true;
      else
        this.li_faulty.Visible = false;
    }
    catch
    {
    }
  }

  private void populate_asset_problems(long prb_id)
  {
  }

  protected void populate_data(long asset_id)
  {
    try
    {
      this.hdn_asset_id.Value = asset_id.ToString();
      asset asset = this.assets.get_asset(asset_id, this.current_user.account_id);
      user user = this.users.get_user(asset.modified_by, asset.account_id);
      DataSet dataSet1 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
      if (dataSet1 == null)
      {
        dataSet1 = this.settings.view_settings(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) dataSet1);
      }
      this.asset_name = !string.IsNullOrEmpty(asset.code) ? asset.code + " / " + asset.name : asset.name;
      this.asset_building = asset.building.value;
      this.asset_level = asset.level.value;
      this.asset_category = asset.category.value;
      this.asset_type = asset.type.value;
      this.asset_capacity = !(asset.capacity.ToString() == "-1") ? asset.capacity.ToString() : "NA";
      this.asset_status = asset.status_value.value;
      this.asset_description = asset.description;
      DataSet groups = this.users.get_groups(this.current_user.account_id);
      if (groups.Tables[0].Rows.Count > 0)
      {
        DataRow[] dataRowArray = groups.Tables[0].Select("group_id='" + (object) asset.asset_owner_group_id + "'");
        if (dataRowArray.Length > 0)
          this.asset_owner_group = Convert.ToString(dataRowArray[0]["group_name"].ToString());
      }
      this.modified_on = this.tzapi.convert_to_user_timestamp(asset.modified_on).ToString(api_constants.display_datetime_format);
      this.modified_by = user.full_name;
      this.populate_asset_properties(asset.asset_properties, asset);
      DataSet usernameFromgroup = this.reportings.get_username_fromgroup(asset.asset_owner_group_id);
      if (usernameFromgroup != null)
      {
        int num = 0;
        if (usernameFromgroup.Tables[0].Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) usernameFromgroup.Tables[0].Rows)
          {
            ++num;
            this.group_memebers = this.group_memebers + "<p><a href='mailto:" + row["email"].ToString() + "'>" + row["full_name"].ToString() + "</a></p>";
            if (num % 2 == 0)
              this.group_memebers += Environment.NewLine;
          }
          this.group_memebers = this.group_memebers.Trim().TrimEnd(',');
          this.owner_group.Visible = true;
        }
      }
      Dictionary<long, asset_property> assetProperties = asset.asset_properties;
      string str1 = "0";
      string str2 = "0";
      foreach (long key in assetProperties.Keys)
      {
        if (assetProperties[key].property_name == "facility_image")
          str1 = assetProperties[key].property_value;
        if (assetProperties[key].property_name == "layout_image")
          str2 = assetProperties[key].property_value;
      }
      if (str2 != "0")
        this.imgLayout.ImageUrl = this.site_full_path + "handlers/show_image.ashx?doc_id=" + str2;
      if (str1 != "0")
        this.imagfacility.ImageUrl = this.site_full_path + "handlers/show_image.ashx?doc_id=" + str1;
      try
      {
        DataSet dataSet2 = (DataSet) this.Session["favourites"];
        if (dataSet2.Tables[0].Rows.Count > 0)
        {
          DataRow[] dataRowArray = dataSet2.Tables[0].Select("resource_id='" + (object) asset.asset_id + "'");
          this.hdn_fav_id.Value = dataRowArray.Length <= 0 ? "0" : dataRowArray[0]["favourite_asset_id"].ToString();
          if (this.hdn_fav_id.Value != "0")
            this.chk_fav.Checked = true;
        }
        else
          this.hdn_fav_id.Value = "0";
      }
      catch
      {
      }
      bool flag1 = false;
      try
      {
        foreach (asset_property assetProperty in assetProperties.Values)
        {
          if (assetProperty.property_name == "operating_hours")
          {
            flag1 = true;
            string[] strArray = assetProperty.property_value.Split('|');
            this.op_hours = Convert.ToDateTime(strArray[0]).ToString("hh:mm tt") + " - " + Convert.ToDateTime(strArray[1]).ToString("hh:mm tt");
          }
        }
        if (!flag1)
        {
          DataRow[] dataRowArray = dataSet1.Tables[0].Select("parameter='operating_hours'");
          if (dataRowArray.Length > 0)
          {
            string[] strArray = dataRowArray[0]["value"].ToString().Split('|');
            this.op_hours = Convert.ToDateTime(strArray[0]).ToString("hh:mm tt") + " - " + Convert.ToDateTime(strArray[1]).ToString("hh:mm tt");
          }
        }
      }
      catch
      {
      }
      try
      {
        bool flag2 = false;
        foreach (asset_property assetProperty in assetProperties.Values)
        {
          if (assetProperty.property_name == "book_weekend")
          {
            flag2 = true;
            this.book_weekend = assetProperty.property_value;
          }
          if (assetProperty.property_name == "booking_lead_time")
            this.booking_lead_time = assetProperty.property_value;
          if (assetProperty.property_name == "cancel_window_before")
            this.cancel_before_window = assetProperty.property_value;
          if (assetProperty.property_name == "cancel_window_after")
            this.cancel_after_window = assetProperty.property_value;
          if (assetProperty.property_name == "advance_booking_window")
            this.advance_booking_window = assetProperty.property_value + " months. (Book up to " + this.current_timestamp.AddMonths(Convert.ToInt32(assetProperty.property_value)).ToString(api_constants.display_datetime_format_short) + ")";
        }
        if (!flag2)
        {
          DataRow[] dataRowArray = dataSet1.Tables[0].Select("parameter='book_weekend'");
          if (dataRowArray.Length > 0)
            this.book_weekend = dataRowArray[0]["value"].ToString();
        }
        this.book_weekend = !Convert.ToBoolean(this.book_weekend) ? "No" : "Yes";
      }
      catch
      {
      }
      try
      {
        bool flag3 = false;
        foreach (asset_property assetProperty in assetProperties.Values)
        {
          if (assetProperty.property_name == "book_holiday")
          {
            flag3 = true;
            this.book_holiday = assetProperty.property_value;
          }
        }
        if (!flag3)
        {
          DataRow[] dataRowArray = dataSet1.Tables[0].Select("parameter='book_holiday'");
          if (dataRowArray.Length > 0)
            this.book_holiday = dataRowArray[0]["value"].ToString();
        }
        if (Convert.ToBoolean(this.book_holiday))
          this.book_holiday = "Yes";
        else
          this.book_holiday = "No";
      }
      catch
      {
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_asset_properties(Dictionary<long, asset_property> assPros, asset objAsset)
  {
    try
    {
      if (assPros.Values.Count <= 0)
        return;
      string str1 = "<table width='100%' class='table table-striped table-bordered table-hover' id='list_table'>";
      DataSet settings = this.settings.get_settings(this.current_user.account_id);
      string str2 = str1 + "<thead>" + "<tr>" + "<th class='hidden-480'>Property Name</th>" + "<th class='hidden-480'>Status</th>" + "<th class='hidden-480'>Comments</th>" + "</tr>" + "</thead>" + "<tbody>";
      foreach (asset_property assetProperty in assPros.Values)
      {
        if (assetProperty.property_name == "asset_property" && assetProperty.property_value != "")
        {
          DataRow[] dataRowArray = settings.Tables[0].Select("setting_id=" + assetProperty.property_value + "  and parameter='asset_property'");
          str2 += "<tr class='odd gradeX'>";
          if (assetProperty.status == (short) 0 || assetProperty.status == (short) 1 && !assetProperty.available)
          {
            str2 = str2 + "<td>" + dataRowArray[0]["value"].ToString() + "<img id='img_prop' style='float:right;' src='assets/img/Facilityerro.png' alt='Faulty Room' /></td>";
            str2 += "<td><span class='label label-Pending'>Not Available</span></td>";
          }
          else
          {
            str2 = str2 + "<td>" + dataRowArray[0]["value"].ToString() + "</td>";
            str2 += "<td><span class='label label-Available'>Available</span></td>";
          }
          str2 = str2 + "<td>" + assetProperty.remarks + "</td>";
          str2 += "</tr>";
        }
      }
      this.asset_properties = str2 + "</tbody>" + "</table>";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  protected void chk_fav_CheckedChanged(object sender, EventArgs e)
  {
    asset_favourite assetFavourite1 = new asset_favourite();
    assetFavourite1.account_id = this.current_user.account_id;
    assetFavourite1.created_by = this.current_user.user_id;
    assetFavourite1.favourite_asset_id = !(this.hdn_fav_id.Value != "") ? 0L : Convert.ToInt64(this.hdn_fav_id.Value);
    assetFavourite1.module_name = "asset";
    assetFavourite1.resource_id = Convert.ToInt64(this.hdn_asset_id.Value);
    assetFavourite1.user_id = this.current_user.user_id;
    asset_favourite assetFavourite2 = this.assets.update_assets_favourite(assetFavourite1);
    if (assetFavourite2.favourite_asset_id > 0L)
      this.Session.Add("favourites", (object) this.assets.get_favourite_assets(assetFavourite2.account_id, assetFavourite2.user_id));
    this.populate_data(assetFavourite2.resource_id);
  }
}
