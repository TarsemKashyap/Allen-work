// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.reporting_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class reporting_api : api_base
  {
    public DataSet cancellation_report(
      string fromdate,
      string todate,
      Guid account_id,
      string groupids)
    {
      try
      {
        return this.db.get_dataset("SELECT FF.Division, FF.Department, c.name, c.asset_id , s.value AS Building, (SELECT COUNT(*) FROM dbo.sbt_asset_bookings WHERE account_id='" + (object) account_id + "' and asset_id=FF.asset_id AND status !=" + (object) 0 + "  AND (book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR book_to BETWEEN '" + fromdate + "' AND '" + todate + "')) AS NoOfBooking, FF.SumOfCancel FROM (SELECT CASE WHEN GROUPING(Division)= 1 THEN 'Grand Total' WHEN GROUPING(asset_id)= 1 THEN Department + ' Total' WHEN GROUPING(Division)!= 1 THEN Division END AS Division, Department, asset_id, SUM(NoOfCancellation) AS SumOfCancel FROM (SELECT R.Division, R.Department, a.asset_id, COUNT(*) AS NoOfCancellation FROM dbo.sbt_asset_bookings AS a INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id INNER JOIN (select user_id, max(case when property_name = 'staff_division' then property_value end) Division,max(case when property_name = 'staff_department' then property_value end) Department from sbt_user_properties where account_id='" + (object) account_id + "' group by user_id) AS R ON a.booked_for=R.user_id WHERE " + groupids + " a.account_id='" + (object) account_id + "' AND a.status=" + (object) 0 + " AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') GROUP BY a.asset_id,r.division,r.department) AS F GROUP BY Division, Department, asset_id WITH ROLLUP) AS FF LEFT JOIN dbo.sbt_assets AS c ON FF.asset_id=c.asset_id LEFT JOIN sbt_settings AS s ON c.building_id=s.setting_id AND s.parameter='building';" + "SELECT b.name, COUNT(*) AS NoOfCancellation FROM dbo.sbt_asset_bookings AS a INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id INNER JOIN (select user_id, max(case when property_name = 'staff_division' then property_value end) Division,max(case when property_name = 'staff_department' then property_value end) Department from sbt_user_properties where account_id='" + (object) account_id + "' group by user_id) AS R ON a.booked_for=R.user_id WHERE " + groupids + "  a.account_id='" + (object) account_id + "' AND a.status=" + (object) 0 + " AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') GROUP BY b.name;") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet requestor_info_by_status_and_daterange(
      string fromdate,
      string todate,
      string status,
      string asset_id,
      Guid account_id,
      string groupids)
    {
      try
      {
        return this.db.get_dataset("SELECT u.full_name AS RequestorName , (SELECT property_value FROM sbt_user_properties WHERE property_name='staff_department' AND user_id=a.booked_for) AS Department , a.contact , (SELECT value FROM sbt_settings WHERE account_id='" + (object) account_id + "' and setting_id=b.building_id AND parameter='building') AS Building , (SELECT value FROM sbt_settings WHERE account_id='" + (object) account_id + "' and setting_id=b.level_id AND parameter='level') AS RoomLevel , b.name , a.purpose , a.book_from , a.book_to , a.status  ,  case when a.modified_by is not null then (SELECT b.full_name FROM dbo.sbt_users b WHERE b.account_id='" + (object) account_id + "' and b.user_id=a.modified_by)  else (SELECT b.full_name FROM dbo.sbt_users b WHERE b.account_id='" + (object) account_id + "' and b.user_id=a.created_by) end as BookedBy  FROM dbo.sbt_asset_bookings AS a INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id INNER JOIN sbt_users AS u ON a.booked_for=u.user_id WHERE " + groupids + "a.status=" + status + " AND a.account_id='" + (object) account_id + "' AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet noshow_report(string fromdate, string todate, Guid account_id, string groupids)
    {
      try
      {
        return this.db.get_dataset("SELECT FF.Division, FF.Department, c.name, c.asset_id , s.value AS Building, (SELECT COUNT(*) FROM dbo.sbt_asset_bookings WHERE asset_id=FF.asset_id AND status !=" + (object) 2 + "  AND (book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR book_to BETWEEN '" + fromdate + "' AND '" + todate + "')) AS NoOfBooking, FF.SumOfCancel AS SumOfNoShow FROM (SELECT CASE WHEN GROUPING(Division)= 1 THEN 'Grand Total' WHEN GROUPING(asset_id)= 1 THEN Department + ' Total' WHEN GROUPING(Division)!= 1 THEN Division END AS Division, Department, asset_id, SUM(NoOfCancellation) AS SumOfCancel FROM (SELECT R.Division, R.Department, a.asset_id, COUNT(*) AS NoOfCancellation FROM dbo.sbt_asset_bookings AS a INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id INNER JOIN (select user_id, max(case when property_name = 'staff_division' then property_value end) Division,max(case when property_name = 'staff_department' then property_value end) Department from sbt_user_properties where account_id='" + (object) account_id + "' group by user_id) AS R ON a.booked_for=R.user_id WHERE " + groupids + "  a.account_id='" + (object) account_id + "' AND a.status=" + (object) 3 + " AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') GROUP BY a.asset_id,r.division,r.department) AS F GROUP BY Division, Department, asset_id WITH ROLLUP) AS FF LEFT JOIN dbo.sbt_assets AS c ON FF.asset_id=c.asset_id LEFT JOIN sbt_settings AS s ON c.building_id=s.setting_id AND s.parameter='building';" + "SELECT b.name, COUNT(*) AS NoOfNoShow FROM dbo.sbt_asset_bookings AS a INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id INNER JOIN (select user_id, max(case when property_name = 'staff_division' then property_value end) Division,max(case when property_name = 'staff_department' then property_value end) Department from sbt_user_properties where account_id='" + (object) account_id + "' group by user_id) AS R ON a.booked_for=R.user_id WHERE " + groupids + "  a.account_id='" + (object) account_id + "' AND a.status=" + (object) 3 + " AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') GROUP BY b.name;") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet utility_report_by_dept(
      string fromdate,
      string todate,
      string where,
      Guid account_id,
      string groupids)
    {
      try
      {
        return this.db.get_dataset("SELECT R.Division , R.Department , R.Section , x.name,x.Category,x.RoomType , SUM(x.TotalNoOfBooked) AS TotalNoOfBooked , SUM(x.TotalHoursBooked ) AS TotalHoursBooked " + " from (select a.booked_for , b.name , (SELECT value FROM sbt_settings WHERE setting_id=b.category_id AND parameter='category' and account_id='" + (object) account_id + "') AS Category , (SELECT value FROM sbt_settings WHERE setting_id=b.asset_type AND parameter='asset_type' and account_id='" + (object) account_id + "') AS RoomType , COUNT(*) AS TotalNoOfBooked , SUM(a.book_duration) AS TotalHoursBooked  FROM dbo.sbt_asset_bookings AS a  INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id  WHERE  " + groupids + "  a.account_id='" + (object) account_id + "'AND a.status !=" + (object) 2 + " AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "')   and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where  (status=1 or status=2   or status=3   ) and transfer_request=1 and account_id='" + (object) account_id + "') " + where + " GROUP BY a.booked_for, b.name, b.category_id, b.asset_type) x " + " INNER JOIN (select user_id,   max(case when property_name = 'staff_division' then property_value Else '' end) Division,  max(case when property_name = 'staff_department' then property_value Else '' end) Department,  max(case when property_name = 'staff_section' then property_value Else '' end) Section  from sbt_user_properties  where account_id='" + (object) account_id + "'  and (property_name = 'staff_division' or property_name = 'staff_department' or property_name = 'staff_section')  and len(property_value) > 0  group by user_id) AS R ON x.booked_for=R.user_id " + " GROUP BY R.Division, R.Department, R.Section , x.name,x.Category,x.RoomType") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet utility_report_by_facility(
      string fromdate,
      string todate,
      string where,
      Guid account_id,
      string groupids)
    {
      try
      {
        return this.db.get_dataset("SELECT b.name , (SELECT value FROM sbt_settings WHERE setting_id=b.category_id AND parameter='category'  and account_id='" + (object) account_id + "') AS Category , (SELECT value FROM sbt_settings WHERE setting_id=b.asset_type AND parameter='asset_type' and account_id='" + (object) account_id + "') AS RoomType , (SELECT value FROM sbt_settings WHERE setting_id=b.building_id AND parameter='building' and account_id='" + (object) account_id + "') AS Building , (SELECT value FROM sbt_settings WHERE setting_id=b.level_id AND parameter='level' and account_id='" + (object) account_id + "') AS RoomLevel , COUNT(*) AS TotalNoOfBooked , SUM(a.book_duration) AS TotalHoursBooked FROM dbo.sbt_asset_bookings AS a INNER JOIN dbo.sbt_assets AS b ON a.asset_id=b.asset_id WHERE  " + groupids + " a.account_id='" + (object) account_id + "' AND a.status !=" + (object) api_constants.book_status.Blocked + " AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "')   and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=1 or status=2   or status=3   ) and transfer_request=1 and account_id='" + (object) account_id + "')" + where + " GROUP BY b.name, b.category_id, b.asset_type, b.building_id, b.level_id ORDER BY Building ASC ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet utility_report_by_facility2(
      string fromdate,
      string todate,
      string where,
      Guid account_id,
      string groupids)
    {
      try
      {
        return this.db.get_dataset("SELECT a.asset_id,COUNT(booking_id) AS TotalNoOfBooked,SUM(a.book_duration) AS TotalHoursBooked FROM dbo.sbt_asset_bookings AS a WHERE a.account_id='" + (object) account_id + "' AND a.status !=2 AND (a.book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR a.book_to BETWEEN '" + fromdate + "' AND '" + todate + "') GROUP BY a.asset_id order by a.asset_id;" + "select asset_id,sum(book_duration)  AS TotalHoursBooked,count(booking_id)  AS TotalNoOfBooked from sbt_asset_bookings where (status=1 or status=2   or status=3   ) and transfer_request=1 and account_id='" + (object) account_id + "' and (book_from BETWEEN '" + fromdate + "' AND '" + todate + "' OR book_to BETWEEN '" + fromdate + "' AND '" + todate + "') group by asset_id ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet getUnassignedData(
      string Report,
      string Fromdate,
      string Todate,
      string groupids,
      Guid account_id)
    {
      try
      {
        string Sql = "SELECT distinct Row_Number() OVER( ORDER BY a.user_id) AS Row, a.full_name,b.purpose as purpose1,c.asset_id,c.account_id,c.building_id,c.level_id,c.category_id," + " a.user_id,c.name AS RoomName,b.contact,(SELECT e.property_value FROM sbt_user_properties e WHERE e.user_id=a.user_id and e.account_id='" + (object) account_id + "' " + " AND e.property_name='staff_department') AS Department,(SELECT d.VALUE  FROM sbt_settings d WHERE d.setting_id=c.building_id and d.account_id='" + (object) account_id + "') AS" + " BuilidingName ,(SELECT d.VALUE   FROM sbt_settings d WHERE d.setting_id=c.level_id and d.account_id='" + (object) account_id + "') AS LEVEL,(SELECT d.VALUE  FROM " + " sbt_settings d WHERE d.setting_id=c.category_id and d.account_id='" + (object) account_id + "')AS purpose1," + " (SELECT u.value FROM sbt_settings u WHERE u.setting_id =b.setup_type and u.account_id='" + (object) account_id + "')AS Setup_type ," + " b.book_from,b.book_to,CASE b.status WHEN 1 THEN 'Booked' " + " when 4 THEN 'Pending' END AS BookedStatus FROM sbt_users a  JOIN dbo.sbt_asset_bookings b ON b.booked_for=a.user_id " + " and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=1 or status=2   or status=3 or status=0  ) and transfer_request=1 and account_id='" + (object) account_id + "')";
        switch (Report)
        {
          case "Unassigned":
            Sql = Sql + " AND a.status = 0 " + " AND b.status IN(1,4) and  " + " book_to between '" + Fromdate + "' and '" + Todate + "' and book_from between '" + Fromdate + "' and '" + Todate + "' " + "JOIN dbo.sbt_assets c ON b.asset_id=c.asset_id WHERE " + groupids + "c.account_id='" + (object) account_id + "' ";
            break;
          case "Setup":
            Sql = Sql + " AND b.setup_required=1" + " AND b.status IN(1,4) and  " + " book_to between '" + Fromdate + "' and '" + Todate + "' and book_from between '" + Fromdate + "' and '" + Todate + "' " + "JOIN dbo.sbt_assets c ON b.asset_id=c.asset_id  WHERE " + groupids + "c.account_id='" + (object) account_id + "'";
            break;
          case "Housekepping":
            Sql = Sql + " AND b.housekeeping_required=1 " + " AND b.status IN(1,4) and  " + " book_to between '" + Fromdate + "' and '" + Todate + "' and book_from between '" + Fromdate + "' and '" + Todate + "' " + "JOIN dbo.sbt_assets c ON b.asset_id=c.asset_id  WHERE " + groupids + "c.account_id='" + (object) account_id + "'";
            break;
        }
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_inactive_users(Guid account_id)
    {
      try
      {
        if (this.db.get_dataset("select user_id,full_name,email,username from sbt_users where account_id='" + (object) account_id + "' and status=0 order by full_name"))
          return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_future_inactive_user_booking_count(
      Guid account_id,
      string ids,
      DateTime starting_date)
    {
      try
      {
        if (this.db.get_dataset("select created_by,count(booking_id) as count from sbt_asset_bookings where account_id='" + (object) account_id + "' and status=1 and book_from>'" + starting_date.ToString(api_constants.sql_datetime_format) + "' and created_by in (" + ids + ") group by created_by"))
          return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_future_bookings_by_user(
      Guid account_id,
      long user_id,
      DateTime starting_date)
    {
      try
      {
        if (this.db.get_dataset("select a.booking_id,a.purpose,a.book_from,a.book_to,a.remarks,a.contact,a.email,(select b.name from sbt_assets b where b.asset_id=a.asset_id) as asset_name from sbt_asset_bookings a where a.account_id='" + (object) account_id + "' and status=1 and created_by='" + (object) user_id + "' and book_from>'" + starting_date.ToString(api_constants.sql_datetime_format) + "' order by a.book_from desc"))
          return this.db.resultDataSet;
      }
      catch (Exception ex)
      {
      }
      return (DataSet) null;
    }

    public DataSet getDailyview_Report(
      string Fromdate,
      string Todate,
      string groupids,
      Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT a.purpose,a.status, b.name  AS FacilityName  ,substring(CAST(a.book_from AS VARCHAR(25)),1,11) AS BookedDate," + " convert(char(5), a.book_from, 108) as TimeFrom, convert(char(5), a.book_to, 108) as TimeTo," + " case when a.modified_by is not null then (SELECT b.full_name FROM sbt_users b WHERE b.user_id=a.modified_by) " + " else (SELECT b.full_name FROM sbt_users b WHERE b.user_id=a.created_by and b.account_id='" + (object) account_id + "') end AS BookedBy," + "(SELECT b.full_name FROM sbt_users b WHERE b.user_id=a.booked_for and b.account_id='" + (object) account_id + "')AS RequestedBy,(SELECT c.property_value" + " FROM sbt_user_properties c WHERE c.property_name='staff_department'AND c.user_id=a.booked_for and c.account_id='" + (object) account_id + "') AS Department,a.contact," + "(SELECT d.VALUE  FROM sbt_settings d WHERE d.setting_id=b.building_id AND d.parameter='building' and d.account_id='" + (object) account_id + "')AS BuilidingName ," + "(SELECT d.VALUE  FROM sbt_settings d WHERE d.setting_id=b.level_id AND d.parameter='level' and d.account_id='" + (object) account_id + "') AS LEVEL," + "(SELECT d.VALUE  FROM sbt_settings d WHERE d.setting_id=b.category_id AND d.parameter='category' and d.account_id='" + (object) account_id + "')AS category ," + " (SELECT u.value FROM sbt_settings u WHERE u.setting_id IN (SELECT top 1 v.property_value FROM sbt_asset_properties v WHERE  v.asset_id=a.asset_id AND v.property_name='setup_type' and v.account_id='" + (object) account_id + "'))AS Setup_type FROM sbt_asset_bookings a " + " INNER join sbt_assets b ON a.asset_id=b.asset_id WHERE  " + groupids + " book_from BETWEEN '" + Fromdate + "' AND '" + Todate + "'" + "AND book_to BETWEEN '" + Fromdate + "'AND '" + Todate + "' AND a.asset_id=b.asset_id " + " and booking_id not in (select transfer_original_booking_id from sbt_asset_bookings where (status=1 or status=2   or status=3 or status=0  ) and transfer_request=1 and account_id='" + (object) account_id + "')" + " AND a.account_id='" + (object) account_id + "' and a.status!=2  and a.status=1  ORDER BY b.name,BookedBy, Department ;" + "SELECT distinct  b.name  AS FacilityName FROM sbt_asset_bookings a INNER join sbt_assets b ON a.asset_id=b.asset_id ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_user_group(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select * from sbt_user_groups where account_id='" + (object) account_id + "' ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_login_details(string email)
    {
      try
      {
        return this.db.get_dataset("select user_id,account_id,email,full_name,login_type,username,password from sbt_users where email='" + email + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_divison_master(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT master_id, name, parent_id, type FROM sbt_division_master WHERE account_id='" + (object) account_id + "' order by name asc") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet getgroup_name(long groupid, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT group_name  FROM sbt_user_groups WHERE group_id=" + (object) groupid + " and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_members_of_superuser_and_administrator(long owner_group_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT u.email,u.full_name FROM sbt_user_groups AS ug INNER JOIN sbt_user_group_mappings AS ugm ON ug.group_id=ugm.group_id INNER JOIN sbt_users AS u ON u.user_id=ugm.user_id WHERE u.status=1 AND ((ug.group_type IN (2) AND ug.group_id=" + (object) owner_group_id + ") OR ug.group_type IN (1)) AND u.account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    private bool name_exists(string sql)
    {
      bool flag = true;
      Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
      try
      {
        if (this.db.get_data_objects(sql).Count > 0)
          flag = false;
      }
      catch (Exception ex)
      {
        this.log.Error((object) sql, ex);
        flag = true;
      }
      return flag;
    }

    public bool checknameavilablity_insertmasterdata(
      string name,
      string Type,
      long Setting_id,
      Guid accountid)
    {
      return this.name_exists("select  setting_id from sbt_settings where value='" + name + "' and parameter='" + Type + "' and setting_id <> " + (object) Setting_id + " and account_id='" + (object) accountid + "'");
    }

    public bool checknameavilablity_insertmasterdata(string name, string Type, Guid accountid) => this.name_exists("select  setting_id from sbt_settings where value='" + name + "' and parameter='" + Type + "' and account_id='" + (object) accountid + "'");

    public DataSet get_asset_propertyfor_add(long asset_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT c.* FROM sbt_settings c WHERE c.setting_id NOT IN(SELECT  DISTINCT b.setting_id FROM  " + " dbo.sbt_asset_properties a, sbt_settings b WHERE  b.parameter='asset_property' " + " AND convert(VARCHAR(30),b.setting_id) = a.property_value AND a.property_name='asset_property' AND a.asset_id=" + (object) asset_id + " AND a.account_id='" + (object) account_id + "') AND " + " c.parameter='asset_property'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_asset_property_id(Guid account_id, string setting_id)
    {
      try
      {
        return this.db.get_dataset("select setting_id from sbt_settings where account_id='" + (object) account_id + "' and parameter='asset_property' and setting_id not in(" + setting_id + ")") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id,
      string status,
      string from,
      string to,
      long asssetid)
    {
      try
      {
        if (status == "" || status == "%")
          status = "0','1','2";
        return this.db.get_dataset("SELECT * FROM (SELECT row_number() OVER (ORDER BY RR." + orderby + " " + orderdir + ") AS Row_number,* FROM (SELECT a.from_msg,a.to_msg,a.sent,a.created_by,a.subject,a.created_on, 'sent_on'=(CASE WHEN a.sent_on IS  NULL THEN 'Not Yet Send' ELSE convert(NVARCHAR(30), a.sent_on) END),a.message_id,(SELECT c.full_name FROM sbt_users c WHERE c.user_id=a.created_by) AS username " + " FROM dbo.sbt_messaging_logs AS a INNER JOIN dbo.sbt_assets AS b ON a.record_id=b.record_id  and a.account_id='" + (object) account_id + "' and  A.created_on BETWEEN  '" + from + "' and '" + to + "'   UNION ALL " + "SELECT a.from_msg,a.to_msg,a.sent,created_by,a.subject,a.created_on,  'sent_on'=(CASE WHEN a.sent_on IS  NULL THEN 'Not Yet Send' ELSE convert(NVARCHAR(30), a.sent_on) END),a.message_id, (SELECT c.full_name FROM sbt_users c WHERE c.user_id=a.created_by) AS username  FROM dbo.sbt_messaging_logs AS a " + " WHERE a.record_id IN( (SELECT d.record_id FROM dbo.sbt_assets AS c INNER JOIN dbo.sbt_asset_bookings AS d ON c.asset_id=d.asset_id AND d.asset_id=" + (object) asssetid + " )) and  created_on BETWEEN  '" + from + "' and '" + to + "' )AS RR " + "WHERE (RR.from_msg LIKE '" + searchkey + "%' OR " + "RR.to_msg LIKE '" + searchkey + "%' OR RR.username LIKE '" + searchkey + "%' or RR.subject like '" + searchkey + "%' or RR.sent_on like '" + searchkey + "%'  ) AND  RR.sent in('" + status + "')" + " and  created_on BETWEEN  '" + from + "' and '" + to + "'  ) AS Result where  Row_number BETWEEN  " + fromrow + " and " + endrow + " ; " + "SELECT count(*) FROM (SELECT row_number() OVER (ORDER BY RR.from_msg ASC) AS Row_number,* FROM (SELECT a.from_msg,a.to_msg,a.sent,a.created_by,a.message_id,a.created_on,(SELECT c.full_name FROM sbt_users c WHERE c.user_id=a.created_by) AS username " + " FROM dbo.sbt_messaging_logs AS a INNER JOIN dbo.sbt_assets AS b ON a.record_id=b.record_id  and a.account_id='" + (object) account_id + "'   UNION ALL " + "SELECT a.from_msg,a.to_msg,a.sent,created_by,a.message_id,a.created_on, (SELECT c.full_name FROM sbt_users c WHERE c.user_id=a.created_by) AS username  FROM dbo.sbt_messaging_logs AS a " + " WHERE a.record_id IN((SELECT d.record_id FROM dbo.sbt_assets AS c INNER JOIN dbo.sbt_asset_bookings AS d ON c.asset_id=d.asset_id AND d.asset_id=" + (object) asssetid + ")))AS RR) AS R1 where   R1.sent in('" + status + "' ) " + " and  created_on BETWEEN  '" + from + "' and '" + to + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_emails(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id,
      string status,
      string from,
      string to)
    {
      try
      {
        if (status == "" || status == "%")
          status = "0','1','2";
        return this.db.get_dataset("select * from(SELECT row_number() OVER (ORDER BY " + orderby + "  " + orderdir + ") AS Row_number,* FROM (SELECT * FROM (SELECT a.from_msg,a.to_msg,a.sent,a.created_by,a.subject,a.account_id,a.record_id,a.created_on, " + "a.sent_on,CASE  a.sent WHEN  0 THEN 'Not Yet Send' ELSE 'Sucessfully Sent' END AS Sentmsg,  a.message_id,(SELECT c.full_name FROM sbt_users c WHERE c.user_id=a.created_by)AS username " + " FROM dbo.sbt_messaging_logs a WHERE  a.account_id='" + (object) account_id + "'" + " AND  a.sent in('" + status + "') " + " and  created_on BETWEEN  '" + from + "' and '" + to + "'" + " ) AS R)   AS Result" + " WHERE (Result.from_msg LIKE '" + searchkey + "%'  OR Result.to_msg LIKE '" + searchkey + "%'  OR Result.subject like '" + searchkey + "%'  OR  Result.sent_on like '" + searchkey + "%' OR Result.username LIKE '" + searchkey + "%'" + " )) as RR where  RR.Row_number BETWEEN  " + fromrow + " and " + endrow + " ;" + "SELECT count(*) FROM (SELECT row_number() OVER (ORDER BY R.from_msg ASC) AS Row_number,* FROM (SELECT a.from_msg,a.to_msg,a.sent,a.created_by,a.subject,a.account_id,a.record_id,a.created_on, " + "CASE  a.sent WHEN  0 THEN 'Not Yet Send' ELSE 'Sucessfully Sent' END AS Sent_on,  a.message_id,(SELECT c.full_name FROM sbt_users c WHERE c.user_id=a.created_by)AS username " + " FROM dbo.sbt_messaging_logs a WHERE  a.account_id='" + (object) account_id + "'   ) AS R)   AS Result" + " WHERE (Result.from_msg LIKE '" + searchkey + "%'  OR Result.to_msg LIKE '" + searchkey + "%'  OR Result.username LIKE '" + searchkey + "%'  or Result.subject like '" + searchkey + "%'  or  Result.sent_on like '" + searchkey + "%'  ) AND  Result.sent in(0,1,2)  " + " and  created_on BETWEEN  '" + from + "' and '" + to + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_usersnameonly(long user_ID, string account_ID)
    {
      try
      {
        return this.db.get_dataset("SELECT  user_id,username,full_name,email  FROM sbt_users where user_id=" + (object) user_ID + " and account_id='" + account_ID + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_usersnameonly(string appliocationID, string account_ID)
    {
      try
      {
        return this.db.get_dataset("SELECT  user_id,username  FROM sbt_users where account_id='" + account_ID + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_logs(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id,
      string modulename,
      string action,
      string status,
      string from,
      string to)
    {
      try
      {
        string Sql;
        if (searchkey == "%")
        {
          string str = "SELECT * FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,a.audit_log_id,a.module_name,a.action,a.status,a.old_value,a.new_value," + "a.created_on,a.created_by,(SELECT full_name FROM sbt_users WHERE  user_id = a.created_by and account_id='" + (object) account_id + "') AS createduser,a.account_id FROM sbt_audit_logs a where " + "a.account_id = '" + (object) account_id + "' and (a.module_name like '%" + modulename + "%' and a.action like '%" + action + "%' and a.status like '%" + status + "%' and a.created_on like '%" + searchkey + "%'" + ") and a.created_on between '" + from + "' and '" + to + "') AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS  RecordCnt FROM sbt_audit_logs " + " WHERE account_id = '" + (object) account_id + "' ";
          if (modulename != "%")
            str = str + " and module_name like '%" + modulename + "%'";
          if (action != "%")
            str = str + " and action like '%" + action + "%'";
          if (status != "%")
            str = str + " and status like '%" + status + "%'";
          Sql = str + " and created_on between '" + from + "' and '" + to + "';";
        }
        else
          Sql = "SELECT *  FROM (SELECT Row_Number() OVER(ORDER BY " + orderby + " " + orderdir + ") AS Row,a.audit_log_id,a.module_name,a.action,a.status,a.old_value,a.new_value," + "a.created_on,a.created_by,(SELECT full_name FROM sbt_users WHERE  user_id =a.created_by and account_id='" + (object) account_id + "') AS createduser,a.account_id FROM sbt_audit_logs a where " + "a.account_id = '" + (object) account_id + "' and (a.module_name like '" + searchkey + "%' or a.action like '" + searchkey + "%' or a.status like '" + searchkey + "%' or a.created_on like '" + searchkey + "%'" + ") and a.created_on between '" + from + "' and '" + to + "') AS t2 WHERE Row BETWEEN " + fromrow + " AND " + endrow + ";" + " SELECT COUNT(*) AS  RecordCnt FROM sbt_audit_logs WHERE " + " account_id = '" + (object) account_id + "' and (module_name like '" + searchkey + "%' or action like '" + searchkey + "%' or status like '" + searchkey + "%' or created_on like '" + searchkey + "%' or created_by like '" + searchkey + "%')" + " and created_on between '" + from + "' and '" + to + "';";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users_by_group(
      int start,
      int recordnumber,
      string dir,
      long group_id,
      Guid account_id,
      string searchkey)
    {
      try
      {
        return this.db.get_dataset(" SELECT * FROM(SELECT Row_number() OVER(ORDER BY user_id  asc) AS row, tw.* FROM ( SELECT full_name,user_id,email,username " + " from sbt_users where user_id in (select user_id from sbt_user_group_mappings where" + " group_id='" + (object) group_id + "' and account_id='" + (object) account_id + "')" + " and full_name like '%" + searchkey + "%') AS tw ) AS result WHERE row BETWEEN " + (object) start + " AND " + (object) recordnumber + "  order by  full_name " + dir + " ;" + " select count(*)  from sbt_users where user_id in (select user_id from sbt_user_group_mappings where group_id='" + (object) group_id + "' and account_id='" + (object) account_id + "' and full_name like '%" + searchkey + "%')") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_Master_settings(
      Guid account_id,
      string searchkey,
      string Masterfilter,
      string columnname,
      string ordertype)
    {
      try
      {
        return this.db.get_dataset("select * from(SELECT a.parameter,a.created_by,a.value,a.modified_on,a.setting_id,'Status'=CASE a.status WHEN 1 THEN 'Active' ELSE 'Inactive' END ,(SELECT b.full_name FROM sbt_users b WHERE a.modified_by=b.user_id and b.account_id='" + (object) account_id + "')AS fullname FROM sbt_settings a WHERE parameter='" + Masterfilter + "' and a.account_id='" + (object) account_id + "') as RR  " + "WHERE  RR.value LIKE '" + searchkey + "%' OR  RR.parameter LIKE '" + searchkey + "%' OR RR.fullname LIKE '" + searchkey + "%' OR RR.Status LIKE '" + searchkey + "%' order by " + columnname + " " + ordertype + " ;  " + "select count(*) from(SELECT a.parameter,a.created_by,a.value,a.modified_on,a.setting_id,'Status'=CASE a.status WHEN 1 THEN 'Active' ELSE 'Inactive' END ,(SELECT b.full_name FROM sbt_users b WHERE a.modified_by=b.user_id and b.account_id='" + (object) account_id + "')AS fullname FROM sbt_settings a WHERE parameter='" + Masterfilter + "' and a.account_id='" + (object) account_id + "') as RR  " + "WHERE  RR.value LIKE '" + searchkey + "%' OR  RR.parameter LIKE '" + searchkey + "%' OR RR.fullname LIKE '" + searchkey + "%' OR RR.Status LIKE '" + searchkey + "%'  ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orderby + " " + orderdir + ") AS RowNumber, a.user_id, a.full_name, a.username, a.email" + ", (CASE a.status WHEN 1 THEN 'Active' ELSE 'Inactive' END) AS Status" + ", (CASE a.locked WHEN 1 THEN 'Yes' ELSE 'No' END) AS Locked ," + "(select c.blacklist_id from sbt_blacklists c WHERE c.user_id=a.user_id  and c.account_id='" + (object) account_id + "')AS Blacklistperson" + " FROM sbt_users AS a WHERE account_id='" + (object) account_id + "' AND (full_name LIKE '%" + searchkey + "%' OR email LIKE '%" + searchkey + "%' OR Status LIKE '%" + searchkey + "%' OR Locked LIKE '%" + searchkey + "%')) AS b " + " WHERE RowNumber BETWEEN " + fromrow + " AND " + endrow + " ORDER BY b.user_id ASC;" + " SELECT COUNT(*) AS RecordCnt FROM (SELECT ROW_NUMBER() OVER (Order by full_name) AS RowNumber, a.user_id, a.full_name, a.username, a.email" + ", (CASE a.status WHEN 1 THEN 'Active' ELSE 'Inactive' END) AS Status" + ", (CASE a.locked WHEN 1 THEN 'Yes' ELSE 'No' END) AS Locked" + " FROM sbt_users AS a WHERE account_id='" + (object) account_id + "') AS b" + " WHERE (full_name LIKE '%" + searchkey + "%' OR email LIKE '%" + searchkey + "%' OR Status LIKE '%" + searchkey + "%' OR Locked LIKE '%" + searchkey + "%');") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      if (orderby == "")
        orderby = "email";
      return (DataSet) null;
    }

    public DataSet get_users_With_properties(
      string fromrow,
      string endrow,
      string orderby,
      string orderdir,
      string searchkey,
      Guid account_id)
    {
      try
      {
        string str1 = " SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY " + orderby + "  " + orderdir + ") AS RowNo, Result.* FROM (SELECT users.email,users.login_type, users.full_name, users.status,users.locked,users.user_insert_type, user_property.* FROM (select user_id, staff_division, staff_department, staff_section,given_name,staff_desg,staff_inst,staff_offphone" + " from(  select user_id, property_value, property_name   from sbt_user_properties where account_id='" + (object) account_id + "' GROUP BY user_id, property_value, property_name) d " + " pivot( max(property_value)  for property_name in (staff_division, staff_department, staff_section,given_name,staff_desg,staff_inst,staff_offphone) " + " ) piv) AS user_property INNER JOIN sbt_users AS users ON users.user_id=user_property.user_id " + "WHERE users.account_id='" + (object) account_id + "'" + " ) AS Result ";
        string str2;
        if (searchkey != "%")
        {
          str2 = str1 + " WHERE staff_section LIKE '%" + searchkey + "%' or  staff_division LIKE '%" + searchkey + "%' or staff_department LIKE '%" + searchkey + "%' or " + " given_name LIKE '%" + searchkey + "%' or staff_desg LIKE '%" + searchkey + "%' or staff_inst LIKE '%" + searchkey + "%' or staff_offphone  LIKE '%" + searchkey + "%' or email like '%" + searchkey + "%' " + " or full_name LIKE '%" + searchkey + "%' ";
          if ("SYSTEM".Contains(searchkey.ToUpper()))
            str2 += " or user_insert_type = 0 ";
        }
        else
          str2 = str1 + " WHERE 1=1";
        string str3 = str2 + " ) AS Final WHERE RowNo BETWEEN " + fromrow + " AND " + endrow + " " + " SELECT count(*) FROM (SELECT ROW_NUMBER() OVER(ORDER BY full_name ASC) AS RowNo, Result.* FROM (SELECT users.email, users.full_name, users.status,users.user_insert_type, user_property.* FROM (select user_id, staff_division, staff_department, staff_section,given_name,staff_desg,staff_inst,staff_offphone" + " from(  select user_id, property_value, property_name   from sbt_user_properties where account_id='" + (object) account_id + "' GROUP BY user_id, property_value, property_name) d " + " pivot( max(property_value)  for property_name in (staff_division, staff_department, staff_section,given_name,staff_desg,staff_inst,staff_offphone) " + " ) piv) AS user_property INNER JOIN sbt_users AS users ON users.user_id=user_property.user_id " + "WHERE users.account_id='" + (object) account_id + "'" + " ) AS Result ";
        string str4;
        if (searchkey != "%")
        {
          str4 = str3 + " WHERE staff_section LIKE '%" + searchkey + "%' or  staff_division LIKE '%" + searchkey + "%' or staff_department LIKE '%" + searchkey + "%' or " + " given_name LIKE '%" + searchkey + "%' or staff_desg LIKE '%" + searchkey + "%' or staff_inst LIKE '%" + searchkey + "%' or staff_offphone  LIKE '%" + searchkey + "%' or email like '%" + searchkey + "%' " + " or full_name LIKE '%" + searchkey + "%' ";
          if ("SYSTEM".Contains(searchkey.ToUpper()))
            str4 += " or user_insert_type = 0 ";
        }
        else
          str4 = str3 + " WHERE 1=1";
        return this.db.get_dataset(str4 + " ) AS Final ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_users_With_properties(Guid account_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT users.email, users.full_name, users.status,users.locked,users.user_insert_type, user_property.* FROM (select user_id, staff_division, staff_department, staff_section,given_name,staff_desg,staff_inst,staff_offphone" + " from(  select user_id, property_value, property_name   from sbt_user_properties where account_id='" + (object) account_id + "' GROUP BY user_id, property_value, property_name) d " + " pivot( max(property_value)  for property_name in (staff_division, staff_department, staff_section,given_name,staff_desg,staff_inst,staff_offphone) " + " ) piv) AS user_property INNER JOIN sbt_users AS users ON users.user_id=user_property.user_id " + "WHERE users.account_id='" + (object) account_id + "' and users.status = 0 and users.activated=0") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_userlist_forexport(Guid account_id, string search, int endrows)
    {
      try
      {
        string str = "select * from (SELECT Row_Number() over(order by full_name asc) as Rownumber, * FROM (SELECT full_name" + ", email" + ",login_type" + ",u.user_id" + ", up.staff_division" + ", up.staff_department" + ", up.staff_section" + ", staff_title" + ", given_name" + ", native_name" + ", staff_id" + ", staff_inst" + ", staff_desg" + ", staff_offphone" + ", staff_pager_mobile" + ", staff_comm_date" + ", staff_cess_date" + ", (CASE u.status WHEN 1 THEN 'Active' ELSE 'Inactive' END) AS [Status] " + ", (CASE u.locked WHEN 1 THEN 'Yes' ELSE 'No' END) AS Locked " + ", u.user_insert_type " + "FROM dbo.sbt_users AS u " + "INNER JOIN " + "(select user_id, " + "  max(case when property_name = 'staff_division' then property_value end) staff_division, " + "  max(case when property_name = 'staff_department' then property_value end) staff_department, " + "  max(case when property_name = 'staff_section' then property_value end) staff_section, " + "  max(case when property_name = 'staff_title' then property_value end) staff_title, " + "  max(case when property_name = 'given_name' then property_value end) given_name, " + "  max(case when property_name = 'native_name' then property_value end) native_name, " + "  max(case when property_name = 'staff_id' then property_value end) staff_id, " + "  max(case when property_name = 'staff_inst' then property_value end) staff_inst, " + "  max(case when property_name = 'staff_desg' then property_value end) staff_desg, " + "  max(case when property_name = 'staff_offphone' then property_value end) staff_offphone, " + "  max(case when property_name = 'staff_pager_mobile' then property_value end) staff_pager_mobile, " + "  max(case when property_name = 'staff_comm_date' then property_value end) staff_comm_date, " + "  max(case when property_name = 'staff_cess_date' then property_value end) staff_cess_date " + " from dbo.sbt_user_properties " + "where account_id='" + (object) account_id + "' " + "group by user_id) AS up ON u.user_id=up.user_id  " + "WHERE u.account_id='" + (object) account_id + "') AS R " + "WHERE (full_name LIKE '%" + search + "%' OR email LIKE '%" + search + "%' OR [Status] LIKE '%" + search + "%' OR Locked LIKE '%" + search + "%'" + " OR staff_inst LIKE '%" + search + "%' OR staff_division LIKE '%" + search + "%' OR staff_section LIKE '%" + search + "%' OR staff_desg LIKE '%" + search + "%' OR staff_offphone LIKE '%" + search + "%'";
        if ("SYSTEM".Contains(search))
          str += " OR user_insert_type = 0 ";
        return this.db.get_dataset(str + " )) as Output ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_userlist_forexport(Guid account_id, string search)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM (SELECT full_name" + ", email" + ", up.staff_division" + ", up.staff_department" + ", up.staff_section" + ", staff_title" + ", given_name" + ", native_name" + ", staff_id" + ", staff_inst" + ", staff_desg" + ", staff_offphone" + ", staff_pager_mobile" + ", staff_comm_date" + ", staff_cess_date" + ", (CASE u.status WHEN 1 THEN 'Active' ELSE 'Inactive' END) AS [Status] " + ", (CASE u.locked WHEN 1 THEN 'Yes' ELSE 'No' END) AS Locked " + "FROM dbo.sbt_users AS u " + "INNER JOIN " + "(select user_id, " + "  max(case when property_name = 'staff_division' then property_value end) staff_division, " + "  max(case when property_name = 'staff_department' then property_value end) staff_department, " + "  max(case when property_name = 'staff_section' then property_value end) staff_section, " + "  max(case when property_name = 'staff_title' then property_value end) staff_title, " + "  max(case when property_name = 'given_name' then property_value end) given_name, " + "  max(case when property_name = 'native_name' then property_value end) native_name, " + "  max(case when property_name = 'staff_id' then property_value end) staff_id, " + "  max(case when property_name = 'staff_inst' then property_value end) staff_inst, " + "  max(case when property_name = 'staff_desg' then property_value end) staff_desg, " + "  max(case when property_name = 'staff_offphone' then property_value end) staff_offphone, " + "  max(case when property_name = 'staff_pager_mobile' then property_value end) staff_pager_mobile, " + "  max(case when property_name = 'staff_comm_date' then property_value end) staff_comm_date, " + "  max(case when property_name = 'staff_cess_date' then property_value end) staff_cess_date " + " from dbo.sbt_user_properties " + "where account_id='" + (object) account_id + "' " + "group by user_id) AS up ON u.user_id=up.user_id  " + "WHERE u.account_id='" + (object) account_id + "') AS R " + "WHERE (full_name LIKE '%" + search + "%' OR email LIKE '%" + search + "%' OR [Status] LIKE '%" + search + "%' OR Locked LIKE '%" + search + "%') order by full_name Asc ") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_asset_property_details(Guid account_id, string search, long asset_id)
    {
      try
      {
        return this.db.get_dataset("select * from(SELECT value ,setting_id, (SELECT remarks FROM sbt_asset_properties WHERE property_name='asset_property' AND asset_id=" + (object) asset_id + " AND property_value=setting_id and account_id='" + (object) account_id + "') AS Remarks " + " , (SELECT property_value FROM sbt_asset_properties WHERE property_name='asset_property' AND asset_id=" + (object) asset_id + " AND property_value=setting_id and account_id='" + (object) account_id + "') AS Facility_Setting_id " + ", (SELECT  'available'=CASE c.available WHEN 0 THEN 'YES' ELSE 'NO' End FROM sbt_asset_properties c WHERE property_name='asset_property' AND asset_id=" + (object) asset_id + " AND property_value=setting_id and account_id='" + (object) account_id + "') AS available" + "   FROM sbt_settings WHERE parameter='asset_property' and account_id='" + (object) account_id + "') as RR  where RR.value like '" + search + "%' or RR.Remarks like '" + search + "%' or RR.available like '" + search + "%'" + " SELECT count(*)  FROM sbt_settings WHERE parameter='asset_property' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_asset_property_details(
      string startno,
      string endno,
      string orderby,
      string searchStr,
      string orderdir,
      Guid account_id,
      string search,
      long asset_id)
    {
      try
      {
        return this.db.get_dataset("select * from(SELECT value ,setting_id, (SELECT remarks FROM sbt_asset_properties WHERE property_name='asset_property' AND asset_id=" + (object) asset_id + " AND property_value=setting_id and account_id='" + (object) account_id + "') AS Remarks " + " , (SELECT property_value FROM sbt_asset_properties WHERE account_id='" + (object) account_id + "' and property_name='asset_property' AND asset_id=" + (object) asset_id + " AND property_value=setting_id) AS Facility_Setting_id " + ", (SELECT  'available'=CASE c.available WHEN 0 THEN 'YES' ELSE 'NO' End FROM sbt_asset_properties c WHERE account_id='" + (object) account_id + "' and property_name='asset_property' AND asset_id=" + (object) asset_id + " AND property_value=setting_id) AS available" + "   FROM sbt_settings WHERE parameter='asset_property' and account_id='" + (object) account_id + "') as RR  where RR.value like '" + search + "%' or RR.Remarks like '" + search + "%' or RR.available like '" + search + "%' order by " + orderby + " " + orderdir + " SELECT count(*)  FROM sbt_settings WHERE parameter='asset_property' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_asset_property_details(
      Guid account_id,
      string search,
      long asset_id,
      long setting_id)
    {
      try
      {
        return this.db.get_dataset(" SELECT * FROM  ( " + "SELECT b.name," + " (SELECT c.value FROM sbt_settings c WHERE c.account_id='" + (object) account_id + "' and CAST(c.setting_id AS VARCHAR(10))=a.property_value) AS Propertyname," + " (SELECT c.setting_id FROM sbt_settings c WHERE c.account_id='" + (object) account_id + "' and CAST(c.setting_id AS VARCHAR(10)) =a.property_value) AS PropertyID, " + " (SELECT c.value  FROM sbt_settings c   WHERE c.account_id='" + (object) account_id + "' and c.setting_id=b.building_id) AS BuildingName," + " (SELECT c.value FROM  sbt_settings c  WHERE c.account_id='" + (object) account_id + "' and c.setting_id=b.level_id) AS LevelID, " + " (SELECT c.value FROM  sbt_settings c  WHERE c.account_id='" + (object) account_id + "' and c.setting_id=b.category_id) AS category,b.code " + "FROM sbt_asset_properties a,sbt_assets b WHERE a.asset_id=" + (object) asset_id + " AND  a.property_name='asset_property' " + " AND a.asset_id=b.asset_id and  a.account_id='" + (object) account_id + "') AS RR where RR.PropertyID=" + (object) setting_id + ";" + " SELECT count(*)  FROM sbt_settings WHERE parameter='asset_property' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_user(long user_id, Guid account_id)
    {
      try
      {
        return this.db.get_dataset("select user_id,full_name, email,status,locked from sbt_users where user_id='" + (object) user_id + "' and account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_user(Guid account_id, long modified_by)
    {
      try
      {
        return this.db.get_dataset("select user_id,full_name,email from sbt_users where  account_id='" + (object) account_id + "' and user_id=" + (object) modified_by) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_username_fromgroup(long groupid)
    {
      try
      {
        return this.db.get_dataset("SELECT full_name,email FROM sbt_users WHERE user_id IN (SELECT user_id FROM sbt_user_group_mappings WHERE group_id=" + (object) groupid + ")") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_report_problem_owner(
      long user_id,
      Guid account_id,
      string from,
      string to,
      string division,
      string department,
      string section)
    {
      try
      {
        string Sql = " select a.Asset_id,a.Problem_id,a.Reported_on,a.Reported_by,a.subject,a.remarks," + "  b.name,b.code, b.building_name,b.level_name,dbo.sbt_fn_user_name(a.reported_by, a.account_id) AS reported_by_name" + "  FROM sbt_report_problem a left join vw_sbt_assets as b on a.asset_id = b.asset_id WHERE a.Asset_id IN( SELECT b.asset_id FROM sbt_assets b WHERE b.asset_owner_group_id IN( " + " SELECT c.group_id FROM sbt_user_group_mappings c WHERE c.user_id=" + (object) user_id + ")) and a.account_id='" + (object) account_id + "'";
        if (from != "" && to != "")
          Sql = Sql + " and   a.Reported_on between '" + from + "' and '" + to + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_report_problem_admin(
      Guid account_id,
      string from,
      string to,
      string division,
      string department,
      string section)
    {
      try
      {
        string Sql = " select a.Asset_id,a.Problem_id,a.Reported_on,a.Reported_by,a.subject,a.remarks," + "  b.name,b.code, b.building_name,b.level_name, dbo.sbt_fn_user_name(a.reported_by, a.account_id) AS reported_by_name" + "  FROM sbt_report_problem a  left join vw_sbt_assets as b on a.asset_id = b.asset_id " + " WHERE  a.account_id='" + (object) account_id + "' ";
        if (from != "" && to != "")
          Sql = Sql + " and Reported_on between '" + from + "' and '" + to + "'";
        return this.db.get_dataset(Sql) ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_acl_group(Guid account_id)
    {
      try
      {
        return this.db.get_dataset("SELECT [asset_id],[group_id],[is_view],[is_book] FROM sbt_assets_permissions where account_id='" + (object) account_id + "' order by group_id") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "get_acl_group error -> ", ex);
      }
      return (DataSet) null;
    }

    public DataSet get_report_utilization_by_asset(Guid account_id, DateTime from, DateTime to)
    {
      try
      {
        return this.db.get_dataset("SELECT * FROM sbt_utilization_report_summary WHERE account_id='" + (object) account_id + "' and booking_date between '" + from.ToString(api_constants.sql_datetime_format) + "' and '" + to.ToString(api_constants.sql_datetime_format) + "'") ? this.db.resultDataSet : (DataSet) null;
      }
      catch (Exception ex)
      {
        this.log.Error((object) "get_report_utilization_by_asset -> ", ex);
      }
      return (DataSet) null;
    }
  }
}
