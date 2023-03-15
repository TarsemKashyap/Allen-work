// Decompiled with JetBrains decompiler
// Type: administration_view_mytask
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
using System.Web.UI.WebControls;
using System.Xml;

public class administration_view_mytask : fbs_base_page, IRequiresSessionState
{
  protected HtmlLink style_color;
  protected HtmlHead Head1;
  protected Label lblTaskName;
  protected Label lblRoom;
  protected Label lblCreatedOn;
  protected Label lblBookedFor;
  protected Label lblBookedFrom;
  protected Label lblBookedTo;
  protected Button btn_approved;
  protected Button btn_reject;
  protected Button btn_withdraw;
  protected Button close;
  protected HtmlForm form1;
  private long workflow_id;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["workflow_id"]))
        this.workflow_id = Convert.ToInt64(this.Request.QueryString["workflow_id"]);
      this.populate_data(this.workflow_id);
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["app"]))
      {
        if (this.Request.QueryString["app"].ToString().ToUpper() == "NO")
        {
          this.btn_approved.Visible = false;
          this.btn_reject.Visible = false;
          this.btn_withdraw.Visible = true;
        }
        else if (this.Request.QueryString["app"].ToString().ToUpper() == "ADMIN")
        {
          this.btn_approved.Visible = false;
          this.btn_reject.Visible = false;
          this.btn_withdraw.Visible = false;
        }
        else
        {
          this.btn_approved.Visible = true;
          this.btn_reject.Visible = true;
          this.btn_withdraw.Visible = false;
        }
      }
      else
      {
        this.btn_approved.Visible = true;
        this.btn_reject.Visible = true;
        this.btn_withdraw.Visible = false;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_data(long workflow_id)
  {
    try
    {
      workflow workflow = this.workflows.get_workflow(workflow_id, this.current_user.account_id);
      XmlDocument xmlDocument = new XmlDocument();
      DataSet assets = this.assets.get_assets(Convert.ToInt64(workflow.properties.SelectSingleNode("properties/asset_id").InnerText), this.current_user.account_id);
      this.lblRoom.Text = assets.Tables[0].Rows[0]["code"].ToString() + " / " + assets.Tables[0].Rows[0]["name"].ToString();
      DataSet bookingById = this.bookings.get_booking_by_id(workflow.reference_id, this.current_user.account_id);
      this.lblTaskName.Text = !(workflow.action_type.ToString() == "1") || !(this.current_user.user_id.ToString() == workflow.created_by.ToString()) ? (!(workflow.action_type.ToString() == "2") ? "Request for transfer<br/>" + bookingById.Tables[0].Rows[0]["purpose"].ToString() : "Request for approval<br/>" + bookingById.Tables[0].Rows[0]["purpose"].ToString()) : "Waiting for approval<br/>" + bookingById.Tables[0].Rows[0]["purpose"].ToString();
      this.lblCreatedOn.Text = this.tzapi.convert_to_user_timestamp(workflow.created_on).ToString(api_constants.display_datetime_format);
      this.lblBookedFrom.Text = Convert.ToDateTime(bookingById.Tables[0].Rows[0]["book_from"].ToString()).ToString(api_constants.display_datetime_format);
      this.lblBookedTo.Text = Convert.ToDateTime(bookingById.Tables[0].Rows[0]["book_to"].ToString()).ToString(api_constants.display_datetime_format);
      this.lblBookedFor.Text = this.users.get_users(workflow.created_by, this.current_user.account_id).Tables[0].Rows[0]["full_name"].ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_approved_Click(object sender, EventArgs e)
  {
    if (string.IsNullOrWhiteSpace(this.Request.QueryString["workflow_id"]))
      return;
    this.workflow_id = Convert.ToInt64(this.Request.QueryString["workflow_id"]);
    this.Response.Redirect("~/mytask.aspx?workflow_id=" + (object) this.workflow_id + "&ap=1&modal=Y");
  }

  protected void btn_reject_Click(object sender, EventArgs e)
  {
    if (string.IsNullOrWhiteSpace(this.Request.QueryString["workflow_id"]))
      return;
    this.workflow_id = Convert.ToInt64(this.Request.QueryString["workflow_id"]);
    this.Response.Redirect("~/reject_mytask.aspx?workflow_id=" + (object) this.workflow_id + "+&tab=inbox");
  }

  protected void close_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  protected void btn_withdraw_Click(object sender, EventArgs e)
  {
    if (string.IsNullOrWhiteSpace(this.Request.QueryString["workflow_id"]))
      return;
    this.workflow_id = Convert.ToInt64(this.Request.QueryString["workflow_id"]);
    this.Response.Redirect("mytask.aspx?workflow_id=" + (object) this.workflow_id + "&wd=1&modal=Y");
  }
}
