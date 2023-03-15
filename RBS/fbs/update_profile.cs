// Decompiled with JetBrains decompiler
// Type: update_profile
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

public class update_profile : fbs_base_page, IRequiresSessionState
{
  public string password = "";
  protected TextBox txt_email;
  protected TextBox txt_username;
  protected TextBox txt_full_name;
  protected TextBox txt_institution;
  protected DropDownList ddlDivision;
  protected DropDownList ddlDepartment;
  protected DropDownList ddlSection;
  protected TextBox txtPIN;
  protected TextBox txt_office_phone;
  protected TextBox txt_mobile_phone;
  protected TextBox txt_staff_id;
  protected HiddenField hidUserId;
  protected HiddenField hdnEditUserId;
  protected HiddenField hdnEmail;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected TextBox txt_Password;
  protected TextBox txt_rePassword;
  protected Button btn_password_update;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.current_user.login_type != 2L)
      this.Response.Redirect("bookings.aspx");
    if (this.IsPostBack)
      return;
    this.txt_institution.Text = this.current_account.name;
    this.populate_form(this.current_user);
    this.bind_all_dropdown();
    this.populate_edit_form(this.current_user);
  }

  private void bind_all_dropdown()
  {
    try
    {
      foreach (DataRow dataRow in this.reportings.get_divison_master(this.current_user.account_id).Tables[0].Select("type=1"))
        this.ddlDivision.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlDivision.Items.Insert(0, new ListItem("Select Division", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error: ->", ex);
    }
  }

  protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      DataRow[] dataRowArray = this.reportings.get_divison_master(this.current_user.account_id).Tables[0].Select("type='2' AND parent_id='" + this.ddlDivision.SelectedValue + "'");
      this.ddlDepartment.Items.Clear();
      foreach (DataRow dataRow in dataRowArray)
        this.ddlDepartment.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error: ->", ex);
    }
  }

  protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
  {
    try
    {
      DataRow[] dataRowArray = this.reportings.get_divison_master(this.current_user.account_id).Tables[0].Select("type='3' AND parent_id='" + this.ddlDepartment.SelectedValue + "'");
      this.ddlSection.Items.Clear();
      foreach (DataRow dataRow in dataRowArray)
        this.ddlSection.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlSection.Items.Insert(0, new ListItem("Select Section", ""));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error: ->", ex);
    }
  }

  private void populate_form(user obj)
  {
    try
    {
      Dictionary<string, user_property> properties = obj.properties;
      this.txt_staff_id.Text = properties.ContainsKey("staff_id") ? properties["staff_id"].property_value : "";
      this.txt_institution.Text = properties.ContainsKey("staff_inst") ? properties["staff_inst"].property_value : "";
      this.txt_office_phone.Text = properties.ContainsKey("staff_offphone") ? properties["staff_offphone"].property_value : "";
      this.txt_mobile_phone.Text = properties.ContainsKey("staff_pager_mobile") ? properties["staff_pager_mobile"].property_value : "";
      this.txt_email.Text = obj.email;
      this.txt_full_name.Text = obj.full_name;
      this.txt_username.Text = obj.username;
      this.txtPIN.Text = properties.ContainsKey("pin") ? properties["pin"].property_value : "";
      this.hidUserId.Value = obj.user_id.ToString();
      this.ddlDivision.SelectedItem.Text = properties.ContainsKey("staff_division") ? properties["staff_division"].property_value : "";
      this.ddlDepartment.SelectedItem.Text = properties.ContainsKey("staff_department") ? properties["staff_department"].property_value : "";
      this.ddlSection.SelectedItem.Text = properties.ContainsKey("staff_section") ? properties["staff_section"].property_value : "";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private void populate_edit_form(user obj)
  {
    try
    {
      Dictionary<string, user_property> properties = obj.properties;
      this.txt_staff_id.Text = properties.ContainsKey("staff_id") ? properties["staff_id"].property_value : "";
      this.txt_institution.Text = properties.ContainsKey("staff_inst") ? properties["staff_inst"].property_value : "";
      this.txt_office_phone.Text = properties.ContainsKey("staff_offphone") ? properties["staff_offphone"].property_value : "";
      this.txt_mobile_phone.Text = properties.ContainsKey("staff_pager_mobile") ? properties["staff_pager_mobile"].property_value : "";
      this.txt_email.Text = obj.email;
      this.txt_full_name.Text = obj.full_name;
      this.hidUserId.Value = obj.user_id.ToString();
      this.hdnEmail.Value = obj.email;
      DataSet divisonMaster = this.reportings.get_divison_master(this.current_user.account_id);
      DataRow[] dataRowArray1 = divisonMaster.Tables[0].Select("type=1");
      this.ddlDivision.Items.Clear();
      foreach (DataRow dataRow in dataRowArray1)
        this.ddlDivision.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      this.ddlDivision.Items.Insert(0, new ListItem("Select Division", ""));
      string text1 = properties.ContainsKey("staff_division") ? properties["staff_division"].property_value : "";
      if (text1 != "")
      {
        this.ddlDivision.Items.FindByText(text1).Selected = true;
        DataRow[] dataRowArray2 = divisonMaster.Tables[0].Select("type='2' AND parent_id='" + this.ddlDivision.Items.FindByText(text1).Value + "'");
        this.ddlDepartment.Items.Clear();
        foreach (DataRow dataRow in dataRowArray2)
          this.ddlDepartment.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      }
      this.ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
      string text2 = properties.ContainsKey("staff_department") ? properties["staff_department"].property_value : "";
      if (text2 != "")
      {
        this.ddlDepartment.Items.FindByText(text2).Selected = true;
        DataRow[] dataRowArray3 = divisonMaster.Tables[0].Select("type='3' AND parent_id='" + this.ddlDepartment.Items.FindByText(text2).Value + "'");
        this.ddlSection.Items.Clear();
        foreach (DataRow dataRow in dataRowArray3)
          this.ddlSection.Items.Add(new ListItem(dataRow["name"].ToString(), dataRow["master_id"].ToString()));
      }
      this.ddlSection.Items.Insert(0, new ListItem("Select Section", ""));
      string text3 = properties.ContainsKey("staff_section") ? properties["staff_section"].property_value : "";
      if (!(text3 != ""))
        return;
      this.ddlSection.Items.FindByText(text3).Selected = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      user currentUser = this.current_user;
      currentUser.email = this.txt_email.Text.Trim();
      currentUser.full_name = this.txt_full_name.Text.Trim();
      currentUser.modified_by = this.current_user.user_id;
      currentUser.modified_on = this.current_timestamp;
      user user = this.users.update_user(currentUser);
      if (user.user_id > 0L)
      {
        user_property userProperty1 = new user_property();
        Dictionary<string, user_property> userProperties = this.users.get_user_properties(user.user_id, this.current_user.account_id);
        if (userProperties.ContainsKey("staff_title"))
        {
          user_property userProperty2 = userProperties["staff_title"];
          userProperty2.property_name = "staff_title";
          userProperty2.property_value = "";
          userProperty1 = this.users.update_user_property(userProperty2);
        }
        if (userProperties.ContainsKey("given_name"))
        {
          user_property userProperty3 = userProperties["given_name"];
          userProperty3.property_name = "given_name";
          userProperty3.property_value = "";
          userProperty3.modified_by = this.current_user.user_id;
          userProperty3.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty3);
        }
        if (userProperties.ContainsKey("native_name"))
        {
          user_property userProperty4 = userProperties["native_name"];
          userProperty4.property_name = "native_name";
          userProperty4.property_value = "";
          userProperty4.modified_by = this.current_user.user_id;
          userProperty4.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty4);
        }
        if (userProperties.ContainsKey("staff_id"))
        {
          user_property userProperty5 = userProperties["staff_id"];
          userProperty5.property_name = "staff_id";
          userProperty5.property_value = this.txt_staff_id.Text.Trim();
          userProperty5.modified_by = this.current_user.user_id;
          userProperty5.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty5);
        }
        if (userProperties.ContainsKey("staff_inst"))
        {
          user_property userProperty6 = userProperties["staff_inst"];
          userProperty6.property_name = "staff_inst";
          userProperty6.property_value = this.txt_institution.Text.Trim();
          userProperty6.modified_by = this.current_user.user_id;
          userProperty6.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty6);
        }
        if (userProperties.ContainsKey("staff_division"))
        {
          user_property userProperty7 = userProperties["staff_division"];
          userProperty7.property_name = "staff_division";
          userProperty7.property_value = this.ddlDivision.SelectedItem.Text.Trim();
          userProperty7.modified_by = this.current_user.user_id;
          userProperty7.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty7);
        }
        if (userProperties.ContainsKey("staff_department"))
        {
          user_property userProperty8 = userProperties["staff_department"];
          userProperty8.property_name = "staff_department";
          userProperty8.property_value = this.ddlDepartment.SelectedItem.Text.Trim();
          userProperty8.modified_by = this.current_user.user_id;
          userProperty8.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty8);
        }
        if (userProperties.ContainsKey("staff_section"))
        {
          user_property userProperty9 = userProperties["staff_section"];
          userProperty9.property_name = "staff_section";
          userProperty9.property_value = this.ddlSection.SelectedItem.Text.Trim();
          userProperty9.modified_by = this.current_user.user_id;
          userProperty9.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty9);
        }
        if (userProperties.ContainsKey("staff_desg"))
        {
          user_property userProperty10 = userProperties["staff_desg"];
          userProperty10.property_name = "staff_desg";
          userProperty10.property_value = "";
          userProperty10.modified_by = this.current_user.user_id;
          userProperty10.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty10);
        }
        if (userProperties.ContainsKey("staff_offphone"))
        {
          user_property userProperty11 = userProperties["staff_offphone"];
          userProperty11.property_name = "staff_offphone";
          userProperty11.property_value = this.txt_office_phone.Text.Trim();
          userProperty11.modified_by = this.current_user.user_id;
          userProperty11.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty11);
        }
        if (userProperties.ContainsKey("staff_pager_mobile"))
        {
          user_property userProperty12 = userProperties["staff_pager_mobile"];
          userProperty12.property_name = "staff_pager_mobile";
          userProperty12.property_value = this.txt_mobile_phone.Text.Trim();
          userProperty12.modified_by = this.current_user.user_id;
          userProperty12.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty12);
        }
        if (userProperties.ContainsKey("staff_comm_date"))
        {
          user_property userProperty13 = userProperties["staff_comm_date"];
          userProperty13.property_name = "staff_comm_date";
          userProperty13.property_value = "";
          userProperty13.modified_by = this.current_user.user_id;
          userProperty13.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty13);
        }
        if (userProperties.ContainsKey("staff_cess_date"))
        {
          user_property userProperty14 = userProperties["staff_cess_date"];
          userProperty14.property_name = "staff_cess_date";
          userProperty14.property_value = "";
          userProperty14.modified_by = this.current_user.user_id;
          userProperty14.modified_on = this.current_timestamp;
          userProperty1 = this.users.update_user_property(userProperty14);
        }
      }
      this.Session["Save"] = (object) "S";
      this.current_user = this.users.get_user(this.current_user.user_id, this.current_user.account_id);
      this.Session["user"] = (object) this.current_user;
      this.Response.Redirect("bookings.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_password_update_Click(object sender, EventArgs e)
  {
    if (!(this.txt_Password.Text == this.txt_rePassword.Text))
      return;
    this.current_user.password = this.users.get_md5(this.txt_Password.Text);
    this.users.update_password(this.current_user);
    this.Response.Redirect("bookings.aspx", true);
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
