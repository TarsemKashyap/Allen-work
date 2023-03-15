// Decompiled with JetBrains decompiler
// Type: outlook_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

public class outlook_api
{
  private DataAccess db;
  private string connection_string = ConfigurationManager.ConnectionStrings["sql"].ConnectionString;
  public string display_datetime_format = ConfigurationManager.AppSettings["date_time_long"];
  public string sql_datetime_format = ConfigurationManager.AppSettings["sql_date_time"];
  public string sql_short_date_format = ConfigurationManager.AppSettings["date_time_short"];
  private bool enable_debug = true;
  private settings_api settings = new settings_api();
  private asset_api assets = new asset_api();
  private booking_bl bookingsbl = new booking_bl();

  public outlook_api() => this.db = new DataAccess(this.connection_string, this.enable_debug);

  public outlook_booking outlook_booking_update(outlook_booking obj)
  {
    try
    {
      obj.outlook_id = !this.db.execute_procedure("sbt_sp_outlook_update", new Dictionary<string, object>()
      {
        {
          "@outlook_id",
          (object) obj.outlook_id
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
          "@booking_id",
          (object) obj.booking_id
        },
        {
          "@global_appointment_id",
          (object) obj.global_appointment_id
        }
      }) ? 0L : Convert.ToInt64(this.db.resultString);
    }
    catch (Exception ex)
    {
    }
    return obj;
  }

