using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;
using Infrastructure;

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
        try
        {
            _logger.LogInformation("Running Import Operation");
            ImportConfig importConfig = ReadImportConfig();
            _logger.LogInformation("Folder config InboundFolder {0}", importConfig.InboundFolder);
            DirectoryInfo folder = new DirectoryInfo(importConfig.InboundFolder);
            var files = folder.GetFiles(importConfig.SearchPattern);
            _logger.LogInformation("total file  {0}", files.Count());
            Dictionary<string, List<ProcessingError>> errors = new Dictionary<string, List<ProcessingError>>();
            foreach (var file in files)
            {
                _logger.LogInformation("total file  {@fileName}", file.Name);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                using var reader = XmlReader.Create(file.FullName, settings);
                var xdocument = XDocument.Load(file.FullName);
                errors.Add(file.Name, new List<ProcessingError>());
                IEnumerable<XElement> employees = xdocument.Root.Elements().Where(e => e.Name == "Employee");
                foreach (var employee in employees)
                {
                    try
                    {

                        var dto = new EmployeeDto
                        {
                            UserId = employee.Element("USER_ID").Value,
                            SOEID = employee.Element("SOE_ID").Value,
                            EMAIL = employee.Element("EMAIL").Value,
                            NRIC = employee.Element("NRIC").Value,
                            DIDEXTENSION = employee.Element("DID_EXTENSION").Value,
                            HANDPHONE = employee.Element("HAND_PHONE").Value,
                            FAXNUMBER = employee.Element("FAX_NUMBER").Value,
                            FIRSTNAME = employee.Element("FIRST_NAME").Value,
                            LASTNAME = employee.Element("LAST_NAME").Value,
                            KNOWNAS = employee.Element("KNOWN_AS").Value,
                            NAME = employee.Element("NAME").Value,
                            POSITIONID = employee.Element("POSITION_ID").Value,
                            POSITIONNAME = employee.Element("POSITION_NAME").Value,
                            LOCATION = employee.Element("LOCATION").Value,
                            EMPLOYEETYPE = employee.Element("EMPLOYEE_TYPE").Value,
                            ROEMAIL = employee.Element("RO_EMAIL").Value,
                            WORKUNITNAME = employee.Element("WORK_UNIT_NAME").Value,
                            WORKUNITCODE = employee.Element("WORK_UNIT_CODE").Value,
                            SECTIONNAME = employee.Element("SECTION_NAME").Value,
                            SECTIONCODE = employee.Element("SECTION_CODE").Value,
                            BRANCHNAME = employee.Element("BRANCH_NAME").Value,
                            BRANCHCODE = employee.Element("BRANCH_CODE").Value,
                            DIVISIONNAME = employee.Element("DIVISION_NAME").Value,
                            DIVISIONCODE = employee.Element("DIVISION_CODE").Value,
                            CLUSTERNAME = employee.Element("CLUSTER_NAME").Value,
                            CLUSTERCODE = employee.Element("CLUSTER_CODE").Value,
                            BOARDNAME = employee.Element("BOARD_NAME").Value,
                            BOARDCODE = employee.Element("BOARD_CODE").Value,
                            TERMINATIONDATE = employee.Element("TERMINATION_DATE").Value
                        };
                        var model = Util.Map(dto);
                        await _context.Upsert(model);
                    }
                    catch (Exception ex)
                    {
                        var error = new ProcessingError
                        {
                            File = file,
                            Data = $"Exception while processing record for UserId:{employee.Element("USER_ID").Value}",
                            exception = ex,
                        };
                        errors[file.Name].Add(error);
                        _logger.LogError(ex, ex.Message);
                    }
                }
                reader.Close();
                reader.Dispose();
                if (errors[file.Name].Count == 0)
                {
                    MoveFile(importConfig.ProcessFolder, file);
                }


            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }


    }

    private static void MoveFile(string folder, FileInfo file)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string fileName = $"{DateTime.Now.ToString("yyyyMMdd.HHmmss")}_{file.Name}";
        string archievePath = Path.Combine(folder, fileName);
        File.Move(file.FullName, archievePath);
    }

    private ImportConfig ReadImportConfig()
    {
        try
        {
            var folderPath = _configuration["ImportConfig:InboundFolder"];
            var archive = _configuration["ImportConfig:ProcessFolder"];
            var searchPattern = _configuration["ImportConfig:SearchPattern"];
            return new ImportConfig
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


        // file.xml -- > process 20230216_file.xml
    }

}
