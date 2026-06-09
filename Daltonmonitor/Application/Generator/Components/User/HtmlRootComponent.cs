using System.IO;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HtmlRootComponent : HtmlComponent
{
    public ConfigManager ConfigManager { get; }
    private readonly Timetable _timetable;

    public HtmlRootComponent(ConfigManager configManager, Timetable timetable)
    {
        ConfigManager = configManager;
        _timetable = timetable;
        
        SetAsRootComponent();
    }
    
    protected override void Initialize()
    {
        HeaderHtmlComponent headerHtmlComponent = new();
        AddChildToComponent(headerHtmlComponent);
        
        MainHtmlComponent mainHtmlComponent = new(_timetable);
        AddChildToComponent(mainHtmlComponent);
    }
    
    public override string GenerateHtml()
    {
        string applicationName = ConfigManager.GetConfigValue(ConfigIdentifier.ApplicationName);
        string htmlHead = $"<!DOCTYPE html><html lang=\"de\"><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" charset=\"UTF-8\"><title>{applicationName}</title><link rel=\"icon\" type=\"image/x-icon\" href=\"Icon.png\"></head>";
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
    
    private string GetCssString()
    {
        string cssPath = ConfigManager.GetConfigValue(ConfigIdentifier.StyleSourcePath);
        try
        {
            string css = File.ReadAllText(cssPath);
            return css.Replace("\n", "").Replace("    ", "");
        }
        catch
        {
            return "";
        }
    }
}