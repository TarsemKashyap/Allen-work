// Decompiled with JetBrains decompiler
// Type: admin_serverisde_asset_list
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;

public class admin_serverisde_asset_list : fbs_base_page, IRequiresSessionState
{
  private StringBuilder html = new StringBuilder();
  private StringBuilder outerhtml = new StringBuilder();

  protected new void Page_Load(object sender, EventArgs e)
  {
    this.outerhtml.Append("{");
    try
    {
      this.outerhtml.Append("\"sEcho\":" + (object) int.Parse(this.Request.Params["sEcho"]));
      int num1 = int.Parse(this.Request.Params["iDisplayLength"]);
      if (num1 == -1)
        num1 = 1000000;
      int num2 = int.Parse(this.Request.Params["iDisplayStart"]);
      string filter = this.Request.Params["sSearch"];
      if (filter == "")
        filter = "%";
      string str1 = this.Request.Params["iSortCol_0"];
      string order = this.Request.Params["sSortDir_0"];
      string colname = "";
      switch (str1)
      {
        case "0":
          colname = "codename";
          break;
        case "1":
          colname = "Building";
          break;
        case "2":
          colname = "level";
          break;
        case "3":
          colname = "capacity";
          break;
        case "4":
          colname = "category";
          break;
        case "5":
          colname = "Type";
          break;
        case "6":
          colname = "Restricted";
          break;
        case "7":
          colname = "comment";
          break;
        case "8":
          colname = "Status";
          break;
      }
      DataSet faultyAssetId = this.bookings.get_faulty_asset_id(this.current_user.account_id);
      DataSet dataSet = this.users.view_groups(this.current_user.account_id);
      string groupIds1 = this.utilities.get_group_ids(this.current_user);
      string group_ids1 = string.IsNullOrEmpty(groupIds1) ? "0" : groupIds1;
      string filterExpression = "";
      if (this.gp.isAdminType)
      {
        string group_ids2 = "0";
        DataSet assets = this.assets.get_assets(this.current_user.account_id, filter, colname, order, (num2 + 1).ToString(), (num2 + num1).ToString(), group_ids2);
        if (this.utilities.isValidDataset(assets))
        {
          this.outerhtml.Append(",");
          this.outerhtml.Append("\"iTotalRecords\":" + assets.Tables[1].Rows[0][0] + ",");
          this.outerhtml.Append("\"iTotalDisplayRecords\":" + assets.Tables[1].Rows[0][0] + ",");
          this.outerhtml.Append("\"aaData\":[");
          try
          {
            foreach (DataRow row in (InternalDataCollectionBase) assets.Tables[0].Rows)
            {
              string str2 = "";
              this.html.Append("[");
              if (row["asset_owner_group_id"].ToString() == "" || row["asset_owner_group_id"].ToString() == "0")
              {
                this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'><a href='#' class='btn green icn-only'><i class='icon-unlock icon-white'></i></a></i>\",");
              }
              else
              {
                DataRow[] dataRowArray = dataSet.Tables[0].Select("group_id='" + row["asset_owner_group_id"].ToString() + "'");
                if (dataRowArray.Length > 0)
                  str2 = "<a href='javascript:show_users(" + row["asset_owner_group_id"].ToString() + ");'>" + dataRowArray[0]["group_name"].ToString() + "</a>";
                else
                  str2 = "";
                this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'><a href='#' class='btn red icn-only'><i class='icon-lock icon-white'></i></a></i>\",");
              }
              if (faultyAssetId.Tables[0].Rows.Count > 0)
              {
                if (faultyAssetId.Tables[0].Select("asset_id=" + row["asset_id"].ToString()).Length > 0)
                {
                  if (row["codename"].ToString().Trim().StartsWith("/"))
                    this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'>" + row["codename"].ToString().Replace("/", "") + "  <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' />\",");
                  else
                    this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'>" + row["codename"].ToString() + "  <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' />\",");
                }
                else if (row["codename"].ToString().Trim().StartsWith("/"))
                  this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'>" + row["codename"].ToString().Replace("/", "") + "\",");
                else
                  this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'>" + row["codename"].ToString() + "\",");
              }
              else if (row["codename"].ToString().Trim().StartsWith("/"))
                this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'>" + row["codename"].ToString().Replace("/", "") + "\",");
              else
                this.html.Append("\"<div class='actions' id='action_" + row["asset_id"].ToString() + "'>" + row["codename"].ToString() + "\",");
              this.html.Append("\"" + row["Building"].ToString() + "\",");
              this.html.Append("\"" + row["Level"].ToString() + "\",");
              if (row["capacity"].ToString() == "-1")
                this.html.Append("\" NA \",");
              else
                this.html.Append("\"" + row["capacity"].ToString() + "\",");
              this.html.Append("\"" + row["Category"].ToString() + "\",");
              this.html.Append("\"" + row["Type"].ToString() + "\",");
              this.html.Append("\"" + str2 + "\",");
              this.html.Append("\"" + row["comment"].ToString().TrimEnd(',') + "\",");
              if (row["Status"].ToString() == "Available")
                this.html.Append("\"<span class='label label-Available'>" + row["Status"].ToString() + "</span>\",");
              else
                this.html.Append("\"<span class='label label-NotAvailable'>" + row["Status"].ToString() + "</span>\",");
              this.html.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              this.html.Append("<ul class='ddm p-r'>");
              this.html.Append("<li><a href='javascript:callfancybox(" + row["asset_id"].ToString() + ")'><i class='icon-table'> View</i></a></li>");
              if (this.gp.facility_edit)
              {
                this.html.Append("<li><a href='javascript:callLoading(" + row["asset_id"].ToString() + ");'><i class='icon-pencil'></i> Edit</a></li>");
                this.html.Append("<li><a href='asset_bookings.aspx?asset_id=" + row["asset_id"].ToString() + "'><i class='icon-table'></i> Bookings</a></li>");
                if (Convert.ToBoolean(this.current_account.properties["audit_log"]))
                  this.html.Append("<li><a href='asset_audit_logs.aspx?asset_id=" + row["asset_id"].ToString() + "'><i class='icon-table'></i> Audit Logs</a></li>");
                if (Convert.ToBoolean(this.current_account.properties["email_log"]))
                  this.html.Append("<li><a href='asset_email_logs.aspx?asset_id=" + row["asset_id"].ToString() + "'><i class='icon-table'></i> Email Logs</a></li>");
              }
              if (this.gp.facility_delete)
                this.html.AppendFormat("<li><a onclick='javascript:delete_facilitylist({0})'><i class='icon-trash'></i> Delete</a></li>", (object) row["asset_id"].ToString());
              this.html.Append("</ul>");
              this.html.Append("</div></div>\"");
              this.html.Append("],");
            }
            string str3 = this.html.ToString();
            if (str3.Trim() != "")
              str3 = str3.TrimEnd(',');
            this.outerhtml.Append(str3);
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) ("serverside asset list admin panel error : " + ex.ToString()));
          }
          this.outerhtml.Append("]");
        }
        else
        {
          this.outerhtml.Append(",");
          this.outerhtml.Append("\"iTotalRecords\":\"0\",");
          this.outerhtml.Append("\"iTotalDisplayRecords\":\"0\",");
          this.outerhtml.Append("\"aaData\":[");
          this.outerhtml.Append("]");
        }
      }
      else if (this.gp.isSuperUserType)
      {
        string groupIds2 = this.utilities.get_group_ids(this.current_user);
        DataSet assets = this.assets.get_assets(this.current_user.account_id, filter, colname, order, (num2 + 1).ToString(), "1000000", groupIds2, group_ids1);
        DataSet usergroupId = this.users.get_usergroup_id(this.current_user.user_id, this.current_user.account_id);
        if (this.utilities.isValidDataset(usergroupId))
        {
          foreach (DataRow row in (InternalDataCollectionBase) usergroupId.Tables[0].Rows)
            filterExpression = filterExpression + " asset_owner_group_id = " + row["group_id"].ToString() + " or ";
        }
        if (filterExpression != "")
          filterExpression = filterExpression.Substring(0, filterExpression.Length - 3);
        if (this.utilities.isValidDataset(assets))
        {
          DataRow[] source = assets.Tables[0].Select();
          if (filterExpression.Trim() != "")
            source = assets.Tables[0].Select(filterExpression);
          if (source.Length > 0)
            source = ((IEnumerable<DataRow>) source).AsEnumerable<DataRow>().Take<DataRow>(num2 + num1).CopyToDataTable<DataRow>().Select("", colname + " " + order);
          DataRow[] dataRowArray1 = source;
          this.outerhtml.Append(",");
          this.outerhtml.Append("\"iTotalRecords\":" + (object) dataRowArray1.Length + ",");
          this.outerhtml.Append("\"iTotalDisplayRecords\":" + (object) dataRowArray1.Length + ",");
          this.outerhtml.Append("\"aaData\":[");
          try
          {
            foreach (DataRow dataRow in dataRowArray1)
            {
              string str4 = "";
              this.html.Append("[");
              if (dataRow["asset_owner_group_id"].ToString() == "" || dataRow["asset_owner_group_id"].ToString() == "0")
              {
                this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'><a href='#' class='btn green icn-only'><i class='icon-unlock icon-white'></i></a></i>\",");
              }
              else
              {
                DataRow[] dataRowArray2 = dataSet.Tables[0].Select("group_id='" + dataRow["asset_owner_group_id"].ToString() + "'");
                if (dataRowArray2.Length > 0)
                  str4 = "<a href='javascript:show_users(" + dataRow["asset_owner_group_id"].ToString() + ");'>" + dataRowArray2[0]["group_name"].ToString() + "</a>";
                else
                  str4 = "";
                this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'><a href='#' class='btn red icn-only'><i class='icon-lock icon-white'></i></a></i>\",");
              }
              if (faultyAssetId.Tables[0].Rows.Count > 0)
              {
                if (faultyAssetId.Tables[0].Select("asset_id=" + dataRow["asset_id"].ToString()).Length > 0)
                {
                  if (dataRow["codename"].ToString().Trim().StartsWith("/"))
                    this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'>" + dataRow["codename"].ToString().Replace("/", "") + "  <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' />\",");
                  else
                    this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'>" + dataRow["codename"].ToString() + "  <img id='img_prop' style='float:right;' src='../assets/img/Facilityerro.png' alt='Faulty Room' />\",");
                }
                else if (dataRow["codename"].ToString().Trim().StartsWith("/"))
                  this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'>" + dataRow["codename"].ToString().Replace("/", "") + "\",");
                else
                  this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'>" + dataRow["codename"].ToString() + "\",");
              }
              else if (dataRow["codename"].ToString().Trim().StartsWith("/"))
                this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'>" + dataRow["codename"].ToString().Replace("/", "") + "\",");
              else
                this.html.Append("\"<div class='actions' id='action_" + dataRow["asset_id"].ToString() + "'>" + dataRow["codename"].ToString() + "\",");
              this.html.Append("\"" + dataRow["Building"].ToString() + "\",");
              this.html.Append("\"" + dataRow["Level"].ToString() + "\",");
              if (dataRow["capacity"].ToString() == "-1")
                this.html.Append("\" NA \",");
              else
                this.html.Append("\"" + dataRow["capacity"].ToString() + "\",");
              this.html.Append("\"" + dataRow["Category"].ToString() + "\",");
              this.html.Append("\"" + dataRow["Type"].ToString() + "\",");
              this.html.Append("\"" + str4 + "\",");
              string str5 = dataRow["comment"].ToString();
              if (str5.Length > 0)
                str5 = str5.Substring(0, str5.Length - 2);
              this.html.Append("\"" + str5 + "\",");
              if (dataRow["Status"].ToString() == "Available")
                this.html.Append("\"<span class='label label-Available'>" + dataRow["Status"].ToString() + "</span>\",");
              else
                this.html.Append("\"<span class='label label-NotAvailable'>" + dataRow["Status"].ToString() + "</span>\",");
              this.html.Append("\"<div class='actions'><div class='bgp'><a class='btn default' href='#' data-toggle='dropdown'><i class='icon-angle-down'></i></a>");
              this.html.Append("<ul class='ddm p-r'>");
              this.html.Append("<li><a href='javascript:callfancybox(" + dataRow["asset_id"].ToString() + ")'><i class='icon-table'> View</i></a></li>");
              if (this.gp.facility_edit && ("," + groupIds2 + ",").Contains("," + dataRow["asset_owner_group_id"].ToString() + ","))
              {
                this.html.Append("<li><a href='asset_form.aspx?asset_id=" + dataRow["asset_id"].ToString() + "'><i class='icon-pencil'></i> Edit</a></li>");
                this.html.Append("<li><a href='asset_bookings.aspx?asset_id=" + dataRow["asset_id"].ToString() + "'><i class='icon-table'></i> Bookings</a></li>");
                this.html.Append("<li><a href='asset_audit_logs.aspx?asset_id=" + dataRow["asset_id"].ToString() + "'><i class='icon-table'></i> Audit Logs</a></li>");
                this.html.Append("<li><a href='asset_email_logs.aspx?asset_id=" + dataRow["asset_id"].ToString() + "'><i class='icon-table'></i> Email Logs</a></li>");
              }
              this.html.Append("</ul>");
              this.html.Append("</div></div>\"");
              this.html.Append("],");
            }
            string str6 = this.html.ToString();
            if (str6.Trim() != "")
              str6 = str6.TrimEnd(',');
            this.outerhtml.Append(str6);
          }
          catch (Exception ex)
          {
            fbs_base_page.log.Error((object) ("serverside asset list admin panel error : " + ex.ToString()));
          }
          this.outerhtml.Append("]");
        }
        else
        {
          this.outerhtml.Append(",");
          this.outerhtml.Append("\"iTotalRecords\":\"0\",");
          this.outerhtml.Append("\"iTotalDisplayRecords\":\"0\",");
          this.outerhtml.Append("\"aaData\":[");
          this.outerhtml.Append("]");
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) ("Error->" + (object) ex));
    }
    this.outerhtml.Append("}");
    this.Response.Clear();
    this.Response.ClearHeaders();
    this.Response.ClearContent();
    this.Response.Write(this.outerhtml.ToString());
    this.Response.Flush();
    this.Response.End();
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
