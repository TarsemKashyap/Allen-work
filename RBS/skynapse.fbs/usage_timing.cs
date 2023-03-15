// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.usage_timing
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class usage_timing : core_class
  {
    public long resource_id;
    public bool book_weekdays;
    public bool book_weekends;
    public bool book_public_holidays;
    public DateTime book_from;
    public DateTime book_to;
    public DateTime peak_from;
    public DateTime peak_to;
    public DateTime block_from;
    public DateTime block_to;
    public int advance_notice_days;
    public int advance_notice_hours;
    public int minimum_minutes;
    public int maximum_minutes;
    public string module_name;
  }
}
