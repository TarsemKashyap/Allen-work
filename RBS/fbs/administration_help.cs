// Decompiled with JetBrains decompiler
// Type: administration_help
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_help : fbs_base_page, IRequiresSessionState
{
  public string page = "";
  public string descrition = "";
  protected Label lblhelp;
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.page = this.Request.QueryString["page"];
      this.page = this.page.Replace(".aspx", "");
      DataSet help = this.help.get_help(this.current_user.account_id, this.page);
      if (help.Tables[0].Rows.Count <= 0)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) help.Tables[0].Rows)
      {
        this.lblhelp.Text = row["help_content"].ToString();
        this.descrition = row["description"].ToString();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
