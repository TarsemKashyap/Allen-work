// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.templates_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class templates_api : api_base
  {
    public DataSet get_templates(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_templates where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_templates_view(Guid account_id)
    {
      DataSet cache = (DataSet) this.capi.get_cache(account_id.ToString() + "_templates");
      if (cache != null)
        return cache;
      try
      {
        if (!this.db.get_dataset("select template_id,title,name, description, content_data,record_id  from sbt_templates where account_id='" + (object) account_id + "'"))
          return (DataSet) null;
        this.capi.set_cache(account_id.ToString() + "_templates", (object) this.db.resultDataSet);
        return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    private template get_template_object(string sql)
    {
      template templateObject = new template();
      templateObject.template_id = 0L;
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(sql);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          int index1 = 0;
          if (this.is_valid(objArray[index1]))
          {
            try
            {
              templateObject.template_id = (long) objArray[index1];
            }
            catch
            {
              templateObject.template_id = 0L;
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              templateObject.account_id = (Guid) objArray[index2];
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
              templateObject.created_on = (DateTime) objArray[index3];
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
              templateObject.created_by = (long) objArray[index4];
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
              templateObject.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              templateObject.modified_on = templateObject.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              templateObject.modified_by = (long) objArray[index6];
            }
            catch
            {
              templateObject.modified_by = templateObject.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              templateObject.name = (string) objArray[index7];
            }
            catch
            {
              templateObject.name = "";
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              templateObject.description = (string) objArray[index8];
            }
            catch
            {
              templateObject.description = "";
            }
          }
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
          {
            try
            {
              templateObject.content_data = (string) objArray[index9];
            }
            catch
            {
              templateObject.content_data = "";
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              templateObject.record_id = (Guid) objArray[index10];
            }
            catch
            {
              templateObject.record_id = Guid.Empty;
            }
          }
          int index11 = index10 + 1;
          if (this.is_valid(objArray[index11]))
          {
            try
            {
              templateObject.title = (string) objArray[index11];
            }
            catch
            {
              templateObject.title = "";
            }
          }
          int num = index11 + 1;
        }
      }
      catch (Exception ex)
      {
        templateObject.template_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return templateObject;
    }

    public template get_template(long template_id, Guid account_id) => this.get_template_object("select * from sbt_templates where template_id='" + (object) template_id + "' and account_id='" + (object) account_id + "'");

    public template get_template(string name, Guid account_id) => this.get_template_object("select * from sbt_templates where name='" + name + "' and account_id='" + (object) account_id + "'");

    public template update_template(template obj)
    {
      try
      {
        obj.template_id = !this.db.execute_procedure("sbt_sp_template_update", new Dictionary<string, object>()
        {
          {
            "@template_id",
            (object) obj.template_id
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
            "@content_data",
            (object) obj.content_data
          },
          {
            "@title",
            (object) obj.title
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      if (obj.template_id > 0L)
        this.capi.remove_cache("templates");
      return obj;
    }
  }
}
