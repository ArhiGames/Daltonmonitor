using System;
using System.IO;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Types.Common.Errors;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HeaderHtmlComponent : HtmlComponent
{
    private readonly DateTime _generatedDate = DateTime.Now;
    
    protected override void Initialize() { }
        
    public override string GenerateHtml()
    {
        Result<string> svgLogoStringResult = GetSvgLogoString();
        if (!svgLogoStringResult.IsSuccess)
        {
            return "";
        }

        string svgLogoString = svgLogoStringResult.Value!;
        
        int classStartIndex = svgLogoString.IndexOf("<svg", StringComparison.Ordinal);
        svgLogoString = svgLogoString.Insert(classStartIndex + 5, "class=\"logo\"");
        string logoHtml = $"{svgLogoString}";

        string dateString = $"{_generatedDate.ToLongDateString()}";
        string timeString = $"{_generatedDate.ToShortTimeString()}";
        string lastUpdateHtml = $"<div class=\"last-update\"><div class=\"label\">Stand:</div><div class=\"date\">{dateString}</div><div class=\"time\">{timeString}</div></div>";
        
        StringBuilder stringBuilder = new();
        stringBuilder.Append("<header>");
        stringBuilder.Append(logoHtml);
        stringBuilder.Append("<h1>Dalton</h1>");
        stringBuilder.Append(lastUpdateHtml);
        stringBuilder.Append("</header>");

        return stringBuilder.ToString();
    }

    private Result<string> GetSvgLogoString()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        string svgLogoPath = htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.LogoSvgSourcePath);

        try
        {
            string svgLines = File.ReadAllText(svgLogoPath);
            int startingIndex = svgLines.IndexOf("<svg", StringComparison.Ordinal);
            int endIndex = svgLines.IndexOf("</svg>", StringComparison.Ordinal) + 6;

            string svgLogoString = svgLines.Substring(startingIndex, endIndex - startingIndex);
            return svgLogoString;
        }
        catch
        {
            return Errors.FileError;
        }
    }
}