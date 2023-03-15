// Decompiled with JetBrains decompiler
// Type: administration_serverside_booking_info
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;

public class administration_serverside_booking_info : fbs_base_page, IRequiresSessionState
{
  private StringBuilder output = new StringBuilder();
  private StringBuilder outputInner = new StringBuilder();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.output.Append("{");
    try
    {
      if (this.current_user == null)
      {
        Modal.Close((Page) this);
        this.redirect_unauthorized();
      }
      this.output.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string searchStr = this.Request.Params["sSearch"];
      string orderby = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string str1 = this.Request.Params["fromdate"];
      string str2 = this.Request.Params["enddate"];
      switch (orderby)
      {
        case "0":
          orderby = "name";
          break;
        case "1":
          orderby = "BuildingName";
          break;
        case "2":
          orderby = "purpose";
          break;
        case "3":
          orderby = "book_from";
          break;
        case "4":
          orderby = "book_to";
          break;
        case "5":
          orderby = "RequestedBy";
          break;
        case "6":
          orderby = "status";
          break;
      }
      string building = "%";
      string level = "%";
      string category = "%";
      string type = "%";
      string requested_by = "%";
      string status = "%";
      if (!string.IsNullOrWhiteSpace(this.Request.Params["_building"]))
        building = this.Request.Params["_building"];
      if (!string.IsNullOrWhiteSpace(this.Request.Params["_level"]))
        level = this.Request.Params["_level"];
      if (!string.IsNullOrWhiteSpace(this.Request.Params["_category"]))
        category = this.Request.Params["_category"];
      if (!string.IsNullOrWhiteSpace(this.Request.Params["_type"]))
        type = this.Request.Params["_type"];
      if (!string.IsNullOrWhiteSpace(this.Request.Params["_requestedby"]))
        requested_by = this.Request.Params["_requestedby"];
      if (!string.IsNullOrWhiteSpace(this.Request.Params["_status"]))
        status = this.Request.Params["_status"];
      string from = "1900-01-01";
      string to = "2100-01-01";
      if (str1 != "")
        from = Convert.ToDateTime(str1).ToString(api_constants.sql_datetime_format_short) + " 00:00:00.000";
      if (str2 != "")
        to = Convert.ToDateTime(str2).ToString(api_constants.sql_datetime_format_short) + " 23:59:59.999";
      DataSet faultyAssetId = this.bookings.get_faulty_asset_id(this.current_user.account_id);
      DataSet dataSet = new DataSet();
      DataSet ds;
      if (this.gp.isSuperUserType)
      {
        string groupIds = this.utilities.get_group_ids(this.current_user);
        ds = !(searchStr == "") ? this.bookings.get_bookings_for_server((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, searchStr, orderdir, from, to, building, level, category, type, status, requested_by, this.current_user.account_id, groupIds, this.current_user.user_id, this.current_user.email) : this.bookings.get_bookings_for_server((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, searchStr, orderdir, from, to, building, level, category, type, status, requested_by, this.current_user.account_id, groupIds, this.current_user.user_id, this.current_user.email);
      }
      else
        ds = !(searchStr == "") ? this.bookings.get_bookings_for_server((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, searchStr, orderdir, from, to, building, level, category, type, status, requested_by, this.current_user.account_id, this.current_user.email) : this.bookings.get_bookings_for_server((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, searchStr, orderdir, from, to, building, level, category, type, status, requested_by, this.current_user.account_id, this.current_user.email);
      int num3 = 0;
      if (this.utilities.isValidDataset(ds))
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"" + ds.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"iTotalDisplayRecords\":\"" + ds.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
          {
            this.outputInner.Append("[");
            if (faultyAssetId.Tables[0].Select("asset_id=" + row["asset_id"].ToString()).Length > 0)
            {
              if (string.IsNullOrEmpty(row["code"].ToString()))
                this.outputInner.Append("\"" + row["name"].ToString() + " <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' />\",");
              else
                this.outputInner.Append("\"" + row["code"].ToString() + " / " + row["name"].ToString() + " <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' />\",");
            }
            else if (string.IsNullOrEmpty(row["code"].ToString()))
              this.outputInner.Append("\"" + row["name"].ToString() + "\",");
            else
              this.outputInner.Append("\"" + row["code"].ToString() + " / " + row["name"].ToString() + "\",");
            this.outputInner.Append("\"" + row["BuildingName"].ToString() + "\",");
            this.outputInner.Append("\"" + row["purpose"].ToString() + "\",");
            this.outputInner.Append("\"" + Convert.ToDateTime(row["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + Convert.ToDateTime(row["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + row["RequestedBy"].ToString() + "\",");
            if (row["status"].ToString() == "Booked")
              this.outputInner.Append("\"<Span class='label label-Booked'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "Cancelled")
              this.outputInner.Append("\"<Span class='label label-cancelled'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "Blocked")
              this.outputInner.Append("\"<Span class='label label-Blocked'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "No Show")
              this.outputInner.Append("\"<Span class='label label-NoShow'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "Pending")
              this.outputInner.Append("\"<Span class='label label-Pending'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "Rejected")
              this.outputInner.Append("\"<Span class='label label-rejected'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "Withdraw")
              this.outputInner.Append("\"<Span class='label label-withdrawan'>" + row["status"].ToString() + "</Span>\",");
            else if (row["status"].ToString() == "Auto Rejected")
              this.outputInner.Append("\"<Span class='label label-rejected'>" + row["status"].ToString() + "</Span>\",");
            if (row["status"].ToString() != "Blocked")
            {
              this.outputInner.Append("\"<div class='actions' id='action_" + row["booking_id"].ToString() + "'>");
              this.outputInner.Append("<div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              if (num3 == 0)
                this.outputInner.AppendFormat("<ul class='ddm p-r'><li><a href='javascript:callfancybox_for_view_booking({0})'><i class='icon-table'></i> View Booking</a></li><div class='altv' style='display:none;'>" + ds.Tables[1].Rows[0][0].ToString() + "</div>", (object) row["booking_id"].ToString());
              else
                this.outputInner.AppendFormat("<ul class='ddm p-r'><li><a href='javascript:callfancybox_for_view_booking({0})'><i class='icon-table'></i> View Booking</a></li>", (object) row["booking_id"].ToString());
              if (Convert.ToDateTime(row["book_to"].ToString()) > this.current_timestamp)
              {
                if (row["status"].ToString() == "Booked" || row["status"].ToString() == "Pending")
                {
                  this.outputInner.AppendFormat("<li><a href='javascript:call_resend_email({0})'><i class='icon-pencil'></i> Resend Email</a></li></div>", (object) row["booking_id"].ToString());
                  num3 = 1;
                }
              }
              else if (row["status"].ToString() == "Booked" || row["status"].ToString() == "Pending")
                this.outputInner.AppendFormat("<li><a href='javascript:call_resend_email({0})'><i class='icon-pencil'></i> Resend Email</a></li></div>", (object) row["booking_id"].ToString());
              this.outputInner.Append("</ul></div></div>\"");
            }
            else
            {
              this.outputInner.Append("\"<div class='actions' id='action_" + row["booking_id"].ToString() + "'>");
              this.outputInner.Append("<div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              if (num3 == 0)
                this.outputInner.AppendFormat("<ul class='ddm p-r'><li><a href='javascript:callfancybox_for_view_booking({0})'><i class='icon-table'></i> View Booking</a></li><div class='altv' style='display:none;'>" + ds.Tables[1].Rows[0][0].ToString() + "</div>", (object) row["booking_id"].ToString());
              else
                this.outputInner.AppendFormat("<ul class='ddm p-r'><li><a href='javascript:callfancybox_for_view_booking({0})'><i class='icon-table'></i> View Booking</a></li>", (object) row["booking_id"].ToString());
              this.outputInner.Append("</ul></div></div>\"");
            }
            this.outputInner.Append("],");
          }
          string str3 = this.outputInner.ToString();
          if (str3.Trim() != "")
            str3 = str3.TrimEnd(',');
          this.output.Append(str3);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serverside booking info admin panel error : " + ex.ToString()));
        }
        this.output.Append("]");
      }
      else
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"0\",");
        this.output.Append("\"iTotalDisplayRecords\":\"0\",");
        this.output.Append("\"aaData\":[");
        this.output.Append("]");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    this.output.Append("}");
    this.Response.Clear();
    this.Response.ClearHeaders();
    this.Response.ClearContent();
    this.Response.Write(this.output.ToString());
    this.Response.Flush();
    this.Response.End();
  }

  public bool CheckMeetingstart(long booking_id)
  {
    bool flag = false;
    try
    {
      DataSet dataSet = this.bookings.checkMeetingStarted(booking_id, this.current_user.account_id, Convert.ToDateTime(this.current_timestamp.ToString("dd-MMM-yyy hh:mm:ss tt")));
      flag = dataSet != null && dataSet.Tables[0].Rows.Count > 0;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    return flag;
  }

  public bool CheckMeetingsover(long booking_id) => this.bookings.CheckMeetingsover(booking_id, this.current_user.account_id, Convert.ToDateTime(this.current_timestamp.ToString("dd-MMM-yyy hh:mm:ss tt")));
}
