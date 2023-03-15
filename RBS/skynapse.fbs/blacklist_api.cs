// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.blacklist_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class blacklist_api : api_base
  {
    private users_api users;

    public blacklist_api() => this.users = new users_api();

    private blacklist get_blacklist_object(string sql)
    {
      blacklist blacklistObject = new blacklist();
      blacklistObject.blacklist_id = 0L;
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
              blacklistObject.blacklist_id = (long) objArray[index1];
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
              blacklistObject.account_id = (Guid) objArray[index2];
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
              blacklistObject.created_on = (DateTime) objArray[index3];
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
              blacklistObject.created_by = (long) objArray[index4];
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
              blacklistObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              blacklistObject.modified_on = blacklistObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              blacklistObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              blacklistObject.modified_by = blacklistObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              blacklistObject.blacklist_type = (short) objArray[index7];
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
              blacklistObject.user_id = (long) objArray[index8];
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
              blacklistObject.from_date = (DateTime) objArray[index9];
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
              blacklistObject.to_date = (DateTime) objArray[index10];
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
              blacklistObject.record_id = (Guid) objArray[index11];
            }
            catch
            {
            }
          }
          int num = index11 + 1;
        }
      }
      catch (Exception ex)
      {
        blacklistObject.blacklist_id = 0L;
        this.log.Error((object) sql, ex);
      }
      blacklistObject.user = new user();
      blacklistObject.user = this.users.get_user(blacklistObject.user_id, blacklistObject.account_id);
      return blacklistObject;
    }

    public blacklist get_blacklist_by_user_id(long userid, Guid account_id) => this.get_blacklist_object("select * from sbt_blacklists where user_id='" + (object) userid + "' and account_id='" + (object) account_id + "' order by from_date desc");

    public blacklist get_blacklist(long blacklist_id, Guid account_id) => this.get_blacklist_object("select * from sbt_blacklists where blacklist_id='" + (object) blacklist_id + "' and account_id='" + (object) account_id + "' order by from_date desc");

    public DataSet get_blacklist(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from vw_sbt_blacklists where account_id='" + (object) account_id + "' order by from_date desc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_User_not_in_blacklist(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM(SELECT user_id,username FROM sbt_users WHERE account_id='" + (object) account_id + "' and user_id NOT IN (select user_id from sbt_blacklists where account_id='" + (object) account_id + "')) AS S") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public blacklist update_blacklist(blacklist obj)
    {
      try
      {
        obj.blacklist_id = !this.db.execute_procedure("sbt_sp_blacklist_update", new Dictionary<string, object>()
        {
          {
            "@blacklist_id",
            (object) obj.blacklist_id
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
            "@blacklist_type",
            (object) obj.blacklist_type
          },
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@from_date",
            (object) obj.from_date
          },
          {
            "@to_date",
            (object) obj.to_date
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        obj.blacklist_id = 0L;
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public blacklist delete_blacklist(blacklist obj)
    {
      try
      {
        obj.blacklist_id = !this.db.execute_procedure("sbt_sp_blacklist_delete", new Dictionary<string, object>()
        {
          {
            "@blacklist_id",
            (object) obj.blacklist_id
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
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.blacklist_id = 0L;
      }
      return obj;
    }

    public DataSet get_blacklist(string from, string to, Guid account_id)
    {
      try
      {
        string Sql = "select * from sbt_blacklists where account_id='" + (object) account_id + "'";
        if (from != "" && to != "")
          Sql = Sql + " and '" + from + "' between from_date and to_date" + " or '" + to + "' between from_date and to_date";
        else if (from != "")
          Sql = Sql + " and '" + from + "' between from_date and to_date";
        else if (to != "")
          Sql = Sql + " or '" + to + "' between from_date and to_date";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }
  }
}
