// Decompiled with JetBrains decompiler
// Type: resources_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using AjaxControlToolkit;
using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class resources_form : fbs_base_page, IRequiresSessionState
{
  protected Literal Literal1;
  protected HtmlGenericControl alt_error_alrdybook;
  protected Label lbl_assetname_heading;
  protected System.Web.UI.ScriptManager ScriptManager1;
  protected HiddenField hdn_invitee;
  protected TextBox txtResource;
  protected Panel pnl_resources;
  protected AutoCompleteExtender AutoCompleteExtender1;
  protected Button btn_add;
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
  }

  protected void btn_add_Click(object sender, EventArgs e)
  {
  }
}
