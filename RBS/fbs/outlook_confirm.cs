// Decompiled with JetBrains decompiler
// Type: outlook_confirm
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class outlook_confirm : Page, IRequiresSessionState
{
  public string html_invitelist;
  public outlook_api outlooks = new outlook_api();
  public skynapse.fbs.user current_user;
  public string user;
  protected TextBox txt_purpose;
  protected TextBox txtBookedFor;
  protected TextBox txt_email;
  protected TextBox txt_telephone;
  protected HtmlTextArea txt_remarks;
  protected HtmlGenericControl lblassets;
  protected HtmlTableRow contrlgrp_invite;
  protected HtmlTableCell tr_invite;
  protected HtmlGenericControl lblTerms;
  protected HtmlInputHidden hdn_room_names;
  protected HtmlForm form1;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.initialize_session())
      this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
    DataTable data = (DataTable) this.Session[this.Request.QueryString["gid"] + "_ds"];
    if (data.Rows.Count > 0)
    {
      DataRow row = data.Rows[0];
      this.txt_purpose.Text = row["purpose"].ToString();
      this.txt_email.Text = row["email"].ToString();
      this.txt_remarks.InnerText = row["remarks"].ToString();
      this.txt_telephone.Text = row["telephone"].ToString();
      this.txtBookedFor.Text = row["booked_for_name"].ToString();
      this.lblassets.InnerHtml = this.outlooks.populate_asset(data, this.current_user.account_id);
      if (string.IsNullOrEmpty(data.Rows[0]["invites_email"].ToString()))
      {
        this.contrlgrp_invite.Visible = false;
        this.tr_invite.Visible = false;
      }
      else
      {
        this.contrlgrp_invite.Visible = true;
        this.tr_invite.Visible = true;
        this.populate_invitelist(data.Rows[0]["invites_email"].ToString(), data.Rows[0]["invites_name"].ToString());
      }
    }
    this.hdn_room_names.Value = this.Request.QueryString["location"];
  }

  private bool initialize_session()
  {
    this.current_user = (skynapse.fbs.user) this.Session["user"];
    if (this.current_user == null)
    {
      users_api usersApi = new users_api();
      this.current_user = usersApi.get_user(this.Request.QueryString["user"]);
      if (this.current_user.user_id == 0L)
        this.current_user = usersApi.get_user_by_email(this.Request.QueryString["email"]);
      if (this.current_user.user_id == 0L)
        this.Response.Redirect("outlook_error.aspx?error=invalid_parameters");
      this.Session.Add("user", (object) this.current_user);
    }
    if (this.current_user.user_id <= 0L)
      return false;
    this.user = this.current_user.full_name + " (" + this.current_user.email + ")";
    return true;
  }

  private void populate_invitelist(string invites_email, string invites_name)
  {
    DataTable dataTable = new DataTable();
    dataTable.Columns.Add("invite_id");
    dataTable.Columns.Add("name");
    dataTable.Columns.Add("email");
    dataTable.AcceptChanges();
    try
    {
      StringBuilder stringBuilder = new StringBuilder();
      char ch = ';';
      string[] strArray1 = invites_email.Split(ch);
      string[] strArray2 = invites_name.Split(',');
      stringBuilder.Append("<table class='table table-striped table-bordered table-hover' width='100%'>");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th class='hidden-480'>S/No.</th>");
      stringBuilder.Append("<th class='hidden-480'>Name</th>");
      stringBuilder.Append("<th class='hidden-480'>Email</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      int num = 1;
      foreach (string str1 in strArray1)
      {
        DataRow row = dataTable.NewRow();
        row["invite_id"] = (object) 0;
        string str2;
        try
        {
          str2 = strArray2[num - 1];
        }
        catch
        {
          str2 = "";
        }
        row["name"] = (object) invites_name;
        row["email"] = (object) str1;
        dataTable.Rows.Add(row);
        dataTable.AcceptChanges();
        stringBuilder.Append("<tr class='odd gradeX'>");
        stringBuilder.Append("<td>" + num.ToString() + "</td>");
        stringBuilder.Append("<td>" + str2 + "</td>");
        stringBuilder.Append("<td>" + str1 + "</td>");
        stringBuilder.Append("</tr>");
        ++num;
      }
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_invitelist = stringBuilder.ToString();
      this.ViewState.Add("invite_data", (object) dataTable);
    }
    catch (Exception ex)
    {
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
