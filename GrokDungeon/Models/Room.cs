namespace GrokDungeon.Models;

public class Room
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Exits { get; set; } = new(); // Direction -> RoomId
    public List<Item> Items { get; set; } = new();
    public List<string> NpcIds { get; set; } = new();
    public bool IsVisited { get; set; } = false;
}