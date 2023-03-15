// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.excel_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Data;
using System.Text;

namespace skynapse.fbs
{
  public class excel_api : api_base
  {
    public string get_excel(excel obj)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("<table class='" + obj.table_identifier + "_table'>");
        stringBuilder.Append("<tr><td colspan='" + (object) obj.column_names.Count + "' class='" + obj.table_identifier + "_header'>" + obj.header + "</td></tr>");
        stringBuilder.Append("<tr><td></td></tr><tr>");
        foreach (string key in obj.column_names.Keys)
          stringBuilder.Append("<td class='" + obj.table_identifier + "_header_cell' style='text-align:centre;'>" + obj.column_names[key] + "</td>");
        stringBuilder.Append("</tr>");
        if (obj.data.Tables[0].Rows.Count == 0)
        {
          stringBuilder.Append("<tr><td colspan='" + (object) obj.column_names.Count + "' class='" + obj.table_identifier + "_norecords'>No records found</td></tr>");
        }
        else
        {
          foreach (DataRow row in (InternalDataCollectionBase) obj.data.Tables[0].Rows)
          {
            stringBuilder.Append("<tr>");
            foreach (DataColumn column in (InternalDataCollectionBase) obj.data.Tables[0].Columns)
            {
              if (obj.column_names.ContainsKey(column.ColumnName))
                stringBuilder.Append("<td class='" + obj.table_identifier + "_row_cell' style='text-align:centre;'>" + row[column.ColumnName].ToString() + "</td>");
            }
            stringBuilder.Append("</tr>");
          }
        }
        stringBuilder.Append("<tr><td></td></tr><tr><td colspan='" + (object) obj.column_names.Count + "' class='" + obj.table_identifier + "_footer'>" + obj.footer + "</td></tr>");
        stringBuilder.Append("</table>");
      }
      catch (Exception ex)
      {
        this.log.Error((object) "Error -> ", ex);
      }
      return stringBuilder.ToString();
    }
  }
}
