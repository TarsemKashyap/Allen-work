using CsvHelper.Configuration.Attributes;

public class EmployeeDto
{

    [Index(0)]
    public string UserId { get; set; }
    [Index(1)]
    public string EMAIL { get; set; }
    [Index(2)]
    public string NRIC { get; set; }
    [Index(3)]
    public string SOEID { get; set; }
    [Index(4)]
    public string DIDEXTENSION { get; set; }
    [Index(5)]
    public string  HANDPHONE { get; set; }
    [Index(6)]
    public string FAXNUMBER { get; set; }
    [Index(7)]
    public string FIRSTNAME { get; set; }
    [Index(8)]
    public string LASTNAME { get; set; }
    [Index(9)]
    public string KNOWNAS { get; set; }
    [Index(10)]
    public string NAME { get; set; }
    [Index(11)]
    public string POSITIONID { get; set; }
    [Index(12)]
    public string POSITIONNAME { get; set; }
    [Index(13)]
    public string LOCATION { get; set; }
    [Index(14)]
    public string EMPLOYEETYPE { get; set; }
    [Index(15)]
    public string ROEMAIL { get; set; }
    [Index(16)]
    public string WORKUNITNAME { get; set; }
    [Index(17)]
    public string WORKUNITCODE { get; set; }
    [Index(18)]
    public string SECTIONNAME { get; set; }
    [Index(19)]
    public string SECTIONCODE { get; set; }
     [Index(20)]
    public string BRANCHNAME { get; set; }
    [Index(21)]
    public string BRANCHCODE { get; set; }
    [Index(22)]
    public string DIVISIONNAME { get; set; }
    [Index(23)]
    public string DIVISIONCODE { get; set; }
    [Index(24)]
    public string CLUSTERNAME { get; set; }
    [Index(25)]
    public string CLUSTERCODE { get; set; }
    [Index(26)]
    public string BOARDNAME { get; set; }
    [Index(27)]
    public string BOARDCODE { get; set; }
    [Index(28)]
    public string TERMINATIONDATE { get; set; }

}

