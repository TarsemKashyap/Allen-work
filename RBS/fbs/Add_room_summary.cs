// Decompiled with JetBrains decompiler
// Type: Add_room_summary
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class Add_room_summary : fbs_base_page, IRequiresSessionState
{
  private user obj = new user();
  private Guid event_id;
  public string htmltable;
  private asset_booking objasset_booking = new asset_booking();
  private DataSet setting_data;
  private DataSet asset_pro_ds;
  private Dictionary<string, string> selectedDates;
  private List<long> assetID = new List<long>();
  private DataSet Asset_ID;
  protected Label lbl_assetname_heading;
  protected HtmlInputText txt_from;
  protected HtmlInputText txt_to;
  protected HtmlInputText lbl_purpose;
  protected HtmlInputText lbl_bookedfor;
  protected HtmlInputText lbl_email;
  protected HtmlInputText lbl_telephone;
  protected HtmlTextArea lbl_remarks;
  protected Button btn_cancel;
  protected HtmlForm form1;

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.obj = (user) this.Session["user"];
    if (this.obj == null)
      this.Response.Redirect("../error.aspx?message=not_authorized");
    try
    {
      fbs_base_page.log.Info((object) "loading Add_room_summary.aspx");
      this.event_id = Guid.Parse(this.Request.QueryString["id"].ToString());
      this.Asset_ID = this.bookings.get_AssetID(this.event_id, this.current_user.account_id);
      this.objasset_booking = this.bookings.get_booking_eventId(this.event_id, this.current_user.account_id);
      asset asset = this.assets.get_asset(this.objasset_booking.asset_id, this.current_user.account_id);
      this.lbl_assetname_heading.Text = asset.code.ToString() + "/" + asset.name.ToString();
      foreach (DataRow row in (InternalDataCollectionBase) this.Asset_ID.Tables[0].Rows)
        this.assetID.Add(Convert.ToInt64(row["asset_id"].ToString()));
      user user = this.users.get_user(this.objasset_booking.booked_for, this.current_user.account_id);
      this.objasset_booking.book_from.ToString("dddd");
      this.txt_from.Value = this.objasset_booking.book_from.ToString(api_constants.display_datetime_format);
      this.objasset_booking.book_to.ToString("dddd");
      this.txt_to.Value = this.objasset_booking.book_to.ToString(api_constants.display_datetime_format);
      this.lbl_email.Value = this.objasset_booking.email;
      this.lbl_purpose.Value = this.objasset_booking.purpose;
      this.lbl_remarks.Value = this.objasset_booking.remarks;
      this.lbl_telephone.Value = this.objasset_booking.contact;
      this.lbl_bookedfor.Value = user.full_name;
      this.populate_asset();
    }
    catch (Exception ex)
    {
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e) => Modal.Close((Page) this, (object) "OK");

  private void populate_asset()
  {
    try
    {
      DataSet assets = this.assets.get_assets(this.current_user.account_id);
      this.setting_data = this.settings.get_settings(this.current_user.account_id);
      this.asset_pro_ds = this.assets.get_asset_properties(this.current_user.account_id);
      this.htmltable = this.bookingsbl.getAssetHtml_with_assetID(assets, this.setting_data, this.asset_pro_ds, this.assetID, this.objasset_booking.book_from, this.objasset_booking.book_to);
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
