namespace MergeSortUnitTests;

[CollectionDefinition("Logger collection")]
public class LoggerCollection : ICollectionFixture<LoggingFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}