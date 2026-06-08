using System.IO;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HtmlRootComponent : HtmlComponent
{
    private readonly ConfigManager _configManager;
    private readonly Timetable _timetable;

    public HtmlRootComponent(ConfigManager configManager, Timetable timetable)
    {
        _configManager = configManager;
        _timetable = timetable;
        
        SetAsRootComponent();
    }
    
    public override string GenerateHtml()
    {
        string applicationName = _configManager.GetConfigValue(ConfigType.ApplicationName);
        string htmlHead = $"<!DOCTYPE html><html lang=\"de\"><head><meta charset=\"UTF-8\"><title>{applicationName}</title><link rel=\"icon\" type=\"image/x-icon\" href=\"Icon.png\"></head>";
        string htmlStyle = $"<style>{GetCssString()}</style>";
        const string htmlBack = "</body></html>";

        StringBuilder stringBuilder = new();
        stringBuilder.Append(htmlHead);
        stringBuilder.Append(htmlStyle);
        stringBuilder.Append("<body>");
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);

        return stringBuilder.ToString();
    }

    protected override void Initialize()
    {
        HeaderHtmlComponent headerHtmlComponent = new();
        AddChildrenToComponent(headerHtmlComponent);
        
        MainHtmlComponent mainHtmlComponent = new(_timetable);
        AddChildrenToComponent(mainHtmlComponent);
    }
    
    private string GetCssString()
    {
        string cssPath = _configManager.GetConfigValue(ConfigType.StyleSourcePath);
        string css = File.ReadAllText(cssPath);
        return css.Replace('\n', ' ').Replace("    ", "");
    }
}