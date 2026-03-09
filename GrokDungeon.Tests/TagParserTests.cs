using DefaultEcs;
using GrokDungeon.Models;
using GrokDungeon.Services;
using Xunit;

namespace GrokDungeon.Tests;

public class TagParserTests
{
    [Fact]
    public async Task Parse_UpdateTag_UpdatesHealth()
    {
        var world = new World();
        var dice = new DiceService();
        var combat = new CombatResolver(dice);
        var executor = new TagExecutor(world, combat, dice);

        var entity = world.CreateEntity();
        entity.Set(new IdComponent { Value = "test_entity" });
        entity.Set(new HealthComponent { Current = 10, Max = 20 });

        string xml = "<update entity=\"Player\" id=\"test_entity\" field=\"hp\" value=\"5\" />";
        
        await executor.ExecuteAsync(xml);

        Assert.Equal(5, entity.Get<HealthComponent>().Current);
    }
}