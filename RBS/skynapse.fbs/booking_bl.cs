// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.booking_bl
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace skynapse.fbs
{
  public class booking_bl : api_base
  {
    private Dictionary<string, int> day_of_week_collection;
    private users_api users;
    private blacklist_api blapi;
    private holidays_api holidays;
    private booking_api bookings;
    private templates_api tapi;
    private resource_api resapi;
    private email_api eapi;
    private asset_api assets;
    private settings_api settings;
    private timezone_api tzapi;
    private workflow_api workflows;
    private user current_user;
    private account current_account;
    private DateTime current_timestamp;
    private groups_permission gp;

    public booking_bl()
    {
      this.users = new users_api();
      this.blapi = new blacklist_api();
      this.holidays = new holidays_api();
      this.bookings = new booking_api();
      this.tapi = new templates_api();
      this.resapi = new resource_api();
      this.eapi = new email_api("smpt");
      this.assets = new asset_api();
      this.settings = new settings_api();
      this.tzapi = new timezone_api();
      this.workflows = new workflow_api();
      this.gp = new groups_permission();
      this.day_of_week_collection = new Dictionary<string, int>();
      this.day_of_week_collection.Add(DayOfWeek.Sunday.ToString(), 0);
      this.day_of_week_collection.Add(DayOfWeek.Monday.ToString(), 1);
      this.day_of_week_collection.Add(DayOfWeek.Tuesday.ToString(), 2);
      this.day_of_week_collection.Add(DayOfWeek.Wednesday.ToString(), 3);
      this.day_of_week_collection.Add(DayOfWeek.Thursday.ToString(), 4);
      this.day_of_week_collection.Add(DayOfWeek.Friday.ToString(), 5);
      this.day_of_week_collection.Add(DayOfWeek.Saturday.ToString(), 6);
    }

    public void set_account(account acc, user usr, groups_permission grp_perm)
    {
      this.current_account = acc;
      this.current_user = usr;
      this.gp = grp_perm;
      this.current_timestamp = DateTime.UtcNow.AddHours(this.current_account.timezone);
    }

    public bool check_user_is_blacklist(
      long user_id,
      Guid acc_id,
      Dictionary<string, string> selectedDates)
    {
      try
      {
        blacklist blacklist1 = new blacklist();
        blacklist blacklistByUserId = this.blapi.get_blacklist_by_user_id(user_id, acc_id);
        if (blacklistByUserId.blacklist_id > 0L)
        {
          if (blacklistByUserId.blacklist_type == (short) 1)
            return true;
          foreach (string key in selectedDates.Keys)
          {
            DataSet dataSet = new DataSet();
            DataSet blacklist2 = this.blapi.get_blacklist(key, selectedDates[key], acc_id);
            if (this.utilities.isValidDataset(blacklist2))
            {
              foreach (DataRow row in (InternalDataCollectionBase) blacklist2.Tables[0].Rows)
              {
                if (row[nameof (user_id)].ToString() == user_id.ToString())
                  return true;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    public string getPublicHolidayList(string from, string to, Guid account_id)
    {
      string publicHolidayList = "";
      try
      {
        DataSet ds = this.holidays.view_holidays(from, to, account_id);
        if (this.utilities.isValidDataset(ds))
        {
          publicHolidayList = "<table><tr>";
          foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
            publicHolidayList = publicHolidayList + "<td><Strong>" + row["holiday_name"].ToString() + "</strong> - " + Convert.ToDateTime(row["from_date"]).ToString(api_constants.display_datetime_format_short) + " - " + Convert.ToDateTime(row["to_date"]).ToString(api_constants.display_datetime_format_short) + "</td><td>&nbsp;</td>";
          publicHolidayList += "</tr></table>";
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return publicHolidayList;
    }

    public string getAssetHtml_with_assetID(
      DataSet assetDs,
      DataSet sDS,
      DataSet assProDs,
      List<long> asset_id,
      DateTime dateFrom,
      DateTime dateTo)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = "";
      try
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table' width='100%'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>Room</th>");
        stringBuilder.Append("<th class='hidden-480'>Building</th>");
        stringBuilder.Append("<th class='hidden-480'>Level</th>");
        stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
        stringBuilder.Append("<th class='hidden-480'>Category</th>");
        stringBuilder.Append("<th class='hidden-480'>From</th>");
        stringBuilder.Append("<th class='hidden-480'>To</th>");
        stringBuilder.Append("<th class='hidden-480'>Comments</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        foreach (long num in asset_id)
        {
          DataRow[] dataRowArray1 = assetDs.Tables[0].Select("asset_id=" + num.ToString());
          DataRow[] dataRowArray2 = assProDs.Tables[0].Select("asset_id=" + num.ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
          foreach (DataRow dataRow1 in dataRowArray1)
          {
            stringBuilder.Append("<tr class='odd gradeX'>");
            if (dataRowArray2.Length > 0)
            {
              if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
                stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
              else
                stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
            }
            else if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
              stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "</td>");
            else
              stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["building_id"].ToString()) + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["level_id"].ToString()) + "</td>");
            if (dataRow1["capacity"].ToString() == "-1")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td>" + dataRow1["capacity"].ToString() + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["category_id"].ToString()) + "</td>");
            string str2 = Convert.ToDateTime(dateFrom).ToString("dddd");
            stringBuilder.Append("<td>" + str2 + "," + Convert.ToDateTime(dateFrom).ToString(api_constants.display_datetime_format) + "</td>");
            string str3 = Convert.ToDateTime(dateTo).ToString("dddd");
            stringBuilder.Append("<td>" + str3 + "," + Convert.ToDateTime(dateTo).ToString(api_constants.display_datetime_format) + "</td>");
            foreach (DataRow dataRow2 in dataRowArray2)
            {
              DataRow[] dataRowArray3 = sDS.Tables[0].Select("setting_id=" + dataRow2["property_value"].ToString() + "  and parameter='asset_property'");
              str1 = str1 + dataRowArray3[0]["value"].ToString() + " - " + dataRow2["remarks"].ToString() + ", ";
            }
            if (str1.Length > 0)
              str1 = str1.Substring(0, str1.Length - 2);
          }
          stringBuilder.Append("<td>" + str1 + "</td>");
          stringBuilder.Append("</div></div></td>");
          stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return stringBuilder.ToString();
    }

    public string getAssetHtml_with_bookingDates(
      DataSet assetDs,
      DataSet sDS,
      DataSet assProDs,
      long asset_id,
      Dictionary<string, string> selectedDates)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table' width='100%'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>Room</th>");
        stringBuilder.Append("<th class='hidden-480'>Building</th>");
        stringBuilder.Append("<th class='hidden-480'>Level</th>");
        stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
        stringBuilder.Append("<th class='hidden-480'>Category</th>");
        stringBuilder.Append("<th class='hidden-480'>From</th>");
        stringBuilder.Append("<th class='hidden-480'>To</th>");
        stringBuilder.Append("<th class='hidden-480'>Comments</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        DataRow[] dataRowArray1 = assetDs.Tables[0].Select("asset_id=" + asset_id.ToString());
        DataRow[] dataRowArray2 = assProDs.Tables[0].Select("asset_id=" + asset_id.ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
        foreach (string key in selectedDates.Keys)
        {
          foreach (DataRow dataRow1 in dataRowArray1)
          {
            string str1 = "";
            stringBuilder.Append("<tr class='odd gradeX'>");
            if (dataRowArray2.Length > 0)
            {
              if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
                stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
              else
                stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
            }
            else if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
              stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "</td>");
            else
              stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["building_id"].ToString()) + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["level_id"].ToString()) + "</td>");
            if (dataRow1["capacity"].ToString() == "-1")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td>" + dataRow1["capacity"].ToString() + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["category_id"].ToString()) + "</td>");
            string str2 = Convert.ToDateTime(key).ToString("dddd");
            stringBuilder.Append("<td>" + str2 + "," + Convert.ToDateTime(key).ToString(api_constants.display_datetime_format) + "</td>");
            string str3 = Convert.ToDateTime(selectedDates[key]).ToString("dddd");
            stringBuilder.Append("<td>" + str3 + "," + Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format) + "</td>");
            foreach (DataRow dataRow2 in dataRowArray2)
            {
              DataRow[] dataRowArray3 = sDS.Tables[0].Select("setting_id=" + dataRow2["property_value"].ToString() + "  and parameter='asset_property'");
              str1 = str1 + dataRowArray3[0]["value"].ToString() + " - " + dataRow2["remarks"].ToString() + ", ";
            }
            if (str1.Length > 0)
              str1 = str1.Substring(0, str1.Length - 2);
            stringBuilder.Append("<td>" + str1 + "</td>");
            stringBuilder.Append("</div></div></td>");
            stringBuilder.Append("</tr>");
          }
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return stringBuilder.ToString();
    }

    public DataTable getAssetDTable_with_bookingDates(
      DataSet assetDs,
      DataSet sDS,
      DataSet assProDs,
      long asset_id,
      Dictionary<string, string> selectedDates)
    {
      DataTable withBookingDates = new DataTable();
      try
      {
        withBookingDates.Columns.Add("Code");
        withBookingDates.Columns.Add("Name");
        withBookingDates.Columns.Add("Code_Name");
        withBookingDates.Columns.Add("Building");
        withBookingDates.Columns.Add("Level");
        withBookingDates.Columns.Add("Capacity");
        withBookingDates.Columns.Add("Category");
        withBookingDates.Columns.Add("FromDate");
        withBookingDates.Columns.Add("FromTime");
        withBookingDates.Columns.Add("ToDate");
        withBookingDates.Columns.Add("ToTime");
        withBookingDates.Columns.Add("Status");
        withBookingDates.Columns.Add("Comments");
        DataRow[] dataRowArray1 = assetDs.Tables[0].Select("asset_id=" + asset_id.ToString());
        DataRow[] dataRowArray2 = assProDs.Tables[0].Select("asset_id=" + asset_id.ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
        foreach (string key in selectedDates.Keys)
        {
          foreach (DataRow dataRow1 in dataRowArray1)
          {
            string str = "";
            DataRow row = withBookingDates.NewRow();
            row["Code"] = (object) dataRow1["code"].ToString();
            row["Name"] = (object) dataRow1["name"].ToString();
            if (dataRowArray2.Length > 0)
              row["Code_Name"] = (object) (dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' />");
            else
              row["Code_Name"] = (object) (dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString());
            row["Building"] = (object) this.utilities.get_setting_value(sDS.Tables[0], dataRow1["building_id"].ToString());
            row["Level"] = (object) this.utilities.get_setting_value(sDS.Tables[0], dataRow1["level_id"].ToString());
            row["Capacity"] = !(dataRow1["capacity"].ToString() == "-1") ? (object) dataRow1["capacity"].ToString() : (object) "NA";
            row["Category"] = (object) this.utilities.get_setting_value(sDS.Tables[0], dataRow1["category_id"].ToString());
            row["FromDate"] = (object) Convert.ToDateTime(key).ToString(api_constants.display_datetime_format_short);
            row["FromTime"] = (object) Convert.ToDateTime(key).ToString("hh:mm tt");
            row["ToDate"] = (object) Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format_short);
            row["ToTime"] = (object) Convert.ToDateTime(selectedDates[key]).ToString("hh:mm tt");
            row["Status"] = !(dataRow1["Status"].ToString() == "1") ? (object) "<Span class='label label-NotAvailable'> NotAvailable</span>" : (object) "<Span class='label label-Available'> Available</span>";
            foreach (DataRow dataRow2 in dataRowArray2)
            {
              DataRow[] dataRowArray3 = sDS.Tables[0].Select("setting_id=" + dataRow2["property_value"].ToString() + "  and parameter='asset_property'");
              str = str + dataRowArray3[0]["value"].ToString() + " - " + dataRow2["remarks"].ToString() + ",";
            }
            if (str.Length > 0)
              str = str.Substring(0, str.Length - 1);
            row["Comments"] = (object) str;
            withBookingDates.Rows.Add(row);
          }
        }
      }
      catch (Exception ex)
      {
        fbs_base_page.log.Error((object) ex.ToString());
      }
      return withBookingDates;
    }

    public string getAssetHtml_with_repeat_bookingDates(
      DataSet assetDs,
      DataSet sDS,
      DataSet assProDs,
      long asset_id,
      Dictionary<string, string> selectedDates,
      bool isinvite)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='list_table' width='100%'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th class='hidden-480'>Room</th>");
        stringBuilder.Append("<th class='hidden-480'>Building</th>");
        stringBuilder.Append("<th class='hidden-480'>Level</th>");
        stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
        stringBuilder.Append("<th class='hidden-480'>Category</th>");
        stringBuilder.Append("<th class='hidden-480'>From</th>");
        stringBuilder.Append("<th class='hidden-480'>To</th>");
        stringBuilder.Append("<th class='hidden-480'>Comments</th>");
        if (isinvite)
        {
          stringBuilder.Append("<th class='hidden-480'>Attend</th>");
          stringBuilder.Append("<th class='hidden-480'>Not Attend</th>");
        }
        else
          stringBuilder.Append("<th class='hidden-480'>Cancel</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        DataRow[] dataRowArray1 = assetDs.Tables[0].Select("asset_id=" + asset_id.ToString());
        DataRow[] dataRowArray2 = assProDs.Tables[0].Select("asset_id=" + asset_id.ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
        foreach (string key in selectedDates.Keys)
        {
          foreach (DataRow dataRow1 in dataRowArray1)
          {
            string str1 = "";
            stringBuilder.Append("<tr class='odd gradeX'>");
            if (dataRowArray2.Length > 0)
            {
              if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
                stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
              else
                stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
            }
            else if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
              stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "</td>");
            else
              stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["building_id"].ToString()) + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["level_id"].ToString()) + "</td>");
            if (dataRow1["capacity"].ToString() == "-1")
              stringBuilder.Append("<td> NA </td>");
            else
              stringBuilder.Append("<td>" + dataRow1["capacity"].ToString() + "</td>");
            stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["category_id"].ToString()) + "</td>");
            stringBuilder.Append("<td>" + Convert.ToDateTime(key).ToString(api_constants.display_datetime_format) + "</td>");
            stringBuilder.Append("<td>" + Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format) + "</td>");
            foreach (DataRow dataRow2 in dataRowArray2)
            {
              DataRow[] dataRowArray3 = sDS.Tables[0].Select("setting_id=" + dataRow2["property_value"].ToString() + "  and parameter='asset_property'");
              str1 = str1 + dataRowArray3[0]["value"].ToString() + " - " + dataRow2["remarks"].ToString() + ", ";
            }
            if (str1.Length > 0)
              str1 = str1.Substring(0, str1.Length - 2);
            stringBuilder.Append("<td>" + str1 + "</td>");
            if (isinvite)
            {
              string str2 = this.site_full_path + "view_booking.aspx?action=going&invite_id=[invite_id]&id=" + this.get_booking_id_by_assetid_dates(asset_id, Convert.ToDateTime(key).ToString(api_constants.display_datetime_format), Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format));
              string str3 = this.site_full_path + "view_booking.aspx?action=not_going&invite_id=[invite_id]&id=" + this.get_booking_id_by_assetid_dates(asset_id, Convert.ToDateTime(key).ToString(api_constants.display_datetime_format), Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format));
              stringBuilder.Append("<td><a href='" + str2 + "'>Attending</a></td>");
              stringBuilder.Append("<td><a href='" + str3 + "'>Not Attending</a></td>");
            }
            else
            {
              string str4 = this.site_full_path + "/booking_cancel.aspx?id=" + this.get_booking_id_by_assetid_dates(asset_id, Convert.ToDateTime(key).ToString(api_constants.display_datetime_format), Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format));
              stringBuilder.Append("<td><a href='" + str4 + "'>Cancel</a></td>");
            }
            stringBuilder.Append("</div></div></td>");
            stringBuilder.Append("</tr>");
          }
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return stringBuilder.ToString();
    }

    public string getAssetHtml(Dictionary<long, asset> av_assets, DataSet sDS, DataSet assProDs)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("<table class='table table-striped table-bordered table-hover' id='available_list_table'>");
        stringBuilder.Append("<thead>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th style='width:8px;'></th>");
        stringBuilder.Append("<th class='hidden-480'>Code/ Name</th>");
        stringBuilder.Append("<th class='hidden-480'>Building</th>");
        stringBuilder.Append("<th class='hidden-480'>Level</th>");
        stringBuilder.Append("<th class='hidden-480'>Capacity</th>");
        stringBuilder.Append("<th class='hidden-480'>Comments</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</thead>");
        stringBuilder.Append("<tbody>");
        foreach (KeyValuePair<long, asset> avAsset in av_assets)
        {
          asset asset = avAsset.Value;
          stringBuilder.Append("<tr class='odd gradeX'>");
          string str1 = "";
          string str2 = "";
          if (this.gp.isAdminType)
          {
            stringBuilder.Append(" <td><input type='radio'  class='radio book' name='optionsRadios1' id='" + asset.asset_id.ToString() + "' value='" + asset.asset_id.ToString() + "' /></td>");
            str2 = "<img src='assets/img/bo.png'/>";
          }
          else if (asset.is_book)
          {
            stringBuilder.Append(" <td><input type='radio'  class='radio book' name='optionsRadios1' id='" + asset.asset_id.ToString() + "' value='" + asset.asset_id.ToString() + "' /></td>");
            str2 = "<img src='assets/img/bo.png'/>";
          }
          else
          {
            stringBuilder.Append(" <td><input type='radio'  class='radio view' disabled name='optionsRadios1' id='" + asset.asset_id.ToString() + "' value='" + asset.asset_id.ToString() + "' /></td>");
            str1 = "style='color:red;text-decoration:none;'";
            str2 = "<img src='assets/img/bn.png'/>";
          }
          string str3 = asset.name;
          if (string.IsNullOrEmpty(asset.code))
            str3 = asset.code + "/" + asset.name;
          string str4 = "";
          string str5 = "";
          DataRow[] dataRowArray = assProDs.Tables[0].Select("asset_id=" + asset.asset_id.ToString() + " and ((status=1 and available=0) or status=0) and property_name='asset_property'");
          if (dataRowArray.Length > 0)
          {
            str4 = "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' />";
            foreach (DataRow dataRow in dataRowArray)
              str5 = str5 + ", " + dataRow["remarks"].ToString() + "</br>";
          }
          string str6;
          if (asset.asset_owner_group_id.ToString() != "0")
          {
            bool flag = false;
            foreach (user_group userGroup in this.current_user.groups.Values)
            {
              if (userGroup.group_id == asset.asset_owner_group_id)
                flag = true;
            }
            str6 = !flag ? "<img src='assets/img/ba.png'/>" : "<img src='assets/img/bo.png'/>";
          }
          else
            str6 = "<img src='assets/img/bo.png'/>";
          string str7 = "";
          if (this.gp.isAdminType)
            str6 = "<img src='assets/img/bo.png'/>";
          stringBuilder.AppendFormat("<td>" + str6 + " <a " + str7 + " href='javascript:assetinfo({0});'>" + str3 + "</a>" + str4 + "</td>", (object) asset.asset_id);
          stringBuilder.Append("<td>" + asset.building.value + "</span></td>");
          stringBuilder.Append("<td>" + asset.level.value + "</td>");
          if (asset.capacity != (short) -1)
            stringBuilder.Append("<td>" + (object) asset.capacity + "</td>");
          else
            stringBuilder.Append("<td>NA</td>");
          stringBuilder.Append("<td>" + str5 + "</td>");
          stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("</tbody>");
        stringBuilder.Append("</table>");
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return stringBuilder.ToString();
    }

    public void add_attachment_for_email(email emailObj, DateTime from, DateTime to)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("BEGIN:VCALENDAR");
        stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
        stringBuilder.AppendLine("VERSION:2.0");
        stringBuilder.AppendLine("METHOD:REQUEST");
        stringBuilder.AppendLine("BEGIN:VEVENT");
        stringBuilder.AppendLine(string.Format("DTSTART: " + from.ToString("yyyyMMddTHHmmssZ")));
        stringBuilder.AppendLine(string.Format("DTEND: " + to.ToString("yyyyMMddTHHmmssZ")));
        stringBuilder.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", (object) this.current_timestamp));
        stringBuilder.AppendLine(string.Format("CREATED:{0:yyyyMMddTHHmmssZ}", (object) this.current_timestamp));
        stringBuilder.AppendLine(string.Format("LAST-MODIFIED:{0:yyyyMMddTHHmmssZ}", (object) this.current_timestamp));
        stringBuilder.AppendLine("LOCATION: ");
        stringBuilder.AppendLine(string.Format("UID:{0}", (object) Guid.NewGuid()));
        stringBuilder.AppendLine("SEQUENCE:0");
        stringBuilder.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", (object) emailObj.body));
        stringBuilder.AppendLine(string.Format("SUMMARY:{0}", (object) emailObj.subject));
        stringBuilder.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", (object) emailObj.from_msg));
        stringBuilder.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=FALSE:mailto:{1}", (object) emailObj.to_msg, (object) emailObj.to_msg));
        stringBuilder.AppendLine("BEGIN:VALARM");
        stringBuilder.AppendLine("TRIGGER:-PT15M");
        stringBuilder.AppendLine("ACTION:DISPLAY");
        stringBuilder.AppendLine("DESCRIPTION:Reminder");
        stringBuilder.AppendLine("END:VALARM");
        stringBuilder.AppendLine("STATUS:CONFIRMED");
        stringBuilder.AppendLine("END:VEVENT");
        stringBuilder.AppendLine("END:VCALENDAR");
        byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
        attachment attachment = new attachment();
        attachment.account_id = this.current_user.account_id;
        attachment.created_on = this.current_timestamp;
        attachment.created_by = this.current_user.user_id;
        attachment.modified_on = this.current_timestamp;
        attachment.modified_by = this.current_user.user_id;
        attachment.message_id = emailObj.message_id;
        attachment.attachment_id = 0L;
        attachment.content_data = bytes;
        attachment.record_id = emailObj.record_id;
        attachment.mime_type = "text/calendar";
        attachment.file_extention = "ics";
        attachment.file_name = emailObj.subject + ".ics";
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml("<propeties></propeties>");
        attachment.properties = xmlDocument;
        this.eapi.update_email_attachment(attachment);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
    }

    public void add_attachment_for_email(email emailObj, asset_booking booking)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("BEGIN:VCALENDAR");
        stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
        stringBuilder.AppendLine("VERSION:2.0");
        stringBuilder.AppendLine("METHOD:REQUEST");
        stringBuilder.AppendLine("BEGIN:VEVENT");
        stringBuilder.AppendLine(string.Format("DTSTART: " + booking.book_from.ToString("yyyyMMddTHHmmssZ")));
        stringBuilder.AppendLine(string.Format("DTEND: " + booking.book_to.ToString("yyyyMMddTHHmmssZ")));
        stringBuilder.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", (object) this.current_timestamp));
        stringBuilder.AppendLine(string.Format("CREATED:{0:yyyyMMddTHHmmssZ}", (object) booking.created_on));
        stringBuilder.AppendLine(string.Format("LAST-MODIFIED:{0:yyyyMMddTHHmmssZ}", (object) booking.modified_on));
        stringBuilder.AppendLine("LOCATION: ");
        stringBuilder.AppendLine(string.Format("UID:{0}", (object) Guid.NewGuid()));
        stringBuilder.AppendLine("SEQUENCE:" + (object) booking.sequence);
        stringBuilder.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", (object) emailObj.body));
        stringBuilder.AppendLine(string.Format("SUMMARY:{0}", (object) emailObj.subject));
        stringBuilder.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", (object) emailObj.from_msg));
        if (booking.repeat_reference_id != Guid.Empty)
          stringBuilder.AppendLine(string.Format("RECURRENCE-ID:{0}", (object) booking.book_from.ToString("yyyyMMddTHHmmssZ")));
        stringBuilder.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=FALSE:mailto:{1}", (object) emailObj.to_msg, (object) emailObj.to_msg));
        stringBuilder.AppendLine("BEGIN:VALARM");
        stringBuilder.AppendLine("TRIGGER:-PT15M");
        stringBuilder.AppendLine("ACTION:DISPLAY");
        stringBuilder.AppendLine("DESCRIPTION:Reminder");
        stringBuilder.AppendLine("END:VALARM");
        if (booking.status == (short) 0)
          stringBuilder.AppendLine("STATUS:CANCELLED");
        if (booking.status == (short) 1)
          stringBuilder.AppendLine("STATUS:CONFIRMED");
        if (booking.status == (short) 4)
          stringBuilder.AppendLine("STATUS:TENTATIVE");
        stringBuilder.AppendLine("END:VEVENT");
        stringBuilder.AppendLine("END:VCALENDAR");
        byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
        attachment attachment = new attachment();
        attachment.account_id = this.current_user.account_id;
        attachment.created_on = this.current_timestamp;
        attachment.created_by = this.current_user.user_id;
        attachment.modified_on = this.current_timestamp;
        attachment.modified_by = this.current_user.user_id;
        attachment.message_id = emailObj.message_id;
        attachment.attachment_id = 0L;
        attachment.content_data = bytes;
        attachment.record_id = emailObj.record_id;
        attachment.mime_type = "text/calendar";
        attachment.file_extention = "ics";
        attachment.file_name = emailObj.subject + ".ics";
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml("<propeties></propeties>");
        attachment.properties = xmlDocument;
        this.eapi.update_email_attachment(attachment);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
    }

    public Dictionary<long, asset> get_non_restricted(Dictionary<long, asset> available_assets)
    {
      try
      {
        Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
        foreach (long key in available_assets.Keys)
        {
          asset availableAsset = available_assets[key];
          if (!availableAsset.is_restricted)
            dictionary.Add(availableAsset.asset_id, availableAsset);
        }
        available_assets = dictionary;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return available_assets;
    }

    public Dictionary<long, asset> get_available_assets(
      Dictionary<string, string> date_range,
      long building_id,
      long level_id,
      long category_id,
      int capacity,
      Guid account_id,
      string group_ids,
      bool isAdmin,
      DataSet asset_properties,
      DataSet settings_data,
      DateTime current_timestamp)
    {
      Dictionary<long, asset> availableAssets = new Dictionary<long, asset>();
      try
      {
        Dictionary<long, long> dictionary1 = new Dictionary<long, long>();
        Dictionary<long, asset> dictionary2 = new Dictionary<long, asset>();
        Dictionary<long, asset> dictionary3 = new Dictionary<long, asset>();
        string building = "0";
        string level = "0";
        string category = "0";
        string capacity1 = "0";
        if (building_id > 0L)
          building = building_id.ToString();
        if (level_id > 0L)
          level = level_id.ToString();
        if (category_id > 0L)
          category = category_id.ToString();
        if (capacity > 0)
          capacity1 = capacity.ToString();
        int index1 = 0;
        KeyValuePair<string, string> keyValuePair1 = date_range.ElementAt<KeyValuePair<string, string>>(index1);
        string key1 = keyValuePair1.Key;
        string str1 = keyValuePair1.Value;
        int index2 = date_range.Count - 1;
        KeyValuePair<string, string> keyValuePair2 = date_range.ElementAt<KeyValuePair<string, string>>(index2);
        string key2 = keyValuePair2.Key;
        string str2 = keyValuePair2.Value;
        availableAssets = this.bookings.check_available_assets(date_range, key1, date_range[key2], account_id, building, category, level, capacity1, group_ids, isAdmin, asset_properties, settings_data, current_timestamp);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return availableAssets;
    }

    public Dictionary<string, string> get_daily_dates(
      string start_date,
      string end_date,
      int no_of_events,
      int frequency)
    {
      Dictionary<string, string> dailyDates = new Dictionary<string, string>();
      try
      {
        int duration = this.utilities.getDuration(Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        DateTime dateTime1 = Convert.ToDateTime(start_date);
        DateTime dateTime2 = Convert.ToDateTime(start_date).AddMinutes((double) duration);
        if (no_of_events == 0)
        {
          if (dateTime2 <= Convert.ToDateTime(end_date) && !dailyDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
            dailyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
          while (dateTime1.AddDays((double) frequency) <= Convert.ToDateTime(end_date))
          {
            dateTime1 = dateTime1.AddDays((double) frequency);
            dateTime2 = dateTime2.AddDays((double) frequency);
            if (dateTime2 <= Convert.ToDateTime(end_date) && !dailyDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
              dailyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
          }
        }
        else
        {
          dailyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
          for (int index = 1; index < no_of_events; ++index)
          {
            dateTime1 = dateTime1.AddDays((double) frequency);
            dateTime2 = dateTime2.AddDays((double) frequency);
            if (!dailyDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
              dailyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return dailyDates;
    }

    public Dictionary<string, string> get_weekly_dates(
      string start_date,
      string end_date,
      int no_of_events,
      int frequency,
      List<int> days)
    {
      Dictionary<string, string> weeklyDates = new Dictionary<string, string>();
      try
      {
        int duration = this.utilities.getDuration(Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        for (int index1 = 0; index1 < 7; ++index1)
        {
          if (days.Contains(index1))
          {
            DateTime dateTime1 = Convert.ToDateTime(start_date);
            DateTime dateTime2 = Convert.ToDateTime(start_date).AddMinutes((double) duration);
            while (this.day_of_week_collection[dateTime1.DayOfWeek.ToString()] != index1)
            {
              dateTime1 = dateTime1.AddDays(1.0);
              dateTime2 = dateTime2.AddDays(1.0);
            }
            if (dateTime2 <= Convert.ToDateTime(end_date) && !weeklyDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
              weeklyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
            if (no_of_events == 0)
            {
              while (dateTime1.AddDays((double) (7 * frequency)) <= Convert.ToDateTime(end_date))
              {
                dateTime1 = dateTime1.AddDays((double) (7 * frequency));
                dateTime2 = dateTime2.AddDays((double) (7 * frequency));
                if (dateTime2 <= Convert.ToDateTime(end_date) && !weeklyDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
                  weeklyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
              }
            }
            else
            {
              for (int index2 = 1; index2 < no_of_events; ++index2)
              {
                dateTime1 = dateTime1.AddDays((double) (7 * frequency));
                dateTime2 = dateTime2.AddDays((double) (7 * frequency));
                if (!weeklyDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
                  weeklyDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return weeklyDates;
    }

    public Dictionary<string, string> get_monthly_every_dates(
      string start_date,
      string end_date,
      int no_of_events,
      int frequency,
      int day_of_month)
    {
      Dictionary<string, string> monthlyEveryDates = new Dictionary<string, string>();
      try
      {
        int duration = this.utilities.getDuration(Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        DateTime dateTime1 = Convert.ToDateTime(start_date);
        DateTime dateTime2 = Convert.ToDateTime(start_date).AddMinutes((double) duration);
        if (dateTime1.Day < day_of_month)
        {
          dateTime1 = dateTime1.AddDays((double) (day_of_month - dateTime1.Day));
          dateTime2 = dateTime2.AddDays((double) (day_of_month - dateTime2.Day));
        }
        else if (dateTime1.Day > day_of_month)
        {
          dateTime1 = new DateTime(dateTime1.AddMonths(frequency).Year, dateTime1.AddMonths(1).Month, day_of_month, dateTime1.Hour, dateTime1.Minute, 0);
          dateTime2 = new DateTime(dateTime2.AddMonths(frequency).Year, dateTime2.AddMonths(1).Month, day_of_month, dateTime2.Hour, dateTime2.Minute, 0);
        }
        if (dateTime2 <= Convert.ToDateTime(end_date) && !monthlyEveryDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
          monthlyEveryDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
        if (no_of_events == 0)
        {
          while (dateTime1.AddMonths(frequency) <= Convert.ToDateTime(end_date))
          {
            dateTime1 = dateTime1.AddMonths(frequency);
            dateTime2 = dateTime2.AddMonths(frequency);
            if (dateTime2 <= Convert.ToDateTime(end_date) && !monthlyEveryDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
              monthlyEveryDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
          }
        }
        else
        {
          for (int index = 1; index <= no_of_events; ++index)
          {
            if (index == 1)
            {
              if (!monthlyEveryDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
                monthlyEveryDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
            }
            else
            {
              dateTime1 = dateTime1.AddMonths(frequency);
              dateTime2 = dateTime2.AddMonths(frequency);
              if (!monthlyEveryDates.ContainsKey(dateTime1.ToString(api_constants.datetime_format)))
                monthlyEveryDates.Add(dateTime1.ToString(api_constants.datetime_format), dateTime2.ToString(api_constants.datetime_format));
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return monthlyEveryDates;
    }

    public Dictionary<string, string> get_monthly_specific_dates(
      string start_date,
      string end_date,
      int no_of_events,
      int frequency,
      int pattern,
      string day)
    {
      Dictionary<string, string> monthlySpecificDates = new Dictionary<string, string>();
      try
      {
        int duration = this.utilities.getDuration(Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        DateTime dateTime1 = Convert.ToDateTime(start_date);
        dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, 1, dateTime1.Hour, dateTime1.Minute, 0);
        int num1 = 0;
        if (pattern == 5)
          num1 = this.Count_no_of_days(dateTime1.Year, dateTime1.Month, day) != 5 ? 1 : 0;
        while (num1 < pattern)
        {
          while (dateTime1.DayOfWeek.ToString() != day)
            dateTime1 = dateTime1.AddDays(1.0);
          ++num1;
          if (num1 < pattern)
            dateTime1 = dateTime1.AddDays(1.0);
        }
        if (dateTime1 < DateTime.Now)
        {
          dateTime1 = dateTime1.AddMonths(frequency);
          dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, 1, dateTime1.Hour, dateTime1.Minute, 0);
          int num2 = 0;
          if (pattern == 5)
            num2 = this.Count_no_of_days(dateTime1.Year, dateTime1.Month, day) != 5 ? 1 : 0;
          while (num2 < pattern)
          {
            while (dateTime1.DayOfWeek.ToString() != day)
              dateTime1 = dateTime1.AddDays(1.0);
            ++num2;
            if (num2 < pattern)
              dateTime1 = dateTime1.AddDays(1.0);
          }
        }
        DateTime dateTime2 = dateTime1;
        DateTime dateTime3 = dateTime1.AddMinutes((double) duration);
        if (no_of_events == 0)
        {
          if (dateTime3 <= Convert.ToDateTime(end_date) && !monthlySpecificDates.ContainsKey(dateTime2.ToString(api_constants.datetime_format)))
            monthlySpecificDates.Add(dateTime2.ToString(api_constants.datetime_format), dateTime3.ToString(api_constants.datetime_format));
          while (dateTime2 <= Convert.ToDateTime(end_date))
          {
            dateTime1 = dateTime1.AddMonths(frequency);
            dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, 1, dateTime1.Hour, dateTime1.Minute, 0);
            int num3 = 0;
            if (pattern == 5)
              num3 = this.Count_no_of_days(dateTime1.Year, dateTime1.Month, day) != 5 ? 1 : 0;
            while (num3 < pattern)
            {
              while (dateTime1.DayOfWeek.ToString() != day)
                dateTime1 = dateTime1.AddDays(1.0);
              ++num3;
              if (num3 < pattern)
                dateTime1 = dateTime1.AddDays(1.0);
            }
            dateTime2 = dateTime1;
            DateTime dateTime4 = dateTime1.AddMinutes((double) duration);
            if (dateTime4 <= Convert.ToDateTime(end_date) && !monthlySpecificDates.ContainsKey(dateTime2.ToString(api_constants.datetime_format)))
              monthlySpecificDates.Add(dateTime2.ToString(api_constants.datetime_format), dateTime4.ToString(api_constants.datetime_format));
          }
        }
        else
        {
          if (!monthlySpecificDates.ContainsKey(dateTime2.ToString(api_constants.datetime_format)))
            monthlySpecificDates.Add(dateTime2.ToString(api_constants.datetime_format), dateTime3.ToString(api_constants.datetime_format));
          for (int index = 1; index < no_of_events; ++index)
          {
            dateTime1 = dateTime1.AddMonths(frequency);
            dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, 1, dateTime1.Hour, dateTime1.Minute, 0);
            int num4 = 0;
            if (pattern == 5)
              num4 = this.Count_no_of_days(dateTime1.Year, dateTime1.Month, day) != 5 ? 1 : 0;
            while (num4 < pattern)
            {
              while (dateTime1.DayOfWeek.ToString() != day)
                dateTime1 = dateTime1.AddDays(1.0);
              ++num4;
              if (num4 < pattern)
                dateTime1 = dateTime1.AddDays(1.0);
            }
            dateTime2 = dateTime1;
            dateTime3 = dateTime1.AddMinutes((double) duration);
            if (!monthlySpecificDates.ContainsKey(dateTime2.ToString(api_constants.datetime_format)))
              monthlySpecificDates.Add(dateTime2.ToString(api_constants.datetime_format), dateTime3.ToString(api_constants.datetime_format));
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return monthlySpecificDates;
    }

    private int Count_no_of_days(int year, int month, string day)
    {
      DateTime dateTime1 = new DateTime(year, month, 1);
      DateTime dateTime2 = dateTime1.AddDays(28.0);
      DateTime dateTime3 = dateTime1.AddDays(29.0);
      DateTime dateTime4 = dateTime1.AddDays(30.0);
      return dateTime2.Month == month && dateTime2.DayOfWeek.ToString() == day || dateTime3.Month == month && dateTime3.DayOfWeek.ToString() == day || dateTime4.Month == month && dateTime4.DayOfWeek.ToString() == day ? 5 : 4;
    }

    public bool send_booking_emails(
      user objBookedFor,
      string assDetails,
      asset_booking objBooking,
      long asset_owner_Group_id,
      DataSet setting_data,
      string invite_emails)
    {
      try
      {
        email mailObj = new email();
        DataSet ds = new DataSet();
        if (asset_owner_Group_id > 0L)
          ds = this.users.get_users_by_group(asset_owner_Group_id, this.current_user.account_id);
        string str1 = "";
        if (this.utilities.isValidDataset(ds))
        {
          foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
            str1 = str1 + row["email"].ToString() + ";";
        }
        if (str1.Length > 0)
          str1 = str1.Substring(0, str1.Length - 1);
        template objtemp = new template();
        if (objBooking.status == (short) 1)
        {
          if (!string.IsNullOrEmpty(objBookedFor.email))
            this.send_email_to_requestor(objBookedFor, assDetails, objBooking, objBookedFor.email, out mailObj, out objtemp);
          if (invite_emails.Length > 0)
            invite_emails = invite_emails.Substring(0, invite_emails.Length - 1);
          if (!string.IsNullOrEmpty(invite_emails))
          {
            objtemp = this.tapi.get_template("email_invitees", this.current_user.account_id);
            this.add_attachment_for_email(this.sendEmail("", this.replace_template(objBookedFor, assDetails, objtemp.content_data, objBooking, false, false), objtemp.title, "", invite_emails, objBooking.record_id), objBooking);
          }
          DataSet usersByGroupType = this.users.get_users_by_group_type(1L, this.current_user.account_id);
          string str2 = "";
          if (this.utilities.isValidDataset(usersByGroupType))
          {
            foreach (DataRow row in (InternalDataCollectionBase) usersByGroupType.Tables[0].Rows)
              str2 = str2 + row["email"].ToString() + ";";
          }
          if (str2.Length > 0)
            str2.Substring(0, str2.Length - 1);
          if (objBooking.housekeeping_required)
          {
            string to = setting_data.Tables[0].Select("parameter='catering_email'")[0]["value"].ToString();
            if (!string.IsNullOrEmpty(to))
            {
              objtemp = this.tapi.get_template("email_housekeeping", this.current_user.account_id);
              this.add_attachment_for_email(!this.bookings.check_sendeamil(this.current_user.account_id, objBooking.asset_id) ? this.sendEmail("", this.replace_template(objBookedFor, assDetails, objtemp.content_data, objBooking, false, false), objtemp.title, "", to, objBooking.record_id) : this.sendEmail("", this.replace_template(objBookedFor, assDetails, objtemp.content_data, objBooking, false, false), objtemp.title, str1, to, objBooking.record_id), objBooking);
            }
          }
          if (objBooking.setup_required)
          {
            string to = setting_data.Tables[0].Select("parameter='facilities_email'")[0]["value"].ToString();
            if (!string.IsNullOrEmpty(to))
            {
              objtemp = this.tapi.get_template("email_facilities", this.current_user.account_id);
              this.add_attachment_for_email(!this.bookings.check_sendeamil(this.current_user.account_id, objBooking.asset_id) ? this.sendEmail("", this.replace_template(objBookedFor, assDetails, objtemp.content_data, objBooking, false, false), objtemp.title, "", to, objBooking.record_id) : this.sendEmail("", this.replace_template(objBookedFor, assDetails, objtemp.content_data, objBooking, false, false), objtemp.title, str1, to, objBooking.record_id), objBooking);
            }
          }
        }
        else
        {
          if (!string.IsNullOrEmpty(objBookedFor.email))
          {
            template template = this.tapi.get_template("email_pending_approval", this.current_user.account_id);
            this.sendEmail("", this.replace_template(objBookedFor, assDetails, template.content_data, objBooking, false, false), template.title, "", objBookedFor.email, objBooking.record_id);
          }
          if (!string.IsNullOrEmpty(str1))
          {
            template template = this.tapi.get_template("email_booking_request", this.current_user.account_id);
            if (this.bookings.check_sendeamil(this.current_user.account_id, objBooking.asset_id))
              this.sendEmail("", this.replace_template(objBookedFor, assDetails, template.content_data, objBooking, false, true), template.title, "", str1, objBooking.record_id);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return false;
    }

    private void send_email_to_requestor(
      user objBookedFor,
      string assDetails,
      asset_booking objBooking,
      string booked_for_email,
      out email mailObj,
      out template objtemp)
    {
      asset asset = this.assets.get_asset(objBooking.asset_id, objBooking.account_id);
      Dictionary<string, string> items = this.get_items(objBooking, asset);
      objtemp = this.tapi.get_template("email_requestor", this.current_user.account_id);
      string body = this.replace_body(items, objtemp.content_data);
      mailObj = this.sendEmail("", body, objtemp.title, "", booked_for_email, objBooking.record_id);
      this.add_attachment_for_email(mailObj, objBooking);
    }

    public email sendEmail(
      string bcc,
      string body,
      string subject,
      string cc,
      string to,
      Guid recID)
    {
      email email = new email();
      try
      {
        DataRow[] dataRowArray = this.settings.get_settings(this.current_user.account_id).Tables[0].Select("parameter='from_email_address'");
        email.account_id = this.current_user.account_id;
        email.created_on = this.current_timestamp;
        email.modified_on = this.current_timestamp;
        email.bcc_msg = bcc;
        email.body = string.IsNullOrEmpty(body) ? "Test" : body;
        email.bounced = false;
        email.cc_msg = cc;
        email.created_by = this.current_user.user_id;
        email.email_message_id = Guid.NewGuid();
        email.from_msg = dataRowArray[0]["value"].ToString();
        email.is_html = true;
        email.message = "";
        email.message_id = 0L;
        email.message_type = 0;
        email.modified_by = this.current_user.user_id;
        email.record_id = recID;
        email.sent = false;
        email.subject = subject;
        email.to_msg = to;
        email = this.eapi.update_email(email);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("Ërror--> " + ex.ToString()));
      }
      return email;
    }

    public string replace_template(
      user objBookedFor,
      string assDet,
      string body,
      asset_booking obj,
      bool showCancelLink,
      bool showApproveRejectLink)
    {
      try
      {
        Dictionary<string, user_property> properties = objBookedFor.properties;
        string newValue1 = properties.ContainsKey("staff_division") ? properties["staff_division"].property_value : "";
        string newValue2 = properties.ContainsKey("staff_department") ? properties["staff_department"].property_value : "";
        string newValue3 = properties.ContainsKey("staff_section") ? properties["staff_section"].property_value : "";
        body = body.Replace("[FULL NAME]", objBookedFor.full_name);
        body = body.Replace("[PURPOSE]", obj.purpose);
        body = body.Replace("[BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
        body = body.Replace("[BOOKED STATUS]", this.bookings.get_status((long) obj.status));
        body = body.Replace("[REQUESTED BY]", objBookedFor.full_name);
        body = body.Replace("[REQUESTED ON]", this.tzapi.convert_to_user_timestamp(obj.created_on).ToString(api_constants.display_datetime_format));
        body = body.Replace("[REQUESTOR DIVISION]", newValue1);
        body = body.Replace("[REQUESTOR DEPARTMENT]", newValue2);
        body = body.Replace("[REQUESTOR SECTION]", newValue3);
        body = body.Replace("[CONTACT NO]", obj.contact);
        body = body.Replace("[EMAILS]", obj.email);
        if (showCancelLink)
        {
          string str = this.site_full_path + "/booking_cancel.aspx?id=" + (object) obj.booking_id;
          assDet = assDet + " <br/> For Cancel Request: <a href='" + str + "'>Cancel</a> ";
        }
        if (showApproveRejectLink)
        {
          string str = this.site_full_path + "/mytask.aspx";
          assDet = assDet + " <br/> For Approve/Reject Request : <a href='" + str + "'> [APPROVE] / [REJECT] </a> ";
        }
        body = body.Replace("[FACILITY DETAILS]", assDet);
        body = body.Replace("[SITE_FULL_PATH]", this.site_full_path);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return body;
    }

    public string replace_transfer_request_template(
      user objBookedFor,
      string assDet,
      string body,
      asset_booking obj,
      asset_booking objOld,
      user objPrevBookedFor,
      bool showCancelLink,
      bool showApproveRejectLink)
    {
      try
      {
        Dictionary<string, user_property> properties1 = objBookedFor.properties;
        string newValue1 = properties1.ContainsKey("staff_division") ? properties1["staff_division"].property_value : "";
        string newValue2 = properties1.ContainsKey("staff_department") ? properties1["staff_department"].property_value : "";
        string newValue3 = properties1.ContainsKey("staff_section") ? properties1["staff_section"].property_value : "";
        Dictionary<string, user_property> properties2 = objPrevBookedFor.properties;
        if (properties2.ContainsKey("staff_division"))
        {
          string propertyValue1 = properties2["staff_division"].property_value;
        }
        if (properties2.ContainsKey("staff_department"))
        {
          string propertyValue2 = properties2["staff_department"].property_value;
        }
        if (properties2.ContainsKey("staff_section"))
        {
          string propertyValue3 = properties2["staff_section"].property_value;
        }
        body = body.Replace("[NEW PURPOSE]", obj.purpose);
        body = body.Replace("[OLD PURPOSE]", objOld.purpose);
        body = body.Replace("[NEW BOOKED RANGE]", obj.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
        body = body.Replace("[OLD BOOKED RANGE]", objOld.book_from.ToString(api_constants.display_datetime_format) + " - " + obj.book_to.ToString(api_constants.display_datetime_format));
        body = body.Replace("[NEW BOOKED STATUS]", this.bookings.get_status((long) obj.status));
        body = body.Replace("[OLD BOOKED STATUS]", this.bookings.get_status((long) objOld.status));
        body = body.Replace("[NEW REQUESTED BY]", objBookedFor.full_name);
        body = body.Replace("[OLD REQUESTED BY]", objPrevBookedFor.full_name);
        body = body.Replace("[NEW REQUESTED ON]", this.tzapi.convert_to_user_timestamp(obj.created_on).ToString(api_constants.display_datetime_format));
        body = body.Replace("[OLD REQUESTED ON]", this.tzapi.convert_to_user_timestamp(objOld.created_on).ToString(api_constants.display_datetime_format));
        body = body.Replace("[NEW REQUESTOR DIVISION]", newValue1);
        body = body.Replace("[OLD REQUESTOR DIVISION]", newValue1);
        body = body.Replace("[NEW REQUESTOR DEPARTMENT]", newValue2);
        body = body.Replace("[OLD REQUESTOR DEPARTMENT]", newValue2);
        body = body.Replace("[NEW REQUESTOR SECTION]", newValue3);
        body = body.Replace("[OLD REQUESTOR SECTION]", newValue3);
        body = body.Replace("[NEW CONTACT NO]", obj.contact);
        body = body.Replace("[OLD CONTACT NO]", objOld.contact);
        body = body.Replace("[NEW EMAILS]", obj.email);
        body = body.Replace("[OLD EMAILS]", objOld.email);
        if (showCancelLink)
        {
          string str = this.site_full_path + "/booking_cancel.aspx?id=" + (object) obj.booking_id;
          assDet = assDet + " <br/> For Cancel Request: <a href='" + str + "'>Cancel</a> ";
        }
        if (showApproveRejectLink)
        {
          string str = this.site_full_path + "/mytask.aspx";
          assDet = assDet + " <br/> For Approve/Reject Request: <a href='" + str + "'> [APPROVE] / [REJECT] </a> ";
        }
        body = body.Replace("[FACILITY DETAILS]", assDet);
        body = body.Replace("[SITE_FULL_PATH]", this.site_full_path);
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return body;
    }

    public string getAssetHtml_with_bookingDates_CustomBookings(
      int counter,
      DataSet assetDs,
      DataSet sDS,
      DataSet assProDs,
      long asset_id,
      Dictionary<string, string> selectedDates,
      string fromdate,
      string bk_status)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        DataRow[] dataRowArray1 = assetDs.Tables[0].Select("asset_id=" + asset_id.ToString());
        DataRow[] dataRowArray2 = assProDs.Tables[0].Select("asset_id=" + asset_id.ToString() + " and property_name='asset_property' and (status=0 or (status=1 and available=0))");
        foreach (string key in selectedDates.Keys)
        {
          foreach (DataRow dataRow1 in dataRowArray1)
          {
            if (key == fromdate)
            {
              string str = "";
              stringBuilder.Append("<tr class='odd gradeX'>");
              stringBuilder.Append("<td>" + (object) counter + "</td>");
              if (dataRowArray2.Length > 0)
              {
                if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
                  stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
                else
                  stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "<img id='img_prop' style='float:right;' src='" + this.site_full_path + "assets/img/Facilityerro.png' alt='Faulty Room' /></td>");
              }
              else if (string.IsNullOrEmpty(dataRow1["code"].ToString()))
                stringBuilder.Append("<td>" + dataRow1["name"].ToString() + "</td>");
              else
                stringBuilder.Append("<td>" + dataRow1["code"].ToString() + " / " + dataRow1["name"].ToString() + "</td>");
              stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["building_id"].ToString()) + "</td>");
              stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["level_id"].ToString()) + "</td>");
              if (dataRow1["capacity"].ToString() == "-1")
                stringBuilder.Append("<td> NA </td>");
              else
                stringBuilder.Append("<td>" + dataRow1["capacity"].ToString() + "</td>");
              stringBuilder.Append("<td>" + this.utilities.get_setting_value(sDS.Tables[0], dataRow1["category_id"].ToString()) + "</td>");
              stringBuilder.Append("<td>" + Convert.ToDateTime(key).ToString(api_constants.display_datetime_format) + "</td>");
              stringBuilder.Append("<td>" + Convert.ToDateTime(selectedDates[key]).ToString(api_constants.display_datetime_format) + "</td>");
              foreach (DataRow dataRow2 in dataRowArray2)
              {
                DataRow[] dataRowArray3 = sDS.Tables[0].Select("setting_id=" + dataRow2["property_value"].ToString() + "  and parameter='asset_property'");
                str = str + dataRowArray3[0]["value"].ToString() + " - " + dataRow2["remarks"].ToString() + ", ";
              }
              if (str.Length > 0)
                str = str.Substring(0, str.Length - 2);
              if (bk_status != "")
                stringBuilder.Append("<td> " + bk_status + " </td>");
              else
                stringBuilder.Append("<td> </td>");
              stringBuilder.Append("<td>" + str + "</td>");
              stringBuilder.Append("</div></div></td>");
              stringBuilder.Append("</tr>");
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return stringBuilder.ToString();
    }

    public string get_booking_id_by_assetid_dates(long asset_id, string from_date, string to_date)
    {
      string idByAssetidDates = "";
      try
      {
        if (this.db.get_dataset("select booking_id from sbt_asset_bookings where asset_id = " + (object) asset_id + " and book_from = '" + from_date + "' and book_to = '" + to_date + "'"))
        {
          DataSet resultDataSet = this.db.resultDataSet;
          if (resultDataSet.Tables.Count > 0)
          {
            if (resultDataSet.Tables[0].Rows.Count > 0)
              idByAssetidDates = resultDataSet.Tables[0].Rows[0][0].ToString();
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return idByAssetidDates;
    }

    public Dictionary<long, asset> filter_assets(
      Dictionary<long, asset> asset_list,
      DataSet settings,
      DataSet asset_properties,
      DateTime from,
      DateTime to)
    {
      if (this.gp.isAdminType)
        return asset_list;
      Dictionary<long, asset> dictionary = new Dictionary<long, asset>();
      foreach (long key1 in asset_list.Keys)
      {
        if (this.can_book(asset_list[key1].asset_id, asset_properties, settings, from, to))
          dictionary.Add(key1, asset_list[key1]);
        else if (asset_list[key1].asset_owner_group_id > 0L)
        {
          foreach (string key2 in this.current_user.groups.Keys)
          {
            if (this.current_user.groups[key2].group_id == asset_list[key1].asset_owner_group_id)
              dictionary.Add(key1, asset_list[key1]);
          }
        }
      }
      return dictionary;
    }

    public bool can_book(
      long asset_id,
      Dictionary<long, asset_property> asset_properties,
      DataSet settings,
      DateTime from,
      DateTime to)
    {
      bool flag1 = true;
      DateTime dateTime1 = new DateTime();
      DateTime dateTime2 = new DateTime();
      bool flag2 = false;
      bool flag3 = false;
      asset_property assetProperty1 = new asset_property();
      foreach (asset_property assetProperty2 in asset_properties.Values)
      {
        if (assetProperty2.property_name == "operating_hours")
          assetProperty1 = assetProperty2;
      }
      DateTime dateTime3;
      DateTime dateTime4;
      if (assetProperty1.asset_property_id > 0L)
      {
        string[] strArray = assetProperty1.property_value.Split('|');
        dateTime3 = Convert.ToDateTime(strArray[0]);
        dateTime4 = Convert.ToDateTime(strArray[1]);
      }
      else
      {
        string[] strArray = settings.Tables[0].Select("parameter='operating_hours'")[0]["value"].ToString().Split('|');
        dateTime3 = Convert.ToDateTime(strArray[0]);
        dateTime4 = Convert.ToDateTime(strArray[1]);
      }
      asset_property assetProperty3 = new asset_property();
      foreach (asset_property assetProperty4 in asset_properties.Values)
      {
        if (assetProperty4.property_name == "book_holiday")
          assetProperty3 = assetProperty4;
      }
      if (assetProperty3.asset_property_id > 0L)
      {
        flag3 = Convert.ToBoolean(assetProperty3.property_value);
      }
      else
      {
        DataRow[] dataRowArray = settings.Tables[0].Select("parameter='book_holiday'");
        if (dataRowArray.Length > 0)
          flag3 = Convert.ToBoolean(dataRowArray[0]["value"]);
      }
      asset_property assetProperty5 = new asset_property();
      foreach (asset_property assetProperty6 in asset_properties.Values)
      {
        if (assetProperty6.property_name == "book_weekend")
          assetProperty5 = assetProperty6;
      }
      if (assetProperty5.asset_property_id > 0L)
      {
        flag2 = Convert.ToBoolean(assetProperty5.property_value);
      }
      else
      {
        DataRow[] dataRowArray = settings.Tables[0].Select("parameter='book_weekend'");
        if (dataRowArray.Length > 0)
          flag2 = Convert.ToBoolean(dataRowArray[0]["value"]);
      }
      List<DateTime> dateTimeList = new List<DateTime>();
      DateTime dateTime5 = new DateTime(from.Year, from.Month, from.Day);
      dateTimeList.Add(dateTime5);
      dateTime5 = new DateTime(to.Year, to.Month, to.Day);
      if (!dateTimeList.Contains(dateTime5))
        dateTimeList.Add(new DateTime(to.Year, to.Month, to.Day));
      bool flag4 = false;
      bool flag5 = false;
      foreach (DateTime date in dateTimeList)
      {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
          flag5 = true;
        if (this.holidays.is_holiday(date, this.current_user.account_id))
          flag4 = true;
      }
      if (flag5)
        flag1 = flag2;
      if (flag1 && flag4)
        flag1 = flag3;
      if (flag1)
      {
        from = new DateTime(2000, 1, 1, from.Hour, from.Minute, from.Second);
        to = new DateTime(2000, 1, 1, to.Hour, to.Minute, to.Second);
        flag1 = from >= dateTime3 && from <= dateTime4 && to >= dateTime3 && to <= dateTime4;
      }
      return flag1;
    }

    public bool can_book(
      long asset_id,
      DataSet asset_properties,
      DataSet settings,
      DateTime from,
      DateTime to)
    {
      bool flag1 = true;
      DateTime dateTime1 = new DateTime();
      DateTime dateTime2 = new DateTime();
      bool flag2 = false;
      bool flag3 = false;
      DataRow[] dataRowArray1 = asset_properties.Tables[0].Select("asset_id='" + (object) asset_id + "' and property_name='operating_hours'");
      DateTime dateTime3;
      DateTime dateTime4;
      if (dataRowArray1.Length > 0)
      {
        string[] strArray = dataRowArray1[0]["property_value"].ToString().Split('|');
        dateTime3 = Convert.ToDateTime(strArray[0]);
        dateTime4 = Convert.ToDateTime(strArray[1]);
      }
      else
      {
        string[] strArray = settings.Tables[0].Select("parameter='operating_hours'")[0]["value"].ToString().Split('|');
        dateTime3 = Convert.ToDateTime(strArray[0]);
        dateTime4 = Convert.ToDateTime(strArray[1]);
      }
      DataRow[] dataRowArray2 = asset_properties.Tables[0].Select("asset_id='" + (object) asset_id + "' and property_name='book_holiday'");
      if (dataRowArray2.Length > 0)
      {
        flag3 = Convert.ToBoolean(dataRowArray2[0]["property_value"]);
      }
      else
      {
        DataRow[] dataRowArray3 = settings.Tables[0].Select("parameter='book_holiday'");
        if (dataRowArray3.Length > 0)
          flag3 = Convert.ToBoolean(dataRowArray3[0]["value"]);
      }
      DataRow[] dataRowArray4 = asset_properties.Tables[0].Select("asset_id='" + (object) asset_id + "' and property_name='book_weekend'");
      if (dataRowArray4.Length > 0)
      {
        flag2 = Convert.ToBoolean(dataRowArray4[0]["property_value"]);
      }
      else
      {
        DataRow[] dataRowArray5 = settings.Tables[0].Select("parameter='book_weekend'");
        if (dataRowArray5.Length > 0)
          flag2 = Convert.ToBoolean(dataRowArray5[0]["value"]);
      }
      List<DateTime> dateTimeList = new List<DateTime>();
      DateTime dateTime5 = new DateTime(from.Year, from.Month, from.Day);
      dateTimeList.Add(dateTime5);
      dateTime5 = new DateTime(to.Year, to.Month, to.Day);
      if (!dateTimeList.Contains(dateTime5))
        dateTimeList.Add(new DateTime(to.Year, to.Month, to.Day));
      bool flag4 = false;
      bool flag5 = false;
      foreach (DateTime date in dateTimeList)
      {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
          flag5 = true;
        if (this.holidays.is_holiday(date, this.current_user.account_id))
          flag4 = true;
      }
      if (flag5)
        flag1 = flag2;
      if (flag1 && flag4)
        flag1 = flag3;
      if (flag1)
      {
        from = new DateTime(2000, 1, 1, from.Hour, from.Minute, from.Second);
        to = new DateTime(2000, 1, 1, to.Hour, to.Minute, to.Second);
        flag1 = from >= dateTime3 && from <= dateTime4 && to >= dateTime3 && to <= dateTime4;
      }
      return flag1;
    }

    public DataSet get_assets_by_slots(
      DateTime from,
      DateTime start_time,
      DateTime end_time,
      int minutes)
    {
      int num1 = minutes / 15;
      string str = "";
      for (int index = 0; index < num1; ++index)
        str += "0";
      int num2 = this.slot_position(start_time);
      int num3 = this.slot_position(end_time);
      return this.db.get_dataset("SELECT asset_id,substring(book_slot," + (object) num2 + "," + (object) (num3 - num2) + ") as selected,'" + (object) num2 + "' as starting,'" + (object) (num3 - num2) + "' as length FROM sbt_asset_bookings_slot where date='" + from.ToString(api_constants.sql_datetime_format) + "' and substring(book_slot, " + (object) num2 + "," + (object) (num3 - num2) + ") like '%" + str + "%';") ? this.db.resultDataSet : new DataSet();
    }

    public int slot_position(DateTime dt) => (dt.Hour * 60 + dt.Minute) / 15;

    public Dictionary<string, string> get_items(asset_booking obj, asset obj_asset)
    {
      Dictionary<string, string> items = new Dictionary<string, string>();
      try
      {
        items.Add("[image_path]", this.site_full_path + "assets/img/");
        items.Add("[logo]", this.site_full_path + "assets/img/" + this.current_account.logo);
        items.Add("[company_name]", this.current_account.name);
        items.Add("[copyright]", "");
        items.Add("[footer_text]", "");
        items.Add("[building]", obj_asset.building.value);
        items.Add("[level]", obj_asset.level.value);
        items.Add("[room_name]", obj_asset.name);
        items.Add("[room_code]", obj_asset.code);
        items.Add("[room_description]", obj_asset.description);
        items.Add("[room_category]", obj_asset.category.value);
        items.Add("[room_type]", obj_asset.type.value);
        items.Add("[room_capacity]", obj_asset.capacity.ToString());
        items.Add("[similar_meetings]", "");
        items.Add("[email_title]", "Meeting Confirmed");
        items.Add("[purpose]", obj.purpose);
        items.Add("[booking_id]", obj.booking_id.ToString());
        items.Add("[repeat_reference_id]", obj.repeat_reference_id.ToString());
        items.Add("[site_full_path]", this.site_full_path);
        string str1;
        if (obj.book_from.Year == obj.book_to.Year && obj.book_from.Month == obj.book_to.Month && obj.book_from.Day == obj.book_to.Day)
          str1 = "<b>" + obj.book_from.ToString("dd-MMM-yyyy") + "</b><br/>" + obj.book_from.ToString("hh:mm") + " <i>" + obj.book_from.ToString("tt") + "</i> - " + obj.book_to.ToString("hh:mm") + " <i>" + obj.book_to.ToString("tt") + "</i>";
        else
          str1 = "<b>" + obj.book_from.ToString("dd-MMM-yyyy") + "</b> " + obj.book_from.ToString("hh:mm") + " <i>" + obj.book_from.ToString("tt") + "</i><br/><b>" + obj.book_to.ToString("dd-MMM-yyyy") + "</b> " + obj.book_to.ToString("hh:mm") + " <i>" + obj.book_to.ToString("tt") + "</i>";
        items.Add("[date_range]", str1);
        items.Add("[requestor]", this.users.get_user_name(obj.created_by, obj.account_id));
        items.Add("[requestor_email]", this.users.get_user_email(obj.created_by, obj.account_id));
        if (obj.created_by != obj.booked_for)
        {
          items.Add("[booked_for]", this.users.get_user_name(obj.booked_for, obj.account_id));
          items.Add("[booked_for_email]", this.users.get_user_email(obj.booked_for, obj.account_id));
          items.Add("[called_by]", items["[requestor]"] + " on behalf of " + items["[booked_for]"]);
        }
        else
        {
          items.Add("[booked_for]", items["[requestor]"]);
          items.Add("[booked_for_email]", items["[requestor_email]"]);
          items.Add("[called_by]", items["[requestor]"]);
        }
        items.Add("[housekeeping]", obj.housekeeping_required.ToString());
        items.Add("[setup]", obj.setup_required.ToString());
        items.Add("[setup_type]", this.settings.get_setting(obj.setup_type, obj.account_id).value);
        items.Add("[meeting_type]", this.settings.get_setting(obj.meeting_type, obj.account_id).value);
        items.Add("[contact]", obj.contact);
        items.Add("[email]", obj.email);
        items.Add("[remarks]", obj.remarks);
        items.Add("[view_more_link]", this.site_full_path + "bookings.aspx?act=v&id=" + (object) obj.booking_id);
        items.Add("[attending_link]", this.site_full_path + "view_booking.aspx?id=" + (object) obj.booking_id + "&action=going");
        items.Add("[not_attending_link]", this.site_full_path + "view_booking.aspx?id=" + (object) obj.booking_id + "&action=notgoing");
        items.Add("[cancel_link]", this.site_full_path + "bookings.aspx?act=c&id=" + (object) obj.booking_id);
        items.Add("[edit_link]", this.site_full_path + "advanced_booking.aspx?id=" + (object) obj.booking_id);
        items.Add("[setup_link]", this.site_full_path + "administration/report_upcomingsetup_report.aspx");
        items.Add("[housekeeping_link]", this.site_full_path + "administration/report_housekeeping_report.aspx");
        if (obj.status == (short) 0)
        {
          items["[email_title]"] = "Meeting Cancelled";
          items.Add("[cancel_reason]", obj.cancel_reason);
          items.Add("[cancel_by]", this.users.get_user_name(obj.cancel_by, obj.account_id));
          string str2 = "<tr><td class='blk-box'><h2>Cancel Note</h2></td></tr><tr><td><div class='cnt'><table><tr>" + "<td colspan='2'><b>" + items["[cancel_by]"] + "</b> has cancelled the meeting on " + obj.cancel_on.ToString(api_constants.display_datetime_format) + ".</td></tr><tr>" + "<td><img src='" + items["[image_path]"] + "quote.png'/></td><td><div class='quote'><i>" + items["[cancel_reason]"] + "</i></div></td></tr></table></div></td></tr>";
          items.Add("[cancel_note]", str2);
        }
        else
        {
          items.Add("[cancel_reason]", "");
          items.Add("[cancel_by]", "");
          items.Add("[cancel_note]", "");
        }
        if (obj_asset.asset_owner_group_id > 0L)
        {
          setting setting = this.settings.get_setting("default_approval_period", obj.account_id);
          int num = setting.setting_id <= 0L ? 0 : Convert.ToInt32(setting.value);
          DataSet assetPropertyByName = this.assets.get_asset_property_by_name(obj.asset_id, "default_approval_period", obj.account_id);
          if (assetPropertyByName.Tables[0].Rows.Count > 0)
            num = Convert.ToInt32(assetPropertyByName.Tables[0].Rows[0]["property_value"]);
          items.Add("[approval_period]", num.ToString() + " hours");
          DateTime dateTime = this.tzapi.convert_to_user_timestamp(obj.created_on.AddHours((double) num));
          TimeSpan timeSpan = dateTime - this.current_timestamp;
          string str3 = timeSpan.TotalHours <= 24.0 ? (timeSpan.TotalHours <= 1.0 || timeSpan.TotalHours > 24.0 ? Convert.ToInt32(timeSpan.TotalMinutes).ToString() + " mins." : Convert.ToInt32(timeSpan.TotalHours).ToString() + " hrs.") : Convert.ToInt32(timeSpan.TotalDays).ToString() + " days";
          if (dateTime > obj.book_from)
            dateTime = obj.book_from;
          items.Add("[approval_time_left]", str3);
          items.Add("[approval_date]", dateTime.ToString(api_constants.display_datetime_format));
        }
        else
        {
          items.Add("[approval_period]", "");
          items.Add("[approval_time_left]", "");
          items.Add("[approval_date]", "");
        }
        if (obj.transfer_original_booking_id > 0L)
        {
          asset_booking booking = this.bookings.get_booking(obj.transfer_original_booking_id, obj.account_id);
          items.Add("[old_booking_purpose]", booking.purpose);
          items.Add("[old_booking_link]", this.site_full_path + "booking_view.aspx?id=" + (object) booking.booking_id);
        }
        else
        {
          items.Add("[old_booking_purpose]", "");
          items.Add("[old_booking_link]", "");
        }
        string str4 = "";
        if (obj.book_from > this.current_timestamp)
        {
          TimeSpan timeSpan = obj.book_from - this.current_timestamp;
          str4 = timeSpan.TotalHours <= 24.0 ? (timeSpan.TotalHours <= 1.0 || timeSpan.TotalHours > 24.0 ? Convert.ToInt32(timeSpan.TotalMinutes).ToString() + " mins." : Convert.ToInt32(timeSpan.TotalHours).ToString() + " hrs.") : Convert.ToInt32(timeSpan.TotalDays).ToString() + " days";
        }
        items.Add("[time_left]", str4);
        items.Add("[approve_link]", this.site_full_path + "mytask.aspx?id=" + (object) obj.booking_id + "&action=approve");
        items.Add("[reject_link]", this.site_full_path + "mytask.aspx?id=" + (object) obj.booking_id + "&action=reject");
        DataSet workflowData = this.workflows.get_workflow_data(obj.booking_id, obj.account_id);
        if (workflowData.Tables[0].Rows.Count > 0)
        {
          string str5 = "";
          foreach (DataRow row in (InternalDataCollectionBase) workflowData.Tables[0].Rows)
            str5 = str5 + this.users.get_user_name(Convert.ToInt64(row["action_owner_id"].ToString()), obj.account_id) + ", ";
          items.Add("[workflow_people]", str5);
          DataRow[] dataRowArray = workflowData.Tables[0].Select("action_status='2'");
          if (dataRowArray.Length > 0)
          {
            items.Add("[reject_reason]", dataRowArray[0]["action_remarks"].ToString());
            items.Add("[rejected_by]", this.users.get_user_name(Convert.ToInt64(dataRowArray[0]["action_taken_by"].ToString()), obj.account_id));
          }
          else
          {
            items.Add("[reject_reason]", "");
            items.Add("[rejected_by]", "");
          }
        }
        else
        {
          items.Add("[reject_reason]", "");
          items.Add("[rejected_by]", "");
          items.Add("[workflow_people]", "");
        }
        items.Add("[notes]", "");
        items.Add("[reassign_person_name]", this.users.get_user_name(obj.modified_by, obj.account_id));
        string str6 = "";
        if (obj.booking_id > 0L)
        {
          DataSet byAssetBookingId = this.resapi.get_resource_bookings_items_by_asset_booking_id(obj.booking_id, obj.account_id, "", "", this.str_resource_module);
          if (byAssetBookingId.Tables[0].Rows.Count > 0)
          {
            string str7 = str6 + "<tr><td class='blk-box'><h2>Resources</h2></td></tr>" + "<tr><td><div class='cnt'><table class='rpt-tbl'>";
            foreach (DataRow row in (InternalDataCollectionBase) byAssetBookingId.Tables[0].Rows)
            {
              str7 = str7 + "<tr><td style='width:5%;'><img src='" + items["[image_path]"] + "tools.png' /></td>";
              str7 = str7 + "<td><h4>" + row["name"].ToString() + "</h4></td>";
              str7 = str7 + "<td width='8%'><h4>" + row["accepted_qty"].ToString() + "</h4></td>";
              str7 = str7 + "<td style='width:5%;'>" + row["requestor_remarks"].ToString() + "</td>";
              str7 += "</tr>";
            }
            str6 = str7 + "</table></div></td></tr>";
          }
        }
        items.Add("[resource_text]", str6);
        if (items["[resource_text]"] == "")
          items.Add("[has_resources]", "no");
        else
          items.Add("[has_resources]", "yes");
        string str8 = "";
        DataSet invites = this.bookings.get_invites(obj.booking_id, obj.account_id);
        if (invites.Tables[0].Rows.Count > 0)
        {
          string str9 = str6 + "<tr><td class='blk-box'><h2>Invites</h2></td></tr>" + "<tr><td><div class='cnt'><table class='rpt-tbl'>";
          foreach (DataRow row in (InternalDataCollectionBase) invites.Tables[0].Rows)
          {
            str9 = str9 + "<tr><td style='width:5%;'><img src='" + items["[image_path]"] + "man.png' /></td>";
            str9 = str9 + "<td><h4>" + row["name"].ToString() + "</h4></td>";
            str9 = str9 + "<td><a href='maiilto:" + row["email"].ToString() + "'>" + row["email"].ToString() + "</a></td>";
            str9 += "</tr>";
          }
          string str10 = str9 + "</table></div></td></tr>";
        }
        items.Add("[invites]", str8);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("email_requestors_error: " + ("booking-id: " + (object) obj.booking_id) + ". Err: " + ex.ToString()));
      }
      return items;
    }

    public string replace_body(Dictionary<string, string> items, string body)
    {
      foreach (string key in items.Keys)
        body = body.Replace(key, items[key]);
      return body;
    }

    public void send_booking_request_workflow_email(
      asset_booking book,
      bool approved,
      workflow work)
    {
      DataSet templates = this.tapi.get_templates(book.account_id);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      asset asset = this.assets.get_asset(book.asset_id, book.account_id);
      Dictionary<string, string> items = this.get_items(book, asset);
      List<asset_booking> bookings = new List<asset_booking>();
      bookings.Add(book);
      new booking_api().get_appointment_id(book.booking_id, book.account_id);
      Dictionary<long, string> appointments = this.get_appointments(bookings);
      if (approved)
      {
        this.email_requestors_workflow(items, templates, book, asset, approved, work);
        this.email_invitees(items, templates, bookings, asset, appointments);
        this.email_setup(items, templates, bookings, asset, appointments);
        this.email_housekeeping(items, templates, bookings, asset, appointments);
        this.email_catering(items, templates, book, asset, appointments);
      }
      else
        this.email_requestors_workflow(items, templates, book, asset, approved, work);
    }

    public void send_booking_emails(List<asset_booking> bookings)
    {
      try
      {
        asset_booking booking1 = bookings[0];
        DataSet templatesView = this.tapi.get_templates_view(booking1.account_id);
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
        asset asset1 = this.assets.get_asset(booking1.asset_id, booking1.account_id);
        Dictionary<string, string> items = this.get_items(booking1, asset1);
        if (bookings.Count > 1)
        {
          Dictionary<long, asset> dictionary2 = new Dictionary<long, asset>();
          string str1 = "" + "<tr><td class='blk-box'><h2>Similar Meetings</h2></td></tr>" + "<tr><td><div class='cnt'><table class='rpt-tbl'><tr><td colspan='2'><b>Where</b></td><td><b>When</b></td><td><b>View</b></td></tr>";
          foreach (asset_booking booking2 in bookings)
          {
            asset asset2 = new asset();
            asset asset3;
            if (dictionary2.ContainsKey(booking1.asset_id))
            {
              asset3 = dictionary2[booking1.asset_id];
            }
            else
            {
              asset3 = this.assets.get_asset(booking2.asset_id, booking2.account_id);
              dictionary2.Add(asset3.asset_id, asset3);
            }
            str1 = str1 + "<tr><td style='width:5%;'><img src='" + items["[image_path]"] + "cal.png' /></td>";
            str1 = str1 + "<td><h4>" + asset3.building.value + ", Level " + asset3.level.value + ", " + asset3.code + "/" + asset3.name + "</h4></td>";
            str1 = str1 + "<td width='50%'><b>" + booking2.book_from.ToString(api_constants.display_datetime_format_short) + "</b><br/><div style='font-size:10pt;'>" + booking2.book_from.ToString("hh:mm tt") + "  - " + booking2.book_to.ToString("hh:mm tt") + "</div></td>";
            str1 = str1 + "<td style='width:5%;'><a href='" + this.site_full_path + "booking_view.aspx?id=" + (object) booking2.booking_id + "'><img src='" + items["[image_path]"] + "web.png' /></a></td>";
            str1 += "</tr>";
          }
          string str2 = str1 + "</table></div></td></tr>";
          if (items.ContainsKey("[similar_meetings]"))
            items["[similar_meetings]"] = str2;
          else
            items.Add("[similar_meetings]", str2);
        }
        Dictionary<long, string> appointments = this.get_appointments(bookings);
        this.send_emails(bookings, templatesView, items, asset1, appointments);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("send_booking_emails list. Err: " + ex.ToString()));
      }
    }

    public Dictionary<long, string> get_appointments(List<asset_booking> bookings)
    {
      Dictionary<long, user> dictionary = new Dictionary<long, user>();
      Dictionary<long, string> appointments = new Dictionary<long, string>();
      DataSet assetsList = this.assets.get_assets_list(this.current_user.account_id);
      DataSet dataSet = this.users.view_user_list(this.current_user.account_id);
      template template = this.tapi.get_template("ics_file", this.current_user.account_id);
      foreach (asset_booking booking in bookings)
      {
        string contentData = template.content_data;
        try
        {
          user user1 = new user();
          if (!dictionary.ContainsKey(booking.created_by))
          {
            user user2 = this.users.get_user(booking.created_by, booking.account_id);
            dictionary.Add(user2.user_id, user2);
          }
          else
            user1 = dictionary[booking.created_by];
          string newValue = "";
          DataRow[] dataRowArray1 = assetsList.Tables[0].Select("asset_id='" + (object) booking.asset_id + "'");
          if (dataRowArray1.Length > 0)
            newValue = dataRowArray1[0]["building_name"].ToString() + ", Level:" + dataRowArray1[0]["level_name"].ToString() + ", " + dataRowArray1[0]["code"].ToString() + ":" + dataRowArray1[0]["name"].ToString();
          string appSetting = ConfigurationManager.AppSettings["smtp_from_email"];
          if (!Convert.ToBoolean(ConfigurationManager.AppSettings["set_generic_organizer"]))
          {
            DataRow[] dataRowArray2 = dataSet.Tables[0].Select("user_id='" + (object) booking.created_by + "'");
            if (dataRowArray2.Length > 0)
              appSetting = dataRowArray2[0]["email"].ToString();
          }
          if (booking.booking_type == 10 && !Convert.ToBoolean(ConfigurationManager.AppSettings["set_generic_organizer"]))
          {
            DataRow[] dataRowArray3 = dataSet.Tables[0].Select("user_id='" + (object) booking.created_by + "'");
            if (dataRowArray3.Length > 0)
              appSetting = dataRowArray3[0]["email"].ToString();
          }
          string str1 = contentData.Replace("[tzoffset]", this.current_account.properties["tzoffset"]).Replace("[tzid]", this.current_account.properties["tzid"]).Replace("[book_from]", booking.book_from.AddHours(this.current_account.timezone * -1.0).ToString("yyyyMMddTHHmmssZ")).Replace("[book_to]", booking.book_to.AddHours(this.current_account.timezone * -1.0).ToString("yyyyMMddTHHmmssZ")).Replace("[utc]", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")).Replace("[location]", newValue).Replace("[UID]", booking.global_appointment_id).Replace("[description]", booking.remarks).Replace("[remarks]", booking.remarks).Replace("[purpose]", booking.purpose).Replace("[sequence]", booking.sequence.ToString()).Replace("[organizer]", appSetting);
          if (booking.status == (short) 1)
            str1 = str1.Replace("[method]", "REQUEST").Replace("[status]", "CONFIRMED");
          if (booking.status == (short) 4)
            str1 = str1.Replace("[method]", "REQUEST").Replace("[status]", "TENTATIVE");
          if (booking.status == (short) 0)
            str1 = str1.Replace("[method]", "CANCEL").Replace("[status]", "CANCELLED");
          StringBuilder stringBuilder = new StringBuilder();
          try
          {
            foreach (asset_booking_invite assetBookingInvite in booking.invites.Values)
              stringBuilder.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", (object) assetBookingInvite.name, (object) assetBookingInvite.email));
          }
          catch (Exception ex)
          {
            this.log.Equals((object) ex.ToString());
          }
          if (booking.status != (short) 10)
          {
            DataRow[] dataRowArray4 = dataSet.Tables[0].Select("user_id='" + (object) booking.created_by + "'");
            if (dataRowArray4.Length > 0)
              stringBuilder.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", (object) dataRowArray4[0]["full_name"].ToString(), (object) dataRowArray4[0]["email"].ToString()));
          }
          string str2 = str1.Replace("[attendees]", stringBuilder.ToString());
          appointments.Add(booking.booking_id, str2);
        }
        catch (Exception ex)
        {
          this.log.Equals((object) ex.ToString());
        }
      }
      return appointments;
    }

    private void send_emails(
      List<asset_booking> bookings,
      DataSet templates,
      Dictionary<string, string> items,
      asset obj,
      Dictionary<long, string> ics_files)
    {
      try
      {
        asset_booking booking = bookings[0];
        switch (booking.status)
        {
          case 0:
            this.email_requestors(items, templates, bookings, ics_files);
            this.email_invitees(items, templates, bookings, obj, ics_files);
            this.email_setup(items, templates, bookings, obj, ics_files);
            this.email_housekeeping(items, templates, bookings, obj, ics_files);
            this.email_catering(items, templates, booking, obj, ics_files);
            this.email_group_owners(items, templates, bookings, obj, ics_files);
            this.email_resource_owners(items, templates, booking, obj);
            break;
          case 1:
            this.email_requestors(items, templates, bookings, ics_files);
            this.email_invitees(items, templates, bookings, obj, ics_files);
            this.email_setup(items, templates, bookings, obj, ics_files);
            this.email_housekeeping(items, templates, bookings, obj, ics_files);
            this.email_catering(items, templates, booking, obj, ics_files);
            this.email_resource_owners(items, templates, booking, obj);
            break;
          case 3:
            this.email_requestors(items, templates, bookings, ics_files);
            this.email_invitees(items, templates, bookings, obj, ics_files);
            this.email_setup(items, templates, bookings, obj, ics_files);
            this.email_housekeeping(items, templates, bookings, obj, ics_files);
            this.email_catering(items, templates, booking, obj, ics_files);
            this.email_group_owners(items, templates, bookings, obj, ics_files);
            this.email_resource_owners(items, templates, booking, obj);
            break;
          case 4:
            this.email_requestors(items, templates, bookings, ics_files);
            this.email_group_owners(items, templates, bookings, obj, ics_files);
            break;
          case 5:
            this.email_requestors(items, templates, bookings, ics_files);
            break;
          case 6:
            this.email_requestors(items, templates, bookings, ics_files);
            break;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("send_emails. Err: " + ex.ToString()));
      }
    }

    private bool email_requestors_workflow(
      Dictionary<string, string> items,
      DataSet templates,
      asset_booking book,
      asset obj,
      bool approved,
      workflow work)
    {
      try
      {
        DataRow[] dataRowArray;
        if (approved)
        {
          dataRowArray = templates.Tables[0].Select("name='email_booking_request_approved'");
          items["[email_title]"] = "Meeting Room Request Approved";
        }
        else
        {
          dataRowArray = templates.Tables[0].Select("name='email_booking_request_rejected'");
          items["[email_title]"] = "Meeting Room Request Declined";
          string str = "<tr><td class='blk-box'><h2>Note</h2></td></tr><tr><td><div class='cnt'><table><tr>" + "<td colspan='2'><b>" + this.users.get_user_name(work.action_taken_by, work.account_id) + "</b> has rejected the meeting request on " + work.modified_on.ToString(api_constants.display_datetime_format) + ".</td></tr><tr>" + "<td><img src='" + items["[image_path]"] + "quote.png'/></td><td><div class='quote'><i>" + work.action_remarks + "</i></div></td></tr></table></div></td></tr>";
          items.Add("[reject_note]", str);
        }
        if (dataRowArray != null)
        {
          string subject = dataRowArray[0]["title"].ToString();
          string body1 = dataRowArray[0]["content_data"].ToString();
          string body2 = this.replace_body(items, body1);
          this.add_attachment_for_email(this.sendEmail("", body2, subject, "", items["[requestor_email]"], book.record_id), book);
          if (book.booked_for != book.created_by)
            this.add_attachment_for_email(this.sendEmail("", body2, subject, "", items["[booked_for_email]"], book.record_id), book);
          if (book.email != "")
          {
            string email = book.email;
            char[] chArray = new char[1]{ ';' };
            foreach (string to in email.Split(chArray))
            {
              if (to.ToLower() != items["[requestor_email]"].ToLower())
              {
                if (to.ToLower() != items["[booked_for_email]"].ToLower())
                {
                  try
                  {
                    this.add_attachment_for_email(this.sendEmail("", body2, subject, "", to, book.record_id), book);
                  }
                  catch (Exception ex)
                  {
                    this.log.Error((object) ("email_requestors_workflow_inner: " + ("booking-id: " + (object) book.booking_id) + ". Err: " + ex.ToString()));
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("email_requestors_workflow: " + ("booking-id: " + (object) book.booking_id) + ". Err: " + ex.ToString()));
      }
      return true;
    }

    private bool email_requestors(
      Dictionary<string, string> items,
      DataSet templates,
      List<asset_booking> bookings,
      Dictionary<long, string> ics_files)
    {
      try
      {
        asset_booking booking = bookings[0];
        DataRow[] dataRowArray = (DataRow[]) null;
        if (booking.status == (short) 0)
        {
          dataRowArray = templates.Tables[0].Select("name='email_cancel'");
          items["[email_title]"] = "Meeting Cancelled";
        }
        else if (booking.status == (short) 1)
          dataRowArray = templates.Tables[0].Select("name='email_requestor'");
        else if (booking.status == (short) 3)
          dataRowArray = templates.Tables[0].Select("name='email_no_show'");
        else if (booking.status == (short) 4)
        {
          dataRowArray = templates.Tables[0].Select("name='email_pending_approval'");
          items["[email_title]"] = "Request Pending Approval";
        }
        else if (booking.status == (short) 5)
          dataRowArray = templates.Tables[0].Select("name='email_requestor_withdraw'");
        else if (booking.status == (short) 6)
          dataRowArray = templates.Tables[0].Select("name='email_booking_request_rejected'");
        if (dataRowArray != null)
        {
          string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
          string body1 = dataRowArray[0]["content_data"].ToString();
          string body2 = this.replace_body(items, body1);
          email mail = this.sendEmail("", body2, subject, "", items["[requestor_email]"], booking.record_id);
          List<attachment> attachmentList = new List<attachment>();
          this.update_email_attachment(mail, items, bookings, ics_files);
          if (booking.booked_for != booking.created_by)
            this.update_email_attachment(this.sendEmail("", body2, subject, "", items["[booked_for_email]"], booking.record_id), items, bookings, ics_files);
          if (booking.email != "")
          {
            string email = booking.email;
            char[] chArray = new char[1]{ ';' };
            foreach (string to in email.Split(chArray))
            {
              if (to.ToLower() != items["[requestor_email]"].ToLower())
              {
                if (to.ToLower() != items["[booked_for_email]"].ToLower())
                {
                  try
                  {
                    this.update_email_attachment(this.sendEmail("", body2, subject, "", to, booking.record_id), items, bookings, ics_files);
                  }
                  catch (Exception ex)
                  {
                    this.log.Error((object) ("email_requestors_error_inner: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("email_requestors_error. Err: " + ex.ToString()));
      }
      return true;
    }

    private bool email_invitees(
      Dictionary<string, string> items,
      DataSet templates,
      List<asset_booking> bookings,
      asset obj,
      Dictionary<long, string> ics_files)
    {
      if (bookings.Count > 0)
      {
        asset_booking booking = bookings[0];
        if (booking.invites != null && booking.invites.Count > 0)
        {
          List<attachment> attachmentList = new List<attachment>();
          DataRow[] dataRowArray = (DataRow[]) null;
          if (booking.status == (short) 0)
            dataRowArray = templates.Tables[0].Select("name='email_cancel'");
          else if (booking.status == (short) 1)
            dataRowArray = templates.Tables[0].Select("name='email_invitees'");
          else if (booking.status == (short) 3)
            dataRowArray = templates.Tables[0].Select("name='email_no_show'");
          try
          {
            if (dataRowArray != null)
            {
              string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
              string body1 = dataRowArray[0]["content_data"].ToString();
              string body2 = this.replace_body(items, body1);
              foreach (long key in booking.invites.Keys)
              {
                email mail = this.sendEmail("", body2, subject, "", booking.invites[key].email, booking.record_id);
                if (booking.status != (short) 4)
                  this.update_email_attachment(mail, items, bookings, ics_files);
              }
            }
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("email_invitees: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
          }
        }
      }
      return true;
    }

    private bool email_setup(
      Dictionary<string, string> items,
      DataSet templates,
      List<asset_booking> bookings,
      asset obj,
      Dictionary<long, string> ics_files)
    {
      if (bookings.Count > 0)
      {
        asset_booking booking = bookings[0];
        if (booking.setup_required)
        {
          List<attachment> attachmentList = new List<attachment>();
          try
          {
            DataRow[] dataRowArray = (DataRow[]) null;
            if (booking.status == (short) 0)
            {
              dataRowArray = templates.Tables[0].Select("name='email_cancel'");
              items["[email_title]"] = "Setup Request Cancelled";
            }
            else if (booking.status == (short) 1)
              dataRowArray = templates.Tables[0].Select("name='email_facilities'");
            else if (booking.status == (short) 3)
              dataRowArray = templates.Tables[0].Select("name='email_no_show'");
            if (dataRowArray != null)
            {
              setting setting = this.settings.get_setting("facilities_email", booking.account_id);
              if (setting.setting_id > 0L)
              {
                if (setting.value != "")
                {
                  string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
                  string body1 = dataRowArray[0]["content_data"].ToString();
                  string body2 = this.replace_body(items, body1);
                  string str = setting.value;
                  char[] chArray = new char[1]{ ';' };
                  foreach (string to in str.Split(chArray))
                  {
                    try
                    {
                      email mail = this.sendEmail("", body2, subject, "", to, booking.record_id);
                      if (booking.status != (short) 4)
                        this.update_email_attachment(mail, items, bookings, ics_files);
                    }
                    catch (Exception ex)
                    {
                      this.log.Error((object) ("email_setup: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
                    }
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("email_setup_error: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
          }
        }
      }
      return true;
    }

    private bool email_housekeeping(
      Dictionary<string, string> items,
      DataSet templates,
      List<asset_booking> bookings,
      asset obj,
      Dictionary<long, string> ics_files)
    {
      if (bookings.Count > 0)
      {
        asset_booking booking = bookings[0];
        if (booking.housekeeping_required)
        {
          List<attachment> attachmentList = new List<attachment>();
          try
          {
            DataRow[] dataRowArray = (DataRow[]) null;
            if (booking.status == (short) 0)
            {
              dataRowArray = templates.Tables[0].Select("name='email_cancel'");
              items["[email_title]"] = "Housekeeping Request Cancelled";
            }
            else if (booking.status == (short) 1)
              dataRowArray = templates.Tables[0].Select("name='email_housekeeping'");
            else if (booking.status == (short) 3)
              dataRowArray = templates.Tables[0].Select("name='email_no_show'");
            if (dataRowArray != null)
            {
              setting setting = this.settings.get_setting("catering_email", booking.account_id);
              if (setting.setting_id > 0L)
              {
                if (setting.value != "")
                {
                  string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
                  string body1 = dataRowArray[0]["content_data"].ToString();
                  string body2 = this.replace_body(items, body1);
                  string str = setting.value;
                  char[] chArray = new char[1]{ ';' };
                  foreach (string to in str.Split(chArray))
                  {
                    try
                    {
                      email mail = this.sendEmail("", body2, subject, "", to, booking.record_id);
                      if (booking.status != (short) 4)
                        this.update_email_attachment(mail, items, bookings, ics_files);
                    }
                    catch (Exception ex)
                    {
                      this.log.Error((object) ("email_housekeeping: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
                    }
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            this.log.Error((object) ("email_housekeeping_error: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
          }
        }
      }
      return true;
    }

    private bool email_group_owners(
      Dictionary<string, string> items,
      DataSet templates,
      List<asset_booking> bookings,
      asset obj,
      Dictionary<long, string> ics_files)
    {
      if (obj.asset_owner_group_id > 0L)
      {
        bool flag = false;
        foreach (long key in obj.asset_properties.Keys)
        {
          if (obj.asset_properties[key].property_name == "is_email_send" && obj.asset_properties[key].property_value == "1")
            flag = true;
        }
        try
        {
          if (flag)
          {
            List<attachment> attachmentList = new List<attachment>();
            DataRow[] dataRowArray = (DataRow[]) null;
            foreach (asset_booking booking in bookings)
            {
              if (booking.status == (short) 3)
                dataRowArray = templates.Tables[0].Select("name='email_noshow'");
              else if (booking.status == (short) 4)
                dataRowArray = templates.Tables[0].Select("name='email_booking_request'");
              if (dataRowArray != null)
              {
                string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
                string body1 = dataRowArray[0]["content_data"].ToString();
                string body2 = this.replace_body(items, body1);
                foreach (DataRow row in (InternalDataCollectionBase) this.users.get_users_by_group(obj.asset_owner_group_id, obj.account_id).Tables[0].Rows)
                {
                  try
                  {
                    email mail = this.sendEmail("", body2, subject, "", row["email"].ToString(), booking.record_id);
                    if (booking.status == (short) 4)
                      this.update_email_attachment(mail, items, bookings, ics_files);
                  }
                  catch (Exception ex)
                  {
                    this.log.Error((object) ("email_group_owners: " + ("booking-id: " + (object) booking.booking_id) + ". Err: " + ex.ToString()));
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          this.log.Error((object) ("email_group_owners_main: Err: " + ex.ToString()));
        }
      }
      return true;
    }

    private bool email_catering(
      Dictionary<string, string> items,
      DataSet templates,
      asset_booking book,
      asset obj,
      Dictionary<long, string> ics_files)
    {
      return true;
    }

    private bool email_transfer_owners(
      Dictionary<string, string> items,
      DataSet templates,
      asset_booking book,
      Dictionary<long, string> ics_files)
    {
      try
      {
        templates.Tables[0].Select("name='email_transfer_request'");
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("email_housekeeping_error: " + ("booking-id: " + (object) book.booking_id) + ". Err: " + ex.ToString()));
      }
      return true;
    }

    public void email_reassign(List<asset_booking> bookings)
    {
      try
      {
        if (bookings.Count <= 0)
          return;
        asset_booking booking1 = bookings[0];
        user user1 = this.users.get_user(booking1.modified_by, booking1.account_id);
        user user2 = this.users.get_user(booking1.created_by, booking1.account_id);
        List<attachment> attachmentList = new List<attachment>();
        DataSet templates = this.tapi.get_templates(booking1.account_id);
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
        asset asset1 = this.assets.get_asset(booking1.asset_id, booking1.account_id);
        Dictionary<string, string> items = this.get_items(booking1, asset1);
        items["[email_title]"] = "Booking Re-assigned - " + items["[purpose]"];
        DataRow[] dataRowArray = templates.Tables[0].Select("name='email_reassign'");
        Dictionary<long, asset> dictionary2 = new Dictionary<long, asset>();
        string str1 = "" + "<tr><td class='blk-box'><h2>Similar Meetings</h2></td></tr>" + "<tr><td><div class='cnt'><table class='rpt-tbl'><tr><td colspan='2'><b>Where</b></td><td><b>When</b></td><td><b>View</b></td></tr>";
        foreach (asset_booking booking2 in bookings)
        {
          asset asset2 = new asset();
          asset asset3;
          if (dictionary2.ContainsKey(booking1.asset_id))
          {
            asset3 = dictionary2[booking1.asset_id];
          }
          else
          {
            asset3 = this.assets.get_asset(booking2.asset_id, booking2.account_id);
            dictionary2.Add(asset3.asset_id, asset3);
          }
          str1 = str1 + "<tr><td style='width:5%;'><img src='" + items["[image_path]"] + "cal.png' /></td>";
          str1 = str1 + "<td><h4>" + asset3.building.value + ", Level " + asset3.level.value + ", " + asset3.code + "/" + asset3.name + "</h4></td>";
          str1 = str1 + "<td width='50%'><b>" + booking2.book_from.ToString(api_constants.display_datetime_format_short) + "</b><br/><div style='font-size:10pt;'>" + booking2.book_from.ToString("hh:mm tt") + "  - " + booking2.book_to.ToString("hh:mm tt") + "</div></td>";
          str1 = str1 + "<td style='width:5%;'><a href='" + this.site_full_path + "booking_view.aspx?id=" + (object) booking2.booking_id + "'><img src='" + items["[image_path]"] + "web.png' /></a></td>";
          str1 += "</tr>";
        }
        string str2 = str1 + "</table></div></td></tr>";
        if (items.ContainsKey("[similar_meetings]"))
          items["[similar_meetings]"] = str2;
        else
          items.Add("[similar_meetings]", str2);
        items.Add("[message]", user1.full_name + " has reassigned these bookings to you. Kindly add them to your calendar.");
        string subject1 = dataRowArray[0]["title"].ToString();
        string body1 = dataRowArray[0]["content_data"].ToString();
        email mail = this.sendEmail("", this.replace_body(items, body1), subject1, "", user2.email, booking1.record_id);
        Dictionary<long, string> appointments1 = this.get_appointments(bookings);
        this.update_email_attachment(mail, items, bookings, appointments1);
        List<asset_booking> bookings1 = new List<asset_booking>();
        foreach (asset_booking booking3 in bookings)
        {
          booking3.status = (short) 0;
          booking3.invites = new Dictionary<long, asset_booking_invite>();
          bookings1.Add(booking3);
        }
        Dictionary<long, string> dictionary3 = new Dictionary<long, string>();
        Dictionary<long, string> appointments2 = this.get_appointments(bookings1);
        items["[message]"] = "You have reassigned these bookings to " + user2.full_name + ". Please remove these events from your calendar if you are not involved in the booking.";
        items["[requestor_email]"] = this.current_user.email;
        string subject2 = dataRowArray[0]["title"].ToString();
        string body2 = dataRowArray[0]["content_data"].ToString();
        this.update_email_attachment(this.sendEmail("", this.replace_body(items, body2), subject2, "", user1.email, booking1.record_id), items, bookings, appointments2);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("email_reassign: Err: " + ex.ToString()));
      }
    }

    public List<attachment> update_email_attachment(
      email mail,
      Dictionary<string, string> items,
      List<asset_booking> bookings,
      Dictionary<long, string> ics_files)
    {
      List<attachment> attachmentList = new List<attachment>();
      int num = 1;
      foreach (asset_booking booking in bookings)
      {
        byte[] bytes = Encoding.ASCII.GetBytes(ics_files[booking.booking_id]);
        attachment attachment1 = new attachment();
        attachment1.account_id = booking.account_id;
        attachment1.created_on = booking.created_on;
        attachment1.created_by = booking.created_by;
        attachment1.modified_on = this.current_timestamp;
        attachment1.modified_by = booking.modified_by;
        attachment1.message_id = mail.message_id;
        attachment1.attachment_id = 0L;
        attachment1.content_data = bytes;
        attachment1.record_id = mail.record_id;
        attachment1.mime_type = "text/calendar";
        attachment1.file_extention = "ics";
        if (bookings.Count > 1)
          attachment1.file_name = booking.purpose + "-" + booking.book_from.ToString("dd-MMM-yyyy HH:mm") + "(" + (object) num + " of " + (object) bookings.Count + ").ics";
        else
          attachment1.file_name = booking.purpose + ".ics";
        string xml = "<properties></properties>";
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        attachment1.properties = xmlDocument;
        attachment attachment2 = this.eapi.update_email_attachment(attachment1);
        attachmentList.Add(attachment2);
        ++num;
      }
      return attachmentList;
    }

    public icalendar get_attachment(
      Dictionary<string, string> items,
      List<asset_booking> bookings,
      string email_body)
    {
      icalendar attachment = new icalendar();
      attachment.events = new List<ievent>();
      attachment.version = "2.0";
      attachment.prodid = "//" + Resources.fbs.prodid + "/" + this.site_full_path + " v1.0//EN";
      attachment.method = Resources.fbs.calmethod;
      attachment.calscale = Resources.fbs.calscale;
      foreach (asset_booking booking in bookings)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Purpose: " + booking.purpose);
        stringBuilder.AppendLine("From: " + (object) booking.book_from);
        stringBuilder.AppendLine("To: " + (object) booking.book_to);
        stringBuilder.AppendLine("Location: " + items["[building]"] + ", " + items["[level]"] + " " + items["[room_code]"] + "/" + items["[room_name]"]);
        stringBuilder.AppendLine("Requested By: " + items["[requestor]"]);
        stringBuilder.AppendLine("Requestor Email: " + items["[requestor_email]"]);
        stringBuilder.AppendLine("Contact: " + items["[contact]"]);
        stringBuilder.AppendLine("Remarks: " + items["[remarks]"]);
        ievent ievent = new ievent()
        {
          alarms = new List<ialarm>(),
          categories = items["[meeting_type]"],
          contact = items["[contact]"],
          description = "",
          dtend = booking.book_to.ToString("yyyyMMddTHHmmss"),
          dtstamp = booking.created_on.ToString("yyyyMMddTHHmmss"),
          dtstart = booking.book_from.ToString("yyyyMMddTHHmmss"),
          location = items["[building]"] + ", " + items["[level]"] + " " + items["[room_code]"] + "/" + items["[room_name]"],
          organizer = "CN=" + items["[requestor]"] + ":mailto:" + items["[requestor_email]"],
          x_alt_description = email_body,
          sequence = "0",
          status = booking.status != (short) 1 ? "CANCELLED" : "CONFIRMED",
          summary = items["[purpose]"],
          transp = Resources.fbs.transp,
          uid = booking.created_on.ToString("yyyyMMddTHHmmss") + "-" + Guid.NewGuid().ToString() + "@" + this.site_full_path.Replace("http://", "").Replace("https://", "").Replace("/", ""),
          url = items["[view_more_link]"]
        };
        ievent.alarms.Add(new ialarm()
        {
          action = Resources.fbs.alarmaction,
          description = booking.purpose,
          trigger = Resources.fbs.alarmat
        });
        attachment.events.Add(ievent);
      }
      return attachment;
    }

    public Dictionary<string, string> get_resource_items(resource_booking obj)
    {
      Dictionary<string, string> resourceItems = new Dictionary<string, string>();
      try
      {
        resourceItems.Add("[image_path]", this.site_full_path + "assets/img/");
        resourceItems.Add("[logo]", this.site_full_path + "assets/img/" + this.current_account.logo);
        resourceItems.Add("[company_name]", this.current_account.name);
        resourceItems.Add("[copyright]", "");
        resourceItems.Add("[footer_text]", "");
        resourceItems.Add("[building]", "");
        resourceItems.Add("[level]", "");
        resourceItems.Add("[room_name]", obj.venue);
        resourceItems.Add("[room_code]", "");
        resourceItems.Add("[room_description]", "");
        resourceItems.Add("[room_category]", "");
        resourceItems.Add("[room_type]", "");
        resourceItems.Add("[room_capacity]", "");
        resourceItems.Add("[remarks]", obj.remarks);
        resourceItems.Add("[admin_more_link]", this.site_full_path + "administration/additional_resources/requests.aspx");
        resourceItems.Add("[view_more_link]", this.site_full_path + "additional_resources/resource_bookings_list.aspx?id=" + (object) obj.resource_booking_id);
        resourceItems.Add("[email_title]", "Resource Booking");
        resourceItems.Add("[purpose]", obj.purpose);
        string str1;
        if (obj.book_from.Year == obj.book_to.Year && obj.book_from.Month == obj.book_to.Month && obj.book_from.Day == obj.book_to.Day)
          str1 = "<b>" + obj.book_from.ToString("dd-MMM-yyyy") + "</b><br/>" + obj.book_from.ToString("hh:mm") + "<i>" + obj.book_from.ToString("tt") + "</i> - " + obj.book_to.ToString("hh:mm") + " <i>" + obj.book_to.ToString("tt") + "</i>";
        else
          str1 = "<b>" + obj.book_from.ToString("dd-MMM-yyyy") + "</b> " + obj.book_from.ToString("hh:mm") + "<i>" + obj.book_from.ToString("tt") + "</i><br/><b>" + obj.book_to.ToString("dd-MMM-yyyy") + "</b> " + obj.book_to.ToString("hh:mm") + " <i>" + obj.book_to.ToString("tt") + "</i>";
        resourceItems.Add("[date_range]", str1);
        resourceItems.Add("[requestor]", this.users.get_user_name(obj.created_by, obj.account_id));
        resourceItems.Add("[requestor_email]", this.users.get_user_email(obj.created_by, obj.account_id));
        resourceItems.Add("[email]", resourceItems["[requestor_email]"]);
        if (obj.created_by != obj.booked_for_id)
        {
          resourceItems.Add("[booked_for]", this.users.get_user_name(obj.booked_for_id, obj.account_id));
          resourceItems.Add("[booked_for_email]", this.users.get_user_email(obj.booked_for_id, obj.account_id));
          resourceItems.Add("[called_by]", resourceItems["[requestor]"] + " on behalf of " + resourceItems["[booked_for]"]);
        }
        else
        {
          resourceItems.Add("[booked_for]", resourceItems["[requestor]"]);
          resourceItems.Add("[booked_for_email]", resourceItems["[requestor_email]"]);
          resourceItems.Add("[called_by]", resourceItems["[requestor]"]);
        }
        string str2 = "";
        DataSet resourceBookingId = this.resapi.get_resource_bookings_by_resource_booking_id(obj.account_id, obj.resource_booking_id, this.str_resource_module);
        if (resourceBookingId.Tables[0].Rows.Count > 0)
        {
          string str3 = str2 + "<tr><td class='blk-box'><h2>Resources</h2></td></tr>" + "<tr><td><div class='cnt'><table class='rpt-tbl'>";
          foreach (DataRow row in (InternalDataCollectionBase) resourceBookingId.Tables[0].Rows)
          {
            str3 = str3 + "<tr><td style='width:5%;'><img src='" + resourceItems["[image_path]"] + "tools.png' /></td>";
            str3 = str3 + "<td><h4>" + row["name"].ToString() + "</h4></td>";
            str3 = str3 + "<td width='8%'><h4>" + row["accepted_qty"].ToString() + "</h4></td>";
            str3 = str3 + "<td>" + row["requestor_remarks"].ToString() + "</td>";
            str3 += "</tr>";
          }
          str2 = str3 + "</table></div></td></tr>";
        }
        resourceItems.Add("[resource_text]", str2);
        if (resourceItems["[resource_text]"] == "")
          resourceItems.Add("[has_resources]", "yes");
        else
          resourceItems.Add("[has_resources]", "no");
      }
      catch
      {
      }
      return resourceItems;
    }

    public bool email_resource_owners(
      asset_booking book,
      asset obj,
      resource_booking resource_book)
    {
      try
      {
        DataRow[] dataRowArray = (DataRow[]) null;
        DataSet dataSet = new DataSet();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        Dictionary<string, string> items;
        if (book != null)
        {
          DataSet templates = this.tapi.get_templates(book.account_id);
          items = this.get_items(book, obj);
          if (book.status == (short) 0)
            dataRowArray = templates.Tables[0].Select("name='email_cancel'");
          else if (book.status == (short) 1)
            dataRowArray = templates.Tables[0].Select("name='email_resource_owner_request'");
          else if (book.status == (short) 3)
            dataRowArray = templates.Tables[0].Select("name='email_no_show'");
        }
        else
        {
          DataSet templates = this.tapi.get_templates(resource_book.account_id);
          items = this.get_resource_items(resource_book);
          if (resource_book.status == 0)
            dataRowArray = templates.Tables[0].Select("name='email_cancel'");
          else if (resource_book.status == 1)
            dataRowArray = templates.Tables[0].Select("name='email_resource_owner_request'");
          else if (resource_book.status == 3)
            dataRowArray = templates.Tables[0].Select("name='email_no_show'");
        }
        try
        {
          if (dataRowArray != null)
          {
            if (book != null && book.booking_id == 0L)
              return false;
            string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
            string body1 = dataRowArray[0]["content_data"].ToString();
            string body2 = this.replace_body(items, body1);
            DataSet resourceBookingId = this.resapi.get_resource_bookings_by_resource_booking_id(resource_book.account_id, resource_book.resource_booking_id, this.str_resource_module);
            if (resource_book.status == 1)
            {
              string cc = "";
              string to = items["[requestor_email]"];
              if (items["[requestor_email]"] != items["[booked_for_email]"])
                cc = items["[booked_for_email]"];
              this.sendEmail("", body2, subject, cc, to, resource_book.record_id);
            }
            items["[view_more_link]"] = items["[admin_more_link]"];
            string body3 = dataRowArray[0]["content_data"].ToString();
            string body4 = this.replace_body(items, body3);
            List<string> source = new List<string>();
            foreach (DataRow row1 in (InternalDataCollectionBase) resourceBookingId.Tables[0].Rows)
            {
              foreach (DataRow row2 in (InternalDataCollectionBase) this.resapi.get_users_by_item(Convert.ToInt64(row1["resource_id"]), resource_book.account_id).Tables[0].Rows)
              {
                if (!((IEnumerable<object>) source).Contains<object>(row2["email"]))
                {
                  this.add_attachment_for_email(this.sendEmail("", body4, subject, "", row2["email"].ToString(), resource_book.record_id), resource_book.book_from, resource_book.book_to);
                  source.Add(row2["email"].ToString());
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          this.log.Error((object) ("email_resource_owners: Err: " + ex.ToString()));
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("email_resource_owners: Err: " + ex.ToString()));
      }
      return true;
    }

    private bool email_resource_owners(
      Dictionary<string, string> items,
      DataSet templates,
      asset_booking book,
      asset obj)
    {
      if (items["[has_resources]"] == "yes")
      {
        DataRow[] dataRowArray = (DataRow[]) null;
        if (book.status == (short) 0)
          dataRowArray = templates.Tables[0].Select("name='email_cancel'");
        else if (book.status == (short) 1)
          dataRowArray = templates.Tables[0].Select("name='email_resource_owner_request'");
        else if (book.status == (short) 3)
          dataRowArray = templates.Tables[0].Select("name='email_no_show'");
        try
        {
          if (dataRowArray != null)
          {
            string subject = dataRowArray[0]["title"].ToString() + " - " + items["[purpose]"];
            string body1 = dataRowArray[0]["content_data"].ToString();
            string body2 = this.replace_body(items, body1);
            DataSet byAssetBookingId = this.resapi.get_resource_bookings_items_by_asset_booking_id(book.booking_id, obj.account_id, "", "", this.str_resource_module);
            List<string> source = new List<string>();
            foreach (DataRow row1 in (InternalDataCollectionBase) byAssetBookingId.Tables[0].Rows)
            {
              foreach (DataRow row2 in (InternalDataCollectionBase) this.resapi.get_users_by_item(Convert.ToInt64(row1["resource_id"]), book.account_id).Tables[0].Rows)
              {
                if (!((IEnumerable<object>) source).Contains<object>(row2["email"]))
                {
                  this.add_attachment_for_email(this.sendEmail("", body2, subject, "", row2["email"].ToString(), book.record_id), book);
                  source.Add(row2["email"].ToString());
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          this.log.Error((object) ("email_resource_owners: " + ("booking-id: " + (object) book.booking_id) + ". Err: " + ex.ToString()));
        }
      }
      return true;
    }

    public void update_outlook(List<asset_booking> bookings)
    {
      outlook_api outlookApi = new outlook_api();
      foreach (asset_booking booking in bookings)
      {
        outlook_booking outlookBooking = new outlook_booking();
        if (!outlookApi.check_global_appointment_id_exists(booking.global_appointment_id, booking.account_id, booking.booking_id))
        {
          outlookBooking.account_id = booking.account_id;
          outlookBooking.booking_id = booking.booking_id;
          outlookBooking.created_by = booking.created_by;
          outlookBooking.global_appointment_id = booking.global_appointment_id;
          outlookBooking.last_modified_on = booking.created_on;
          outlookBooking.outlook_id = 0L;
          outlookApi.outlook_booking_update(outlookBooking);
        }
      }
    }
  }
}
