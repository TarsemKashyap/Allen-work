// Decompiled with JetBrains decompiler
// Type: users_list
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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class users_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public string Save = "";
  public string blkSave = "";
  public string blkdlt = "";
  protected DropDownList ddl_user_status;
  protected HtmlGenericControl div_add_user;
  protected HtmlGenericControl div_add_user_2;
  protected HtmlGenericControl div_add_ad_user;
  protected Button btnExportExcel;
  protected HiddenField hdn_userlistsearch;
  protected HiddenField hdn_totalrecord;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["users"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.users_view)
      this.redirect_unauthorized();
    if (!this.IsPostBack)
    {
      this.populate_ui();
      if (this.Session["Save"] != null && this.Session["Save"] == (object) "S")
      {
        this.Save = "S";
        this.Session.Remove("Save");
      }
      if (this.Session["blcklistupd"] != null && this.Session["blcklistupd"] == (object) "S")
      {
        this.blkSave = "S";
        this.Session.Remove("blcklistupd");
      }
      if (this.Session["Blacklist_delete"] != null && this.Session["Blacklist_delete"] == (object) "S")
      {
        this.blkdlt = "S";
        this.Session.Remove("Blacklist_delete");
      }
    }
    if (this.gp.users_add)
    {
      this.div_add_user.Visible = true;
      this.div_add_user_2.Visible = true;
    }
    if (fbs_base_page.ActiveDirectory)
      this.div_add_ad_user.Visible = true;
    this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
  }

  private void populate_ui()
  {
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='user_list_table'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>Full Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Username</th>");
      stringBuilder.Append("<th class='hidden-480'>Email</th>");
      stringBuilder.Append("<th class='hidden-480'>Type</th>");
      stringBuilder.Append("<th class='hidden-480'>Status</th>");
      stringBuilder.Append("<th class='hidden-480'>Action</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      stringBuilder.Append(this.get_users("html", this.ddl_user_status.SelectedItem.Value));
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private string get_users(string type, string status)
  {
    StringBuilder stringBuilder = new StringBuilder();
    DataSet users = this.users.get_users(this.current_user.account_id);
    DataRow[] dataRowArray = (DataRow[]) null;
    if (status == "all")
      dataRowArray = users.Tables[0].Select("1=1");
    if (status == "active")
      dataRowArray = users.Tables[0].Select("status=1");
    if (status == "inactive")
      dataRowArray = users.Tables[0].Select("status=0");
    foreach (DataRow dataRow in dataRowArray)
    {
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<td>" + dataRow["full_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + dataRow["username"].ToString() + "</td>");
      stringBuilder.Append("<td><a href='mailto:" + dataRow["email"].ToString() + "'>" + dataRow["email"].ToString() + "</a></td>");
      if (Convert.ToInt64(dataRow["user_insert_type"]) == 1L)
        stringBuilder.Append("<td>Local</td>");
      else
        stringBuilder.Append("<td>AD</td>");
      if (dataRow[nameof (status)].ToString() == "1")
        stringBuilder.Append("<td><span class='label label-Available'>Available</span></td>");
      else
        stringBuilder.Append("<td><span class='label label-NotAvailable'>Not Available</span></td>");
      stringBuilder.Append("<td>");
      if (type == "html")
      {
        stringBuilder.Append("<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
        stringBuilder.Append("<ul class='ddm p-r'>");
        stringBuilder.Append("<li><a href='javascript:view(" + dataRow["user_id"].ToString() + ")'><i class='icon-table'></i>View Details</a></li>");
        if (this.gp.users_edit)
        {
          stringBuilder.Append("<li><a href='user_form.aspx?user_id=" + dataRow["user_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
          stringBuilder.Append("<li><a href='change_password_pin.aspx?user_id=" + dataRow["user_id"].ToString() + "'><i class='icon-pencil'></i> Change Password/Pin</a></li>");
        }
        if (this.gp.users_delete && Convert.ToBoolean(dataRow["user_insert_type"]))
          stringBuilder.Append("<li><a href='javascript:delete_user(" + dataRow["user_id"].ToString() + ")'><i class='icon-trash'></i> Delete</a></li>");
        stringBuilder.Append("<li><a href='bookings.aspx?user_id=" + dataRow["user_id"].ToString() + "'><i class='icon-pencil'></i> Bookings</a></li>");
        stringBuilder.Append("</ul></div></div>");
      }
      stringBuilder.Append("</td>");
      stringBuilder.Append("</tr>");
    }
    return stringBuilder.ToString();
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      DataSet users = this.users.get_users(this.current_user.account_id);
      users.Tables[0].Columns.Add("user_type");
      users.Tables[0].AcceptChanges();
      users.Tables[0].Columns.Add("status_string");
      users.Tables[0].AcceptChanges();
      if (!this.utilities.isValidDataset(users))
        return;
      foreach (DataRow row in (InternalDataCollectionBase) users.Tables[0].Rows)
      {
        row["user_type"] = !(row["user_insert_type"].ToString() == "True") ? (object) "AD" : (object) "Local";
        users.Tables[0].AcceptChanges();
        row["status_string"] = !(row["status"].ToString() == "1") ? (object) "Not Available" : (object) "Available";
        users.Tables[0].AcceptChanges();
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("full_name", "Username");
      dictionary.Add("username", "Fullname");
      dictionary.Add("email", "Email");
      dictionary.Add("user_type", "Type");
      dictionary.Add("status_string", "Status");
      excel excel = new excel();
      excel.file_name = "userlist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls";
      excel.footer = "Generated By : " + this.current_user.full_name + "     <br/> Generated on :  " + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format);
      excel.data = users;
      excel.column_names = dictionary;
      excel.table_identifier = "userlist";
      excel.header = "User List";
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + excel.file_name);
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      this.Response.Write(this.exapi.get_excel(excel));
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected void ddl_user_status_SelectedIndexChanged(object sender, EventArgs e) => this.populate_ui();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
