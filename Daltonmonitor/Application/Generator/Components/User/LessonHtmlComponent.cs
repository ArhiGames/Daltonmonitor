using System;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Substitution;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;

namespace Daltonmonitor.Application.Generator.Components.User;

public class LessonHtmlComponent(TimetableLessonData timetableLessonData) : HtmlComponent
{
    private bool _isCancelled;
    
    protected override void Initialize()
    {
        Room room = timetableLessonData.Rooms[0];
        Teacher teacher = timetableLessonData.Teachers[0];

        IdentifierHtmlComponent? substituteRoomIdentifierHtmlComponent = null;
        IdentifierHtmlComponent? substituteTeacherIdentifierHtmlComponent = null;

        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        DayHtmlComponent dayHtmlComponent = GetOuter<DayHtmlComponent>()!;
        
        switch (timetableLessonData.DaltonType)
        {
            case DaltonType.None:
            case DaltonType.Dalton:
                break;
            case DaltonType.Workshop:
            {
                LabelHtmlComponent labelHtmlComponent = new(LabelType.Workshop);
                AddChildToComponent(labelHtmlComponent);
                break;
            }
            case DaltonType.Mentor:
            {
                LabelHtmlComponent labelHtmlComponent = new(LabelType.Mentor);
                AddChildToComponent(labelHtmlComponent);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        foreach (SubstitutionData substitutionData in timetableLessonData.SubstitutionDatas)
        {
            if (dayHtmlComponent.DateTime.Day != substitutionData.DateTime.Day)
            {
                continue;
            }

            if (substitutionData.SubstitutionType == SubstitutionType.Cancelled)
            {
                _isCancelled = true;
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
        
        IdentifierHtmlComponent roomIdentifierHtmlComponent = new(IdentifierType.Room, room.RoomId, _isCancelled);
        AddChildToComponent(roomIdentifierHtmlComponent);

        if (substituteRoomIdentifierHtmlComponent is not null)
        {
            AddChildToComponent(substituteRoomIdentifierHtmlComponent);
        }

        IdentifierHtmlComponent teacherIdentifierHtmlComponent =
            new(IdentifierType.Teacher, teacher.TeacherName, _isCancelled);
        AddChildToComponent(teacherIdentifierHtmlComponent);

        if (substituteTeacherIdentifierHtmlComponent is not null)
        {
            AddChildToComponent(substituteTeacherIdentifierHtmlComponent);
        }

        bool highlightMentorLesson =
            htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.HighlightMentorDalton) == "true";
        if (timetableLessonData.DaltonType == DaltonType.Mentor && highlightMentorLesson)
        {
            Class? classData = timetableLessonData.Classes.Count > 0 ? timetableLessonData.Classes[0] : null;
            if (classData is not null)
            {
                MetaLessonInfoHtmlComponent metaLessonInfoHtmlComponent =
                    new(MetadataType.Mentor, classData.ClassDescriptor);
                AddChildToComponent(metaLessonInfoHtmlComponent);
            }
        } 
    }
    
    public override string GenerateHtml()
    {
        string removedString = _isCancelled ? "removed" : "";
        string htmlHead = $"<div class=\"lesson {removedString}\">";
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