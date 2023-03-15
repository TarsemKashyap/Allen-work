// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.groups_permission
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;

namespace skynapse.fbs
{
  public class groups_permission
  {
    public bool facility_view;
    public bool facility_add;
    public bool facility_edit;
    public bool facility_delete;
    public bool facility_permissions;
    public bool users_view;
    public bool users_add;
    public bool users_edit;
    public bool users_delete;
    public bool users_blacklist;
    public bool groups_view;
    public bool groups_add;
    public bool groups_edit;
    public bool groups_delete;
    public bool holidays_view;
    public bool holidays_add;
    public bool holidays_edit;
    public bool holidays_delete;
    public bool holidays_upload;
    public bool settings_view;
    public bool settings_edit;
    public bool master_view;
    public bool master_add;
    public bool master_edit;
    public bool master_delete;
    public bool logs_view;
    public bool emaillogs_view;
    public bool templates_view;
    public bool templates_edit;
    public bool utilization_report_by_department_view;
    public bool utilization_report_by_room_view;
    public bool cancellation_report_view;
    public bool noshow_report_view;
    public bool unassigned_report_view;
    public bool upcoming_setup_report_view;
    public bool housekeeping_report_view;
    public bool daily_report_view;
    public List<string> group_ids;
    public List<string> group_types;
    public bool isAdminType;
    public bool isSuperUserType;
  }
}
