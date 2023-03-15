// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.workflow_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace skynapse.fbs
{
  public class workflow_api : api_base
  {
    public string get_action_status(short id)
    {
      try
      {
        foreach (string key in api_constants.workflow_action_status.Keys)
        {
          if ((int) api_constants.workflow_action_status[key] == (int) id)
            return key;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public short get_action_status(string status)
    {
      try
      {
        if (api_constants.workflow_action_status.ContainsKey(status))
          return api_constants.workflow_action_status[status];
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return -1;
    }

    public string get_action_type(short id)
    {
      try
      {
        foreach (string key in api_constants.workflow_action_type.Keys)
        {
          if ((int) api_constants.workflow_action_type[key] == (int) id)
            return key;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return "";
    }

    public short get_action_type(string type)
    {
      try
      {
        if (api_constants.workflow_action_type.ContainsKey(type))
          return api_constants.workflow_action_type[type];
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return -1;
    }

    public DataSet get_workflows(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select a.* from sbt_workflow a where a.account_id='" + (object) account_id + "' " + " and (a.workflow_id=(select  top 1 workflow_id from sbt_workflow where reference_id= a.reference_id order by workflow_id desc))" + " ORDER BY a.workflow_id DESC ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_workflow_data(long reference_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_workflow where account_id='" + (object) account_id + "' and reference_id='" + (object) reference_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_workflows(long user_id, string group_id, Guid account_id)
    {
      try
      {
        string str = "select a.* from sbt_workflow a where 1=1 ";
        if (!string.IsNullOrEmpty(group_id))
          str = str + " and a.action_owner_id IN (" + group_id + ") ";
        return this.db.get_dataset(str + " and (a.action_type='" + api_constants.workflow_action_type["New_Booking"].ToString() + "' or a.action_type='" + api_constants.workflow_action_type["Transfer_Group"].ToString() + "' ) and a.account_id='" + (object) account_id + "' and a.action_status=0;" + " and (a.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= a.reference_id order by workflow_id desc))" + " ORDER BY a.workflow_id DESC " + "select b.workflow_id,b.created_by,b.action_type from sbt_workflow b where b.action_owner_id IN (" + group_id + ") and b.action_type='" + api_constants.workflow_action_type["Transfer_User"].ToString() + "' and b.account_id='" + (object) account_id + "' and b.action_status=0;" + " and (b.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= b.reference_id order by workflow_id desc))" + " ORDER BY b.workflow_id DESC " + "select c.workflow_id,c.created_by,c.action_type from sbt_workflow c where c.created_by='" + (object) user_id + "' and c.account_id='" + (object) account_id + "' and c.action_status=0" + " and (c.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= c.reference_id order by workflow_id desc))" + " ORDER BY c.workflow_id DESC ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_workflows(
      long user_id,
      string group_id,
      Guid account_id,
      string fromdate,
      string todate)
    {
      try
      {
        string str1 = "select a.* from sbt_workflow a ,sbt_asset_bookings h where 1=1 ";
        if (!string.IsNullOrEmpty(group_id))
          str1 = str1 + " and a.action_owner_id IN (" + group_id + ") ";
        string str2 = str1 + " and (a.action_type='" + api_constants.workflow_action_type["New_Booking"].ToString() + "' or a.action_type='" + api_constants.workflow_action_type["Transfer_Group"].ToString() + "' ) and a.account_id='" + (object) account_id + "' and a.action_status=0" + " and (a.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= a.reference_id order by workflow_id desc))" + "  AND h.booking_id=a.reference_id  AND h.status=4";
        if (fromdate != "" && todate != "")
          str2 = str2 + " AND a.reference_id IN   ( SELECT booking_id FROM sbt_asset_bookings " + "WHERE  ( book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "' ))";
        string str3 = str2 + " ORDER BY a.workflow_id DESC " + "select b.workflow_id,b.created_by,b.action_type from sbt_workflow b, sbt_asset_bookings h  where " + "b.action_owner_id IN (" + (object) user_id + ") " + "and b.action_type='" + api_constants.workflow_action_type["Transfer_User"].ToString() + "' and b.account_id='" + (object) account_id + "' and b.action_status=0  " + " and (b.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= b.reference_id order by workflow_id desc))" + "  AND h.booking_id=b.reference_id AND h.status=4";
        if (fromdate != "" && todate != "")
          str3 = str3 + " AND b.reference_id IN   ( SELECT booking_id FROM sbt_asset_bookings " + "WHERE account_id='" + (object) account_id + "' and  ( book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "' ))";
        string str4 = str3 + " ORDER BY b.workflow_id DESC " + "select c.workflow_id,c.created_by,c.action_type from sbt_workflow c,sbt_asset_bookings h where c.created_by='" + (object) user_id + "' and c.account_id='" + (object) account_id + "' and c.action_status=0 " + " and (c.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= c.reference_id order by workflow_id desc))" + "  AND h.booking_id=c.reference_id AND h.status=4";
        if (fromdate != "" && todate != "")
          str4 = str4 + " AND c.reference_id IN   ( SELECT booking_id FROM sbt_asset_bookings" + " WHERE account_id='" + (object) account_id + "' and  (  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "' ))";
        return this.db.get_dataset(str4 + " ORDER BY c.workflow_id DESC ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_workflows_reuqest_forapproval(
      long user_id,
      string group_id,
      Guid account_id,
      string fromdate,
      string todate)
    {
      try
      {
        string str = "select a.* from sbt_workflow a ,sbt_asset_bookings c where  (a.action_type='" + api_constants.workflow_action_type["New_Booking"].ToString() + "' or a.action_type='" + api_constants.workflow_action_type["Transfer_Group"].ToString() + "' ) and a.account_id='" + (object) account_id + "' and a.action_status=0  " + " and (a.workflow_id=(select  top 1 workflow_id from sbt_workflow where account_id='" + (object) account_id + "' and reference_id= a.reference_id order by workflow_id desc))" + " AND a.reference_id=c.booking_id and c.status=4";
        if (fromdate != "" && todate != "")
          str = str + "AND a.reference_id IN   ( SELECT booking_id FROM sbt_asset_bookings  WHERE account_id='" + (object) account_id + "' and status=4 " + " and (  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "' ))";
        return this.db.get_dataset(str + " ORDER BY a.workflow_id DESC ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    private workflow get_workflow_object(string sql)
    {
      workflow workflowObject = new workflow();
      workflowObject.workflow_id = 0L;
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        Dictionary<int, object[]> dataObjects = this.db.get_data_objects(sql);
        if (dataObjects.Count > 0)
        {
          object[] objArray = dataObjects[0];
          int index1 = 0;
          if (this.is_valid(objArray[index1]))
            workflowObject.workflow_id = (long) objArray[index1];
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
            workflowObject.account_id = (Guid) objArray[index2];
          int index3 = index2 + 1;
          if (this.is_valid(objArray[index3]))
            workflowObject.created_on = (DateTime) objArray[index3];
          int index4 = index3 + 1;
          if (this.is_valid(objArray[index4]))
            workflowObject.created_by = (long) objArray[index4];
          int index5 = index4 + 1;
          try
          {
            workflowObject.modified_on = (DateTime) objArray[index5];
          }
          catch
          {
            workflowObject.modified_on = workflowObject.created_on;
          }
          int index6 = index5 + 1;
          try
          {
            workflowObject.modified_by = (long) objArray[index6];
          }
          catch
          {
            workflowObject.modified_by = workflowObject.created_by;
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              workflowObject.title = (string) objArray[index7];
            }
            catch
            {
              workflowObject.title = "";
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              workflowObject.message = (string) objArray[index8];
            }
            catch
            {
              workflowObject.message = "";
            }
          }
          int index9 = index8 + 1;
          if (this.is_valid(objArray[index9]))
          {
            try
            {
              workflowObject.due_on = (DateTime) objArray[index9];
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
              workflowObject.reference_id = (long) objArray[index10];
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
              workflowObject.action_status = (short) objArray[index11];
            }
            catch
            {
            }
          }
          int index12 = index11 + 1;
          if (this.is_valid(objArray[index12]))
          {
            try
            {
              workflowObject.action_taken_on = (DateTime) objArray[index12];
            }
            catch
            {
            }
          }
          int index13 = index12 + 1;
          if (this.is_valid(objArray[index13]))
          {
            try
            {
              workflowObject.action_taken_by = (long) objArray[index13];
            }
            catch
            {
            }
          }
          int index14 = index13 + 1;
          if (this.is_valid(objArray[index14]))
          {
            try
            {
              workflowObject.action_remarks = (string) objArray[index14];
            }
            catch
            {
            }
          }
          int index15 = index14 + 1;
          if (this.is_valid(objArray[index15]))
          {
            try
            {
              workflowObject.action_type = (short) objArray[index15];
            }
            catch
            {
            }
          }
          int index16 = index15 + 1;
          if (this.is_valid(objArray[index16]))
          {
            try
            {
              workflowObject.action_owner_id = (long) objArray[index16];
            }
            catch
            {
            }
          }
          int index17 = index16 + 1;
          if (this.is_valid(objArray[index17]))
          {
            try
            {
              workflowObject.record_id = (Guid) objArray[index17];
            }
            catch
            {
            }
          }
          int index18 = index17 + 1;
          workflowObject.properties = new XmlDocument();
          workflowObject.properties.LoadXml("<root></root>");
          if (this.is_valid(objArray[index18]))
          {
            try
            {
              workflowObject.properties.LoadXml((string) objArray[index18]);
            }
            catch
            {
              workflowObject.properties.LoadXml("<root></root>");
            }
          }
        }
      }
      catch (Exception ex)
      {
        workflowObject.workflow_id = 0L;
        this.log.Error((object) sql, ex);
      }
      return workflowObject;
    }

    public workflow get_workflow(long workflow_id, Guid account_id) => this.get_workflow_object("select * from sbt_workflow where workflow_id='" + (object) workflow_id + "' and account_id='" + (object) account_id + "'");

    public workflow get_workflow_reference_id(long reference_id, Guid account_id) => this.get_workflow_object("select * from sbt_workflow where reference_id='" + (object) reference_id + "' and action_status=" + (object) api_constants.workflow_action_status["Rejected"] + " and account_id='" + (object) account_id + "'");

    public workflow update_workflow(workflow obj)
    {
      try
      {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("@workflow_id", (object) obj.workflow_id);
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@created_by", (object) obj.created_by);
        parameters.Add("@modified_by", (object) obj.modified_by);
        parameters.Add("@record_id", (object) obj.record_id);
        if (obj.title == null)
          parameters.Add("@title", (object) DBNull.Value);
        else
          parameters.Add("@title", (object) obj.title);
        if (obj.message == null)
          parameters.Add("@message", (object) DBNull.Value);
        else
          parameters.Add("@message", (object) obj.message);
        if (obj.due_on.Year.ToString() == "1900" || obj.due_on.Year.ToString() == "1")
          parameters.Add("@due_on", (object) DBNull.Value);
        else
          parameters.Add("@due_on", (object) obj.due_on);
        parameters.Add("@reference_id", (object) obj.reference_id);
        parameters.Add("@action_status", (object) obj.action_status);
        parameters.Add("@action_taken_by", (object) obj.action_taken_by);
        if (obj.action_remarks == null)
          parameters.Add("@action_remarks", (object) DBNull.Value);
        else
          parameters.Add("@action_remarks", (object) obj.action_remarks);
        parameters.Add("@action_type", (object) obj.action_type);
        parameters.Add("@action_owner_id", (object) obj.action_owner_id);
        parameters.Add("@properties", (object) obj.properties.InnerXml);
        try
        {
          obj.workflow_id = !this.db.execute_procedure("sbt_sp_workflow_update", parameters) ? 0L : Convert.ToInt64(this.db.resultString);
        }
        catch (Exception ex)
        {
          obj.workflow_id = 0L;
          this.log.Error((object) ("asset booking : " + ex.ToString()));
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return obj;
    }

    public DataSet get_mytask_records(Guid account_id, string fromdate, string todate)
    {
      try
      {
        string Sql = " select * from vw_sbt_mytask where account_id = '" + (object) account_id + "' ";
        if (fromdate != "" && todate != "")
          Sql = Sql + " and  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error ->", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_mytask_records_superuser(
      Guid account_id,
      string fromdate,
      string todate,
      string actionOwnerGrpId)
    {
      try
      {
        string Sql = " select * from vw_sbt_mytask where account_id = '" + (object) account_id + "' and action_owner_id in ( " + actionOwnerGrpId + " ) and status=4 ";
        if (fromdate != "" && todate != "")
          Sql = Sql + " and  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error ->", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_mytask_records_requestor(
      Guid account_id,
      string fromdate,
      string todate,
      long uId)
    {
      try
      {
        string Sql = " select * from vw_sbt_mytask where account_id = '" + (object) account_id + "' and (booked_for=" + (object) uId + " or  created_by = " + (object) uId + ") and status = 4 ";
        if (fromdate != "" && todate != "")
          Sql = Sql + " and  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error ->", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_mytask_records_admin(Guid account_id, string fromdate, string todate)
    {
      try
      {
        string Sql = " select * from vw_sbt_mytask where account_id = '" + (object) account_id + "'  and status=4 ";
        if (fromdate != "" && todate != "")
          Sql = Sql + " and  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error ->", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_mytask_records_history(
      Guid account_id,
      string fromdate,
      string todate,
      long uId,
      string actionOwnerGrpId)
    {
      try
      {
        string Sql = " select * from vw_sbt_mytask where account_id = '" + (object) account_id + "'   and (action_status = 1 or action_status =2 or action_status = 3)" + "  and (action_owner_id in ( " + actionOwnerGrpId + " ) or (booked_for=" + (object) uId + " or  created_by = " + (object) uId + ") )";
        if (fromdate != "" && todate != "")
          Sql = Sql + " and  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error ->", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_mytask_records_history_admin(
      Guid account_id,
      string fromdate,
      string todate)
    {
      try
      {
        string Sql = " select * from vw_sbt_mytask where account_id = '" + (object) account_id + "'   and (action_status = 1 or action_status =2 or action_status = 3)";
        if (fromdate != "" && todate != "")
          Sql = Sql + " and  book_from  between '" + fromdate + "' and '" + todate + "' or book_to  between '" + fromdate + "' and '" + todate + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error ->", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_workflow_withdraw_email(long reference_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select email from sbt_users where account_id = '" + (object) account_id + "' and user_id in (select user_id " + " from sbt_user_group_mappings where  group_id = (select asset_owner_group_id from sbt_assets where asset_id = (select asset_id from sbt_asset_bookings where booking_id = " + " (select reference_id from sbt_workflow where reference_id = " + (object) reference_id + ")))) ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }
  }
}
