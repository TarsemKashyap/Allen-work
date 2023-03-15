// Decompiled with JetBrains decompiler
// Type: myprofile
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class myprofile : fbs_base_page, IRequiresSessionState
{
  protected HtmlLink style_color;
  protected Label lblEmail;
  protected Label lblTitle;
  protected Label lblFullName;
  protected Label lblGivenName;
  protected Label lblNativeName;
  protected Label lblStaffID;
  protected Label lblInstitution;
  protected Label lblDivision;
  protected Label lblDepartment;
  protected Label lblSection;
  protected Label lblDesignation;
  protected Label lblOfficePhone;
  protected Label lblPIN;
  protected Label lbl_groups;
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.populate_user_info(this.users.get_user(this.current_user.user_id, this.current_user.account_id));
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private void populate_user_info(user obj)
  {
    try
    {
      Dictionary<string, user_property> properties = obj.properties;
      this.lblTitle.Text = properties.ContainsKey("staff_title") ? properties["staff_title"].property_value : "";
      this.lblGivenName.Text = properties.ContainsKey("given_name") ? properties["given_name"].property_value : "";
      this.lblNativeName.Text = properties.ContainsKey("native_name") ? properties["native_name"].property_value : "";
      this.lblStaffID.Text = properties.ContainsKey("staff_id") ? properties["staff_id"].property_value : "";
      this.lblInstitution.Text = properties.ContainsKey("staff_inst") ? properties["staff_inst"].property_value : "";
      this.lblDesignation.Text = properties.ContainsKey("staff_desg") ? properties["staff_desg"].property_value : "";
      this.lblOfficePhone.Text = properties.ContainsKey("staff_offphone") ? properties["staff_offphone"].property_value : "";
      this.lblEmail.Text = obj.email;
      this.lblFullName.Text = obj.full_name;
      this.lblPIN.Text = properties.ContainsKey("pin") ? properties["pin"].property_value : "";
      this.lblDivision.Text = properties.ContainsKey("staff_division") ? properties["staff_division"].property_value : "";
      this.lblDepartment.Text = properties.ContainsKey("staff_department") ? properties["staff_department"].property_value : "";
      this.lblSection.Text = properties.ContainsKey("staff_section") ? properties["staff_section"].property_value : "";
      foreach (string key in obj.groups.Keys)
        this.lbl_groups.Text = this.lbl_groups.Text + obj.groups[key].group_name + ", ";
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private string GetDateFormatted(string dt)
  {
    string dateFormatted = "";
    try
    {
      string[] strArray = dt.Split('/');
      dateFormatted = Convert.ToDateTime(strArray[2] + "-" + strArray[1] + "-" + strArray[0]).ToString(api_constants.display_datetime_format_short);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
    return dateFormatted;
  }
}
