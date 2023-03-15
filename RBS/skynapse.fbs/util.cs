// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.util
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace skynapse.fbs
{
  public class util
  {
    private double AllowedMinutes;
    private user current_user = new user();
    private timezone_api tzapi = new timezone_api();
    private DateTime current_timestamp = DateTime.Now;
    private ILog log = LogManager.GetLogger("fbs_log");

    public util()
    {
      try
      {
        this.AllowedMinutes = Convert.ToDouble(Resources.fbs.Apply_rules_for_minutes);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
    }

    public void set_user(user _user)
    {
      this.current_user = _user;
      this.tzapi.offset = this.current_user.timezone;
      this.current_timestamp = this.tzapi.current_user_timestamp();
    }

    public DateTime TimeRoundUp(DateTime input) => new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0).AddMinutes((double) input.Minute % this.AllowedMinutes == 0.0 ? 0.0 : this.AllowedMinutes - (double) input.Minute % this.AllowedMinutes);

    public int getDuration(DateTime from, DateTime to)
    {
      int duration = 0;
      try
      {
        TimeSpan timeSpan = to - from;
        int hours = timeSpan.Hours;
        duration = timeSpan.Minutes;
        if (hours > 0)
          duration = hours * 60 + duration;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return duration;
    }

    public string get_setting_value(DataTable table, string parameter)
    {
      try
      {
        DataRow[] dataRowArray = table.Select("setting_id='" + parameter + "'");
        return dataRowArray.Length == 0 ? "" : dataRowArray[0]["value"].ToString();
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public int IndexOfOccurence(string s, string match, int occurence)
    {
      int num = 1;
      for (int index = 0; num <= occurence && (index = s.IndexOf(match, index + 1)) != -1; ++num)
      {
        if (num == occurence)
          return index;
      }
      return -1;
    }

    public bool isValidDataset(DataSet ds)
    {
      try
      {
        return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
      }
      catch
      {
        return false;
      }
    }

    public user_group get_group(user objUsr)
    {
      Dictionary<string, user_group> dictionary = new Dictionary<string, user_group>();
      Dictionary<string, user_group> groups = objUsr.groups;
      user_group group = new user_group();
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        if (objUsr.groups.Count >= 1)
        {
          foreach (user_group userGroup in objUsr.groups.Values)
          {
            if (userGroup.group_type != 0)
            {
              if (userGroup.group_type == 1)
              {
                group = userGroup;
                break;
              }
              if (userGroup.group_type == 2)
              {
                group = userGroup;
                flag2 = true;
              }
              else if (!flag1 && !flag2)
                group = userGroup;
            }
          }
        }
        else
        {
          foreach (user_group userGroup in objUsr.groups.Values)
            group = userGroup;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return group;
    }

    public string get_group_ids(user current_user)
    {
      Dictionary<string, user_group> groups = current_user.groups;
      user_group userGroup1 = new user_group();
      string groupIds = "";
      try
      {
        foreach (string key in groups.Keys)
        {
          if (key.ToLower() != api_constants.all_users_text.ToLower())
          {
            user_group userGroup2 = groups[key];
            groupIds = groupIds + userGroup2.group_id.ToString() + ",";
          }
        }
        if (groupIds != "")
          groupIds = groupIds.Substring(0, groupIds.Length - 1);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return groupIds;
    }

    public email sendEmail(
      string bcc,
      string body,
      string subject,
      string cc,
      string to,
      Guid recID)
    {
      email email = new email();
      setting setting1 = new setting();
      this.current_user = (user) HttpContext.Current.Session["user"];
      try
      {
        settings_api settingsApi = new settings_api();
        email.account_id = this.current_user.account_id;
        setting setting2 = settingsApi.get_setting("from_email_address", this.current_user.account_id);
        email.created_on = this.current_timestamp;
        email.modified_on = this.current_timestamp;
        email.bcc_msg = bcc;
        email.body = string.IsNullOrEmpty(body) ? "" : body;
        email.bounced = false;
        email.cc_msg = cc;
        email.created_by = 0L;
        email.email_message_id = Guid.NewGuid();
        email.from_msg = setting2.value;
        email.is_html = true;
        email.message = "";
        email.message_id = 0L;
        email.message_type = 0;
        email.modified_by = 0L;
        email.record_id = recID;
        email.sent = false;
        email.subject = subject;
        email.to_msg = to;
        email = new email_api("smpt").update_email(email);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("Ërror--> " + ex.ToString()));
      }
      return email;
    }

    public string generate_password()
    {
      int num = 8;
      StringBuilder stringBuilder = new StringBuilder();
      Random random = new Random();
      while (0 < num--)
        stringBuilder.Append("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[random.Next("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length)]);
      return stringBuilder.ToString();
    }

    public void Populate_Time(DropDownList cbo, DateTime current_timestamp)
    {
      try
      {
        cbo.Items.Clear();
        string str = current_timestamp.ToShortDateString() + " 00:00 AM";
        DateTime dateTime = new DateTime(current_timestamp.Year, current_timestamp.Month, current_timestamp.Day, 0, 0, 0);
        for (int index = 0; index <= 95; ++index)
        {
          cbo.Items.Add(new ListItem(dateTime.ToShortTimeString(), dateTime.ToString("hh:mm tt")));
          dateTime = dateTime.AddMinutes(this.AllowedMinutes);
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
    }

    public void populate_dropdown(DataSet data, DropDownList ddl, string parameter)
    {
      try
      {
        foreach (DataRow dataRow in data.Tables[0].Select("parameter='" + parameter + "'"))
        {
          ListItem listItem = new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString());
          ddl.Items.Add(listItem);
        }
        ddl.Items.Insert(0, new ListItem("All", "0"));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
    }
  }
}
