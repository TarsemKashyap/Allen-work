// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.dashboard_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class dashboard_api : api_base
  {
    private Dictionary<int, int> range;
    public Dictionary<int, string> range_text;
    private Dictionary<int, int> cancel_range;
    public Dictionary<int, string> cancel_range_text;

    public dashboard_api()
    {
      this.range = new Dictionary<int, int>();
      this.range.Add(0, 15);
      this.range.Add(15, 60);
      this.range.Add(60, 240);
      this.range.Add(240, 1440);
      this.range.Add(1440, 10080);
      this.range.Add(10080, 20160);
      this.range.Add(20160, 43200);
      this.range.Add(43200, 100000);
      this.range_text = new Dictionary<int, string>();
      this.range_text.Add(0, "Within 15 minutes");
      this.range_text.Add(1, "From 15 mins. to 1 hour");
      this.range_text.Add(2, "From 1 hour to 4 hours");
      this.range_text.Add(3, "From 4 hours to 1 day");
      this.range_text.Add(4, "FRom 1 day to 2 days");
      this.range_text.Add(5, "From 2 days to 1 week");
      this.range_text.Add(6, "From 1 week to 1 month");
      this.range_text.Add(7, "More than a month");
      this.cancel_range = new Dictionary<int, int>();
      this.cancel_range.Add(-100000, 0);
      this.cancel_range.Add(0, 15);
      this.cancel_range.Add(15, 60);
      this.cancel_range.Add(60, 240);
      this.cancel_range.Add(240, 1440);
      this.cancel_range.Add(1440, 10080);
      this.cancel_range.Add(10080, 20160);
      this.cancel_range.Add(20160, 43200);
      this.cancel_range.Add(43200, 100000);
      this.cancel_range_text = new Dictionary<int, string>();
      this.cancel_range_text.Add(0, "After meeting started");
      this.cancel_range_text.Add(1, "Within 15 minutes");
      this.cancel_range_text.Add(2, "From 15 mins. to 1 hour");
      this.cancel_range_text.Add(3, "From 1 hour to 4 hours");
      this.cancel_range_text.Add(4, "From 4 hours to 1 day");
      this.cancel_range_text.Add(5, "FRom 1 day to 2 days");
      this.cancel_range_text.Add(6, "From 2 days to 1 week");
      this.cancel_range_text.Add(7, "From 1 week to 1 month");
      this.cancel_range_text.Add(8, "More than a month");
    }

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

    public DataSet get_stats(DateTime from, DateTime to, Guid account_id) => this.get_data("select count(a.booking_id) as counter,AVG(DATEDIFF(minute,a.book_from,a.book_to)) as average,MIN(DATEDIFF(minute,a.book_from,a.book_to)) as min,MAX(DATEDIFF(minute,a.book_from,a.book_to)) as max from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'");

    public DataSet get_stats(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data("select count(a.booking_id) as counter,AVG(DATEDIFF(minute,a.book_from,a.book_to)) as average,MIN(DATEDIFF(minute,a.book_from,a.book_to)) as min,MAX(DATEDIFF(minute,a.book_from,a.book_to)) as max from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' and booked_for = " + (object) user_id);

    public DataSet get_bookings_by_asset(DateTime from, DateTime to, Guid account_id) => this.get_data("select (select name from sbt_assets where asset_id=a.asset_id and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value,sum(book_duration) as duration from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' group by a.asset_id order by value desc");

    public DataSet get_bookings_by_asset(
      DateTime from,
      DateTime to,
      Guid account_id,
      long user_id)
    {
      return this.get_data("select (select name from sbt_assets where asset_id=a.asset_id and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value,sum(book_duration) as duration from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' and booked_for = " + (object) user_id + " group by a.asset_id order by value desc");
    }

    public DataSet get_bookings_by_user(DateTime from, DateTime to, Guid account_id) => this.get_data("select (select full_name from sbt_users where user_id=a.booked_for and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' group by a.booked_for order by value desc");

    public DataSet get_bookings_by_user(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data("select (select full_name from sbt_users where user_id=a.booked_for and account_id = '" + (object) account_id + "') as name,count(a.booking_id) as value from sbt_asset_bookings a where a.account_id = '" + (object) account_id + "' and " + " a.status = 1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' and booked_for in (select distinct user_id from sbt_user_group_mappings where group_id in(select b.group_id  from sbt_user_group_mappings as b left join " + " sbt_user_groups as g on b.group_id = g.group_id  where user_id = " + (object) user_id + " and group_name<> 'All Users')) group by a.booked_for order by value desc");

    public DataSet get_cancels_by_asset(DateTime from, DateTime to, Guid account_id) => this.get_data("select (select name from sbt_assets where asset_id=a.asset_id and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=0 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' group by a.asset_id order by value desc");

    public DataSet get_cancels_by_asset(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data("select (select name from sbt_assets where asset_id=a.asset_id and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=0 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' and booked_for = " + (object) user_id + " group by a.asset_id order by value desc");

    public DataSet get_cancels_by_user(DateTime from, DateTime to, Guid account_id) => this.get_data("select (select full_name from sbt_users where user_id=a.booked_for and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=0 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' group by a.booked_for order by value desc");

    public DataSet get_cancels_by_user(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data("select (select full_name from sbt_users where user_id=a.booked_for and account_id='" + (object) account_id + "') as name,count(a.booking_id) as value from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and a.status=0 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' and booked_for in (select distinct user_id from sbt_user_group_mappings where group_id in(select b.group_id  from sbt_user_group_mappings as b left join " + " sbt_user_groups as g on b.group_id = g.group_id  where user_id = " + (object) user_id + " and group_name<> 'All Users')) group by a.booked_for order by value desc");

    public DataSet get_bookings_by_date(DateTime from, DateTime to, Guid account_id) => this.get_data("select count(booking_id) as count,sum(book_duration) as dur,CONVERT(date, book_from) as dt from sbt_asset_bookings where account_id='" + (object) account_id + "' and status=1 and book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' group by CONVERT(date, book_from) order by dt");

    public DataSet get_bookings_by_date(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data("select count(booking_id) as count,sum(book_duration) as dur,CONVERT(date, book_from) as dt from sbt_asset_bookings where account_id='" + (object) account_id + "' and status=1 and book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' and booked_for = " + (object) user_id + " group by CONVERT(date, book_from) order by dt");

    public DataTable get_booking_lead_times(DateTime from, DateTime to, Guid account_id)
    {
      DataSet data = this.get_data("select DATEDIFF(minute,dateadd(hour,8,a.created_on),a.book_from) as diff from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and  a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'  order by diff desc");
      DataTable bookingLeadTimes = new DataTable();
      bookingLeadTimes.Columns.Add(new DataColumn("min"));
      bookingLeadTimes.Columns.Add(new DataColumn("max"));
      bookingLeadTimes.Columns.Add(new DataColumn("count"));
      bookingLeadTimes.AcceptChanges();
      foreach (int key in this.range.Keys)
      {
        DataRow row = bookingLeadTimes.NewRow();
        row["min"] = (object) key;
        row["max"] = (object) this.range[key];
        DataRow[] dataRowArray = data.Tables[0].Select("diff > '" + (object) key + "' and diff<='" + (object) this.range[key] + "'");
        row["count"] = dataRowArray.Length <= 0 ? (object) "0" : (object) dataRowArray.Length;
        bookingLeadTimes.Rows.Add(row);
        bookingLeadTimes.AcceptChanges();
      }
      return bookingLeadTimes;
    }

    public DataTable get_booking_lead_times(
      DateTime from,
      DateTime to,
      Guid account_id,
      long user_id)
    {
      DataSet data = this.get_data("select DATEDIFF(minute,dateadd(hour,8,a.created_on),a.book_from) as diff from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and  a.status=1 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'  and booked_for = " + (object) user_id + "  order by diff desc");
      DataTable bookingLeadTimes = new DataTable();
      bookingLeadTimes.Columns.Add(new DataColumn("min"));
      bookingLeadTimes.Columns.Add(new DataColumn("max"));
      bookingLeadTimes.Columns.Add(new DataColumn("count"));
      bookingLeadTimes.AcceptChanges();
      foreach (int key in this.range.Keys)
      {
        DataRow row = bookingLeadTimes.NewRow();
        row["min"] = (object) key;
        row["max"] = (object) this.range[key];
        DataRow[] dataRowArray = data.Tables[0].Select("diff > '" + (object) key + "' and diff<='" + (object) this.range[key] + "'");
        row["count"] = dataRowArray.Length <= 0 ? (object) "0" : (object) dataRowArray.Length;
        bookingLeadTimes.Rows.Add(row);
        bookingLeadTimes.AcceptChanges();
      }
      return bookingLeadTimes;
    }

    public DataTable get_cancel_lead_times(DateTime from, DateTime to, Guid account_id)
    {
      DataSet data = this.get_data("select DATEDIFF(minute,dateadd(hour,8,a.cancel_on),a.book_from) as diff from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and  a.status=0 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'  order by diff desc");
      DataTable cancelLeadTimes = new DataTable();
      cancelLeadTimes.Columns.Add(new DataColumn("min"));
      cancelLeadTimes.Columns.Add(new DataColumn("max"));
      cancelLeadTimes.Columns.Add(new DataColumn("count"));
      cancelLeadTimes.AcceptChanges();
      foreach (int key in this.cancel_range.Keys)
      {
        DataRow row = cancelLeadTimes.NewRow();
        row["min"] = (object) key;
        row["max"] = (object) this.cancel_range[key];
        DataRow[] dataRowArray = data.Tables[0].Select("diff > '" + (object) key + "' and diff<='" + (object) this.cancel_range[key] + "'");
        row["count"] = dataRowArray.Length <= 0 ? (object) "0" : (object) dataRowArray.Length;
        cancelLeadTimes.Rows.Add(row);
        cancelLeadTimes.AcceptChanges();
      }
      return cancelLeadTimes;
    }

    public DataTable get_cancel_lead_times(
      DateTime from,
      DateTime to,
      Guid account_id,
      long user_id)
    {
      DataSet data = this.get_data("select DATEDIFF(minute,dateadd(hour,8,a.cancel_on),a.book_from) as diff from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and  a.status=0 and a.book_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'  and booked_for = " + (object) user_id + "  order by diff desc");
      DataTable cancelLeadTimes = new DataTable();
      cancelLeadTimes.Columns.Add(new DataColumn("min"));
      cancelLeadTimes.Columns.Add(new DataColumn("max"));
      cancelLeadTimes.Columns.Add(new DataColumn("count"));
      cancelLeadTimes.AcceptChanges();
      foreach (int key in this.cancel_range.Keys)
      {
        DataRow row = cancelLeadTimes.NewRow();
        row["min"] = (object) key;
        row["max"] = (object) this.cancel_range[key];
        DataRow[] dataRowArray = data.Tables[0].Select("diff > '" + (object) key + "' and diff<='" + (object) this.cancel_range[key] + "'");
        row["count"] = dataRowArray.Length <= 0 ? (object) "0" : (object) dataRowArray.Length;
        cancelLeadTimes.Rows.Add(row);
        cancelLeadTimes.AcceptChanges();
      }
      return cancelLeadTimes;
    }

    public DataSet get_booking_hour(DateTime from, DateTime to, Guid account_id) => this.get_data("select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=0 and DATEPART(HOUR,a.book_to)<=1);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=1 and DATEPART(HOUR,a.book_to)<=2);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=2 and DATEPART(HOUR,a.book_to)<=3);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=3 and DATEPART(HOUR,a.book_to)<=4);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=4 and DATEPART(HOUR,a.book_to)<=5);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=5 and DATEPART(HOUR,a.book_to)<=6);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=6 and DATEPART(HOUR,a.book_to)<=7);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=7 and DATEPART(HOUR,a.book_to)<=8);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=8 and DATEPART(HOUR,a.book_to)<=9);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=9 and DATEPART(HOUR,a.book_to)<=10);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=10 and DATEPART(HOUR,a.book_to)<=11);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=11 and DATEPART(HOUR,a.book_to)<=12);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=12 and DATEPART(HOUR,a.book_to)<=13);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=13 and DATEPART(HOUR,a.book_to)<=14);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=14 and DATEPART(HOUR,a.book_to)<=15);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=15 and DATEPART(HOUR,a.book_to)<=16);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=16 and DATEPART(HOUR,a.book_to)<=17);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=17 and DATEPART(HOUR,a.book_to)<=18);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=18 and DATEPART(HOUR,a.book_to)<=19);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=19 and DATEPART(HOUR,a.book_to)<=20);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=20 and DATEPART(HOUR,a.book_to)<=21);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=21 and DATEPART(HOUR,a.book_to)<=22);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=22 and DATEPART(HOUR,a.book_to)<=23);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=23 and DATEPART(HOUR,a.book_to)<=24);".Replace("#account_id#", account_id.ToString()).Replace("#from#", from.ToString(api_constants.sql_datetime_format)).Replace("#to#", to.ToString(api_constants.sql_datetime_format)));

    public DataSet get_booking_hour(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data(("select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=0 and DATEPART(HOUR,a.book_to)<=1) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=1 and DATEPART(HOUR,a.book_to)<=2) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=2 and DATEPART(HOUR,a.book_to)<=3) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=3 and DATEPART(HOUR,a.book_to)<=4) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=4 and DATEPART(HOUR,a.book_to)<=5) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=5 and DATEPART(HOUR,a.book_to)<=6) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=6 and DATEPART(HOUR,a.book_to)<=7) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=7 and DATEPART(HOUR,a.book_to)<=8) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=8 and DATEPART(HOUR,a.book_to)<=9) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=9 and DATEPART(HOUR,a.book_to)<=10) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=10 and DATEPART(HOUR,a.book_to)<=11) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=11 and DATEPART(HOUR,a.book_to)<=12) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=12 and DATEPART(HOUR,a.book_to)<=13) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=13 and DATEPART(HOUR,a.book_to)<=14) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=14 and DATEPART(HOUR,a.book_to)<=15) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=15 and DATEPART(HOUR,a.book_to)<=16) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=16 and DATEPART(HOUR,a.book_to)<=17) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=17 and DATEPART(HOUR,a.book_to)<=18) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=18 and DATEPART(HOUR,a.book_to)<=19) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=19 and DATEPART(HOUR,a.book_to)<=20) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=20 and DATEPART(HOUR,a.book_to)<=21) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=21 and DATEPART(HOUR,a.book_to)<=22) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=22 and DATEPART(HOUR,a.book_to)<=23) and booked_for = " + (object) user_id + ";" + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=1 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=23 and DATEPART(HOUR,a.book_to)<=24);").Replace("#account_id#", account_id.ToString()).Replace("#from#", from.ToString(api_constants.sql_datetime_format)).Replace("#to#", to.ToString(api_constants.sql_datetime_format)));

    public DataSet get_cancel_hour(DateTime from, DateTime to, Guid account_id) => this.get_data("select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=0 and DATEPART(HOUR,a.book_to)<=1);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=1 and DATEPART(HOUR,a.book_to)<=2);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=2 and DATEPART(HOUR,a.book_to)<=3);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=3 and DATEPART(HOUR,a.book_to)<=4);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=4 and DATEPART(HOUR,a.book_to)<=5);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=5 and DATEPART(HOUR,a.book_to)<=6);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=6 and DATEPART(HOUR,a.book_to)<=7);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=7 and DATEPART(HOUR,a.book_to)<=8);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=8 and DATEPART(HOUR,a.book_to)<=9);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=9 and DATEPART(HOUR,a.book_to)<=10);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=10 and DATEPART(HOUR,a.book_to)<=11);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=11 and DATEPART(HOUR,a.book_to)<=12);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=12 and DATEPART(HOUR,a.book_to)<=13);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=13 and DATEPART(HOUR,a.book_to)<=14);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=14 and DATEPART(HOUR,a.book_to)<=15);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=15 and DATEPART(HOUR,a.book_to)<=16);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=16 and DATEPART(HOUR,a.book_to)<=17);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=17 and DATEPART(HOUR,a.book_to)<=18);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=18 and DATEPART(HOUR,a.book_to)<=19);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=19 and DATEPART(HOUR,a.book_to)<=20);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=20 and DATEPART(HOUR,a.book_to)<=21);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=21 and DATEPART(HOUR,a.book_to)<=22);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=22 and DATEPART(HOUR,a.book_to)<=23);select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=23 and DATEPART(HOUR,a.book_to)<=24);".Replace("#account_id#", account_id.ToString()).Replace("#from#", from.ToString(api_constants.sql_datetime_format)).Replace("#to#", to.ToString(api_constants.sql_datetime_format)));

    public DataSet get_cancel_hour(DateTime from, DateTime to, Guid account_id, long user_id) => this.get_data(("select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=0 and DATEPART(HOUR,a.book_to)<=1) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=1 and DATEPART(HOUR,a.book_to)<=2) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=2 and DATEPART(HOUR,a.book_to)<=3) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=3 and DATEPART(HOUR,a.book_to)<=4) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=4 and DATEPART(HOUR,a.book_to)<=5) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=5 and DATEPART(HOUR,a.book_to)<=6) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=6 and DATEPART(HOUR,a.book_to)<=7) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=7 and DATEPART(HOUR,a.book_to)<=8) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=8 and DATEPART(HOUR,a.book_to)<=9) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=9 and DATEPART(HOUR,a.book_to)<=10) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=10 and DATEPART(HOUR,a.book_to)<=11) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=11 and DATEPART(HOUR,a.book_to)<=12) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=12 and DATEPART(HOUR,a.book_to)<=13) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=13 and DATEPART(HOUR,a.book_to)<=14) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=14 and DATEPART(HOUR,a.book_to)<=15) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=15 and DATEPART(HOUR,a.book_to)<=16) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=16 and DATEPART(HOUR,a.book_to)<=17) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=17 and DATEPART(HOUR,a.book_to)<=18) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=18 and DATEPART(HOUR,a.book_to)<=19) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=19 and DATEPART(HOUR,a.book_to)<=20) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=20 and DATEPART(HOUR,a.book_to)<=21) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=21 and DATEPART(HOUR,a.book_to)<=22) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=22 and DATEPART(HOUR,a.book_to)<=23) and booked_for = " + (object) user_id + "; " + " select count(a.booking_id) from sbt_asset_bookings a where a.account_id='#account_id#' and a.status=0 and a.book_from between '#from#' and '#to#' and (DATEPART(HOUR,a.book_from) >=23 and DATEPART(HOUR,a.book_to)<=24) and booked_for = " + (object) user_id + "; ").Replace("#account_id#", account_id.ToString()).Replace("#from#", from.ToString(api_constants.sql_datetime_format)).Replace("#to#", to.ToString(api_constants.sql_datetime_format)));
  }
}
