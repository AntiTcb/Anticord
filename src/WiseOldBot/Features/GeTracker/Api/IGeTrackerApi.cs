using RestEase;
using WiseOldBot.Features.GeTracker.Api.Models;

namespace WiseOldBot.Features.GeTracker.Api;

[BaseAddress("https://www.ge-tracker.com/api")]
public interface IGeTrackerApi
{
    [Header("Authorization")]
    string Token { get; set; }

    [Header("Accept", "application/x.getracker.v1+json")]
    string Accept { get; set; }

    [Get("items/{itemId}")]
    Task<ApiResponse<Item>> GetItemAsync([Path] int itemId);

    [Get("items")]
    Task<ApiResponse<Item[]>> GetItemsAsync();

    [Get("osb-uptime/status")]
    Task<ApiResponse<OsbStatus>> GetOsbStatusAsync();
}
