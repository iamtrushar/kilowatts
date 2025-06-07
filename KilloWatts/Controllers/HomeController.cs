using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KilloWatts.Models;

namespace KilloWatts.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SmartMeterService _smartMeterService;
    private readonly IConfiguration _config;
    private readonly UsageStore _usageStore;

    public HomeController(
        ILogger<HomeController> logger,
        SmartMeterService smartMeterService,
        IConfiguration config,
        UsageStore usageStore)
    {
        _logger = logger;
        _smartMeterService = smartMeterService;
        _config = config;
        _usageStore = usageStore;
    }

    public async Task<IActionResult> Index()
    {
        var model = _usageStore.Get();

        if (model == null)
        {
            // Show loading or fallback view
            ViewBag.Message = "No usage data available yet.";
            return View("Loading");
        }

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}