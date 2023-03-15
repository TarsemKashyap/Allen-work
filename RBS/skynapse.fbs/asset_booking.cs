// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.asset_booking
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;

namespace skynapse.fbs
{
  public class asset_booking : core_class
  {
    public long booking_id;
    public string purpose;
    public long asset_id;
    public DateTime book_from;
    public DateTime book_to;
    public double book_duration;
    public short status;
    public string remarks;
    public bool is_repeat;
    public Guid repeat_reference_id;
    public bool transfer_request;
    public long transfer_original_booking_id;
    public string transfer_reject_reason;
    public string transfer_reason;
    public DateTime cancel_on;
    public long cancel_by;
    public string cancel_reason;
    public string contact;
    public string email;
    public long booked_for;
    public bool setup_required;
    public long setup_type;
    public bool housekeeping_required;
    public int booking_type;
    public long meeting_type;
    public Guid event_id;
    public int sequence;
    public Dictionary<long, asset_booking_invite> invites;
    public string global_appointment_id;
  }
}
