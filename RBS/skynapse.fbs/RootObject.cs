// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.RootObject
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;

namespace skynapse.fbs
{
  public class RootObject
  {
    public string docDate { get; set; }

    public List<Sentence> sentences { get; set; }
  }
}
