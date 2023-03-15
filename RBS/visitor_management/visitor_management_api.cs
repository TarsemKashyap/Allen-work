// Decompiled with JetBrains decompiler
// Type: visitor_management.visitor_management_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.core;
using System;
using System.Collections.Generic;
using System.Data;

namespace visitor_management
{
  public class visitor_management_api : core_api
  {
    public void blacklist_visitor(long visitor_id, bool blacklist, Guid account_id)
    {
      if (blacklist)
        this.db.execute_scalar("update sbt_vms_visitor set status=-1,is_banned=1 where visitor_id='" + (object) visitor_id + "'");
      else
        this.db.execute_scalar("update sbt_vms_visitor set status=1,is_banned=0 where visitor_id='" + (object) visitor_id + "'");
    }

    public List<visitor> search_visitors(string keyword, Guid account_id)
    {
      List<visitor> visitorList = new List<visitor>();
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' and UPPER(full_name) like '%" + keyword + "%' or UPPER(company_name) like '%" + keyword + "%' or UPPER(identification) like '%" + keyword + "%' or mobile  like '%" + keyword + "%' or telephone  like '%" + keyword + "%' or email  like '%" + keyword + "%' or company_address_1 like '%" + keyword + "%' or company_address_2 like '%" + keyword + "%' or department like '%" + keyword + "%' or designation like '%" + keyword + "%' or division like '%" + keyword + "%' or vehicle_number like '%" + keyword + "%' order by full_name");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          visitor visitorObject = this.get_visitor_object(dataObjects[key]);
          visitorList.Add(visitorObject);
        }
      }
      return visitorList;
    }

    public List<visitor> search_visitors(
      string keyword,
      long type,
      long category,
      long status,
      Guid account_id)
    {
      List<visitor> visitorList = new List<visitor>();
      string str = "select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' ";
      if (keyword != "")
        str = str + " and UPPER(full_name) like '%" + keyword + "%' or UPPER(company_name) like '%" + keyword + "%' or UPPER(identification) like '%" + keyword + "%' or mobile  like '%" + keyword + "%' or telephone  like '%" + keyword + "%' or email  like '%" + keyword + "%' or company_address_1 like '%" + keyword + "%' or company_address_2 like '%" + keyword + "%' or department like '%" + keyword + "%' or designation like '%" + keyword + "%' or division like '%" + keyword + "%' or vehicle_number like '%" + keyword + "%' ";
      if (type > 0L)
        str = str + " and visitor_type='" + (object) type + "' ";
      if (category > 0L)
        str = str + " and visitor_category='" + (object) category + "' ";
      if (status < 9L)
        str = str + " and status='" + (object) status + "' ";
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str + " order by full_name");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          visitor visitorObject = this.get_visitor_object(dataObjects[key]);
          visitorList.Add(visitorObject);
        }
      }
      return visitorList;
    }

    public visitor get_visitor(long visitor_id, Guid account_id)
    {
      visitor visitor = new visitor();
      visitor.visitor_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' and visitor_id='" + (object) visitor_id + "'");
      if (dataObjects.Count > 0)
        visitor = this.get_visitor_object(dataObjects[0]);
      return visitor;
    }

    public visitor get_visitor(string id, Guid account_id)
    {
      visitor visitor = new visitor();
      visitor.visitor_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' and lower(identification)='" + id.ToLower() + "'");
      if (dataObjects.Count > 0)
        visitor = this.get_visitor_object(dataObjects[0]);
      return visitor;
    }

    public visitor get_visitor(string parameter, string value, Guid account_id)
    {
      visitor visitor = new visitor();
      visitor.visitor_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' and " + parameter + "='" + value + "'");
      if (dataObjects.Count > 0)
        visitor = this.get_visitor_object(dataObjects[0]);
      return visitor;
    }

    public visitor get_visitor_by_id(string value, Guid account_id)
    {
      visitor visitorById = new visitor();
      visitorById.visitor_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' and UPPER(identification)='" + value.ToUpper() + "'");
      if (dataObjects.Count > 0)
        visitorById = this.get_visitor_object(dataObjects[0]);
      return visitorById;
    }

    public List<visitor> get_visitors(Guid account_id)
    {
      List<visitor> visitors = new List<visitor>();
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_visitors where account_id='" + (object) account_id + "' order by full_name");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          visitor visitorObject = this.get_visitor_object(dataObjects[key]);
          visitors.Add(visitorObject);
        }
      }
      return visitors;
    }

    private visitor get_visitor_object(object[] value)
    {
      visitor visitorObject = new visitor();
      visitorObject.visitor_id = 0L;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            visitorObject.visitor_id = (long) value[index1];
          }
          catch
          {
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            visitorObject.title = (string) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            visitorObject.full_name = (string) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            visitorObject.email = (string) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            visitorObject.telephone = (string) value[index5];
          }
          catch
          {
            visitorObject.telephone = "";
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            visitorObject.mobile = (string) value[index6];
          }
          catch
          {
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            visitorObject.identification = (string) value[index7];
          }
          catch
          {
          }
        }
        int index8 = index7 + 1;
        if (this.is_valid(value[index8]))
        {
          try
          {
            visitorObject.company_name = (string) value[index8];
          }
          catch
          {
          }
        }
        int index9 = index8 + 1;
        if (this.is_valid(value[index9]))
        {
          try
          {
            visitorObject.company_address_1 = (string) value[index9];
          }
          catch
          {
          }
        }
        int index10 = index9 + 1;
        if (this.is_valid(value[index10]))
        {
          try
          {
            visitorObject.company_address_2 = (string) value[index10];
          }
          catch
          {
          }
        }
        int index11 = index10 + 1;
        if (this.is_valid(value[index11]))
        {
          try
          {
            visitorObject.city = (string) value[index11];
          }
          catch
          {
          }
        }
        int index12 = index11 + 1;
        if (this.is_valid(value[index12]))
        {
          try
          {
            visitorObject.country = (string) value[index12];
          }
          catch
          {
          }
        }
        int index13 = index12 + 1;
        if (this.is_valid(value[index13]))
        {
          try
          {
            visitorObject.division = (string) value[index13];
          }
          catch
          {
          }
        }
        int index14 = index13 + 1;
        if (this.is_valid(value[index14]))
        {
          try
          {
            visitorObject.department = (string) value[index14];
          }
          catch
          {
          }
        }
        int index15 = index14 + 1;
        if (this.is_valid(value[index15]))
        {
          try
          {
            visitorObject.designation = (string) value[index15];
          }
          catch
          {
          }
        }
        int index16 = index15 + 1;
        if (this.is_valid(value[index16]))
        {
          try
          {
            visitorObject.vehicle_number = (string) value[index16];
          }
          catch
          {
          }
        }
        int index17 = index16 + 1;
        if (this.is_valid(value[index17]))
        {
          try
          {
            visitorObject.account_id = (Guid) value[index17];
          }
          catch
          {
          }
        }
        int index18 = index17 + 1;
        if (this.is_valid(value[index18]))
        {
          try
          {
            visitorObject.created_on = (DateTime) value[index18];
          }
          catch
          {
          }
        }
        int index19 = index18 + 1;
        if (this.is_valid(value[index19]))
        {
          try
          {
            visitorObject.created_by = (long) value[index19];
          }
          catch
          {
          }
        }
        int index20 = index19 + 1;
        if (this.is_valid(value[index20]))
        {
          try
          {
            visitorObject.modified_on = (DateTime) value[index20];
          }
          catch
          {
            visitorObject.modified_on = visitorObject.created_on;
          }
        }
        int index21 = index20 + 1;
        if (this.is_valid(value[index21]))
        {
          try
          {
            visitorObject.modified_by = (long) value[index21];
          }
          catch
          {
            visitorObject.modified_by = visitorObject.created_by;
          }
        }
        int index22 = index21 + 1;
        if (this.is_valid(value[index22]))
        {
          try
          {
            visitorObject.status = (long) value[index22];
          }
          catch
          {
          }
        }
        int index23 = index22 + 1;
        if (this.is_valid(value[index23]))
        {
          try
          {
            visitorObject.is_banned = (bool) value[index23];
          }
          catch
          {
          }
        }
        int index24 = index23 + 1;
        if (this.is_valid(value[index24]))
        {
          try
          {
            visitorObject.banned_reason = (string) value[index24];
          }
          catch
          {
          }
        }
        int index25 = index24 + 1;
        if (this.is_valid(value[index25]))
        {
          try
          {
            visitorObject.is_regular = (bool) value[index25];
          }
          catch
          {
          }
        }
        int index26 = index25 + 1;
        if (this.is_valid(value[index26]))
        {
          try
          {
            visitorObject.visitor_type = (long) value[index26];
          }
          catch
          {
          }
        }
        int index27 = index26 + 1;
        if (this.is_valid(value[index27]))
        {
          try
          {
            visitorObject.visitor_category = (long) value[index27];
          }
          catch
          {
          }
        }
        int index28 = index27 + 1;
        if (this.is_valid(value[index28]))
        {
          try
          {
            visitorObject.is_special = (bool) value[index28];
          }
          catch
          {
          }
        }
        int index29 = index28 + 1;
        if (this.is_valid(value[index29]))
        {
          try
          {
            visitorObject.is_pre_registered = (bool) value[index29];
          }
          catch
          {
          }
        }
        int index30 = index29 + 1;
        if (this.is_valid(value[index30]))
        {
          try
          {
            visitorObject.pre_registered_from = (DateTime) value[index30];
          }
          catch
          {
          }
        }
        int index31 = index30 + 1;
        if (this.is_valid(value[index31]))
        {
          try
          {
            visitorObject.pre_registered_to = (DateTime) value[index31];
          }
          catch
          {
          }
        }
        int num = index31 + 1;
        if (this.is_valid(value[31]))
          visitorObject._visitor_type = new item()
          {
            id = visitorObject.visitor_type,
            value = (string) value[31]
          };
        if (this.is_valid(value[32]))
          visitorObject._visitor_category = new item()
          {
            id = visitorObject.visitor_category,
            value = (string) value[32]
          };
        visitorObject._status = new item()
        {
          id = visitorObject.status,
          value = this.get_visitor_status(visitorObject.status)
        };
      }
      catch
      {
        visitorObject.visitor_id = 0L;
      }
      return visitorObject;
    }

    private string get_visitor_status(long status_id)
    {
      string visitorStatus = "";
      switch (status_id)
      {
        case -1:
          visitorStatus = "Blacklisted";
          break;
        case 0:
          visitorStatus = "Inactive";
          break;
        case 1:
          visitorStatus = "Active";
          break;
      }
      return visitorStatus;
    }

    public visitor update_visitor(visitor obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visitor_update", new Dictionary<string, object>()
      {
        {
          "@visitor_id",
          (object) obj.visitor_id
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@created_on",
          (object) obj.created_on
        },
        {
          "@created_by",
          (object) obj.created_by
        },
        {
          "@modified_on",
          (object) obj.modified_on
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
          "@city",
          (object) obj.city
        },
        {
          "@company_address_1",
          (object) obj.company_address_1
        },
        {
          "@company_address_2",
          (object) obj.company_address_2
        },
        {
          "@country",
          (object) obj.country
        },
        {
          "@division",
          (object) obj.division
        },
        {
          "@department",
          (object) obj.department
        },
        {
          "@designation",
          (object) obj.designation
        },
        {
          "@vehicle_number",
          (object) obj.vehicle_number
        },
        {
          "@is_banned",
          (object) obj.is_banned
        },
        {
          "@banned_reason",
          (object) obj.banned_reason
        },
        {
          "@is_regular",
          (object) obj.is_regular
        },
        {
          "@visitor_type",
          (object) obj.visitor_type
        },
        {
          "@visitor_category",
          (object) obj.visitor_category
        },
        {
          "@is_special",
          (object) obj.is_special
        },
        {
          "@is_pre_registered",
          (object) obj.is_pre_registered
        },
        {
          "@pre_registered_from",
          (object) obj.pre_registered_from
        },
        {
          "@pre_registered_to",
          (object) obj.pre_registered_to
        },
        {
          "@email",
          (object) obj.email
        },
        {
          "@telephone",
          (object) obj.telephone
        },
        {
          "@mobile",
          (object) obj.mobile
        },
        {
          "@identification",
          (object) obj.identification
        },
        {
          "@company_name",
          (object) obj.company_name
        },
        {
          "@title",
          (object) obj.title
        },
        {
          "@full_name",
          (object) obj.full_name
        }
      }))
      {
        obj.visitor_id = Convert.ToInt64(this.db.resultString);
        return obj;
      }
      obj.visitor_id = 0L;
      return obj;
    }

    public visitor delete_visitor(visitor obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visitor_delete", new Dictionary<string, object>()
      {
        {
          "@visitor_id",
          (object) obj.visitor_id
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@modified_by",
          (object) obj.modified_by
        }
      }))
        return obj;
      obj.visitor_id = 0L;
      return obj;
    }

    public visit_register delete_visitor_register(visit_register obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visit_register_delete", new Dictionary<string, object>()
      {
        {
          "@register_id",
          (object) obj.register_id
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@modified_by",
          (object) obj.modified_by
        }
      }))
        return obj;
      obj.visitor_id = 0L;
      return obj;
    }

    public DataSet view_visitors(long register_id, Guid account_id) => this.db.get_dataset("select visitor_id from sbt_vms_visit_register where account_id='" + (object) account_id + "' and registration_code in (select registration_code from sbt_vms_visit_register where register_id='" + (object) register_id + "')") ? this.db.resultDataSet : (DataSet) null;

    public visitor_property get_visitor_property(long property_id, Guid account_id)
    {
      visitor_property visitorProperty = new visitor_property();
      visitorProperty.visitor_property_id = 0L;
      string Sql = "select * from sbt_vms_visitor_properties where account_id='" + (object) account_id + "' and visitor_property_id='" + (object) property_id + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(Sql);
        if (dataObjects.Count > 0)
          visitorProperty = this.get_visitor_property_object(dataObjects[0]);
      }
      catch
      {
        visitorProperty.visitor_property_id = 0L;
      }
      return visitorProperty;
    }

    public List<visitor_property> get_visitor_properties(long visitor_id, Guid account_id)
    {
      List<visitor_property> visitorProperties = new List<visitor_property>();
      visitor_property visitorProperty = new visitor_property();
      visitorProperty.visitor_property_id = 0L;
      string Sql = "select * from sbt_vms_visitor_properties where account_id='" + (object) account_id + "' and visitor_id='" + (object) visitor_id + "'";
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(Sql);
        if (dataObjects.Count > 0)
        {
          foreach (int key in dataObjects.Keys)
          {
            visitorProperty = this.get_visitor_property_object(dataObjects[key]);
            visitorProperties.Add(visitorProperty);
          }
        }
      }
      catch
      {
        visitorProperty.visitor_property_id = 0L;
      }
      return visitorProperties;
    }

    private visitor_property get_visitor_property_object(object[] value)
    {
      visitor_property visitorPropertyObject = new visitor_property();
      visitorPropertyObject.visitor_property_id = 0L;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            visitorPropertyObject.visitor_property_id = (long) value[index1];
          }
          catch
          {
            visitorPropertyObject.visitor_property_id = 0L;
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            visitorPropertyObject.visitor_id = (long) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            visitorPropertyObject.property_name = (string) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            visitorPropertyObject.property_value = (string) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            visitorPropertyObject.available = (bool) value[index5];
          }
          catch
          {
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            visitorPropertyObject.remarks = (string) value[index6];
          }
          catch
          {
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            visitorPropertyObject.account_id = (Guid) value[index7];
          }
          catch
          {
          }
        }
        int index8 = index7 + 1;
        if (this.is_valid(value[index8]))
        {
          try
          {
            visitorPropertyObject.created_on = (DateTime) value[index8];
          }
          catch
          {
          }
        }
        int index9 = index8 + 1;
        if (this.is_valid(value[index9]))
        {
          try
          {
            visitorPropertyObject.created_by = (long) value[index9];
          }
          catch
          {
          }
        }
        int index10 = index9 + 1;
        if (this.is_valid(value[index10]))
        {
          try
          {
            visitorPropertyObject.modified_on = (DateTime) value[index10];
          }
          catch
          {
            visitorPropertyObject.modified_on = visitorPropertyObject.created_on;
          }
        }
        int index11 = index10 + 1;
        if (this.is_valid(value[index11]))
        {
          try
          {
            visitorPropertyObject.modified_by = (long) value[index11];
          }
          catch
          {
            visitorPropertyObject.modified_by = visitorPropertyObject.created_by;
          }
        }
        int index12 = index11 + 1;
        if (this.is_valid(value[index12]))
        {
          try
          {
            visitorPropertyObject.status = (long) value[index12];
          }
          catch
          {
          }
        }
        int num = index12 + 1;
      }
      catch
      {
        visitorPropertyObject.visitor_property_id = 0L;
      }
      return visitorPropertyObject;
    }

    public visitor_property update_visitor_property(visitor_property obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visitor_property_update", new Dictionary<string, object>()
      {
        {
          "@visitor_property_id",
          (object) obj.visitor_property_id
        },
        {
          "@visitor_id",
          (object) obj.visitor_id
        },
        {
          "@property_name",
          (object) obj.property_name
        },
        {
          "@property_value",
          (object) obj.property_value
        },
        {
          "@available",
          (object) obj.available
        },
        {
          "@remarks",
          (object) obj.remarks
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@created_on",
          (object) obj.created_on
        },
        {
          "@created_by",
          (object) obj.created_by
        },
        {
          "@modified_on",
          (object) obj.modified_on
        },
        {
          "@modified_by",
          (object) obj.modified_by
        },
        {
          "@status",
          (object) obj.status
        }
      }))
        return obj;
      obj.visitor_property_id = 0L;
      return obj;
    }

    public visitor_property delete_visitor_property(visitor_property obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visitor_property_delete", new Dictionary<string, object>()
      {
        {
          "@visitor_property_id",
          (object) obj.visitor_property_id
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@modified_by",
          (object) obj.modified_by
        }
      }))
        return obj;
      obj.visitor_property_id = 0L;
      return obj;
    }

    public List<visit> search_visits(string purpose, string notes, Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string str = "select * from sbt_vms_visit where account_id='" + (object) account_id + "' ";
      if (purpose != "")
        str = str + " and UPPER(purpose) like '%" + purpose.ToUpper() + "%'";
      if (notes != "")
        str = str + " and UPPER(notes) like '%" + notes.ToUpper() + "%'";
      string Sql = str + " order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    public List<visit> get_visits_by_card(long card_id, Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and card_id='" + (object) card_id + "' order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    public List<visit> get_visits_by_card(
      long card_id,
      DateTime from,
      DateTime to,
      Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and card_id='" + (object) card_id + "' and time_in between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "'  order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    public List<visit> get_visits_by_visitor(long visitor_id, Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and visitor_id='" + (object) visitor_id + "' order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    public List<visit> get_visits_by_visitor(
      long visitor_id,
      DateTime from,
      DateTime to,
      Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and visitor_id='" + (object) visitor_id + "' and time_in between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "' order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    public List<visit> get_visits(DateTime from, DateTime to, Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and (time_in between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    private List<visit> get_list(Dictionary<int, object[]> items)
    {
      List<visit> list = new List<visit>();
      if (items.Count > 0)
      {
        visit visit = new visit();
        foreach (int key in items.Keys)
        {
          visit visitObject = this.get_visit_object(items[key]);
          list.Add(visitObject);
        }
      }
      return list;
    }

    public List<visit> get_visits(DateTime from, DateTime to, long card_id, Guid account_id)
    {
      List<visit> visitList = new List<visit>();
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and card_id='" + (object) card_id + "' and (time_in between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "') order by time_in desc";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      return this.get_list(this.db.get_data_objects(Sql));
    }

    public visit get_visit(long visit_id, Guid account_id)
    {
      visit visit = new visit();
      visit.visit_id = 0L;
      string Sql = "select * from sbt_vw_vms_visits where account_id='" + (object) account_id + "' and visit_id='" + (object) visit_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects(Sql);
      if (dataObjects.Count > 0)
        visit = this.get_visit_object(dataObjects[0]);
      return visit;
    }

    private visit get_visit_object(object[] value)
    {
      visit visitObject = new visit();
      visitObject.visit_id = 0L;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            visitObject.visit_id = (long) value[index1];
          }
          catch
          {
            visitObject.visit_id = 0L;
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            visitObject.visitor_id = (long) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            visitObject.card_id = (long) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            visitObject.purpose = (string) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            visitObject.time_in = (DateTime) value[index5];
          }
          catch
          {
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            visitObject.time_out = (DateTime) value[index6];
          }
          catch
          {
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            visitObject.notes = (string) value[index7];
          }
          catch
          {
          }
        }
        int index8 = index7 + 1;
        if (this.is_valid(value[index8]))
        {
          try
          {
            visitObject.account_id = (Guid) value[index8];
          }
          catch
          {
          }
        }
        int index9 = index8 + 1;
        if (this.is_valid(value[index9]))
        {
          try
          {
            visitObject.created_on = (DateTime) value[index9];
          }
          catch
          {
          }
        }
        int index10 = index9 + 1;
        if (this.is_valid(value[index10]))
        {
          try
          {
            visitObject.created_by = (long) value[index10];
          }
          catch
          {
          }
        }
        int index11 = index10 + 1;
        if (this.is_valid(value[index11]))
        {
          try
          {
            visitObject.modified_on = (DateTime) value[index11];
          }
          catch
          {
            visitObject.modified_on = visitObject.created_on;
          }
        }
        int index12 = index11 + 1;
        if (this.is_valid(value[index12]))
        {
          try
          {
            visitObject.modified_by = (long) value[index12];
          }
          catch
          {
            visitObject.modified_by = visitObject.created_by;
          }
        }
        int index13 = index12 + 1;
        if (this.is_valid(value[index13]))
        {
          try
          {
            visitObject.status = (long) value[index13];
          }
          catch
          {
          }
        }
        int index14 = index13 + 1;
        if (this.is_valid(value[index14]))
        {
          try
          {
            visitObject.card_number = (string) value[index14];
          }
          catch
          {
          }
        }
        int index15 = index14 + 1;
        if (this.is_valid(value[index15]))
        {
          try
          {
            visitObject.purpose_id = (long) value[index15];
          }
          catch
          {
          }
        }
        int index16 = index15 + 1;
        if (this.is_valid(value[index16]))
        {
          try
          {
            visitObject.register_id = (long) value[index16];
          }
          catch
          {
          }
        }
        int index17 = index16 + 1;
        if (this.is_valid(value[index17]))
        {
          try
          {
            visitObject.escort = (string) value[21];
          }
          catch
          {
          }
        }
        int num = index17 + 1;
        visitObject._card = new item();
        visitObject._card.id = visitObject.card_id;
        visitObject._card.value = (string) value[16];
        visitObject._visitor = new visitor();
        visitObject._visitor = this.get_visitor_by_id(visitObject.visitor_id.ToString(), visitObject.account_id);
        visitObject.visitor_name = (string) value[17];
        visitObject.visitor_company = (string) value[18];
        visitObject.visitor_identification = (string) value[19];
      }
      catch
      {
        visitObject.visit_id = 0L;
      }
      visitObject._purpose = new item()
      {
        id = visitObject.purpose_id,
        value = (string) value[15]
      };
      return visitObject;
    }

    public visit update_visit(visit obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visit_update", new Dictionary<string, object>()
      {
        {
          "@visit_id",
          (object) obj.visit_id
        },
        {
          "@visitor_id",
          (object) obj.visitor_id
        },
        {
          "@card_id",
          (object) obj.card_id
        },
        {
          "@purpose",
          (object) obj.purpose
        },
        {
          "@time_in",
          (object) obj.time_in
        },
        {
          "@time_out",
          (object) obj.time_out
        },
        {
          "@notes",
          (object) obj.notes
        },
        {
          "@card_number",
          (object) obj.card_number
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@register_id",
          (object) obj.register_id
        },
        {
          "@created_on",
          (object) obj.created_on
        },
        {
          "@created_by",
          (object) obj.created_by
        },
        {
          "@modified_on",
          (object) obj.modified_on
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
          "@escort",
          (object) obj.escort
        },
        {
          "@purpose_id",
          (object) obj.purpose_id
        }
      }))
      {
        obj.visit_id = Convert.ToInt64(this.db.resultString);
        return obj;
      }
      obj.card_id = 0L;
      return obj;
    }

    public bool scan_out(long card_id, Guid account_id)
    {
      try
      {
        this.db.execute_scalar("update sbt_vms_visit set status=1, time_out=CURRENT_TIMESTAMP where account_id='" + (object) account_id + "' and card_id='" + (object) card_id + "' and status=0");
      }
      catch
      {
        return false;
      }
      return true;
    }

    public visit delete_visit(visit obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visit_delete", new Dictionary<string, object>()
      {
        {
          "@card_id",
          (object) obj.card_id
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
          "@status",
          (object) obj.status
        }
      }))
        return obj;
      obj.card_id = 0L;
      return obj;
    }

    public List<card> get_cards_by_category(long type, Guid account_id)
    {
      List<card> cardList = new List<card>();
      List<card> cards = this.get_cards(account_id);
      foreach (card card in cards)
      {
        if (card.card_type != type)
          cards.Remove(card);
      }
      return cards;
    }

    public List<card> get_cards_by_type(long category, Guid account_id)
    {
      List<card> cardList = new List<card>();
      List<card> cards = this.get_cards(account_id);
      foreach (card card in cards)
      {
        if (card.card_category != category)
          cards.Remove(card);
      }
      return cards;
    }

    public List<card> get_cards(Guid account_id)
    {
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_cards where account_id='" + (object) account_id + "' order by card_no");
      List<card> cards = new List<card>();
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          card cardObject = this.get_card_object(dataObjects[key]);
          cards.Add(cardObject);
        }
      }
      return cards;
    }

    public card get_card(long card_id, Guid account_id)
    {
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_cards where account_id='" + (object) account_id + "' and card_id='" + (object) card_id + "'");
      card card = new card();
      card.card_id = 0L;
      if (dataObjects.Count == 1)
        card = this.get_card_object(dataObjects[0]);
      return card;
    }

    public card get_card_by_card_number(string card_no, Guid account_id)
    {
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vw_vms_cards where account_id='" + (object) account_id + "' and UPPER(card_no)='" + card_no.ToUpper() + "'");
      card cardByCardNumber = new card();
      cardByCardNumber.card_id = 0L;
      if (dataObjects.Count == 1)
        cardByCardNumber = this.get_card_object(dataObjects[0]);
      return cardByCardNumber;
    }

    private card get_card_object(object[] value)
    {
      card cardObject = new card();
      cardObject.card_id = 0L;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            cardObject.card_id = (long) value[index1];
          }
          catch
          {
            cardObject.card_id = 0L;
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            cardObject.card_no = (string) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            cardObject.description = (string) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            cardObject.account_id = (Guid) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            cardObject.created_on = (DateTime) value[index5];
          }
          catch
          {
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            cardObject.created_by = (long) value[index6];
          }
          catch
          {
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            cardObject.modified_on = (DateTime) value[index7];
          }
          catch
          {
            cardObject.modified_on = cardObject.created_on;
          }
        }
        int index8 = index7 + 1;
        if (this.is_valid(value[index8]))
        {
          try
          {
            cardObject.modified_by = (long) value[index8];
          }
          catch
          {
            cardObject.modified_by = cardObject.created_by;
          }
        }
        int index9 = index8 + 1;
        if (this.is_valid(value[index9]))
        {
          try
          {
            cardObject.status = (long) value[index9];
          }
          catch
          {
          }
        }
        int index10 = index9 + 1;
        if (this.is_valid(value[index10]))
        {
          try
          {
            cardObject.barcode_location = (string) value[index10];
          }
          catch
          {
          }
        }
        int index11 = index10 + 1;
        if (this.is_valid(value[index11]))
        {
          try
          {
            cardObject.qrcode_location = (string) value[index11];
          }
          catch
          {
          }
        }
        int index12 = index11 + 1;
        if (this.is_valid(value[index12]))
        {
          try
          {
            cardObject.card_type = (long) value[index12];
          }
          catch
          {
          }
        }
        int num = index12 + 1;
        cardObject._card_type = new item()
        {
          id = cardObject.card_type,
          value = (string) value[13]
        };
        cardObject._card_category = new item()
        {
          id = cardObject.card_category,
          value = (string) value[14]
        };
        item obj = new item() { id = cardObject.status };
        obj.value = this.card_status(obj.id);
        cardObject._status = obj;
      }
      catch (Exception ex)
      {
        cardObject.card_id = 0L;
        this.log.Error((object) ("ERR (get_card_object): " + ex.ToString()));
      }
      return cardObject;
    }

    public card update_card(card obj)
    {
      obj.card_id = !this.db.execute_procedure("sbt_sp_vms_card_update", new Dictionary<string, object>()
      {
        {
          "@card_id",
          (object) obj.card_id
        },
        {
          "@card_no",
          (object) obj.card_no
        },
        {
          "@description",
          (object) obj.description
        },
        {
          "@barcode_location",
          (object) obj.barcode_location
        },
        {
          "@qrcode_location",
          (object) obj.qrcode_location
        },
        {
          "@card_type",
          (object) obj.card_type
        },
        {
          "@card_category",
          (object) obj.card_category
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
        }
      }) ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public card delete_card(card obj)
    {
      obj.card_id = !this.db.execute_procedure("sbt_sp_vms_card_delete", new Dictionary<string, object>()
      {
        {
          "@card_id",
          (object) obj.card_id
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
      return obj;
    }

    public bool card_in_use(long card_id, Guid account_id)
    {
      bool flag = false;
      if (this.db.get_dataset("select visit_id from sbt_vms_visit where account_id='" + (object) account_id + "' and card_id='" + (object) card_id + "' and status=0") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
        flag = true;
      return flag;
    }

    public string card_status(long status_id)
    {
      switch (status_id)
      {
        case -1:
          return "Removed";
        case 0:
          return "Inactive";
        case 1:
          return "Active";
        default:
          return "";
      }
    }

    public List<item> get_items_by_parameter(string parameter, Guid account_id)
    {
      List<item> itemsByParameter = new List<item>();
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_settings where account_id='" + (object) account_id + "' and parameter='" + parameter + "' order by sort_order");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          new item().id = 0L;
          item itemObject = this.get_item_object(dataObjects[key]);
          itemsByParameter.Add(itemObject);
        }
      }
      return itemsByParameter;
    }

    public item get_item(long item_id, Guid account_id)
    {
      item obj = new item();
      obj.id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select setting_id,value from sbt_vms_settings where account_id='" + (object) account_id + "' and item_id='" + (object) item_id + "'");
      if (dataObjects.Count > 0)
        obj = this.get_item_object(dataObjects[0]);
      return obj;
    }

    private item get_item_object(object[] value)
    {
      item itemObject = new item();
      itemObject.id = 0L;
      int index1 = 0;
      if (this.is_valid(value[index1]))
      {
        try
        {
          itemObject.id = (long) value[index1];
        }
        catch
        {
        }
      }
      int index2 = index1 + 1;
      if (this.is_valid(value[index2]))
      {
        try
        {
          itemObject.value = (string) value[index2];
        }
        catch
        {
        }
      }
      int index3 = index2 + 1;
      if (this.is_valid(value[index3]))
      {
        try
        {
          itemObject.status = (long) (int) value[index3];
        }
        catch (Exception ex)
        {
        }
      }
      return itemObject;
    }

    public List<item> get_card_types(Guid account_id) => this.get_items_by_parameter("card_type", account_id);

    public List<item> get_card_categories(Guid account_id) => this.get_items_by_parameter("card_category", account_id);

    public List<item> get_card_status(Guid account_id) => new List<item>()
    {
      new item() { id = 0L, value = "Inactive" },
      new item() { id = 1L, value = "Active" },
      new item() { id = -1L, value = "Removed" }
    };

    public visit_register update_visit_register(visit_register obj)
    {
      if (this.db.execute_procedure("sbt_sp_vms_visit_register_update", new Dictionary<string, object>()
      {
        {
          "@register_id",
          (object) obj.register_id
        },
        {
          "@visitor_id",
          (object) obj.visitor_id
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
          "@escort_id",
          (object) obj.escort_id
        },
        {
          "@purpose",
          (object) obj.purpose
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@asset_booking_id",
          (object) obj.asset_booking_id
        },
        {
          "@created_on",
          (object) obj.created_on
        },
        {
          "@created_by",
          (object) obj.created_by
        },
        {
          "@modified_on",
          (object) obj.modified_on
        },
        {
          "@modified_by",
          (object) obj.modified_by
        },
        {
          "@all_day",
          (object) obj.all_day
        },
        {
          "@registration_code",
          (object) obj.registration_code
        },
        {
          "@sms",
          (object) obj.sms
        },
        {
          "@email",
          (object) obj.email
        },
        {
          "@record_id",
          (object) obj.record_id
        },
        {
          "@visit_type",
          (object) obj.visit_type
        },
        {
          "@purpose_id",
          (object) obj.purpose_id
        },
        {
          "@registration_status",
          (object) obj.registration_status
        },
        {
          "@visitor_status",
          (object) obj.visitor_status
        }
      }))
      {
        obj.register_id = Convert.ToInt64(this.db.resultString);
        return obj;
      }
      obj.register_id = 0L;
      return obj;
    }

    public visit_register get_visit_register(long register_id, Guid account_id)
    {
      visit_register visitRegister = new visit_register();
      visitRegister.register_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_visit_register where account_id='" + (object) account_id + "' and register_id='" + (object) register_id + "'");
      if (dataObjects.Count > 0)
        visitRegister = this.get_visit_register_object(dataObjects[0]);
      return visitRegister;
    }

    public List<visit_register> get_visit_register(Guid record_id, Guid account_id)
    {
      List<visit_register> visitRegister = new List<visit_register>();
      new visit_register().register_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_visit_register where account_id='" + (object) account_id + "' and record_id='" + (object) record_id + "'");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          visit_register visitRegisterObject = this.get_visit_register_object(dataObjects[key]);
          visitRegister.Add(visitRegisterObject);
        }
      }
      return visitRegister;
    }

    public List<visit_register> get_visit_register(
      DateTime from,
      DateTime to,
      long visitor_id,
      Guid account_id)
    {
      List<visit_register> visitRegister = new List<visit_register>();
      new visit_register().register_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_visit_register where account_id='" + (object) account_id + "' and visitor_id='" + (object) visitor_id + "' and from_date between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "'");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          visit_register visitRegisterObject = this.get_visit_register_object(dataObjects[key]);
          visitRegister.Add(visitRegisterObject);
        }
      }
      return visitRegister;
    }

    public List<visit_register> get_visit_register(DateTime from, DateTime to, Guid account_id)
    {
      List<visit_register> visitRegister = new List<visit_register>();
      new visit_register().register_id = 0L;
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_visit_register where account_id='" + (object) account_id + "' and from_date between '" + from.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + to.ToString("yyyy-MM-dd HH:mm:ss") + "'");
      if (dataObjects.Count > 0)
      {
        foreach (int key in dataObjects.Keys)
        {
          visit_register visitRegisterObject = this.get_visit_register_object(dataObjects[key]);
          visitRegister.Add(visitRegisterObject);
        }
      }
      return visitRegister;
    }

        public void cancel_visit(string id, Guid account_id) {
            //"update sbt_vms_visit_register set registration_status=0 where record_id='" + id + "' and account_id='" + (object)account_id + "'"
            throw new Exception();
                }

    public visit_register get_visit_register_object(object[] value)
    {
      visit_register visitRegisterObject = new visit_register();
      visitRegisterObject.register_id = 0L;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            visitRegisterObject.register_id = (long) value[index1];
          }
          catch
          {
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            visitRegisterObject.visitor_id = (long) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            visitRegisterObject.from_date = (DateTime) value[index3];
          }
          catch
          {
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            visitRegisterObject.to_date = (DateTime) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            visitRegisterObject.escort_id = (long) value[index5];
          }
          catch
          {
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            visitRegisterObject.purpose = (string) value[index6];
          }
          catch
          {
          }
        }
        int index7 = index6 + 1;
        if (this.is_valid(value[index7]))
        {
          try
          {
            visitRegisterObject.account_id = (Guid) value[index7];
          }
          catch
          {
          }
        }
        int index8 = index7 + 1;
        if (this.is_valid(value[index8]))
        {
          try
          {
            visitRegisterObject.created_on = (DateTime) value[index8];
          }
          catch
          {
          }
        }
        int index9 = index8 + 1;
        if (this.is_valid(value[index9]))
        {
          try
          {
            visitRegisterObject.created_by = (long) value[index9];
          }
          catch
          {
          }
        }
        int index10 = index9 + 1;
        if (this.is_valid(value[index10]))
        {
          try
          {
            visitRegisterObject.modified_on = (DateTime) value[index10];
          }
          catch
          {
            visitRegisterObject.modified_on = visitRegisterObject.created_on;
          }
        }
        int index11 = index10 + 1;
        if (this.is_valid(value[index11]))
        {
          try
          {
            visitRegisterObject.modified_by = (long) value[index11];
          }
          catch
          {
            visitRegisterObject.modified_by = visitRegisterObject.created_by;
          }
        }
        int index12 = index11 + 1;
        if (this.is_valid(value[index12]))
        {
          try
          {
            visitRegisterObject.all_day = (bool) value[index12];
          }
          catch
          {
          }
        }
        int index13 = index12 + 1;
        if (this.is_valid(value[index13]))
        {
          try
          {
            visitRegisterObject.sms = (bool) value[index13];
          }
          catch
          {
          }
        }
        int index14 = index13 + 1;
        if (this.is_valid(value[index14]))
        {
          try
          {
            visitRegisterObject.email = (bool) value[index14];
          }
          catch
          {
          }
        }
        int index15 = index14 + 1;
        if (this.is_valid(value[index15]))
        {
          try
          {
            visitRegisterObject.record_id = (Guid) value[index15];
          }
          catch
          {
          }
        }
        int index16 = index15 + 1;
        if (this.is_valid(value[index16]))
        {
          try
          {
            visitRegisterObject.visit_type = (long) value[index16];
          }
          catch
          {
          }
        }
        int index17 = index16 + 1;
        if (this.is_valid(value[index17]))
        {
          try
          {
            visitRegisterObject.asset_booking_id = (long) value[index17];
          }
          catch
          {
          }
        }
        int index18 = index17 + 1;
        if (this.is_valid(value[index18]))
        {
          try
          {
            visitRegisterObject.registration_code = (string) value[index18];
          }
          catch
          {
          }
        }
        int index19 = index18 + 1;
        if (this.is_valid(value[index19]))
        {
          try
          {
            visitRegisterObject.registration_status = (int) value[index19];
          }
          catch
          {
          }
        }
        int index20 = index19 + 1;
        if (this.is_valid(value[index20]))
        {
          try
          {
            visitRegisterObject.visitor_status = (int) value[index20];
          }
          catch
          {
          }
        }
        int index21 = index20 + 1;
        if (this.is_valid(value[index21]))
        {
          try
          {
            visitRegisterObject.invite_sent = (bool) value[index21];
          }
          catch
          {
          }
        }
        int index22 = index21 + 1;
        if (this.is_valid(value[index22]))
        {
          try
          {
            visitRegisterObject.purpose_id = (long) value[index22];
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        visitRegisterObject.visitor_id = 0L;
        this.log.Error((object) ("ERR (get_visit_register_object): " + ex.ToString()));
      }
      return visitRegisterObject;
    }

    public void update_alert_read(long user_id, Guid account_id)
    {
      string Sql = "select alert_id from sbt_alerts where account_id='" + (object) account_id + "' and alert_id not in (select alert_id from sbt_alerts_read where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "')";
      DataSet dataSet = new DataSet();
      if (!this.db.get_dataset(Sql))
        return;
      foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
        this.db.execute_scalar("insert into sbt_alerts_read (alert_id,user_id,account_id,read_on) values('" + row["alert_id"].ToString() + "','" + (object) user_id + "','" + (object) account_id + "',CURRENT_TIMESTAMP);");
    }

    public DataSet get_alerts(DateTime from, DateTime to, string text)
    {
      DataSet alerts = new DataSet();
      string str = "select * from sbt_alerts ";
      if (text != "")
        str = str + " where LOWER(message) like '%" + text.ToLower() + "%' ";
      if (this.db.get_dataset(str + " order by created_on desc"))
        alerts = this.db.resultDataSet;
      return alerts;
    }

    public alert update_alert(alert obj)
    {
      obj.alert_id = !this.db.execute_procedure("sbt_sp_alert_update", new Dictionary<string, object>()
      {
        {
          "@alert_id",
          (object) obj.alert_id
        },
        {
          "@message",
          (object) obj.message
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@created_by",
          (object) obj.created_by
        }
      }) ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public int get_unread_alerts(long user_id, Guid account_id) => (int) this.db.execute_scalar("select count(alert_id) as count from sbt_alerts where account_id='" + (object) account_id + "' and alert_id not in (select alert_id from sbt_alerts_read where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "')");

    public visit_print get_print(Guid account_id, long visit_id)
    {
      Dictionary<int, object[]> dataObjects = this.db.get_data_objects("select * from sbt_vms_visit_print where account_id='" + (object) account_id + "' and visit_id='" + (object) visit_id + "'");
      visit_print print = new visit_print();
      print.print_id = 0L;
      if (dataObjects.Count == 1)
        print = this.get_print_object(dataObjects[0]);
      return print;
    }

    private visit_print get_print_object(object[] value)
    {
      visit_print printObject = new visit_print();
      printObject.print_id = 0L;
      try
      {
        int index1 = 0;
        if (this.is_valid(value[index1]))
        {
          try
          {
            printObject.print_id = (long) value[index1];
          }
          catch
          {
            printObject.print_id = 0L;
          }
        }
        int index2 = index1 + 1;
        if (this.is_valid(value[index2]))
        {
          try
          {
            printObject.account_id = (Guid) value[index2];
          }
          catch
          {
          }
        }
        int index3 = index2 + 1;
        if (this.is_valid(value[index3]))
        {
          try
          {
            printObject.visit_id = (long) value[index3];
          }
          catch
          {
            printObject.visit_id = 0L;
          }
        }
        int index4 = index3 + 1;
        if (this.is_valid(value[index4]))
        {
          try
          {
            printObject.created_on = (DateTime) value[index4];
          }
          catch
          {
          }
        }
        int index5 = index4 + 1;
        if (this.is_valid(value[index5]))
        {
          try
          {
            printObject.printed_on = (DateTime) value[index5];
          }
          catch
          {
          }
        }
        int index6 = index5 + 1;
        if (this.is_valid(value[index6]))
        {
          try
          {
            printObject.printed = (bool) value[index6];
          }
          catch
          {
          }
        }
      }
      catch (Exception ex)
      {
        printObject.print_id = 0L;
        this.log.Error((object) ("ERR (get_print_object): " + ex.ToString()));
      }
      return printObject;
    }

    public visit_print update_visit_print(visit_print obj)
    {
      obj.print_id = !this.db.execute_procedure("sbt_sp_vms_visit_print_update", new Dictionary<string, object>()
      {
        {
          "@print_id",
          (object) obj.print_id
        },
        {
          "@visit_id",
          (object) obj.visit_id
        },
        {
          "@account_id",
          (object) obj.account_id
        },
        {
          "@printed",
          (object) obj.printed
        }
      }) ? 0L : Convert.ToInt64(this.db.resultString);
      return obj;
    }

    public Dictionary<long, string> get_users(Guid account_id)
    {
      Dictionary<long, string> users = new Dictionary<long, string>();
      if (this.db.get_dataset("select user_id, full_name from sbt_users where account_id='" + (object) account_id + "' and status=1 order by full_name"))
      {
        foreach (DataRow row in (InternalDataCollectionBase) this.db.resultDataSet.Tables[0].Rows)
          users.Add(Convert.ToInt64(row["user_id"]), row["full_name"].ToString());
      }
      return users;
    }
  }
}
