using DkToolbox.Cli;
using DkToolbox.Cli.Commands;
using DkToolbox.Cli.Infrastructure;
using DkToolbox.Core.Abstractions;
using DkToolbox.Platform.Windows;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;
using System.Runtime.InteropServices;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    AnsiConsole.MarkupLine("[red]Error:[/] This application currently only supports Windows.");
    AnsiConsole.MarkupLine("[yellow]Supported platforms will be added in future releases.[/]");
    return ExitCodes.UnexpectedError;
}

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
        proc.AddCommand<ProcKillCommand>("kill")
            .WithDescription("Kill a process by PID");
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
