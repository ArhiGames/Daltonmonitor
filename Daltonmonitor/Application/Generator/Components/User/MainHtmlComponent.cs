using System;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class MainHtmlComponent : HtmlComponent
{
    private readonly Timetable _timetable;
    
    public MainHtmlComponent(Timetable timetable)
    {
        _timetable = timetable;
        
        BuildChildrenData();
    }

    private void BuildChildrenData()
    {
        DateTime today = DateTime.UtcNow;

        DayHtmlComponent dayHtmlComponent = new(_timetable, today);
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