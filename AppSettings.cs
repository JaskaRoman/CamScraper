namespace CamScraper;

public record AppSettings
{
    public string Directory { get; set; }
    public Camera[] Cameras { get; set; }
    public record Camera
    {
        public Uri Url { get; set; }
        public TimeSpan Interval { get; set; }
    }
}