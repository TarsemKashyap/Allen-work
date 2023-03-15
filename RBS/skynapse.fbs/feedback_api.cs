// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.feedback_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class feedback_api : api_base
  {
    public Dictionary<string, int> feedback_status;

    public feedback_api()
    {
      this.feedback_status = new Dictionary<string, int>();
      this.feedback_status.Add("Replied", 1);
      this.feedback_status.Add("Not Replied", 0);
    }

    public feedback update_feedback(feedback obj)
    {
      try
      {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("@feedback_id", (object) obj.feedback_id);
        parameters.Add("@account_id", (object) obj.account_id);
        parameters.Add("@created_by", (object) obj.created_by);
        parameters.Add("@modified_by", (object) obj.modified_by);
        parameters.Add("@record_id", (object) obj.record_id);
        parameters.Add("@subject", (object) obj.subject);
        parameters.Add("@description", (object) obj.description);
        parameters.Add("@responsetext", (object) obj.responsetext);
        parameters.Add("@email", (object) obj.email);
        if (obj.dateoffeedback.Year.ToString() != "1")
          parameters.Add("@dateoffeedback", (object) obj.dateoffeedback);
        else
          parameters.Add("@dateoffeedback", (object) DBNull.Value);
        if (obj.dateofrespond.Year.ToString() != "1")
          parameters.Add("@dateofrespond", (object) obj.dateofrespond);
        else
          parameters.Add("@dateofrespond", (object) DBNull.Value);
        bool flag;
        try
        {
          flag = this.db.execute_procedure("sbt_sp_feedback_update", parameters);
        }
        catch (Exception ex)
        {
          flag = false;
          this.log.Error((object) ("Feedback : " + ex.ToString()));
        }
        obj.feedback_id = !flag ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
        obj.feedback_id = 0L;
      }
      return obj;
    }

    public DataSet get_feedbacks(
      Guid account_id,
      string filter,
      string colname,
      string order,
      string startno,
      string endno)
    {
      try
      {
        return this.db.get_dataset("select * from(select  ROW_NUMBER() OVER (ORDER BY " + colname + " " + order + ") AS RowNumber,* from sbt_feedback where account_id='" + (object) account_id + "' and subject like '%" + filter + "%' or email like '%" + filter + "%' or description like '%" + filter + "%' or responsetext like '%" + filter + "%' ) as sq where RowNumber between " + startno + " and " + endno + ";select count(*) from (select * from(select  ROW_NUMBER() OVER (ORDER BY subject asc) AS RowNumber,* from sbt_feedback where account_id='" + (object) account_id + "' and subject like '%" + filter + "%' or email like '%" + filter + "%' or description like '%" + filter + "%' or responsetext like '%" + filter + "%') as sq where RowNumber between 1 and 1000000) as s") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_feedbacks(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_feedback where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public feedback get_feedback(long feedback_id, Guid account_id, string type)
    {
      feedback feedback = new feedback();
      feedback.feedback_id = 0L;
      string str = "select * from sbt_feedback where feedback_id = '" + (object) feedback_id + "' and account_id = '" + (object) account_id + "'";
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
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
              feedback.feedback_id = (long) objArray[index1];
            }
            catch
            {
              feedback.feedback_id = 0L;
            }
          }
          int index2 = index1 + 1;
          if (this.is_valid(objArray[index2]))
          {
            try
            {
              feedback.account_id = (Guid) objArray[index2];
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
              feedback.created_on = (DateTime) objArray[index3];
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
              feedback.subject = (string) objArray[index4];
            }
            catch
            {
              feedback.subject = "";
            }
          }
          int index5 = index4 + 1;
          if (this.is_valid(objArray[index5]))
          {
            try
            {
              feedback.dateoffeedback = (DateTime) objArray[index5];
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
              feedback.email = (string) objArray[index6];
            }
            catch
            {
              feedback.email = "";
            }
          }
          int index7 = index6 + 1;
          if (this.is_valid(objArray[index7]))
          {
            try
            {
              feedback.description = (string) objArray[index7];
            }
            catch
            {
              feedback.description = "";
            }
          }
          int index8 = index7 + 1;
          if (this.is_valid(objArray[index8]))
          {
            try
            {
              feedback.responsetext = (string) objArray[index8];
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
              feedback.dateofrespond = (DateTime) objArray[index9];
            }
            catch
            {
            }
          }
          int num = index9 + 1;
        }
      }
      catch (Exception ex)
      {
        feedback.feedback_id = 0L;
        this.log.Error((object) str, ex);
      }
      return feedback;
    }
  }
}
