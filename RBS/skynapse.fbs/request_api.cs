// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.request_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class request_api : api_base
  {
    public Dictionary<string, int> request_status;

    public request_api()
    {
      this.request_status = new Dictionary<string, int>();
      this.request_status.Add("Return", 1);
      this.request_status.Add("New Request", 0);
    }

    public request update_request(request obj)
    {
      try
      {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("@request_id", (object) obj.request_id);
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@created_by", (object) obj.created_by);
        parameters.Add("@modified_by", (object) obj.modified_by);
        parameters.Add("@record_id", (object) obj.record_id);
        parameters.Add("@status", (object) obj.status);
        parameters.Add("@request_ref_no", (object) obj.request_ref_no);
        parameters.Add("@requestor_name", (object) obj.requestor_name);
        parameters.Add("@description", (object) obj.description);
        parameters.Add("@responsetext", (object) obj.responsetext);
        parameters.Add("@email", (object) obj.email);
        parameters.Add("@costcenter", (object) obj.costcenter);
        parameters.Add("@department", (object) obj.dept);
        parameters.Add("@division", (object) obj.division);
        parameters.Add("@section", (object) obj.section);
        parameters.Add("@entilement", (object) obj.entilement);
        parameters.Add("@hire_department", (object) obj.hire_dept);
        parameters.Add("@hire_division", (object) obj.hire_division);
        parameters.Add("@hire_section", (object) obj.hire_section);
        parameters.Add("@hire_designation", (object) obj.hire_designation);
        parameters.Add("@hire_name", (object) obj.hire_name);
        if (obj.requestdate.Year.ToString() != "1")
          parameters.Add("@requestdate", (object) obj.requestdate.ToString(api_constants.sql_datetime_format));
        else
          parameters.Add("@requestdate", (object) DBNull.Value);
        parameters.Add("@responseby", (object) obj.responseby);
        if (obj.responsedate.Year.ToString() != "1")
          parameters.Add("@responsedate", (object) obj.responsedate.ToString(api_constants.sql_datetime_format));
        else
          parameters.Add("@responsedate", (object) DBNull.Value);
        parameters.Add("@telephone", (object) obj.telephone);
        parameters.Add("@remarks", (object) obj.remarks);
        bool flag;
        try
        {
          flag = this.db.execute_procedure("sbt_sp_request_update", parameters);
        }
        catch (Exception ex)
        {
          flag = false;
          this.log.Error((object) ("Request : " + ex.ToString()));
        }
        obj.request_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.request_id = 0L;
      }
      return obj;
    }

    public DataSet get_requests(
      Guid account_id,
      string filter,
      string colname,
      string order,
      string startno,
      string endno,
      string email)
    {
      try
      {
        return this.db.get_dataset("select * from(select  ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,b.* ,m.name as department_name from sbt_request as b  left join sbt_division_master as m on b.dept = m.master_id where email ='" + email + "' and (requestor_name like '%" + filter + "%' or m.name like '%" + filter + "%' or telephone like '%" + filter + "%' or email like '%" + filter + "%' or description like '%" + filter + "%' or requestdate like '%" + filter + "%')) as sq where RowNumber between " + startno + " and " + endno + ";select count(*) from (select * from(select  ROW_NUMBER() OVER (ORDER BY status asc) AS RowNumber,b.* ,m.name as department_name from sbt_request as b  left join sbt_division_master as m on b.dept = m.master_id where email ='" + email + "' and (requestor_name like '%" + filter + "%' or m.name like '%" + filter + "%' or telephone like '%" + filter + "%' or email like '%" + filter + "%' or description like '%" + filter + "%' or requestdate like '%" + filter + "%')) as sq where RowNumber between 1 and 1000000) as s") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_requests(
      Guid account_id,
      string filter,
      string colname,
      string order,
      string startno,
      string endno)
    {
      try
      {
        return this.db.get_dataset("select * from(select  ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,b.* ,m.name as department_name from sbt_request as b  left join sbt_division_master as m on b.dept = m.master_id where requestor_name like '%" + filter + "%' or m.name like '%" + filter + "%' or hire_designation like '%" + filter + "%' or responsedate like '%" + filter + "%' or responseby like '%" + filter + "%' or requestdate like '%" + filter + "%') as sq where RowNumber between " + startno + " and " + endno + ";select count(*) from (select * from(select  ROW_NUMBER() OVER (ORDER BY status asc) AS RowNumber,b.* ,m.name as department_name from sbt_request as b  left join sbt_division_master as m on b.dept = m.master_id where requestor_name like '%" + filter + "%' or m.name like '%" + filter + "%' or hire_designation like '%" + filter + "%' or responsedate like '%" + filter + "%' or responseby like '%" + filter + "%' or requestdate like '%" + filter + "%') as sq where RowNumber between 1 and 1000000) as s") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_designation()
    {
      try
      {
        return this.db.get_dataset("select distinct property_value from sbt_user_properties where property_name = 'staff_desg' and property_value is not null and property_value != ''") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public request get_request(long request_id, Guid account_id, string type)
    {
      request request = new request();
      request.request_id = 0L;
      string str = "select b.request_id,b.account_id,b.created_on,b.created_by,b.modified_on,b.modified_by,b.record_id,b.request_ref_no,b.requestor_name,b.costcenter,b.telephone,b.email,b.description,b.hire_name,b.hire_designation," + " b.status,b.requestdate,b.responsetext,b.entilement,b.responsedate,b.responseby,b.remarks,b.division,b.section,b.hire_division,b.hire_section,b.dept,b.hire_dept ,m.name as department_name,hd.name as hire_department_name ,s.name as section_name,hs.name as hire_section_name from sbt_request as b  left join sbt_division_master as m on b.dept = m.master_id left join sbt_division_master as hd on b.hire_dept = hd.master_id left join sbt_division_master as s on b.section = s.master_id left join sbt_division_master as hs on b.hire_section = hs.master_id where request_id='" + (object) request_id + "' and b.account_id='" + (object) account_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(str);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          if (objArray[0] != null)
          {
            try
            {
              request.request_id = (long) objArray[0];
            }
            catch
            {
            }
          }
          if (objArray[1] != null)
          {
            try
            {
              request.account_id = (Guid) objArray[1];
            }
            catch
            {
            }
          }
          if (objArray[3] != null)
          {
            try
            {
              request.created_on = (DateTime) objArray[3];
            }
            catch
            {
            }
          }
          if (objArray[9] != null)
          {
            try
            {
              request.requestor_name = (string) objArray[9];
            }
            catch
            {
            }
          }
          if (objArray[17] != null)
          {
            try
            {
              request.requestdate = (DateTime) objArray[17];
            }
            catch
            {
            }
          }
          if (objArray[16] != null)
          {
            try
            {
              request.status = (short) objArray[16];
            }
            catch
            {
            }
          }
          if (objArray[13] != null)
          {
            try
            {
              request.description = (string) objArray[13];
            }
            catch
            {
            }
          }
          if (type == "get_data")
          {
            if (objArray[29] != null)
            {
              try
              {
                request.dept = (string) objArray[29];
              }
              catch
              {
                request.dept = "";
              }
            }
          }
          else if (objArray[27] != null)
          {
            try
            {
              request.dept = (string) objArray[27];
            }
            catch
            {
              request.dept = "";
            }
          }
          if (objArray[11] != null)
          {
            try
            {
              request.telephone = (string) objArray[11];
            }
            catch
            {
            }
          }
          if (objArray[12] != null)
          {
            try
            {
              request.email = (string) objArray[12];
            }
            catch
            {
            }
          }
          if (objArray[14] != null)
          {
            try
            {
              request.hire_name = (string) objArray[14];
            }
            catch
            {
            }
          }
          if (type == "get_data")
          {
            if (objArray[30] != null)
            {
              try
              {
                request.hire_dept = (string) objArray[30];
              }
              catch
              {
                request.hire_dept = "";
              }
            }
          }
          else
          {
            try
            {
              request.hire_dept = (string) objArray[28];
            }
            catch
            {
              request.hire_dept = "";
            }
          }
          if (objArray[15] != null)
          {
            try
            {
              request.hire_designation = (string) objArray[15];
            }
            catch
            {
              request.hire_designation = "";
            }
          }
          if (objArray[20] != null)
          {
            try
            {
              request.responsedate = (DateTime) objArray[20];
            }
            catch
            {
            }
          }
          if (objArray[21] != null)
          {
            try
            {
              request.responseby = (long) objArray[21];
            }
            catch
            {
            }
          }
          if (objArray[18] != null)
          {
            try
            {
              request.responsetext = (string) objArray[18];
            }
            catch
            {
              request.responsetext = "";
            }
          }
          if (objArray[19] != null)
          {
            try
            {
              request.entilement = (string) objArray[19];
            }
            catch
            {
              request.entilement = "";
            }
          }
          if (objArray[8] != null)
          {
            try
            {
              request.request_ref_no = (string) objArray[8];
            }
            catch
            {
            }
          }
          if (objArray[22] != null)
          {
            try
            {
              request.remarks = (string) objArray[22];
            }
            catch
            {
            }
          }
          if (objArray[10] != null)
          {
            try
            {
              request.costcenter = (string) objArray[10];
            }
            catch
            {
            }
          }
          if (objArray[23] != null)
          {
            try
            {
              request.division = (string) objArray[23];
            }
            catch
            {
            }
          }
          if (type == "get_data")
          {
            if (objArray[31] != null)
            {
              try
              {
                request.section = (string) objArray[31];
              }
              catch
              {
                request.section = "";
              }
            }
          }
          else if (objArray[24] != null)
          {
            try
            {
              request.section = (string) objArray[24];
            }
            catch
            {
              request.section = "";
            }
          }
          if (objArray[25] != null)
          {
            try
            {
              request.hire_division = (string) objArray[25];
            }
            catch
            {
            }
          }
          if (type == "get_data")
          {
            if (objArray[32] != null)
            {
              try
              {
                request.hire_section = (string) objArray[32];
              }
              catch
              {
                request.hire_section = "";
              }
            }
          }
          else if (objArray[26] != null)
          {
            try
            {
              request.hire_section = (string) objArray[26];
            }
            catch
            {
              request.hire_section = "";
            }
          }
          if (objArray[7] != null)
          {
            try
            {
              request.record_id = (Guid) objArray[7];
            }
            catch
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
        request.request_id = 0L;
        this.log.Error((object) str, ex);
      }
      return request;
    }

    public long get_master_id_by_name(string name)
    {
      long masterIdByName = 0;
      try
      {
        if (this.db.get_dataset("select master_id from sbt_division_master where name='" + name + "'"))
        {
          foreach (DataTable table in (InternalDataCollectionBase) this.db.resultDataSet.Tables)
            masterIdByName = (long) Convert.ToInt32(table.Rows[0][0]);
          this.db.resultDataSet.Dispose();
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return masterIdByName;
    }
  }
}
