using System;
using System.IO;
using System.Text;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HeaderHtmlComponent : HtmlComponent
{
    private readonly DateTime _generatedDate = DateTime.Now;
    
    protected override void Initialize() { }
        
    public override string GenerateHtml()
    {
        string svgLogoString = GetSvgLogoString();
        int classStartIndex = svgLogoString.IndexOf("<svg", StringComparison.Ordinal);
        svgLogoString = svgLogoString.Insert(classStartIndex + 5, "class=\"logo\"");
        string logoHtml = $"{svgLogoString}";

        string lastUpdateHtml = $"<div class=\"last-update\"><div class=\"label\">Stand:</div><div class=\"date\">{_generatedDate.ToLongDateString()}</div><div class=\"time\">{_generatedDate.Hour}:{_generatedDate.Minute}</div></div>";
        
        StringBuilder stringBuilder = new();
        stringBuilder.Append("<header>");
        stringBuilder.Append(logoHtml);
        stringBuilder.Append("<h1>Dalton</h1>");
        stringBuilder.Append(lastUpdateHtml);
        stringBuilder.Append("</header>");

        return stringBuilder.ToString();
    }

    private string GetSvgLogoString()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        string svgLogoPath = htmlRootComponent.ConfigManager.GetConfigValue(ConfigIdentifier.LogoSvgSourcePath);

        string fileContent = File.ReadAllText(svgLogoPath);
        int startingIndex = fileContent.IndexOf("<svg", StringComparison.InvariantCulture);
        int endIndex = fileContent.IndexOf("</svg>", StringComparison.InvariantCulture) + 6;

        string svgLogoString = fileContent.Substring(startingIndex, endIndex - startingIndex);
        return svgLogoString;
    }
}