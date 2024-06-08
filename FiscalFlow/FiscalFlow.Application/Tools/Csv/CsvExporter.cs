using CsvHelper;
using FiscalFlow.Application.Tools.Csv.Mappings;
using System.Globalization;

namespace FiscalFlow.Application.Tools.Csv;

public static class CsvExporter
{
    public static MemoryStream ExportList<T>(IList<T> list, string fileName)
    {
        var memoryStream = new MemoryStream();
        using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TransactionMap>();
            csv.WriteRecords(list);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}