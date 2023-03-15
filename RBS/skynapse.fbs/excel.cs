// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.excel
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;
using System.Data;

namespace skynapse.fbs
{
  public class excel
  {
    public string file_name;
    public string header;
    public DataSet data;
    public Dictionary<string, string> column_names;
    public string footer;
    public string table_identifier;
  }
}
