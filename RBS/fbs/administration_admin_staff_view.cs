// Decompiled with JetBrains decompiler
// Type: administration_admin_staff_view
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_admin_staff_view : fbs_base_page, IRequiresSessionState
{
  protected HtmlLink style_color;
  protected HtmlHead Head1;
  protected Label lbl_Title;
  protected Label lbl_fullname;
  protected Label lbl_nativename;
  protected Label lbl_GivenName;
  protected Label lbl_Institution;
  protected Label lbl_staff_id;
  protected Label lbl_Division;
  protected Label lbl_Department;
  protected Label lbl_Section;
  protected Label lbl_desgination;
  protected Label lbl_OffPhone;
  protected Label lblPIN;
  protected Label lbl_EmailAddress;
  protected Label lbl_groups;
  protected Button btn_cancel;
  protected HtmlForm form1;
  private user obj = new user();

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      this.Generate_details();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error->" + (object) ex));
    }
  }

  public void Generate_details()
  {
    CultureInfo cultureInfo = new CultureInfo("en-GB", true);
    if (this.Request.QueryString["id"] == null)
      return;
    string str = this.Request.QueryString["id"];
    user_property userProperty = new user_property();
    Dictionary<string, user_property> userProperties = this.users.get_user_properties(Convert.ToInt64(this.Request.QueryString["id"].ToString()), this.current_user.account_id);
    DataSet usersnameonly = this.reportings.get_usersnameonly(Convert.ToInt64(str), this.current_user.account_id.ToString());
    if (usersnameonly.Tables[0].Rows.Count > 0)
    {
      this.lbl_EmailAddress.Text = "<a href='mailto:" + usersnameonly.Tables[0].Rows[0]["email"].ToString() + "'>" + usersnameonly.Tables[0].Rows[0]["email"].ToString() + "</a>";
      this.lbl_fullname.Text = usersnameonly.Tables[0].Rows[0]["full_name"].ToString();
    }
    this.lbl_Department.Text = userProperties["staff_department"].property_value;
    this.lbl_desgination.Text = userProperties["staff_desg"].property_value;
    this.lbl_staff_id.Text = userProperties["staff_id"].property_value;
    this.lbl_Division.Text = userProperties["staff_division"].property_value;
    this.lbl_GivenName.Text = userProperties["given_name"].property_value;
    this.lbl_Institution.Text = userProperties["staff_inst"].property_value;
    this.lbl_OffPhone.Text = userProperties["staff_offphone"].property_value;
    this.lbl_Section.Text = userProperties["staff_section"].property_value;
    this.lbl_Title.Text = userProperties["staff_title"].property_value;
    this.lbl_desgination.Text = userProperties["staff_desg"].property_value;
    this.lbl_nativename.Text = userProperties["native_name"].property_value;
    this.lblPIN.Text = userProperties["pin"].property_value;
    foreach (DataRow row in (InternalDataCollectionBase) this.bookings.get_user_groupname(Convert.ToInt64(str), this.current_user.account_id).Tables[0].Rows)
      this.lbl_groups.Text = this.lbl_groups.Text + row["group_name"].ToString() + ", ";
    this.lbl_groups.Text = this.lbl_groups.Text.Trim().TrimEnd(',');
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");
}
