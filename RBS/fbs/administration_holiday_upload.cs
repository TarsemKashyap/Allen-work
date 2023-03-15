// Decompiled with JetBrains decompiler
// Type: administration_holiday_upload
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using Excel;
using skynapse.fbs;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public class administration_holiday_upload : fbs_base_page, IRequiresSessionState
{
  protected Literal ltlErrorSummary;
  protected HtmlGenericControl divErrorSummary;
  protected FileUpload upload_holiday;
  protected Button btn_submit;
  protected Button btn_cancel;

  protected new void Page_Load(object sender, EventArgs e)
  {
    if (!Convert.ToBoolean(this.current_account.properties["holidays"]))
      this.Server.Transfer("~//unauthorized.aspx");
    if (this.gp.holidays_view && this.gp.holidays_upload)
      return;
    this.redirect_unauthorized();
  }

  protected void btn_submit_Click(object sender, EventArgs e)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    try
    {
      if (this.upload_holiday.HasFile)
      {
        this.upload_holiday.SaveAs(this.Server.MapPath("/holidayfiles/" + this.upload_holiday.FileName));
        DataSet dataSet1 = new DataSet();
        DataSet datasetFromExcel = this.get_dataset_from_excel(this.upload_holiday.FileName);
        string extension = Path.GetExtension(this.upload_holiday.FileName);
        if (extension == ".xls" || extension == ".xlsx")
        {
          if (datasetFromExcel != null && datasetFromExcel.Tables.Count >= 1)
          {
            if (datasetFromExcel.Tables != null)
            {
              int count1 = datasetFromExcel.Tables[0].Rows.Count;
              int count2 = datasetFromExcel.Tables[0].Columns.Count;
              if (count1 > 1)
              {
                if (count2 == 4)
                {
                  int num = 1;
                  for (int index = 1; index <= datasetFromExcel.Tables[0].Rows.Count - 1; ++index)
                  {
                    bool flag = true;
                    string str1 = datasetFromExcel.Tables[0].Rows[index][0].ToString();
                    string strdate1 = datasetFromExcel.Tables[0].Rows[index][1].ToString();
                    string strdate2 = datasetFromExcel.Tables[0].Rows[index][2].ToString();
                    string str2 = datasetFromExcel.Tables[0].Rows[index][3].ToString();
                    StringBuilder stringBuilder2 = new StringBuilder();
                    if (str1 != "" && strdate1 != "" && strdate2 != "" && str2 != "")
                    {
                      if (str1 == "")
                        stringBuilder2.Append("<li>" + Resources.fbs.holidya_name_cannotblank + "</li>");
                      if (!this.isDate(strdate1))
                      {
                        flag = false;
                        stringBuilder2.Append("<li>" + Resources.fbs.holiday_startdate_fomrat_invalid + "</li>");
                      }
                      if (!this.isDate(strdate2))
                      {
                        flag = false;
                        stringBuilder2.Append("<li>" + Resources.fbs.holiday_enddate_fomrat_invalid + "</li>");
                      }
                      DateTime dateTime1;
                      if (this.isDate(strdate1) && this.isDate(strdate2))
                      {
                        dateTime1 = Convert.ToDateTime(Convert.ToDateTime(strdate1).ToString("dd-MMM-yyy") + " 12:00:00 AM");
                        DateTime dateTime2 = Convert.ToDateTime(Convert.ToDateTime(strdate2).ToString("dd-MMM-yyy") + " 11:59:59 PM");
                        if (dateTime1 > dateTime2)
                          stringBuilder2.Append("<li>" + Resources.fbs.holiday_file_format + "</li>");
                      }
                      if (str2 != "N" && str2 != "Y")
                        stringBuilder2.Append("<li>" + Resources.fbs.holiday_repat_notcorrect + "</li>");
                      DataRow[] dataRowArray1 = datasetFromExcel.Tables[0].Select("Column1='" + str1.Replace("'", "''") + "' AND Column4='Y'");
                      DataRow[] dataRowArray2 = datasetFromExcel.Tables[0].Select("Column1='" + str1.Replace("'", "''") + "' AND Column4='N'");
                      if (dataRowArray1.Length > 1 || dataRowArray1.Length == 1 && dataRowArray2.Length == 1)
                        stringBuilder2.Append("<li>" + Resources.fbs.holiday_name_check_insheet + "</li>");
                      if (str1 != "" && flag)
                      {
                        dateTime1 = Convert.ToDateTime(Convert.ToDateTime(strdate1).ToString("dd-MMM-yyy") + " 12:00:00 AM");
                        DateTime dateTime3 = Convert.ToDateTime(Convert.ToDateTime(strdate2).ToString("dd-MMM-yyy") + " 11:59:59 PM");
                        DataSet dataSet2 = new DataSet();
                        if (this.utilities.isValidDataset(this.holidays.check_already_holidays_name(str1.Trim(), dateTime1.Year.ToString(), this.current_user.account_id, dateTime1, dateTime3)))
                        {
                          stringBuilder2.Append("<li>" + Resources.fbs.holiday_name_already_exsit + "</li>");
                        }
                        else
                        {
                          DataSet ds = this.holidays.check_already_holidays_Date(str1.Trim(), dateTime1.Year.ToString(), this.current_user.account_id, dateTime1, dateTime3);
                          if (this.utilities.isValidDataset(ds))
                          {
                            foreach (DataRow row in (InternalDataCollectionBase) ds.Tables[0].Rows)
                            {
                              if (row["holiday_name"].ToString() == str1)
                                stringBuilder2.Append("<li>" + Resources.fbs.holiday_name_already_exsit + "</li>");
                              else
                                stringBuilder2.Append("<li>" + Resources.fbs.holiday_Date_already_exsit + "</li>");
                            }
                          }
                        }
                      }
                      if (stringBuilder2.ToString() != "")
                        stringBuilder1.AppendFormat("<ul>RowNo:<b>{0}</b>{1}</ul>", (object) num.ToString(), (object) stringBuilder2.ToString());
                      else
                        this.insertdata(datasetFromExcel.Tables[0].Rows[index]);
                      ++num;
                    }
                  }
                  if (stringBuilder1.ToString() != "")
                  {
                    this.ltlErrorSummary.Text = stringBuilder1.ToString();
                  }
                  else
                  {
                    this.Session["holiday"] = (object) "S";
                    this.Response.Redirect("holiday_list.aspx", true);
                  }
                }
                else
                  this.ltlErrorSummary.Text = Resources.fbs.holiday_fourcol_reuqired;
              }
              else if (count1 < 1)
                this.ltlErrorSummary.Text = "No Header and records Found..";
              else
                this.ltlErrorSummary.Text = "No Records Found..";
            }
            else
              this.ltlErrorSummary.Text = "No Records Found..";
          }
          else
            this.ltlErrorSummary.Text = Resources.fbs.holiday_nodata_file;
        }
        else
          this.ltlErrorSummary.Text = Resources.fbs.holiday_file_format;
      }
      else
        this.ltlErrorSummary.Text = Resources.fbs.holiday_no_file;
    }
    catch (Exception ex)
    {
      fbs_base_page.log.Error((object) "Error ->", ex);
    }
  }

  private bool isDate(string strdate)
  {
    try
    {
      Convert.ToDateTime(strdate);
      return true;
    }
    catch
    {
      return false;
    }
  }

  private void insertdata(DataRow dr)
  {
    string str1 = dr[0].ToString().Trim();
    string str2 = dr[1].ToString().Trim();
    string str3 = dr[2].ToString().Trim();
    string str4 = dr[3].ToString().Trim();
    DateTime dateTime1 = Convert.ToDateTime(str2);
    DateTime dateTime2 = Convert.ToDateTime(str3);
    holiday holiday = new holiday();
    holiday.account_id = this.current_user.account_id;
    holiday.created_by = this.current_user.user_id;
    holiday.created_on = this.current_timestamp;
    holiday.holiday_id = 0L;
    holiday.holiday_name = str1;
    holiday.from_date = dateTime1;
    holiday.modified_by = this.current_user.user_id;
    holiday.modified_on = this.current_timestamp;
    holiday.record_id = Guid.NewGuid();
    holiday.repeat = str4.ToLower() == "y";
    holiday.to_date = dateTime2;
    this.holidays.update_holiday(holiday);
  }

  private DataSet get_dataset_from_excel(string fname)
  {
    DataSet datasetFromExcel = new DataSet();
    Stream fileContent = this.upload_holiday.FileContent;
    if (new FileInfo(fname).Extension == ".xls")
    {
      try
      {
        IExcelDataReader binaryReader = ExcelReaderFactory.CreateBinaryReader(fileContent);
        binaryReader.IsFirstRowAsColumnNames = false;
        datasetFromExcel = binaryReader.AsDataSet(true);
      }
      catch (Exception ex1)
      {
        fbs_base_page.log.Error((object) "Error ->", ex1);
        try
        {
          IExcelDataReader openXmlReader = ExcelReaderFactory.CreateOpenXmlReader(fileContent);
          openXmlReader.IsFirstRowAsColumnNames = false;
          datasetFromExcel = openXmlReader.AsDataSet(true);
        }
        catch (Exception ex2)
        {
          fbs_base_page.log.Error((object) "Error -> ", ex2);
        }
      }
    }
    else
    {
      try
      {
        IExcelDataReader openXmlReader = ExcelReaderFactory.CreateOpenXmlReader(fileContent);
        openXmlReader.IsFirstRowAsColumnNames = false;
        datasetFromExcel = openXmlReader.AsDataSet(true);
      }
      catch (Exception ex3)
      {
        fbs_base_page.log.Error((object) "Error ->", ex3);
        try
        {
          IExcelDataReader binaryReader = ExcelReaderFactory.CreateBinaryReader(fileContent);
          binaryReader.IsFirstRowAsColumnNames = false;
          datasetFromExcel = binaryReader.AsDataSet(true);
        }
        catch (Exception ex4)
        {
          fbs_base_page.log.Error((object) "Error -> ", ex4);
        }
      }
    }
    return datasetFromExcel;
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
