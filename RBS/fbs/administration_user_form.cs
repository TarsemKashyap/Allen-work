// Decompiled with JetBrains decompiler
// Type: administration_user_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using log4net;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_user_form : fbs_base_page, IRequiresSessionState
{
  protected TextBox txt_uer_name;
  protected Button btnCheckAvailability;
  protected Label lblError;
  protected DropDownList ddl_usergroup;
  protected TextBox txt_email;
  protected TextBox txt_full_name;
  protected TextBox txt_telephone;
  protected CheckBox check_password;
  protected TextBox txt_Password;
  protected TextBox txt_rePassword;
  protected HtmlGenericControl div_password;
  protected HtmlGenericControl div_pwd;
  protected CheckBox check_pin;
  protected Literal litError;
  protected HtmlGenericControl alt_err;
  protected TextBox txt_pin;
  protected HtmlGenericControl div_pin;
  protected HtmlGenericControl div_show_pin;
  protected HiddenField hidUserId;
  protected HiddenField hdnEditUserId;
  protected HiddenField hdnEmail;
  protected Button btn_submit;
  protected Button btn_cancel;
  public string password = "";
  private users_api uapi = new users_api();
  private user obj;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      this.current_user.login_type = 2L;
      if (!this.gp.users_view)
        this.redirect_unauthorized();
      if (this.IsPostBack)
        return;
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["id"]))
      {
        this.obj = this.users.get_user(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id);
        this.populate_form(this.obj);
        this.div_pwd.Visible = false;
        this.div_show_pin.Visible = false;
        this.btn_submit.Text = "Save";
        this.btnCheckAvailability.Visible = false;
        this.txt_uer_name.ReadOnly = true;
      }
      else
        this.btn_submit.Text = "Create User";
      this.bind_ddl_group();
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["user_id"]))
      {
        this.obj = this.users.get_user(Convert.ToInt64(this.Request.QueryString["user_id"]), this.current_user.account_id);
        this.populate_edit_form(this.obj);
        this.div_pwd.Visible = false;
        this.div_show_pin.Visible = false;
        this.btn_submit.Text = "Save";
        this.btnCheckAvailability.Visible = false;
        this.txt_uer_name.ReadOnly = true;
      }
      else
        this.btn_submit.Text = "Create User";
      this.alt_err.Visible = false;
      this.lblError.Text = "";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private void bind_ddl_group()
  {
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.reportings.get_user_group(this.current_user.account_id).Tables[0].Rows)
      {
        if (row["group_name"].ToString() != "All Users")
          this.ddl_usergroup.Items.Add(new ListItem(row["group_name"].ToString(), row["group_id"].ToString()));
      }
      this.ddl_usergroup.Items.Insert(0, new ListItem("Select Group", ""));
    }
    catch (Exception ex)
    {
    }
  }

  private void populate_form(user obj)
  {
    try
    {
      Dictionary<string, user_property> properties = obj.properties;
      this.txt_telephone.Text = properties.ContainsKey("staff_offphone") ? properties["staff_offphone"].property_value : "";
      this.txt_email.Text = obj.email;
      this.txt_full_name.Text = obj.full_name;
      this.txt_uer_name.Text = obj.username;
      this.hidUserId.Value = obj.user_id.ToString();
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
      this.txt_telephone.Text = properties.ContainsKey("staff_offphone") ? properties["staff_offphone"].property_value : "";
      this.txt_email.Text = obj.email;
      this.txt_full_name.Text = obj.full_name;
      this.txt_uer_name.Text = obj.username;
      this.hidUserId.Value = obj.user_id.ToString();
      this.hdnEmail.Value = obj.email;
      this.reportings.get_divison_master(this.current_user.account_id);
      foreach (string key in obj.groups.Keys)
      {
        if (key != "All Users")
        {
          this.ddl_usergroup.Items.FindByText(key).Selected = true;
          break;
        }
      }
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
      user user1 = new user();
      this.password = !this.check_password.Checked ? this.txt_Password.Text.Trim() : this.utilities.generate_password();
      user user2 = this.users.get_user(this.txt_uer_name.Text.Trim());
      bool flag = false;
      if (user2 != null && this.hidUserId.Value.Trim() == "" && user2.user_id > 0L)
        flag = true;
      if (user2 != null && !flag && !string.IsNullOrEmpty(this.hdnEmail.Value.ToString()) && user2.email != null & this.hdnEmail.Value != user2.email)
        flag = true;
      if (flag)
      {
        this.lblError.Visible = true;
        this.lblError.Text = "Aleady exist this email.";
      }
      else
      {
        string str = !this.check_pin.Checked ? this.txt_pin.Text : this.users.generate_check_pin(this.current_user.account_id);
        if (!this.users.is_duplicate_pin(this.txt_pin.Text, this.current_user.account_id))
        {
          this.alt_err.Visible = false;
          this.litError.Text = "";
          if (this.hidUserId.Value.Trim() != "")
          {
            user2 = this.users.get_user(Convert.ToInt64(this.hidUserId.Value), this.current_user.account_id);
            user2.email = this.txt_email.Text.Trim();
            user2.full_name = this.txt_full_name.Text.Trim();
            user2.created_by = this.current_user.user_id;
            user2.created_on = this.current_timestamp;
            user2.modified_by = this.current_user.user_id;
            user2.modified_on = this.current_timestamp;
          }
          else
          {
            user2.account_id = this.current_user.account_id;
            user2.created_by = this.current_user.user_id;
            user2.created_on = this.current_timestamp;
            user2.modified_by = this.current_user.user_id;
            user2.modified_on = this.current_timestamp;
            user2.email = this.txt_email.Text.Trim();
            user2.password = this.users.get_md5(this.password);
            user2.full_name = this.txt_full_name.Text.Trim();
            user2.locked = false;
            user2.login_type = 2L;
            user2.password_reset_request = true;
            user2.primary_user = false;
            user2.timezone = this.current_account.timezone;
            user2.user_id = 0L;
            user2.username = this.txt_uer_name.Text.Trim();
            user2.record_id = Guid.NewGuid();
            user2.status = 1L;
            user2.activated = true;
          }
          user2.User_insert_type = true;
          user user3 = this.users.update_user(user2);
          if (this.hidUserId.Value == "")
          {
            if (user3.user_id > 0L)
            {
              user_property userProperty1 = new user_property();
              Dictionary<string, user_property> userProperties = this.users.get_user_properties(user3.user_id, this.current_user.account_id);
              user_property userProperty2;
              if (userProperties.ContainsKey("staff_title"))
              {
                user_property userProperty3 = userProperties["staff_title"];
                userProperty3.property_name = "staff_title";
                userProperty3.property_value = "";
                userProperty3.created_by = this.current_user.user_id;
                userProperty3.created_on = this.current_timestamp;
                userProperty3.modified_by = this.current_user.user_id;
                userProperty3.modified_on = this.current_timestamp;
                userProperty2 = this.users.update_user_property(userProperty3);
              }
              else
              {
                userProperty1.account_id = this.current_user.account_id;
                userProperty1.created_by = this.current_user.user_id;
                userProperty1.created_on = this.current_timestamp;
                userProperty1.modified_by = this.current_user.user_id;
                userProperty1.modified_on = this.current_timestamp;
                userProperty1.property_name = "staff_title";
                userProperty1.property_value = "";
                userProperty1.record_id = user3.record_id;
                userProperty1.user_id = user3.user_id;
                userProperty1.user_property_id = 0L;
                userProperty2 = this.users.update_user_property(userProperty1);
              }
              user_property userProperty4;
              if (userProperties.ContainsKey("given_name"))
              {
                user_property userProperty5 = userProperties["given_name"];
                userProperty5.property_name = "given_name";
                userProperty5.property_value = "";
                userProperty5.created_by = this.current_user.user_id;
                userProperty5.created_on = this.current_timestamp;
                userProperty5.modified_by = this.current_user.user_id;
                userProperty5.modified_on = this.current_timestamp;
                userProperty4 = this.users.update_user_property(userProperty5);
              }
              else
              {
                userProperty2.account_id = this.current_user.account_id;
                userProperty2.created_by = this.current_user.user_id;
                userProperty2.created_on = this.current_timestamp;
                userProperty2.modified_by = this.current_user.user_id;
                userProperty2.modified_on = this.current_timestamp;
                userProperty2.property_name = "given_name";
                userProperty2.property_value = "";
                userProperty2.record_id = user3.record_id;
                userProperty2.user_id = user3.user_id;
                userProperty2.user_property_id = 0L;
                userProperty4 = this.users.update_user_property(userProperty2);
              }
              user_property userProperty6;
              if (userProperties.ContainsKey("native_name"))
              {
                user_property userProperty7 = userProperties["native_name"];
                userProperty7.property_name = "native_name";
                userProperty7.property_value = "";
                userProperty7.created_by = this.current_user.user_id;
                userProperty7.created_on = this.current_timestamp;
                userProperty7.modified_by = this.current_user.user_id;
                userProperty7.modified_on = this.current_timestamp;
                userProperty6 = this.users.update_user_property(userProperty7);
              }
              else
              {
                userProperty4.account_id = this.current_user.account_id;
                userProperty4.created_by = this.current_user.user_id;
                userProperty4.created_on = this.current_timestamp;
                userProperty4.modified_by = this.current_user.user_id;
                userProperty4.modified_on = this.current_timestamp;
                userProperty4.property_name = "native_name";
                userProperty4.property_value = "";
                userProperty4.record_id = user3.record_id;
                userProperty4.user_id = user3.user_id;
                userProperty4.user_property_id = 0L;
                userProperty6 = this.users.update_user_property(userProperty4);
              }
              user_property userProperty8;
              if (userProperties.ContainsKey("staff_id"))
              {
                user_property userProperty9 = userProperties["staff_id"];
                userProperty9.property_name = "staff_id";
                userProperty9.property_value = "";
                userProperty9.created_by = this.current_user.user_id;
                userProperty9.created_on = this.current_timestamp;
                userProperty9.modified_by = this.current_user.user_id;
                userProperty9.modified_on = this.current_timestamp;
                userProperty8 = this.users.update_user_property(userProperty9);
              }
              else
              {
                userProperty6.account_id = this.current_user.account_id;
                userProperty6.created_by = this.current_user.user_id;
                userProperty6.created_on = this.current_timestamp;
                userProperty6.modified_by = this.current_user.user_id;
                userProperty6.modified_on = this.current_timestamp;
                userProperty6.property_name = "staff_id";
                userProperty6.property_value = "";
                userProperty6.record_id = user3.record_id;
                userProperty6.user_id = user3.user_id;
                userProperty6.user_property_id = 0L;
                userProperty8 = this.users.update_user_property(userProperty6);
              }
              user_property userProperty10;
              if (userProperties.ContainsKey("staff_inst"))
              {
                user_property userProperty11 = userProperties["staff_inst"];
                userProperty11.property_name = "staff_inst";
                userProperty11.property_value = "";
                userProperty11.created_by = this.current_user.user_id;
                userProperty11.created_on = this.current_timestamp;
                userProperty11.modified_by = this.current_user.user_id;
                userProperty11.modified_on = this.current_timestamp;
                userProperty10 = this.users.update_user_property(userProperty11);
              }
              else
              {
                userProperty8.account_id = this.current_user.account_id;
                userProperty8.created_by = this.current_user.user_id;
                userProperty8.created_on = this.current_timestamp;
                userProperty8.modified_by = this.current_user.user_id;
                userProperty8.modified_on = this.current_timestamp;
                userProperty8.property_name = "staff_inst";
                userProperty8.property_value = "";
                userProperty8.record_id = user3.record_id;
                userProperty8.user_id = user3.user_id;
                userProperty8.user_property_id = 0L;
                userProperty10 = this.users.update_user_property(userProperty8);
              }
              user_property userProperty12;
              if (userProperties.ContainsKey("staff_division"))
              {
                user_property userProperty13 = userProperties["staff_division"];
                userProperty13.property_name = "staff_division";
                userProperty13.property_value = "";
                userProperty13.created_by = this.current_user.user_id;
                userProperty13.created_on = this.current_timestamp;
                userProperty13.modified_by = this.current_user.user_id;
                userProperty13.modified_on = this.current_timestamp;
                userProperty12 = this.users.update_user_property(userProperty13);
              }
              else
              {
                userProperty10.account_id = this.current_user.account_id;
                userProperty10.created_by = this.current_user.user_id;
                userProperty10.created_on = this.current_timestamp;
                userProperty10.modified_by = this.current_user.user_id;
                userProperty10.modified_on = this.current_timestamp;
                userProperty10.property_name = "staff_division";
                userProperty10.property_value = "";
                userProperty10.record_id = user3.record_id;
                userProperty10.user_id = user3.user_id;
                userProperty10.user_property_id = 0L;
                userProperty12 = this.users.update_user_property(userProperty10);
              }
              user_property userProperty14;
              if (userProperties.ContainsKey("staff_department"))
              {
                user_property userProperty15 = userProperties["staff_department"];
                userProperty15.property_name = "staff_department";
                userProperty15.property_value = "";
                userProperty15.created_by = this.current_user.user_id;
                userProperty15.created_on = this.current_timestamp;
                userProperty15.modified_by = this.current_user.user_id;
                userProperty15.modified_on = this.current_timestamp;
                userProperty14 = this.users.update_user_property(userProperty15);
              }
              else
              {
                userProperty12.account_id = this.current_user.account_id;
                userProperty12.created_by = this.current_user.user_id;
                userProperty12.created_on = this.current_timestamp;
                userProperty12.modified_by = this.current_user.user_id;
                userProperty12.modified_on = this.current_timestamp;
                userProperty12.property_name = "staff_department";
                userProperty12.property_value = "";
                userProperty12.record_id = user3.record_id;
                userProperty12.user_id = user3.user_id;
                userProperty12.user_property_id = 0L;
                userProperty14 = this.users.update_user_property(userProperty12);
              }
              user_property userProperty16;
              if (userProperties.ContainsKey("staff_section"))
              {
                user_property userProperty17 = userProperties["staff_section"];
                userProperty17.property_name = "staff_section";
                userProperty17.property_value = "";
                userProperty17.created_by = this.current_user.user_id;
                userProperty17.created_on = this.current_timestamp;
                userProperty17.modified_by = this.current_user.user_id;
                userProperty17.modified_on = this.current_timestamp;
                userProperty16 = this.users.update_user_property(userProperty17);
              }
              else
              {
                userProperty14.account_id = this.current_user.account_id;
                userProperty14.created_by = this.current_user.user_id;
                userProperty14.created_on = this.current_timestamp;
                userProperty14.modified_by = this.current_user.user_id;
                userProperty14.modified_on = this.current_timestamp;
                userProperty14.property_name = "staff_section";
                userProperty14.property_value = "";
                userProperty14.record_id = user3.record_id;
                userProperty14.user_id = user3.user_id;
                userProperty14.user_property_id = 0L;
                userProperty16 = this.users.update_user_property(userProperty14);
              }
              user_property userProperty18;
              if (userProperties.ContainsKey("staff_desg"))
              {
                user_property userProperty19 = userProperties["staff_desg"];
                userProperty19.property_name = "staff_desg";
                userProperty19.property_value = "";
                userProperty19.created_by = this.current_user.user_id;
                userProperty19.created_on = this.current_timestamp;
                userProperty19.modified_by = this.current_user.user_id;
                userProperty19.modified_on = this.current_timestamp;
                userProperty18 = this.users.update_user_property(userProperty19);
              }
              else
              {
                userProperty16.account_id = this.current_user.account_id;
                userProperty16.created_by = this.current_user.user_id;
                userProperty16.created_on = this.current_timestamp;
                userProperty16.modified_by = this.current_user.user_id;
                userProperty16.modified_on = this.current_timestamp;
                userProperty16.property_name = "staff_desg";
                userProperty16.property_value = "";
                userProperty16.record_id = user3.record_id;
                userProperty16.user_id = user3.user_id;
                userProperty16.user_property_id = 0L;
                userProperty18 = this.users.update_user_property(userProperty16);
              }
              user_property userProperty20;
              if (userProperties.ContainsKey("staff_offphone"))
              {
                user_property userProperty21 = userProperties["staff_offphone"];
                userProperty21.property_name = "staff_offphone";
                userProperty21.property_value = this.txt_telephone.Text.Trim();
                userProperty21.created_by = this.current_user.user_id;
                userProperty21.created_on = this.current_timestamp;
                userProperty21.modified_by = this.current_user.user_id;
                userProperty21.modified_on = this.current_timestamp;
                userProperty20 = this.users.update_user_property(userProperty21);
              }
              else
              {
                userProperty18.account_id = this.current_user.account_id;
                userProperty18.created_by = this.current_user.user_id;
                userProperty18.created_on = this.current_timestamp;
                userProperty18.modified_by = this.current_user.user_id;
                userProperty18.modified_on = this.current_timestamp;
                userProperty18.property_name = "staff_offphone";
                userProperty18.property_value = this.txt_telephone.Text.Trim();
                userProperty18.record_id = user3.record_id;
                userProperty18.user_id = user3.user_id;
                userProperty18.user_property_id = 0L;
                userProperty20 = this.users.update_user_property(userProperty18);
              }
              user_property userProperty22;
              if (userProperties.ContainsKey("staff_pager_mobile"))
              {
                user_property userProperty23 = userProperties["staff_pager_mobile"];
                userProperty23.property_name = "staff_pager_mobile";
                userProperty23.property_value = "";
                userProperty23.created_by = this.current_user.user_id;
                userProperty23.created_on = this.current_timestamp;
                userProperty23.modified_by = this.current_user.user_id;
                userProperty23.modified_on = this.current_timestamp;
                userProperty22 = this.users.update_user_property(userProperty23);
              }
              else
              {
                userProperty20.account_id = this.current_user.account_id;
                userProperty20.created_by = this.current_user.user_id;
                userProperty20.created_on = this.current_timestamp;
                userProperty20.modified_by = this.current_user.user_id;
                userProperty20.modified_on = this.current_timestamp;
                userProperty20.property_name = "staff_pager_mobile";
                userProperty20.property_value = "";
                userProperty20.record_id = user3.record_id;
                userProperty20.user_id = user3.user_id;
                userProperty20.user_property_id = 0L;
                userProperty22 = this.users.update_user_property(userProperty20);
              }
              user_property userProperty24;
              if (userProperties.ContainsKey("staff_comm_date"))
              {
                user_property userProperty25 = userProperties["staff_comm_date"];
                userProperty25.property_name = "staff_comm_date";
                userProperty25.property_value = "";
                userProperty25.created_by = this.current_user.user_id;
                userProperty25.created_on = this.current_timestamp;
                userProperty25.modified_by = this.current_user.user_id;
                userProperty25.modified_on = this.current_timestamp;
                userProperty24 = this.users.update_user_property(userProperty25);
              }
              else
              {
                userProperty22.account_id = this.current_user.account_id;
                userProperty22.created_by = this.current_user.user_id;
                userProperty22.created_on = this.current_timestamp;
                userProperty22.modified_by = this.current_user.user_id;
                userProperty22.modified_on = this.current_timestamp;
                userProperty22.property_name = "staff_comm_date";
                userProperty22.property_value = "";
                userProperty22.record_id = user3.record_id;
                userProperty22.user_id = user3.user_id;
                userProperty22.user_property_id = 0L;
                userProperty24 = this.users.update_user_property(userProperty22);
              }
              user_property userProperty26;
              if (userProperties.ContainsKey("staff_cess_date"))
              {
                user_property userProperty27 = userProperties["staff_cess_date"];
                userProperty27.property_name = "staff_cess_date";
                userProperty27.property_value = "";
                userProperty27.created_by = this.current_user.user_id;
                userProperty27.created_on = this.current_timestamp;
                userProperty27.modified_by = this.current_user.user_id;
                userProperty27.modified_on = this.current_timestamp;
                userProperty26 = this.users.update_user_property(userProperty27);
              }
              else
              {
                userProperty24.account_id = this.current_user.account_id;
                userProperty24.created_by = this.current_user.user_id;
                userProperty24.created_on = this.current_timestamp;
                userProperty24.modified_by = this.current_user.user_id;
                userProperty24.modified_on = this.current_timestamp;
                userProperty24.property_name = "staff_cess_date";
                userProperty24.property_value = "";
                userProperty24.record_id = user3.record_id;
                userProperty24.user_id = user3.user_id;
                userProperty24.user_property_id = 0L;
                userProperty26 = this.users.update_user_property(userProperty24);
              }
              user_property userProperty28;
              if (userProperties.ContainsKey("pin"))
              {
                user_property userProperty29 = userProperties["pin"];
                userProperty29.property_name = "pin";
                userProperty29.property_value = str;
                userProperty29.created_by = this.current_user.user_id;
                userProperty29.created_on = this.current_timestamp;
                userProperty29.modified_by = this.current_user.user_id;
                userProperty29.modified_on = this.current_timestamp;
                userProperty28 = this.users.update_user_property(userProperty29);
              }
              else if (str != "")
              {
                userProperty26.account_id = this.current_user.account_id;
                userProperty26.created_by = this.current_user.user_id;
                userProperty26.created_on = this.current_timestamp;
                userProperty26.modified_by = this.current_user.user_id;
                userProperty26.modified_on = this.current_timestamp;
                userProperty26.property_name = "pin";
                userProperty26.property_value = str;
                userProperty26.record_id = user3.record_id;
                userProperty26.user_id = user3.user_id;
                userProperty26.user_property_id = 0L;
                userProperty28 = this.users.update_user_property(userProperty26);
              }
              if (!this.users.get_user_group(user3.user_id, this.current_user.account_id).ContainsKey(api_constants.all_users_text))
              {
                user_group groupByname = this.users.get_group_byname(api_constants.all_users_text, this.current_user.account_id);
                user_group_mapping userGroupMapping1 = new user_group_mapping()
                {
                  user_id = user3.user_id,
                  group_id = groupByname == null ? 1L : (groupByname.group_id == 0L ? 1L : groupByname.group_id)
                };
                userGroupMapping1.group_id = groupByname.group_id;
                userGroupMapping1.account_id = this.current_user.account_id;
                userGroupMapping1.created_by = this.current_user.user_id;
                userGroupMapping1.created_on = this.current_timestamp;
                userGroupMapping1.modified_by = this.current_user.user_id;
                userGroupMapping1.modified_on = this.current_timestamp;
                userGroupMapping1.record_id = user3.record_id;
                user_group_mapping userGroupMapping2 = this.users.insert_user_group_mapping(userGroupMapping1);
                userGroupMapping2.group_id = Convert.ToInt64(this.ddl_usergroup.SelectedItem.Value.ToString());
                this.users.insert_user_group_mapping(userGroupMapping2);
              }
              template template1 = new template();
              template template2 = this.tapi.get_template("email_user_add", this.current_user.account_id);
              account account = this.users.get_account(this.current_user.account_id);
              string newValue = this.site_full_path + "assets/img/" + this.current_account.logo;
              template2.content_data = template2.content_data.Replace("[password]", this.password);
              template2.content_data = template2.content_data.Replace("[full_name]", user3.full_name);
              template2.content_data = template2.content_data.Replace("[logo]", newValue);
              template2.content_data = template2.content_data.Replace("[company_name]", account.name);
              template2.content_data = template2.content_data.Replace("[creator]", this.current_user.full_name);
              template2.content_data = template2.content_data.Replace("[login_link]", this.site_full_path + "login.aspx");
              template2.content_data = template2.content_data.Replace("[username]", user3.username);
              email email = new email();
              if (Convert.ToBoolean(this.current_account.properties["send_email"]))
                this.utilities.sendEmail("", template2.content_data, template2.title, "", user3.email, user3.record_id);
            }
          }
          else if (user3.user_id > 0L)
          {
            user_property userProperty30 = new user_property();
            Dictionary<string, user_property> userProperties = this.users.get_user_properties(user3.user_id, this.current_user.account_id);
            user_property userProperty31;
            if (userProperties.ContainsKey("staff_offphone"))
            {
              user_property userProperty32 = userProperties["staff_offphone"];
              userProperty32.property_name = "staff_offphone";
              userProperty32.property_value = this.txt_telephone.Text.Trim();
              userProperty32.created_by = this.current_user.user_id;
              userProperty32.created_on = this.current_timestamp;
              userProperty32.modified_by = this.current_user.user_id;
              userProperty32.modified_on = this.current_timestamp;
              userProperty31 = this.users.update_user_property(userProperty32);
            }
            else
            {
              userProperty30.account_id = this.current_user.account_id;
              userProperty30.created_by = this.current_user.user_id;
              userProperty30.created_on = this.current_timestamp;
              userProperty30.modified_by = this.current_user.user_id;
              userProperty30.modified_on = this.current_timestamp;
              userProperty30.property_name = "staff_offphone";
              userProperty30.property_value = this.txt_telephone.Text.Trim();
              userProperty30.record_id = user3.record_id;
              userProperty30.user_id = user3.user_id;
              userProperty30.user_property_id = 0L;
              userProperty31 = this.users.update_user_property(userProperty30);
            }
            if (!this.users.get_user_group(user3.user_id, this.current_user.account_id).ContainsKey(api_constants.all_users_text))
            {
              user_group groupByname = this.users.get_group_byname(api_constants.all_users_text, this.current_user.account_id);
              user_group_mapping userGroupMapping3 = new user_group_mapping()
              {
                user_id = user3.user_id,
                group_id = groupByname == null ? 1L : (groupByname.group_id == 0L ? 1L : groupByname.group_id)
              };
              userGroupMapping3.group_id = groupByname.group_id;
              userGroupMapping3.account_id = this.current_user.account_id;
              userGroupMapping3.created_by = this.current_user.user_id;
              userGroupMapping3.created_on = this.current_timestamp;
              userGroupMapping3.modified_by = this.current_user.user_id;
              userGroupMapping3.modified_on = this.current_timestamp;
              userGroupMapping3.record_id = user3.record_id;
              user_group_mapping userGroupMapping4 = this.users.insert_user_group_mapping(userGroupMapping3);
              userGroupMapping4.group_id = Convert.ToInt64(this.ddl_usergroup.SelectedItem.Value.ToString());
              this.users.insert_user_group_mapping(userGroupMapping4);
            }
            else
            {
              user_group_mapping userGroupMapping5 = new user_group_mapping();
              userGroupMapping5.user_id = user3.user_id;
              long num = 0;
              foreach (string key in user3.groups.Keys)
              {
                if (key != "All Users")
                  num = user3.groups[key].group_id;
              }
              userGroupMapping5.group_id = num;
              userGroupMapping5.account_id = this.current_user.account_id;
              userGroupMapping5.modified_by = this.current_user.user_id;
              userGroupMapping5.modified_on = this.current_timestamp;
              userGroupMapping5.record_id = user3.record_id;
              user_group_mapping userGroupMapping6 = this.users.delete_user_group_mapping(userGroupMapping5);
              userGroupMapping6.group_id = Convert.ToInt64(this.ddl_usergroup.SelectedItem.Value.ToString());
              this.users.insert_user_group_mapping(userGroupMapping6);
            }
          }
          this.Session["Save"] = (object) "S";
          this.Response.Redirect("users_list.aspx", true);
        }
        else
        {
          this.litError.Text = "PIN is already assigned to someone. Please enter a nunique PIN.";
          this.alt_err.Visible = true;
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  [WebMethod]
  public static string CheckAvailability(string email)
  {
    ILog logger = LogManager.GetLogger("fbs_log");
    string str = "true";
    try
    {
      users_api usersApi = new users_api();
      user user1 = new user();
      user user2 = usersApi.get_user(email);
      if (user2 != null)
      {
        if (user2.user_id > 0L)
          str = "false";
      }
    }
    catch (Exception ex)
    {
      str = "false";
      logger.Error((object) "Error ->", ex);
    }
    return str;
  }
}
