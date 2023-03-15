// Decompiled with JetBrains decompiler
// Type: visitor_management.visitor_management_report_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System;
using System.Collections.Generic;
using System.Data;

namespace visitor_management
{
  public class visitor_management_report_api : core_api
  {
    public Dictionary<int, string> months;
    private string sql_date_format = "yyyy-MM-dd hh:mm:ss";

    public visitor_management_report_api()
    {
      this.months = new Dictionary<int, string>();
      this.months.Add(1, "Jan");
      this.months.Add(2, "Feb");
      this.months.Add(3, "Mar");
      this.months.Add(4, "Apr");
      this.months.Add(5, "May");
      this.months.Add(6, "Jun");
      this.months.Add(7, "Jul");
      this.months.Add(8, "Aug");
      this.months.Add(9, "Sep");
      this.months.Add(10, "Oct");
      this.months.Add(11, "Nov");
      this.months.Add(12, "Dec");
    }

    public DataSet visit_durations(DateTime from, DateTime to, Guid account_id) => this.db.get_dataset("SELECT max(datediff(minute,time_in,time_out)) as max_min,min(datediff(minute,time_in,time_out)) as min_min,avg(datediff(minute,time_in,time_out)) as avg_min FROM sbt_vms_visit where account_id = '" + (object) account_id + "' and (status = 1 and time_in between '" + from.ToString(this.sql_date_format) + "' and '" + to.ToString(this.sql_date_format) + "')") ? this.db.resultDataSet : (DataSet) null;

    public int illegal_scan_count(DateTime from, DateTime to, Guid account_id)
    {
      int num = 0;
      if (this.db.get_dataset("select count(alert_id) as counter from abt_alerts where alert_type=1 and account_id='" + (object) account_id + "'"))
        num = Convert.ToInt32(this.db.resultDataSet.Tables[0].Rows[0][0]);
      return num;
    }

    public int outstanding_card_count(Guid account_id)
    {
      int num = 0;
      if (this.db.get_dataset("select count(visit_id) as counter from sbt_vms_visit where status=0 and account_id='" + (object) account_id + "'"))
        num = Convert.ToInt32(this.db.resultDataSet.Tables[0].Rows[0][0]);
      return num;
    }

    public DataSet visitor_types_by_period(DateTime from, DateTime to, Guid account_id) => this.db.get_dataset("select count(a.visit_id) visit_counts, (select value from sbt_vms_settings where setting_id=b.visitor_type) as type,(select properties.value('(properties/color)[1]','nvarchar(10)') from sbt_vms_settings where setting_id=b.visitor_type) as type_color from sbt_vms_visit a,sbt_vms_visitor b where a.visitor_id=b.visitor_id and a.account_id='" + (object) account_id + "' and (time_in between '" + from.ToString(this.sql_date_format) + "' and '" + to.ToString(this.sql_date_format) + "') group by b.visitor_type") ? this.db.resultDataSet : (DataSet) null;

    public DataSet visitor_category_by_period(DateTime from, DateTime to, Guid account_id) => this.db.get_dataset("select count(a.visit_id) visit_counts, (select value from sbt_vms_settings where setting_id=b.visitor_category) as category,(select properties.value('(properties/color)[1]','nvarchar(10)') from sbt_vms_settings where setting_id=b.visitor_category) as category_color from sbt_vms_visit a,sbt_vms_visitor b where a.visitor_id=b.visitor_id and a.account_id='" + (object) account_id + "' and (time_in between '" + from.ToString(this.sql_date_format) + "' and '" + to.ToString(this.sql_date_format) + "') group by b.visitor_category") ? this.db.resultDataSet : (DataSet) null;

    public DataSet visits_by_period(DateTime from, DateTime to, Guid account_id) => this.db.get_dataset("select year(time_in) as year_val, month(time_in) as month_val, count(visit_id) as visits from sbt_vms_visit where time_in between '" + from.ToString(this.sql_date_format) + "' and '" + to.ToString(this.sql_date_format) + "' group by month(time_in),year(time_in) order by year_val,month_val") ? this.db.resultDataSet : (DataSet) null;
  }
}
