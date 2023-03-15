// Decompiled with JetBrains decompiler
// Type: administration_bulk_user_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_bulk_user_form : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  protected DropDownList ddl_usergroup;
  protected TextBox txt_email;
  protected HiddenField hidUserId;
  protected HiddenField hdnEditUserId;
  protected HiddenField hdnEmail;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (this.IsPostBack)
      return;
    this.bind_ddl_group();
  }

  private void bind_ddl_group()
  {
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.reportings.get_user_group(this.current_user.account_id).Tables[0].Rows)
        this.ddl_usergroup.Items.Add(new ListItem(row["group_name"].ToString(), row["group_id"].ToString()));
      this.ddl_usergroup.Items.Insert(0, new ListItem("Select Group", "0"));
    }
    catch
    {
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    string[] strArray1 = this.txt_email.Text.Split(new string[1]
    {
      Environment.NewLine
    }, StringSplitOptions.None);
    if (strArray1.Length <= 0)
      return;
    user_group groupByname = this.users.get_group_byname(api_constants.all_users_text, this.current_user.account_id);
    DataSet dataSet = this.users.view_user_list(this.current_user.account_id);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='holidylist_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th>Name</th>");
    stringBuilder.Append("<th class='hidden-480'>Email</th>");
    stringBuilder.Append("<th class='hidden-480'>Message</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    stringBuilder.Append("<tbody>");
    foreach (string address in strArray1)
    {
      string str1;
      string str2;
      if (address.Contains("|"))
      {
        string[] strArray2 = address.Split('|');
        str1 = strArray2[1];
        str2 = strArray2[0];
      }
      else
      {
        str1 = address;
        str2 = "User";
      }
      try
      {
        MailAddress mailAddress = new MailAddress(address);
        DataRow[] dataRowArray = dataSet.Tables[0].Select("username='" + str1 + "' or email='" + str1 + "' and status>=0");
        if (dataRowArray.Length > 0)
          stringBuilder.Append("<tr><td>" + str1 + "</td><td>Email already exists for user <a href='user_form.aspx?user_id=" + dataRowArray[0]["user_id"].ToString() + "' target='_blank'><b>" + dataRowArray[0]["full_name"].ToString() + "</b></a>.</td></tr>");
        if (dataRowArray.Length == 0)
        {
          string password = this.utilities.generate_password();
          user user1 = new user();
          user1.account_id = this.current_user.account_id;
          user1.created_by = this.current_user.user_id;
          user1.created_on = this.current_timestamp;
          user1.modified_by = this.current_user.user_id;
          user1.modified_on = this.current_timestamp;
          user1.email = str1.ToLower().Trim();
          user1.password = this.users.get_md5(password);
          user1.full_name = str2;
          user1.locked = false;
          user1.login_type = 2L;
          user1.password_reset_request = true;
          user1.primary_user = false;
          user1.timezone = this.current_account.timezone;
          user1.user_id = 0L;
          user1.username = str1.Trim();
          user1.record_id = Guid.NewGuid();
          user1.status = 1L;
          user1.activated = true;
          user1.User_insert_type = true;
          user user2 = this.users.update_user(user1);
          if (user2.user_id > 0L)
          {
            user_property userProperty = new user_property();
            Dictionary<string, user_property> userProperties = this.users.get_user_properties(user2.user_id, this.current_user.account_id);
            if (!userProperties.ContainsKey("staff_title"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_title";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("given_name"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "given_name";
              userProperty.property_value = str2;
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("native_name"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "native_name";
              userProperty.property_value = str2;
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_id"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_id";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_inst"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_inst";
              userProperty.property_value = this.current_account.name;
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_division"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_division";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_department"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_department";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_section"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_section";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_desg"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_desg";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_offphone"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_offphone";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_pager_mobile"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_pager_mobile";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_comm_date"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_comm_date";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("staff_cess_date"))
            {
              userProperty.account_id = this.current_user.account_id;
              userProperty.created_by = this.current_user.user_id;
              userProperty.created_on = this.current_timestamp;
              userProperty.modified_by = this.current_user.user_id;
              userProperty.modified_on = this.current_timestamp;
              userProperty.property_name = "staff_cess_date";
              userProperty.property_value = "";
              userProperty.record_id = user2.record_id;
              userProperty.user_id = user2.user_id;
              userProperty.user_property_id = 0L;
              userProperty = this.users.update_user_property(userProperty);
            }
            if (!userProperties.ContainsKey("pin"))
            {
              string checkPin = this.users.generate_check_pin(this.current_user.account_id);
              if (checkPin != "")
              {
                userProperty.account_id = this.current_user.account_id;
                userProperty.created_by = this.current_user.user_id;
                userProperty.created_on = this.current_timestamp;
                userProperty.modified_by = this.current_user.user_id;
                userProperty.modified_on = this.current_timestamp;
                userProperty.property_name = "pin";
                userProperty.property_value = checkPin;
                userProperty.record_id = user2.record_id;
                userProperty.user_id = user2.user_id;
                userProperty.user_property_id = 0L;
                this.users.update_user_property(userProperty);
              }
            }
            user_group_mapping userGroupMapping1 = new user_group_mapping()
            {
              user_id = user2.user_id,
              group_id = groupByname == null ? 1L : (groupByname.group_id == 0L ? 1L : groupByname.group_id)
            };
            userGroupMapping1.group_id = groupByname.group_id;
            userGroupMapping1.account_id = this.current_user.account_id;
            userGroupMapping1.created_by = this.current_user.user_id;
            userGroupMapping1.created_on = this.current_timestamp;
            userGroupMapping1.modified_by = this.current_user.user_id;
            userGroupMapping1.modified_on = this.current_timestamp;
            userGroupMapping1.record_id = user2.record_id;
            user_group_mapping userGroupMapping2 = this.users.insert_user_group_mapping(userGroupMapping1);
            userGroupMapping2.group_id = Convert.ToInt64(this.ddl_usergroup.SelectedItem.Value.ToString());
            this.users.insert_user_group_mapping(userGroupMapping2);
            template template1 = new template();
            template template2 = this.tapi.get_template("email_user_add", this.current_user.account_id);
            account account = this.users.get_account(this.current_user.account_id);
            string newValue = this.site_full_path + "assets/img/" + account.logo;
            template2.content_data = template2.content_data.Replace("[password]", password);
            template2.content_data = template2.content_data.Replace("[full_name]", user2.full_name);
            template2.content_data = template2.content_data.Replace("[logo]", newValue);
            template2.content_data = template2.content_data.Replace("[company_name]", account.name);
            template2.content_data = template2.content_data.Replace("[creator]", this.current_user.full_name);
            template2.content_data = template2.content_data.Replace("[login_link]", this.site_full_path + "login.aspx");
            template2.content_data = template2.content_data.Replace("[username]", user2.username);
            template2.content_data = template2.content_data.Replace("[copyright]", Resources.fbs.copyright_text);
            email email = new email();
            if (Convert.ToBoolean(this.current_account.properties["send_email"]))
              this.utilities.sendEmail("", template2.content_data, template2.title, "", user2.email, user2.record_id);
            stringBuilder.Append("<tr><td>" + address + "</td><td>User created.</td></tr>");
          }
        }
      }
      catch (Exception ex)
      {
        stringBuilder.Append("<tr><td>" + address + "</td><td>Invalid. Error: " + ex.ToString() + "</td></tr>");
      }
    }
    stringBuilder.Append("</tbody></table>");
    this.html_table = stringBuilder.ToString();
  }

  protected void ddl_usergroup_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (this.ddl_usergroup.SelectedItem.Value != "0")
      this.btn_submit.Visible = true;
    else
      this.btn_submit.Visible = false;
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
