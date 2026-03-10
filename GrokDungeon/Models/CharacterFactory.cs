namespace GrokDungeon.Models;

public static class CharacterFactory
{
    public static Player Create5eFighter(string name)
    {
        return new Player
        {
            Id = $"players/{Guid.NewGuid()}",
            Name = name,
            Level = 1,
            Health = 12,
            MaxHealth = 12,
            Stats = new Dictionary<string, int>
            {
                { "Strength", 16 },
                { "Dexterity", 13 },
                { "Constitution", 15 },
                { "Intelligence", 10 },
                { "Wisdom", 12 },
                { "Charisma", 8 }
            },
            Inventory = new List<Item>
            {
                new Item { Id = "items/longsword", Name = "Longsword", Description = "A standard longsword." },
                new Item { Id = "items/shield", Name = "Shield", Description = "A wooden shield." }
            }
        };
    }

    public static Player Create5eWizard(string name)
    {
        return new Player
        {
            Id = $"players/{Guid.NewGuid()}",
            Name = name,
            Level = 1,
            Health = 6,
            MaxHealth = 6,
            Stats = new Dictionary<string, int>
            {
                { "Strength", 8 },
                { "Dexterity", 14 },
                { "Constitution", 12 },
                { "Intelligence", 16 },
                { "Wisdom", 13 },
                { "Charisma", 10 }
            },
            Inventory = new List<Item>
            {
                new Item { Id = "items/spellbook", Name = "Spellbook", Description = "A book containing arcane knowledge." },
                new Item { Id = "items/staff", Name = "Quarterstaff", Description = "A simple wooden staff." }
            }
        };
    }
}
