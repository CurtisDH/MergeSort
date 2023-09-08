using Microsoft.Extensions.Logging;

namespace MergeSortUnitTests;

public class LoggingFixture
{
    public ILogger<MergeSortTests> Logger { get; }

    public LoggingFixture()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = loggerFactory.CreateLogger<MergeSortTests>();
    }
}