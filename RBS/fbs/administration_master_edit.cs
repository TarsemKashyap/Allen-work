// Decompiled with JetBrains decompiler
// Type: administration_master_edit
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Xml;

public class administration_master_edit : fbs_base_page, IRequiresSessionState
{
  protected long settingid;
  public string type = "";
  public string Mater_type = "";
  public string Mater_type_details = "";
  public string Master_list_Add = "";
  public string Master_list_Add_href = "";
  protected Label lbl_duplicateerr;
  protected TextBox txt_name;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["master_list"]))
      this.Server.Transfer("~//unauthorized.aspx");
    try
    {
      string str = "";
      this.type = this.Request.QueryString["type"];
      switch (this.type)
      {
        case "building":
          str = "Building";
          break;
        case "level":
          str = "Level";
          break;
        case "category":
          str = "Category";
          break;
        case "asset_type":
          str = "Asset Type";
          break;
        case "setup_type":
          str = "Setup Type";
          break;
        case "asset_property":
          str = "Asset Property";
          break;
        case "meeting_type":
          str = "Meeting Type";
          break;
      }
      if (this.Request.QueryString["id"] != null & this.Request.QueryString["Type"] != null)
      {
        this.Mater_type_details = this.Request.QueryString["Type"];
        if (this.Request.QueryString["id"] != "0")
        {
          this.Mater_type = "Edit";
          this.Master_list_Add_href = "master_edit.aspx?id=" + this.Request.QueryString["id"] + "&Type=" + this.type + "&Action=E";
          this.Master_list_Add = str + " List Edit";
        }
        else
        {
          this.Master_list_Add_href = "master_edit.aspx?id=0&type=" + this.type;
          this.Master_list_Add = str + " List Add";
        }
      }
      else
        this.Master_list_Add_href = "master_edit.aspx?id=0&type=" + this.type;
      this.txt_name.Attributes.Add("placeholder", "Enter the " + str + " Name");
      this.btn_submit.Text = "Save " + str;
      if (this.IsPostBack)
        return;
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["Action"]))
      {
        if (this.Request.QueryString["Action"] == "D")
          this.Master_Edit("D");
        else
          this.Mater_type = "Add";
      }
      this.Pageload_data();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void Pageload_data()
  {
    try
    {
      if (!(this.Request.QueryString["id"] != null & this.Request.QueryString["Type"] != null))
        return;
      this.Mater_type_details = this.Request.QueryString["Type"];
      if (this.Request.QueryString["id"] != "0")
      {
        this.Mater_type = "Edit";
        this.settingid = Convert.ToInt64(this.Request.QueryString["id"].ToString().TrimEnd());
        this.txt_name.Text = this.settings.get_setting(this.settingid, this.current_user.account_id).value;
      }
      else
        this.Mater_type = "Add";
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
      bool flag = this.reportings.checknameavilablity_insertmasterdata(this.txt_name.Text, this.Request.QueryString["Type"].ToString(), Convert.ToInt64(this.Request.QueryString["id"]), this.current_user.account_id);
      if (this.Request.QueryString["id"].ToString() != "0")
      {
        if (flag)
        {
          this.Master_Edit(this.Request.QueryString["Action"].ToString());
          this.lbl_duplicateerr.Visible = false;
          this.lbl_duplicateerr.Text = "";
        }
        else
        {
          this.lbl_duplicateerr.Text = Resources.fbs.master_name;
          this.lbl_duplicateerr.Visible = true;
        }
      }
      else if (flag)
      {
        this.Master_Edit("New");
        this.lbl_duplicateerr.Text = "";
        this.lbl_duplicateerr.Visible = false;
      }
      else
      {
        this.lbl_duplicateerr.Text = Resources.fbs.master_name;
        this.lbl_duplicateerr.Visible = true;
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_cancel_Click(object sender, EventArgs e)
  {
    try
    {
      string str1 = "";
      string str2 = "";
      switch (this.Request.QueryString["Type"].ToString())
      {
        case "building":
          str1 = "tab_buildings";
          str2 = "Buildings";
          break;
        case "level":
          str1 = "tab_levels";
          str2 = "Levels";
          break;
        case "category":
          str1 = "tab_categories";
          str2 = "Categories";
          break;
        case "asset_type":
          str1 = "tab_types";
          str2 = "Types";
          break;
        case "setup_type":
          str1 = "tab_setups";
          str2 = "Setups";
          break;
        case "asset_property":
          str1 = "tab_properties";
          str2 = "Asset Properties";
          break;
        case "meeting_type":
          str1 = "tab_meeting_types";
          str2 = "Meeting Types";
          break;
      }
      this.Session.Add("Master_current_tab", (object) new List<string>()
      {
        str2,
        str1
      });
      this.Response.Redirect("~/administration/master_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void Master_Edit(string Type)
  {
    string str1 = "";
    string str2 = "";
    try
    {
      this.settingid = Convert.ToInt64(this.Request.QueryString["id"].ToString().TrimEnd());
      setting setting1 = new setting();
      setting setting2 = this.settings.get_setting(this.settingid, this.current_user.account_id);
      Guid.Parse(setting2.record_id.ToString());
      Guid guid = new Guid(setting2.record_id.ToString());
      setting1.setting_id = this.settingid;
      setting1.account_id = this.current_user.account_id;
      setting1.modified_by = this.current_user.user_id;
      setting1.record_id = guid;
      switch (Type)
      {
        case "E":
        case "New":
          setting1.value = this.txt_name.Text;
          setting1.created_by = this.current_user.user_id;
          setting1.created_on = this.current_timestamp;
          setting1.modified_on = this.current_timestamp;
          setting1.parameter = this.Request.QueryString[nameof (Type)].ToString();
          setting1.properties = new XmlDocument();
          setting1.properties.LoadXml(this.set_properties());
          setting1.status = 1;
          this.settings.update_setting(setting1);
          break;
        case "D":
          this.settings.delete_setting(setting1);
          break;
      }
      this.Session["MasetEdit"] = (object) "S";
      this.Session["MasterTypeforsave"] = (object) (this.Request.QueryString[nameof (Type)].ToString() + "&" + Type);
      switch (this.Request.QueryString[nameof (Type)].ToString())
      {
        case "building":
          str1 = "tab_buildings";
          str2 = "Buildings";
          break;
        case "level":
          str1 = "tab_levels";
          str2 = "Levels";
          break;
        case "category":
          str1 = "tab_categories";
          str2 = "Categories";
          break;
        case "asset_type":
          str1 = "tab_types";
          str2 = "Types";
          break;
        case "setup_type":
          str1 = "tab_setups";
          str2 = "Setups";
          break;
        case "asset_property":
          str1 = "tab_properties";
          str2 = "Asset Properties";
          break;
        case "meeting_type":
          str1 = "tab_meeting_types";
          str2 = "Meeting Types";
          break;
      }
      this.Session.Add("Master_current_tab", (object) new List<string>()
      {
        str2,
        str1
      });
      this.Response.Redirect("~/administration/master_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private string set_properties() => "<properties></properties>";

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
