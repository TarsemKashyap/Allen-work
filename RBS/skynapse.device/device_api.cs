// Decompiled with JetBrains decompiler
// Type: skynapse.device.device_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace skynapse.device
{
  public class device_api
  {
    private string connection_string;
    public static string sql_datetime_format = "yyyy-MM-dd HH:mm:ss";
    private Dictionary<string, string> room_list;
    private List<Guid> device_codes_list;
    private DataAccess db;
    private asset_api aapi;
    public string error_status = "1";

    public device_api()
    {
      this.connection_string = ConfigurationManager.ConnectionStrings["sql"].ConnectionString;
      this.db = new DataAccess(this.connection_string);
      this.device_codes_list = new List<Guid>();
      this.device_codes_list = this.get_device_codes();
    }

    public Guid get_account_for_device(Guid device_code) => (Guid) this.db.execute_scalar("select account_id from sbt_apps_api_devices where device_code='" + (object) device_code + "'");

    public Dictionary<string, string> get_configuration(Guid device_code)
    {
      Dictionary<string, string> configuration = new Dictionary<string, string>();
      if (this.db.get_dataset("select parameter,value from sbt_apps_api_configurations_items where app_config_id=(select app_config_id from sbt_apps_api_devices where device_code='" + (object) device_code + "')"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          configuration.Add(row["parameter"].ToString(), row["value"].ToString());
      }
      return configuration;
    }

    public void associate_device_with_room(Guid account_id, Guid device_code, long room_id) => this.db.execute_scalar("update sbt_apps_api_devices set asset_id = '" + (object) room_id + "' where device_code = '" + (object) device_code + "' and account_id = '" + (object) account_id + "';" + "update sbt_apps_api_devices set app_config_id=(select app_config_id from sbt_apps_api_configurations_asset_map where asset_id='" + (object) room_id + "' and account_id='" + (object) account_id + "') where device_code = '" + (object) device_code + "' and account_id = '" + (object) account_id + "';");

    public bool is_conflict(
      Guid account_id,
      long asset_id,
      long booking_id,
      DateTime from,
      DateTime to)
    {
      object obj;
      if (booking_id > 0L)
        obj = this.db.execute_scalar("select booking_id from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and booking_id != '" + (object) booking_id + "' and ((book_from between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') or (book_to between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') or ('" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' between book_from and book_to) or ('" + to.ToString("yyyy-MM-dd HH:mm:ss") + "' between book_from and book_to)) and status !=0");
      else
        obj = this.db.execute_scalar("select booking_id from sbt_asset_bookings where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and ((book_from between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') or (book_to between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') or ('" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' between book_from and book_to) or ('" + to.ToString("yyyy-MM-dd HH:mm:ss") + "' between book_from and book_to)) and status !=0");
      bool flag;
      if (obj != null)
      {
        long num;
        try
        {
          num = Convert.ToInt64(obj);
        }
        catch
        {
          num = 0L;
        }
        flag = num > 0L;
      }
      else
        flag = false;
      return flag;
    }

    public bool can_book(Guid account_id, long room_id, long user_id)
    {
      bool flag = false;
      if (this.db.get_dataset("select is_view,is_book from vw_sbt_asset_user_permissions where (group_user_id='" + (object) user_id + "' or user_id='" + (object) user_id + "') and asset_id=" + (object) room_id + " account_id='" + (object) account_id + "' and group_name !='All Users'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
        flag = Convert.ToBoolean(this.db.resultDataSet.Tables[0].Rows[0]["is_view"]);
      return flag;
    }

    public bool device_exists(Guid account_id, string name) => this.db.execute_scalar("select device_id from sbt_apps_api_devices where account_id='" + (object) account_id + "' and UPPER(name)='" + name.ToUpper() + "'") != null;

    public Guid register_device(
      Guid device_code,
      string mac,
      string name,
      string sno,
      Guid account_id,
      long user_id)
    {
      try
      {
        this.db.execute_scalar("insert into sbt_apps_api_devices (account_id,created_on,created_by,modified_on,modified_by,status,device_name,device_code,mac_address,serial_no,asset_id,properties,app_config_id,record_id) " + " values('" + (object) account_id + "',GETUTCDATE(),'" + (object) user_id + "',GETUTCDATE(),'" + (object) user_id + "','1','" + name + "','" + (object) device_code + "','" + mac + "','" + sno + "','0','<root></root>',0,'" + (object) Guid.NewGuid() + "');");
      }
      catch
      {
        device_code = Guid.Empty;
      }
      return device_code;
    }

    public List<booking_item> get_bookings(
      Guid account_id,
      long room_id,
      DateTime from,
      DateTime to)
    {
      List<booking_item> bookings = new List<booking_item>();
      if (this.db.get_dataset("select booking_id,purpose,book_from,book_to,status,dbo.sbt_fn_user_name(created_by,account_id) as created_by_name,dbo.sbt_fn_user_name(booked_for,account_id) as booked_for_name from sbt_asset_bookings where account_id='" + (object) account_id + "' and (status='1' or status='2' or status='4') and asset_id='" + (object) room_id + "' " + " and ((book_from between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') or (book_to between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') or ('" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' between book_from and book_to) or ('" + to.ToString("yyyy-MM-dd HH:mm:ss") + "' between book_from and book_to)) " + " order by book_from"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          bookings.Add(new booking_item()
          {
            booked_for = row["booked_for_name"].ToString(),
            end = Convert.ToDateTime(row["book_to"]),
            id = Convert.ToInt64(row["booking_id"]),
            purpose = row["purpose"].ToString(),
            requested_by = row["created_by_name"].ToString(),
            start = Convert.ToDateTime(row["book_from"]),
            is_blocked = row["status"].ToString() == "2" || row["status"].ToString() == "4"
          });
      }
      return bookings;
    }

    public response_message book(
      Guid dcode,
      long room_id,
      long booking_id,
      long requestor_id,
      DateTime from,
      DateTime to,
      string type,
      string purpose)
    {
      string sql;
      if (booking_id > 0L)
        sql = "update sbt_asset_bookings set book_to='" + to.ToString("yyyy-MM-dd HH:mm:00") + "',modified_by='" + (object) requestor_id + "', modified_on=CURRENT_TIMESTAMP where booking_id='" + (object) booking_id + "' and asset_id='" + (object) room_id + "'";
      else
        sql = "insert into sbt_asset_bookings (purpose,book_from,book_to,booked_for,created_by,created_on,modified_by,modified_on,status,asset_id) values('" + purpose + "','" + from.ToString("yyyy-MM-dd HH:mm:00") + "','" + to.ToString("yyyy-MM-dd HH:mm:00") + "','" + (object) requestor_id + "','" + (object) requestor_id + "',CURRENT_TIMESTAMP,'" + (object) requestor_id + "',CURRENT_TIMESTAMP,'1','" + (object) room_id + "');";
      this.db.execute_scalar(sql);
      booking_status bookingStatus = new booking_status();
      bookingStatus.status = "1";
      bookingStatus.message = "Booking successfully made.";
      response_message_book responseMessageBook = new response_message_book();
      responseMessageBook.error = new response_error();
      responseMessageBook.error.errcode = "0";
      responseMessageBook.error.errmsg = "";
      responseMessageBook.dcode = Guid.Empty;
      responseMessageBook.status = "1";
      responseMessageBook.data = bookingStatus;
      return (response_message) responseMessageBook;
    }

    public bool has_started(Guid account_id, long booking_id) => this.db.get_data_objects("select usage_id from sbt_asset_bookings_usage where account_id='" + (object) account_id + "' and booking_id='" + (object) booking_id + "'").Count > 0;

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

    private List<Guid> get_device_codes()
    {
      List<Guid> deviceCodes = new List<Guid>();
      if (this.db.get_dataset("select device_code from sbt_apps_api_devices"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          deviceCodes.Add(new Guid(row["device_code"].ToString()));
      }
      return deviceCodes;
    }

    private bool device_exists(Guid device_code) => this.device_codes_list.Contains(device_code);

    public device_details get_device(string dcode)
    {
      device_details device = new device_details();
      device.device_id = 0L;
      if (this.db.get_dataset("select * from sbt_apps_api_devices where device_code='" + dcode + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      {
        DataRow row = this.db.resultDataSet.Tables[0].Rows[0];
        device.account_id = new Guid(row["account_id"].ToString());
        device.app_config_id = Convert.ToInt64(row["app_config_id"]);
        device.asset_id = Convert.ToInt64(row["asset_id"]);
        device.created_by = Convert.ToInt64(row["created_by"]);
        device.created_on = Convert.ToDateTime(row["created_on"]);
        device.device_code = new Guid(row["device_code"].ToString());
        device.device_id = Convert.ToInt64(row["device_id"]);
        device.device_name = row["device_name"].ToString();
        device.mac_address = row["mac_address"].ToString();
        device.modified_by = Convert.ToInt64(row["modified_by"]);
        device.modified_on = Convert.ToDateTime(row["modified_on"]);
        device.serial_no = row["serial_no"].ToString();
        device.config = this.get_device_properties(device.device_id, device.account_id);
      }
      return device;
    }

    public Dictionary<string, string> get_device_properties(long device_id, Guid account_id)
    {
      Dictionary<string, string> deviceProperties = new Dictionary<string, string>();
      if (this.db.get_dataset("select parameter,value from sbt_apps_api_device_settings where account_id='" + (object) account_id + "' and device_id='" + (object) device_id + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          deviceProperties.Add(row["parameter"].ToString(), row["value"].ToString());
      }
      return deviceProperties;
    }

    public device_details get_device(Guid dcode)
    {
      device_details device = new device_details();
      device.device_id = 0L;
      if (this.db.get_dataset("select device_id,account_id,asset_id,app_config_id,device_name from sbt_apps_api_devices where device_code='" + (object) dcode + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      {
        DataRow row = this.db.resultDataSet.Tables[0].Rows[0];
        device.account_id = new Guid(row["account_id"].ToString());
        device.app_config_id = Convert.ToInt64(row["app_config_id"]);
        device.asset_id = Convert.ToInt64(row["asset_id"]);
        device.device_code = dcode;
        device.device_id = Convert.ToInt64(row["device_id"]);
        device.device_name = row["device_name"].ToString();
      }
      return device;
    }

    private Guid get_account_id(Guid device_code) => new Guid();

    public string get_room_name(long asset_id, Guid account_id) => (string) this.db.execute_scalar("select code + ' \\ ' + name from sbt_assets where asset_id='" + (object) asset_id + "' and account_id='" + (object) account_id + "'");

    public DataSet get_display_details(string fromdate, string todate, device_details dev)
    {
      try
      {
        return this.db.get_dataset("SELECT DISTINCT a.booking_id,a.purpose,a.book_from,a.book_to," + "(SELECT c.full_name FROM sbt_users c WHERE a.booked_for=c.user_id) AS Requestor,a.status " + " FROM sbt_asset_bookings a  WHERE a.account_id='" + (object) dev.account_id + "' and " + " a.asset_id='" + (object) dev.asset_id + "'" + " AND (a.status=1 OR a.status=2 OR a.status=4) AND  ((a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "') OR " + "(a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') OR ('" + fromdate + "' BETWEEN a.book_from AND a.book_to) OR ('" + todate + "' BETWEEN a.book_from AND a.book_to))" + "ORDER BY a.book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public DataSet get_next_meeting(string fromdate, string todate, device_details dev)
    {
      try
      {
        return this.db.get_dataset("SELECT TOP 1 a.booking_id,a.purpose,a.book_from,a.book_to," + "(SELECT c.full_name FROM sbt_users c WHERE a.booked_for=c.user_id) AS Requestor,a.status " + " FROM sbt_asset_bookings a  WHERE a.account_id='" + (object) dev.account_id + "' and " + " a.asset_id='" + (object) dev.asset_id + "'" + " AND (a.status=1 OR a.status=2 OR a.status=4) AND  ((a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "') OR " + "(a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') OR ('" + fromdate + "' BETWEEN a.book_from AND a.book_to) OR ('" + todate + "' BETWEEN a.book_from AND a.book_to))" + "ORDER BY a.book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public List<booking> get_bookings(DateTime from, DateTime to, device_details dev)
    {
      List<booking> bookings = new List<booking>();
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("SELECT DISTINCT a.booking_id,a.purpose,a.book_from,a.book_to," + "(SELECT c.full_name FROM sbt_users c WHERE a.booked_for=c.user_id) AS Requestor,a.status " + " FROM sbt_asset_bookings a  WHERE a.account_id='" + (object) dev.account_id + "' and " + " a.asset_id='" + (object) dev.asset_id + "'" + " AND (a.status=1 OR a.status=2 OR a.status=4) AND  ((a.book_from BETWEEN '" + from.ToString(device_api.sql_datetime_format) + "' AND '" + to.ToString(device_api.sql_datetime_format) + "') OR " + "(a.book_to BETWEEN '" + from.ToString(device_api.sql_datetime_format) + "' AND '" + to.ToString(device_api.sql_datetime_format) + "') OR ('" + from.ToString(device_api.sql_datetime_format) + "' BETWEEN a.book_from AND a.book_to) OR ('" + to.ToString(device_api.sql_datetime_format) + "' BETWEEN a.book_from AND a.book_to))" + "ORDER BY a.book_from");
      foreach (int key in dataObjects.Keys)
        bookings.Add(new booking()
        {
          booking_id = (long) dataObjects[key][0],
          purpose = (string) dataObjects[key][1],
          from = (DateTime) dataObjects[key][2],
          to = (DateTime) dataObjects[key][3],
          requested_by = (string) dataObjects[key][4],
          status = (short) dataObjects[key][5]
        });
      return bookings;
    }

    public DataSet get_booking_schedule(DateTime from, DateTime to, device_details dev)
    {
      try
      {
        return this.db.get_dataset("SELECT a.booking_id,a.book_from,a.book_to" + " FROM sbt_asset_bookings a  WHERE a.account_id='" + (object) dev.account_id + "' and a.asset_id='" + (object) dev.asset_id + "'" + " AND (a.status=1 OR a.status=2 OR a.status=4) AND  ((a.book_from BETWEEN '" + from.ToString(device_api.sql_datetime_format) + "' AND '" + to.ToString(device_api.sql_datetime_format) + "') OR " + "(a.book_to BETWEEN '" + from.ToString(device_api.sql_datetime_format) + "' AND '" + to.ToString(device_api.sql_datetime_format) + "') OR ('" + from.ToString(device_api.sql_datetime_format) + "' BETWEEN a.book_from AND a.book_to) OR ('" + to.ToString(device_api.sql_datetime_format) + "' BETWEEN a.book_from AND a.book_to)) order by a.book_from") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public DataSet get_booking(long booking_id, device_details dev)
    {
      try
      {
        return this.db.get_dataset("SELECT DISTINCT a.booking_id,a.purpose,a.book_from,a.book_to," + " substring(CONVERT(VARCHAR, a.book_from, 108),0,6) AS book_from_time," + " substring(CONVERT(VARCHAR, a.book_to, 108),0,6) AS book_to_time," + "(SELECT c.full_name FROM sbt_users c WHERE a.booked_for=c.user_id) AS Requestor,a.status " + " FROM sbt_asset_bookings a  WHERE a.account_id='" + (object) dev.account_id + "' and a.booking_id='" + (object) booking_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public booking get_booking_object(long booking_id, device_details dev)
    {
      booking bookingObject = new booking();
      bookingObject.booking_id = 0L;
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects("SELECT DISTINCT a.booking_id,a.purpose,a.book_from,a.book_to," + " substring(CONVERT(VARCHAR, a.book_from, 108),0,6) AS book_from_time," + " substring(CONVERT(VARCHAR, a.book_to, 108),0,6) AS book_to_time," + "(SELECT c.full_name FROM sbt_users c WHERE a.booked_for=c.user_id) AS Requestor,a.status " + " FROM sbt_asset_bookings a  WHERE a.account_id='" + (object) dev.account_id + "' and a.booking_id='" + (object) booking_id + "'");
        if (dataObjects.Count > 0)
        {
          int key = 0;
          bookingObject.booking_id = (long) dataObjects[key][0];
          bookingObject.purpose = (string) dataObjects[key][1];
          bookingObject.from = (DateTime) dataObjects[key][2];
          bookingObject.to = (DateTime) dataObjects[key][3];
          bookingObject.requested_by = (string) dataObjects[key][6];
          bookingObject.status = (short) dataObjects[key][7];
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return bookingObject;
    }

    public bool user_can_book(Guid account_id, long asset_id, long user_id) => this.db.get_dataset("SELECT asset_permission_id FROM vw_sbt_asset_user_permissions where account_id='" + (object) account_id + "' and asset_id='" + (object) asset_id + "' and user_id='" + (object) user_id + "' and is_book='1'") && this.db.resultDataSet.Tables[0].Rows.Count > 0;
  }
}
