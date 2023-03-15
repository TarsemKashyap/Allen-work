// Decompiled with JetBrains decompiler
// Type: administration_serverside_master_list
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

public class administration_serverside_master_list : fbs_base_page, IRequiresSessionState
{
  private StringBuilder output = new StringBuilder();
  private StringBuilder outputInner = new StringBuilder();
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.output.Append("{");
    try
    {
      this.output.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      if (int.Parse(this.Request.Params["iDisplayLength"]) == -1)
        ;
      int.Parse(this.Request.Params["iDisplayStart"]);
      string searchkey = this.Request.Params["sSearch"];
      string columnname = this.Request.Params["iSortCol_0"];
      string ordertype = this.Request.Params["sSortDir_0"];
      string Masterfilter = this.Request.QueryString["Mastertype"];
      if (searchkey == "")
        searchkey = "%";
      switch (columnname)
      {
        case "0":
          columnname = "RR.value";
          break;
        case "2":
          columnname = "RR.modified_on";
          break;
        case "3":
          columnname = "RR.fullname";
          break;
      }
      DataSet ds = new DataSet();
      if (Masterfilter != "FandB")
        ds = this.reportings.get_Master_settings(this.current_user.account_id, searchkey, Masterfilter, columnname, ordertype);
      if (this.utilities.isValidDataset(ds))
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"" + ds.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"iTotalDisplayRecords\":\"" + ds.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
          {
            if (Masterfilter != "FandB")
            {
              this.outputInner.Append("[");
              if (!string.IsNullOrEmpty(row["value"].ToString()))
                this.outputInner.Append("\"" + row["value"].ToString() + "\",");
              else
                this.outputInner.Append("\" \",");
              if (!string.IsNullOrEmpty(row["modified_on"].ToString()))
                this.outputInner.Append("\"" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["modified_on"])).ToString(api_constants.display_datetime_format) + "\",");
              else
                this.outputInner.Append("\" \",");
              if (!string.IsNullOrEmpty(row["fullname"].ToString()))
                this.outputInner.Append("\"" + row["fullname"] + "\",");
              else
                this.outputInner.Append("\" \",");
              this.outputInner.Append("\"<div class='actions'><div class='bgp'>");
              this.outputInner.Append("<a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              this.outputInner.Append("<ul class='ddm p-r'>");
              if (this.gp.master_edit)
                this.outputInner.AppendFormat("<li><a href='../administration/master_edit.aspx?id={0}&Type={1}&Action=E'><i class='icon-pencil'></i> Edit</a></li>", (object) row["setting_id"].ToString(), (object) Masterfilter);
              if (this.gp.master_delete)
                this.outputInner.AppendFormat("<li><a onclick=javascript:delete_masterlist('{0}','{1}');><i class='icon-trash'></i> Delete</a></li>", (object) row["setting_id"].ToString(), (object) Masterfilter);
              this.outputInner.Append("</ul>");
              this.outputInner.Append("</div></div>\"");
              this.outputInner.Append("],");
            }
            else
            {
              this.outputInner.Append("[");
              if (!string.IsNullOrEmpty(row["name"].ToString()))
                this.outputInner.Append("\"" + row["name"].ToString() + "\",");
              else
                this.outputInner.Append("\" \",");
              if (!string.IsNullOrEmpty(row["Unit"].ToString()))
                this.outputInner.Append("\"" + row["Unit"].ToString() + "\",");
              else
                this.outputInner.Append("\" \",");
              if (!string.IsNullOrEmpty(row["Price"].ToString()))
                this.outputInner.Append("\"" + row["Price"].ToString() + "\",");
              else
                this.outputInner.Append("\" \",");
              if (!string.IsNullOrEmpty(row["modified_on"].ToString()))
                this.outputInner.Append("\"" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["modified_on"])).ToString(api_constants.display_datetime_format) + "\",");
              else
                this.outputInner.Append("\" \",");
              if (!string.IsNullOrEmpty(row["fullname"].ToString()))
                this.outputInner.Append("\"" + row["fullname"] + "\",");
              else
                this.outputInner.Append("\" \",");
              this.outputInner.Append("\"<div class='actions'><div class='bgp'>");
              this.outputInner.Append("<a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              this.outputInner.Append("<ul class='ddm p-r'>");
              this.outputInner.Append("</ul>");
              this.outputInner.Append("</div></div>\"");
              this.outputInner.Append("],");
            }
          }
          string str = this.outputInner.ToString();
          if (str != "")
            str = str.Substring(0, str.Length - 1);
          this.output.Append(str);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serverside master list admin panel error : " + ex.ToString()));
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

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
