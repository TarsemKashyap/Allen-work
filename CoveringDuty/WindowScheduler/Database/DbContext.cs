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
        await OpenConnection();

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
        await OpenConnection();
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
        var valueObject = new
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
        await _sqlcon.ExecuteAsync(insertQuery, valueObject);
    }

    public async Task OpenConnection()
    {
        if (_sqlcon.State != System.Data.ConnectionState.Open)
            await _sqlcon.OpenAsync();
    }

    private async Task UpdateDataAsync(CoveringDutyModel record)
    {
        await OpenConnection();
    }

    private async Task<bool> AlreadyExists(string userId)
    {
        await OpenConnection();

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