// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.hotdesk_layout
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class hotdesk_layout : core_class
  {
    public long layout_id;
    public string image_name;
    public string name;
    public int image_width;
    public int image_height;
    public int status;
    public long building_id;
    public long level_id;
    public long category_id;
  }
}
