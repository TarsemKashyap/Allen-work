// Decompiled with JetBrains decompiler
// Type: view_mytask
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class view_mytask : fbs_base_page, IRequiresSessionState
{
  protected PlaceHolder ph_control;
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      UserControl userControl = new UserControl();
      UserControl child = (UserControl) this.Page.LoadControl("~/controls/view_mytask.ascx");
      child.Attributes.Add("workflow_id", this.Request.QueryString["workflow_id"]);
      this.ph_control.Controls.Add((Control) child);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
