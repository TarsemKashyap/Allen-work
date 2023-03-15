// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.content_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class content_api : api_base
  {
    public DataSet get_content_list(
      Guid account_id,
      string from,
      string to,
      string search,
      string start,
      string end,
      string orderby,
      string orderdir)
    {
      return this.db.get_dataset("select * from (SELECT Row_Number() OVER(ORDER BY r." + orderby + " " + orderdir + ") AS [Row],r.* from (" + "select content_id,show_from,show_to,repeatable,year(show_to) as Year,Month(show_to) as Month,title,content_details from sbt_content  WHERE account_id='" + (object) account_id + "' " + " and (show_from between '" + from + "' and '" + to + "'  or show_to between '" + from + "' and '" + to + "') " + "  ) as r ) AS result WHERE [Row] BETWEEN " + start + " AND " + end + " AND  (title like '%" + search + "%' or result.Year like'%" + search + "%' or result.Month like '%" + search + "%') " + "select count(*) from(SELECT Row_Number() OVER(ORDER BY r." + orderby + " " + orderdir + ") AS [Row],r.* from(select *,year(show_to) as Year,Month(show_to) as Month from  sbt_content  WHERE account_id='" + (object) account_id + "' " + " and (show_from between '" + from + "' and '" + to + "'  or show_to between '" + from + "' and '" + to + "')" + "  ) as r ) AS result WHERE [Row] BETWEEN " + start + " AND " + end + " AND  (title like '%" + search + "%' or result.Year like'%" + search + "%' or result.Month like '%" + search + "%') ") ? this.db.resultDataSet : (DataSet) null;
    }

    public long get_unread_content_count(long user_id, Guid account_id) => Convert.ToInt64(this.db.execute_scalar("select dbo.sbt_fn_unread_content_count('" + (object) user_id + "','" + (object) account_id + "') as counter"));

    public DataSet get_contents(Guid account_id, DateTime from, DateTime to, string keyword)
    {
      string str = "select * from sbt_content  WHERE account_id='" + (object) account_id + "'";
      if (keyword != null && keyword.Trim() != "")
        str = str + " and title like '%" + keyword + "%' or content_details like '%" + keyword + "%' ";
      return this.db.get_dataset(str + " and show_from between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "' order by show_from desc") ? this.db.resultDataSet : (DataSet) null;
    }

    public DataSet get_content_list(long content_id, Guid account_id) => this.db.get_dataset("select *,year(show_to) as Year from sbt_content  WHERE content_id=" + (object) content_id + " and account_id='" + (object) account_id + "'  ") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_years(Guid account_id) => this.db.get_dataset("select distinct(year(show_from)) as year from sbt_content  WHERE account_id='" + (object) account_id + "'  ") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_pushlished_content_list(Guid account_id, string from, string to) => this.db.get_dataset("select *,year(show_to) as Year from sbt_content  WHERE  published=1 and  account_id='" + (object) account_id + "' " + " and (show_from between '" + from + "' and '" + to + "' or show_to between '" + from + "' and '" + to + "' ) order by show_from desc") ? this.db.resultDataSet : (DataSet) null;

    public content_properties update_content(content_properties obj)
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("@content_id", (object) obj.content_id);
      parameters.Add("@account_id", (object) obj.account_id);
      parameters.Add("@created_by", (object) obj.created_by);
      parameters.Add("@modified_by", (object) obj.modified_by);
      parameters.Add("@record_id", (object) obj.record_id);
      parameters.Add("@title", (object) obj.title);
      parameters.Add("@content_details", (object) obj.content_details);
      parameters.Add("@asset_id", (object) obj.asset_id);
      parameters.Add("@flag", (object) obj.flag);
      parameters.Add("@show_from", (object) obj.show_from);
      parameters.Add("@show_to", (object) obj.show_to);
      parameters.Add("@published", (object) obj.published);
      parameters.Add("@repeatable", (object) obj.repeatable);
      parameters.Add("@type", (object) obj.type);
      bool flag;
      try
      {
        flag = this.db.execute_procedure("sbt_sp_content_update", parameters);
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ("asset booking : " + ex.ToString()));
      }
      obj.content_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public bool update_view(long content_id, long user_id, Guid account_id) => this.db.execute_procedure("sbt_sp_content_view_update", new Dictionary<string, object>()
    {
      {
        "@content_id",
        (object) content_id
      },
      {
        "@account_id",
        (object) account_id
      },
      {
        "@user_id",
        (object) user_id
      }
    });

    public bool update_view(long user_id, Guid account_id)
    {
      string sql = "";
      if (this.db.get_dataset("select content_id from sbt_content where content_id not in (select content_id from sbt_content_view where account_id='" + (object) account_id + "' and user_id='" + (object) user_id + "')") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          sql = sql + "insert into sbt_content_view (content_id,user_id,viewed_on,account_id) values ('" + row["content_id"].ToString() + "','" + (object) user_id + "',GETUTCDATE(),'" + (object) account_id + "');";
        this.db.execute_scalar(sql);
      }
      return true;
    }

    public content_properties delete_content(content_properties obj)
    {
      obj.content_id = !this.db.execute_procedure("sbt_sp_content_delete", new Dictionary<string, object>()
      {
        {
          "@content_id",
          (object) obj.content_id
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
      return obj;
    }
  }
}
