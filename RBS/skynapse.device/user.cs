// Decompiled with JetBrains decompiler
// Type: skynapse.device.user
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.device
{
  [Serializable]
  public class user
  {
    public string name;
    public long user_id;
    public Guid reference_id;
    public bool logged_in;
    public bool is_admin;
    public bool is_super_user;
    public bool can_book;
    public bool is_blacklisted;
  }
}
