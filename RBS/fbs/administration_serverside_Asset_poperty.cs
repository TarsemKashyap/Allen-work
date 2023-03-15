// Decompiled with JetBrains decompiler
// Type: administration_serverside_Asset_poperty
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;

public class administration_serverside_Asset_poperty : fbs_base_page, IRequiresSessionState
{
  private StringBuilder html = new StringBuilder();
  private StringBuilder outerhtml = new StringBuilder();
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.outerhtml.Append("{");
    try
    {
      this.outerhtml.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      string str1 = this.Request.QueryString["asset_id"];
      long int64 = Convert.ToInt64(str1);
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string str2 = this.Request.Params["sSearch"];
      if (str2 == "")
        str2 = "%";
      string str3 = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string orderby = "";
      switch (str3)
      {
        case "0":
          orderby = "Value";
          break;
        case "1":
          orderby = "Value";
          break;
        case "2":
          orderby = "Remarks";
          break;
      }
      DataSet assetPropertyDetails = this.reportings.get_asset_property_details((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, str2, orderdir, this.current_user.account_id, str2, int64);
      if (this.utilities.isValidDataset(assetPropertyDetails))
      {
        this.outerhtml.Append(",");
        this.outerhtml.Append("\"iTotalRecords\":" + assetPropertyDetails.Tables[1].Rows[0][0] + ",");
        this.outerhtml.Append("\"iTotalDisplayRecords\":" + assetPropertyDetails.Tables[1].Rows[0][0] + ",");
        this.outerhtml.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) assetPropertyDetails.Tables[0].Rows)
          {
            this.html.Append("[");
            string str4 = "checked";
            if (row["Facility_Setting_id"].ToString().ToUpper() != "")
              this.html.Append("\"<input class='checkboxes' name='chk_propadd'  id='" + row["setting_id"].ToString() + "' value='" + row["setting_id"].ToString() + "'   type='checkbox' " + str4 + " />\",");
            else
              this.html.Append(" \"<input type='checkbox'  name='chk_propadd' id='" + row["setting_id"].ToString() + "' value='" + row["setting_id"].ToString() + "' class='checkboxes'  />\",");
            string source = row["Value"].ToString();
            if (source.Contains<char>('"'))
              source = source.Replace("\"", "''");
            this.html.Append("\"<div class='actions' id='setting_" + row["setting_id"].ToString() + "'>" + source + "\",");
            if (row["remarks"].ToString() != "")
            {
              this.html.Append("\"" + row["remarks"] + "\",");
              this.html.Append("\"Yes\",");
            }
            else
            {
              this.html.Append("\"" + row["remarks"] + "\",");
              this.html.Append("\"No\",");
            }
            if (row["Facility_Setting_id"] != (object) "")
            {
              this.html.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              this.html.Append("<ul class='ddm p-r'>");
              this.html.AppendFormat("<li><a onclick='javascript:callfancybox_property({0},{1});'><i class='icon-pencil'></i> Edit</a></li>", (object) str1, (object) row["setting_id"].ToString());
              this.html.Append("</div></div>\"");
            }
            else
            {
              this.html.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              this.html.Append("<ul class='ddm p-r'>");
              this.html.AppendFormat("<li><a onclick=javascript:callfancybox_property('{0}','{1}');><i class='icon-pencil'></i> Edit</a></li>", (object) str1, (object) row["setting_id"].ToString());
              this.html.Append("</ul>");
              this.html.Append("</div></div>\"");
            }
            this.html.Append("],");
          }
          string str5 = this.html.ToString();
          if (str5.Trim() != "")
            str5 = str5.TrimEnd(',');
          this.outerhtml.Append(str5);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serverside asset property admin panel error : " + ex.ToString()));
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
