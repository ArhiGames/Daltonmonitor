using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class DayHtmlComponent : HtmlComponent
{
    private readonly Timetable _timetable;
    private readonly DateTime _dateTime;

    public DayHtmlComponent(Timetable timetable, DateTime dateTime)
    {
        _timetable = timetable;
        _dateTime = dateTime;
        
        BuildChildrenData();
    }

    private void BuildChildrenData()
    {
        Dictionary<int, List<TimetableLessonData>> lessonTimetableLessonDatas = _timetable.TimetableLessonDatas
            .Where(tld => tld.Day == _dateTime.DayOfWeek)
            .GroupBy(tld => tld.Lesson)
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        foreach (KeyValuePair<int, List<TimetableLessonData>> pair in lessonTimetableLessonDatas)
        {
            PeriodHtmlComponent periodHtmlComponent = new(pair.Value, pair.Key);
            AddChildrenToComponent(periodHtmlComponent);
        }
    }
    
    public override string GenerateHtml()
    {
        const string htmlHead = "<div class=\"day\">";
        
        string dateTimeString = _dateTime.ToShortDateString();
        string htmlDay = $"<h1 class=\"date\">{dateTimeString}</h1>";
        
        const string htmlBack = "</div>";

        StringBuilder stringBuilder = new();
        stringBuilder.Append(htmlHead);
        stringBuilder.Append(htmlDay);
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);

        return stringBuilder.ToString();
    }
}