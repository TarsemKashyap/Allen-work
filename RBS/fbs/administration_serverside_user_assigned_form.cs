// Decompiled with JetBrains decompiler
// Type: administration_serverside_user_assigned_form
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

public class administration_serverside_user_assigned_form : fbs_base_page, IRequiresSessionState
{
  private StringBuilder html = new StringBuilder();
  private StringBuilder outerhtml = new StringBuilder();
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["user_groups"]))
      this.Server.Transfer("~//unauthorized.aspx");
    this.outerhtml.Append("{");
    try
    {
      this.outerhtml.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      long int64 = Convert.ToInt64(this.Request.Params["group_id"]);
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string searchkey = this.Request.Params["sSearch"];
      if (searchkey == "")
        searchkey = "%";
      string str1 = this.Request.Params["iSortCol_0"];
      string dir = this.Request.Params["sSortDir_0"];
      DataSet usersByGroup = this.reportings.get_users_by_group(num2 + 1, num2 + num1, dir, int64, this.current_user.account_id, searchkey);
      if (this.utilities.isValidDataset(usersByGroup))
      {
        this.outerhtml.Append(",");
        this.outerhtml.Append("\"iTotalRecords\":" + usersByGroup.Tables[1].Rows[0][0] + ",");
        this.outerhtml.Append("\"iTotalDisplayRecords\":" + usersByGroup.Tables[1].Rows[0][0] + ",");
        this.outerhtml.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) usersByGroup.Tables[0].Rows)
          {
            this.html.Append("[");
            this.html.AppendFormat("\"<input type='checkbox' class='checkboxes'  name='userids' value='{0}' />\",", (object) row["user_id"].ToString());
            this.html.Append("\" " + row["full_name"].ToString() + "\",");
            this.html.Append("\"  <a href='mailto:" + row["username"] + "'> " + row["username"] + " </a> \",");
            this.html.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
            this.html.Append("<ul class='ddm'>");
            this.html.Append("<li><a href='javascript:view(" + row["user_id"].ToString() + ")'><i class='icon-table'></i>View Details</a></li>");
            this.html.AppendFormat("<li><a href='javascript:remove({0},{1});'><i class='icon-table'></i>Remove from Group</a></li>", (object) row["user_id"].ToString(), (object) int64.ToString());
            this.html.Append("</ul>");
            this.html.Append("</div></div>\"");
            this.html.Append("],");
          }
          string str2 = this.html.ToString();
          if (str2 != "")
            str2 = str2.Substring(0, str2.Length - 1);
          this.outerhtml.Append(str2);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serverside user assigned form  error : " + ex.ToString()));
        }
        this.outerhtml.Append("]");
      }
      else
      {
        this.outerhtml.Append(",");
        this.outerhtml.Append("\"iTotalRecords\":\"0\",");
        this.outerhtml.Append("\"iTotalDisplayRecords\":\"0\",");
        this.outerhtml.Append("\"aaData\":[");
        this.outerhtml.Append("]");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
    this.outerhtml.Append("}");
    this.Response.Clear();
    this.Response.ClearHeaders();
    this.Response.ClearContent();
    this.Response.Write(this.outerhtml.ToString());
    this.Response.Flush();
    this.Response.End();
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
