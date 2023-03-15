// Decompiled with JetBrains decompiler
// Type: administration_content_view
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_content_view : fbs_base_page, IRequiresSessionState
{
  protected TextBox txt_title;
  protected HtmlGenericControl errorspan;
  protected HtmlTextArea txtarea;
  protected DropDownList drp_assets;
  protected CheckBox chkbx_flag;
  protected TextBox txt_startDate;
  protected TextBox txt_end_date;
  protected CheckBox chkbx_publish;
  protected CheckBox chkbx_repeatable;
  protected Button btn_cancel;
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack || string.IsNullOrEmpty(this.Request.QueryString["id"]))
        return;
      this.Generate_detail(Convert.ToInt64(this.Request.QueryString["id"].ToString()));
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
      this.chkbx_repeatable.Checked = Convert.ToBoolean(row["repeatable"]);
      this.populate_assets(row["asset_id"].ToString());
      this.drp_assets.Items.FindByValue(row["asset_id"].ToString()).Selected = true;
      this.txt_title.Attributes.Add("readonly", "readonly");
      this.txt_startDate.Attributes.Add("readonly", "readonly");
      this.txt_end_date.Attributes.Add("readonly", "readonly");
    }
  }

  public void populate_assets(string asset_id)
  {
    try
    {
      this.drp_assets.Items.Clear();
      DataSet assets = this.assets.get_assets(this.current_user.account_id);
      if (assets == null || assets.Tables[0].Rows.Count <= 0)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
      {
        if (row[nameof (asset_id)].ToString() == asset_id)
          this.drp_assets.Items.Add(new ListItem(row["code"].ToString() + "/" + row["name"].ToString(), row[nameof (asset_id)].ToString()));
      }
    }
    catch (Exception ex)
    {
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");
}
