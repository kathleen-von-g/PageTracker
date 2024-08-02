namespace PageTracker.Common.Exceptions;

/// <summary>
/// Application exceptions should have messages that would be readable by the end-user
/// </summary>
public class ApplicationException : Exception
{
    public ApplicationException(string? message) : base(message) { }

    public ApplicationException(string? message, Exception? innerException) : base(message, innerException) { }
}
