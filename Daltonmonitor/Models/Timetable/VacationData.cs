using System;

namespace Daltonmonitor.Models.Timetable;

public class VacationData(string vacationName, string vacationDescription, DateTime vacationStartDate, DateTime vacationEndDate, bool isOffDay)
{
    public string VacationName { get; } = vacationName;
    
    public string VacationDescription { get; } = vacationDescription;
    
    public DateTime VacationStartDate { get; } = vacationStartDate;
    
    public DateTime VacationEndDate { get; } = vacationEndDate;
    
    public bool IsOffDay { get; } = isOffDay;
}