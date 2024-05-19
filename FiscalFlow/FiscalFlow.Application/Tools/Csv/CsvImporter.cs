using CsvHelper;
using FiscalFlow.Application.Tools.Csv.Mappings;
using System.Globalization;

namespace FiscalFlow.Application.Tools.Csv
{
    public static class CsvImporter
    {
        public static IList<T> Import<T>(MemoryStream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TransactionMap>();
            var transactions = csv.GetRecords<T>().ToList();
            return transactions;
        }
    }
}
