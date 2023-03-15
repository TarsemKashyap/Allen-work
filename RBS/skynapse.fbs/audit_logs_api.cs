// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.audit_logs_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace skynapse.fbs
{
  public class audit_logs_api : api_base
  {
    public DataSet get_logs(Guid record_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_audit_logs where record_id='" + (object) record_id + "' and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_logs(DateTime from, DateTime to, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_audit_logs where created_on between '" + (object) from + "' and '" + (object) to + "' and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_logs(DateTime from, DateTime to, string module, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_audit_logs where created_on between '" + (object) from + "' and '" + (object) to + "' and module_name='" + module + "' and account_id='" + (object) account_id + "' order by created_on desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_logs(
      DateTime from,
      DateTime to,
      string module,
      string action,
      Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_audit_logs where module_name='" + module + "' and action='" + action + "' and created_on between '" + (object) from + "' and '" + (object) to + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_logs(
      string from,
      string to,
      Guid record_id,
      string module,
      string action,
      string status,
      Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_audit_logs where module_name LIKE '" + module + "' and action LIKE '" + action + "' and record_id='" + (object) record_id + "' and status LIKE '" + status + "' and created_on between '" + from + "' and '" + to + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public audit_log get_log(long log_id, Guid account_id)
    {
      audit_log log = new audit_log();
      log.audit_log_id = 0L;
      string str = "select * from sbt_audit_logs where audit_log_id='" + (object) log_id + "' and account_id='" + (object) account_id + "'";
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
              log.audit_log_id = (long) objArray[index1];
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
              log.account_id = (Guid) objArray[index2];
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
              log.created_on = (DateTime) objArray[index3];
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
              log.created_by = (long) objArray[index4];
            }
            catch
            {
            }
          }
          int index5 = index4 + 1;
          try
          {
            log.old_value = new XmlDocument();
            log.old_value.LoadXml("<row></row>");
            if (this.is_valid(objArray[index5]))
            {
              if ((string) objArray[index5] != "")
                log.old_value.LoadXml((string) objArray[index5]);
              else
                log.old_value.LoadXml("<row></row>");
            }
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("XML Error: " + ex.ToString()));
            log.old_value.LoadXml("<row></row>");
          }
          int index6 = index5 + 1;
          try
          {
            log.new_value = new XmlDocument();
            log.new_value.LoadXml("<row></row>");
            if (this.is_valid(objArray[index6]))
            {
              if ((string) objArray[index6] != "")
                log.new_value.LoadXml((string) objArray[index6]);
            }
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("XML Error: " + ex.ToString()));
            log.new_value.LoadXml("<row></row>");
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              log.module_name = (string) objArray[index7];
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
              log.action = (string) objArray[index8];
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
              log.status = (string) objArray[index9];
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
              log.stack_trace = (string) objArray[index10];
            }
            catch
            {
              log.stack_trace = "";
            }
          }
          int num = index10 + 1;
        }
      }
      catch (Exception ex)
      {
        log.audit_log_id = 0L;
        this.log.Error((object) str, ex);
      }
      log.change_details = this.format_data(log.old_value.InnerXml, log.new_value.InnerXml);
      return log;
    }

    public string format_data(string old_xml, string new_xml)
    {
      string str = "";
      string empty = string.Empty;
      try
      {
        bool flag = false;
        if (old_xml != "<row></row>")
        {
          XmlDocument xmlDocument1 = new XmlDocument();
          xmlDocument1.LoadXml(old_xml);
          XmlNode xmlNode1 = xmlDocument1.GetElementsByTagName("row")[0];
          if (new_xml == "<row></row>")
          {
            foreach (XmlNode childNode in xmlNode1.ChildNodes)
            {
              if (childNode.Name != "created_on" && childNode.Name != "created_by")
              {
                flag = true;
                if (childNode.Name == "book_from" || childNode.Name == "book_to" || childNode.Name == "created_on" || childNode.Name == "from_date" || childNode.Name == "to_date")
                  str = str + "<p>Deleted <u>" + childNode.Name + "</u> field with value &quot;<b>" + Convert.ToDateTime(childNode.InnerText.Replace("<", "&lt;").Replace(">", "&gt;")).ToString(api_constants.display_datetime_format) + "</b>&quot;</p>";
                else
                  str = str + "<p>Deleted <u>" + childNode.Name + "</u> field with value &quot;<b>" + childNode.InnerText.Replace("<", "&lt;").Replace(">", "&gt;") + "</b>&quot;</p>";
              }
            }
          }
          else
          {
            XmlDocument xmlDocument2 = new XmlDocument();
            xmlDocument2.LoadXml(new_xml);
            foreach (XmlNode childNode in xmlNode1.ChildNodes)
            {
              if (childNode.Name == "parameter")
              {
                string innerText = xmlDocument2.GetElementsByTagName(childNode.Name)[0].InnerText;
              }
              if (childNode.Name != "created_on" && childNode.Name != "created_by")
              {
                XmlNode xmlNode2 = xmlDocument2.GetElementsByTagName(childNode.Name)[0];
                if (xmlNode2 != null && xmlNode2.InnerText != childNode.InnerText)
                {
                  flag = true;
                  str = str + "<p>Changed <u>" + childNode.Name + "</u> field from &quot;<b>" + childNode.InnerText.Replace("<", "&lt;").Replace(">", "&gt;") + "</b>&quot; to &quot;<b>" + xmlNode2.InnerText.Replace("<", "&lt;").Replace(">", "&gt;") + "</b>&quot;</p>";
                }
              }
            }
          }
        }
        else
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(new_xml);
          foreach (XmlNode childNode in xmlDocument.GetElementsByTagName("row")[0].ChildNodes)
          {
            if (childNode.Name == "parameter")
            {
              string innerText = xmlDocument.GetElementsByTagName(childNode.Name)[0].InnerText;
            }
            if (childNode.Name != "created_on" && childNode.Name != "created_by")
            {
              flag = true;
              if (childNode.Name == "book_from" || childNode.Name == "book_to" || childNode.Name == "created_on" || childNode.Name == "from_date" || childNode.Name == "to_date")
                str = str + "<p>Inserted new <u>" + childNode.Name.Replace("<", "&lt;").Replace(">", "&gt;") + "</u> field with value &quot;<b>" + Convert.ToDateTime(childNode.InnerText.Replace("<", "&lt;").Replace(">", "&gt;")).ToString(api_constants.display_datetime_format) + "</b>&quot;</p>";
              else if (childNode.Name == "account_id")
                str = str ?? "";
              else
                str = str + "<p>Inserted new <u>" + childNode.Name.Replace("<", "&lt;").Replace(">", "&gt;") + "</u> field with value &quot;<b>" + childNode.InnerText.Replace("<", "&lt;").Replace(">", "&gt;") + "</b>&quot;</p>";
            }
          }
        }
        if (!flag)
          str = "<p><i>There were no changes made.</i></p>";
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return str;
    }

    public DataSet get_logs(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      string modulename,
      string action,
      string status,
      string from,
      string to,
      Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,audit_log_id,module_name,action,status," + "created_on,created_by,account_id FROM sbt_audit_logs where " + "account_id='" + (object) account_id + "' and module_name like '%" + modulename + "%' and action like '%" + action + "%' and status like '%" + status + "%' and created_on like '%" + searchkey + "%' and created_by like '%" + searchkey + "%'" + "and created_on between '" + from + "' and '" + to + "') AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + "SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER by audit_log_id) AS Row,module_name,action,status," + "created_on,created_by,account_id FROM sbt_audit_logs) AS t2 WHERE " + "account_id='" + (object) account_id + "' and module_name like '%" + modulename + "%' and action like '%" + action + "%' and status like '%" + status + "%' and created_on like '%" + searchkey + "%' and created_by like '%" + searchkey + "%'" + "and created_on between '" + from + "' and '" + to + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }
  }
}
