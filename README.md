# DkToolbox

A cross-platform command-line toolbox for system administration and diagnostics.

## Features

### Process Management
- **List processes** - View running processes with filtering and sorting
- **Kill processes** - Terminate processes by PID with confirmation prompts

### Port Management
- **Port ownership** - Discover which process is using a specific port (TCP/UDP)

## Installation

### Prerequisites
- .NET 10.0 SDK or later

### Build from Source
```bash
git clone <repository-url>
cd DkToolbox
dotnet build
dotnet run --project src/DkToolbox.Cli
```

### Install as Global Tool (Coming Soon)
```bash
dotnet tool install -g DkToolbox
```

## Usage

### Process Commands

#### List Running Processes
```bash
# List all processes
dktoolbox proc list

# Filter by name
dktoolbox proc list --name chrome

# Show top 10 by memory
dktoolbox proc list --top 10 --sort mem

# JSON output
dktoolbox proc list --json
```

#### Kill a Process
```bash
# Kill with confirmation prompt
dktoolbox proc kill 1234

# Kill without confirmation
dktoolbox proc kill 1234 --yes

# Kill process tree
dktoolbox proc kill 1234 --tree

# Force kill
dktoolbox proc kill 1234 --force
```

### Port Commands

#### Find Process Using a Port
```bash
# Check TCP port (default)
dktoolbox port who 8080

# Check UDP port
dktoolbox port who 53 --udp

# JSON output
dktoolbox port who 443 --tcp --json
```

## Exit Codes

| Code | Meaning |
|------|---------|
| 0 | Success |
| 2 | Invalid arguments |
| 3 | Not found (process/port) |
| 5 | Access denied (try elevated shell) |
| 10 | Unexpected error |

## Examples

### Find and kill a process on a specific port
```bash
# Find what's on port 8080
dktoolbox port who 8080

# Kill the process (using PID from above)
dktoolbox proc kill 12345 --yes
```

### Monitor memory usage
```bash
# Show top 20 processes by memory
dktoolbox proc list --top 20 --sort mem
```

### Scripting with JSON output
```bash
# Export process list to file
dktoolbox proc list --json > processes.json

# Export port ownership
dktoolbox port who 443 --tcp --json > port-443.json
```

## Platform Support

Currently supports:
- ✅ Windows 10/11

Planned support:
- ⏳ Linux
- ⏳ macOS

## Architecture

- **DkToolbox.Core** - Platform-independent models and interfaces
- **DkToolbox.Platform.Windows** - Windows-specific implementations using native APIs
- **DkToolbox.Cli** - Command-line interface built with Spectre.Console

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [Spectre.Console](https://spectreconsole.net/) for rich terminal UI
- Uses Windows IP Helper API for accurate port ownership information
