// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.asset_permission
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class asset_permission : core_class
  {
    public long asset_permission_id;
    public long asset_id;
    public long group_id;
    public long user_id;
    public bool is_view;
    public bool is_book;
    public string remarks;
  }
}
