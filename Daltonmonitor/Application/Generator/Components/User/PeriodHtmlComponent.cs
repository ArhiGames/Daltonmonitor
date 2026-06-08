using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Application.Generator.Components.User;

public class PeriodHtmlComponent(List<TimetableLessonData> lessonDatas, int lesson) : HtmlComponent
{
    public override string GenerateHtml()
    {
        const string htmlHead = "<div class=\"period\">";
        string htmlHour = $"<h1>{lesson}. Stunde</h1>";
        const string htmlBack = "</div>";

        StringBuilder stringBuilder = new();
        stringBuilder.Append(htmlHead);
        stringBuilder.Append(htmlHour);
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);

        return stringBuilder.ToString();
    }

    protected override void Initialize()
    {
        /* floor, data*/
        Dictionary<int, List<TimetableLessonData>> timetableLessonDatas = lessonDatas
            .Where(tld => tld.Lesson == lesson)
            .GroupBy(tld => GetFloorByRoom(tld.Rooms.Count > 0 ? tld.Rooms[0] : null))
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        foreach (KeyValuePair<int, List<TimetableLessonData>> keyValuePair in timetableLessonDatas)
        {
            FloorHtmlComponent floorHtmlComponent = new(keyValuePair.Value, keyValuePair.Key);
            AddChildrenToComponent(floorHtmlComponent);
        }
    }
    
    private int GetFloorByRoom(Room? room)
    {
        if (room is null)
        {
            return 1;
        }
        
        // todo read from config
        char startChar = room.RoomId[0];
        return startChar switch
        {
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            _ => 1
        };
    }
}