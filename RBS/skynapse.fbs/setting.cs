// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.setting
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Xml;

namespace skynapse.fbs
{
  public class setting : core_class
  {
    public long setting_id;
    public string parameter;
    public string value;
    public int status;
    public XmlDocument properties;
  }
}
