using System.Globalization;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DkToolbox.Cli.Commands;

public sealed class ProcListCommand(IProcessService processes) : Command<ProcListCommand.Settings>
{
    private const double BytesToMegabytes = 1024.0 * 1024.0;

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--name <TEXT>")]
        public string? Name { get; init; }

        [CommandOption("--top <N>")]
        public int? Top { get; init; }

        [CommandOption("--sort <name|mem>")]
        public string Sort { get; } = "name";
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        ProcessSort sortOrder = settings.Sort.Equals("mem", StringComparison.OrdinalIgnoreCase)
            ? ProcessSort.Memory
            : ProcessSort.Name;

        IReadOnlyList<ProcessInfo> processList = processes.List(new ProcessQuery(settings.Name, settings.Top, sortOrder));

        Table processTable = new Table().AddColumn("PID").AddColumn("Name").AddColumn("Memory (MB)");

        foreach (ProcessInfo processInfo in processList)
        {
            string memoryInMegabytes = (processInfo.WorkingSetBytes / BytesToMegabytes).ToString("F1", CultureInfo.InvariantCulture);
            processTable.AddRow(processInfo.Pid.ToString(CultureInfo.InvariantCulture), processInfo.Name, memoryInMegabytes);
        }

        AnsiConsole.Write(processTable);
        return ExitCodes.Success;
    }
}
