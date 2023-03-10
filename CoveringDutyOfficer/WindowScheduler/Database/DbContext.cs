using Microsoft.Data.SqlClient;
using Dapper;
public class DbContext : IDisposable
{

    private readonly SqlConnection _sqlcon;
    private bool disposedValue;

    public DbContext(string connection)
    {
        _sqlcon = new SqlConnection(connection);

    }


    public async Task InsertData(IAsyncEnumerable<CoveringDutyModel> list)
    {

        await foreach (var item in list)
        {
            if (await AlreadyExists(item.UserId))
                await UpdateDataAsync(item);
            else
                await InsertAsync(item);

        }

    }

    public async Task Upsert(CoveringDutyModel item)
    {
        if (await AlreadyExists(item.UserId))
            await UpdateDataAsync(item);
        else
            await InsertAsync(item);

    }

    private async Task InsertAsync(CoveringDutyModel item)
    {
        string insertQuery = @"
                   INSERT INTO [dbo].[CoveringDuty] (
                        [UserId],[SOEID],[UserName],
                        [DelegateUserId],[DelegateSOEID],[DelegateUserName],
                        [BeginDate],[EndDate],[Designation],[StartOnMyBehalf],
                        [InboxMyBehalf],[CreatedDate],[ModifiedDate])
                    VALUES (
                                 @UserId
                                ,@SOEID
                                ,@UserName
                                ,@DelegateUserId
                                ,@DelegateSOEID
                                ,@DelegateUserName
                                ,@BeginDate
                                ,@EndDate
                                ,@Designation
                                ,@StartOnMyBehalf
                                ,@InboxMyBehalf
                                ,@CreatedDate
                                ,@ModifiedDate
                                )
        ";
        var valueObject = QueryParams(item);
        await _sqlcon.ExecuteAsync(insertQuery, valueObject);
    }

    private object QueryParams(CoveringDutyModel item)
    {
        return new
        {
            UserId = item.UserId,
            SOEID = item.SOEID,
            UserName = item.UserName,
            DelegateUserId = item.DelegateUserID,
            DelegateSOEID = item.DelegateSOEID,
            DelegateUserName = item.DelegateUserName,
            BeginDate = item.BeginDate,
            EndDate = item.EndDate,
            Designation = item.Designation,
            StartOnMyBehalf = item.StartOnMyBehalf,
            InboxMyBehalf = item.InboxMyBehalf,
            CreatedDate = item.CreatedDate,
            ModifiedDate = item.ModifiedDate
        };
    }

    

    private async Task UpdateDataAsync(CoveringDutyModel record)
    {
        string updateQuery = @"
                UPDATE [CoveringDuty]
                    SET 
                        [SOEID] = @SOEID,
                        [UserName] = @UserName,
                        [DelegateUserId] = @DelegateUserId,
                        [DelegateSOEID] = @DelegateSOEID,
                        [DelegateUserName] = @DelegateUserName,
                        [BeginDate] = @BeginDate,
                        [EndDate] = @EndDate,
                        [Designation] = @Designation,
                        [StartOnMyBehalf] = @StartOnMyBehalf,
                        [InboxMyBehalf] = @InboxMyBehalf,
                        [ModifiedDate] = @ModifiedDate
                    WHERE [UserId] = @UserId
        ";
        record.ModifiedDate = DateTime.Now;
        var valueObject = QueryParams(record);
        await _sqlcon.ExecuteAsync(updateQuery, valueObject);

    }

    private async Task<bool> AlreadyExists(string userId)
    {

        const string userExists = "select 1 from CoveringDuty where userId=@userId";
        return _sqlcon.ExecuteScalar<bool>(userExists, new { userId = userId });

    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _sqlcon.Close();
                _sqlcon.Dispose();
            }

            disposedValue = true;
        }
    }



    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}