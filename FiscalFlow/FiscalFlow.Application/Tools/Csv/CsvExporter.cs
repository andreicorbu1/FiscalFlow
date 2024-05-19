using System.Globalization;
using CsvHelper;
using FiscalFlow.Application.Tools.Csv.Mappings;

namespace FiscalFlow.Application.Tools.Csv;

public static class CsvExporter
{
    public static void ExportList<T>(IList<T> list, string fileName)
    {
        using var writer = new StreamWriter($"CSV\\{fileName}.csv");
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<TransactionMap>();
        csv.WriteRecords(list);
    }
}