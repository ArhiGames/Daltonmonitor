using System.Collections.Generic;
using System.IO;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Application.Managers.Variants;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HtmlRootComponent : HtmlComponent
{
    public ConfigManager ConfigManager { get; }
    public VariantsManager VariantsManager { get; }
    private readonly Timetable _timetable;

    public HtmlRootComponent(ConfigManager configManager, VariantsManager variantsManager, Timetable timetable)
    {
        ConfigManager = configManager;
        VariantsManager = variantsManager;
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
        string htmlHead = $"<!DOCTYPE html><html lang=\"de\"><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" charset=\"UTF-8\"><title>{applicationName}</title></head>";
        string htmlStyle = $"<style>{GetCssString()}</style>";
        string htmlScript = $"<script>{GetJsString()}</script>";
        const string htmlBack = "</body></html>";

        StringBuilder stringBuilder = new();
        stringBuilder.Append(htmlHead);
        stringBuilder.Append(htmlStyle);
        stringBuilder.Append(htmlScript);
        stringBuilder.Append("<body>");
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);

        return stringBuilder.ToString();
    }

    private string GetJsString()
    {
        string jsPath = ConfigManager.GetConfigValue(ConfigIdentifier.ScriptSourcePath);
        try
        {
            string js = File.ReadAllText(jsPath);
            return js;
        }
        catch
        {
            return "";
        }
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
                { "--text-color", ConfigIdentifier.TextColor },
                { "--text-color-changed", ConfigIdentifier.TextColorSubstitution },
                { "--lesson-text-size", ConfigIdentifier.LessonTextSize }
            };

            Dictionary<string, ConfigIdentifier> borderColorIdentifiers = new()
            {
                { "--border", ConfigIdentifier.BorderColor },
                { "--border-date", ConfigIdentifier.BorderColorDate },
                { "--border-off", ConfigIdentifier.BorderColorOffDay },
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
                    continue;
                }

                found = borderColorIdentifiers.TryGetValue(keyString, out configIdentifier);
                if (found)
                {
                    string borderSize = ConfigManager.GetConfigValue(ConfigIdentifier.BorderSize);
                    string configValue = ConfigManager.GetConfigValue(configIdentifier);
                    css[i] = $"{keyString}:solid {configValue} {borderSize};";
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