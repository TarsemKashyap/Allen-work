using CsvHelper.Configuration.Attributes;

public class CoveringDutyDto
{

    [Index(0)]
    public string UserId { get; set; }

    [Index(1)]
    public string SOEID { get; set; }

    [Index(2)]
    public string UserName { get; set; }
    
    [Index(3)]
    public string DelegateUserID { get; set; }

    [Index(4)]
    public string DelegateSOEID { get; set; }

    [Index(5)]
    public string DelegateUserName { get; set; }

    [Index(6)]
    public string BeginDate { get; set; }

    [Index(7)]
    public string EndDate { get; set; }

    [Index(8)]
    public string Designation { get; set; }

    [Index(9)]
    public string StartOnMyBehalf { get; set; }

    [Index(10)]
    public string InboxMyBehalf { get; set; }

}

