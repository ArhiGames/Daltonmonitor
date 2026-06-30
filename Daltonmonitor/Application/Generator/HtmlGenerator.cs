using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.User;
using Daltonmonitor.Application.Managers.Variants;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator;

public class HtmlGenerator(ConfigManager configManager, VariantsManager variantsManager)
{
    private HtmlRootComponent? _rootComponent;

    public string GenerateHtmlStructure(Timetable timetable)
    {
        _rootComponent = new HtmlRootComponent(configManager, variantsManager, timetable);
        return _rootComponent.GenerateHtml();
    }
}