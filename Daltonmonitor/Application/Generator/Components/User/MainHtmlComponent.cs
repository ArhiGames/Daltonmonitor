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

        int previewingDays = Convert.ToInt32(htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.PreviewingDaysCount));
        int maxFutureDaysAfterOffDays =
            Convert.ToInt32(htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.MaxFutureDaysAfterOffDay));
        bool showVacationDays = htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.ShowVacationDays) == "true";
        int outstandingDays = previewingDays;
        int currentShowingDaysAfterOffDays = 0;

        DateTime currentDate = DateTime.Now;
        while (outstandingDays > 0)
        {
            VacationData? vacationData = timetable.GetVacationData(currentDate);

            bool isWeekend = currentDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
            bool showVacation = vacationData is null || showVacationDays;
            bool showDay = !isWeekend && showVacation; 
            
            if (showDay)
            {
                DayHtmlComponent dayHtmlComponent = new(timetable, currentDate, vacationData);
                AddChildToComponent(dayHtmlComponent);
                outstandingDays--;
                if (vacationData is null)
                {
                    currentShowingDaysAfterOffDays = 0;
                }
                else
                {
                    currentShowingDaysAfterOffDays++;
                }
            }
            else
            {
                currentShowingDaysAfterOffDays++;
            }

            if (currentShowingDaysAfterOffDays >= maxFutureDaysAfterOffDays && maxFutureDaysAfterOffDays > 0)
            {
                return;
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