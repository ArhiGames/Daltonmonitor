using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class FloorHtmlComponent(List<TimetableLessonData> timetableLessonDatas, int floor) : HtmlComponent
{
    protected override void Initialize()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        
        List<TimetableLessonData> orderedLessons = timetableLessonDatas
            .OrderBy(tld => DoesDaltonTypeShowTag(htmlRootComponent.ConfigManager, tld.DaltonType))
            .ThenBy(tld => tld.Rooms[0].RoomId)
            .ToList();
        
        foreach (TimetableLessonData timetableLessonData in orderedLessons)
        {
            LessonHtmlComponent lessonHtmlComponent = new(timetableLessonData);
            AddChildToComponent(lessonHtmlComponent);
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

    private bool DoesDaltonTypeShowTag(ConfigManager configManager, DaltonType daltonType)
    {
        ConfigIdentifier configIdentifier = daltonType switch
        {
            DaltonType.None or DaltonType.Dalton => ConfigIdentifier.None,
            DaltonType.Workshop => ConfigIdentifier.WorkshopLabel,
            DaltonType.Mentor => ConfigIdentifier.MentorLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(daltonType), daltonType, null)
        };

        bool shouldShowTag = configManager.GetConfigValue(configIdentifier) != string.Empty;
        return shouldShowTag;
    }
}