using DkToolbox.Cli.Extensions;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DkToolbox.Cli.Commands;

public sealed class ProcKillCommand(IProcessService processes) : Command<ProcKillCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PID>")]
        public int Pid { get; init; }

        [CommandOption("--force")]
        public bool Force { get; init; }

        [CommandOption("--tree")]
        public bool Tree { get; init; }

        [CommandOption("--yes")]
        public bool Yes { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        // Lookup PID -> show name (if not found, exit 3)
        IReadOnlyList<ProcessInfo> processList = processes.List(new ProcessQuery(null, null, ProcessSort.Name));
        ProcessInfo? targetProcess = processList.FirstOrDefault(p => p.Pid == settings.Pid);

        if (targetProcess is null)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] Process with PID {settings.Pid} not found.");
            return ExitCodes.NotFound;
        }

        // If not --yes: confirm
        if (!settings.Yes)
        {
            string treeText = settings.Tree ? " and its children" : "";
            if (!AnsiConsole.Confirm($"Kill PID {settings.Pid} ({targetProcess.Name}){treeText}?", defaultValue: false))
            {
                AnsiConsole.MarkupLine("[yellow]Cancelled.[/]");
                return ExitCodes.Success;
            }
        }

        // Call Kill() -> print result -> return result.ToExitCode()
        KillOptions options = new KillOptions(settings.Force, settings.Tree);
        KillResult result = processes.Kill(settings.Pid, options);

        return result switch
        {
            KillSuccess => HandleSuccess(result.Pid),
            KillFailure failure => HandleFailure(failure),
            _ => ExitCodes.UnexpectedError
        };
    }

    private static int HandleSuccess(int pid)
    {
        AnsiConsole.MarkupLine($"[green]Success:[/] Process {pid} killed.");
        return ExitCodes.Success;
    }

    private static int HandleFailure(KillFailure failure)
    {
        string errorMessage = failure.Kind switch
        {
            KillFailureKind.NotFound => $"Process {failure.Pid} not found or already exited.",
            KillFailureKind.AccessDenied => $"Access denied killing process {failure.Pid}. Try elevated shell.",
            _ => $"Unexpected error killing process {failure.Pid}: {failure.Error}"
        };

        AnsiConsole.MarkupLine($"[red]Error:[/] {errorMessage}");
        return failure.ToExitCode();
    }
}
