﻿namespace Angler.Features.RSS;

public class YGOrgRssReader : RssReader
{
    public YGOrgRssReader(IConfiguration configuration) : base(configuration["RssUrls:YGOrg"])
    {

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
