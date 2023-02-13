using System;

public record CoveringDutyModel
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string SOEID { get; set; }
    public string UserName { get; set; }
    public string DelegateUserID { get; set; }
    public string DelegateSOEID { get; set; }
    public string DelegateUserName { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Designation { get; set; }
    public string StartOnMyBehalf { get; set; }
    public string InboxMyBehalf { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

}

