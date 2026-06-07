using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.User;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator;

public class HtmlGenerator(ConfigManager configManager)
{
    private HtmlRootComponent? _rootComponent;

    public string GenerateHtmlStructure(Timetable timetable)
    {
        _rootComponent = new HtmlRootComponent(configManager, timetable);
        return _rootComponent.GenerateHtml();
    }
}