namespace GrokDungeon.Models;

public class GameState
{
    public string Id { get; set; } = "gamestate/1";
    public DateTime LastSaved { get; set; } = DateTime.UtcNow;
    public int TurnCount { get; set; } = 0;
    public string CurrentNarrative { get; set; } = "You stand at the entrance of a dark dungeon. The air is stale and cold.";
    public List<string> LogHistory { get; set; } = new();
}