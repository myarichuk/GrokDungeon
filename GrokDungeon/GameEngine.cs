using System.Text;
using System.Xml;
using DefaultEcs;
using GrokDungeon.Models;
using GrokDungeon.Services;
using Microsoft.Extensions.AI;
using Raven.Client.Documents;
using Spectre.Console;

namespace GrokDungeon;

public class GameEngine(
    IChatClient chatClient,
    IDocumentStore store,
    World world,
    TagExecutor tagExecutor)
{
    private const string SystemPrompt = @"
You are the Dungeon Master for GrokDungeon 
Rules: 5th Edition D&D simplified.
Output Format:
STRICT XML ONLY. No markdown, no conversational text outside tags.

Structure:
<response>
  <narrative>
    Flavor text for the player. Max 4 sentences.
  </narrative>
  <gm_only>
    <update entity=""Player|NPC"" id=""..."" field=""hp|gold|xp"" value=""..."" />
    <spawn entity=""NPC|Item"" type=""goblin|sword"" room=""room_id"" />
    <action type=""move|attack|cast"" target=""target_id"" room=""room_id"" />
    <status effect=""poisoned|healed"" target=""target_id"" value=""..."" />
  </gm_only>
</response>

Example:
<response>
  <narrative>The goblin snarls and lunges at you with a rusty dagger.</narrative>
  <gm_only>
    <spawn entity=""NPC"" type=""Goblin"" room=""rooms/1"" />
    <action type=""attack"" target=""players/1"" />
  </gm_only>
</response>
";

    public async Task InitializeAsync()
    {
        // Load Player from RavenDB or create new
        using var session = store.OpenAsyncSession();
        var playerDoc = await session.LoadAsync<Player>("players/1");
        
        var playerEntity = world.CreateEntity();
        playerEntity.Set(new PlayerTag());
        playerEntity.Set(new IdComponent { Value = "players/1" });
        
        if (playerDoc != null)
        {
            playerEntity.Set(new NameComponent { Value = playerDoc.Name });
            playerEntity.Set(new HealthComponent { Current = playerDoc.Health, Max = playerDoc.MaxHealth });
            playerEntity.Set(new StatsComponent { 
                Strength = playerDoc.Stats["Strength"], 
                Dexterity = playerDoc.Stats["Dexterity"],
                Constitution = playerDoc.Stats["Constitution"]
            });
            playerEntity.Set(new LocationComponent { RoomId = playerDoc.CurrentRoomId });
            playerEntity.Set(new InventoryComponent { Items = playerDoc.Inventory.Select(i => i.Name).ToList() });
        }
        else
        {
            // Defaults
            playerEntity.Set(new NameComponent { Value = "Hero" });
            playerEntity.Set(new HealthComponent { Current = 20, Max = 20 });
            playerEntity.Set(new StatsComponent { Strength = 14, Dexterity = 12, Constitution = 14 });
            playerEntity.Set(new LocationComponent { RoomId = "rooms/start" });
            playerEntity.Set(new InventoryComponent { Items = new List<string>() });
        }
        
        playerEntity.Set(new ArmorClassComponent { Value = 14 });
        playerEntity.Set(new WeaponComponent { Name = "Longsword", DamageDice = "1d8" });
    }

    public async Task RunLoopAsync()
    {
        AnsiConsole.Write(new FigletText("Grok Dungeon II").Color(Color.Purple));
        
        while (true)
        {
            DisplayHUD();
            AnsiConsole.Markup("[green]>[/] ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input == "quit") break;

            await ProcessTurnAsync(input);
        }
    }

    private void DisplayHUD()
    {
        var player = world.GetEntities().With<PlayerTag>().AsEnumerable().First();
        var hp = player.Get<HealthComponent>();
        var loc = player.Get<LocationComponent>();
        
        AnsiConsole.Write(new Rule($"[red]HP: {hp.Current}/{hp.Max}[/] | [blue]Loc: {loc.RoomId}[/]").LeftJustified());
    }

    private async Task ProcessTurnAsync(string input)
    {
        var player = world.GetEntities().With<PlayerTag>().AsEnumerable().First();
        var context = $"Player HP: {player.Get<HealthComponent>().Current}. Location: {player.Get<LocationComponent>().RoomId}. Input: {input}";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, SystemPrompt),
            new ChatMessage(ChatRole.User, context)
        };

        await AnsiConsole.Status().StartAsync("Thinking...", async ctx =>
        {
            var response = await chatClient.CompleteAsync(messages);
            var text = response.Message.Text ?? "";
            
            // Extract XML
            var xmlStart = text.IndexOf("<response>");
            var xmlEnd = text.LastIndexOf("</response>");
            if (xmlStart == -1 || xmlEnd == -1) 
            {
                AnsiConsole.MarkupLine("[red]Invalid AI response format.[/]");
                return;
            }
            
            var xml = text.Substring(xmlStart, (xmlEnd - xmlStart) + 11);
            
            // Parse Narrative
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var narrative = doc.SelectSingleNode("//narrative")?.InnerText;
            AnsiConsole.MarkupLine($"[italic yellow]{narrative}[/]");
            AnsiConsole.WriteLine();

            // Execute GM Logic
            var gmOnly = doc.SelectSingleNode("//gm_only");
            if (gmOnly != null)
            {
                await tagExecutor.ExecuteAsync(gmOnly.InnerXml);
            }
        });
        
        // Save State (Simplified: just updating the RavenDB doc from ECS)
        await SaveStateAsync();
    }

    private async Task SaveStateAsync()
    {
        using var session = store.OpenAsyncSession();
        var playerEntity = world.GetEntities().With<PlayerTag>().AsEnumerable().First();
        
        var playerDoc = await session.LoadAsync<Player>("players/1");
        if (playerDoc == null) playerDoc = new Player();

        playerDoc.Health = playerEntity.Get<HealthComponent>().Current;
        playerDoc.CurrentRoomId = playerEntity.Get<LocationComponent>().RoomId;
        // Sync other stats...
        
        await session.StoreAsync(playerDoc);
        await session.SaveChangesAsync();
    }
}