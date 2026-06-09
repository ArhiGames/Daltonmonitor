using System;
using System.Collections.Generic;
using Daltonmonitor.Models.Substitution;
using Daltonmonitor.Models.Types;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Models.Timetable;

public class TimetableLessonData(int lessonId, DaltonType daltonType, List<Class> classes, List<Teacher> teachers, List<Room> rooms, DayOfWeek day, int lesson)
{
    public int LessonId { get; init; } = lessonId;

    public DaltonType DaltonType { get; init; } = daltonType;

    public List<Class> Classes { get; init; } = classes;

    public List<Teacher> Teachers { get; init; } = teachers;

    public List<Room> Rooms { get; init; } = rooms;

    public DayOfWeek Day { get; init; } = day;

    public int Lesson { get; init; } = lesson;

    public List<SubstitutionData> SubstitutionDatas { get; } = [];

    public Result AddTeachers(List<Teacher> teachers)
    {
        Teachers.AddRange(teachers);
        return Result.Success();
    }
    
    public Result AddSubstitutionData(SubstitutionData substitutionData)
    {
        SubstitutionDatas.Add(substitutionData);
        return Result.Success();
    }
}