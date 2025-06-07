using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using KiloWatts.Models;

namespace KiloWatts;

public class SmartMeterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SmartMeterService> _logger;

    public SmartMeterService(HttpClient httpClient, ILogger<SmartMeterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EnergyUsageViewModel?> GetUsageDataAsync(string esiid)
    {
        // Example XML request body; customize based on actual API structure
        string requestXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<GetUsageRequest>
    <ESIID>{esiid}</ESIID>
    <RequestDate>{DateTime.UtcNow:yyyy-MM-dd}</RequestDate>
</GetUsageRequest>";

        var content = new StringContent(requestXml, Encoding.UTF8, "application/xml");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

        try
        {
            var response = await _httpClient.PostAsync("/custodian-api/usage", content);
            response.EnsureSuccessStatusCode();

            var responseXml = await response.Content.ReadAsStringAsync();

            // Parse the XML — adjust element names based on actual API
            var doc = XDocument.Parse(responseXml);
            var usageElement = doc.Descendants("UsageData").FirstOrDefault();

            if (usageElement == null)
                return null;

            var timestamp = DateTime.Parse(usageElement.Element("IntervalEnd")?.Value ?? "");
            var usage = decimal.Parse(usageElement.Element("kWh")?.Value ?? "0");

            return new EnergyUsageViewModel
            {
                Timestamp = timestamp,
                UsageInKWh = usage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching or parsing Smart Meter data.");
            return null;
        }
    }
}
//
// public class SmartMeterApiResponse
// {
//     public UsageData Usage { get; set; }
// }

public class UsageData
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
