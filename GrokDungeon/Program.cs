using Autofac;
using Autofac.Extensions.DependencyInjection;
using GrokDungeon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>();

// Use Autofac
builder.ConfigureContainer(new AutofacServiceProviderFactory());

builder.ConfigureContainer<ContainerBuilder>((containerBuilder) =>
{
    containerBuilder.RegisterInstance(builder.Configuration).As<IConfiguration>();
    containerBuilder.RegisterModule(new GrokDungeonModule());
});

var host = builder.Build();

// Run Game
using (var scope = host.Services.CreateScope())
{
    var engine = scope.ServiceProvider.GetRequiredService<GameEngine>();
    await engine.InitializeAsync();
    await engine.RunLoopAsync();
}