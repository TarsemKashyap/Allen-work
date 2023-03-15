// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.icalendar
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace skynapse.fbs
{
  public class icalendar
  {
    public string version;
    public string prodid;
    public string calscale;
    public string method;
    public List<ievent> events;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("BEGIN:VCALENDAR");
      stringBuilder.AppendLine("VERSION:" + this.version);
      stringBuilder.AppendLine("PRODID:" + this.prodid);
      stringBuilder.AppendLine("CALSCALE:" + this.calscale);
      stringBuilder.AppendLine("METHOD:" + this.method);
      foreach (ievent ievent in this.events)
        stringBuilder.AppendLine(ievent.ToString());
      stringBuilder.AppendLine("END:VCALENDAR");
      return stringBuilder.ToString();
    }

    public string ToXML()
    {
      string str = "<calendar>" + "<version>" + this.version + "</version>" + "<prodid>" + this.prodid + "</prodid>" + "<calscale>" + this.calscale + "</calscale>" + "<method>" + this.method + "</method>" + "<events>";
      foreach (ievent ievent in this.events)
        str += ievent.ToXML();
      return str + "</events>" + "</calendar>";
    }

    public void set_data(string xml_data)
    {
      XmlDocument doc = new XmlDocument();
      try
      {
        doc.LoadXml(xml_data);
      }
      catch
      {
      }
      if (doc == null)
        return;
      this.events = new List<ievent>();
      this.version = this.get_text(doc, "version");
      this.prodid = this.get_text(doc, "prodid");
      this.calscale = this.get_text(doc, "calscale");
      this.method = this.get_text(doc, "method");
      XmlNodeList elementsByTagName = doc.GetElementsByTagName("event");
      if (elementsByTagName.Count <= 0)
        return;
      foreach (XmlNode xmlNode in elementsByTagName)
        this.events.Add(this.get_event(xmlNode.ChildNodes));
    }

    private ievent get_event(XmlNodeList nodes)
    {
      ievent ievent = new ievent();
      foreach (XmlNode node in nodes)
      {
        if (node.Name == "categories")
          ievent.categories = node.InnerText;
        if (node.Name == "contact")
          ievent.contact = node.InnerText;
        if (node.Name == "description")
          ievent.description = node.InnerText;
        if (node.Name == "dtend")
          ievent.dtend = node.InnerText;
        if (node.Name == "dtstamp")
          ievent.dtstamp = node.InnerText;
        if (node.Name == "dtstart")
          ievent.dtstart = node.InnerText;
        if (node.Name == "location")
          ievent.location = node.InnerText;
        if (node.Name == "organizer")
          ievent.organizer = node.InnerText;
        if (node.Name == "sequence")
          ievent.sequence = node.InnerText;
        if (node.Name == "status")
          ievent.status = node.InnerText;
        if (node.Name == "summary")
          ievent.summary = node.InnerText;
        if (node.Name == "transp")
          ievent.transp = node.InnerText;
        if (node.Name == "uid")
          ievent.uid = node.InnerText;
        if (node.Name == "url")
          ievent.url = node.InnerText;
        if (node.Name == "x_alt_description")
          ievent.x_alt_description = node.InnerText;
        if (node.Name == "alarms")
        {
          ievent.alarms = new List<ialarm>();
          foreach (XmlNode childNode in node.ChildNodes)
            ievent.alarms.Add(this.get_alarm(childNode));
        }
      }
      return ievent;
    }

    private ialarm get_alarm(XmlNode node)
    {
      ialarm alarm = new ialarm();
      foreach (XmlNode xmlNode in node)
      {
        if (xmlNode.Name == "action")
          alarm.action = xmlNode.InnerText;
        if (xmlNode.Name == "description")
          alarm.description = xmlNode.InnerText;
        if (xmlNode.Name == "trigger")
          alarm.trigger = xmlNode.InnerText;
      }
      return alarm;
    }

    private string get_text(XmlDocument doc, string tag_name)
    {
      XmlNodeList elementsByTagName = doc.GetElementsByTagName("version");
      return elementsByTagName.Count > 0 ? elementsByTagName[0].InnerText : "";
    }
  }
}
