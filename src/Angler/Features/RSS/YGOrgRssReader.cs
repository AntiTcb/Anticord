namespace Angler.Features.RSS;

public class YGOrgRssReader : RssReader
{
    private readonly IConfiguration _configuration;
    public YGOrgRssReader(IConfiguration configuration) : base(configuration["RssUrls:YGOrg"])
    {
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
