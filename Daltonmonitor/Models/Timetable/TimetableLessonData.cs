using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Models.Timetable;

public class TimetableLessonData(int lessonId, DaltonType daltonType, Teacher teacher, Room room, EDay day, int lesson)
{
    public int LessonId { get; init; } = lessonId;

    public DaltonType DaltonType { get; init; } = daltonType;

    public Teacher Teacher { get; init; } = teacher;

    public Room Room { get; init; } = room;

    public EDay Day { get; init; } = day;

    public int Lesson { get; init; } = lesson;
}