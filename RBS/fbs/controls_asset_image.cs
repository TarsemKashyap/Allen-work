// Decompiled with JetBrains decompiler
// Type: controls_asset_image
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using skynapse.fbs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Profile;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class controls_asset_image : fbs_base_user_control
{
  protected Literal lblError;
  protected HtmlGenericControl alertInfo;
  protected Literal lit_msg;
  protected System.Web.UI.WebControls.Image facilityImg;
  protected FileUpload upload_image;
  protected Button btn_remove;
  protected Button btn_submit;
  protected Label lblError_;
  public DataSet settings_value;
  public asset _objAsset;

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;

  public asset objAsset
  {
    get => this._objAsset;
    set => this._objAsset = value;
  }

  protected new void Page_Load(object sender, EventArgs e)
  {
    try
    {
      if (this.Session["user"] == null)
        this.Response.Redirect("../error.aspx?message=not_authorized");
      if (this.IsPostBack)
        return;
      this.settings_value = this.settings.view_settings(this.current_user.account_id);
      int int32_1 = Convert.ToInt32(this.settings_value.Tables[0].Select("parameter='image_width'")[0]["value"].ToString());
      int int32_2 = Convert.ToInt32(this.settings_value.Tables[0].Select("parameter='image_height'")[0]["value"].ToString());
      int int32_3 = Convert.ToInt32(this.settings_value.Tables[0].Select("parameter='upload_image_size'")[0]["value"].ToString());
      string str = this.settings_value.Tables[0].Select("parameter='upload_image_type'")[0]["value"].ToString();
      int num = int32_3 / 1024;
      this.lit_msg.Text = "<b>" + ("Uploaded image should be less than " + (object) int32_1 + "px in width and " + (object) int32_2 + "px in height. Maximum allowed size is " + (object) num + " Kb. The following types are allowed:" + str + ".") + "</b>";
      if (string.IsNullOrWhiteSpace(this.Request.QueryString["asset_id"]))
        return;
      if (this.Request.QueryString["asset_id"] != "0")
      {
        foreach (long key1 in this.objAsset.asset_properties.Keys)
        {
          asset_property assetProperty = this.objAsset.asset_properties[key1];
          if (assetProperty.property_name == "facility_image" && assetProperty.property_value != "")
          {
            long int64 = Convert.ToInt64(assetProperty.property_value);
            foreach (long key2 in this.objAsset.documents.Keys)
            {
              asset_document document = this.objAsset.documents[key2];
              if (document.document_id == int64)
              {
                byte[] binaryData = document.binary_data;
                this.facilityImg.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(binaryData, 0, binaryData.Length);
              }
            }
          }
        }
      }
      if (this.Session["invalid_image_size"] == null || string.IsNullOrWhiteSpace(this.Session["invalid_image_size"].ToString()))
        return;
      this.lblError.Visible = true;
      this.alertInfo.Visible = true;
      this.lblError.Text = Resources.fbs.asset_image_aboutimage.Replace("['W']", this.Session["W"].ToString()).Replace("['H']", this.Session["H"].ToString()).Replace("['SZ']", this.Session["SZ"].ToString());
      this.Session.Remove("invalid_image_size");
      this.Session.Remove("W");
      this.Session.Remove("H");
      this.Session.Remove("SZ");
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error ->", ex);
    }
  }

  public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
  {
    MemoryStream memoryStream = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
    memoryStream.Write(byteArrayIn, 0, byteArrayIn.Length);
    return System.Drawing.Image.FromStream((Stream) memoryStream, true);
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    try
    {
      if (!this.upload_image.HasFile)
        return;
      int contentLength = this.upload_image.PostedFile.ContentLength;
      byte[] numArray = new byte[contentLength];
      byte[] fileBytes = this.upload_image.FileBytes;
      System.Drawing.Image image = this.byteArrayToImage(fileBytes);
      DataSet dataSet = this.settings.view_settings(this.current_user.account_id);
      int int32_1 = Convert.ToInt32(dataSet.Tables[0].Select("parameter='image_width'")[0]["value"].ToString());
      int int32_2 = Convert.ToInt32(dataSet.Tables[0].Select("parameter='image_height'")[0]["value"].ToString());
      int int32_3 = Convert.ToInt32(dataSet.Tables[0].Select("parameter='upload_image_size'")[0]["value"].ToString());
      this.Session.Add("current_tab", (object) new List<string>()
      {
        "Room Image",
        "tab_image"
      });
      if (image.Width <= int32_1 && image.Height <= int32_2 && contentLength <= int32_3)
      {
        long num1 = 0;
        long num2 = 0;
        asset_document assetDocument1 = new asset_document();
        asset_property assetProperty1 = new asset_property();
        foreach (long key in this.objAsset.asset_properties.Keys)
        {
          asset_property assetProperty2 = this.objAsset.asset_properties[key];
          if (assetProperty2.property_name == "facility_image")
          {
            assetProperty1 = assetProperty2;
            num1 = assetProperty2.asset_property_id;
            if (assetProperty2.property_value != "")
            {
              long int64 = Convert.ToInt64(assetProperty2.property_value);
              using (Dictionary<long, asset_document>.KeyCollection.Enumerator enumerator = this.objAsset.documents.Keys.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  asset_document document = this.objAsset.documents[enumerator.Current];
                  if (document.document_id == int64)
                  {
                    assetDocument1 = document;
                    num2 = int64;
                    break;
                  }
                }
                break;
              }
            }
            else
              break;
          }
        }
        if (assetDocument1.document_id > 0L)
          assetDocument1 = this.assets.delete_document(assetDocument1);
        assetDocument1.asset_id = Convert.ToInt64(this.Request.QueryString["asset_id"]);
        assetDocument1.document_id = 0L;
        assetDocument1.account_id = this.current_user.account_id;
        assetDocument1.created_by = this.current_user.user_id;
        assetDocument1.modified_by = this.current_user.user_id;
        assetDocument1.document_name = this.upload_image.FileName;
        assetDocument1.document_size = contentLength;
        assetDocument1.document_type = this.upload_image.PostedFile.ContentType;
        assetDocument1.document_meta = "";
        if (num2 == 0L)
          assetDocument1.record_id = this.objAsset.record_id;
        assetDocument1.binary_data = fileBytes;
        asset_document assetDocument2 = this.assets.update_document(assetDocument1);
        if (assetDocument2.document_id <= 0L)
          return;
        assetProperty1.asset_property_id = num1;
        assetProperty1.asset_id = assetDocument2.asset_id;
        assetProperty1.account_id = this.current_user.account_id;
        assetProperty1.created_by = this.current_user.user_id;
        assetProperty1.modified_by = this.current_user.user_id;
        assetProperty1.property_name = "facility_image";
        assetProperty1.property_value = assetDocument2.document_id.ToString();
        if (num1 == 0L)
          assetProperty1.record_id = this.objAsset.record_id;
        assetProperty1.remarks = "";
        assetProperty1.status = (short) 1;
        assetProperty1.available = true;
        this.assets.update_asset_property(assetProperty1);
        this.facilityImg.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(fileBytes, 0, fileBytes.Length);
        this.Response.Redirect("asset_form.aspx?asset_id=" + assetDocument2.asset_id.ToString() + "#tab_image");
      }
      else
      {
        this.Session["invalid_image_size"] = (object) "1";
        this.Session["H"] = (object) int32_2;
        this.Session["W"] = (object) int32_1;
        this.Session["SZ"] = (object) (Convert.ToInt32(int32_3) / 1024);
        this.Response.Redirect("asset_form.aspx?asset_id=" + this.Request.QueryString["asset_id"] + "#tab_image");
      }
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error ->", ex);
    }
  }

  protected void btn_remove_Click(object sender, EventArgs e)
  {
    try
    {
      asset_document assetDocument = new asset_document();
      asset_property assetProperty1 = new asset_property();
      foreach (long key in this.objAsset.asset_properties.Keys)
      {
        asset_property assetProperty2 = this.objAsset.asset_properties[key];
        if (assetProperty2.property_name == "facility_image")
        {
          assetProperty1 = assetProperty2;
          long int64 = Convert.ToInt64(assetProperty2.property_value);
          using (Dictionary<long, asset_document>.KeyCollection.Enumerator enumerator = this.objAsset.documents.Keys.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              asset_document document = this.objAsset.documents[enumerator.Current];
              if (document.document_id == int64)
              {
                assetDocument = document;
                break;
              }
            }
            break;
          }
        }
      }
      long assetId = this.objAsset.asset_id;
      if (assetProperty1 == null)
        return;
      this.assets.delete_asset_property(assetProperty1);
      if (assetDocument != null)
        this.assets.delete_document(assetDocument);
      this.facilityImg.ImageUrl = "~/assets/img/noimage.gif";
      this.Response.Redirect("asset_form.aspx?asset_id=" + assetId.ToString() + "#tab_image", true);
    }
    catch (Exception ex)
    {
      this.log.Error((object) "Error ->", ex);
    }
  }
}
