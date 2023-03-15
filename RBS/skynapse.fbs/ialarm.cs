// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.ialarm
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Text;

namespace skynapse.fbs
{
  public class ialarm
  {
    public string trigger;
    public string action;
    public string description;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("BEGIN:VALARM");
      stringBuilder.AppendLine("TRIGGER:" + this.trigger);
      stringBuilder.AppendLine("ACTION:" + this.action);
      stringBuilder.AppendLine("DESCRIPTION:" + this.description);
      stringBuilder.AppendLine("END:VALARM");
      return stringBuilder.ToString();
    }

    public string ToXML() => "<alarm>" + "<trigger>" + this.trigger + "</trigger>" + "<action>" + this.action + "</action>" + "<description><![CDATA[" + this.description + "]]></description>" + "</alarm>";
  }
}
