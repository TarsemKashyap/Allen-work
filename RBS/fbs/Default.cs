// Decompiled with JetBrains decompiler
// Type: Default
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class Default : fbs_base_page, IRequiresSessionState
{
  protected new void Page_Load(object sender, EventArgs e)
  {
    if (this.u_group.group_type == 0)
      this.Response.Redirect("bookings.aspx");
    else
      this.Response.Redirect("bookings.aspx");
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
