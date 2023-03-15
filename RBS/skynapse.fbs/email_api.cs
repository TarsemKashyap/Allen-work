// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.email_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace skynapse.fbs
{
  public class email_api : api_base
  {
    public Exception email_exception;
    private SmtpClient smtp_client;

    public email_api(string host) => this.smtp_client = new SmtpClient(host);

    public email_api(SmtpClient client) => this.smtp_client = client;

    public void set_emails(MailAddressCollection mail_address_collection, string to_emails)
    {
      if (string.IsNullOrEmpty(this.format_emails(to_emails)))
        return;
      try
      {
        mail_address_collection.Add(this.format_emails(to_emails));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
    }

    public string format_emails(string emails)
    {
      if (!string.IsNullOrEmpty(emails))
      {
        emails = emails.ToLower();
        try
        {
          string[] strArray = emails.Split(';');
          if (strArray.Length > 0)
          {
            foreach (string input in strArray)
            {
              emails = "";
              if (Regex.IsMatch(input, "\\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\\Z") && !emails.Contains(input))
                emails = emails + input + ";";
              emails = emails.Substring(0, emails.LastIndexOf(';'));
            }
          }
        }
        catch (Exception ex)
        {
          this.log.Error((object) "Error -> ", ex);
        }
      }
      return emails;
    }

    public bool send_email(email obj)
    {
      try
      {
        MailMessage message = new MailMessage();
        message.From = new MailAddress(obj.from_msg);
        message.Subject = obj.subject;
        message.Body = obj.body;
        message.IsBodyHtml = obj.is_html;
        this.set_emails(message.To, obj.to_msg);
        if (!string.IsNullOrEmpty(obj.cc_msg))
          this.set_emails(message.CC, obj.cc_msg);
        if (!string.IsNullOrEmpty(obj.bcc_msg))
          this.set_emails(message.Bcc, obj.bcc_msg);
        this.smtp_client.Send(message);
      }
      catch (Exception ex)
      {
        this.email_exception = ex;
        this.log.Error((object) "Error -> ", ex);
        return false;
      }
      return true;
    }

    public email update_email(email obj)
    {
      try
      {
        obj.message_id = !this.db.execute_procedure("sbt_sp_messaging_update", new Dictionary<string, object>()
        {
          {
            "@message_id",
            (object) obj.message_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@created_by",
            (object) obj.created_by
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@subject",
            (object) obj.subject
          },
          {
            "@body",
            (object) obj.body
          },
          {
            "@from_msg",
            (object) obj.from_msg
          },
          {
            "@to_msg",
            (object) obj.to_msg
          },
          {
            "@cc_msg",
            (object) obj.cc_msg
          },
          {
            "@bcc_msg",
            (object) obj.bcc_msg
          },
          {
            "@is_html",
            (object) obj.is_html
          },
          {
            "@sent",
            (object) obj.sent
          },
          {
            "@message",
            (object) obj.message
          },
          {
            "@email_message_id",
            (object) obj.email_message_id
          },
          {
            "@message_type",
            (object) obj.message_type
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.message_id = 0L;
      }
      return obj;
    }

    public email update_email_after_sent(email obj)
    {
      try
      {
        obj.message_id = !this.db.execute_procedure("sbt_sp_messaging_update_after_sent_mail", new Dictionary<string, object>()
        {
          {
            "@message_id",
            (object) obj.message_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@modified_by",
            (object) obj.modified_by
          },
          {
            "@sent",
            (object) obj.sent
          },
          {
            "@bounced",
            (object) obj.bounced
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@failed_attempts",
            (object) obj.failed_attempts
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.message_id = 0L;
      }
      return obj;
    }

    public email get_email(long email_id, Guid account_id)
    {
      email email = new email();
      email.message_id = 0L;
      string str = "select * from sbt_messaging_logs where message_id='" + (object) email_id + "' and account_id='" + (object) account_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          int index1 = 0;
          if (this.is_valid(objArray[index1]))
          {
            try
            {
              email.message_id = (long) objArray[index1];
            }
            catch
            {
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              email.account_id = (Guid) objArray[index2];
            }
            catch
            {
            }
          }
          int index3 = index2 + 1;
          if (this.is_valid(objArray[index3]))
          {
            try
            {
              email.created_on = (DateTime) objArray[index3];
            }
            catch
            {
            }
          }
          int index4 = index3 + 1;
          if (this.is_valid(objArray[index4]))
          {
            try
            {
              email.created_by = (long) objArray[index4];
            }
            catch
            {
            }
          }
          int index5 = index4 + 1;
          if (this.is_valid(objArray[index5]))
          {
            try
            {
              email.subject = (string) objArray[index5];
            }
            catch
            {
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              email.body = (string) objArray[index6];
            }
            catch
            {
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              email.from_msg = (string) objArray[index7];
            }
            catch
            {
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              email.to_msg = (string) objArray[index8];
            }
            catch
            {
            }
          }
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
          {
            try
            {
              email.cc_msg = (string) objArray[index9];
            }
            catch
            {
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              email.bcc_msg = (string) objArray[index10];
            }
            catch
            {
            }
          }
          int index11 = index10 + 1;
          if (this.is_valid(objArray[index11]))
          {
            try
            {
              email.is_html = (bool) objArray[index11];
            }
            catch
            {
            }
          }
          int index12 = index11 + 1;
          if (this.is_valid(objArray[index12]))
          {
            try
            {
              email.sent = (bool) objArray[index12];
            }
            catch
            {
            }
          }
          int index13 = index12 + 1;
          if (this.is_valid(objArray[index13]))
          {
            try
            {
              email.bounced = (bool) objArray[index13];
            }
            catch
            {
            }
          }
          int index14 = index13 + 1;
          if (this.is_valid(objArray[index14]))
          {
            try
            {
              email.message = (string) objArray[index14];
            }
            catch
            {
            }
          }
          int index15 = index14 + 1;
          if (this.is_valid(objArray[index15]))
          {
            try
            {
              email.record_id = (Guid) objArray[index15];
            }
            catch
            {
            }
          }
          int index16 = index15 + 1;
          if (this.is_valid(objArray[index16]))
          {
            try
            {
              email.email_message_id = (Guid) objArray[index16];
            }
            catch
            {
            }
          }
          int index17 = index16 + 1;
          if (this.is_valid(objArray[index17]))
          {
            try
            {
              email.sent_on = (DateTime) objArray[index17];
            }
            catch
            {
            }
          }
          int index18 = index17 + 1;
          if (this.is_valid(objArray[index18]))
          {
            try
            {
              email.message_type = (int) objArray[index18];
            }
            catch
            {
            }
          }
          int index19 = index18 + 1;
          if (this.is_valid(objArray[index19]))
          {
            try
            {
              email.failed_attempts = (int) objArray[index19];
            }
            catch
            {
            }
          }
          int index20 = index19 + 1;
          if (this.is_valid(objArray[index20]))
          {
            try
            {
              email.last_attempted_on = (DateTime) objArray[index20];
            }
            catch
            {
            }
          }
          int num = index20 + 1;
        }
      }
      catch (Exception ex)
      {
        email.message_id = 0L;
        this.log.Error((object) str, ex);
      }
      return email;
    }

    public DataSet get_emails(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_messaging_logs where account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(Guid record_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_messaging_logs where record_id='" + (object) record_id + "' and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(DateTime from, DateTime to, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_messaging_logs where created_on between '" + (object) from + "' and '" + (object) to + "' and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(string to, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_messaging_logs where to like '%" + to + "%' and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet search_emails(string keyword, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_messaging_logs where (UPPER(subject) like '%" + keyword.ToUpper() + "%' or UPPER(body) like '%" + keyword.ToUpper() + "%') and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet search_emails(DateTime from, DateTime to, string keyword, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_messaging_logs where created_on between '" + (object) from + "' and '" + (object) to + "' and (UPPER(subject) like '%" + keyword.ToUpper() + "%' or UPPER(body) like '%" + keyword.ToUpper() + "%') and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id,
      string status,
      string from,
      string to)
    {
      try
      {
        string Sql;
        if (status == "2")
          Sql = "SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,message_id,from_msg,created_by,to_msg,sent,sent_on,subject," + "account_id FROM sbt_messaging_logs where " + "account_id='" + (object) account_id + "' and  sent_on IS null  and sent!=1 and (from_msg like '%" + searchkey + "%' or to_msg like '%" + searchkey + "%' or subject like '%" + searchkey + "%' or sent_on like '%" + searchkey + "%')  " + "and created_on between '" + from + "' and '" + to + "'   ) AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,message_id,from_msg,to_msg,sent,sent_on,created_on,subject," + "account_id FROM sbt_messaging_logs where account_id='" + (object) account_id + "') AS t2 WHERE " + "account_id='" + (object) account_id + "' and  sent_on IS null and sent!=1 and (from_msg like '%" + searchkey + "%' or to_msg like '%" + searchkey + "%' or subject like '%" + searchkey + "%' or sent_on like '%" + searchkey + "%')  " + "and created_on between '" + from + "' and '" + to + "'";
        else
          Sql = "SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,message_id,created_by,from_msg,to_msg,sent,sent_on,subject," + "account_id FROM sbt_messaging_logs where " + "account_id='" + (object) account_id + "' and  sent like '%" + status + "%' and (from_msg like '%" + searchkey + "%' or to_msg like '%" + searchkey + "%' or subject like '%" + searchkey + "%' or sent_on like '%" + searchkey + "%')  " + "and created_on between '" + from + "' and '" + to + "') AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,message_id,from_msg,to_msg,sent,sent_on,created_on,subject," + "account_id FROM sbt_messaging_logs where account_id='" + (object) account_id + "') AS t2 WHERE " + "account_id='" + (object) account_id + "' and  sent like '%" + status + "%' and (from_msg like '%" + searchkey + "%' or to_msg like '%" + searchkey + "%' or subject like '%" + searchkey + "%' or sent_on like '%" + searchkey + "%') " + "and created_on between '" + from + "' and '" + to + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id,
      string status,
      string from,
      string to,
      long asssetid)
    {
      try
      {
        string Sql;
        if (status == "2")
          Sql = "SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY a." + orderby + " " + orderdir + ") AS Row,a.message_id,a.created_by,a.from_msg,a.to_msg,a.sent,a.sent_on,a.subject," + "a.account_id,a.created_by FROM sbt_messaging_logs a,skynapse_fbs.dbo.sbt_asset_bookings b where " + "a.account_id='" + (object) account_id + "' and  a.sent_on IS null  and a.sent!=1  and (a.from_msg like '%" + searchkey + "%' or a.to_msg like '%" + searchkey + "%' or a.subject like '%" + searchkey + "%' or a.sent_on like '%" + searchkey + "%')  " + "and a.created_on between '" + from + "' and '" + to + "' and  a.record_id=b.record_id and b.asset_id=" + (object) asssetid + " and b.status<>2) AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER BY a." + orderby + " " + orderdir + ") AS Row,message_id,a.from_msg,a.to_msg,a.sent,a.sent_on,a.created_on,a.subject," + "a.account_id FROM sbt_messaging_logs a,skynapse_fbs.dbo.sbt_asset_bookings b WHERE " + "a.account_id='" + (object) account_id + "' and  a.sent_on IS null and a.sent!=1 and (a.from_msg like '%" + searchkey + "%' or a.to_msg like '%" + searchkey + "%' or a.subject like '%" + searchkey + "%' or a.sent_on like '%" + searchkey + "%') " + "and a.created_on between '" + from + "' and '" + to + "'  and  a.record_id=b.record_id and b.asset_id=" + (object) asssetid + " and b.status<>2) AS t2 ";
        else
          Sql = "SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY a." + orderby + " " + orderdir + ") AS Row,a.message_id,a.created_by,a.from_msg,a.to_msg,a.sent,a.sent_on,a.subject," + "a.account_id,a.created_by FROM sbt_messaging_logs a,skynapse_fbs.dbo.sbt_asset_bookings b where " + "a.account_id='" + (object) account_id + "' and  a.sent like '%" + status + "%' and (a.from_msg like '%" + searchkey + "%' or a.to_msg like '%" + searchkey + "%' or a.subject like '%" + searchkey + "%' or a.sent_on like '%" + searchkey + "%')  " + "and a.created_on between '" + from + "' and '" + to + "' and  a.record_id=b.record_id and b.asset_id=" + (object) asssetid + " and b.status<>2) AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER BY a." + orderby + " " + orderdir + ") AS Row,a.message_id,a.from_msg,a.to_msg,a.sent,a.sent_on,a.created_on,a.subject," + "a.account_id FROM sbt_messaging_logs a,skynapse_fbs.dbo.sbt_asset_bookings b WHERE " + "a.account_id='" + (object) account_id + "' and  a.sent like '%" + status + "%' and (a.from_msg like '%" + searchkey + "%' or a.to_msg like '%" + searchkey + "%' or a.subject like '%" + searchkey + "%' or a.sent_on like '%" + searchkey + "%') " + "and a.created_on between '" + from + "' and '" + to + "'  and  a.record_id=b.record_id and b.asset_id=" + (object) asssetid + " and b.status<>2) AS t2 ";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public attachment update_email_attachment(attachment obj)
    {
      try
      {
        obj.attachment_id = !this.db.execute_procedure("sbt_sp_message_attachment_update", new Dictionary<string, object>()
        {
          {
            "@attachment_id",
            (object) obj.attachment_id
          },
          {
            "@message_id",
            (object) obj.message_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@created_by",
            (object) obj.created_by
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@mime_type",
            (object) obj.mime_type
          },
          {
            "@file_name",
            (object) obj.file_name
          },
          {
            "@file_extention",
            (object) obj.file_extention
          },
          {
            "@content_data",
            (object) obj.content_data
          },
          {
            "@properties",
            (object) obj.properties.InnerXml
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        obj.attachment_id = 0L;
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public Dictionary<string, string> get_templates(string name, Guid account_id)
    {
      Dictionary<string, string> templates = new Dictionary<string, string>();
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      string str = "select content_data,title from sbt_templates where name='" + name + "' and account_id='" + (object) account_id + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          foreach (int key in dataObjects.Keys)
          {
            object[] objArray = dataObjects[key];
            if (objArray[0] != null)
            {
              try
              {
                templates.Add("body", (string) objArray[0]);
              }
              catch
              {
                templates.Add("body", "");
              }
            }
            if (objArray[1] != null)
            {
              try
              {
                templates.Add("subject", (string) objArray[1]);
              }
              catch
              {
                templates.Add("subject", (string) objArray[1]);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        templates = new Dictionary<string, string>();
        this.log.Error((object) str, ex);
      }
      return templates;
    }
  }
}
