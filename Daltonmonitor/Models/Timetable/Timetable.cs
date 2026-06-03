using System.Collections.Generic;
using System.Linq;
using Daltonmonitor.Models.Types;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Models.Timetable;

public class Timetable
{
    private readonly List<TimetableLessonData> _timetableLessonDatas = [];
    public IReadOnlyCollection<TimetableLessonData> TimetableLessonDatas => _timetableLessonDatas.AsReadOnly();

    public Timetable(int lessonCount)
    {
        _timetableLessonDatas.EnsureCapacity(lessonCount);
    }
    
    public Result AddDaltonLesson(TimetableLessonData timetableLessonData)
    {
        _timetableLessonDatas.Add(timetableLessonData);
        return Result.Success();
    }

    public List<TimetableLessonData> GetLessonDataForDay(EDay day)
    {
        return TimetableLessonDatas.Where(tld => tld.Day == day).ToList();
    }
}