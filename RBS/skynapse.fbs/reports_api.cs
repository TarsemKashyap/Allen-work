// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.reports_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Data;

namespace skynapse.fbs
{
  public class reports_api : api_base
  {
    public DataSet get_allowed_facilities(Guid account_id, long user_id) => this.get_data("select setting_id,parameter,value from sbt_settings where account_id='" + (object) account_id + "'");

    public DataSet get_settings(Guid account_id) => this.get_data("select setting_id,parameter,value from sbt_settings where account_id='" + (object) account_id + "'");

    public DataSet get_users(Guid account_id) => this.get_data("select user_id,full_name from sbt_users where account_id='" + (object) account_id + "' order by full_name");

    public DataSet get_assets(Guid account_id) => this.get_data("select asset_id,code,name,building_id,level_id,category_id,asset_type from sbt_assets where account_id='" + (object) account_id + "' order by name");

    public DataSet get_assets(
      Guid account_id,
      long building_id,
      long level_id,
      long category_id,
      long asset_type)
    {
      string str = "select asset_id,code,name,building_id,level_id,category_id,asset_type from sbt_assets where account_id='" + (object) account_id + "'";
      if (building_id > 0L)
        str = str + " and building_id='" + (object) building_id + "'";
      if (level_id > 0L)
        str = str + " and level_id='" + (object) level_id + "'";
      if (category_id > 0L)
        str = str + " and category_id='" + (object) category_id + "'";
      if (asset_type > 0L)
        str = str + " and asset_type='" + (object) asset_type + "'";
      return this.get_data(str + " order by name");
    }

    public DataSet get_bookings(Guid account_id, DateTime from, DateTime to, string facilities) => this.get_data("select booking_id,asset_id,book_from,book_to,duration,created_by,booked_for from sbt_asset_bookings where account_id='" + (object) account_id + "' and status='1' and book_from between '" + from.ToString("yyyy-MM-dd hh:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd hh:mm:ss") + "' order by book_from desc");

    public DataSet get_cancellations(Guid account_id, DateTime from, DateTime to) => this.get_data("select booking_id,asset_id,book_from,book_to,duration,created_by,booked_for,cancel_on from sbt_asset_bookings where account_id='" + (object) account_id + "' and status='0' and book_from between '" + from.ToString("yyyy-MM-dd hh:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd hh:mm:ss") + "' order by book_from desc");

    public DataSet get_no_show(Guid account_id, DateTime from, DateTime to) => this.get_data("select booking_id,asset_id,book_from,book_to,duration,created_by,booked_for from sbt_asset_bookings where account_id='" + (object) account_id + "' and status='3' and book_from between '" + from.ToString("yyyy-MM-dd hh:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd hh:mm:ss") + "' order by book_from desc");

    private DataSet get_data(string sql)
    {
      try
      {
        return this.db.get_dataset(sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("get_data error -> . SQL:|" + sql + "|"), ex);
      }
      return (DataSet) null;
    }

    public DataSet get_bookings_by_facility(
      Guid account_id,
      DateTime from,
      DateTime to,
      string fac)
    {
      string str = "select asset_id,book_from,book_to from sbt_asset_bookings where status='1' and account_id='" + (object) account_id + "'" + " and ((book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or (book_to between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') " + " or ('" + from.ToString(api_constants.sql_datetime_format) + "' between book_from and book_to) or ('" + to.ToString(api_constants.sql_datetime_format) + "' between book_from and book_to))";
      if (fac != "")
        str = str + " and asset_id in(" + fac + ")";
      return this.get_data(str + " order by book_from");
    }

    public DataSet get_bookings_by_facility_summary(
      Guid account_id,
      DateTime from,
      DateTime to,
      string fac)
    {
      string str = "select asset_id, max(book_duration) as max_duration,min(book_duration) as min_duration, avg(book_duration) as avg_duration, sum(book_duration) as total_duration,count(book_duration) as count_bookings from sbt_asset_bookings where status='1' and account_id='" + (object) account_id + "' and book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' ";
      if (fac != "")
        str = str + " and asset_id in(" + fac + ") ";
      return this.get_data(str + " group by asset_id  order by sum(book_duration) desc");
    }

    public DataSet get_cancels_by_facility(
      Guid account_id,
      DateTime from,
      DateTime to,
      string fac)
    {
      string str = "select asset_id,sum(book_duration) as total_minutes,count(asset_id) as counter,CONVERT(date, book_from) as book_date from sbt_asset_bookings where status='0' and account_id='" + (object) account_id + "' and book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' ";
      if (fac != "")
        str = str + " and asset_id in(" + fac + ") ";
      return this.get_data(str + " group by asset_id,CONVERT(date, book_from) order by CONVERT(date, book_from) asc");
    }

    public DataSet get_cancels_by_facility_summary(
      Guid account_id,
      DateTime from,
      DateTime to,
      string fac)
    {
      string str = "select asset_id, max(book_duration) as max_duration,min(book_duration) as min_duration, avg(book_duration) as avg_duration, sum(book_duration) as total_duration,count(book_duration) as count_bookings from sbt_asset_bookings where status='0' and account_id='" + (object) account_id + "' and book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' ";
      if (fac != "")
        str = str + " and asset_id in(" + fac + ") ";
      return this.get_data(str + " group by asset_id  order by sum(book_duration) desc");
    }

    public DateTime start_of_week(DateTime dt, DayOfWeek startOfWeek)
    {
      int num = dt.DayOfWeek - startOfWeek;
      if (num < 0)
        num += 7;
      return dt.AddDays((double) (-1 * num)).Date;
    }
  }
}
