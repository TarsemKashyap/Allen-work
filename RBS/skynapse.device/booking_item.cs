// Decompiled with JetBrains decompiler
// Type: skynapse.device.booking_item
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.device
{
  [Serializable]
  public class booking_item
  {
    public long id;
    public string purpose;
    public string requested_by;
    public string booked_for;
    public DateTime start;
    public DateTime end;
    public bool is_blocked;
  }
}
