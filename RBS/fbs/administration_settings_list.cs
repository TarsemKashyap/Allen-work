// Decompiled with JetBrains decompiler
// Type: administration_settings_list
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public class administration_settings_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public DataSet data;
  public List<string> globalSettings;
  public string show;
  protected HtmlGenericControl divmsg;
  protected TextBox txt_advance_booking_window;
  protected DropDownList txt_default_approval_period;
  protected TextBox txt_booking_slot;
  protected TextBox txt_catering_email;
  protected TextBox txt_facilities_email;
  protected TextBox txt_upload_image_size;
  protected TextBox txt_image_width;
  protected TextBox txt_image_height;
  protected TextBox txt_layout_size;
  protected TextBox txt_layout_width;
  protected TextBox txt_layout_height;
  protected TextBox txt_from_email_address;
  protected TextBox txt_upload_document_type;
  protected TextBox txt_upload_document_size;
  protected TextBox txt_new_user_approver_email_address;
  protected TextBox txt_Cancel_period;
  protected CheckBox chk_show_terms_and_conditions;
  protected CheckBox chk_allow_housekeeping;
  protected CheckBox chk_book_weekend;
  protected CheckBox chk_book_holiday;
  protected DropDownList ddl_start_op_hours;
  protected DropDownList ddl_end_op_hours;
  protected TextBox txt_email_reminder_mins;
  protected Button btn_submit;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["global_settings"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.settings_view)
      this.redirect_unauthorized();
    if (!this.gp.settings_edit)
      this.btn_submit.Visible = false;
    this.btn_submit.Visible = true;
    try
    {
      this.data = this.settings.get_settings(this.current_user.account_id);
      this.globalSettings = new List<string>();
      this.globalSettings.Add("advance_booking_window");
      this.globalSettings.Add("default_approval_period");
      this.globalSettings.Add("booking_slot");
      this.globalSettings.Add("catering_email");
      this.globalSettings.Add("facilities_email");
      this.globalSettings.Add("upload_image_size");
      this.globalSettings.Add("upload_document_size");
      this.globalSettings.Add("upload_document_type");
      this.globalSettings.Add("image_width");
      this.globalSettings.Add("image_height");
      this.globalSettings.Add("image_type");
      this.globalSettings.Add("layout_width");
      this.globalSettings.Add("layout_height");
      this.globalSettings.Add("layout_size");
      this.globalSettings.Add("from_email_address");
      this.globalSettings.Add("Cancel_period");
      this.globalSettings.Add("new_user_approver_email_address");
      this.globalSettings.Add("allow_housekeeping");
      this.globalSettings.Add("show_terms_and_conditions");
      this.globalSettings.Add("book_weekend");
      this.globalSettings.Add("book_holiday");
      this.globalSettings.Add("operating_hours");
      this.globalSettings.Add("email_reminder_mins");
      if (this.IsPostBack)
        return;
      if (this.Session["gs"] != null && !string.IsNullOrWhiteSpace(this.Session["gs"].ToString()) && this.Session["gs"].ToString() == "1")
      {
        this.show = "S";
        this.Session.Remove("gs");
      }
      this.populate_ui(this.data);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_ui(DataSet data)
  {
    try
    {
      this.Populate_Time(this.ddl_start_op_hours);
      this.Populate_Time(this.ddl_end_op_hours);
      foreach (string globalSetting in this.globalSettings)
      {
        DataRow[] dataRowArray = data.Tables[0].Select("parameter='" + globalSetting + "'");
        if (dataRowArray != null && dataRowArray.Length > 0)
        {
          if (dataRowArray[0]["parameter"].ToString() != "default_approval_period" && dataRowArray[0]["parameter"].ToString() != "allow_housekeeping" && dataRowArray[0]["parameter"].ToString() != "show_terms_and_conditions" && dataRowArray[0]["parameter"].ToString() != "book_weekend" && dataRowArray[0]["parameter"].ToString() != "book_holiday" && dataRowArray[0]["parameter"].ToString() != "operating_hours")
          {
            TextBox control = (TextBox) this.Page.Master.FindControl("ContentPlaceHolder1").FindControl("txt_" + globalSetting);
            if (control != null)
              control.Text = dataRowArray[0]["value"].ToString();
          }
          else
          {
            if (dataRowArray[0]["parameter"].ToString() == "default_approval_period")
              this.txt_default_approval_period.SelectedValue = dataRowArray[0]["value"].ToString();
            if (dataRowArray[0]["parameter"].ToString() == "allow_housekeeping")
              this.chk_allow_housekeeping.Checked = Convert.ToBoolean(dataRowArray[0]["value"]);
            if (dataRowArray[0]["parameter"].ToString() == "show_terms_and_conditions")
              this.chk_show_terms_and_conditions.Checked = Convert.ToBoolean(dataRowArray[0]["value"]);
            if (dataRowArray[0]["parameter"].ToString() == "book_weekend")
              this.chk_book_weekend.Checked = Convert.ToBoolean(dataRowArray[0]["value"]);
            if (dataRowArray[0]["parameter"].ToString() == "book_holiday")
              this.chk_book_holiday.Checked = Convert.ToBoolean(dataRowArray[0]["value"]);
            if (dataRowArray[0]["parameter"].ToString() == "operating_hours")
            {
              try
              {
                string[] strArray = dataRowArray[0]["value"].ToString().Split(new string[1]
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
          }
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
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
      cbo.Items.Add(new ListItem("12:00 AM (+1 day)", dateTime.ToString("yyyy-MM-dd HH:mm:ss")));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      foreach (string globalSetting in this.globalSettings)
      {
        DataRow[] dataRowArray = this.data.Tables[0].Select("parameter='" + globalSetting + "'");
        if (dataRowArray.Length > 0)
        {
          string str = "";
          if (globalSetting != "default_approval_period" && globalSetting != "allow_housekeeping" && globalSetting != "show_terms_and_conditions" && dataRowArray[0]["parameter"].ToString() != "book_weekend" && dataRowArray[0]["parameter"].ToString() != "book_holiday" && dataRowArray[0]["parameter"].ToString() != "operating_hours")
          {
            TextBox control = (TextBox) this.Page.Master.FindControl("ContentPlaceHolder1").FindControl("txt_" + globalSetting);
            if (control != null)
              str = control.Text;
          }
          else
          {
            if (globalSetting == "default_approval_period")
              str = this.txt_default_approval_period.SelectedItem.Text;
            if (globalSetting == "allow_housekeeping")
              str = this.chk_allow_housekeeping.Checked.ToString();
            if (globalSetting == "show_terms_and_conditions")
              str = this.chk_show_terms_and_conditions.Checked.ToString();
            if (globalSetting == "book_weekend")
              str = this.chk_book_weekend.Checked.ToString();
            if (globalSetting == "book_holiday")
              str = this.chk_book_holiday.Checked.ToString();
            if (globalSetting == "operating_hours")
              str = this.ddl_start_op_hours.SelectedItem.Value + "|" + this.ddl_end_op_hours.SelectedItem.Value;
          }
          if (str != "")
          {
            setting setting = new setting();
            Guid guid = string.IsNullOrEmpty(dataRowArray[0]["record_id"].ToString()) ? Guid.NewGuid() : new Guid(dataRowArray[0]["record_id"].ToString());
            setting.account_id = this.current_user.account_id;
            setting.created_by = this.current_user.user_id;
            setting.created_on = this.current_timestamp;
            setting.modified_by = this.current_user.user_id;
            setting.modified_on = this.current_timestamp;
            setting.parameter = globalSetting;
            setting.properties = new XmlDocument();
            setting.properties.LoadXml(this.set_properties());
            setting.record_id = guid;
            setting.setting_id = Convert.ToInt64(dataRowArray[0]["setting_id"].ToString());
            setting.status = 1;
            setting.value = str;
            this.settings.update_setting(setting);
          }
          else if (globalSetting == "facilities_email" || globalSetting == "catering_email")
          {
            setting setting = new setting();
            Guid guid = string.IsNullOrEmpty(dataRowArray[0]["record_id"].ToString()) ? Guid.NewGuid() : new Guid(dataRowArray[0]["record_id"].ToString());
            setting.account_id = this.current_user.account_id;
            setting.created_by = this.current_user.user_id;
            setting.created_on = this.current_timestamp;
            setting.modified_by = this.current_user.user_id;
            setting.modified_on = this.current_timestamp;
            setting.parameter = globalSetting;
            setting.properties = new XmlDocument();
            setting.properties.LoadXml(this.set_properties());
            setting.record_id = guid;
            setting.setting_id = Convert.ToInt64(dataRowArray[0]["setting_id"].ToString());
            setting.status = 1;
            setting.value = str;
            this.settings.update_setting(setting);
          }
        }
        else
        {
          string str = "";
          TextBox control = (TextBox) this.Page.Master.FindControl("ContentPlaceHolder1").FindControl("txt_" + globalSetting);
          if (control != null)
            str = control.Text;
          if (str != "")
          {
            setting setting = new setting();
            Guid guid = Guid.NewGuid();
            setting.account_id = this.current_user.account_id;
            setting.created_by = this.current_user.user_id;
            setting.created_on = this.current_timestamp;
            setting.modified_by = this.current_user.user_id;
            setting.modified_on = this.current_timestamp;
            setting.parameter = globalSetting;
            setting.properties = new XmlDocument();
            setting.properties.LoadXml(this.set_properties());
            setting.record_id = guid;
            setting.setting_id = 0L;
            setting.status = 1;
            setting.value = str;
            this.settings.update_setting(setting);
          }
        }
      }
      this.Session["gs"] = (object) "1";
      this.Session["setting_update"] = (object) "S";
      this.Response.Redirect("settings_list.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private string set_properties() => "<properties></properties>";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
