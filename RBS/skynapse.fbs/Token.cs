// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.Token
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

namespace skynapse.fbs
{
  public class Token
  {
    public int index { get; set; }

    public string word { get; set; }

    public string originalText { get; set; }

    public string lemma { get; set; }

    public int characterOffsetBegin { get; set; }

    public int characterOffsetEnd { get; set; }

    public string pos { get; set; }

    public string ner { get; set; }

    public string normalizedNER { get; set; }

    public string before { get; set; }

    public string after { get; set; }

    public Timex timex { get; set; }
  }
}
