using DefaultEcs;
using GrokDungeon.Models;
using GrokDungeon.Services;
using Xunit;

namespace GrokDungeon.Tests;

public class CombatTests
{
    [Fact]
    public void Attack_Hits_WhenRollIsHigh()
    {
        var world = new World();
        var dice = new DiceService();
        var resolver = new CombatResolver(dice);

        var attacker = world.CreateEntity();
        attacker.Set(new StatsComponent { Strength = 20 }); // +5 mod
        attacker.Set(new WeaponComponent { Name = "TestSword", DamageDice = "1d1" }); // Always 1 dmg
        attacker.Set(new NameComponent { Value = "Hero" });

        var defender = world.CreateEntity();
        defender.Set(new ArmorClassComponent { Value = 5 }); // Very low AC
        defender.Set(new HealthComponent { Current = 10, Max = 10 });
        defender.Set(new NameComponent { Value = "Dummy" });

        var result = resolver.ResolveAttack(attacker, defender);

        Assert.Contains("hits", result);
        Assert.True(defender.Get<HealthComponent>().Current < 10);
    }

    [Fact]
    public void DiceService_RollsCorrectly()
    {
        var dice = new DiceService();
        var result = dice.Roll("1d1+5");
        Assert.Equal(6, result);
    }
}