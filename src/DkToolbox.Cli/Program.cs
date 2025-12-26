using DkToolbox.Cli.Infrastructure;
using DkToolbox.Cli.Commands;
using DkToolbox.Core.Abstractions;
using DkToolbox.Platform.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IProcessService, WindowsProcessService>();
        services.AddSingleton<IPortService, WindowsPortService>();
    })
    .Build();

TypeRegistrar registrar = new(host.Services);
CommandApp app = new(registrar);

app.Configure(config =>
{
    config.SetApplicationName("dktoolbox");
    config.SetApplicationVersion("1.0.0");

    config.AddCommand<ProcListCommand>("proc:list")
        .WithDescription("List running processes");
});

return app.Run(args);
