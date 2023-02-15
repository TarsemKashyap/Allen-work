using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Xml;
using System.Xml.Linq;

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
        foreach (var file in files)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            using var reader = XmlReader.Create(file.FullName, settings);

            XDocument xdocument = XDocument.Load(file.FullName);
            IEnumerable<XElement> employees = xdocument.Root.Elements().Where(e=>e.Name == "Employee");
            foreach (var employee in employees)
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
                    KNOWNAS =  employee.Element("KNOWN_AS").Value,
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

            //  while (reader.ReadToFollowing("Employee"))
            // {
            //     var _dto = new EmployeeDto();
            //     reader.ReadToFollowing("EMAIL");

            //     // _dto.EMAIL = reader.ReadElementContentAsString();
            //     Console.WriteLine($"Email: {reader.ReadNodeValue("Email")}");

            //     reader.ReadToFollowing("NRIC");
            //     Console.WriteLine($"author: {reader.ReadElementContentAsString()}");

            //     reader.ReadToFollowing("USER_ID");
            //     Console.WriteLine($"price: {reader.ReadElementContentAsString()}");

            //     Console.WriteLine("-------------------------");
            // } 

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
