// Decompiled with JetBrains decompiler
// Type: administration_ajax_page
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;

public class administration_ajax_page : fbs_base_page, IRequiresSessionState
{
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
  }

  [WebMethod]
  public static string checkName(string name, string Building, string Level, string asset_id)
  {
    user user = (user) HttpContext.Current.Session["user"];
    return new asset_api().checknameavilablity(Convert.ToInt32(Building), Convert.ToInt32(Level), name, Convert.ToInt64(asset_id), user.account_id) ? "1" : "0";
  }

  [WebMethod]
  public static string checkAsswithfacility(string setting_id, string filter)
  {
    user user = (user) HttpContext.Current.Session["user"];
    return new asset_api().checknameavilablity_deletemasterdata(Convert.ToInt32(setting_id), filter, user.account_id) ? "1" : "0";
  }

  [WebMethod]
  public static string checkFacility_toDelete(string asset_id)
  {
    user user = (user) HttpContext.Current.Session["user"];
    asset_api assetApi = new asset_api();
    DataSet bookingAssteid = new booking_api().get_booking_assteid(Convert.ToInt64(asset_id), user.account_id);
    return bookingAssteid != null ? (Convert.ToInt32(bookingAssteid.Tables[0].Rows[0].ItemArray[0]) <= 0 ? "1" : "0") : (assetApi.checknameavilablity_deleteFacility(Convert.ToInt64(asset_id), 1, user.account_id) ? "1" : "0");
  }

  [WebMethod]
  public static string checkOrder(string sortOrder, string AssetType)
  {
    asset_api assetApi = new asset_api();
    return "0";
  }

  [WebMethod]
  public string populate_ui(user_group obj, Guid account_id, long user_id) => this.workflows.get_workflows(user_id, obj.group_id.ToString(), account_id).Tables[0].Rows.Count.ToString();

  [WebMethod]
  public static string mycount(string groupid, string userid, string ac) => new administration_ajax_page().mycount_process(groupid, userid, ac);

  [WebMethod]
  public static string Resendemail(string booking_id) => "";

  [WebMethod]
  public static string[] useremail(string userid, string accountid)
  {
    List<string> source = new List<string>();
    if (userid != "0")
    {
      Guid account_id = new Guid(accountid);
      users_api usersApi = new users_api();
      user user = usersApi.get_user((long) Convert.ToInt32(userid), account_id);
      Dictionary<string, user_property> userProperties = usersApi.get_user_properties(Convert.ToInt64(user.user_id), user.account_id);
      source.Add(user.email);
      source.Add(userProperties["staff_offphone"].property_value);
    }
    return source.ToArray<string>();
  }

  [WebMethod]
  public static string[] get_resources(string resource_type_id, string accountID)
  {
    DataTable resourceRecords = administration_ajax_page.get_resource_records(resource_type_id, accountID);
    DataSet dataSet = (DataSet) HttpContext.Current.Session["allowed_items"];
    List<string> stringList = new List<string>();
    foreach (DataRow row in (InternalDataCollectionBase) resourceRecords.Rows)
    {
      if (dataSet.Tables[0].Select("item_id='" + row["item_id"] + "'").Length > 0)
      {
        string str = row["name"].ToString() + "," + row["item_id"].ToString();
        stringList.Add(str);
      }
    }
    return stringList.ToArray();
  }

  public static DataTable get_resource_records(string resource_type_id, string accID)
  {
    resource_api resourceApi = new resource_api();
    DataSet dataSet = new DataSet();
    return resourceApi.get_resource_items_by_item_type_id(Convert.ToInt64(resource_type_id), new Guid(accID), "resource_module").Tables[0];
  }

  private void mycount_process() => throw new NotImplementedException();

  public string mycount_process(string groupid, string userid, string ac)
  {
    workflow_api workflowApi = new workflow_api();
    int num1 = 0;
    Guid account_id = new Guid(ac);
    string group_id = groupid;
    long int64 = Convert.ToInt64(userid);
    DataSet workflows = workflowApi.get_workflows(int64, group_id, account_id, "", "");
    DataSet reuqestForapproval = workflowApi.get_workflows_reuqest_forapproval(int64, group_id, account_id, "", "");
    int num2 = 0;
    if (workflows != null)
    {
      if (workflows.Tables[0].Rows.Count > 0)
        workflows.Tables[0].Merge(workflows.Tables[0]);
      if (workflows.Tables[1].Rows.Count > 0)
        workflows.Tables[0].Merge(workflows.Tables[1]);
      if (workflows.Tables[2].Rows.Count > 0)
        workflows.Tables[0].Merge(workflows.Tables[2]);
      workflows.AcceptChanges();
      DataRow[] dataRowArray1 = workflows.Tables[0].Select("created_by <> '" + userid + "'");
      DataRow[] dataRowArray2 = reuqestForapproval.Tables[0].Select("created_by = '" + userid + "' and action_type='1'");
      if (dataRowArray2.Length > 0)
        num2 = dataRowArray2.Length;
      num1 = dataRowArray1.Length + num2;
    }
    return num1.ToString();
  }

  [WebMethod]
  public static string Checkvaluelogin() => new administration_ajax_page().checksession();

  public string checksession() => this.Session["user"] != null ? "y" : "N";
}
