// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.holidays_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class holidays_api : api_base
  {
    public holiday get_holiday(long holiday_id, Guid account_id)
    {
      holiday holiday = new holiday();
      holiday.holiday_id = 0L;
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      string str = "select * from sbt_holidays where holiday_id='" + (object) holiday_id + "' and account_id='" + (object) account_id + "'";
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
              holiday.holiday_id = (long) objArray[index1];
            }
            catch
            {
              holiday.holiday_id = 0L;
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              holiday.account_id = (Guid) objArray[index2];
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
              holiday.created_on = (DateTime) objArray[index3];
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
              holiday.created_by = (long) objArray[index4];
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
              holiday.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              holiday.modified_on = holiday.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              holiday.modified_by = (long) objArray[index6];
            }
            catch
            {
              holiday.modified_by = holiday.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              holiday.holiday_name = (string) objArray[index7];
            }
            catch
            {
              holiday.holiday_name = "";
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              holiday.from_date = (DateTime) objArray[index8];
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
              holiday.record_id = (Guid) objArray[index9];
            }
            catch
            {
              holiday.record_id = Guid.Empty;
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              holiday.repeat = (bool) objArray[index10];
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
              holiday.to_date = (DateTime) objArray[index11];
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
        holiday.holiday_id = 0L;
        this.log.Error((object) str, ex);
      }
      return holiday;
    }

    public DataSet get_holidays(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_holidays where account_id='" + (object) account_id + "' order by from_date") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:get_holidays -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_holidays(DateTime from, DateTime to, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select holiday_id,holiday_name,from_date,to_date from sbt_holidays where account_id='" + (object) account_id + "' and ((from_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "') or (to_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'))  order by from_date;" + "select holiday_id,holiday_name,from_date,to_date from sbt_holidays where account_id='" + (object) account_id + "' and repeat='1';") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:get_holidays -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_repeat_holiday(DateTime from, DateTime to, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select holiday_id,holiday_name,from_date,to_date from sbt_holidays where account_id='" + (object) account_id + "' and repeat = 1 and day(from_date) >= " + (object) from.Day + " and month(from_date) >= " + (object) from.Month + " and day(to_date) <= " + (object) to.Day + " and month(to_date) <= " + (object) to.Month) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:get_holidays -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_already_holidays_Date(Guid account_id, DateTime start, DateTime end)
    {
      try
      {
        return this.db.get_dataset("select holiday_id from sbt_holidays where  account_id='" + (object) account_id + "' " + " and  from_date between '" + start.ToString(api_constants.sql_datetime_format_short) + "' and '" + end.ToString(api_constants.sql_datetime_format_short) + "' or to_date between '" + start.ToString(api_constants.sql_datetime_format_short) + "' and '" + end.ToString(api_constants.sql_datetime_format_short) + "' ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:check_already_holidays_Date -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_already_holidays_name(string holidayname, Guid account_id, string year)
    {
      try
      {
        return this.db.get_dataset(" select holiday_id from sbt_holidays where  account_id='" + (object) account_id + "'" + " and holiday_name='" + holidayname.Replace("'", "''") + "' and year(from_date)='" + year + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:check_already_holidays_name -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_already_holidays_name(
      string holidayname,
      string year,
      Guid account_id,
      DateTime start,
      DateTime end)
    {
      try
      {
        return this.db.get_dataset(" select holiday_id from sbt_holidays where  account_id='" + (object) account_id + "' and holiday_name = '" + holidayname.Replace("'", "''") + "' and(repeat = 1 or " + " (('" + start.ToString("MM-dd-yyyy hh:mm:ss") + "' between from_date and to_date) and('" + end.ToString("MM-dd-yyyy hh:mm:ss") + "' between from_date and to_date)))") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:check_already_holidays_name -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_already_holidays_Date(
      string holidayname,
      string year,
      Guid account_id,
      DateTime start,
      DateTime end)
    {
      try
      {
        return this.db.get_dataset(" select holiday_id,holiday_name from sbt_holidays where  account_id='" + (object) account_id + "'" + " and (from_date between '" + start.ToString(api_constants.sql_datetime_format_short) + "' and '" + end.ToString(api_constants.sql_datetime_format_short) + "' or to_date between '" + start.ToString(api_constants.sql_datetime_format_short) + "' and '" + end.ToString(api_constants.sql_datetime_format_short) + "'" + " or (repeat = 1 and day(from_date) = day('" + start.ToString(api_constants.sql_datetime_format_short) + "') and month(from_date) = month('" + start.ToString(api_constants.sql_datetime_format_short) + "') and day(to_date) = day('" + end.ToString(api_constants.sql_datetime_format_short) + "') and month(to_date) = month('" + end.ToString(api_constants.sql_datetime_format_short) + "')))") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:check_already_holidays_Date -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_already_holidays_edit_name(
      long holiday_id,
      string holidayname,
      string year,
      Guid account_id,
      DateTime start,
      DateTime end)
    {
      try
      {
        return this.db.get_dataset(" select holiday_id from sbt_holidays where holiday_id!='" + (object) holiday_id + "' AND account_id='" + (object) account_id + "' and holiday_name = '" + holidayname.Replace("'", "''") + "' and(repeat = 1 or " + " (('" + start.ToString("MM-dd-yyyy hh:mm:ss") + "' between from_date and to_date) and('" + end.ToString("MM-dd-yyyy hh:mm:ss") + "' between from_date and to_date)))") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:check_already_holidays_edit_name -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet check_already_holidays_edit_date(
      long holiday_id,
      string holidayname,
      string year,
      Guid account_id,
      DateTime start,
      DateTime end)
    {
      try
      {
        return this.db.get_dataset("select holiday_id from sbt_holidays where holiday_id!='" + (object) holiday_id + "' AND  account_id='" + (object) account_id + "'" + " and (from_date between '" + start.ToString(api_constants.sql_datetime_format_short) + "' and '" + end.ToString(api_constants.sql_datetime_format_short) + "' or to_date between '" + start.ToString(api_constants.sql_datetime_format_short) + "' and '" + end.ToString(api_constants.sql_datetime_format_short) + "'" + " or (repeat = 1 and day(from_date) = day('" + start.ToString(api_constants.sql_datetime_format_short) + "') and month(from_date) = month('" + start.ToString(api_constants.sql_datetime_format_short) + "') and day(to_date) = day('" + end.ToString(api_constants.sql_datetime_format_short) + "') and month(to_date) = month('" + end.ToString(api_constants.sql_datetime_format_short) + "')))") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:check_already_holidays_edit_date -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet view_holidays(string from, string to, Guid account_id)
    {
      try
      {
        string Sql = "select holiday_id,holiday_name,from_date,to_date,repeat from sbt_holidays where account_id='" + (object) account_id + "'";
        if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
          Sql = Sql + "  and ( (from_date between '" + from + "'  and '" + to + "')" + "     or (to_date between '" + from + "'  and '" + to + "')" + "     or ('" + from + "' between from_date and to_date)" + "     or ('" + to + "' between from_date and to_date))";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:view_holidays -> ", ex);
      }
      return (DataSet) null;
    }

    public bool is_holiday(DateTime date, Guid account_id)
    {
      try
      {
        return Convert.ToInt16(this.db.execute_scalar("select count(holiday_id) from sbt_holidays where account_id='" + (object) account_id + "' and from_date='" + date.ToString(api_constants.sql_datetime_format) + "'")) > (short) 0;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:is_holiday -> ", ex);
      }
      return false;
    }

    public holiday update_holiday(holiday obj)
    {
      try
      {
        obj.holiday_id = !this.db.execute_procedure("sbt_sp_holiday_update", new Dictionary<string, object>()
        {
          {
            "@holiday_id",
            (object) obj.holiday_id
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
            "@holiday_name",
            (object) obj.holiday_name
          },
          {
            "@from_date",
            (object) obj.from_date
          },
          {
            "@to_date",
            (object) obj.to_date
          },
          {
            "@repeat",
            (object) obj.repeat
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "err:update_holiday -> ", ex);
      }
      return obj;
    }

    public holiday delete_holiday(holiday obj)
    {
      try
      {
        obj.holiday_id = !this.db.execute_procedure("sbt_sp_holiday_delete", new Dictionary<string, object>()
        {
          {
            "@holiday_id",
            (object) obj.holiday_id
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
        this.log.Error((object) "err:delete_holiday -> ", ex);
      }
      return obj;
    }
  }
}
