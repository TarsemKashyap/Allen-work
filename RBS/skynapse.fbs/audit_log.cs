// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.audit_log
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Xml;

namespace skynapse.fbs
{
  public class audit_log : core_class
  {
    public long audit_log_id;
    public XmlDocument old_value;
    public XmlDocument new_value;
    public string module_name;
    public string action;
    public string status;
    public string stack_trace;
    public string change_details;
  }
}
