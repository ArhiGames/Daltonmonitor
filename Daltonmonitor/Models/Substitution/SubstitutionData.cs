using System;
using System.Collections.Generic;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Models.Substitution;

public class SubstitutionData
{
    public required int SubstitutionId { get; init; }
    
    public required DateTimeOffset Date { get; init; }
    
    public required int Lesson { get; init; }
    
    public required Teacher AbsentTeacher { get; init; }

    public Teacher? SubstituteTeacher { get; init; } = null;

    public Lesson? LessonData { get; init; } = null;
    
    public Lesson? SubstituteLesson { get; init; } = null;
    
    public List<Room>? Room { get; init; } = null;
    
    public required List<Room> SubstituteRoom { get; init; }
    
    public List<Class>? ClassDatas { get; init; } = null;
    
    public required SubstitutionFlags SubstitutionFlags { get; init; }
    
    public List<Class>? SubstituteClassDatas { get; init; } = null;

    public SubstitutionType SubstitutionType { get; init; } = SubstitutionType.None;
}