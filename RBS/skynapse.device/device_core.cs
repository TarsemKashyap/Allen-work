// Decompiled with JetBrains decompiler
// Type: skynapse.device.device_core
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace skynapse.device
{
  public class device_core
  {
    protected Guid account_id;
    protected Guid device_code;
    protected response_message _msg;
    protected booking_api bookings;
    protected device_api dapi;
    protected Dictionary<string, string> parameters;
    protected Dictionary<int, string> error_codes;
    protected account current_account;
    protected users_api uapi = new users_api();

    public device_core()
    {
      this.bookings = new booking_api();
      this._msg = new response_message();
      this.dapi = new device_api();
      this.parameters = new Dictionary<string, string>();
      this.populate_error_codes();
    }

    public void set_query_parameters(HttpRequest request)
    {
      foreach (string str in (NameObjectCollectionBase) request.QueryString)
      {
        try
        {
          this.parameters.Add(str, request.QueryString[str]);
        }
        catch
        {
          this.parameters.Add(str, "");
        }
      }
    }

    protected bool is_valid()
    {
      try
      {
        this.device_code = new Guid(this.parameters["dcode"]);
      }
      catch
      {
        this.device_code = Guid.Empty;
        return false;
      }
      this._msg.dcode = this.device_code;
      this.account_id = this.dapi.get_account_for_device(this.device_code);
      if (!(this.account_id != Guid.Empty))
        return false;
      this.current_account = this.uapi.get_account(this.account_id);
      return this.current_account.account_id != Guid.Empty;
    }

    private void populate_error_codes()
    {
      this.error_codes = new Dictionary<int, string>();
      this.error_codes.Add(0, "Invalid Device Id");
      this.error_codes.Add(101, "Invalid Request");
      this.error_codes.Add(102, "General Error");
      this.error_codes.Add(201, "Invalid Username");
      this.error_codes.Add(202, "Invalid Password");
      this.error_codes.Add(203, "Invalid Device Name");
      this.error_codes.Add(204, "Inactive User");
      this.error_codes.Add(205, "Blacklisted User");
      this.error_codes.Add(206, "User Does Not Exist");
      this.error_codes.Add(207, "Device Already Registered");
      this.error_codes.Add(208, "Failed to Register Device");
      this.error_codes.Add(301, "Invalid Asset Id");
      this.error_codes.Add(302, "Invalid Asset Id");
      this.error_codes.Add(303, "Failed to Get Configuration Data");
      this.error_codes.Add(401, "Invalid Booking Id");
      this.error_codes.Add(402, "Invalid User Id");
      this.error_codes.Add(403, "Invalid Asset Id");
      this.error_codes.Add(404, "Invalid From Date");
      this.error_codes.Add(405, "Past From Date Not Allowed");
      this.error_codes.Add(406, "Invalid To Date");
      this.error_codes.Add(407, "Past To Date Not Allowed");
      this.error_codes.Add(408, "To Date Earlier than From Date");
      this.error_codes.Add(409, "No Purpose Found");
      this.error_codes.Add(410, "Failed to Book");
      this.error_codes.Add(411, "Booking Conflict");
      this.error_codes.Add(501, "Invalid Action");
      this.error_codes.Add(502, "Invalid Username");
      this.error_codes.Add(503, "Invalid Password");
      this.error_codes.Add(504, "Invalid Asset Id");
      this.error_codes.Add(505, "Invalid Asset Id");
      this.error_codes.Add(506, "Inactive User");
      this.error_codes.Add(507, "Blacklisted User");
      this.error_codes.Add(508, "User Does Not Exist");
      this.error_codes.Add(509, "Invalid User");
      this.error_codes.Add(510, "Invalid Username/Password");
      this.error_codes.Add(601, "Invalid Action");
      this.error_codes.Add(602, "Invalid Booking Id");
      this.error_codes.Add(603, "Booking Id does not exist");
      this.error_codes.Add(701, "Invalid Asset Id");
      this.error_codes.Add(702, "Invalid Asset Id");
      this.error_codes.Add(703, "Invalid Year");
      this.error_codes.Add(704, "Zero Year");
      this.error_codes.Add(705, "Invalid Month");
      this.error_codes.Add(706, "Zero Month");
      this.error_codes.Add(707, "Invalid Day");
      this.error_codes.Add(708, "Invalid Date");
      this.error_codes.Add(709, "Invalid Results");
    }

    protected void set_error(int number)
    {
      this._msg.error = new response_error();
      this._msg.error.errcode = number.ToString();
      this._msg.error.errmsg = this.error_codes[number];
      this._msg.status = "0";
    }

    protected void send_data(string msg, HttpContext context)
    {
      context.Response.ContentType = "text/plain";
      context.Response.Write(msg);
      context.Response.End();
    }
  }
}
