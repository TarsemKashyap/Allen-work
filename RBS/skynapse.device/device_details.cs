// Decompiled with JetBrains decompiler
// Type: skynapse.device.device_details
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;

namespace skynapse.device
{
  [Serializable]
  public class device_details : core_class
  {
    public Guid device_code;
    public long device_id;
    public string device_name;
    public string mac_address;
    public string serial_no;
    public long asset_id;
    public long app_config_id;
    public Dictionary<string, string> config;
  }
}
