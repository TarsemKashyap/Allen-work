// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.users_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace skynapse.fbs
{
  public class users_api : api_base
  {
    private account get_account_object(object[] value)
    {
      account accountObject = new account();
      accountObject.account_id = Guid.Empty;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            accountObject.account_id = (Guid) value[index1];
          }
          catch
          {
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            accountObject.created_on = (DateTime) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            accountObject.created_by = (long) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            accountObject.modified_on = (DateTime) value[index4];
          }
          catch
          {
            accountObject.modified_on = accountObject.created_on;
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            accountObject.modified_by = (long) value[index5];
          }
          catch
          {
            accountObject.modified_by = accountObject.created_by;
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            accountObject.status = (long) value[index6];
          }
          catch
          {
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            accountObject.name = (string) value[index7];
          }
          catch
          {
          }
        }
        int index8 = index7 + 1;
        if (this.is_valid(value[index8]))
        {
          try
          {
            accountObject.account_type = (int) value[index8];
          }
          catch
          {
          }
        }
        int index9 = index8 + 1;
        if (this.is_valid(value[index9]))
        {
          try
          {
            accountObject.expiry_date = (DateTime) value[index9];
          }
          catch
          {
          }
        }
        int index10 = index9 + 1;
        if (this.is_valid(value[index10]))
        {
          try
          {
            accountObject.record_id = (Guid) value[index10];
          }
          catch
          {
          }
        }
        int index11 = index10 + 1;
        if (this.is_valid(value[index11]))
        {
          try
          {
            accountObject.subdomain = (string) value[index11];
          }
          catch
          {
          }
        }
        int index12 = index11 + 1;
        if (this.is_valid(value[index12]))
        {
          try
          {
            accountObject.year_start = (DateTime) value[index12];
          }
          catch
          {
          }
        }
        int index13 = index12 + 1;
        if (this.is_valid(value[index13]))
        {
          try
          {
            accountObject.activate_date = (DateTime) value[index13];
          }
          catch
          {
          }
        }
        int index14 = index13 + 1;
        if (this.is_valid(value[index14]))
        {
          try
          {
            accountObject.logo = (string) value[index14];
          }
          catch
          {
          }
        }
        int index15 = index14 + 1;
        if (this.is_valid(value[index15]))
        {
          try
          {
            accountObject.devices = (int) value[index15];
          }
          catch
          {
          }
        }
        int index16 = index15 + 1;
        if (this.is_valid(value[index16]))
        {
          try
          {
            accountObject.timezone = Convert.ToDouble(value[index16]);
          }
          catch (Exception ex)
          {
          }
        }
        int index17 = index16 + 1;
        if (this.is_valid(value[index17]))
        {
          try
          {
            accountObject.language = (string) value[index17];
          }
          catch
          {
          }
        }
        int index18 = index17 + 1;
        if (this.is_valid(value[index18]))
        {
          try
          {
            accountObject.full_url_page = (string) value[index18];
          }
          catch
          {
          }
        }
        int index19 = index18 + 1;
        if (this.is_valid(value[index19]))
        {
          try
          {
            accountObject.info_url_page = (string) value[index19];
          }
          catch
          {
          }
        }
        int index20 = index19 + 1;
        if (this.is_valid(value[index20]))
        {
          try
          {
            accountObject.timezone_text = (string) value[index20];
          }
          catch
          {
          }
        }
        int num = index20 + 1;
      }
      catch
      {
        accountObject.account_id = Guid.Empty;
      }
      if (accountObject.account_id != Guid.Empty)
        accountObject.properties = this.get_account_properties_list(accountObject.account_id);
      return accountObject;
    }

    public account get_account(Guid account_id)
    {
      account account = new account();
      account.account_id = Guid.Empty;
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_accounts where account_id='" + (object) account_id + "'");
        if (dataObjects.Count > 0)
          account = this.get_account_object(dataObjects[0]);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return account;
    }

    public account get_account(string subdomain)
    {
      account account = new account();
      account.account_id = Guid.Empty;
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_accounts where UPPER(subdomain)='" + subdomain.ToUpper() + "'");
        if (dataObjects.Count > 0)
          account = this.get_account_object(dataObjects[0]);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return account;
    }

    private account_property get_account_property_object(object[] value)
    {
      account_property accountPropertyObject = new account_property();
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            accountPropertyObject.account_id = (Guid) value[index1];
          }
          catch
          {
            accountPropertyObject.account_id = Guid.Empty;
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            accountPropertyObject.parameter = (string) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            accountPropertyObject.value = (string) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            accountPropertyObject.created_on = (DateTime) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            accountPropertyObject.created_by = (long) value[index5];
          }
          catch
          {
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            accountPropertyObject.modified_on = (DateTime) value[index6];
          }
          catch
          {
            accountPropertyObject.modified_on = accountPropertyObject.created_on;
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            accountPropertyObject.modified_by = (long) value[index7];
          }
          catch
          {
            accountPropertyObject.modified_by = accountPropertyObject.created_by;
          }
        }
        int num = index7 + 1;
      }
      catch
      {
      }
      return accountPropertyObject;
    }

    public Dictionary<string, string> get_account_properties_list(Guid account_id)
    {
      Dictionary<string, string> accountPropertiesList = new Dictionary<string, string>();
      if (this.db.get_dataset("select parameter,value from sbt_account_properties where account_id='" + (object) account_id + "'"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        {
          try
          {
            accountPropertiesList.Add(row["parameter"].ToString(), row["value"].ToString());
          }
          catch
          {
          }
        }
      }
      return accountPropertiesList;
    }

    public account_property update_account_property(account_property obj)
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      try
      {
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@modified_on", (object) obj.modified_on);
        parameters.Add("@modified_by", (object) obj.modified_by);
        parameters.Add("@parameter", (object) obj.parameter);
        parameters.Add("@value", (object) obj.value);
        if (this.db.execute_procedure("sbt_sp_account_properties_update", parameters))
          return obj;
        obj.account_id = Guid.Empty;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public DataSet get_user_properties(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select user_property_id,user_id,property_name,property_value from sbt_user_properties where account_id='" + (object) account_id + "' order by user_id") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public Dictionary<string, user_property> get_user_properties(long user_id, Guid account_id)
    {
      Dictionary<string, user_property> userProperties = new Dictionary<string, user_property>();
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      string str = "select * from sbt_user_properties where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          foreach (int key in dataObjects.Keys)
          {
            object[] objArray = dataObjects[key];
            user_property userProperty = new user_property();
            int index1 = 0;
            if (this.is_valid(objArray[index1]))
            {
              try
              {
                userProperty.user_property_id = (long) objArray[index1];
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
                userProperty.user_id = (long) objArray[index2];
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
                userProperty.account_id = (Guid) objArray[index3];
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
                userProperty.created_on = (DateTime) objArray[index4];
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
                userProperty.created_by = (long) objArray[index5];
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
                userProperty.modified_on = (DateTime) objArray[index6];
              }
              catch
              {
                userProperty.modified_on = userProperty.created_on;
              }
            }
            int index7 = index6 + 1;
            if (this.is_valid(objArray[index7]))
            {
              try
              {
                userProperty.modified_by = (long) objArray[index7];
              }
              catch
              {
                userProperty.modified_by = userProperty.created_by;
              }
            }
            int index8 = index7 + 1;
            if (this.is_valid(objArray[index8]))
            {
              try
              {
                userProperty.property_name = (string) objArray[index8];
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
                userProperty.property_value = (string) objArray[index9];
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
                userProperty.record_id = (Guid) objArray[index10];
              }
              catch
              {
              }
            }
            int num = index10 + 1;
            userProperties.Add(userProperty.property_name, userProperty);
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) str, ex);
      }
      return userProperties;
    }

    public user_property update_user_property(user_property obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_user_property_update", new Dictionary<string, object>()
        {
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@user_property_id",
            (object) obj.user_property_id
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
            "@record_id",
            (object) obj.record_id
          },
          {
            "@property_name",
            (object) obj.property_name
          },
          {
            "@property_value",
            (object) obj.property_value
          }
        }))
          return obj;
        obj.user_property_id = 0L;
        return obj;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      obj.user_property_id = 0L;
      return obj;
    }

    public user_property delete_user_property(user_property obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_user_property_delete", new Dictionary<string, object>()
        {
          {
            "@user_property_id",
            (object) obj.user_property_id
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
        obj.user_property_id = 0L;
        return obj;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      obj.user_property_id = 0L;
      return obj;
    }

    public bool delete_user_properties(user_property obj)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_user_properties_delete", new Dictionary<string, object>()
        {
          {
            "@user_property_id",
            (object) obj.user_property_id
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
        });
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public DataSet get_users_by_property(string property_name, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select a.*,b.* from sbt_user_properties a, sbt_users b where a.user_id=b.user_id and a.property_name='" + property_name + "' and a.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public Dictionary<string, user_group> get_user_group(long user_id, Guid account_id)
    {
      Dictionary<string, user_group> userGroup1 = new Dictionary<string, user_group>();
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      string str = "select * from sbt_user_groups where group_id in (select group_id from sbt_user_group_mappings where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "')";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          foreach (int key in dataObjects.Keys)
          {
            object[] objArray = dataObjects[key];
            int index1 = 0;
            user_group userGroup2 = new user_group();
            if (this.is_valid(objArray[index1]))
            {
              try
              {
                userGroup2.group_id = (long) objArray[index1];
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
                userGroup2.account_id = (Guid) objArray[index2];
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
                userGroup2.created_on = (DateTime) objArray[index3];
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
                userGroup2.created_by = (long) objArray[index4];
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
                userGroup2.modified_on = (DateTime) objArray[index5];
              }
              catch
              {
                userGroup2.modified_on = userGroup2.created_on;
              }
            }
            int index6 = index5 + 1;
            if (this.is_valid(objArray[index6]))
            {
              try
              {
                userGroup2.modified_by = (long) objArray[index6];
              }
              catch
              {
                userGroup2.modified_by = userGroup2.created_by;
              }
            }
            int index7 = index6 + 1;
            if (this.is_valid(objArray[index7]))
            {
              try
              {
                userGroup2.status = (long) objArray[index7];
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
                userGroup2.group_name = (string) objArray[index8];
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
                userGroup2.description = (string) objArray[index9];
              }
              catch
              {
              }
            }
            int index10 = index9 + 1;
            userGroup2.properties = new XmlDocument();
            userGroup2.properties.LoadXml("<root></root>");
            if (this.is_valid(objArray[index10]))
            {
              try
              {
                userGroup2.properties.LoadXml((string) objArray[index10]);
              }
              catch
              {
                userGroup2.properties.LoadXml("<root></root>");
              }
            }
            int index11 = index10 + 1;
            if (this.is_valid(objArray[index11]))
            {
              try
              {
                userGroup2.record_id = (Guid) objArray[index11];
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
                userGroup2.group_type = (int) objArray[index12];
              }
              catch
              {
              }
            }
            int num = index12 + 1;
            userGroup1.Add(userGroup2.group_name, userGroup2);
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) str, ex);
      }
      return userGroup1;
    }

    public DataSet get_group_mappings(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select group_id from sbt_user_group_mappings where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_groups(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select [group_id],[status],[group_name],[description],[properties],[record_id],[group_type] from sbt_user_groups where account_id='" + (object) account_id + "' order by group_name asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_groups(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select [group_id],[status],[group_name] from sbt_user_groups where account_id='" + (object) account_id + "' order by group_name asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_user_groups()
    {
      try
      {
        return this.db.get_dataset("select top 4 * from sbt_user_groups ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public user_group get_group(long group_id, Guid account_id) => this.get_group_object("select * from sbt_user_groups where group_id='" + (object) group_id + "' and account_id='" + (object) account_id + "'");

    private user_group get_group_object(string sql)
    {
      user_group groupObject = new user_group();
      groupObject.group_id = 0L;
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
              groupObject.group_id = (long) objArray[index1];
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
              groupObject.account_id = (Guid) objArray[index2];
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
              groupObject.created_on = (DateTime) objArray[index3];
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
              groupObject.created_by = (long) objArray[index4];
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
              groupObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              groupObject.modified_on = groupObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              groupObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              groupObject.modified_by = groupObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              groupObject.status = (long) objArray[index7];
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
              groupObject.group_name = (string) objArray[index8];
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
              groupObject.description = (string) objArray[index9];
            }
            catch
            {
            }
          }
          int index10 = index9 + 1;
          groupObject.properties = new XmlDocument();
          groupObject.properties.LoadXml("<root></root>");
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              groupObject.properties.LoadXml((string) objArray[index10]);
            }
            catch
            {
              groupObject.properties.LoadXml("<root></root>");
            }
          }
          int index11 = index10 + 1;
          if (this.is_valid(objArray[index11]))
          {
            try
            {
              groupObject.record_id = (Guid) objArray[index11];
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
              groupObject.group_type = (int) objArray[index12];
            }
            catch
            {
            }
          }
          int num = index12 + 1;
        }
      }
      catch (Exception ex)
      {
        groupObject.group_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return groupObject;
    }

    public user_group get_group_byname(string group_name, Guid account_id) => this.get_group_object("select * from sbt_user_groups where group_name='" + group_name + "' and account_id='" + (object) account_id + "'");

    public user_group update_group(user_group obj)
    {
      try
      {
        obj.group_id = !this.db.execute_procedure("sp_sp_group_update", new Dictionary<string, object>()
        {
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
            "@group_id",
            (object) obj.group_id
          },
          {
            "@group_name",
            (object) obj.group_name
          },
          {
            "@description",
            (object) obj.description
          },
          {
            "@properties",
            (object) obj.properties.OuterXml
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@group_type",
            (object) obj.group_type
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public user_group delete_group(user_group obj)
    {
      try
      {
        obj.group_id = !this.db.execute_procedure("sp_sp_group_delete", new Dictionary<string, object>()
        {
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@modified_by",
            (object) obj.modified_by
          },
          {
            "@group_id",
            (object) obj.group_id
          },
          {
            "@record_id",
            (object) obj.record_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public DataSet get_group_mapping_details(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from vw_sbt_user_group_mappings where account_id='" + (object) account_id + "' order by group_name,full_name asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public string get_user_name(long user_id, Guid account_id)
    {
      string userName;
      try
      {
        userName = (string) this.db.execute_scalar("select full_name from sbt_users where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'");
      }
      catch (Exception ex)
      {
        userName = "";
        this.log.Error((object) "Error -> ", ex);
      }
      return userName;
    }

    public string get_user_email(long user_id, Guid account_id)
    {
      string userEmail;
      try
      {
        userEmail = (string) this.db.execute_scalar("select email from sbt_users where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'");
      }
      catch (Exception ex)
      {
        userEmail = "";
        this.log.Error((object) "Error -> ", ex);
      }
      return userEmail;
    }

    public groups_permission get_permissions(user obj)
    {
      groups_permission permissions = new groups_permission();
      permissions.group_ids = new List<string>();
      permissions.group_types = new List<string>();
      foreach (string key in obj.groups.Keys)
      {
        user_group group = obj.groups[key];
        XmlDocument properties = group.properties;
        if (key.ToLower() != api_constants.all_users_text.ToLower())
        {
          permissions.group_ids.Add(group.group_id.ToString());
          permissions.group_types.Add(group.group_type.ToString());
          if (group.group_type == 1)
            permissions.isAdminType = true;
          if (group.group_type == 2)
            permissions.isSuperUserType = true;
          try
          {
            if (properties.SelectSingleNode("rights/facilities/view").InnerText == "true" && !permissions.facility_view)
              permissions.facility_view = true;
            if (properties.SelectSingleNode("rights/facilities/add").InnerText == "true" && !permissions.facility_add)
              permissions.facility_add = true;
            if (properties.SelectSingleNode("rights/facilities/edit").InnerText == "true" && !permissions.facility_edit)
              permissions.facility_edit = true;
            if (properties.SelectSingleNode("rights/facilities/delete").InnerText == "true" && !permissions.facility_delete)
              permissions.facility_delete = true;
            permissions.facility_permissions = properties.SelectSingleNode("rights/facilities/permissions") != null && properties.SelectSingleNode("rights/facilities/permissions").InnerText == "true" && !permissions.facility_permissions;
            if (properties.SelectSingleNode("rights/users/view").InnerText == "true" && !permissions.users_view)
              permissions.users_view = true;
            if (properties.SelectSingleNode("rights/users/add").InnerText == "true" && !permissions.users_add)
              permissions.users_add = true;
            if (properties.SelectSingleNode("rights/users/edit").InnerText == "true" && !permissions.users_edit)
              permissions.users_edit = true;
            if (properties.SelectSingleNode("rights/users/delete").InnerText == "true" && !permissions.users_delete)
              permissions.users_delete = true;
            if (properties.SelectSingleNode("rights/users/blacklist").InnerText == "true" && !permissions.users_blacklist)
              permissions.users_blacklist = true;
            if (properties.SelectSingleNode("rights/groups/view").InnerText == "true" && !permissions.groups_view)
              permissions.groups_view = true;
            if (properties.SelectSingleNode("rights/groups/add").InnerText == "true" && !permissions.groups_add)
              permissions.groups_add = true;
            if (properties.SelectSingleNode("rights/groups/edit").InnerText == "true" && !permissions.groups_edit)
              permissions.groups_edit = true;
            if (properties.SelectSingleNode("rights/groups/delete").InnerText == "true" && !permissions.groups_delete)
              permissions.groups_delete = true;
            if (properties.SelectSingleNode("rights/holidays/view").InnerText == "true" && !permissions.holidays_view)
              permissions.holidays_view = true;
            if (properties.SelectSingleNode("rights/holidays/add").InnerText == "true" && !permissions.holidays_add)
              permissions.holidays_add = true;
            if (properties.SelectSingleNode("rights/holidays/edit").InnerText == "true" && !permissions.holidays_edit)
              permissions.holidays_edit = true;
            if (properties.SelectSingleNode("rights/holidays/delete").InnerText == "true" && !permissions.holidays_delete)
              permissions.holidays_delete = true;
            if (properties.SelectSingleNode("rights/holidays/upload").InnerText == "true" && !permissions.holidays_upload)
              permissions.holidays_upload = true;
            if (properties.SelectSingleNode("rights/master/view").InnerText == "true" && !permissions.master_view)
              permissions.master_view = true;
            if (properties.SelectSingleNode("rights/master/add").InnerText == "true" && !permissions.master_add)
              permissions.master_add = true;
            if (properties.SelectSingleNode("rights/master/edit").InnerText == "true" && !permissions.master_edit)
              permissions.master_edit = true;
            if (properties.SelectSingleNode("rights/master/delete").InnerText == "true" && !permissions.master_delete)
              permissions.master_delete = true;
            if (properties.SelectSingleNode("rights/settings/view").InnerText == "true" && !permissions.settings_view)
              permissions.settings_view = true;
            if (properties.SelectSingleNode("rights/settings/view").InnerText == "true" && !permissions.settings_edit)
              permissions.settings_edit = true;
            if (properties.SelectSingleNode("rights/logs/view").InnerText == "true" && !permissions.logs_view)
              permissions.logs_view = true;
            if (properties.SelectSingleNode("rights/emaillogs/view").InnerText == "true" && !permissions.emaillogs_view)
              permissions.emaillogs_view = true;
            if (properties.SelectSingleNode("rights/reports/utilization_by_department/view").InnerText == "true" && !permissions.utilization_report_by_department_view)
              permissions.utilization_report_by_department_view = true;
            if (properties.SelectSingleNode("rights/reports/utilization_by_room/view").InnerText == "true" && !permissions.utilization_report_by_room_view)
              permissions.utilization_report_by_room_view = true;
            if (properties.SelectSingleNode("rights/reports/cancellation/view").InnerText == "true" && !permissions.cancellation_report_view)
              permissions.cancellation_report_view = true;
            if (properties.SelectSingleNode("rights/reports/noshow/view").InnerText == "true" && !permissions.noshow_report_view)
              permissions.noshow_report_view = true;
            if (properties.SelectSingleNode("rights/reports/unassigned/view").InnerText == "true" && !permissions.unassigned_report_view)
              permissions.unassigned_report_view = true;
            if (properties.SelectSingleNode("rights/reports/upcoming_setup/view").InnerText == "true" && !permissions.upcoming_setup_report_view)
              permissions.upcoming_setup_report_view = true;
            if (properties.SelectSingleNode("rights/reports/housekeeping/view").InnerText == "true" && !permissions.housekeeping_report_view)
              permissions.housekeeping_report_view = true;
            if (properties.SelectSingleNode("rights/reports/daily/view").InnerText == "true" && !permissions.daily_report_view)
              permissions.daily_report_view = true;
            if (properties.SelectSingleNode("rights/templates/view").InnerText == "true" && !permissions.templates_view)
              permissions.templates_view = true;
            if (properties.SelectSingleNode("rights/templates/edit").InnerText == "true")
            {
              if (!permissions.templates_edit)
                permissions.templates_edit = true;
            }
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("users_api.get_permissions: " + ex.ToString()));
          }
        }
      }
      return permissions;
    }

    public string get_md5(string password)
    {
      try
      {
        return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(password)));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public string get_user_name(DataSet data, long user_id)
    {
      try
      {
        DataRow[] dataRowArray = data.Tables[0].Select("user_id='" + (object) user_id + "'");
        return dataRowArray.Length == 0 ? "" : dataRowArray[0]["full_name"].ToString();
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public user get_user(string username) => this.get_user_object("select * from sbt_users where LOWER(username) = '" + username.ToLower() + "'");

    public user get_user_by_email(string email) => this.get_user_object("select * from sbt_users where LOWER(email) = '" + email.ToLower() + "'");

    private user get_user_object(string sql)
    {
      user userObject = new user();
      userObject.user_id = 0L;
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
              userObject.user_id = (long) objArray[index1];
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
              userObject.account_id = (Guid) objArray[index2];
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
              userObject.created_on = (DateTime) objArray[index3];
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
              userObject.created_by = (long) objArray[index4];
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
              userObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              userObject.modified_on = userObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              userObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              userObject.modified_by = userObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              userObject.status = (long) objArray[index7];
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
              userObject.username = (string) objArray[index8];
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
              userObject.password = (string) objArray[index9];
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
              userObject.full_name = (string) objArray[index10];
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
              userObject.email = (string) objArray[index11];
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
              userObject.activated = (bool) objArray[index12];
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
              userObject.login_type = (long) (int) objArray[index13];
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
              userObject.primary_user = (bool) objArray[index14];
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
              userObject.password_reset_request = (bool) objArray[index15];
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
              userObject.failed_login_count = (int) objArray[index16];
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
              userObject.last_login_on = (DateTime) objArray[index17];
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
              userObject.timezone = Convert.ToDouble(objArray[index18]);
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
              userObject.record_id = (Guid) objArray[index19];
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
              userObject.locked = (bool) objArray[index20];
            }
            catch
            {
            }
          }
          int index21 = index20 + 1;
          if (this.is_valid(objArray[index21]))
          {
            try
            {
              userObject.User_insert_type = (bool) objArray[index21];
            }
            catch
            {
            }
          }
          int index22 = index21 + 1;
          if (this.is_valid(objArray[index22]))
          {
            try
            {
              userObject.language = (string) objArray[index22];
            }
            catch
            {
            }
          }
          int index23 = index22 + 1;
          if (this.is_valid(objArray[index23]))
          {
            try
            {
              userObject.profile_pic = (string) objArray[index23];
            }
            catch
            {
            }
          }
          int index24 = index23 + 1;
          if (this.is_valid(objArray[index24]))
          {
            try
            {
              userObject.country_code = (string) objArray[index24];
            }
            catch
            {
            }
          }
          int index25 = index24 + 1;
          if (this.is_valid(objArray[index25]))
          {
            try
            {
              userObject.timezone_text = (string) objArray[index25];
            }
            catch
            {
            }
          }
          int num = index25 + 1;
          try
          {
            userObject.properties = this.get_user_properties(userObject.user_id, userObject.account_id);
            userObject.groups = this.get_user_group(userObject.user_id, userObject.account_id);
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        userObject.user_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return userObject;
    }

    public user get_user(long user_id, Guid account_id) => this.get_user_object("select * from sbt_users where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'");

    public DataSet get_users(long user_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_users where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_users");
      if (cache != null)
        return cache;
      try
      {
        if (!this.db.get_dataset("select user_id,status,username,password,full_name,email,activated,login_type,primary_user, password_reset_request, failed_login_count, last_login_on,timezone,record_id,locked,user_insert_type from sbt_users where account_id='" + (object) account_id + "'"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_users", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orderby + " " + orderdir + ") AS RowNumber, a.user_id, a.full_name, a.username, a.email" + ", (CASE a.status WHEN 1 THEN 'Active' ELSE 'Inactive' END) AS Status" + ", (CASE a.locked WHEN 1 THEN 'Yes' ELSE 'No' END) AS Locked" + " FROM sbt_users AS a WHERE account_id='" + (object) account_id + "' AND (full_name LIKE '%" + searchkey + "%' OR email LIKE '%" + searchkey + "%' OR Status LIKE '%" + searchkey + "%' OR Locked LIKE '%" + searchkey + "%')) AS b" + " WHERE RowNumber BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS RecordCnt FROM (SELECT ROW_NUMBER() OVER (Order by full_name) AS RowNumber, a.user_id, a.full_name, a.username, a.email" + ", (CASE a.status WHEN 1 THEN 'Active' ELSE 'Inactive' END) AS Status" + ", (CASE a.locked WHEN 1 THEN 'Yes' ELSE 'No' END) AS Locked" + " FROM sbt_users AS a WHERE account_id='" + (object) account_id + "') AS b" + " WHERE (full_name LIKE '%" + searchkey + "%' OR email LIKE '%" + searchkey + "%' OR Status LIKE '%" + searchkey + "%' OR Locked LIKE '%" + searchkey + "%');") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public string get_users_name(string email)
    {
      try
      {
        return this.db.get_dataset("select full_name from sbt_users where email='" + email + "'") ? this.db.resultDataSet.Tables[0].Rows[0]["full_name"].ToString() : "";
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public DataSet get_users_by_group_type(long group_type, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_users where user_id in " + " (select user_id from sbt_user_group_mappings where group_id in " + " (select group_id from sbt_user_groups where group_type='" + (object) group_type + "' and account_id='" + (object) account_id + "') " + " and account_id='" + (object) account_id + "') " + " and account_id='" + (object) account_id + "' order by username") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users_by_group(long group_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_users where user_id in (select user_id from sbt_user_group_mappings where group_id='" + (object) group_id + "' and account_id='" + (object) account_id + "') order by username") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_usergroup_id(long userid, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT group_id FROM sbt_user_group_mappings WHERE user_id=" + (object) userid + " and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public user authenticate_user_form(string username, string password)
    {
      string str = "select user_id,account_id,password from sbt_users where LOWER(username)='" + username.ToLower() + "'";
      user user = new user();
      user.user_id = 0L;
      try
      {
        password = this.get_md5(password);
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          if ((string) objArray[2] == password)
          {
            user.user_id = (long) objArray[0];
            user.account_id = (Guid) objArray[1];
            this.db.oDataReader.Close();
            this.db.oDataReader.Dispose();
            user = this.get_user(user.user_id, user.account_id);
          }
          else
            this.db.execute_procedure("sbt_sp_login_attempt_update", new Dictionary<string, object>()
            {
              {
                "@user_id",
                (object) user.user_id
              },
              {
                "@account_id",
                (object) user.account_id
              }
            });
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) str, ex);
      }
      return user;
    }

    public string generate_password(int length)
    {
      try
      {
        string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#$%&!";
        string password = "";
        Random random = new Random();
        while (0 < length--)
          password += (string) (object) str[random.Next(str.Length)];
        return password;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public user update_user(user obj)
    {
      try
      {
        obj.user_id = !this.db.execute_procedure("sbt_sp_user_update", new Dictionary<string, object>()
        {
          {
            "@user_id",
            (object) obj.user_id
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
            "@record_id",
            (object) obj.record_id
          },
          {
            "@username",
            (object) obj.username
          },
          {
            "@password",
            (object) obj.password
          },
          {
            "@full_name",
            (object) obj.full_name
          },
          {
            "@email",
            (object) obj.email
          },
          {
            "@activated",
            (object) obj.activated
          },
          {
            "@login_type",
            (object) obj.login_type
          },
          {
            "@primary_user",
            (object) obj.primary_user
          },
          {
            "@password_reset_request",
            (object) obj.password_reset_request
          },
          {
            "@failed_login_count",
            (object) obj.failed_login_count
          },
          {
            "@timezone",
            (object) obj.timezone
          },
          {
            "@locked",
            (object) obj.locked
          },
          {
            "@User_insert_type",
            (object) obj.User_insert_type
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public account update_account(account obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_account_insert", new Dictionary<string, object>()
        {
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
            "@name",
            (object) obj.name
          },
          {
            "@account_type",
            (object) obj.account_type
          },
          {
            "@expiry_date",
            (object) obj.expiry_date
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@subdomain",
            (object) obj.subdomain
          },
          {
            "@year_start",
            (object) obj.year_start
          },
          {
            "@activate_date",
            (object) obj.activate_date
          },
          {
            "@logo",
            (object) obj.logo
          },
          {
            "@devices",
            (object) obj.devices
          },
          {
            "@timezone",
            (object) obj.timezone
          },
          {
            "@language",
            (object) obj.language
          },
          {
            "@full_url_page",
            (object) obj.full_url_page
          },
          {
            "@info_url_page",
            (object) obj.info_url_page
          },
          {
            "@timezone_text",
            (object) obj.timezone_text
          }
        }))
          obj.account_id = new Guid(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public bool update_password(user obj)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_user_password_update", new Dictionary<string, object>()
        {
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@password",
            (object) obj.password
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
        });
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public bool activate_user(long user_id, Guid account_id)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_user_account_activate", new Dictionary<string, object>()
        {
          {
            "@user_id",
            (object) user_id
          },
          {
            "@account_id",
            (object) account_id
          }
        });
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public bool reset_password(string email)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_user_password_reset", new Dictionary<string, object>()
        {
          {
            "@email",
            (object) email
          }
        });
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public bool update_last_login(long user_id, DateTime last_login_on, Guid account_id)
    {
      bool flag = true;
      try
      {
        this.db.get_nonquery("update sbt_users set last_login_on='" + last_login_on.ToString(api_constants.sql_datetime_format) + "' where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'");
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ex.ToString());
      }
      return flag;
    }

    public DataSet get_users_other_all_user_type_view(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_get_users_other_all_user_type_view");
      if (cache != null)
        return cache;
      try
      {
        if (!this.db.get_dataset("select full_name,user_id from sbt_users where user_id in" + " (select user_id from sbt_user_group_mappings where group_id in" + " (select group_id from sbt_user_groups where group_type !=0 and account_id='" + (object) account_id + "')" + " and account_id='" + (object) account_id + "')" + " and account_id='" + (object) account_id + "' order by full_name"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_get_users_other_all_user_type_view", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users_other_all_user_type(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_get_users_other_all_user_type");
      if (cache != null)
        return cache;
      try
      {
        if (!this.db.get_dataset("select user_id,full_name,email,username,status from sbt_users where user_id in" + " (select user_id from sbt_user_group_mappings where full_name <>'' and status=1 and group_id in" + " (select group_id from sbt_user_groups where group_type !=0 and account_id='" + (object) account_id + "')" + ") order by full_name"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_get_users_other_all_user_type", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_user_list(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_view_user_list");
      if (cache != null)
        return cache;
      try
      {
        if (!this.db.get_dataset("select user_id,full_name,email,username,status from sbt_users where account_id='" + (object) account_id + "'"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_view_user_list", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public bool is_blacklisted(DateTime date, Guid account_id, long user_id)
    {
      try
      {
        return this.db.get_dataset("select blacklist_id from sbt_blacklists where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "' and '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "' between from_date and to_date") && this.db.resultDataSet.Tables[0].Rows.Count > 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public user_group_mapping insert_user_group_mapping(user_group_mapping obj)
    {
      try
      {
        obj.user_id = !this.db.execute_procedure("sbt_sp_user_group_mappings_insert", new Dictionary<string, object>()
        {
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
            "@user_id",
            (object) obj.user_id
          },
          {
            "@group_id",
            (object) obj.group_id
          },
          {
            "@record_id",
            (object) obj.record_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public void bulk_remove_from_group(long group_id, Guid account_id) => this.db.execute_scalar("delete from sbt_user_group_mappings where account_id='" + (object) account_id + "' and group_id='" + (object) group_id + "'");

    public user_group_mapping delete_user_group_mapping(user_group_mapping obj)
    {
      try
      {
        obj.user_id = !this.db.execute_procedure("sbt_sp_user_group_mappings_delete", new Dictionary<string, object>()
        {
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@modified_by",
            (object) obj.modified_by
          },
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@group_id",
            (object) obj.group_id
          },
          {
            "@record_id",
            (object) obj.record_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public user delete_user(user obj)
    {
      try
      {
        obj.user_id = !this.db.execute_procedure("sbt_sp_user_delete", new Dictionary<string, object>()
        {
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@modified_by",
            (object) obj.modified_by
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public DataSet get_division_dept_sect(string name, Guid account_id, long parent_id, int type) => this.db.get_dataset("select * from sbt_division_master where name='" + name + "' and parent_id=" + (object) parent_id + " and type=" + (object) type + " and   account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public divison get_division_dept_sect_update(divison obj)
    {
      obj.master_id = !this.db.execute_procedure("sbt_division_master_update", new Dictionary<string, object>()
      {
        {
          "@master_id",
          (object) obj.master_id
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
          "@name",
          (object) obj.name
        },
        {
          "@parent_id",
          (object) obj.parent_id
        },
        {
          "@type",
          (object) obj.type
        },
        {
          "@record_id",
          (object) obj.record_id
        }
      }) ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public void updatecostcentre(string name, string costcnt, Guid account_id) => this.db.execute_scalar("update sbt_division_master set costcentre='" + costcnt + "' where name='" + name + "' and type=3 and account_id='" + (object) account_id + "'");

    public DataSet get_divison_master(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_get_divison_master");
      try
      {
        if (!this.db.get_dataset("SELECT master_id, name, parent_id, type,costcentre FROM sbt_division_master WHERE account_id='" + (object) account_id + "' order by name asc"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_get_divison_master", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users_other_all_user_type_view(string strname, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select full_name, user_id from sbt_users where account_id='" + (object) account_id + "' and user_id in" + " (select user_id from sbt_user_group_mappings where  group_id in" + " (select group_id from sbt_user_groups where group_type !=0 and account_id='" + (object) account_id + "' )" + ")  and full_name <>'' and status=1 and (full_name like '" + strname + "%' ) order by full_name") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users_other_all_user_type(string strname, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select ( full_name  + ' ( '  + username + ' ) ') as uname, user_id from sbt_users where account_id='" + (object) account_id + "' and " + " status=1 and (Lower(full_name) like '%" + strname.ToLower() + "%' or Lower(username) like '%" + strname.ToLower() + "%' or Lower(email) like '%" + strname.ToLower() + "%') order by full_name") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_allusers_not_in_admingroup(long group_id, string strname, Guid account_id)
    {
      try
      {
        string str1 = "1";
        string str2 = "select ( full_name  + ' ( '  + username + ' ) ') as full_name , user_id from sbt_users where account_id='" + (object) account_id + "' and user_id IN " + " (select distinct user_id from sbt_user_group_mappings where group_id IN (select group_id From sbt_user_groups where account_id='" + (object) account_id + "' and group_type NOT IN ('" + str1 + "') )" + " ) and full_name <>'' and status=1 and (full_name like '" + strname + "%' or username like '" + strname + "') " + " and user_id NOT IN ";
        string str3;
        if (group_id > 0L)
          str3 = str2 + " (select distinct user_id from sbt_user_group_mappings where group_id IN (select group_id From sbt_user_groups where account_id='" + (object) account_id + "' and (group_type IN ('" + str1 + "') or group_id IN('" + (object) group_id + "'))))";
        else
          str3 = str2 + " (select distinct user_id from sbt_user_group_mappings where group_id IN (select group_id From sbt_user_groups where account_id='" + (object) account_id + "' and (group_type IN ('" + str1 + "') ) ) )";
        return this.db.get_dataset(str3 + "  order by full_name Asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    private new bool is_valid(object obj) => obj != null && obj != DBNull.Value;

    public DataSet get_users_names_list(Guid account_id) => this.db.get_dataset("select user_id,full_name from sbt_users where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public string Get6Digits_pin()
    {
      byte[] data = new byte[4];
      RandomNumberGenerator.Create().GetBytes(data);
      return string.Format("{0:D6}", (object) (BitConverter.ToUInt32(data, 0) % 1000000U));
    }

    public bool is_duplicate_pin(string pin, Guid account_id)
    {
      object obj = this.db.execute_scalar("select user_property_id from sbt_user_properties where account_id='" + (object) account_id + "' and property_name='" + pin + "'");
      if (obj == null)
        return false;
      long num;
      try
      {
        num = Convert.ToInt64(obj);
      }
      catch
      {
        num = 0L;
      }
      return num > 0L;
    }

    public string generate_check_pin(Guid account_id)
    {
      string pin = this.Get6Digits_pin();
      if (!this.is_duplicate_pin(pin, account_id))
        return pin;
      this.generate_check_pin(account_id);
      return "";
    }

    public DataSet get_users_not_in_mappings(Guid account_id, long group_id)
    {
      try
      {
        return this.db.get_dataset("select b.user_id from sbt_users as b where b.status=1 and b.user_id not in (select user_id from sbt_user_group_mappings where account_id = '" + (object) account_id + "' and group_id='" + (object) group_id + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }
  }
}
