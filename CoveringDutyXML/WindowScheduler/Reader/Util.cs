using System.Web;
public static class Util
{

    internal static EmployeeModel Map(EmployeeDto dto)
    {
        return new EmployeeModel
        {
            UserId = HttpUtility.HtmlDecode(dto.UserId),
            SOEID = HttpUtility.HtmlDecode(dto.SOEID),
            EMAIL = HttpUtility.HtmlDecode(dto.EMAIL),
            NRIC = HttpUtility.HtmlDecode(dto.NRIC),
            DIDEXTENSION = HttpUtility.HtmlDecode(dto.DIDEXTENSION),
            HANDPHONE = HttpUtility.HtmlDecode(dto.HANDPHONE),
            FAXNUMBER = HttpUtility.HtmlDecode(dto.FAXNUMBER),
            FIRSTNAME = HttpUtility.HtmlDecode(dto.FIRSTNAME),
            LASTNAME = HttpUtility.HtmlDecode(dto.LASTNAME),
            KNOWNAS = HttpUtility.HtmlDecode(dto.KNOWNAS),
            NAME = HttpUtility.HtmlDecode(dto.NAME),
            POSITIONID = HttpUtility.HtmlDecode(dto.POSITIONID),
            POSITIONNAME = HttpUtility.HtmlDecode(dto.POSITIONNAME),
            LOCATION = HttpUtility.HtmlDecode(dto.LOCATION),
            EMPLOYEETYPE = HttpUtility.HtmlDecode(dto.EMPLOYEETYPE),
            ROEMAIL = HttpUtility.HtmlDecode(dto.ROEMAIL),
            WORKUNITNAME = HttpUtility.HtmlDecode(dto.WORKUNITNAME),
            WORKUNITCODE = HttpUtility.HtmlDecode(dto.WORKUNITCODE),
            SECTIONNAME = HttpUtility.HtmlDecode(dto.SECTIONNAME),
            SECTIONCODE = HttpUtility.HtmlDecode(dto.SECTIONCODE),
            BRANCHNAME = HttpUtility.HtmlDecode(dto.BRANCHNAME),
            BRANCHCODE = HttpUtility.HtmlDecode(dto.BRANCHCODE),
            DIVISIONNAME = HttpUtility.HtmlDecode(dto.DIVISIONNAME),
            DIVISIONCODE = HttpUtility.HtmlDecode(dto.DIVISIONCODE),
            CLUSTERNAME = HttpUtility.HtmlDecode(dto.CLUSTERNAME),
            CLUSTERCODE = HttpUtility.HtmlDecode(dto.CLUSTERCODE),
            BOARDNAME = HttpUtility.HtmlDecode(dto.BOARDNAME),
            BOARDCODE = HttpUtility.HtmlDecode(dto.BOARDCODE),
            TERMINATIONDATE = dto.TERMINATIONDATE.Trim() == "" ? null : ParseDate(dto.TERMINATIONDATE),
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

    }

     private static DateTime ParseDate(string dateTime)
    {
        if (DateTime.TryParseExact(dateTime, "yyyy-MM-dd-hh:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
        {
            return dt;
        }
        DateTime.TryParseExact(dateTime, "yyyy-MM-dd-hh:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dt2);
        return dt2;

    }
}