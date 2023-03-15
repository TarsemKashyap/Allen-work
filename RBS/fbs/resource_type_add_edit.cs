// Decompiled with JetBrains decompiler
// Type: resource_type_add_edit
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

public class resource_type_add_edit : fbs_base_page, IRequiresSessionState
{
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected HiddenField hdnResTypeID;
  protected TextBox txt_name;
  protected HtmlTextArea txt_description;
  protected DropDownList ddl_status;
  protected Button btn_submit;
  protected Button btn_cancel;
  private long resource_type_id;
  private bool is_admin;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    foreach (user_group userGroup in this.current_user.groups.Values)
    {
      if (userGroup.group_type == 1)
        this.is_admin = true;
      else if (!this.is_admin)
        this.is_admin = false;
    }
    if (!this.is_admin)
      this.redirect_unauthorized();
    try
    {
      if (this.IsPostBack)
        return;
      try
      {
        this.resource_type_id = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        this.resource_type_id = 0L;
      }
      this.hdnResTypeID.Value = this.resource_type_id.ToString();
      this.pageload_data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    try
    {
      long res_type_id;
      try
      {
        res_type_id = Convert.ToInt64(this.hdnResTypeID.Value);
      }
      catch
      {
        res_type_id = 0L;
      }
      DataSet resourceTypeById = this.resapi.get_resource_type_by_id(res_type_id, this.current_user.account_id);
      if (res_type_id > 0L)
      {
        if (resourceTypeById.Tables.Count <= 0 || resourceTypeById.Tables[0].Rows.Count <= 0)
          return;
        this.txt_name.Text = resourceTypeById.Tables[0].Rows[0]["value"].ToString();
        this.txt_description.Value = resourceTypeById.Tables[0].Rows[0]["description"].ToString();
        this.ddl_status.SelectedValue = resourceTypeById.Tables[0].Rows[0]["status"].ToString();
      }
      else
      {
        this.txt_name.Text = "";
        this.txt_description.Value = "";
        this.ddl_status.SelectedIndex = 0;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_click(object sender, EventArgs e)
  {
    try
    {
      if (this.checkDuplicate_type())
      {
        resource_type resourceType = new resource_type();
        try
        {
          resourceType.setting_id = Convert.ToInt64(this.hdnResTypeID.Value);
        }
        catch
        {
          resourceType.setting_id = 0L;
        }
        resourceType.parameter = "resource_type";
        resourceType.value = this.txt_name.Text;
        resourceType.description = this.txt_description.InnerText;
        resourceType.status = Convert.ToInt64(this.ddl_status.SelectedValue);
        resourceType.created_by = this.current_user.user_id;
        resourceType.modified_by = this.current_user.user_id;
        resourceType.account_id = this.current_user.account_id;
        resourceType.record_id = Guid.NewGuid();
        resourceType.module_name = this.str_resource_module;
        this.resapi.update_resource_types(resourceType);
        this.Response.Redirect("resource_types.aspx", true);
        Modal.Close((Page) this);
        this.Session["Save"] = (object) "S";
      }
      else
      {
        this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.resource_type_already_exsit;
        this.alertError.Attributes.Add("style", "display: block;");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private bool checkDuplicate_type()
  {
    long num;
    try
    {
      num = Convert.ToInt64(this.hdnResTypeID.Value);
    }
    catch
    {
      num = 0L;
    }
    DataSet settingsByParameter = this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, "resource_type", this.str_resource_module);
    return num > 0L ? settingsByParameter.Tables[0].Select("setting_id=" + (object) num + " and value='" + this.txt_name.Text + "' and status >= 0").Length > 0 || settingsByParameter.Tables[0].Select("value='" + this.txt_name.Text + "' and status >=0 ").Length <= 0 : settingsByParameter.Tables[0].Select("value='" + this.txt_name.Text + "' and status >= 0").Length <= 0;
  }

  protected void btn_cancel_click(object sender, EventArgs e) => this.Response.Redirect("resource_types.aspx", true);
}
