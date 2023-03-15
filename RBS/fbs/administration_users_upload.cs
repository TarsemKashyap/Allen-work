// Decompiled with JetBrains decompiler
// Type: administration_users_upload
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_users_upload : fbs_base_page, IRequiresSessionState
{
  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.Server.Transfer("~//unauthorized.aspx");
    string[] strArray1 = File.ReadAllLines(this.Server.MapPath("/users_upload/HRM_SD_20140303_0100.txt"));
    string[] strArray2 = strArray1[0].Split(';');
    List<Dictionary<string, string>> dictionaryList = new List<Dictionary<string, string>>();
    for (int index = 1; index < strArray1.Length; ++index)
    {
      try
      {
        string[] strArray3 = strArray1[index].Split(';');
        user user1 = new user();
        string username = "test" + index.ToString() + "@tts.com";
        user user2 = this.users.get_user(username);
        if (user2.user_id > 0L)
        {
          user2.email = username;
          user2.full_name = strArray2[1].Replace("\"", "").ToLower() == "staff_name" ? strArray3[1].Replace("\"", "") : "";
          user2.username = username;
        }
        else
        {
          user2.account_id = this.current_user.account_id;
          user2.created_by = this.current_user.user_id;
          user2.created_on = this.current_timestamp;
          user2.modified_by = this.current_user.user_id;
          user2.modified_on = this.current_timestamp;
          user2.email = username;
          user2.full_name = strArray2[1].Replace("\"", "").ToLower() == "staff_name" ? strArray3[1].Replace("\"", "") : "";
          user2.locked = false;
          user2.login_type = 1L;
          user2.password_reset_request = true;
          user2.primary_user = false;
          user2.timezone = Convert.ToDouble("8.00");
          user2.user_id = 0L;
          user2.username = username;
          user2.password = "";
          user2.record_id = Guid.NewGuid();
          user2.status = 1L;
          user2.activated = true;
        }
        user2.User_insert_type = false;
        user user3 = this.users.update_user(user2);
        if (user3.user_id > 0L)
        {
          user_property userProperty1 = new user_property();
          Dictionary<string, user_property> userProperties = this.users.get_user_properties(user3.user_id, this.current_user.account_id);
          user_property userProperty2;
          if (userProperties.ContainsKey("staff_title"))
          {
            user_property userProperty3 = userProperties["staff_title"];
            userProperty3.property_name = "staff_title";
            userProperty3.property_value = strArray3[0].Replace("\"", "");
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
            userProperty1.property_value = strArray3[0].Replace("\"", "");
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
            userProperty5.property_value = strArray3[2].Replace("\"", "");
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
            userProperty2.property_value = strArray3[2].Replace("\"", "");
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
            userProperty7.property_value = strArray3[3].Replace("\"", "");
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
            userProperty4.property_value = strArray3[3].Replace("\"", "");
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
            userProperty9.property_value = strArray3[4].Replace("\"", "");
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
            userProperty6.property_value = strArray3[4].Replace("\"", "");
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
            userProperty11.property_value = strArray3[5].Replace("\"", "");
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
            userProperty8.property_value = strArray3[5].Replace("\"", "");
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
            userProperty13.property_value = strArray3[6].Replace("\"", "");
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
            userProperty10.property_value = strArray3[6].Replace("\"", "");
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
            userProperty15.property_value = strArray3[7].Replace("\"", "");
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
            userProperty12.property_value = strArray3[7].Replace("\"", "");
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
            userProperty17.property_value = strArray3[8].Replace("\"", "");
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
            userProperty14.property_value = strArray3[8].Replace("\"", "");
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
            userProperty19.property_value = strArray3[9].Replace("\"", "");
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
            userProperty16.property_value = strArray3[9].Replace("\"", "");
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
            userProperty21.property_value = strArray3[11].Replace("\"", "");
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
            userProperty18.property_value = strArray3[11].Replace("\"", "");
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
            userProperty23.property_value = strArray3[12].Replace("\"", "");
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
            userProperty20.property_value = strArray3[12].Replace("\"", "");
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
            userProperty25.property_value = strArray3[13].Replace("\"", "");
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
            userProperty22.property_value = strArray3[13].Replace("\"", "");
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
            userProperty27.property_value = strArray3[14].Replace("\"", "");
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
            userProperty24.property_value = strArray3[14].Replace("\"", "");
            userProperty24.record_id = user3.record_id;
            userProperty24.user_id = user3.user_id;
            userProperty24.user_property_id = 0L;
            userProperty26 = this.users.update_user_property(userProperty24);
          }
          if (!userProperties.ContainsKey("pin"))
          {
            string checkPin = this.users.generate_check_pin(this.current_user.account_id);
            if (checkPin != "")
            {
              userProperty26.account_id = this.current_user.account_id;
              userProperty26.created_by = this.current_user.user_id;
              userProperty26.created_on = this.current_timestamp;
              userProperty26.modified_by = this.current_user.user_id;
              userProperty26.modified_on = this.current_timestamp;
              userProperty26.property_name = "pin";
              userProperty26.property_value = checkPin;
              userProperty26.record_id = user3.record_id;
              userProperty26.user_id = user3.user_id;
              userProperty26.user_property_id = 0L;
              this.users.update_user_property(userProperty26);
            }
          }
          if (!this.users.get_user_group(user3.user_id, this.current_user.account_id).ContainsKey(api_constants.all_users_text))
          {
            user_group groupByname = this.users.get_group_byname(api_constants.all_users_text, this.current_user.account_id);
            user_group_mapping userGroupMapping = new user_group_mapping();
            userGroupMapping.user_id = user3.user_id;
            userGroupMapping.group_id = groupByname != null ? groupByname.group_id : 1L;
            userGroupMapping.account_id = this.current_user.account_id;
            userGroupMapping.created_by = this.current_user.user_id;
            userGroupMapping.created_on = this.current_timestamp;
            userGroupMapping.modified_by = this.current_user.user_id;
            userGroupMapping.modified_on = this.current_timestamp;
            userGroupMapping.record_id = user3.record_id;
            this.users.insert_user_group_mapping(userGroupMapping);
          }
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Error -> ", ex);
      }
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
