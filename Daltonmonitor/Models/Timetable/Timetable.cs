using System;
using System.Collections.Generic;
using System.Linq;

namespace Daltonmonitor.Models.Timetable;

public class Timetable
{
    private readonly List<TimetableLessonData> _timetableLessonDatas = [];
    public IReadOnlyCollection<TimetableLessonData> TimetableLessonDatas => _timetableLessonDatas.AsReadOnly();

    private readonly List<VacationData> _vacationDatas = [];
    public IReadOnlyCollection<VacationData> VacationDatas => _vacationDatas.AsReadOnly();

    public Timetable(int lessonCount)
    {
        _timetableLessonDatas.EnsureCapacity(lessonCount);
    }
    
    public void AddDaltonLesson(TimetableLessonData timetableLessonData)
    {
        _timetableLessonDatas.Add(timetableLessonData);
    }

    public void AddVacationData(VacationData vacationData)
    {
        _vacationDatas.Add(vacationData);
    }

    public VacationData? GetVacationData(DateTime date)
    {
        return _vacationDatas.FirstOrDefault(vacationData => vacationData.VacationStartDate <= date && vacationData.VacationEndDate >= date);
    }
}