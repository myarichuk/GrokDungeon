namespace GrokDungeon.Models;

public class Profile
{
    public string Id { get; set; } = "profiles/default";
    public string Name { get; set; } = "Default Profile";
    public List<Campaign> Campaigns { get; set; } = new();
    public List<Player> Characters { get; set; } = new();
}

public class Campaign
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "New Campaign";
    public string PlotSummary { get; set; } = "";
    public List<string> Factions { get; set; } = new();
    public string Ruleset { get; set; } = "5e";
    public string CurrentGameStateId { get; set; } = "";
}
