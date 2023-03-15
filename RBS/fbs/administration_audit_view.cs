// Decompiled with JetBrains decompiler
// Type: administration_audit_view
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public class administration_audit_view : fbs_base_page, IRequiresSessionState
{
  public string audit_module = "";
  public string audit_action = "";
  public string audit_Status = "";
  public string audit_Createdon;
  public string audit_CreatedBy;
  public string audit_Modifiedon;
  public string audit_ModifiedBy;
  public string audit_name = "";
  public string audit_oldvalue = "";
  public string audit_newvalue = "";
  public string errormessage = "";
  protected HtmlGenericControl errormess;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["audit_log"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.logs_view)
      this.redirect_unauthorized();
    try
    {
      if (this.IsPostBack)
        return;
      this.pageload_data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    try
    {
      audit_log log = this.logs.get_log(Convert.ToInt64(this.Request.QueryString["audit_ID"]), this.current_user.account_id);
      if (log.audit_log_id == 0L)
        return;
      this.audit_module = log.module_name;
      this.audit_action = log.action;
      this.audit_Status = log.status;
      this.audit_Createdon = this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(log.created_on)).ToString(api_constants.display_datetime_format);
      DataSet user1 = this.reportings.get_user(this.current_user.account_id, Convert.ToInt64(log.created_by));
      if (user1.Tables[0].Rows.Count > 0)
        this.audit_CreatedBy = user1.Tables[0].Rows[0]["full_name"].ToString();
      DataSet user2 = this.reportings.get_user(this.current_user.account_id, Convert.ToInt64(log.modified_by));
      if (user2.Tables[0].Rows.Count > 0)
        this.audit_ModifiedBy = user2.Tables[0].Rows[0]["full_name"].ToString();
      this.audit_Modifiedon = this.tzapi.convert_to_user_timestamp(Convert.ToDateTime(log.modified_on)).ToString(api_constants.display_datetime_format);
      this.audit_oldvalue = log.change_details;
      if (log.stack_trace != null)
      {
        if (log.stack_trace != "")
        {
          this.errormessage = log.stack_trace;
          this.errormess.Visible = true;
        }
        else
        {
          this.errormess.Visible = false;
          this.errormessage = "No error";
        }
      }
      else
      {
        this.errormess.Visible = false;
        this.errormessage = "No error";
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this);

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
