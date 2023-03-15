// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.email
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class email : core_class
  {
    public long message_id;
    public string subject;
    public string body;
    public string from_msg;
    public string to_msg;
    public string cc_msg;
    public string bcc_msg;
    public bool is_html;
    public bool sent;
    public bool bounced;
    public string message;
    public Guid email_message_id;
    public DateTime sent_on;
    public int message_type;
    public int failed_attempts;
    public DateTime last_attempted_on;
  }
}
