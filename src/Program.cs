using System.Security.Cryptography.X509Certificates;
using KiloWatts;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var cert = new X509Certificate2(
    config["SmartMeter:CertPath"] ?? throw new InvalidOperationException(),
    config["SmartMeter:CertPassword"]
);

// HttpClient with client cert
builder.Services.AddHttpClient<SmartMeterService>(client =>
{
    client.BaseAddress = new Uri("https://services.smartmetertexas.net/");
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ClientCertificates.Add(cert);
    return handler;
});

builder.Services.AddSingleton<UsageStore>();
builder.Services.AddHostedService<SmartMeterPollingService>();
builder.Services.AddHostedService<EnergyUsageHostedService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();