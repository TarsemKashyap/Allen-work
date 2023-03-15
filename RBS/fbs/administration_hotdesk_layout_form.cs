// Decompiled with JetBrains decompiler
// Type: administration_hotdesk_layout_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_hotdesk_layout_form : fbs_base_page, IRequiresSessionState
{
  private hotdesk_api hapi = new hotdesk_api();
  protected Label lblError;
  protected TextBox txt_layout_name;
  protected DropDownList ddl_building;
  protected DropDownList ddl_level;
  protected FileUpload fl_upload;
  protected HyperLink hyp_link;
  protected HiddenField hdn_layout_id;
  protected Button btn_submit;
  protected Button btn_cancel2;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["hotdesk_booking"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (this.IsPostBack)
      return;
    DataSet settings1 = this.settings.get_settings(this.current_user.account_id, "building");
    DataSet settings2 = this.settings.get_settings(this.current_user.account_id, "level");
    foreach (DataRow row in (InternalDataCollectionBase) settings1.Tables[0].Rows)
      this.ddl_building.Items.Add(new ListItem(row["value"].ToString(), row["setting_id"].ToString()));
    foreach (DataRow row in (InternalDataCollectionBase) settings2.Tables[0].Rows)
      this.ddl_level.Items.Add(new ListItem(row["value"].ToString(), row["setting_id"].ToString()));
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    this.db.execute_scalar("insert into sbt_hotdesk_layout (image_name,name,account_id,created_on,created_by,modified_on,modified_by,building_id,level_id,status) values('" + this.SaveFile(this.fl_upload) + "','" + this.txt_layout_name.Text + "','" + (object) this.current_user.account_id + "',getutcdate(),'" + (object) this.current_user.user_id + "',getutcdate(),'" + (object) this.current_user.user_id + "','" + this.ddl_building.SelectedItem.Value + "','" + this.ddl_level.SelectedItem.Value + "','1')");
    this.Response.Redirect("layouts.aspx");
  }

  private string SaveFile(FileUpload file)
  {
    string str1 = this.Server.MapPath("~\\hotdesk\\");
    string fileName = file.FileName;
    string str2 = str1 + fileName;
    string filename = str1 + fileName;
    file.SaveAs(filename);
    return fileName;
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
