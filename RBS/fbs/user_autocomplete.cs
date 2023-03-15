// Decompiled with JetBrains decompiler
// Type: user_autocomplete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class user_autocomplete : WebService
{
  [WebMethod]
  public string[] get_users_other_all_user_type(string prefixText, int count, string contextKey)
  {
    if (count == 0)
      count = 10;
    DataTable allUserTypeRecords = this.get_users_other_all_user_type_records(prefixText, contextKey);
    List<string> stringList = new List<string>();
    for (int index = 0; index < allUserTypeRecords.Rows.Count; ++index)
    {
      string autoCompleteItem = AutoCompleteExtender.CreateAutoCompleteItem(allUserTypeRecords.Rows[index][0].ToString(), allUserTypeRecords.Rows[index][1].ToString());
      stringList.Add(autoCompleteItem);
    }
    return stringList.ToArray();
  }

  [WebMethod]
  public string[] get_users_other_all_user_type_view(
    string prefixText,
    int count,
    string contextKey)
  {
    if (count == 0)
      count = 10;
    DataTable userTypeViewRecords = this.get_users_other_all_user_type_view_records(prefixText, contextKey);
    List<string> stringList = new List<string>();
    for (int index = 0; index < userTypeViewRecords.Rows.Count; ++index)
    {
      string autoCompleteItem = AutoCompleteExtender.CreateAutoCompleteItem(userTypeViewRecords.Rows[index][0].ToString(), userTypeViewRecords.Rows[index][1].ToString());
      stringList.Add(autoCompleteItem);
    }
    return stringList.ToArray();
  }

  [WebMethod]
  public string[] get_allusers_not_in_admingroup(
    string prefixText,
    int count,
    string contextKey1,
    string contextKey2)
  {
    if (count == 0)
      count = 10;
    DataTable admingroupRecords = this.get_allusers_not_in_admingroup_records(Convert.ToInt64(contextKey1), prefixText, contextKey2);
    List<string> stringList = new List<string>();
    for (int index = 0; index < admingroupRecords.Rows.Count; ++index)
    {
      string autoCompleteItem = AutoCompleteExtender.CreateAutoCompleteItem(admingroupRecords.Rows[index][0].ToString(), admingroupRecords.Rows[index][1].ToString());
      stringList.Add(autoCompleteItem);
    }
    return stringList.ToArray();
  }

  public DataTable get_users_other_all_user_type_records(string strName, string accID)
  {
    users_api usersApi = new users_api();
    DataSet dataSet = new DataSet();
    return usersApi.get_users_other_all_user_type(strName, new Guid(accID)).Tables[0];
  }

  public DataTable get_users_other_all_user_type_view_records(string strName, string accID)
  {
    users_api usersApi = new users_api();
    DataSet dataSet = new DataSet();
    return usersApi.get_users_other_all_user_type_view(strName, new Guid(accID)).Tables[0];
  }

  public DataTable get_allusers_not_in_admingroup_records(long gid, string strName, string accID)
  {
    users_api usersApi = new users_api();
    DataSet dataSet = new DataSet();
    return usersApi.get_allusers_not_in_admingroup(gid, strName, new Guid(accID)).Tables[0];
  }
}
