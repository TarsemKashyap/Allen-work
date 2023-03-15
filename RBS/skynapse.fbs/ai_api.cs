// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.ai_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace skynapse.fbs
{
  public class ai_api
  {
    private string url = "http://localhost:9000/?properties=%7B%22annotators%22%3A%20%22tokenize%2Cner%22%2C%20%22date%22%3A%20%22[year]-[month]-[day]T[hour]%3A[minute]%3A[second]%22%7D&pipelineLanguage=en";
    public static ILog log = LogManager.GetLogger("fbs_log");

    public RootObject get_data(string message)
    {
      RootObject data = new RootObject();
      string dataFromServer = this.get_data_from_server(message);
      if (dataFromServer != "")
      {
        StringBuilder stringBuilder = new StringBuilder();
        data = new ai_api().Deserialize(dataFromServer);
      }
      return data;
    }

    private string get_data_from_server(string text)
    {
      this.url = this.url.Replace("[year]", DateTime.Now.Year.ToString());
      this.url = this.url.Replace("[month]", DateTime.Now.Month.ToString());
      this.url = this.url.Replace("[day]", DateTime.Now.Day.ToString());
      this.url = this.url.Replace("[hour]", DateTime.Now.Hour.ToString());
      this.url = this.url.Replace("[minute]", DateTime.Now.Minute.ToString());
      this.url = this.url.Replace("[second]", DateTime.Now.Second.ToString());
      byte[] bytes = Encoding.ASCII.GetBytes(text);
      WebRequest webRequest = WebRequest.Create(this.url);
      webRequest.Method = "POST";
      webRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
      webRequest.ContentLength = (long) bytes.Length;
      using (Stream requestStream = webRequest.GetRequestStream())
        requestStream.Write(bytes, 0, bytes.Length);
      string end = new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd();
      ai_api.log.Info((object) ("<data><text>" + text + "</text><response>" + end + "</response></data>"));
      return end;
    }

    public RootObject Deserialize(string json) => JsonConvert.DeserializeObject<RootObject>(json);
  }
}
