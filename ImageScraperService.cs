using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace CamScraper;

public class ImageScraperService : BackgroundService
{
    private readonly TimeSpan interval;
    private readonly Uri url;
    private readonly string directory;
    private readonly string extension;
    private readonly int id;

    private void WriteLine(string value) => Console.WriteLine($"[{id}] {value}");

    public ImageScraperService(TimeSpan interval, Uri url)
    {
        id = RuntimeHelpers.GetHashCode(this);
        this.interval = interval;
        this.url = url;

        directory = Path.GetFileNameWithoutExtension(url.LocalPath);
        if (!Directory.Exists(directory))
        {
            WriteLine("Creating " + directory);
            Directory.CreateDirectory(directory);
        }

        extension = Path.GetExtension(url.LocalPath);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        HttpClient? client = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                client ??= new HttpClient { BaseAddress = url };
                var response = await client.GetAsync(string.Empty, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    WriteLine("Successfully loaded " + client.BaseAddress);
                    await SaveToFile(response, cancellationToken);
                }
                else throw new HttpRequestException(response.ToString());
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                client?.Dispose();
                client = null;
            }
            var nextRun = DateTime.Now + interval;
            WriteLine("Next run at " + nextRun);
            await Task.Delay(interval, cancellationToken);
        }
        client?.Dispose();
    }

    private async Task SaveToFile(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var path = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        path = Path.ChangeExtension(path, extension);
        path = Path.Combine(directory, path);
        Console.WriteLine("Saving " + path);
        await using var fs = new FileStream(path, FileMode.CreateNew);
        await response.Content.CopyToAsync(fs, cancellationToken);
    }
}