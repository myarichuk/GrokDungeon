using GrokDungeon.Models;
using Spectre.Console;

namespace GrokDungeon;

public class GameConsole(IAnsiConsole? console = null)
{
    private readonly IAnsiConsole _console = console ?? AnsiConsole.Console;

    public void ShowBanner()
    {
        _console.Write(new FigletText("Grok Dungeon II").Color(Color.Purple));
        _console.Write(new Rule("[grey]Adventure awaits[/]").LeftJustified());
    }

    public void ShowHud(HealthComponent health, LocationComponent location)
    {
        var table = new Table().Border(TableBorder.Rounded).HideHeaders();
        table.AddColumn("stat");
        table.AddColumn("value");
        table.AddRow("[red]HP[/]", $"[red]{health.Current}/{health.Max}[/]");
        table.AddRow("[blue]Location[/]", $"[blue]{location.RoomId}[/]");
        _console.Write(table);
    }

    public void ShowNarrative(string narrative)
    {
        _console.Write(new Panel($"[italic yellow]{Markup.Escape(narrative)}[/]").Border(BoxBorder.Rounded));
        _console.WriteLine();
    }

    public void ShowError(string message) => _console.MarkupLine($"[red]{Markup.Escape(message)}[/]");

    public void ShowInfo(string message) => _console.MarkupLine($"[dim]{Markup.Escape(message)}[/]");

    public void ShowCombatResult(string message) => _console.MarkupLine($"[bold red]{Markup.Escape(message)}[/]");

    public void ShowStatus(string message) => _console.MarkupLine($"[green]{Markup.Escape(message)}[/]");
}
