using DkToolbox.Core.Models;

namespace DkToolbox.Cli.Extensions;

public static class KillResultExtensions
{
    public static int ToExitCode(this KillResult result)
    {
        return result switch
        {
            KillSuccess => ExitCodes.Success,
            KillFailure failure => failure.Kind switch
            {
                KillFailureKind.NotFound => ExitCodes.NotFound,
                KillFailureKind.AccessDenied => ExitCodes.AccessDenied,
                _ => ExitCodes.UnexpectedError
            },
            _ => ExitCodes.UnexpectedError
        };
    }
}
