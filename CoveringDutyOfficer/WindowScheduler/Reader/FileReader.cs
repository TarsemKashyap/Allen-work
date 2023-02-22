using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Reflection;


public class FileReader : IDisposable
{
    private readonly DbContext _context;
    private readonly ILogger<FileReader> _logger;
    private readonly IConfiguration _configuration;
    private bool _disposedValue;

    public FileReader(DbContext context, ILogger<FileReader> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;

    }
    internal async Task Import()
    {
        _logger.LogInformation("Running Import Operation");
        FileConfig importConfig = ReadImportConfig();
        DirectoryInfo folder = new DirectoryInfo(importConfig.InboundFolder);

        var files = folder.GetFiles(importConfig.SearchPattern);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            //  MissingFieldFound = null,
            Delimiter = "|"
        };
        try
        {
            
                var records = new List<LogHeader>();
            foreach (var file in files)
            {
                 
                int successCount = 0;
                int failedCount = 0;
                using (var reader = new StreamReader(file.FullName))
                using (var pipeFile = new CsvReader(reader, config))

                using (var writer = new StreamWriter(WriteProcessingLogs(file)))
               
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    
                    await pipeFile.ReadAsync();
                    pipeFile.ReadHeader();
                    while (await pipeFile.ReadAsync())
                    {
                        try
                        {
                            var dto = pipeFile.GetRecord<CoveringDutyDto>();
                            var model = Util.Map(dto);
                            await _context.Upsert(model);
                           
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                           
                            failedCount++;
                        }
                        
                    }
                }

                MoveFile(importConfig.ProcessFolder, file);
               
                var record =  new LogHeader {FileName = file.Name,SuccessCount=successCount,FailedCount=failedCount};
                records.Add(record);
            }
                string textBody = " <table border="+1+" cellpadding="+0+" cellspacing="+0+" width = "+400+"><tr bgcolor='#4da6ff'><td><b>File Name</b></td> <td> <b> Success Count</b> </td><td> <b> Failed Count</b> </td></tr>";  
                foreach(var rec in  records) {
                    textBody += "<tr><td>" + rec.FileName + "</td><td> " + rec.SuccessCount+ "</td><td> " + rec.FailedCount+ "</td> </tr>";
                }      
                textBody += "</table>";

                EmailService.Send("","",textBody,_configuration,_logger,"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

    }

     public class LogHeader
    {
        public string FileName { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }

    private static void MoveFile(string folder, FileInfo file)
    {
        string fileName = $"{DateTime.Today.ToString("yyyyMMddhhmmssffftt")}_{file.Name}";
        MoveFile(folder, fileName, file);
    }

    private static void MoveFile(string folder, string fileName, FileInfo file)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string archievePath = Path.Combine(folder, fileName);
        File.Move(file.FullName, archievePath);
    }

    private string WriteProcessingLogs(FileInfo file )
    {
        var processingLogFolder = _configuration["FileConfig:ProcessingLogs"];
        string fileName = $"{DateTime.Today.ToString("yyyyMMddhhmmssfff")}_{file.Name}";
       return Path.Combine(processingLogFolder, fileName+"Log_" + file.Name.Split('.')[0] + ".csv");
       

    }

    private FileConfig ReadImportConfig()
    {
        var folderPath = _configuration["FileConfig:InboundFolder"];
        var archive = _configuration["FileConfig:ProcessFolder"];
        var searchPattern = _configuration["FileConfig:SearchPattern"];
        return new FileConfig
        {
            InboundFolder = folderPath,
            SearchPattern = searchPattern,
            ProcessFolder = archive
        };

    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~FileReader()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}