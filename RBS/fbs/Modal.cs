// Decompiled with JetBrains decompiler
// Type: Modal
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using DayPilot.Web.Ui.Json;
using System.Text;
using System.Web.UI;

public class Modal
{
  public static void Close(Page page) => Modal.Close(page, (object) null);

  public static string Script(object result)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<script type='text/javascript'>");
    stringBuilder.Append("if (parent && parent.DayPilot && parent.DayPilot.ModalStatic) {");
    stringBuilder.Append("parent.DayPilot.ModalStatic.close(" + SimpleJsonSerializer.Serialize(result) + ");");
    stringBuilder.Append("}");
    stringBuilder.Append("</script>");
    return stringBuilder.ToString();
  }

  public static void Close(Page page, object result) => page.ClientScript.RegisterStartupScript(typeof (Page), "close", Modal.Script(result));
}
