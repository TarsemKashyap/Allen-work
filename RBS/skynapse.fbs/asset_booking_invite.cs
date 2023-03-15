// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.asset_booking_invite
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class asset_booking_invite : core_class
  {
    public long booking_invite_id;
    public string name;
    public string email;
    public long booking_id;
    public int is_attending;
    public Guid repeat_reference_id;
    public DateTime attendance_updated_on;
  }
}
