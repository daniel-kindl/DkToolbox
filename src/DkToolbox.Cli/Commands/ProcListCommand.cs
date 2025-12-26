using System.Globalization;
using System.Text.Json;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DkToolbox.Cli.Commands;

public sealed class ProcListCommand(IProcessService processes) : Command<ProcListCommand.Settings>
{
    private const double BytesToMegabytes = 1024.0 * 1024.0;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--name <TEXT>")]
        public string? Name { get; init; }

        [CommandOption("--top <N>")]
        public int? Top { get; init; }

        [CommandOption("--sort <name|mem>")]
        public string Sort { get; init; } = "name";

        [CommandOption("--json")]
        public bool Json { get; init; }

        public override ValidationResult Validate()
        {
            if (Top is < 1)
            {
                return ValidationResult.Error("--top must be a positive number (>= 1)");
            }

            string sortLower = Sort.ToLowerInvariant();
            if (sortLower != "name" && sortLower != "mem" && sortLower != "memory")
            {
                return ValidationResult.Error("--sort must be 'name', 'mem', or 'memory'");
            }

            return ValidationResult.Success();
        }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        string sortLower = settings.Sort.ToLowerInvariant();
        ProcessSort sortOrder = sortLower is "mem" or "memory" ? ProcessSort.Memory : ProcessSort.Name;

        IReadOnlyList<ProcessInfo> processList = processes.List(new ProcessQuery(settings.Name, settings.Top, sortOrder));

        if (settings.Json)
        {
            OutputJson(processList);
        }
        else
        {
            OutputTable(processList);
        }

        return ExitCodes.Success;
    }

    private static void OutputJson(IReadOnlyList<ProcessInfo> processList)
    {
        string json = JsonSerializer.Serialize(processList, _jsonOptions);
        AnsiConsole.WriteLine(json);
    }

    private static void OutputTable(IReadOnlyList<ProcessInfo> processList)
    {
        Table processTable = new Table()
            .AddColumn("PID")
            .AddColumn("Name")
            .AddColumn("Memory (MB)");

        foreach (ProcessInfo processInfo in processList)
        {
            string memoryInMegabytes = (processInfo.WorkingSetBytes / BytesToMegabytes)
                .ToString("F1", CultureInfo.InvariantCulture);

            processTable.AddRow(
                processInfo.Pid.ToString(CultureInfo.InvariantCulture),
                processInfo.Name,
                memoryInMegabytes);
        }

        AnsiConsole.Write(processTable);
    }
}
