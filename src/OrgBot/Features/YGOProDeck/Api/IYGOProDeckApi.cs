using OrgBot.Features.YGOProDeck.Api.Models;
using RestEase;

namespace OrgBot.Features.YGOProDeck.Api;

[BaseAddress("https://db.ygoprodeck.com/api/v7/")]
public interface IYGOProDeckApi
{
    [Get("cardinfo.php")]
    Task<ApiResponse<CardInfoResponse>> GetCardInfoAsync([Query("name")] string? cardName = null, [Query("konami_id")] int? konamiId = null);

    [Get("cardinfo.php?tcgplayer_data=1")]
    Task<ApiResponse<CardInfoResponse>> GetCardPriceAsync([Query("name")] string cardName);
}
