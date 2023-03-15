// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.book_usage
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class book_usage : core_class
  {
    public long usage_id;
    public long booking_id;
    public bool occupied;
    public DateTime occupied_on;
    public long occupied_by;
    public DateTime end_on;
  }
}
