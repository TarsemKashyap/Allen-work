// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.feedback
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class feedback : core_class
  {
    public long feedback_id;
    public string subject;
    public string description;
    public string email;
    public string responsetext;
    public DateTime dateoffeedback;
    public DateTime dateofrespond;
  }
}
