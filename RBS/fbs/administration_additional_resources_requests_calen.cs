// Decompiled with JetBrains decompiler
// Type: administration_additional_resources_requests_calendar
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_additional_resources_requests_calendar : 
  fbs_base_page,
  IRequiresSessionState
{
  protected DropDownList ddlType;
  protected HtmlSelect ddl_resources;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected TextBox txtUser;
  protected Panel myPanel;
  protected AutoCompleteExtender autoComplete1;
  protected HiddenField hfUserId;
  protected TextBox txt_keyword;
  protected HiddenField hdn_search;
  protected HiddenField hdn_orderby;
  protected HiddenField hdn_orderdir;
  public string accountid = "";
  public string html_table;
  public string current_date;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.accountid = this.current_user.account_id.ToString();
    try
    {
      if (this.Request.QueryString["type"] == "delete")
        this.resapi.delete_resource_booking_items(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id, this.current_user.user_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Delete Resource Booking Item Error ->", ex);
    }
    if (this.IsPostBack)
      return;
    this.current_date = this.current_timestamp.ToString("yyyy-MM-dd");
    DataSet userItemMap = this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, this.str_resource_module);
    this.ViewState.Add("allowed_items", (object) userItemMap);
    this.Session.Add("allowed_items", (object) userItemMap);
    this.populate_dropdown();
    this.set_resource_dropdown(0L);
  }

  private void populate_dropdown()
  {
    DataSet dataSet1 = new DataSet();
    DataSet dataSet2 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_resource_type");
    if (dataSet2 == null)
    {
      dataSet2 = this.resapi.get_all_resource_settings_by_parameter(this.current_user.account_id, "resource_type", this.str_resource_module);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_resource_type", (object) dataSet2);
    }
    this.ddlType.Items.Clear();
    this.ddlType.Items.Add(new ListItem("All Resource Types", "0"));
    foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
      this.ddlType.Items.Add(new ListItem(row["value"].ToString(), row["setting_id"].ToString()));
  }

  private void set_resource_dropdown(long resource_type_id)
  {
    this.ddl_resources.Items.Clear();
    this.ddl_resources.Items.Add(new ListItem("All Resources", "0"));
    if (resource_type_id == 0L)
      return;
    DataSet dataSet = (DataSet) this.ViewState["allowed_items"];
    foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resource_items_by_item_type_id(resource_type_id, this.current_user.account_id, this.str_resource_module).Tables[0].Rows)
    {
      if (dataSet.Tables[0].Select("item_id='" + row["item_id"] + "'").Length > 0)
        this.ddl_resources.Items.Add(new ListItem(row["name"].ToString(), row["item_id"].ToString()));
    }
  }
}
