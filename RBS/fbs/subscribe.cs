// Decompiled with JetBrains decompiler
// Type: subscribe
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;

public class subscribe : Page, IRequiresSessionState
{
  private long meeting_type_id;
  private string title = "";
  private string type = "";
  private string site_path = ConfigurationManager.AppSettings["site_full_path"].ToString();
  private Dictionary<long, string> room_list;
  public static string display_datetime_format = ConfigurationManager.AppSettings["date_time_long"];

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.meeting_type_id = Convert.ToInt64(this.Request.QueryString["id"]);
    }
    catch
    {
    }
    try
    {
      this.title = this.Request.QueryString["title"].ToString();
    }
    catch
    {
    }
    try
    {
      this.type = this.Request.QueryString["type"].ToString();
    }
    catch
    {
      this.type = "rss";
    }
    if (this.meeting_type_id <= 0L)
      return;
    DataAccess dataAccess = new DataAccess(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
    this.room_list = new Dictionary<long, string>();
    DataSet dataSet = new DataSet();
    if (dataAccess.get_dataset("select asset_id,code,name from sbt_assets"))
      dataSet = dataAccess.resultDataSet;
    foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
      this.room_list.Add(Convert.ToInt64(row["asset_id"].ToString()), row["code"].ToString() + " - " + row["name"].ToString());
    DataSet data = new DataSet();
    if (dataAccess.get_dataset("select booking_id,purpose,book_from,book_to,asset_id from sbt_asset_bookings where meeting_type='" + (object) this.meeting_type_id + "' and status='1' and book_from>='" + DateTime.Today.ToString("yyyy-MM-dd 00:00:00") + "'"))
      data = dataAccess.resultDataSet;
    if (this.type == "rss")
      this.do_rss(data, this.room_list);
    else
      this.do_ical(data, this.room_list);
  }

  private void do_rss(DataSet data, Dictionary<long, string> room_list)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<?xml version='1.0' encoding='utf-8' ?><rss version='2.0'>");
    stringBuilder.Append("<channel>");
    stringBuilder.Append("<title>Facilities Booking System Feed - " + this.title + "</title>");
    stringBuilder.Append("<link>" + this.site_path + "</link>");
    stringBuilder.Append("<description></description>");
    foreach (DataRow row in (InternalDataCollectionBase) data.Tables[0].Rows)
    {
      stringBuilder.Append("<item>");
      stringBuilder.Append("<title>" + row["purpose"].ToString() + "</title>");
      stringBuilder.Append("<link>" + this.site_path + "view_booking.aspx?id=" + row["booking_id"].ToString() + "</link>");
      stringBuilder.Append("<description><p>" + room_list[Convert.ToInt64(row["asset_id"])] + "</p><p>" + Convert.ToDateTime(row["book_from"]).ToString(api_constants.display_datetime_format) + " - " + Convert.ToDateTime(row["book_to"]).ToString(api_constants.display_datetime_format) + "</p></description>");
      stringBuilder.Append("</item>");
    }
    stringBuilder.Append("</channel>");
    stringBuilder.Append("</rss>");
    this.Response.Write(stringBuilder.ToString());
  }

  private void do_ical(DataSet data, Dictionary<long, string> room_list)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("BEGIN:VCALENDAR");
    stringBuilder.Append("VERSION:2.0");
    stringBuilder.Append("CALSCALE:GREGORIAN");
    stringBuilder.Append("METHOD:PUBLISH");
    stringBuilder.Append("X-WR-CALNAME:" + this.title);
    stringBuilder.Append("X-WR-TIMEZONE:Asia/Singapore");
    stringBuilder.Append("BEGIN:VTIMEZONE");
    stringBuilder.Append("TZID:Asia/Singapore");
    stringBuilder.Append("X-LIC-LOCATION:Asia/Singapore");
    stringBuilder.Append("BEGIN:STANDARD");
    stringBuilder.Append("TZOFFSETFROM:+0800");
    stringBuilder.Append("TZOFFSETTO: +0800");
    stringBuilder.Append("TZNAME: SGT");
    stringBuilder.Append("DTSTART:19700101T000000");
    stringBuilder.Append("END:STANDARD");
    stringBuilder.Append("END:VTIMEZONE");
    foreach (DataRow row in (InternalDataCollectionBase) data.Tables[0].Rows)
    {
      stringBuilder.Append("BEGIN: VEVENT");
      stringBuilder.Append("DTSTART; VALUE = DATE:" + Convert.ToDateTime(row["book_from"]).ToString("yyyyMMddTHHmmssZ"));
      stringBuilder.Append("DTEND; VALUE = DATE:" + Convert.ToDateTime(row["book_to"]).ToString("yyyyMMddTHHmmssZ"));
      stringBuilder.Append("UID:" + Guid.NewGuid().ToString());
      stringBuilder.Append("DESCRIPTION:");
      stringBuilder.Append("LOCATION:" + room_list[Convert.ToInt64(row["asset_id"])]);
      stringBuilder.Append("STATUS: CONFIRMED");
      stringBuilder.Append("SUMMARY:" + row["purpose"].ToString());
      stringBuilder.Append("BEGIN:VALARM");
      stringBuilder.Append("ACTION:DISPLAY");
      stringBuilder.Append("DESCRIPTION:This is an event reminder");
      stringBuilder.Append("TRIGGER:-P0DT7H10M0S");
      stringBuilder.Append("END:VALARM");
      stringBuilder.Append("BEGIN:VALARM");
      stringBuilder.Append("ACTION:DISPLAY");
      stringBuilder.Append("DESCRIPTION:This is an event reminder");
      stringBuilder.Append("TRIGGER:-P0DT0H10M0S");
      stringBuilder.Append("END:VALARM");
      stringBuilder.Append("END:VEVENT");
    }
    stringBuilder.Append("END:VCALENDAR");
    this.Response.Write(stringBuilder.ToString());
  }
}
