// Decompiled with JetBrains decompiler
// Type: skynapse.core.core_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using skynapse.fbs;
using System;
using System.Configuration;

namespace skynapse.core
{
  public class core_api
  {
    protected DataAccess db;
    protected ILog log = LogManager.GetLogger("fbs_log");
    protected string sql_datetime_format = ConfigurationManager.AppSettings["sql_date_time"];
    protected string sql_datetime_format_short = ConfigurationManager.AppSettings["sql_date_time_short"];
    protected string display_datetime_format = ConfigurationManager.AppSettings["date_time_long"];
    protected string display_datetime_format_short = ConfigurationManager.AppSettings["date_time_short"];

    public core_api() => this.db = new DataAccess(ConfigurationManager.ConnectionStrings["vms_sql"].ConnectionString);

    public bool is_valid(object obj) => obj != null && obj != DBNull.Value;
  }
}
