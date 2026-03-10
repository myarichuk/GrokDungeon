using Autofac;
using DefaultEcs;
using GrokDungeon.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using Raven.Client.Documents;
using Raven.Embedded;
using System.ClientModel;

namespace GrokDungeon;

public class GrokDungeonModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register Configuration
        builder.Register(c =>
        {
            var configuration = c.Resolve<IConfiguration>();
            return configuration.GetSection("GrokDungeon").Get<GrokDungeonConfig>() ?? new GrokDungeonConfig();
        }).AsSelf().SingleInstance();

        // Register RavenDB
        builder.Register(c =>
        {
            var config = c.Resolve<GrokDungeonConfig>();

            // Initialize Embedded Server
            try
            {
                EmbeddedServer.Instance.StartServer(new ServerOptions
                {
                    ServerUrl = config.RavenDbUrl
                });
            }
            catch
            {
                // Ignore if already started or other issues during registration in non-critical paths
                // In production, we might want to handle this more robustly
            }

            var store = EmbeddedServer.Instance.GetDocumentStore(config.DatabaseName);
            return store;
        }).As<IDocumentStore>().SingleInstance();

        // Register AI Client
        builder.Register(c =>
        {
            var config = c.Resolve<GrokDungeonConfig>();
            var configuration = c.Resolve<IConfiguration>();

            var providerName = config.AiProviders.SelectedProvider;
            if (!config.AiProviders.Providers.TryGetValue(providerName, out var providerConfig))
            {
                throw new InvalidOperationException($"AI Provider '{providerName}' is not configured.");
            }

            var apiKey = configuration[providerConfig.ApiKeyEnvVar];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException($"{providerConfig.ApiKeyEnvVar} not found in Configuration/Environment Variables.");
            }

            return new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
            {
                Endpoint = new Uri(providerConfig.Endpoint)
            }).AsChatClient(providerConfig.Model);
        }).As<IChatClient>().SingleInstance();

        // Register ECS World
        builder.RegisterType<World>().AsSelf().SingleInstance();

        // Register Game Services
        builder.RegisterType<GameConsole>().AsSelf().SingleInstance();
        builder.RegisterType<DiceService>().AsSelf().SingleInstance();
        builder.RegisterType<CombatResolver>().AsSelf().SingleInstance();
        builder.RegisterType<TagExecutor>().AsSelf().SingleInstance();
        builder.RegisterType<GameEngine>().AsSelf().SingleInstance();
    }
}
