// Decompiled with JetBrains decompiler
// Type: administration_serverside_email
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

public class administration_serverside_email : fbs_base_page, IRequiresSessionState
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
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string searchkey = this.Request.Params["sSearch"];
      string orderby = this.Request.Params["iSortCol_0"];
      string orderdir = this.Request.Params["sSortDir_0"];
      string str1 = Convert.ToDateTime(this.Request.Params["From"]).ToString(api_constants.sql_datetime_format_short);
      string str2 = Convert.ToDateTime(this.Request.Params["To"]).ToString(api_constants.sql_datetime_format_short);
      string status = this.Request.Params["Status"];
      string str3 = this.Request.Params["Export"];
      string str4 = this.Request.Params["asset_id"];
      string str5 = str1 + " 00:00:00";
      string str6 = str2 + " 23:59:59";
      string from = Convert.ToDateTime(str5).AddHours(this.current_account.timezone * -1.0).ToString(api_constants.sql_datetime_format);
      string to = Convert.ToDateTime(str6).AddHours(this.current_account.timezone * -1.0).ToString(api_constants.sql_datetime_format);
      switch (orderby)
      {
        case "0":
          orderby = "to_msg";
          break;
        case "1":
          orderby = "subject";
          break;
        case "2":
          orderby = "created_by";
          break;
        case "3":
          orderby = "Sent";
          break;
        case "4":
          orderby = "sent_on";
          break;
        case "5":
          orderby = "from_msg";
          break;
      }
      DataSet dataSet = new DataSet();
      if (str4 != "0")
      {
        DataSet emails = this.reportings.get_emails((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, orderdir, searchkey, this.current_user.account_id, status, from, to, Convert.ToInt64(str4));
        if (this.utilities.isValidDataset(emails))
        {
          this.output.Append(",");
          this.output.Append("\"iTotalRecords\":\"" + emails.Tables[1].Rows[0][0].ToString() + "\",");
          this.output.Append("\"iTotalDisplayRecords\":\"" + emails.Tables[1].Rows[0][0].ToString() + "\",");
          this.output.Append("\"aaData\":[");
          try
          {
            foreach (DataRow row in (InternalDataCollectionBase) emails.Tables[0].Rows)
            {
              this.outputInner.Append("[");
              if (row["to_msg"].ToString().Split(';').ToString() != "")
              {
                if (row["to_msg"].ToString().Split(';').Length > 1)
                {
                  if (row["to_msg"].ToString().Split(';')[1] != "")
                    this.outputInner.Append("\"<div  style='overflow-y:scroll;height:100px;width:200px;'>" + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "</div>\",");
                  else
                    this.outputInner.Append("\" " + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "\",");
                }
                else
                  this.outputInner.Append("\" " + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "\",");
              }
              else
                this.outputInner.Append("\"<div  style='overflow-y:scroll;height:100px;width:200px;'>" + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "</div>\",");
              this.outputInner.Append("\"" + row["subject"].ToString() + "\",");
              this.outputInner.Append("\"" + row["username"].ToString() + "\",");
              if (Convert.ToBoolean(row["sent"]))
                this.outputInner.Append("\" <center> <img  src='../assets/img/ok-icon.png' alt='Sorry' /> </center> \",");
              else
                this.outputInner.Append("\" <center> <img  src='../assets/img/error.png' alt='Sorry' /> </center> \",");
              if (row["sent_on"].ToString() == "Not Yet Send")
                this.outputInner.Append("\"" + row["sent_on"].ToString() + "\",");
              else if (row["sent_on"].ToString() != "")
                this.outputInner.Append("\"" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["sent_on"])).ToString("dd-MMM-yyy hh:mm tt") + "\",");
              else
                this.outputInner.Append("\"No entry\",");
              this.outputInner.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'><li><a  onclick='javascript:callfancybox(" + row["message_id"].ToString() + ")'><i class='icon-pencil'></i> View</a></li><li><a  onclick='javascript:resendEmail(" + row["message_id"].ToString() + ")'><i class='icon-pencil'></i>Resend Email</a></li></ul></div><div class='altv' style='display:none;'>" + emails.Tables[1].Rows[0][0].ToString() + "</div></div>\"");
              this.outputInner.Append("],");
            }
            string str7 = this.outputInner.ToString();
            if (str7 != "")
              str7 = str7.Substring(0, str7.Length - 1);
            this.output.Append(str7);
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) ("serverside email list admin panel error : " + ex.ToString()));
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
      else
      {
        DataSet emails = this.reportings.get_emails((num2 + 1).ToString(), (num2 + num1).ToString(), orderby, orderdir, searchkey, this.current_user.account_id, status, from, to);
        if (this.utilities.isValidDataset(emails))
        {
          this.output.Append(",");
          this.output.Append("\"iTotalRecords\":\"" + emails.Tables[1].Rows[0][0].ToString() + "\",");
          this.output.Append("\"iTotalDisplayRecords\":\"" + emails.Tables[1].Rows[0][0].ToString() + "\",");
          this.output.Append("\"aaData\":[");
          try
          {
            foreach (DataRow row in (InternalDataCollectionBase) emails.Tables[0].Rows)
            {
              this.outputInner.Append("[");
              if (row["to_msg"].ToString().Split(';').ToString() != "")
              {
                if (row["to_msg"].ToString().Split(';').Length > 1)
                {
                  if (row["to_msg"].ToString().Split(';')[1] != "")
                    this.outputInner.Append("\"<div  style='overflow-y:scroll;height:100px;width:200px;'>" + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "</div>\",");
                  else
                    this.outputInner.Append("\" " + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "\",");
                }
                else
                  this.outputInner.Append("\" " + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "\",");
              }
              else
                this.outputInner.Append("\"<div  style='overflow-y:scroll;height:100px;width:200px;'>" + row["to_msg"].ToString().ToString().Replace(";", ";<br/>") + "</div>\",");
              this.outputInner.Append("\"" + row["subject"].ToString() + "\",");
              this.outputInner.Append("\"" + row["username"].ToString() + "\",");
              if (Convert.ToBoolean(row["sent"]))
                this.outputInner.Append("\" <center> <img  src='../assets/img/ok-icon.png' alt='Yes' /> </center> \",");
              else
                this.outputInner.Append("\" <center> <img   src='../assets/img/error.png' alt='No' /> </center> \",");
              if (row["sentmsg"].ToString() == "Not Yet Send")
                this.outputInner.Append("\"" + row["sentmsg"].ToString() + "\",");
              else if (row["sent_on"].ToString() != "")
                this.outputInner.Append("\"" + this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(row["sent_on"])).ToString("dd-MMM-yyy hh:mm tt") + "\",");
              else
                this.outputInner.Append("\"No entry\",");
              this.outputInner.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a><ul class='ddm p-r'><li><a  onclick='javascript:callfancybox(" + row["message_id"].ToString() + ")'><i class='icon-pencil'></i> View</a></li><li><a  onclick='javascript:resendEmail(" + row["message_id"].ToString() + ")'><i class='icon-pencil'></i>Resend Email</a></li></ul></div><div class='altv' style='display:none;'>" + emails.Tables[1].Rows[0][0].ToString() + "</div></div>\"");
              this.outputInner.Append("],");
            }
            string str8 = this.outputInner.ToString();
            if (str8 != "")
              str8 = str8.Substring(0, str8.Length - 1);
            this.output.Append(str8);
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) ("serverside email list admin panel error : " + ex.ToString()));
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
