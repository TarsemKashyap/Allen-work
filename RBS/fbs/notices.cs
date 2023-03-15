// Decompiled with JetBrains decompiler
// Type: notices
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
using System.Web.UI.WebControls;

public class notices : fbs_base_page, IRequiresSessionState
{
  protected Label lblDateRage;
  protected Button btn_filter;
  protected HiddenField hdn_report_start;
  protected HiddenField hdn_report_end;
  protected HiddenField hdn_daterange;
  public StringBuilder html_builder = new StringBuilder();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      string str1 = this.hdn_report_start.Value;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        string str2 = str1 + " 00:00:00.000";
      }
      else
        this.hdn_report_start.Value = this.current_timestamp.AddDays(-7.0).ToString("yyyy-MM-dd") + " 00:00:00.000";
      string str3 = this.hdn_report_end.Value;
      if (!string.IsNullOrWhiteSpace(str3))
      {
        string str4 = str3 + " 23:59:59.999";
      }
      else
        this.hdn_report_end.Value = this.current_timestamp.ToString("yyyy-MM-dd") + " 23:59:59.999";
      this.hdn_daterange.Value = Convert.ToDateTime(this.hdn_report_start.Value).ToString("MMMM d, yyyy") + " - " + Convert.ToDateTime(this.hdn_report_end.Value).ToString("MMMM d, yyyy");
      this.Load_announcements(this.hdn_report_start.Value, this.hdn_report_end.Value);
      this.contents.update_view(this.current_user.user_id, this.current_user.account_id);
    }
    catch (Exception ex)
    {
    }
  }

  public void Load_announcements(string from, string to)
  {
    this.html_builder = new StringBuilder();
    DataSet pushlishedContentList = this.contents.get_pushlished_content_list(this.current_user.account_id, from, to);
    if (pushlishedContentList != null)
    {
      if (pushlishedContentList.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) pushlishedContentList.Tables[0].Rows)
        {
          this.html_builder.Append("<div class='news-blocks'>");
          this.html_builder.Append("<h3>" + row["title"].ToString() + "</h3>");
          this.html_builder.Append("<div class='news-block-tags'>");
          if (Convert.ToBoolean(row["flag"]))
            this.html_builder.Append("<span class='label label-Pending'>IMPORTANT <span>");
          this.html_builder.Append("<em>" + Convert.ToDateTime(row["show_from"].ToString()).ToString("dd-MMM-yyyy") + "</em></div>");
          this.html_builder.Append("<p>" + row["content_details"].ToString() + "</p>");
          this.html_builder.Append("</div>");
        }
      }
      else
        this.html_builder.Append("<div style='padding:5px;'><span class='label label-cancelled'>" + Resources.fbs.norecords_noticeandannouncements + "</span></div>");
    }
    else
      this.html_builder.Append("<div style='padding:5px;'><span class='label label-cancelled'>" + Resources.fbs.norecords_noticeandannouncements + "</span></div>");
  }

  protected void btn_filter_Click(object sender, EventArgs e)
  {
    try
    {
      this.Load_announcements(this.hdn_report_start.Value, this.hdn_report_end.Value);
    }
    catch (Exception ex)
    {
    }
  }
}
