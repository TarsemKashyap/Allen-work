// Decompiled with JetBrains decompiler
// Type: administration_user_group_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

public class administration_user_group_form : fbs_base_page, IRequiresSessionState
{
  public string current_tab_li = "";
  public string current_tab = "";
  public string html_table;
  private static string chkClientID;
  public string html_group_name;
  protected Label lblError;
  protected TextBox txt_group_name;
  protected DropDownList ddlGroupType;
  protected TextBox txt_group_description;
  protected DropDownList ddl_status;
  protected CheckBox chk_facilities_view;
  protected CheckBox chk_facilities_add;
  protected CheckBox chk_facilities_edit;
  protected CheckBox chk_facilities_delete;
  protected CheckBox chk_facilities_permissions;
  protected CheckBox chk_users_view;
  protected CheckBox chk_users_add;
  protected CheckBox chk_users_edit;
  protected CheckBox chk_users_delete;
  protected CheckBox chk_users_blacklist;
  protected CheckBox chk_groups_view;
  protected CheckBox chk_groups_add;
  protected CheckBox chk_groups_edit;
  protected CheckBox chk_groups_delete;
  protected CheckBox chk_holidays_view;
  protected CheckBox chk_holidays_add;
  protected CheckBox chk_holidays_upload;
  protected CheckBox chk_holidays_edit;
  protected CheckBox chk_holidays_delete;
  protected CheckBox chk_settings_view;
  protected CheckBox chk_settings_edit;
  protected CheckBox chk_masterdata_view;
  protected CheckBox chk_masterdata_add;
  protected CheckBox chk_masterdata_edit;
  protected CheckBox chk_masterdata_delete;
  protected CheckBox chk_templates_view;
  protected CheckBox chk_templates_edit;
  protected CheckBox chk_auditlogs_view;
  protected CheckBox chk_emaillogs_view;
  protected CheckBox chk_utilization_report_by_department_view;
  protected CheckBox chk_utilization_report_by_room_view;
  protected CheckBox chk_cancellation_report_view;
  protected CheckBox chk_noshow_report_view;
  protected CheckBox chk_unassigned_report_view;
  protected CheckBox chk_upcoming_setup_report_view;
  protected CheckBox chk_housekeeping_report_view;
  protected CheckBox chk_daily_report_view;
  protected HtmlGenericControl div_chklist;
  protected HiddenField hidGroupId;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected Repeater rpt_permissions;
  protected HiddenField HiddenField1;
  protected Button Button1;
  protected Button Button2;
  protected HiddenField hdn_current_tab;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["user_groups"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.groups_view)
      this.redirect_unauthorized();
    if (!this.IsPostBack)
    {
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["id"]))
      {
        user_group group = this.users.get_group(Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id);
        this.html_group_name = group.group_name;
        this.populate_form(group);
      }
      else
      {
        this.ddlGroupType.Items.Add(new ListItem("Super User", "2"));
        this.ddlGroupType.Items.Add(new ListItem("Administrator", "1"));
        this.ddlGroupType.Items.Add(new ListItem("Requestor", "3"));
      }
      this.load_permissions_data();
    }
    if (!string.IsNullOrEmpty(this.hdn_current_tab.Value))
    {
      this.current_tab_li = "Permissions";
      this.current_tab = "tab_permissions";
      this.hdn_current_tab.Value = string.Empty;
    }
    else
    {
      this.current_tab_li = "Add User Group";
      this.current_tab = "tab_user_group";
    }
  }

  private void populate_form(user_group ugdata)
  {
    try
    {
      this.div_chklist.Visible = true;
      switch (ugdata.group_type)
      {
        case 0:
          this.ddlGroupType.Items.Add(new ListItem(api_constants.all_users_text, "0"));
          this.div_chklist.Visible = false;
          break;
        case 1:
          this.ddlGroupType.Items.Add(new ListItem("Administrator", "1")
          {
            Selected = true
          });
          this.ddlGroupType.Items.Add(new ListItem("Super User", "2"));
          break;
        case 2:
          this.ddlGroupType.Items.Add(new ListItem("Administrator", "1"));
          this.ddlGroupType.Items.Add(new ListItem("Super User", "2")
          {
            Selected = true
          });
          break;
        case 3:
          this.ddlGroupType.Items.Add(new ListItem("Requestor", "3"));
          this.div_chklist.Visible = false;
          break;
        default:
          this.ddlGroupType.Items.Add(new ListItem("Administrator", "1"));
          this.ddlGroupType.Items.Add(new ListItem("Super User", "2")
          {
            Selected = true
          });
          break;
      }
      this.txt_group_name.Text = ugdata.group_name;
      this.txt_group_description.Text = ugdata.description;
      this.hidGroupId.Value = ugdata.group_id.ToString();
      this.ddlGroupType.SelectedValue = ugdata.group_type.ToString();
      foreach (ListItem listItem in this.ddl_status.Items)
      {
        if (listItem.Value == ugdata.status.ToString())
          listItem.Selected = true;
      }
      XmlDocument xmlDocument = new XmlDocument();
      XmlDocument properties = ugdata.properties;
      this.chk_facilities_view.Checked = properties.SelectSingleNode("rights/facilities/view").InnerText == "true";
      this.chk_facilities_add.Checked = properties.SelectSingleNode("rights/facilities/add").InnerText == "true";
      this.chk_facilities_edit.Checked = properties.SelectSingleNode("rights/facilities/edit").InnerText == "true";
      this.chk_facilities_delete.Checked = properties.SelectSingleNode("rights/facilities/delete").InnerText == "true";
      this.chk_facilities_permissions.Checked = properties.SelectSingleNode("rights/facilities/permissions") != null && properties.SelectSingleNode("rights/facilities/permissions").InnerText == "true";
      this.chk_users_view.Checked = properties.SelectSingleNode("rights/users/view").InnerText == "true";
      this.chk_users_add.Checked = properties.SelectSingleNode("rights/users/add").InnerText == "true";
      this.chk_users_edit.Checked = properties.SelectSingleNode("rights/users/edit").InnerText == "true";
      this.chk_users_delete.Checked = properties.SelectSingleNode("rights/users/delete").InnerText == "true";
      this.chk_users_blacklist.Checked = properties.SelectSingleNode("rights/users/blacklist").InnerText == "true";
      this.chk_groups_view.Checked = properties.SelectSingleNode("rights/groups/view").InnerText == "true";
      this.chk_groups_add.Checked = properties.SelectSingleNode("rights/groups/add").InnerText == "true";
      this.chk_groups_edit.Checked = properties.SelectSingleNode("rights/groups/edit").InnerText == "true";
      this.chk_groups_delete.Checked = properties.SelectSingleNode("rights/groups/delete").InnerText == "true";
      this.chk_holidays_view.Checked = properties.SelectSingleNode("rights/holidays/view").InnerText == "true";
      this.chk_holidays_add.Checked = properties.SelectSingleNode("rights/holidays/add").InnerText == "true";
      this.chk_holidays_edit.Checked = properties.SelectSingleNode("rights/holidays/edit").InnerText == "true";
      this.chk_holidays_delete.Checked = properties.SelectSingleNode("rights/holidays/delete").InnerText == "true";
      this.chk_holidays_upload.Checked = properties.SelectSingleNode("rights/holidays/upload").InnerText == "true";
      this.chk_settings_view.Checked = properties.SelectSingleNode("rights/settings/view").InnerText == "true";
      this.chk_settings_edit.Checked = properties.SelectSingleNode("rights/settings/edit").InnerText == "true";
      this.chk_masterdata_view.Checked = properties.SelectSingleNode("rights/master/view").InnerText == "true";
      this.chk_masterdata_add.Checked = properties.SelectSingleNode("rights/master/add").InnerText == "true";
      this.chk_masterdata_edit.Checked = properties.SelectSingleNode("rights/master/edit").InnerText == "true";
      this.chk_masterdata_delete.Checked = properties.SelectSingleNode("rights/master/delete").InnerText == "true";
      this.chk_auditlogs_view.Checked = properties.SelectSingleNode("rights/logs/view").InnerText == "true";
      this.chk_emaillogs_view.Checked = properties.SelectSingleNode("rights/emaillogs/view").InnerText == "true";
      this.chk_utilization_report_by_department_view.Checked = properties.SelectSingleNode("rights/reports/utilization_by_department/view").InnerText == "true";
      this.chk_utilization_report_by_room_view.Checked = properties.SelectSingleNode("rights/reports/utilization_by_room/view").InnerText == "true";
      this.chk_cancellation_report_view.Checked = properties.SelectSingleNode("rights/reports/cancellation/view").InnerText == "true";
      this.chk_noshow_report_view.Checked = properties.SelectSingleNode("rights/reports/noshow/view").InnerText == "true";
      this.chk_unassigned_report_view.Checked = properties.SelectSingleNode("rights/reports/unassigned/view").InnerText == "true";
      this.chk_upcoming_setup_report_view.Checked = properties.SelectSingleNode("rights/reports/upcoming_setup/view").InnerText == "true";
      this.chk_housekeeping_report_view.Checked = properties.SelectSingleNode("rights/reports/housekeeping/view").InnerText == "true";
      this.chk_daily_report_view.Checked = properties.SelectSingleNode("rights/reports/daily/view").InnerText == "true";
      this.chk_templates_view.Checked = properties.SelectSingleNode("rights/templates/view").InnerText == "true";
      this.chk_templates_edit.Checked = properties.SelectSingleNode("rights/templates/edit").InnerText == "true";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<rights>");
      stringBuilder.Append("<facilities>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_facilities_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<add>{0}</add>", this.chk_facilities_add.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_facilities_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<delete>{0}</delete>", this.chk_facilities_delete.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<permissions>{0}</permissions>", this.chk_facilities_permissions.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</facilities>");
      stringBuilder.Append("<users>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_users_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<add>{0}</add>", this.chk_users_add.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_users_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<delete>{0}</delete>", this.chk_users_delete.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<blacklist>{0}</blacklist>", this.chk_users_blacklist.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</users>");
      stringBuilder.Append("<groups>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_groups_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<add>{0}</add>", this.chk_groups_add.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_groups_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<delete>{0}</delete>", this.chk_groups_delete.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</groups>");
      stringBuilder.Append("<holidays>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_holidays_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<add>{0}</add>", this.chk_holidays_add.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_holidays_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<delete>{0}</delete>", this.chk_holidays_delete.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<upload>{0}</upload>", this.chk_holidays_upload.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</holidays>");
      stringBuilder.Append("<settings>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_settings_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_settings_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</settings>");
      stringBuilder.Append("<master>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_masterdata_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<add>{0}</add>", this.chk_masterdata_add.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_masterdata_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<delete>{0}</delete>", this.chk_masterdata_delete.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</master>");
      stringBuilder.Append("<logs>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_auditlogs_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</logs>");
      stringBuilder.Append("<emaillogs>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_emaillogs_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</emaillogs>");
      stringBuilder.Append("<templates>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_templates_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.AppendFormat("<edit>{0}</edit>", this.chk_templates_edit.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</templates>");
      stringBuilder.Append("<reports>");
      stringBuilder.Append("<utilization_by_department>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_utilization_report_by_department_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</utilization_by_department>");
      stringBuilder.Append("<utilization_by_room>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_utilization_report_by_room_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</utilization_by_room>");
      stringBuilder.Append("<cancellation>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_cancellation_report_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</cancellation>");
      stringBuilder.Append("<noshow>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_noshow_report_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</noshow>");
      stringBuilder.Append("<unassigned>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_unassigned_report_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</unassigned>");
      stringBuilder.Append("<upcoming_setup>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_upcoming_setup_report_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</upcoming_setup>");
      stringBuilder.Append("<housekeeping>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_housekeeping_report_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</housekeeping>");
      stringBuilder.Append("<daily>");
      stringBuilder.AppendFormat("<view>{0}</view>", this.chk_daily_report_view.Checked ? (object) "true" : (object) "false");
      stringBuilder.Append("</daily>");
      stringBuilder.Append("</reports>");
      stringBuilder.Append("</rights>");
      long group_id = 0;
      Guid group_record_id = Guid.NewGuid();
      bool flag = false;
      user_group userGroup = new user_group();
      if (this.hidGroupId.Value == "")
      {
        DataSet groups = this.users.get_groups(this.current_user.account_id);
        int count = groups.Tables[0].Rows.Count;
        if (groups.Tables[0].Select("group_name='" + this.txt_group_name.Text.Trim().Replace("'", "''") + "'").Length == 0)
        {
          userGroup.account_id = this.current_user.account_id;
          userGroup.created_by = this.current_user.user_id;
          userGroup.created_on = this.current_timestamp;
          userGroup.modified_by = this.current_user.user_id;
          userGroup.modified_on = this.current_timestamp;
          userGroup.status = (long) Convert.ToInt32(this.ddl_status.SelectedItem.Value);
          userGroup.group_id = 0L;
          userGroup.group_name = this.txt_group_name.Text.Trim();
          userGroup.description = this.txt_group_description.Text.Trim();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(stringBuilder.ToString());
          userGroup.properties = xmlDocument;
          userGroup.record_id = Guid.NewGuid();
          userGroup.group_type = Convert.ToInt32(this.ddlGroupType.SelectedValue);
          group_record_id = userGroup.record_id;
          group_id = this.users.update_group(userGroup).group_id;
          flag = true;
          this.Session["Usergroup"] = (object) "S";
        }
        else
          this.lblError.Text = Resources.fbs.user_group_already_exsit;
      }
      else
      {
        user_group group = this.users.get_group(Convert.ToInt64(this.hidGroupId.Value), this.current_user.account_id);
        DataSet groups = this.users.get_groups(this.current_user.account_id);
        int count = groups.Tables[0].Rows.Count;
        if (groups.Tables[0].Select("group_name='" + this.txt_group_name.Text.Trim().Replace("'", "''") + "' AND group_id<>'" + this.hidGroupId.Value + "'").Length == 0)
        {
          group.account_id = this.current_user.account_id;
          group.created_by = this.current_user.user_id;
          group.created_on = this.current_timestamp;
          group.modified_by = this.current_user.user_id;
          group.modified_on = this.current_timestamp;
          group.status = (long) Convert.ToInt32(this.ddl_status.SelectedItem.Value);
          group.group_id = Convert.ToInt64(this.hidGroupId.Value);
          group.group_name = this.txt_group_name.Text.Trim();
          group.description = this.txt_group_description.Text.Trim();
          group.group_type = Convert.ToInt32(this.ddlGroupType.SelectedValue);
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(stringBuilder.ToString());
          group.properties = xmlDocument;
          group_record_id = group.record_id;
          this.users.update_group(group);
          this.Session["Usergroup"] = (object) "S";
          flag = true;
        }
        else
          this.lblError.Text = Resources.fbs.user_group_already_exsit;
      }
      if (!flag)
        return;
      this.call_update_asset_permissions(group_id, group_record_id);
      if (this.Session["user"] != null && this.current_user.user_id > 0L)
      {
        user user = new user();
        this.Session.Add("user", (object) this.users.get_user(this.current_user.username));
      }
      this.Response.Redirect("user_groups_list.aspx", true);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void ddlGroupType_SelectedIndexChanged(object sender, EventArgs e)
  {
    string str = this.ddlGroupType.SelectedItem.Value;
    if (!string.IsNullOrEmpty(str))
    {
      if (str == "3")
        this.div_chklist.Visible = false;
      else
        this.div_chklist.Visible = true;
    }
    this.load_permissions_data();
  }

  private void load_permissions_data()
  {
    try
    {
      long group_id = string.IsNullOrEmpty(this.hidGroupId.Value) ? 0L : Convert.ToInt64(this.hidGroupId.Value);
      if (!string.IsNullOrEmpty(this.ddlGroupType.SelectedItem.Value))
        Convert.ToInt64(this.ddlGroupType.SelectedItem.Value);
      DataSet assetsPermissions = this.assets.get_assets_permissions(this.current_user.account_id, 0L, group_id, 0L, "");
      this.ViewState.Add("perm", (object) assetsPermissions);
      if (assetsPermissions == null)
        return;
      this.rpt_permissions.DataSource = (object) assetsPermissions.Tables[0];
      this.rpt_permissions.DataBind();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void view_all_CheckedChanged(object sender, EventArgs e)
  {
    try
    {
      this.hdn_current_tab.Value = "tab_permissions";
      this.current_tab_li = "Permissions";
      this.current_tab = "tab_permissions";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void call_update_asset_permissions(long group_id, Guid group_record_id)
  {
    try
    {
      DataSet dataSet = (DataSet) this.ViewState["perm"];
      foreach (RepeaterItem repeaterItem in this.rpt_permissions.Items)
      {
        CheckBox control1 = (CheckBox) repeaterItem.FindControl("chkRowView");
        CheckBox control2 = (CheckBox) repeaterItem.FindControl("chkRowBook");
        HiddenField control3 = (HiddenField) repeaterItem.FindControl("hdn_asset_id");
        HiddenField control4 = (HiddenField) repeaterItem.FindControl("hdn_asset_permission_id");
        HiddenField control5 = (HiddenField) repeaterItem.FindControl("hdn_group_id");
        if (dataSet != null)
        {
          DataRow[] dataRowArray = dataSet.Tables[0].Select("asset_id='" + control3.Value + "' and group_id='" + control5.Value + "'");
          if (dataRowArray.Length > 0)
          {
            DataRow dataRow = dataRowArray[0];
            if (Convert.ToBoolean(dataRow["is_view"]) == control1.Checked && Convert.ToBoolean(dataRow["is_book"]) == control2.Checked)
              goto label_8;
          }
        }
        asset_permission assetPermission1 = new asset_permission();
        assetPermission1.asset_permission_id = 0L;
        assetPermission1.account_id = this.current_user.account_id;
        assetPermission1.created_by = this.current_user.user_id;
        assetPermission1.modified_by = this.current_user.user_id;
        assetPermission1.asset_id = string.IsNullOrEmpty(control3.Value) ? 0L : Convert.ToInt64(control3.Value);
        assetPermission1.group_id = string.IsNullOrEmpty(this.hidGroupId.Value) ? group_id : Convert.ToInt64(this.hidGroupId.Value);
        assetPermission1.user_id = 0L;
        assetPermission1.is_view = control1.Checked;
        assetPermission1.is_book = control2.Checked;
        assetPermission1.remarks = "";
        assetPermission1.record_id = group_record_id;
        asset_permission assetPermission2;
        if (!string.IsNullOrEmpty(control4.Value))
        {
          assetPermission1.asset_permission_id = string.IsNullOrEmpty(control4.Value) ? 0L : Convert.ToInt64(control4.Value);
          assetPermission2 = this.assets.update_assets_permissions(assetPermission1);
        }
        else
          assetPermission2 = this.assets.update_assets_permissions(assetPermission1);
label_8:
        assetPermission2 = new asset_permission();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  public bool IsChecked(string is_view)
  {
    bool flag = true;
    try
    {
      if (!string.IsNullOrEmpty(is_view))
        flag = Convert.ToBoolean(is_view);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    return flag;
  }

  public bool IsEnabled(
    string asset_owner_group_id,
    string asset_owner_group_type,
    string group_id,
    string group_type)
  {
    bool flag = true;
    try
    {
      long num1 = string.IsNullOrEmpty(this.hidGroupId.Value) ? 0L : Convert.ToInt64(this.hidGroupId.Value);
      long num2 = string.IsNullOrEmpty(this.ddlGroupType.SelectedItem.Value) ? 0L : Convert.ToInt64(this.ddlGroupType.SelectedItem.Value);
      long num3 = string.IsNullOrEmpty(asset_owner_group_id) ? 0L : Convert.ToInt64(asset_owner_group_id);
      if (!string.IsNullOrEmpty(asset_owner_group_type))
        Convert.ToInt64(asset_owner_group_type);
      if (!string.IsNullOrEmpty(group_id))
        Convert.ToInt64(group_id);
      if (!string.IsNullOrEmpty(group_type))
        Convert.ToInt64(group_type);
      if (num2 == 1L)
        flag = false;
      else if (num1 != 0L)
      {
        if (num2 == 2L)
        {
          if (num1 == num3)
            flag = false;
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    return flag;
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
