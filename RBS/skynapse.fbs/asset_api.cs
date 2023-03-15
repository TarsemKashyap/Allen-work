// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.asset_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

namespace skynapse.fbs
{
  public class asset_api : api_base
  {
    private settings_api settings;
    private users_api users;

    public asset_api()
    {
      this.settings = new settings_api();
      this.users = new users_api();
    }

    private bool check_validation(string sql)
    {
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects(sql);
      bool flag;
      try
      {
        flag = dataObjects.Count <= 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) sql, ex);
        flag = true;
      }
      return flag;
    }

    public bool checknameavilablity(
      int buildingid,
      int levelid,
      string name,
      long asset_id,
      string code,
      Guid account_id)
    {
      return this.check_validation("select distinct name from sbt_assets where account_id='" + (object) account_id + "' and building_id='" + (object) buildingid + "' and level_id='" + (object) levelid + "' and LOWER(name)='" + name.Trim().ToLower() + "' and UPPER(code)='" + code.ToUpper() + "' and asset_id <> " + (object) asset_id);
    }

    public bool checknameavilablity(
      int buildingid,
      int levelid,
      string name,
      long asset_id,
      Guid account_id)
    {
      bool flag = false;
      if (this.db.get_dataset("select distinct name from sbt_assets where building_id='" + (object) buildingid + "' and level_id='" + (object) levelid + "' and LOWER(name)='" + name.Trim().ToLower() + "' and asset_id <> " + (object) asset_id + " and account_id='" + (object) account_id + "'"))
        flag = this.db.resultDataSet.Tables[0].Rows.Count <= 0;
      return flag;
    }

    public bool checknameavilablity_deletemasterdata(int setting_id, string Type, Guid account_id)
    {
      string str = "";
      bool flag;
      try
      {
        switch (Type)
        {
          case "building":
            str = "select asset_id from sbt_assets where account_id='" + (object) account_id + "' and building_id=" + (object) setting_id;
            break;
          case "level":
            str = "select asset_id from sbt_assets where account_id='" + (object) account_id + "' and level_id=" + (object) setting_id;
            break;
          case "category":
            str = "select asset_id from sbt_assets where account_id='" + (object) account_id + "' and  category_id=" + (object) setting_id;
            break;
          case "asset_type":
            str = "select asset_id from sbt_assets where account_id='" + (object) account_id + "' and asset_type=" + (object) setting_id;
            break;
          case "setup_type":
            str = "SELECT asset_property_id FROM sbt_asset_properties WHERE account_id='" + (object) account_id + "' and property_name='setup_type' AND  property_value='" + (object) setting_id + "'";
            break;
          case "asset_property":
            str = "SELECT asset_property_id FROM sbt_asset_properties WHERE account_id='" + (object) account_id + "' and property_name='asset_property' AND  property_value='" + (object) setting_id + "'";
            break;
          case "meeting_type":
            str = "SELECT asset_property_id FROM sbt_asset_properties WHERE account_id='" + (object) account_id + "' and property_name='meeting_type' AND  property_value='" + (object) setting_id + "'";
            break;
        }
        flag = this.check_validation(str);
      }
      catch (Exception ex)
      {
        flag = true;
        this.log.Error((object) str, ex);
      }
      return flag;
    }

    public bool checknameavilablity_deleteFacility(long name, int value, Guid account_id) => this.check_validation("SELECT asset_id FROM sbt_assets where account_id='" + (object) account_id + "' and asset_id=" + (object) name + " and available_for_booking=" + (object) value);

    public DataSet get_faulty_assets(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select asset_id,remarks from sbt_asset_properties where account_id='" + (object) account_id + "' and property_name='asset_property' and available='0' ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets_list(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from vw_sbt_assets where account_id='" + (object) account_id + "' ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets(
      List<long> rooms,
      Guid account_id,
      long building_id,
      long level_id,
      long category_id,
      int capacity,
      string search)
    {
      if (rooms.Count <= 0)
        return (DataSet) null;
      string str1 = "";
      foreach (long room in rooms)
        str1 = str1 + "'" + (object) room + "',";
      string str2 = str1 + "'0'";
      string str3 = "";
      if (building_id > 0L)
        str3 = str3 + " and building_id='" + (object) building_id + "'";
      if (level_id > 0L)
        str3 = str3 + " and level_id='" + (object) level_id + "'";
      if (category_id > 0L)
        str3 = str3 + " and category_id='" + (object) category_id + "'";
      if (capacity > 0)
        str3 = str3 + " and capacity>='" + (object) capacity + "'";
      if (search != "")
        str3 = str3 + " and (name like '%" + search + "%' or code like '%" + search + "%')";
      try
      {
        return this.db.get_dataset("select asset_id,name,code,capacity,building_id,level_id,category_id,asset_type,asset_owner_group_id,status from sbt_assets where account_id='" + (object) account_id + "' and asset_id in(" + str2 + ") and status=1 " + str3 + " order by name ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select asset_id,name,code,capacity,description,available_for_booking,is_restricted,properties,record_id,building_id,level_id,category_id,asset_type,asset_owner_group_id,status from sbt_assets where account_id='" + (object) account_id + "' order by name ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets(Guid account_id, string filter, string colname, string order)
    {
      try
      {
        if (colname == "")
          colname = "building_id";
        string str = "select  * from sbt_assets where account_id='" + (object) account_id + "' ";
        if (!string.IsNullOrEmpty(filter) && filter.Trim() != "%")
          str = str + " and ( building_id IN(SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')" + " OR  level_id IN(SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')  " + " OR  category_id IN (SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')  " + " OR asset_type IN (SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')" + " OR name LIKE '" + filter + "%'   OR code LIKE '" + filter + "%' OR capacity LIKE '" + filter + "%'" + ") ";
        string Sql = str + " order by " + colname + "  " + order + "select  count(*) from sbt_assets where account_id='" + (object) account_id + "' ";
        if (!string.IsNullOrEmpty(filter) && filter.Trim() != "%")
          Sql = Sql + " and ( building_id IN(SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')" + " OR  level_id IN(SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')  " + " OR  category_id IN (SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%') " + " OR asset_type  IN (SELECT setting_id FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and VALUE  LIKE '" + filter + "%')" + "  OR name LIKE '" + filter + "%'   OR code LIKE '" + filter + "%' OR capacity LIKE '" + filter + "%')";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_assets where asset_id='" + (object) asset_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets(
      long building_id,
      long category_id,
      long level_id,
      long capacity,
      Guid account_id)
    {
      try
      {
        string Sql = "select * from sbt_assets where account_id='" + (object) account_id + "' ";
        if (building_id > 0L)
          Sql = Sql + " and building_id='" + (object) building_id + "'";
        if (category_id > 0L)
          Sql = Sql + " and category_id='" + (object) category_id + "'";
        if (level_id > 0L)
          Sql = Sql + " and level_id='" + (object) level_id + "'";
        if (capacity > 0L)
          Sql = Sql + " and capacity >=" + (object) capacity;
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets_by_owner(long owner_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_assets where account_id='" + (object) account_id + "' and asset_owner_group_id='" + (object) owner_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets_booking_slot(
      DateTime start,
      DateTime end,
      long building_id,
      long category_id,
      long level_id,
      long capacity,
      Guid account_id,
      long user_id,
      bool admin_group = false,
      string group_ids = "0")
    {
      try
      {
        int num = admin_group ? 1 : 0;
        string str = "select a.asset_id,a.name,a.code,a.status,a.asset_owner_group_id,a.capacity,a.building_id,a.level_id,(select value from sbt_settings where setting_id=a.building_id) as building_name from sbt_assets a where 1=1 " + " and a.account_id='" + (object) account_id + "'" + " AND a.status=1 ";
        if (building_id > 0L)
          str = str + " and a.building_id='" + (object) building_id + "'";
        if (category_id > 0L)
          str = str + " and a.category_id='" + (object) category_id + "'";
        if (level_id > 0L)
          str = str + " and a.level_id='" + (object) level_id + "'";
        if (capacity > 0L)
          str = str + " and a.capacity >=" + (object) capacity;
        return this.db.get_dataset(str + " ;" + "SELECT max(convert(int,isnull(is_view,0))) is_view, max(convert(int,isnull(is_book,0))) is_book, [asset_id] FROM vw_sbt_asset_user_permissions where account_id='" + (object) account_id + "' and group_id in (" + group_ids + ") group by asset_id;") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public asset get_asset(long asset_id, Guid account_id)
    {
      string str = "select * from sbt_assets where asset_id='" + (object) asset_id + "' and account_id='" + (object) account_id + "'";
      asset asset = new asset();
      asset.asset_id = 0L;
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
              asset.asset_id = (long) objArray[index1];
            }
            catch
            {
              asset.asset_id = 0L;
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              asset.account_id = (Guid) objArray[index2];
            }
            catch
            {
              asset.asset_id = 0L;
            }
          }
          int index3 = index2 + 1;
          if (this.is_valid(objArray[index3]))
          {
            try
            {
              asset.created_on = (DateTime) objArray[index3];
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
              asset.created_by = (long) objArray[index4];
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
              asset.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              asset.modified_on = asset.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              asset.modified_by = (long) objArray[index6];
            }
            catch
            {
              asset.modified_by = asset.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              asset.name = (string) objArray[index7];
            }
            catch
            {
              asset.name = "";
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              asset.code = (string) objArray[index8];
            }
            catch
            {
              asset.code = "";
            }
          }
          else
            asset.code = "";
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
          {
            try
            {
              asset.capacity = (short) objArray[index9];
            }
            catch
            {
              asset.capacity = (short) 0;
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              asset.description = (string) objArray[index10];
            }
            catch
            {
              asset.description = "";
            }
          }
          else
            asset.description = "";
          int index11 = index10 + 1;
          if (this.is_valid(objArray[index11]))
          {
            try
            {
              asset.available_for_booking = (bool) objArray[index11];
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
              asset.is_restricted = (bool) objArray[index12];
            }
            catch
            {
              asset.is_restricted = false;
            }
          }
          int index13 = index12 + 1;
          asset.properties = new XmlDocument();
          asset.properties.LoadXml("<root></root>");
          if (this.is_valid(objArray[index13]))
          {
            try
            {
              asset.properties.LoadXml((string) objArray[index13]);
            }
            catch
            {
              asset.properties.LoadXml("<root></root>");
            }
          }
          int index14 = index13 + 1;
          if (this.is_valid(objArray[index14]))
          {
            try
            {
              asset.record_id = (Guid) objArray[index14];
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
              asset.building_id = (int) objArray[index15];
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
              asset.level_id = (int) objArray[index16];
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
              asset.category_id = (long) objArray[index17];
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
              asset.asset_type = (long) objArray[index18];
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
              asset.asset_owner_group_id = (long) objArray[index19];
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
              asset.status = (short) objArray[index20];
            }
            catch
            {
            }
          }
          int num = index20 + 1;
          asset.building = new setting();
          try
          {
            asset.building = this.settings.get_setting((long) asset.building_id, account_id);
          }
          catch
          {
          }
          asset.level = new setting();
          try
          {
            asset.level = this.settings.get_setting((long) asset.level_id, account_id);
          }
          catch
          {
          }
          asset.category = new setting();
          try
          {
            asset.category = this.settings.get_setting(asset.category_id, account_id);
          }
          catch
          {
          }
          asset.owner_group = new user_group();
          try
          {
            asset.owner_group = this.users.get_group(asset.asset_owner_group_id, account_id);
          }
          catch
          {
          }
          asset.type = new setting();
          try
          {
            asset.type = this.settings.get_setting(asset.asset_type, account_id);
          }
          catch
          {
          }
          asset.status_value = new setting();
          try
          {
            asset.status_value = this.settings.get_setting((long) asset.status, account_id);
          }
          catch
          {
          }
          asset.asset_properties = new Dictionary<long, asset_property>();
          try
          {
            asset.asset_properties = this.get_asset_properties_collection(asset.asset_id, account_id);
          }
          catch
          {
          }
          string doc_ids = "";
          try
          {
            foreach (long key in asset.asset_properties.Keys)
            {
              asset_property assetProperty = asset.asset_properties[key];
              if (assetProperty.property_name == "facility_image")
                doc_ids = doc_ids + assetProperty.property_value + ",";
              if (assetProperty.property_name == "layout_image")
                doc_ids = doc_ids + assetProperty.property_value + ",";
            }
            doc_ids = doc_ids.TrimEnd(',');
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("Error -->" + ex.ToString()));
          }
          asset.documents = new Dictionary<long, asset_document>();
          try
          {
            if (!string.IsNullOrEmpty(doc_ids.Trim()))
              asset.documents = this.get_document_list(asset.asset_id, account_id, doc_ids);
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        asset.asset_id = 0L;
        this.log.Error((object) str, ex);
      }
      return asset;
    }

    private asset_property get_asset_property_object(string sql)
    {
      asset_property assetPropertyObject = new asset_property();
      assetPropertyObject.asset_property_id = 0L;
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
              assetPropertyObject.asset_property_id = (long) objArray[index1];
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
              assetPropertyObject.account_id = (Guid) objArray[index2];
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
              assetPropertyObject.created_on = (DateTime) objArray[index3];
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
              assetPropertyObject.created_by = (long) objArray[index4];
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
              assetPropertyObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              assetPropertyObject.modified_on = assetPropertyObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              assetPropertyObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              assetPropertyObject.modified_by = assetPropertyObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              assetPropertyObject.asset_id = (long) objArray[index7];
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
              assetPropertyObject.property_value = (string) objArray[index8];
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
              assetPropertyObject.record_id = (Guid) objArray[index9];
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
              assetPropertyObject.available = (bool) objArray[index10];
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
              assetPropertyObject.remarks = (string) objArray[index11];
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
              assetPropertyObject.property_name = (string) objArray[index12];
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
              assetPropertyObject.status = (short) objArray[index13];
            }
            catch
            {
            }
          }
          int num = index13 + 1;
        }
      }
      catch (Exception ex)
      {
        assetPropertyObject.asset_property_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return assetPropertyObject;
    }

    public asset_property get_asset_property(
      long asset_id,
      Guid account_id,
      string property_id,
      string property_type)
    {
      return this.get_asset_property_object("select * from sbt_asset_properties where asset_id='" + (object) asset_id + "' and account_id='" + (object) account_id + "' and property_value='" + property_id + "' and property_name='" + property_type + "'");
    }

    public DataSet get_assets(
      Guid account_id,
      string filter,
      string colname,
      string order,
      string startno,
      string endno,
      string group_ids = "0")
    {
      try
      {
        string str1 = "SELECT asset_id,codename,Building,LEVEL,Category ,Type, capacity, Restricted,asset_owner_group_id, Status,isnull(comment,'NA') as comment" + " FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,* from (select  r.* FROM (SELECT asset_id, asset_owner_group_id, (code +' / '+name) AS codename" + ", (SELECT comment =(SELECT (VALUE +'-'+ remarks+', ') from( SELECT property_value, (SELECT VALUE  FROM sbt_settings WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=property_value ) AS VALUE , remarks FROM sbt_asset_properties k WHERE k.account_id='" + (object) account_id + "' and k.property_name='asset_property' AND  k.asset_id=a.asset_id AND remarks <> ''AND  remarks IS NOT null )  AS t FOR XML path(''))) AS comment" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.building_id) AS Building" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.level_id) AS [Level]" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.category_id) AS Category" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.asset_type) AS [Type]" + ", a.capacity" + ", (CASE is_restricted WHEN 1 THEN 'Yes' ELSE 'No' END) AS Restricted" + ", (CASE status WHEN 1 THEN 'Available' ELSE 'Not Available' END) AS [Status]" + " FROM sbt_assets AS a where a.account_id='" + (object) account_id + "' and a.asset_id NOT IN (select asset_id from sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN(" + group_ids + ") " + " group by asset_id having max(convert(int,isnull(is_view,0))) = 0)) AS r) AS result  ";
        if (!string.IsNullOrEmpty(filter) && filter.Trim() != "%")
          str1 = str1 + " WHERE (codename LIKE '%" + filter + "%' OR Building LIKE '%" + filter + "%' OR [Level] LIKE '%" + filter + "%' OR " + " Category LIKE '%" + filter + "%' OR [Type] LIKE '%" + filter + "%' OR capacity LIKE '%" + filter + "%' OR " + " Restricted LIKE '%" + filter + "%' OR [Status] LIKE '%" + filter + "%'  OR comment LIKE '%" + filter + "%')";
        string str2 = str1 + " ) AS Finaloutput " + " where RowNumber BETWEEN " + startno + " AND " + endno + " " + " ;select count(*) from(SELECT codename,Building,LEVEL,Category ,Type, capacity, Restricted,asset_owner_group_id, Status,isnull(comment,'NA') as comment" + " FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,* from (select  r.* FROM (SELECT asset_id, asset_owner_group_id, (code +' / '+name) AS codename" + ", (SELECT comment =(SELECT (VALUE +'-'+ remarks+', ') from( SELECT property_value, (SELECT VALUE  FROM sbt_settings WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=property_value ) AS VALUE , remarks FROM sbt_asset_properties k WHERE k.account_id='" + (object) account_id + "' and  k.property_name='asset_property' AND  k.asset_id=a.asset_id AND remarks <> ''AND  remarks IS NOT null )  AS t FOR XML path(''))) AS comment" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.building_id) AS Building" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.level_id) AS [Level]" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.category_id) AS Category" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.asset_type) AS [Type]" + ", a.capacity" + ", (CASE is_restricted WHEN 1 THEN 'Yes' ELSE 'No' END) AS Restricted" + ", (CASE status WHEN 1 THEN 'Available' ELSE 'Not Available' END) AS [Status]" + "  FROM sbt_assets AS a where a.account_id='" + (object) account_id + "' and a.asset_id NOT IN (select asset_id from sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN(" + group_ids + ") " + "  group by asset_id having max(convert(int,isnull(is_view,0))) = 0)) AS r) AS result  ";
        if (!string.IsNullOrEmpty(filter) && filter.Trim() != "%")
          str2 = str2 + "  WHERE (codename LIKE '%" + filter + "%' OR Building LIKE '%" + filter + "%' OR [Level] LIKE '%" + filter + "%' OR " + "  Category LIKE '%" + filter + "%' OR [Type] LIKE '%" + filter + "%' OR capacity LIKE '%" + filter + "%' OR " + "  Restricted LIKE '%" + filter + "%' OR [Status] LIKE '%" + filter + "%'  OR comment LIKE '%" + filter + "%')";
        return this.db.get_dataset(str2 + "  ) AS Finaloutput " + " where RowNumber BETWEEN 1 AND 1000000) as rrrrrrr ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets(
      Guid account_id,
      string filter,
      string colname,
      string order,
      string startno,
      string endno,
      string asset_owner_group_id,
      string group_ids = "0")
    {
      try
      {
        return this.db.get_dataset("SELECT asset_owner_group_id,asset_id,codename,Building,LEVEL,Category ,Type, capacity, Restricted, Status,isnull(comment,'NA') as comment" + " FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,* from (select  r.* FROM (SELECT asset_id, asset_owner_group_id, (code +' / '+name) AS codename" + ", (SELECT comment =(SELECT (VALUE +'-'+ remarks+', ') from( SELECT property_value, (SELECT VALUE  FROM sbt_settings WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=property_value ) AS VALUE , remarks FROM sbt_asset_properties k WHERE k.account_id='" + (object) account_id + "' and  k.property_name='asset_property' AND  k.asset_id=a.asset_id AND remarks <> ''AND  remarks IS NOT null )  AS t FOR XML path(''))) AS comment" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.building_id) AS Building" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.level_id) AS [Level]" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.category_id) AS Category" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.asset_type) AS [Type]" + ", a.capacity" + ", (CASE is_restricted WHEN 1 THEN 'Yes' ELSE 'No' END) AS Restricted" + ", (CASE status WHEN 1 THEN 'Available' ELSE 'Not Available' END) AS [Status]" + " FROM sbt_assets AS a where a.account_id='" + (object) account_id + "' and  a.asset_id NOT IN (select asset_id from sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN(" + group_ids + ") group by asset_id having max(convert(int,isnull(is_view,0))) = 0)) AS r) AS result WHERE " + " (codename LIKE '%" + filter + "%' OR Building LIKE '%" + filter + "%' OR [Level] LIKE '%" + filter + "%' OR Category LIKE '%" + filter + "%' OR [Type] LIKE '%" + filter + "%' OR capacity LIKE '%" + filter + "%' OR Restricted LIKE '%" + filter + "%' OR [Status] LIKE '%" + filter + "%'  OR comment LIKE '%" + filter + "%')) AS Finaloutput " + " where RowNumber BETWEEN " + startno + " AND " + endno + " " + " ;select count(*) from( SELECT codename,Building,LEVEL,Category ,Type, capacity, Restricted, Status,isnull(comment,'NA') as comment" + " FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,* from (select  r.* FROM (SELECT asset_id, asset_owner_group_id, (code +' / '+name) AS codename" + ", (SELECT comment =(SELECT (VALUE +'-'+ remarks+', ') from( SELECT property_value, (SELECT VALUE  FROM sbt_settings WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=property_value ) AS VALUE , remarks FROM sbt_asset_properties k WHERE  k.property_name='asset_property' AND  k.asset_id=a.asset_id AND remarks <> ''AND  remarks IS NOT null )  AS t FOR XML path(''))) AS comment" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.building_id) AS Building" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.level_id) AS [Level]" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.category_id) AS Category" + ", (SELECT value FROM sbt_settings  WHERE account_id='" + (object) account_id + "' and convert(nvarchar,setting_id)=a.asset_type) AS [Type]" + ", a.capacity" + ", (CASE is_restricted WHEN 1 THEN 'Yes' ELSE 'No' END) AS Restricted" + ", (CASE status WHEN 1 THEN 'Available' ELSE 'Not Available' END) AS [Status]" + " FROM sbt_assets AS a where a.account_id='" + (object) account_id + "' and a.asset_id NOT IN (select asset_id from sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN(" + group_ids + ") group by asset_id having max(convert(int,isnull(is_view,0))) = 0)) AS r) AS result WHERE " + " (codename LIKE '%" + filter + "%' OR Building LIKE '%" + filter + "%' OR [Level] LIKE '%" + filter + "%' OR Category LIKE '%" + filter + "%' OR [Type] LIKE '%" + filter + "%' OR capacity LIKE '%" + filter + "%' OR Restricted LIKE '%" + filter + "%' OR [Status] LIKE '%" + filter + "%'  OR comment LIKE '%" + filter + "%')) AS Finaloutput " + " where RowNumber BETWEEN 1 AND 1000000) as rrrrr") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public asset update_asset(asset obj)
    {
      try
      {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("@asset_id", (object) obj.asset_id);
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@created_by", (object) obj.created_by);
        parameters.Add("@modified_by", (object) obj.modified_by);
        parameters.Add("@record_id", (object) obj.record_id);
        parameters.Add("@name", (object) obj.name);
        parameters.Add("@code", (object) obj.code);
        parameters.Add("@capacity", (object) obj.capacity);
        parameters.Add("@description", (object) obj.description);
        parameters.Add("@available_for_booking", (object) obj.available_for_booking);
        parameters.Add("@is_restricted", (object) obj.is_restricted);
        parameters.Add("@properties", (object) obj.properties.InnerXml);
        parameters.Add("@building_id", (object) obj.building_id);
        parameters.Add("@level_id", (object) obj.level_id);
        parameters.Add("@category_id", (object) obj.category_id);
        parameters.Add("@asset_type", (object) obj.asset_type);
        if (obj.asset_owner_group_id == 0L)
          parameters.Add("@asset_owner_group_id", (object) DBNull.Value);
        else
          parameters.Add("@asset_owner_group_id", (object) obj.asset_owner_group_id);
        parameters.Add("@status", (object) obj.status);
        obj.asset_id = !this.db.execute_procedure("sbt_sp_asset_update", parameters) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public asset delete_asset(asset obj)
    {
      try
      {
        obj.asset_id = !this.db.execute_procedure("sbt_sp_asset_delete", new Dictionary<string, object>()
        {
          {
            "@asset_id",
            (object) obj.asset_id
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
      }
      return obj;
    }

    public DataSet get_asset_properties(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_properties where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public string get_asset_name(long asset_id, Guid account_id)
    {
      string assetName = "";
      try
      {
        if (this.db.get_dataset("select code,name from sbt_assets where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'"))
        {
          if (this.db.resultDataSet.Tables[0].Rows.Count > 0)
            assetName = this.db.resultDataSet.Tables[0].Rows[0]["code"].ToString() + "/" + this.db.resultDataSet.Tables[0].Rows[0]["name"].ToString();
        }
        else
          assetName = "";
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        assetName = "";
      }
      return assetName;
    }

    public DataSet get_asset_properties(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_properties where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_asset_property_by_name(long asset_id, string property_name, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_properties where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and property_name='" + property_name + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public Dictionary<long, asset_property> get_asset_properties_collection(
      long asset_id,
      Guid account_id)
    {
      Dictionary<long, asset_property> propertiesCollection = new Dictionary<long, asset_property>();
      try
      {
        DataSet assetProperties = this.get_asset_properties(asset_id, account_id);
        foreach (DataRow row in (InternalDataCollectionBase) assetProperties.Tables[0].Rows)
        {
          asset_property assetProperty = this.get_asset_property(Convert.ToInt64(row["asset_property_id"]), account_id);
          try
          {
            propertiesCollection.Add(assetProperty.asset_property_id, assetProperty);
          }
          catch
          {
          }
        }
        assetProperties.Dispose();
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return propertiesCollection;
    }

    public string get_property_value(
      Dictionary<long, asset_property> properties,
      string property_name)
    {
      try
      {
        foreach (KeyValuePair<long, asset_property> property in properties)
        {
          asset_property assetProperty = property.Value;
          if (assetProperty.property_name == property_name)
            return assetProperty.property_value;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public asset_property get_asset_property(long asset_property_id, Guid account_id) => this.get_asset_property_object("select * from sbt_asset_properties where asset_property_id='" + (object) asset_property_id + "' and account_id='" + (object) account_id + "'");

    public asset_property update_asset_property(asset_property obj)
    {
      try
      {
        obj.asset_property_id = !this.db.execute_procedure("sbt_sp_asset_property_update", new Dictionary<string, object>()
        {
          {
            "@asset_property_id",
            (object) obj.asset_property_id
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
            "@asset_id",
            (object) obj.asset_id
          },
          {
            "@property_name",
            (object) obj.property_name
          },
          {
            "@property_value",
            (object) obj.property_value
          },
          {
            "@available",
            (object) obj.available
          },
          {
            "@remarks",
            (object) obj.remarks
          },
          {
            "@status",
            (object) obj.status
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.asset_property_id = 0L;
      }
      return obj;
    }

    public asset_property delete_asset_property(asset_property obj)
    {
      try
      {
        obj.asset_property_id = !this.db.execute_procedure("sbt_sp_asset_property_delete", new Dictionary<string, object>()
        {
          {
            "@asset_property_id",
            (object) obj.asset_property_id
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
        obj.asset_property_id = 0L;
      }
      return obj;
    }

    public DataSet get_documents(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_documents where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public Dictionary<long, asset_document> get_document_list(long asset_id, Guid account_id)
    {
      Dictionary<long, asset_document> documentList = new Dictionary<long, asset_document>();
      try
      {
        DataSet documents = this.get_documents(asset_id, account_id);
        if (documents != null)
        {
          foreach (DataRow row in (InternalDataCollectionBase) documents.Tables[0].Rows)
          {
            asset_document document = this.get_document(Convert.ToInt64(row["document_id"]), account_id);
            documentList.Add(document.document_id, document);
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return documentList;
    }

    public asset_document get_document(long document_id, Guid account_id)
    {
      string str = "select * from sbt_asset_documents where document_id='" + (object) document_id + "' and account_id='" + (object) account_id + "'";
      asset_document document = new asset_document();
      document.document_id = 0L;
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
              document.document_id = (long) objArray[index1];
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
              document.account_id = (Guid) objArray[index2];
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
              document.created_on = (DateTime) objArray[index3];
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
              document.created_by = (long) objArray[index4];
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
              document.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              document.modified_on = document.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              document.modified_by = (long) objArray[index6];
            }
            catch
            {
              document.modified_by = document.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              document.document_name = (string) objArray[index7];
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
              document.document_size = (int) objArray[index8];
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
              document.document_type = (string) objArray[index9];
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
              document.document_meta = (string) objArray[index10];
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
              document.binary_data = (byte[]) objArray[index11];
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
              document.record_id = (Guid) objArray[index12];
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
        document.document_id = 0L;
        this.log.Error((object) str, ex);
      }
      return document;
    }

    public DataSet get_documents(long asset_id, Guid account_id, string ids)
    {
      try
      {
        string Sql = "select * from sbt_asset_documents where  account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'";
        if (ids != "")
          Sql = Sql + " and document_id in (" + ids + ")";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public Dictionary<long, asset_document> get_document_list(
      long asset_id,
      Guid account_id,
      string doc_ids)
    {
      Dictionary<long, asset_document> documentList = new Dictionary<long, asset_document>();
      try
      {
        DataSet documents = this.get_documents(asset_id, account_id, doc_ids);
        if (documents != null)
        {
          foreach (DataRow row in (InternalDataCollectionBase) documents.Tables[0].Rows)
          {
            asset_document document = this.get_document(Convert.ToInt64(row["document_id"]), account_id);
            documentList.Add(document.document_id, document);
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return documentList;
    }

    public asset_document update_document(asset_document obj)
    {
      try
      {
        obj.document_id = !this.db.execute_procedure("sbt_sp_asset_document_update", new Dictionary<string, object>()
        {
          {
            "@document_id",
            (object) obj.document_id
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
            "@document_name",
            (object) obj.document_name
          },
          {
            "@document_size",
            (object) obj.document_size
          },
          {
            "@document_type",
            (object) obj.document_type
          },
          {
            "@document_meta",
            (object) obj.document_meta
          },
          {
            "@binary_data",
            (object) obj.binary_data
          },
          {
            "@asset_id",
            (object) obj.asset_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        obj.document_id = 0L;
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public asset_document delete_document(asset_document obj)
    {
      try
      {
        obj.document_id = !this.db.execute_procedure("sbt_sp_asset_document_delete", new Dictionary<string, object>()
        {
          {
            "@document_id",
            (object) obj.document_id
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
        obj.document_id = 0L;
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public byte[] get_document_data(long doc_id, Guid account_id)
    {
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select binary_data from sbt_asset_documents where account_id='" + (object) account_id + "' and document_id='" + (object) doc_id + "'");
      return dataObjects.Count > 0 ? (byte[]) dataObjects[0][0] : (byte[]) null;
    }

    public string get_restricted_remarks(
      Dictionary<long, asset_property> properties,
      string property_name)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<long, asset_property> property in properties)
        {
          asset_property assetProperty = property.Value;
          if (assetProperty.property_name == property_name)
          {
            DataSet dataSet = this.getowner_group_emailby_asset_id(assetProperty.asset_id, assetProperty.account_id);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
              foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
                stringBuilder.Append("<a href='mailto:" + row["email"] + "'> " + row["email"] + "</a> ,");
            }
            stringBuilder.Append("|" + assetProperty.property_value);
            return stringBuilder.ToString();
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public DataSet getowner_group_emailby_asset_id(long asset_id, Guid account_ID)
    {
      try
      {
        return this.db.get_dataset("SELECT email FROM sbt_users WHERE account_id='" + (object) account_ID + "' and user_id IN (SELECT b.user_id FROM sbt_user_group_mappings b WHERE b.account_id='" + (object) account_ID + "' " + " and group_id=(SELECT a.asset_owner_group_id FROM sbt_assets a WHERE a.asset_id=" + (object) asset_id + " AND a.account_id='" + (object) account_ID + "'))") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_assets_permissions(
      Guid account_id,
      long asset_id,
      long group_id,
      long user_id,
      string filter)
    {
      try
      {
        return this.db.get_dataset(" select sbt_a.asset_id,sbt_a.name,sbt_a.code,sbt_a.asset_owner_group_id," + " case when isnull(sbt_a.asset_owner_group_id,0)= 0 then ''" + " else (select top 1 group_type from sbt_user_groups where account_id='" + (object) account_id + "' and group_id = asset_owner_group_id)" + " end as asset_owner_group_type," + " sbt_ap.asset_permission_id,sbt_ap.group_id,sbt_ap.is_view,sbt_ap.is_book," + " sbt_gr.group_name,sbt_gr.description,sbt_gr.group_type" + " from sbt_assets sbt_a" + " left join sbt_assets_permissions sbt_ap on sbt_ap.asset_id = sbt_a.asset_id  and sbt_ap.group_id='" + (object) group_id + "'" + " left join sbt_user_groups sbt_gr on sbt_gr.group_id = sbt_ap.group_id" + " where sbt_a.account_id='" + (object) account_id + "'" + " order by sbt_a.name;" + " select count(*) " + " from sbt_assets sbt_a" + " left join sbt_assets_permissions sbt_ap on sbt_ap.asset_id = sbt_a.asset_id  and sbt_ap.group_id='" + (object) group_id + "'" + " left join sbt_user_groups sbt_gr on sbt_gr.group_id = sbt_ap.group_id" + " where sbt_a.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_groups_permissions(
      Guid account_id,
      long asset_id,
      long group_id,
      long user_id,
      string filter)
    {
      try
      {
        return this.db.get_dataset(" select sbt_a.asset_id,sbt_a.name,sbt_a.code,sbt_a.asset_owner_group_id," + " case when isnull(sbt_a.asset_owner_group_id,0)= 0 then ''" + " else (select top 1 group_type from sbt_user_groups where account_id='" + (object) account_id + "' and group_id = asset_owner_group_id)" + " end as asset_owner_group_type," + " sbt_ap.asset_permission_id,sbt_ap.is_view,sbt_ap.is_book," + " sbt_gr.group_id,sbt_gr.group_name,sbt_gr.description,sbt_gr.group_type,sbt_gr.record_id" + " from sbt_user_groups sbt_gr" + " left join sbt_assets_permissions sbt_ap on sbt_ap.group_id = sbt_gr.group_id and sbt_ap.asset_id = '" + (object) asset_id + "'" + " left join sbt_assets sbt_a on sbt_a.asset_id = sbt_ap.asset_id" + " where sbt_gr.account_id = '" + (object) account_id + "' " + " order by sbt_gr.group_name;" + " select count(*) " + " from sbt_user_groups sbt_gr" + " left join sbt_assets_permissions sbt_ap on sbt_ap.group_id = sbt_gr.group_id and sbt_ap.asset_id = '" + (object) asset_id + "'" + " left join sbt_assets sbt_a on sbt_a.asset_id = sbt_ap.asset_id" + " where sbt_gr.account_id = '" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public List<asset_permission> get_asset_permissions(long group_id, Guid account_id)
    {
      List<asset_permission> assetPermissions = new List<asset_permission>();
      string str = "select * from sbt_assets_permissions where group_id='" + (object) group_id + "' and account_id='" + (object) account_id + "'";
      asset_permission assetPermission = new asset_permission();
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          foreach (object[] objArray in dataObjects.Values)
          {
            assetPermission = new asset_permission();
            assetPermission.asset_permission_id = 0L;
            int index1 = 0;
            if (this.is_valid(objArray[index1]))
            {
              try
              {
                assetPermission.asset_permission_id = (long) objArray[index1];
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
                assetPermission.account_id = (Guid) objArray[index2];
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
                assetPermission.created_on = (DateTime) objArray[index3];
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
                assetPermission.created_by = (long) objArray[index4];
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
                assetPermission.modified_on = (DateTime) objArray[index5];
              }
              catch
              {
                assetPermission.modified_on = assetPermission.created_on;
              }
            }
            int index6 = index5 + 1;
            if (this.is_valid(objArray[index6]))
            {
              try
              {
                assetPermission.modified_by = (long) objArray[index6];
              }
              catch
              {
                assetPermission.modified_by = assetPermission.created_by;
              }
            }
            int index7 = index6 + 1;
            if (this.is_valid(objArray[index7]))
            {
              try
              {
                assetPermission.asset_id = (long) objArray[index7];
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
                assetPermission.group_id = (long) objArray[index8];
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
                assetPermission.user_id = (long) objArray[index9];
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
                assetPermission.is_view = (bool) objArray[index10];
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
                assetPermission.is_book = (bool) objArray[index11];
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
                assetPermission.remarks = objArray[index12].ToString();
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
                assetPermission.record_id = (Guid) objArray[index13];
              }
              catch
              {
              }
            }
            int num = index13 + 1;
            assetPermissions.Add(assetPermission);
          }
        }
      }
      catch (Exception ex)
      {
        assetPermission.asset_permission_id = 0L;
        this.log.Error((object) str, ex);
      }
      return assetPermissions;
    }

    public asset_permission update_assets_permissions(asset_permission obj)
    {
      try
      {
        obj.asset_permission_id = !this.db.execute_procedure("sbt_sp_asset_permissions_update", new Dictionary<string, object>()
        {
          {
            "@asset_permission_id",
            (object) obj.asset_permission_id
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
            "@asset_id",
            (object) obj.asset_id
          },
          {
            "@group_id",
            (object) obj.group_id
          },
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@is_view",
            (object) obj.is_view
          },
          {
            "@is_book",
            (object) obj.is_book
          },
          {
            "@remarks",
            (object) obj.remarks
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
        obj.asset_permission_id = 0L;
      }
      return obj;
    }

    public DataSet view_asset_properties(Guid account_id, string[] properties)
    {
      string str = "";
      if (properties.Length > 0)
      {
        foreach (string property in properties)
          str = str + "'" + property + "',";
        str = str.Substring(0, str.LastIndexOf(','));
      }
      string Sql = "select asset_property_id,asset_id,property_value,available,property_name,status,remarks from sbt_asset_properties where account_id='" + (object) account_id + "'";
      if (str != "")
        Sql = Sql + " and property_name in (" + str + ")";
      try
      {
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_asset_properties(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select asset_property_id,asset_id,property_value,available,property_name,status,remarks from sbt_asset_properties where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_asset_properties(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select asset_property_id,asset_id,property_value,available,property_name,status,remarks from sbt_asset_properties where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_asset_properties(long asset_id, Guid account_id, string[] properties)
    {
      string str = "";
      if (properties.Length > 0)
      {
        foreach (string property in properties)
          str = str + "'" + property + "',";
        str = str.Substring(0, str.LastIndexOf(','));
      }
      string Sql = "select asset_property_id,asset_id,property_value,available,property_name,status,remarks from sbt_asset_properties where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'";
      if (str != "")
        Sql = Sql + " and property_name in (" + str + ")";
      try
      {
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_assets(Guid account_id)
    {
      DataSet dataSet = (DataSet) null;
      if (this.capi != null)
        dataSet = (DataSet) this.capi.get_cache(account_id.ToString() + "_assets");
      if (dataSet != null)
        return dataSet;
      try
      {
        if (!this.db.get_dataset("select asset_id,name,code,capacity,description,available_for_booking,is_restricted,building_id,level_id,category_id,asset_type,asset_owner_group_id,status from sbt_assets where account_id='" + (object) account_id + "' order by name"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_assets", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_assets(long asset_id, Guid account_id)
    {
      DataSet dataSet = (DataSet) null;
      if (this.capi != null)
        dataSet = (DataSet) this.capi.get_cache(account_id.ToString() + "_assets");
      if (dataSet != null)
        return dataSet;
      try
      {
        if (!this.db.get_dataset("select asset_id,name,code,capacity,description,available_for_booking,is_restricted,building_id,level_id,category_id,asset_type,asset_owner_group_id,status from sbt_assets where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' order by name"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_assets", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public List<long> get_bookable_assets(long user_id, Guid account_id, bool is_admin)
    {
      List<long> bookableAssets = new List<long>();
      string Sql = "select distinct(asset_id) from vw_sbt_asset_user_permissions where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "'";
      if (!is_admin)
        Sql += " and is_book='1'";
      try
      {
        if (!this.db.get_dataset(Sql))
          return bookableAssets;
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          bookableAssets.Add(Convert.ToInt64(row["asset_id"]));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return bookableAssets;
    }

    public List<long> get_approvable_assets(long user_id, Guid account_id, bool is_admin)
    {
      List<long> approvableAssets = new List<long>();
      string Sql = "select asset_id from sbt_assets where account_id='" + (object) account_id + "' and asset_owner_group_id is not null";
      if (!is_admin)
        Sql = Sql + "  and asset_owner_group_id in (select group_id from sbt_user_group_mappings where user_id='" + (object) user_id + "')";
      try
      {
        if (!this.db.get_dataset(Sql))
          return approvableAssets;
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          approvableAssets.Add(Convert.ToInt64(row["asset_id"]));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return approvableAssets;
    }

    public List<long> get_visible_assets(long user_id, Guid account_id, bool is_admin)
    {
      List<long> visibleAssets = new List<long>();
      string Sql = "select distinct(asset_id) from vw_sbt_asset_user_permissions where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "'";
      if (!is_admin)
        Sql += " and (is_view='1')";
      try
      {
        if (!this.db.get_dataset(Sql))
          return visibleAssets;
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          visibleAssets.Add(Convert.ToInt64(row["asset_id"]));
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return visibleAssets;
    }

    public asset_report_problem update_report_problem(asset_report_problem obj)
    {
      try
      {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("@Problem_id", (object) obj.problem_id);
        parameters.Add("@Asset_id", (object) obj.asset_id);
        parameters.Add("@Reported_by", (object) obj.Reported_by);
        parameters.Add("@Subject", (object) obj.Subject);
        parameters.Add("@Remarks", (object) obj.Remarks);
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@created_by", (object) obj.created_by);
        parameters.Add("@modified_by", (object) obj.modified_by);
        bool flag;
        try
        {
          flag = this.db.execute_procedure("sbt_sp_report_problem_update", parameters);
        }
        catch (Exception ex)
        {
          flag = false;
          this.log.Error((object) "Error -> ", ex);
        }
        obj.problem_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public Dictionary<long, asset> filter_favourites(
      Dictionary<long, asset> available_assets,
      DataSet fav_data,
      bool is_checked)
    {
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      if (fav_data == null)
        return available_assets;
      if (fav_data.Tables[0].Rows.Count == 0)
        dictionary = available_assets;
      else if (is_checked)
      {
        foreach (DataRow row in (InternalDataCollectionBase) fav_data.Tables[0].Rows)
        {
          if (available_assets.ContainsKey(Convert.ToInt64(row["resource_id"])))
            dictionary.Add(Convert.ToInt64(row["resource_id"]), available_assets[Convert.ToInt64(row["resource_id"])]);
        }
      }
      else
        dictionary = available_assets;
      return dictionary;
    }

    public Dictionary<long, asset> filter_weekend_holiday(
      Dictionary<long, asset> available_assets,
      DateTime from,
      DateTime to,
      DataSet asset_propertis,
      DataSet settings)
    {
      return new Dictionary<long, asset>();
    }

    public asset_favourite get_favourite_asset(Guid account_id, long favourite_asset_id)
    {
      string str = "select * from sbt_asset_favourites where favourite_asset_id='" + (object) favourite_asset_id + "' and account_id='" + (object) account_id + "'";
      asset_favourite favouriteAsset = new asset_favourite();
      favouriteAsset.favourite_asset_id = 0L;
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
              favouriteAsset.favourite_asset_id = (long) objArray[index1];
            }
            catch
            {
              favouriteAsset.favourite_asset_id = 0L;
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              favouriteAsset.account_id = (Guid) objArray[index2];
            }
            catch
            {
              favouriteAsset.favourite_asset_id = 0L;
            }
          }
          int index3 = index2 + 1;
          if (this.is_valid(objArray[index3]))
          {
            try
            {
              favouriteAsset.user_id = (long) objArray[index3];
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
              favouriteAsset.resource_id = (long) objArray[index4];
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
              favouriteAsset.module_name = (string) objArray[index5];
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
              favouriteAsset.created_on = (DateTime) objArray[index6];
            }
            catch
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
        favouriteAsset.favourite_asset_id = 0L;
        this.log.Error((object) str, ex);
      }
      return favouriteAsset;
    }

    public DataSet get_favourite_assets(Guid account_id, long user_id)
    {
      try
      {
        return this.db.get_dataset("select favourite_asset_id,resource_id,module_name from sbt_asset_favourites where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public asset_favourite update_assets_favourite(asset_favourite obj)
    {
      try
      {
        obj.favourite_asset_id = !this.db.execute_procedure("sbt_sp_asset_favourites_update", new Dictionary<string, object>()
        {
          {
            "@favourite_asset_id",
            (object) obj.favourite_asset_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@resource_id",
            (object) obj.resource_id
          },
          {
            "@module_name",
            (object) obj.module_name
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.favourite_asset_id = 0L;
      }
      return obj;
    }
  }
}
