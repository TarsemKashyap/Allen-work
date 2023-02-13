
public static class Util
{

   internal static CoveringDutyModel Map(CoveringDutyDto dto)
    {
        return new CoveringDutyModel
        {
            UserId = dto.UserId,
            SOEID = dto.SOEID,
            UserName = dto.UserName,
            DelegateUserID = dto.DelegateUserID,
            DelegateUserName = dto.DelegateUserName,
            DelegateSOEID = dto.DelegateSOEID,
            BeginDate = ParseDate(dto.BeginDate),
            EndDate = ParseDate(dto.EndDate),
            Designation=dto.Designation,
            StartOnMyBehalf=dto.StartOnMyBehalf,
            InboxMyBehalf=dto.InboxMyBehalf,
            CreatedDate=DateTime.Now,
            ModifiedDate=DateTime.Now
        };

    }

    private static DateTime ParseDate(string dateTime)
    {
        DateTime.TryParse(dateTime, out DateTime dt);
        return dt;

    }
}