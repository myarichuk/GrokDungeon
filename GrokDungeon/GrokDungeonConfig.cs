namespace GrokDungeon;

public class GrokDungeonConfig
{
    public string RavenDbUrl { get; set; } = "http://127.0.0.1:8080";
    public string DatabaseName { get; set; } = "GrokDungeon";
    public string AiModel { get; set; } = "grok-beta";
    public string AiEndpoint { get; set; } = "https://api.x.ai/v1";
}