using System;
using System.Collections.Generic;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Models.Substitution;

public class SubstitutionData(int substitutionId, DateTime dateTime, int lesson, Teacher substituteTeacher, 
    List<Room> substituteRooms, DaltonType? overrideDaltonType, SubstitutionFlags substitutionFlags, SubstitutionType substitutionType,
    string? additionalInformation)
{
    public int SubstitutionId { get; init; } = substitutionId;

    public DateTime DateTime { get; init; } = dateTime;

    public int Lesson { get; init; } = lesson;

    public Teacher? SubstituteTeacher { get; init; } = substituteTeacher;

    public List<Room>? SubstituteRooms { get; init; } = substituteRooms;

    public DaltonType? OverrideDaltonType { get; init; } = overrideDaltonType;

    public SubstitutionFlags SubstitutionFlags { get; init; } = substitutionFlags;

    public SubstitutionType SubstitutionType { get; init; } = substitutionType;

    public string? AdditionalInformation { get; init; } = additionalInformation;
}