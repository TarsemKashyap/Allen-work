// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.account
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;

namespace skynapse.fbs
{
  public class account : core_class
  {
    public int account_type;
    public long status;
    public string name;
    public string subdomain;
    public DateTime year_start;
    public DateTime activate_date;
    public DateTime expiry_date;
    public string logo;
    public int devices;
    public double timezone;
    public string language;
    public string full_url_page;
    public string info_url_page;
    public string timezone_text;
    public Dictionary<string, string> properties;
  }
}
