// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.resource_booking
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class resource_booking : core_class
  {
    public long resource_booking_id;
    public long item_id;
    public DateTime book_from;
    public DateTime book_to;
    public long booked_for_id;
    public long requested_by_id;
    public string purpose;
    public string email;
    public long layout_id;
    public int booking_type;
    public long asset_booking_id;
    public string module_name;
    public int status;
    public string remarks;
    public string venue;
    public Guid repeat_reference;
  }
}
