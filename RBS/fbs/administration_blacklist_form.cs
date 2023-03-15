// Decompiled with JetBrains decompiler
// Type: administration_blacklist_form
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Data;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class administration_blacklist_form : fbs_base_page, IRequiresSessionState
{
  private long blacklist_id;
  private string temp_acountid;
  public string sucess_msg = "0";
  protected DropDownList ddl_user;
  protected CheckBox chk_block;
  protected TextBox txtFromDate;
  protected TextBox txtToDate;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["blacklist_user"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (!this.gp.users_blacklist)
      this.redirect_unauthorized();
    this.sucess_msg = !this.gp.isAdminType ? "2" : "1";
    try
    {
      if (!this.IsPostBack)
      {
        if (this.Request.QueryString["User_id"] != null)
          this.populate_uiedit();
        else if (this.Request.QueryString["blacklist_id"] != null)
          this.populate_uiedit();
        else
          this.Getusers();
      }
      this.txtFromDate.Attributes.Add("readonly", "readonly");
      this.txtToDate.Attributes.Add("readonly", "readonly");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void populate_uiedit()
  {
    try
    {
      if (this.Request.QueryString["User_id"] != null)
      {
        blacklist blacklist = this.blapi.get_blacklist(Convert.ToInt64(this.Request.QueryString["User_id"]), this.current_user.account_id);
        DataSet usersnameonly = this.reportings.get_usersnameonly(Convert.ToInt64(this.Request.QueryString["User_id"].ToString()), Convert.ToString((object) this.current_user.account_id));
        if (usersnameonly.Tables[0].Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) usersnameonly.Tables[0].Rows)
          {
            if (row["user_id"].ToString() == this.Request.QueryString["User_id"])
            {
              this.ddl_user.Items.Add(new ListItem(row["username"].ToString(), Convert.ToString(row["user_id"].ToString())));
              this.ddl_user.Visible = true;
            }
          }
        }
        if (blacklist.blacklist_id <= 0L)
          return;
        this.chk_block.Checked = blacklist.blacklist_type == (short) 1;
        this.txtFromDate.Text = blacklist.from_date.ToString(api_constants.display_datetime_format);
        if (blacklist.to_date.ToString() != "1/1/0001 12:00:00 AM")
          this.txtToDate.Text = blacklist.to_date.ToString(api_constants.display_datetime_format);
        else
          this.txtToDate.Text = "";
      }
      else
      {
        this.blacklist_id = Convert.ToInt64(this.Request.QueryString["blacklist_id"].ToString());
        blacklist blacklist = this.blapi.get_blacklist(this.blacklist_id, this.current_user.account_id);
        this.chk_block.Checked = blacklist.blacklist_type == (short) 1;
        this.txtFromDate.Text = blacklist.from_date.ToString(api_constants.display_datetime_format);
        this.txtToDate.Text = !(blacklist.to_date.ToString() != "1/1/0001 12:00:00 AM") ? "" : blacklist.to_date.ToString(api_constants.display_datetime_format);
        DataSet user = this.reportings.get_user(blacklist.user_id, this.current_user.account_id);
        if (user.Tables[0].Rows.Count <= 0)
          return;
        foreach (DataRow row in (InternalDataCollectionBase) user.Tables[0].Rows)
        {
          this.ddl_user.Items.Add(new ListItem(row["email"].ToString(), row["user_id"].ToString()));
          this.ddl_user.Visible = true;
        }
      }
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  public void Getusers()
  {
    try
    {
      string str = "";
      if (this.Request.QueryString["user_id"] != null)
        str = this.Request.QueryString["user_id"].ToString();
      DataSet userNotInBlacklist = this.blapi.get_User_not_in_blacklist(this.current_user.account_id);
      if (str == "")
      {
        foreach (DataRow row in (InternalDataCollectionBase) userNotInBlacklist.Tables[0].Rows)
          this.ddl_user.Items.Add(new ListItem(row["username"].ToString(), row["user_id"].ToString()));
      }
      else
      {
        foreach (DataRow row in (InternalDataCollectionBase) userNotInBlacklist.Tables[0].Rows)
        {
          if (row["user_id"].ToString() == str)
            this.ddl_user.Items.Add(new ListItem(row["username"].ToString(), row["user_id"].ToString()));
        }
      }
      this.ddl_user.Visible = true;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected void btn_submit_Click1(object sender, EventArgs e)
  {
    try
    {
      if (this.Request.QueryString["User_id"] != null)
        this.BlockList_Action_byuserID();
      else
        this.Blocklist_Action();
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void BlockList_Action_byuserID()
  {
    try
    {
      blacklist blacklist1 = new blacklist();
      blacklist blacklist2 = this.blapi.get_blacklist(Convert.ToInt64(this.Request.QueryString["User_id"]), this.current_user.account_id);
      this.users.get_user(Convert.ToInt64(this.Request.QueryString["User_id"].ToString()), this.current_user.account_id);
      this.blacklist_id = blacklist2.blacklist_id == 0L ? 0L : blacklist2.blacklist_id;
      blacklist2.account_id = this.current_user.account_id;
      blacklist2.created_on = this.current_timestamp;
      blacklist2.created_by = this.current_user.user_id;
      blacklist2.modified_on = this.current_timestamp;
      blacklist2.user_id = Convert.ToInt64(this.Request.QueryString["User_id"].ToString());
      blacklist2.blacklist_id = this.blacklist_id;
      blacklist2.record_id = Guid.NewGuid();
      short num = !this.chk_block.Checked ? (short) 0 : (short) 1;
      blacklist2.from_date = !(this.txtFromDate.Text != "") ? this.current_timestamp : Convert.ToDateTime(this.txtFromDate.Text);
      blacklist2.to_date = !(this.txtToDate.Text != "") ? this.current_timestamp : Convert.ToDateTime(this.txtToDate.Text);
      blacklist2.blacklist_type = num;
      this.blapi.update_blacklist(blacklist2);
      this.capi.remove_cache("bl");
      this.Session["blcklistupd"] = (object) "S";
      this.Response.Redirect("~/administration/users_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  private void Blocklist_Action()
  {
    try
    {
      blacklist blacklist = new blacklist();
      if (this.Request.QueryString["blacklist_id"] != null)
      {
        this.blacklist_id = Convert.ToInt64(this.Request.QueryString["blacklist_id"].ToString());
        blacklist = this.blapi.get_blacklist(this.blacklist_id, this.current_user.account_id);
        blacklist.user_id = Convert.ToInt64(this.ddl_user.SelectedValue);
      }
      else
      {
        this.blacklist_id = 0L;
        blacklist.user_id = Convert.ToInt64(this.ddl_user.SelectedValue);
        blacklist.account_id = this.current_user.account_id;
        blacklist.created_on = this.current_timestamp;
        blacklist.created_by = this.current_user.user_id;
        blacklist.modified_on = this.current_timestamp;
        blacklist.blacklist_id = this.blacklist_id;
        blacklist.record_id = Guid.NewGuid();
      }
      short num = !this.chk_block.Checked ? (short) 0 : (short) 1;
      blacklist.from_date = !(this.txtFromDate.Text != "") ? this.current_timestamp : Convert.ToDateTime(this.txtFromDate.Text);
      blacklist.to_date = !(this.txtToDate.Text != "") ? this.current_timestamp : Convert.ToDateTime(this.txtToDate.Text);
      blacklist.blacklist_type = num;
      this.blapi.update_blacklist(blacklist);
      this.capi.remove_cache("bl");
      this.Session["Blacklist"] = (object) "S";
      this.Response.Redirect("~/administration/blacklist_list.aspx");
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
      if (this.Request.QueryString["User_id"] != null)
        this.Response.Redirect("~/administration/users_list.aspx");
      else
        this.Response.Redirect("~/administration/blacklist_list.aspx");
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error -> ", ex);
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
