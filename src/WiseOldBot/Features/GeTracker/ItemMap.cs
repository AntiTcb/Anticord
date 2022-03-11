namespace WiseOldBot.Features.GeTracker;

public class ItemMap : Dictionary<string, List<GeTrackerItem>>
{
    public ItemMap() { }

    public ItemMap(IDictionary<string, List<GeTrackerItem>> dict) : base(dict) { }

    public IReadOnlyCollection<GeTrackerItem> Find(string itemName)
    {
        itemName = itemName.ToLower();
        return TryGetValue(itemName, out var returnItems) ? returnItems : FindItems(itemName);
    }

    private IReadOnlyCollection<GeTrackerItem> FindItems(string itemName)
        => this.Where(x => x.Key.Contains(itemName.ToLower()))
        .SelectMany(kvp => kvp.Value)
        .ToArray();
}
