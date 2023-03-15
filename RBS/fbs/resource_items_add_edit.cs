// Decompiled with JetBrains decompiler
// Type: resource_items_add_edit
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class resource_items_add_edit : fbs_base_page, IRequiresSessionState
{
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected HtmlGenericControl liPermissions;
  protected HiddenField hdnResItemID;
  protected DropDownList ddlType;
  protected TextBox txt_name;
  protected TextBox txt_qty;
  protected TextBox txt_up;
  protected HiddenField hdnResAdvancePeriodID;
  protected HiddenField hdnResAdvancePeriodHrsID;
  protected HtmlInputRadioButton rndDays;
  protected HtmlInputRadioButton rndHours;
  protected TextBox txtNotice;
  protected DropDownList ddl_group;
  protected FileUpload upload_image;
  protected DropDownList ddl_status;
  protected HtmlTextArea txt_description;
  protected Image levelImg;
  protected HtmlGenericControl edit_image;
  protected HiddenField hdn_perm;
  protected HiddenField hdn_admin;
  protected HiddenField hdn_res_doc_id;
  protected Button btn_submit;
  protected Button btn_cancel;
  public DataSet data;
  private List<long> allowed_items;
  private bool is_admin;
  private DataSet user_item_data;
  public string html_perm = "";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["resource_booking"]))
      this.Server.Transfer("~\\unauthorized.aspx");
    if (!Convert.ToBoolean(this.current_account.properties["resource_items_group_permission"]))
      this.liPermissions.Visible = false;
    foreach (user_group userGroup in this.current_user.groups.Values)
    {
      if (userGroup.group_type == 1)
        this.is_admin = true;
      else if (!this.is_admin)
        this.is_admin = false;
    }
    try
    {
      if (this.IsPostBack)
        return;
      long resource_id;
      try
      {
        resource_id = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        resource_id = 0L;
      }
      if (resource_id > 0L && !this.is_admin)
      {
        this.get_allowed_items();
        if (!this.allowed_items.Contains(Convert.ToInt64(this.Request.QueryString["id"])))
          this.redirect_unauthorized();
      }
      this.hdnResItemID.Value = resource_id.ToString();
      this.data = this.resapi.get_resource_settings(this.current_user.account_id, this.str_resource_module);
      this.populate_resource_types(this.data);
      this.populate_groups();
      this.pageload_data();
      this.populate_permissions(resource_id);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_permissions(long resource_id)
  {
    StringBuilder stringBuilder = new StringBuilder();
    DataSet groups = this.users.get_groups(this.current_user.account_id);
    DataSet permissions = this.resapi.get_permissions(this.current_user.account_id, resource_id);
    DataSet dataSet = new DataSet();
    stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='available_list_table'>");
    stringBuilder.Append("<thead>");
    stringBuilder.Append("<tr>");
    stringBuilder.Append("<th style='width:5%;'>S.No.</th>");
    stringBuilder.Append("<th style='width:10%;'><input type='checkbox' name='cbc' id='chk_select_dates' onclick='SelectAll(this.id)' />Can Book</th>");
    stringBuilder.Append("<th class='hidden-480'>Group</th>");
    stringBuilder.Append("<th class='hidden-480'>Group Type</th>");
    stringBuilder.Append("<th class='hidden-480'>Remarks</th>");
    stringBuilder.Append("</tr>");
    stringBuilder.Append("</thead>");
    string str1 = "checked";
    string str2 = "";
    int num = 1;
    foreach (DataRow row in (InternalDataCollectionBase) groups.Tables[0].Rows)
    {
      if (row["group_name"].ToString() != api_constants.all_users_text)
      {
        if (resource_id > 0L)
        {
          DataSet resourceItemsById = this.resapi.get_resource_items_by_id(resource_id, this.current_user.account_id, "resource_module");
          if (resourceItemsById.Tables[0].Rows.Count > 0)
            str2 = !(row["group_id"].ToString() == resourceItemsById.Tables[0].Rows[0]["owner_group_id"].ToString()) ? "" : "Owner of this resource.";
          DataRow[] dataRowArray = permissions.Tables[0].Select("group_id='" + row["group_id"].ToString() + "'");
          if (dataRowArray.Length > 0)
            str1 = !Convert.ToBoolean(dataRowArray[0]["can_book"]) ? "" : "checked";
        }
        string str3 = !(row["group_type"].ToString() == "1") ? (!(str2 != "") ? "" : "disabled readonly") : "disabled readonly";
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td>" + (object) num + "</td>");
        stringBuilder.Append("<td><input class='chkperm' name='cbc' type='checkbox' " + str1 + " " + str3 + " id='grp_" + row["group_id"].ToString() + "'></td>");
        if (str3 != "")
          this.hdn_admin.Value = this.hdn_admin.Value + "grp_" + row["group_id"].ToString() + ",";
        stringBuilder.Append("<td>" + row["group_name"].ToString() + "</td>");
        if (row["group_type"].ToString() == "1")
          stringBuilder.Append("<td><span class='label label-success'>Administrator</span></td>");
        else if (row["group_type"].ToString() == "2")
          stringBuilder.Append("<td><span class='label label-info'>Super User</span></td>");
        else if (row["group_type"].ToString() == "3")
          stringBuilder.Append("<td><span class='label label-warning'>Requestor</span></td>");
        if (row["group_type"].ToString() == "0")
          stringBuilder.Append("<td><span class='label label-inverse'>" + api_constants.all_users_text + "</span></td>");
        stringBuilder.Append("<td>" + str2 + "</td>");
        stringBuilder.Append("</tr>");
        ++num;
      }
    }
    stringBuilder.Append("<tbody>");
    stringBuilder.Append("</tbody>");
    stringBuilder.Append("</table>");
    this.html_perm = stringBuilder.ToString();
  }

  private void get_allowed_items()
  {
    this.allowed_items = new List<long>();
    this.user_item_data = this.resapi.get_user_item_map(this.current_user.user_id, this.current_user.account_id, this.gp.isAdminType, this.str_resource_module);
    foreach (DataRow row in (InternalDataCollectionBase) this.user_item_data.Tables[0].Rows)
      this.allowed_items.Add(Convert.ToInt64(row["item_id"]));
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("resource_items.aspx");

  private void populate_resource_types(DataSet data)
  {
    try
    {
      this.ddlType.Items.Clear();
      foreach (DataRow dataRow in data.Tables[0].Select("parameter='resource_type' and status > -1"))
        this.ddlType.Items.Add(new ListItem(dataRow["value"].ToString(), dataRow["setting_id"].ToString()));
      this.ddlType.Items.Insert(0, new ListItem("Select Type", "0"));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_groups()
  {
    try
    {
      DataSet groups = this.users.get_groups(this.current_user.account_id);
      this.ddl_group.Items.Add(new ListItem("", "0"));
      foreach (DataRow dataRow in groups.Tables[0].Select("group_type <> 0 "))
        this.ddl_group.Items.Add(new ListItem(dataRow["group_name"].ToString(), dataRow["group_id"].ToString()));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void pageload_data()
  {
    long num;
    try
    {
      num = Convert.ToInt64(this.hdnResItemID.Value);
    }
    catch
    {
      num = 0L;
    }
    try
    {
      if (num > 0L)
      {
        DataSet resourceItemsById = this.resapi.get_resource_items_by_id(num, this.current_user.account_id, this.str_resource_module);
        DataSet documentByItemId = this.resapi.get_resource_document_by_item_id(num, this.current_user.account_id);
        if (resourceItemsById.Tables.Count > 0 && resourceItemsById.Tables[0].Rows.Count > 0)
        {
          this.txt_name.Text = resourceItemsById.Tables[0].Rows[0]["name"].ToString();
          this.txt_description.Value = resourceItemsById.Tables[0].Rows[0]["description"].ToString();
          this.ddl_status.SelectedValue = resourceItemsById.Tables[0].Rows[0]["status"].ToString();
          this.txt_qty.Text = resourceItemsById.Tables[0].Rows[0]["quantity"].ToString();
          this.txt_up.Text = resourceItemsById.Tables[0].Rows[0]["unit_of_measure"].ToString();
          this.ddl_group.SelectedValue = resourceItemsById.Tables[0].Rows[0]["owner_group_id"].ToString();
          this.ddlType.SelectedValue = resourceItemsById.Tables[0].Rows[0]["item_type_id"].ToString();
          this.hdnResAdvancePeriodID.Value = resourceItemsById.Tables[0].Rows[0]["advance_notice_period_setting_id"].ToString();
          this.hdnResAdvancePeriodHrsID.Value = resourceItemsById.Tables[0].Rows[0]["advance_notice_period_hours_setting_id"].ToString();
          try
          {
            if (!string.IsNullOrEmpty(resourceItemsById.Tables[0].Rows[0]["advance_notice_period"].ToString()) && resourceItemsById.Tables[0].Rows[0]["advance_notice_period"].ToString() != "0")
            {
              this.txtNotice.Text = resourceItemsById.Tables[0].Rows[0]["advance_notice_period"].ToString();
              this.rndDays.Checked = true;
            }
            else if (!string.IsNullOrEmpty(resourceItemsById.Tables[0].Rows[0]["advance_notice_period_hours"].ToString()) && resourceItemsById.Tables[0].Rows[0]["advance_notice_period_hours"].ToString() != "0")
            {
              this.txtNotice.Text = resourceItemsById.Tables[0].Rows[0]["advance_notice_period_hours"].ToString();
              this.rndHours.Checked = true;
            }
            else
            {
              this.txtNotice.Text = "0";
              this.rndDays.Checked = true;
            }
          }
          catch
          {
          }
        }
        if (documentByItemId.Tables.Count <= 0 || documentByItemId.Tables[0].Rows.Count <= 0)
          return;
        this.hdn_res_doc_id.Value = documentByItemId.Tables[0].Rows[0]["resource_document_id"].ToString();
        this.edit_image.Visible = true;
        byte[] inArray = (byte[]) documentByItemId.Tables[0].Rows[0]["binary_data"];
        this.levelImg.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(inArray, 0, inArray.Length);
      }
      else
      {
        this.txtNotice.Text = "";
        this.txt_name.Text = "";
        this.txt_description.Value = "";
        this.ddl_status.SelectedIndex = 0;
        this.txt_qty.Text = "";
        this.txt_up.Text = "";
        this.edit_image.Visible = false;
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
      if (this.checkDuplicate_item())
      {
        additional_resource additionalResource1 = new additional_resource();
        long item_id;
        try
        {
          item_id = Convert.ToInt64(this.hdnResItemID.Value);
        }
        catch
        {
          item_id = 0L;
        }
        if (item_id > 0L)
        {
          additionalResource1 = this.resapi.get_resource_item_obj(item_id, this.current_user.account_id, "resource_module");
        }
        else
        {
          additionalResource1.item_id = item_id;
          additionalResource1.created_by = this.current_user.user_id;
        }
        additionalResource1.name = this.txt_name.Text;
        additionalResource1.status = Convert.ToInt32(this.ddl_status.SelectedValue);
        additionalResource1.quantity = Convert.ToDecimal(this.txt_qty.Text);
        additionalResource1.unit_of_measure = this.txt_up.Text;
        additionalResource1.price = 0M;
        additionalResource1.description = this.txt_description.InnerText;
        additionalResource1.item_type_id = Convert.ToInt32(this.ddlType.SelectedValue);
        additionalResource1.modified_by = this.current_user.user_id;
        additionalResource1.account_id = this.current_user.account_id;
        additionalResource1.record_id = Guid.NewGuid();
        additionalResource1.module_name = this.str_resource_module;
        if (this.ddl_group.SelectedValue != "")
          additionalResource1.owner_group_id = Convert.ToInt64(this.ddl_group.SelectedValue);
        additional_resource additionalResource2 = this.resapi.update_resource(additionalResource1);
        if (additionalResource2.item_id > 0L)
        {
          if (this.upload_image.HasFile && this.upload_image.PostedFile.ContentLength <= Convert.ToInt32(this.settings.get_settings(this.current_user.account_id).Tables[0].Select("parameter='upload_image_size'")[0]["value"].ToString()))
          {
            resource_document resourceDocument = new resource_document();
            if (this.hdn_res_doc_id.Value != "0")
            {
              DataRow row = this.resapi.get_resource_document_by_id(Convert.ToInt64(this.hdn_res_doc_id.Value), this.current_account.account_id).Tables[0].Rows[0];
              resourceDocument.account_id = this.current_user.account_id;
              resourceDocument.attachment_type = row["attachment_type"].ToString();
              resourceDocument.binary_data = Encoding.ASCII.GetBytes(row["binary_data"].ToString());
              resourceDocument.created_by = Convert.ToInt64(row["created_by"]);
              resourceDocument.document_meta = row["document_meta"].ToString();
              resourceDocument.document_name = row["document_name"].ToString();
              resourceDocument.document_size = new int?(Convert.ToInt32(row["document_size"]));
              resourceDocument.document_type = row["document_type"].ToString();
              resourceDocument.modified_by = this.current_user.user_id;
              resourceDocument.record_id = new Guid(row["record_id"].ToString());
              resourceDocument.resource_document_id = Convert.ToInt64(this.hdn_res_doc_id.Value);
              resourceDocument.resource_item_id = additionalResource2.item_id;
            }
            else
            {
              resourceDocument.account_id = this.current_user.account_id;
              resourceDocument.attachment_type = "resource_item";
              resourceDocument.created_by = this.current_user.user_id;
              resourceDocument.document_meta = "";
              resourceDocument.modified_by = this.current_user.user_id;
              resourceDocument.record_id = additionalResource2.record_id;
              resourceDocument.resource_document_id = Convert.ToInt64(this.hdn_res_doc_id.Value);
              resourceDocument.resource_item_id = additionalResource2.item_id;
            }
            byte[] numArray = new byte[this.upload_image.PostedFile.ContentLength];
            byte[] fileBytes = this.upload_image.FileBytes;
            resourceDocument.document_name = this.upload_image.FileName;
            resourceDocument.document_size = new int?(this.upload_image.PostedFile.ContentLength);
            resourceDocument.document_type = this.upload_image.PostedFile.ContentType;
            resourceDocument.binary_data = fileBytes;
            this.resapi.update_resource_document(resourceDocument);
          }
          resource_settings resourceSettings = new resource_settings();
          resourceSettings.parent_id = additionalResource2.item_id;
          resourceSettings.account_id = this.current_user.account_id;
          resourceSettings.created_by = this.current_user.user_id;
          resourceSettings.description = "";
          resourceSettings.modified_by = this.current_user.user_id;
          resourceSettings.module_name = this.str_resource_module;
          if (this.rndDays.Checked)
          {
            try
            {
              resourceSettings.setting_id = Convert.ToInt64(this.hdnResAdvancePeriodID.Value);
            }
            catch
            {
              resourceSettings.setting_id = 0L;
            }
            resourceSettings.parameter = "Advance Notice Period";
            resourceSettings.value = this.txtNotice.Text;
            resourceSettings.status = 1L;
            resourceSettings.record_id = Guid.NewGuid();
            this.resapi.update_resource_settings(resourceSettings);
            try
            {
              resourceSettings.setting_id = Convert.ToInt64(this.hdnResAdvancePeriodHrsID.Value);
            }
            catch
            {
              resourceSettings.setting_id = 0L;
            }
            resourceSettings.parameter = "Advance Notice Period Hours";
            resourceSettings.value = "0";
            resourceSettings.status = 1L;
            resourceSettings.record_id = Guid.NewGuid();
            this.resapi.update_resource_settings(resourceSettings);
          }
          else
          {
            try
            {
              resourceSettings.setting_id = Convert.ToInt64(this.hdnResAdvancePeriodHrsID.Value);
            }
            catch
            {
              resourceSettings.setting_id = 0L;
            }
            resourceSettings.parameter = "Advance Notice Period Hours";
            resourceSettings.value = this.txtNotice.Text;
            resourceSettings.status = 1L;
            resourceSettings.record_id = Guid.NewGuid();
            this.resapi.update_resource_settings(resourceSettings);
            try
            {
              resourceSettings.setting_id = Convert.ToInt64(this.hdnResAdvancePeriodID.Value);
            }
            catch
            {
              resourceSettings.setting_id = 0L;
            }
            resourceSettings.parameter = "Advance Notice Period";
            resourceSettings.value = "0";
            resourceSettings.status = 1L;
            resourceSettings.record_id = Guid.NewGuid();
            this.resapi.update_resource_settings(resourceSettings);
          }
          DataSet groups = this.users.get_groups(this.current_user.account_id);
          DataSet permissions = this.resapi.get_permissions(this.current_user.account_id, additionalResource2.item_id);
          foreach (DataRow row in (InternalDataCollectionBase) groups.Tables[0].Rows)
          {
            if (row["group_name"].ToString() != api_constants.all_users_text)
            {
              resource_permission resourcePermission = new resource_permission();
              resourcePermission.account_id = this.current_user.account_id;
              resourcePermission.can_book = this.hdn_perm.Value.Contains("_" + row["group_id"].ToString() + ",") || this.hdn_admin.Value.Contains("_" + row["group_id"].ToString() + ",");
              resourcePermission.created_by = this.current_user.user_id;
              resourcePermission.created_on = DateTime.UtcNow;
              resourcePermission.group_id = Convert.ToInt64(row["group_id"]);
              resourcePermission.item_id = additionalResource2.item_id;
              resourcePermission.modified_by = this.current_user.user_id;
              resourcePermission.modified_on = DateTime.UtcNow;
              resourcePermission.record_id = Guid.NewGuid();
              resourcePermission.resource_permission_id = 0L;
              DataRow[] dataRowArray = permissions.Tables[0].Select("group_id='" + row["group_id"].ToString() + "'");
              if (dataRowArray.Length > 0)
                resourcePermission.resource_permission_id = Convert.ToInt64(dataRowArray[0]["resource_permission_id"]);
              this.resapi.update_resource_permission(resourcePermission);
            }
          }
          this.Response.Redirect("resource_items_add_edit.aspx?id=" + this.hdnResItemID.Value, true);
          Modal.Close((Page) this);
          this.Session["Save"] = (object) "S";
        }
        else
        {
          this.litErrorMsg.Text = "<strong>Error!</strong> Unable to save resource.";
          this.alertError.Attributes.Add("style", "display: block;");
        }
      }
      else
      {
        this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.resource_item_already_exsit;
        this.alertError.Attributes.Add("style", "display: block;");
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private bool checkDuplicate_item()
  {
    long int32 = (long) Convert.ToInt32(this.ddlType.SelectedValue);
    long num;
    try
    {
      num = Convert.ToInt64(this.hdnResItemID.Value);
    }
    catch
    {
      num = 0L;
    }
    string text = this.txt_name.Text;
    DataSet itemsByItemTypeId = this.resapi.get_resource_items_by_item_type_id(int32, this.current_user.account_id, this.str_resource_module);
    return num > 0L ? itemsByItemTypeId.Tables[0].Select("item_id=" + (object) num + " and name='" + this.txt_name.Text + "' and status >= 0").Length > 0 || itemsByItemTypeId.Tables[0].Select("name='" + this.txt_name.Text + "' and status >= 0").Length <= 0 : itemsByItemTypeId.Tables[0].Select("name='" + this.txt_name.Text + "' and status >= 0").Length <= 0;
  }
}
