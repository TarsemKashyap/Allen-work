// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.resource_booking_item
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class resource_booking_item : core_class
  {
    public long resource_booking_item_id;
    public long resource_booking_id;
    public long resource_id;
    public double req_qty;
    public double accepted_qty;
    public double req_price;
    public double accepted_price;
    public string requestor_remakrs;
    public string other_remarks;
    public string module_name;
    public int status;
  }
}
