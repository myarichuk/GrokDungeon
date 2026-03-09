namespace GrokDungeon.Models;

// Identity
public struct IdComponent { public string Value; }
public struct NameComponent { public string Value; }
public struct PlayerTag { }
public struct NpcTag { }

// Stats (5e style)
public struct StatsComponent 
{ 
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Wisdom;
    public int Charisma;
}

// Vitals
public struct HealthComponent 
{ 
    public int Current; 
    public int Max; 
}

// State
public struct LocationComponent { public string RoomId; }
public struct InventoryComponent { public List<string> Items; }
public struct GoldComponent { public int Amount; }

// Combat
public struct ArmorClassComponent { public int Value; }
public struct WeaponComponent { public string Name; public string DamageDice; } // e.g., "1d8"