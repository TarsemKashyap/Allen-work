// Decompiled with JetBrains decompiler
// Type: administration_asset_property_form
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_asset_property_form : fbs_base_page, IRequiresSessionState
{
  public string room_name;
  protected HiddenField hdn_setting;
  protected HiddenField hdn_type;
  protected HiddenField hdn_remarks;
  protected Label lblprop;
  protected CheckBox chkremarks;
  protected RegularExpressionValidator RegularExpressionValidator1;
  protected TextBox txtremark;
  protected CheckBox chkSendEmail;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
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
      string str1 = this.Request.QueryString["asset_id"];
      string str2 = this.Request.QueryString["set_id"];
      this.hdn_setting.Value = str2;
      setting setting = this.settings.get_setting(Convert.ToInt64(str2), this.current_user.account_id);
      DataSet assetProperties = this.assets.get_asset_properties(Convert.ToInt64(str1), this.current_user.account_id);
      this.room_name = this.assets.get_asset(Convert.ToInt64(str1), this.current_user.account_id).name;
      DataRow[] dataRowArray = assetProperties.Tables[0].Select("property_value='" + str2 + "'");
      if (!(setting.value != ""))
        return;
      if (dataRowArray.Length > 0)
      {
        this.txtremark.Text = dataRowArray[0]["remarks"].ToString();
        if (dataRowArray[0]["available"].ToString().ToUpper() == "FALSE")
          this.chkremarks.Checked = true;
      }
      this.lblprop.Text = setting.value;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    asset_property objasst_property1 = new asset_property();
    try
    {
      string str = this.Request.QueryString["asset_id"];
      asset_property objasst_property2 = this.transcation(objasst_property1);
      if (this.chkSendEmail.Checked)
      {
        this.Email_log(objasst_property2);
        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "close_fancybox();", true);
        Modal.Close((Page) this);
      }
      else
      {
        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "close_fancybox();", true);
        Modal.Close((Page) this);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected asset_property transcation(asset_property objasst_property)
  {
    try
    {
      string str1 = this.Request.QueryString["asset_id"];
      string str2 = this.Request.QueryString["set_id"];
      asset asset = new asset();
      asset.asset_id = Convert.ToInt64(str1);
      this.settings.get_setting(Convert.ToInt64(str2), this.current_user.account_id);
      DataRow[] dataRowArray = this.assets.get_asset_properties(Convert.ToInt64(str1), this.current_user.account_id).Tables[0].Select("property_value='" + str2 + "'");
      if (dataRowArray.Length > 0)
        objasst_property.asset_property_id = Convert.ToInt64(dataRowArray[0]["asset_property_id"]);
      if (asset.asset_id > 0L)
        asset = this.assets.get_asset(asset.asset_id, this.current_user.account_id);
      else
        asset.record_id = Guid.NewGuid();
      setting setting = this.settings.get_setting(Convert.ToInt64(str2), this.current_user.account_id);
      objasst_property.property_value = str2;
      objasst_property.status = Convert.ToInt16(setting.status);
      objasst_property.created_by = this.current_user.user_id;
      objasst_property.created_on = this.current_timestamp;
      objasst_property.record_id = asset.record_id;
      objasst_property.account_id = this.current_user.account_id;
      objasst_property.modified_by = this.current_user.user_id;
      objasst_property.modified_on = this.current_timestamp;
      if (this.chkremarks.Checked)
      {
        objasst_property.property_name = setting.parameter;
        objasst_property.remarks = this.txtremark.Text;
        objasst_property.available = false;
        this.hdn_type.Value = "Yes";
        this.hdn_remarks.Value = this.txtremark.Text;
      }
      else
      {
        objasst_property.property_name = setting.parameter;
        objasst_property.remarks = "";
        objasst_property.available = true;
        this.hdn_type.Value = "No";
        this.hdn_remarks.Value = "";
      }
      objasst_property.asset_id = asset.asset_id;
      objasst_property = this.assets.update_asset_property(objasst_property);
      if (objasst_property.asset_id == 0L)
        this.assets.delete_asset_property(objasst_property);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    return objasst_property;
  }

  protected void Email_log(asset_property objasst_property)
  {
    email email = new email();
    asset asset = new asset();
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      string str1 = this.Request.QueryString["asset_id"];
      string str2 = this.Request.QueryString["set_id"];
      this.settings.get_setting(Convert.ToInt64(str2), this.current_user.account_id);
      DataSet assetProperties = this.assets.get_asset_properties(Convert.ToInt64(str1), this.current_user.account_id);
      DataSet bookingsMail = this.bookings.get_bookings_mail(Convert.ToInt64(str1), this.current_user.account_id);
      this.settings.get_setting("from_email_address", this.current_user.account_id);
      template template = this.tapi.get_template("email_facilities_faulty", this.current_user.account_id);
      assetProperties.Tables[0].Select("property_value='" + str2 + "'");
      this.settings.get_settings(this.current_user.account_id);
      asset.asset_id = Convert.ToInt64(str1);
      if (asset.asset_id > 0L)
        asset = this.assets.get_asset(asset.asset_id, this.current_user.account_id);
      else
        asset.record_id = Guid.NewGuid();
      Dictionary<string, string> items = new Dictionary<string, string>();
      items.Add("[image_path]", this.site_full_path + "assets/img/");
      items.Add("[logo]", this.site_full_path + "assets/img/" + this.current_account.logo);
      items.Add("[company_name]", this.current_account.name);
      items.Add("[copyright]", "");
      items.Add("[footer_text]", "");
      items.Add("[building]", asset.building.value);
      items.Add("[level]", asset.level.value);
      items.Add("[room_name]", asset.name);
      items.Add("[room_code]", asset.code);
      items.Add("[room_description]", asset.description);
      items.Add("[room_category]", asset.category.value);
      items.Add("[room_type]", asset.type.value);
      items.Add("[room_capacity]", asset.capacity.ToString());
      items.Add("[requestor]", this.users.get_user_name(objasst_property.modified_by, objasst_property.account_id));
      items.Add("[email]", this.users.get_user_email(objasst_property.modified_by, objasst_property.account_id));
      string str3 = this.settings.get_setting(objasst_property.property_value, objasst_property.account_id).value + " for this room is faulty. " + objasst_property.remarks + ".";
      items.Add("[remarks]", str3);
      string contentData = template.content_data;
      string body = this.bookingsbl.replace_body(items, contentData);
      foreach (DataRow row in (InternalDataCollectionBase) bookingsMail.Tables[0].Rows)
      {
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
        {
          this.bookingsbl.sendEmail("", body, template.title, "", row["email"].ToString(), asset.record_id);
          stringBuilder.Append(row["email"].ToString() + ";");
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this);

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
