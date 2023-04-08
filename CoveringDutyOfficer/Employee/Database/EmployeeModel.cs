using System;

public record EmployeeModel
{
    public string UserId { get; set; }
    public string EMAIL { get; set; }
    public string NRIC { get; set; }
    public string SOEID { get; set; }
    public string DIDEXTENSION { get; set; }
    public string  HANDPHONE { get; set; }
    public string FAXNUMBER { get; set; }
    public string FIRSTNAME { get; set; }
    public string LASTNAME { get; set; }
    public string KNOWNAS { get; set; }
    public string NAME { get; set; }
    public string POSITIONID { get; set; }
    public string POSITIONNAME { get; set; }
    public string LOCATION { get; set; }
    public string EMPLOYEETYPE { get; set; }
    public string ROEMAIL { get; set; }
    public string WORKUNITNAME { get; set; }
    public string WORKUNITCODE { get; set; }
    public string SECTIONNAME { get; set; }
    public string SECTIONCODE { get; set; }
    public string BRANCHNAME { get; set; }
    public string BRANCHCODE { get; set; }
    public string DIVISIONNAME { get; set; }
    public string DIVISIONCODE { get; set; }
    public string CLUSTERNAME { get; set; }
    public string CLUSTERCODE { get; set; }
    public string BOARDNAME { get; set; }
    public string BOARDCODE { get; set; }
    public DateTime? TERMINATIONDATE { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

}

