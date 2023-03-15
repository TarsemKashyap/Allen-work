// Decompiled with JetBrains decompiler
// Type: administration_ad_user_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_ad_user_form : fbs_base_page, IRequiresSessionState
{
  public string password = "";
  public string pwd = "";
  public string mail = "";
  public string displayname = "";
  public string org = "";
  private string logo = "";
  private string companyname = "";
  private users_api uapi = new users_api();
  protected TextBox txt_adAccountid;
  protected Button btnRetrieveData;
  protected Label lblError;
  protected TextBox txt_email;
  protected TextBox txt_full_name;
  protected TextBox txt_institution;
  protected DropDownList ddl_usergroup;
  protected HiddenField hidUserId;
  protected HiddenField hdnEditUserId;
  protected HiddenField hdnEmail;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.Server.Transfer("~//unauthorized.aspx");
    this.current_user.login_type = 1L;
    this.bind_ddl_group();
    if (!this.gp.users_view)
      this.redirect_unauthorized();
    if (this.IsPostBack)
      return;
    if (!string.IsNullOrWhiteSpace(this.Request.QueryString["id"]))
    {
      string username = this.current_user.username;
      this.populate_form(this.users.get_user(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id));
    }
    if (string.IsNullOrWhiteSpace(this.Request.QueryString["user_id"]))
      return;
    this.hdnEditUserId.Value = this.Request.QueryString["user_id"].ToString();
    this.populate_edit_form(this.users.get_user(Convert.ToInt64(this.Request.QueryString["user_id"]), this.current_user.account_id));
  }

  private void populate_form(user obj)
  {
    try
    {
      Dictionary<string, user_property> properties = obj.properties;
      this.txt_institution.Text = properties.ContainsKey("staff_inst") ? properties["staff_inst"].property_value : "";
      this.txt_email.Text = obj.email;
      this.txt_full_name.Text = obj.full_name;
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
      this.txt_institution.Text = properties.ContainsKey("staff_inst") ? properties["staff_inst"].property_value : "";
      this.txt_email.Text = obj.email;
      this.txt_full_name.Text = obj.full_name;
      this.hidUserId.Value = obj.user_id.ToString();
      this.hdnEmail.Value = obj.email;
      this.reportings.get_divison_master(this.current_user.account_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  public void checkADID(string userName)
  {
    users_api usersApi = new users_api();
    try
    {
      if (!Convert.ToBoolean(ConfigurationManager.AppSettings["ActiveDirectory"]))
        return;
      try
      {
        activedirectory activedirectory = new ldap_api(ConfigurationManager.AppSettings["path"], ConfigurationManager.AppSettings["domain"]).is_authenticated(ConfigurationManager.AppSettings["search_field"], userName, this.pwd, ConfigurationManager.AppSettings["return_field"]);
        this.mail = activedirectory.email;
        this.displayname = activedirectory.displayname;
        this.org = activedirectory.organisation;
        this.txt_email.Text = this.mail;
        this.txt_full_name.Text = this.displayname;
        this.txt_institution.Text = this.org;
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Error -> ", ex);
      }
    }
    catch (Exception ex)
    {
    }
  }

  private void bind_ddl_group()
  {
    this.ddl_usergroup.Items.Clear();
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.reportings.get_user_group(this.current_user.account_id).Tables[0].Rows)
        this.ddl_usergroup.Items.Add(new ListItem(row["group_name"].ToString(), row["group_id"].ToString()));
      this.ddl_usergroup.Items.Insert(0, new ListItem("Select Group", ""));
    }
    catch (Exception ex)
    {
    }
  }

  protected void btnRetrieveData_Click(object sender, EventArgs e)
  {
    try
    {
      this.checkADID(this.txt_adAccountid.Text);
    }
    catch (Exception ex)
    {
      this.lblError.Text = ex.Message.ToString();
    }
    if (!(this.mail == ""))
      return;
    this.lblError.Text = "The user doen not exist!!";
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      user user1 = new user();
      user user2 = this.users.get_user(this.txt_email.Text.Trim());
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
          user2.password = "";
          user2.full_name = this.txt_full_name.Text.Trim();
          user2.locked = false;
          user2.login_type = 1L;
          user2.password_reset_request = true;
          user2.primary_user = false;
          user2.timezone = this.current_account.timezone;
          user2.user_id = 0L;
          user2.username = this.txt_adAccountid.Text;
          user2.record_id = Guid.NewGuid();
          user2.status = 1L;
          user2.activated = true;
        }
        user2.User_insert_type = true;
        user user3 = this.users.update_user(user2);
        if (user3.user_id > 0L)
        {
          user_property userProperty1 = new user_property();
          Dictionary<string, user_property> userProperties = this.users.get_user_properties(user3.user_id, this.current_user.account_id);
          user_property userProperty2;
          if (userProperties.ContainsKey("staff_inst"))
          {
            user_property userProperty3 = userProperties["staff_inst"];
            userProperty3.property_name = "staff_inst";
            userProperty3.property_value = this.txt_institution.Text.Trim();
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
            userProperty1.property_name = "staff_inst";
            userProperty1.property_value = this.txt_institution.Text.Trim();
            userProperty1.record_id = user3.record_id;
            userProperty1.user_id = user3.user_id;
            userProperty1.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty1);
          }
          if (!userProperties.ContainsKey("given_name"))
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
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("native_name"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "native_name";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_id"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_id";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_title"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_title";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_division"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_division";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_department"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_department";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_section"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_section";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_desg"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_desg";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_offphone"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_offphone";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_pager_mobile"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_pager_mobile";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_comm_date"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_comm_date";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("staff_cess_date"))
          {
            userProperty2.account_id = this.current_user.account_id;
            userProperty2.created_by = this.current_user.user_id;
            userProperty2.created_on = this.current_timestamp;
            userProperty2.modified_by = this.current_user.user_id;
            userProperty2.modified_on = this.current_timestamp;
            userProperty2.property_name = "staff_cess_date";
            userProperty2.property_value = "";
            userProperty2.record_id = user3.record_id;
            userProperty2.user_id = user3.user_id;
            userProperty2.user_property_id = 0L;
            userProperty2 = this.users.update_user_property(userProperty2);
          }
          if (!userProperties.ContainsKey("pin"))
          {
            string checkPin = this.users.generate_check_pin(this.current_user.account_id);
            if (checkPin != "")
            {
              userProperty2.account_id = this.current_user.account_id;
              userProperty2.created_by = this.current_user.user_id;
              userProperty2.created_on = this.current_timestamp;
              userProperty2.modified_by = this.current_user.user_id;
              userProperty2.modified_on = this.current_timestamp;
              userProperty2.property_name = "pin";
              userProperty2.property_value = checkPin;
              userProperty2.record_id = user3.record_id;
              userProperty2.user_id = user3.user_id;
              userProperty2.user_property_id = 0L;
              this.users.update_user_property(userProperty2);
            }
          }
          if (!this.users.get_user_group(user3.user_id, this.current_user.account_id).ContainsKey(api_constants.all_users_text))
          {
            user_group groupByname = this.users.get_group_byname(api_constants.all_users_text, this.current_user.account_id);
            if (groupByname.group_id > 0L)
            {
              user_group_mapping userGroupMapping1 = new user_group_mapping();
              userGroupMapping1.user_id = user3.user_id;
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
          }
        }
        this.Session["Save"] = (object) "S";
        this.Response.Redirect("users_list.aspx", true);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private string replaceTemplate(string content, user objU) => content;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
