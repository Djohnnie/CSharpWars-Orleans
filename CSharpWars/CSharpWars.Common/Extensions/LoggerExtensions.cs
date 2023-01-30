using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace CSharpWars.Common.Extensions;

public static class LoggerExtensions
{
    public static void AutoLogInformation<TCategoryName>(this ILogger<TCategoryName> logger, string message, [CallerMemberName] string caller = "<unknown caller>")
    {
        logger.LogInformation(
            "{UtcNow:dd-MM-yyyy HH:mm:ss} | {CategoryTypeName}.{Caller} | {Message}",
            DateTime.UtcNow,
            typeof(TCategoryName).Name,
            caller,
            message);
    }
}