// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.settings_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace skynapse.fbs
{
  public class settings_api : api_base
  {
    private DataSet cache_data;
    private DataSet cache_data_full;

    private setting get_setting_object(string sql)
    {
      setting settingObject = new setting();
      settingObject.setting_id = 0L;
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(sql);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          int index1 = 0;
          if (this.is_valid(objArray[index1]))
          {
            try
            {
              settingObject.setting_id = (long) objArray[index1];
            }
            catch
            {
              settingObject.setting_id = 0L;
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              settingObject.account_id = (Guid) objArray[index2];
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
              settingObject.created_on = (DateTime) objArray[index3];
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
              settingObject.created_by = (long) objArray[index4];
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
              settingObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              settingObject.modified_on = settingObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              settingObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              settingObject.modified_by = settingObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              settingObject.parameter = (string) objArray[index7];
            }
            catch
            {
              settingObject.parameter = "";
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              settingObject.value = (string) objArray[index8];
            }
            catch
            {
              settingObject.value = "";
            }
          }
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
          {
            try
            {
              settingObject.status = (int) objArray[index9];
            }
            catch
            {
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            settingObject.properties = new XmlDocument();
            settingObject.properties.LoadXml("<root></root>");
            try
            {
              settingObject.properties.LoadXml((string) objArray[index10]);
            }
            catch
            {
              settingObject.properties.LoadXml("<root></root>");
            }
          }
          int index11 = index10 + 1;
          if (this.is_valid(objArray[index11]))
          {
            try
            {
              settingObject.record_id = (Guid) objArray[index11];
            }
            catch
            {
              settingObject.record_id = Guid.Empty;
            }
          }
          int num = index11 + 1;
        }
      }
      catch (Exception ex)
      {
        settingObject.setting_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return settingObject;
    }

    public setting get_setting(long setting_id, Guid account_id) => this.get_setting_object("select * from sbt_settings where setting_id='" + (object) setting_id + "' and account_id='" + (object) account_id + "' order by value");

    public setting get_setting(string parameter, Guid account_id) => this.get_setting_object("select * from sbt_settings where parameter='" + parameter + "' and account_id='" + (object) account_id + "' order by value");

    public DataSet get_settings(Guid account_id) => this.view_settings(account_id);

    public DataSet get_settings(Guid account_id, string parameter)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_settings where parameter='" + parameter + "' and  account_id='" + (object) account_id + "' order by value") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public setting update_setting(setting obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_setting_update", new Dictionary<string, object>()
        {
          {
            "@setting_id",
            (object) obj.setting_id
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
            "@modified_by",
            (object) obj.modified_by
          },
          {
            "@status",
            (object) obj.status
          },
          {
            "@parameter",
            (object) obj.parameter
          },
          {
            "@value",
            (object) obj.value
          },
          {
            "@properties",
            (object) obj.properties.OuterXml
          },
          {
            "@record_id",
            (object) obj.record_id
          }
        }))
          return obj;
        obj.account_id = Guid.Empty;
        return obj;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      obj.account_id = Guid.Empty;
      if (obj.setting_id > 0L)
        this.capi.remove_cache("settings");
      return obj;
    }

    public setting delete_setting(setting obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_setting_delete", new Dictionary<string, object>()
        {
          {
            "@setting_id",
            (object) obj.setting_id
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
            "@record_id",
            (object) obj.record_id
          }
        }))
          return obj;
        obj.account_id = Guid.Empty;
        return obj;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      obj.account_id = Guid.Empty;
      if (obj.setting_id > 0L)
        this.capi.remove_cache("settings");
      return obj;
    }

    public DataSet view_settings(Guid account_id)
    {
      if (this.cache_data != null)
        return this.cache_data;
      try
      {
        if (!this.db.get_dataset("select setting_id,parameter,value,status,properties,record_id from sbt_settings where account_id='" + (object) account_id + "' order by value"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_settings", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_settings(Guid account_id, string[] parameters)
    {
      if (this.cache_data != null)
        return this.cache_data;
      string str = "";
      if (parameters.Length > 0)
      {
        foreach (string parameter in parameters)
          str = str + "'" + parameter + "',";
        str = str.Substring(0, str.LastIndexOf(','));
      }
      string Sql = "select setting_id,parameter,value,status,properties,record_id from sbt_settings where account_id='" + (object) account_id + "'";
      if (str != "")
        Sql = Sql + " and parameter in (" + str + ")";
      try
      {
        if (!this.db.get_dataset(Sql))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_settings", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }
  }
}
