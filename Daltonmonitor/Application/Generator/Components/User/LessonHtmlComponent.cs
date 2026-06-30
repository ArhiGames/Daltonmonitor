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
    private ConfigManager _configManager = null!;
    private DaltonType _relevantDaltonType = DaltonType.None;
    
    protected override void Initialize()
    {
        Room? room = timetableLessonData.Rooms.Count > 0 ? timetableLessonData.Rooms[0] : null;
        Teacher? teacher = timetableLessonData.Teachers.Count > 0 ? timetableLessonData.Teachers[0] : null;
        if (room is null || teacher is null)
        {
            // todo remove from parent
            return;
        }
        
        IdentifierHtmlComponent? substituteRoomIdentifierHtmlComponent = null;
        IdentifierHtmlComponent? substituteTeacherIdentifierHtmlComponent = null;

        _configManager = GetOuter<HtmlRootComponent>()!.ConfigManager;
        DayHtmlComponent dayHtmlComponent = GetOuter<DayHtmlComponent>()!;

        // The dalton type might change due to substitution, as so, the relevant dalton type stands for the relevant dalton type
        _relevantDaltonType = timetableLessonData.DaltonType;
        string? additionalInformation = null;
        
        foreach (SubstitutionData substitutionData in timetableLessonData.SubstitutionDatas)
        {
            if (dayHtmlComponent.DateTime.Date != substitutionData.DateTime.Date)
            {
                continue;
            }

            if (substitutionData.SubstitutionType == SubstitutionType.Cancelled)
            {
                _isCancelled = true;
                break;
            }

            if (substitutionData.SubstituteRooms is not null && substitutionData.SubstituteRooms.Count > 0)
            {
                Room substituteRoom = substitutionData.SubstituteRooms[0];
                if (room.RoomId != substituteRoom.RoomId)
                {
                    substituteRoomIdentifierHtmlComponent = 
                        new IdentifierHtmlComponent(IdentifierType.Room, substituteRoom.RoomId, true);
                }
            }

            if (substitutionData.SubstituteTeacher is not null)
            {
                Teacher substituteTeacher = substitutionData.SubstituteTeacher;
                if (teacher.TeacherName != substituteTeacher.TeacherName)
                {
                    substituteTeacherIdentifierHtmlComponent = 
                        new IdentifierHtmlComponent(IdentifierType.Teacher, substituteTeacher.TeacherName, true);
                }
            }

            _relevantDaltonType = substitutionData.OverrideDaltonType ?? _relevantDaltonType;
            additionalInformation = substitutionData.AdditionalInformation;
            
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
            _configManager.GetConfigValue(ConfigIdentifier.HighlightMentorDalton) == "true";
        if (_relevantDaltonType == DaltonType.Mentor && highlightMentorLesson)
        {
            Class? classData = timetableLessonData.Classes.Count > 0 ? timetableLessonData.Classes[0] : null;
            if (classData is not null)
            {
                MetaLessonInfoHtmlComponent metaLessonInfoHtmlComponent =
                    new(MetadataType.Mentor, classData.ClassDescriptor);
                AddChildToComponent(metaLessonInfoHtmlComponent);
            }
        }

        if (additionalInformation is not null)
        {
            MetaLessonInfoHtmlComponent metaLessonInfoHtmlComponent =
                new(MetadataType.Information, additionalInformation);
            AddChildToComponent(metaLessonInfoHtmlComponent);
        }
    }
    
    public override string GenerateHtml()
    {
        string removedString = _isCancelled ? "removed" : "";
        string htmlHead = $"<div class=\"lesson {removedString}\">";
        const string htmlBack = "</div>";
        
        bool enableInlineTags = _configManager.GetConfigValue(ConfigIdentifier.EnableInlineTags) == "true";
        LabelHtmlComponent? labelHtmlComponent = GetLabelHtmlComponent(_relevantDaltonType);

        StringBuilder stringBuilder = new();
        if (!enableInlineTags && labelHtmlComponent is not null) stringBuilder.Append(labelHtmlComponent.GenerateHtml());
        stringBuilder.Append(htmlHead); 
        if (enableInlineTags && labelHtmlComponent is not null) stringBuilder.Append(labelHtmlComponent.GenerateHtml());
        
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);
        return stringBuilder.ToString();
    }

    private LabelHtmlComponent? GetLabelHtmlComponent(DaltonType daltonType)
    {
        ConfigIdentifier configIdentifier = daltonType switch
        {
            DaltonType.None or DaltonType.Dalton => ConfigIdentifier.None,
            DaltonType.Workshop => ConfigIdentifier.WorkshopLabel,
            DaltonType.Mentor => ConfigIdentifier.MentorLabel,
            DaltonType.Bound => ConfigIdentifier.BoundDaltonLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(daltonType), daltonType, null)
        };
        if (configIdentifier == ConfigIdentifier.None)
        {
            return null;
        }

        string labelString = _configManager.GetConfigValue(configIdentifier);
        if (labelString == string.Empty)
        {
            return null;
        }

        LabelHtmlComponent labelHtmlComponent = new(labelString);
        return labelHtmlComponent;
    }
}