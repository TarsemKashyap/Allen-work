// Decompiled with JetBrains decompiler
// Type: resource_type_view
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class resource_type_view : fbs_base_page, IRequiresSessionState
{
  protected Label lblName;
  protected Label lblDescription;
  protected Label lblStatus;
  protected Button btnCancel;
  protected HtmlForm form1;
  public DataSet data;
  private StringBuilder html = new StringBuilder();
  public long setting_id;
  private bool is_admin;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    foreach (user_group userGroup in this.current_user.groups.Values)
    {
      if (userGroup.group_type == 1)
        this.is_admin = true;
      else if (!this.is_admin)
        this.is_admin = false;
    }
    if (!this.is_admin)
      this.redirect_unauthorized();
    if (this.IsPostBack)
      return;
    try
    {
      this.setting_id = 0L;
      if (string.IsNullOrWhiteSpace(this.Request.QueryString["id"]))
      {
        Modal.Close((Page) this, (object) "OK");
      }
      else
      {
        this.setting_id = Convert.ToInt64(this.Request.QueryString["id"]);
        DataSet resourceTypeById = this.resapi.get_resource_type_by_id(this.setting_id, this.current_user.account_id);
        if (resourceTypeById.Tables.Count <= 0 || resourceTypeById.Tables[0].Rows.Count <= 0)
          return;
        this.lblName.Text = resourceTypeById.Tables[0].Rows[0]["value"].ToString();
        this.lblDescription.Text = resourceTypeById.Tables[0].Rows[0]["description"].ToString();
        this.lblStatus.Text = resourceTypeById.Tables[0].Rows[0]["status"].ToString() == "1" ? "Available" : "Not Available";
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ex.ToString());
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");
}
