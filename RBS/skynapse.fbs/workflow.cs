// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.workflow
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Xml;

namespace skynapse.fbs
{
  public class workflow : core_class
  {
    public long workflow_id;
    public string title;
    public string message;
    public DateTime due_on;
    public long reference_id;
    public short action_status;
    public DateTime action_taken_on;
    public long action_taken_by;
    public string action_remarks;
    public short action_type;
    public long action_owner_id;
    public XmlDocument properties;
  }
}
