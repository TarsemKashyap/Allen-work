// Decompiled with JetBrains decompiler
// Type: administration_panels_panel
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
using System.Web.UI.WebControls;

public class administration_panels_panel : fbs_base_page, IRequiresSessionState
{
  private device_admin_api dapi;
  protected TextBox txt_device_name;
  protected Label lbl_code;
  protected DropDownList ddl_app_config_id;
  protected DropDownList ddl_rooms;
  protected TextBox txt_screen_refresh_rate;
  protected DropDownList ddl_time_format;
  protected TextBox txt_available_text;
  protected TextBox txt_display_style;
  protected TextBox txt_screen_width;
  protected TextBox txt_screen_height;
  protected TextBox txt_available_bg;
  protected TextBox txt_start_bg;
  protected TextBox txt_occupied_bg;
  protected TextBox txt_meeting_list;
  protected DropDownList DropDownList3;
  protected CheckBox chk_active;
  protected DropDownList ddl_enable_led;
  protected HiddenField hdn_did;
  protected DropDownList ddl_allow_login;
  protected TextBox txt_pin_length;
  protected DropDownList ddl_allow_book;
  protected DropDownList ddl_allow_cancel;
  protected DropDownList ddl_allow_extend;
  protected DropDownList ddl_allow_noshow;
  protected TextBox txt_noshow_after;
  protected DropDownList ddl_allow_start;
  protected DropDownList ddl_allow_end;
  protected Panel pnl_interactive;
  protected TextBox txt_available_led_color;
  protected TextBox txt_start_led_color;
  protected TextBox txt_occupied_led_color;
  protected Panel pnl_led;
  protected Button btn_save;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.dapi = new device_admin_api();
      if (this.IsPostBack)
        return;
      long int64 = Convert.ToInt64(this.Request.QueryString["did"]);
      this.hdn_did.Value = int64.ToString();
      foreach (DataRow row in (InternalDataCollectionBase) this.assets.get_assets(this.current_user.account_id).Tables[0].Rows)
        this.ddl_rooms.Items.Add(new ListItem(row["name"].ToString(), row["asset_id"].ToString()));
      DataSet device = this.dapi.get_device(int64, this.current_user.account_id);
      DataSet dataSet = new DataSet();
      DataSet setting_data = int64 <= 0L ? this.dapi.get_device_properties(-1L, this.current_user.account_id) : this.dapi.get_device_properties(int64, this.current_user.account_id);
      this.populate_fields(device, setting_data);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  private void populate_fields(DataSet data, DataSet setting_data)
  {
    try
    {
      bool flag = true;
      DataRow dataRow = (DataRow) null;
      if (data.Tables[0].Rows.Count > 0)
      {
        dataRow = data.Tables[0].Rows[0];
        flag = false;
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (DataRow row in (InternalDataCollectionBase) setting_data.Tables[0].Rows)
        dictionary.Add(row["parameter"].ToString(), row["value"].ToString());
      if (!flag)
      {
        if (dataRow.Table.Columns.Contains("device_name"))
          this.txt_device_name.Text = dataRow["device_name"].ToString();
        if (dataRow.Table.Columns.Contains("device_code"))
          this.lbl_code.Text = dataRow["device_code"].ToString();
        foreach (ListItem listItem in this.ddl_app_config_id.Items)
        {
          if (dataRow.Table.Columns.Contains("app_config_id") && listItem.Value == dataRow["app_config_id"].ToString())
          {
            listItem.Selected = true;
            break;
          }
        }
        foreach (ListItem listItem in this.ddl_rooms.Items)
        {
          if (dataRow.Table.Columns.Contains("asset_id") && listItem.Value == dataRow["asset_id"].ToString())
          {
            listItem.Selected = true;
            break;
          }
        }
        if (dataRow.Table.Columns.Contains("screen_width"))
          this.txt_screen_width.Text = dataRow["screen_width"].ToString();
        if (dataRow.Table.Columns.Contains("screen_height"))
          this.txt_screen_height.Text = dataRow["screen_height"].ToString();
        if (dataRow.Table.Columns.Contains("status") && dataRow["status"].ToString() == "1")
          this.chk_active.Checked = true;
      }
      else
        this.lbl_code.Text = Guid.NewGuid().ToString();
      if (dictionary.Count <= 0)
        return;
      if (dictionary.ContainsKey("screen_refresh_rate"))
        this.txt_screen_refresh_rate.Text = dictionary["screen_refresh_rate"];
      foreach (ListItem listItem in this.ddl_time_format.Items)
      {
        if (listItem.Value == dictionary["time_format"])
        {
          listItem.Selected = true;
          break;
        }
      }
      this.txt_available_text.Text = dictionary["available_text"];
      this.txt_display_style.Text = dictionary["display_style"];
      this.txt_available_bg.Text = dictionary["available_bg"];
      this.txt_start_bg.Text = dictionary["start_bg"];
      this.txt_occupied_bg.Text = dictionary["occupied_bg"];
      this.txt_meeting_list.Text = dictionary["meeting_list"];
      foreach (ListItem listItem in this.ddl_enable_led.Items)
      {
        if (listItem.Value == dictionary["led"])
        {
          listItem.Selected = true;
          this.pnl_led.Visible = true;
          break;
        }
      }
      foreach (ListItem listItem in this.DropDownList3.Items)
      {
        if (listItem.Value == dictionary["allow_login"])
        {
          listItem.Selected = true;
          this.pnl_interactive.Visible = true;
          break;
        }
      }
      foreach (ListItem listItem in this.ddl_allow_login.Items)
      {
        if (listItem.Value == dictionary["allow_login"])
        {
          listItem.Selected = true;
          break;
        }
      }
      this.txt_pin_length.Text = dictionary["pin_length"];
      foreach (ListItem listItem in this.ddl_allow_login.Items)
      {
        if (listItem.Value == dictionary["allow_login"])
        {
          listItem.Selected = true;
          break;
        }
      }
      foreach (ListItem listItem in this.ddl_allow_book.Items)
      {
        if (listItem.Value == dictionary["allow_book"])
        {
          listItem.Selected = true;
          break;
        }
      }
      foreach (ListItem listItem in this.ddl_allow_cancel.Items)
      {
        if (listItem.Value == dictionary["allow_cancel"])
        {
          listItem.Selected = true;
          break;
        }
      }
      foreach (ListItem listItem in this.ddl_allow_extend.Items)
      {
        if (listItem.Value == dictionary["allow_extend"])
        {
          listItem.Selected = true;
          break;
        }
      }
      foreach (ListItem listItem in this.ddl_allow_noshow.Items)
      {
        if (listItem.Value == dictionary["allow_noshow"])
        {
          listItem.Selected = true;
          break;
        }
      }
      this.txt_noshow_after.Text = dictionary["noshow_start_after"];
      foreach (ListItem listItem in this.ddl_allow_start.Items)
      {
        if (listItem.Value == dictionary["allow_start"])
        {
          listItem.Selected = true;
          break;
        }
      }
      foreach (ListItem listItem in this.ddl_allow_end.Items)
      {
        if (listItem.Value == dictionary["allow_end"])
        {
          listItem.Selected = true;
          break;
        }
      }
      this.txt_available_led_color.Text = dictionary["available_led"];
      this.txt_start_led_color.Text = dictionary["start_led"];
      this.txt_occupied_led_color.Text = dictionary["occupied_led"];
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      if (this.DropDownList3.SelectedItem.Value == "1")
        this.pnl_interactive.Visible = true;
      else
        this.pnl_interactive.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void ddl_enable_led_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      if (this.ddl_enable_led.SelectedItem.Value == "1")
        this.pnl_led.Visible = true;
      else
        this.pnl_led.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void btn_save_Click(object sender, EventArgs e)
  {
    try
    {
      device device1 = new device();
      long int64 = Convert.ToInt64(this.hdn_did.Value);
      device1.account_id = this.current_user.account_id;
      device1.app_config_id = Convert.ToInt64(this.ddl_app_config_id.SelectedItem.Value);
      device1.asset_id = Convert.ToInt64(this.ddl_rooms.SelectedItem.Value);
      device1.created_by = this.current_user.user_id;
      device1.created_on = DateTime.UtcNow;
      device1.device_code = new Guid(this.lbl_code.Text);
      device1.device_id = int64;
      device1.device_name = this.txt_device_name.Text;
      device1.mac_address = "";
      device1.modified_by = this.current_user.user_id;
      device1.modified_on = DateTime.UtcNow;
      device1.record_id = Guid.NewGuid();
      device1.screen_height = Convert.ToInt32(this.txt_screen_height.Text);
      device1.screen_width = Convert.ToInt32(this.txt_screen_width.Text);
      device1.serial_no = "";
      device1.status = !this.chk_active.Checked ? 0 : 1;
      device device2 = this.dapi.update_device(device1);
      if (device2.device_id <= 0L)
        return;
      List<device_setting> deviceSettingList = new List<device_setting>();
      foreach (device_setting deviceSetting in int64 <= 0L ? this.dapi.get_device_properties(-1L) : this.dapi.get_device_properties(device2.device_id))
      {
        deviceSetting.device_id = device2.device_id;
        switch (deviceSetting.parameter)
        {
          case "screen_refresh_rate":
            deviceSetting.value = this.txt_screen_refresh_rate.Text;
            break;
          case "time_format":
            deviceSetting.value = this.ddl_time_format.SelectedItem.Value;
            break;
          case "allow_book":
            deviceSetting.value = this.ddl_allow_book.SelectedItem.Value;
            break;
          case "allow_extend":
            deviceSetting.value = this.ddl_allow_extend.SelectedItem.Value;
            break;
          case "allow_cancel":
            deviceSetting.value = this.ddl_allow_cancel.SelectedItem.Value;
            break;
          case "allow_start":
            deviceSetting.value = this.ddl_allow_start.SelectedItem.Value;
            break;
          case "allow_end":
            deviceSetting.value = this.ddl_allow_end.SelectedItem.Value;
            break;
          case "allow_noshow":
            deviceSetting.value = this.ddl_allow_noshow.SelectedItem.Value;
            break;
          case "allow_fault":
            deviceSetting.value = "0";
            break;
          case "noshow_start_after":
            deviceSetting.value = this.txt_noshow_after.Text;
            break;
          case "allow_login":
            deviceSetting.value = this.ddl_allow_login.SelectedItem.Value;
            break;
          case "inactive_login_duration":
            deviceSetting.value = "30";
            break;
          case "device_start_time":
            deviceSetting.value = "0";
            break;
          case "device_end_time":
            deviceSetting.value = "24";
            break;
          case "available_bg":
            deviceSetting.value = this.txt_available_bg.Text;
            break;
          case "start_bg":
            deviceSetting.value = this.txt_start_bg.Text;
            break;
          case "occupied_bg":
            deviceSetting.value = this.txt_occupied_bg.Text;
            break;
          case "led":
            deviceSetting.value = this.ddl_enable_led.SelectedItem.Value;
            break;
          case "available_led":
            deviceSetting.value = this.txt_available_led_color.Text;
            break;
          case "start_led":
            deviceSetting.value = this.txt_start_led_color.Text;
            break;
          case "occupied_led":
            deviceSetting.value = this.txt_occupied_led_color.Text;
            break;
          case "pin_length":
            deviceSetting.value = this.txt_pin_length.Text;
            break;
          case "display_style":
            deviceSetting.value = this.txt_display_style.Text;
            break;
        }
        this.dapi.update_device_setting(deviceSetting);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
