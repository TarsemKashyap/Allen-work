// Decompiled with JetBrains decompiler
// Type: visitor_management.visitor_management_settings_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace visitor_management
{
  public class visitor_management_settings_api : core_api
  {
    public DataSet get_locations(Guid account_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and parameter='location' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_location(Guid account_id, long location_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and setting_id='" + (object) location_id + "' and parameter='location' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public bool delete_location(Guid account_id, long location_id)
    {
      try
      {
        this.db.execute_scalar("update sbt_vms_settings set status=-1 where account_id='" + (object) account_id + "' and setting_id='" + (object) location_id + "' and parameter='location'");
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    public DataSet get_card_types(Guid account_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and parameter='card_type' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_card_type(Guid account_id, long location_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and setting_id='" + (object) location_id + "' and parameter='card_type' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public bool delete_card_type(Guid account_id, long location_id)
    {
      try
      {
        this.db.execute_scalar("update sbt_vms_settings set status=-1 where account_id='" + (object) account_id + "' and setting_id='" + (object) location_id + "' and parameter='card_type'");
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    public DataSet get_card_categories(Guid account_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and parameter='card_category' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_card_category(Guid account_id, long location_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and setting_id='" + (object) location_id + "' and parameter='card_category' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public bool delete_card_category(Guid account_id, long location_id)
    {
      try
      {
        this.db.execute_scalar("update sbt_vms_settings set status=-1 where account_id='" + (object) account_id + "' and setting_id='" + (object) location_id + "' and parameter='card_category'");
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    public DataSet get_all_users(Guid account_id) => this.db.get_dataset("select user_id,full_name,username,email,status from sbt_users where account_id='" + (object) account_id + "' order by full_name") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_user(Guid account_id, long user_id) => this.db.get_dataset("select user_id,full_name,username,email,status from sbt_users where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "' order by full_name") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_vms_users(Guid account_id) => this.db.get_dataset("select * from sbt_vw_vms_users where account_id='" + (object) account_id + "' order by full_name") ? this.db.resultDataSet : (DataSet) null;

    public vms_user_access get_vms_user(Guid account_id, long user_id)
    {
      vms_user_access vmsUser = new vms_user_access();
      vmsUser.vms_user_id = 0L;
      string Sql = "select * from sbt_vw_vms_users where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(Sql);
        if (dataObjects.Count > 0)
          vmsUser = this.get_vms_user_object(dataObjects[0]);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return vmsUser;
    }

    public vms_user_access get_vms_user(Guid code)
    {
      vms_user_access vmsUser = new vms_user_access();
      vmsUser.vms_user_id = 0L;
      string Sql = "select * from sbt_vms_user_access where code='" + (object) code + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(Sql);
        if (dataObjects.Count > 0)
          vmsUser = this.get_vms_user_object(dataObjects[0]);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return vmsUser;
    }

    public DataSet get_vms_user_access(Guid account_id, long vms_user_id) => this.db.get_dataset("select * from sbt_vw_vms_users where account_id='" + (object) account_id + "' and vms_user_id='" + (object) vms_user_id + "' order by full_name") ? this.db.resultDataSet : (DataSet) null;

    public vms_user_access get_vms_user_access_object(Guid account_id, long vms_user_id)
    {
      vms_user_access userAccessObject = new vms_user_access();
      userAccessObject.vms_user_id = 0L;
      string Sql = "select * from sbt_users where vms_user_id='" + (object) vms_user_id + "' and account_id='" + (object) account_id + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(Sql);
        if (dataObjects.Count > 0)
          userAccessObject = this.get_vms_user_object(dataObjects[0]);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return userAccessObject;
    }

    public vms_user_access update_vms_user_access(vms_user_access obj)
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      try
      {
        parameters.Add("@vms_user_id", (object) obj.vms_user_id);
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@modified_by", (object) obj.modified_by);
        parameters.Add("@status", (object) obj.status);
        parameters.Add("@role_id", (object) obj.role_id);
        parameters.Add("@user_id", (object) obj.user_id);
        parameters.Add("@location_id", (object) obj.location_id);
        obj.user_id = !this.db.execute_procedure("sbt_sp_vms_user_access_update", parameters) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    private vms_user_access get_vms_user_object(object[] value)
    {
      vms_user_access vmsUserObject = new vms_user_access();
      vmsUserObject.vms_user_id = 0L;
      int index1 = 0;
      try
      {
        if (this.is_valid(value[index1]))
        {
          try
          {
            vmsUserObject.vms_user_id = (long) value[index1];
          }
          catch
          {
            vmsUserObject.vms_user_id = 0L;
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            vmsUserObject.account_id = (Guid) value[index2];
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
            vmsUserObject.created_on = (DateTime) value[index3];
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
            vmsUserObject.created_by = (long) value[index4];
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
            vmsUserObject.modified_on = (DateTime) value[index5];
          }
          catch
          {
            vmsUserObject.modified_on = vmsUserObject.created_on;
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            vmsUserObject.modified_by = (long) value[index6];
          }
          catch
          {
            vmsUserObject.modified_by = vmsUserObject.created_by;
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            vmsUserObject.role_id = (int) value[index7];
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
            vmsUserObject.user_id = (long) value[index8];
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
            vmsUserObject.status = (long) value[index9];
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
            vmsUserObject.location_id = (long) value[index10];
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
            vmsUserObject.code = new Guid(value[index11].ToString());
          }
          catch
          {
            vmsUserObject.code = Guid.Empty;
          }
        }
      }
      catch
      {
        vmsUserObject.vms_user_id = 0L;
      }
      return vmsUserObject;
    }

    private vms_setting get_setting_object(object[] value)
    {
      vms_setting settingObject = new vms_setting();
      settingObject.setting_id = 0L;
      int index1 = 0;
      if (this.is_valid(value[index1]))
      {
        try
        {
          settingObject.setting_id = (long) value[index1];
        }
        catch
        {
          settingObject.setting_id = 0L;
        }
      }
      int index2 = index1 + 1;
      if (this.is_valid(value[index2]))
      {
        try
        {
          settingObject.account_id = (Guid) value[index2];
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
          settingObject.created_on = (DateTime) value[index3];
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
          settingObject.created_by = (long) value[index4];
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
          settingObject.modified_on = (DateTime) value[index5];
        }
        catch
        {
          settingObject.modified_on = settingObject.created_on;
        }
      }
      int index6 = index5 + 1;
      if (this.is_valid(value[index6]))
      {
        try
        {
          settingObject.modified_by = (long) value[index6];
        }
        catch
        {
          settingObject.modified_by = settingObject.created_by;
        }
      }
      int index7 = index6 + 1;
      if (this.is_valid(value[index7]))
      {
        try
        {
          settingObject.parameter = (string) value[index7];
        }
        catch
        {
          settingObject.parameter = "";
        }
      }
      int index8 = index7 + 1;
      if (this.is_valid(value[index8]))
      {
        try
        {
          settingObject.value = (string) value[index8];
        }
        catch
        {
          settingObject.value = "";
        }
      }
      int index9 = index8 + 1;
      if (this.is_valid(value[index9]))
      {
        try
        {
          settingObject.sort_order = (short) value[index9];
        }
        catch
        {
          settingObject.sort_order = (short) 0;
        }
      }
      int index10 = index9 + 1;
      if (this.is_valid(value[index10]))
      {
        try
        {
          settingObject.status = (long) (int) value[index10];
        }
        catch
        {
        }
      }
      int index11 = index10 + 1;
      if (this.is_valid(value[index11]))
      {
        settingObject.properties = new XmlDocument();
        settingObject.properties.LoadXml("<root></root>");
        try
        {
          settingObject.properties.LoadXml((string) value[index11]);
        }
        catch
        {
          settingObject.properties.LoadXml("<root></root>");
        }
      }
      int index12 = index11 + 1;
      if (this.is_valid(value[index12]))
      {
        try
        {
          settingObject.record_id = (Guid) value[index12];
        }
        catch
        {
          settingObject.record_id = Guid.Empty;
        }
      }
      int index13 = index12 + 1;
      if (this.is_valid(value[index13]))
      {
        try
        {
          settingObject.color = (string) value[index13];
        }
        catch
        {
          settingObject.color = "";
        }
      }
      int num = index13 + 1;
      return settingObject;
    }

    public vms_setting get_setting(long setting_id, Guid account_id)
    {
      vms_setting setting = new vms_setting();
      setting.setting_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_settings where setting_id='" + (object) setting_id + "' and account_id='" + (object) account_id + "'");
      if (dataObjects.Count > 0)
        setting = this.get_setting_object(dataObjects[0]);
      return setting;
    }

    public vms_setting get_setting(string parameter, Guid account_id)
    {
      vms_setting setting = new vms_setting();
      setting.setting_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_settings where parameter='" + parameter + "' and account_id='" + (object) account_id + "'");
      if (dataObjects.Count > 0)
        setting = this.get_setting_object(dataObjects[0]);
      return setting;
    }

    public DataSet get_settings(Guid account_id) => this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' order by parameter,sort_order,value") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_settings(Guid account_id, string parameter) => this.db.get_dataset("select * from sbt_vms_settings where parameter='" + parameter + "' and  account_id='" + (object) account_id + "' and status='1' order by value") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_settings(Guid account_id, string temp, string temp_value)
    {
      this.db.execute_scalar("update sbt_vms_settings set value='" + temp + "' where account_id='" + (object) account_id + "' and parameter='temperature_recording' ;update sbt_settings set value='" + temp_value + "' where account_id='" + (object) account_id + "' and parameter='temperature_threshold' ");
      return (DataSet) null;
    }

    public string get_setting_value(DataSet data, string parameter)
    {
      DataRow[] dataRowArray = data.Tables[0].Select("parameter='" + parameter + "'");
      return dataRowArray.Length > 0 ? dataRowArray[0]["value"].ToString() : "";
    }

    public vms_setting update_setting(vms_setting obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_setting_update", new Dictionary<string, object>()
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
          "@sort_order",
          (object) obj.sort_order
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
          "@color",
          (object) obj.color
        }
      }))
      {
        obj.setting_id = Convert.ToInt64(this.db.resultString);
        return obj;
      }
      obj.setting_id = 0L;
      return obj;
    }

    public bool is_duplicate(Guid account_id, string parameter, string value) => Convert.ToInt64(this.db.execute_scalar("select count(setting_id) from sbt_vms_settings where account_id='" + (object) account_id + "' and parameter='" + parameter + "' and upper(value) like='" + value.ToUpper() + "'")) > 0L;

    public Dictionary<long, string> get_settings_collection(Guid account_id, string param)
    {
      Dictionary<long, string> settingsCollection = new Dictionary<long, string>();
      if (this.db.get_dataset("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and parameter='" + param + "' order by parameter,sort_order,value"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        {
          if (Convert.ToInt32(row["status"]) == 1)
            settingsCollection.Add(Convert.ToInt64(row["setting_id"]), row["value"].ToString());
        }
      }
      return settingsCollection;
    }

    public DataSet get_fields(Guid account_id) => this.db.get_dataset("select * from sbt_vms_fields where account_id='" + (object) account_id + "' order by sort_order") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_field(Guid account_id, long field_id) => this.db.get_dataset("select * from sbt_vms_fields where account_id='" + (object) account_id + "' and field_id='" + (object) field_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public void update_Field(
      Guid account_id,
      long field_id,
      string default_values,
      bool required,
      bool show,
      int sort_order)
    {
      this.db.execute_scalar("update sbt_vms_fields set default_values='" + default_values + "', required='" + (object) required + "', show='" + (object) show + "', sort_order='" + (object) sort_order + "' where account_id='" + (object) account_id + "' and field_id='" + (object) field_id + "'");
    }
  }
}
