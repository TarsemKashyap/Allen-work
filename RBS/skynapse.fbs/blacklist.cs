// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.blacklist
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class blacklist : core_class
  {
    public long blacklist_id;
    public short blacklist_type;
    public long user_id;
    public DateTime from_date;
    public DateTime to_date;
    public user user;
  }
}
