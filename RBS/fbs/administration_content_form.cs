// Decompiled with JetBrains decompiler
// Type: administration_content_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Configuration;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_content_form : fbs_base_page, IRequiresSessionState
{
  protected TextBox txt_title;
  protected DropDownList ddl_type;
  protected HtmlGenericControl errorspan;
  protected HtmlTextArea txtarea;
  protected DropDownList drp_assets;
  protected CheckBox chkbx_flag;
  protected TextBox txt_startDate;
  protected HtmlGenericControl errorto;
  protected TextBox txt_end_date;
  protected CheckBox chkbx_publish;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["announcements"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      if (!Convert.ToBoolean(ConfigurationManager.AppSettings["enable_announcements"]) || this.IsPostBack)
        return;
      this.txt_startDate.Text = this.current_timestamp.ToString("dd-MMM-yyyy");
      this.txt_end_date.Text = this.current_timestamp.ToString("dd-MMM-yyyy");
      if (!string.IsNullOrEmpty(this.Request.QueryString["delete"]))
      {
        this.delete_content(Convert.ToInt64(this.Request.QueryString["delete"].ToString()));
      }
      else
      {
        this.populate_assets();
        if (string.IsNullOrEmpty(this.Request.QueryString["content_id"]))
          return;
        this.Generate_detail(Convert.ToInt64(this.Request.QueryString["content_id"].ToString()));
      }
    }
    catch (Exception ex)
    {
    }
  }

  public void populate_assets()
  {
    try
    {
      this.drp_assets.Items.Clear();
      ListItem listItem = new ListItem();
      DataSet assets = this.assets.get_assets(this.current_user.account_id);
      listItem.Text = "Select Asset";
      listItem.Value = "0";
      this.drp_assets.Items.Add(listItem);
      if (assets == null || assets.Tables[0].Rows.Count <= 0)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
        this.drp_assets.Items.Add(new ListItem(row["code"].ToString() + "/" + row["name"].ToString(), row["asset_id"].ToString()));
    }
    catch (Exception ex)
    {
    }
  }

  public void Generate_detail(long content_id)
  {
    DataSet contentList = this.contents.get_content_list(content_id, this.current_user.account_id);
    if (contentList == null || contentList.Tables[0].Rows.Count <= 0)
      return;
    foreach (DataRow row in (InternalDataCollectionBase) contentList.Tables[0].Rows)
    {
      this.txt_title.Text = row["title"].ToString();
      this.txtarea.InnerText = row["content_details"].ToString();
      this.txt_startDate.Text = Convert.ToDateTime(row["show_from"].ToString()).ToString("dd-MMM-yyyy");
      this.txt_end_date.Text = Convert.ToDateTime(row["show_to"].ToString()).ToString("dd-MMM-yyyy");
      this.chkbx_flag.Checked = Convert.ToBoolean(row["flag"]);
      this.chkbx_publish.Checked = Convert.ToBoolean(row["published"]);
      this.drp_assets.Items.FindByValue(row["asset_id"].ToString()).Selected = true;
    }
  }

  public void delete_content(long content_id)
  {
    content_properties contentProperties = new content_properties();
    contentProperties.content_id = content_id;
    contentProperties.account_id = this.current_user.account_id;
    contentProperties.modified_by = this.current_user.user_id;
    contentProperties.record_id = Guid.NewGuid();
    if (this.contents.delete_content(contentProperties).content_id <= 0L)
      return;
    this.Response.Redirect("content_list.aspx?notify_D=Y", true);
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      string str = this.validateInput();
      if (this.txtarea.InnerHtml != "")
      {
        if (str == "")
        {
          content_properties contentProperties = new content_properties();
          contentProperties.content_id = string.IsNullOrEmpty(this.Request.QueryString["content_id"]) ? 0L : Convert.ToInt64(this.Request.QueryString["content_id"].ToString());
          contentProperties.account_id = this.current_user.account_id;
          contentProperties.created_by = this.current_user.user_id;
          contentProperties.record_id = Guid.NewGuid();
          contentProperties.asset_id = Convert.ToInt64(this.drp_assets.SelectedValue);
          contentProperties.title = this.txt_title.Text;
          contentProperties.content_details = this.txtarea.InnerText;
          contentProperties.show_from = Convert.ToDateTime(this.txt_startDate.Text);
          contentProperties.show_to = Convert.ToDateTime(this.txt_end_date.Text);
          contentProperties.flag = this.chkbx_flag.Checked;
          contentProperties.published = this.chkbx_publish.Checked;
          contentProperties.repeatable = false;
          contentProperties.type = Convert.ToInt32(this.ddl_type.SelectedItem.Value);
          if (this.contents.update_content(contentProperties).content_id > 0L)
            this.Response.Redirect("content_list.aspx?notify_I=Y", true);
        }
        else
          this.errorto.InnerHtml = str;
        this.errorspan.InnerHtml = "";
      }
      else
        this.errorspan.InnerHtml = Resources.fbs.NC_required_content;
    }
    catch (Exception ex)
    {
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Redirect("content_list.aspx", true);
    }
    catch (Exception ex)
    {
    }
  }

  private string validateInput()
  {
    try
    {
      if (Convert.ToDateTime(this.txt_end_date.Text) < Convert.ToDateTime(this.txt_startDate.Text))
        return Resources.fbs.booking_wizard_start_end_datecheck;
    }
    catch
    {
    }
    return "";
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
