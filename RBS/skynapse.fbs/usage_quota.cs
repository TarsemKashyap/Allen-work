// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.usage_quota
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class usage_quota : core_class
  {
    public long user_id;
    public long group_id;
    public double quota_hours;
    public short quota_units;
    public double free_hours;
    public double normal_rate;
    public double peak_rate;
    public double weekend_rate;
    public double public_holiday_rate;
    public double cancel_charges;
    public double noshow_charges;
    public long resource_id;
    public string module_name;
  }
}
