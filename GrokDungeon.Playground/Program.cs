using GrokDungeon;
using GrokDungeon.Models;

var gameConsole = new GameConsole();

gameConsole.ShowBanner();
gameConsole.ShowHud(
    new HealthComponent { Current = 15, Max = 20 },
    new LocationComponent { RoomId = "rooms/playground" });
gameConsole.ShowNarrative("You enter the playground where UI experiments can happen safely.");

gameConsole.ShowCombatResult("Critical Hit! You deal 12 damage to the Orc.");
gameConsole.ShowStatus("You found a Potion of Healing!");
gameConsole.ShowError("You cannot enter this room. The door is locked.");
gameConsole.ShowInfo("Tip: use this project to iterate on console presentation.");
