using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Application.Generator.Components.User;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator;

public class HtmlGenerator
{
    private HtmlRootComponent? _rootComponent;

    public string GenerateHtmlStructure(Timetable timetable)
    {
        _rootComponent = new HtmlRootComponent(timetable);
        return _rootComponent.GenerateHtml();
    }
}