// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.book_slot_item
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;

namespace skynapse.fbs
{
  public class book_slot_item
  {
    public long booking_slot_id;
    public DateTime date;
    public long asset_id;
    public long user_id;
    public string book_slot;
    public Guid account_id;
    public Dictionary<string, bool> book_slot_dic;
  }
}
