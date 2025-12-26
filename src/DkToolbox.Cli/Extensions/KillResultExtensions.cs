using DkToolbox.Core.Models;

namespace DkToolbox.Cli.Extensions;

public static class KillResultExtensions
{
    public static int ToExitCode(this KillResult result)
    {
        if (result.Success)
        {
            return ExitCodes.Success;
        }

        return result.FailureKind switch
        {
            KillFailureKind.NotFound => ExitCodes.NotFound,
            KillFailureKind.AccessDenied => ExitCodes.AccessDenied,
            _ => ExitCodes.UnexpectedError
        };
    }
}
