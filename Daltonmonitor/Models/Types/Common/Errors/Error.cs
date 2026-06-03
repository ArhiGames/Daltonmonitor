namespace Daltonmonitor.Models.Types.Common.Errors;

public enum ErrorType
{
    Unknown,
    FileNotFound
}

public record Error(string Id, ErrorType ErrorType, string Description);

public static class Errors
{
    public static Error Unknown { get; } = new("Unknown", ErrorType.Unknown, "Unknown error!");

    public static Error FileNotFound { get; } = new("FileNotFound", ErrorType.FileNotFound, "File not found!");
}