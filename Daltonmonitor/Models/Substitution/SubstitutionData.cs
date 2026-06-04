using System;
using System.Collections.Generic;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Models.Substitution;

public class SubstitutionData(int substitutionId, DateTime date, int lesson, Teacher substituteTeacher, 
    List<Room> substituteRooms, SubstitutionFlags substitutionFlags, SubstitutionType substitutionType)
{
    public int SubstitutionId { get; init; } = substitutionId;

    public DateTime Date { get; init; } = date;

    public int Lesson { get; init; } = lesson;

    public Teacher? SubstituteTeacher { get; init; } = substituteTeacher;

    public List<Room>? SubstituteRooms { get; init; } = substituteRooms;

    public SubstitutionFlags SubstitutionFlags { get; init; } = substitutionFlags;

    public SubstitutionType SubstitutionType { get; init; } = substitutionType;
}