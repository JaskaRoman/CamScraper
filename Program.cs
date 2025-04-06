using CamScraper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application = System.Windows.Forms.Application;

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var appSettings = new AppSettings();
configurationBuilder.Bind(appSettings);

if (!string.IsNullOrWhiteSpace(appSettings.Directory))
{
    Directory.SetCurrentDirectory(appSettings.Directory);
}

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddFilter(nameof(Microsoft), LogLevel.None);
foreach (var camera in appSettings.Cameras)
{
    builder.Services.AddSingleton<IHostedService>(_ => new ImageScraperService(camera.Interval, camera.Url));
}

var app = builder.Build();
_ = app.RunAsync();

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
using var tray = new TrayIcon();
Application.Run();