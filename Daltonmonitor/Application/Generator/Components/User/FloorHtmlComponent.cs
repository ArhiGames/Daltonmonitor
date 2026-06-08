using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class FloorHtmlComponent(List<TimetableLessonData> timetableLessonDatas, int floor) : HtmlComponent
{
    protected override void Initialize()
    {
        List<TimetableLessonData> orderedLessons = timetableLessonDatas.OrderBy(tld => tld.Rooms[0].RoomId).ToList(); 
        foreach (TimetableLessonData timetableLessonData in orderedLessons)
        {
            LessonHtmlComponent lessonHtmlComponent = new(timetableLessonData);
            AddChildrenToComponent(lessonHtmlComponent);
        }
    }
    
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
}