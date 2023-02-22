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
        try
        {
            _logger.LogInformation("Running Import Operation");
            FileConfig importConfig = ReadImportConfig();
            _logger.LogInformation("ImportConfig InboundFolder {0}", importConfig.InboundFolder);
            DirectoryInfo folder = new DirectoryInfo(importConfig.InboundFolder);

            var files = folder.GetFiles(importConfig.SearchPattern);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                //  MissingFieldFound = null,
                Delimiter = "|"
            };

            foreach (var file in files)
            {
                using (var reader = new StreamReader(file.FullName))
                using (var pipeFile = new CsvReader(reader, config))
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
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                }

                MoveFile(importConfig.ProcessFolder, file);

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

    }

    private static void MoveFile(string folder, FileInfo file)
    {

        string fileName = $"{DateTime.Now.ToString("yyyyMMdd.HHmmss")}_{file.Name}";
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



    private FileConfig ReadImportConfig()
    {
        _logger.LogInformation("Reading Import Config");
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            _disposedValue = true;
        }
    }



    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}