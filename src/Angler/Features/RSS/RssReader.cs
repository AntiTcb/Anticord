namespace Angler.Features.RSS;

public interface IRssReader : IHostedService { }

public abstract class RssReader : BackgroundService, IRssReader
{
    private readonly string _feedUrl;

    protected RssReader(string feedUrl)
    {
        _feedUrl = feedUrl;
    }
}
