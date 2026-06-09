namespace Daltonmonitor.Models.Types.Common.Errors;

public enum ErrorType
{
    Unknown,
    FileNotFound,
    FileError,
    InvalidUserMode
}

public record Error(string Id, ErrorType ErrorType, string Description);

public static class Errors
{
    public static Error Unknown { get; } = new("Unknown", ErrorType.Unknown, "Unknown error!");

    public static Error FileNotFound { get; } = new("FileNotFound", ErrorType.FileNotFound, "File not found!");

    public static Error FileError { get; } = new("FileError", ErrorType.FileError, "Unspecified file error!");

    public static Error InvalidUserMode { get; } =
        new("InvalidUserMode", ErrorType.InvalidUserMode, "Invalid user mode!");
}