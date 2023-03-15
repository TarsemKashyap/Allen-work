// Decompiled with JetBrains decompiler
// Type: skynapse.device.device_base
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web.UI;

namespace skynapse.device
{
  public class device_base : Page
  {
    protected skynapse.fbs.user current_user;
    protected account current_account;
    protected device_details current_device;
    protected ILog log = LogManager.GetLogger("device_log");
    protected Dictionary<string, string> parameters;
    protected device_api dapi;
    protected users_api uapi;
    protected DateTime current_time;
    protected asset current_asset;
    protected asset_api assets;
    protected settings_api settings;
    protected DataSet asset_properties;
    protected DataSet settings_data;
    protected cache_api capi;
    protected booking_api bookings;
    protected booking_bl bookingsbl;
    protected string default_url;

    public device_base() => this.Load += new EventHandler(this.Page_Load);

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.Response.IsClientConnected)
        this.Response.End();
      this.Response.BufferOutput = true;
      this.query_string();
      if (!this.parameters.ContainsKey("dcode"))
        return;
      this.settings = (settings_api) this.Application["rdp_settings"];
      if (this.settings == null)
      {
        this.settings = new settings_api();
        this.Application.Add("rdp_settings", (object) this.settings);
      }
      this.dapi = (device_api) this.Application["rdp_device_api"];
      if (this.dapi == null)
      {
        this.dapi = new device_api();
        this.Application.Add("rdp_device_api", (object) this.dapi);
      }
      this.uapi = (users_api) this.Application["rdp_users_api"];
      if (this.uapi == null)
      {
        this.uapi = new users_api();
        this.Application.Add("rdp_users_api", (object) this.uapi);
      }
      this.assets = (asset_api) this.Application["rdp_asset_api"];
      if (this.assets == null)
      {
        this.assets = new asset_api();
        this.Application.Add("rdp_asset_api", (object) this.assets);
      }
      this.bookings = (booking_api) this.Application["rdp_booking_api"];
      if (this.bookings == null)
      {
        this.bookings = new booking_api();
        this.Application.Add("rdp_booking_api", (object) this.bookings);
      }
      this.bookingsbl = (booking_bl) this.Application["rdp_booking_bl"];
      if (this.bookingsbl == null)
      {
        this.bookingsbl = new booking_bl();
        this.Application.Add("rdp_booking_bl", (object) this.bookingsbl);
      }
      this.capi = (cache_api) this.Application["rdp_cache_api"];
      if (this.capi == null)
      {
        this.capi = new cache_api(Convert.ToDouble(ConfigurationManager.AppSettings["cache_expiration"]));
        this.Application.Add("rdp_cache_api", (object) this.capi);
      }
      this.init_session_details();
      this.current_time = this.current_timestamp();
      this.default_url = ConfigurationManager.AppSettings["rdp_start"] + "?dcode=" + (object) this.current_device.device_code;
    }

    protected DateTime current_timestamp() => DateTime.UtcNow.AddHours(this.current_account.timezone);

    private void query_string()
    {
      this.parameters = new Dictionary<string, string>();
      foreach (string str in (NameObjectCollectionBase) this.Request.QueryString)
      {
        try
        {
          if (!this.parameters.ContainsKey(str))
            this.parameters.Add(str, this.Request.QueryString[str]);
        }
        catch (Exception ex)
        {
          this.log.Error((object) ("key '" + str + "' does not have a value. err:" + ex.ToString()));
        }
      }
    }

    private void init_session_details()
    {
      try
      {
        this.current_device = (device_details) this.Session["device"];
        if (this.current_device != null)
        {
          if (!(this.current_device.device_code == Guid.Empty))
            goto label_4;
        }
        this.current_device = this.dapi.get_device(this.parameters["dcode"]);
        this.Session.Add("device", (object) this.current_device);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("null device in session: " + ex.ToString()));
        this.current_device = new device_details();
        this.current_device.device_code = Guid.Empty;
      }
label_4:
      try
      {
        this.current_account = (account) this.Session["account"];
        if (this.current_account == null)
        {
          this.current_account = this.uapi.get_account(this.current_device.account_id);
          if (this.current_account.account_id != Guid.Empty)
            this.Session.Add("account", (object) this.current_account);
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("account info:" + ex.ToString()));
        this.current_account = new account();
        this.current_account.account_id = Guid.Empty;
      }
      try
      {
        this.current_asset = (asset) this.Session["asset"];
        if (this.current_asset == null)
        {
          this.current_asset = new asset_api().get_asset(this.current_device.asset_id, this.current_device.account_id);
          this.Session.Add("asset", (object) this.current_asset);
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("asset info:" + ex.ToString()));
        this.current_asset = new asset();
        this.current_asset.asset_id = 0L;
      }
      try
      {
        this.asset_properties = (DataSet) this.Session["asset_properties"];
        if (this.asset_properties == null)
        {
          this.asset_properties = this.assets.get_asset_properties(this.current_asset.asset_id, this.current_asset.account_id);
          this.Session.Add("asset_properties", (object) this.asset_properties);
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("asset props info: " + ex.ToString()));
      }
      try
      {
        this.current_user = (skynapse.fbs.user) this.Session["user"];
        if (this.current_user == null)
        {
          this.current_user = new skynapse.fbs.user();
          this.current_user.user_id = 0L;
        }
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("user info: " + ex.ToString()));
        this.current_user = new skynapse.fbs.user();
        this.current_user.user_id = 0L;
      }
      try
      {
        this.settings_data = (DataSet) this.capi.get_cache(this.current_device.account_id.ToString() + "_rdp_settings");
        if (this.settings_data != null)
          return;
        this.settings_data = this.settings.get_settings(this.current_device.account_id);
        this.capi.set_cache(this.current_device.account_id.ToString() + "_rdp_settings", (object) this.settings_data);
      }
      catch (Exception ex)
      {
        this.log.Error((object) ("settings info: " + ex.ToString()));
      }
    }

    protected void clear_user_session()
    {
      this.current_user = (skynapse.fbs.user) null;
      this.Session.Remove("user");
    }

    protected void show_message(int message_id) => this.Response.Redirect("message.aspx?id=" + (object) message_id);

    protected void redirect_main() => this.Response.Redirect("default.aspx?dcode=" + (object) this.current_device.device_code);
  }
}
