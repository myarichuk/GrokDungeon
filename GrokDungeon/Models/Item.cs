namespace GrokDungeon.Models;

public class Item
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public int Value { get; set; }
    public Dictionary<string, int> Attributes { get; set; } = new();
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Key,
    Misc
}