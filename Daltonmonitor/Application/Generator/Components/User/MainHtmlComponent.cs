using System;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class MainHtmlComponent(Timetable timetable) : HtmlComponent
{
    protected override void Initialize()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;

        string previewingDaysString =
            htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.PreviewingDaysCount);
        int previewingDays = Convert.ToInt32(previewingDaysString);
        int outstandingDays = previewingDays;

        DateTime currentDate = DateTime.Now;
        while (outstandingDays > 0)
        {
            if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                DayHtmlComponent dayHtmlComponent = new(timetable, currentDate);
                AddChildToComponent(dayHtmlComponent);
                outstandingDays--;
            }
            
            currentDate = currentDate.AddDays(1);
        }
    }
    
    public override string GenerateHtml()
    {
        const string htmlHead = "<main>";
        const string htmlBack = "</main>";

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