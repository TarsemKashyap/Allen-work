// Decompiled with JetBrains decompiler
// Type: administration_serverside_auditlog
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

public class administration_serverside_auditlog : fbs_base_page, IRequiresSessionState
{
  private StringBuilder outputInner = new StringBuilder();
  private StringBuilder output = new StringBuilder();
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["audit_log"]))
      this.Server.Transfer("~//unauthorized.aspx");
    this.output.Append("{");
    try
    {
      this.output.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string searchkey = this.Request.Params["sSearch"];
      if (searchkey == "")
        searchkey = "%";
      string orderby = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string str1 = Convert.ToDateTime(this.Request.Params["From"]).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.Request.Params["To"]).ToString(api_constants.sql_datetime_format_short);
      string modulename = this.Request.Params["Mod"];
      string status = this.Request.Params["Status"];
      string action = this.Request.Params["Action"];
      string from = str1 + " 00:00:00:00";
      string to = str2 + " 23:59:59:59";
      switch (orderby)
      {
        case "0":
          orderby = "a.module_name";
          break;
        case "1":
          orderby = "a.action";
          break;
        case "2":
          orderby = "a.status";
          break;
        case "3":
          orderby = "a.created_On";
          break;
        case "4":
          orderby = "a.created_by";
          break;
      }
      DataSet dataSet = new DataSet();
      DataSet logs = this.reportings.get_logs((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, orderdir, searchkey, this.current_user.account_id, modulename, action, status, from, to);
      bool flag = true;
      if (this.utilities.isValidDataset(logs))
      {
        this.output.Append(",");
        this.output.Append("\"iTotalRecords\":\"" + logs.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"iTotalDisplayRecords\":\"" + logs.Tables[1].Rows[0][0].ToString() + "\",");
        this.output.Append("\"aaData\":[");
        try
        {
          foreach (DataRow row in (InternalDataCollectionBase) logs.Tables[0].Rows)
          {
            this.outputInner.Append("[");
            this.outputInner.Append("\"" + row["module_name"].ToString() + "\",");
            this.outputInner.Append("\"" + row["action"].ToString() + "\",");
            if (row["status"].ToString() == "success")
              this.outputInner.Append("\"<span class='label label-Success'>" + row["status"].ToString() + "</span>\",");
            else
              this.outputInner.Append("\"<span class='label label-Fail'>" + row["status"].ToString() + "</span>\",");
            this.outputInner.Append("\"" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["created_on"])).ToString(api_constants.display_datetime_format) + "\",");
            this.outputInner.Append("\"" + row["createduser"] + "\",");
            if (flag)
            {
              this.outputInner.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'><li><a onclick='javascript:load_modal(" + row["audit_log_id"].ToString() + ")'><i class='icon-pencil'></i>View</a></li></ul></div><div class='altv' style='display:none;'>" + logs.Tables[1].Rows[0][0].ToString() + "</div></div>\"");
              flag = false;
            }
            else
              this.outputInner.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'><li><a onclick='javascript:load_modal(" + row["audit_log_id"].ToString() + ")'><i class='icon-pencil'></i>View</a></li></ul></div></div>\"");
            this.outputInner.Append("],");
          }
          string str3 = this.outputInner.ToString();
          if (str3.Trim() != "")
            str3 = str3.TrimEnd(',');
          this.output.Append(str3);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) ("serverside auditlog admin panel error : " + ex.ToString()));
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
