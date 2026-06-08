using System.Collections.Generic;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class FloorHtmlComponent(List<TimetableLessonData> timetableLessonDatas, int floor) : HtmlComponent
{
    public override string GenerateHtml()
    {
        string htmlHead = $"<div class=\"floor\" style=\"--floor: {floor};\">";
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

    protected override void Initialize()
    {
        foreach (TimetableLessonData timetableLessonData in timetableLessonDatas)
        {
            LessonHtmlComponent lessonHtmlComponent = new(timetableLessonData);
            AddChildrenToComponent(lessonHtmlComponent);
        }
    }
}