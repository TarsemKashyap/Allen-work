// Decompiled with JetBrains decompiler
// Type: administration_content_list
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_content_list : fbs_base_page, IRequiresSessionState
{
  public StringBuilder html;
  public string show_notify = "";
  public string show_delete = "";
  protected HtmlAnchor sample_editable_1_new;
  protected DropDownList drp_year;
  protected DropDownList drp_Month;
  protected TextBox txt_search;
  protected Button Button1;
  protected Button btnExportExcel;
  protected HiddenField searchcon;
  protected HiddenField hidorderby;
  protected HiddenField hidorderdir;
  protected HiddenField hind_year;
  protected HiddenField hdn_report_start;
  protected HiddenField hdn_report_end;
  protected HiddenField hdn_daterange;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["announcements"]))
      this.Server.Transfer("~//unauthorized.aspx");
    this.html = new StringBuilder();
    try
    {
      if (!Convert.ToBoolean(ConfigurationManager.AppSettings["enable_announcements"]) || this.IsPostBack)
        return;
      this.generateyear();
      this.show_notify = string.IsNullOrEmpty(this.Request.QueryString["notify_I"]) ? "N" : (!(this.Request.QueryString["notify_I"].ToString() == "Y") ? "N" : "Y");
      this.show_delete = string.IsNullOrEmpty(this.Request.QueryString["notify_D"]) ? "N" : (!(this.Request.QueryString["notify_D"].ToString() == "Y") ? "N" : "Y");
      this.createtable();
    }
    catch (Exception ex)
    {
    }
  }

  public void createtable()
  {
    DateTime from = new DateTime();
    DateTime dateTime = new DateTime();
    DateTime to;
    if (this.drp_Month.SelectedItem.Value == "0")
    {
      from = new DateTime(Convert.ToInt32(this.drp_year.SelectedItem.Value), 1, 1);
      to = from.AddYears(1).AddDays(-1.0);
    }
    else
    {
      from = new DateTime(Convert.ToInt32(this.drp_year.SelectedItem.Value), Convert.ToInt32(this.drp_Month.SelectedItem.Value), 1);
      to = from.AddMonths(1).AddDays(-1.0);
    }
    DataSet contents = this.contents.get_contents(this.current_user.account_id, from, to, this.txt_search.Text);
    this.html = new StringBuilder();
    this.html.Append("<table class='table table-striped table-bordered table-hover' id='contentlist_table'>");
    this.html.Append("<thead>");
    this.html.Append("<tr>");
    this.html.Append("<th style='width:16%;' class='hidden-480'>Title</th>");
    this.html.Append("<th style='width:6%;' class='hidden-480'>From Date</th>");
    this.html.Append("<th style='width:6%;' class='hidden-480'>End Date</th>");
    this.html.Append("<th style='width:7%;' class='hidden-480'>Flag</th>");
    this.html.Append("<th style='width:7%;' class='hidden-480'>Published</th>");
    this.html.Append("<th style='width:7%;' class='hidden-480'>Last Modified By</th>");
    this.html.Append("<th style='width:7%;' class='hidden-480'>Last Modified On</th>");
    this.html.Append("<th style='width:3%;' class='hidden-480'>Action</th>");
    this.html.Append("</tr>");
    this.html.Append("</thead>");
    this.html.Append("<tbody>");
    foreach (DataRow row in (InternalDataCollectionBase) contents.Tables[0].Rows)
    {
      this.html.Append("<tr>");
      this.html.Append("<td>" + row["title"].ToString() + "</td>");
      this.html.Append("<td>" + Convert.ToDateTime(row["show_from"]).ToString(api_constants.display_datetime_format) + "</td>");
      this.html.Append("<td>" + Convert.ToDateTime(row["show_to"]).ToString(api_constants.display_datetime_format) + "</td>");
      if (Convert.ToBoolean(row["flag"]))
        this.html.Append("<td>Yes</td>");
      else
        this.html.Append("<td>No</td>");
      if (Convert.ToBoolean(row["published"]))
        this.html.Append("<td>Yes</td>");
      else
        this.html.Append("<td>No</td>");
      this.html.Append("<td>" + this.users.get_user_name(Convert.ToInt64(row["created_by"]), this.current_user.account_id) + "</td>");
      this.html.Append("<td>" + Convert.ToDateTime(row["modified_on"]).ToString(api_constants.display_datetime_format) + "</td>");
      this.html.Append("<td>");
      this.html.Append("<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
      this.html.Append("<ul class='ddm p-r'>");
      this.html.AppendFormat("<li><a href='content_form.aspx?content_id={0}'><i class='icon-pencil'></i> Edit</a></li>", (object) row["content_id"].ToString());
      this.html.AppendFormat("<li><a href='javascript:delete_content({0});'><i class='icon-trash'></i> Delete</a></li>", (object) row["content_id"].ToString());
      this.html.Append("</ul>");
      this.html.Append("</div></div>");
      this.html.Append("</td>");
      this.html.Append("</tr>");
    }
    this.html.Append("</tbody>");
    this.html.Append("</table>");
  }

  public void generateyear()
  {
    try
    {
      DataSet years = this.contents.get_years(this.current_user.account_id);
      this.drp_year.Items.Clear();
      if (years.Tables[0].Rows.Count == 0)
      {
        this.drp_year.Items.Add(new ListItem()
        {
          Text = this.current_timestamp.Year.ToString(),
          Value = this.current_timestamp.Year.ToString(),
          Selected = true
        });
      }
      else
      {
        foreach (DataRow row in (InternalDataCollectionBase) years.Tables[0].Rows)
        {
          ListItem listItem = new ListItem();
          listItem.Text = row["year"].ToString();
          listItem.Value = row["year"].ToString();
          if (Convert.ToInt32(row["year"]) == this.current_timestamp.Year)
            listItem.Selected = true;
          this.drp_year.Items.Add(listItem);
        }
      }
      foreach (ListItem listItem in this.drp_Month.Items)
      {
        if (Convert.ToInt32(listItem.Value) == this.current_timestamp.Month)
          listItem.Selected = true;
      }
    }
    catch
    {
    }
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      int month = 0;
      string search = !(this.searchcon.Value == "") ? this.searchcon.Value : "%";
      string str1 = Convert.ToDateTime(this.hdn_report_start.Value).ToString("dd-MMM-yyyy");
      string str2 = Convert.ToDateTime(this.hdn_report_end.Value).ToString("dd-MMM-yyyy");
      string from = str1 + " 00:00:00";
      string to = str2 + " 23:59:59";
      string str3 = this.hind_year.Value == "" ? "0" : this.hind_year.Value;
      string str4 = this.drp_Month.SelectedItem.Value == "" ? "0" : this.drp_Month.SelectedItem.Value;
      if (str4 != "0")
      {
        switch (str4)
        {
          case "Jan":
            month = 1;
            break;
          case "Feb":
            month = 2;
            break;
          case "Mar":
            month = 3;
            break;
          case "Apr":
            month = 4;
            break;
          case "May":
            month = 5;
            break;
          case "Jun":
            month = 6;
            break;
          case "Jul":
            month = 7;
            break;
          case "Aug":
            month = 8;
            break;
          case "Sep":
            month = 9;
            break;
          case "Oct":
            month = 10;
            break;
          case "Nov":
            month = 11;
            break;
          case "Dec":
            month = 12;
            break;
        }
      }
      if (str3 != "0")
      {
        if (str4 != "0")
        {
          int num = DateTime.DaysInMonth(Convert.ToInt32(str3), month);
          string str5 = Convert.ToDateTime(str4 + "01-" + str3).ToString("dd-MMM-yyyy");
          string str6 = Convert.ToDateTime(str4 + "-" + (object) num + "-" + str3).ToString("dd-MMM-yyyy");
          from = str5 + " 00:00:00";
          to = str6 + " 23:59:59";
        }
        else
        {
          from = "01-Jan-" + str3 + " 00:00:00";
          to = "30-Dec-" + str3 + " 23:59:59";
        }
      }
      else if (str4 != "0")
      {
        int num = DateTime.DaysInMonth(this.current_timestamp.Year, month);
        string str7 = Convert.ToDateTime(str4 + "-01-" + (object) this.current_timestamp.Year).ToString("dd-MMM-yyyy");
        string str8 = Convert.ToDateTime(str4 + "-" + (object) num + "-" + (object) this.current_timestamp.Year).ToString("dd-MMM-yyyy");
        from = str7 + " 00:00:00";
        to = str8 + " 23:59:59";
      }
      DataSet contentList = this.contents.get_content_list(this.current_user.account_id, from, to, search, "1", "1000000", this.hidorderby.Value, this.hidorderdir.Value);
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=Contentlist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th colspan='6'><h2>Notice & Content List</h2></th>");
      stringBuilder.Append("</tr>");
      if (str3 == "0" && str4 == "0")
      {
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th>Date Range</th>");
        stringBuilder.Append("<th>From </th>");
        stringBuilder.Append("<th>To</th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th>" + Convert.ToDateTime(this.hdn_report_start.Value).ToString("dd-MMM-yyyy") + "</th>");
        stringBuilder.Append("<th>" + Convert.ToDateTime(this.hdn_report_end.Value).ToString("dd-MMM-yyyy") + "</th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("</tr>");
      }
      else
      {
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th>Year</th>");
        stringBuilder.Append("<th>" + str3 + "</th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("<th></th>");
        stringBuilder.Append("</tr>");
        if (str4 != "0")
        {
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<th>Month</th>");
          stringBuilder.Append("<th>" + str4 + "</th>");
          stringBuilder.Append("<th></th>");
          stringBuilder.Append("<th></th>");
          stringBuilder.Append("<th></th>");
          stringBuilder.Append("<th></th>");
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Title</th>");
      stringBuilder.Append("<th>Content Data</th>");
      stringBuilder.Append("<th>Show From</th>");
      stringBuilder.Append("<th>Show To</th>");
      stringBuilder.Append("<th>Year</th>");
      stringBuilder.Append("<th>Repeatable</th>");
      stringBuilder.Append("</tr>");
      if (contentList != null)
      {
        foreach (DataRow row in (InternalDataCollectionBase) contentList.Tables[0].Rows)
        {
          stringBuilder.Append("<tr class='odd gradeX'>");
          stringBuilder.Append("<td>" + row["title"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["content_details"].ToString() + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["show_from"]).ToString("dd-MMM-yyyy") + "</td>");
          stringBuilder.Append("<td>" + Convert.ToDateTime(row["show_to"]).ToString("dd-MMM-yyyy") + "</td>");
          stringBuilder.Append("<td>" + row["Year"].ToString() + "</td>");
          stringBuilder.Append("<td>" + row["repeatable"].ToString() + "</td>");
          stringBuilder.Append("</tr>");
        }
      }
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Generated By </th>");
      stringBuilder.Append("<td>" + this.current_user.full_name + " </td>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Generated on </th>");
      stringBuilder.Append("<td align='left'>" + this.current_timestamp.AddHours(this.current_user.timezone).ToString("dd-MMM-yyyy hh:mm tt") + "</td>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th> </th>");
      stringBuilder.Append("<th></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.Response.Write(stringBuilder.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
    }
  }

  protected void Button1_Click(object sender, EventArgs e) => this.createtable();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
