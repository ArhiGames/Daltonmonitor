using System;

namespace Daltonmonitor.Models.Substitution;

[Flags]
public enum SubstitutionFlags
{
    Cancelled = 1 << 0,
    Supervision = 1 << 1,
    SpecialAssignment = 1 << 2,
    RouteChange = 1 << 3,
    Release = 1 << 4,
    PlusAsSubstitute = 1 << 5,
    PartialSubstitution = 1 << 6,
    Transfer = 1 << 7,
    ClassSubstitution = 1 << 16,
    BreakSupervisionSubstitution = 1 << 17,
    NoLessonsDuringThisPeriod = 1 << 18,
    DoNotPrintCode = 1 << 20,
    NewCode = 1 << 21
}