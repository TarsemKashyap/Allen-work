// Decompiled with JetBrains decompiler
// Type: outlook_view
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

public class outlook_view : Page, IRequiresSessionState
{
  protected TextBox txt_purpose;
  protected TextBox txtBookedFor;
  protected TextBox txt_email;
  protected TextBox txt_telephone;
  protected HtmlTextArea txt_remarks;
  protected HtmlGenericControl lblassets;
  protected HtmlTableRow contrlgrp_invite;
  protected HtmlTableCell tr_invite;
  protected HtmlGenericControl lblTerms;
  protected HtmlForm form1;
  public string html_invitelist;
  private users_api uapi = new users_api();
  private settings_api settings = new settings_api();
  private asset_api assets = new asset_api();
  private outlook_api outlooks = new outlook_api();
  public string html_user;
  private util utilities = new util();
  private DataTable booking_table;
  private user current_user;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.current_user = (user) this.Session["user"];
      if (this.current_user.user_id > 0L)
        this.html_user = this.current_user.full_name + " (" + this.current_user.email + ")";
      DataTable dt = new DataTable();
      try
      {
        dt = (DataTable) this.Session["bookingsDS"];
      }
      catch
      {
      }
      if (dt.Rows.Count <= 0)
        return;
      this.populate_details(dt);
    }
    catch
    {
    }
  }

  private void populate_details(DataTable dt)
  {
    DataRow row = dt.Rows[0];
    this.txt_email.Text = row["email"].ToString();
    this.txt_purpose.Text = row["purpose"].ToString();
    this.txtBookedFor.Text = row["booked_for_name"].ToString();
    this.txt_telephone.Text = row["telephone"].ToString();
    this.txt_remarks.InnerText = row["remarks"].ToString();
    if (string.IsNullOrEmpty(row["invites_email"].ToString()))
    {
      this.contrlgrp_invite.Visible = false;
      this.tr_invite.Visible = false;
    }
    else
    {
      this.contrlgrp_invite.Visible = true;
      this.tr_invite.Visible = true;
      this.populate_invitelist(row["invites_email"].ToString(), row["invites_name"].ToString());
    }
    this.lblassets.InnerHtml = this.outlooks.populate_asset(dt, this.current_user.account_id);
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
}
