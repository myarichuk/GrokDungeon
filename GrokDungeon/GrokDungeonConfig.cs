namespace GrokDungeon;

public class GrokDungeonConfig
{
    public string RavenDbUrl { get; set; } = "http://127.0.0.1:8080";
    public string DatabaseName { get; set; } = "GrokDungeon";

    public AiProvidersConfig AiProviders { get; set; } = new();
}

public class AiProvidersConfig
{
    public string SelectedProvider { get; set; } = "Grok";
    public Dictionary<string, AiProviderDetails> Providers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class AiProviderDetails
{
    public string Endpoint { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ApiKeyEnvVar { get; set; } = string.Empty;
}
