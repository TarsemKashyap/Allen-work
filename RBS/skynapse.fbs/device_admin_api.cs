// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.device_admin_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace skynapse.fbs
{
  public class device_admin_api
  {
    private string connection_string;
    public static string sql_datetime_format = "yyyy-MM-dd HH:mm:ss";
    private Dictionary<string, string> room_list;
    private List<Guid> device_codes_list;
    private DataAccess db;
    private asset_api aapi;
    public string error_status = "1";

    public device_admin_api()
    {
      this.connection_string = ConfigurationManager.ConnectionStrings["sql"].ConnectionString;
      this.db = new DataAccess(this.connection_string);
    }

    public DataSet get_settings(Guid account_id) => this.db.get_dataset("select * from sbt_apps_api_device_settings where account_id='" + (object) account_id + "' and device_id=0") ? this.db.resultDataSet : (DataSet) null;

    public string get_setting(string parameter, Guid account_id)
    {
      string setting = "";
      if (this.db.get_dataset("select value from sbt_apps_api_device_settings where account_id='" + (object) account_id + "' and device_id=0 and parameter='" + parameter + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
        setting = this.db.resultDataSet.Tables[0].Rows[0]["value"].ToString();
      return setting;
    }

    public DataSet get_devices(Guid account_id) => this.db.get_dataset("select * from sbt_apps_api_devices where account_id='" + (object) account_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device(long device_id, Guid account_id) => this.db.get_dataset("select * from sbt_apps_api_devices where account_id='" + (object) account_id + "' and device_id='" + (object) device_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device_properties(long device_id, Guid account_id) => this.db.get_dataset("select * from sbt_apps_api_device_settings where account_id='" + (object) account_id + "' and device_id='" + (object) device_id + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device_properties(string parameter, Guid account_id) => this.db.get_dataset("select * from sbt_apps_api_device_settings where account_id='" + (object) account_id + "' and parameter='" + parameter + "'") ? this.db.resultDataSet : (DataSet) null;

    public DataSet get_device_properties(Guid device_code, Guid account_id) => this.db.get_dataset("select * from sbt_apps_api_device_settings where account_id='" + (object) account_id + "' and device_id in (select device_id from sbt_apps_api_devices where device_code='" + (object) device_code + "')") ? this.db.resultDataSet : (DataSet) null;

    public List<device_setting> get_device_properties(long device_id)
    {
      List<device_setting> deviceProperties = new List<device_setting>();
      new device_setting().device_item_id = 0L;
      if (this.db.get_dataset("select * from sbt_apps_api_device_settings where device_id='" + (object) device_id + "'") && this.db.resultDataSet.Tables[0].Rows.Count > 0)
      {
        device_setting deviceSetting = new device_setting();
        DataRow row = this.db.resultDataSet.Tables[0].Rows[0];
        deviceSetting.device_item_id = Convert.ToInt64(row["device_item_id"]);
        deviceSetting.device_id = device_id;
        deviceSetting.parameter = row["parameter"].ToString();
        deviceSetting.value = row["value"].ToString();
        deviceSetting.account_id = new Guid(row["account_id"].ToString());
        deviceProperties.Add(deviceSetting);
      }
      return deviceProperties;
    }

    public device update_device(device obj)
    {
      try
      {
        obj.device_id = !this.db.execute_procedure("sbt_sp_apps_api_devices", new Dictionary<string, object>()
        {
          {
            "@device_id",
            (object) obj.device_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@created_by",
            (object) obj.created_by
          },
          {
            "@modified_by",
            (object) obj.modified_by
          },
          {
            "@status",
            (object) obj.status
          },
          {
            "@device_code",
            (object) obj.device_code
          },
          {
            "@device_name",
            (object) obj.device_name
          },
          {
            "@mac_address",
            (object) obj.mac_address
          },
          {
            "@serial_no",
            (object) obj.serial_no
          },
          {
            "@asset_id",
            (object) obj.asset_id
          },
          {
            "@app_config_id",
            (object) obj.app_config_id
          },
          {
            "@screen_width",
            (object) obj.screen_width
          },
          {
            "@screen_height",
            (object) obj.screen_height
          },
          {
            "@record_id",
            (object) obj.record_id
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
      }
      return obj;
    }

    public device_setting update_device_setting(device_setting obj)
    {
      try
      {
        obj.device_id = !this.db.execute_procedure("sbt_sp_apps_api_device_settings", new Dictionary<string, object>()
        {
          {
            "@device_id",
            (object) obj.device_id
          },
          {
            "@account_id",
            (object) obj.account_id
          },
          {
            "@device_item_id",
            (object) obj.device_item_id
          },
          {
            "@parameter",
            (object) obj.parameter
          },
          {
            "@value",
            (object) obj.value
          }
        }) ? 0L : Convert.ToInt64(this.db.resultString);
      }
      catch (Exception ex)
      {
      }
      return obj;
    }
  }
}
