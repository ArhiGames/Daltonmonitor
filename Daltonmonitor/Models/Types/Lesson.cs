namespace Daltonmonitor.Models.Types;

public class Lesson(string lessonShortcut)
{
    public required string LessonShortcut { get; init; } = lessonShortcut;
}