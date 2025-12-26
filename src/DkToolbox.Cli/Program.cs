using DkToolbox.Cli.Commands;
using DkToolbox.Cli.Infrastructure;
using DkToolbox.Core.Abstractions;
using DkToolbox.Platform.Windows;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Reflection;

IServiceCollection services = new ServiceCollection();
services.AddSingleton<IProcessService, WindowsProcessService>();
services.AddSingleton<IPortService, WindowsPortService>();

TypeRegistrar registrar = new(services);
CommandApp app = new(registrar);

app.Configure(config =>
{
    config.SetApplicationName("dktoolbox");
    config.SetApplicationVersion(GetVersion());

    config.AddBranch("proc", proc =>
    {
        proc.SetDescription("Process utilities");
        proc.AddCommand<ProcListCommand>("list")
            .WithDescription("List running processes");
    });
});

return app.Run(args);

static string GetVersion()
{
    Assembly? assembly = Assembly.GetEntryAssembly();
    return assembly?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion
        ?? assembly?.GetName().Version?.ToString()
        ?? "dev";
}
