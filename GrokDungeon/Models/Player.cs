namespace GrokDungeon.Models;

public class Player
{
    public string Id { get; set; } = "players/1";
    public string Name { get; set; } = "Adventurer";
    public int Health { get; set; } = 100;
    public int MaxHealth { get; set; } = 100;
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int Gold { get; set; } = 0;
    
    public Dictionary<string, int> Stats { get; set; } = new()
    {
        { "Strength", 10 },
        { "Dexterity", 10 },
        { "Intelligence", 10 },
        { "Constitution", 10 }
    };

    public List<Item> Inventory { get; set; } = new();
    public List<string> ActiveQuests { get; set; } = new();
    public List<string> CompletedQuests { get; set; } = new();
    
    public string CurrentRoomId { get; set; } = "rooms/start";
}