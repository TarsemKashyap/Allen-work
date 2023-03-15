// Decompiled with JetBrains decompiler
// Type: administration_serversideBlockDate
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
using System.Web.UI.HtmlControls;

public class administration_serversideBlockDate : fbs_base_page, IRequiresSessionState
{
  private StringBuilder output = new StringBuilder();
  private StringBuilder outputInner = new StringBuilder();
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      try
      {
        int int32 = Convert.ToInt32(this.Request.QueryString["Booking_ID"]);
        long int64 = Convert.ToInt64(this.Request.QueryString["Asset_ID"]);
        if (this.Request.Params["Action"] != null)
        {
          if (this.Request.Params["Action"] == "delete")
          {
            DataSet bookings = this.bookings.get_bookings(int64, (long) int32, this.current_user.account_id);
            asset_booking assetBooking = new asset_booking();
            if (bookings != null)
            {
              assetBooking.booking_id = (long) int32;
              assetBooking.account_id = (Guid) bookings.Tables[0].Rows[0]["account_id"];
              assetBooking.modified_by = this.current_user.user_id;
              assetBooking.record_id = (Guid) bookings.Tables[0].Rows[0]["record_id"];
            }
            this.bookings.delete_booking(assetBooking);
          }
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ("serverside blockdate delete error : " + ex.ToString()));
      }
    }
    this.output.Append("{");
    try
    {
      this.output.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string searchkey = this.Request.Params["sSearch"];
      string orderby = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string str1 = !(this.Request.Params["From"] != "") ? DateTime.Today.ToString(api_constants.sql_datetime_format_short) : Convert.ToDateTime(this.Request.Params["From"]).ToString(api_constants.sql_datetime_format_short);
      string str2 = !(this.Request.Params["To"] != "") ? DateTime.Today.AddMonths(11).ToString(api_constants.sql_datetime_format_short) : Convert.ToDateTime(this.Request.Params["To"]).ToString(api_constants.sql_datetime_format_short);
      string str3 = this.Request.Params["Status"];
      string str4 = this.Request.Params["Export"];
      string from = !(str1 != "") ? this.current_timestamp.ToString(api_constants.sql_datetime_format_short) + " 00:00:00:00" : str1 + " 00:00:00:00";
      string to = !(str2 != "") ? this.current_timestamp.ToString(api_constants.sql_datetime_format_short) + " 23:59:59:59" : str2 + " 23:59:59:59";
      switch (orderby)
      {
        case "0":
          orderby = "remarks";
          break;
        case "1":
          orderby = "book_from";
          break;
        case "2":
          orderby = "book_to";
          break;
        case "3":
          orderby = "Created_by";
          break;
      }
      int int32 = Convert.ToInt32(api_constants.booking_status["Blocked"]);
      long int64 = Convert.ToInt64(this.Request.QueryString["Asset_ID"]);
      DataSet bookings = this.bookings.get_bookings((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, orderdir, searchkey, int64, this.current_user.account_id, int32, from, to);
      bool flag = true;
      if (this.utilities.isValidDataset(bookings))
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"" + bookings.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"iTotalDisplayRecords\":\"" + bookings.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) bookings.Tables[0].Rows)
          {
            this.outputInner.Append("[");
            this.outputInner.Append("\"" + row["remarks"] + "\",");
            this.outputInner.Append("\"" + Convert.ToDateTime(row["book_from"]).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + Convert.ToDateTime(row["book_to"]).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + row["full_name"] + "\",");
            this.outputInner.Append("\"<div class='actions'><div class='bgp'>");
            this.outputInner.Append("<a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
            this.outputInner.Append("<ul class='ddm p-r'>");
            this.outputInner.AppendFormat("<li><a onclick='javascript:Edit_BlockDate({0},{1});'><i class='icon-pencil'></i>Edit</a></li>", (object) row["booking_id"].ToString(), (object) this.Request.QueryString["asset_id"]);
            this.outputInner.AppendFormat("<li><a onclick='javascript:delete_blockdate({0},{1});'><i class='icon-trash'></i> Delete</a></li>", (object) row["booking_id"].ToString(), (object) this.Request.QueryString["asset_id"]);
            this.outputInner.Append("</ul>");
            if (flag)
            {
              this.outputInner.Append("</div><div class='altv'  style='display:none;' >" + bookings.Tables[1].Rows[0][0].ToString() + "</div></div>\"");
              flag = false;
            }
            else
            {
              flag = false;
              this.outputInner.Append("</div></div>\"");
            }
            this.outputInner.Append("],");
          }
          string str5 = this.outputInner.ToString();
          if (str5 != "")
            str5 = str5.Substring(0, str5.Length - 1);
          this.output.Append(str5);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serversider blockdate error : " + ex.ToString()));
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
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    this.output.Append("}");
    this.Response.Clear();
    this.Response.ClearHeaders();
    this.Response.ClearContent();
    this.Response.Write(this.output.ToString());
    this.Response.Flush();
    this.Response.End();
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
