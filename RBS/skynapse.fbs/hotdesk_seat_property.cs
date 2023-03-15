// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.hotdesk_seat_property
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class hotdesk_seat_property : core_class
  {
    public long seat_property_id;
    public string parameter;
    public string value;
    public bool faulty;
    public int status;
    public string remarks;
    public long seat_id;
  }
}
