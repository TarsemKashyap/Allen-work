// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.ievent
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;
using System.Text;

namespace skynapse.fbs
{
  public class ievent
  {
    public string summary;
    public string uid;
    public string sequence;
    public string status;
    public string transp;
    public string dtstart;
    public string dtend;
    public string dtstamp;
    public string categories;
    public string location;
    public string description;
    public string organizer;
    public string contact;
    public string url;
    public string x_alt_description;
    public List<ialarm> alarms;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("BEGIN:VEVENT");
      stringBuilder.AppendLine("SUMMARY:" + this.summary);
      stringBuilder.AppendLine("UID:" + this.uid);
      stringBuilder.AppendLine("SEQUENCE:" + this.sequence);
      stringBuilder.AppendLine("STATUS:" + this.status);
      stringBuilder.AppendLine("TRANSP:" + this.transp);
      stringBuilder.AppendLine("DTSTART:" + this.dtstart);
      stringBuilder.AppendLine("DTEND:" + this.dtend);
      stringBuilder.AppendLine("DTSTAMP:" + this.dtstamp);
      stringBuilder.AppendLine("CATEGORIES:" + this.categories);
      stringBuilder.AppendLine("LOCATION:" + this.location);
      stringBuilder.AppendLine("DESCRIPTION:" + this.description);
      stringBuilder.AppendLine("ORGANIZER; " + this.organizer);
      stringBuilder.AppendLine("CONTACT:" + this.contact);
      stringBuilder.AppendLine("URL:" + this.url);
      stringBuilder.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + this.x_alt_description);
      foreach (ialarm alarm in this.alarms)
        stringBuilder.AppendLine(alarm.ToString());
      stringBuilder.AppendLine("END:VEVENT");
      return stringBuilder.ToString();
    }

    public string ToXML()
    {
      string str = "<event>" + "<summary><![CDATA[" + this.summary + "]]></summary>" + "<uid>" + this.uid + "</uid>" + "<sequence>" + this.sequence + "</sequence>" + "<status>" + this.status + "</status>" + "<transp>" + this.transp + "</transp>" + "<dtstart>" + this.dtstart + "</dtstart>" + "<dtend>" + this.dtend + "</dtend>" + "<dtstamp>" + this.dtstamp + "</dtstamp>" + "<categories>" + this.categories + "</categories>" + "<location><![CDATA[" + this.location + "]]></location>" + "<description><![CDATA[" + this.description + "]]></description>" + "<organizer><![CDATA[" + this.organizer + "]]></organizer>" + "<contact>" + this.contact + "</contact>" + "<url><![CDATA[" + this.url + "]]></url>" + "<x_alt_description><![CDATA[" + this.x_alt_description + "]]></x_alt_description>" + "<alarms>";
      foreach (ialarm alarm in this.alarms)
        str += alarm.ToXML();
      return str + "</alarms>" + "</event>";
    }
  }
}
