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


    public async Task InsertData(IAsyncEnumerable<LeaveDetail> list)
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
    public async Task Upsert(LeaveDetail item)
    {
        if (await AlreadyExists(item.UserId))
            await UpdateDataAsync(item);
        else
            await InsertAsync(item);

    }

    private async Task InsertAsync(LeaveDetail item)
    {
        await OpenConnection();
        string insertQuery = @"
                    Insert into CoveringDuty()
                    Value()
        ";

        await _sqlcon.ExecuteAsync(insertQuery, new { });
    }

    public async Task OpenConnection()
    {
        if (_sqlcon.State != System.Data.ConnectionState.Open)
            await _sqlcon.OpenAsync();
    }

    private async Task UpdateDataAsync(LeaveDetail record)
    {
        await OpenConnection();
    }

    private async Task<bool> AlreadyExists(string userId)
    {
        await OpenConnection();

        const string userExists = "select 1 from CoveringDuty where id=@id";
        return _sqlcon.ExecuteScalar<bool>(userExists, new { id = userId });

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