// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.holiday
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class holiday : core_class
  {
    public long holiday_id;
    public string holiday_name;
    public DateTime from_date;
    public DateTime to_date;
    public bool repeat;
  }
}
