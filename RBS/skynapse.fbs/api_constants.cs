// Decompiled with JetBrains decompiler
// Type: api_constants
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;

public static class api_constants
{
  public static string sql_datetime_format = "yyyy-MM-dd hh:mm:ss tt";
  public static string sql_datetime_format_short = "yyyy-MM-dd";
  public static string display_datetime_format = "dd-MMM-yyyy hh:mm tt";
  public static string display_datetime_format_short = "dd-MMM-yyyy";
  public static string all_users_text = "All Users";
  public static string datetime_format = "yyyy-MM-dd hh:mm:ss tt";
  public static Dictionary<string, int> booking_status = new Dictionary<string, int>()
  {
    {
      "Auto Rejected",
      7
    },
    {
      "Blocked",
      2
    },
    {
      "Booked",
      1
    },
    {
      "Cancelled",
      0
    },
    {
      "No Show",
      3
    },
    {
      "Pending",
      4
    },
    {
      "Rejected",
      6
    },
    {
      "Withdraw",
      5
    }
  };
  public static Dictionary<string, short> workflow_action_status = new Dictionary<string, short>()
  {
    {
      "Pending",
      (short) 0
    },
    {
      "Approved",
      (short) 1
    },
    {
      "Rejected",
      (short) 2
    },
    {
      "Withdraw",
      (short) 3
    }
  };
  public static Dictionary<string, short> workflow_action_type = new Dictionary<string, short>()
  {
    {
      "New_Booking",
      (short) 1
    },
    {
      "Transfer_Group",
      (short) 2
    },
    {
      "Transfer_User",
      (short) 3
    }
  };

  public enum book_status : short
  {
    Cancelled,
    Booked,
    Blocked,
    NoShow,
    Pending,
    Withdraw,
    Rejected,
    AutoReject,
  }

  public enum booking_type : short
  {
    Quick = 1,
    Wizard = 2,
    Repeat = 3,
    Custom = 4,
    Clone = 5,
    Display = 6,
    QR = 7,
    Mobile = 8,
    Exchange = 9,
    Outlook = 10, // 0x000A
    Others = 11, // 0x000B
  }

  public enum status : short
  {
    inactive,
    active,
  }

  public enum group_type
  {
    all_users,
    administrator,
    super_user,
    requestor,
  }
}
