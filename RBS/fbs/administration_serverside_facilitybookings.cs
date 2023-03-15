// Decompiled with JetBrains decompiler
// Type: administration_serverside_facilitybookings
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

public class administration_serverside_facilitybookings : fbs_base_page, IRequiresSessionState
{
  protected HtmlForm form1;
  private StringBuilder output = new StringBuilder();
  private StringBuilder outputInner = new StringBuilder();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.output.Append("{");
    try
    {
      this.output.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string str1 = this.Request.Params["sSearch"];
      string searchkey = !(str1 == "") ? str1 + "%" : "%";
      string orderby = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string str2 = string.IsNullOrWhiteSpace(this.Request.Params["From"]) ? Convert.ToDateTime(this.current_timestamp).ToString(api_constants.sql_datetime_format_short) : Convert.ToDateTime(this.Request.Params["From"]).ToString(api_constants.sql_datetime_format_short);
      string str3 = string.IsNullOrWhiteSpace(this.Request.Params["From"]) ? Convert.ToDateTime(this.current_timestamp).ToString(api_constants.sql_datetime_format_short) : Convert.ToDateTime(this.Request.Params["To"]).ToString(api_constants.sql_datetime_format_short);
      string str4 = this.Request.Params["Asset_ID"];
      string from = str2 + " 00:00:00:00";
      string to = str3 + " 23:59:59:59";
      switch (orderby)
      {
        case "0":
          orderby = "t2.book_from";
          break;
        case "1":
          orderby = "t2.book_to";
          break;
        case "2":
          orderby = "t2.booked_for";
          break;
        case "3":
          orderby = "t2.status";
          break;
      }
      DataSet dataSet = new DataSet();
      DataSet bookings = this.bookings.get_bookings((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, orderdir, searchkey, Convert.ToInt64(str4), this.current_user.account_id, from, to);
      if (this.utilities.isValidDataset(bookings))
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"" + bookings.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"iTotalDisplayRecords\":\"" + bookings.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"aaData\":[");
        try
        {
          int num3 = 0;
          foreach (DataRow row in (InternalDataCollectionBase) bookings.Tables[0].Rows)
          {
            this.outputInner.Append("[");
            this.outputInner.Append("\"" + Convert.ToDateTime(row["book_from"].ToString()).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + Convert.ToDateTime(row["book_to"].ToString()).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + row["RequestedBy"] + "\",");
            switch (row["status"].ToString())
            {
              case "0":
                this.outputInner.Append("\"<Span class='label label-cancelled'>Cancelled</Span>\",");
                break;
              case "1":
                this.outputInner.Append("\"<Span class='label label-Booked'>Booked</Span>\",");
                break;
              case "2":
                this.outputInner.Append("\"<Span class='label label-Blocked'>Blocked</Span>\",");
                break;
              case "3":
                this.outputInner.Append("\"<Span class='label label-NoShow'>No Show</Span>\",");
                break;
              case "4":
                this.outputInner.Append("\"<Span class='label label-Pending'>Pending</Span>\",");
                break;
              case "5":
                this.outputInner.Append("\"<Span class='label label-withdrawan'>Withdrwal</Span>\",");
                break;
              case "6":
                this.outputInner.Append("\"<Span class='label label-rejected'>Rejected</Span>\",");
                break;
              case "7":
                this.outputInner.Append("\"<Span class='label label-rejected'>Auto Rejected</Span>\",");
                break;
            }
            string str5 = "<div class='actions' id='action_" + row["booking_id"].ToString() + "'><div  class='bgp dropup'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>" + "<ul class='ddm p-r'>" + "<li><a href='javascript:callfancybox_for_view_booking(" + row["booking_id"].ToString() + ")'><i class='icon-table'></i> View Booking</a></li>";
            if (!(row["status"].ToString() == "1"))
            {
              int num4 = row["status"].ToString() == "4" ? 1 : 0;
            }
            if (num3 == 0)
            {
              if (!(row["status"].ToString() == "1"))
              {
                int num5 = row["status"].ToString() == "4" ? 1 : 0;
              }
              num3 = 1;
            }
            else if (!(row["status"].ToString() == "1"))
            {
              int num6 = row["status"].ToString() == "4" ? 1 : 0;
            }
            this.outputInner.Append("\"" + (str5 + "</ul>" + "</div><div class='altv' style='display:none;'>" + bookings.Tables[1].Rows[0][0].ToString() + "</div></div>").TrimEnd() + "\"");
            this.outputInner.Append("],");
          }
          string str6 = this.outputInner.ToString();
          if (str6 != "")
            str6 = str6.Substring(0, str6.Length - 1);
          this.output.Append(str6);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serverside facilitybookings admin panel error : " + ex.ToString()));
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
}
