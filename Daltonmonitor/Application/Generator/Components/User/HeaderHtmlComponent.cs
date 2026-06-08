using System;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HeaderHtmlComponent : HtmlComponent
{
    private readonly DateTime _generatedDate = DateTime.Now;
    
    protected override void Initialize() { }
        
    public override string GenerateHtml()
    {
        const string logoHtml = "<img src=\"Icon.png\" alt=\"Logo\">";

        string lastUpdateHtml = $"<div class=\"last-update\"><div class=\"label\">Stand:</div><div class=\"date\">{_generatedDate.ToLongDateString()}</div><div class=\"time\">{_generatedDate.Hour}:{_generatedDate.Minute}</div></div>";
        
        StringBuilder stringBuilder = new();
        stringBuilder.Append("<header>");
        stringBuilder.Append(logoHtml);
        stringBuilder.Append("<h1>Dalton</h1>");
        stringBuilder.Append(lastUpdateHtml);
        stringBuilder.Append("</header>");

        return stringBuilder.ToString();
    }
}