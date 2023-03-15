// Decompiled with JetBrains decompiler
// Type: outlook_error
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public class outlook_error : Page, IRequiresSessionState
{
  public string html;
  protected HtmlForm form1;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!(this.Request.QueryString["error"] == "no_dates"))
      return;
    this.html = "<h1>Cannot Change Date</h1>";
    this.html += "<p>For recurrent meetings, you cannot change the date of the recurrence. You can change the timing or the room.</p>";
    this.html += "<p>If you need to change the date, then cancel the particular booking and create a new single booking.</p>";
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
