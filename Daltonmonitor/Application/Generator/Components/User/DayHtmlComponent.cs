using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Application.Managers.Variants;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class DayHtmlComponent(Timetable timetable, DateTime dateTime) : HtmlComponent
{
    public DateTime DateTime { get; } = dateTime;
    public int ExtraColumns { get; set; } = 0;

    private ConfigManager _configManager = null!;
    private VariantsManager _variantsManager = null!;

    protected override void Initialize()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        _configManager = htmlRootComponent.ConfigManager;
        _variantsManager = htmlRootComponent.VariantsManager;
        
        Dictionary<int, List<TimetableLessonData>> lessonTimetableLessonDatas = timetable.TimetableLessonDatas
            .Where(IsTimetableLessonRelevant)
            .GroupBy(tld => tld.Lesson)
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        foreach (KeyValuePair<int, List<TimetableLessonData>> pair in lessonTimetableLessonDatas)
        {
            PeriodHtmlComponent periodHtmlComponent = new(pair.Value, pair.Key);
            AddChildToComponent(periodHtmlComponent);
        }
    }

    private bool IsTimetableLessonRelevant(TimetableLessonData timetableLessonData)
    {
        if (timetableLessonData.Day != DateTime.DayOfWeek)
        {
            return false;
        }

        bool showWorkshops = _configManager.GetConfigValue(ConfigIdentifier.ShowWorkshops) == "true";
        if (timetableLessonData.DaltonType == DaltonType.Workshop && !showWorkshops)
        {
            return false;
        }
        
        string currentVariantIdentifier = _variantsManager.GetVariantsWeekIdentifier(DateTime);
        return timetableLessonData.VariantIdentifier == string.Empty ||
               currentVariantIdentifier == string.Empty ||
               currentVariantIdentifier == timetableLessonData.VariantIdentifier;
    }
    
    public override string GenerateHtml()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        int floorCount = Convert.ToInt32(htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.FloorCount));
        
        string dataDateTimeString = DateTime.ToString("yyyyMMdd");
        string htmlHead = $"<div class=\"day\" data-date={dataDateTimeString} style=\"--floor-count: {floorCount + ExtraColumns};\">";
        
        string dateTimeString = DateTime.ToLongDateString();
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
}