using System;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class MainHtmlComponent(Timetable timetable) : HtmlComponent
{
    protected override void Initialize()
    {
        DateTime today = DateTime.UtcNow;

        DateTime day2 = new(2026, 5, 25);
        DayHtmlComponent day2HtmlComponent = new(timetable, day2);
        AddChildrenToComponent(day2HtmlComponent);
        
        DateTime day = new(2026, 5, 26);
        DayHtmlComponent dayHtmlComponent = new(timetable, day);
        AddChildrenToComponent(dayHtmlComponent);
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