using OrgBot.Features.YugiohPrices.Api.Models;
using RestEase;

namespace OrgBot.Features.YugiohPrices.Api;

[BaseAddress("https://yugiohprices.com/api/")]
public interface IYugiohPricesApi
{
    [Get("card_sets")]
    Task<List<string>> GetSetNamesAsync();

    [Get("get_card_prices/{cardName}")]
    Task<ApiResponse<CardSet>> GetCardPriceAsync([Path] string cardName);

    [Get("card_data/{cardName}")]
    Task<string> GetCardDataAsync([Path] string cardName);

    [Get("https://yugiohprices.com/autocomplete_names")]
    Task<string[]> SearchCardAndSetNames([Query("term")] string searchTerm);
}