using System;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public enum LabelType
{
    Special = 0,
    Workshop,
    Bound,
    Mentor,
}

public class LabelHtmlComponent(LabelType labelType) : HtmlComponent
{
    protected override void Initialize() { }

    public override string GenerateHtml()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        ConfigIdentifier configIdentifier = labelType switch
        {
            LabelType.Special => ConfigIdentifier.SpecialLabel,
            LabelType.Workshop => ConfigIdentifier.WorkshopLabel,
            LabelType.Bound => ConfigIdentifier.BoundDaltonLabel,
            LabelType.Mentor => ConfigIdentifier.MentorLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(labelType), labelType, null)
        };
        string labelTypeString = labelType.ToString();
        string labelString = htmlRootComponent.ConfigManager.GetConfigValue(configIdentifier);
        if (labelString == string.Empty)
        {
            return "";
        }

        string html = $"<div class=\"tag {labelTypeString}\">{labelString}</div>";
        return html;
    }
}