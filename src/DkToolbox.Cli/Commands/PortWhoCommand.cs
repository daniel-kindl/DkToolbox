using System.Globalization;
using System.Text.Json;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DkToolbox.Cli.Commands;

public sealed class PortWhoCommand(IPortService portService) : Command<PortWhoCommand.Settings>
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PORT>")]
        public int Port { get; init; }

        [CommandOption("--tcp")]
        public bool Tcp { get; init; }

        [CommandOption("--udp")]
        public bool Udp { get; init; }

        [CommandOption("--json")]
        public bool Json { get; init; }

        public override ValidationResult Validate()
        {
            if (Port is < 1 or > 65535)
            {
                return ValidationResult.Error("Port must be between 1 and 65535");
            }

            if (Tcp && Udp)
            {
                return ValidationResult.Error("Cannot specify both --tcp and --udp");
            }

            return ValidationResult.Success();
        }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        PortProtocol protocol = settings.Udp ? PortProtocol.Udp : PortProtocol.Tcp;
        PortQuery query = new PortQuery(settings.Port, protocol);

        IReadOnlyList<PortOwnerInfo> owners = portService.WhoOwns(query);

        if (owners.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No process found listening on port {settings.Port} ({protocol}).[/]");
            return ExitCodes.NotFound;
        }

        if (settings.Json)
        {
            OutputJson(owners);
        }
        else
        {
            OutputTable(owners);
        }

        return ExitCodes.Success;
    }

    private static void OutputJson(IReadOnlyList<PortOwnerInfo> owners)
    {
        string json = JsonSerializer.Serialize(owners, _jsonOptions);
        Console.Out.WriteLine(json);
    }

    private static void OutputTable(IReadOnlyList<PortOwnerInfo> owners)
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Expand()
            .AddColumn("Proto")
            .AddColumn("Local Endpoint")
            .AddColumn(new TableColumn("PID").RightAligned())
            .AddColumn("Process Name");

        foreach (PortOwnerInfo owner in owners)
        {
            table.AddRow(
                owner.Protocol.ToString().ToUpperInvariant(),
                owner.LocalEndpoint,
                owner.Pid.ToString(CultureInfo.InvariantCulture),
                owner.ProcessName);
        }

        AnsiConsole.Write(table);
    }
}
