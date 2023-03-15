// Decompiled with JetBrains decompiler
// Type: visitor_management.vms_setting
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System.Xml;

namespace visitor_management
{
  public class vms_setting : core_class
  {
    public long setting_id;
    public string parameter;
    public string value;
    public short sort_order;
    public string color;
    public XmlDocument properties;
  }
}
