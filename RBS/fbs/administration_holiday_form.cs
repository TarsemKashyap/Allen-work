// Decompiled with JetBrains decompiler
// Type: administration_holiday_form
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

public class administration_holiday_form : fbs_base_page, IRequiresSessionState
{
  protected Label lblError;
  protected TextBox txt_holiday_name;
  protected CheckBox chk_repeat;
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected HiddenField hidHolidayID;
  protected Button btn_submit;
  protected Button btn_cancel2;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["holidays"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.holidays_view)
    {
      if (!this.gp.holidays_add | !this.gp.holidays_edit)
        this.redirect_unauthorized();
    }
    try
    {
      if (this.IsPostBack)
        return;
      this.txtFromDate.Attributes.Add("readonly", "readonly");
      this.txtToDate.Attributes.Add("readonly", "readonly");
      string str = this.Request.QueryString["id"];
      if (string.IsNullOrEmpty(str))
        return;
      this.populate_form(Convert.ToInt64(str));
      this.hidHolidayID.Value = str;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_form(long holiday_id)
  {
    try
    {
      holiday holiday = this.holidays.get_holiday(holiday_id, this.current_user.account_id);
      this.txt_holiday_name.Text = holiday.holiday_name;
      this.txtFromDate.Text = holiday.from_date.ToString(api_constants.display_datetime_format_short);
      this.txtToDate.Text = holiday.to_date.ToString(api_constants.display_datetime_format_short);
      if (!holiday.repeat)
        return;
      this.chk_repeat.Checked = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      bool flag = false;
      DateTime dateTime1 = Convert.ToDateTime(Convert.ToDateTime(this.txtFromDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " 12:00:00 AM");
      DateTime dateTime2 = Convert.ToDateTime(Convert.ToDateTime(this.txtToDate.Text.Trim()).ToString(api_constants.sql_datetime_format_short) + " 11:59:59 PM");
      if (this.hidHolidayID.Value != "")
      {
        DataSet dataSet1 = this.holidays.check_already_holidays_edit_name(Convert.ToInt64(this.hidHolidayID.Value), this.txt_holiday_name.Text.Trim(), dateTime1.Year.ToString(), this.current_user.account_id, dateTime1, dateTime2);
        if (dataSet1.Tables.Count > 0 && dataSet1.Tables[0].Rows.Count > 0)
        {
          flag = true;
          this.lblError.Text = Resources.fbs.holiday_name_already_exsit;
        }
        DataSet dataSet2 = this.holidays.check_already_holidays_edit_date(Convert.ToInt64(this.hidHolidayID.Value), this.txt_holiday_name.Text.Trim(), dateTime1.Year.ToString(), this.current_user.account_id, dateTime1, dateTime2);
        if (dataSet2.Tables.Count > 0 && dataSet2.Tables[0].Rows.Count > 0)
        {
          flag = true;
          this.lblError.Text = Resources.fbs.holiday_Date_already_exsit;
        }
      }
      else
      {
        DataSet dataSet3 = this.holidays.check_already_holidays_name(this.txt_holiday_name.Text.Trim(), dateTime1.Year.ToString(), this.current_user.account_id, dateTime1, dateTime2);
        if (dataSet3.Tables.Count > 0 && dataSet3.Tables[0].Rows.Count > 0)
        {
          flag = true;
          this.lblError.Text = Resources.fbs.holiday_name_already_exsit;
        }
        if (!flag)
        {
          DataSet dataSet4 = this.holidays.check_already_holidays_Date(this.txt_holiday_name.Text.Trim(), dateTime1.Year.ToString(), this.current_user.account_id, dateTime1, dateTime2);
          if (dataSet4.Tables.Count > 0 && dataSet4.Tables[0].Rows.Count > 0)
          {
            flag = true;
            this.lblError.Text = Resources.fbs.holiday_Date_already_exsit;
          }
        }
      }
      if (!flag)
      {
        long num = 0;
        holiday holiday = new holiday();
        if (this.hidHolidayID.Value != "")
        {
          holiday = this.holidays.get_holiday(Convert.ToInt64(this.hidHolidayID.Value), this.current_user.account_id);
          holiday.holiday_name = this.txt_holiday_name.Text.Trim();
          holiday.from_date = dateTime1;
          holiday.to_date = dateTime2;
          holiday.repeat = this.chk_repeat.Checked;
          holiday.modified_by = this.current_user.user_id;
          holiday.modified_on = this.current_timestamp;
        }
        else
        {
          holiday.account_id = this.current_user.account_id;
          holiday.created_by = this.current_user.user_id;
          holiday.created_on = this.current_timestamp;
          holiday.holiday_id = num;
          holiday.holiday_name = this.txt_holiday_name.Text.Trim();
          holiday.from_date = dateTime1;
          holiday.modified_by = this.current_user.user_id;
          holiday.modified_on = this.current_timestamp;
          holiday.record_id = Guid.NewGuid();
          holiday.repeat = this.chk_repeat.Checked;
          holiday.to_date = dateTime2;
        }
        if (this.holidays.update_holiday(holiday).holiday_id <= 0L)
          return;
        this.Session["holiday"] = (object) "S";
        this.Response.Redirect("holiday_list.aspx", true);
      }
      else
        this.lblError.Visible = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
