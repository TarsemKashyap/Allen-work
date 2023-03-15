// Decompiled with JetBrains decompiler
// Type: controls_asset_edit_blocked_dates
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.UI.WebControls;

public class controls_asset_edit_blocked_dates : fbs_base_user_control
{
  public string htmltable = "";
  public int boockedvalue;
  public long asset_idblockdate;
  protected HiddenField hdn_start;
  protected HiddenField hdn_end;
  protected HiddenField hdn_bookingid;
  protected HiddenField hdn_asset_id;
  protected HiddenField hdn_totalrecords;
  protected HiddenField hdn_searchcon_block;
  protected Button btn_exporttoexcel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      this.btn_exporttoexcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      if (this.IsPostBack)
        return;
      this.boockedvalue = Convert.ToInt32(api_constants.booking_status["Blocked"]);
      this.asset_idblockdate = Convert.ToInt64(this.Request.QueryString["asset_id"]);
      this.hdn_bookingid.Value = this.boockedvalue.ToString();
      this.hdn_asset_id.Value = this.asset_idblockdate.ToString();
      this.pageload_Data();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_Data()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("<tr class='odd'><td valign='top' colspan='7' class='dataTables_empty'>No data available in table</td></tr>");
      this.htmltable = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect(this.Request.UrlReferrer.ToString());
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
  }

  protected void Unnamed1_Click(object sender, EventArgs e)
  {
    try
    {
      DataSet dataSet1 = new DataSet();
      string str1 = Convert.ToDateTime(this.hdn_start.Value).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.hdn_end.Value).ToString(api_constants.sql_datetime_format_short);
      long int64 = Convert.ToInt64(this.Request.QueryString["Asset_ID"]);
      string from = str1 + " 00:00:00:00";
      string to = str2 + " 23:59:59:59";
      int int32 = Convert.ToInt32(api_constants.booking_status["Blocked"]);
      DataSet dataSet2 = new DataSet();
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      DataSet bookings = this.bookings.get_bookings("1", !(this.hdn_totalrecords.Value != "") ? "1" : this.hdn_totalrecords.Value, "book_from", "Asc", !(this.hdn_searchcon_block.Value != "") ? "%" : this.hdn_searchcon_block.Value, int64, this.current_user.account_id, int32, from, to);
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_BlockDateList_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder3 = new StringBuilder();
      stringBuilder3.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder3.Append("<tbody>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th style='font-size:16pt;'>BlockDate List</th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th></th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th></th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th>Date Range</th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("<tr class='odd gradeX'>");
      stringBuilder3.Append("<td> " + this.hdn_start.Value + "&" + this.hdn_end.Value + "</td>");
      stringBuilder3.Append("<td>  </td>");
      stringBuilder3.Append("<td>  </td>");
      stringBuilder3.Append("<td>  </td>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th></th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("</tbody>");
      stringBuilder3.Append("</table>");
      stringBuilder3.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder3.Append("<tbody>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th>Remarks</th>");
      stringBuilder3.Append("<th>Email From</th>");
      stringBuilder3.Append("<th>Email To</th>");
      stringBuilder3.Append("<th>Requested By</th>");
      stringBuilder3.Append("</tr>");
      if (bookings == null)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) bookings.Tables[0].Rows)
      {
        stringBuilder3.Append("<tr class='odd gradeX'>");
        stringBuilder3.Append("<td>" + row["remarks"] + "</td>");
        stringBuilder3.Append("<td>" + row["book_from"] + "</td>");
        stringBuilder3.Append("<td>" + row["book_to"] + "</td>");
        stringBuilder3.Append("<td>" + row["full_name"] + "</td>");
      }
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th></th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th>Generated By </th>");
      stringBuilder3.Append("<td>" + this.current_user.full_name + " </td>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("<tr>");
      stringBuilder3.Append("<th>Generated on </th>");
      stringBuilder3.Append("<td align='left'>" + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format) + "</td>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("<th> </th>");
      stringBuilder3.Append("</tr>");
      stringBuilder3.Append("</tbody>");
      stringBuilder3.Append("</table>");
      this.Response.Write(stringBuilder3.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
