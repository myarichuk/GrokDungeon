using Autofac;
using Autofac.Extensions.DependencyInjection;
using GrokDungeon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

builder.ConfigureContainer(new AutofacServiceProviderFactory(container =>
{
    container.RegisterAssemblyTypes(typeof(Program).Assembly)
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
}));

var host = builder.Build();
var engine = host.Services.GetRequiredService<GameEngine>();

await engine.InitializeAsync();
await engine.RunLoopAsync();