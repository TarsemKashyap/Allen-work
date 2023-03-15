// Decompiled with JetBrains decompiler
// Type: visitor_management.card
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;

namespace visitor_management
{
  public class card : core_class
  {
    public long card_id;
    public string card_no;
    public string description;
    public string barcode_location;
    public string qrcode_location;
    public long card_type;
    public long card_category;
    public item _card_type;
    public item _card_category;
    public item _status;
  }
}
