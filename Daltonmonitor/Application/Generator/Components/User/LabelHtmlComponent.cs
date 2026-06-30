using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public class LabelHtmlComponent(string labelString) : HtmlComponent
{
    protected override void Initialize() { }

    public override string GenerateHtml()
    {
        string html = $"<div class=\"tag {labelString}\">{labelString}</div>";
        return html;
    }
}