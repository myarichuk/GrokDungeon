using GrokDungeon;
using GrokDungeon.Models;

var gameConsole = new GameConsole();

gameConsole.ShowBanner();
gameConsole.ShowHud(
    new HealthComponent { Current = 15, Max = 20 },
    new LocationComponent { RoomId = "rooms/playground" });
gameConsole.ShowNarrative("You enter the playground where UI experiments can happen safely.");
gameConsole.ShowInfo("Tip: use this project to iterate on console presentation.");
