// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.resource_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class resource_api : api_base
  {
    public DataSet get_resource_type_by_id(long res_type_id, Guid account_id)
    {
      DataSet resourceTypeById = new DataSet();
      try
      {
        resourceTypeById = !this.db.get_dataset("\tselect setting_id, value,status,parameter,description FROM sbt_modules_resource_settings where  account_id='" + (object) account_id + "' and setting_id = " + (object) res_type_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceTypeById;
    }

    public resource_type get_resource_type_obj(long setting_id, Guid account_id)
    {
      string str = "select setting_id,account_id,created_on,created_by,modified_on,modified_by,parameter,value,status,description,record_id from sbt_modules_resource_settings where setting_id='" + (object) setting_id + "' and account_id='" + (object) account_id + "'";
      resource_type resourceTypeObj = new resource_type();
      resourceTypeObj.setting_id = 0L;
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          int index1 = 0;
          if (objArray[index1] != null)
          {
            try
            {
              resourceTypeObj.setting_id = (long) objArray[index1];
            }
            catch
            {
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              resourceTypeObj.account_id = (Guid) objArray[index2];
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
              resourceTypeObj.created_on = (DateTime) objArray[index3];
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
              resourceTypeObj.created_by = (long) objArray[index4];
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
              resourceTypeObj.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              resourceTypeObj.modified_on = resourceTypeObj.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              resourceTypeObj.modified_by = (long) objArray[index6];
            }
            catch
            {
              resourceTypeObj.modified_by = resourceTypeObj.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              resourceTypeObj.parameter = (string) objArray[index7];
            }
            catch
            {
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              resourceTypeObj.value = (string) objArray[index8];
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
              resourceTypeObj.status = (long) objArray[index9];
            }
            catch
            {
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              resourceTypeObj.description = (string) objArray[index10];
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
              resourceTypeObj.record_id = (Guid) objArray[index11];
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
        resourceTypeObj.setting_id = 0L;
        this.log.Error((object) str, ex);
      }
      return resourceTypeObj;
    }

    public resource_type update_resource_types(resource_type obj)
    {
      try
      {
        obj.setting_id = !this.db.execute_procedure("sbt_sp_resource_type_update", new Dictionary<string, object>()
        {
          {
            "@setting_id",
            (object) obj.setting_id
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
            "@status",
            (object) obj.status
          },
          {
            "@parameter",
            (object) obj.parameter
          },
          {
            "@value",
            (object) obj.value
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@module",
            (object) obj.module_name
          },
          {
            "@description",
            (object) obj.description
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
        obj.setting_id = 0L;
      }
      return obj;
    }

    public resource_type delete_resource_type(resource_type obj)
    {
      try
      {
        obj.setting_id = !this.db.execute_procedure("sbt_sp_resource_type_delete", new Dictionary<string, object>()
        {
          {
            "@setting_id",
            (object) obj.setting_id
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
        this.log.Error((object) "Error -> ", ex);
        obj.setting_id = 0L;
      }
      return obj;
    }

    public resource_settings update_resource_settings(resource_settings obj)
    {
      try
      {
        obj.setting_id = !this.db.execute_procedure("sbt_sp_modules_resource_settings", new Dictionary<string, object>()
        {
          {
            "@setting_id",
            (object) obj.setting_id
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
            "@status",
            (object) obj.status
          },
          {
            "@parameter",
            (object) obj.parameter
          },
          {
            "@value",
            (object) obj.value
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@parent_id",
            (object) obj.parent_id
          },
          {
            "@module",
            (object) obj.module_name
          },
          {
            "@description",
            (object) obj.description
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
        obj.setting_id = 0L;
      }
      return obj;
    }

    public DataSet get_resource_settings(Guid account_id, string module_name)
    {
      DataSet resourceSettings = new DataSet();
      try
      {
        resourceSettings = !this.db.get_dataset("\tselect * FROM sbt_modules_resource_settings where  account_id='" + (object) account_id + "' and module_name = '" + module_name + "'") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceSettings;
    }

    public DataSet get_resource_settings_by_parameter(
      Guid account_id,
      string parameter,
      string modulename)
    {
      DataSet settingsByParameter = new DataSet();
      try
      {
        settingsByParameter = !this.db.get_dataset("\tselect setting_id, value,status,description,parent_id FROM sbt_modules_resource_settings where  account_id='" + (object) account_id + "' and parameter='" + parameter + "' and status=1 and module_name = '" + modulename + "' order by value") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return settingsByParameter;
    }

    public DataSet get_all_resource_settings_by_parameter(
      Guid account_id,
      string parameter,
      string modulename)
    {
      DataSet settingsByParameter = new DataSet();
      try
      {
        settingsByParameter = !this.db.get_dataset("\tselect setting_id, value,status,description,parent_id FROM sbt_modules_resource_settings where  account_id='" + (object) account_id + "' and parameter='" + parameter + "' and module_name = '" + modulename + "' order by value") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return settingsByParameter;
    }

    public DataSet get_resource_items(Guid account_id, string modulename)
    {
      DataSet resourceItems = new DataSet();
      try
      {
        resourceItems = !this.db.get_dataset("\tselect a.item_id,a.name,a.item_type_id,a.status,a.owner_group_id,a.quantity,a.unit_price,a.description, " + " (select value from sbt_modules_resource_settings where account_id='" + (object) account_id + "' and parameter='resource_type' and setting_id=a.item_type_id) as resource_type" + " from sbt_modules_resource_items a where  account_id='" + (object) account_id + "' and module_name = '" + modulename + "'") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceItems;
    }

    public DataSet get_resource_items_by_id(long res_id, Guid account_id, string modulename)
    {
      DataSet resourceItemsById = new DataSet();
      try
      {
        resourceItemsById = !this.db.get_dataset("\tselect item_id,name,item_type_id,status,owner_group_id,quantity,unit_price,description,unit_of_measure, " + " (select setting_id from sbt_modules_resource_settings where account_id='" + (object) account_id + "' and parent_id=" + (object) res_id + " and parameter='Advance Notice Period') as advance_notice_period_setting_id, " + " (select value from sbt_modules_resource_settings where account_id='" + (object) account_id + "' and parent_id=" + (object) res_id + " and parameter='Advance Notice Period') as advance_notice_period, " + " (select setting_id from sbt_modules_resource_settings where account_id='" + (object) account_id + "' and parent_id=" + (object) res_id + " and parameter='Advance Notice Period Hours') as advance_notice_period_hours_setting_id, " + " (select value from sbt_modules_resource_settings where account_id='" + (object) account_id + "' and parent_id=" + (object) res_id + " and parameter='Advance Notice Period Hours') as advance_notice_period_hours " + " from sbt_modules_resource_items where  account_id='" + (object) account_id + "' and module_name = '" + modulename + "' and item_id = " + (object) res_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceItemsById;
    }

    public DataSet get_resource_items_by_item_type_id(
      long res_type_id,
      Guid account_id,
      string modulename)
    {
      DataSet itemsByItemTypeId = new DataSet();
      try
      {
        itemsByItemTypeId = !this.db.get_dataset("\tselect item_id,name,status,owner_group_id,quantity,unit_price,description from sbt_modules_resource_items where status=1 and module_name = '" + modulename + "' and  account_id='" + (object) account_id + "' and item_type_id = " + (object) res_type_id + " order by name") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return itemsByItemTypeId;
    }

    public additional_resource get_resource_item_obj(
      long item_id,
      Guid account_id,
      string modulename)
    {
      string str = "select item_id,account_id,created_on,created_by,modified_on,modified_by,name,status,description,owner_group_id from sbt_modules_resource_items where item_id ='" + (object) item_id + "' and module_name = '" + modulename + "' and account_id='" + (object) account_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      additional_resource resourceItemObj = new additional_resource();
      resourceItemObj.item_id = 0L;
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
              resourceItemObj.item_id = (long) objArray[index1];
            }
            catch
            {
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              resourceItemObj.account_id = (Guid) objArray[index2];
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
              resourceItemObj.created_on = (DateTime) objArray[index3];
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
              resourceItemObj.created_by = (long) objArray[index4];
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
              resourceItemObj.modified_on = (DateTime) objArray[index5];
            }
            catch
            {
              resourceItemObj.modified_on = resourceItemObj.created_on;
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              resourceItemObj.modified_by = (long) objArray[index6];
            }
            catch
            {
              resourceItemObj.modified_by = resourceItemObj.created_by;
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              resourceItemObj.name = (string) objArray[index7];
            }
            catch
            {
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              resourceItemObj.status = (int) objArray[index8];
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
              resourceItemObj.description = (string) objArray[index9];
            }
            catch
            {
            }
          }
          int index10 = index9 + 1;
          if (this.is_valid(objArray[index10]))
          {
            try
            {
              resourceItemObj.owner_group_id = (long) objArray[index10];
            }
            catch
            {
            }
          }
          int num = index10 + 1;
        }
      }
      catch (Exception ex)
      {
        resourceItemObj.item_id = 0L;
        this.log.Error((object) str, ex);
      }
      return resourceItemObj;
    }

    public additional_resource delete_resource_item(additional_resource obj)
    {
      try
      {
        obj.item_id = !this.db.execute_procedure("sbt_sp_resource_delete", new Dictionary<string, object>()
        {
          {
            "@item_id",
            (object) obj.item_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@modified_by",
            (object) obj.modified_by
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.item_id = 0L;
      }
      return obj;
    }

    public DataSet get_users_by_item(long item_id, Guid account_id)
    {
      DataSet usersByItem = new DataSet();
      try
      {
        usersByItem = !this.db.get_dataset("select * from vw_sbt_user_group_mappings where group_id in (select owner_group_id from sbt_modules_resource_items where item_id='" + (object) item_id + "' and account_id='" + (object) account_id + "')") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return usersByItem;
    }

    public DataSet get_user_item_map(
      long user_id,
      Guid account_id,
      bool is_admin,
      string modulename)
    {
      DataSet userItemMap = new DataSet();
      try
      {
        string Sql;
        if (!is_admin)
          Sql = "select item_id from sbt_modules_resource_items where account_id='" + (object) account_id + "' and module_name = '" + modulename + "' and owner_group_id in (select group_id from sbt_user_group_mappings where user_id=" + (object) user_id + " and account_id='" + (object) account_id + "')";
        else
          Sql = "select item_id from sbt_modules_resource_items where account_id='" + (object) account_id + "' and module_name = '" + modulename + "' and status>0";
        userItemMap = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return userItemMap;
    }

    public byte[] get_image(long resource_id, Guid account_id)
    {
      byte[] image = (byte[]) null;
      string Sql = "select binary_data from sbt_modules_resource_documents where account_id='" + (object) account_id + "' and resource_item_id='" + (object) resource_id + "'";
      try
      {
        if (this.db.get_dataset(Sql))
        {
          if (this.db.resultDataSet.Tables[0].Rows.Count > 0)
            image = (byte[]) this.db.resultDataSet.Tables[0].Rows[0][0];
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return image;
    }

    public DataSet get_resource_document_by_id(long res_doc_id, Guid account_id)
    {
      DataSet resourceDocumentById = new DataSet();
      try
      {
        resourceDocumentById = !this.db.get_dataset("\tselect * from [dbo].[sbt_modules_resource_documents] where  account_id='" + (object) account_id + "' and resource_document_id = " + (object) res_doc_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceDocumentById;
    }

    public DataSet get_resource_document_by_item_id(long res_item_id, Guid account_id)
    {
      DataSet documentByItemId = new DataSet();
      try
      {
        documentByItemId = !this.db.get_dataset("\tselect A.*, (select description from sbt_modules_resource_items where account_id='" + (object) account_id + "' and item_id=A.resource_item_id) as resource_description, " + " (select quantity from sbt_modules_resource_items where account_id='" + (object) account_id + "' and item_id=A.resource_item_id) as total_qty " + "  from sbt_modules_resource_documents A where A.attachment_type='resource_item' and  A.account_id='" + (object) account_id + "' and A.resource_item_id = " + (object) res_item_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return documentByItemId;
    }

    public DataSet get_resource_document_by_resource_booking_id(
      long res_booking_id,
      Guid account_id)
    {
      DataSet resourceBookingId = new DataSet();
      try
      {
        resourceBookingId = !this.db.get_dataset("\tselect A.*, (select description from sbt_modules_resource_items where account_id='" + (object) account_id + "' and item_id=A.resource_item_id) as resource_description, " + " (select quantity from sbt_modules_resource_items where account_id='" + (object) account_id + "' and item_id=A.resource_item_id) as total_qty " + "  from sbt_modules_resource_documents A where A.attachment_type='resource_booking' and  A.account_id='" + (object) account_id + "' and A.resource_item_id = " + (object) res_booking_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceBookingId;
    }

    public resource_document update_resource_document(resource_document obj)
    {
      try
      {
        obj.resource_document_id = !this.db.execute_procedure("sbt_sp_resource_document_update", new Dictionary<string, object>()
        {
          {
            "@resource_document_id",
            (object) obj.resource_document_id
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
            "@document_name",
            (object) obj.document_name
          },
          {
            "@document_size",
            (object) obj.document_size
          },
          {
            "@document_meta",
            (object) obj.document_meta
          },
          {
            "@document_type",
            (object) obj.document_type
          },
          {
            "@attachment_type",
            (object) obj.attachment_type
          },
          {
            "@resource_item_id",
            (object) obj.resource_item_id
          },
          {
            "@binary_data",
            (object) obj.binary_data
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
        obj.resource_document_id = 0L;
      }
      return obj;
    }

    public resource_document delete_resource_document(resource_document obj)
    {
      try
      {
        obj.resource_document_id = !this.db.execute_procedure("sbt_sp_resource_document_delete", new Dictionary<string, object>()
        {
          {
            "@resource_document_id",
            (object) obj.resource_document_id
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
        this.log.Error((object) "Error -> ", ex);
        obj.resource_document_id = 0L;
      }
      return obj;
    }

    public DataSet view_templates(Guid account_id, string modulename)
    {
      DataSet dataSet = new DataSet();
      string Sql = "select a.resource_template_id,a.status,a.template_name,a.description, b.item_id,b.quantity,(select unit_of_measure from sbt_modules_resource_items where account_id = '" + (object) account_id + "' and item_id = b.item_id) as unit_of_measure, (select name from sbt_modules_resource_items where account_id = '" + (object) account_id + "' and item_id = b.item_id) as item_name  from sbt_modules_resource_templates a, sbt_modules_resource_templates_items b where a.resource_template_id = b.resource_template_id and a.account_id = '" + (object) account_id + "' and a.module_name = '" + modulename + "'  and a.status = b.status order by a.template_name";
      try
      {
        dataSet = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return dataSet;
    }

    public DataSet get_resource_templates(Guid account_id)
    {
      DataSet resourceTemplates = new DataSet();
      try
      {
        resourceTemplates = !this.db.get_dataset("\tselect * from sbt_modules_resource_templates where status > 0 and  account_id='" + (object) account_id + "'") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceTemplates;
    }

    public DataSet get_resource_template_by_templateId(Guid account_id, long templateId)
    {
      DataSet templateByTemplateId = new DataSet();
      try
      {
        templateByTemplateId = !this.db.get_dataset("\tselect * from vw_sbt_modules_resource_templates_items where template_item_status > 0  and account_id='" + (object) account_id + "' and resource_template_id=" + (object) templateId) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return templateByTemplateId;
    }

    public DataSet get_resource_template_item_by_templateId(long template_id, Guid account_id)
    {
      DataSet itemByTemplateId = new DataSet();
      try
      {
        itemByTemplateId = !this.db.get_dataset("\tselect A.*,(select name from sbt_modules_resource_items where account_id='" + (object) account_id + "' and item_id=A.item_id) as resource_name from sbt_modules_resource_templates_items A where A.status > 0  and A.account_id='" + (object) account_id + "' and A.resource_template_id = " + (object) template_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return itemByTemplateId;
    }

    public DataSet get_resource_templates(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id)
    {
      DataSet resourceTemplates = new DataSet();
      try
      {
        string str = "\tselect * from(SELECT  ROW_NUMBER() OVER (ORDER BY " + orderby + " " + orderdir + ") AS RowNumber,* FROM " + " (SELECT resource_template_id,template_name,status " + " FROM sbt_modules_resource_templates AS b WHERE b.account_id='" + (object) account_id + "' and b.status >= 0) AS c ";
        if (searchkey != "%")
          str = str + " wHERE (c.template_name LIKE '%" + searchkey + "%')";
        string Sql = str + " ) as sq where RowNumber BETWEEN " + fromrow + " and " + endrow + " ;" + " SELECT COUNT(*) AS RecordCnt FROM sbt_modules_resource_templates AS b WHERE b.status >= 0 and b.account_id='" + (object) account_id + "' ";
        if (searchkey != "%")
          Sql = Sql + " and (b.template_name LIKE '%" + searchkey + "%')";
        resourceTemplates = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceTemplates;
    }

    public resource_template update_resource_template(resource_template obj)
    {
      try
      {
        obj.resource_template_id = !this.db.execute_procedure("sbt_sp_resource_template_update", new Dictionary<string, object>()
        {
          {
            "@resource_template_id",
            (object) obj.resource_template_id
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
            "@template_name",
            (object) obj.template_name
          },
          {
            "@status",
            (object) obj.status
          },
          {
            "@description",
            (object) obj.description
          },
          {
            "@module",
            (object) obj.module_name
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return obj;
    }

    public resource_template_item update_resource_template_item(resource_template_item obj)
    {
      try
      {
        obj.resource_template_id = !this.db.execute_procedure("sbt_sp_resource_template_item_update", new Dictionary<string, object>()
        {
          {
            "@template_item_id",
            (object) obj.template_item_id
          },
          {
            "@resource_template_id",
            (object) obj.resource_template_id
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
            "@item_id",
            (object) obj.item_id
          },
          {
            "@quantity",
            (object) obj.quantity
          },
          {
            "@status",
            (object) obj.status
          },
          {
            "@module",
            (object) obj.module_name
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return obj;
    }

    public long delete_resource_template(long resource_template_id, Guid account_id, long user_id)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_resource_template_delete", new Dictionary<string, object>()
        {
          {
            "@resource_template_id",
            (object) resource_template_id
          },
          {
            "@account_id",
            (object) account_id
          },
          {
            "@modified_by",
            (object) user_id
          }
        }) ? Convert.ToInt64(this.db.resultString) : 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        return 0;
      }
    }

    public long delete_resource_template_item_template_id(
      long resTemplateId,
      Guid account_id,
      long user_id)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_resource_template_item_by_template_id_delete", new Dictionary<string, object>()
        {
          {
            "@resource_template_id",
            (object) resTemplateId
          },
          {
            "@account_id",
            (object) account_id
          },
          {
            "@modified_by",
            (object) user_id
          }
        }) ? Convert.ToInt64(this.db.resultString) : 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        return 0;
      }
    }

    public resource_template_item delete_resource_template_item(resource_template_item obj)
    {
      try
      {
        obj.resource_template_id = !this.db.execute_procedure("sbt_sp_resource_template_item_delete", new Dictionary<string, object>()
        {
          {
            "@template_item_id",
            (object) obj.template_item_id
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
        this.log.Error((object) "Error -> ", ex);
        obj.resource_template_id = 0L;
      }
      return obj;
    }

    public DataSet get_resource_bookings(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id,
      string modulename)
    {
      DataSet resourceBookings = new DataSet();
      try
      {
        string str = "\tselect * from(SELECT  ROW_NUMBER() OVER (ORDER BY " + orderby + " " + orderdir + ") AS RowNumber,* FROM (SELECT requested_qty,accepted_qty," + "  requestor_remarks, name, description, value as resource_type, from_date, to_date, purpose, venue, booked_for, requested_by_name as requestor, email, remarks,status " + "  FROM vw_sbt_modules_resource_bookings_items AS b WHERE b.account_id = " + (object) account_id + " and b.module_name = '" + modulename + "' and b.status >= 0) AS c ";
        if (searchkey != "%")
          str = str + "  wHERE ((c.name LIKE '%" + searchkey + "%') or (c.accepted_qty like '%" + searchkey + "%')  or (c.purpose like '%" + searchkey + "%')" + "  or  (c.resource_type like '%" + searchkey + "%'))";
        string Sql = str + " ) as sq where RowNumber BETWEEN " + fromrow + " and " + endrow + ";" + "  SELECT COUNT(*) AS RecordCnt FROM vw_sbt_modules_resource_bookings_items AS b WHERE b.account_id = " + (object) account_id + " and b.module_name = '" + modulename + "' and b.status >= 0 ";
        if (searchkey != "%")
          Sql = Sql + "  and ((b.name LIKE '%" + searchkey + "%') or (b.accepted_qty like '%" + searchkey + "%')  or (b.purpose like '%" + searchkey + "%')" + "  or  (b.resource_type like '%" + searchkey + "%'))";
        resourceBookings = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceBookings;
    }

    public DataSet get_resource_bookings_by_date_range(
      Guid account_id,
      DateTime from,
      DateTime to,
      long resource_type_id,
      long resource_id,
      string modulename)
    {
      DataSet bookingsByDateRange = new DataSet();
      string str = "SELECT *, value as resource_type, requested_by_name as requestor,status " + " from vw_sbt_modules_resource_bookings_items where  status > 0 and module_name = '" + modulename + "' and  account_id='" + account_id.ToString() + "'  ";
      if (resource_type_id > 0L)
        str = str + " and setting_id='" + (object) resource_type_id + "'  ";
      if (resource_id > 0L)
        str = str + " and resource_id='" + (object) resource_id + "'  ";
      if (from.Year != 1900 && to.Year != 1900)
        str = str + " and ((from_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and  '" + to.ToString(api_constants.sql_datetime_format) + "' ) or  " + " (to_date between '" + from.ToString(api_constants.sql_datetime_format) + "'  and  '" + to.ToString(api_constants.sql_datetime_format) + "') or  " + " ('" + from.ToString(api_constants.sql_datetime_format) + "' between from_date and to_date ) or  " + "  ('" + to.ToString(api_constants.sql_datetime_format) + "' between from_date and to_date )) ";
      string Sql = str + " order by from_date desc";
      try
      {
        bookingsByDateRange = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return bookingsByDateRange;
    }

    public DataSet get_resource_bookings_by_date_range(
      Guid account_id,
      long resource_type_id,
      long resource_id,
      string modulename)
    {
      DataSet bookingsByDateRange = new DataSet();
      string str = "SELECT *, value as resource_type, requested_by_name as requestor,status " + " from vw_sbt_modules_resource_bookings_items where  status > 0 and module_name = '" + modulename + "' and  account_id='" + account_id.ToString() + "'  ";
      if (resource_type_id > 0L)
        str = str + " and setting_id='" + (object) resource_type_id + "'  ";
      if (resource_id > 0L)
        str = str + " and resource_id='" + (object) resource_id + "'  ";
      string Sql = str + " order by from_date desc";
      try
      {
        bookingsByDateRange = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return bookingsByDateRange;
    }

    public DataSet get_resource_bookings_by_resource_booking_id(
      Guid account_id,
      long resource_booking_id,
      string modulename)
    {
      DataSet resourceBookingId = new DataSet();
      string Sql = "select * from vw_sbt_modules_resource_bookings_items where status > 0 and module_name = '" + modulename + "' and  account_id='" + account_id.ToString() + "'  ";
      if (resource_booking_id > 0L)
        Sql = Sql + " and resource_booking_id='" + (object) resource_booking_id + "'  ";
      try
      {
        resourceBookingId = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceBookingId;
    }

    public DataSet get_resource_bookings_by_repeat_reference(
      Guid account_id,
      Guid repeat_reference,
      string modulename)
    {
      DataSet byRepeatReference = new DataSet();
      string Sql = "select * from vw_sbt_modules_resource_bookings_items where status > 0 and module_name = '" + modulename + "' and  account_id='" + account_id.ToString() + "'  ";
      if (repeat_reference != Guid.Empty)
        Sql = Sql + " and repeat_reference='" + (object) repeat_reference + "'  ";
      try
      {
        byRepeatReference = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return byRepeatReference;
    }

    public DataSet get_resource_bookings_items_by_asset_booking_id(
      long asset_booking_id,
      Guid account_id,
      string fdate,
      string tdate,
      string modulename)
    {
      DataSet byAssetBookingId = new DataSet();
      string str = "select A.*  ";
      if (fdate != "" && tdate != "")
        str = str + "  , (A.quantity -(select SUM(accepted_qty) from vw_sbt_modules_resource_bookings_items where status=1 and module_name = '" + modulename + "' and resource_id=A.resource_id and  " + " ((from_date between '" + fdate + "' and  '" + tdate + "' ) or  " + " (to_date between '" + fdate + "'  and  '" + tdate + "') or  " + " ('" + fdate + "' between from_date and to_date ) or  " + "  ('" + tdate + "' between from_date and to_date )))) as available_qty  ";
      string Sql = str + " from vw_sbt_modules_resource_bookings_items A where status>0 and module_name = '" + modulename + "' and  account_id='" + (object) account_id + "'";
      if (asset_booking_id > 0L)
        Sql = Sql + "  and A.asset_booking_id=" + (object) asset_booking_id;
      try
      {
        byAssetBookingId = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return byAssetBookingId;
    }

    public DataSet get_resource_booking_items(
      long res_item_id,
      Guid account_id,
      string fdate,
      string tdate,
      string modulename)
    {
      DataSet resourceBookingItems = new DataSet();
      try
      {
        resourceBookingItems = !this.db.get_dataset("\tselect A.*, (select description from sbt_modules_resource_items where item_id=A.resource_item_id) as resource_description, " + " (select quantity from sbt_modules_resource_items where item_id=A.resource_item_id) as total_qty, " + " (select name from sbt_modules_resource_items where item_id=A.resource_item_id) as name, " + " (select value from sbt_modules_resource_settings where setting_id=(select item_type_id from sbt_modules_resource_items where item_id=A.resource_item_id and module_name = '" + modulename + "' and parameter='resource_type' )) as value," + "  (select SUM(accepted_qty) from vw_sbt_modules_resource_bookings_items where status=1 and module_name = '" + modulename + "' and  resource_id=A.resource_item_id and " + " ( (from_date between '" + fdate + "' and  '" + tdate + "' ) or  " + "   (to_date between '" + fdate + "'  and  '" + tdate + "') or  " + "   ('" + fdate + "' between from_date and to_date ) or  " + "   ('" + tdate + "' between from_date and to_date ) )" + " ) as accepted_qty  " + "  from sbt_modules_resource_documents A where A.attachment_type='resource_item' and  A.account_id='" + (object) account_id + "' and  A.resource_item_id = " + (object) res_item_id) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceBookingItems;
    }

    public DataSet get_resource_bookings(long asset_booking_id, Guid account_id, string modulename)
    {
      DataSet resourceBookings = new DataSet();
      string Sql = "select * from sbt_modules_resource_bookings where account_id='" + (object) account_id + "' and module_name = '" + modulename + "' and asset_booking_id=" + (object) asset_booking_id;
      try
      {
        resourceBookings = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceBookings;
    }

    public long delete_resource_bookings(long resource_booking_id, Guid account_id, long user_id)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_resource_bookings_delete", new Dictionary<string, object>()
        {
          {
            "@resource_booking_id",
            (object) resource_booking_id
          },
          {
            "@account_id",
            (object) account_id
          },
          {
            "@modified_by",
            (object) user_id
          }
        }) ? Convert.ToInt64(this.db.resultString) : 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        return 0;
      }
    }

    public long delete_resource_booking_items_by_resource_booking_id(
      long resource_booking_id,
      Guid account_id,
      long user_id)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_resource_booking_items_by_resource_booking_id_delete", new Dictionary<string, object>()
        {
          {
            "@resource_booking_id",
            (object) resource_booking_id
          },
          {
            "@account_id",
            (object) account_id
          },
          {
            "@modified_by",
            (object) user_id
          }
        }) ? Convert.ToInt64(this.db.resultString) : 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        return 0;
      }
    }

    public long delete_resource_booking_items(
      long resource_booking_item_id,
      Guid account_id,
      long user_id)
    {
      try
      {
        return this.db.execute_procedure("sbt_sp_resource_booking_items_delete", new Dictionary<string, object>()
        {
          {
            "@resource_booking_item_id",
            (object) resource_booking_item_id
          },
          {
            "@account_id",
            (object) account_id
          },
          {
            "@modified_by",
            (object) user_id
          }
        }) ? Convert.ToInt64(this.db.resultString) : 0L;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        return 0;
      }
    }

    public resource_booking update_resource_booking(resource_booking obj)
    {
      bool flag;
      try
      {
        flag = this.db.execute_procedure("sbt_sp_resource_booking_update", new Dictionary<string, object>()
        {
          {
            "@resource_booking_id",
            (object) obj.resource_booking_id
          },
          {
            "@item_id",
            (object) obj.item_id
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
            "@from_date",
            (object) obj.book_from
          },
          {
            "@to_date",
            (object) obj.book_to
          },
          {
            "@booked_for_id",
            (object) obj.booked_for_id
          },
          {
            "@layout_id",
            (object) obj.layout_id
          },
          {
            "@requested_by",
            (object) obj.requested_by_id
          },
          {
            "@booking_type",
            (object) obj.booking_type
          },
          {
            "@status",
            (object) obj.status
          },
          {
            "@purpose",
            (object) obj.purpose
          },
          {
            "@remarks",
            (object) obj.remarks
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@module",
            (object) obj.module_name
          },
          {
            "@email",
            (object) obj.email
          },
          {
            "@asset_booking_id",
            (object) obj.asset_booking_id
          },
          {
            "@venue",
            (object) obj.venue
          },
          {
            "@repeat_reference",
            (object) obj.repeat_reference
          }
        });
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ("Additional resource booking  : " + ex.ToString()));
      }
      obj.resource_booking_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public resource_booking_item update_resource_booking_item(resource_booking_item obj)
    {
      bool flag;
      try
      {
        flag = this.db.execute_procedure("sbt_sp_resource_booking_item_update", new Dictionary<string, object>()
        {
          {
            "@resource_booking_item_id",
            (object) obj.resource_booking_item_id
          },
          {
            "@resource_booking_id",
            (object) obj.resource_booking_id
          },
          {
            "@resource_id",
            (object) obj.resource_id
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
            "@status",
            (object) obj.status
          },
          {
            "@requested_qty",
            (object) obj.req_qty
          },
          {
            "@accepted_qty",
            (object) obj.accepted_qty
          },
          {
            "@requested_price",
            (object) obj.req_price
          },
          {
            "@accepted_price",
            (object) obj.accepted_price
          },
          {
            "@requestor_remarks",
            (object) obj.requestor_remakrs
          },
          {
            "@other_remarks",
            (object) obj.other_remarks
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@module",
            (object) obj.module_name
          }
        });
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ("Additional resource booking items : " + ex.ToString()));
      }
      obj.resource_booking_item_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public long get_resource_booking_id(long asset_booking_id, Guid account_id, string modulename)
    {
      long resourceBookingId = 0;
      string Sql = "select resource_booking_id from sbt_modules_resource_bookings where account_id='" + (object) account_id + "' and module_name = '" + modulename + "' and asset_booking_id=" + (object) asset_booking_id;
      try
      {
        if (this.db.get_dataset(Sql))
          resourceBookingId = this.db.resultDataSet.Tables[0].Rows.Count <= 0 ? 0L : Convert.ToInt64(this.db.resultDataSet.Tables[0].Rows[0][0]);
      }
      catch (Exception ex)
      {
        resourceBookingId = 0L;
        this.log.Error((object) ex.ToString());
      }
      return resourceBookingId;
    }

    public double get_booked_quantity(
      long resource_id,
      Guid account_id,
      DateTime from_date,
      DateTime to_date,
      string modulename)
    {
      from_date = from_date.AddSeconds(1.0);
      to_date = to_date.AddSeconds(-1.0);
      string Sql = "select from_date, to_date, accepted_qty from vw_sbt_modules_resource_bookings_items where status = 1 and module_name = '" + modulename + "' and resource_id ='" + (object) resource_id + "'and account_id ='" + (object) account_id + "' and" + "(" + "(from_date between '" + from_date.ToString(api_constants.sql_datetime_format) + "' and  '" + to_date.ToString(api_constants.sql_datetime_format) + "') or " + "(to_date between '" + from_date.ToString(api_constants.sql_datetime_format) + "'  and  '" + to_date.ToString(api_constants.sql_datetime_format) + "') or " + "('" + from_date.ToString(api_constants.sql_datetime_format) + "' between from_date and to_date) or " + "('" + to_date.ToString(api_constants.sql_datetime_format) + "' between from_date and to_date) )";
      int bookedQuantity = 0;
      if (this.db.get_dataset(Sql))
      {
        Dictionary<DateTime, int> dictionary = new Dictionary<DateTime, int>();
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        {
          DateTime key = Convert.ToDateTime(row[nameof (from_date)]);
          for (DateTime dateTime = Convert.ToDateTime(row[nameof (to_date)]); key < dateTime; key = key.AddMinutes(15.0))
          {
            if (!dictionary.ContainsKey(key))
              dictionary.Add(key, Convert.ToInt32(row["accepted_qty"]));
            else
              dictionary[key] += Convert.ToInt32(row["accepted_qty"]);
          }
        }
        foreach (DateTime key in dictionary.Keys)
        {
          if (dictionary[key] > bookedQuantity)
            bookedQuantity = dictionary[key];
        }
      }
      return (double) bookedQuantity;
    }

    public Dictionary<DateTime, int> get_booked_quantity_distribution(
      long resource_id,
      Guid account_id,
      DateTime from_date,
      DateTime to_date,
      string modulename)
    {
      from_date = from_date.AddSeconds(1.0);
      to_date = to_date.AddSeconds(-1.0);
      Dictionary<DateTime, int> quantityDistribution = new Dictionary<DateTime, int>();
      string Sql = "select from_date, to_date, accepted_qty from vw_sbt_modules_resource_bookings_items where status = 1 and module_name = '" + modulename + "' and resource_id ='" + (object) resource_id + "'and account_id ='" + (object) account_id + "' and" + "(" + "(from_date between '" + from_date.ToString(api_constants.sql_datetime_format) + "' and  '" + to_date.ToString(api_constants.sql_datetime_format) + "') or " + "(to_date between '" + from_date.ToString(api_constants.sql_datetime_format) + "'  and  '" + to_date.ToString(api_constants.sql_datetime_format) + "') or " + "('" + from_date.ToString(api_constants.sql_datetime_format) + "' between from_date and to_date) or " + "('" + to_date.ToString(api_constants.sql_datetime_format) + "' between from_date and to_date) )";
      int num = 0;
      if (this.db.get_dataset(Sql))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        {
          DateTime key = Convert.ToDateTime(row[nameof (from_date)]);
          for (DateTime dateTime = Convert.ToDateTime(row[nameof (to_date)]); key < dateTime; key = key.AddMinutes(15.0))
          {
            if (!quantityDistribution.ContainsKey(key))
              quantityDistribution.Add(key, Convert.ToInt32(row["accepted_qty"]));
            else
              quantityDistribution[key] += Convert.ToInt32(row["accepted_qty"]);
          }
        }
        foreach (DateTime key in quantityDistribution.Keys)
        {
          if (quantityDistribution[key] > num)
            num = quantityDistribution[key];
        }
      }
      return quantityDistribution;
    }

    public additional_resource update_resource(additional_resource obj)
    {
      try
      {
        obj.item_id = !this.db.execute_procedure("sbt_sp_resource_update", new Dictionary<string, object>()
        {
          {
            "@item_id",
            (object) obj.item_id
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
            "@status",
            (object) obj.status
          },
          {
            "@name",
            (object) obj.name
          },
          {
            "@quantity",
            (object) obj.quantity
          },
          {
            "@record_id",
            (object) obj.record_id
          },
          {
            "@module",
            (object) obj.module_name
          },
          {
            "@price",
            (object) obj.price
          },
          {
            "@description",
            (object) obj.description
          },
          {
            "@unit_of_measure",
            (object) obj.unit_of_measure
          },
          {
            "@item_type_id",
            (object) obj.item_type_id
          },
          {
            "@owner_group_id",
            (object) obj.owner_group_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return obj;
    }

    public DataSet get_resource_types(Guid account_id, string modulename)
    {
      DataSet resourceTypes = new DataSet();
      string Sql = "select a.setting_id,a.value,a.modified_on,a.created_on,a.status,(select dbo.sbt_fn_user_name(a.modified_by,a.account_id)) as modified_name,(select dbo.sbt_fn_user_name(a.created_by,a.account_id)) as created_name,(select count(b.item_id) from sbt_modules_resource_items b  where b.account_id='" + (object) account_id + "' and b.item_type_id=a.setting_id) as item_count from sbt_modules_resource_settings a where a.account_id='" + (object) account_id + "' and a.parameter='resource_type' and a.module_name='" + modulename + "'  order by a.value";
      try
      {
        resourceTypes = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceTypes;
    }

    public DataSet get_resource_item_list(Guid account_id, string modulename)
    {
      DataSet resourceItemList = new DataSet();
      string Sql = "select (select value from sbt_modules_resource_settings where setting_id=a.item_type_id) as type_name,a.unit_of_measure,a.item_id,a.name,a.created_on,a.modified_on,(select dbo.sbt_fn_user_name(a.modified_by,a.account_id)) as modified_name,(select dbo.sbt_fn_user_name(a.created_by,a.account_id)) as created_name,a.status,(select dbo.sbt_fn_group_name(a.owner_group_id,a.account_id)) as group_name,a.quantity,a.description from sbt_modules_resource_items a where a.account_id='" + (object) account_id + "' and a.module_name='" + modulename + "' ";
      try
      {
        resourceItemList = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceItemList;
    }

    public DataSet get_export_resource_item_list(Guid account_id, string modulename)
    {
      DataSet resourceItemList = new DataSet();
      string Sql = "select (select value from sbt_modules_resource_settings where setting_id=a.item_type_id) as type_name,a.unit_of_measure,a.item_id,a.name,a.created_on,a.modified_on,(select dbo.sbt_fn_user_name(a.modified_by,a.account_id)) as modified_name,(select dbo.sbt_fn_user_name(a.created_by,a.account_id)) as created_name,a.status,(select dbo.sbt_fn_group_name(a.owner_group_id,a.account_id)) as group_name,a.quantity,a.description from sbt_modules_resource_items a where a.account_id='" + (object) account_id + "' and a.module_name='" + modulename + "' and status <> -1";
      try
      {
        resourceItemList = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceItemList;
    }

    public DataSet get_resource_types(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id)
    {
      DataSet resourceTypes = new DataSet();
      try
      {
        string str = "\tselect * from(SELECT  ROW_NUMBER() OVER (ORDER BY " + orderby + " " + orderdir + ") AS RowNumber,* FROM (SELECT A.setting_id, A.value,A.description," + " case when A.modified_on is null then A.created_on else A.modified_on end as modified_on,  " + " case when A.modified_by is null then A.created_by else A.modified_by end as modified_by, " + " (select full_name from sbt_users where user_id=(case when A.modified_by is null then A.created_by else A.modified_by end )) as modified_by_name,status" + " FROM sbt_modules_resource_settings AS A " + " WHERE parameter='resource_type' and status >=0 and  A.account_id='" + (object) account_id + "') AS b ";
        if (searchkey != "%")
          str = str + " WHERE ((b.value LIKE '%" + searchkey + "%') or (b.status like '%" + searchkey + "%') or (b.description like '%" + searchkey + "%'))";
        string Sql = str + " ) as sq  where RowNumber BETWEEN " + fromrow + " and " + endrow + ";" + " SELECT COUNT(*) AS RecordCnt FROM sbt_modules_resource_settings AS A WHERE parameter='resource_type' and status > 0 and  A.account_id='" + (object) account_id + "'";
        if (searchkey != "%")
          Sql = Sql + " and  ((A.value LIKE '%" + searchkey + "%') or (A.status like '%" + searchkey + "%') or (A.description like '%" + searchkey + "%'))";
        resourceTypes = !this.db.get_dataset(Sql) ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceTypes;
    }

    public DataSet get_resource_names(long type_id, Guid account_id)
    {
      DataSet resourceNames = new DataSet();
      try
      {
        string str = "\tselect item_id,name from sbt_modules_resource_items where account_id='" + (object) account_id + "' and status=1";
        if (type_id > 0L)
          str = str + " and item_type_id = " + (object) type_id;
        resourceNames = !this.db.get_dataset(str + " order by name asc") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return resourceNames;
    }

    public DataSet get_requests(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id)
    {
      DataSet requests = new DataSet();
      try
      {
        requests = !this.db.get_dataset("\tselect * from(select ROW_NUMBER() OVER (ORDER BY " + orderby + " " + orderdir + ") AS RowNumber, b.resource_booking_id,convert(varchar,created_on,106) as created_on,LTRIM(RIGHT(CONVERT(VARCHAR(20), from_date, 100), 7)) as from_date,LTRIM(RIGHT(CONVERT(VARCHAR(20), to_date, 100), 7)) as to_date,requested_by,purpose,venue,sq.requests from sbt_modules_resource_bookings as b left join (SELECT resource_booking_id ," + " STUFF((SELECT ', ' + convert(varchar,(select name from sbt_modules_resource_items where item_id = ri.resource_id)) + '(' + convert(varchar,ri.requested_qty) + ')' FROM sbt_modules_resource_bookings_items as ri WHERE b.resource_booking_id = ri.resource_booking_id" + " FOR XML PATH('')), 1, 1, '') requests FROM  [dbo].[sbt_modules_resource_bookings] as b GROUP BY b.resource_booking_id ) as sq on b.resource_booking_id = sq.resource_booking_id where b.account_id='" + (object) account_id + "' and " + " ((created_on like '%" + searchkey + "%') or (from_date like '%" + searchkey + "%') or (requests like '%" + searchkey + "%') or (to_date like '%" + searchkey + "%') or (requested_by like '%" + searchkey + "%') or (purpose like '%" + searchkey + "%') or (venue like '%" + searchkey + "%'))) as sq1 where RowNumber between " + fromrow + " and " + endrow + ";" + " select count(*) from(select b.resource_booking_id,created_on,from_date,to_date,requested_by,purpose,venue,sq.requests from sbt_modules_resource_bookings as b left join (SELECT   resource_booking_id ,STUFF((SELECT ', ' + convert(varchar,(select name from sbt_modules_resource_items where item_id = ri.resource_id)) + '(' + convert(varchar,ri.requested_qty) + ')'" + " FROM sbt_modules_resource_bookings_items as ri WHERE b.resource_booking_id = ri.resource_booking_id FOR XML PATH('')), 1, 1, '') requests FROM  [dbo].[sbt_modules_resource_bookings] as b GROUP BY b.resource_booking_id ) as sq on b.resource_booking_id = sq.resource_booking_id where b.account_id='" + (object) account_id + "' ) as sq1") ? (DataSet) null : this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) ex.ToString());
      }
      return requests;
    }

    public void update_resource_booking_date(
      long asset_booking_id,
      string fdate,
      string tdate,
      string modulename,
      long user_id)
    {
      this.db.execute_scalar("update sbt_modules_resource_bookings set from_date='" + fdate + "', to_date='" + tdate + "', modified_by=" + (object) user_id + ",  modified_on= GETUTCDATE() where  module_name = '" + modulename + "' and asset_booking_id=" + (object) asset_booking_id);
    }

    public DataSet get_resources_list(Guid account_id)
    {
      DataSet dataSet = new DataSet();
      return !this.db.get_dataset("select a.item_id,a.name,a.quantity,a.item_type_id,a.owner_group_id,a.unit_of_measure,b.value from sbt_modules_resource_items a,sbt_modules_resource_settings b where a.item_type_id=b.setting_id and b.module_name='resource_module' and a.status=1 and a.account_id='" + (object) account_id + "' order by b.value,a.name") ? (DataSet) null : this.db.resultDataSet;
    }

    public DataSet get_permissions(Guid account_id)
    {
      DataSet dataSet = new DataSet();
      return !this.db.get_dataset("select * from sbt_modules_resource_items_permissions where account_id='" + (object) account_id + "'") ? (DataSet) null : this.db.resultDataSet;
    }

    public DataSet get_permissions(Guid account_id, long item_id)
    {
      DataSet dataSet = new DataSet();
      return !this.db.get_dataset("select * from sbt_modules_resource_items_permissions where account_id='" + (object) account_id + "' and item_id='" + (object) item_id + "'") ? (DataSet) null : this.db.resultDataSet;
    }

    public DataSet get_permissions_by_group(Guid account_id, long group_id)
    {
      DataSet dataSet = new DataSet();
      return !this.db.get_dataset("select * from sbt_modules_resource_items_permissions where account_id='" + (object) account_id + "' and group_id='" + (object) group_id + "'") ? (DataSet) null : this.db.resultDataSet;
    }

    public resource_permission get_permission(long permission_id, Guid account_id)
    {
      string str = "select * from sbt_modules_resource_items_permissions where resource_permission_id ='" + (object) permission_id + "' and account_id='" + (object) account_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      resource_permission permission = new resource_permission();
      permission.resource_permission_id = 0L;
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
              permission.resource_permission_id = (long) objArray[index1];
            }
            catch
            {
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              permission.item_id = (long) objArray[index2];
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
              permission.group_id = (long) objArray[index3];
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
              permission.can_book = (bool) objArray[index4];
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
              permission.account_id = (Guid) objArray[index5];
            }
            catch
            {
            }
          }
          int index6 = index5 + 1;
          if (this.is_valid(objArray[index6]))
          {
            try
            {
              permission.created_on = (DateTime) objArray[index6];
            }
            catch
            {
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              permission.created_by = (long) objArray[index7];
            }
            catch
            {
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              permission.modified_on = (DateTime) objArray[index8];
            }
            catch
            {
              permission.modified_on = permission.created_on;
            }
          }
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
          {
            try
            {
              permission.modified_by = (long) objArray[index9];
            }
            catch
            {
              permission.modified_by = permission.created_by;
            }
          }
          int num = index9 + 1;
        }
      }
      catch (Exception ex)
      {
        permission.item_id = 0L;
        this.log.Error((object) str, ex);
      }
      return permission;
    }

    public resource_permission update_resource_permission(resource_permission obj)
    {
      bool flag;
      try
      {
        flag = this.db.execute_procedure("sbt_sp_resource_permission_update", new Dictionary<string, object>()
        {
          {
            "@resource_permission_id",
            (object) obj.resource_permission_id
          },
          {
            "@item_id",
            (object) obj.item_id
          },
          {
            "@group_id",
            (object) obj.group_id
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
            "@can_book",
            (object) obj.can_book
          }
        });
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ("resource permission error:" + ex.ToString()));
      }
      obj.resource_permission_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public bool cancel_resource_booking(
      long resource_booking_id,
      long modified_by,
      Guid account_id)
    {
      bool flag;
      try
      {
        flag = this.db.execute_procedure("sbt_sp_resource_bookings_delete", new Dictionary<string, object>()
        {
          {
            "@resource_booking_id",
            (object) resource_booking_id
          },
          {
            "@account_id",
            (object) account_id
          },
          {
            "@modified_by",
            (object) modified_by
          }
        });
      }
      catch (Exception ex)
      {
        flag = false;
        this.log.Error((object) ("resource permission error:" + ex.ToString()));
      }
      return flag;
    }
  }
}
