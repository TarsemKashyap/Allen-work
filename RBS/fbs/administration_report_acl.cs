// Decompiled with JetBrains decompiler
// Type: administration_report_acl
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_report_acl : fbs_base_page, IRequiresSessionState
{
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected Button btnExportExcel;
  protected Label lblDateRage;
  protected Button btn_group;
  protected Button btn_user;
  protected HiddenField hdn_report_start;
  protected HiddenField hdn_report_end;
  protected HiddenField hdn_daterange;
  public string html_table;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
  }

  protected void btn_group_Click(object sender, EventArgs e)
  {
  }

  protected void btn_user_Click(object sender, EventArgs e)
  {
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
  }
}
