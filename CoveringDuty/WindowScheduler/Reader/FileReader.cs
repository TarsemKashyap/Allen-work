using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;

public class FileReader
{
    private readonly DbContext _context;
    private readonly ILogger<FileReader> _logger;
    private readonly IConfiguration _configuration;

    public FileReader(DbContext context, ILogger<FileReader> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;

    }
    internal async Task Import()
    {
        _logger.LogInformation("Running Import Operation");
        ImportConfig importConfig = ReadImportConfig();
        DirectoryInfo folder = new DirectoryInfo(importConfig.FolderPath);
        var files = folder.GetFiles(importConfig.SearchPattern);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        foreach (var file in files)
        {
            using (var reader = new StreamReader(file.FullName))
            using (var pipeFile = new CsvReader(reader, config))
            {
                await _context.OpenConnection();
                await pipeFile.ReadAsync();
                pipeFile.ReadHeader();
                while (await pipeFile.ReadAsync())
                {
                    var record = pipeFile.GetRecord<LeaveDetail>();
                    await _context.Upsert(record);
                }
            }

        }



    }

    private ImportConfig ReadImportConfig()
    {
        var folderPath = _configuration["ImportConfig:FolderPath"];
        var searchPattern = _configuration["ImportConfig:SearchPattern"];
        return new ImportConfig
        {
            FolderPath = folderPath,
            SearchPattern = searchPattern
        };

    }
}