// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.booking_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace skynapse.fbs
{
  public class booking_api : api_base
  {
    private asset_api assets;

    public booking_api() => this.assets = new asset_api();

    public string get_booking_type(int tt)
    {
      try
      {
        return Enum.GetName(typeof (api_constants.booking_type), (object) tt);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public int get_bookingtype(string ty)
    {
      try
      {
        switch (ty)
        {
          case "Quick":
            return Convert.ToInt32((object) api_constants.booking_type.Quick);
          case "Wizard":
            return Convert.ToInt32((object) api_constants.booking_type.Wizard);
          case "Repeat":
            return Convert.ToInt32((object) api_constants.booking_type.Repeat);
          case "Custom":
            return Convert.ToInt32((object) api_constants.booking_type.Custom);
          case "Clone":
            return Convert.ToInt32((object) api_constants.booking_type.Clone);
          case "Display":
            return Convert.ToInt32((object) api_constants.booking_type.Display);
          case "QR":
            return Convert.ToInt32((object) api_constants.booking_type.QR);
          case "Mobile":
            return Convert.ToInt32((object) api_constants.booking_type.Mobile);
          case "Exchange":
            return Convert.ToInt32((object) api_constants.booking_type.Exchange);
          case "Outlook":
            return Convert.ToInt32((object) api_constants.booking_type.Outlook);
          case "Others":
            return Convert.ToInt32((object) api_constants.booking_type.Others);
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return 0;
    }

    public string get_status(long id)
    {
      try
      {
        foreach (string key in api_constants.booking_status.Keys)
        {
          if ((long) api_constants.booking_status[key] == id)
            return key;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public int get_status(string value) => api_constants.booking_status[value];

    public bool is_available(long asset_id, DateTime from, DateTime to, Guid account_id)
    {
      try
      {
        if (!this.db.get_dataset("select count(booking_id) from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and " + "  ((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or " + "(book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "')  " + "OR ('" + from.ToString(api_constants.sql_datetime_format) + "' between  book_from   and  book_to) or " + "('" + to.ToString(api_constants.sql_datetime_format) + "'  between  book_from   and book_to )) " + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) api_constants.book_status.Booked + " or status= " + (object) api_constants.book_status.Blocked + "  or status= " + (object) api_constants.book_status.NoShow + "  ) and transfer_request=1 and account_id='" + (object) account_id + "')"))
          return false;
        int num = 0;
        foreach (DataTable table in (InternalDataCollectionBase) this.db.resultDataSet.Tables)
          num += Convert.ToInt32(table.Rows[0][0]);
        this.db.resultDataSet.Dispose();
        return num <= 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public Dictionary<long, asset> check_available_assets(string from, string to, Guid account_id)
    {
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      try
      {
        DataSet assets = this.assets.get_assets(account_id);
        DataSet bookings = this.get_bookings(from, to, account_id);
        foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
        {
          if (bookings.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "'").Length == 0)
          {
            asset asset = this.assets.get_asset(Convert.ToInt64(row["asset_id"]), account_id);
            dictionary.Add(asset.asset_id, asset);
          }
        }
        assets.Dispose();
        bookings.Dispose();
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return dictionary;
    }

    public DataSet get_assets_booking_slot(
      DateTime current_timestamp,
      DateTime start,
      DateTime end,
      long building_id,
      long category_id,
      long level_id,
      long capacity,
      Guid account_id,
      bool isAdmin = false,
      string group_ids = "0")
    {
      try
      {
        string str1 = "select a.asset_id,a.asset_type,a.available_for_booking,a.building_id,a.level_id,a.capacity,a.category_id,a.code,a.description,a.is_restricted,a.level_id,a.name,a.status,a.asset_owner_group_id," + " isnull(b.property_value,0),c.is_view,c.is_book from sbt_assets a   inner join sbt_asset_properties b on b.asset_id=a.asset_id and     b.property_name='booking_slot' ";
        if (!isAdmin)
          str1 = str1 + " AND ('" + end.ToString(api_constants.display_datetime_format) + "' <= DateAdd(mm,convert(INT ,b.property_value),'" + start.ToString(api_constants.display_datetime_format) + "'))";
        string str2 = str1 + " and a.account_id='" + (object) account_id + "' " + " AND a.status=1 ";
        if (building_id > 0L)
          str2 = str2 + " and a.building_id='" + (object) building_id + "'";
        if (category_id > 0L)
          str2 = str2 + " and a.category_id='" + (object) category_id + "'";
        if (level_id > 0L)
          str2 = str2 + " and a.level_id='" + (object) level_id + "'";
        if (capacity > 0L)
          str2 = str2 + " and a.capacity >=" + (object) capacity;
        string str3 = str2 + " AND a.asset_id NOT IN (select asset_id from sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN(" + group_ids + ") group by asset_id having max(convert(int,isnull(is_view,0))) = 0) " + " left join (select asset_id,max(convert(int,isnull(is_view,0))) as is_view,max(convert(int,isnull(is_book,0))) as is_book From sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN (" + group_ids + ") group by asset_id) c on isnull(c.asset_id,0) = a.asset_id and a.account_id='" + (object) account_id + "' order by a.name;" + " SELECT a.asset_id,a.asset_type,a.available_for_booking,a.building_id,a.level_id,a.capacity,a.category_id,a.code,a.description,a.is_restricted,a.level_id,a.name,a.status,a.asset_owner_group_id,''AS timeperiod,c.is_view,c.is_book  FROM sbt_assets a " + " left join (select asset_id,max(convert(int,isnull(is_view,0))) as is_view,max(convert(int,isnull(is_book,0))) as is_book From sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN (" + group_ids + ") group by asset_id) c on isnull(c.asset_id,0) = a.asset_id " + " WHERE a.asset_id NOT IN ( SELECT b.asset_id FROM sbt_asset_properties b WHERE b.property_name = 'booking_slot' and b.account_id='" + (object) account_id + "')" + " AND a.status=1 ";
        if (building_id > 0L)
          str3 = str3 + " and a.building_id='" + (object) building_id + "'";
        if (category_id > 0L)
          str3 = str3 + " and a.category_id='" + (object) category_id + "'";
        if (level_id > 0L)
          str3 = str3 + " and a.level_id='" + (object) level_id + "'";
        if (capacity > 0L)
          str3 = str3 + " and a.capacity >=" + (object) capacity;
        return this.db.get_dataset(str3 + " AND a.asset_id NOT IN (select asset_id from sbt_assets_permissions where account_id='" + (object) account_id + "' and group_id IN(" + group_ids + ") group by asset_id having max(convert(int,isnull(is_view,0))) = 0) and a.account_id='" + (object) account_id + "'  order by a.name;" + " SELECT  value FROM dbo.sbt_settings WHERE parameter = 'advance_booking_window' and account_id='" + (object) account_id + "';" + " SELECT convert(VARCHAR(20),DateAdd(mm,convert(INT, b.value),'" + current_timestamp.ToString(api_constants.display_datetime_format) + "')) FROM sbt_settings b WHERE b.account_id='" + (object) account_id + "' and b.parameter='advance_booking_window'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public List<long> check_availability(
      DateTime from,
      DateTime to,
      Guid account_id,
      List<long> bookable_assets)
    {
      List<long> longList = new List<long>();
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      if (this.db.get_dataset("select distinct(asset_id) from sbt_asset_bookings where account_id='" + (object) account_id + "' and " + "((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or (book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "')  " + "OR ('" + from.ToString(api_constants.sql_datetime_format) + "' between  book_from   and  book_to) or ('" + to.ToString(api_constants.sql_datetime_format) + "'  between  book_from   and book_to )) and (status=1 or status=2 or status=4)"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        {
          if (!longList.Contains(Convert.ToInt64(row["asset_id"])))
            longList.Add(Convert.ToInt64(row["asset_id"]));
        }
      }
      return longList;
    }

    public List<long> check_availability(
      DateTime from,
      DateTime to,
      Guid account_id,
      List<long> bookable_assets,
      string booking_id)
    {
      List<long> longList = new List<long>();
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      string str = " select distinct(asset_id) from sbt_asset_bookings where account_id='" + (object) account_id + "'";
      if (booking_id != "" && !booking_id.Contains("-"))
        str = str + " and booking_id !='" + booking_id + "' ";
      if (this.db.get_dataset(str + "  and   ((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or (book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "')  " + "OR ('" + from.ToString(api_constants.sql_datetime_format) + "' between  book_from   and  book_to) or ('" + to.ToString(api_constants.sql_datetime_format) + "'  between  book_from   and book_to )) and (status=1 or status=2 or status=4)"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        {
          if (!longList.Contains(Convert.ToInt64(row["asset_id"])))
            longList.Add(Convert.ToInt64(row["asset_id"]));
        }
      }
      return longList;
    }

    public Dictionary<long, asset> check_available_assets(
      string from,
      string to,
      Guid account_id,
      string building,
      string category,
      string level,
      string capacity,
      string group_ids,
      bool isAdmin,
      DataSet asset_properties,
      DataSet sDS,
      DateTime current_timestamp)
    {
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      try
      {
        DataSet assetsBookingSlot = this.get_assets_booking_slot(current_timestamp, Convert.ToDateTime(current_timestamp.ToString(api_constants.display_datetime_format)), Convert.ToDateTime(to), Convert.ToInt64(building), Convert.ToInt64(category), Convert.ToInt64(level), Convert.ToInt64(capacity), account_id, isAdmin, group_ids);
        if (DateTime.Compare(Convert.ToDateTime(assetsBookingSlot.Tables[3].Rows[0][0].ToString()), Convert.ToDateTime(to)) >= 1 || isAdmin)
          assetsBookingSlot.Tables[0].Merge(assetsBookingSlot.Tables[1]);
        DataSet quickbookingsIds = this.get_Quickbookings_ids(from, to, account_id);
        asset asset1 = new asset();
        foreach (DataRow row in (InternalDataCollectionBase) assetsBookingSlot.Tables[0].Rows)
        {
          try
          {
            asset asset2 = this.get_asset(row, sDS.Tables[0], asset_properties.Tables[0]);
            asset2.is_view = !string.IsNullOrEmpty(row["is_view"].ToString()) && Convert.ToBoolean(row["is_view"]);
            asset2.is_book = !string.IsNullOrEmpty(row["is_book"].ToString()) && Convert.ToBoolean(row["is_book"]);
            if (isAdmin)
            {
              if (quickbookingsIds.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=1 or status=4)").Length == 0)
                dictionary.Add(asset2.asset_id, asset2);
            }
            else
            {
              if (asset2.is_restricted)
              {
                if (quickbookingsIds.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=1 or status=4)").Length == 0)
                  dictionary.Add(asset2.asset_id, asset2);
              }
              else if (quickbookingsIds.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=1 or status=4)").Length == 0)
                dictionary.Add(asset2.asset_id, asset2);
              if (quickbookingsIds.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=2)").Length > 0)
                dictionary.Remove(asset2.asset_id);
            }
          }
          catch
          {
          }
        }
        assetsBookingSlot.Dispose();
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return dictionary;
    }

    public asset get_asset(DataRow row, DataTable setting_table, DataTable asset_properties_table)
    {
      asset asset = new asset() { asset_id = 0 };
      asset.asset_id = Convert.ToInt64(row["asset_id"]);
      asset.asset_type = Convert.ToInt64(row["asset_type"]);
      asset.available_for_booking = Convert.ToBoolean(row["available_for_booking"]);
      asset.building_id = Convert.ToInt32(row["building_id"]);
      asset.capacity = Convert.ToInt16(row["capacity"]);
      asset.category_id = Convert.ToInt64(row["category_id"]);
      asset.code = row["code"].ToString();
      asset.description = row["description"].ToString();
      asset.is_restricted = Convert.ToBoolean(row["is_restricted"]);
      asset.level_id = Convert.ToInt32(row["level_id"]);
      asset.name = row["name"].ToString();
      asset.status = Convert.ToInt16(row["status"]);
      asset.is_book = Convert.ToBoolean(row["is_book"]);
      asset.is_view = Convert.ToBoolean(row["is_view"]);
      if (row["asset_owner_group_id"] != DBNull.Value)
        asset.asset_owner_group_id = Convert.ToInt64(row["asset_owner_group_id"]);
      DataRow[] dataRowArray1 = setting_table.Select("setting_id='" + (object) asset.building_id + "'");
      asset.building = new setting();
      asset.building.setting_id = Convert.ToInt64(dataRowArray1[0]["setting_id"]);
      asset.building.value = dataRowArray1[0]["value"].ToString();
      DataRow[] dataRowArray2 = setting_table.Select("setting_id='" + (object) asset.level_id + "'");
      asset.level = new setting();
      asset.level.setting_id = Convert.ToInt64(dataRowArray2[0]["setting_id"]);
      asset.level.value = dataRowArray2[0]["value"].ToString();
      DataRow[] dataRowArray3 = setting_table.Select("setting_id='" + (object) asset.category_id + "'");
      asset.category = new setting();
      asset.category.setting_id = Convert.ToInt64(dataRowArray3[0]["setting_id"]);
      asset.category.value = dataRowArray3[0]["value"].ToString();
      DataRow[] dataRowArray4 = setting_table.Select("setting_id='" + (object) asset.asset_type + "'");
      asset.type = new setting();
      asset.type.setting_id = Convert.ToInt64(dataRowArray4[0]["setting_id"]);
      asset.type.value = dataRowArray4[0]["value"].ToString();
      setting_table.Select("setting_id='" + (object) asset.status + "'");
      asset.status_value = new setting();
      if (asset.status == (short) 1)
      {
        asset.status_value.setting_id = (long) asset.status;
        asset.status_value.value = "Available";
      }
      else
      {
        asset.status_value.setting_id = (long) asset.status;
        asset.status_value.value = "Not Available";
      }
      asset.asset_properties = new Dictionary<long, asset_property>();
      asset.asset_properties = this.get_asset_properties(asset.asset_id, asset_properties_table);
      return asset;
    }

    private Dictionary<long, asset_property> get_asset_properties(long asset_id, DataTable table)
    {
      Dictionary<long, asset_property> assetProperties = new Dictionary<long, asset_property>();
      try
      {
        foreach (DataRow row in table.Select("asset_id='" + (object) asset_id + "'"))
        {
          asset_property assetProperty = this.get_asset_property(asset_id, row);
          assetProperties.Add(assetProperty.asset_property_id, assetProperty);
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return assetProperties;
    }

    private asset_property get_asset_property(long asset_id, DataRow row) => new asset_property()
    {
      asset_id = asset_id,
      asset_property_id = Convert.ToInt64(row["asset_property_id"]),
      available = Convert.ToBoolean(row["available"]),
      property_name = row["property_name"].ToString(),
      property_value = row["property_value"].ToString(),
      remarks = row["remarks"].ToString(),
      status = Convert.ToInt16(row["status"])
    };

    public Dictionary<long, asset> check_available_assets(
      Dictionary<string, string> date_range,
      string from,
      string to,
      Guid account_id,
      string building,
      string category,
      string level,
      string capacity,
      string group_ids,
      bool isAdmin,
      DataSet asset_properties,
      DataSet sDS,
      DateTime current_timestamp)
    {
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      DataSet assetsBookingSlot = this.get_assets_booking_slot(current_timestamp, Convert.ToDateTime(current_timestamp.ToString(api_constants.display_datetime_format)), Convert.ToDateTime(to), Convert.ToInt64(building), Convert.ToInt64(category), Convert.ToInt64(level), Convert.ToInt64(capacity), account_id, isAdmin, group_ids);
      if (DateTime.Compare(Convert.ToDateTime(assetsBookingSlot.Tables[3].Rows[0][0].ToString()), Convert.ToDateTime(to)) >= 1 || isAdmin)
        assetsBookingSlot.Tables[0].Merge(assetsBookingSlot.Tables[1]);
      string str1 = "1=1" + " And status=1";
      if (building != "")
        str1 = str1 + " And building_id=" + building;
      if (category != "")
        str1 = str1 + " And category_id=" + category;
      if (level != "")
        str1 = str1 + " And level_id=" + level;
      if (capacity != "")
      {
        string str2 = str1 + " And capacity >=" + capacity;
      }
      DataSet dataSet = (DataSet) null;
      DataTable table = new DataTable();
      foreach (string key in date_range.Keys)
      {
        DataSet quickbookingsIds = this.get_Quickbookings_ids(key, date_range[key], account_id);
        if (quickbookingsIds != null && quickbookingsIds.Tables[0].Rows.Count > 0)
          table.Merge(quickbookingsIds.Tables[0]);
      }
      if (table.Rows.Count > 0)
      {
        dataSet = new DataSet();
        dataSet.Tables.Add(table);
      }
      assetsBookingSlot.Tables[0].DefaultView.Sort = "name";
      assetsBookingSlot.Tables[0].DefaultView.ToTable();
      foreach (DataRow row in (InternalDataCollectionBase) assetsBookingSlot.Tables[0].Rows)
      {
        try
        {
          asset asset = this.get_asset(row, sDS.Tables[0], asset_properties.Tables[0]);
          asset.is_view = !string.IsNullOrEmpty(row["is_view"].ToString()) && Convert.ToBoolean(row["is_view"]);
          asset.is_book = !string.IsNullOrEmpty(row["is_book"].ToString()) && Convert.ToBoolean(row["is_book"]);
          if (isAdmin)
          {
            if (dataSet != null)
            {
              if (dataSet.Tables.Count > 0)
              {
                if (dataSet.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=1 or status=4)").Length == 0)
                  dictionary.Add(asset.asset_id, asset);
              }
            }
            else
              dictionary.Add(asset.asset_id, asset);
          }
          else
          {
            if (asset.is_restricted)
            {
              if (dataSet != null)
              {
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=1 or status=4)").Length == 0)
                  dictionary.Add(asset.asset_id, asset);
              }
              else
                dictionary.Add(asset.asset_id, asset);
            }
            else if (dataSet != null)
            {
              if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=1 or status=4)").Length == 0)
                dictionary.Add(asset.asset_id, asset);
            }
            else
              dictionary.Add(asset.asset_id, asset);
            if (dataSet.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "' and (status=2)").Length > 0)
              dictionary.Remove(asset.asset_id);
          }
        }
        catch (Exception ex)
        {
          this.log.Error((object) "Error -> ", ex);
        }
      }
      assetsBookingSlot.Dispose();
      return dictionary;
    }

    public DataSet get_transferd_booking(long booking_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where account_id='" + (object) account_id + "'" + " and transfer_original_booking_id =" + (object) booking_id) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where account_id='" + (object) account_id + "'" + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(Guid account_id, long booking_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where account_id='" + (object) account_id + "' and booking_id='" + (object) booking_id + "' order by book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_booking_by_id(long booking_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where booking_id='" + (object) booking_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_user_group(long user_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT a.group_id,a.user_id,(select group_type from sbt_user_groups k where k.account_id='" + (object) account_id + "' and k.group_id=a.group_id)  " + " FROM sbt_user_group_mappings a WHERE a.user_id=" + (object) user_id + " " + " AND  a.group_id NOT IN (SELECT e.group_id FROM sbt_user_groups e WHERE e.group_name ='" + api_constants.all_users_text + "' and e.account_id='" + (object) account_id + "' )  and a.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_repeat_bookings(Guid repeat_reference_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT * from sbt_asset_bookings where repeat_reference_id='" + (object) repeat_reference_id + "' and  account_id='" + (object) account_id + "' order by book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_repeat_booking_ids(Guid repeat_reference_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT book_from,booking_id from sbt_asset_bookings where repeat_reference_id='" + (object) repeat_reference_id + "' and  account_id='" + (object) account_id + "' and status='1' order by book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public bool check_sendeamil(Guid account_id, long asset_id)
    {
      bool flag = false;
      try
      {
        if (this.db.get_dataset("SELECT property_value FROM sbt_asset_properties WHERE property_name = 'is_email_send' AND asset_id=" + (object) asset_id + " AND account_id='" + (object) account_id + "'"))
          flag = this.db.resultDataSet.Tables[0].Rows.Count > 0 && this.db.resultDataSet.Tables[0].Rows[0]["property_value"].ToString() == "1";
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return flag;
    }

    public DataSet get_booking_assets(long bookingid, Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT DISTINCT  b.booking_id,a.asset_id,b.housekeeping_required,b.purpose, b.email, b.repeat_reference_id, " + " b.booked_for, b.created_by,  " + "  (select full_name from sbt_users where user_id=b.booked_for and account_id='" + (object) account_id + "') as booked_for_name," + "  (select full_name from sbt_users where user_id=b.created_by and account_id='" + (object) account_id + "') as requested_by_name," + " a.name,a.code,b.book_from,b.book_to,b.setup_required,b.setup_type,a.building_id,a.level_id,a.category_id," + "a.capacity,b.remarks from sbt_assets a,sbt_asset_bookings b where  a.asset_id=b.asset_id  AND b.booking_id=" + (object) bookingid + " and  a.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_repeat_booking_assets(Guid account_id, Guid ref_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT DISTINCT  b.booking_id,a.asset_id,b.housekeeping_required,b.purpose, b.email, " + " b.booked_for, b.created_by,b.status,  " + "  (select full_name from sbt_users where user_id=b.booked_for and account_id='" + (object) account_id + "') as booked_for_name," + "  (select full_name from sbt_users where user_id=b.created_by and account_id='" + (object) account_id + "') as requested_by_name," + "  a.name,a.code,b.book_from,b.book_to,b.setup_required,b.setup_type,a.building_id,a.level_id,a.category_id," + "  a.capacity,b.remarks from sbt_assets a,sbt_asset_bookings b where  a.asset_id=b.asset_id  AND " + " a.account_id='" + (object) account_id + "' " + "  and b.is_repeat=1 and b.repeat_reference_id='" + (object) ref_id + "' order by b.book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_usersname(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT user_id,username FROM sbt_users where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_usersname(Guid account_id, long user_id)
    {
      try
      {
        return this.db.get_dataset("SELECT user_id,username,full_name FROM sbt_users where user_id=" + (object) user_id + " and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_readonly(
      string fromStr,
      string toStr,
      Guid account_id,
      long user_id)
    {
      try
      {
        string str = "select booking_id,book_from,book_to,status,asset_id,booked_for,created_by, purpose from sbt_asset_bookings where account_id='" + (object) account_id + "' " + " and (booked_for='" + (object) user_id + "'  or created_by='" + (object) user_id + "')";
        if (fromStr != "" && toStr != "")
          str = str + "and ((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to )) ";
        return this.db.get_dataset(str + "  and booking_id not in (select distinct(transfer_original_booking_id) from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_readonly(
      long asset_id,
      string fromStr,
      string toStr,
      Guid account_id,
      long user_id)
    {
      try
      {
        string str1 = "select booking_id,book_from,book_to,status,asset_id,booked_for,purpose from sbt_asset_bookings where account_id='" + (object) account_id + "' ";
        if (asset_id > 0L)
          str1 = str1 + " and asset_id=" + (object) asset_id;
        string str2 = str1 + "  and (booked_for='" + (object) user_id + "'  or created_by='" + (object) user_id + "' )";
        if (fromStr != "" && toStr != "")
          str2 = str2 + " and ((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to )) ";
        return this.db.get_dataset(str2 + "  and  booking_id not in (select distinct(transfer_original_booking_id) from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_readonly(
      long asset_id,
      string fromStr,
      string toStr,
      Guid account_id)
    {
      try
      {
        string str = "select booking_id,book_from,book_to,status,asset_id,booked_for,purpose from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and ";
        if (fromStr != "" && toStr != "")
          str = str + "((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to )) and";
        return this.db.get_dataset(str + "   booking_id not in (select distinct(transfer_original_booking_id) from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(string fromStr, string toStr, Guid account_id)
    {
      try
      {
        string str = "select * from sbt_asset_bookings where account_id='" + (object) account_id + "' and ";
        if (fromStr != "" && toStr != "")
          str = str + "((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to )) and";
        return this.db.get_dataset(str + "   booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(string fromStr, string toStr, Guid account_id, int status)
    {
      try
      {
        string str = "select booking_id,purpose,asset_id,book_from,book_to,status,email,booked_for,remarks,created_on from sbt_asset_bookings where account_id='" + (object) account_id + "' and ";
        if (fromStr != "" && toStr != "")
          str = str + "((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to )) and";
        return this.db.get_dataset(str + "   booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "')" + " and status=" + (object) status) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_Quickbookings(string fromStr, string toStr, Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" select * from ( " + " select booking_id,purpose,asset_id,book_from,book_to,status,email,booked_for,remarks,created_by_Name from vw_bookings where account_id='" + (object) account_id + "' and " + "((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to ))" + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where transfer_original_booking_id >0 and (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "')" + ") as RR " + " WHERE RR.booking_id NOT IN (SELECT booking_id FROM sbt_asset_bookings WHERE account_id='" + (object) account_id + "' and transfer_request=1 AND (status=4 or status=0 )) ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_Quickbookings(string fromStr, string toStr, Guid account_id, long user_id)
    {
      try
      {
        return this.db.get_dataset(" select * from ( " + " select booking_id,purpose,asset_id,book_from,book_to,status,email,booked_for,remarks,created_by_Name from vw_bookings where account_id='" + (object) account_id + "' and booked_for='" + (object) user_id + "' and " + "((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to ))" + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where transfer_original_booking_id >0 and (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "' and booked_for='" + (object) user_id + "')" + ") as RR " + " WHERE RR.booking_id NOT IN (SELECT booking_id FROM sbt_asset_bookings WHERE account_id='" + (object) account_id + "' and transfer_request=1 AND (status=4 or status=0 )) ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_Quickbookings_ids(string fromStr, string toStr, Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" select * from ( " + " select booking_id,asset_id,status from sbt_asset_bookings where account_id='" + (object) account_id + "' and " + "((book_from between '" + fromStr + "' and '" + toStr + "') or " + "(book_to between '" + fromStr + "' and '" + toStr + "')  " + "OR ('" + fromStr + "' between  book_from   and  book_to) or " + "('" + toStr + "'  between  book_from   and book_to ))" + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where transfer_original_booking_id >0 and (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "')" + ") as RR " + " WHERE RR.booking_id NOT IN (SELECT booking_id FROM sbt_asset_bookings WHERE account_id='" + (object) account_id + "' and transfer_request=1 AND (status=4 or status=0 )) ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "'" + " and booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " ) and transfer_request=1 and account_id='" + (object) account_id + "') ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(long asset_id, Guid account_id, int status)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and status=" + (object) status + " and booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " ) and transfer_request=1 and account_id='" + (object) account_id + "') ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(long asset_id, string from, string to, Guid account_id)
    {
      try
      {
        string str = "select A.*, (select name from sbt_assets where asset_id=A.asset_id) as room from sbt_asset_bookings A where A.account_id='" + (object) account_id + "' ";
        if (asset_id > 0L)
          str = str + "  and a.asset_id='" + (object) asset_id + "'  ";
        return this.db.get_dataset(str + "  and ((A.book_from between '" + from + "' and '" + to + "') or " + "(A.book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  A.book_from   and  A.book_to) or " + "('" + to + "'  between  A.book_from   and A.book_to ))" + " and A.booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "') ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_bookings(long asset_id, string from, string to, Guid account_id)
    {
      try
      {
        string str = "select A.booking_id from sbt_asset_bookings A where A.account_id='" + (object) account_id + "' and (a.status=1 or a.status=2 or a.status=4) ";
        if (asset_id > 0L)
          str = str + "  and a.asset_id='" + (object) asset_id + "'  ";
        return this.db.get_dataset(str + "  and ((A.book_from between '" + from + "' and '" + to + "') or " + "(A.book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  A.book_from   and  A.book_to) or " + "('" + to + "'  between  A.book_from   and A.book_to ))" + " and A.booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status='" + (object) 1 + "' or status= '" + (object) 2 + "'  or status= '" + (object) 3 + "' or status='" + (object) 0 + "'  ) and transfer_request=1 and account_id='" + (object) account_id + "') ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_for_server(
      string startno,
      string endno,
      string orderby,
      string searchStr,
      string orderdir,
      string from,
      string to,
      string building,
      string level,
      string category,
      string type,
      string status,
      string requested_by,
      Guid account_id,
      string email_id)
    {
      try
      {
        string str1 = "SELECT * FROM (SELECT ROW_NUMBER() OVER (" + (!(orderby == "code") ? "ORDER BY  RR." + orderby + " " + orderdir : "ORDER BY  RR.code,RR.name  " + orderdir) + ") AS RowNumber,RR.* FROM (SELECT DISTINCT (SELECT comment =(SELECT (VALUE +'-'+ remarks+',  ') from( SELECT property_value, (SELECT VALUE  FROM sbt_settings WHERE convert(NVARCHAR(10), setting_id)=property_value ) AS VALUE , remarks FROM sbt_asset_properties k WHERE  k.property_name='asset_property' AND  k.asset_id=a.asset_id AND remarks <> ''AND  remarks IS NOT null )  AS t FOR XML path(''))) AS comment " + "  ,d.asset_id,A.purpose,A.book_from,A.book_to,a.booked_for,a.booking_id,d.code,d.name,a.status as status1,(CASE a.status WHEN 1 THEN 'Booked' WHEN 2  THEN 'Blocked' WHEN 3 THEN  'No Show' WHEN 0 THEN  'Cancelled' WHEN 4 THEN 'Pending' when 6 then 'Rejected' when 5 then 'Withdraw' when 7 then 'Auto Rejected' END ) AS Status,(SELECT E.full_name FROM sbt_users E WHERE E.user_id=A.booked_for) AS RequestedBy," + " (SELECT value FROM sbt_settings  WHERE setting_id= D.building_id and account_id='" + (object) account_id + "') AS BuildingName " + " FROM sbt_asset_bookings a,sbt_asset_properties b,sbt_settings c,sbt_assets d " + " WHERE a.account_id='" + (object) account_id + "' " + " AND a.asset_id=d.asset_id  " + " and d.status=(select setting_id from sbt_settings where value='available' and parameter='asset_status' and account_id='" + (object) account_id + "')" + " and d.building_id LIKE '" + building + "' AND d.level_id LIKE '" + level + "' AND d.category_id LIKE '" + category + "' AND d.asset_type LIKE '" + type + "'";
        if (requested_by != "%")
          str1 = str1 + " and (a.booked_for LIKE '" + requested_by + "'" + "  or a.booking_id in (select booking_id from sbt_asset_booking_invites where email='" + email_id + "' and account_id='" + (object) account_id + "'))";
        string str2 = (!(status != "%") ? str1 + " and a.status   like '" + status + "'" : str1 + " and a.status  in (" + status + ")") + "   AND b.property_name='asset_property'AND b.property_value=convert(NVARCHAR(10), c.setting_id) AND (B.status=0 or (B.status=1 and B.available=1)) " + " and a.booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + "  or status= " + (object) this.get_status("Cancelled") + ") and transfer_request=1 and account_id='" + (object) account_id + "') " + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + ") AS RR   " + " WHERE " + " (RR.comment LIKE '%" + searchStr + "%' OR RR.purpose LIKE '%" + searchStr + "%' OR RR.book_from LIKE '%" + searchStr + "%' " + " OR RR.book_to LIKE '%" + searchStr + "%' OR RR.booked_for LIKE '%" + searchStr + "%' OR RR.status LIKE '%" + searchStr + "%' OR RR.RequestedBy LIKE  '%" + searchStr + "%' " + " OR RR.buildingname LIKE '%" + searchStr + "%' or RR.code Like '%" + searchStr + "%' OR RR.name Like '%" + searchStr + "%' ) ) AS outRR where  outRR.RowNumber BETWEEN " + startno + " AND  " + endno + " " + "SELECT count(*) FROM (SELECT ROW_NUMBER() OVER (ORDER BY RR." + orderby + " " + orderdir + ") AS RowNumber,RR.* FROM (SELECT DISTINCT (SELECT comment =(SELECT (VALUE + '-' + remarks+',  ') from( SELECT property_value, (SELECT VALUE  FROM sbt_settings WHERE convert(NVARCHAR(10), setting_id)=property_value ) AS VALUE , remarks FROM sbt_asset_properties k WHERE  k.property_name='asset_property' AND  k.asset_id=a.asset_id AND remarks <> ''AND  remarks IS NOT null )  AS t FOR XML path(''))) AS comment " + " ,A.purpose,A.book_from,A.book_to,a.booked_for,a.booking_id,d.code,d.name,a.status as status1,(CASE a.status WHEN 1 THEN 'Booked' WHEN 2  THEN 'Blocked' WHEN 3 THEN  'No Show' WHEN 0 THEN  'Cancelled' WHEN 4 THEN 'Pending' when 6 then 'Rejected' when 5 then 'Withdraw' when 7 then 'Auto Rejected'  END ) AS Status,(SELECT E.full_name FROM sbt_users E WHERE E.user_id=A.booked_for) AS RequestedBy," + " (SELECT value FROM sbt_settings  WHERE setting_id= D.building_id and account_id='" + (object) account_id + "') AS BuildingName " + " FROM sbt_asset_bookings a,sbt_asset_properties b,sbt_settings c,sbt_assets d " + " WHERE a.account_id='" + (object) account_id + "' " + " AND a.asset_id=d.asset_id  " + " and d.status=(select setting_id from sbt_settings where value='available' and parameter='asset_status' and account_id='" + (object) account_id + "')" + " and d.building_id LIKE '" + building + "' AND d.level_id LIKE '" + level + "' AND d.category_id LIKE '" + category + "' AND d.asset_type LIKE '" + type + "'";
        if (requested_by != "%")
          str2 = str2 + " and (a.booked_for LIKE '" + requested_by + "'" + " or a.booking_id in (select booking_id from sbt_asset_booking_invites where email='" + email_id + "' and account_id='" + (object) account_id + "'))";
        return this.db.get_dataset((!(status != "%") ? str2 + " and a.status   like '" + status + "'" : str2 + " and a.status  in (" + status + ")") + " AND b.property_name='asset_property'AND b.property_value=convert(NVARCHAR(10), c.setting_id) AND (B.status=0 or (B.status=1 and B.available=1)) " + " and a.booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status= " + (object) this.get_status("Cancelled") + ") and transfer_request=1 and account_id='" + (object) account_id + "') " + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + ") AS RR  ) AS outRR " + " WHERE (outRR.comment LIKE '%" + searchStr + "%' OR outRR.purpose LIKE '%" + searchStr + "%' OR outRR.book_from LIKE '%" + searchStr + "%' " + " OR outRR.book_to LIKE '%" + searchStr + "%' OR outRR.booked_for LIKE '%" + searchStr + "%' OR outRR.status LIKE '%" + searchStr + "%' OR outRR.RequestedBy LIKE  '%" + searchStr + "%' " + " OR outRR.buildingname LIKE '%" + searchStr + "%' OR outRR.name LIKE '%" + searchStr + "%' ) ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_for_server(
      string startno,
      string endno,
      string orderby,
      string searchStr,
      string orderdir,
      string from,
      string to,
      string building,
      string level,
      string category,
      string type,
      string status,
      string requested_by,
      Guid account_id,
      string group_ids,
      long user_id,
      string email_id)
    {
      try
      {
        long num = !(requested_by != "%") ? user_id : Convert.ToInt64(requested_by);
        string str1 = "SELECT * FROM (SELECT ROW_NUMBER() OVER (" + (!(orderby == "code") ? "ORDER BY  RR." + orderby + " " + orderdir : "ORDER BY  RR.code,RR.name  " + orderdir) + ") AS RowNumber,RR.* FROM (SELECT DISTINCT " + " d.asset_id,A.purpose,A.book_from,A.book_to,a.booked_for,a.booking_id,d.code,d.name,a.status as status1,(CASE a.status WHEN 1 THEN 'Booked' WHEN 2  THEN 'Blocked' WHEN 3 THEN  'No Show' WHEN 0 THEN  'Cancelled' WHEN 4 THEN 'Pending' when 6 then 'Rejected' when 5 then 'Withdraw' when 7 then 'Auto Rejected' END ) AS Status,(SELECT E.full_name FROM sbt_users E WHERE E.user_id=A.booked_for and E.account_id='" + (object) account_id + "') AS RequestedBy," + " (SELECT value FROM sbt_settings  WHERE setting_id= D.building_id and account_id='" + (object) account_id + "') AS BuildingName " + " FROM sbt_asset_bookings a,sbt_asset_properties b,sbt_settings c,sbt_assets d " + " WHERE a.account_id='" + (object) account_id + "' " + " AND a.asset_id=d.asset_id " + " and d.status=(select setting_id from sbt_settings where value='available' and parameter='asset_status' and account_id='" + (object) account_id + "')" + " and (d.asset_owner_group_id IN (" + group_ids + ") or  a.booked_for=" + (object) num + ") and d.building_id   LIKE '" + building + "' AND d.level_id LIKE '" + level + "' AND d.category_id LIKE '" + category + "' AND d.asset_type LIKE '" + type + "' ";
        if (requested_by != "%")
          str1 = str1 + " and (a.booked_for LIKE '" + requested_by + "'" + " or a.booking_id in (select booking_id from sbt_asset_booking_invites where email='" + email_id + "' and account_id='" + (object) account_id + "'))";
        string str2 = (!(status != "%") ? str1 + " and a.status   like '" + status + "'" : str1 + " and a.status in (" + status + ")") + "   AND b.property_name='asset_property'AND b.property_value=convert(NVARCHAR(10), c.setting_id) AND (B.status=0 or (B.status=1 and B.available=1)) " + " and a.booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status= " + (object) this.get_status("Cancelled") + ") and transfer_request=1 and account_id='" + (object) account_id + "') " + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + ") AS RR  " + " WHERE " + " (RR.purpose LIKE '%" + searchStr + "%' OR RR.book_from LIKE '%" + searchStr + "%' " + " OR RR.book_to LIKE '%" + searchStr + "%' OR RR.booked_for LIKE '%" + searchStr + "%' OR RR.status LIKE '%" + searchStr + "%' OR RR.RequestedBy LIKE  '%" + searchStr + "%' " + " OR RR.buildingname LIKE '%" + searchStr + "%' or RR.code Like '%" + searchStr + "%' OR RR.name Like '%" + searchStr + "%' ) ) AS outRR  where  outRR.RowNumber BETWEEN " + startno + " AND  " + endno + "SELECT count(*) FROM (SELECT ROW_NUMBER() OVER (ORDER BY RR." + orderby + " " + orderdir + ") AS RowNumber,RR.* FROM (SELECT DISTINCT " + " A.purpose,A.book_from,A.book_to,a.booked_for,a.booking_id,d.code,d.name,a.status as status1,(CASE a.status WHEN 1 THEN 'Booked' WHEN 2  THEN 'Blocked' WHEN 3 THEN  'No Show' WHEN 0 THEN  'Cancelled' WHEN 4 THEN 'Pending' when 6 then 'Rejected' when 5 then 'Withdraw' when 7 then 'Auto Rejected' END ) AS Status,(SELECT E.full_name FROM sbt_users E WHERE E.user_id=A.booked_for) AS RequestedBy," + " (SELECT value FROM sbt_settings  WHERE setting_id= D.building_id and account_id='" + (object) account_id + "') AS BuildingName " + " FROM sbt_asset_bookings a,sbt_asset_properties b,sbt_settings c,sbt_assets d " + " WHERE a.account_id='" + (object) account_id + "' " + " AND a.asset_id=d.asset_id  " + " and d.status=(select setting_id from sbt_settings where value='available' and parameter='asset_status' and account_id='" + (object) account_id + "')" + " and ( d.asset_owner_group_id IN (" + group_ids + ")  or  a.booked_for=" + (object) num + ") and d.building_id   LIKE '" + building + "' AND d.level_id LIKE '" + level + "' AND d.category_id LIKE '" + category + "' AND d.asset_type LIKE '" + type + "'";
        if (requested_by != "%")
          str2 = str2 + " and (a.booked_for LIKE '" + requested_by + "'" + " or a.booking_id in (select booking_id from sbt_asset_booking_invites where email='" + email_id + "' and account_id='" + (object) account_id + "'))";
        return this.db.get_dataset((!(status != "%") ? str2 + " and a.status  like '" + status + "'" : str2 + " and a.status  in (" + status + ")") + " AND b.property_name='asset_property'AND b.property_value=convert(NVARCHAR(10), c.setting_id) AND (B.status=0 or (B.status=1 and B.available=1)) " + " and a.booking_id NOT IN (SELECT transfer_original_booking_id FROM sbt_asset_bookings WHERE (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + "   or status= " + (object) this.get_status("Cancelled") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "') " + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + ") AS RR) AS outRR " + " WHERE ( outRR.purpose LIKE '%" + searchStr + "%' OR outRR.book_from LIKE '%" + searchStr + "%' " + " OR outRR.book_to LIKE '%" + searchStr + "%' OR outRR.booked_for LIKE '%" + searchStr + "%' OR outRR.status LIKE '%" + searchStr + "%' OR outRR.RequestedBy LIKE  '%" + searchStr + "%' " + " OR outRR.buildingname LIKE '%" + searchStr + "%' OR outRR.name LIKE '%" + searchStr + "%' ) ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(long asset_id, long booking_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_bookings where asset_id='" + (object) asset_id + "' and   booking_id='" + (object) booking_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      long asset_id,
      Guid account_id,
      int status,
      string from,
      string to)
    {
      try
      {
        return this.db.get_dataset("SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row," + " A.created_by,A.remarks,A.book_from,A.book_to,A.booking_id,(SELECT full_name FROM sbt_users B WHERE B.user_id=A.created_by and B.account_id='" + (object) account_id + "') AS full_name" + " FROM sbt_asset_bookings A where " + " account_id='" + (object) account_id + "' and  status =" + (object) status + " and A.booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + "  ) and transfer_request=1 and account_id='" + (object) account_id + "')" + "  and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to )) " + " AND asset_id=" + (object) asset_id + " )  AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + " AND (t2.full_name LIKE '" + searchkey + "%' OR t2.remarks LIKE '" + searchkey + "%' ); " + " SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER by booking_id) AS Row,*" + " FROM sbt_asset_bookings) AS t2  WHERE " + " account_id='" + (object) account_id + "' and  status =" + (object) status + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + "  or status=" + (object) this.get_status("Cancelled") + ") and transfer_request=1 and account_id='" + (object) account_id + "')" + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + " AND asset_id=" + (object) asset_id) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      long asset_id,
      Guid account_id,
      string from,
      string to)
    {
      try
      {
        return this.db.get_dataset("SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,t2.* from" + " (SELECT a.booking_id,a.book_from,a.book_to,a.booked_for,a.status ,(SELECT full_name FROM sbt_users  WHERE user_id=a.booked_for and account_id='" + (object) account_id + "') " + " AS RequestedBy FROM sbt_asset_bookings a where " + " a.account_id='" + (object) account_id + "' " + " and a.booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')" + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + " AND a.asset_id=" + (object) asset_id + " " + "  )  AS t2 WHERE  t2.book_from like '" + searchkey + "%'  or t2.book_to like '" + searchkey + "%' OR t2.RequestedBy LIKE '" + searchkey + "%') as result where result.Row BETWEEN " + fromrow + " AND " + endrow + "; " + "  SELECT COUNT(*) AS  RecordCnt FROM (SELECT Row_Number() OVER(ORDER by booking_id) AS Row,t2.*" + " FROM ( SELECT a.booking_id,a.book_from,a.book_to,a.booked_for,a.status , (SELECT full_name FROM sbt_users  WHERE user_id=a.booked_for and account_id='" + (object) account_id + "') AS RequestedBy  FROM  sbt_asset_bookings a WHERE   " + " a.account_id='" + (object) account_id + "' " + "  and a.booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + "  or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')" + " and ((book_from between '" + from + "' and '" + to + "') or " + "(book_to between '" + from + "' and '" + to + "')  " + "OR ('" + from + "' between  book_from   and  book_to) or " + "('" + to + "'  between  book_from   and book_to ))" + " AND a.asset_id=" + (object) asset_id + " ) AS t2  where t2.book_from like '" + searchkey + "'  or t2.book_to like '" + searchkey + "' OR t2.RequestedBy LIKE '" + searchkey + "') as Result") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet is_available_forBlocked(
      long asset_id,
      DateTime from,
      DateTime to,
      Guid account_id,
      short status)
    {
      try
      {
        return this.db.get_dataset("select booking_id,status from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' " + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')" + "  and ((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or " + "(book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "')  " + "OR ('" + from.ToString(api_constants.sql_datetime_format) + "' between  book_from   and  book_to) or " + "('" + to.ToString(api_constants.sql_datetime_format) + "'  between  book_from   and book_to )) " + " and status <>" + (object) status) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public bool is_available(
      long asset_id,
      DateTime from,
      DateTime to,
      Guid account_id,
      short status)
    {
      try
      {
        if (!this.db.get_dataset("select count(booking_id) from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' " + "  and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=" + (object) this.get_status("Booked") + " or status= " + (object) this.get_status("Blocked") + "  or status= " + (object) this.get_status("No Show") + " or status=" + (object) this.get_status("Cancelled") + " ) and transfer_request=1 and account_id='" + (object) account_id + "')" + "  and ((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or " + "(book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "')  " + "OR ('" + from.ToString(api_constants.sql_datetime_format) + "' between  book_from   and  book_to) or " + "('" + to.ToString(api_constants.sql_datetime_format) + "'  between  book_from   and book_to )) " + " and status=" + (object) status))
          return false;
        int num = 0;
        foreach (DataTable table in (InternalDataCollectionBase) this.db.resultDataSet.Tables)
          num += Convert.ToInt32(table.Rows[0][0]);
        this.db.resultDataSet.Dispose();
        return num <= 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public DataSet get_faulty_asset_id(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT distinct a.asset_id ,b.property_name FROM sbt_assets a,sbt_asset_properties b WHERE a.account_id='" + (object) account_id + "' and a.asset_id=b.asset_id AND b.property_name='asset_property' AND b.available=0 AND b.remarks <> ''") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_mail(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select distinct email,asset_id,book_from from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and cast(book_from as datetime) between  cast(getdate() as datetime) and convert(datetime,convert(varchar,cast(dateadd(day," + ConfigurationManager.AppSettings["asset_property_email_days"].ToString() + ",getdate()) as date)) + ' 23:59:59.000')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    private asset_booking get_booking_object(string sql)
    {
      asset_booking bookingObject = new asset_booking();
      bookingObject.booking_id = 0L;
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
              bookingObject.booking_id = (long) objArray[index1];
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
              bookingObject.account_id = (Guid) objArray[index2];
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
              bookingObject.created_on = (DateTime) objArray[index3];
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
              bookingObject.created_by = (long) objArray[index4];
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
              bookingObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              bookingObject.modified_on = bookingObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              bookingObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              bookingObject.modified_by = bookingObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              bookingObject.purpose = (string) objArray[index7];
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
              bookingObject.asset_id = (long) objArray[index8];
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
              bookingObject.book_from = (DateTime) objArray[index9];
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
              bookingObject.book_to = (DateTime) objArray[index10];
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
              bookingObject.book_duration = Convert.ToDouble(objArray[index11]);
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
              bookingObject.status = (short) objArray[index12];
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
              bookingObject.record_id = (Guid) objArray[index13];
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
              bookingObject.remarks = (string) objArray[index14];
            }
            catch
            {
              bookingObject.remarks = "";
            }
          }
          int index15 = index14 + 1;
          if (this.is_valid(objArray[index15]))
          {
            try
            {
              bookingObject.is_repeat = (bool) objArray[index15];
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
              bookingObject.transfer_request = (bool) objArray[index16];
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
              bookingObject.transfer_original_booking_id = (long) objArray[index17];
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
              bookingObject.transfer_reject_reason = (string) objArray[index18];
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
              bookingObject.cancel_on = (DateTime) objArray[index19];
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
              bookingObject.cancel_by = (long) objArray[index20];
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
              bookingObject.cancel_reason = (string) objArray[index21];
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
              bookingObject.transfer_reason = (string) objArray[index22];
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
              bookingObject.contact = (string) objArray[index23];
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
              bookingObject.email = (string) objArray[index24];
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
              bookingObject.booked_for = (long) objArray[index25];
            }
            catch
            {
            }
          }
          int index26 = index25 + 1;
          if (this.is_valid(objArray[index26]))
          {
            try
            {
              bookingObject.setup_required = (bool) objArray[index26];
            }
            catch
            {
            }
          }
          int index27 = index26 + 1;
          if (this.is_valid(objArray[index27]))
          {
            try
            {
              bookingObject.setup_type = (long) objArray[index27];
            }
            catch
            {
            }
          }
          int index28 = index27 + 1;
          if (this.is_valid(objArray[index28]))
          {
            try
            {
              bookingObject.housekeeping_required = (bool) objArray[index28];
            }
            catch
            {
            }
          }
          int index29 = index28 + 1;
          if (this.is_valid(objArray[index29]))
          {
            try
            {
              bookingObject.repeat_reference_id = (Guid) objArray[index29];
            }
            catch
            {
            }
          }
          int index30 = index29 + 1;
          if (this.is_valid(objArray[index30]))
          {
            try
            {
              bookingObject.booking_type = (int) objArray[index30];
            }
            catch
            {
            }
          }
          int index31 = index30 + 1;
          if (this.is_valid(objArray[index31]))
          {
            try
            {
              bookingObject.meeting_type = (long) objArray[index31];
            }
            catch
            {
            }
          }
          int index32 = index31 + 1;
          try
          {
            if (this.is_valid(objArray[index32]))
            {
              try
              {
                bookingObject.event_id = (Guid) objArray[index32];
              }
              catch
              {
              }
            }
          }
          catch
          {
          }
          int index33 = index32 + 1;
          try
          {
            if (this.is_valid(objArray[index33]))
            {
              try
              {
                bookingObject.sequence = (int) objArray[index33];
              }
              catch
              {
              }
            }
          }
          catch
          {
          }
          bookingObject.invites = new Dictionary<long, asset_booking_invite>();
          try
          {
            bookingObject.invites = this.get_invite_list(bookingObject.booking_id, bookingObject.account_id);
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        bookingObject.booking_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return bookingObject;
    }

    public asset_booking get_booking(long booking_id, Guid account_id) => this.get_booking_object("select * from sbt_asset_bookings where booking_id='" + (object) booking_id + "' and account_id='" + (object) account_id + "'");

    public asset_booking get_booking_eventId(Guid event_id, Guid account_id) => this.get_booking_object("select top 1 * from sbt_asset_bookings where event_id='" + (object) event_id + "' and account_id='" + (object) account_id + "'");

    public DataSet get_AssetID(Guid event_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select asset_id from sbt_asset_bookings where event_id='" + (object) event_id + "'and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_booking_assteid(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select count(booking_id) from sbt_asset_bookings where (status =" + (object) api_constants.booking_status["Booked"] + " or status=" + (object) api_constants.booking_status["Pending"] + ") and asset_id='" + (object) asset_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public asset_booking update_booking(asset_booking obj)
    {
      bool flag;
      try
      {
        flag = this.db.execute_procedure("sbt_sp_asset_booking_update", new Dictionary<string, object>()
        {
          {
            "@booking_id",
            (object) obj.booking_id
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
            "@purpose",
            (object) obj.purpose
          },
          {
            "@asset_id",
            (object) obj.asset_id
          },
          {
            "@book_from",
            (object) obj.book_from
          },
          {
            "@book_to",
            (object) obj.book_to
          },
          {
            "@book_duration",
            (object) obj.book_duration
          },
          {
            "@status",
            (object) obj.status
          },
          {
            "@remarks",
            (object) obj.remarks
          },
          {
            "@is_repeat",
            (object) obj.is_repeat
          },
          {
            "@repeat_reference_id",
            (object) obj.repeat_reference_id
          },
          {
            "@transfer_request",
            (object) obj.transfer_request
          },
          {
            "@transfer_original_booking_id",
            (object) obj.transfer_original_booking_id
          },
          {
            "@transfer_reason",
            (object) obj.transfer_reason
          },
          {
            "@contact",
            (object) obj.contact
          },
          {
            "@email",
            (object) obj.email
          },
          {
            "@booked_for",
            (object) obj.booked_for
          },
          {
            "@setup_required",
            (object) obj.setup_required
          },
          {
            "@setup_type",
            (object) obj.setup_type
          },
          {
            "@housekeeping_required",
            (object) obj.housekeeping_required
          },
          {
            "@booking_type",
            (object) obj.booking_type
          },
          {
            "@meeting_type",
            (object) obj.meeting_type
          }
        });
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ("asset booking : " + ex.ToString()));
      }
      obj.booking_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public asset_booking delete_booking(asset_booking obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_asset_booking_delete", new Dictionary<string, object>()
        {
          {
            "@booking_id",
            (object) obj.booking_id
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
        {
          obj.booking_id = Convert.ToInt64(this.db.resultString);
          if (obj.invites != null)
          {
            if (obj.invites.Count > 0)
            {
              foreach (long key in obj.invites.Keys)
                this.delete_invite(obj.invites[key]);
            }
          }
        }
        else
          obj.booking_id = 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.booking_id = 0L;
      }
      return obj;
    }

    public asset_booking update_booking_status(asset_booking obj)
    {
      try
      {
        obj.booking_id = !this.db.execute_procedure("sbt_sp_asset_booking_status_update", new Dictionary<string, object>()
        {
          {
            "@booking_id",
            (object) obj.booking_id
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
          },
          {
            "@booking_status",
            (object) obj.status
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.booking_id = 0L;
      }
      return obj;
    }

    public bool update_booking_reassign(asset_booking obj, string bids)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_asset_booking_reassign_update", new Dictionary<string, object>()
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
            "@reassignto",
            (object) obj.created_by
          },
          {
            "@booked_for",
            (object) obj.booked_for
          },
          {
            "@purpose",
            (object) obj.purpose
          },
          {
            "@booking_ids",
            (object) bids
          },
          {
            "@email",
            (object) obj.email
          }
        }))
          obj.created_by = Convert.ToInt64(this.db.resultString);
        else
          obj.created_by = 0L;
        return obj.created_by > 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public bool update_event_id(long booking_id, Guid event_id, Guid account_id) => this.db.get_nonquery("update sbt_asset_bookings set event_id='" + (object) event_id + "' where booking_id=" + (object) booking_id + "and account_id='" + (object) account_id + "'");

    public asset_booking cancel_booking(asset_booking obj)
    {
      try
      {
        obj.booking_id = !this.db.execute_procedure("sbt_sp_asset_booking_cancel", new Dictionary<string, object>()
        {
          {
            "@booking_id",
            (object) obj.booking_id
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
          },
          {
            "@booking_status",
            (object) obj.status
          },
          {
            "@cancel_reason",
            (object) obj.cancel_reason
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.booking_id = 0L;
      }
      return obj;
    }

    public bool actualize_booking(long booking_id, Guid acctount_id, string endDT)
    {
      bool flag = false;
      try
      {
        if (this.db.execute_procedure("sbt_sp_actualize_booking_update", new Dictionary<string, object>()
        {
          {
            "@booking_id",
            (object) booking_id
          },
          {
            "@account_id",
            (object) acctount_id
          },
          {
            "@book_to",
            (object) endDT
          }
        }))
          flag = true;
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
      }
      return flag;
    }

    public DataSet get_user_groupname(long user_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT group_name FROM sbt_user_groups WHERE group_id IN(SELECT group_id FROM sbt_user_group_mappings WHERE user_id=" + (object) user_id + " and account_id='" + (object) account_id + "' ) and  account_id='" + (object) account_id + "' AND group_name <>'" + api_constants.all_users_text + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet checkMeetingStarted(long bookingid, Guid Account_Id, DateTime current)
    {
      try
      {
        return this.db.get_dataset("SELECT booking_id FROM sbt_asset_bookings WHERE  '" + current.ToString(api_constants.display_datetime_format) + "' >=book_from  AND booking_id=" + (object) bookingid + " and  account_id='" + (object) Account_Id + "' ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public bool CheckMeetingsover(long bookingid, Guid Account_Id, DateTime current)
    {
      bool flag = true;
      try
      {
        flag = this.db.get_dataset("SELECT booking_id FROM sbt_asset_bookings WHERE  '" + current.ToString(api_constants.display_datetime_format) + "' <=book_to  AND booking_id=" + (object) bookingid + " and  account_id='" + (object) Account_Id + "' ") && this.db.resultDataSet.Tables[0].Rows.Count > 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return flag;
    }

    public bool Check_isTransferBooking(long booking_id, Guid account_id)
    {
      try
      {
        if (!this.db.get_dataset("select booking_id from sbt_asset_bookings where transfer_original_booking_id=" + (object) booking_id + "  and status=" + (object) this.get_status("Pending") + " and account_id='" + (object) account_id + "'"))
          return false;
        DataSet resultDataSet = this.db.resultDataSet;
        return resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public Dictionary<long, asset_booking_invite> get_invite_list(long booking_id, Guid account_id)
    {
      Dictionary<long, asset_booking_invite> inviteList = new Dictionary<long, asset_booking_invite>();
      try
      {
        DataSet invites = this.get_invites(booking_id, account_id);
        foreach (DataRow row in (InternalDataCollectionBase) invites.Tables[0].Rows)
        {
          asset_booking_invite invite = this.get_invite(Convert.ToInt64(row["booking_invite_id"]), account_id);
          inviteList.Add(invite.booking_invite_id, invite);
        }
        invites.Dispose();
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return inviteList;
    }

    public DataSet get_invites(long booking_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_asset_booking_invites where account_id='" + (object) account_id + "' and booking_id='" + (object) booking_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public asset_booking_invite get_invite(long booking_invite_id, Guid account_id)
    {
      asset_booking_invite invite = new asset_booking_invite();
      invite.booking_invite_id = 0L;
      string str = "select * from sbt_asset_booking_invites where booking_invite_id='" + (object) booking_invite_id + "' and account_id='" + (object) account_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          int index1 = 0;
          if (this.is_valid(objArray[index1]))
            invite.booking_invite_id = (long) objArray[index1];
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
            invite.account_id = (Guid) objArray[index2];
          int index3 = index2 + 1;
          if (this.is_valid(objArray[index3]))
            invite.created_on = (DateTime) objArray[index3];
          int index4 = index3 + 1;
          if (this.is_valid(objArray[index4]))
            invite.created_by = (long) objArray[index4];
          int index5 = index4 + 1;
          try
          {
            invite.modified_on = (DateTime) objArray[index5];
          }
          catch
          {
            invite.modified_on = invite.created_on;
          }
          int index6 = index5 + 1;
          try
          {
            invite.modified_by = (long) objArray[index6];
          }
          catch
          {
            invite.modified_by = invite.created_by;
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
            invite.name = (string) objArray[index7];
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
            invite.email = (string) objArray[index8];
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
            invite.booking_id = (long) objArray[index9];
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
            invite.repeat_reference_id = (Guid) objArray[index10];
          int index11 = index10 + 1;
          if (this.is_valid(objArray[index11]))
            invite.is_attending = (int) objArray[index11];
          int index12 = index11 + 1;
          if (this.is_valid(objArray[index12]))
            invite.record_id = (Guid) objArray[index12];
          int num = index12 + 1;
        }
      }
      catch (Exception ex)
      {
        invite.booking_invite_id = 0L;
        this.log.Error((object) str, ex);
      }
      return invite;
    }

    public asset_booking_invite update_invite(asset_booking_invite obj)
    {
      try
      {
        obj.booking_invite_id = !this.db.execute_procedure("sbt_sp_asset_booking_invite_update", new Dictionary<string, object>()
        {
          {
            "@booking_invite_id",
            (object) obj.booking_invite_id
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
            "@name",
            (object) obj.name
          },
          {
            "@email",
            (object) obj.email
          },
          {
            "@booking_id",
            (object) obj.booking_id
          },
          {
            "@repeat_reference_id",
            (object) obj.repeat_reference_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public asset_booking_invite delete_invite(asset_booking_invite obj)
    {
      try
      {
        obj.booking_invite_id = !this.db.execute_procedure("sbt_sp_asset_booking_invite_delete", new Dictionary<string, object>()
        {
          {
            "@booking_invite_id",
            (object) obj.booking_invite_id
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

    public void update_invite_status(long booking_id, long booking_invite_id, string action)
    {
      string sql;
      if (action == "going")
        sql = "update sbt_asset_booking_invites set is_attending=1,attendance_updated_on= GETDATE() where booking_invite_id=" + (object) booking_invite_id + " and booking_id=" + (object) booking_id;
      else
        sql = "update sbt_asset_booking_invites set is_attending=0,attendance_updated_on= GETDATE() where booking_invite_id=" + (object) booking_invite_id + " and booking_id=" + (object) booking_id;
      this.db.execute_scalar(sql);
    }

    public book_slot_item get_book_status(DateTime date, long asset_id, Guid account_id) => this.get_booking_status_object("select * from sbt_asset_bookings_slot where asset_id='" + (object) asset_id + "' and account_id='" + (object) account_id + "' and date='" + date.ToString("yyyy-MM-dd 00:00:00") + "'");

    public book_slot_item update_book_status(book_slot_item obj)
    {
      try
      {
        obj.booking_slot_id = !this.db.execute_procedure("sbt_sp_asset_bookings_slot_update", new Dictionary<string, object>()
        {
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@booking_slot_id",
            (object) obj.booking_slot_id
          },
          {
            "@date",
            (object) obj.date
          },
          {
            "@asset_id",
            (object) obj.asset_id
          },
          {
            "@user_id",
            (object) obj.user_id
          },
          {
            "@book_slot",
            (object) obj.book_slot
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public book_slot_item set_cancel_status(asset_booking obj)
    {
      book_slot_item bookSlotItem = new book_slot_item();
      if (obj.book_from.Day == obj.book_to.Day && obj.book_from.Month == obj.book_to.Month && obj.book_from.Year == obj.book_to.Year)
      {
        bookSlotItem = this.cancel_process(obj.book_from, obj.book_to, obj.asset_id, obj.account_id, 0L);
      }
      else
      {
        Dictionary<DateTime, DateTime> dictionary = new Dictionary<DateTime, DateTime>();
        dictionary.Add(obj.book_from, new DateTime(obj.book_from.Year, obj.book_from.Month, obj.book_from.Day, 23, 45, 0));
        dictionary.Add(new DateTime(obj.book_to.Year, obj.book_to.Month, obj.book_to.Day, 0, 0, 0), obj.book_to.AddMinutes(-15.0));
        foreach (DateTime key in dictionary.Keys)
          bookSlotItem = this.cancel_process(key, dictionary[key], obj.asset_id, obj.account_id, 0L);
      }
      return bookSlotItem;
    }

    private book_slot_item cancel_process(
      DateTime book_from,
      DateTime book_to,
      long asset_id,
      Guid account_id,
      long user_id)
    {
      book_slot_item bookSlotItem = new book_slot_item();
      List<string> stringList = new List<string>();
      for (DateTime dateTime = book_from; dateTime <= book_to; dateTime = dateTime.AddMinutes(15.0))
        stringList.Add(dateTime.ToString("HHmm"));
      book_slot_item bookStatus = this.get_book_status(new DateTime(book_from.Year, book_from.Month, book_from.Day, 0, 0, 0), asset_id, account_id);
      bookStatus.account_id = account_id;
      bookStatus.asset_id = asset_id;
      bookStatus.user_id = user_id;
      bookStatus.date = new DateTime(book_from.Year, book_from.Month, book_from.Day, 0, 0, 0);
      foreach (string key in stringList)
        bookStatus.book_slot_dic[key] = false;
      bookStatus.book_slot = "";
      foreach (string key in bookStatus.book_slot_dic.Keys)
        bookStatus.book_slot = !bookStatus.book_slot_dic[key] ? bookStatus.book_slot + "0" : bookStatus.book_slot + "1";
      return this.update_book_status(bookStatus);
    }

    private book_slot_item do_process(
      DateTime book_from,
      DateTime book_to,
      long asset_id,
      Guid account_id,
      long user_id)
    {
      book_slot_item bookSlotItem = new book_slot_item();
      List<string> stringList = new List<string>();
      for (DateTime dateTime = book_from; dateTime <= book_to; dateTime = dateTime.AddMinutes(15.0))
        stringList.Add(dateTime.ToString("HHmm"));
      book_slot_item bookStatus = this.get_book_status(new DateTime(book_from.Year, book_from.Month, book_from.Day, 0, 0, 0), asset_id, account_id);
      bookStatus.account_id = account_id;
      bookStatus.asset_id = asset_id;
      bookStatus.user_id = user_id;
      bookStatus.date = new DateTime(book_from.Year, book_from.Month, book_from.Day, 0, 0, 0);
      foreach (string key in stringList)
        bookStatus.book_slot_dic[key] = true;
      bookStatus.book_slot = "";
      foreach (string key in bookStatus.book_slot_dic.Keys)
        bookStatus.book_slot = !bookStatus.book_slot_dic[key] ? bookStatus.book_slot + "0" : bookStatus.book_slot + "1";
      return this.update_book_status(bookStatus);
    }

    public book_slot_item set_book_status(asset_booking obj, asset_booking obj_previous)
    {
      book_slot_item bookSlotItem = new book_slot_item();
      if (obj_previous.booking_id > 0L)
        this.set_cancel_status(obj_previous);
      if (obj.book_from.Day == obj.book_to.Day && obj.book_from.Month == obj.book_to.Month && obj.book_from.Year == obj.book_to.Year)
      {
        bookSlotItem = this.do_process(obj.book_from, obj.book_to, obj.asset_id, obj.account_id, 0L);
      }
      else
      {
        Dictionary<DateTime, DateTime> dictionary = new Dictionary<DateTime, DateTime>();
        double totalDays = (obj.book_to - obj.book_from).TotalDays;
        DateTime dateTime1 = new DateTime(obj.book_from.Year, obj.book_from.Month, obj.book_from.Day, 0, 0, 0);
        DateTime dateTime2 = new DateTime(obj.book_to.Year, obj.book_to.Month, obj.book_to.Day, 0, 0, 0);
        DateTime bookFrom = obj.book_from;
        DateTime dateTime3 = new DateTime(obj.book_from.Year, obj.book_from.Month, obj.book_from.Day, 23, 45, 0);
        while (dateTime1 <= dateTime2)
        {
          if (dateTime1.Year == obj.book_from.Year && dateTime1.Month == obj.book_from.Month && dateTime1.Day == obj.book_from.Day)
            dictionary.Add(obj.book_from, new DateTime(obj.book_from.Year, obj.book_from.Month, obj.book_from.Day, 23, 45, 0));
          if (dateTime2.Year == obj.book_to.Year && dateTime2.Month == obj.book_to.Month && dateTime2.Day == obj.book_to.Day)
            dictionary.Add(new DateTime(obj.book_to.Year, obj.book_to.Month, obj.book_to.Day, 0, 0, 0), obj.book_to);
        }
        foreach (DateTime key in dictionary.Keys)
          bookSlotItem = this.do_process(key, dictionary[key], obj.asset_id, obj.account_id, 0L);
      }
      return bookSlotItem;
    }

    private book_slot_item get_booking_status_object(string sql)
    {
      book_slot_item bookingStatusObject = new book_slot_item();
      bookingStatusObject.booking_slot_id = 0L;
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
              bookingStatusObject.booking_slot_id = (long) objArray[index1];
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
              bookingStatusObject.date = (DateTime) objArray[index2];
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
              bookingStatusObject.asset_id = (long) objArray[index3];
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
              bookingStatusObject.book_slot = (string) objArray[index4];
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
              bookingStatusObject.account_id = (Guid) objArray[index5];
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
              bookingStatusObject.user_id = (long) objArray[index6];
            }
            catch
            {
            }
          }
          char[] charArray = bookingStatusObject.book_slot.ToCharArray();
          bookingStatusObject.book_slot_dic = new Dictionary<string, bool>();
          DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
          for (int index7 = 0; index7 < 96; ++index7)
          {
            if (charArray[index7] == '0')
              bookingStatusObject.book_slot_dic.Add(dateTime.ToString("HHmm"), false);
            else
              bookingStatusObject.book_slot_dic.Add(dateTime.ToString("HHmm"), true);
            dateTime = dateTime.AddMinutes(15.0);
          }
        }
        else
        {
          bookingStatusObject.book_slot_dic = new Dictionary<string, bool>();
          DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
          for (int index = 0; index < 96; ++index)
          {
            string key = dateTime.ToString("HHmm");
            bookingStatusObject.book_slot_dic.Add(key, false);
            bookingStatusObject.book_slot += "0";
            dateTime = dateTime.AddMinutes(15.0);
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        bookingStatusObject.booking_slot_id = 0L;
      }
      return bookingStatusObject;
    }

    public short get_booking_status(
      long asset_owner_group_id,
      user user_object,
      groups_permission permission_object)
    {
      short bookingStatus;
      if (permission_object.isAdminType)
        bookingStatus = (short) 1;
      else if (asset_owner_group_id > 0L)
      {
        bool flag = false;
        foreach (string key in user_object.groups.Keys)
        {
          if (user_object.groups[key].group_id == asset_owner_group_id)
          {
            flag = true;
            break;
          }
        }
        bookingStatus = !flag ? (short) 4 : (short) 1;
      }
      else
        bookingStatus = (short) 1;
      return bookingStatus;
    }

    public DataSet get_confirmed_bookings(
      Guid account_id,
      long room_id,
      DateTime from,
      DateTime to)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT booking_id,asset_id,book_from,book_to,name,purpose,is_repeat,repeat_reference_id,status from vw_sbt_asset_bookings where account_id='" + (object) account_id + "' and (status='1' or status='2')";
        if (room_id > 0L)
          str3 = str3 + " and asset_id='" + (object) room_id + "' ";
        return this.db.get_dataset(str3 + " and ((book_from between '" + str1 + "' and '" + str2 + "') or (book_to between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between book_from and book_to) or ('" + str2 + "' between book_from and book_to)) order by book_from asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_confirmed_bookings(
      Guid account_id,
      long room_id,
      DateTime from,
      DateTime to,
      long user_id)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT booking_id,asset_id,book_from,book_to,name,purpose,is_repeat,repeat_reference_id,status from vw_sbt_asset_bookings where account_id='" + (object) account_id + "' and (status='1' or status='2')";
        if (user_id > 0L)
          str3 = str3 + " and (created_by='" + (object) user_id + "' or booked_for='" + (object) user_id + "') ";
        if (room_id > 0L)
          str3 = str3 + " and asset_id='" + (object) room_id + "' ";
        return this.db.get_dataset(str3 + " and ((book_from between '" + str1 + "' and '" + str2 + "') or (book_to between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between book_from and book_to) or ('" + str2 + "' between book_from and book_to)) order by book_from asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_confirmed_bookings_by_invite(
      Guid account_id,
      long room_id,
      DateTime from,
      DateTime to,
      string email)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT booking_id,asset_id,book_from,book_to,name,purpose,is_repeat,repeat_reference_id,status from vw_sbt_asset_bookings where account_id='" + (object) account_id + "' and status='1'" + " and ((book_from between '" + str1 + "' and '" + str2 + "') or (book_to between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between book_from and book_to) or ('" + str2 + "' between book_from and book_to))";
        if (email != "")
          str3 = str3 + " and booking_id in (select booking_id from sbt_asset_booking_invites where UPPER(email)='" + email.ToUpper() + "') ";
        return this.db.get_dataset(str3 + " order by book_from asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_confirmed_bookings(
      Guid account_id,
      DateTime from,
      DateTime to,
      string fav_rooms)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT booking_id,asset_id,book_from,book_to,name,purpose,is_repeat,repeat_reference_id,status from vw_sbt_asset_bookings where account_id='" + (object) account_id + "' and (status='1' or status='2')";
        if (fav_rooms != "")
          str3 = str3 + " and asset_id in (" + fav_rooms + ") ";
        return this.db.get_dataset(str3 + " and ((book_from between '" + str1 + "' and '" + str2 + "') or (book_to between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between book_from and book_to) or ('" + str2 + "' between book_from and book_to)) order by book_from asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_confirmed_resource_bookings(
      Guid account_id,
      long resource_id,
      DateTime from,
      DateTime to)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT resource_booking_id,purpose,venue,from_date,to_date,asset_booking_id,repeat_reference,status from vw_sbt_modules_resource_bookings_items where account_id='" + (object) account_id + "' and status='1' and module_name='resource_module'";
        if (resource_id > 0L)
          str3 = str3 + " and resource_id='" + (object) resource_id + "' ";
        return this.db.get_dataset(str3 + " and ((from_date between '" + str1 + "' and '" + str2 + "') or (to_date between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between from_date and to_date) or ('" + str2 + "' between from_date and to_date)) order by from_date asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_confirmed_resource_bookings(
      Guid account_id,
      long resource_id,
      DateTime from,
      DateTime to,
      long user_id)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT resource_booking_id,purpose,venue,from_date,to_date,asset_booking_id,repeat_reference,status from vw_sbt_modules_resource_bookings_items where account_id='" + (object) account_id + "' and status > 0 and module_name='resource_module'";
        if (resource_id > 0L)
          str3 = str3 + " and resource_id='" + (object) resource_id + "' ";
        if (user_id > 0L)
          str3 = str3 + " and (booked_for_id='" + (object) user_id + "' or requested_by='" + (object) user_id + "')";
        return this.db.get_dataset(str3 + " and ((from_date between '" + str1 + "' and '" + str2 + "') or (to_date between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between from_date and to_date) or ('" + str2 + "' between from_date and to_date)) " + " order by from_date asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_confirmed_catering_bookings(
      Guid account_id,
      long resource_id,
      DateTime from,
      DateTime to)
    {
      try
      {
        string str1 = from.ToString(api_constants.sql_datetime_format);
        string str2 = to.ToString(api_constants.sql_datetime_format);
        string str3 = "SELECT resource_booking_id,purpose,venue,from_date,to_date,asset_booking_id,repeat_reference,status from vw_sbt_modules_resource_bookings_items where account_id='" + (object) account_id + "' and status='1' and module_name='catering_module'";
        if (resource_id > 0L)
          str3 = str3 + " and resource_id='" + (object) resource_id + "' ";
        return this.db.get_dataset(str3 + " and ((from_date between '" + str1 + "' and '" + str2 + "') or (to_date between '" + str1 + "' and '" + str2 + "') " + " or ('" + str1 + "' between from_date and to_date) or ('" + str2 + "' between from_date and to_date)) order by from_date asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings2(Guid account_id, DateTime from, DateTime to, long bk_for)
    {
      try
      {
        string str = "  SELECT a.booking_id,a.purpose,a.status,a.asset_id,a.book_from,a.book_to,a.booked_for,a.created_by,a.contact, a.repeat_reference_id,a.remarks ,(select asset_owner_group_id from sbt_assets where asset_id = a.asset_id) as owner_gp_id FROM sbt_asset_bookings a " + " WHERE a.account_id = '" + (object) account_id + "'";
        if (bk_for > 0L)
          str = str + " and (created_by = " + (object) bk_for + " or " + (object) bk_for + " in (select b.user_id from sbt_users as b left join sbt_user_group_mappings as g on b.user_id = g.user_id where group_id = (select asset_owner_group_id from sbt_assets where asset_id = a.asset_id)))";
        return this.db.get_dataset(str + " and a.status != 2 and ((book_from BETWEEN '" + from.ToString("yyyy-MM-dd hh:mm tt") + "' AND '" + to.ToString("yyyy-MM-dd hh:mm tt") + "') or (book_to BETWEEN '" + from.ToString("yyyy-MM-dd hh:mm tt") + "' AND '" + to.ToString("yyyy-MM-dd hh:mm tt") + "'))  ORDER BY a.book_from desc;" + "select transfer_original_booking_id from sbt_asset_bookings where account_id='" + (object) account_id + "' and (status=1 or status=2 or status=3) and transfer_request=1 and ((book_from BETWEEN '" + from.ToString("yyyy-MM-dd hh:mm tt") + "' AND '" + to.ToString("yyyy-MM-dd hh:mm tt") + "') or (book_to BETWEEN '" + from.ToString("yyyy-MM-dd hh:mm tt") + "'AND '" + to.ToString("yyyy-MM-dd hh:mm tt") + "'));") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_booking_for_asset(
      Guid account_id,
      DateTime from,
      DateTime to,
      long asset_id)
    {
      try
      {
        return this.db.get_dataset("select booking_id from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and status not in (" + (object) 0 + "," + (object) 6 + "," + (object) 5 + "," + (object) 3 + ")" + " and (((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or (book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') OR ('" + from.ToString(api_constants.sql_datetime_format) + "' between book_from and book_to) or ('" + to.ToString(api_constants.sql_datetime_format) + "' between book_from and book_to)))") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public string get_appointment_id(long booking_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select global_appointment_id from sbt_outlook where account_id='" + (object) account_id + "' and booking_id='" + (object) booking_id + "'") ? this.db.resultDataSet.Tables[0].Rows[0][0].ToString() : "";
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (string) null;
    }

    public DataSet get_slots(
      Guid account_id,
      DateTime date,
      int start_index,
      int length,
      string pattern)
    {
      try
      {
        if (this.db.get_dataset("SELECT asset_id, substring(book_slot," + (object) start_index + "," + (object) length + ") as book_slot FROM sbt_asset_bookings_slot where account_id='" + (object) account_id + "' and date='" + date.ToString(api_constants.sql_datetime_format) + "' and substring(book_slot," + (object) start_index + "," + (object) length + ") like '%" + pattern + "%'"))
          return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public string get_slots(Guid account_id, DateTime date, long asset_id)
    {
      string slots = "";
      try
      {
        slots = (string) this.db.execute_scalar("SELECT book_slot FROM sbt_asset_bookings_slot where account_id='" + (object) account_id + "' and date='" + date.ToString(api_constants.sql_datetime_format) + "' and asset_id='" + (object) asset_id + "'");
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return slots;
    }

    public bool compare_booking(asset_booking old, asset_booking changed) => old.asset_id != changed.asset_id || old.booked_for != changed.booked_for || old.booking_type != changed.booking_type || old.book_from != changed.book_from || old.book_to != changed.book_to || old.cancel_by != changed.cancel_by || old.cancel_on != changed.cancel_on || old.cancel_reason != changed.cancel_reason || old.contact != changed.contact || old.created_by != changed.created_by || old.email != changed.email || old.housekeeping_required != changed.housekeeping_required || old.invites.Count != changed.invites.Count || old.is_repeat != changed.is_repeat || old.meeting_type != changed.meeting_type || old.purpose != changed.purpose || old.remarks != changed.remarks || old.setup_required != changed.setup_required || old.setup_type != changed.setup_type || (int) old.status != (int) changed.status;

    public book_usage get_usage(long booking_id, Guid account_id)
    {
      string str = "select * from sbt_asset_bookings_usage where account_id='" + (object) account_id + "' and booking_id='" + (object) booking_id + "'";
      book_usage usage = new book_usage();
      usage.usage_id = 0L;
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
              usage.usage_id = (long) objArray[index1];
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
              usage.account_id = (Guid) objArray[index2];
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
              usage.booking_id = (long) objArray[index3];
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
              usage.occupied = (bool) objArray[index4];
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
              usage.occupied_on = (DateTime) objArray[index5];
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
              usage.occupied_by = (long) objArray[index6];
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
              usage.end_on = (DateTime) objArray[index7];
            }
            catch
            {
            }
          }
          int num = index7 + 1;
        }
      }
      catch (Exception ex)
      {
        usage.usage_id = 0L;
        this.log.Error((object) str, ex);
      }
      return usage;
    }

    public book_usage update_usage(book_usage obj)
    {
      try
      {
        obj.usage_id = !this.db.execute_procedure("sbt_sp_asset_bookings_usage_update", new Dictionary<string, object>()
        {
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@usage_id",
            (object) obj.usage_id
          },
          {
            "@occupied",
            (object) obj.occupied
          },
          {
            "@occupied_by",
            (object) obj.occupied_by
          },
          {
            "@booking_id",
            (object) obj.booking_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public long get_register_id(long booking_id, Guid account_id)
    {
      long registerId = 0;
      if (this.db.get_dataset("select register_id from sbt_vms_visit_register where account_id='" + (object) account_id + "' and asset_booking_id='" + (object) booking_id + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
        registerId = Convert.ToInt64(this.db.resultDataSet.Tables[0].Rows[0]["register_id"]);
      return registerId;
    }
  }
}
