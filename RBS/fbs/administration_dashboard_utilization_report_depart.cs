// Decompiled with JetBrains decompiler
// Type: administration_dashboard_utilization_report_department
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

public class administration_dashboard_utilization_report_department : 
  fbs_base_page,
  IRequiresSessionState
{
  protected PlaceHolder pn_menu;
  protected HiddenField hdn_original_end;
  protected HiddenField hdn_original_start;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    UserControl userControl = new UserControl();
    this.pn_menu.Controls.Add(this.Page.LoadControl("dashboard_menu.ascx"));
  }
}
