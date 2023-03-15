// Decompiled with JetBrains decompiler
// Type: visitor_management.visitor
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System;
using System.Collections.Generic;

namespace visitor_management
{
  public class visitor : core_class
  {
    public long visitor_id;
    public string title;
    public string full_name;
    public string email;
    public string telephone;
    public string mobile;
    public string identification;
    public string company_name;
    public string company_address_1;
    public string company_address_2;
    public string city;
    public string country;
    public string division;
    public string department;
    public string designation;
    public string vehicle_number;
    public bool is_banned;
    public string banned_reason;
    public bool is_regular;
    public long visitor_type;
    public long visitor_category;
    public bool is_special;
    public bool is_pre_registered;
    public DateTime pre_registered_from;
    public DateTime pre_registered_to;
    public Dictionary<long, visitor_property> _properties;
    public item _visitor_type;
    public item _visitor_category;
    public item _status;
  }
}
