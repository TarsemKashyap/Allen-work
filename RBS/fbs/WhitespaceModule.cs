// Decompiled with JetBrains decompiler
// Type: WhitespaceModule
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.IO;
using System.Web;

public class WhitespaceModule : IHttpModule
{
  void IHttpModule.Dispose()
  {
  }

  void IHttpModule.Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.context_BeginRequest);

  private void context_BeginRequest(object sender, EventArgs e)
  {
    HttpApplication httpApplication = sender as HttpApplication;
    if (!httpApplication.Request.RawUrl.Contains(".aspx"))
      return;
    httpApplication.Response.Filter = (Stream) new WhitespaceFilter(httpApplication.Response.Filter);
  }
}
