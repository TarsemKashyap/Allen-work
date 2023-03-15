// Decompiled with JetBrains decompiler
// Type: administration_ajax_booking
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_ajax_booking : fbs_base_page, IRequiresSessionState
{
  private long booking_id;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(this.Request.QueryString["action"]) || string.IsNullOrWhiteSpace(this.Request.QueryString["booking_id"]))
        return;
      if (this.Request.QueryString["action"] == "actualize")
      {
        this.Response.Clear();
        this.Response.ClearHeaders();
        this.Response.ClearContent();
        this.booking_id = Convert.ToInt64(this.Request.QueryString["booking_id"]);
        if ((Convert.ToDateTime(this.Request.QueryString["book_to"]) - this.current_timestamp).TotalMinutes <= this.AllowedMinutes)
          return;
        if (this.bookings.actualize_booking(this.booking_id, this.current_user.account_id, this.utilities.TimeRoundUp(this.current_timestamp).ToString(api_constants.sql_datetime_format)))
        {
          this.Response.Write(this.utilities.TimeRoundUp(this.current_timestamp).ToString(api_constants.display_datetime_format));
          this.Response.Flush();
          this.Response.End();
        }
        else
        {
          this.Response.Write(" ");
          this.Response.Flush();
          this.Response.End();
        }
      }
      else
      {
        if (!(this.Request.QueryString["action"] == "update_noshow"))
          return;
        this.Response.Clear();
        this.Response.ClearHeaders();
        this.Response.ClearContent();
        this.booking_id = Convert.ToInt64(this.Request.QueryString["booking_id"]);
        DataSet bookingById = this.bookings.get_booking_by_id(this.booking_id, this.current_user.account_id);
        asset_booking assetBooking1 = new asset_booking();
        assetBooking1.booking_id = this.booking_id;
        assetBooking1.account_id = this.current_user.account_id;
        assetBooking1.modified_by = this.current_user.user_id;
        assetBooking1.record_id = new Guid(bookingById.Tables[0].Rows[0]["record_id"].ToString());
        assetBooking1.status = Convert.ToInt16(this.bookings.get_status("No Show"));
        asset_booking assetBooking2 = this.bookings.update_booking_status(assetBooking1);
        List<asset_booking> bookings = new List<asset_booking>();
        bookings.Add(assetBooking2);
        if (Convert.ToBoolean(this.current_account.properties["send_email"]))
          this.bookingsbl.send_booking_emails(bookings);
        this.Response.Write(assetBooking2.booking_id.ToString());
        this.Response.Flush();
        this.Response.End();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
