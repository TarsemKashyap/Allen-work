// Decompiled with JetBrains decompiler
// Type: resource_bookings_list
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class resource_bookings_list : fbs_base_page, IRequiresSessionState
{
  public string htmltable;
  public DataSet user_data;
  public string id = "";
  protected DropDownList ddl_res_type;
  protected DropDownList ddl_res_name;
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlAnchor sample_editable_1_new;
  protected Button btnExportExcel;
  protected HiddenField hdn_search;
  protected HiddenField hdn_orderby;
  protected HiddenField hdn_orderdir;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    long ticks = this.current_timestamp.Ticks;
    try
    {
      try
      {
        if (this.Request.QueryString["type"] == "delete")
        {
          this.resapi.delete_resource_bookings(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id, this.current_user.user_id);
          this.Response.Redirect(this.site_full_path + "bookings.aspx");
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) "Delete Resource Booking Item Error ->", ex);
      }
      if (!this.IsPostBack)
      {
        this.populate_res_type();
        this.txtFromDate.Text = this.current_timestamp.ToString(api_constants.display_datetime_format_short);
        this.txtToDate.Text = this.current_timestamp.AddDays(30.0).ToString(api_constants.display_datetime_format_short);
        this.populate_data();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    TimeSpan timeSpan = new TimeSpan(this.current_timestamp.Ticks - ticks);
    fbs_base_page.log.Info((object) ("Resource Bookings list time taken: " + timeSpan.TotalMilliseconds.ToString()));
  }

  private void populate_res_type()
  {
    this.ddl_res_type.Items.Clear();
    this.ddl_res_type.Items.Add(new ListItem()
    {
      Text = "select",
      Value = "select"
    });
    try
    {
      foreach (DataRow dataRow in this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, "resource_type", this.str_resource_module).Tables[0].Select("status > 0"))
        this.ddl_res_type.Items.Add(new ListItem()
        {
          Text = dataRow["value"].ToString(),
          Value = dataRow["setting_id"].ToString()
        });
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_resource_names()
  {
    this.ddl_res_name.Items.Clear();
    this.ddl_res_name.Items.Add(new ListItem()
    {
      Text = "select",
      Value = "select"
    });
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resource_items_by_item_type_id(Convert.ToInt64(this.ddl_res_type.SelectedItem.Value), this.current_user.account_id, this.str_resource_module).Tables[0].Rows)
        this.ddl_res_name.Items.Add(new ListItem()
        {
          Text = row["name"].ToString(),
          Value = row["item_id"].ToString()
        });
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_data()
  {
    try
    {
      DateTime dateTime = Convert.ToDateTime(this.txtFromDate.Text);
      DateTime to = Convert.ToDateTime(this.txtToDate.Text).AddDays(1.0).AddSeconds(-1.0);
      long resource_type_id;
      try
      {
        resource_type_id = Convert.ToInt64(this.ddl_res_type.SelectedItem.Value);
      }
      catch
      {
        resource_type_id = 0L;
      }
      long resource_id;
      try
      {
        resource_id = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
      }
      catch
      {
        resource_id = 0L;
      }
      DataSet bookingsByDateRange = this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, dateTime, to, resource_type_id, resource_id, this.str_resource_module);
      StringBuilder stringBuilder = new StringBuilder();
      if (this.utilities.isValidDataset(bookingsByDateRange))
      {
        DataRow[] dataRowArray1 = bookingsByDateRange.Tables[0].Select("status>0 and ( booked_for_id=" + (object) this.current_user.user_id + " or requested_by=" + (object) this.current_user.user_id + ")");
        if (dataRowArray1.Length > 0)
        {
          List<string> stringList = new List<string>();
          stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='booking_list_table'>");
          stringBuilder.Append("<thead>");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<th class='hidden-480'>Venue / Purpose</th>");
          stringBuilder.Append("<th class='hidden-480'>From </th>");
          stringBuilder.Append("<th class='hidden-480'>To</th>");
          stringBuilder.Append("<th class='hidden-480'>Items</th>");
          stringBuilder.Append("<th class='hidden-480' style='width:20%;'>Remarks</th>");
          stringBuilder.Append("<th class='hidden-480'>Action</th>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("</thead>");
          stringBuilder.Append("<tbody>");
          foreach (DataRow dataRow1 in dataRowArray1)
          {
            if (!stringList.Contains(dataRow1["resource_booking_id"].ToString()))
            {
              stringList.Add(dataRow1["resource_booking_id"].ToString());
              stringBuilder.Append("<tr><td><b>" + dataRow1["venue"].ToString() + "</b><br/> " + dataRow1["purpose"].ToString() + "</td>");
              stringBuilder.Append("<td>" + Convert.ToDateTime(dataRow1["from_date"]).ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder.Append("<td>" + Convert.ToDateTime(dataRow1["to_date"]).ToString(api_constants.display_datetime_format) + "</td>");
              DataRow[] dataRowArray2 = bookingsByDateRange.Tables[0].Select("resource_booking_id='" + dataRow1["resource_booking_id"].ToString() + "'");
              string str1 = "<table width='100%' cellpadding='10' cellspacing='10'>";
              foreach (DataRow dataRow2 in dataRowArray2)
                str1 = str1 + "<tr>" + "<td>" + dataRow2["name"].ToString() + "</td>" + "<td style='width:15%; text-align:right;'>" + dataRow2["accepted_qty"].ToString() + "</td></tr>";
              string str2 = str1 + "</table>";
              stringBuilder.Append("<td>" + str2 + "</td>");
              stringBuilder.Append("<td>" + dataRow1["remarks"].ToString() + "</td>");
              stringBuilder.Append("<td>");
              stringBuilder.Append("<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'>");
              stringBuilder.Append("<li><a href='javascript:eventClick(" + dataRow1["resource_booking_id"].ToString() + ")'><i class='icon-table'></i> View</a></li>");
              if (Convert.ToDateTime(dataRow1["from_date"]) > this.current_timestamp)
              {
                stringBuilder.Append("<li><a href='request_resources.aspx?resource_booking_id=" + dataRow1["resource_booking_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
                stringBuilder.Append("<li><a href='javascript:delete_resource_booking_item(" + dataRow1["resource_booking_item_id"].ToString() + ")'><i class='icon-trash'></i> Remove</a></li>");
              }
              stringBuilder.Append("</ul></div></div>");
              stringBuilder.Append("</td>");
              stringBuilder.Append("</tr>");
            }
          }
          stringBuilder.Append("</tbody>");
          stringBuilder.Append("</table>");
        }
      }
      else
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='booking_list_table'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>Resource Type</th>");
        stringBuilder.Append("<th class='hidden-480'>Resource Name</th>");
        stringBuilder.Append("<th class='hidden-480'>From </th>");
        stringBuilder.Append("<th class='hidden-480'>To</th>");
        stringBuilder.Append("<th class='hidden-480'>Purpose</th>");
        stringBuilder.Append("<th class='hidden-480'>Quantity</th>");
        stringBuilder.Append("<th class='hidden-480'>Action</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      this.htmltable = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
  }

  protected void btn_submit_Click(object sender, EventArgs e) => this.populate_data();

  protected void ddl_res_type_SelectedIndexChanged(object sender, EventArgs e) => this.populate_resource_names();

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      DateTime dateTime = Convert.ToDateTime(this.txtFromDate.Text);
      DateTime to = Convert.ToDateTime(this.txtToDate.Text).AddDays(1.0).AddSeconds(-1.0);
      long resource_type_id;
      try
      {
        resource_type_id = Convert.ToInt64(this.ddl_res_type.SelectedItem.Value);
      }
      catch
      {
        resource_type_id = 0L;
      }
      long resource_id;
      try
      {
        resource_id = Convert.ToInt64(this.ddl_res_name.SelectedItem.Value);
      }
      catch
      {
        resource_id = 0L;
      }
      DataSet bookingsByDateRange = this.resapi.get_resource_bookings_by_date_range(this.current_user.account_id, dateTime, to, resource_type_id, resource_id, this.str_resource_module);
      if (bookingsByDateRange.Tables[0].Select("status>0 and ( booked_for_id=" + (object) this.current_user.user_id + " or requested_by=" + (object) this.current_user.user_id + ")").Length <= 0)
        return;
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Resource_items_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='14'>Resource Bookings</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='14'></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Resource Type</td>");
      stringBuilder.Append("<td>Name</td>");
      stringBuilder.Append("<td>Purpose</td>");
      stringBuilder.Append("<td>Venue</td>");
      stringBuilder.Append("<td>From </td>");
      stringBuilder.Append("<td>To</td>");
      stringBuilder.Append("<td>Requested Quantity</td>");
      stringBuilder.Append("<td>Accepted Quantity</td>");
      stringBuilder.Append("<td>Booked For</td>");
      stringBuilder.Append("<td>Requestor</td>");
      stringBuilder.Append("<td>Email</td>");
      stringBuilder.Append("<td>Requestor Remarks</td>");
      stringBuilder.Append("<td>Remarks</td>");
      stringBuilder.Append("<td>Status</td>");
      stringBuilder.Append("</tr>");
      foreach (DataRow row in (InternalDataCollectionBase) bookingsByDateRange.Tables[0].Rows)
      {
        stringBuilder.Append("<tr class='odd gradeX'>");
        stringBuilder.Append("<td>" + row["resource_type"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["purpose"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["venue"].ToString() + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(row["from_Date"]).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>" + Convert.ToDateTime(row["to_Date"]).ToString(api_constants.display_datetime_format) + "</td>");
        stringBuilder.Append("<td>" + row["requested_qty"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["accepted_qty"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["booked_for"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["requestor"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["email"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["requestor_remarks"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["remarks"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["status"].ToString() + "</td>");
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td colspan='14'></td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Generated By </td>");
      stringBuilder.Append("<td colspan='13'>" + this.current_user.full_name + " </td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Generated on </td>");
      stringBuilder.Append("<td align='left' colspan='13'>" + this.current_timestamp.AddHours(this.current_user.timezone).ToString(api_constants.display_datetime_format) + "</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
