// Decompiled with JetBrains decompiler
// Type: administration_sql
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Configuration;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_sql : Page, IRequiresSessionState
{
  protected TextBox txt_sql;
  protected Button btn_query;
  protected Button btn_clear;
  protected GridView gv1;
  protected Literal lit_message;
  protected HtmlForm form1;

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void btn_query_Click(object sender, EventArgs e)
  {
    DataAccess dataAccess = new DataAccess(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
    try
    {
      if (dataAccess.get_dataset(this.txt_sql.Text))
      {
        this.gv1.DataSource = (object) dataAccess.resultDataSet;
        this.gv1.DataBind();
      }
      else
        this.lit_message.Text = dataAccess.sqlErrorException.ToString();
    }
    catch (Exception ex)
    {
      this.lit_message.Text = ex.ToString();
    }
  }

  protected void btn_clear_Click(object sender, EventArgs e) => this.Response.Redirect("sql.aspx");

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
