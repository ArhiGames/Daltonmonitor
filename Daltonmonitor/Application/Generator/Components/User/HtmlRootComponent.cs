using System.Collections.Generic;
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
            Dictionary<string, ConfigIdentifier> identifiers = new()
            {
                { "--background-color", ConfigIdentifier.BackgroundColor },
                { "--element-color", ConfigIdentifier.ElementColor },
                { "--tag-color", ConfigIdentifier.TagColor },
                { "--date-color", ConfigIdentifier.DateColor },
                { "--off-day-color", ConfigIdentifier.OffDayColor },
                { "--text-color", ConfigIdentifier.TextColor }
            };
            
            string[] css = File.ReadAllLines(cssPath);
            for (int i = 0; i < css.Length; i++)
            {
                string cssString = css[i].Trim();
                
                int index = cssString.IndexOf(':');
                if (index == -1)
                {
                    continue;
                }

                string keyString = cssString[..index];
                
                bool found = identifiers.TryGetValue(keyString, out ConfigIdentifier configIdentifier);
                if (found)
                {
                    string configValue = ConfigManager.GetConfigValue(configIdentifier);
                    css[i] = $"{keyString}:{configValue};";
                }
            }

            StringBuilder finalCss = new();
            foreach (string line in css)
            {
                finalCss.Append(line.Trim());
            }

            return finalCss.ToString();
        }
        catch
        {
            return "";
        }
    }
}