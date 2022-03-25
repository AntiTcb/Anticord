namespace Angler.Features.RSS;

public class CardCoalRssReader : RssReader
{
    public CardCoalRssReader(IConfiguration configuration) : base(configuration["RssUrls:CardCoal"])
    {

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
