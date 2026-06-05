using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Substitution;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Application.Generator.Components.User;

public class LessonHtmlComponent(TimetableLessonData timetableLessonData) : HtmlComponent
{
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

    protected override void Initialize()
    {
        Room room = timetableLessonData.Rooms[0];
        Teacher teacher = timetableLessonData.Teachers[0];

        IdentifierHtmlComponent? substituteRoomIdentifierHtmlComponent = null;
        IdentifierHtmlComponent? substituteTeacherIdentifierHtmlComponent = null;

        bool isCancelled = false;
        
        DayHtmlComponent dayHtmlComponent = GetOuter<DayHtmlComponent>()!;
        foreach (SubstitutionData substitutionData in timetableLessonData.SubstitutionDatas)
        {
            if (dayHtmlComponent.DateTime.Day != substitutionData.DateTime.Day)
            {
                continue;
            }

            if (substitutionData.SubstitutionType == SubstitutionType.Cancelled)
            {
                isCancelled = true;
                break;
            }

            if (room.RoomId != substitutionData.SubstituteRooms![0].RoomId)
            {
                substituteRoomIdentifierHtmlComponent = 
                    new IdentifierHtmlComponent(IdentifierType.Room, substitutionData.SubstituteRooms![0].RoomId, true);
            }
            if (teacher.TeacherName != substitutionData.SubstituteTeacher!.TeacherName)
            {
                substituteTeacherIdentifierHtmlComponent = 
                    new IdentifierHtmlComponent(IdentifierType.Teacher, substitutionData.SubstituteTeacher!.TeacherName, true);
            }
            break;
        }
        
        IdentifierHtmlComponent roomIdentifierHtmlComponent = new(IdentifierType.Room, room.RoomId, isCancelled);
        AddChildrenToComponent(roomIdentifierHtmlComponent);

        if (substituteRoomIdentifierHtmlComponent is not null)
        {
            AddChildrenToComponent(substituteRoomIdentifierHtmlComponent);
        }

        IdentifierHtmlComponent teacherIdentifierHtmlComponent =
            new(IdentifierType.Teacher, teacher.TeacherName, isCancelled);
        AddChildrenToComponent(teacherIdentifierHtmlComponent);

        if (substituteTeacherIdentifierHtmlComponent is not null)
        {
            AddChildrenToComponent(substituteTeacherIdentifierHtmlComponent);
        }
    }
}