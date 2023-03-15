// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.request
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;

namespace skynapse.fbs
{
  public class request : core_class
  {
    public long request_id;
    public string request_ref_no;
    public string requestor_name;
    public string dept;
    public string division;
    public string section;
    public string costcenter;
    public string telephone;
    public string email;
    public short status;
    public string description;
    public string hire_name;
    public string hire_dept;
    public string hire_division;
    public string hire_section;
    public string hire_designation;
    public DateTime requestdate;
    public string responsetext;
    public DateTime responsedate;
    public string entilement;
    public long responseby;
    public string remarks;
  }
}
