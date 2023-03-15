// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.help_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class help_api : api_base
  {
    public DataSet get_help(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_help");
      if (cache != null)
        return cache;
      try
      {
        if (!this.db.get_dataset("SELECT * FROM sbt_help_template WHERE account_id='" + (object) account_id + "'"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_help", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_help(Guid account_id, long help_id)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM sbt_help_template WHERE  help_id=" + (object) help_id + " and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_help(Guid account_id, string pageName)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM sbt_help_template WHERE  Page_name='" + pageName + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public clsHelp update_help(clsHelp obj)
    {
      try
      {
        if (this.db.execute_procedure("sbt_sp_help_template_update", new Dictionary<string, object>()
        {
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@sbt_help_id",
            (object) obj.help_id
          },
          {
            "@page_name",
            (object) obj.page_name
          },
          {
            "@description",
            (object) obj.description
          },
          {
            "@help_content",
            (object) obj.help_content
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
          }
        }))
          obj.help_id = Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.help_id = 0L;
      }
      if (obj.help_id > 0L)
        this.capi.remove_cache("help");
      return obj;
    }
  }
}
