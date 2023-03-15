// Decompiled with JetBrains decompiler
// Type: assets_list
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
using System.Web.UI.WebControls;

public class assets_list : fbs_base_page, IRequiresSessionState
{
  public string html_table;
  public DataSet data;
  public DataSet faulty_asset_data;
  private StringBuilder html = new StringBuilder();
  protected Button btnExportExcel;
  protected HiddenField hdn_assetsearch;
  protected HiddenField hidorderby;
  protected HiddenField hidorderdir;

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      this.faulty_asset_data = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_faulty_assets");
      if (this.faulty_asset_data == null)
      {
        this.faulty_asset_data = this.assets.get_faulty_assets(this.current_user.account_id);
        this.capi.set_cache(this.current_user.account_id.ToString() + "_faulty_assets", (object) this.faulty_asset_data);
      }
      this.btnExportExcel.Attributes.Add("style", "width:160px; height:29px; font-weight:normal; color:#333; font-size:14px; font-family:Segoe UI,Helvetica,Arial,sans-serif; padding:0 0 6px 13px; background-color:#FFF; text-align:left;");
      this.populate_assets();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_assets()
  {
    bool flag = false;
    if (this.faulty_asset_data.Tables[0].Rows.Count > 0)
      flag = true;
    StringBuilder stringBuilder = new StringBuilder();
    DataSet dataSet = (DataSet) this.capi.get_cache(this.current_user.account_id.ToString() + "_assets_list");
    if (dataSet == null || dataSet.Tables[0].Rows.Count == 0)
    {
      dataSet = this.assets.get_assets_list(this.current_user.account_id);
      this.capi.set_cache(this.current_user.account_id.ToString() + "_assets_list", (object) dataSet);
    }
    foreach (DataRow row in (InternalDataCollectionBase) dataSet.Tables[0].Rows)
    {
      int num = 0;
      string str = "";
      if (flag)
      {
        DataRow[] dataRowArray = this.faulty_asset_data.Tables[0].Select("asset_id='" + row["asset_id"].ToString() + "'");
        num = dataRowArray.Length;
        if (num > 0)
        {
          foreach (DataRow dataRow in dataRowArray)
            str = str + dataRow["remarks"].ToString() + ",";
        }
      }
      stringBuilder.Append("<tr>");
      if (num == 0)
        stringBuilder.Append("<td>" + row["code"].ToString() + "/ " + row["name"].ToString() + "</td>");
      else
        stringBuilder.Append("<td>" + row["code"].ToString() + "/ " + row["name"].ToString() + " <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
      stringBuilder.Append("<td>" + row["building_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["level_name"].ToString() + "</td>");
      if (Convert.ToInt32(row["capacity"]) == -1)
        stringBuilder.Append("<td>N/A</td>");
      else
        stringBuilder.Append("<td>" + row["capacity"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["category_name"].ToString() + "</td>");
      stringBuilder.Append("<td>" + row["asset_type_name"].ToString() + "</td>");
      if (Convert.ToBoolean(row["is_restricted"]))
        stringBuilder.Append("<td>Yes</td>");
      else
        stringBuilder.Append("<td>No</td>");
      stringBuilder.Append("<td>" + str + "</td>");
      if (row["status_name"].ToString() == "Available")
        stringBuilder.Append("<td><span class='label label-Available'>" + row["status_name"].ToString() + "</span></td>");
      else
        stringBuilder.Append("<td><span class='label label-NotAvailable    '>" + row["status_name"].ToString() + "</span></td>");
      stringBuilder.Append("<td><a href='javascript:callfancybox(" + row["asset_id"].ToString() + ")'>View</a></td>");
      stringBuilder.Append("</tr>");
    }
    this.html_table = stringBuilder.ToString();
    this.ViewState.Add("table", (object) this.html_table);
  }

  protected void btnExportExcel_Click(object sender, EventArgs e)
  {
    try
    {
      this.Response.Clear();
      this.Response.AddHeader("content-disposition", "attachment;filename=" + this.current_user.username + "_facilitieslist_" + this.current_timestamp.ToString(api_constants.display_datetime_format) + ".xls");
      this.Response.Charset = "";
      this.Response.ContentType = Resources.fbs.excel_mime_type;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<table border=\"1\" cellspacing=\"2\" cellpadding=\"2\">");
      stringBuilder.Append("<thead>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th colspan='8' style='font-size:x-large;'>Assets List</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th colspan='8'></th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Code / Name</th>");
      stringBuilder.Append("<th>Building</th>");
      stringBuilder.Append("<th>Level</th>");
      stringBuilder.Append("<th>Capacity</th>");
      stringBuilder.Append("<th>Category</th>");
      stringBuilder.Append("<th>Type</th>");
      stringBuilder.Append("<th>Restricted</th>");
      stringBuilder.Append("<th>Comment</th>");
      stringBuilder.Append("<th>Status</th>");
      stringBuilder.Append("<th>view</th>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</thead>");
      stringBuilder.Append("<tbody>");
      string str = ((string) this.ViewState["table"]).Replace("View</a>", "</a>");
      stringBuilder.Append(str);
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Generated By </th>");
      stringBuilder.Append("<td>" + this.current_user.full_name + " </td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("<tr>");
      stringBuilder.Append("<th>Generated on </th>");
      stringBuilder.Append("<td align='left'>" + this.tzapi.current_user_timestamp().ToString(api_constants.display_datetime_format) + "</td>");
      stringBuilder.Append("</tr>");
      stringBuilder.Append("</tbody>");
      stringBuilder.Append("</table>");
      this.html_table = stringBuilder.ToString();
      this.Response.Write(this.html_table.ToString());
      this.Response.End();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
