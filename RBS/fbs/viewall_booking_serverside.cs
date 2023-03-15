// Decompiled with JetBrains decompiler
// Type: viewall_booking_serverside
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

public class viewall_booking_serverside : fbs_base_page, IRequiresSessionState
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
      string searchStr = this.Request.Params["sSearch"];
      string orderby = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string str1 = this.Request.Params["fromdate"];
      string str2 = this.Request.Params["enddate"];
      switch (orderby)
      {
        case "0":
          orderby = "code";
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
        case "7":
          orderby = "comment";
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
      if (searchStr == "")
        searchStr = "%";
      DataSet dataSet = new DataSet();
      DataSet bookingsForServer = this.bookings.get_bookings_for_server((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, searchStr, orderdir, from, to, building, level, category, type, status, requested_by, this.current_user.account_id, this.current_user.email);
      if (this.utilities.isValidDataset(bookingsForServer))
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"" + bookingsForServer.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"iTotalDisplayRecords\":\"" + bookingsForServer.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) bookingsForServer.Tables[0].Rows)
          {
            this.outputInner.Append("[");
            if (row["status"].ToString() == "Pending" || row["status"].ToString() == "Booked")
            {
              if (row["comment"].ToString() != "")
              {
                if (string.IsNullOrEmpty(row["code"].ToString()))
                  this.outputInner.Append("\"" + row["name"].ToString() + "<img id='img_prop' style='float:right;' src='assets/img/Facilityerro.png' alt='Faulty Room' />\",");
                else
                  this.outputInner.Append("\"" + row["code"].ToString() + " / " + row["name"].ToString() + "<img id='img_prop' style='float:right;' src='assets/img/Facilityerro.png' alt='Faulty Room' />\",");
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
              else if (row["status"].ToString() == "Pending")
                this.outputInner.Append("\"<Span class='label label-Pending'>" + row["status"].ToString() + "</Span>\",");
              this.outputInner.Append("\"" + row["comment"].ToString() + "\",");
              this.outputInner.Append("\"<a href='javascript:eventClick(" + row["booking_id"].ToString() + ")'>View</a>\"");
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
          fbs_base_page.log.Error((object) ("serverside viewall bookings front panel error : " + ex.ToString()));
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
      fbs_base_page.log.Error((object) ("serverside asset list - front panel error : " + ex.ToString()));
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
