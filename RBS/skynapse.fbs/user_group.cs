// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.user_group
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Xml;

namespace skynapse.fbs
{
  public class user_group : core_class
  {
    public long group_id;
    public long status;
    public string group_name;
    public int group_type;
    public string description;
    public XmlDocument properties;
  }
}
