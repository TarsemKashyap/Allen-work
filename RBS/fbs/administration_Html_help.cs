// Decompiled with JetBrains decompiler
// Type: administration_Html_help
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

public class administration_Html_help : fbs_base_page, IRequiresSessionState
{
  public string html_table = "";
  public string help_sucess = "";

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["help"] != null && this.Session["help"] == (object) "yes")
      {
        this.help_sucess = "yes";
        this.Session.Remove("help");
      }
      if (this.IsPostBack)
        return;
      this.page_loadDatat();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void page_loadDatat()
  {
    try
    {
      DataSet help = this.help.get_help(this.current_user.account_id);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='helplist_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th style='width:25%;' class='hidden-480'>Page</th>");
      stringBuilder.Append("<th style='width:70%;' class='hidden-480'>Description</th>");
      stringBuilder.Append("<th style='width:5%;' class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      foreach (DataRow row in (InternalDataCollectionBase) help.Tables[0].Rows)
      {
        stringBuilder.Append("<tr class='odd gradeX'>");
        stringBuilder.Append("<td>" + row["page_name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["description"].ToString() + "</td>");
        stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
        stringBuilder.Append("<ul class='ddm p-r'>");
        stringBuilder.Append("<li><a href='html_help_form.aspx?id=" + row["help_id"] + "'><i class='icon-pencil'></i> Edit</a></li>");
        stringBuilder.Append("</ul>");
        stringBuilder.Append("</div></div></td>");
        stringBuilder.Append("</tr>");
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
