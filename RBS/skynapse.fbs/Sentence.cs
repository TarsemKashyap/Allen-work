// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.Sentence
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;

namespace skynapse.fbs
{
  public class Sentence
  {
    public int index { get; set; }

    public List<Token> tokens { get; set; }
  }
}
