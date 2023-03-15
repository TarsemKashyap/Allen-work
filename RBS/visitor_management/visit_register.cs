// Decompiled with JetBrains decompiler
// Type: visitor_management.visit_register
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System;

namespace visitor_management
{
  public class visit_register : core_class
  {
    public long register_id;
    public long visitor_id;
    public DateTime from_date;
    public DateTime to_date;
    public long escort_id;
    public string purpose;
    public long purpose_id;
    public bool all_day;
    public bool sms;
    public bool email;
    public long visit_type;
    public long asset_booking_id;
    public string registration_code;
    public int registration_status;
    public int visitor_status;
    public bool invite_sent;
    public item _escort;
    public item _visitor;
  }
}
