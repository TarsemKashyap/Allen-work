// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.timezone_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class timezone_api
  {
    private double _offset;

    public timezone_api() => this._offset = 0.0;

    public timezone_api(double offset) => this._offset = offset;

    public double offset
    {
      get => this._offset;
      set => this._offset = value;
    }

    public DateTime current_timestamp() => DateTime.UtcNow.AddHours(this._offset);

    public DateTime current_timestamp(double offset) => DateTime.UtcNow.AddHours(offset);

    public DateTime current_user_timestamp() => DateTime.UtcNow.AddHours(this._offset);

    public DateTime current_user_timestamp(double offset) => DateTime.UtcNow.AddHours(offset);

    public DateTime convert_to_utc_timestamp(DateTime date) => date.AddHours(this._offset * -1.0);

    public DateTime convert_to_utc_timestamp(DateTime date, double offset) => date.AddHours(offset * -1.0);

    public DateTime convert_to_user_timestamp(DateTime utc_date) => utc_date.AddHours(this._offset);

    public DateTime convert_to_user_timestamp(DateTime utc_date, double offset) => utc_date.AddHours(offset);
  }
}
