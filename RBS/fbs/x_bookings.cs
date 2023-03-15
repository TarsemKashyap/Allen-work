// Decompiled with JetBrains decompiler
// Type: x_bookings
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class x_bookings : fbs_base_page, IRequiresSessionState
{
  public string html_action;
  public string accountid = "";
  public string html_table;
  public string current_date;
  public string holidays;
  public string alluser = "";
  private DataSet res_data;
  private DataSet rooms;
  public static bool showResources;
  protected HiddenField hdnBookingWindow;
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected DropDownList ddl_room;
  protected DropDownList ddl_resources;
  protected DropDownList ddl_show;
  protected CheckBox chk_hide_past;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    x_bookings.showResources = Convert.ToBoolean(this.current_account.properties["resource_booking"]);
    this.populate_rooms();
    if (x_bookings.showResources)
      this.populate_resources();
    this.load_preference();
    this.current_date = this.current_timestamp.ToString("yyyy-MM-dd");
    this.do_action();
  }

  private void do_action()
  {
    string str;
    try
    {
      str = this.Request.QueryString["act"];
    }
    catch
    {
      str = "";
    }
    long num;
    try
    {
      num = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
      num = 0L;
    }
    if (num <= 0L)
      return;
    if (str == "v")
      this.html_action = "ab(" + (object) num + ")";
    if (!(str == "c"))
      return;
    this.html_action = "cb(" + (object) num + ")";
  }

  private void load_preference()
  {
    if (this.current_user.properties.ContainsKey("default_calendar_view"))
    {
      foreach (ListItem listItem in this.ddl_show.Items)
      {
        if (listItem.Value == this.current_user.properties["default_calendar_view"].property_value)
        {
          listItem.Selected = true;
          break;
        }
      }
    }
    if (!this.current_user.properties.ContainsKey("show_past"))
      return;
    if (Convert.ToBoolean(this.current_user.properties["show_past"].property_value))
      this.chk_hide_past.Checked = false;
    else
      this.chk_hide_past.Checked = true;
  }

  private void populate_resources()
  {
    this.ddl_resources.Items.Add(new ListItem("All Resources", "0"));
    this.res_data = this.resapi.get_resource_names(0L, this.current_user.account_id);
    foreach (DataRow row in (InternalDataCollectionBase) this.res_data.Tables[0].Rows)
      this.ddl_resources.Items.Add(new ListItem(row["name"].ToString(), row["item_id"].ToString()));
  }

  private void populate_rooms()
  {
    this.rooms = this.assets.view_assets(this.current_user.account_id);
    this.ddl_room.Items.Add(new ListItem("All Rooms", "0"));
    foreach (DataRow row in (InternalDataCollectionBase) this.rooms.Tables[0].Rows)
    {
      if (this.visible_rooms.Contains(Convert.ToInt64(row["asset_id"])))
        this.ddl_room.Items.Add(new ListItem(row["name"].ToString(), row["asset_id"].ToString()));
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("bookings.aspx");

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
