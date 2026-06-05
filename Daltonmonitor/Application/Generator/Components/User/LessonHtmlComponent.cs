using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Application.Generator.Components.User;

public class LessonHtmlComponent : HtmlComponent
{
    private readonly TimetableLessonData _timetableLessonData;
    
    public LessonHtmlComponent(TimetableLessonData timetableLessonData)
    {
        _timetableLessonData = timetableLessonData;
        
        BuildChildrenData();
    }

    private void BuildChildrenData()
    {
        Room room = _timetableLessonData.Rooms[0];
        Teacher teacher = _timetableLessonData.Teachers[0];

        IdentifierHtmlComponent roomIdentifierHtmlComponent = new(IdentifierType.Room, room.RoomId, false);
        AddChildrenToComponent(roomIdentifierHtmlComponent);

        IdentifierHtmlComponent teacherIdentifierHtmlComponent =
            new(IdentifierType.Teacher, teacher.TeacherName, false);
        AddChildrenToComponent(teacherIdentifierHtmlComponent);
    }
    
    public override string GenerateHtml()
    {
        const string htmlHead = "<div class=\"lesson\">";
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