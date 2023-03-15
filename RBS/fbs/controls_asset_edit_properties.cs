// Decompiled with JetBrains decompiler
// Type: controls_asset_edit_properties
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class controls_asset_edit_properties : fbs_base_user_control
{
  protected HiddenField hdn_id;
  protected Button btn_add_asset_prop;
  protected HtmlGenericControl error;
  public string html_table;
  public string htmlbtn;
  public DataSet _objSetting;
  public string Prop = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.IsPostBack)
        return;
      this.pageload_data();
      if (this.Session["Prop"] == null || this.Session["Prop"] != (object) "S")
        return;
      this.Prop = "S";
      this.Session.Remove("Prop");
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    try
    {
      if (Convert.ToInt64(this.Attributes["asset_id"]) != 0L)
      {
        this.hdn_id.Value = Convert.ToInt64(this.Attributes["asset_id"]).ToString();
        StringBuilder stringBuilder1 = new StringBuilder();
        StringBuilder stringBuilder2 = new StringBuilder();
        stringBuilder1.Append("<table class='table table-striped table-bordered table-hover' id='asseteditProperty_table'>");
        stringBuilder1.Append("<thead>");
        stringBuilder1.Append("<tr>");
        stringBuilder1.Append("<th style='width:5%;'><input type='checkbox'   id='selectall' class='group-checkable' data-set='#list_table .checkboxes' />Select All</th>");
        stringBuilder1.Append("<th style='width:20%;' class='hidden-480'>Feature</th>");
        stringBuilder1.Append("<th style='width:20%;' class='hidden-480'>Remarks</th>");
        stringBuilder1.Append("<th style='width:10%;' class='hidden-480'>Faulty</th>");
        stringBuilder1.Append("<th style='width:5%;' class='hidden-480'>Action</th>");
        stringBuilder1.Append("</tr>");
        stringBuilder1.Append("</thead>");
        stringBuilder1.Append("<tbody>");
        stringBuilder1.Append("</tbody>");
        stringBuilder1.Append("</table>");
        this.html_table = stringBuilder1.ToString();
      }
      else
        this.error.InnerHtml = Resources.fbs.asset_add_Facilities;
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_add_asset_prop_Click(object sender, EventArgs e)
  {
  }

  protected void btn_add_asset_prop_Click1(object sender, EventArgs e)
  {
    string str1 = "";
    try
    {
      string[] strArray = this.Request.Form["chk_propadd"].Split(',');
      for (int index = 0; index <= strArray.Length - 1; ++index)
        this.handle_property_insert(strArray[index].ToString());
      foreach (string str2 in strArray)
        str1 = str1 + str2 + ",";
      DataSet assetPropertyId = this.reportings.get_asset_property_id(this.current_user.account_id, str1.TrimEnd(','));
      if (assetPropertyId.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) assetPropertyId.Tables[0].Rows)
          this.handle_property_delete(row["setting_id"].ToString());
      }
      this.Session.Add("current_tab", (object) new List<string>()
      {
        "Edit Room Equipment",
        "tab_edit_properties"
      });
      this.Session["Prop"] = (object) "S";
      this.Response.Redirect("~/administration/asset_form.aspx?asset_id=" + this.Attributes["asset_id"]);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void handle_property_insert(string property_id)
  {
    try
    {
      long int64 = Convert.ToInt64(this.Request.QueryString["asset_id"].ToString());
      foreach (DataRow row in (InternalDataCollectionBase) this.reportings.get_asset_propertyfor_add(int64, this.current_user.account_id).Tables[0].Rows)
      {
        if (row["setting_id"].ToString() == property_id)
        {
          asset asset = new asset();
          asset_property assetProperty1 = new asset_property();
          asset.asset_id = Convert.ToInt64(int64);
          if (asset.asset_id > 0L)
            asset = this.assets.get_asset(asset.asset_id, this.current_user.account_id);
          else
            asset.record_id = Guid.NewGuid();
          setting setting = this.settings.get_setting(Convert.ToInt64(property_id), this.current_user.account_id);
          assetProperty1.property_value = property_id;
          assetProperty1.status = Convert.ToInt16(setting.status);
          assetProperty1.created_by = this.current_user.user_id;
          assetProperty1.created_on = this.current_timestamp;
          assetProperty1.record_id = asset.record_id;
          assetProperty1.account_id = this.current_user.account_id;
          assetProperty1.modified_by = this.current_user.user_id;
          assetProperty1.modified_on = this.current_timestamp;
          assetProperty1.property_name = setting.parameter;
          assetProperty1.remarks = "";
          assetProperty1.available = true;
          assetProperty1.asset_id = asset.asset_id;
          asset_property assetProperty2 = this.assets.update_asset_property(assetProperty1);
          if (assetProperty2.asset_id == 0L)
            this.assets.delete_asset_property(assetProperty2);
        }
      }
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void handle_property_delete(string property_id)
  {
    try
    {
      this.assets.delete_asset_property(this.assets.get_asset_property(Convert.ToInt64(this.Request.QueryString["asset_id"].ToString()), this.current_user.account_id, property_id, "asset_property"));
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }
}
