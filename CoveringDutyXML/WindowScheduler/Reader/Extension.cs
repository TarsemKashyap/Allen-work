using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Xml;

public static class Extension {
    public static string ReadNodeValue(this XmlReader reader,string nodeName) {
        
        return reader.ReadElementContentAsString();
    }
}