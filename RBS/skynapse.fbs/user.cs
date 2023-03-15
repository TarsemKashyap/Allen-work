// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.user
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;

namespace skynapse.fbs
{
  public class user : core_class
  {
    public long user_id;
    public long status;
    public string username;
    public string password;
    public string full_name;
    public string email;
    public bool activated;
    public long login_type;
    public bool primary_user;
    public bool password_reset_request;
    public int failed_login_count;
    public DateTime last_login_on;
    public Dictionary<string, user_property> properties;
    public Dictionary<string, user_group> groups;
    public double timezone;
    public bool locked;
    public bool User_insert_type;
    public string language;
    public string profile_pic;
    public string country_code;
    public string timezone_text;
  }
}
