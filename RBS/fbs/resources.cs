// Decompiled with JetBrains decompiler
// Type: resources
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using skynapse.fbs;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class resources : WebService
{
  private DataAccess db;

  public resources() => this.db = new DataAccess(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);

  [WebMethod]
  public string[] get_resources(string prefixText, int count, string contextKey)
  {
    if (count == 0)
      count = 10;
    List<string> stringList = new List<string>();
    if (this.db.get_dataset("select item_id,name from sbt_modules_resource_items where account_id='" + contextKey + "' and name like '%" + prefixText + "%' and status=1"))
    {
      DataTable table = this.db.resultDataSet.Tables[0];
      for (int index = 0; index < table.Rows.Count; ++index)
      {
        string autoCompleteItem = AutoCompleteExtender.CreateAutoCompleteItem(table.Rows[index][1].ToString(), table.Rows[index][0].ToString());
        stringList.Add(autoCompleteItem);
      }
    }
    return stringList.ToArray();
  }
}
