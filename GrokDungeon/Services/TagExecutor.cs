using System.Xml;
using DefaultEcs;
using GrokDungeon.Models;

namespace GrokDungeon.Services;

public class TagExecutor
{
    private readonly World _world;
    private readonly CombatResolver _combat;
    private readonly DiceService _dice;
    private readonly GameConsole _console;

    public TagExecutor(World world, CombatResolver combat, DiceService dice, GameConsole? console = null)
    {
        _world = world;
        _combat = combat;
        _dice = dice;
        _console = console ?? new GameConsole();
    }

    public async Task ExecuteAsync(string xmlFragment)
    {
        // Wrap in a root for valid parsing if needed, though XmlReader handles fragments
        var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, Async = true };
        using var reader = XmlReader.Create(new StringReader(xmlFragment), settings);

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "update":
                        HandleUpdate(reader);
                        break;
                    case "spawn":
                        HandleSpawn(reader);
                        break;
                    case "action":
                        HandleAction(reader);
                        break;
                    case "status":
                        HandleStatus(reader);
                        break;
                }
            }
        }
    }

    private void HandleUpdate(XmlReader reader)
    {
        var id = reader.GetAttribute("id");
        var field = reader.GetAttribute("field");
        var value = reader.GetAttribute("value");

        var entity = FindEntityById(id);
        if (entity == default) return;

        if (field == "hp" && int.TryParse(value, out var hp))
        {
            var health = entity.Get<HealthComponent>();
            health.Current = hp;
            entity.Set(health);
        }
        else if (field == "gold" && int.TryParse(value, out var gold))
        {
            var g = entity.Has<GoldComponent>() ? entity.Get<GoldComponent>() : new GoldComponent();
            g.Amount = gold;
            entity.Set(g);
        }
    }

    private void HandleSpawn(XmlReader reader)
    {
        var type = reader.GetAttribute("type");
        var room = reader.GetAttribute("room");

        var e = _world.CreateEntity();
        e.Set(new IdComponent { Value = Guid.NewGuid().ToString() });
        e.Set(new NameComponent { Value = type ?? "Unknown" });
        e.Set(new LocationComponent { RoomId = room ?? "unknown" });
        e.Set(new HealthComponent { Current = 10, Max = 10 });
        e.Set(new StatsComponent { Strength = 10, Dexterity = 10, Constitution = 10 });
        e.Set(new ArmorClassComponent { Value = 10 });
        e.Set(new NpcTag());

        _console.ShowInfo($"Spawned {type} in {room}");
    }

    private void HandleAction(XmlReader reader)
    {
        var type = reader.GetAttribute("type");
        var targetId = reader.GetAttribute("target");

        // Assuming player is the actor for now, or context dependent
        var player = FindPlayer();
        var target = FindEntityById(targetId);

        if (type == "attack" && target != default)
        {
            var result = _combat.ResolveAttack(player, target);
            _console.ShowCombatResult(result);
        }
        else if (type == "move")
        {
            var room = reader.GetAttribute("room");
            if (player.Has<LocationComponent>())
            {
                var loc = player.Get<LocationComponent>();
                loc.RoomId = room ?? loc.RoomId;
                player.Set(loc);
            }
        }
    }

    private void HandleStatus(XmlReader reader)
    {
        var effect = reader.GetAttribute("effect");
        var targetId = reader.GetAttribute("target");
        var entity = FindEntityById(targetId);

        if (entity != default && effect == "healed")
        {
            var val = int.Parse(reader.GetAttribute("value") ?? "0");
            var hp = entity.Get<HealthComponent>();
            hp.Current = Math.Min(hp.Max, hp.Current + val);
            entity.Set(hp);
            _console.ShowStatus($"{GetEntityName(entity)} healed for {val} HP.");
        }
    }

    private Entity FindEntityById(string? id)
    {
        if (string.IsNullOrEmpty(id)) return default;
        // Simple linear search for now
        foreach (var e in _world.GetEntities().With<IdComponent>().AsEnumerable())
        {
            if (e.Get<IdComponent>().Value == id) return e;
        }
        return default;
    }

    private Entity FindPlayer()
    {
        return _world.GetEntities().With<PlayerTag>().AsEnumerable().FirstOrDefault();
    }

    private string GetEntityName(Entity e) => e.Has<NameComponent>() ? e.Get<NameComponent>().Value : "Unknown";
}
