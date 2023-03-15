// Decompiled with JetBrains decompiler
// Type: preferences
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

public class preferences : fbs_base_page, IRequiresSessionState
{
  protected DropDownList ddl_landing;
  protected DropDownList ddl_calendar_filter;
  protected CheckBox chk_past;
  protected Button btn_submit;
  protected DropDownList ddl_rooms;
  protected Button btn_add;
  protected Button Button1;
  public string html_favourites;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    this.delete_fav();
    this.load_preferences();
    this.load_favourites();
  }

  private void delete_fav()
  {
    long num;
    try
    {
      num = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
      num = 0L;
    }
    if (num <= 0L)
      return;
    this.assets.update_assets_favourite(this.assets.get_favourite_asset(this.current_user.account_id, Convert.ToInt64(this.Request.QueryString["id"])));
    this.Session.Add("favourites", (object) this.assets.get_favourite_assets(this.current_user.account_id, this.current_user.user_id));
    this.Response.Redirect("preferences.aspx");
  }

  private void load_favourites()
  {
    List<string> stringList = new List<string>();
    DataSet assets = this.assets.get_assets(this.current_user.account_id);
    DataSet favouriteAssets = this.assets.get_favourite_assets(this.current_user.account_id, this.current_user.user_id);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover'>");
    stringBuilder.Append("<tr><td><b>Room Name</b></td><td><b>Remove</b></td></tr>");
    foreach (DataRow row in (InternalDataCollectionBase) favouriteAssets.Tables[0].Rows)
    {
      stringList.Add(row["resource_id"].ToString());
      DataRow[] dataRowArray = assets.Tables[0].Select("asset_id='" + row["resource_id"] + "'");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + dataRowArray[0]["name"].ToString() + "</td>");
      stringBuilder.Append("<td><a href='javascript:remove_fav(" + row["favourite_asset_id"].ToString() + ");'>Remove</a></td>");
      stringBuilder.Append("</tr>");
    }
    stringBuilder.Append("</table>");
    this.html_favourites = stringBuilder.ToString();
    this.ddl_rooms.Items.Clear();
    foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
    {
      if (row["status"].ToString() == "1" && !stringList.Contains(row["asset_id"].ToString()))
        this.ddl_rooms.Items.Add(new ListItem(row["name"].ToString(), row["asset_id"].ToString()));
    }
  }

  private void load_preferences()
  {
    user_property userProperty1 = new user_property();
    if (this.current_user.properties.ContainsKey("default_landing"))
    {
      user_property property = this.current_user.properties["default_landing"];
      foreach (ListItem listItem in this.ddl_landing.Items)
      {
        if (listItem.Value == property.property_value)
        {
          listItem.Selected = true;
          break;
        }
      }
    }
    user_property userProperty2 = new user_property();
    if (this.current_user.properties.ContainsKey("default_calendar_view"))
    {
      user_property property = this.current_user.properties["default_calendar_view"];
      foreach (ListItem listItem in this.ddl_calendar_filter.Items)
      {
        if (listItem.Value == property.property_value)
        {
          listItem.Selected = true;
          break;
        }
      }
    }
    user_property userProperty3 = new user_property();
    if (!this.current_user.properties.ContainsKey("show_past") || !Convert.ToBoolean(this.current_user.properties["show_past"].property_value))
      return;
    this.chk_past.Checked = true;
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    user_property userProperty1 = new user_property();
    if (this.current_user.properties.ContainsKey("default_landing"))
    {
      userProperty1 = this.current_user.properties["default_landing"];
      userProperty1.property_value = this.ddl_landing.SelectedItem.Value;
    }
    else
    {
      userProperty1.account_id = this.current_user.account_id;
      userProperty1.created_by = this.current_user.user_id;
      userProperty1.created_on = DateTime.UtcNow;
      userProperty1.modified_by = this.current_user.user_id;
      userProperty1.modified_on = DateTime.UtcNow;
      userProperty1.property_name = "default_landing";
      userProperty1.record_id = Guid.NewGuid();
      userProperty1.user_id = this.current_user.user_id;
      userProperty1.property_value = this.ddl_landing.SelectedItem.Value;
      userProperty1.user_property_id = 0L;
    }
    this.users.update_user_property(userProperty1);
    user_property userProperty2 = new user_property();
    if (this.current_user.properties.ContainsKey("default_calendar_view"))
    {
      userProperty2 = this.current_user.properties["default_calendar_view"];
      userProperty2.property_value = this.ddl_calendar_filter.SelectedItem.Value;
    }
    else
    {
      userProperty2.account_id = this.current_user.account_id;
      userProperty2.created_by = this.current_user.user_id;
      userProperty2.created_on = DateTime.UtcNow;
      userProperty2.modified_by = this.current_user.user_id;
      userProperty2.modified_on = DateTime.UtcNow;
      userProperty2.property_name = "default_calendar_view";
      userProperty2.record_id = Guid.NewGuid();
      userProperty2.user_id = this.current_user.user_id;
      userProperty2.property_value = this.ddl_calendar_filter.SelectedItem.Value;
      userProperty2.user_property_id = 0L;
    }
    this.users.update_user_property(userProperty2);
    user_property userProperty3 = new user_property();
    if (this.current_user.properties.ContainsKey("show_past"))
    {
      userProperty3 = this.current_user.properties["show_past"];
      userProperty3.property_value = this.chk_past.Checked.ToString();
    }
    else
    {
      userProperty3.account_id = this.current_user.account_id;
      userProperty3.created_by = this.current_user.user_id;
      userProperty3.created_on = DateTime.UtcNow;
      userProperty3.modified_by = this.current_user.user_id;
      userProperty3.modified_on = DateTime.UtcNow;
      userProperty3.property_name = "show_past";
      userProperty3.record_id = Guid.NewGuid();
      userProperty3.user_id = this.current_user.user_id;
      userProperty3.property_value = this.chk_past.Checked.ToString();
      userProperty3.user_property_id = 0L;
    }
    this.users.update_user_property(userProperty3);
    this.current_user = this.users.get_user(this.current_user.user_id, this.current_user.account_id);
    this.Session["user"] = (object) this.current_user;
    this.Response.Redirect("preferences.aspx");
  }

  protected void btn_add_Click(object sender, EventArgs e)
  {
    asset_favourite assetFavourite1 = new asset_favourite();
    assetFavourite1.account_id = this.current_user.account_id;
    assetFavourite1.created_by = this.current_user.user_id;
    assetFavourite1.favourite_asset_id = 0L;
    assetFavourite1.module_name = "asset";
    assetFavourite1.resource_id = Convert.ToInt64(this.ddl_rooms.SelectedItem.Value);
    assetFavourite1.user_id = this.current_user.user_id;
    asset_favourite assetFavourite2 = this.assets.update_assets_favourite(assetFavourite1);
    if (assetFavourite2.favourite_asset_id > 0L)
      this.Session.Add("favourites", (object) this.assets.get_favourite_assets(assetFavourite2.account_id, assetFavourite2.user_id));
    this.load_favourites();
  }
}
