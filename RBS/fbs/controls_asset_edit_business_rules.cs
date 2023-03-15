// Decompiled with JetBrains decompiler
// Type: controls_asset_edit_business_rules
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

public class controls_asset_edit_business_rules : fbs_base_user_control
{
  public string assest_id = "";
  public string setting_ID = "";
  public asset _objAsset;
  public string Staus = "";
  public DataSet _objSetting;
  protected TextBox txt_cannotbookbeyond;
  protected TextBox txt_cancel_before;
  protected TextBox txt_cancel_after;
  protected DropDownList txt_approval_period;
  protected CheckBox chk_housekeeping;
  protected HiddenField hdn_housekeeping_id;
  protected CheckBox chk_allow_setup;
  protected ListBox lst_setup_types;
  protected CheckBox chk_agree_terms_and_conditions;
  protected HiddenField hdn_terms_id;
  protected HtmlGenericControl hrs_error;
  protected DropDownList ddl_start_op_hours;
  protected DropDownList ddl_end_op_hours;
  protected HtmlGenericControl div_operating_hours;
  protected CheckBox chk_book_ph;
  protected HiddenField HiddenField1;
  protected CheckBox chk_book_weekend;
  protected HiddenField HiddenField3;
  protected TextBox txt_lead_time;
  protected DropDownList ddl_lead_time;
  protected Button Button1;
  protected Button btn_cancel;

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
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.Session["Status_businessrules"] != null && this.Session["Status_businessrules"] == (object) "S")
      {
        this.Staus = "S";
        this.Session.Remove("Status_businessrules");
      }
      if (this.IsPostBack)
        return;
      this.loadSetup();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void BindSetupTypeData()
  {
    try
    {
      DataRow[] dataRowArray = this.settings.get_settings(this.current_user.account_id).Tables[0].Select("parameter='setup_type'");
      if (dataRowArray.Length <= 0)
        return;
      for (int index = 0; index <= dataRowArray.Length - 1; ++index)
        this.lst_setup_types.Items.Add(dataRowArray[index]["value"].ToString());
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private string get_setting_value(DataSet data, string param)
  {
    DataRow[] dataRowArray = data.Tables[0].Select("parameter='" + param + "'");
    return dataRowArray.Length > 0 ? dataRowArray[0]["value"].ToString() : "";
  }

  private string get_property_value(DataSet data, string param)
  {
    DataRow[] dataRowArray = data.Tables[0].Select("property_name='" + param + "'");
    return dataRowArray.Length > 0 ? dataRowArray[0]["property_value"].ToString() : "";
  }

  public void loadSetup()
  {
    try
    {
      long int64 = Convert.ToInt64(this.Attributes["asset_id"]);
      if (int64 == 0L)
        return;
      DataSet settings = this.settings.get_settings(this.current_user.account_id);
      DataRow[] dataRowArray1 = settings.Tables[0].Select("parameter='setup_type'");
      DataSet assetProperties = this.assets.get_asset_properties(int64, this.current_user.account_id);
      string propertyValue1 = this.get_property_value(assetProperties, "book_weekend");
      this.chk_book_weekend.Checked = !(propertyValue1 != "") ? Convert.ToBoolean(this.get_setting_value(settings, "book_weekend")) : Convert.ToBoolean(propertyValue1);
      string propertyValue2 = this.get_property_value(assetProperties, "book_holiday");
      this.chk_book_ph.Checked = !(propertyValue2 != "") ? Convert.ToBoolean(this.get_setting_value(settings, "book_holiday")) : Convert.ToBoolean(propertyValue2);
      string propertyValue3 = this.get_property_value(assetProperties, "cancel_window_before");
      this.txt_cancel_before.Text = !(propertyValue3 != "") ? this.get_setting_value(settings, "cancel_before") : propertyValue3;
      string propertyValue4 = this.get_property_value(assetProperties, "cancel_window_after");
      this.txt_cancel_after.Text = !(propertyValue4 != "") ? this.get_setting_value(settings, "cancel_after") : propertyValue4;
      string propertyValue5 = this.get_property_value(assetProperties, "advance_booking_window");
      this.txt_cannotbookbeyond.Text = !(propertyValue5 != "") ? this.get_setting_value(settings, "advance_booking_window") : propertyValue5;
      string propertyValue6 = this.get_property_value(assetProperties, "default_approval_period");
      if (propertyValue6 != "")
        this.txt_approval_period.Text = propertyValue6;
      else
        this.txt_approval_period.Text = this.get_setting_value(settings, "default_approval_period");
      string propertyValue7 = this.get_property_value(assetProperties, "show_terms_and_conditions");
      this.chk_agree_terms_and_conditions.Checked = !(propertyValue7 != "") ? Convert.ToBoolean(this.get_setting_value(settings, "show_terms_and_conditions")) : Convert.ToBoolean(propertyValue7);
      string propertyValue8 = this.get_property_value(assetProperties, "allow_housekeeping");
      this.chk_housekeeping.Checked = !(propertyValue8 != "") ? Convert.ToBoolean(this.get_setting_value(settings, "allow_housekeeping")) : Convert.ToBoolean(propertyValue8);
      string propertyValue9 = this.get_property_value(assetProperties, "allow_setup");
      this.chk_allow_setup.Checked = !(propertyValue9 != "") ? Convert.ToBoolean(this.get_setting_value(settings, "allow_setup")) : Convert.ToBoolean(propertyValue9);
      DataRow[] dataRowArray2 = assetProperties.Tables[0].Select("property_name='setup_type'");
      if (dataRowArray2.Length > 0)
      {
        for (int index1 = 0; index1 <= dataRowArray1.Length - 1; ++index1)
        {
          bool flag = false;
          for (int index2 = 0; index2 <= dataRowArray2.Length - 1; ++index2)
          {
            if (dataRowArray1[index1]["Setting_id"].ToString() == dataRowArray2[index2]["Property_value"].ToString())
            {
              this.chk_allow_setup.Checked = true;
              this.lst_setup_types.Items.Add(new ListItem()
              {
                Text = dataRowArray1[index1]["value"].ToString(),
                Value = dataRowArray1[index1]["Setting_id"].ToString(),
                Selected = true
              });
              flag = true;
              break;
            }
          }
          if (!flag)
            this.lst_setup_types.Items.Add(new ListItem()
            {
              Text = dataRowArray1[index1]["value"].ToString(),
              Value = dataRowArray1[index1]["Setting_id"].ToString()
            });
        }
      }
      else if (dataRowArray1.Length > 0)
      {
        for (int index = 0; index <= dataRowArray1.Length - 1; ++index)
          this.lst_setup_types.Items.Add(dataRowArray1[index]["value"].ToString());
      }
      this.div_operating_hours.Visible = Convert.ToBoolean(this.current_account.properties["operating_hours"]);
      if (this.div_operating_hours.Visible)
      {
        this.Populate_Time(this.ddl_start_op_hours);
        this.Populate_Time(this.ddl_end_op_hours);
        DataRow[] dataRowArray3 = assetProperties.Tables[0].Select("property_name='operating_hours'");
        if (dataRowArray3.Length > 0)
        {
          try
          {
            string[] strArray = dataRowArray3[0]["property_value"].ToString().Split(new string[1]
            {
              "|"
            }, StringSplitOptions.None);
            foreach (ListItem listItem in this.ddl_start_op_hours.Items)
            {
              if (listItem.Value == strArray[0])
                listItem.Selected = true;
            }
            foreach (ListItem listItem in this.ddl_end_op_hours.Items)
            {
              if (listItem.Value == strArray[1])
                listItem.Selected = true;
            }
          }
          catch
          {
          }
        }
        else
        {
          DataRow[] dataRowArray4 = settings.Tables[0].Select("parameter='operating_hours'");
          if (dataRowArray4.Length > 0)
          {
            try
            {
              string[] strArray = dataRowArray4[0]["value"].ToString().Split(new string[1]
              {
                "|"
              }, StringSplitOptions.None);
              foreach (ListItem listItem in this.ddl_start_op_hours.Items)
              {
                if (listItem.Value == strArray[0])
                {
                  listItem.Selected = true;
                  break;
                }
              }
              foreach (ListItem listItem in this.ddl_end_op_hours.Items)
              {
                if (listItem.Value == strArray[1])
                {
                  listItem.Selected = true;
                  break;
                }
              }
            }
            catch
            {
            }
          }
        }
      }
      try
      {
        string propertyValue10 = this.get_property_value(assetProperties, "booking_lead_time");
        if (propertyValue10 != "")
        {
          this.txt_lead_time.Text = propertyValue10.Split(' ')[0];
          this.ddl_lead_time.Items.FindByValue(propertyValue10.Split(' ')[1]).Selected = true;
        }
        else
          this.txt_lead_time.Text = "0";
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      this.ViewState.Add("asset_prop", (object) assetProperties);
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

  private void HandleTranscation()
  {
    try
    {
      string str = "";
      setting setting = new setting();
      long int64 = Convert.ToInt64(this.Attributes["asset_id"]);
      DataSet settings = this.settings.get_settings(this.current_user.account_id);
      asset_property assetProperty1 = new asset_property();
      asset asset = new asset();
      asset.asset_id = Convert.ToInt64(int64);
      if (asset.asset_id > 0L)
        asset = this.assets.get_asset(Convert.ToInt64(int64), this.current_user.account_id);
      else
        asset.record_id = Guid.NewGuid();
      DataSet dataSet = (DataSet) this.ViewState["asset_prop"] ?? this.assets.get_asset_properties(int64, this.current_user.account_id);
      bool flag1;
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='advance_booking_window'");
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.status = (short) 1;
        assetProperty1.property_value = this.txt_cannotbookbeyond.Text;
        assetProperty1.property_name = "advance_booking_window";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='cancel_window_before'");
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.property_value = this.txt_cancel_before.Text;
        assetProperty1.status = (short) 1;
        assetProperty1.property_name = "cancel_window_before";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='cancel_window_after'");
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.status = (short) 1;
        assetProperty1.property_value = this.txt_cancel_after.Text;
        assetProperty1.property_name = "cancel_window_after";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='book_holiday'");
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.status = (short) 1;
        assetProperty1.property_value = this.chk_book_ph.Checked.ToString();
        assetProperty1.property_name = "book_holiday";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='book_weekend'");
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.status = (short) 1;
        assetProperty1.property_value = this.chk_book_weekend.Checked.ToString();
        assetProperty1.property_name = "book_weekend";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        assetProperty1 = new asset_property();
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='allow_housekeeping'");
        assetProperty1 = new asset_property();
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.property_value = this.chk_housekeeping.Checked.ToString();
        assetProperty1.status = (short) 1;
        assetProperty1.property_name = "allow_housekeeping";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        assetProperty1 = new asset_property();
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='show_terms_and_conditions'");
        assetProperty1 = new asset_property();
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.property_value = this.chk_agree_terms_and_conditions.Checked.ToString();
        assetProperty1.status = (short) 1;
        assetProperty1.property_name = "show_terms_and_conditions";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        asset_property assetProperty2 = new asset_property();
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='default_approval_period'");
        assetProperty2.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty2.property_value = this.txt_approval_period.Text;
        assetProperty2.status = (short) 1;
        assetProperty2.property_name = "default_approval_period";
        assetProperty2.created_by = this.current_user.user_id;
        assetProperty2.record_id = asset.record_id;
        assetProperty2.account_id = this.current_user.account_id;
        assetProperty2.modified_by = this.current_user.user_id;
        assetProperty2.asset_id = int64;
        assetProperty2.remarks = "";
        this.assets.update_asset_property(assetProperty2);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='allow_setup'");
        assetProperty1 = new asset_property();
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.property_value = this.chk_allow_setup.Checked.ToString();
        assetProperty1.status = (short) 1;
        assetProperty1.property_name = "allow_setup";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        assetProperty1 = this.assets.update_asset_property(assetProperty1);
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        if (this.chk_allow_setup.Checked)
        {
          List<string> stringList = new List<string>();
          foreach (ListItem listItem in this.lst_setup_types.Items)
          {
            if (listItem.Selected)
              stringList.Add(listItem.Value);
          }
          foreach (ListItem listItem in this.lst_setup_types.Items)
          {
            asset_property assetProperty3 = new asset_property();
            DataRow[] dataRowArray1 = settings.Tables[0].Select("parameter='setup_type' and value='" + listItem.Text + "'");
            DataRow[] dataRowArray2 = dataSet.Tables[0].Select("property_value='" + str + "' and property_name='setup_type'");
            assetProperty3.asset_property_id = dataRowArray2.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray2[0]["asset_property_id"].ToString());
            assetProperty3.property_value = str;
            assetProperty3.status = Convert.ToInt16(dataRowArray1[0]["status"].ToString());
            assetProperty3.property_name = "setup_type";
            assetProperty3.created_by = this.current_user.user_id;
            assetProperty3.created_on = this.current_timestamp;
            assetProperty3.record_id = asset.record_id;
            assetProperty3.account_id = this.current_user.account_id;
            assetProperty3.modified_by = this.current_user.user_id;
            assetProperty3.modified_on = this.current_timestamp;
            assetProperty3.asset_id = int64;
            assetProperty3.remarks = "";
            this.assets.delete_asset_property(assetProperty3);
            dataSet.Tables[0].Select("property_name='allow_setup'");
            flag1 = true;
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      try
      {
        if (this.validate_operating_hours() == "")
        {
          asset_property assetProperty4 = new asset_property();
          DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='operating_hours'");
          if (dataRowArray.Length > 0)
          {
            assetProperty4.asset_property_id = Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
            assetProperty4.status = Convert.ToInt16(dataRowArray[0]["status"].ToString());
          }
          else
          {
            assetProperty4.asset_property_id = 0L;
            assetProperty4.status = (short) 1;
          }
          assetProperty4.property_value = this.ddl_start_op_hours.SelectedItem.Value + "|" + this.ddl_end_op_hours.SelectedItem.Value;
          assetProperty4.property_name = "operating_hours";
          assetProperty4.created_by = this.current_user.user_id;
          assetProperty4.created_on = this.current_timestamp;
          assetProperty4.record_id = asset.record_id;
          assetProperty4.account_id = this.current_user.account_id;
          assetProperty4.modified_by = this.current_user.user_id;
          assetProperty4.modified_on = this.current_timestamp;
          assetProperty4.asset_id = int64;
          assetProperty4.remarks = "";
          this.assets.update_asset_property(assetProperty4);
          flag1 = true;
        }
        else
        {
          this.hrs_error.InnerText = Resources.fbs.strat_end_time_compare;
          return;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag1 = false;
      }
      bool flag2;
      try
      {
        DataRow[] dataRowArray = dataSet.Tables[0].Select("property_name='booking_lead_time'");
        assetProperty1.asset_property_id = dataRowArray.Length <= 0 ? 0L : Convert.ToInt64(dataRowArray[0]["asset_property_id"].ToString());
        assetProperty1.status = (short) 1;
        assetProperty1.property_value = this.txt_lead_time.Text + " " + this.ddl_lead_time.SelectedItem.Value;
        assetProperty1.property_name = "booking_lead_time";
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.record_id = asset.record_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.asset_id = int64;
        assetProperty1.remarks = "";
        this.assets.update_asset_property(assetProperty1);
        flag2 = true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        flag2 = false;
      }
      if (!flag2)
        return;
      this.Session["Status_businessrules"] = (object) "S";
      this.Session.Add("current_tab", (object) new List<string>()
      {
        "Business Rules",
        "tab_edit_business_rules"
      });
      this.Response.Redirect("~/administration/asset_form.aspx?asset_id=" + (object) asset.asset_id + "#tab_edit_business_rules");
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private string validate_operating_hours()
  {
    try
    {
      DateTime dateTime = Convert.ToDateTime(this.ddl_start_op_hours.SelectedItem.Value);
      if (Convert.ToDateTime(this.ddl_end_op_hours.SelectedItem.Value) <= dateTime)
        return Resources.fbs.strat_end_time_compare;
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
    return "";
  }

  protected void btn_submit_Click1(object sender, EventArgs e)
  {
    try
    {
      this.HandleTranscation();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  public void Populate_Time(DropDownList cbo)
  {
    try
    {
      cbo.Items.Clear();
      DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
      for (int index = 0; index <= 95; ++index)
      {
        cbo.Items.Add(new ListItem(dateTime.ToString("hh:mm tt"), dateTime.ToString("yyyy-MM-dd HH:mm:ss")));
        dateTime = dateTime.AddMinutes(this.AllowedMinutes);
      }
      cbo.Items.Add(new ListItem("12:00 AM (+1 day)", dateTime.AddSeconds(-1.0).ToString("yyyy-MM-dd HH:mm:ss")));
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
