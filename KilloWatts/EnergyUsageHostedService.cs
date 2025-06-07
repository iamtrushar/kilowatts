namespace KilloWatts;

public class EnergyUsageHostedService: IHostedService, IDisposable
{
    public EnergyUsageHostedService(
        SmartMeterService smartMeterService,
        IConfiguration config)
    {
        _smartMeterService = smartMeterService;
        _config = config;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        var esiid = _config["SmartMeter:Esiid"];
        var data = await _smartMeterService.GetUsageDataAsync(esiid);
        // Store or process the data as needed
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
    
    private Timer _timer;
    private readonly SmartMeterService _smartMeterService;
    private readonly IConfiguration _config;
}