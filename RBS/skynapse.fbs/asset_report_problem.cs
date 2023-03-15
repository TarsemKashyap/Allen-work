// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.asset_report_problem
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class asset_report_problem : core_class
  {
    public long problem_id;
    public long asset_id;
    public long Reported_by;
    public string Subject;
    public string Remarks;
    public DateTime Reported_on;
  }
}
