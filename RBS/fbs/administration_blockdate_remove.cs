// Decompiled with JetBrains decompiler
// Type: administration_blockdate_remove
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

public class administration_blockdate_remove : fbs_base_page, IRequiresSessionState
{
  protected Literal errorcheckblockdate;
  protected HtmlGenericControl alertInfo;
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected TextBox txt_remarks;
  protected TextBox txtContact;
  protected Button btn_submit;
  protected Button btn_cancel;
  protected HtmlForm form1;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.IsPostBack)
        return;
      this.pageload_data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  public void pageload_data()
  {
    try
    {
      Convert.ToInt32(api_constants.booking_status["Booked"]);
      this.txtContact.Text = this.current_user.properties["staff_offphone"].property_value.ToString();
      if (this.Request.QueryString["booking_id"] == null)
        return;
      this.EditBlockDate();
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
      long int64 = Convert.ToInt64(this.Request.QueryString["asset_id"]);
      long booking_id = 0;
      DataSet dataSet1 = new DataSet();
      bool flag = false;
      if (this.Request.QueryString["Type"] != null)
      {
        if (this.Request.QueryString["Type"] == "Edit")
        {
          flag = true;
          booking_id = Convert.ToInt64(this.Request.QueryString["booking_id"]);
        }
      }
      else
        flag = this.bookings.is_available(int64, Convert.ToDateTime(this.txtFromDate.Text).AddMinutes(1.0), Convert.ToDateTime(this.txtToDate.Text).AddMinutes(-1.0), this.current_user.account_id, Convert.ToInt16(api_constants.booking_status["Blocked"]));
      if (flag)
      {
        DataSet dataSet2;
        if (this.Request.QueryString["Type"] != null)
        {
          if (this.Request.QueryString["Type"] == "Edit")
          {
            dataSet2 = this.bookings.get_bookings(int64, booking_id, this.current_user.account_id);
            foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
            {
              asset_booking assetBooking = new asset_booking();
              assetBooking.booking_id = booking_id;
              assetBooking.created_by = Convert.ToInt64(row["created_by"]);
              assetBooking.created_on = Convert.ToDateTime(row["created_on"]);
              assetBooking.account_id = new Guid(row["account_id"].ToString());
              assetBooking.record_id = new Guid(row["record_id"].ToString());
              assetBooking.remarks = this.txt_remarks.Text;
              assetBooking.book_from = Convert.ToDateTime(this.txtFromDate.Text);
              assetBooking.book_to = Convert.ToDateTime(this.txtToDate.Text);
              assetBooking.book_duration = 0.0;
              assetBooking.asset_id = Convert.ToInt64(row["asset_id"]);
              assetBooking.setup_required = false;
              assetBooking.transfer_original_booking_id = 0L;
              assetBooking.is_repeat = false;
              assetBooking.transfer_reason = "";
              assetBooking.transfer_request = false;
              assetBooking.contact = this.txtContact.Text;
              assetBooking.email = row["email"].ToString();
              assetBooking.booked_for = 0L;
              assetBooking.purpose = this.txt_remarks.Text;
              assetBooking.housekeeping_required = false;
              assetBooking.status = Convert.ToInt16(api_constants.booking_status["Blocked"]);
              this.bookings.update_booking(assetBooking);
            }
          }
          else
            dataSet2 = this.bookings.is_available_forBlocked(int64, Convert.ToDateTime(this.txtFromDate.Text), Convert.ToDateTime(this.txtToDate.Text), this.current_user.account_id, Convert.ToInt16(api_constants.booking_status["Blocked"]));
        }
        else
          dataSet2 = this.bookings.is_available_forBlocked(int64, Convert.ToDateTime(this.txtFromDate.Text), Convert.ToDateTime(this.txtToDate.Text), this.current_user.account_id, Convert.ToInt16(api_constants.booking_status["Blocked"]));
        if (dataSet2.Tables[0].Select("status = 1 or status = 4").Length <= 0)
        {
          if (this.Request.QueryString["Type"] == null && this.Request.QueryString["Type"] != "Edit")
          {
            asset_booking assetBooking = new asset_booking();
            assetBooking.booking_id = 0L;
            assetBooking.created_by = this.current_user.user_id;
            assetBooking.created_on = this.current_timestamp;
            assetBooking.account_id = this.current_user.account_id;
            assetBooking.record_id = this.current_user.record_id;
            assetBooking.remarks = this.txt_remarks.Text;
            assetBooking.book_from = Convert.ToDateTime(this.txtFromDate.Text);
            assetBooking.book_to = Convert.ToDateTime(this.txtToDate.Text);
            assetBooking.book_duration = 0.0;
            assetBooking.asset_id = int64;
            assetBooking.setup_required = false;
            assetBooking.transfer_original_booking_id = 0L;
            assetBooking.is_repeat = false;
            assetBooking.transfer_reason = "No Reason";
            assetBooking.transfer_request = false;
            assetBooking.contact = this.txtContact.Text;
            assetBooking.email = this.current_user.email;
            assetBooking.booked_for = 0L;
            assetBooking.purpose = this.txt_remarks.Text;
            assetBooking.housekeeping_required = false;
            assetBooking.status = Convert.ToInt16(api_constants.booking_status["Blocked"]);
            this.bookings.update_booking(assetBooking);
          }
          this.errorcheckblockdate.Text = "";
          this.alertInfo.Visible = false;
          this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFun1", "close_fancybox();", true);
          Modal.Close((Page) this);
        }
        else
        {
          this.alertInfo.Visible = true;
          this.errorcheckblockdate.Text = Resources.fbs.blocked_booking_exsits;
        }
      }
      else
      {
        this.alertInfo.Visible = true;
        this.errorcheckblockdate.Text = Resources.fbs.blockdate_check;
      }
    }
    catch (Exception ex)
    {
      this.errorcheckblockdate.Text = ex.Message;
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void EditBlockDate()
  {
    try
    {
      foreach (DataRow row in (InternalDataCollectionBase) this.bookings.get_bookings(Convert.ToInt64(this.Request.QueryString["asset_id"]), Convert.ToInt64(this.Request.QueryString["booking_id"]), this.current_user.account_id).Tables[0].Rows)
      {
        this.txtFromDate.Text = Convert.ToDateTime(row["book_from"]).ToString(api_constants.display_datetime_format);
        this.txtToDate.Text = Convert.ToDateTime(row["book_to"]).ToString(api_constants.display_datetime_format);
        this.txt_remarks.Text = row["remarks"].ToString();
        this.txtContact.Text = row["contact"].ToString();
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this);
}
