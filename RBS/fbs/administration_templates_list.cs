// Decompiled with JetBrains decompiler
// Type: administration_templates_list
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

public class administration_templates_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public string status = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["templates"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.templates_view)
      this.redirect_unauthorized();
    try
    {
      if (this.IsPostBack)
        return;
      this.populate_ui(this.tapi.get_templates(this.current_user.account_id));
      if (this.Session["temp_edit"] == null || !(this.Session["temp_edit"].ToString() == "S"))
        return;
      this.status = "S";
      this.Session.Remove("temp_edit");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_ui(DataSet data)
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='templatelist_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>Name</td>");
      stringBuilder.Append("<th class='hidden-480'>Email Title</th>");
      stringBuilder.Append("<th class='hidden-480'>Description of Template</th>");
      stringBuilder.Append("<th class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      foreach (DataRow row in (InternalDataCollectionBase) data.Tables[0].Rows)
      {
        stringBuilder.Append("<tr class='odd gradeX'>");
        stringBuilder.Append("<td>" + row["name"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["title"].ToString() + "</td>");
        stringBuilder.Append("<td>" + row["description"].ToString() + "</td>");
        stringBuilder.Append("<td><div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
        if (this.gp.templates_edit)
        {
          stringBuilder.Append("<ul class='ddm p-r'>");
          stringBuilder.Append("<li><a href='template_edit.aspx?templateID=" + row["template_id"] + "'><i class='icon-pencil'></i> Edit</a></li>");
          stringBuilder.Append("</ul>");
        }
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
}
