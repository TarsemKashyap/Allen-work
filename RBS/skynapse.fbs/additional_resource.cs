// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.additional_resource
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class additional_resource : core_class
  {
    public long item_id;
    public string name;
    public int item_type_id;
    public int status;
    public string document_name;
    public int? document_size;
    public string document_type;
    public byte[] binary_data;
    public string module_name;
    public string description;
    public long owner_group_id;
    public Decimal quantity;
    public string unit_of_measure;
    public Decimal price;
  }
}
