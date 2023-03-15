// Decompiled with JetBrains decompiler
// Type: administration_html_help_form
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

public class administration_html_help_form : fbs_base_page, IRequiresSessionState
{
  protected HiddenField hdnRecordID;
  protected HiddenField hdnHelpID;
  protected HtmlInputText lbl_name;
  protected TextBox txt_description;
  protected HtmlTextArea txtarea;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      long int64 = Convert.ToInt64(this.Request.QueryString["id"]);
      if (int64 > 0L)
      {
        this.hdnHelpID.Value = int64.ToString();
        this.populate_details(int64);
      }
      else
        this.hdnHelpID.Value = "";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void populate_details(long help_id)
  {
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.help.get_help(this.current_user.account_id, help_id).Tables[0].Rows)
      {
        this.hdnHelpID.Value = row[nameof (help_id)].ToString();
        this.lbl_name.Value = row["page_name"].ToString();
        this.txt_description.Text = row["description"].ToString();
        this.txtarea.InnerText = row["help_content"].ToString();
        this.hdnRecordID.Value = row["record_id"].ToString();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      clsHelp clsHelp = new clsHelp();
      DataSet dataSet = new DataSet();
      if (!string.IsNullOrEmpty(this.hdnHelpID.Value))
      {
        try
        {
          clsHelp.help_id = Convert.ToInt64(this.hdnHelpID.Value);
          this.help.get_help(this.current_user.account_id, clsHelp.help_id);
        }
        catch (Exception ex)
        {
          fbs_base_page.log.Error((object) "Error -> ", ex);
        }
      }
      else
        clsHelp.help_id = 0L;
      clsHelp.account_id = this.current_user.account_id;
      clsHelp.record_id = new Guid(this.hdnRecordID.Value);
      clsHelp.created_by = this.current_user.user_id;
      clsHelp.modified_by = this.current_user.user_id;
      string innerText = this.txtarea.InnerText;
      clsHelp.help_content = innerText;
      clsHelp.description = this.txt_description.Text;
      clsHelp.page_name = this.lbl_name.Value;
      if (this.help.update_help(clsHelp).help_id == 0L)
        return;
      this.Session["help"] = (object) "yes";
      this.Response.Redirect("Html_help.aspx");
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
      this.Response.Redirect("html_help.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
