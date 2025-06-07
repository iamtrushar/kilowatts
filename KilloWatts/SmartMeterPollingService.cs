using KilloWatts.Models;

namespace KilloWatts;

public class SmartMeterPollingService: BackgroundService
{
    private readonly ILogger<SmartMeterPollingService> _logger;
    private readonly SmartMeterService _smartMeterService;
    private readonly UsageStore _usageStore;
    private readonly IConfiguration _config;

    /// <inheritdoc />
    public SmartMeterPollingService(
        ILogger<SmartMeterPollingService> logger,
        SmartMeterService smartMeterService,
        UsageStore usageStore,
        IConfiguration config)
    {
        _logger = logger;
        _smartMeterService = smartMeterService;
        _usageStore = usageStore;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var esiid = _config["SmartMeter:Esiid"];

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Fetching usage data...");
                var usage = await _smartMeterService.GetUsageDataAsync(esiid);
                _usageStore.Update(usage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch usage data.");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
public class UsageStore
{
    private EnergyUsageViewModel? _data;
    private readonly object _lock = new();

    public void Update(EnergyUsageViewModel model)
    {
        lock (_lock)
        {
            _data = model;
        }
    }

    public EnergyUsageViewModel? Get()
    {
        lock (_lock)
        {
            return _data;
        }
    }
}