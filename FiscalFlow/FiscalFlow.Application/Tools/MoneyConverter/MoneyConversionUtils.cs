using System.Xml.Linq;

namespace FiscalFlow.Application.Tools.MoneyConverter;

public static class MoneyConversionUtils
{
    public static Dictionary<string, decimal> Conversions { get; private set; } = new();
    const string BnrUrl = "https://www.bnr.ro/nbrfxrates.xml";
    private static XDocument _document;
    static MoneyConversionUtils()
    {
        if (File.Exists("nbrfxrates.xml"))
        {
            var fileInfo = new FileInfo("nbrfxrates.xml");
            if (fileInfo.CreationTime.AddDays(1) < DateTime.Now)
            {
                DownloadXml();
            }
        }
        else
        {
            DownloadXml();
        }
        _document = XDocument.Load("nbrfxrates.xml");
        MapConversionRates();
    }

    static void MapConversionRates()
    {
        XNamespace ns = "http://www.bnr.ro/xsd";
        foreach (var rate in _document.Descendants(ns + "Rate"))
        {
            string currency = rate.Attribute("currency").Value;
            decimal rateValue = decimal.Parse(rate.Value);
            decimal multiplier = 1;

            XAttribute multiplierAttribute = rate.Attribute("multiplier");
            if (multiplierAttribute != null)
            {
                multiplier = decimal.Parse(multiplierAttribute.Value);
                rateValue /= multiplier;
                Console.WriteLine(multiplier);
                Console.WriteLine($"{currency}-RON {rateValue}");
            }

            Conversions[$"{currency}-RON"] = rateValue;

            decimal reverseRateValue = 1 / rateValue;
            Conversions[$"RON-{currency}"] = reverseRateValue;
        }
    }

    private static void DownloadXml()
    {
        using var httpClient = new HttpClient();
        using var stream = httpClient.GetStreamAsync(BnrUrl);
        using var fileStream = new FileStream("nbrfxrates.xml", FileMode.OpenOrCreate);
        stream.Result.CopyTo(fileStream);
    }
}
