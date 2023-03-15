// Decompiled with JetBrains decompiler
// Type: skynapse.device.response_message_rooms
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;

namespace skynapse.device
{
  [Serializable]
  public class response_message_rooms : response_message
  {
    public List<room_item> data;
  }
}
