using System.Diagnostics.CodeAnalysis;

namespace PageTracker.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Returns true if the string is not null or whitespace.
    /// </summary>
    public static bool HasValue([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
