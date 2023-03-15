// Decompiled with JetBrains decompiler
// Type: administration_log_delete
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using ASP;
using System;
using System.Configuration;
using System.IO;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public class administration_log_delete : Page, IRequiresSessionState
{
  protected HtmlForm form1;

  protected void Page_Load(object sender, EventArgs e) => this.delete_log();

  private void delete_log()
  {
    DirectoryInfo directoryInfo = new DirectoryInfo(ConfigurationManager.AppSettings["logFilepath"].ToString());
    int int32 = Convert.ToInt32(ConfigurationManager.AppSettings["log_delete_days"].ToString());
    foreach (FileInfo file in directoryInfo.GetFiles(ConfigurationManager.AppSettings["Filename1"].ToString()))
    {
      if (file.CreationTime < DateTime.Now.AddDays((double) -int32) || file.LastWriteTime < DateTime.Now.AddDays((double) -int32) || file.Length == 0L)
        file.Delete();
    }
    foreach (FileInfo file in directoryInfo.GetFiles(ConfigurationManager.AppSettings["Filename2"].ToString()))
    {
      if (file.CreationTime < DateTime.Now.AddDays((double) -int32) || file.LastWriteTime < DateTime.Now.AddDays((double) -int32) || file.Length == 0L)
        file.Delete();
    }
    foreach (FileInfo file in directoryInfo.GetFiles(ConfigurationManager.AppSettings["Filename3"].ToString()))
    {
      if (file.CreationTime < DateTime.Now.AddDays((double) -int32) || file.LastWriteTime < DateTime.Now.AddDays((double) -int32) || file.Length == 0L)
        file.Delete();
    }
    foreach (FileInfo file in directoryInfo.GetFiles(ConfigurationManager.AppSettings["Filename4"].ToString()))
    {
      if (file.CreationTime < DateTime.Now.AddDays((double) -int32) || file.LastWriteTime < DateTime.Now.AddDays((double) -int32) || file.Length == 0L)
        file.Delete();
    }
  }

  protected DefaultProfile Profile => (DefaultProfile) this.Context.Profile;

  protected global_asax ApplicationInstance => (global_asax) this.Context.ApplicationInstance;
}
