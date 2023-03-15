// Decompiled with JetBrains decompiler
// Type: administration_master_list
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Web.Profile;
using System.Web.SessionState;

public class administration_master_list : fbs_base_page, IRequiresSessionState
{
  public string room_name;
  public string successmsg = "";
  public string Mastername = "";
  public string name = "";
  protected controls_admin_master_list ucBuilding;
  protected controls_admin_master_list ucLevel;
  protected controls_admin_master_list ucCategories;
  protected controls_admin_master_list ucType;
  protected controls_admin_master_list ucSetups;
  protected controls_admin_master_list ucProperties;
  protected controls_admin_master_list ucMeetingType;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["master_list"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.master_view)
      this.redirect_unauthorized();
    try
    {
      this.Pageload_data();
      if (this.Session["MasetEdit"] == null || this.Session["MasetEdit"] != (object) "S")
        return;
      this.successmsg = "S";
      this.Session.Remove("MasetEdit");
      string[] strArray = this.Session["MasterTypeforsave"].ToString().Split('&');
      this.Mastername = strArray[0].ToString();
      string str = strArray[1];
      switch (this.Mastername)
      {
        case "building":
          this.name = "Building Master List";
          break;
        case "level":
          this.name = "Level Master List";
          break;
        case "category":
          this.name = "Category Master List";
          break;
        case "asset_type":
          this.name = "Asset Type Master List";
          break;
        case "setup_type":
          this.name = "Setup Type Master List";
          break;
        case "asset_property":
          this.name = "Asset Property Master List";
          break;
        case "meeting_type":
          this.name = "Meeting Types Master List";
          break;
      }
      this.Session.Remove("MasterTypeforsave");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void Pageload_data()
  {
    try
    {
      this.ucBuilding.Attributes.Add("filter", "building");
      this.ucBuilding.Attributes.Add("title", "Buildings");
      this.ucBuilding.isEditable = this.gp.master_edit;
      this.ucBuilding.isDeletable = this.gp.master_delete;
      this.ucLevel.Attributes.Add("filter", "level");
      this.ucLevel.Attributes.Add("title", "Levels");
      this.ucLevel.isEditable = this.gp.master_edit;
      this.ucLevel.isDeletable = this.gp.master_delete;
      this.ucCategories.Attributes.Add("filter", "category");
      this.ucCategories.Attributes.Add("title", "Categories");
      this.ucCategories.isEditable = this.gp.master_edit;
      this.ucCategories.isDeletable = this.gp.master_delete;
      this.ucType.Attributes.Add("filter", "asset_type");
      this.ucType.Attributes.Add("title", "Types");
      this.ucType.isEditable = this.gp.master_edit;
      this.ucType.isDeletable = this.gp.master_delete;
      this.ucSetups.Attributes.Add("filter", "setup_type");
      this.ucSetups.Attributes.Add("title", "Setups");
      this.ucSetups.isEditable = this.gp.master_edit;
      this.ucSetups.isDeletable = this.gp.master_delete;
      this.ucProperties.Attributes.Add("filter", "asset_property");
      this.ucProperties.Attributes.Add("title", "Properties");
      this.ucProperties.isEditable = this.gp.master_edit;
      this.ucProperties.isDeletable = this.gp.master_delete;
      this.ucMeetingType.Attributes.Add("filter", "meeting_type");
      this.ucMeetingType.Attributes.Add("title", "Meeting Types");
      this.ucMeetingType.isEditable = this.gp.master_edit;
      this.ucMeetingType.isDeletable = this.gp.master_delete;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
