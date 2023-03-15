// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.hotdesk_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class hotdesk_api : api_base
  {
    public Dictionary<string, int> asset_status;

    public hotdesk_api()
    {
      this.asset_status = new Dictionary<string, int>();
      this.asset_status.Add("Active", 1);
      this.asset_status.Add("Inactive", 0);
    }

    public DataSet get_layouts(Guid account_id) => this.db.get_dataset("select * from sbt_hotdesk_layout where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet view_layouts(Guid account_id) => this.db.get_dataset("select layout_id,name,image_name from sbt_hotdesk_layout where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_layout(Guid account_id, long layout_id) => this.db.get_dataset("select * from sbt_hotdesk_layout where account_id='" + (object) account_id + "' and layout_id='" + (object) layout_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_layouts(Guid account_id, long building_id, long level_id, long category_id)
    {
      string Sql = "select * from sbt_hotdesk_layout where account_id='" + (object) account_id + "'";
      if (building_id > 0L)
        Sql = Sql + " and building_id='" + (object) building_id + "'";
      if (level_id > 0L)
        Sql = Sql + " and level_id='" + (object) level_id + "'";
      if (category_id > 0L)
        Sql = Sql + " and category_id='" + (object) category_id + "'";
      return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
    }

    public hotdesk_layout update_layout(hotdesk_layout obj) => obj;

    public hotdesk_layout delete_layout(hotdesk_layout obj) => obj;

    public DataSet get_buildings(Guid account_id) => this.db.get_dataset("select a.setting_id,a.value from sbt_settings a, sbt_hotdesk_layout b where a.setting_id=b.building_id and a.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_seats(Guid account_id) => this.db.get_dataset("select * from vw_sbt_hotdesk_seats where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_seat(Guid account_id, long seat_id) => this.db.get_dataset("select * from vw_sbt_hotdesk_seats where account_id='" + (object) account_id + "' and seat_id='" + (object) seat_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_seats(Guid account_id, long layout_id) => this.db.get_dataset("select * from vw_sbt_hotdesk_seats where layout_id='" + (object) layout_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public int get_seat_count(Guid account_id, long layout_id) => Convert.ToInt32(this.db.execute_scalar("select count(seat_id) from sbt_hotdesk_seats where layout_id='" + (object) layout_id + "' and account_id='" + (object) account_id + "'"));

    public hotdesk_seat update_seat(hotdesk_seat obj) => obj;

    public hotdesk_seat delete_seat(hotdesk_seat obj)
    {
      this.db.execute_scalar("update sbt_hotdesk_seats set status=-1,modified_on=getutcdate(),modified_by='" + (object) obj.modified_by + "' where account_id='" + (object) obj.account_id + "' and seat_id='" + (object) obj.seat_id + "'");
      return obj;
    }

    public DataSet get_seat_properties(Guid account_id, long seat_id) => this.db.get_dataset("select * from sbt_hotdesk_seat_properties where seat_id='" + (object) seat_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_seat_by_property_value(Guid account_id, string parameter, string value) => this.db.get_dataset("select * from sbt_hotdesk_seat_properties where parameter='" + parameter + "' and value='" + value + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_bookings(
      Guid account_id,
      DateTime from,
      DateTime to,
      long layout_id,
      long seat_id,
      long status,
      long user_id)
    {
      string Sql = "select * from vw_sbt_hotdesk_bookings where (from_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') and (to_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') and account_id='" + (object) account_id + "'";
      if (layout_id > 0L)
        Sql = Sql + " and layout_id='" + (object) layout_id + "'";
      if (seat_id > 0L)
        Sql = Sql + " and seat_id='" + (object) seat_id + "'";
      if (status > -1L)
        Sql = Sql + " and status='" + (object) status + "'";
      if (user_id > 0L)
        Sql = Sql + " and (requested_by='" + (object) user_id + "' or booked_for_id='" + (object) user_id + "') ";
      return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
    }

    public DataSet get_bookings(
      Guid account_id,
      DateTime from,
      DateTime to,
      long layout_id,
      long building_id,
      long level_id)
    {
      string Sql = "select * from vw_sbt_hotdesk_bookings where (from_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') and (to_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') and account_id='" + (object) account_id + "'";
      if (layout_id > 0L)
        Sql = Sql + " and layout_id='" + (object) layout_id + "'";
      if (building_id > 0L)
        Sql = Sql + " and building_id='" + (object) building_id + "'";
      if (level_id > 0L)
        Sql = Sql + " and level_id='" + (object) level_id + "'";
      return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
    }

    public DataSet get_booking(Guid account_id, long booking_id) => this.db.get_dataset("select * from vw_sbt_hotdesk_bookings where account_id='" + (object) account_id + "' and hotdesk_booking_id='" + (object) booking_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_booking(Guid account_id, Guid record_id) => this.db.get_dataset("select * from vw_sbt_hotdesk_bookings where account_id='" + (object) account_id + "' and record_id='" + (object) record_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public hotdesk_booking release_booking(hotdesk_booking obj)
    {
      this.db.execute_scalar("update sbt_hotdesk_bookings set status=1,to_date='" + obj.to_date.ToString(api_constants.sql_datetime_format) + "', modified_on=getutcdate(),modified_by='" + (object) obj.modified_by + "' where hotdesk_booking_id='" + (object) obj.hotdesk_booking_id + "' and account_id='" + (object) obj.account_id + "'");
      return obj;
    }

    public hotdesk_booking cancel_booking(hotdesk_booking obj)
    {
      this.db.execute_scalar("update sbt_hotdesk_bookings set status=0,modified_on=getutcdate(),modified_by='" + (object) obj.modified_by + "' where hotdesk_booking_id='" + (object) obj.hotdesk_booking_id + "' and account_id='" + (object) obj.account_id + "'");
      return obj;
    }

    public DataSet get_device(Guid dcode) => this.db.get_dataset("select device_id,status,asset_id,account_id from sbt_apps_api_devices where device_code='" + (object) dcode + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device(long device_id) => this.db.get_dataset("select device_id,status,asset_id,account_id from sbt_apps_api_devices where app_config_id=0 and device_id='" + (object) device_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device_settings(Guid dcode) => this.db.get_dataset("select device_item_id,parameter,value from sbt_apps_api_device_settings where app_config_id=0 and device_id in (select device_id from sbt_apps_api_devices where device_code='" + (object) dcode + "')") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device_settings(long device_id) => this.db.get_dataset("select device_item_id,parameter,value from sbt_apps_api_device_settings where device_id='" + (object) device_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public long get_user_by_pin(Guid account_id, string pin)
    {
      object userByPin = this.db.execute_scalar("select user_id from sbt_user_properties where account_id='" + (object) account_id + "' and property_name='pin' and property_value='" + pin + "'");
      try
      {
        return (long) userByPin;
      }
      catch
      {
        return 0;
      }
    }

    public long get_user_by_card(Guid account_id, string pin)
    {
      object userByCard = this.db.execute_scalar("select user_id from sbt_user_properties where account_id='" + (object) account_id + "' and property_name='staff_id' and property_value='" + pin + "'");
      try
      {
        return (long) userByCard;
      }
      catch
      {
        return 0;
      }
    }
  }
}
