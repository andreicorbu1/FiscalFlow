using CsvHelper.Configuration;
using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Application.Tools.Csv.Mappings;

public class TransactionMap : ClassMap<Transaction>
{
    public TransactionMap()
    {
        Map(m => m.MoneyValue).Name("Value");
        Map(m => m.MoneyCurrency).Name("Currency");
        Map(m => m.Description).Name("Description");
        Map(m => m.Payee).Name("Payee");
        Map(m => m.Type).Name("Type");
        Map(m => m.Category).Name("Category");
        Map(m => m.CreatedOnUtc).Name("Created");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
        Map(m => m.ModifiedOnUtc).Name("Modified");
    }
}