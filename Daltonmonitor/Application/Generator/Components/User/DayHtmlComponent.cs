using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class DayHtmlComponent(Timetable timetable, DateTime dateTime) : HtmlComponent
{
    public DateTime DateTime { get; } = dateTime;

    public override string GenerateHtml()
    {
        // todo calculate floor count dynamically
        string htmlHead = $"<div class=\"day\" style=\"--floor-count: {4};\">";
        
        string dateTimeString = DateTime.ToShortDateString();
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

    protected override void Initialize()
    {
        Dictionary<int, List<TimetableLessonData>> lessonTimetableLessonDatas = timetable.TimetableLessonDatas
            .Where(tld => tld.Day == DateTime.DayOfWeek)
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
}