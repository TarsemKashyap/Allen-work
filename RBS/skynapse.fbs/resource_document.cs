// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.resource_document
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class resource_document : core_class
  {
    public long resource_document_id;
    public string document_name;
    public int? document_size;
    public string document_type;
    public string document_meta;
    public byte[] binary_data;
    public string attachment_type;
    public long resource_item_id;
  }
}
