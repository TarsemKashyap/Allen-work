// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.hotdesk_booking
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class hotdesk_booking : core_class
  {
    public long hotdesk_booking_id;
    public long seat_id;
    public DateTime from_date;
    public DateTime to_date;
    public long layout_id;
    public long booked_for_id;
    public long requested_by;
    public string purpose;
    public string email;
    public int booking_type;
    public int status;
  }
}
