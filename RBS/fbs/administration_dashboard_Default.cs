// Decompiled with JetBrains decompiler
// Type: administration_dashboard_Default
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

public class administration_dashboard_Default : Page, IRequiresSessionState
{
  protected PlaceHolder pn_menu;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    UserControl userControl = new UserControl();
    this.pn_menu.Controls.Add(this.Page.LoadControl("dashboard_menu.ascx"));
  }
}
