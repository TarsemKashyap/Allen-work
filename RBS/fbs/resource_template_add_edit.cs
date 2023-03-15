// Decompiled with JetBrains decompiler
// Type: resource_template_add_edit
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

public class resource_template_add_edit : fbs_base_page, IRequiresSessionState
{
  protected Literal litErrorMsg;
  protected HtmlGenericControl alertError;
  protected HiddenField hdnResourceTemplateID;
  protected TextBox txt_TempName;
  protected DropDownList ddl_status;
  protected HtmlTextArea txt_description;
  protected HtmlGenericControl div1;
  protected DropDownList ddl_res_type;
  protected HtmlGenericControl errorResType;
  protected HtmlGenericControl div2;
  protected HtmlGenericControl div3;
  protected DropDownList ddl_res_name;
  protected HtmlGenericControl errorResName;
  protected HtmlGenericControl div4;
  protected HtmlGenericControl div7;
  protected TextBox txtQty;
  protected HtmlGenericControl div5;
  protected Image img_res;
  protected HtmlGenericControl div6;
  protected Button btnAdd;
  protected GridView gdResource;
  protected Button btn_submit;
  protected Button btn_cancel;
  private long resTemplateId;
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
    if (this.IsPostBack)
      return;
    try
    {
      this.populate_resourcetype();
      try
      {
        this.resTemplateId = Convert.ToInt64(this.Request.QueryString["id"]);
      }
      catch
      {
        this.resTemplateId = 0L;
      }
      if (this.resTemplateId > 0L)
      {
        this.hdnResourceTemplateID.Value = this.resTemplateId.ToString();
        DataSet dataSet = new DataSet();
        DataSet templateByTemplateId = this.resapi.get_resource_template_by_templateId(this.current_user.account_id, this.resTemplateId);
        if (!this.utilities.isValidDataset(templateByTemplateId))
          return;
        this.txt_TempName.Text = templateByTemplateId.Tables[0].Rows[0]["template_name"].ToString();
        this.SetInitialRow(templateByTemplateId.Tables[0]);
      }
      else
      {
        this.hdnResourceTemplateID.Value = "";
        this.SetInitialRow(new DataTable());
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_resourcetype()
  {
    this.ddl_res_type.Items.Clear();
    this.ddl_res_name.Items.Clear();
    this.txtQty.Text = "";
    ListItem listItem = new ListItem();
    listItem.Text = "select";
    listItem.Value = "select";
    this.ddl_res_type.Items.Add(listItem);
    this.ddl_res_name.Items.Add(listItem);
    try
    {
      foreach (DataRow dataRow in this.resapi.get_resource_settings_by_parameter(this.current_user.account_id, "resource_type", this.str_resource_module).Tables[0].Select("status > 0  and value <> 'Template'"))
        this.ddl_res_type.Items.Add(new ListItem()
        {
          Text = dataRow["value"].ToString(),
          Value = dataRow["setting_id"].ToString()
        });
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void ddl_res_type_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.populate_resource_names();
    this.txtQty.Text = "";
  }

  private void populate_resource_names()
  {
    this.img_res.ImageUrl = (string) null;
    this.ddl_res_name.Items.Clear();
    this.ddl_res_name.Items.Add(new ListItem()
    {
      Text = "select",
      Value = "select"
    });
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.resapi.get_resource_items_by_item_type_id(Convert.ToInt64(this.ddl_res_type.SelectedItem.Value), this.current_user.account_id, this.str_resource_module).Tables[0].Rows)
        this.ddl_res_name.Items.Add(new ListItem()
        {
          Text = row["name"].ToString(),
          Value = row["item_id"].ToString()
        });
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => this.Response.Redirect("resource_templates.aspx");

  private void SetInitialRow(DataTable table)
  {
    try
    {
      DataTable dataTable = new DataTable();
      dataTable.Columns.Add("RowNumber");
      dataTable.Columns.Add("TemplateItemID");
      dataTable.Columns.Add("ItemID");
      dataTable.Columns.Add("ResName");
      dataTable.Columns.Add("ResQty");
      dataTable.Columns.Add("Remove_image_visibility");
      int num = 1;
      foreach (DataRow row1 in (InternalDataCollectionBase) table.Rows)
      {
        DataRow row2 = dataTable.NewRow();
        row2["RowNumber"] = (object) num;
        row2["TemplateItemID"] = (object) row1["template_item_id"].ToString();
        row2["ItemID"] = (object) row1["item_id"].ToString();
        row2["ResName"] = (object) row1["name"].ToString();
        row2["ResQty"] = (object) row1["req_qty"].ToString();
        row2["Remove_image_visibility"] = (object) "True";
        dataTable.Rows.Add(row2);
        ++num;
      }
      this.ViewState["ResourceList"] = (object) dataTable;
      this.gdResource.DataSource = (object) dataTable;
      this.gdResource.DataBind();
      this.SetPreviousData();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void AddNewRowToGrid()
  {
    try
    {
      bool flag = true;
      int index1 = 0;
      if (this.ViewState["ResourceList"] != null)
      {
        DataTable dataTable1 = (DataTable) this.ViewState["ResourceList"];
        DataTable dataTable2 = dataTable1.Clone();
        if (dataTable1.Rows.Count > 0)
        {
          for (int index2 = 0; index2 < dataTable1.Rows.Count; ++index2)
          {
            TextBox control1 = (TextBox) this.gdResource.Rows[index1].Cells[1].FindControl("txtResName");
            TextBox control2 = (TextBox) this.gdResource.Rows[index1].Cells[2].FindControl("txtResQty");
            Label control3 = (Label) this.gdResource.Rows[index1].Cells[0].FindControl("lblTemplateItemID");
            Label control4 = (Label) this.gdResource.Rows[index1].Cells[0].FindControl("lblItemID");
            DataRow row = dataTable2.NewRow();
            row["RowNumber"] = (object) (index2 + 1);
            row["TemplateItemID"] = (object) control3.Text;
            row["ItemID"] = (object) control4.Text;
            row["ResName"] = (object) control1.Text;
            row["ResQty"] = (object) control2.Text;
            row["Remove_image_visibility"] = (object) "True";
            if (control4.Text != "")
            {
              dataTable2.Rows.Add(row);
              dataTable2.AcceptChanges();
              ++index1;
            }
          }
        }
        foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
        {
          if (row["ItemID"].ToString() == this.ddl_res_name.SelectedItem.Value)
            flag = false;
        }
        if (flag && this.ddl_res_name.SelectedItem.Text != "select")
        {
          DataRow row = dataTable2.NewRow();
          row["RowNumber"] = (object) (index1 + 1);
          row["TemplateItemID"] = (object) this.hdnResourceTemplateID.Value;
          row["ItemID"] = (object) this.ddl_res_name.SelectedItem.Value;
          row["ResName"] = (object) this.ddl_res_name.SelectedItem.Text;
          row["ResQty"] = (object) this.txtQty.Text;
          row["Remove_image_visibility"] = (object) "True";
          if (this.ddl_res_name.SelectedItem.Value != "")
          {
            dataTable2.Rows.Add(row);
            dataTable2.AcceptChanges();
          }
        }
        this.ViewState["ResourceList"] = (object) dataTable2;
        this.gdResource.DataSource = (object) dataTable2;
        this.gdResource.DataBind();
      }
      this.SetPreviousData();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void SetPreviousData()
  {
    try
    {
      int index1 = 0;
      if (this.ViewState["ResourceList"] == null)
        return;
      DataTable dataTable = (DataTable) this.ViewState["ResourceList"];
      if (dataTable.Rows.Count <= 0)
        return;
      for (int index2 = 0; index2 < dataTable.Rows.Count; ++index2)
      {
        TextBox control1 = (TextBox) this.gdResource.Rows[index1].Cells[1].FindControl("txtResName");
        TextBox control2 = (TextBox) this.gdResource.Rows[index1].Cells[2].FindControl("txtResQty");
        Label control3 = (Label) this.gdResource.Rows[index1].Cells[0].FindControl("lblTemplateItemID");
        Label control4 = (Label) this.gdResource.Rows[index1].Cells[0].FindControl("lblItemID");
        control1.Text = dataTable.Rows[index2]["ResName"].ToString();
        control2.Text = dataTable.Rows[index2]["ResQty"].ToString();
        control3.Text = dataTable.Rows[index2]["TemplateItemID"].ToString();
        control4.Text = dataTable.Rows[index2]["ItemID"].ToString();
        ++index1;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void gdResource_RowCreated(object sender, GridViewRowEventArgs e)
  {
    try
    {
      if (e.Row.RowType != DataControlRowType.DataRow)
        return;
      DataTable dataTable = (DataTable) this.ViewState["ResourceList"];
      ImageButton control = (ImageButton) e.Row.FindControl("LinkButton1");
      if (control == null || dataTable.Rows.Count != 1)
        return;
      control.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void gdResource_RowDataBound(object sender, GridViewRowEventArgs e)
  {
    try
    {
      if (e.Row.RowType != DataControlRowType.DataRow)
        return;
      DataTable dataTable = (DataTable) this.ViewState["ResourceList"];
      ImageButton control = (ImageButton) e.Row.FindControl("LinkButton1");
      if (control == null || dataTable.Rows.Count != 1)
        return;
      control.Visible = false;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void LinkButton1_Click(object sender, ImageClickEventArgs e)
  {
    try
    {
      int rowIndex = ((GridViewRow) ((Control) sender).NamingContainer).RowIndex;
      if (this.ViewState["ResourceList"] != null)
      {
        DataTable dataTable = (DataTable) this.ViewState["ResourceList"];
        if (dataTable.Rows.Count > 1)
          dataTable.Rows.Remove(dataTable.Rows[rowIndex]);
        if (dataTable.Rows[dataTable.Rows.Count - 1]["ResName"].ToString() == "")
        {
          dataTable.Rows[dataTable.Rows.Count - 1]["Remove_image_visibility"] = (object) "False";
          dataTable.AcceptChanges();
          for (int index = rowIndex; index <= dataTable.Rows.Count - 1; ++index)
            dataTable.Rows[index][0] = (object) (Convert.ToInt32(dataTable.Rows[index][0]) - 1);
        }
        this.ViewState["ResourceList"] = (object) dataTable;
        this.gdResource.DataSource = (object) dataTable;
        this.gdResource.DataBind();
      }
      this.SetPreviousData();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btnAdd_Click(object sender, EventArgs e)
  {
    if (this.ddl_res_type.SelectedItem.Text != "select" && this.ddl_res_name.SelectedItem.Text != "select")
    {
      this.AddNewRowToGrid();
      this.populate_resourcetype();
    }
    else
    {
      this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.error_resource_template_item_add;
      this.alertError.Attributes.Add("style", "display: block;");
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    if (this.gdResource.Rows.Count > 0)
    {
      if (this.checkDuplicate())
      {
        this.litErrorMsg.Text = "";
        this.alertError.Attributes.Add("style", "display: none;");
        resource_template resourceTemplate1 = new resource_template();
        resourceTemplate1.account_id = this.current_user.account_id;
        try
        {
          resourceTemplate1.resource_template_id = Convert.ToInt64(this.hdnResourceTemplateID.Value);
        }
        catch
        {
          resourceTemplate1.resource_template_id = 0L;
        }
        resourceTemplate1.created_by = this.current_user.user_id;
        resourceTemplate1.created_on = this.current_timestamp;
        resourceTemplate1.description = this.txt_description.InnerText;
        resourceTemplate1.modified_by = this.current_user.user_id;
        resourceTemplate1.modified_on = this.current_timestamp;
        resourceTemplate1.module_name = this.str_resource_module;
        resourceTemplate1.record_id = Guid.NewGuid();
        resourceTemplate1.status = Convert.ToInt32(this.ddl_status.SelectedItem.Value);
        resourceTemplate1.template_name = this.txt_TempName.Text;
        resource_template resourceTemplate2 = this.resapi.update_resource_template(resourceTemplate1);
        if (resourceTemplate2.resource_template_id <= 0L)
          return;
        this.resapi.delete_resource_template_item_template_id(resourceTemplate2.resource_template_id, this.current_user.account_id, this.current_user.user_id);
        for (int index = 0; index < this.gdResource.Rows.Count; ++index)
        {
          Label control1 = (Label) this.gdResource.Rows[index].Cells[0].FindControl("lblTemplateItemID");
          Label control2 = (Label) this.gdResource.Rows[index].Cells[1].FindControl("lblItemID");
          TextBox control3 = (TextBox) this.gdResource.Rows[index].Cells[3].FindControl("txtResName");
          TextBox control4 = (TextBox) this.gdResource.Rows[index].Cells[5].FindControl("txtResQty");
          resource_template_item resourceTemplateItem = new resource_template_item();
          resourceTemplateItem.account_id = this.current_user.account_id;
          resourceTemplateItem.created_by = this.current_user.user_id;
          resourceTemplateItem.created_on = this.current_timestamp;
          resourceTemplateItem.item_id = Convert.ToInt64(control2.Text);
          resourceTemplateItem.modified_by = this.current_user.user_id;
          resourceTemplateItem.modified_on = this.current_timestamp;
          resourceTemplateItem.module_name = this.str_resource_module;
          resourceTemplateItem.quantity = Convert.ToDouble(control4.Text);
          resourceTemplateItem.record_id = Guid.NewGuid();
          resourceTemplateItem.resource_template_id = resourceTemplate2.resource_template_id;
          resourceTemplateItem.status = 1;
          resourceTemplateItem.template_item_id = 0L;
          this.resapi.update_resource_template_item(resourceTemplateItem);
        }
        this.Response.Redirect("resource_templates.aspx");
      }
      else
      {
        this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.error_resource_template_duplicate;
        this.alertError.Attributes.Add("style", "display: block;");
      }
    }
    else
    {
      this.litErrorMsg.Text = "<strong>Error!</strong>" + Resources.fbs.error_resource_template_items;
      this.alertError.Attributes.Add("style", "display: block;");
    }
  }

  private bool checkDuplicate()
  {
    try
    {
      DataSet dataSet = new DataSet();
      DataRow[] dataRowArray = this.resapi.get_resource_templates(this.current_user.account_id).Tables[0].Select("template_name='" + this.txt_TempName.Text + "'");
      if (dataRowArray.Length > 0)
      {
        if (!(this.hdnResourceTemplateID.Value != ""))
          return false;
        if (!(dataRowArray[0]["resource_template_id"].ToString() == this.hdnResourceTemplateID.Value))
          return false;
      }
    }
    catch
    {
    }
    return true;
  }
}
