// Decompiled with JetBrains decompiler
// Type: administration_template_edit
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

public class administration_template_edit : fbs_base_page, IRequiresSessionState
{
  private long template_ID;
  protected TextBox txt_title;
  protected HtmlTextArea txtarea;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      this.loadEditUI();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void loadEditUI()
  {
    try
    {
      this.template_ID = Convert.ToInt64(this.Request.QueryString["templateID"].ToString());
      template template = this.tapi.get_template(this.template_ID, this.current_user.account_id);
      this.txt_title.Text = template.title;
      this.txtarea.InnerText = template.content_data;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["templates"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      template template1 = new template();
      this.template_ID = Convert.ToInt64(this.Request.QueryString["templateID"].ToString());
      template template2 = this.tapi.get_template(this.template_ID, this.current_user.account_id);
      template1.title = this.txt_title.Text;
      string innerText = this.txtarea.InnerText;
      template1.content_data = innerText;
      template1.template_id = Convert.ToInt64(this.Request.QueryString["templateID"].ToString());
      template1.record_id = template2.record_id;
      template1.name = template2.name;
      template1.account_id = template2.account_id;
      template1.created_by = template2.created_by;
      template1.created_on = template2.created_on;
      template1.modified_by = this.current_user.user_id;
      template1.modified_on = this.current_timestamp;
      template1.description = template2.description;
      if (this.tapi.update_template(template1).template_id == 0L)
        return;
      this.Session["temp_edit"] = (object) "S";
      this.Response.Redirect("~/administration/templates_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect("~/administration/templates_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
