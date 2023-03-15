// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.attachment
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Xml;

namespace skynapse.fbs
{
  public class attachment : core_class
  {
    public long attachment_id;
    public long message_id;
    public string mime_type;
    public string file_name;
    public string file_extention;
    public byte[] content_data;
    public XmlDocument properties;
  }
}
