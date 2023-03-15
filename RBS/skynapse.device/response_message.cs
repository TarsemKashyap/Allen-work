// Decompiled with JetBrains decompiler
// Type: skynapse.device.response_message
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.device
{
  [Serializable]
  public class response_message
  {
    public string status;
    public Guid dcode;
    public response_error error;
  }
}
