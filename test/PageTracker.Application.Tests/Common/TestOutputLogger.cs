using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace PageTracker.Application.Tests.Common;
public class TestOutputLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        output.WriteLine("{0} {1}", logLevel, formatter(state, exception));
    }
}
