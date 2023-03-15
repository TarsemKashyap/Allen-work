// Decompiled with JetBrains decompiler
// Type: book
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class book : fbs_base_page, IRequiresSessionState
{
  protected Literal Literal1;
  protected HtmlGenericControl alt_error_alrdybook;
  protected Label lbl_assetname_heading;
  protected HtmlGenericControl li_invite_list;
  protected HtmlGenericControl li_termsandconditions;
  protected HtmlInputText txt_purpose;
  protected HtmlGenericControl errorpurpose;
  protected HtmlInputText txt_from_date;
  protected HtmlGenericControl Span1;
  protected HtmlInputText txt_to_date;
  protected HtmlGenericControl Span2;
  protected DropDownList ddl_from;
  protected HtmlGenericControl Span3;
  protected DropDownList ddl_to;
  protected HtmlGenericControl Span4;
  protected Button btn_find_room;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlForm form1;
  public string html_date;
  public string html_rooms;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.html_date = this.Request.QueryString["dt"];
    if (this.IsPostBack)
      return;
    this.txt_from_date.Value = Convert.ToDateTime(this.html_date).ToString("dd-MMM-yyyy");
    this.txt_to_date.Value = Convert.ToDateTime(this.html_date).ToString("dd-MMM-yyyy");
    this.Populate_Time(this.ddl_from);
    this.Populate_Time(this.ddl_to);
    DateTime input = DateTime.UtcNow.AddHours(this.current_account.timezone);
    this.ddl_from.SelectedValue = this.utilities.TimeRoundUp(input).ToString("hh:mm tt");
    this.ddl_to.SelectedValue = this.utilities.TimeRoundUp(input.AddMinutes(60.0)).ToString("hh:mm tt");
  }

  public void Populate_Time(DropDownList cbo)
  {
    try
    {
      cbo.Items.Clear();
      string str = this.current_timestamp.ToShortDateString() + " 00:00 AM";
      DateTime dateTime = new DateTime(this.current_timestamp.Year, this.current_timestamp.Month, this.current_timestamp.Day, 0, 0, 0);
      for (int index = 0; index <= 95; ++index)
      {
        cbo.Items.Add(new ListItem(dateTime.ToShortTimeString(), dateTime.ToString("hh:mm tt")));
        dateTime = dateTime.AddMinutes(this.AllowedMinutes);
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_find_room_Click(object sender, EventArgs e) => this.get_assets();

  private void get_assets()
  {
    DataSet assets = this.assets.get_assets(this.current_user.account_id);
    if (this.bookable_rooms == null || this.bookable_rooms.Count == 0)
      this.bookable_rooms = this.assets.get_bookable_assets(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType);
    List<long> longList1 = new List<long>();
    bool flag1 = true;
    bool flag2 = true;
    int num1 = 0;
    DateTime dateTime1 = new DateTime(2000, 1, 1, 0, 0, 0);
    DateTime dateTime2 = new DateTime(2000, 1, 1, 23, 59, 59);
    List<long> bookableRooms = this.bookable_rooms;
    DataSet dataSet1 = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_settings");
    if (dataSet1 == null)
    {
      dataSet1 = this.settings.view_settings(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_settings", (object) dataSet1);
    }
    DataRow[] dataRowArray1 = dataSet1.Tables[0].Select("parameter='advance_booking_window'");
    if (dataRowArray1.Length > 0)
      num1 = Convert.ToInt32(dataRowArray1[0]["value"]);
    DataRow[] dataRowArray2 = dataSet1.Tables[0].Select("parameter='book_holiday'");
    if (dataRowArray2.Length > 0)
      flag1 = Convert.ToBoolean(dataRowArray2[0]["value"]);
    DataRow[] dataRowArray3 = dataSet1.Tables[0].Select("parameter='book_weekend'");
    if (dataRowArray3.Length > 0)
      flag2 = Convert.ToBoolean(dataRowArray3[0]["value"]);
    DataRow[] dataRowArray4 = dataSet1.Tables[0].Select("parameter='operating_hours'");
    if (dataRowArray4.Length > 0)
    {
      string[] strArray = dataRowArray4[0]["value"].ToString().Split('|');
      dateTime1 = Convert.ToDateTime(strArray[0]);
      dateTime2 = Convert.ToDateTime(strArray[1]);
    }
    DataSet dataSet2 = this.assets.view_asset_properties(this.current_user.account_id, new string[4]
    {
      "operating_hours",
      "book_holiday",
      "book_weekend",
      "advance_booking_window"
    });
    Dictionary<long, bool> dic_book_holiday = new Dictionary<long, bool>();
    Dictionary<long, bool> dic_book_weekend = new Dictionary<long, bool>();
    Dictionary<long, int> dictionary = new Dictionary<long, int>();
    Dictionary<long, DateTime> dic_advance_booking_date = new Dictionary<long, DateTime>();
    Dictionary<long, DateTime> dic_op_start = new Dictionary<long, DateTime>();
    Dictionary<long, DateTime> dic_op_end = new Dictionary<long, DateTime>();
    foreach (long bookableRoom in this.bookable_rooms)
    {
      DataRow[] dataRowArray5 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='advance_booking_window'");
      if (dataRowArray5.Length > 0)
        dictionary.Add(bookableRoom, Convert.ToInt32(dataRowArray5[0]["property_value"]));
      else
        dictionary.Add(bookableRoom, num1);
      dic_advance_booking_date.Add(bookableRoom, this.current_timestamp.AddMonths(dictionary[bookableRoom]));
      DataRow[] dataRowArray6 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='book_holiday'");
      if (dataRowArray6.Length > 0)
        dic_book_holiday.Add(bookableRoom, Convert.ToBoolean(dataRowArray6[0]["property_value"]));
      else
        dic_book_holiday.Add(bookableRoom, flag1);
      DataRow[] dataRowArray7 = dataSet2.Tables[0].Select("asset_id='" + (object) bookableRoom + "' and property_name='book_weekend'");
      if (dataRowArray7.Length > 0)
        dic_book_weekend.Add(bookableRoom, Convert.ToBoolean(dataRowArray7[0]["property_value"]));
      else
        dic_book_weekend.Add(bookableRoom, flag2);
      DataRow[] dataRowArray8 = dataSet2.Tables[0].Select("property_name='operating_hours'");
      if (dataRowArray8.Length > 0)
      {
        string[] strArray = dataRowArray8[0]["property_value"].ToString().Split('|');
        dic_op_start.Add(bookableRoom, Convert.ToDateTime(strArray[0]));
        dic_op_end.Add(bookableRoom, Convert.ToDateTime(strArray[1]));
      }
      else
      {
        dic_op_start.Add(bookableRoom, dateTime1);
        dic_op_end.Add(bookableRoom, dateTime2);
      }
    }
    List<long> bookable_assets = new List<long>();
    List<long> longList2 = new List<long>();
    DateTime dateTime3 = Convert.ToDateTime(this.txt_from_date.Value);
    DateTime dateTime4 = Convert.ToDateTime(this.txt_to_date.Value);
    bool is_weekend = false;
    bool is_holiday = false;
    foreach (long asset_id in bookableRooms)
    {
      bool flag3 = true;
      if (this.gp.isAdminType)
      {
        flag3 = true;
      }
      else
      {
        DataRow[] dataRowArray9 = assets.Tables[0].Select("asset_id='" + (object) asset_id + "'");
        if (dataRowArray9.Length > 0 && dataRowArray9[0]["asset_owner_group_id"].ToString() != "")
        {
          foreach (user_group userGroup in this.current_user.groups.Values)
          {
            if (userGroup.group_id == Convert.ToInt64(dataRowArray9[0]["asset_owner_group_id"]))
            {
              flag3 = true;
              goto label_40;
            }
          }
        }
        flag3 = this.can_book_asset(asset_id, dateTime3, dateTime4, dic_advance_booking_date, dic_book_holiday, dic_book_weekend, is_weekend, is_holiday, dic_op_start, dic_op_end);
      }
label_40:
      if (flag3)
        bookable_assets.Add(asset_id);
    }
    List<long> longList3 = this.bookings.check_availability(dateTime3.AddSeconds(1.0), dateTime4.AddSeconds(-1.0), this.current_user.account_id, bookable_assets);
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = "";
      foreach (long num2 in bookable_assets)
      {
        if (!longList3.Contains(num2))
        {
          DataRow[] dataRowArray10 = assets.Tables[0].Select("asset_id='" + (object) num2 + "'");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td><input type='checkbox' id='chk_" + (object) num2 + "'></td>");
          stringBuilder.Append("<td>" + dataRowArray10[0]["name"].ToString() + "</td>");
          stringBuilder.Append("<td>" + dataRowArray10[0]["capacity"].ToString() + "</td>");
          stringBuilder.Append("</tr>");
          str = str + "," + num2.ToString();
        }
      }
      this.html_rooms = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("check Availablility Inisde(assProDs,htmltable) : Error --> " + ex.ToString()));
    }
  }

  private bool can_book_asset(
    long asset_id,
    DateTime sDT,
    DateTime eDT,
    Dictionary<long, DateTime> dic_advance_booking_date,
    Dictionary<long, bool> dic_book_holiday,
    Dictionary<long, bool> dic_book_weekend,
    bool is_weekend,
    bool is_holiday,
    Dictionary<long, DateTime> dic_op_start,
    Dictionary<long, DateTime> dic_op_end)
  {
    bool flag = true;
    if (sDT > dic_advance_booking_date[asset_id] && eDT > dic_advance_booking_date[asset_id])
      flag = false;
    else if (is_holiday && !dic_book_holiday[asset_id])
      flag = false;
    else if (is_weekend && !dic_book_weekend[asset_id])
      flag = false;
    else if (sDT.Hour < dic_op_start[asset_id].Hour)
      flag = false;
    else if (sDT.Hour == dic_op_start[asset_id].Hour && sDT.Minute < dic_op_start[asset_id].Minute)
      flag = false;
    else if (eDT.Hour > dic_op_end[asset_id].Hour)
      flag = dic_op_end[asset_id].Day == 2;
    else if (eDT.Hour == dic_op_end[asset_id].Hour && eDT.Minute > dic_op_end[asset_id].Minute)
      flag = dic_op_end[asset_id].Day == 2;
    return flag;
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this);
}
