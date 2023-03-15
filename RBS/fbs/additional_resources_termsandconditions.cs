// Decompiled with JetBrains decompiler
// Type: additional_resources_termsandconditions
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

public class additional_resources_termsandconditions : fbs_base_page, IRequiresSessionState
{
  public string strTerms;
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    try
    {
      DataSet settingsByParameter = this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, "terms_and_conditions", this.str_resource_module);
      if (!this.utilities.isValidDataset(settingsByParameter))
        return;
      this.strTerms = settingsByParameter.Tables[0].Rows[0]["value"].ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Resource Booking Error->", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