  public void populate_dropdown(DataSet data, DropDownList ddl, string parameter)
  {
    ddl.Items.Clear();
    try
    {
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='" + parameter + "'"))
      {
        ListItem listItem = new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString());
        ddl.Items.Add(listItem);
      }
      ddl.Items.Insert(0, new ListItem("All", ""));
    }
    catch (Exception ex)
    {
    }
  }

  private string decimal_to_binary(string dayOfWeekMask)
  {
    string binary = "";
    int num;
    for (int index = Convert.ToInt32(dayOfWeekMask); index >= 1; index = num)
    {
      num = index / 2;
      binary += (index % 2).ToString();
    }
    return binary;
  }

  public List<int> getDaysColl(string dayOfWeekMask)
  {
    List<int> daysColl = new List<int>();
    string str1 = "";
    string str2 = "";
    try
    {
      str1 = this.decimal_to_binary(dayOfWeekMask);
    }
    catch (Exception ex)
    {
      str2 = dayOfWeekMask.Substring(2);
    }
    if (str2 != "")
    {
      switch (str2)
      {
        case "Sunday":
          daysColl.Add(0);
          break;
        case "Monday":
          daysColl.Add(1);
          break;
        case "Tuesday":
          daysColl.Add(2);
          break;
        case "Wednesday":
          daysColl.Add(3);
          break;
        case "Thursday":
          daysColl.Add(4);
          break;
        case "Friday":
          daysColl.Add(5);
          break;
        case "Saturday":
          daysColl.Add(6);
          break;
      }
    }
    else
    {
      try
      {
        if (str1.Substring(0, 1) == "1")
          daysColl.Add(0);
        if (str1.Substring(1, 1) == "1")
          daysColl.Add(1);
        if (str1.Substring(2, 1) == "1")
          daysColl.Add(2);
        if (str1.Substring(3, 1) == "1")
          daysColl.Add(3);
        if (str1.Substring(4, 1) == "1")
          daysColl.Add(4);
        if (str1.Substring(5, 1) == "1")
          daysColl.Add(5);
        if (str1.Substring(6, 1) == "1")
          daysColl.Add(6);
      }
      catch (Exception ex)
      {
      }
    }
    return daysColl;
  }

  public DataTable initialize_table()
  {
    DataTable dataTable = new DataTable();
    dataTable.Columns.Add(new DataColumn("booking_id", Type.GetType("System.Int64")));
    dataTable.Columns.Add(new DataColumn("outlook_id", Type.GetType("System.Int64")));
    dataTable.Columns.Add(new DataColumn("outlook_guid", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("purpose", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("booked_for", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("booked_for_name", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("email", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("invites_email", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("invites_name", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("book_from", Type.GetType("System.DateTime")));
    dataTable.Columns.Add(new DataColumn("book_to", Type.GetType("System.DateTime")));
    dataTable.Columns.Add(new DataColumn("asset_id", Type.GetType("System.Int64")));
    dataTable.Columns.Add(new DataColumn("asset_options", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("remarks", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("is_holiday", Type.GetType("System.Boolean")));
    dataTable.Columns.Add(new DataColumn("holiday_name", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("is_weekend", Type.GetType("System.Boolean")));
    dataTable.Columns.Add(new DataColumn("telephone", Type.GetType("System.String")));
    dataTable.Columns.Add(new DataColumn("booking_status", Type.GetType("System.Int64")));
    dataTable.AcceptChanges();
    return dataTable;
  }

  public string populate_asset(DataTable data, Guid accId)
  {
    try
    {
      List<long> rooms = new List<long>();
      foreach (DataRow row in (InternalDataCollectionBase) data.Rows)
      {
        if (!rooms.Contains(Convert.ToInt64(row["asset_id"])))
          rooms.Add(Convert.ToInt64(row["asset_id"]));
      }
      this.settings.view_settings(accId);
      DataSet assets = this.assets.get_assets(rooms, accId, 0L, 0L, 0L, 0, "");
      DataSet dataSet = this.assets.view_asset_properties(accId, new string[1]
      {
        "asset_property"
      });
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append("<table class='table table-striped table-bordered table-hover' id='list_table' width='100%'>");
      stringBuilder1.Append("<thead>");
      stringBuilder1.Append("<tr>");
      stringBuilder1.Append("<th class='hidden-480'>#</th>");
      stringBuilder1.Append("<th class='hidden-480'>From</th>");
      stringBuilder1.Append("<th class='hidden-480'>To</th>");
      stringBuilder1.Append("<th class='hidden-480'>Remarks</th>");
      stringBuilder1.Append("<th class='hidden-480'>Room Name</th>");
      stringBuilder1.Append("</tr>");
      stringBuilder1.Append("</thead>");
      stringBuilder1.Append("<tbody>");
      util util = new util();
      int num = 1;
      foreach (DataRow row in (InternalDataCollectionBase) data.Rows)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add(Convert.ToDateTime(row["book_from"]).ToString(api_constants.sql_datetime_format), Convert.ToDateTime(row["book_to"]).ToString(api_constants.sql_datetime_format));
        StringBuilder stringBuilder2 = new StringBuilder();
        try
        {
          DataRow[] dataRowArray1 = assets.Tables[0].Select("asset_id=" + row["asset_id"].ToString());
          DataRow[] dataRowArray2 = dataSet.Tables[0].Select("asset_id=" + row["asset_id"].ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
          foreach (string key in dictionary.Keys)
          {
            foreach (DataRow dataRow in dataRowArray1)
            {
              stringBuilder2.Append("<tr class='odd gradeX'>");
              stringBuilder2.Append("<td>" + (object) num + "</td>");
              stringBuilder2.Append("<td>" + Convert.ToDateTime(row["book_from"]).ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder2.Append("<td>" + Convert.ToDateTime(row["book_to"]).ToString(api_constants.display_datetime_format) + "</td>");
              switch (Convert.ToInt32(row["booking_status"]))
              {
                case 0:
                  stringBuilder2.Append("<td><Span class='label label-NotAvailable'>Cancelled</span> </td>");
                  break;
                case 1:
                  stringBuilder2.Append("<td><Span class='label label-Available'>Booked</span> </td>");
                  break;
                case 2:
                  stringBuilder2.Append("<td><Span class='label label-NotAvailable'>Blocked</span> </td>");
                  break;
                case 3:
                  stringBuilder2.Append("<td><Span class='label label-NotAvailable'>No Show</span> </td>");
                  break;
                case 4:
                  stringBuilder2.Append("<td><Span class='label label-NotAvailable'>Pending</span> </td>");
                  break;
                case 6:
                  stringBuilder2.Append("<td><Span class='label label-NotAvailable'>Rejected</span> </td>");
                  break;
              }
              if (dataRowArray2.Length > 0)
              {
                if (string.IsNullOrEmpty(dataRow["code"].ToString()))
                  stringBuilder2.Append("<td>" + dataRow["name"].ToString() + "</td>");
                else
                  stringBuilder2.Append("<td>" + dataRow["code"].ToString() + " / " + dataRow["name"].ToString() + "</td>");
              }
              else if (string.IsNullOrEmpty(dataRow["code"].ToString()))
                stringBuilder2.Append("<td>" + dataRow["name"].ToString() + "</td>");
              else
                stringBuilder2.Append("<td>" + dataRow["code"].ToString() + " / " + dataRow["name"].ToString() + "</td>");
              stringBuilder2.Append("</div></div></td>");
              stringBuilder2.Append("</tr>");
            }
            ++num;
          }
        }
        catch (Exception ex)
        {
        }
        stringBuilder1.Append(stringBuilder2.ToString());
      }
      stringBuilder1.Append("</tbody>");
      stringBuilder1.Append("</table>");
      return stringBuilder1.ToString();
    }
    catch (Exception ex)
    {
    }
    return "";
  }

  public DataSet get_assets(Guid account_id, string ids)
  {
    try
    {
      return this.db.get_dataset("select name from sbt_assets where account_id='" + (object) account_id + "' and asset_id in (" + ids + ")") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public bool check_global_appointment_id_exists(
    string globalAppointmentID,
    Guid account_id,
    long booking_id)
  {
    try
    {
      string Sql = "select * from sbt_outlook where global_appointment_id='" + globalAppointmentID + "' and account_id='" + (object) account_id + "'";
      if (booking_id > 0L)
        Sql = Sql + " and booking_id='" + (object) booking_id + "'";
      if (this.db.get_dataset(Sql))
      {
        if (this.db.resultDataSet.Tables.Count > 0)
        {
          if (this.db.resultDataSet.Tables[0].Rows.Count > 0)
            return true;
        }
      }
    }
    catch (Exception ex)
    {
    }
    return false;
  }

  public long get_owner_by_appointment_id(string gid, Guid account_id) => this.db.get_dataset("select a.created_by from sbt_asset_bookings a where a.booking_id in (select booking_id from sbt_outlook where account_id='" + (object) account_id + "' and global_appointment_id='" + gid + "')") && this.db.resultDataSet.Tables.Count > 0 && this.db.resultDataSet.Tables[0].Rows.Count > 0 ? Convert.ToInt64(this.db.resultDataSet.Tables[0].Rows[0][0]) : 0L;

  public DataSet get_bookings(string startDate, string endDate, string globalAppointmentID)
  {
    try
    {
      return this.db.get_dataset("select * from vw_bookings where global_appointment_id='" + globalAppointmentID + "' and  " + "  ( (book_from between '" + startDate + "' and '" + endDate + "') or " + "  (book_to between '" + startDate + "' and '" + endDate + "') or " + "  ('" + startDate + "' between book_from  and book_to ) or " + "  ( '" + endDate + "' between book_from  and book_to ) ) ") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_bookings_asset(string startDate, string endDate, long asset_id)
  {
    try
    {
      return this.db.get_dataset("select * from vw_bookings where asset_id='" + (object) asset_id + "' and  " + "  ( (book_from between '" + startDate + "' and '" + endDate + "') or " + "  (book_to between '" + startDate + "' and '" + endDate + "') or " + "  ('" + startDate + "' between book_from  and book_to ) or " + "  ( '" + endDate + "' between book_from  and book_to ) ) ") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_bookings_date_only(
    string startDate,
    string endDate,
    string globalAppointmentID)
  {
    try
    {
      return this.db.get_dataset("select * from vw_bookings where global_appointment_id='" + globalAppointmentID + "' and  " + "  ( CONVERT(datetime, CONVERT(varchar, book_from, 101))='" + startDate + "' " + "  and CONVERT(datetime, CONVERT(varchar, book_to, 101))='" + endDate + "') ") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_bookings(string globalAppointmentID)
  {
    try
    {
      return this.db.get_dataset("select * from vw_bookings where global_appointment_id='" + globalAppointmentID + "'  ") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public bool delete_invites(long bookingId, string email, Guid acc_id) => this.db.get_nonquery("delete from sbt_asset_booking_invites where account_id = '" + (object) acc_id + "'  and email='" + email.Trim() + "' and booking_id=" + (object) bookingId);

  public Dictionary<string, string> do_filter(
    Dictionary<string, string> selecteDates,
    int holiday_option,
    int weekend_option)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string key in selecteDates.Keys)
    {
      DateTime dateTime1 = Convert.ToDateTime(key);
      DateTime dateTime2 = Convert.ToDateTime(selecteDates[key]);
      if (dateTime1.DayOfWeek == DayOfWeek.Sunday || dateTime1.DayOfWeek == DayOfWeek.Saturday)
      {
        switch (weekend_option)
        {
          case 0:
            if (!dictionary.ContainsKey(key))
            {
              dictionary.Add(key, selecteDates[key]);
              continue;
            }
            continue;
          case 1:
            if (dateTime1.DayOfWeek == DayOfWeek.Saturday)
            {
              dateTime1 = dateTime1.AddDays(2.0);
              dateTime2 = dateTime2.AddDays(2.0);
              if (!dictionary.ContainsKey(dateTime1.ToString(api_constants.sql_datetime_format)))
                dictionary.Add(dateTime1.ToString(api_constants.sql_datetime_format), dateTime2.ToString(api_constants.sql_datetime_format));
            }
            if (dateTime1.DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime1 = dateTime1.AddDays(1.0);
              DateTime dateTime3 = dateTime2.AddDays(1.0);
              if (!dictionary.ContainsKey(dateTime1.ToString(api_constants.sql_datetime_format)))
              {
                dictionary.Add(dateTime1.ToString(api_constants.sql_datetime_format), dateTime3.ToString(api_constants.sql_datetime_format));
                continue;
              }
              continue;
            }
            continue;
          case 2:
            if (dateTime1.DayOfWeek == DayOfWeek.Saturday)
            {
              dateTime1 = dateTime1.AddDays(-1.0);
              dateTime2 = dateTime2.AddDays(-1.0);
              if (!dictionary.ContainsKey(dateTime1.ToString(api_constants.sql_datetime_format)))
                dictionary.Add(dateTime1.ToString(api_constants.sql_datetime_format), dateTime2.ToString(api_constants.sql_datetime_format));
            }
            if (dateTime1.DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime1 = dateTime1.AddDays(-2.0);
              DateTime dateTime4 = dateTime2.AddDays(-2.0);
              if (!dictionary.ContainsKey(dateTime1.ToString(api_constants.sql_datetime_format)))
              {
                dictionary.Add(dateTime1.ToString(api_constants.sql_datetime_format), dateTime4.ToString(api_constants.sql_datetime_format));
                continue;
              }
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      else if (!dictionary.ContainsKey(key))
        dictionary.Add(key, selecteDates[key]);
    }
    return dictionary;
  }

  public DataSet get_device(Guid account_id, string pc_name)
  {
    try
    {
      return this.db.get_dataset("select * from sbt_outlook_devices where account_id='" + (object) account_id + "' and pc_name='" + pc_name + "'") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_device(Guid account_id, long outlook_device_id)
  {
    try
    {
      return this.db.get_dataset("select * from sbt_outlook_devices where account_id='" + (object) account_id + "' and outlook_device_id='" + (object) outlook_device_id + "'") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_devices(Guid account_id, long user_id)
  {
    try
    {
      return this.db.get_dataset("select * from sbt_outlook_devices where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "'") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_devices(Guid account_id)
  {
    try
    {
      return this.db.get_dataset("select a.*,b.full_name,b.email from sbt_outlook_devices a, sbt_users b where a.user_id=b.user_id and b.status=1 and a.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public DataSet get_users_without_outlook(Guid account_id)
  {
    try
    {
      return this.db.get_dataset("select user_id,full_name,email from sbt_users where account_id='" + (object) account_id + "' and status=1 and user_id not in (select user_id from sbt_outlook_devices where account_id='" + (object) account_id + "') order by full_name") ? this.db.resultDataSet : (DataSet) null;
    }
    catch (Exception ex)
    {
    }
    return (DataSet) null;
  }

  public long get_outlook_user(string email)
  {
    long outlookUser = 0;
    if (this.db.get_dataset("select user_id from sbt_users where email='" + email + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      outlookUser = Convert.ToInt64(this.db.resultDataSet.Tables[0].Rows[0]["user_id"]);
    return outlookUser;
  }

  public long get_outlook_device_id(long user_id, string pc_name)
  {
    long outlookDeviceId = 0;
    if (this.db.get_dataset("select outlook_device_id from sbt_outlook_devices where pc_name='" + pc_name + "' and user_id='" + (object) user_id + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      outlookDeviceId = Convert.ToInt64(this.db.resultDataSet.Tables[0].Rows[0]["outlook_device_id"]);
    return outlookDeviceId;
  }

  public outlook_device update_device(outlook_device obj)
  {
    try
    {
      obj.outlook_device_id = !this.db.execute_procedure("sbt_sp_outlook_devices_update", new Dictionary<string, object>()
      {
        {
          "@outlook_device_id",
          (object) obj.outlook_device_id
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
          "@pc_name",
          (object) obj.pc_name
        },
        {
          "@status",
          (object) obj.status
        },
        {
          "@properties",
          (object) obj.properties
        },
        {
          "@created_on",
          (object) obj.created_on
        },
        {
          "@last_accessed_on",
          (object) obj.last_accessed_on
        }
      }) ? 0L : Convert.ToInt64(this.db.resultString);
    }
    catch
    {
    }
    return obj;
  }
}
