// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.content_properties
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class content_properties : core_class
  {
    public long content_id { get; set; }

    public string title { get; set; }

    public string content_details { get; set; }

    public long asset_id { get; set; }

    public bool flag { get; set; }

    public bool repeatable { get; set; }

    public DateTime show_from { get; set; }

    public DateTime show_to { get; set; }

    public bool published { get; set; }

    public int type { get; set; }
  }
}
