using Microsoft.Data.SqlClient;
using Dapper;
using System.Web;
public class DbContext : IDisposable
{

    private readonly SqlConnection _sqlcon;
    private bool disposedValue;

    public DbContext(string connection)
    {
        _sqlcon = new SqlConnection(connection);

    }


    public async Task InsertData(IAsyncEnumerable<EmployeeModel> list)
    {
        await foreach (var item in list)
        {
            if (await AlreadyExists(item.UserId))
                await UpdateDataAsync(item);
            else
                await InsertAsync(item);

        }

    }

    public async Task Upsert(EmployeeModel item)
    {
        if (await AlreadyExists(item.UserId))
            await UpdateDataAsync(item);
        else
            await InsertAsync(item);

    }

    private async Task InsertAsync(EmployeeModel item)
    {
        string insertQuery = @"
                   INSERT INTO [dbo].[Employee] (
                     UserId
                    ,EMAIL
                    ,NRIC
                    ,SOEID
                    ,DIDEXTENSION
                    ,HANDPHONE
                    ,FAXNUMBER
                    ,FIRSTNAME
                    ,LASTNAME
                    ,KNOWNAS
                    ,NAME
                    ,POSITIONID
                    ,POSITIONNAME
                    ,LOCATION
                    ,EMPLOYEETYPE
                    ,ROEMAIL
                    ,WORKUNITNAME
                    ,WORKUNITCODE
                    ,SECTIONNAME
                    ,SECTIONCODE
                    ,BRANCHNAME
                    ,BRANCHCODE
                    ,DIVISIONNAME
                    ,DIVISIONCODE
                    ,CLUSTERNAME
                    ,CLUSTERCODE
                    ,BOARDNAME
                    ,BOARDCODE
                    ,TERMINATIONDATE
                    ,CreatedDate
                    ,ModifiedDate)
                    VALUES (
                     @UserId
                    ,@EMAIL
                    ,@NRIC
                    ,@SOEID 
                    ,@DIDEXTENSION
                    ,@HANDPHONE
                    ,@FAXNUMBER
                    ,@FIRSTNAME
                    ,@LASTNAME
                    ,@KNOWNAS
                    ,@NAME
                    ,@POSITIONID 
                    ,@POSITIONNAME 
                    ,@LOCATION
                    ,@EMPLOYEETYPE
                    ,@ROEMAIL
                    ,@WORKUNITNAME
                    ,@WORKUNITCODE
                    ,@SECTIONNAME 
                    ,@SECTIONCODE
                    ,@BRANCHNAME
                    ,@BRANCHCODE
                    ,@DIVISIONNAME 
                    ,@DIVISIONCODE 
                    ,@CLUSTERNAME 
                    ,@CLUSTERCODE 
                    ,@BOARDNAME
                    ,@BOARDCODE
                    ,@TERMINATIONDATE
                    ,@CreatedDate
                    ,@ModifiedDate
                    )";
        item.CreatedDate = DateTime.Now;
        item.ModifiedDate = DateTime.Now;
        var valueObject = QueryParams(item);
        await _sqlcon.ExecuteAsync(insertQuery, valueObject);
    }

    private object QueryParams(EmployeeModel item)
    {
        return new
        {
            UserId = item.UserId,
            SOEID = item.SOEID,
            EMAIL = item.EMAIL,
            NRIC = item.NRIC,
            DIDEXTENSION = item.DIDEXTENSION,
            HANDPHONE = item.HANDPHONE,
            FAXNUMBER = item.FAXNUMBER,
            FIRSTNAME = item.FIRSTNAME,
            LASTNAME = item.LASTNAME,
            KNOWNAS = item.KNOWNAS,
            NAME = item.NAME,
            POSITIONID = item.POSITIONID,
            POSITIONNAME = item.POSITIONNAME,
            LOCATION = item.LOCATION,
            EMPLOYEETYPE = item.EMPLOYEETYPE,
            ROEMAIL = item.ROEMAIL,
            WORKUNITNAME = item.WORKUNITNAME,
            WORKUNITCODE = item.WORKUNITCODE,
            SECTIONNAME = item.SECTIONNAME,
            SECTIONCODE = item.SECTIONCODE,
            BRANCHNAME = item.BRANCHNAME,
            BRANCHCODE = item.BRANCHCODE,
            DIVISIONNAME = item.DIVISIONNAME,
            DIVISIONCODE = HttpUtility.HtmlDecode(item.DIVISIONCODE),
            CLUSTERNAME = item.CLUSTERNAME,
            CLUSTERCODE = item.CLUSTERCODE,
            BOARDNAME = item.BOARDNAME,
            BOARDCODE = item.BOARDCODE,
            TERMINATIONDATE = item.TERMINATIONDATE,
            CreatedDate = item.CreatedDate,
            ModifiedDate = item.ModifiedDate
        };
    }



    private async Task UpdateDataAsync(EmployeeModel record)
    {
        string updateQuery = @"
                UPDATE [Employee]
                    SET 
                        [EMAIL] = @EMAIL,
                        [NRIC] = @NRIC,
                        [SOEID] = @SOEID,
                        [DIDEXTENSION] = @DIDEXTENSION,
                        HANDPHONE = @HANDPHONE,
                        [FAXNUMBER] = @FAXNUMBER,
                        [FIRSTNAME] = @FIRSTNAME,
                        [LASTNAME] = @LASTNAME,
                        [KNOWNAS] = @KNOWNAS,
                        [NAME] = @NAME,
                        [POSITIONID] = @POSITIONID,
                        [POSITIONNAME] = @POSITIONNAME,
                        [LOCATION] = @LOCATION,
                        [EMPLOYEETYPE] = @EMPLOYEETYPE,
                        [ROEMAIL] = @ROEMAIL,
                        [WORKUNITNAME] = @WORKUNITNAME,
                        [WORKUNITCODE] = @WORKUNITCODE,
                        [SECTIONNAME] = @SECTIONNAME,
                        [SECTIONCODE] = @SECTIONCODE,
                        [BRANCHNAME] = @BRANCHNAME,
                        [BRANCHCODE] = @BRANCHCODE,
                        [DIVISIONNAME] = @DIVISIONNAME,
                        [DIVISIONCODE] = @DIVISIONCODE,
                        [CLUSTERNAME] = @CLUSTERNAME,
                        [CLUSTERCODE] = @CLUSTERCODE,
                        [BOARDNAME] = @BOARDNAME,
                        [BOARDCODE] = @BOARDCODE,
                        TERMINATIONDATE = @TERMINATIONDATE,
                        ModifiedDate=@ModifiedDate
                        WHERE [UserId] = @UserId
        ";
        record.ModifiedDate = DateTime.Now;
        var valueObject = QueryParams(record);
        await _sqlcon.ExecuteAsync(updateQuery, valueObject);

    }

    private async Task<bool> AlreadyExists(string userId)
    {

        const string userExists = "select 1 from employee where userId=@userId";
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