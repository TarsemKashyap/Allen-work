using System.Web;
public static class Util
{

    internal static EmployeeModel Map(EmployeeDto dto)
    {
        return new EmployeeModel
        {
            UserId = dto.UserId,
            SOEID = dto.SOEID,
            EMAIL = dto.EMAIL,
            NRIC = dto.NRIC,
            DIDEXTENSION = dto.DIDEXTENSION,
            HANDPHONE = dto.HANDPHONE,
            FAXNUMBER = dto.FAXNUMBER,
            FIRSTNAME = dto.FIRSTNAME,
            LASTNAME = dto.LASTNAME,
            KNOWNAS = dto.KNOWNAS,
            NAME = dto.NAME,
            POSITIONID = dto.POSITIONID,
            POSITIONNAME = dto.POSITIONNAME,
            LOCATION = dto.LOCATION,
            EMPLOYEETYPE = dto.EMPLOYEETYPE,
            ROEMAIL = dto.ROEMAIL,
            WORKUNITNAME = dto.WORKUNITNAME,
            WORKUNITCODE = dto.WORKUNITCODE,
            SECTIONNAME = dto.SECTIONNAME,
            SECTIONCODE = dto.SECTIONCODE,
            BRANCHNAME = dto.BRANCHNAME,
            BRANCHCODE = dto.BRANCHCODE,
            DIVISIONNAME = dto.DIVISIONNAME,
            DIVISIONCODE = HttpUtility.HtmlDecode(dto.DIVISIONCODE),
            CLUSTERNAME = dto.CLUSTERNAME,
            CLUSTERCODE = dto.CLUSTERCODE,
            BOARDNAME = dto.BOARDNAME,
            BOARDCODE = dto.BOARDCODE,
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