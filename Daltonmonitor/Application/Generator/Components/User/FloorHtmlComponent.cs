using System.Collections.Generic;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class FloorHtmlComponent : HtmlComponent
{
    private readonly List<TimetableLessonData> _timetableLessonDatas;
    private readonly int _floor;

    public FloorHtmlComponent(List<TimetableLessonData> timetableLessonDatas, int floor)
    {
        _timetableLessonDatas = timetableLessonDatas;
        _floor = floor;
        
        BuildChildrenData();
    }

    private void BuildChildrenData()
    {
        foreach (TimetableLessonData timetableLessonData in _timetableLessonDatas)
        {
            LessonHtmlComponent lessonHtmlComponent = new(timetableLessonData);
            AddChildrenToComponent(lessonHtmlComponent);
        }
    }
    
    public override string GenerateHtml()
    {
        string htmlHead = $"<div class=\"floor\" data-floor-index=\"{_floor}\">";
        const string htmlBack = "</div>";

        StringBuilder stringBuilder = new();
        stringBuilder.Append(htmlHead);
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);
        return stringBuilder.ToString();
    }
}