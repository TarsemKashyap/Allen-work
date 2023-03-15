// Decompiled with JetBrains decompiler
// Type: visitor_management.visit
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System;

namespace visitor_management
{
  public class visit : core_class
  {
    public long visit_id;
    public long visitor_id;
    public long card_id;
    public string purpose;
    public DateTime time_in;
    public DateTime time_out;
    public string notes;
    public string card_number;
    public long purpose_id;
    public visitor _visitor;
    public item _purpose;
    public item _card;
    public string visitor_name;
    public string visitor_company;
    public string visitor_identification;
    public long register_id;
    public string escort;
  }
}
