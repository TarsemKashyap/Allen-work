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
                        catch (CsvHelper.MissingFieldException ex)
                        {
                            throw ex;
                        }
                    }
                }

                MoveFile(importConfig.ProcessFolder, file);

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }





    }

    private static void MoveFile(string folder, FileInfo file)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string fileName = $"{DateTime.Today.ToString("yyyyMMdd")}_{file.Name}";

        string archievePath = Path.Combine(folder, fileName);
        File.Move(file.FullName, archievePath);
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
}